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

namespace SgxMap.Objects
{
    public class MapLine :BasePoint, ISerializable, IDeserializationCallback, IDisposable
    {
        public PointLatLng m_position2;
        public Point       LocalPosition2;

        protected ArrowPositions m_arrowPosition = ArrowPositions.None;
        protected CustomLineCap m_endCap;
        protected CustomLineCap m_startCap;

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

                    if (Layer != null && Layer.Control != null)
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
                if (m_arrowPosition != value)
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
                            Stroke.CustomStartCap = m_startCap;
                            Stroke.EndCap = LineCap.NoAnchor;
                            break;
                        case ArrowPositions.AtEnd:
                            Stroke.StartCap = LineCap.NoAnchor;
                            Stroke.CustomEndCap = m_endCap;
                            break;
                        case ArrowPositions.Both:
                            Stroke.CustomStartCap = m_startCap;
                            Stroke.CustomEndCap = m_endCap;
                            break;
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

                return Layer.Control.GetDistance(m_position,m_position2);
            }
        }

        public double Bearing
        {
            get
            {
                if ((m_position == PointLatLng.Empty) || (m_position2 == PointLatLng.Empty) || (m_position == m_position2))
                    return 0;

                return Layer.Control.GetBearing2(m_position, m_position2);
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


        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.Control != null)
            {
                base.UpdateLocalPosition();
                LocalPosition2 = Layer.Control.PositiontoLocal(Position2);
            }
        }


        public override void BuildGraphicsPath()
        {
            m_graphicsPath?.AddLine(LocalPosition, LocalPosition2);
        }

        public virtual void UpdateEndPoint(int x, int y)
        {
            var point = Layer.Control.FromLocalToLatLng(x, y);
            Position2 = point;
        }


        #region serialization

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Position2", m_position2);

        }

        public void OnDeserialization(object sender)
        {
        }

        public override void Dispose()
        {
        }

        #endregion serialization

    } /* class MapLine */
} /* namespace SgxMap.Objects */
