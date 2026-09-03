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
        }

        public MapLine(PointLatLng point1, PointLatLng point2, Color color)
            : base(point1, color)
        {
            Position2 = point2;
        }

        public MapLine(string name, PointLatLng point1, PointLatLng point2, Color color)
            : base(name, point1, color)
        {
            Position2 = point2;
        }

        protected MapLine(SerializationInfo info, StreamingContext context)
            :base(info, context) 
        {
            Position2 = Extensions.GetStruct(info, "Position2", PointLatLng.Empty);
        }


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


        public static MapLine CreateLine(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "line", 1: name,  2: lat1, 3: lng1, 4: lat2, 5: lng2, 6: color
                errorstring = null;
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


    } /* class MapLine */
} /* namespace SgxMap.Objects */
