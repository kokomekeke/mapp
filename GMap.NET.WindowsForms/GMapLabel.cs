using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMap.NET.WindowsForms
{
        
    public class GMapLabel : ISerializable, IDeserializationCallback, IDisposable
    {

        public GMapOverlay Overlay { get; set; }

        public PointLatLng GeoPosition;     // position in geo coordinates
        public Point LocalPosition;        // position of label in screen buffer


        public static readonly Brush DefaultTextColor = new SolidBrush(Color.Black); //FromArgb(155, Color.Black));

        /// <summary>
        ///     foreground color
        /// </summary>
        [NonSerialized] public Brush TextColor = DefaultTextColor;

        public Size Size { get; set; }

        public static readonly Font DefaultFont = new Font("Arial", 12);
        [NonSerialized] public Font Font = DefaultFont;

        public bool IsVisible { get; set; }= true;
        public bool DrawBorder { get; set; } = false;    


        string _text;
        public string Text
        {   
            get { return _text; }
            set
            {
                _text = value;
                Size = TextRenderer.MeasureText(_text, Font);

                if (Overlay != null && Overlay.Control != null)
                {
                    Overlay.Control.UpdateLabelLocalPosition(this);
                    if (!Overlay.Control.HoldInvalidation)
                    {
                        Overlay.Control.Invalidate();
                    }
                }

            }
        }

        public GMapLabel()
        {
        }

        public GMapLabel(PointLatLng pointLatLng, string text)
        {
            GeoPosition = pointLatLng;
            Text = text;
        }

        public GMapLabel(PointLatLng pointLatLng, string text, Font font, bool border = false)
        {
            Font = font;
            Text = text;
            DrawBorder = border;
            GeoPosition = pointLatLng;
        }

        protected GMapLabel(SerializationInfo info, StreamingContext context)
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }   

        public void OnDeserialization(object sender)
        {
        }   

        public void Dispose()
        {
            TextColor.Dispose();
            Font.Dispose();
        }

        GraphicsPath _graphicsPath;

        public void UpdateGraphicsPath()
        {
            if (_graphicsPath == null)
            {
                _graphicsPath = new GraphicsPath();
            }
            else
            {
                _graphicsPath.Reset();
            }

            _graphicsPath.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, LocalPosition, StringFormat.GenericDefault);
        } /* UpdateGraphicsPath */


        public virtual void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdateLabelLocalPosition(this);
                UpdateGraphicsPath();
            }
        } /* UpdateLocalPosition */


        public virtual void OnRender(Graphics g)
        {
            try
            {
                var drawpoint = new PointF(LocalPosition.X, LocalPosition.Y);
                g.DrawString(Text, Font, TextColor, drawpoint);
            }
            catch { }
            ;
        }

        public static GMapLabel CreateLabelExt(string[] ls, out string errorstring)
        {
            // 0: "label", 2: "lat", 3: "lng", 4: "text", 5: "fontname", 6: "fontsize", 7: "fontstyle", 8: "drawborder"
            errorstring = string.Empty;
            var pointLatLng = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
            string text = ls[4];
            string fontName = ls[5];
            float fontSize = float.Parse(ls[6]);
            var fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), ls[7]);
            bool drawBorder = bool.Parse(ls[8]);

            return new GMapLabel(pointLatLng, text, new Font(fontName, fontSize, fontStyle), drawBorder);
        }


        public static GMapLabel CreateLabel(string[] ls, out string errorstring)
        {
            // 0: "label", 2: "lat", 3: "lng", 4: "text"
            try
            {
                errorstring = string.Empty;
                var pointLatLng = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                string text = ls[4];

                return new GMapLabel(pointLatLng, text);
            }
            catch (Exception ex)
            {
                errorstring = ex.Message;
                return null;
            }
        }

    } /* class GMapText */
} /* namespace GMap.NET.WindowsForms */
