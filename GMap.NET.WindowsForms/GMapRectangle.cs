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
    public class GMapRectangle : ISerializable
    {

        private PointLatLng _center;
        private float _width;             // in km
        private float _height;

        private float _localWidth;
        private float _localHeight;

        private bool _editable = true;      // if true, then the ellipse can be edited by mouse, otherwise it is read-only
        public bool Editable
        {
            get { return _editable; }
            set { _editable = value; }
        }

        public string Name = "Rectangle";

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

        public float Width
        {
            get { return _width; }
            set 
            { 
                _width = value; 
                UpdateLocalPosition();
            }
        }

        public float Height
        {
            get { return _height; }
            set 
            { 
                _height = value;
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



        public GMapRectangle(PointLatLng center, float width, float height, System.Drawing.Color color, string name)
        {
            _center = center;
            _width = width;
            _height = height;
            Stroke = new Pen(color, DefaultStroke.Width);
            Name = name;
            LastColor = Stroke.Color;
        }

        public void UpdateLocalPosition()
        {
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdateRectangleLocalPosition(this);
                _localWidth  = Overlay.Control.Core.GetLocalDistance(Center, _width);
                _localHeight = Overlay.Control.Core.GetLocalDistance(Center, _height);
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
                        var m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _width, 90), GMapMarker.MovementType.Horizontal);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);

                        m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _width, 270), GMapMarker.MovementType.Horizontal);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);

                        m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _height, 0), GMapMarker.MovementType.Vertical);
                        Overlay.Control.UpdateMarkerLocalPosition(m);
                        Overlay?.Markers.Add(m);

                        m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Overlay.Control.GetPositionFromPoint(Center, _height, 180), GMapMarker.MovementType.Vertical);
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
            return Math.Abs(((dx * dx) / (_localWidth * _localWidth) + (dy * dy) / (_localHeight * _localHeight)) - 1.0) <= 0.05;
        }


        public void MovePoint(GMarkerLinePoint oldpoint, PointLatLng newpoint)
        {
            if (!_editable || (newpoint == null))
                return;

            if (oldpoint.Movement == GMapMarker.MovementType.Horizontal)
                Width = (float)Overlay.Control.GetDistance(Center, newpoint);
            else if (oldpoint.Movement == GMapMarker.MovementType.Vertical)
                Height = (float)Overlay.Control.GetDistance(Center, newpoint);
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
                _graphicsPath.AddRectangle(new RectangleF((float)(LocalPosition.X - _localWidth), (float)(LocalPosition.Y - _localHeight),
                                            (float)(_localWidth * 2), (float)(_localHeight * 2)));
            }
        }


        public void OnRender(Graphics g)
        {
            // Implement the rendering logic for the rectangle here
            // For example, you can use the Graphics object to draw the rectangle on a map control
            if (!IsVisible)
                return;

            g.DrawPath(Stroke, _graphicsPath);
        }


        public static GMapRectangle CreateRectangle(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "circle", 1: name,  2: lat1, 3: lng1, 4: radius, 5: Width, 6: color
                errorstring = null;
                var p1 = new PointLatLng(double.Parse(ls[2]), double.Parse(ls[3]));
                float Width = float.Parse(ls[4]);
                float Height = float.Parse(ls[5]);
                var color = ColorTranslator.FromHtml(ls[6]);

                return new GMapRectangle(p1, Width, Height, color, ls[1]);
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
            info.AddValue("Width", Width);  
            info.AddValue("Height", Height);
            info.AddValue("Visible", IsVisible);
            info.AddValue("Color", Stroke.Color.ToArgb());
        }

        protected GMapRectangle(SerializationInfo info, StreamingContext context)
        {             
            Name = info.GetString("Name");
            _center = (PointLatLng)info.GetValue("Center", typeof(PointLatLng));
            _width = info.GetSingle("Width");
            _height = info.GetSingle("Height");
            IsVisible = info.GetBoolean("Visible");
            Stroke = new Pen(Color.FromArgb(info.GetInt32("Color")), DefaultStroke.Width);
            LastColor = Stroke.Color;
        }

        #endregion 

    } /*class GMapRectangle */

} /* namespace GMap.NET.WindowsForms */
