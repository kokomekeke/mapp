using System
    ;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GMap.NET.WindowsForms
{
    [Serializable]

    public class GMapCircle : GMapPolyLine, ISerializable, IDeserializationCallback, IDisposable
    {
        private double _radius;
        private PointLatLng _center;
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
            get { return _center; } 
            set 
            { 
                _center = value;
                BuildCircle();
                UpdateLocalPosition();
            } 
        }

        public double Step = Math.PI / _numpoint;


        public GMapCircle(PointLatLng center, double radius, System.Drawing.Color color, string name)
            : base(new List<PointLatLng>(), color, name)
        {
            _center = center;
            _radius = radius;
            Stroke = new Pen(color, DefaultStroke.Width)
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


        public void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdatePolygonLocalPosition(this);
                UpdateGraphicsPath();
            }
        }


        public void ChangeRadius(PointLatLng newpoint)
        {
            if (!_editable || (newpoint == null))
                return;

            //int index = Points.IndexOf(oldpoint);
            //if (index != -1)
            //{
                Radius = Overlay.Control.GetDistance(Center, newpoint);

                if (Overlay != null && Overlay.Control != null)
                {
                    Overlay.Control.UpdatePolygonLocalPosition(this);
                    if (!Overlay.Control.HoldInvalidation)
                    {
                        Overlay.Control.Invalidate();
                    }
                }
            //}
        }


        public static GMapCircle CreateCircle(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "circle", 1: name,  2: lat1, 3: lng1, 4: radius, 5: color
                errorstring = null;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                var color = ColorTranslator.FromHtml(ls[5]);
                double radius = double.Parse(ls[4]);

                return new GMapCircle(p1, radius, color, ls[1]) ;
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

        protected GMapCircle(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            _center = (PointLatLng)info.GetValue("Center", typeof(PointLatLng));
            Step = info.GetDouble("Step");
            _radius = info.GetDouble("Radius");
            Stroke.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
            //_editable = false;
            //Points.Remove(Points[Points.Count - 1]);
            //Points.Remove(Points[0]);
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
        }

         #endregion

        } /* class GMapCircle */
    } /* namespace GMap.NET.WindowsForms */
