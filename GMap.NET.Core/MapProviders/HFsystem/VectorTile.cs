using System;
using System.Collections.Generic;
using System.Text;

namespace GMap.NET.MapProviders.HFsystem
{
    /// <summary>
    ///     Mapbox Vector Tile (.pbf / .mvt) dekódoló, külső függőség nélkül.
    ///     Specifikáció: https://github.com/mapbox/vector-tile-spec/tree/master/2.1
    /// </summary>
    public sealed class VectorTile
    {
        public readonly List<VectorTileLayer> Layers = new List<VectorTileLayer>();

        public VectorTileLayer GetLayer(string name)
        {
            foreach (var l in Layers)
            {
                if (l.Name == name)
                    return l;
            }

            return null;
        }

        public static VectorTile Parse(byte[] data)
        {
            var tile = new VectorTile();
            var r = new PbfReader(data, 0, data.Length);

            while (r.Next(out int field, out int wire))
            {
                if (field == 3 && wire == 2)
                {
                    r.ReadLengthDelimited(out int start, out int len);
                    var layer = VectorTileLayer.Parse(data, start, len);

                    // azonos nevű rétegek összevonása (a GetLayer így mindent visszaad)
                    var existing = tile.GetLayer(layer.Name);
                    if (existing != null && existing.Extent == layer.Extent)
                        existing.Features.AddRange(layer.Features);
                    else
                        tile.Layers.Add(layer);
                }
                else
                {
                    r.Skip(wire);
                }
            }

            return tile;
        }
    }

    public sealed class VectorTileLayer
    {
        public string Name = string.Empty;
        public int Extent = 4096;
        public readonly List<VectorTileFeature> Features = new List<VectorTileFeature>();

        internal static VectorTileLayer Parse(byte[] data, int offset, int length)
        {
            var layer = new VectorTileLayer();
            var keys = new List<string>();
            var values = new List<object>();
            var rawFeatures = new List<KeyValuePair<int, int>>();

            var r = new PbfReader(data, offset, length);
            while (r.Next(out int field, out int wire))
            {
                switch (field)
                {
                    case 1: // name
                        layer.Name = r.ReadString();
                        break;
                    case 2: // features - a kulcsok/értékek a feature-ök után is jöhetnek, ezért később dolgozzuk fel
                        r.ReadLengthDelimited(out int fs, out int fl);
                        rawFeatures.Add(new KeyValuePair<int, int>(fs, fl));
                        break;
                    case 3: // keys
                        keys.Add(r.ReadString());
                        break;
                    case 4: // values
                        r.ReadLengthDelimited(out int vs, out int vl);
                        values.Add(ParseValue(data, vs, vl));
                        break;
                    case 5: // extent
                        layer.Extent = (int)r.ReadVarint();
                        break;
                    default:
                        r.Skip(wire);
                        break;
                }
            }

            foreach (var f in rawFeatures)
            {
                layer.Features.Add(VectorTileFeature.Parse(data, f.Key, f.Value, keys, values));
            }

            return layer;
        }

        static object ParseValue(byte[] data, int offset, int length)
        {
            var r = new PbfReader(data, offset, length);
            object ret = null;

            while (r.Next(out int field, out int wire))
            {
                switch (field)
                {
                    case 1: ret = r.ReadString(); break;
                    case 2: ret = (double)r.ReadFloat(); break;
                    case 3: ret = r.ReadDouble(); break;
                    case 4: ret = (long)r.ReadVarint(); break;
                    case 5: ret = (long)r.ReadVarint(); break;
                    case 6: ret = PbfReader.ZigZag(r.ReadVarint()); break;
                    case 7: ret = r.ReadVarint() != 0; break;
                    default: r.Skip(wire); break;
                }
            }

            return ret;
        }
    }

    public enum VectorGeometryType
    {
        Unknown = 0,
        Point = 1,
        LineString = 2,
        Polygon = 3
    }

    public struct TilePoint
    {
        public int X;
        public int Y;

        public TilePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public sealed class VectorTileFeature
    {
        public VectorGeometryType Type;
        public readonly Dictionary<string, object> Tags = new Dictionary<string, object>();

        /// <summary>
        ///     Geometria a csempe koordinátarendszerében (0..Extent).
        ///     Pontnál minden lista egy pont, vonalnál egy-egy vonalszakasz, poligonnál egy-egy gyűrű.
        /// </summary>
        public readonly List<List<TilePoint>> Parts = new List<List<TilePoint>>();

        public string GetString(string key)
        {
            return Tags.TryGetValue(key, out var v) && v != null ? v.ToString() : null;
        }

        public long GetLong(string key, long def = 0)
        {
            if (Tags.TryGetValue(key, out var v) && v != null)
            {
                if (v is long l) return l;
                if (v is double d) return (long)d;
                if (v is bool b) return b ? 1 : 0;
                if (long.TryParse(v.ToString(), out l)) return l;
            }

            return def;
        }

        internal static VectorTileFeature Parse(byte[] data, int offset, int length, List<string> keys, List<object> values)
        {
            var feature = new VectorTileFeature();
            var tags = new List<uint>();
            var geometry = new List<uint>();

            var r = new PbfReader(data, offset, length);
            while (r.Next(out int field, out int wire))
            {
                switch (field)
                {
                    case 2: r.ReadPackedOrSingle(wire, tags); break;
                    case 3: feature.Type = (VectorGeometryType)r.ReadVarint(); break;
                    case 4: r.ReadPackedOrSingle(wire, geometry); break;
                    default: r.Skip(wire); break;
                }
            }

            for (int i = 0; i + 1 < tags.Count; i += 2)
            {
                int k = (int)tags[i];
                int v = (int)tags[i + 1];
                if (k < keys.Count && v < values.Count)
                {
                    feature.Tags[keys[k]] = values[v];
                }
            }

            DecodeGeometry(geometry, feature);
            return feature;
        }

        static void DecodeGeometry(List<uint> geom, VectorTileFeature feature)
        {
            int x = 0, y = 0;
            int i = 0;
            List<TilePoint> current = null;

            while (i < geom.Count)
            {
                uint cmdInt = geom[i++];
                int cmd = (int)(cmdInt & 0x7);
                int count = (int)(cmdInt >> 3);

                if (cmd == 1 || cmd == 2) // MoveTo / LineTo
                {
                    for (int n = 0; n < count && i + 1 < geom.Count; n++)
                    {
                        x += (int)PbfReader.ZigZag(geom[i++]);
                        y += (int)PbfReader.ZigZag(geom[i++]);

                        if (cmd == 1 && (current == null || current.Count > 0))
                        {
                            current = new List<TilePoint>();
                            feature.Parts.Add(current);
                        }

                        current.Add(new TilePoint(x, y));
                    }
                }
                else if (cmd == 7) // ClosePath
                {
                    if (current != null && current.Count > 0)
                    {
                        current.Add(current[0]);
                    }
                }
                else
                {
                    break; // ismeretlen parancs: sérült geometria
                }
            }
        }
    }

    /// <summary>
    ///     Minimális protobuf olvasó (varint, fixed32, fixed64, length-delimited).
    /// </summary>
    internal struct PbfReader
    {
        readonly byte[] _buf;
        int _pos;
        readonly int _end;

        public PbfReader(byte[] buf, int offset, int length)
        {
            _buf = buf;
            _pos = offset;
            _end = offset + length;
        }

        public bool Next(out int field, out int wireType)
        {
            if (_pos >= _end)
            {
                field = 0;
                wireType = 0;
                return false;
            }

            ulong key = ReadVarint();
            field = (int)(key >> 3);
            wireType = (int)(key & 0x7);
            return true;
        }

        public ulong ReadVarint()
        {
            ulong result = 0;
            int shift = 0;

            while (_pos < _end)
            {
                byte b = _buf[_pos++];
                result |= (ulong)(b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                    return result;
                shift += 7;
                if (shift > 63)
                    throw new FormatException("Hibás varint a .pbf adatban");
            }

            throw new FormatException("Váratlan adatvég a .pbf adatban");
        }

        public static long ZigZag(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        public void ReadLengthDelimited(out int start, out int length)
        {
            length = (int)ReadVarint();
            start = _pos;
            if (length < 0 || _pos + length > _end)
                throw new FormatException("Hibás hossz a .pbf adatban");
            _pos += length;
        }

        public string ReadString()
        {
            ReadLengthDelimited(out int start, out int len);
            return Encoding.UTF8.GetString(_buf, start, len);
        }

        public float ReadFloat()
        {
            CheckAvailable(4);
            float v = BitConverter.ToSingle(_buf, _pos);
            _pos += 4;
            return v;
        }

        public double ReadDouble()
        {
            CheckAvailable(8);
            double v = BitConverter.ToDouble(_buf, _pos);
            _pos += 8;
            return v;
        }

        public void ReadPackedOrSingle(int wireType, List<uint> target)
        {
            if (wireType == 2)
            {
                ReadLengthDelimited(out int start, out int len);
                var sub = new PbfReader(_buf, start, len);
                while (sub._pos < sub._end)
                {
                    target.Add((uint)sub.ReadVarint());
                }
            }
            else
            {
                target.Add((uint)ReadVarint());
            }
        }

        public void Skip(int wireType)
        {
            switch (wireType)
            {
                case 0: ReadVarint(); break;
                case 1: CheckAvailable(8); _pos += 8; break;
                case 2: ReadLengthDelimited(out _, out _); break;
                case 5: CheckAvailable(4); _pos += 4; break;
                default: throw new FormatException("Ismeretlen protobuf wire type: " + wireType);
            }
        }

        void CheckAvailable(int n)
        {
            if (_pos + n > _end)
                throw new FormatException("Váratlan adatvég a .pbf adatban");
        }
    }
}
