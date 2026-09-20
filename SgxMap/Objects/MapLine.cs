/*=============================================================================
*  Map line: line on map (linear line on projected map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         02-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;

namespace SgxMap.Shapes
{
    public class MapLine: BasePoint, ISerializable, IDeserializationCallback, IDisposable
    {
        public PointLatLng m_position2;
        public Point       LocalPosition2;

        protected ArrowPositions m_arrowPosition = ArrowPositions.None;
        protected CustomLineCap? m_endCap;
        protected CustomLineCap? m_startCap;

        protected bool m_showLength = false;
        protected MapLabel m_label;

        public PointLatLng Position2
        {
            get { return m_position2; }
            set
            {
                if (m_position2 != value)
                {
                    m_position2 = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }

        public Color LineColor
        {
            get { return Stroke.Color; }
            set
            {
                if (Stroke.Color.ToArgb() != value.ToArgb())
                {
                    Stroke.Color = value;

                    if (Layer != null && Layer.MapPanel != null)
                    {
                        m_layer?.RefreshObject(this);
                    }
                }
            }
        }

        public ArrowPositions ArrowPosition
        {
            get { return m_arrowPosition; }
            set
            {
                //if (m_arrowPosition != value)
                {
                    m_arrowPosition = value;
                    // TODO: apply arror position
                    switch (m_arrowPosition)
                    {
                        case ArrowPositions.None:
                            Stroke.StartCap = LineCap.NoAnchor;
                            Stroke.EndCap = LineCap.NoAnchor;
                            break;
                        case ArrowPositions.AtBegin:
                            if (m_startCap != null)
                                 Stroke.CustomStartCap = m_startCap;
                            else Stroke.StartCap = LineCap.NoAnchor;
                            Stroke.EndCap = LineCap.NoAnchor;
                            break;
                        case ArrowPositions.AtEnd:
                            Stroke.StartCap = LineCap.NoAnchor;
                            if (m_endCap != null)
                                 Stroke.CustomEndCap = m_endCap;
                            else Stroke.EndCap = LineCap.NoAnchor;
                                break;
                        case ArrowPositions.Both:
                            if (m_startCap != null)
                                 Stroke.CustomStartCap = m_startCap;
                            else Stroke.StartCap = LineCap.NoAnchor;
                            if (m_endCap != null)
                                Stroke.CustomEndCap = m_endCap;
                            else Stroke.EndCap= LineCap.NoAnchor;
                            break;
                    }

                    if (Layer != null && Layer.MapPanel != null)
                    {
                        m_layer?.RefreshObject(this);
                    }

                }
            }
        }

        public double Length
        {
            get 
            { 
                if ((m_position == PointLatLng.Empty) || (m_position2 == PointLatLng.Empty) || (m_position == m_position2))
                    return 0;

                return Layer.MapPanel.GetDistance(m_position,m_position2);
            }
        }

        public double Bearing
        {
            get
            {
                if ((m_position == PointLatLng.Empty) || (m_position2 == PointLatLng.Empty) || (m_position == m_position2))
                    return 0;

                return Layer.MapPanel.GetBearing2(m_position, m_position2);
            }
        }


        public bool ShowLength
        {
            get {  return m_showLength; }
            set
            {
                if (m_showLength != value)
                {
                    m_showLength = value;
                    if (value)
                    {
                        if (m_label == null)
                            m_label = Layer.AddLabel(Position2, Length.ToString("F2") + " km", false);
                        else
                        {
                            m_label.Text = Length.ToString("F2") + " km";
                            m_label.Position = Position2;
                            m_label.IsVisible = true;
                        }
                        Layer.RefreshObject(m_label);
                    }
                    else if (m_label != null)
                    {          
                        m_label.IsVisible = false;
                   }

                }
            }
        }

        public override bool IsVisible
        {
            get { return m_isVisible; }
            set
            {
                if (m_isVisible != value)
                {
                    if (m_label != null)
                        m_label.IsVisible = value;
                    m_isVisible = value;
                    m_layer?.RefreshObject(this);
                }
            }
        }


        // Constructors
        public MapLine()
            : base()
        {
            CreateCap();
        }

        public MapLine(PointLatLng point1, PointLatLng point2, Color color)
            : base(point1, color)
        {
            CreateCap();
            Position2 = point2;
        }

        public MapLine(string name, PointLatLng point1, PointLatLng point2, Color color)
            : base(name, point1, color)
        {
            CreateCap();
            Position2 = point2;
        }

        protected MapLine(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            CreateCap();
            Position2 = Extensions.GetStruct(info, "Position2", PointLatLng.Empty);
        }

        public static MapLine? CreateLine(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "line", 1: name,  2: lat1, 3: lng1, 4: lat2, 5: lng2, 6: color
                errorstring = string.Empty;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var p2 = new PointLatLng(double.Parse(ls[4]), double.Parse(ls[5]));
                var color = ColorTranslator.FromHtml(ls[6]);

                return new MapLine(p1, p2, color) { Name = ls[1] };
            }
            catch (Exception ex)
            {
                errorstring = ex.ToString();
                return null;
            }
        }

        public void CreateCap()
        {
            var hPath = new GraphicsPath();

            // Create the outline for our custom end cap.
            hPath.AddLine(new Point(0, 0), new Point(3, -5));
            hPath.AddLine(new Point(3, -5), new Point(-3, -5));
            hPath.AddLine(new Point(-3, -5), new Point(0, 0));

            m_endCap = new CustomLineCap(hPath, null);

            hPath = new GraphicsPath();

            // Create the outline for our custom end cap.
            hPath.AddLine(new Point(0, 0), new Point(3, -5));
            hPath.AddLine(new Point(3, -5), new Point(-3, -5));
            hPath.AddLine(new Point(-3, -5), new Point(0, 0));

            m_startCap = new CustomLineCap(hPath, null);

        }


        public override void Select(bool select)
        {
            if (select)
            {
                if (m_isSelected)
                    return;

                Layer?.MapPanel.ClearLinePoints();
                Layer?.AddLinePoint(this, Position, MovementType.Both, Color.Black);
                Layer?.AddLinePoint(this, Position2, MovementType.Both, Color.Black);
            }
            else
            {
                Layer?.MapPanel.ClearLinePoints(this);
            }
            base.Select(select);
        }


        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.MapPanel != null)
            {
                base.UpdateLocalPosition();
                LocalPosition2 = Layer.MapPanel.PositiontoLocal(Position2);
            }
        }


        public override bool IsInside(int x, int y)
        {
            double x1 = LocalPosition.X;
            double y1 = LocalPosition.Y;
            double x2 = LocalPosition2.X;
            double y2 = LocalPosition2.Y;

            if (Math.Abs(y2 - y1) > Math.Abs(x2 - x1))
            {
                if (y1 < y && y2 >= y || y2 < y && y1 >= y)
                {
                    if (Math.Abs(x1 + (y - y1) / (y2 - y1) * (x2 - x1) - x) < 10)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (x1 < x && x2 >= x || x2 < x && x1 >= x)
                {
                    if (Math.Abs(y1 + (x - x1) / (x2 - x1) * (y2 - y1) - y) < 10)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool IsInside(PointLatLng p)
        {
                var p1 = Position;
                var p2 = Position2;

                if (Math.Abs(p2.Lat - p1.Lat) > Math.Abs(p2.Lng - p1.Lng))
                {
                    if (p1.Lat < p.Lat && p2.Lat >= p.Lat || p2.Lat < p.Lat && p1.Lat >= p.Lat)
                    {
                        if (Math.Abs(p1.Lng + (p.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) - p.Lng) < 0.02)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        if (Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat) < 0.02)
                        {
                            return true;
                        }
                    }
                }

                return false;
        } /*IsInside */


        public override void MovePoint(PointLatLng oldpoint, PointLatLng newpoint)
        {
            if (Position == oldpoint)
                Position = newpoint;
            else if (Position2 == oldpoint)
                Position2 = newpoint;

            if (m_showLength)
            {
                m_showLength = false;
                ShowLength = true;
            }
        }

        public override void Shift(PointLatLng oldpoint, PointLatLng newpoint)
        {
            var b = Layer.MapPanel.GetBearing2(oldpoint, newpoint);
            var d = Layer.MapPanel.GetDistance(oldpoint, newpoint);

            m_position = Layer.MapPanel.GetPositionFromPoint(Position, d, b);
            Position2  = Layer.MapPanel.GetPositionFromPoint(Position2, d, b);
        }

        public override void BuildGraphicsPath()
        {
            m_graphicsPath?.AddLine(LocalPosition, LocalPosition2);
        }

        public virtual void UpdateEndPoint(int x, int y)
        {
            var point = Layer.MapPanel.FromLocalToLatLng(x, y);
            Position2 = point;
            if (m_showLength)
            {
                m_showLength = false;
                ShowLength = true;
            }
        }


        public override void Remove()
        {
            if (m_label != null)
                m_label.Remove();
            Layer?.MapPanel.RemoveLinePoints(this);
            base.Remove();
        }

        public override void OnRender(Graphics g)
        {
            if (m_graphicsPath == null)
            {
                UpdateGraphicsPath();
            }
            if (m_graphicsPath != null)
            {
                if (IsHighlighted)
                    g.DrawLine(HighlightStroke, LocalPosition, LocalPosition2);
                g.DrawLine(Stroke, LocalPosition, LocalPosition2);
            }
        }

        #region serialization

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Position2", m_position2);

        }

        public override void OnDeserialization(object? sender)
        {
        }

        public override void Dispose()
        {
        }

        #endregion serialization

    } /* class MapLine */
} /* namespace SgxMap.Objects */
