/*=============================================================================
*  MapPolyLine: polyline on map
*
* (c) Kótai 2026.
*
* ver 1.00                                                         05-Sep-2026.
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
        /// <summary>
        ///     background color
        /// </summary>
        //[NonSerialized] public Brush Fill = new SolidBrush(Color.FromArgb(155, Color.AliceBlue));

        public readonly List<PointLatLng> Points = new List<PointLatLng>(); // points of route in geo coordinates
        public readonly List<Point> LocalPoints = new List<Point>();      // List of points in map  (x,y)



        private Color m_lastcolor = Color.Black;
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


        public MapPolyLine()
            : base()
        {
            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 5;
        }

        public MapPolyLine(string name, List<PointLatLng> points)
            : base(name, points, Color.Black)
        {
            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 5;
            LocalPoints.Capacity = Points.Count;
        }

        public MapPolyLine(string name, List<PointLatLng> points, Color color )
            : base(name, points, color)
        {
            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 5;
            LocalPoints.Capacity = Points.Count;
            Stroke.SetLineCap(LineCap.NoAnchor, LineCap.ArrowAnchor, DashCap.Flat);
            LastColor = Stroke.Color;
        }

        public MapPolyLine(string name, PointLatLng point, Color color)
                : base(name, point, color)
        {
            Points.Add(point);
            Stroke.LineJoin = LineJoin.Round;
            Stroke.Width = 5;
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


        /// <summary>
        ///     clears points and sets tag and name to null
        /// </summary>
        public void Clear()
        {
            Points.Clear();
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


        /// <summary>
        ///     Indicates whether the specified point is contained within this System.Drawing.Drawing2D.GraphicsPath
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal bool IsInsideLocal(int x, int y)
        {
            if (m_graphicsPath != null)
            {
                return m_graphicsPath.IsVisible(x, y);
            }

            return false;
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

                    if (Points.Count > 2)
                         g.DrawPath(Stroke, m_graphicsPath);
                    else g.DrawLine(Stroke, LocalPoints[0].X, LocalPoints[0].Y, LocalPoints[1].X, LocalPoints[1].Y);
                }
            }
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
                {
                    if (p1.Lng < p.Lng && p2.Lng >= p.Lng || p2.Lng < p.Lng && p1.Lng >= p.Lng)
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
            if (!m_isEditable || point.IsEmpty)
                return;

            Points.Add(point);
            LocalPoints.Capacity = Points.Count;

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


        public void MovePoint(int index, PointLatLng point)
        {
            if (!m_isEditable || point.IsEmpty)
                return;

            if (index >= 0 && index < Points.Count)
            {
                Points[index] = point;
                if (IsSelected)
                {
                    //for (int i = Layer.Markers.Count - 1; i >= 0; i--)
                    //{
                    //    if (Layer.Markers[i].Parent == this && ((GMap.NET.WindowsForms.Markers.GMarkerLinePoint)Layer.Markers[i]).Position == point)
                    //    {
                    //        ((GMap.NET.WindowsForms.Markers.GMarkerLinePoint)Layer.Markers[i]).Position = point;
                    //    }
                    //}
                }
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
        }

        public void RemovePoint(PointLatLng point)
        {
            if (!m_isEditable || !point.IsEmpty)
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
            }
        }

        public void ShowLength()
        {
            if (Points.Count == 0)
                return;

            //if (label == null)
            //{
            //    label = new GMapTextBox(Points[Points.Count - 1], Distance.ToString("F1") + " km ");
            //    label.Layer = this.Layer;
            //    label.Font = new Font("Arial", 8);
            //}
            //else label.Text = Distance.ToString("F1") + " km ";
            //label.UpdateLocalPosition();
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
            Layer.RefreshObject(this);
        }


        public override void Shift(PointLatLng oldpoint, PointLatLng newpoint)
        {
            var b = Layer.MapPanel.GetBearing2(oldpoint, newpoint);
            var d = Layer.MapPanel.GetDistance(oldpoint, newpoint);

            for (int i = 0; i < Points.Count; i++)
                Points[i] = Layer.MapPanel.GetPositionFromPoint(Points[i], d, b);

            Layer.RefreshObject(this);
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
