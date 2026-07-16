using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;
using static GMap.NET.Entity.OpenStreetMapGraphHopperGeocodeEntity;

namespace GMap.NET.WindowsForms
{
    /// <summary>
    ///     GMap.NET polygon
    /// </summary>
    [Serializable]
    public class GMapPolyLine : MapRoute, ISerializable, IDeserializationCallback, IDisposable
    {
        private bool _visible = true;

        /// <summary>
        ///     is polyline visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value != _visible)
                {
                    _visible = value;

                    if (Overlay != null && Overlay.Control != null)
                    {
                        if (_visible)
                        {
                            Overlay.Control.UpdatePolygonLocalPosition(this);
                        }
                        else
                        {
                            if (Overlay.Control.IsMouseOverPolygon)
                            {
                                Overlay.Control.IsMouseOverPolygon = false;
                                Overlay.Control.RestoreCursorOnLeave();
                            }
                        }

                        {
                            if (!Overlay.Control.HoldInvalidation)
                            {
                                Overlay.Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

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

        public GMapOverlay Overlay { get; set; }

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
                        for (int i = 0; i < Points.Count; i++)
                        {
                            var m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, Points[i]);
                            Overlay?.Markers.Add(m);
                        }
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

        bool _isclosed = false;
        public bool IsClosed
        {
            get { return _isclosed; } 
            set
            {
                if (value != _isclosed)
                {
                    _isclosed = value;
                    if (_graphicsPath != null)
                    {
                        if (_isclosed)
                        {
                            Points.Add(Points[0]);
                        }
                        else
                        {
                            Points.RemoveAt(Points.Count - 1);
                        }
                    }

                    if (Overlay != null && Overlay.Control != null)
                    {
                        Overlay.Control.UpdatePolygonLocalPosition(this);
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Invalidate();
                        }
                    }
                }
            }
        }

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

        GraphicsPath _graphicsPath;
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
                var pnts = new System.Drawing.Point[LocalPoints.Count];
                for (int i = 0; i < LocalPoints.Count; i++)
                {
                    var p2 = new System.Drawing.Point((int)LocalPoints[i].X, (int)LocalPoints[i].Y);
                    pnts[pnts.Length - 1 - i] = p2;
                }

                if (pnts.Length > 2)
                    _graphicsPath.AddLines(pnts);
                else if (pnts.Length == 2)
                    _graphicsPath.AddLine(pnts[0], pnts[1]);
            }
        }

        public virtual void OnRender(Graphics g)
        {
            if (IsVisible)
            {
                if (_graphicsPath != null)
                {
                    if (Points.Count < 2) return;

                    if (Points.Count > 2)
                         g.DrawPath(Stroke, _graphicsPath);
                    else g.DrawLine(Stroke, LocalPoints[0].X, LocalPoints[0].Y, LocalPoints[1].X, LocalPoints[1].Y);

                }
            }
        }

        //public double Area
        //{
        //   get
        //   {
        //      return 0;
        //   }
        //}

        public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(155, Color.MidnightBlue));

        /// <summary>
        ///     specifies how the outline is painted
        /// </summary>
        [NonSerialized] public Pen Stroke = DefaultStroke;

        public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(155, Color.AliceBlue));
        
        /// <summary>
        ///     background color
        /// </summary>
        [NonSerialized] public Brush Fill = DefaultFill;

        /// <summary>
        /// List of points in map  (x,y)
        /// </summary>
        public readonly List<GPoint> LocalPoints = new List<GPoint>();

        static GMapPolyLine()
        {
            DefaultStroke.LineJoin = LineJoin.Round;
            DefaultStroke.Width = 5;
        }

        public GMapPolyLine(List<PointLatLng> points, string name)
            : base(points, name)
        {
            LocalPoints.Capacity = Points.Count;
            Stroke.EndCap = LineCap.ArrowAnchor;
        }

        public GMapPolyLine(List<PointLatLng> points, Color color, string name)
            : base(points, name)
        {
            LocalPoints.Capacity = Points.Count;
            Stroke = new Pen(color, DefaultStroke.Width)
            {
                LineJoin = DefaultStroke.LineJoin
            };
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
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
                        Overlay.Control.UpdatePolygonLocalPosition(this);
                        if (!Overlay.Control.HoldInvalidation)
                        {
                            Overlay.Control.Invalidate();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     checks if point is inside the polygon,
        ///     info.: http://greatmaps.codeplex.com/discussions/279437#post700449
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsInside(PointLatLng p)
        {
            int count = Points.Count;

            if (count == 2)
            {
                var p1 = Points[0];
                var p2 = Points[1];

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
                {   if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        if (Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat) < 0.02)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else if (count < 3)
                return false;

            bool result = false;

            for (int i = 0, j = count - 1; i < count; i++)
            {
                var p1 = Points[i];
                var p2 = Points[j];

                if (Math.Abs(p2.Lat - p1.Lat) > Math.Abs(p2.Lng - p1.Lng))
                {
                    if (p1.Lat < p.Lat && p2.Lat >= p.Lat || p2.Lat < p.Lat && p1.Lat >= p.Lat)
                    {
                        if (Math.Abs(p1.Lng + (p.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) - p.Lng) < 0.02)
                        {
                            result = !result;
                        }
                    }
                }
                else
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        if (Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat) < 0.02)
                        {
                            result = !result;
                        }
                    }
                }

                j = i;
            }

            return result;
        }


        /// <summary>
        /// Get index of line in lines
        /// </summary>
        /// <param name="p"></param>
        /// <returns>index of end point of line or 0 or -1</returns>
        public int IndexOfPoint(PointLatLng p)
        {
            int count = Points.Count;
            if (count == 0)
                return -1;

            int pos = Points.IndexOf(p);
            if (pos >= 0 && pos < LocalPoints.Count)
                return pos;

            var p1 = Points[0];
            var p2 = Points[1];

            if (count == 2)
            {
                if (Math.Abs(p2.Lat - p1.Lat) > Math.Abs(p2.Lng - p1.Lng))
                {
                    if (p1.Lat < p.Lat && p2.Lat >= p.Lat || p2.Lat < p.Lat && p1.Lat >= p.Lat)
                    {
                        if (Math.Abs(p1.Lng + (p.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) - p.Lng) < 0.02)
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        if (Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat) < 0.02)
                        {
                            return 1;
                        }
                    }
                }

                return -1;
            }
            else if (count < 3)
                return -1;


            for (int i = 1; i < count; i++)
            {
                p2 = Points[i];

                if (Math.Abs(p2.Lat - p1.Lat) > Math.Abs(p2.Lng - p1.Lng))
                {
                    if (p1.Lat < p.Lat && p2.Lat >= p.Lat || p2.Lat < p.Lat && p1.Lat >= p.Lat)
                    {
                        if (Math.Abs(p1.Lng + (p.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) - p.Lng) < 0.02)
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        if (Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat) < 0.02)
                        {
                            return i;
                        }
                    }
                }

                p1 = p2;
            }
            return -1;
        }


        public void Append(PointLatLng point)
        {
            Points.Add(point);
            LocalPoints.Capacity = Points.Count;

            if (IsSelected)
            {
                var m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, point);
                Overlay?.Markers.Add(m);
            }

            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdatePolygonLocalPosition(this);
                if (!Overlay.Control.HoldInvalidation)
                {
                    Overlay.Control.Invalidate();
                }
            }
        }


        public void Insert(int index, PointLatLng point)
        {
            Points.Insert(index, point);
            LocalPoints.Capacity = Points.Count;
            if (IsSelected)
            {
                var m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, point);
                Overlay?.Markers.Add(m);
            }
            if (Overlay != null && Overlay.Control != null)
            {
                Overlay.Control.UpdatePolygonLocalPosition(this);
                if (!Overlay.Control.HoldInvalidation)
                {
                    Overlay.Control.Invalidate();
                }
            }
        }


        public void MovePoint(int index, PointLatLng point)
        {
            if (index >= 0 && index < Points.Count)
            {
                Points[index] = point;
                if (IsSelected)
                {
                    for (int i = Overlay.Markers.Count - 1; i >= 0; i--)
                    {
                        if (Overlay.Markers[i].Parent == this && ((GMap.NET.WindowsForms.Markers.GMarkerLinePoint)Overlay.Markers[i]).Position == point)
                        {
                            ((GMap.NET.WindowsForms.Markers.GMarkerLinePoint)Overlay.Markers[i]).Position = point;
                        }
                    }
                }
                if (Overlay != null && Overlay.Control != null)
                {
                    Overlay.Control.UpdatePolygonLocalPosition(this);
                    if (!Overlay.Control.HoldInvalidation)
                    {
                        Overlay.Control.Invalidate();
                    }
                }
            }
        }

        public void MovePoint(PointLatLng oldpoint, PointLatLng newpoint)
        {
            int index = Points.IndexOf(oldpoint);
            if (index != -1)
            {
                Points[index] = newpoint;

                if (Overlay != null && Overlay.Control != null)
                {
                    Overlay.Control.UpdatePolygonLocalPosition(this);
                    if (!Overlay.Control.HoldInvalidation)
                    {
                        Overlay.Control.Invalidate();
                    }
                }
            }
        }


        public void ClearPoints()
        {
            Points.Clear();
            LocalPoints.Clear();
            if (IsSelected)
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

        public void RemovePoint(PointLatLng point)
        {
            int index = Points.IndexOf(point);
            if (index != -1)
            {
                Points.RemoveAt(index);
                LocalPoints.RemoveAt(index);
                if (IsSelected)
                {
                    for (int i = Overlay.Markers.Count - 1; i >= 0; i--)
                    {
                        if (Overlay.Markers[i].Parent == this && ((GMap.NET.WindowsForms.Markers.GMarkerLinePoint)Overlay.Markers[i]).Position == point)
                        {
                            Overlay.Markers.RemoveAt(i);
                        }
                    }
                }
                if (Overlay != null && Overlay.Control != null)
                {
                    Overlay.Control.UpdatePolygonLocalPosition(this);
                    if (!Overlay.Control.HoldInvalidation)
                    {
                        Overlay.Control.Invalidate();
                    }
                }
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("LocalPoints", LocalPoints.ToArray());
            info.AddValue("Visible", IsVisible);
        }

        // Temp store for de-serialization.
        private GPoint[] deserializedLocalPoints;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapRoute" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GMapPolyLine(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            deserializedLocalPoints = Extensions.GetValue<GPoint[]>(info, "LocalPoints");
            IsVisible = Extensions.GetStruct(info, "Visible", true);
        }

        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        ///     Runs when the entire object graph has been de-serialized.
        /// </summary>
        /// <param name="sender">
        ///     The object that initiated the callback. The functionality for this parameter is not currently
        ///     implemented.
        /// </param>
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            // Accounts for the de-serialization being breadth first rather than depth first.
            LocalPoints.AddRange(deserializedLocalPoints);
            LocalPoints.Capacity = Points.Count;
        }

        #endregion

        #region IDisposable Members

        bool _disposed;

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                LocalPoints.Clear();

                if (Overlay != null)
                {
                    for (int i = Overlay.Markers.Count - 1; i >= 0; i--)
                    {
                        if (Overlay.Markers[i].Parent == this)
                        {
                            Overlay.Markers.RemoveAt(i);
                        }
                    }
                }


                if (_graphicsPath != null)
                {
                    _graphicsPath.Dispose();
                    _graphicsPath = null;
                }

                Clear();
            }
        }

        #endregion

        public static GMapPolyLine CreatePolyline(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "polyline", 1: name,  2: lat1, 3: lng1, 4: lat2, 5: lng2, 6: lat3, 7: lng3, ... , x: color
                errorstring = null;
                var points = new List<PointLatLng>();
                for (int i = 2; i < ls.Length - 1; i += 2)
                {
                    points.Add(new PointLatLng(double.Parse(ls[i]), double.Parse(ls[i + 1])));
                }
                var color = ColorTranslator.FromHtml(ls[ls.Length - 1]);
                return new GMapPolyLine(points, color, ls[1]);
            }
            catch (Exception ex)
            {
                errorstring = ex.ToString();
                return null;
            }
        }

    }

    public delegate void PolygonClick(GMapPolyLine item, MouseEventArgs e);

    public delegate void PolygonDoubleClick(GMapPolyLine item, MouseEventArgs e);

    public delegate void PolygonEnter(GMapPolyLine item);

    public delegate void PolygonLeave(GMapPolyLine item);
}
