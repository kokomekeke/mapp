using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;

namespace SgxMap.Objects
{
    [Serializable]
    public class MapLabel : BasePoint, ISerializable, IDeserializationCallback, IDisposable
    {


        public static readonly Brush DefaultTextColor = new SolidBrush(Color.Black); //FromArgb(155, Color.Black));

        /// <summary>
        ///     foreground color
        /// </summary>
        public Brush TextColor = DefaultTextColor;

        public Size Size { get; set; }

        public static readonly Font DefaultFont = new Font("Arial", 12);
        [NonSerialized] public Font Font = DefaultFont;

        public bool DrawBorder { get; set; } = false;    


        string _text;
        public string Text
        {   
            get { return _text; }
            set
            {
                _text = value;
                Size = TextRenderer.MeasureText(_text, Font);

                if (Layer != null && Layer.Control != null)
                {
                    m_layer?.RefreshObjects();
                }

            }
        }

        public MapLabel() :
            base()
        {
        }

        public MapLabel(PointLatLng pointLatLng, string text)
            : base (pointLatLng, Color.Black)
        {
            Text = text;
        }

        public MapLabel(PointLatLng pointLatLng, string text, Font font, bool border = false)
            : base(pointLatLng, Color.Black)
        {
            Font = font;
            Text = text;
            DrawBorder = border;
        }

        #region serialization

        protected MapLabel(SerializationInfo info, StreamingContext context)
        {
            _text = info.GetString("text");
            Position = (PointLatLng)info.GetValue("GeoPosition", typeof(PointLatLng));
            Font = (Font)info.GetValue("font", typeof(Font));
            DrawBorder = info.GetBoolean("drawborder");
            TextColor = (Brush)info.GetValue("textcolor", typeof(Brush));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("GeoPosition", Position);
            info.AddValue("text", Text);
            info.AddValue("font", Font);
            info.AddValue("drawborder", DrawBorder);
            info.AddValue("textcolor", TextColor);
        }   

        public override void OnDeserialization(object sender)
        {
        }   

        public override void Dispose()
        {
            base.Dispose();
            TextColor.Dispose();
            Font.Dispose();
        }

        #endregion serialization


        public override void BuildGraphicsPath()
        {
            base.BuildGraphicsPath();

            m_graphicsPath?.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, LocalPosition, StringFormat.GenericDefault);
        } /* UpdateGraphicsPath */



        public static MapLabel CreateLabelExt(string[] ls, out string errorstring)
        {
            // 0: "label", 2: "lat", 3: "lng", 4: "text", 5: "fontname", 6: "fontsize", 7: "fontstyle", 8: "drawborder"
            errorstring = string.Empty;
            var pointLatLng = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
            string text = ls[4];
            string fontName = ls[5];
            float fontSize = float.Parse(ls[6]);
            var fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), ls[7]);
            bool drawBorder = bool.Parse(ls[8]);

            return new MapLabel(pointLatLng, text, new Font(fontName, fontSize, fontStyle), drawBorder);
        }


        public static MapLabel CreateLabel(string[] ls, out string errorstring)
        {
            // 0: "label", 2: "lat", 3: "lng", 4: "text"
            try
            {
                errorstring = string.Empty;
                var pointLatLng = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                string text = ls[4];

                return new MapLabel(pointLatLng, text);
            }
            catch (Exception ex)
            {
                errorstring = ex.Message;
                return null;
            }
        }

    } /* class MapLabel */
} /* namespace SgxMap.Objects */
