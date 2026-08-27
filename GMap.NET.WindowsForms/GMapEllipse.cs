using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.WindowsForms.Markers;

namespace GMap.NET.WindowsForms
{
    [Serializable]
    public class GMapEllipse : ISerializable
    {

        private PointLatLng _center;
        private float _radiusX;             // in km
        private float _radiusY;

        private float _localradiusX;
        private float _localradiusY;

        private bool _editable = true;      // if true, then the ellipse can be edited by mouse, otherwise it is read-only
        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; }
        }

        public string Name = "Ellipse";

        public Point LocalPosition;

        public GMapOverlay Overlay { get; set; }

        public PointLatLng Center
        {
            get { return _center; }
            set 
            { 
                _center = value;
                UpdateLocalPosition();
            }
        }

        public float RadiusX
        {
            get { return _radiusX; }
            set 
            { 
                _radiusX = value; 
                UpdateLocalPosition();
            }
        }

        public float RadiusY
        {
            get { return _radiusY; }
            set 
            { 
                _radiusY = value;
                UpdateLocalPosition();
            }
        }

        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(155, Color.MidnightBlue), 5);

        /// <summary>
        ///     specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;


        private Color _lastcolor = DefaultStroke.Color;
        public Color LastColor
        {
            get { return _lastcolor; }
            private set { _lastcolor = value; }
        }

        public Color LineColor
        {
            get { return Stroke.Color; }
            set
            {
                if (Stroke.Color.ToArgb() != value.ToArgb())
                {
                    Stroke.Color = value;
                    LastColor = value;

                    if (Overlay != null && Overlay.Control != null)
                    {
                        //Overlay.Control.UpdateEllipseLocalPosition(this);
                        UpdateLocalPosition();
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Invalidate();
                        }
                    }
                }
            }
        }


        public bool IsVisible
        { get; set; } = true;

        /// <summary>
        ///     can receive input
        /// </summary>
        public bool IsHitTestVisible = true;

        private bool _isMouseOver;

        /// <summary>
        ///     is mouse over
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return _isMouseOver;
            }
            internal set
            {
                _isMouseOver = value;
            }
        }



        public GMapEllipse(PointLatLng center, float radiusX, float radiusY, System.Drawing.Color color, string name)
        {
            _center = center;
            _radiusX = radiusX;
            _radiusY = radiusY;
            Stroke = new Pen(color, DefaultStroke.Width);
            Name = name;
            LastColor = Stroke.Color;
        }

        public void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdateEllipseLocalPosition(this);
                _localradiusX = Overlay.Control.Core.GetLocalDistance(Center, _radiusX);
                _localradiusY = Overlay.Control.Core.GetLocalDistance(Center, _radiusY);
                UpdateGraphicsPath();
            }
        }




        protected bool _isselected = false;

        public bool IsSelected
        {
            get { return _isselected; }
            set
            {
                if (value != _isselected)
                {
                    _isselected = value;

                    if (_isselected)
                    {
                        var m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _radiusX, 90), GMapMarker.MovementType.Horizontal);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);

                        m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _radiusX, 270), GMapMarker.MovementType.Horizontal);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);

                        m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _radiusY, 0), GMapMarker.MovementType.Vertical);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);

                        m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _radiusY, 180), GMapMarker.MovementType.Vertical);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);
                    }
                    else
                    {
                        for (int i = Overlay.Markers.Count - 1; i >= 0; i--)
                        {
                            if (Overlay.Markers[i].Parent == this)
                            {
                                Overlay.Markers.RemoveAt(i);
                            }
                        }
                    }

                    if (Overlay != null && Overlay.Control != null)
                    {
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Invalidate();
                        }
                    }
                }
            }
        }


        GraphicsPath _graphicsPath;

        /// <summary>
        ///     Indicates whether the specified point is contained within this System.Drawing.Drawing2D.GraphicsPath
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal bool IsInsideLocal(int x, int y)
        {
            if (_graphicsPath != null)
            {
                return _graphicsPath.IsVisible(x, y);
            }

            return false;
        }


        /// <summary>
        ///     checks if point is inside the ellipse,
        ///     info.: http://greatmaps.codeplex.com/discussions/279437#post700449
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsInside(PointLatLng p)
        {
            var d = Overlay.Control.FromLatLngToLocalPosition(p);
            float dx = d.X - LocalPosition.X;
            float dy = d.Y - LocalPosition.Y;
            return Math.Abs(((dx * dx) / (_localradiusX * _localradiusX) + (dy * dy) / (_localradiusY * _localradiusY)) - 1.0) <= 0.05;
        }


        public void MovePoint(GMarkerLinePoint oldpoint, PointLatLng newpoint)
        {
            if (!_editable || (newpoint == null))
                return;

            if (oldpoint.Movement == GMapMarker.MovementType.Horizontal)
                RadiusX = (float)Overlay.Control.GetDistance(Center, newpoint);
            else if (oldpoint.Movement == GMapMarker.MovementType.Vertical)
                RadiusY = (float)Overlay.Control.GetDistance(Center, newpoint);
            else return;

            if (Overlay != null && Overlay.Control != null)
            {
                UpdateLocalPosition();

                if (!Overlay.Control.HoldInvalidation)
                {
                    Overlay.Control.Invalidate();
                }
            }

        }

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

            {
                _graphicsPath.AddEllipse((float)(LocalPosition.X - _localradiusX), (float)(LocalPosition.Y - _localradiusY),
                                         (float)(_localradiusX * 2), (float)(_localradiusY * 2));
            }
        }


        public void OnRender(Graphics g)
        {
            // Implement the rendering logic for the ellipse here
            // For example, you can use the Graphics object to draw the ellipse on a map control
            if (!IsVisible)
                return;

            //g.DrawEllipse(Stroke, (float)(LocalPosition.X - _localradiusX), (float)(LocalPosition.Y - _localradiusY), 
            //                             (float)(_localradiusX * 2), (float)(_localradiusY * 2));

            g.DrawPath(Stroke, _graphicsPath);
        }


        public static GMapEllipse CreateEllipse(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "circle", 1: name,  2: lat1, 3: lng1, 4: radius, 5: radiusx, 6: color
                errorstring = null;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                float radiusx = float.Parse(ls[4]);
                float radiusy = float.Parse(ls[5]);
                var color = ColorTranslator.FromHtml(ls[6]);

                return new GMapEllipse(p1, radiusx, radiusy, color, ls[1]);
            }
            catch (Exception ex)
            {
                errorstring = ex.ToString();
                return null;
            }
        }


        #region ISerializable Members

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">
        ///     The caller does not have the required permission.
        /// </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Center", Center);
            info.AddValue("RadiusX", RadiusX);  
            info.AddValue("RadiusY", RadiusY);
            info.AddValue("Visible", IsVisible);
            info.AddValue("Color", Stroke.Color.ToArgb());
        }

        protected GMapEllipse(SerializationInfo info, StreamingContext context)
        {             
            Name = info.GetString("Name");
            _center = (PointLatLng)info.GetValue("Center", typeof(PointLatLng));
            _radiusX = info.GetSingle("RadiusX");
            _radiusY = info.GetSingle("RadiusY");
            IsVisible = info.GetBoolean("Visible");
            Stroke = new Pen(Color.FromArgb(info.GetInt32("Color")), DefaultStroke.Width);
            LastColor = Stroke.Color;
        }

        #endregion 

    } /*class GMapEllipse */

} /* namespace GMap.NET.WindowsForms */
