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

        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(155, Color.Black));

        /// <summary>
        ///     specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;

        public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(155, Color.Black));

        /// <summary>
        ///     background color
        /// </summary>
        [NonSerialized] public Brush Fill = DefaultFill;

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
            Stroke.Dispose();
            Fill.Dispose();
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
                g.DrawString(Text, Font, Fill, LocalPosition);
            }
            catch { }
            ;
        }

    } /* class GMapText */
} /* namespace GMap.NET.WindowsForms */
