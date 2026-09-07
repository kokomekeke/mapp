/*=============================================================================
*  Geo circle: projection of circle on earth
*
* (c) Kótai 2026.
*
* ver 1.00                                                         05-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.Internals;

namespace SgxMap.Objects
{
    public class GeoCircle: MapPolyLine, ISerializable, IDeserializationCallback, IDisposable
    {
        private double _radius;
         const int _numpoint = 20;

        public double Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                BuildCircle();
                UpdateLocalPosition();
            }
        }

        public PointLatLng Center
        {
            get { return Position; }
            set
            {
                Position = value;
                BuildCircle();
                UpdateLocalPosition();
            }
        }

        public double Step = Math.PI / _numpoint;


        public GeoCircle(string name, PointLatLng center, double radius, System.Drawing.Color color)
            : base(name, new List<PointLatLng>(), color)
        {
            Position = center;
            _radius = radius;
            Stroke = new Pen(color, 5)
            {
                LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            };

            BuildCircle();
            //_editable = false;
        }

        public GeoCircle(PointLatLng center, double radius, System.Drawing.Color color)
            : base("", new List<PointLatLng>(), color)
        {
            Position = center;
            _radius = radius;
            Stroke = new Pen(color, 5)
            {
                LineJoin = System.Drawing.Drawing2D.LineJoin.Round
            };

            BuildCircle();
            //_editable = false;
        }


        private void BuildCircle()
        {
            Points.Clear();
            double bearing = 0.0;
            double startRadLat = Center.Lat / 180.0 * Math.PI;
            double startRadLon = Center.Lng / 180.0 * Math.PI;
            double length = Radius / 6371.0; //the radius of earth in km

            double sinRadLat = Math.Sin(startRadLat);
            double cosRadLat = Math.Cos(startRadLat);
            double sinRadLon = Math.Sin(startRadLon);
            double cosLength = Math.Cos(length);
            double sinLength = Math.Sin(length);

            while (bearing < 2.0 * Math.PI)
            {
                double newLat = Math.Asin(sinRadLat * cosLength +
                                       cosRadLat * sinLength * Math.Cos(bearing));

                double newLon = startRadLon + Math.Atan2(
                                 Math.Sin(bearing) * sinLength * cosRadLat,
                                 cosLength - sinRadLat * Math.Sin(newLat));

                newLat = newLat * 180.0 / Math.PI % 90.0;
                newLon = (newLon * 180.0 / Math.PI + 540.0) % 360.0 - 180.0;

                Points.Add(new PointLatLng(newLat, newLon));
                bearing += Step;
            }

            Points.Add(Points[0]);
        }


        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.Control != null)
            {
                LocalPoints.Clear();

                for (int i = 0; i < Points.Count; i++)
                {
                    var p = Layer.Control.FromLatLngToLocal(Points[i]);
                    p.OffsetNegative(Layer.Control.Core.RenderOffset);
                    LocalPoints.Add(p);
                }

                UpdateGraphicsPath();
            }
        }


        public void ChangeRadius(PointLatLng newpoint)
        {
            if (!m_isEditable || (newpoint == null))
                return;

            Radius = Layer.Control.GetDistance(Center, newpoint);

            if (Layer != null && Layer.Control != null)
            {
                Layer.RefreshObject(this);
            }
        }

        public void ChangeRadius(int x, int y)
        {
            var p = Layer.Control.FromLocalToLatLng(x, y);
            ChangeRadius(p);
        }


        public static GeoCircle? CreateCircle(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "circle", 1: name,  2: lat1, 3: lng1, 4: radius, 5: color
                errorstring = string.Empty;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var color = ColorTranslator.FromHtml(ls[5]);
                double radius = double.Parse(ls[4]);

                return new GeoCircle(ls[1], p1, radius, color);
            }
            catch (Exception ex)
            {
                errorstring = ex.ToString();
                return null;
            }
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Radius", Radius);
            info.AddValue("Center", Center);
            info.AddValue("Step", Step);
        }

        protected GeoCircle(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            Position = (PointLatLng)info.GetValue("Center", typeof(PointLatLng));
            Step = info.GetDouble("Step");
            _radius = info.GetDouble("Radius");
            Stroke.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
            //_editable = false;
            //Points.Remove(Points[Points.Count - 1]);
            //Points.Remove(Points[0]);
        }

        public override void OnDeserialization(object? sender)
        {
            base.OnDeserialization(sender);
        }

        #endregion



    }
}
