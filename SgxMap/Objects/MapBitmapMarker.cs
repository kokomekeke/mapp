/*=============================================================================
*  MapBitmapMarker : bitmap marker  
*
* (c) Kótai 2026.
*
* ver 1.00                                                         15-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using SgxMap.Shapes;

namespace SgxMap.Shapes
{
    // ReSharper disable InconsistentNaming
    public enum MarkerBitmapType
    {
        none = 0,
        arrow,
        blue,
        blue_small,
        blue_dot,
        blue_pushpin,
        brown_small,
        gray_small,
        green,
        green_small,
        green_dot,
        green_pushpin,
        green_big_go,
        yellow,
        yellow_small,
        yellow_dot,
        yellow_big_pause,
        yellow_pushpin,
        lightblue,
        lightblue_dot,
        lightblue_pushpin,
        orange,
        orange_small,
        orange_dot,
        pink,
        pink_dot,
        pink_pushpin,
        purple,
        purple_small,
        purple_dot,
        purple_pushpin,
        red,
        red_small,
        red_dot,
        red_pushpin,
        red_big_stop,
        black_small,
        white_small,
    }
    // ReSharper restore InconsistentNaming

    [Serializable]
    public class MapBitmapMarker : MapBaseMarker, ISerializable, IDeserializationCallback
    {
        private Bitmap? m_bitmap;
        private Bitmap? m_bitmapShadow;
        static Bitmap? m_arrowShadow;
        static Bitmap? m_msmarkerShadow;
        static Bitmap? m_shadowSmall;
        static Bitmap? _pushpinShadow;

        private static readonly Dictionary<string, Bitmap> IconCache = new Dictionary<string, Bitmap>();


        public  readonly MarkerBitmapType BitmapType = MarkerBitmapType.none;



        public Bitmap? Bitmap
        {
            get { return m_bitmap; }
            set
            {
                m_bitmap = value;
            }
        }


        //-------------------------------------------------------------------------


        public MapBitmapMarker(string name, PointLatLng pos, MarkerBitmapType type)                          //PointLatLng p, GMarkerGoogleType type, string name = null)
            : base(name, pos, Size.Empty, Color.Black)
        {
            BitmapType = type;

            if (type != MarkerBitmapType.none)
            {
                LoadBitmap();
            }
        }

        /// <summary>
        ///     marker using manual bitmap, NonSerialized
        /// </summary>
        /// <param name="p"></param>
        /// <param name="bitmap"></param>
        public MapBitmapMarker(PointLatLng p, Bitmap bitmap)
            : base("", p, new Size(bitmap.Width, bitmap.Height), Color.Black)
        {
            Bitmap = bitmap;
            //Size = new Size(bitmap.Width, bitmap.Height);
            Offset = new Point(-Size.Width / 2, -Size.Height);
        }


        public static MapBitmapMarker? CreateBitmapMarker(string[] ls, out string errorstr)
        {
            // 0: marker, 1:name, 2:lat, 3:long, 4: bitmaptype
            string name = ls[1];
            if (name == ".") name = "";
            double lat = 0;
            double lng = 0;
            errorstr = string.Empty;
            string bitmapname = ls[4];

            if (!double.TryParse(ls[2], out lat))
            {
                errorstr = "Invalid latitude";
                return null;
            }
            if (!double.TryParse(ls[3], out lng))
            {
                errorstr = "Invalid longitude";
                return null;
            }

            var bitmaptype = MarkerBitmapType.none;

            switch (ls[4])
            {
                case "arrow": bitmaptype = MarkerBitmapType.arrow; break;
                case "blue": bitmaptype = MarkerBitmapType.blue; break;
                case "blue_small": bitmaptype = MarkerBitmapType.blue_small; break;
                case "blue_dot": bitmaptype = MarkerBitmapType.blue_dot; break;
                case "blue_pushpin": bitmaptype = MarkerBitmapType.blue_pushpin; break;
                case "brown_small": bitmaptype = MarkerBitmapType.brown_small; break;
                case "gray_small": bitmaptype = MarkerBitmapType.gray_small; break;
                case "green": bitmaptype = MarkerBitmapType.green; break;
                case "green_small": bitmaptype = MarkerBitmapType.green_small; break;
                case "green_dot": bitmaptype = MarkerBitmapType.green_dot; break;
                case "green_pushpin": bitmaptype = MarkerBitmapType.green_pushpin; break;
                case "green_big_go": bitmaptype = MarkerBitmapType.green_big_go; break;
                case "yellow": bitmaptype = MarkerBitmapType.yellow; break;
                case "yellow_small": bitmaptype = MarkerBitmapType.yellow_small; break;
                case "yellow_dot": bitmaptype = MarkerBitmapType.yellow_dot; break;
                case "yellow_big_pause": bitmaptype = MarkerBitmapType.yellow_big_pause; break;
                case "yellow_pushpin": bitmaptype = MarkerBitmapType.yellow_pushpin; break;
                case "lightblue": bitmaptype = MarkerBitmapType.lightblue; break;
                case "lightblue_dot": bitmaptype = MarkerBitmapType.lightblue_dot; break;
                case "lightblue_pushpin": bitmaptype = MarkerBitmapType.lightblue_pushpin; break;
                case "orange": bitmaptype = MarkerBitmapType.orange; break;
                case "orange_small": bitmaptype = MarkerBitmapType.orange_small; break;
                case "orange_dot": bitmaptype = MarkerBitmapType.orange_dot; break;
                case "pink": bitmaptype = MarkerBitmapType.pink; break;
                case "pink_dot": bitmaptype = MarkerBitmapType.pink_dot; break;
                case "pink_pushpin": bitmaptype = MarkerBitmapType.pink_pushpin; break;
                case "purple": bitmaptype = MarkerBitmapType.purple; break;
                case "purple_small": bitmaptype = MarkerBitmapType.purple_small; break;
                case "purple_dot": bitmaptype = MarkerBitmapType.purple_dot; break;
                case "purple_pushpin": bitmaptype = MarkerBitmapType.purple_pushpin; break;
                case "red": bitmaptype = MarkerBitmapType.red; break;
                case "red_small": bitmaptype = MarkerBitmapType.red_small; break;
                case "red_dot": bitmaptype = MarkerBitmapType.red_dot; break;
                case "red_pushpin": bitmaptype = MarkerBitmapType.red_pushpin; break;
                case "red_big_stop": bitmaptype = MarkerBitmapType.red_big_stop; break;
                case "black_small": bitmaptype = MarkerBitmapType.black_small; break;
                case "white_small": bitmaptype = MarkerBitmapType.white_small; break;

            }


            var p = new PointLatLng(lat, lng);

            return new MapBitmapMarker(name, p, bitmaptype);
        } /* CreateMarker */



        void LoadBitmap()
        {
            Bitmap = GetIcon(BitmapType.ToString());
            if (Bitmap == null)
                return;

            Size = new Size(Bitmap.Width, Bitmap.Height);

            switch (BitmapType)
            {
                case MarkerBitmapType.arrow:
                    {
                        Offset = new Point(-11, -Size.Height);

                        if (m_arrowShadow == null)
                        {
                            m_arrowShadow = Properties.Resources.arrowshadow;
                        }

                        m_bitmapShadow = m_arrowShadow;
                    }
                    break;

                case MarkerBitmapType.blue:
                case MarkerBitmapType.blue_dot:
                case MarkerBitmapType.green:
                case MarkerBitmapType.green_dot:
                case MarkerBitmapType.yellow:
                case MarkerBitmapType.yellow_dot:
                case MarkerBitmapType.lightblue:
                case MarkerBitmapType.lightblue_dot:
                case MarkerBitmapType.orange:
                case MarkerBitmapType.orange_dot:
                case MarkerBitmapType.pink:
                case MarkerBitmapType.pink_dot:
                case MarkerBitmapType.purple:
                case MarkerBitmapType.purple_dot:
                case MarkerBitmapType.red:
                case MarkerBitmapType.red_dot:
                    {
                        Offset = new Point(-Size.Width / 2 + 1, -Size.Height + 1);

                        if (m_msmarkerShadow == null)
                        {
                            m_msmarkerShadow = Properties.Resources.msmarker_shadow;
                        }

                        m_bitmapShadow = m_msmarkerShadow;
                    }
                    break;

                case MarkerBitmapType.black_small:
                case MarkerBitmapType.blue_small:
                case MarkerBitmapType.brown_small:
                case MarkerBitmapType.gray_small:
                case MarkerBitmapType.green_small:
                case MarkerBitmapType.yellow_small:
                case MarkerBitmapType.orange_small:
                case MarkerBitmapType.purple_small:
                case MarkerBitmapType.red_small:
                case MarkerBitmapType.white_small:
                    {
                        Offset = new Point(-Size.Width / 2, -Size.Height + 1);

                        if (m_shadowSmall == null)
                        {
                            m_shadowSmall = Properties.Resources.shadow_small;
                        }

                        m_bitmapShadow = m_shadowSmall;
                    }
                    break;

                case MarkerBitmapType.green_big_go:
                case MarkerBitmapType.yellow_big_pause:
                case MarkerBitmapType.red_big_stop:
                    {
                        Offset = new Point(-Size.Width / 2, -Size.Height + 1);
                        if (m_msmarkerShadow == null)
                        {
                            m_msmarkerShadow = Properties.Resources.msmarker_shadow;
                        }

                        m_bitmapShadow = m_msmarkerShadow;
                    }
                    break;

                case MarkerBitmapType.blue_pushpin:
                case MarkerBitmapType.green_pushpin:
                case MarkerBitmapType.yellow_pushpin:
                case MarkerBitmapType.lightblue_pushpin:
                case MarkerBitmapType.pink_pushpin:
                case MarkerBitmapType.purple_pushpin:
                case MarkerBitmapType.red_pushpin:
                    {
                        Offset = new Point(-9, -Size.Height + 1);

                        if (_pushpinShadow == null)
                        {
                            _pushpinShadow = Properties.Resources.pushpin_shadow;
                        }

                        m_bitmapShadow = _pushpinShadow;
                    }
                    break;
            }
        }


        internal static Bitmap? GetIcon(string name)
        {
            Bitmap? ret;
            if (!IconCache.TryGetValue(name, out ret))
            {
                ret = Properties.Resources.ResourceManager.GetObject(name, Properties.Resources.Culture) as Bitmap;
                if (ret != null)
                    IconCache.Add(name, ret);
            }

            return ret;
        }

        public override void OnRender(Graphics g)
        {
            if (Bitmap == null)
                return;

            lock (Bitmap)
            {
                if (IsSelected)
                    g.DrawRectangle(BorderPen, m_area);

                if (IsHighlighted)
                    g.FillEllipse(HighlightFill, Rectangle.Inflate(m_area,4,4));

                if (m_bitmapShadow != null)
                {
                    g.DrawImage(m_bitmapShadow,
                        LocalPosition.X,
                        LocalPosition.Y,
                        m_bitmapShadow.Width,
                        m_bitmapShadow.Height);
                }

                g.DrawImage(Bitmap, LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            }

            //g.DrawString(LocalPosition.ToString(), SystemFonts.DefaultFont, Brushes.Red, LocalPosition);
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("type", BitmapType);
        }

        protected MapBitmapMarker(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            BitmapType = Extensions.GetStruct(info, "type", MarkerBitmapType.none);
        }

        #endregion

        #region IDeserializationCallback Members

        public override void OnDeserialization(object sender)
        {
            if (BitmapType != MarkerBitmapType.none)
            {
                LoadBitmap();
            }
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


        #endregion

    } /* class MapGoogleMarker */
} /* namespace SgxMap.Objects */
