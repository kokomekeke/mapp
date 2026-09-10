/*=============================================================================
*  Layer in SgxMap
*
* (c) Kótai 2026.
*
* ver 1.00                                                         01-Sep-2026.
*          Last modification                                       04-Sep-2026.
=============================================================================*/
using System.Runtime.Serialization;
using GMap.NET;
using SgxMap.ObjectModel;
using SgxMap.Objects;
using SgxMap.Objects.ToolTips;
using static System.Net.Mime.MediaTypeNames;

namespace SgxMap
{
    public class Layer
    {


        private long m_id;
        private string m_name = string.Empty;
        bool m_isVisibile = true;
        bool m_isZoomSignificant = true;
        SgxMapPanel m_mapPanel;


        /// <summary>
        ///     list of objects, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<BasePoint> Objects =
            new ObservableCollectionThreadSafe<BasePoint>();


        public long ID
        {
            get { return m_id; }
            private set 
            { 
                m_id = value;
                if (m_id > BasePoint.LastID)
                    BasePoint.LastID = m_id;
            }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public bool IsVisible
        {
            get { return m_isVisibile; }
            set { m_isVisibile = value; }
        }



        /// <summary>
        ///     if false don't consider contained objects when box zooming
        /// </summary>
        public bool IsZoomSignificant
        {
            get { return m_isZoomSignificant; }
            set { m_isZoomSignificant = value; }
        }



        public SgxMapPanel MapPanel
        {
            get
            {
                return m_mapPanel;
            }
            internal set
            {
                m_mapPanel = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Constructor for Layer class. 
        /// Initializes a new instance of the Layer class with an optional name and control.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="control"></param>
        //---------------------------------------------------------------------
        public Layer(string? name, SgxMapPanel control) // = null)
        {
            m_id = BasePoint.LastID++;
            m_name = name ?? "layer-" + m_id;
            m_mapPanel = control;
        }


        public static Layer? CreateLayer(string[] ls, SgxMapPanel control, out string errorstring)
        {
            try
            {
                // 0: "Layer", 1: name

                errorstring = string.Empty;

                if (ls.Length < 2)
                {
                    errorstring = "Not enough parameters to create a layer.";
                    return null;
                }

                string name = ls[1];
                return new Layer(name, control);
            }
            catch (Exception ex)
            {
                errorstring = $"Error creating layer: {ex.Message}";
                return null;
            }
        }


        public void AddObject(BasePoint obj)
        {
            if (obj == null) return;
            Objects.Add(obj);
            obj.Layer = this;
            obj.UpdateObject();
        }

        public void RemoveObject(BasePoint obj)
        {
            if (obj == null) return;
            try
            {
                Objects.Remove(obj);
                obj.Layer = null;
            }
            catch { };
        }

        public void ClearObjects()
        {
            foreach (var obj in Objects)
            {
                obj.Layer = null;
            }
            Objects.Clear();
        }

        public void Clear()
        {
            ClearObjects();
            //ClearLinePoints();
        }


        public void RemoveLinePoint(MapLinePoint linePoint)
        {
            MapPanel.LinePoints.Remove(linePoint);
        }

        public void RemoveLinePoint(int i)
        {
            MapPanel.LinePoints.RemoveAt(i);
        }



        public Point? FromLatLngToLocalPosition(PointLatLng position)
        {
            if (MapPanel != null)
            {
                return MapPanel.FromLatLngToLocalPosition(position);
            }
            return null;
        }


        public BasePoint? GetBasePointbyName(string name)
        {
            return Objects.FirstOrDefault(obj => obj.Name == name);
        }

        public BasePoint? GetBasePointbyID(long id)
        {
            return Objects.FirstOrDefault(obj => obj.ID == id);
        }

        public BasePoint? GetBasePointbyPosition(PointLatLng position)
        {
            return Objects.FirstOrDefault(obj => obj.Position == position);
        }

        public BasePoint? GetObjectbyLocalPosition(int x, int y)
        {
            var p = new GPoint(x, y);
            p.OffsetNegative(MapPanel.Core.RenderOffset);
            return Objects.FirstOrDefault(obj => obj.IsInside((int)p.X, (int)p.Y));
        }


        public void ForceUpdate()
        {
            if (MapPanel != null)
            {
                foreach (var obj in Objects)
                {
                    if (obj.IsVisible)
                    {
                        obj.UpdateObject();     //UpdateLocalPosition();
                    }
                }

            }
        } /* ForceUpdate */


        /// <summary>
        ///     renders objects/routes/polygons
        /// </summary>
        /// <param name="g"></param>
        public virtual void OnRender(Graphics g)
        {
            if (MapPanel != null)
            {
                foreach (var o in Objects)
                {
                    if (o.IsVisible)
                       o.OnRender(g);
                
                }
               
            }
        }

        public void AddFrom(Layer overlay)
        {
            if (overlay != null)
            {
                foreach (var m in overlay.Objects)
                {
                    Objects.Add(m);
                }
            }
        }


        public void RefreshObjects()
        {
            foreach (var obj in Objects)
            {
                obj.UpdateObject();
            }

            foreach (var obj in MapPanel.LinePoints)
            {
                obj.UpdateObject();
            }


            if (MapPanel != null && !MapPanel.m_holdInvalidation)
            {
                MapPanel.Invalidate();
            }
        }

        public void RefreshObject(BasePoint obj)
        {
             obj.UpdateObject();

            if (MapPanel != null && !MapPanel.m_holdInvalidation)
            {
                MapPanel.Invalidate();
            }
        }


        #region Create and add objects to the layer


        public MapLine? AddLine(int x, int y, Color color)
        {
            var p1 = MapPanel.FromLocalToLatLng(x, y);
            if (!p1.IsEmpty)
            {
                var line = new MapLine(p1, p1, color);
                AddObject(line);
                return line;
            }
            return null;
        }

        public MapLine AddLine(string name, PointLatLng p1, PointLatLng p2, Color color)
        {
            var line = new MapLine(name, p1, p1, color);
            AddObject(line);
            return line;
        }


        public MapLabel AddLabel(string name, PointLatLng point, string text, System.Drawing.Font font)
        {
            var label = new MapLabel(name, point, text, font);
            AddObject(label);
            return label;
        }

        public MapTextBox AddTextBox(string name, PointLatLng point, string text, System.Drawing.Font font)
        {
            var label = new MapTextBox(name, point, text, font);
            AddObject(label);
            return label;
        }

        public MapTextBox? AddTextBox(string name, int x, int y, string text, System.Drawing.Font font)
        {
            var p1 = MapPanel.FromLocalToLatLng(x, y);
            if (!p1.IsEmpty)
            {
                var label = new MapTextBox(name, p1, text, font);
                AddObject(label);
                return label;
            }
            return null;
        }

        public MapBaseMarker? AddBaseMarker(int x, int y, int size, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            if (!p.IsEmpty)
            {
                var marker = new MapBaseMarker("", p, size, color);
                AddObject(marker);
                AddToolTip(marker, "Próba");
                return marker;
            }
            return null;
        }

        public MapBaseMarker? AddBaseMarker(string name, PointLatLng p, int size, Color color)
        {
            if (!p.IsEmpty)
            {
                var marker = new MapBaseMarker(name, p, size, color);
                AddObject(marker);
                AddToolTip(marker, "Próba");
                return marker;
            }
            return null;
        }


        public MapRuler AddRuler(int x, int y, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            var ruler = new MapRuler(p, p, color);
            AddObject(ruler);
            return ruler;
        }

        public MapCircleRuler AddCircleRuler(int x, int y, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            var ruler = new MapCircleRuler(p, p, color);
            AddObject(ruler);
            return ruler;
        }

        public MapRectangle AddRectangle(string name, PointLatLng point, float with, float height, Color color)
        {
            var rec = new MapRectangle(name, point, with, height, color);
            AddObject(rec);
            return rec;
        }

        public MapRectangle AddRectangle(int x, int y, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            var rec = new MapRectangle(p, 0.1f, 0.1f, color);
            AddObject(rec);
            return rec;
        }


        public MapRectangle AddEllipse(string name, PointLatLng point, float with, float height, Color color)
        {
            var rec = new MapEllipse(name, point, with, height, color);
            AddObject(rec);
            return rec;
        }

        public MapRectangle AddEllipse(int x, int y, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            var rec = new MapEllipse(p, 0.1f, 0.1f, color);
            AddObject(rec);
            return rec;
        }

        public GeoCircle AddCircle(int x, int y, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            var rec = new GeoCircle(p, 0.1, color);
            AddObject(rec);
            return rec;
        }

        public MapRayLine AddRayLine(int x, int y, Color color)
        {
            var p = MapPanel.FromLocalToLatLng(x, y);
            var rec = new MapRayLine(p, 35, 10, color);
            AddObject(rec);
            return rec;
        }


        public MapLinePoint AddLinePoint(BasePoint parent, PointLatLng point, MovementType movement,  Color color)
        {
            var p = new MapLinePoint("", parent, point, color, movement);
            MapPanel.AddLinePoint(this, p);
            return p;
        }

        public MapToolTip AddToolTip(MapToolTip toolTip)
        {
            AddObject(toolTip);
            return toolTip;
        }

        public MapToolTip AddToolTip(MapBaseMarker marker, string text)
        {
            var t = marker.AddToolTip();
            marker.ToolTipText = text;
            AddObject(t);
            return t;
        }


        #endregion Create and add objects to the layer


        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", ID);
            info.AddValue("Name", Name);
            info.AddValue("IsVisible", IsVisible);

            var objectArray = new BasePoint[Objects.Count];
            Objects.CopyTo(objectArray, 0);
            info.AddValue("Objects", objectArray);
        }

        protected Layer(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64("Id");
            Name = info.GetString("Name") ?? string.Empty;
            IsVisible = info.GetBoolean("IsVisible");
            var objectArray = (BasePoint[])info.GetValue("Objects", typeof(BasePoint[]))!;
            foreach (var obj in objectArray)
            {
                AddObject(obj);
            }
        }

        #endregion ISerializable Members

        #region IDisposable Members

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                //ClearEvents();

                foreach (var m in Objects)
                {
                    m.Dispose();
                }

                ClearObjects();

            }
        }

        #endregion IDisposable Members





    } /* class Layer */
} /* namespace SgxMap */

