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
    public enum TextPositionOnScreen { custom = 0, left, bottom, top, right };
        
    public class GMapLabel : ISerializable, IDeserializationCallback, IDisposable
    {

        public GMapOverlay Overlay { get; set; }
        public PointLatLng Point1;
        public Point LocalPosition1;
        public Point LabelPosition;

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

        private TextPositionOnScreen _textPosition = TextPositionOnScreen.custom;
        public TextPositionOnScreen TextPosition 
        { 
            get { return _textPosition; }
            set 
            {
                if (value != _textPosition)
                {
                    _textPosition = value;
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
        }



        public GMapLabel()
        {
        }

        public GMapLabel(PointLatLng pointLatLng, string text)
        {
            Point1 = pointLatLng;
            Text = text;
        }

        public GMapLabel(PointLatLng pointLatLng, string text, Font font, TextPositionOnScreen textPosition = TextPositionOnScreen.custom, bool border = false)
        {
            Font = font;
            Text = text;
            DrawBorder = border;
            Point1 = pointLatLng;
            TextPosition = textPosition;
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

        internal void UpdateGraphicsPath()
        {
            if (_graphicsPath == null)
            {
                _graphicsPath = new GraphicsPath();
            }
            else
            {
                _graphicsPath.Reset();
            }

            var point = LocalPosition1;
            //point.X = 0; point.Y = 0;
            switch (TextPosition)
            {
                case TextPositionOnScreen.bottom:
                    point.Y = Overlay.Control.Height / 2 - Size.Height - 3;
                    break;
                case TextPositionOnScreen.top:
                    point.Y = -Overlay.Control.Height/2 + 3;
                    break;
                case TextPositionOnScreen.left:
                        point.X = -Overlay.Control.Width / 2 + 3;
                    break;
                case TextPositionOnScreen.right:
                    point.X = Overlay.Control.Width / 2 - Size.Width - 3;
                    break;
            }
            LabelPosition = point;
            _graphicsPath.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, point, StringFormat.GenericDefault);
        }


        public virtual void OnRender(Graphics g)
        {
            try
            {
                g.DrawString(Text, Font, Fill, LabelPosition);
            }
            catch { }
            ;
        }

    } /* class GMapText */
} /* namespace GMap.NET.WindowsForms */
