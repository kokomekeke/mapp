using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace GMap.NET.WindowsForms
{
    public class GMapLine : GMapMarker, ISerializable, IDeserializationCallback, IDisposable
    {
        //public GMapOverlay Overlay { get; set; }
        public PointLatLng Point1, Point2;
        public Point LocalPosition1, LocalPosition2;

        protected string _name;

        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(155, Color.Gray));

        /// <summary>
        ///     specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;

        public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(155, Color.LightGray));

        /// <summary>
        ///     background color
        /// </summary>
        [NonSerialized] public Brush Fill = DefaultFill;

        private GMapTextBox label;


        public new string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    ToolTipText = _name;
                }
            }
        }

        public double Length
        {
            get 
            { 
                if ((Point1 == null) || (Point2 == null) || (Point1 == Point2))
                    return 0;

                return Overlay.Control.GetDistance(Point1, Point2);
            }
        }

        public double Bearing
        {
            get
            {
                if ((Point1 == null) || (Point2 == null) || (Point1 == Point2))
                    return 0;

                return Overlay.Control.GetBearing2(Point1, Point2);
            }
        }


        // Constructors
        public GMapLine()
        {
        }

        public GMapLine(PointLatLng point1, PointLatLng point2, Color color)
        {
            Point1 = point1;
            Point2 = point2;

            Stroke = new Pen(color, DefaultStroke.Width);
            Fill = new SolidBrush(color);
            Position = point1;
        }
        

        protected GMapLine(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            Point2 = Extensions.GetStruct(info, "Position2", PointLatLng.Empty);
            Point1 = Position;

        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Position2", Point2);

        }

        public void OnDeserialization(object sender)
        {
        }

        public override void Dispose()
        {
        }

        public Color LineColor
        {
            get { return Stroke.Color; }
            set
            {
                if (Stroke.Color.ToArgb() != value.ToArgb())
                {
                    Stroke.Color = value;

                    if (Overlay != null && Overlay.Control != null)
                    {
                        Overlay.Control.UpdateLineLocalPosition(this);
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Invalidate();
                        }
                    }
                }
            }
        }

        public static GMapLine CreateLine(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "line", 1: name,  2: lat1, 3: lng1, 4: lat2, 5: lng2, 6: color
                errorstring = null;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var p2 = new PointLatLng(double.Parse(ls[4]), double.Parse(ls[5]));
                var color = ColorTranslator.FromHtml(ls[6]);

                return new GMapLine(p1, p2, color) { Name = ls[1] };
            }
            catch (Exception ex)
            {
                errorstring = ex.ToString();
                return null;
            }
        }

        internal GraphicsPath _graphicsPath;

        internal virtual void UpdateGraphicsPath()
        {
            if (_graphicsPath == null)
            {
                _graphicsPath = new GraphicsPath();
            }
            else
            {
                _graphicsPath.Reset();
            }

            _graphicsPath.AddLine(LocalPosition1, LocalPosition2);
        }


        public void UpdateLabel()
        {
            if ((label != null) && !string.IsNullOrEmpty(label.Text))
                label.UpdateLocalPosition();
        }


        public void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdateLineLocalPosition(this);
                UpdateGraphicsPath();
            }
        }


        public virtual void OnRender(Graphics g)
        {
            try
            {
                g.DrawLine(Stroke, LocalPosition1.X, LocalPosition1.Y, LocalPosition2.X, LocalPosition2.Y);
                label?.OnRender(g);
            }
            catch { };
        }

        public void SetPosition2(PointLatLng point)
        {
            if ((point == null) || point.IsEmpty || (point == Point2))
                return;

            Point2 = point;
            if (label != null) 
                label.Text = Length.ToString("F1") + " km";
            UpdateLocalPosition();
        }

        public void ShowLength()
        {
            if (label == null)
            {
                label = new GMapTextBox(Point2, Length.ToString("F1") + " km ");
                label.Overlay = this.Overlay;
                label.Font = new Font("Arial", 8);
            }
            else label.Text = Length.ToString("F1") + " km";
            label.UpdateLocalPosition();
        }

    } /* class GMapLine */

} /* namespace GMap.NET.WindowsForms */
