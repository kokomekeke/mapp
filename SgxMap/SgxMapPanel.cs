/*=============================================================================
*  SgxMapPanel: drawing panel for SgxMap. (Main class of SgxMap)
*
* (c) Kótai 2026.
*
* ver 1.00                                                         01-Sep-2026.
*          Last modification                                       12-Sep-2026.
=============================================================================*/
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Security.Permissions;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using SgxMap.ObjectModel;
using SgxMap.Shapes;


namespace SgxMap
{

    #pragma warning disable SYSLIB0011

    /// <summary>
    ///     GMap.NET control for Windows Forms
    /// </summary>
    public partial class SgxMapPanel : UserControl, Interface
    {
        #region Fields

        public bool m_showCenter = true;
        public bool m_show_distance = false;
        private bool m_showTileGridLines = false;
        private bool m_showGeoGridLines = false;
        private HelperLineOptions m_helperLineOption = HelperLineOptions.DontShow;
        public bool m_showHelperLine = false;                                               // if true, draw cross lines at the mouse pointer position
        public bool m_showMapScaleInfo = false;                                             // if true, then show map scale info
        public bool m_showPlaceInfo = false;                                                // if true, then show place info(address) of given location 
        public MapScaleInfoPositions m_mapScaleInfoPosition;                                // Position of the map scale info

        //.. selecting area
        private RectLatLng m_selectedArea;                                                  // current selected area in map
        bool m_isSelecting = false;                                                         // true, when selection of an area is in progress
        bool m_isDragging = false;                                                          // true, when dragging is in progress
        PointLatLng m_selectionStart;
        PointLatLng m_selectionEnd;
        public bool m_selectionAlways = true;                                               // if true, selects area just by holding mouse and moving (does not check for alt os shift key)


        double      m_zoomReal;
        Bitmap?     m_backBuffer;
        Graphics?   m_gxOff;

        public Pen ScalePen = new Pen(Color.Black, 3);                                      // pen for scale info
        public Pen ScalePenBorder = new Pen(Color.WhiteSmoke, 6);
        public Pen CenterPen = new Pen(Brushes.Red, 1);
        public Pen SelectionPen = new Pen(Brushes.Blue, 2);                                 // area selection pen
        public Pen HelperLinePen = new Pen(Color.Blue, 1);                                  // pen for helper lines
        public Pen GeoLinePen = new Pen(Color.FromArgb(100, Color.Blue), 1);                // pen for geographical grid lines
        public Brush GeoTextColor = new SolidBrush(Color.FromArgb(100, Color.Black));       // brush for geographical grid lines text 
        public Font GeotextFont = new Font("Arial", 12);
        Brush m_selectedAreaFill = new SolidBrush(Color.FromArgb(33, Color.RoyalBlue));
        Color m_selectedAreaFillColor = Color.FromArgb(33, Color.RoyalBlue);


        [Category("GMap.NET")] public MouseButtons DragButton = MouseButtons.Left;          // mouse drag button
        [Category("GMap.NET")] public MouseButtons SelectionButton = MouseButtons.Right;    // mouse selection button



        /// <summary>
        ///     enables integrated DoubleBuffer for running on windows mobile
        /// </summary>
        public bool m_forceDoubleBuffer;


        /// <summary>
        ///     stops immediate marker/route/polygon invalidations;
        ///     call Refresh to perform single refresh and reset invalidation state
        /// </summary>
        public bool m_holdInvalidation;


        bool m_lazyEvents = true;
        RectLatLng? m_lazySetZoomToFitRect;
        readonly ImageAttributes m_tileFlipXYAttributes = new ImageAttributes();

        /// <summary>
        ///     list of overlays, should be thread safe
        /// </summary>
        public readonly ObservableCollectionThreadSafe<Layer> Layers =
            new ObservableCollectionThreadSafe<Layer>();

        //.. creating of objects on map is in progress
        public MouseMoveStatusType MouseMoveStatus = MouseMoveStatusType.None;
        public MapRuler? m_MapRuler = null;
        public MapRuler? m_MapCircleRuler = null;
        public MapLine? m_MapLine = null;
        public MapRectangle? m_MapRectangle = null;
        public GeoCircle? m_MapCircle = null;
        public MapBaseMarker? m_MapMarker = null;
        public MapRayLine? m_MapRayLine = null;
        public MapTextBox? m_MapTextBox = null;

        //.. selected objects
        public BasePoint? m_selected_object = null; 
        public MapLinePoint? m_selected_linepoint = null;
        public readonly ObservableCollectionThreadSafe<MapLinePoint> LinePoints =
            new ObservableCollectionThreadSafe<MapLinePoint>();                         // list of line points of selected lines, should be thread safe
        public PointLatLng m_lastpoint;

        #endregion Fields

        #region EVENTS

        /// <summary>
        ///     occurs when an exception is thrown inside the map control
        /// </summary>
        public event ExceptionThrown OnExceptionThrown;
        public ShowPlaceInfo OnShowPlaceInfo;
        public ShowPlaceInfoEx OnShowPlaceInfoEx;

        #endregion EVENTS


        #region Properties

        public string ProjectionName
        {
            get
            {
                return Core.ProjectionName;
            }
        }

        #region Show
        public bool ShowCenter
        {
            get { return m_showCenter; }
            set
            {
                if (value != m_showCenter)
                {
                    m_showCenter = value;
                    Invalidate();
                }
            }
        }


        public bool ShowDistance
        {
            get { return m_show_distance; }
            set { m_show_distance = value; }
        }

        /// <summary>
        ///     shows tile gridlines
        /// </summary>
        [Category("GMap.NET")]
        [Description("shows tile gridlines")]
        public bool ShowTileGridLines
        {
            get
            {
                return m_showTileGridLines;
            }
            set
            {
                m_showTileGridLines = value;
                Invalidate();
            }
        }


        /// <summary>
        ///     shows geographical gridlines
        /// </summary>
        [Category("GMap.NET")]
        [Description("shows geographical gridlines")]
        public bool ShowGeoGridLines
        {
            get
            {
                return m_showGeoGridLines;
            }
            set
            {
                m_showGeoGridLines = value;
                Invalidate();
            }
        }


        /// <summary>
        ///     draw cross lines at the mouse pointer position
        /// </summary>
        //[Browsable(false)]
        [Category("GMap.NET")]
        [Description("draw cross lines at the mouse pointer position")]
        public HelperLineOptions HelperLineOption
        {
            get
            {
                return m_helperLineOption;
            }
            set
            {
                m_helperLineOption = value;
                m_showHelperLine = m_helperLineOption == HelperLineOptions.ShowAlways;
                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }


        [Category("GMap.NET")]
        [Description("Show map scale line")]
        public bool ShowMapScaleInfo
        {
            get
            {
                return m_showMapScaleInfo;
            }
            set
            {
                m_showMapScaleInfo = value;
                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }

        [Category("GMap.NET")]
        [Description("Position of the map scale information")]
        public MapScaleInfoPositions MapScaleInfoPosition
        {
            get
            {
                return m_mapScaleInfoPosition;
            }
            set
            {
                m_mapScaleInfoPosition = value;
                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///     background of selected area
        /// </summary>
        [Category("GMap.NET")]
        [Description("background color of the selected area")]
        public Color SelectedAreaFillColor
        {
            get
            {
                return m_selectedAreaFillColor;
            }
            set
            {
                if (m_selectedAreaFillColor != value)
                {
                    m_selectedAreaFillColor = value;
                    m_selectedAreaFill = new SolidBrush(m_selectedAreaFillColor);
                }
            }
        }

        /// <summary>
        ///     is user dragging map
        /// </summary>
        [Browsable(false)]
        public bool IsDragging
        {
            get
            {
                return m_isDragging;
            }
        }


        #endregion SHOW

        #region ZOOM

        /// <summary>
        ///     enable map zoom on mouse wheel
        /// </summary>
        [Category("GMap.NET")]
        [Description("enable map zoom on mouse wheel")]
        public bool MouseWheelZoomEnabled
        {
            get
            {
                return Core.MouseWheelZoomEnabled;
            }
            set
            {
                Core.MouseWheelZoomEnabled = value;
            }
        }

        /// <summary>
        ///     max zoom
        /// </summary>
        [Category("GMap.NET")]
        [Description("maximum zoom level of map")]
        public int MaxZoom
        {
            get
            {
                return Core.MaxZoom;
            }
            set
            {
                Core.MaxZoom = value;
            }
        }

        /// <summary>
        ///     min zoom
        /// </summary>
        [Category("GMap.NET")]
        [Description("minimum zoom level of map")]
        public int MinZoom
        {
            get
            {
                return Core.MinZoom;
            }
            set
            {
                Core.MinZoom = value;
            }
        }

        /// <summary>
        ///     map zooming type for mouse wheel
        /// </summary>
        [Category("GMap.NET")]
        [Description("map zooming type for mouse wheel")]
        public MouseWheelZoomType MouseWheelZoomType
        {
            get
            {
                return Core.MouseWheelZoomType;
            }
            set
            {
                Core.MouseWheelZoomType = value;
            }
        }

        #endregion ZOOM


        [Browsable(false)]
        public RectLatLng SelectedArea
        {
            get
            {
                return m_selectedArea;
            }
            set
            {
                m_selectedArea = value;

                if (Core.IsStarted)
                {
                    Invalidate();
                }
            }
        }


        /// <summary>
        ///     can user drag map
        /// </summary>
        [Category("GMap.NET")]
        public bool CanDragMap
        {
            get
            {
                return Core.CanDragMap;
            }
            set
            {
                Core.CanDragMap = value;
            }
        }

        /// <summary>
        ///     map render mode
        /// </summary>
        [Browsable(false)]
        public RenderMode RenderMode
        {
            get
            {
                return Core.RenderMode;
            }
            internal set
            {
                Core.RenderMode = value;
            }
        }

        /// <summary>
        ///     gets map manager
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GMaps Manager
        {
            get
            {
                return GMaps.Instance;
            }
        }


        public BasePoint? SelectedObject
        {
            get { return m_selected_object; }
            set
            {
                if (value != m_selected_object)
                {
                    m_selected_object = value;
                }
            }
        }

        public MapLinePoint? SelectedLinePoint
        {
            get { return m_selected_linepoint; }
            set
            {
                if (value != m_selected_linepoint)
                {
                    m_selected_linepoint = value;
                }
            }
        }


        #endregion Properties


        /*------------------------------------------------------------------------------------------------*/

        #region Constructors
#if !DESIGN
        /// <summary>
        ///     constructor
        /// </summary>
        public SgxMapPanel()
        {
            if (!IsDesignerHosted)
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.Opaque, true);
                ResizeRedraw = true;

                m_tileFlipXYAttributes.SetWrapMode(WrapMode.TileFlipXY);

                // only one mode will be active, to get mixed mode create new ColorMatrix
                Core.SystemType = "WindowsForms";

                RenderMode = RenderMode.GDI_PLUS;

                CenterFormat.Alignment = StringAlignment.Center;
                CenterFormat.LineAlignment = StringAlignment.Center;

                BottomFormat.Alignment = StringAlignment.Center;

                BottomFormat.LineAlignment = StringAlignment.Far;

                if (GMaps.Instance.IsRunningOnMono)
                {
                    // no imports to move pointer
                    MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
                }

                AddLayer("DEFAULT");
            }
        }

#endif

        static SgxMapPanel()
        {
            if (!IsDesignerHosted)
            {
                GMapImageProxy.Enable();
                GMaps.Instance.SQLitePing();
            }
        }

        #endregion Constructors


        #region ZOOM AND CENTER

        /// <summary>
        /// <summary>
        ///     sets zoom to max to fit rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool SetZoomToFitRect(RectLatLng rect)
        {
            if (m_lazyEvents)
            {
                m_lazySetZoomToFitRect = rect;
            }
            else
            {
                int maxZoom = Core.GetMaxZoomToFitRect(rect);
                if (maxZoom > 0)
                {
                    var center = new PointLatLng(rect.Lat - rect.HeightLat / 2, rect.Lng + rect.WidthLng / 2);
                    Position = center;

                    if (maxZoom > MaxZoom)
                    {
                        maxZoom = MaxZoom;
                    }

                    if ((int)Zoom != maxZoom)
                    {
                        Zoom = maxZoom;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     sets to max zoom to fit all markers and centers them in map
        /// </summary>
        /// <param name="overlayId">overlay id or null to check all</param>
        /// <returns></returns>
        public bool ZoomAndCenterObjects(string overlayId)
        {
            var rect = GetRectOfAllMarkers(overlayId);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }


        public RectLatLng? GetRectOfAllObjects(string name)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            foreach (var l in Layers)
            {
                if (string.IsNullOrEmpty(name) && l.IsZoomSignificant || l.Name == name)
                {
                    if (l.IsVisible && l.Shapes.Count > 0)
                    {
                        foreach (var o in l.Shapes)
                        {
                            if (o.IsVisible)
                            {
                                // left
                                if (o.Position.Lng < left)
                                    left = o.Position.Lng;

                                // top
                                if (o.Position.Lat > top)
                                    top = o.Position.Lat;

                                // right
                                if (o.Position.Lng > right)
                                    right = o.Position.Lng;

                                // bottom
                                if (o.Position.Lat < bottom)
                                    bottom = o.Position.Lat;
                            }
                        }
                    }
                }
            } /* foreach (var l in Layers) */

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue &&
                bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        } /* GetRectOfAllMarkers */



        #endregion ZOOM AND CENTER


        #region Export/Import

        /// <summary>
        ///     gets image of the current view
        /// </summary>
        /// <returns></returns>
        public Image? ToImage()
        {
            Image? ret = null;

            bool r = m_forceDoubleBuffer;
            try
            {
                UpdateBackBuffer();

                if (!r)
                {
                    m_forceDoubleBuffer = true;
                }

                Refresh();
                Application.DoEvents();

                using (var ms = new MemoryStream())
                {
                    using (var frame = m_backBuffer?.Clone() as Bitmap)
                    {
                        frame?.Save(ms, ImageFormat.Png);
                    }

                    ret = Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!r)
                {
                    m_forceDoubleBuffer = false;
                    ClearBackBuffer();
                }
            }

            return ret;
        } /* ToImage */

        #endregion Export/Import


        #region Drawing

#if !DESIGN
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (m_forceDoubleBuffer && m_backBuffer != null)
                {
                    if (m_gxOff != null)
                    {
                        DrawGraphics(m_gxOff);
                        e.Graphics.DrawImage(m_backBuffer, 0, 0);
                    }
                }
                else
                {
                    DrawGraphics(e.Graphics);
                }

                base.OnPaint(e);
            }
            catch (Exception ex)
            {
                if (OnExceptionThrown != null)
                    OnExceptionThrown.Invoke(ex);
                else
                    throw;
            }
        } /* OnPaint */


        void DrawGraphics(Graphics g)
        {
            // render white background
            g.Clear(EmptyMapBackground);

            if (_mapRenderTransform.HasValue)
            {
                #region -- scale --

                var center = new GPoint(Width / 2, Height / 2);
                var delta = center;
                delta.OffsetNegative(Core.RenderOffset);
                var pos = center;
                pos.OffsetNegative(delta);

                g.ScaleTransform(_mapRenderTransform.Value, _mapRenderTransform.Value, MatrixOrder.Append);
                g.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                DrawMap(g);
                g.ResetTransform();

                g.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

                OnPaintLayers(g);

                #endregion
            }
            else
            {
                if (IsRotated)
                {
                    #region -- rotation --

                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    g.TranslateTransform((float)(Core.Width / 2.0), (float)(Core.Height / 2.0));
                    g.RotateTransform(-Bearing);
                    g.TranslateTransform((float)(-Core.Width / 2.0), (float)(-Core.Height / 2.0));

                    g.TranslateTransform(Core.RenderOffset.X, Core.RenderOffset.Y);

                    DrawMap(g);

                    g.ResetTransform();
                    g.TranslateTransform(Core.RenderOffset.X, Core.RenderOffset.Y);

                    OnPaintLayers(g);

                    #endregion
                }
                else
                {
                    g.TranslateTransform(Core.RenderOffset.X, Core.RenderOffset.Y);
                    DrawMap(g);
                    OnPaintLayers(g);
                }
            }
        } /* DrawGraphics */
#endif


        void DrawMap(Graphics g)
        {
            if (Core.UpdatingBounds || MapProvider == EmptyProvider.Instance || MapProvider == null)
            {
                Debug.WriteLine("Core.updatingBounds");
                return;
            }

            Core.TileDrawingListLock.AcquireReaderLock();
            Core.Matrix.EnterReadLock();

            //g.TextRenderingHint = TextRenderingHint.AntiAlias;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.CompositingQuality = CompositingQuality.HighQuality;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;  

            try
            {
                foreach (var tilePoint in Core.TileDrawingList)
                {
                    Core.TileRect.Location = tilePoint.PosPixel;
                    Core.TileRect.OffsetNegative(Core.CompensationOffset);

                    //if(Core.currentRegion.IntersectsWith(Core.tileRect) || IsRotated)
                    {
                        bool found = false;

                        var t = Core.Matrix.GetTileWithNoLock(Core.Zoom, tilePoint.PosXY);
                        if (t.NotEmpty)
                        {
                            // render tile
                            {
                                foreach (GMapImage img in t.Overlays)
                                {
                                    if (img != null && img.Img != null)
                                    {
                                        if (!found)
                                            found = true;

                                        if (!img.IsParent)
                                        {
                                            if (!_mapRenderTransform.HasValue && !IsRotated)
                                            {
                                                g.DrawImage(img.Img,
                                                    Core.TileRect.X,
                                                    Core.TileRect.Y,
                                                    Core.TileRect.Width,
                                                    Core.TileRect.Height);
                                            }
                                            else
                                            {
                                                g.DrawImage(img.Img,
                                                    new Rectangle((int)Core.TileRect.X,
                                                        (int)Core.TileRect.Y,
                                                        (int)Core.TileRect.Width,
                                                        (int)Core.TileRect.Height),
                                                    0,
                                                    0,
                                                    Core.TileRect.Width,
                                                    Core.TileRect.Height,
                                                    GraphicsUnit.Pixel,
                                                    m_tileFlipXYAttributes);
                                            }
                                        }
                                        else
                                        {
                                            // TODO: move calculations to loader thread
                                            var srcRect = new RectangleF(
                                                img.Xoff * (img.Img.Width / img.Ix),
                                                img.Yoff * (img.Img.Height / img.Ix),
                                                img.Img.Width / img.Ix,
                                                img.Img.Height / img.Ix);
                                            var dst = new Rectangle((int)Core.TileRect.X,
                                                (int)Core.TileRect.Y,
                                                (int)Core.TileRect.Width,
                                                (int)Core.TileRect.Height);

                                            g.DrawImage(img.Img,
                                                dst,
                                                srcRect.X,
                                                srcRect.Y,
                                                srcRect.Width,
                                                srcRect.Height,
                                                GraphicsUnit.Pixel,
                                                m_tileFlipXYAttributes);
                                        }
                                    }
                                }
                            }
                        }
                        else if (FillEmptyTiles && MapProvider.Projection is MercatorProjection)
                        {
                            #region -- fill empty lines --

                            int zoomOffset = 1;
                            var parentTile = Tile.Empty;
                            long ix = 0;

                            while (!parentTile.NotEmpty && zoomOffset < Core.Zoom &&
                                    zoomOffset <= LevelsKeepInMemory)
                            {
                                ix = (long)Math.Pow(2, zoomOffset);
                                parentTile = Core.Matrix.GetTileWithNoLock(Core.Zoom - zoomOffset++,
                                    new GPoint((int)(tilePoint.PosXY.X / ix), (int)(tilePoint.PosXY.Y / ix)));
                            }

                            if (parentTile.NotEmpty)
                            {
                                long xOff = Math.Abs(tilePoint.PosXY.X - parentTile.Pos.X * ix);
                                long yOff = Math.Abs(tilePoint.PosXY.Y - parentTile.Pos.Y * ix);

                                // render tile 
                                {
                                    foreach (GMapImage img in parentTile.Overlays)
                                    {
                                        if (img != null && img.Img != null && !img.IsParent)
                                        {
                                            if (!found)
                                                found = true;

                                            var srcRect = new RectangleF(
                                                xOff * (img.Img.Width / ix),
                                                yOff * (img.Img.Height / ix),
                                                img.Img.Width / ix,
                                                img.Img.Height / ix);
                                            var dst = new Rectangle((int)Core.TileRect.X,
                                                (int)Core.TileRect.Y,
                                                (int)Core.TileRect.Width,
                                                (int)Core.TileRect.Height);

                                            g.DrawImage(img.Img,
                                                dst,
                                                srcRect.X,
                                                srcRect.Y,
                                                srcRect.Width,
                                                srcRect.Height,
                                                GraphicsUnit.Pixel,
                                                m_tileFlipXYAttributes);
                                            g.FillRectangle(m_selectedAreaFill, dst);
                                        }
                                    }
                                }
                            }

                            #endregion
                        }

                        // add text if tile is missing
                        if (!found)
                        {
                            lock (Core.FailedLoads)
                            {
                                var lt = new LoadTask(tilePoint.PosXY, Core.Zoom);
                                if (Core.FailedLoads.ContainsKey(lt))
                                {
                                    var ex = Core.FailedLoads[lt];
                                    g.FillRectangle(_emptyTileBrush,
                                        new RectangleF(Core.TileRect.X,
                                            Core.TileRect.Y,
                                            Core.TileRect.Width,
                                            Core.TileRect.Height));

                                    g.DrawString("Exception: " + ex.Message,
                                        MissingDataFont,
                                        Brushes.Red,
                                        new RectangleF(Core.TileRect.X + 11,
                                            Core.TileRect.Y + 11,
                                            Core.TileRect.Width - 11,
                                            Core.TileRect.Height - 11));

                                    g.DrawString(EmptyTileText,
                                        MissingDataFont,
                                        Brushes.Blue,
                                        new RectangleF(Core.TileRect.X,
                                            Core.TileRect.Y,
                                            Core.TileRect.Width,
                                            Core.TileRect.Height),
                                        CenterFormat);

                                    g.DrawRectangle(EmptyTileBorders,
                                        (int)Core.TileRect.X,
                                        (int)Core.TileRect.Y,
                                        (int)Core.TileRect.Width,
                                        (int)Core.TileRect.Height);
                                }
                            }
                        }

                        if (ShowTileGridLines)
                        {
                            g.DrawRectangle(EmptyTileBorders,
                                (int)Core.TileRect.X,
                                (int)Core.TileRect.Y,
                                (int)Core.TileRect.Width,
                                (int)Core.TileRect.Height);
                            {
                                g.DrawString(
                                    (tilePoint.PosXY == Core.CenterTileXYLocation ? "CENTER: " : "TILE: ") +
                                    tilePoint,
                                    MissingDataFont,
                                    Brushes.Red,
                                    new RectangleF(Core.TileRect.X,
                                        Core.TileRect.Y,
                                        Core.TileRect.Width,
                                        Core.TileRect.Height),
                                    CenterFormat);
                            }
                        }
                    }
                } /* foreach (var tilePoint in Core.TileDrawingList) */
            }
            finally
            {
                Core.Matrix.LeaveReadLock();
                Core.TileDrawingListLock.ReleaseReaderLock();
            }
        } /* DrawMap */


        /// <summary>
        ///     override, to render something more
        /// </summary>
        /// <param name="g"></param>
        protected virtual void OnPaintLayers(Graphics g)
        {
            try
            {
                g.SmoothingMode = SmoothingMode.HighQuality;

                foreach (var o in Layers)
                {
                    if (o.IsVisible)
                    {
                        o.OnRender(g);
                    }
                }

                for (int i = 0; i < LinePoints.Count; i++)
                {
                    var o = LinePoints[i];
                    if (o.IsVisible && (o.Parent != null) && o.Parent.IsVisible)
                        o.OnRender(g);
                }


                //// separate tooltip drawing
                //foreach (var l in Layers)
                //{
                //    if (l.IsVisible)
                //    {
                //        l.OnRenderToolTips(g);
                //    }
                //}


                g.ResetTransform();

                if (!SelectedArea.IsEmpty)
                {
                    var p1 = FromLatLngToLocal(SelectedArea.LocationTopLeft);
                    var p2 = FromLatLngToLocal(SelectedArea.LocationRightBottom);

                    long x1 = p1.X;
                    long y1 = p1.Y;
                    long x2 = p2.X;
                    long y2 = p2.Y;

                    g.DrawRectangle(SelectionPen, x1, y1, x2 - x1, y2 - y1);
                    g.FillRectangle(m_selectedAreaFill, x1, y1, x2 - x1, y2 - y1);
                }

                if (m_showHelperLine)
                {
                    var p = PointToClient(MousePosition);

                    g.DrawLine(HelperLinePen, p.X, 0, p.X, Height);
                    g.DrawLine(HelperLinePen, 0, p.Y, Width, p.Y);
                    OnShowPlaceInfo?.Invoke(p.X, p.Y);
                }

                if (ShowCenter)
                {
                    g.DrawLine(ScalePen, Width / 2 - 10, Height / 2, Width / 2 + 10, Height / 2);
                    g.DrawLine(ScalePen, Width / 2, Height / 2 - 10, Width / 2, Height / 2 + 10);
                }

                #region -- copyright --

                if (!string.IsNullOrEmpty(Core.Provider.Copyright))
                {
                    g.DrawString(Core.Provider.Copyright + " | " + Core.ProjectionName,
                        CopyrightFont,
                        Brushes.Navy,
                        3,
                        Height - CopyrightFont.Height - 5);
                }

                #endregion

                #region -- draw scale --

                if (m_showMapScaleInfo)
                {
                    int top = m_mapScaleInfoPosition == MapScaleInfoPositions.Top ? 10 : Bottom - 30;
                    int left = 10;
                    int bottom = top + 7;

                    if (Width > Core.PxRes5000Km)
                    {
                        DrawScale(g, top, left + Core.PxRes5000Km, bottom, left, "5000 km");
                    }

                    if (Width > Core.PxRes1000Km)
                    {
                        DrawScale(g, top, left + Core.PxRes1000Km, bottom, left, "1000 km");
                    }

                    if (Width > Core.PxRes100Km && Zoom > 2)
                    {
                        DrawScale(g, top, left + Core.PxRes100Km, bottom, left, "100 km");
                    }

                    if (Width > Core.PxRes10Km && Zoom > 5)
                    {
                        DrawScale(g, top, left + Core.PxRes10Km, bottom, left, "10 km");
                    }

                    if (Width > Core.PxRes1000M && Zoom >= 10)
                    {
                        DrawScale(g, top, left + Core.PxRes1000M, bottom, left, "1000 m");
                    }

                    if (Width > Core.PxRes100M && Zoom > 11)
                    {
                        DrawScale(g, top, left + Core.PxRes100M, bottom, left, "100 m");
                    }
                }


                if (m_showGeoGridLines)
                {
                    int _offset = 5;
                    for (int i = -180; i <= 180; i += 10)
                    {
                        var p1 = FromLatLngToLocal(new PointLatLng(85, i));
                        var p2 = FromLatLngToLocal(new PointLatLng(-85, i));
                        g.DrawLine(GeoLinePen, p1.X, p1.Y, p2.X, p2.Y);
                        var pos = new Point((int)p1.X, _offset);
                        g.DrawString(i.ToString(), GeotextFont, GeoTextColor, pos);
                    }

                    for (int i = -80; i <= 80; i += 10)
                    {
                        var p1 = FromLatLngToLocal(new PointLatLng(i, -180));
                        var p2 = FromLatLngToLocal(new PointLatLng(i, 180));
                        g.DrawLine(GeoLinePen, p1.X, p1.Y, p2.X, p2.Y);
                        var pos = new Point(_offset, (int)p1.Y);
                        g.DrawString(i.ToString(), GeotextFont, GeoTextColor, pos);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (OnExceptionThrown != null)
                    OnExceptionThrown.Invoke(ex);
                else
                    throw;
            }
        }


        private void DrawScale(Graphics g, int top, int right, int bottom, int left, string caption)
        {
            g.DrawLine(ScalePenBorder, left, top, left, bottom);
            g.DrawLine(ScalePenBorder, left, bottom, right, bottom);
            g.DrawLine(ScalePenBorder, right, bottom, right, top);

            g.DrawLine(ScalePen, left, top, left, bottom);
            g.DrawLine(ScalePen, left, bottom, right, bottom);
            g.DrawLine(ScalePen, right, bottom, right, top);

            g.DrawString(caption, ScaleFont, Brushes.Black, right + 3, top - 5);
        }

        /// <summary>
        ///     call this to stop HoldInvalidation and perform single forced instant refresh
        /// </summary>
        public override void Refresh()
        {
            m_holdInvalidation = false;

            lock (Core.InvalidationLock)
            {
                Core.LastInvalidation = DateTime.Now;
            }

            base.Refresh();
        }


        public void ClearAll()
        {
            foreach(var layer in Layers)
            {
                layer.Clear();
            }

            foreach (var obj in LinePoints)
            {
                obj.Reset();
            }

            LinePoints.Clear();
            m_selected_object = null;
            m_selected_linepoint = null;
            Invalidate();
        }


        public void ClearAllLayers()
        {
            ClearAll();
            Layers.Clear();
            Invalidate();
        }

#if !DESIGN
        /// <summary>
        ///     enque built-in thread safe invalidation
        /// </summary>
        public new void Invalidate()
        {
            if (Core.Refresh != null)
            {
                Core.Refresh.Set();
            }
        }
#endif


        #endregion Drawing


        #region User Events

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (HelperLineOption == HelperLineOptions.ShowOnModifierKey)
            {
                m_showHelperLine = e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt;
                if (m_showHelperLine)
                {
                    Invalidate();
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (HelperLineOption == HelperLineOptions.ShowOnModifierKey)
            {
                m_showHelperLine = e.Modifiers == Keys.Shift || e.Modifiers == Keys.Alt;
                if (!m_showHelperLine)
                {
                    Invalidate();
                }
            }
        }

        #endregion  User Events



        #region MouseEvents

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            m_lastpoint = FromLocalToLatLng(e.X, e.Y);

            if (!IsMouseOverObject)
            {
                if (e.Button == MouseButtons.Left)
                {

                    switch (MouseMoveStatus)
                    {
                        case MouseMoveStatusType.Line:
                            m_MapLine = Layers[0].AddLine(e.X, e.Y, Color.Black);
                            Invalidate();
                            return;

                        case MouseMoveStatusType.Arrow:
                            m_MapLine = Layers[0].AddLine(e.X, e.Y, Color.Black);
                            if (m_MapLine != null)
                                m_MapLine.ArrowPosition = ArrowPositions.AtEnd;
                            Invalidate();
                            return;

                        case MouseMoveStatusType.Ruler:
                            m_MapRuler = Layers[0].AddRuler(e.X, e.Y, Color.Black);
                            Invalidate();
                            return;

                        case MouseMoveStatusType.CircleRuler:
                            m_MapCircleRuler = Layers[0].AddCircleRuler(e.X, e.Y, Color.Black);
                            Invalidate();
                            return;

                        case MouseMoveStatusType.Rectangle:
                            m_MapRectangle = Layers[0].AddRectangle(e.X, e.Y, Color.Black);
                            Invalidate();
                            return;

                        case MouseMoveStatusType.Ellipse:
                            m_MapRectangle = Layers[0].AddEllipse(e.X, e.Y, Color.Black);
                            Invalidate();
                            return;

                        case MouseMoveStatusType.Circle:
                            m_MapCircle = Layers[0].AddCircle(e.X, e.Y, Color.Black);
                            break;

                        case MouseMoveStatusType.Marker:
                            m_MapMarker = Layers[0].AddBaseMarker(e.X, e.Y, 14, Color.Red);
                            break;

                        case MouseMoveStatusType.Ray:
                            m_MapRayLine = Layers[0].AddRayLine(e.X, e.Y, Color.Black);
                            break;

                        case MouseMoveStatusType.Textbox:
                            m_MapTextBox = Layers[0].AddTextBox(e.X, e.Y, Color.Black);
                            break;

                    } /* switch */
                }

                if (e.Button == DragButton && FindSelectedLinePoint(e))
                    return;

                if (e.Button == DragButton && FindSelectedObject(e))
                    return;

                if (e.Button == DragButton && CanDragMap)
                {
                    Core.MouseDown = new GPoint(e.X, e.Y);      //ApplyRotationInversion(e.X, e.Y);
                    Invalidate();
                }
                else if (!m_isSelecting && e.Button == SelectionButton)
                {
                    m_isSelecting = true;
                    SelectedArea = RectLatLng.Empty;
                    m_selectionEnd = PointLatLng.Empty;
                    m_selectionStart = FromLocalToLatLng(e.X, e.Y);
                }
            }
        } /* OnMouseDown */


        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (m_MapRuler != null)
            {
                m_MapRuler.Remove();
                m_MapRuler = null;
            }

            if (m_MapLine != null)
            {
                if (m_MapLine.ArrowPosition == ArrowPositions.None) 
                    m_MapLine.Thickness = 1;
                m_MapLine.LineColor = Color.Red;
                m_MapLine = null;
            }

            if (m_MapCircleRuler != null)
            {
                m_MapCircleRuler.Thickness = 1;
                m_MapCircleRuler.LineColor = Color.Red;
                m_MapCircleRuler = null;
            }

            if (m_MapRectangle != null)
            {
                m_MapRectangle.Thickness = 1;
                m_MapRectangle.LineColor = Color.Red;
                m_MapRectangle = null;
            }

            if (m_MapCircle != null)
            {
                m_MapCircle.Thickness = 1;
                m_MapCircle.Color = Color.Red;
                m_MapCircle = null;
            }

            if (m_MapRayLine != null)
            {
                if (m_MapRayLine.ArrowPosition == ArrowPositions.None)
                    m_MapRayLine.Thickness = 1;
                m_MapRayLine.Color = Color.Red;
                m_MapRayLine.ArrowPosition = ArrowPositions.AtEnd;
                m_MapRayLine = null;
            }

            if (m_MapTextBox != null)
            {
                OnTextBoxEdit?.Invoke(m_MapTextBox);
                m_MapTextBox = null;
            }

            if (m_selected_linepoint != null)
            {
                m_selected_linepoint.Parent?.UpdateLinePoints();
                m_selected_linepoint.IsSelected = false;
                m_selected_linepoint = null;
                Invalidate();
                MouseMoveStatus = MouseMoveStatusType.None;
                return;
            }


            if (m_isSelecting)          // selecting an area
            {
                m_isSelecting = false;
            }

            if (Core.IsDragging)
            {
                if (m_isDragging)
                {
                    m_isDragging = false;
                    Debug.WriteLine("IsDragging = " + m_isDragging);
                    Cursor = m_cursorBefore;
                    m_cursorBefore = null;
                    if (OnDraggedDone != null)
                        OnDraggedDone.Invoke();
                }

                Core.EndDrag();

                //if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                //{
                //    if (Core.LastLocationInBounds.HasValue)
                //    {
                //        Position = Core.LastLocationInBounds.Value;
                //    }
                //}
            }
            else
            {
                if (e.Button == DragButton)
                {
                    Core.MouseDown = GPoint.Empty;
                }

                if (!m_selectionEnd.IsEmpty && !m_selectionStart.IsEmpty)
                {
                    bool zoomtofit = false;

                    if (!SelectedArea.IsEmpty && ModifierKeys == Keys.Shift)
                    {
                        zoomtofit = SetZoomToFitRect(SelectedArea);
                        ClearSelection();
                    }

                    OnSelectionChange?.Invoke(SelectedArea, zoomtofit);
                }
                else
                {
                    Invalidate();
                }
            }

            //if (MouseMoveStatus != MouseMoveStatusType.Textbox)
                MouseMoveStatus = MouseMoveStatusType.None;
        }  /* OnMouseUp */


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_MapRuler != null)
            {
                m_MapRuler.UpdateEndPoint(e.X, e.Y);
                Invalidate();
                return;
            }

            if (m_MapCircleRuler != null)
            {
                m_MapCircleRuler.UpdateEndPoint(e.X, e.Y);
                Invalidate();
                return;
            }

            if (m_MapLine != null)
            {
                m_MapLine.UpdateEndPoint(e.X, e.Y);
                Invalidate();
                return;
            }

            if (m_MapRectangle != null)
            {
                m_MapRectangle.UpdateEndPoint(e.X, e.Y);
                Invalidate();
                return;
            }

            if (m_MapCircle != null)
            {
                m_MapCircle.ChangeRadius(e.X, e.Y); 
                Invalidate();
                return;
            }

            if (m_MapRayLine != null)
            {
                m_MapRayLine.UpdateEndPoint(e.X, e.Y);
                Invalidate();
                OnShowPlaceInfoEx?.Invoke(e.X, e.Y, m_MapRayLine.Direction, m_MapRayLine.Distance);
                return;
            }

            if (m_MapTextBox != null)
            {
                m_MapTextBox.UpdateEndPoint(e.X, e.Y);
                Invalidate();
                return;
            }

            if (!Core.IsDragging && !Core.MouseDown.IsEmpty)
            {
                var p = ApplyRotationInversion(e.X, e.Y);
                if (Math.Abs(p.X - Core.MouseDown.X) * 2 >= DragSize.Width ||
                    Math.Abs(p.Y - Core.MouseDown.Y) * 2 >= DragSize.Height)
                {
                    Core.BeginDrag(Core.MouseDown);
                }
            }

            if (SelectedLinePoint != null)
            {
                SelectedLinePoint.MovePoint(SelectedLinePoint.Position, FromLocalToLatLng(e.X, e.Y));
                return;
            }


            if ((SelectedObject != null) && (e.Button == DragButton))
            {
                SelectedObject.Shift(m_lastpoint, FromLocalToLatLng(e.X, e.Y));
                m_lastpoint = FromLocalToLatLng(e.X, e.Y);
                OnShowPlaceInfo?.Invoke(e.X, e.Y);
            }

            if (Core.IsDragging)
            {
                if (!m_isDragging)
                {
                    m_isDragging = true;
                    Debug.WriteLine("IsDragging = " + m_isDragging);

                    m_cursorBefore = Cursor;
                    Cursor = Cursors.SizeAll;
                }

                //if (BoundsOfMap.HasValue && !BoundsOfMap.Value.Contains(Position))
                //{
                //    // ...
                //}
                //else
                {
                    Core.MouseCurrent = ApplyRotationInversion(e.X, e.Y);
                    Core.Drag(Core.MouseCurrent);
                    base.Invalidate();
                }
            }
            else
            {
                if (m_isSelecting && !m_selectionStart.IsEmpty &&
                    (ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift || m_selectionAlways))
                {
                    m_selectionEnd = FromLocalToLatLng(e.X, e.Y);
                    {
                        var p1 = m_selectionStart;
                        var p2 = m_selectionEnd;

                        double x1 = Math.Min(p1.Lng, p2.Lng);
                        double y1 = Math.Max(p1.Lat, p2.Lat);
                        double x2 = Math.Max(p1.Lng, p2.Lng);
                        double y2 = Math.Min(p1.Lat, p2.Lat);

                        SelectedArea = new RectLatLng(y1, x1, x2 - x1, y1 - y2);
                    }
                }
                else
                if (Core.MouseDown.IsEmpty)
                {
                }

                if (m_showHelperLine)
                {
                    base.Invalidate();
                }
            }
        } /* OnMouseMove */


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (!Core.IsDragging)
            {
                //bool overlayObject = FindSelectedLinePoint(e);

                //if (!overlayObject && Core.MouseDown != GPoint.Empty)
                //    OnMapClick?.Invoke(FromLocalToLatLng(e.X, e.Y), e);
                //if (overlayObject)
                //    return;

                //overlayObject = FindSelectedObject(e);

                //if (!overlayObject && Core.MouseDown != GPoint.Empty)
                    OnMapClick?.Invoke(FromLocalToLatLng(e.X, e.Y), e);
            }
        } /* OnMouseClick */

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!Core.IsDragging)
            {
                bool overlayObjet = FindSelectedObject(e);
                if (m_selected_object is MapTextBox)
                {
                    OnTextBoxEdit?.Invoke(m_selected_object as MapTextBox);
                }

                if (!overlayObjet && Core.MouseDown != GPoint.Empty)
                    OnMapDoubleClick?.Invoke(FromLocalToLatLng(e.X, e.Y), e);

            }
        } /* OnMouseDoubleClick */


        protected bool FindSelectedObject(MouseEventArgs e)
        {
            bool overlayObject = false;
            if (m_selected_object != null)
                m_selected_object.IsSelected = false;
            m_selected_object = null;

            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                var l = Layers[i];
                if (l.IsVisible)
                {
                    var o = l.GetObjectbyLocalPosition(e.X, e.Y);
                    if (o != null)
                    {
                        overlayObject = true;
                        o.IsSelected = true;
                        m_selected_object = o;
                        break;
                    }
                }
            }

            return overlayObject;
        }

        protected bool FindSelectedLinePoint(MouseEventArgs e)
        {
            bool overlayObject = false;
            if (m_selected_linepoint != null)
                m_selected_linepoint.IsSelected = false;
            m_selected_linepoint = null;

            var o = GetLinePointbyLocalPosition(e.X, e.Y);
            if (o != null)
            {
                overlayObject = true;
                o.IsSelected = true;
                m_selected_linepoint = o;
            }

            return overlayObject;
        }


        #endregion MouseEvents 


        #region Macro
        public bool LoadMacroFile(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    string errors = string.Empty;
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            if (line.StartsWith("#"))
                                continue;

                            if (!ProcessMacroCommand(line, out string errorstring))
                            {
                                errors += errorstring + Environment.NewLine;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(errors))
                    {
                        MessageBox.Show("Errors occurred while processing macro file: " + Environment.NewLine + errors, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    return true;
                }
                else
                {
                    MessageBox.Show("Macro file not found: " + file, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading macro file: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }


        public bool ProcessMacroCommand(string command, out string errorstring)
        {
            char[] separator = new Char[] { ',', ';', ' ' };
            errorstring = string.Empty;

            try
            {
                // Process the macro command here
                Debug.WriteLine("Processing macro command: " + command);


                if (string.IsNullOrEmpty(command))
                    return true;

                command = command.Trim();
                if (command.Length == 0)
                    return true;

                if (command.StartsWith("clear all"))                // clear all objects
                {
                    ClearAll();
                    return true;
                }

                if (command.StartsWith("clear layers"))                // clear all layers
                {
                    ClearAllLayers();
                    return true;
                }


                var currentLayer = Layers.Count == 0 ? null : Layers[0];

                string[] ls = command.Split(separator);

                switch (ls[0].ToLower())
                {
                    case "layer":                       // create layer: "layer", name
                        {
                            if (ls.Length < 2)
                            {
                                errorstring = "Not enough parameters to create a layer.";
                                return false;
                            }
                            string name = ls[1];
                            currentLayer = AddLayer(name);
                        }
                        break;

                    case "center":                      // set center: "center", lat, lng
                        if ((ls.Length > 2) && double.TryParse(ls[1], out double lat) && double.TryParse(ls[2], out double lng))
                        {
                            Position = new PointLatLng(lat, lng);
                            return true;
                        }
                        break;

                    case "zoom":
                        if ((ls.Length > 1) && double.TryParse(ls[1], out double z))
                        {
                            Zoom = z;
                            return true;
                        }
                        break;

                    case "line":                        // draw line: "line", name, lat1, lng1, lat2, lng2, color
                    case "ray":                         // draw ray: "ray", name, lat, lng, degree, color
                    case "polyline":                    // draw poliline: "polyline", name, lat1, lng1, lat2, lng2, lat3, lng3, ..., color
                    case "circle":                       // draw circle "circle name lat, long, radius, color
                    case "ellipse":                       // draw ellipse "ellipse name lat, long, radius1, radius2, color
                    case "rectangle":                       // draw rectangle "rectangle name lat, long, width, height, color
                    case "label":                       // draw label: "label", name, lat,lng, text
                    case "route":
                    case "marker":                      // draw marker: "marker", type, name, lat,lng, color
                    case "gmarker":                      // draw google marker: "marker", name, lat,lng, color
                    case "hmarker":                      // draw HFsystem marker: "marker", name, lat,lng, color
                        break;

                    default:
                        errorstring = "Invalid command : " + command;
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing macro command: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        public void ProcessMacroText(string text)
        {
            try
            {
                // Process the macro text here
                Debug.WriteLine("Processing macro text: " + text);
                string errors = string.Empty;

                string[] lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.StartsWith("#"))
                            continue;

                        if (!ProcessMacroCommand(line, out string errorstring))
                        {
                            errors += errorstring + Environment.NewLine;
                        }   
                    }
                }

                if (!string.IsNullOrEmpty(errors))
                {
                    MessageBox.Show("Errors occurred while processing macro file: " + Environment.NewLine + errors, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing macro text: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Layer AddLayer(string name)
        {
            var existingLayer = GetLayerbyName(name);
            if (existingLayer != null)
            {
                return existingLayer;
            }

            var layer = new Layer(name, this);
            Layers.Add(layer);
            return layer;
        }

        public void RemoveLayer(string name)
        {
            var layer = GetLayerbyName(name);
            if (layer != null)
            {
                layer.ClearObjects();
                Layers.Remove(layer);
            }
        }

        public Layer? GetLayerbyName(string name)
        {
            return Layers.FirstOrDefault(obj => obj?.Name == name, null);
        }


        #endregion Macro

        /*=====================================================================================================*/



        /// <summary>
        ///   occurs when clicked on map.
        /// </summary>
        public event MapClick? OnMapClick;

        /// <summary>
        ///     occurs when double clicked on map.
        /// </summary>
        public event MapDoubleClick? OnMapDoubleClick;


        /// <summary>
        ///     occurs when mouse selection is changed
        /// </summary>
        public event SelectionChange? OnSelectionChange;

        public event TextBoxEdit? OnTextBoxEdit;

        /// <summary>
        ///     Import From Kmz
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Category("GMap.NET")]
        [Description("Import From Kmz")]
        public bool ImportFromKmz(string file)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Export From Kmz
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [Category("GMap.NET")]
        [Description("Export From Kmz")]
        public bool ExportFromKmz(string file)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




        /// <summary>
        ///     text on empty tiles
        /// </summary>
        public string EmptyTileText = "We are sorry, but we don't\nhave imagery at this zoom\nlevel for this region.";

        /// <summary>
        ///     pen for empty tile borders
        /// </summary>
        public Pen EmptyTileBorders = new Pen(Brushes.White, 1);




        Brush _emptyTileBrush = new SolidBrush(Color.White);
        Color _emptyTileColor = Color.White;

        /// <summary>
        ///     color of empty tile background
        /// </summary>
        [Category("GMap.NET")]
        [Description("background color of the empty tile")]
        public Color EmptyTileColor
        {
            get
            {
                return _emptyTileColor;
            }
            set
            {
                if (_emptyTileColor != value)
                {
                    _emptyTileColor = value;

                    if (_emptyTileBrush != null)
                    {
                        _emptyTileBrush.Dispose();
                        //_emptyTileBrush = null;
                    }

                    _emptyTileBrush = new SolidBrush(_emptyTileColor);
                }
            }
        }


        /// <summary>
        ///     enables filling empty tiles using lower level images
        /// </summary>
        public bool FillEmptyTiles = true;


        /// <summary>
        ///     retry count to get tile
        /// </summary>
        [Browsable(false)]
        public int RetryLoadTile
        {
            get
            {
                return Core.RetryLoadTile;
            }
            set
            {
                Core.RetryLoadTile = value;
            }
        }

        /// <summary>
        ///     how many levels of tiles are staying decompresed in memory
        /// </summary>
        [Browsable(false)]
        public int LevelsKeepInMemory
        {
            get
            {
                return Core.LevelsKeepInMemory;
            }

            set
            {
                Core.LevelsKeepInMemory = value;
            }
        }

        /// <summary>
        ///     map boundaries
        /// </summary>
        //public RectLatLng? BoundsOfMap = null;

        //ColorMatrix _colorMatrix;

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[Browsable(false)]
        //public ColorMatrix ColorMatrix
        //{
        //    get
        //    {
        //        return _colorMatrix;
        //    }
        //    set
        //    {
        //        _colorMatrix = value;
        //        if (GMapProvider.TileImageProxy != null && GMapProvider.TileImageProxy is GMapImageProxy)
        //        {
        //            (GMapProvider.TileImageProxy as GMapImageProxy).ColorMatrix = value;
        //            if (Core.IsStarted)
        //            {
        //                ReloadMap();
        //            }
        //        }
        //    }
        //}

        // internal stuff
        internal readonly Core Core = new Core();

        internal readonly Font CopyrightFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular);
        internal readonly Font MissingDataFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold);
        Font ScaleFont = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular);
        internal readonly StringFormat CenterFormat = new StringFormat();
        internal readonly StringFormat BottomFormat = new StringFormat();


        void Overlays_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                //foreach (GMapOverlay obj in e.NewItems)
                //{
                //    if (obj != null)
                //    {
                //        obj.Control = this;
                //    }
                //}

                if (Core.IsStarted && !m_holdInvalidation)
                {
                    Invalidate();
                }
            }
        }

        void InvalidatorEngage(object? sender, ProgressChangedEventArgs e)
        {
            base.Invalidate();
        }

        /// <summary>
        ///     update objects when map is draged/zoomed
        /// </summary>
        internal void ForceUpdateLayers()
        {
            try
            {
                m_holdInvalidation = true;

                foreach (var o in Layers)
                {
                    if (o.IsVisible)
                    {
                        o.ForceUpdate();
                    }
                }

                foreach (var obj in LinePoints)
                {
                    if (obj.IsVisible)
                    {
                        obj.UpdateObject();     //UpdateLocalPosition();
                    }
                }
            }
            finally
            {
                Refresh();
            }
        }


        /// <summary>
        ///     sets to max zoom to fit all markers and centers them in map
        /// </summary>
        /// <param name="overlayId">overlay id or null to check all</param>
        /// <returns></returns>
        public bool ZoomAndCenterMarkers(string overlayId)
        {
            var rect = GetRectOfAllMarkers(overlayId);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }

        /// <summary>
        ///     zooms and centers all route
        /// </summary>
        /// <param name="overlayId">overlay id or null to check all</param>
        /// <returns></returns>
        public bool ZoomAndCenterRoutes(string overlayId)
        {
            var rect = GetRectOfAllRoutes(overlayId);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }

        /// <summary>
        ///     zooms and centers route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public bool ZoomAndCenterRoute(MapRoute route)
        {
            var rect = GetRectOfRoute(route);
            if (rect.HasValue)
            {
                return SetZoomToFitRect(rect.Value);
            }

            return false;
        }

        /// <summary>
        ///     gets rectangle with all objects inside
        /// </summary>
        /// <param name="overlayId">overlay id or null to check all except zoomInsignificant</param>
        /// <returns></returns>
        public RectLatLng? GetRectOfAllMarkers(string overlayId)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            //foreach (var o in Overlays)
            //{
            //    if (overlayId == null && o.IsZoomSignificant || o.Id == overlayId)
            //    {
            //        if (o.IsVisibile && o.Markers.Count > 0)
            //        {
            //            foreach (var m in o.Markers)
            //            {
            //                if (m.IsVisible)
            //                {
            //                    // left
            //                    if (m.Position.Lng < left)
            //                    {
            //                        left = m.Position.Lng;
            //                    }

            //                    // top
            //                    if (m.Position.Lat > top)
            //                    {
            //                        top = m.Position.Lat;
            //                    }

            //                    // right
            //                    if (m.Position.Lng > right)
            //                    {
            //                        right = m.Position.Lng;
            //                    }

            //                    // bottom
            //                    if (m.Position.Lat < bottom)
            //                    {
            //                        bottom = m.Position.Lat;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue &&
                bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }

        /// <summary>
        ///     gets rectangle with all objects inside
        /// </summary>
        /// <param name="overlayId">overlay id or null to check all except zoomInsignificant</param>
        /// <returns></returns>
        public RectLatLng? GetRectOfAllRoutes(string overlayId)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            //foreach (var o in Overlays)
            //{
            //    if (overlayId == null && o.IsZoomSignificant || o.Id == overlayId)
            //    {
            //        if (o.IsVisibile && o.Routes.Count > 0)
            //        {
            //            foreach (var route in o.Routes)
            //            {
            //                if (route.IsVisible && route.From.HasValue && route.To.HasValue)
            //                {
            //                    foreach (var p in route.Points)
            //                    {
            //                        // left
            //                        if (p.Lng < left)
            //                        {
            //                            left = p.Lng;
            //                        }

            //                        // top
            //                        if (p.Lat > top)
            //                        {
            //                            top = p.Lat;
            //                        }

            //                        // right
            //                        if (p.Lng > right)
            //                        {
            //                            right = p.Lng;
            //                        }

            //                        // bottom
            //                        if (p.Lat < bottom)
            //                        {
            //                            bottom = p.Lat;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            if (left != double.MaxValue && right != double.MinValue && top != double.MinValue &&
                bottom != double.MaxValue)
            {
                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }

        /// <summary>
        ///     gets rect of route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public RectLatLng? GetRectOfRoute(MapRoute route)
        {
            RectLatLng? ret = null;

            double left = double.MaxValue;
            double top = double.MinValue;
            double right = double.MinValue;
            double bottom = double.MaxValue;

            if (route.From.HasValue && route.To.HasValue)
            {
                foreach (var p in route.Points)
                {
                    // left
                    if (p.Lng < left)
                    {
                        left = p.Lng;
                    }

                    // top
                    if (p.Lat > top)
                    {
                        top = p.Lat;
                    }

                    // right
                    if (p.Lng > right)
                    {
                        right = p.Lng;
                    }

                    // bottom
                    if (p.Lat < bottom)
                    {
                        bottom = p.Lat;
                    }
                }

                ret = RectLatLng.FromLTRB(left, top, right, bottom);
            }

            return ret;
        }

        /// <summary>
        ///     offset position in pixels
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Offset(int x, int y)
        {
            if (IsHandleCreated)
            {
                if (IsRotated)
                {
                    var p = new[] { new Point(x, y) };
                    _rotationMatrixInvert.TransformVectors(p);
                    x = p[0].X;
                    y = p[0].Y;
                }

                Core.DragOffset(new GPoint(x, y));

                ForceUpdateLayers();
            }
        }

        /// <summary>
        ///     Obtains the orientation between two points expressed in degrees
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public double GetBearing(PointLatLng startPoint, PointLatLng endPoint)
        {
            //double startLat, double startLong, double endLat, double endLong
            double startLat = radians(startPoint.Lat);
            double startLong = radians(startPoint.Lng);
            double endLat = radians(endPoint.Lat);
            double endLong = radians(endPoint.Lng);

            double dLong = endLong - startLong;

            double dPhi = Math.Log(Math.Tan(endLat / 2.0 + Math.PI / 4.0) / Math.Tan(startLat / 2.0 + Math.PI / 4.0));
            if (Math.Abs(dLong) > Math.PI)
            {
                if (dLong > 0.0)
                    dLong = -(2.0 * Math.PI - dLong);
                else
                    dLong = 2.0 * Math.PI + dLong;
            }

            return Math.Round((degrees(Math.Atan2(dLong, dPhi)) + 360.0) % 360.0, 2);
        }

        /// <summary>
        ///     check if a given point is within the given point based map boundary
        /// </summary>
        /// <param name="points"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public bool IsPointInBoundary(List<PointLatLng> points, string lat, string lng)
        {
            //var polyOverlay = new GMapOverlay();
            //var polygon = new GMapPolyLine(points, "routePolygon");
            //polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            //polygon.Stroke = new Pen(Color.Red, 1);
            //polyOverlay.Polygons.Add(polygon);
            //var pnt = new PointLatLng(double.Parse(lat), double.Parse(lng));
            //return polygon.IsInside(pnt);
            return false;
        }

        double radians(double n)
        {
            return n * (Math.PI / 180);
        }

        double degrees(double n)
        {
            return n * (180 / Math.PI);
        }


        public Point PositiontoLocal(PointLatLng point)
        {
            var p = FromLatLngToLocal(point);
            {
                p.OffsetNegative(Core.RenderOffset);
                return new Point((int)(p.X), (int)(p.Y));
            }
        }

        #region UserControl Events

        public static readonly bool IsDesignerHosted = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);

                if (!IsDesignerHosted)
                {
                    //MethodInvoker m = delegate
                    //{
                    //   Thread.Sleep(444);

                    //OnSizeChanged(null);

                    if (m_lazyEvents)
                    {
                        m_lazyEvents = false;

                        if (m_lazySetZoomToFitRect.HasValue)
                        {
                            SetZoomToFitRect(m_lazySetZoomToFitRect.Value);
                            m_lazySetZoomToFitRect = null;
                        }
                    }

                    Core.OnMapOpen().ProgressChanged += InvalidatorEngage;
                    ForceUpdateLayers();
                    //};
                    //this.BeginInvoke(m);
                }
            }
            catch (Exception ex)
            {
                if (OnExceptionThrown != null)
                    OnExceptionThrown.Invoke(ex);
                else
                    throw;
            }
        }

        protected override void OnCreateControl()
        {
            try
            {
                base.OnCreateControl();

                if (!IsDesignerHosted)
                {
                    var f = ParentForm;
                    if (f != null)
                    {
                        while (f.ParentForm != null)
                        {
                            f = f.ParentForm;
                        }

                        if (f != null)
                        {
                            f.FormClosing += ParentForm_FormClosing;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (OnExceptionThrown != null)
                    OnExceptionThrown.Invoke(ex);
                else
                    throw;
            }
        }

        void ParentForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                Manager.CancelTileCaching();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Core.OnMapClose();

                //Overlays.CollectionChanged -= Overlays_CollectionChanged;

                //foreach (var o in Overlays)
                //{
                //    o.Dispose();
                //}

                //Overlays.Clear();

                ScaleFont.Dispose();
                ScalePen.Dispose();
                CenterFormat.Dispose();
                CenterPen.Dispose();
                BottomFormat.Dispose();
                CopyrightFont.Dispose();
                EmptyTileBorders.Dispose();
                _emptyTileBrush.Dispose();

                m_selectedAreaFill.Dispose();
                SelectionPen.Dispose();
                ClearBackBuffer();
            }

            base.Dispose(disposing);
        }

        float? _mapRenderTransform;

        public Color EmptyMapBackground = Color.WhiteSmoke;



        readonly Matrix _rotationMatrix = new Matrix();
        readonly Matrix _rotationMatrixInvert = new Matrix();

        /// <summary>
        ///     updates rotation matrix
        /// </summary>
        void UpdateRotationMatrix()
        {
            var center = new PointF(Core.Width / 2, Core.Height / 2);

            _rotationMatrix.Reset();
            _rotationMatrix.RotateAt(-Bearing, center);

            _rotationMatrixInvert.Reset();
            _rotationMatrixInvert.RotateAt(-Bearing, center);
            _rotationMatrixInvert.Invert();
        }

        /// <summary>
        ///     returs true if map bearing is not zero
        /// </summary>
        [Browsable(false)]
        public bool IsRotated
        {
            get
            {
                return Core.IsRotated;
            }
        }

        /// <summary>
        ///     bearing for rotation of the map
        /// </summary>
        [Category("GMap.NET")]
        public float Bearing
        {
            get
            {
                return Core.Bearing;
            }
            set
            {
                if (Core.Bearing != value)
                {
                    bool resize = Core.Bearing == 0;
                    Core.Bearing = value;

                    //if(VirtualSizeEnabled)
                    //{
                    // c.X += (Width - Core.vWidth) / 2;
                    // c.Y += (Height - Core.vHeight) / 2;
                    //}

                    UpdateRotationMatrix();

                    if (value != 0 && value % 360 != 0)
                    {
                        Core.IsRotated = true;

                        if (Core.TileRectBearing.Size == Core.TileRect.Size)
                        {
                            Core.TileRectBearing = Core.TileRect;
                            Core.TileRectBearing.Inflate(1, 1);
                        }
                    }
                    else
                    {
                        Core.IsRotated = false;
                        Core.TileRectBearing = Core.TileRect;
                    }

                    if (resize)
                    {
                        Core.OnMapSizeChanged(Width, Height);
                    }

                    if (!m_holdInvalidation && Core.IsStarted)
                    {
                        ForceUpdateLayers();
                    }
                }
            }
        }

        /// <summary>
        ///     shrinks map area, useful just for testing
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool VirtualSizeEnabled
        {
            get
            {
                return Core.VirtualSizeEnabled;
            }
            set
            {
                Core.VirtualSizeEnabled = value;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Width == 0 || Height == 0)
            {
                Debug.WriteLine("minimized");
                return;
            }

            if (Width == Core.Width && Height == Core.Height)
            {
                Debug.WriteLine("maximized");
                return;
            }

            if (!IsDesignerHosted)
            {
                if (m_forceDoubleBuffer)
                {
                    UpdateBackBuffer();
                }

                if (VirtualSizeEnabled)
                {
                    Core.OnMapSizeChanged(Core.VWidth, Core.VHeight);
                }
                else
                {
                    Core.OnMapSizeChanged(Width, Height);
                }
                //Core.currentRegion = new GRect(-50, -50, Core.Width + 50, Core.Height + 50);

                if (Visible && IsHandleCreated && Core.IsStarted)
                {
                    if (IsRotated)
                    {
                        UpdateRotationMatrix();
                    }

                    ForceUpdateLayers();
                }
            }
        }

        void UpdateBackBuffer()
        {
            ClearBackBuffer();

            m_backBuffer = new Bitmap(Width, Height);
            m_gxOff = Graphics.FromImage(m_backBuffer);
        }

        private void ClearBackBuffer()
        {
            if (m_backBuffer != null)
            {
                m_backBuffer.Dispose();
                m_backBuffer = null;
            }

            if (m_gxOff != null)
            {
                m_gxOff.Dispose();
                m_gxOff = null;
            }
        }

        public delegate void OnDragged();
        public event OnDragged? OnDraggedDone;



        /// <summary>
        /// Clears the selection rectangle.
        /// </summary>
        public void ClearSelection()
        {
            if (!SelectedArea.IsEmpty)
            {
                SelectedArea = RectLatLng.Empty;
                m_selectionEnd = PointLatLng.Empty;
                m_selectionStart = PointLatLng.Empty;
                Invalidate();
            }
        }

        public bool IsSelected()
        {
            return !SelectedArea.IsEmpty;
        }

        public bool InSelectedArea(PointLatLng pos)
        {
             return IsSelected() && SelectedArea.Contains(pos);
        }


        public void CheckSelection(PointLatLng pos)
        {
            if (IsSelected() && SelectedArea.Contains(pos))
            {
                bool zoomtofit = SetZoomToFitRect(SelectedArea);

                OnSelectionChange?.Invoke(SelectedArea, zoomtofit);
            }

            ClearSelection();
        }


        /// <summary>
        ///     apply transformation if in rotation mode
        /// </summary>
        GPoint ApplyRotationInversion(int x, int y)
        {
            var ret = new GPoint(x, y);

            if (IsRotated)
            {
                var tt = new[] { new Point(x, y) };
                _rotationMatrixInvert.TransformPoints(tt);
                var f = tt[0];

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        /// <summary>
        ///     apply transformation if in rotation mode
        /// </summary>
        GPoint ApplyRotation(int x, int y)
        {
            var ret = new GPoint(x, y);

            if (IsRotated)
            {
                var tt = new[] { new Point(x, y) };
                _rotationMatrix.TransformPoints(tt);
                var f = tt[0];

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        Cursor? m_cursorBefore = Cursors.Default;

        /// <summary>
        ///     Gets the width and height of a rectangle centered on the point the mouse
        ///     button was pressed, within which a drag operation will not begin.
        /// </summary>
        public Size DragSize = SystemInformation.DragSize;


        internal void RestoreCursorOnLeave()
        {
            if (OverObjectCount <= 0 && m_cursorBefore != null)
            {
                OverObjectCount = 0;
                Cursor = m_cursorBefore;
                m_cursorBefore = null;
            }
        }

        internal void SetCursorHandOnEnter()
        {
            if (OverObjectCount <= 0 && Cursor != Cursors.Hand)
            {
                OverObjectCount = 0;
                m_cursorBefore = Cursor;
                Cursor = Cursors.Hand;
            }
        }

        /// <summary>
        ///     prevents focusing map if mouse enters it's area
        /// </summary>
        public bool DisableFocusOnMouseEnter = false;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!DisableFocusOnMouseEnter)
            {
                Focus();
            }

            _mouseIn = true;
        }

        bool _mouseIn;

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseIn = false;
        }

        /// <summary>
        ///     reverses MouseWheel zooming direction
        /// </summary>
        public bool InvertedMouseWheelZooming = false;

        /// <summary>
        ///     lets you zoom by MouseWheel even when pointer is in area of marker
        /// </summary>
        public bool IgnoreMarkerOnMouseWheel = false;

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (MouseWheelZoomEnabled && _mouseIn && (!IsMouseOverObject || IgnoreMarkerOnMouseWheel) &&
                !Core.IsDragging)
            {
                if (Core.MouseLastZoom.X != e.X && Core.MouseLastZoom.Y != e.Y)
                {
                    if (MouseWheelZoomType == MouseWheelZoomType.MousePositionAndCenter)
                    {
                        Core._position = FromLocalToLatLng(e.X, e.Y);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.ViewCenter)
                    {
                        Core._position = FromLocalToLatLng(Width / 2, Height / 2);
                    }
                    else if (MouseWheelZoomType == MouseWheelZoomType.MousePositionWithoutCenter)
                    {
                        Core._position = FromLocalToLatLng(e.X, e.Y);
                    }

                    Core.MouseLastZoom.X = e.X;
                    Core.MouseLastZoom.Y = e.Y;
                }

                // set mouse position to map center
                if (MouseWheelZoomType != MouseWheelZoomType.MousePositionWithoutCenter)
                {
                    if (!GMaps.Instance.IsRunningOnMono)
                    {
                        var p = PointToScreen(new Point(Width / 2, Height / 2));
                        Stuff.SetCursorPos(p.X, p.Y);
                    }
                }

                Core.MouseWheelZooming = true;

                if (e.Delta > 0)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = (int)Zoom + 1;
                    }
                    else
                    {
                        Zoom = (int)(Zoom + 0.99) - 1;
                    }
                }
                else if (e.Delta < 0)
                {
                    if (!InvertedMouseWheelZooming)
                    {
                        Zoom = (int)(Zoom + 0.99) - 1;
                    }
                    else
                    {
                        Zoom = (int)Zoom + 1;
                    }
                }

                Core.MouseWheelZooming = false;
            }
        }

        #endregion

        #region LinePoints

        internal void AddLinePoint(Layer layer, MapLinePoint obj)
        {
            if (obj == null) return;
            LinePoints.Add(obj);
            obj.Layer = layer;
            obj.UpdateObject();
        }

        internal void RemoveLinePoint(MapLinePoint obj)
        {
            if (obj == null) return;
            try
            {
                LinePoints.Remove(obj);
                obj.Layer = null;
            }
            catch { }
            ;
        }

        internal void RemoveLinePoints(MapLine obj)
        {
            if (obj == null) return;

            try
            {
                for (int i = LinePoints.Count - 1; i >= 0; i--)
                {
                    var o = LinePoints[i];
                    if (o.Parent == obj)
                    {
                        RemoveLinePoint(o);
                    }
                }
            }
            catch { }
    ;
        }


        internal void ClearLinePoints()
        {
            foreach (var obj in LinePoints)
            {
                obj.Layer = null;
            }
            LinePoints.Clear();
        }

        internal void ClearLinePoints(BasePoint parent)
        {
            //foreach (var obj in LinePoints)
            for (int i = LinePoints.Count - 1; i >= 0; i--)
            {
                var obj = LinePoints[i];
                if ((obj != null) && (obj.Parent == parent))
                {
                    obj.Layer = null;
                    LinePoints.Remove(obj);
                }
            }
        }

        internal MapLinePoint? GetLinePointbyLocalPosition(int x, int y)
        {
            var p = new GPoint(x, y);
            p.OffsetNegative(Core.RenderOffset);
            return LinePoints.FirstOrDefault(obj => obj.IsInside((int)p.X, (int)p.Y));
        }


        #endregion LinePoints


        #region IGControl Members

        /// <summary>
        ///     Call it to empty tile cache & reload tiles
        /// </summary>
        public void ReloadMap()
        {
            Core.ReloadMap();
        }


        public PointLatLng GetPositionBySettlement(string keys)
        {
            var status = GeoCoderStatusCode.UNKNOWN_ERROR;
            var gp = MapProvider as GeocodingProvider;

            if (gp == null)
                gp = GMapProviders.OpenStreetMap as GeocodingProvider;

            if (gp != null)
            {
                var pt = gp.GetPoint(keys.Replace("#", "%23"), out status);

                if (status == GeoCoderStatusCode.OK && pt.HasValue)
                    return pt.Value;
            }

            return PointLatLng.Empty;
        }


        /// <summary>
        ///     set current position using keywords
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>true if successfull</returns>
        public GeoCoderStatusCode SetPositionByKeywords(string keys)
        {
            var status = GeoCoderStatusCode.UNKNOWN_ERROR;
            var gp = MapProvider as GeocodingProvider;

            if (gp == null)
                gp = GMapProviders.OpenStreetMap as GeocodingProvider;

            if (gp != null)
            {
                var pt = gp.GetPoint(keys.Replace("#", "%23"), out status);

                if (status == GeoCoderStatusCode.OK && pt.HasValue)
                    Position = pt.Value;
            }

            return status;
        }

        /// <summary>
        ///     get current position using keywords
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public GeoCoderStatusCode GetPositionByKeywords(string keys, out PointLatLng point)
        {
            point = new PointLatLng();

            var status = GeoCoderStatusCode.UNKNOWN_ERROR;
            var gp = MapProvider as GeocodingProvider;

            if (gp == null)
                gp = GMapProviders.OpenStreetMap as GeocodingProvider;

            if (gp != null)
            {
                var pt = gp.GetPoint(keys.Replace("#", "%23"), out status);

                if (status == GeoCoderStatusCode.OK && pt.HasValue)
                    point = pt.Value;
            }

            return status;
        }


        public PointLatLng GetPositionFromPoint(PointLatLng point, double distance, double bearing)
        {
            return MapProvider.Projection.GetPositionFromPoint(point, distance, bearing);
        }

        /// <summary>
        ///     gets world coordinate from local control coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointLatLng FromLocalToLatLng(int x, int y)
        {
            if (_mapRenderTransform.HasValue)
            {
                //var xx = (int)(Core.renderOffset.X + ((x - Core.renderOffset.X) / MapRenderTransform.Value));
                //var yy = (int)(Core.renderOffset.Y + ((y - Core.renderOffset.Y) / MapRenderTransform.Value));

                //PointF center = new PointF(Core.Width / 2, Core.Height / 2);

                //Matrix m = new Matrix();
                //m.Translate(-Core.renderOffset.X, -Core.renderOffset.Y);
                //m.Scale(MapRenderTransform.Value, MapRenderTransform.Value);

                //System.Drawing.Point[] tt = new System.Drawing.Point[] { new System.Drawing.Point(x, y) };
                //m.TransformPoints(tt);
                //var z = tt[0];

                //

                x = (int)(Core.RenderOffset.X + (x - Core.RenderOffset.X) / _mapRenderTransform.Value);
                y = (int)(Core.RenderOffset.Y + (y - Core.RenderOffset.Y) / _mapRenderTransform.Value);
            }

            if (IsRotated)
            {
                var tt = new[] { new Point(x, y) };
                _rotationMatrixInvert.TransformPoints(tt);
                var f = tt[0];

                if (VirtualSizeEnabled)
                {
                    f.X += (Width - Core.VWidth) / 2;
                    f.Y += (Height - Core.VHeight) / 2;
                }

                x = f.X;
                y = f.Y;
            }

            return Core.FromLocalToLatLng(x, y);
        }

        /// <summary>
        ///     gets local coordinate from world coordinate
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GPoint FromLatLngToLocal(PointLatLng point)
        {
            var ret = Core.FromLatLngToLocal(point);

            if (_mapRenderTransform.HasValue)
            {
                ret.X = (int)(Core.RenderOffset.X + (Core.RenderOffset.X - ret.X) * -_mapRenderTransform.Value);
                ret.Y = (int)(Core.RenderOffset.Y + (Core.RenderOffset.Y - ret.Y) * -_mapRenderTransform.Value);
            }

            if (IsRotated)
            {
                var tt = new[] { new Point((int)ret.X, (int)ret.Y) };
                _rotationMatrix.TransformPoints(tt);
                var f = tt[0];

                if (VirtualSizeEnabled)
                {
                    f.X += (Width - Core.VWidth) / 2;
                    f.Y += (Height - Core.VHeight) / 2;
                }

                ret.X = f.X;
                ret.Y = f.Y;
            }

            return ret;
        }

        public Point FromLatLngToLocalPosition(PointLatLng point)
        {
            var p = FromLatLngToLocal(point);
            p.OffsetNegative(Core.RenderOffset);
            return new Point((int)p.X, (int)p.Y);
        }


        /// <summary>
        /// Get screen coordinate from world coordinate
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public GPoint FromLatLngToPixel(PointLatLng point)
        {
            var p = Core.FromLatLngToPixel(point);
            p.OffsetNegative(Core.RenderOffset);
            return p;
        }


        public static string GetGeoCoderStatusString(GeoCoderStatusCode status)
        {
            switch (status)
            {
                case GeoCoderStatusCode.UNKNOWN_ERROR: return "Unknown error";
                case GeoCoderStatusCode.ZERO_RESULTS: return "No result";
                case GeoCoderStatusCode.OVER_QUERY_LIMIT: return "Query limit is exceed";
                case GeoCoderStatusCode.REQUEST_DENIED:  return "Request is denied";
                case GeoCoderStatusCode.INVALID_REQUEST: return "Invalid request";
                case GeoCoderStatusCode.ERROR: return "Error";
                case GeoCoderStatusCode.EXCEPTION_IN_CODE: return "Exception in code";
                case GeoCoderStatusCode.NOT_IMPLEMENTED: return "Function is not implemented";
            }

            return status.ToString();
        }

        public string GetPlaceAddress(PointLatLng point, bool showerror = false)
        {
            try
            {
                var pos = GeocodingProvider.GetPlacemark(point, out GeoCoderStatusCode status);
                if (status == GeoCoderStatusCode.OK && pos != null)
                {
                    return pos.Value.Address;
                }
                else if (showerror)
                    return GetGeoCoderStatusString(status);
            }
            catch (Exception ex)
            {
                if (showerror)
                    return ex.Message;
            }

            return string.Empty;
        }


        public string GetPlaceInfo(int x, int y, bool showaddress = false)
        {
            var point = FromLocalToLatLng(x,y);
            return  point.ToGeoString() + "   "  +   (showaddress ? GetPlaceAddress(point) : "");
        }


        /// <summary>
        /// Get distance of two geo point in km
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public double GetDistance(PointLatLng point1, PointLatLng point2)
        {
            return MapProvider.Projection.GetDistance(point1, point2);
        }


        /// <summary>
        /// Get bearing of two geo point in degree
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public double GetBearing2(PointLatLng point1, PointLatLng point2)
        {
            return MapProvider.Projection.GetBearing(point1, point2);
        }


        /// <summary>
        ///     shows map db export dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowExportDialog()
        {
            using (FileDialog dlg = new SaveFileDialog())
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.AddExtension = true;
                dlg.DefaultExt = "gmdb";
                dlg.ValidateNames = true;
                dlg.Title = "GMap.NET: Export map to db, if file exsist only new data will be added";
                dlg.FileName = "DataExp";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = "GMap.NET DB files (*.gmdb)|*.gmdb";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bool ok = GMaps.Instance.ExportToGMDB(dlg.FileName);
                    if (ok)
                    {
                        MessageBox.Show("Complete!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    return ok;
                }
            }

            return false;
        }

        /// <summary>
        ///     shows map dbimport dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowImportDialog()
        {
            using (FileDialog dlg = new OpenFileDialog())
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.AddExtension = true;
                dlg.DefaultExt = "gmdb";
                dlg.ValidateNames = true;
                dlg.Title = "GMap.NET: Import to db, only new data will be added";
                dlg.FileName = "DataImport";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = "GMap.NET DB files (*.gmdb)|*.gmdb";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bool ok = GMaps.Instance.ImportFromGMDB(dlg.FileName);
                    if (ok)
                    {
                        MessageBox.Show("Complete!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ReloadMap();
                    }
                    else
                    {
                        MessageBox.Show("Failed!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    return ok;
                }
            }

            return false;
        }

        /// <summary>
        /// save map in jpeg file
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            var img = ToImage();

            if (img != null)
                img.Save(filename, ImageFormat.Jpeg);
        }

        /// <summary>
        /// Copy map into clipboard
        /// </summary>
        public void Copy()
        {
            var img = ToImage();

            if (img != null)
                Clipboard.SetImage(img);
        }


        [Category("GMap.NET")]
        [Description("map scale type")]
        public ScaleModes ScaleMode { get; set; } = ScaleModes.Integer;

        [Category("GMap.NET")]
        [DefaultValue(0)]
        public double Zoom
        {
            get
            {
                return m_zoomReal;
            }
            set
            {
                if (m_zoomReal != value)
                {
                    Debug.WriteLine("ZoomPropertyChanged: " + m_zoomReal + " -> " + value);

                    if (value > MaxZoom)
                    {
                        m_zoomReal = MaxZoom;
                    }
                    else if (value < MinZoom)
                    {
                        m_zoomReal = MinZoom;
                    }
                    else
                    {
                        m_zoomReal = value;
                    }

                    double remainder = value % 1;
                    if (ScaleMode == ScaleModes.Fractional && remainder != 0)
                    {
                        float scaleValue = (float)Math.Pow(2d, remainder);
                        {
                            _mapRenderTransform = scaleValue;
                        }

                        ZoomStep = Convert.ToInt32(value - remainder);
                    }
                    else
                    {
                        _mapRenderTransform = null;
                        ZoomStep = (int)Math.Floor(value);
                        //zoomReal = ZoomStep;
                    }

                    if (Core.IsStarted && !IsDragging)
                    {
                        ForceUpdateLayers();
                    }
                }
            }
        }

        /// <summary>
        ///     map zoom level
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal int ZoomStep
        {
            get
            {
                return Core.Zoom;
            }
            set
            {
                if (value > MaxZoom)
                {
                    Core.Zoom = MaxZoom;
                }
                else if (value < MinZoom)
                {
                    Core.Zoom = MinZoom;
                }
                else
                {
                    Core.Zoom = value;
                }
            }
        }

        /// <summary>
        ///     current map center position
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public PointLatLng Position
        {
            get
            {
                return Core.Position;
            }
            set
            {
                Core.Position = value;

                if (Core.IsStarted)
                {
                    ForceUpdateLayers();
                }
            }
        }

        /// <summary>
        ///     current position in pixel coordinates
        /// </summary>
        [Browsable(false)]
        public GPoint PositionPixel
        {
            get
            {
                return Core.PositionPixel;
            }
        }

        /// <summary>
        ///     location of cache
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string CacheLocation
        {
            get
            {
#if !DESIGN
                return CacheLocator.Location;
#else
            return string.Empty;
#endif
            }
            set
            {
#if !DESIGN
                CacheLocator.Location = value;
#endif
            }
        }



        bool _isMouseOverObject;
        internal int OverObjectCount;

        /// <summary>
        ///     is mouse over marker
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsMouseOverObject
        {
            get
            {
                return _isMouseOverObject;
            }
            internal set
            {
                _isMouseOverObject = value;
                OverObjectCount += value ? 1 : -1;
            }
        }


        /// <summary>
        ///     gets current map view top/left coordinate, width in Lng, height in Lat
        /// </summary>
        [Browsable(false)]
        public RectLatLng ViewArea
        {
            get
            {
                if (!IsRotated)
                {
                    return Core.ViewArea;
                }
                else if (Core.Provider.Projection != null)
                {
                    var p = FromLocalToLatLng(0, 0);
                    var p2 = FromLocalToLatLng(Width, Height);

                    return RectLatLng.FromLTRB(p.Lng, p.Lat, p2.Lng, p2.Lat);
                }

                return RectLatLng.Empty;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GMapProvider MapProvider
        {
            get
            {
                return Core.Provider;
            }

            set
            {
                if (value != null && (Core.Provider == null || !Core.Provider.Equals(value)))
                {
                    Debug.WriteLine("MapType: " + Core.Provider.Name + " -> " + value.Name);

                    var viewarea = SelectedArea;

                    if (viewarea != RectLatLng.Empty)
                    {
                        Position = new PointLatLng(viewarea.Lat - viewarea.HeightLat / 2,
                            viewarea.Lng + viewarea.WidthLng / 2);
                    }
                    else
                    {
                        viewarea = ViewArea;
                    }

                    Core.Provider = value;

                    if (Core.IsStarted)
                    {
                        if (Core.ZoomToArea)
                        {
                            // restore zoomrect as close as possible
                            if (viewarea != RectLatLng.Empty && viewarea != ViewArea)
                            {
                                int bestZoom = Core.GetMaxZoomToFitRect(viewarea);
                                if (bestZoom > 0 && Zoom != bestZoom)
                                {
                                    Zoom = bestZoom;
                                }
                            }
                        }
                        else
                        {
                            ForceUpdateLayers();
                        }
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RoutingProvider RoutingProvider
        {
            get
            {
                var dp = MapProvider as RoutingProvider;

                if (dp == null)
                    dp = GMapProviders
                        .OpenStreetMap as RoutingProvider; // use OpenStreetMap if provider does not implement routing

                return dp;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DirectionsProvider? DirectionsProvider
        {
            get
            {
                var dp = MapProvider as DirectionsProvider;

                if (dp == null)
                    dp = GMapProviders
                            .OpenStreetMap as
                        DirectionsProvider; // use OpenStreetMap if provider does not implement routing

                return dp;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GeocodingProvider GeocodingProvider
        {
            get
            {
                var dp = MapProvider as GeocodingProvider;

                if (dp == null)
                    dp = GMapProviders
                        .OpenStreetMap as GeocodingProvider; // use OpenStreetMap if provider does not implement routing

                return dp;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RoadsProvider RoadsProvider
        {
            get
            {
                var dp = MapProvider as RoadsProvider;

                if (dp == null)
                    dp = GMapProviders
                        .GoogleMap as RoadsProvider; // use GoogleMap if provider does not implement routing

                return dp;
            }
        }

        /// <summary>
        ///     is routes enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool RoutesEnabled
        {
            get
            {
                return Core.RoutesEnabled;
            }
            set
            {
                Core.RoutesEnabled = value;
            }
        }

        /// <summary>
        ///     is polygons enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool PolygonsEnabled
        {
            get
            {
                return Core.PolygonsEnabled;
            }
            set
            {
                Core.PolygonsEnabled = value;
            }
        }


        /// <summary>
        ///     is lines enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool LinesEnabled
        {
            get
            {
                return Core.LinesEnabled;
            }
            set
            {
                Core.LinesEnabled = value;
            }
        }

        /// <summary>
        ///     is text enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool LabelEnabled
        {
            get
            {
                return Core.TextEnabled;
            }
            set
            {
                Core.TextEnabled = value;
            }
        }

        /// <summary>
        ///     is ellipses enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool EllipsesEnabled
        {
            get
            {
                return Core.EllipsesEnabled;
            }
            set
            {
                Core.EllipsesEnabled = value;
            }
        }

        /// <summary>
        ///     is ellipses enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool RectanglesEnabled
        {
            get
            {
                return Core.RectanglesEnabled;
            }
            set
            {
                Core.RectanglesEnabled = value;
            }
        }


        /// <summary>
        ///     is markers enabled
        /// </summary>
        [Category("GMap.NET")]
        public bool MarkersEnabled
        {
            get
            {
                return Core.MarkersEnabled;
            }
            set
            {
                Core.MarkersEnabled = value;
            }
        }

        #endregion IGControl Members


        #region IGControl event Members

        /// <summary>
        ///     occurs when current position is changed
        /// </summary>
        public event PositionChanged OnPositionChanged
        {
            add
            {
                Core.OnCurrentPositionChanged += value;
            }
            remove
            {
                Core.OnCurrentPositionChanged -= value;
            }
        }

        /// <summary>
        ///     occurs when tile set load is complete
        /// </summary>
        public event TileLoadComplete OnTileLoadComplete
        {
            add
            {
                Core.OnTileLoadComplete += value;
            }
            remove
            {
                Core.OnTileLoadComplete -= value;
            }
        }

        /// <summary>
        ///     occurs when tile set is starting to load
        /// </summary>
        public event TileLoadStart OnTileLoadStart
        {
            add
            {
                Core.OnTileLoadStart += value;
            }
            remove
            {
                Core.OnTileLoadStart -= value;
            }
        }

        /// <summary>
        ///     occurs on map drag
        /// </summary>
        public event MapDrag OnMapDrag
        {
            add
            {
                Core.OnMapDrag += value;
            }
            remove
            {
                Core.OnMapDrag -= value;
            }
        }

        /// <summary>
        ///     occurs on map zoom changed
        /// </summary>
        public event MapZoomChanged OnMapZoomChanged
        {
            add
            {
                Core.OnMapZoomChanged += value;
            }
            remove
            {
                Core.OnMapZoomChanged -= value;
            }
        }

        /// <summary>
        ///     occures on map type changed
        /// </summary>
        public event MapTypeChanged OnMapTypeChanged
        {
            add
            {
                Core.OnMapTypeChanged += value;
            }
            remove
            {
                Core.OnMapTypeChanged -= value;
            }
        }

        /// <summary>
        ///     occurs on empty tile displayed
        /// </summary>
        public event EmptyTileError OnEmptyTileError
        {
            add
            {
                Core.OnEmptyTileError += value;
            }
            remove
            {
                Core.OnEmptyTileError -= value;
            }
        }

        #endregion IGControl event Members

        #region Serialization

        //static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        /// <summary>
        ///     Serializes the overlays.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SerializeOverlays(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // Create an array from the overlays
            var layerArray = new Layer[Layers.Count];
            Layers.CopyTo(layerArray, 0);

            // Serialize the overlays
            //BinaryFormatter.Serialize(stream, layerArray);
        }

        /// <summary>
        ///     De-serializes the overlays.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void DeserializeOverlays(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // De-serialize the overlays
            //var layerArray = BinaryFormatter.Deserialize(stream) as Layer[];

            //// Populate the collection of overlays.
            //foreach (var layer in layerArray)
            //{
            //    layer.Control = this;
            //    Layers.Add(layer);
            //}
            //ForceUpdateOverlays();
        }

        #endregion Serialization
    } /* class GMapControl */


    /*******************************************************************************************************************************/

    #region Enums
    public enum ScaleModes
    {
        Integer,    // no scaling
        Fractional, // scales to fractional level, CURRENT VERSION DOESN'T HANDLE OBJECT POSITIONS CORRECLTY,http://greatmaps.codeplex.com/workitem/16046
    }

    public enum HelperLineOptions
    {
        DontShow = 0,
        ShowAlways = 1,
        ShowOnModifierKey = 2
    }

    public enum MapScaleInfoPositions
    {
        Top,
        Bottom
    }

    public enum MouseMoveStatusType
    {
        None,
        Drag,
        Line,
        Arrow,
        Rectangle,
        Circle,
        Ellipse,
        Ruler,
        CircleRuler,
        Textbox,
        Marker,
        Ray,
        MovePoint,
    }

    public enum ArrowPositions
    {
        None,
        AtBegin,
        AtEnd,
        Both,
    }

    public enum MovementType
    {
        None,
        Horizontal,
        Vertical,
        Both
    }

    public enum MarkerTooltipMode
    {
        OnMouseOver,
        Never,
        Always,
    }

    public enum MarketTooltipTextType
    {
        None,
        MarkerName,
        Location,
        Address,
        LocationAdderss,
        DF,
        Custom,
    }


    #endregion Enums

    #region Delegates
    public delegate void SelectionChange(RectLatLng selection, bool zoomToFit);

    public delegate void MapClick(PointLatLng pointClick, MouseEventArgs e);

    public delegate void MapDoubleClick(PointLatLng pointClick, MouseEventArgs e);

    public delegate void ShowPlaceInfo(int x, int y);
    public delegate void ShowPlaceInfoEx(int x, int y, double bearing, double distance);

    public delegate void TextBoxEdit(MapTextBox textbox);

    #endregion Delegates

} /* namespace GMap.NET.WindowsForms */
