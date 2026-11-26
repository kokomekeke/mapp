using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace GMap.NET.MapProviders.HFsystem
{
    /// <summary>
    ///     OpenMapTiles sémájú (/data/v3) vektoros csempe kirajzolása raszteres (PNG) képpé,
    ///     egyszerű, beépített OSM-szerű stílussal.
    ///     Rétegek: https://openmaptiles.org/schema/
    /// </summary>
    public static class VectorTileRenderer
    {
        public const int TileSize = 256;

        static readonly Color Background = Color.FromArgb(242, 239, 233);
        static readonly Color WaterColor = Color.FromArgb(170, 211, 223);
        static readonly Color BuildingColor = Color.FromArgb(217, 208, 201);
        static readonly Color BuildingOutline = Color.FromArgb(196, 182, 171);
        static readonly Color BoundaryColor = Color.FromArgb(160, 120, 170);
        static readonly Color LabelColor = Color.FromArgb(40, 40, 40);

        /// <summary>
        ///     .pbf bájtokból PNG képet készít.
        /// </summary>
        /// <param name="pbf">a (már kitömörített) vektoros csempe; üres is lehet</param>
        /// <param name="zoom">a megjelenített zoomszint (a stílushoz)</param>
        /// <param name="overzoom">hány szinttel nagyítunk a forrás csempéhez képest (0 = nincs nagyítás)</param>
        /// <param name="subX">a forrás csempén belüli rész X indexe (0 .. 2^overzoom-1)</param>
        /// <param name="subY">a forrás csempén belüli rész Y indexe (0 .. 2^overzoom-1)</param>
        /// <param name="layers">a kirajzolandó rétegek</param>
        public static byte[] RenderToPng(byte[] pbf, int zoom, int overzoom = 0, long subX = 0, long subY = 0,
            HFsystemLayers layers = HFsystemLayers.All)
        {
            var tile = pbf != null && pbf.Length > 0 ? VectorTile.Parse(pbf) : new VectorTile();

            using (var bmp = new Bitmap(TileSize, TileSize, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    g.Clear(Background);

                    var ctx = new RenderContext(g, zoom, overzoom, subX, subY);
                    bool On(HFsystemLayers l) => (layers & l) != 0;

                    // területek (alulról felfelé)
                    if (On(HFsystemLayers.Vegetation))
                        DrawFills(ctx, tile.GetLayer("landcover"), LandcoverColor);
                    if (On(HFsystemLayers.Landuse))
                    {
                        DrawFills(ctx, tile.GetLayer("landuse"), LanduseColor);
                        DrawFills(ctx, tile.GetLayer("aeroway"), f => Color.FromArgb(220, 220, 228));
                    }
                    if (On(HFsystemLayers.Vegetation))
                        DrawFills(ctx, tile.GetLayer("park"), f => Color.FromArgb(200, 230, 180));
                    if (On(HFsystemLayers.Water))
                    {
                        DrawFills(ctx, tile.GetLayer("water"), f => WaterColor);
                        DrawWaterways(ctx, tile.GetLayer("waterway"));
                    }
                    if (On(HFsystemLayers.Buildings))
                        DrawBuildings(ctx, tile.GetLayer("building"));

                    // vonalak
                    DrawTransportation(ctx, tile.GetLayer("transportation"),
                        On(HFsystemLayers.Roads), On(HFsystemLayers.Paths), On(HFsystemLayers.Railways));
                    if (On(HFsystemLayers.Boundaries))
                        DrawBoundaries(ctx, tile.GetLayer("boundary"));

                    // feliratok (legfelül), fontossági sorrendben: amelyik később jönne
                    // és ütközne egy már kirakottal, az kimarad
                    if (On(HFsystemLayers.PlaceNames))
                        DrawPlaceLabels(ctx, tile.GetLayer("place"));
                    if (On(HFsystemLayers.StreetNames))
                        DrawStreetNames(ctx, tile.GetLayer("transportation_name"));
                    if (On(HFsystemLayers.WaterNames))
                        DrawWaterNames(ctx, tile.GetLayer("water_name"), tile.GetLayer("waterway"));
                    if (On(HFsystemLayers.HouseNumbers))
                        DrawHouseNumbers(ctx, tile.GetLayer("housenumber"));
                }

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        sealed class RenderContext
        {
            public readonly Graphics G;
            public readonly int Zoom;

            /// <summary>
            ///     a már kirakott feliratok befoglaló téglalapjai (ütközésvizsgálathoz)
            /// </summary>
            public readonly List<RectangleF> Occupied = new List<RectangleF>();

            public bool TryOccupy(RectangleF r)
            {
                foreach (var o in Occupied)
                {
                    if (o.IntersectsWith(r))
                        return false;
                }

                Occupied.Add(r);
                return true;
            }
            readonly double _factor;
            readonly double _offX;
            readonly double _offY;

            public RenderContext(Graphics g, int zoom, int overzoom, long subX, long subY)
            {
                G = g;
                Zoom = zoom;
                _factor = TileSize * (double)(1L << overzoom);
                _offX = subX * (double)TileSize;
                _offY = subY * (double)TileSize;
            }

            public PointF[] ToPixels(List<TilePoint> part, int extent)
            {
                var pts = new PointF[part.Count];
                double s = _factor / extent;
                for (int i = 0; i < part.Count; i++)
                {
                    pts[i] = new PointF((float)(part[i].X * s - _offX), (float)(part[i].Y * s - _offY));
                }

                return pts;
            }

            /// <summary>
            ///     vonalvastagság-szorzó a zoom függvényében
            /// </summary>
            public float WidthScale
            {
                get { return (float)Math.Max(0.35, Math.Min(6.0, Math.Pow(1.45, Zoom - 12))); }
            }
        }

        #region színek

        static Color? LandcoverColor(VectorTileFeature f)
        {
            switch (f.GetString("class"))
            {
                case "wood": return Color.FromArgb(173, 209, 158);
                case "grass": return Color.FromArgb(205, 235, 176);
                case "farmland": return Color.FromArgb(238, 240, 213);
                case "wetland": return Color.FromArgb(214, 230, 222);
                case "sand": return Color.FromArgb(245, 233, 198);
                case "rock": return Color.FromArgb(225, 220, 215);
                case "ice": return Color.FromArgb(240, 248, 255);
                default: return null;
            }
        }

        static Color? LanduseColor(VectorTileFeature f)
        {
            switch (f.GetString("class"))
            {
                case "residential":
                case "suburb":
                case "neighbourhood": return Color.FromArgb(232, 228, 224);
                case "commercial":
                case "retail": return Color.FromArgb(242, 218, 217);
                case "industrial":
                case "railway": return Color.FromArgb(235, 219, 232);
                case "cemetery": return Color.FromArgb(170, 203, 175);
                case "pitch":
                case "playground":
                case "stadium": return Color.FromArgb(170, 224, 203);
                case "hospital":
                case "school":
                case "college":
                case "university": return Color.FromArgb(255, 255, 229);
                case "military": return Color.FromArgb(240, 220, 220);
                default: return null;
            }
        }

        static bool TryGetRoadStyle(string cls, out Color fill, out Color casing, out float width)
        {
            switch (cls)
            {
                case "motorway": fill = Color.FromArgb(232, 146, 162); casing = Color.FromArgb(220, 42, 103); width = 4.0f; return true;
                case "trunk": fill = Color.FromArgb(249, 178, 156); casing = Color.FromArgb(200, 90, 60); width = 3.6f; return true;
                case "primary": fill = Color.FromArgb(252, 214, 164); casing = Color.FromArgb(160, 110, 40); width = 3.2f; return true;
                case "secondary": fill = Color.FromArgb(247, 250, 191); casing = Color.FromArgb(120, 125, 60); width = 2.8f; return true;
                case "tertiary": fill = Color.White; casing = Color.FromArgb(160, 160, 160); width = 2.4f; return true;
                case "minor": fill = Color.White; casing = Color.FromArgb(180, 180, 180); width = 1.8f; return true;
                case "service": fill = Color.White; casing = Color.FromArgb(190, 190, 190); width = 1.2f; return true;
                default: fill = casing = Color.Empty; width = 0; return false;
            }
        }

        #endregion

        static void DrawFills(RenderContext ctx, VectorTileLayer layer, Func<VectorTileFeature, Color?> colorOf)
        {
            if (layer == null)
                return;

            foreach (var f in layer.Features)
            {
                if (f.Type != VectorGeometryType.Polygon)
                    continue;

                var c = colorOf(f);
                if (c == null)
                    continue;

                using (var path = BuildPolygonPath(ctx, f, layer.Extent))
                using (var brush = new SolidBrush(c.Value))
                {
                    ctx.G.FillPath(brush, path);
                }
            }
        }

        static GraphicsPath BuildPolygonPath(RenderContext ctx, VectorTileFeature f, int extent)
        {
            // Alternate kitöltés: a lyukak (belső gyűrűk) üresen maradnak
            var path = new GraphicsPath(FillMode.Alternate);
            foreach (var ring in f.Parts)
            {
                if (ring.Count >= 3)
                {
                    path.AddPolygon(ctx.ToPixels(ring, extent));
                }
            }

            return path;
        }

        static void DrawWaterways(RenderContext ctx, VectorTileLayer layer)
        {
            if (layer == null)
                return;

            foreach (var f in layer.Features)
            {
                if (f.Type != VectorGeometryType.LineString)
                    continue;

                float w = (f.GetString("class") == "river" ? 2.2f : 1.0f) * ctx.WidthScale;
                using (var pen = new Pen(WaterColor, w) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                {
                    DrawLines(ctx, f, layer.Extent, pen);
                }
            }
        }

        static void DrawBuildings(RenderContext ctx, VectorTileLayer layer)
        {
            if (layer == null || ctx.Zoom < 13)
                return;

            using (var brush = new SolidBrush(BuildingColor))
            using (var pen = new Pen(BuildingOutline, 0.7f))
            {
                foreach (var f in layer.Features)
                {
                    if (f.Type != VectorGeometryType.Polygon)
                        continue;

                    using (var path = BuildPolygonPath(ctx, f, layer.Extent))
                    {
                        ctx.G.FillPath(brush, path);
                        if (ctx.Zoom >= 15)
                            ctx.G.DrawPath(pen, path);
                    }
                }
            }
        }

        static void DrawTransportation(RenderContext ctx, VectorTileLayer layer, bool roadsOn, bool pathsOn, bool railOn)
        {
            if (layer == null || !(roadsOn || pathsOn || railOn))
                return;

            var roads = new List<KeyValuePair<int, VectorTileFeature>>();

            foreach (var f in layer.Features)
            {
                if (f.Type != VectorGeometryType.LineString)
                    continue;

                string cls = f.GetString("class");
                switch (cls)
                {
                    case "rail":
                    case "transit":
                        if (!railOn)
                            break;
                        using (var pen = new Pen(Color.FromArgb(140, 140, 140), 1.2f * ctx.WidthScale) { DashStyle = DashStyle.Dash })
                        {
                            DrawLines(ctx, f, layer.Extent, pen);
                        }
                        break;
                    case "track":
                    case "path":
                        if (!pathsOn)
                            break;
                        using (var pen = new Pen(Color.FromArgb(170, 120, 80), 0.8f * ctx.WidthScale) { DashStyle = DashStyle.Dot })
                        {
                            DrawLines(ctx, f, layer.Extent, pen);
                        }
                        break;
                    default:
                        if (roadsOn && TryGetRoadStyle(cls, out _, out _, out float wid))
                        {
                            // kisebb utak előbb, a fontosabbak felülre kerülnek
                            roads.Add(new KeyValuePair<int, VectorTileFeature>((int)(wid * 10), f));
                        }
                        break;
                }
            }

            roads.Sort((a, b) => a.Key.CompareTo(b.Key));

            // először az összes szegély, aztán a kitöltés, így a kereszteződések szépen összeolvadnak
            for (int pass = 0; pass < 2; pass++)
            {
                foreach (var kv in roads)
                {
                    var f = kv.Value;
                    TryGetRoadStyle(f.GetString("class"), out Color fill, out Color casing, out float w);
                    bool tunnel = f.GetString("brunnel") == "tunnel";

                    float width = w * ctx.WidthScale;
                    Color color = pass == 0 ? casing : fill;
                    if (pass == 0)
                        width += Math.Max(1.0f, width * 0.35f);
                    if (tunnel)
                        color = Color.FromArgb(120, color);

                    using (var pen = new Pen(color, width) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round })
                    {
                        if (tunnel && pass == 0)
                            pen.DashStyle = DashStyle.Dash;
                        DrawLines(ctx, f, layer.Extent, pen);
                    }
                }
            }
        }

        static void DrawBoundaries(RenderContext ctx, VectorTileLayer layer)
        {
            if (layer == null)
                return;

            foreach (var f in layer.Features)
            {
                if (f.Type != VectorGeometryType.LineString || f.GetLong("maritime") == 1)
                    continue;

                long level = f.GetLong("admin_level", 10);
                if (level > 4)
                    continue;

                float w = (level <= 2 ? 2.0f : 1.0f) * Math.Max(1.0f, ctx.WidthScale * 0.6f);
                using (var pen = new Pen(BoundaryColor, w) { DashStyle = level <= 2 ? DashStyle.DashDot : DashStyle.Dash })
                {
                    DrawLines(ctx, f, layer.Extent, pen);
                }
            }
        }

        static void DrawPlaceLabels(RenderContext ctx, VectorTileLayer layer)
        {
            if (layer == null)
                return;

            using (var halo = new Pen(Color.FromArgb(220, 255, 255, 255), 3.0f) { LineJoin = LineJoin.Round })
            using (var brush = new SolidBrush(LabelColor))
            using (var family = new FontFamily(GenericFontFamilies.SansSerif))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                foreach (var f in layer.Features)
                {
                    if (f.Type != VectorGeometryType.Point || f.Parts.Count == 0 || f.Parts[0].Count == 0)
                        continue;

                    float size;
                    switch (f.GetString("class"))
                    {
                        case "country": size = 14; break;
                        case "state": size = 12; break;
                        case "city": size = 13; break;
                        case "town": size = 11; break;
                        case "village": size = ctx.Zoom >= 11 ? 10 : 0; break;
                        case "suburb": size = ctx.Zoom >= 13 ? 10 : 0; break;
                        case "hamlet":
                        case "neighbourhood": size = ctx.Zoom >= 14 ? 9 : 0; break;
                        default: size = 0; break;
                    }

                    if (size <= 0)
                        continue;

                    string name = f.GetString("name:latin") ?? f.GetString("name");
                    if (string.IsNullOrEmpty(name))
                        continue;

                    var p = ctx.ToPixels(f.Parts[0], layer.Extent)[0];
                    if (p.X < 0 || p.Y < 0 || p.X >= TileSize || p.Y >= TileSize)
                        continue;

                    using (var path = new GraphicsPath())
                    {
                        path.AddString(name, family, (int)FontStyle.Regular, size, p, format);
                        var b = path.GetBounds();
                        b.Inflate(2, 2);
                        if (!ctx.TryOccupy(b))
                            continue;
                        ctx.G.DrawPath(halo, path);
                        ctx.G.FillPath(brush, path);
                    }
                }
            }
        }

        static string GetName(VectorTileFeature f)
        {
            string n = f.GetString("name:latin");
            if (string.IsNullOrEmpty(n))
                n = f.GetString("name");
            return string.IsNullOrEmpty(n) ? null : n;
        }

        /// <summary>
        ///     Felirat egy vonal mentén: a vonal leghosszabb, csempén belül eső szakaszának közepére,
        ///     a szakasz irányába forgatva (mindig olvasható irányban). Ha a felirat nem fér el, kimarad.
        /// </summary>
        static bool DrawLineLabel(RenderContext ctx, VectorTileFeature f, int extent, string text,
            FontFamily family, FontStyle style, float size, Brush brush, Pen halo)
        {
            PointF bestA = PointF.Empty, bestB = PointF.Empty;
            double bestLen = 0;

            foreach (var part in f.Parts)
            {
                if (part.Count < 2)
                    continue;

                var pts = ctx.ToPixels(part, extent);
                for (int i = 1; i < pts.Length; i++)
                {
                    float mx = (pts[i - 1].X + pts[i].X) / 2;
                    float my = (pts[i - 1].Y + pts[i].Y) / 2;
                    if (mx < 0 || my < 0 || mx >= TileSize || my >= TileSize)
                        continue;

                    double dx = pts[i].X - pts[i - 1].X;
                    double dy = pts[i].Y - pts[i - 1].Y;
                    double len = Math.Sqrt(dx * dx + dy * dy);
                    if (len > bestLen)
                    {
                        bestLen = len;
                        bestA = pts[i - 1];
                        bestB = pts[i];
                    }
                }
            }

            if (bestLen < 8)
                return false;

            using (var path = new GraphicsPath())
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                path.AddString(text, family, (int)style, size, PointF.Empty, format);
                var textBounds = path.GetBounds();
                if (textBounds.Width > bestLen * 1.1)
                    return false; // nem fér el a szakaszon

                float angle = (float)(Math.Atan2(bestB.Y - bestA.Y, bestB.X - bestA.X) * 180 / Math.PI);
                if (angle > 90) angle -= 180;
                if (angle < -90) angle += 180;

                using (var m = new Matrix())
                {
                    m.Translate((bestA.X + bestB.X) / 2, (bestA.Y + bestB.Y) / 2);
                    m.Rotate(angle);
                    path.Transform(m);
                }

                var b = path.GetBounds();
                b.Inflate(1, 1);
                if (!ctx.TryOccupy(b))
                    return false;

                ctx.G.DrawPath(halo, path);
                ctx.G.FillPath(brush, path);
                return true;
            }
        }

        static void DrawStreetNames(RenderContext ctx, VectorTileLayer layer)
        {
            if (layer == null || ctx.Zoom < 12)
                return;

            var done = new HashSet<string>();

            using (var halo = new Pen(Color.FromArgb(230, 255, 255, 255), 3.0f) { LineJoin = LineJoin.Round })
            using (var brush = new SolidBrush(Color.FromArgb(60, 60, 60)))
            using (var family = new FontFamily(GenericFontFamilies.SansSerif))
            {
                // a fontosabb utak neve kerüljön ki először
                var features = new List<VectorTileFeature>(layer.Features);
                features.Sort((a, b) => RoadRank(a.GetString("class")).CompareTo(RoadRank(b.GetString("class"))));

                foreach (var f in features)
                {
                    if (f.Type != VectorGeometryType.LineString)
                        continue;

                    string cls = f.GetString("class");
                    int rank = RoadRank(cls);
                    if (ctx.Zoom < 14 && rank > 2)
                        continue; // kis zoomon csak a főutak neve

                    string name = GetName(f);
                    if (name == null || done.Contains(name))
                        continue;

                    float size = ctx.Zoom >= 16 ? 11 : 10;
                    if (DrawLineLabel(ctx, f, layer.Extent, name, family, FontStyle.Regular, size, brush, halo))
                        done.Add(name);
                }
            }
        }

        static int RoadRank(string cls)
        {
            switch (cls)
            {
                case "motorway": return 0;
                case "trunk": return 1;
                case "primary": return 2;
                case "secondary": return 3;
                case "tertiary": return 4;
                case "minor": return 5;
                case "service": return 6;
                default: return 7;
            }
        }

        static void DrawWaterNames(RenderContext ctx, VectorTileLayer waterName, VectorTileLayer waterway)
        {
            var color = Color.FromArgb(40, 90, 160);
            var done = new HashSet<string>();

            using (var halo = new Pen(Color.FromArgb(200, 255, 255, 255), 3.0f) { LineJoin = LineJoin.Round })
            using (var brush = new SolidBrush(color))
            using (var family = new FontFamily(GenericFontFamilies.SansSerif))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                // tavak, tengerek: pont feliratok
                if (waterName != null)
                {
                    foreach (var f in waterName.Features)
                    {
                        string name = GetName(f);
                        if (name == null || done.Contains(name) || f.Parts.Count == 0 || f.Parts[0].Count == 0)
                            continue;

                        if (f.Type == VectorGeometryType.LineString)
                        {
                            if (DrawLineLabel(ctx, f, waterName.Extent, name, family, FontStyle.Italic, 11, brush, halo))
                                done.Add(name);
                            continue;
                        }

                        var p = ctx.ToPixels(f.Parts[0], waterName.Extent)[0];
                        if (p.X < 0 || p.Y < 0 || p.X >= TileSize || p.Y >= TileSize)
                            continue;

                        using (var path = new GraphicsPath())
                        {
                            path.AddString(name, family, (int)FontStyle.Italic, 11, p, format);
                            var b = path.GetBounds();
                            if (!ctx.TryOccupy(b))
                                continue;
                            ctx.G.DrawPath(halo, path);
                            ctx.G.FillPath(brush, path);
                            done.Add(name);
                        }
                    }
                }

                // folyók, patakok: a vonal mentén
                if (waterway != null && ctx.Zoom >= 12)
                {
                    foreach (var f in waterway.Features)
                    {
                        if (f.Type != VectorGeometryType.LineString)
                            continue;
                        if (ctx.Zoom < 14 && f.GetString("class") != "river")
                            continue;

                        string name = GetName(f);
                        if (name == null || done.Contains(name))
                            continue;

                        if (DrawLineLabel(ctx, f, waterway.Extent, name, family, FontStyle.Italic, 10, brush, halo))
                            done.Add(name);
                    }
                }
            }
        }

        static void DrawHouseNumbers(RenderContext ctx, VectorTileLayer layer)
        {
            if (layer == null || ctx.Zoom < 17)
                return;

            using (var brush = new SolidBrush(Color.FromArgb(110, 100, 95)))
            using (var font = new Font(FontFamily.GenericSansSerif, 8f, GraphicsUnit.Pixel))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                foreach (var f in layer.Features)
                {
                    if (f.Type != VectorGeometryType.Point || f.Parts.Count == 0 || f.Parts[0].Count == 0)
                        continue;

                    string hn = f.GetString("housenumber");
                    if (string.IsNullOrEmpty(hn))
                        continue;

                    var p = ctx.ToPixels(f.Parts[0], layer.Extent)[0];
                    if (p.X < 0 || p.Y < 0 || p.X >= TileSize || p.Y >= TileSize)
                        continue;

                    var sz = ctx.G.MeasureString(hn, font);
                    var r = new RectangleF(p.X - sz.Width / 2, p.Y - sz.Height / 2, sz.Width, sz.Height);
                    if (!ctx.TryOccupy(r))
                        continue;

                    ctx.G.DrawString(hn, font, brush, p, format);
                }
            }
        }

        static void DrawLines(RenderContext ctx, VectorTileFeature f, int extent, Pen pen)
        {
            foreach (var part in f.Parts)
            {
                if (part.Count >= 2)
                {
                    ctx.G.DrawLines(pen, ctx.ToPixels(part, extent));
                }
            }
        }
    }
}
