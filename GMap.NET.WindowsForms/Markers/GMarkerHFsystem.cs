using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace GMap.NET.WindowsForms.Markers
{
    // ReSharper disable InconsistentNaming
    public enum GMarkerHFsystemType
    {
        none = 0,
        Világoskékkör,
        Lilakör,
        Sárgakör,
        Bíborkör,
        Feketekör,
        Piroskör,
        Kékkör,
        Sötétvöröskör,
        Zöldkör
    }

    // ReSharper restore InconsistentNaming

    [Serializable]
    public class GMarkerHFsystem : GMapMarker, ISerializable, IDeserializationCallback
    {
        private Bitmap _bitmap;

        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
            }
        }

        //Bitmap _bitmapShadow;

        //static Bitmap _arrowShadow;
        //static Bitmap _msmarkerShadow;
        //static Bitmap _shadowSmall;
        //static Bitmap _pushpinShadow;

        public readonly GMarkerHFsystemType Type;

        public GMarkerHFsystem(PointLatLng p, GMarkerHFsystemType type)
            : base(p)
        {
            Type = type;
            Offset = Point.Empty;

            if (type != GMarkerHFsystemType.none)
            {
                LoadBitmap();
            }
        }

        void LoadBitmap()
        {
            Bitmap = GetIcon(Type.ToString());
            Size = new Size(Bitmap.Width, Bitmap.Height);
            Offset = Point.Empty;
        }

        /// <summary>
        ///     marker using manual bitmap, NonSerialized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="bitmap"></param>
        public GMarkerHFsystem(PointLatLng p, Bitmap bitmap)
            : base(p)
        {
            Bitmap = bitmap;
            Size = new Size(bitmap.Width, bitmap.Height);
            Offset = Point.Empty;
        }

        static readonly Dictionary<string, Bitmap> IconCache = new Dictionary<string, Bitmap>();

        internal static Bitmap GetIcon(string name)
        {
            Bitmap ret;
            if (!IconCache.TryGetValue(name, out ret))
            {
                ret = Properties.Resources.ResourceManager.GetObject(name, Properties.Resources.Culture) as Bitmap;
                IconCache.Add(name, ret);
            }

            return ret;
        }

        public override void OnRender(Graphics g)
        {
            lock (Bitmap)
            {
                g.DrawImage(Bitmap, LocalPosition.X - Size.Width/2, LocalPosition.Y- Size.Height/2, Size.Width, Size.Height);
            }

            //g.DrawString(LocalPosition.ToString(), SystemFonts.DefaultFont, Brushes.Red, LocalPosition);
        }

        public override void Dispose()
        {
            if (Bitmap != null)
            {
                if (!IconCache.ContainsValue(Bitmap))
                {
                    Bitmap.Dispose();
                    Bitmap = null;
                }
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", Type);
            //info.AddValue("Bearing", this.Bearing);

            base.GetObjectData(info, context);
        }

        protected GMarkerHFsystem(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Type = Extensions.GetStruct(info, "type", GMarkerHFsystemType.none);
            //this.Bearing = Extensions.GetStruct<float>(info, "Bearing", null);
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            if (Type != GMarkerHFsystemType.none)
            {
                LoadBitmap();
            }
        }

        #endregion
    }
}
