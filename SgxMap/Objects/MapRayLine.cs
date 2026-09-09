/*=============================================================================
*  MapRayLine: projection of a ray on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         09-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;

namespace SgxMap.Objects
{
    public class MapRayLine : MapLine, ISerializable
    {
        private static int _r = 6;
        private double m_direction = 0;          // direction from North in degree
        private double m_distance = 4000;        // distance in km
        private bool m_updated = false;          // true, when line locations are set.
        
        /// <summary>
        /// Direction of line from North in degree
        /// </summary>
        public double Direction
        {
            get { return m_direction; }
            set
            {
                if (m_direction != value)
                {
                    m_direction = value;
                    m_updated = false;
                    UpdateLocalPosition();
                }
            }
        }


        /// <summary>
        /// Distance in km
        /// </summary>
        public double Distance
        {
            get { return m_distance; }
            set
            {
                if (m_distance != value)
                {
                    m_distance = value;
                    m_updated = false;
                    UpdateLocalPosition();
                }
            }
        }

        public MapRayLine()
            : base()
        {

        }

        public MapRayLine(string name, PointLatLng point1, double degree, Color color)
            : base(name, point1, point1, color)
        {
            Direction = degree;
            ArrowPosition = ArrowPositions.AtEnd;
            m_isEditable = false;
            //ToolTipText = string.IsNullOrEmpty(Name) ? point1.ToGeoString() : Name;
        }

        public MapRayLine(string name, PointLatLng point1, double degree, double length, Color color)
            : base(name, point1, point1, color)
        {
            Direction = degree;
            Distance = length;
            ArrowPosition = ArrowPositions.AtEnd;
            m_isEditable = false;
            //ToolTipText = string.IsNullOrEmpty(Name) ? point1.ToGeoString() : Name;
        }


        public MapRayLine(PointLatLng point1, double degree, Color color)
            : base("", point1, point1, color)
        {
            Direction = degree;
            ArrowPosition = ArrowPositions.AtEnd;
            m_isEditable = false;
            //ToolTipText = string.IsNullOrEmpty(Name) ? point1.ToGeoString() : Name;
        }

        public MapRayLine(PointLatLng point1, double degree, double length, Color color)
            : base("", point1, point1, color)
        {
            Direction = degree;
            Distance += length;
            ArrowPosition = ArrowPositions.AtEnd;
            m_isEditable = false;
            //ToolTipText = string.IsNullOrEmpty(Name) ? point1.ToGeoString() : Name;
        }




        public static MapRayLine CreateRay(string[] ls, out string errorstring)
        {
            // 0: "ray", 1: name,  2: lat1, 3: lng1, 4: direction, 5: distance, 6: color
            try
            {
                errorstring = string.Empty;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var direction = double.Parse(ls[4]);
                var distance = double.Parse(ls[5]);
                var color = ColorTranslator.FromHtml(ls[6]);
                return new MapRayLine(ls[1], p1, direction, distance, color);
            }
            catch (Exception ex)
            {
                errorstring = ex.Message;
                return null;
            }
        }


        public void UpdateLocalPosition2()
        {
            double len = m_distance * 1000.0 / Layer.MapPanel.MapProvider.Projection.GetGroundResolution((int)Layer.MapPanel.Zoom, Position.Lat);
            double x = Math.Sin(Direction * Math.PI / 180);
            double y = Math.Cos(Direction * Math.PI / 180);
            LocalPosition2.X = LocalPosition.X + (int)(x * len);
            LocalPosition2.Y = LocalPosition.Y - (int)(y * len);
        }


        public override void BuildGraphicsPath()
        {
            if (Layer != null && Layer.MapPanel != null)
            {
                //.. from HF manager

                LocalPosition = Layer.MapPanel.FromLatLngToLocalPosition(Position);
                //LocalPosition = LocalPosition1;
                double bearing = (Direction % 360.0) / 180.0 * Math.PI;
                double maxlength = Distance / 6371.0; //the radius of earth in km
                double dl = maxlength / 100.0;
                double length = dl;

//                var pnts = new Point[101];
  //              pnts[0] = LocalPosition;
                 var points = new List<Point>();
                 points.Add(LocalPosition);

                double startRadLat = Position.Lat / 180.0 * Math.PI;
                double startRadLng = Position.Lng / 180.0 * Math.PI;
                int i = 1;

                while (length < maxlength)
                {
                    double newLato = Math.Asin(Math.Sin(startRadLat) * Math.Cos(length) +
                                           Math.Cos(startRadLat) * Math.Sin(length) * Math.Cos(bearing));

                    double newLngo = startRadLng + Math.Atan2(
                                     Math.Sin(bearing) * Math.Sin(length) * Math.Cos(startRadLat),
                                     Math.Cos(length) - Math.Sin(startRadLat) * Math.Sin(newLato));

                    //double newLato = Math.Asin(Math.Sin(startRadLat) * Math.Cos(dl) +
                    //                       Math.Cos(startRadLat) * Math.Sin(dl) * Math.Cos(bearing));

                    //double newLngo = startRadLng + Math.Atan2(
                    //                 Math.Sin(bearing) * Math.Sin(dl) * Math.Cos(startRadLat),
                    //                 Math.Cos(dl) - Math.Sin(startRadLat) * Math.Sin(newLato));


                    double newLat = newLato * 180.0 / Math.PI % 90.0;
                    double newLng = (newLngo * 180.0 / Math.PI + 540.0) % 360.0 - 180.0;

                    var p2 = Layer.FromLatLngToLocalPosition(new PointLatLng(newLat, newLng));
                    //pnts[i] = (Point)p2;
                    points.Add((Point)p2);
                    length += dl;
                    //startRadLat = newLato;
                    //startRadLng = newLngo;
                    i++;
                }
                var pnts = points.ToArray();
                m_graphicsPath?.AddLines(pnts);

            }
        } /* BuildGraphicsPath */


        public override void OnRender(Graphics g)
        {
            try
            {
                if (Layer != null && Layer.MapPanel != null)
                {
                    if (m_graphicsPath != null) 
                        g.DrawPath(Stroke, m_graphicsPath);
                    g.FillEllipse(Fill, new Rectangle(LocalPosition.X - _r, LocalPosition.Y - _r, 2 * _r, 2 * _r));
                }
            }
            catch { }
            ;
        }


        public virtual void UpdateEndPoint(int x, int y)
        {
            var point = Layer.MapPanel.FromLocalToLatLng(x, y);
            m_distance = Layer.MapPanel.GetDistance(Position, point);
            m_direction = Layer.MapPanel.GetBearing(Position, point);

            Layer.RefreshObject(this);
        }



        #region serialization

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Direction", Direction);
            info.AddValue("Distance", Distance);
        }


        protected MapRayLine(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Direction = info.GetDouble("Direction");
            Distance = info.GetDouble("Distance");
            CreateCap();
        }



        //public void OnDeserialization(object sender)
        //{
        //    base.OnDeserialization(sender);
        //}

        #endregion serialization




    } /* class MapRayLine */

} /* namespace SgxMap.Objects */
