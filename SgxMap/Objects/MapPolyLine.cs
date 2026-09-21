/*=============================================================================
*  MapPolyLine: polyline on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         05-Sep-2026.
*          Last modification                                       16-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SgxMap.Shapes
{
    [Serializable]
    public class MapPolyLine : BasePoint, ISerializable, IDeserializationCallback, IDisposable
    {
        protected bool m_isClosed = false;
        protected bool m_showLength = false;
        protected MapLabel m_label;

        protected ArrowPositions m_arrowPosition = ArrowPositions.None;
        private Color m_lastcolor = Color.Black;

        public readonly List<PointLatLng> Points = new List<PointLatLng>(); // points of route in geo coordinates
        public readonly List<Point> LocalPoints = new List<Point>();      // List of points in map  (x,y)

        protected CustomLineCap? m_endCap;
        protected CustomLineCap? m_startCap;

        public Color LastColor
        {
            get { return m_lastcolor; }
            private set { m_lastcolor = value; }
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

                    if (Layer != null && Layer.MapPanel != null)
                    {
                        Layer.RefreshObject(this);
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
                            else Stroke.EndCap = LineCap.NoAnchor;
                            break;
                    }

                    if (Layer != null && Layer.MapPanel != null)
                    {
                        m_layer?.RefreshObject(this);
                    }

                }
            }
        }

        public bool IsClosed
        {
            get { return m_isClosed; }
            set
            {
                if (value != m_isClosed)
                {
                    m_isClosed = value;
                    if (m_graphicsPath != null)
                    {
                        if (m_isClosed)
                        {
                            Points.Add(Points[0]);
                        }
                        else
                        {
                            Points.RemoveAt(Points.Count - 1);
                        }
                    }

                    if (Layer != null && Layer.MapPanel != null)
                    {
                        Layer?.RefreshObject(this);
                    }
                }
            }
        }


        public bool ShowLength
        {
            get { return m_showLength; }
            set
            {
                if (m_showLength != value)
                {
                    m_showLength = value;
                    if (value && (Points.Count > 1))
                    {
                        if (m_label == null)
                            m_label = Layer.AddLabel(Points[Points.Count-1], Length.ToString("F2") + " km", false);
                        else
                        {
                            m_label.Text = Length.ToString("F2") + " km";
                            m_label.Position = Points[Points.Count - 1];
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


        public double Length
        {
            get
            {
                double length = 0;
                for (int i = 1; i < Points.Count; i++)
                    length += Layer.MapPanel.GetDistance(Points[i - 1], Points[i]);
                return length;
            }
        }


        //---------------------------------------------------------------------------------------

        public MapPolyLine()
            : base()
        {
            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 5;
        }

        public MapPolyLine(string name, List<PointLatLng> points)
            : base(name, points, Color.Black)
        {
            for (int i = 0; i < points.Count; i++)
                Points.Add(points[i]);

            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 2;
            LocalPoints.Capacity = Points.Count;
        }

        public MapPolyLine(string name, List<PointLatLng> points, Color color )
            : base(name, points, color)
        {
            for (int i = 0; i < points.Count; i++)
                Points.Add(points[i]);

            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 2;
            LocalPoints.Capacity = Points.Count;
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
            LastColor = Stroke.Color;
        }

        public MapPolyLine(string name, PointLatLng point1, PointLatLng point2, Color color)
                : base(name, point1, color)
        {
            Points.Add(point1);
            Points.Add(point2);
            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 2;
            LocalPoints.Capacity = Points.Count;
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
            LastColor = Stroke.Color;
        }


        public static MapPolyLine? CreatePolyline(string[] ls, out string errorstring)
        {
            try
            {
                // 0: "polyline", 1: name,  2: lat1, 3: lng1, 4: lat2, 5: lng2, 6: lat3, 7: lng3, ... , x: color
                errorstring = string.Empty;
                var points = new List<PointLatLng>();
                for (int i = 2; i < ls.Length - 1; i += 2)
                {
                    points.Add(new PointLatLng(double.Parse(ls[i]), double.Parse(ls[i + 1])));
                }
                var color = ColorTranslator.FromHtml(ls[ls.Length - 1]);
                return new MapPolyLine(ls[1], points, color);
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




        /// <summary>
        ///     clears points and sets tag and name to null
        /// </summary>
        public void Clear()
        {
            Points.Clear();
        }


        protected void RefreshLabel(bool showit)
        {
            m_showLength = !showit;
            ShowLength = showit;
        }


        public override void Remove()
        {
            Layer?.MapPanel.RemoveLinePoints(this);
            base.Remove();
        }


        public override void UpdateLocalPosition()
        {
            if (Layer != null && Layer.MapPanel != null)
            {
                base.UpdateLocalPosition();
                //LocalPoints.Capacity = Points.Count;
                LocalPoints.Clear();

                for (int i = 0; i < Points.Count; i++)
                {
                    var o = Layer.FromLatLngToLocalPosition(Points[i]);
                    if (o != null)
                        LocalPoints.Add((Point)o);
                }

                UpdateGraphicsPath();
            }
        }


        public override void BuildGraphicsPath()
        {
            var pnts = new Point[LocalPoints.Count];
            for (int i = 0; i < LocalPoints.Count; i++)
            {
                pnts[pnts.Length - 1 - i] = LocalPoints[i];
            }

            if (pnts.Length > 2)
                m_graphicsPath?.AddLines(pnts);
            else if (pnts.Length == 2)
                m_graphicsPath?.AddLine(pnts[0], pnts[1]);
        }


        public override void OnRender(Graphics g)
        {
            if (IsVisible)
            {
                if (m_graphicsPath != null)
                {
                    if (Points.Count < 2) return;

                    if (IsHighlighted)
                    {
                        if (Points.Count > 2)
                            g.DrawPath(HighlightStroke, m_graphicsPath);
                        else g.DrawLine(HighlightStroke, LocalPoints[0].X, LocalPoints[0].Y, LocalPoints[1].X, LocalPoints[1].Y);
                    }

                    if (IsFilled)
                    {
                        g.FillPath(Fill, m_graphicsPath);
                    }

                    if (Points.Count > 2)
                         g.DrawPath(Stroke, m_graphicsPath);
                    else g.DrawLine(Stroke, LocalPoints[0].X, LocalPoints[0].Y, LocalPoints[1].X, LocalPoints[1].Y);
                }
            }
        }



        /// <summary>
        ///     Indicates whether the specified point is contained within this System.Drawing.Drawing2D.GraphicsPath
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public  bool IsInsideLocal(int x, int y)
        {
            if (m_graphicsPath != null)
            {
                return m_graphicsPath.IsVisible(x, y);
            }

            return false;
        }


        /// <summary>
        ///     checks if point is inside the polygon,
        ///     info.: http://greatmaps.codeplex.com/discussions/279437#post700449
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool IsInside(PointLatLng p)
        {
            int count = Points.Count;

            if (count == 2)
            {
                var p1 = Points[0];
                var p2 = Points[1];

                return MapLine.DistancePointToLine(p.Lat, p.Lng, p1.Lat, p1.Lng, p2.Lat, p2.Lng) < MapLine.geo_resolution;
            }
            else if (count < 3)
                return false;

            bool result = false;
            for (int i = 0, j = count - 1; i < count; i++)
            {
                var p1 = Points[i];
                var p2 = Points[j];

                if (MapLine.DistancePointToLine(p.Lat, p.Lng, p1.Lat, p1.Lng, p2.Lat, p2.Lng) < MapLine.geo_resolution)
                    result = true;

                j = i;
            }

            return result;
        }

        public override bool IsInside(int x, int y)
        {
            if (Points.Count == 2)
            {
                double x1 = LocalPoints[0].X;
                double y1 = LocalPoints[0].Y;
                double x2 = LocalPoints[1].X;
                double y2 = LocalPoints[1].Y;

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
            else
            {
                for (int i = 1; i < LocalPoints.Count; i++)
                {
                    var p1 = LocalPoints[i-1];
                    var p2 = LocalPoints[i];

                    if (MapLine.DistancePointToLine(x, y, p1.X, p1.Y, p2.X, p2.Y) < MapLine.local_resolution)
                        return true;
                }

                return false;
            }

        } /* IsInside */



        /// <summary>
        /// Get index of point in lines
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
                        double l = Math.Abs(p1.Lng + (p.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) - p.Lng);
                        if (l < MapLine.geo_resolution)
                        {
                            return 1;
                        }
                    }
                }
                else
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        double l = Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat);
                        if (l < MapLine.geo_resolution)
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
                        double l = Math.Abs(p1.Lng + (p.Lat - p1.Lat) / (p2.Lat - p1.Lat) * (p2.Lng - p1.Lng) - p.Lng);
                        if ( l < MapLine.geo_resolution)
                        {
                            return i;
                        }
                    }
                }
                else
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
                    {
                        double l = Math.Abs(p1.Lat + (p.Lng - p1.Lng) / (p2.Lng - p1.Lng) * (p2.Lat - p1.Lat) - p.Lat);
                        if (l < MapLine.geo_resolution)
                        {
                            return i;
                        }
                    }
                }

                p1 = p2;
            }
            return -1;
        }

        /// <summary>
        /// Get index of point in lines
        /// </summary>
        /// <param name="p"></param>
        /// <returns>index of end point of line or 0 or -1</returns>
        public int IndexOfPoint(int x, int y)
        {
            int count = Points.Count;
            if (count == 0)
                return -1;

            var lp = Layer.MapPanel.FromLocalToLatLng(x, y);
            int pos = Points.IndexOf(lp);
            if (pos >= 0 && pos < LocalPoints.Count)
                return pos;

            var p1 = LocalPoints[0];
            var p2 = LocalPoints[1];
            var p = Layer.MapPanel.FromLatLngToLocalPosition(lp);

            if (count == 2)
            {
                if (MapLine.DistancePointToLine(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y) < MapLine.local_resolution)
                    return 1;
                return -1;
            }
            else if (count < 3)
                return -1;

            double minl = double.MaxValue;
            int mini = -1;

            for (int i = 1; i < count; i++)
            {
                p2 = LocalPoints[i];

                double l = MapLine.DistancePointToLine(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y);
                if (l < minl)
                {
                    minl = l;
                    mini = i;
                }
                p1 = p2;
            }
            return mini;
        }



        public void Append(PointLatLng point)
        {
            if (!m_isEditable || point.IsEmpty)
                return;

            Points.Add(point);
            LocalPoints.Capacity = Points.Count;
            UpdateLocalPosition();

            //if (IsSelected)
            //{
            //    var m = new GMap.NET.WindowsForms.Markers.GMarkerLinePoint(this, point);
            //    Layer?.Markers.Add(m);
            //}

            if (Layer != null && Layer.MapPanel != null)
            {
                Layer.RefreshObject(this);
            }
        }


        public void Insert(int index, PointLatLng point)
        {
            if (!m_isEditable || point.IsEmpty)
                return;

            Points.Insert(index, point);
            LocalPoints.Capacity = Points.Count;
            UpdateLocalPosition();
            Layer.AddLinePoint(this, point, MovementType.Both, Color.Black);

            if (Layer != null && Layer.MapPanel != null)
            {
                Layer.RefreshObject(this);
            }

            RefreshLabel(m_showLength);
        }

        public void InsertPoint(PointLatLng point)
        {
            var p = Layer.MapPanel.FromLatLngToLocal(point);
            int index = IndexOfPoint((int)p.X, (int)p.Y);
            if (index < 0) return;
            Insert(index, point);
        }


        public void MovePoint(int index, PointLatLng point)
        {
            if (!m_isEditable || point.IsEmpty)
                return;

            if (index >= 0 && index < Points.Count)
            {
                Points[index] = point;
                //if (IsSelected)
                //{
                //}

                if (Layer != null && Layer.MapPanel != null)
                {
                    Layer.RefreshObject(this);
                }
            }
        }

        public override void MovePoint(PointLatLng oldpoint, PointLatLng newpoint)
        {
            if (!m_isEditable || newpoint.IsEmpty || oldpoint.IsEmpty)
                return;

            int index = Points.IndexOf(oldpoint);
            if (index != -1)
            {
                Points[index] = newpoint;

                if (Layer != null && Layer.MapPanel != null)
                {
                    Layer.RefreshObject(this);
                }
                RefreshLabel(m_showLength);
            }
        }


        public void ClearPoints()
        {
            Points.Clear();
            LocalPoints.Clear();

            if (IsSelected)
            {
                Layer.MapPanel.ClearLinePoints(this);
            }

            if (Layer != null && Layer.MapPanel != null)
            {
                Layer.RefreshObject(this);
            }

            RefreshLabel(m_showLength);

        }

        public void RemovePoint(PointLatLng point)
        {
            if (!m_isEditable || point.IsEmpty)
                return;

            if (Points.Count < 3) 
                return;
            int index = Points.IndexOf(point);
            if (index != -1)
            {
                Points.RemoveAt(index);
                LocalPoints.RemoveAt(index);
                if (IsSelected)
                {
                    for (int i = Layer.MapPanel.LinePoints.Count - 1; i >= 0; i--)
                    {
                        var o = Layer.MapPanel.LinePoints[i];
                        if ((o.Parent == this) && (o.Position == point))
                        {
                            Layer.RemoveLinePoint(i);
                        }
                    }
                }

                if (Layer != null && Layer.MapPanel != null)
                {
                    Layer.RefreshObject(this);
                }

                RefreshLabel(m_showLength);
            }
        }


        public override void Select(bool select)
        {
            if (select)
            {
                if (m_isSelected)
                    return;

                Layer.MapPanel.ClearLinePoints();

                for (int i = 0; i < Points.Count; i++)
                {
                    if (!Points[i].IsEmpty)
                        Layer.AddLinePoint(this, Points[i], MovementType.Both, Color.Black);
                }
            }
            else
            {
                Layer.MapPanel.ClearLinePoints(this);
            }

            //base.Select(select);
            m_isSelected = select;
            m_layer?.RefreshObject(this);
        }


        public override void UpdateLinePoints()
        {
            if (m_isSelected && m_layer != null)
            {
                Layer.MapPanel.ClearLinePoints(this);
                for (int i = 0; i < Points.Count; i++)
                {
                    if (!Points[i].IsEmpty)
                        Layer.AddLinePoint(this, Points[i], MovementType.Both, Color.Black);
                }
            }
            RefreshLabel(m_showLength);
            Layer.RefreshObject(this);
        }


        public override void Shift(PointLatLng oldpoint, PointLatLng newpoint)
        {
            var b = Layer.MapPanel.GetBearing2(oldpoint, newpoint);
            var d = Layer.MapPanel.GetDistance(oldpoint, newpoint);

            for (int i = 0; i < Points.Count; i++)
                Points[i] = Layer.MapPanel.GetPositionFromPoint(Points[i], d, b);

            Layer.RefreshObject(this);
            RefreshLabel(m_showLength);
        }


        public virtual void UpdateEndPoint(int x, int y)
        {
            var point = Layer.MapPanel.FromLocalToLatLng(x, y);
            if (Points.Count > 0)
            {
                Points[Points.Count - 1] = point;
                UpdateLinePoints();
            }
        }


        public override void CopyToClipboard()
        {
            var st = string.Empty;
            for (int i = 0; i < Points.Count; i++)
            {
                if (i > 0) st += " ";
                st += Points[i].ToDegreeString();
            }
            Clipboard.SetText(st);
        }




        #region ISerializable Members

        public Point[] DeserializedLocalPoints { get => deserializedLocalPoints; set => deserializedLocalPoints = value; }
        public Point[] DeserializedLocalPoints1 { get => deserializedLocalPoints; set => deserializedLocalPoints = value; }
        public Point[] DeserializedLocalPoints2 { get => deserializedLocalPoints; set => deserializedLocalPoints = value; }



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
            info.AddValue("LineColor", LineColor.ToArgb());
        }

        // Temp store for de-serialization.

        private Point[] deserializedLocalPoints = new Point[1];

        /// <summary>
        ///     Initializes a new instance of the <see cref="MapRoute" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected MapPolyLine(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            deserializedLocalPoints = Extensions.GetValue<Point[]>(info, "LocalPoints");
            IsVisible = Extensions.GetStruct(info, "Visible", true);
            LineColor = Color.FromArgb(Extensions.GetStruct(info, "LineColor", Color.MidnightBlue.ToArgb()));
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
        public override void OnDeserialization(object? sender)
        {
            base.OnDeserialization(sender);

            // Accounts for the de-serialization being breadth first rather than depth first.
            LocalPoints.AddRange(deserializedLocalPoints);
            LocalPoints.Capacity = Points.Count;
        }

        #endregion

        #region IDisposable Members

        bool _disposed;

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                ClearPoints();
                LocalPoints.Clear();
            }
        }

        #endregion


    } /* class GMapPolyLine */

        public delegate void PolygonClick(MapPolyLine item, MouseEventArgs e);
    public delegate void PolygonDoubleClick(MapPolyLine item, MouseEventArgs e);
    public delegate void PolygonEnter(MapPolyLine item);
    public delegate void PolygonLeave(MapPolyLine item);

    //public delegate void EllipseEnter(GMapEllipse item);
    //public delegate void EllipseLeave(GMapEllipse item);
    //public delegate void EllipseClick(GMapEllipse item, MouseEventArgs e);

    //public delegate void RectangleEnter(GMapRectangle item);
    //public delegate void RectangleLeave(GMapRectangle item);
    //public delegate void RectangleClick(GMapRectangle item, MouseEventArgs e);


}
