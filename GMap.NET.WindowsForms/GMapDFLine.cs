/*=============================================================================
*  DF line: direction line
*
* (c) Kótai 2026.
*
* ver 1.00                                                         10-Jun-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GMap.NET.WindowsForms
{
    public class GMapDFLine : GMapLine
    {
        private static int _r = 6;
        private double _direction = 0;          // direction from North in degree
        private double _distance = 7000;        // distance in km
        private bool _updated = false;          // true, when line locations are set.
        private CustomLineCap endCap;

        /// <summary>
        /// Direction of line from North in degree
        /// </summary>
        public double Direction 
        {
            get { return _direction; } 
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    _updated = false;
                    UpdateLocalPosition();
                }
            }
        }


        /// <summary>
        /// Distance in km
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    _updated = false;
                    UpdateLocalPosition();
                }
            }
        }

        public GMapDFLine()
            : base()
        {

        }

        public GMapDFLine(PointLatLng point1, double degree, Color color)
            : base(point1, point1, color)
        {
            Direction = degree;
            CreateCap();
            ToolTipText = string.IsNullOrEmpty(Name) ? point1.ToGeoString() : Name;
        }


        public void CreateCap()
        {
            var hPath = new GraphicsPath();

            // Create the outline for our custom end cap.
            hPath.AddLine(new Point(0, 0), new Point(3, -5));
            hPath.AddLine(new Point(3, -5), new Point(-3, -5));
            hPath.AddLine(new Point(-3, -5), new Point(0, 0));

            endCap = new CustomLineCap(hPath, null);
            Stroke.CustomEndCap = endCap;
        }

        public void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                _updated = true;
                Overlay.Control.UpdateDFLineLocalPosition(this);
                LocalPosition = LocalPosition1;

                //UpdateLocalPosition2();
                //if (!Overlay.Control.HoldInvalidation)
                //    Overlay.Control.Invalidate();
            }
        }

        public void UpdateLocalPosition2()
        {
            double len = _distance * 1000.0 / Overlay.Control.MapProvider.Projection.GetGroundResolution((int)Overlay.Control.Zoom, Position.Lat);
            double x = Math.Sin(Direction * Math.PI / 180);
            double y = Math.Cos(Direction * Math.PI / 180);
            LocalPosition2.X = LocalPosition1.X + (int)(x * len);
            LocalPosition2.Y = LocalPosition1.Y - (int)(y * len);
        }

        internal override void UpdateGraphicsPath()
        {
            if (_graphicsPath == null)
            {
                _graphicsPath = new GraphicsPath();
            }
            else
            {
                _graphicsPath.Reset();
            }

            if (!_updated)
                UpdateLocalPosition();

            _graphicsPath.AddLine(LocalPosition1, LocalPosition2);
            _graphicsPath.AddEllipse(new Rectangle(LocalPosition1.X - _r, LocalPosition1.Y - _r, 2 * _r, 2 * _r));
        }


        public override void OnRender(Graphics g)
        {
            try
            {
                if (!_updated)
                    UpdateLocalPosition();

                g.DrawLine(Stroke, LocalPosition1.X, LocalPosition1.Y, LocalPosition2.X, LocalPosition2.Y);
                g.FillEllipse(Fill, new Rectangle(LocalPosition1.X - _r, LocalPosition1.Y - _r, 2*_r, 2*_r));
            }
            catch { }
            ;
        }



    }
}
