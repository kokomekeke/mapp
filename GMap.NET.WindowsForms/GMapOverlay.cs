using System;
using System.Drawing;
using System.Runtime.Serialization;
using GMap.NET.ObjectModel;
using GMap.NET.WindowsForms.Markers;

namespace GMap.NET.WindowsForms
{
    /// <summary>
    ///     GMap.NET overlay
    /// </summary>
    [Serializable]

    public class GMapOverlay : ISerializable, IDeserializationCallback, IDisposable
    {
        bool _isVisibile = true;

        /// <summary>
        ///     is overlay visible
        /// </summary>
        public bool IsVisibile
        {
            get
            {
                return _isVisibile;
            }
            set
            {
                if (value != _isVisibile)
                {
                    _isVisibile = value;

                    if (Control != null)
                    {
                        if (_isVisibile)
                        {
                            Control.HoldInvalidation = true;
                            {
                                ForceUpdate();
                            }
                            Control.Refresh();
                        }
                        else
                        {
                            if (Control.IsMouseOverMarker)
                            {
                                Control.IsMouseOverMarker = false;
                            }

                            if (Control.IsMouseOverPolygon)
                            {
                                Control.IsMouseOverPolygon = false;
                            }

                            if (Control.IsMouseOverRoute)
                            {
                                Control.IsMouseOverRoute = false;
                            }

                            Control.RestoreCursorOnLeave();

                            if (!Control.HoldInvalidation)
                            {
                                Control.Invalidate();
                            }
                        }
                    }
                }
            }
        }

        bool _isHitTestVisible = true;

        /// <summary>
        ///     HitTest visibility for entire overlay
        /// </summary>
        public bool IsHitTestVisible
        {
            get { return _isHitTestVisible; }
            set { _isHitTestVisible = value; }
        }

        bool _isZoomSignificant = true;

        /// <summary>
        ///     if false don't consider contained objects when box zooming
        /// </summary>
        public bool IsZoomSignificant
        {
            get { return _isZoomSignificant; }
            set { _isZoomSignificant = value; }
        }

        /// <summary>
        ///     overlay Id
        /// </summary>
        public string Id;

        /// <summary>
        ///     list of markers, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapMarker> Markers =
            new ObservableCollectionThreadSafe<GMapMarker>();

        /// <summary>
        ///     list of routes, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapRoute> Routes =
            new ObservableCollectionThreadSafe<GMapRoute>();

        /// <summary>
        ///     list of polygons, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapPolyLine> Polygons =
            new ObservableCollectionThreadSafe<GMapPolyLine>();

        /// <summary>
        ///     list of lines, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapLine> Lines =
            new ObservableCollectionThreadSafe<GMapLine>();

        /// <summary>
        ///     list of lines, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapRay> Rays =
            new ObservableCollectionThreadSafe<GMapRay>();

        /// <summary>
        ///     list of labels, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapLabel> Labels =
            new ObservableCollectionThreadSafe<GMapLabel>();

        /// <summary>
        ///     list of labels, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapScaleLabel> ScaleLabels =
            new ObservableCollectionThreadSafe<GMapScaleLabel>();

        /// <summary>
        ///     list of ellipses, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapEllipse> Ellipses =
            new ObservableCollectionThreadSafe<GMapEllipse>();

        /// <summary>
        ///     list of rectangle, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<GMapRectangle> Rectangles =
            new ObservableCollectionThreadSafe<GMapRectangle>();


        GMapControl _control;

        public GMapControl Control
        {
            get
            {
                return _control;
            }
            internal set
            {
                _control = value;
            }
        }

        public GMapOverlay()
        {
            CreateEvents();
        }

        public GMapOverlay(string id)
        {
            Id = id;
            CreateEvents();
        }

        void CreateEvents()
        {
            Markers.CollectionChanged += Markers_CollectionChanged;
            Routes.CollectionChanged += Routes_CollectionChanged;
            Polygons.CollectionChanged += Polygons_CollectionChanged;   
            Lines.CollectionChanged += Lines_CollectionChanged;   
            Rays.CollectionChanged += DFLines_CollectionChanged;   
            Labels.CollectionChanged += Labels_CollectionChanged;
            ScaleLabels.CollectionChanged += ScaleLabels_CollectionChanged; 
            Ellipses.CollectionChanged += Ellipses_CollectionChanged;
            Rectangles.CollectionChanged += Rectangles_CollectionChanged;
        }

        void ClearEvents()
        {
            Markers.CollectionChanged -= Markers_CollectionChanged;
            Routes.CollectionChanged -= Routes_CollectionChanged;
            Polygons.CollectionChanged -= Polygons_CollectionChanged;
            Lines.CollectionChanged -= Lines_CollectionChanged;
            Rays.CollectionChanged -= DFLines_CollectionChanged;
            Labels.CollectionChanged -= Labels_CollectionChanged;
            ScaleLabels.CollectionChanged -= ScaleLabels_CollectionChanged;
            Ellipses.CollectionChanged -= Ellipses_CollectionChanged;
            Rectangles.CollectionChanged -= Rectangles_CollectionChanged;
        }

        public void Clear()
        {
            Markers.Clear();
            Routes.Clear();
            Polygons.Clear();
            Lines.Clear();
            Rays.Clear();
            Labels.Clear();
            Ellipses.Clear();
            Rectangles.Clear();
        }


        public void AddFrom(GMapOverlay overlay)
        {
            if (overlay != null)
            {
                foreach (var m in overlay.Markers)
                {
                    Markers.Add(m);
                }
                foreach (var r in overlay.Routes)
                {
                    Routes.Add(r);
                }
                foreach (var p in overlay.Polygons)
                {
                    Polygons.Add(p);
                }
                foreach (var l in overlay.Lines)
                {
                    Lines.Add(l);
                }
                foreach (var l in overlay.Rays)
                {
                    Rays.Add(l);
                }
                foreach (var l in overlay.Labels)
                {
                    Labels.Add(l);
                }
                foreach (var l in overlay.Ellipses)
                {
                    Ellipses.Add(l);
                }
                foreach (var l in overlay.Rectangles)
                {
                    Rectangles.Add(l);
                }
            }
        }

        void Lines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapLine obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateLineLocalPosition(obj);
                        }
                    }
                }
            }
            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    //if (Control.IsMouseOverLine)
                    //{
                    //    Control.IsMouseOverLine = false;
                    //    Control.RestoreCursorOnLeave();
                    //}
                }
                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        void DFLines_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapRay obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateDFLineLocalPosition(obj);
                        }
                    }
                }
            }
            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    //if (Control.IsMouseOverLine)
                    //{
                    //    Control.IsMouseOverLine = false;
                    //    Control.RestoreCursorOnLeave();
                    //}
                }
                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        } /* DFLines_CollectionChanged */




        void Labels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapLabel obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateLabelLocalPosition(obj);
                        }
                    }
                }
            }
            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    //if (Control.IsMouseOverLine)
                    //{
                    //    Control.IsMouseOverLine = false;
                    //    Control.RestoreCursorOnLeave();
                    //}
                }
                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }


        void ScaleLabels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapScaleLabel obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateScaleLabelLocalPosition(obj);
                        }
                    }
                }
            }
            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    //..
                }
                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        void Polygons_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapPolyLine obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdatePolygonLocalPosition(obj);
                        }
                    }
                }
            }

            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (Control.IsMouseOverPolygon)
                    {
                        Control.IsMouseOverPolygon = false;
                        Control.RestoreCursorOnLeave();
                    }
                }

                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        void Routes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapRoute obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateRouteLocalPosition(obj);
                        }
                    }
                }
            }

            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (Control.IsMouseOverRoute)
                    {
                        Control.IsMouseOverRoute = false;
                        Control.RestoreCursorOnLeave();
                    }
                }

                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        void Markers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapMarker obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        if (Control != null)
                        {
                            Control.UpdateMarkerLocalPosition(obj);
                        }
                    }
                }
            }

            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (Control.IsMouseOverMarker)
                    {
                        Control.IsMouseOverMarker = false;
                        Control.RestoreCursorOnLeave();
                    }
                }

                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        void Ellipses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapEllipse obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        obj.UpdateLocalPosition();
                        if ((Control != null) && !Control.HoldInvalidation)
                        {
                            Control.Invalidate();
                        }
                    }
                }
            }

            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (Control.IsMouseOverEllipse)
                    {
                        Control.IsMouseOverEllipse = false;
                        Control.RestoreCursorOnLeave();
                    }
                }

                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        void Rectangles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GMapRectangle obj in e.NewItems)
                {
                    if (obj != null)
                    {
                        obj.Overlay = this;
                        obj.UpdateLocalPosition();
                        if ((Control != null) && !Control.HoldInvalidation)
                        {
                            Control.Invalidate();
                        }
                    }
                }
            }

            if (Control != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (Control.IsMouseOverRectangle)
                    {
                        Control.IsMouseOverRectangle = false;
                        Control.RestoreCursorOnLeave();
                    }
                }

                if (!Control.HoldInvalidation)
                {
                    Control.Invalidate();
                }
            }
        }

        /// <summary>
        ///     updates local positions of objects
        /// </summary>
        internal void ForceUpdate()
        {
            if (Control != null)
            {
                foreach (var obj in Markers)
                {
                    if (obj.IsVisible)
                    {
                        Control.UpdateMarkerLocalPosition(obj);
                    }
                }

                foreach (var obj in Polygons)
                {
                    if (obj.IsVisible)
                    {
                        Control.UpdatePolygonLocalPosition(obj);
                    }
                }

                foreach (var obj in Routes)
                {
                    if (obj.IsVisible)
                    {
                        Control.UpdateRouteLocalPosition(obj);
                    }
                }

                foreach (var obj in Lines)
                {
                    Control.UpdateLineLocalPosition(obj);
                }

                foreach (var obj in Rays)
                {
                    Control.UpdateDFLineLocalPosition(obj);
                }


                foreach (var obj in Labels)
                {
                    Control.UpdateLabelLocalPosition(obj);
                }

                foreach (var obj in Ellipses)
                {
                    obj.UpdateLocalPosition();
                    //Control.UpdateEllipseLocalPosition(obj);
                }
            }
        }

        /// <summary>
        ///     renders objects/routes/polygons
        /// </summary>
        /// <param name="g"></param>
        public virtual void OnRender(Graphics g)
        {
            if (Control != null)
            {
                if (Control.RoutesEnabled)
                {
                    foreach (var r in Routes)
                    {
                        if (r.IsVisible)
                        {
                            r.OnRender(g);
                        }
                    }
                }

                if (Control.PolygonsEnabled)
                {
                    foreach (var r in Polygons)
                    {
                        if (r.IsVisible)
                        {
                            r.OnRender(g);
                        }
                    }
                }

                if (Control.LinesEnabled)
                {
                    foreach (var r in Lines)
                    {
                       r.OnRender(g);
                    }

                    foreach (var r in Rays)
                    {
                        r.OnRender(g);
                    }

                }


                if (Control.LabelEnabled)
                {
                    foreach (var r in Labels)
                    {
                        r.OnRender(g);
                    }
                }


                if (Control.EllipsesEnabled)
                {
                    // ellipses
                    foreach (var e in Ellipses)
                    {
                        if (e.IsVisible)
                        {
                            e.OnRender(g);
                        }
                    }
                }

                if (Control.RectanglesEnabled)
                {
                    // ellipses
                    foreach (var e in Rectangles)
                    {
                        if (e.IsVisible)
                        {
                            e.OnRender(g);
                        }
                    }
                }



                if (Control.MarkersEnabled)
                {
                    // markers
                    foreach (var m in Markers)
                    {
                        //if(m.IsVisible && (m.DisableRegionCheck || Control.Core.currentRegion.Contains(m.LocalPosition.X, m.LocalPosition.Y)))
                        if (m.IsVisible || m.DisableRegionCheck)
                        {
                            m.OnRender(g);
                        }
                    }

                    // ToolTips are drawn in separate method to ensure they are top most
                }
            }
        }


        /// <summary>
        ///     renders objects/routes/polygons
        /// </summary>
        /// <param name="g"></param>
        public virtual void OnRenderScale(Graphics g)
        {
            if (Control != null)
            {
                if (Control.LabelEnabled)                       //.. TODO: add separate flag for scale labels
                {
                    foreach (var r in ScaleLabels)
                    {
                        r.OnRender(g);
                    }
                }
            }
        }




        public virtual void OnRenderToolTips(Graphics g)
        {
            if (Control != null)
            {
                if (Control.MarkersEnabled)
                {
                    // tooltips of markers 
                    foreach (var m in Markers)
                    {
                        //if(m.ToolTip != null && m.IsVisible && Control.Core.currentRegion.Contains(m.LocalPosition.X, m.LocalPosition.Y))
                        if (m.ToolTip != null && m.IsVisible)
                        {
                            if (!string.IsNullOrEmpty(m.ToolTipText) &&
                                (m.ToolTipMode == MarkerTooltipMode.Always ||
                                 m.ToolTipMode == MarkerTooltipMode.OnMouseOver && m.IsMouseOver))
                            {
                                m.ToolTip.OnRender(g);
                            }
                        }
                    }

                    // tooltips of lines 
                    foreach (var m in Lines)
                    {
                        //if(m.ToolTip != null && m.IsVisible && Control.Core.currentRegion.Contains(m.LocalPosition.X, m.LocalPosition.Y))
                        if (m.ToolTip != null && m.IsVisible)
                        {
                            if (!string.IsNullOrEmpty(m.ToolTipText) &&
                                (m.ToolTipMode == MarkerTooltipMode.Always ||
                                 m.ToolTipMode == MarkerTooltipMode.OnMouseOver && m.IsMouseOver))
                            {
                                m.ToolTip.OnRender(g);
                            }
                        }
                    }

                    // tooltips of lines 
                    foreach (var m in Rays)
                    {
                        //if(m.ToolTip != null && m.IsVisible && Control.Core.currentRegion.Contains(m.LocalPosition.X, m.LocalPosition.Y))
                        if (m.ToolTip != null && m.IsVisible)
                        {
                            if (!string.IsNullOrEmpty(m.ToolTipText) &&
                                (m.ToolTipMode == MarkerTooltipMode.Always ||
                                 m.ToolTipMode == MarkerTooltipMode.OnMouseOver && m.IsMouseOver))
                            {
                                m.ToolTip.OnRender(g);
                            }
                        }
                    }


                }
            }
        } /* OnRenderToolTips */

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
            info.AddValue("Id", Id);
            info.AddValue("IsVisible", IsVisibile);

            var markerArray = new GMapMarker[Markers.Count];
            Markers.CopyTo(markerArray, 0);
            info.AddValue("Markers", markerArray);

            var routeArray = new GMapRoute[Routes.Count];
            Routes.CopyTo(routeArray, 0);
            info.AddValue("Routes", routeArray);

            var polygonArray = new GMapPolyLine[Polygons.Count];
            Polygons.CopyTo(polygonArray, 0);
            info.AddValue("Polygons", polygonArray);

            var linesArray = new GMapLine[Lines.Count];
            Lines.CopyTo(linesArray, 0);
            info.AddValue("Lines", linesArray);

            var rayArray = new GMapRay[Rays.Count];
            Rays.CopyTo(rayArray, 0);
            info.AddValue("Rays", rayArray);

            var labelArray = new GMapLabel[Labels.Count];
            Labels.CopyTo(labelArray, 0);
            info.AddValue("Labels", labelArray);

            // TODO: lines, rays, labels
        }

        private GMapMarker[] deserializedMarkerArray;
        private GMapRoute[] deserializedRouteArray;
        private GMapPolyLine[] deserializedPolygonArray;
        private GMapLine[] deserializedLineArray;
        private GMapRay[] deserializedRayArray;
        private GMapLabel[] deserializedLabelArray;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GMapOverlay" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GMapOverlay(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            IsVisibile = info.GetBoolean("IsVisible");

            deserializedMarkerArray = Extensions.GetValue(info, "Markers", new GMapMarker[0]);
            deserializedRouteArray = Extensions.GetValue(info, "Routes", new GMapRoute[0]);
            deserializedPolygonArray = Extensions.GetValue(info, "Polygons", new GMapPolyLine[0]);
            deserializedLineArray = Extensions.GetValue(info, "Lines", new GMapLine[0]);
            deserializedRayArray = Extensions.GetValue(info, "Rays", new GMapRay[0]);
            deserializedLabelArray = Extensions.GetValue(info, "Labels", new GMapLabel[0]);


            CreateEvents();
        }

        #endregion

        #region IDeserializationCallback Members

        /// <summary>
        ///     Runs when the entire object graph has been deserialized.
        /// </summary>
        /// <param name="sender">
        ///     The object that initiated the callback. The functionality for this parameter is not currently
        ///     implemented.
        /// </param>
        public void OnDeserialization(object sender)
        {
            // Populate Markers
            foreach (var marker in deserializedMarkerArray)
            {
                marker.Overlay = this;
                Markers.Add(marker);
            }

            // Populate Routes
            foreach (var route in deserializedRouteArray)
            {
                route.Overlay = this;
                Routes.Add(route);
            }

            // Populate Polygons
            foreach (var polygon in deserializedPolygonArray)
            {
                polygon.Overlay = this;
                Polygons.Add(polygon);
            }

            // Populate Lines
            foreach (var line in deserializedLineArray)
            {
                line.Overlay = this;
                Lines.Add(line);
            }

            // Populate Rays
            foreach (var ray in deserializedRayArray)
            {
                ray.Overlay = this;
                Rays.Add(ray);
            }

            // Populate Labels
            foreach (var label in deserializedLabelArray)
            {
                label.Overlay = this;
                Labels.Add(label);
            }
        }

        #endregion

        #region IDisposable Members

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                ClearEvents();

                foreach (var m in Markers)
                {
                    m.Dispose();
                }

                foreach (var r in Routes)
                {
                    r.Dispose();
                }

                foreach (var p in Polygons)
                {
                    p.Dispose();
                }

                foreach (var p in Lines)
                {
                    p.Dispose();
                }

                foreach (var p in Rays)
                {
                    p.Dispose();
                }

                foreach (var p in Labels)
                {
                    p.Dispose();
                }

                Clear();
            }
        }

        #endregion
    }
}
