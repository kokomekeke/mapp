using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Demo.WindowsForms.CustomMarkers;
using Demo.WindowsForms.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using static System.Net.Mime.MediaTypeNames;
using static GMap.NET.Entity.OpenStreetMapRouteEntity;

namespace Demo.WindowsForms
{
    public partial class MainForm : Form
    {
        public static MainForm Instance = null;

        // layers
        readonly GMapOverlay _top = new GMapOverlay();
        internal readonly GMapOverlay Objects = new GMapOverlay("objects");
        internal readonly GMapOverlay Routes = new GMapOverlay("routes");
        internal readonly GMapOverlay Polygons = new GMapOverlay("polygons");
        internal readonly GMapOverlay Geogrid = new GMapOverlay("geogrid");
        internal readonly GMapOverlay Geogrid10 = new GMapOverlay("geogrid10");

        // marker
        GMapMarker _currentMarker;
        GMapMarkerRect _currentRectMarker;
        GMarkerHFsystem _currentHFmarker;
        GMarkerHFsystem _tempHFmarker;
        GMarkerLinePoint _currentLinePoint;
        GMapPolyLine _currentPolyline;
        PointLatLng _currentLatLong;

        // polygons
        GMapPolyLine _polygon;

        // etc
        readonly Random _rnd = new Random();
        readonly DescendingComparer _comparerIpStatus = new DescendingComparer();
        string _mobileGpsLog = string.Empty;
        bool _isMouseDown;
        PointLatLng _start;
        PointLatLng _end;
        int _lineindex = 0;
        GMarkerCross _startMarker;
        GMarkerCross _endMarker;

        // macro
        string _filename = string.Empty;


        public MainForm()
        {
            InitializeComponent();

            Instance = this;

            if (!GMapControl.IsDesignerHosted)
            {
                // add your custom map db provider
                //MsSQLPureImageCache ch = new MsSQLPureImageCache();
                //ch.ConnectionString = @"data source = sql5040.site4now.net;User Id=DB_A3B2C9_GMapNET_admin; initial catalog = DB_A3B2C9_GMapNET; password = Usuario@2018;";                
                //MainMap.Manager.SecondaryCache = ch;

                // set your proxy here if need
                //GMapProvider.IsSocksProxy = true;
                //GMapProvider.WebProxy = new WebProxy("127.0.0.1", 1080);
                //GMapProvider.WebProxy.Credentials = new NetworkCredential("ogrenci@bilgeadam.com", "bilgeada");
                // or
                //GMapProvider.WebProxy = WebRequest.DefaultWebProxy;
                //                          

                // set cache mode only if no internet available
                if (!Stuff.PingNetwork(Program.pingaddress))
                {
                    //MainMap.Manager.Mode = AccessMode.CacheOnly;
                    //MessageBox.Show("No internet connection available, going to CacheOnly mode.", "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MessageBox.Show("No internet connection available", "GMap.NET - Demo.WindowsForms", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                Text += " - HFsystem version - " + " Build=" + AssemblyCreationDate().ToString("yyyy.MM.dd HH:mm");


                //----------------------------------------
                // Config Map at Startup
                //----------------------------------------
                MainMap.MapProvider = Program.mapProvider;  //GMapProviders.GoogleMap;
                MainMap.Manager.Mode = Program.accesmode;

                OpenStreetMapGraphHopperProvider.Instance.ApiKey = Stuff.OpenStreetMapsGraphHopperApiKey;
                GoogleMapProvider.Instance.ApiKey = Stuff.GoogleMapsApiKey;

                // Custom Map Provider
                //MainMap.MapProvider = GMapProviders.CustomMap;
                //GMapProviders.CustomMap.CustomServerUrl = "https://{l}.tile.openstreetmap.org/{z}/{x}/{y}.png";
                //GMapProviders.CustomMap.CustomServerLetters = "abc";

                //----------------------------------------
                // Initial Position
                //----------------------------------------
                //MainMap.Position = new PointLatLng(54.6961334816182, 25.2985095977783); // Lithuania, Vilnius
                MainMap.Position = new PointLatLng(Program.initialLatitude, Program.initialLongitude);
                MainMap.MinZoom = 0;
                MainMap.MaxZoom = 24;
                MainMap.Zoom = Program.initialZoom;

                textBoxLat.Text = MainMap.Position.Lat.ToString(CultureInfo.InvariantCulture);
                textBoxLng.Text = MainMap.Position.Lng.ToString(CultureInfo.InvariantCulture);
                textBoxGeo.Text = Program.settlement;   //  "Lithuania, Vilnius";
              

                MainMap.ScaleMode = ScaleModes.Fractional;

                //----------------------------------------
                // Map Events
                //----------------------------------------
                MainMap.OnPositionChanged += MainMap_OnPositionChanged;
                MainMap.OnTileLoadStart += MainMap_OnTileLoadStart;
                MainMap.OnTileLoadComplete += MainMap_OnTileLoadComplete;
                MainMap.OnMapClick += MainMap_OnMapClick;

                MainMap.OnMapZoomChanged += MainMap_OnMapZoomChanged;
                MainMap.OnMapTypeChanged += MainMap_OnMapTypeChanged;

                MainMap.OnMarkerClick += MainMap_OnMarkerClick;
                MainMap.OnMarkerDoubleClick += MainMap_OnMarkerDoubleClick;

                MainMap.OnMarkerEnter += MainMap_OnMarkerEnter;
                MainMap.OnMarkerLeave += MainMap_OnMarkerLeave;

                MainMap.OnPolygonEnter += MainMap_OnPolygonEnter;
                MainMap.OnPolygonLeave += MainMap_OnPolygonLeave;

                MainMap.OnPolygonClick += MainMap_OnPolygonClick;
                MainMap.OnPolygonDoubleClick += MainMap_OnPolygonDoubleClick;

                MainMap.OnRouteEnter += MainMap_OnRouteEnter;
                MainMap.OnRouteLeave += MainMap_OnRouteLeave;

                MainMap.OnRouteClick += MainMap_OnRouteClick;
                MainMap.OnRouteDoubleClick += MainMap_OnRouteDoubleClick;

                MainMap.Manager.OnTileCacheComplete += OnTileCacheComplete;
                MainMap.Manager.OnTileCacheStart += OnTileCacheStart;
                MainMap.Manager.OnTileCacheProgress += OnTileCacheProgress;

                MainMap.MouseMove += MainMap_MouseMove;
                MainMap.MouseDown += MainMap_MouseDown;
                MainMap.MouseUp += MainMap_MouseUp;
                MainMap.MouseDoubleClick += MainMap_MouseDoubleClick;

                //MainMap.OnDraggedDone += AlignGeoLabels;

                //----------------------------------------
                // Custom Layers
                //----------------------------------------
                MainMap.Overlays.Add(Routes);
                MainMap.Overlays.Add(Polygons);
                MainMap.Overlays.Add(Geogrid);
                MainMap.Overlays.Add(Geogrid10);
                MainMap.Overlays.Add(Objects);
                MainMap.Overlays.Add(_top);

                //----------------------------------------
                // Other Events
                //----------------------------------------
                Routes.Routes.CollectionChanged += Routes_CollectionChanged;
                Objects.Markers.CollectionChanged += Markers_CollectionChanged;

                //----------------------------------------
                // Background Workers
                //----------------------------------------

                // vehicle demo ([jokubokla]: Doesn't seem to work anymore, to be investigated)
                _transportWorker.DoWork += transport_DoWork;
                _transportWorker.ProgressChanged += transport_ProgressChanged;
                _transportWorker.WorkerSupportsCancellation = true;
                _transportWorker.WorkerReportsProgress = true;

                // performance demo
                _timerPerf.Tick += timer_Tick;

                //----------------------------------------
                // Populate Map ComboBox
                //----------------------------------------
#if !MONO       // mono doesn't handle it, so we 'lost' provider list ;]
                comboBoxMapType.ValueMember = "Name";
                comboBoxMapType.DataSource = GMapProviders.List;
                comboBoxMapType.SelectedItem = MainMap.MapProvider;
#endif
                // acccess mode
                MainMap.Manager.Mode = Program.accesmode;
                comboBoxMode.DataSource = Enum.GetValues(typeof(AccessMode));
                comboBoxMode.SelectedItem = MainMap.Manager.Mode;

                // get cache modes
                //checkBoxUseRouteCache.Checked = MainMap.Manager.UseRouteCache;

                // get zoom  
                //trackBarZoom.Minimum = MainMap.MinZoom * 100;
                //trackBarZoom.Maximum = MainMap.MaxZoom * 100;
                //trackBarZoom.TickFrequency = 100;

#if DEBUG
                checkBox_Debug.Checked = false;
                MainMap.ShowTileGridLines = false;
#endif

                // set current marker
                _currentMarker = new GMarkerGoogle(MainMap.Position, GMarkerGoogleType.arrow);
                _currentMarker.IsHitTestVisible = false;
                _top.Markers.Add(_currentMarker);


                // add my city location for demo
                // [jokubokla]: The stuff down below doesn't work anymore either, but I leave it in case someone wants to fix it
                //GeoCoderStatusCode status;
                //var pos = MainMap.GeocodingProvider.GetPoint("Lithuania, Vilnius", out status);

                //if (pos != null && status == GeoCoderStatusCode.OK)
                //{
                //    _currentMarker.Position = pos.Value;

                //    GMapMarker myCity = new GMarkerGoogle(pos.Value, GMarkerGoogleType.green_small);
                //    myCity.ToolTipMode = MarkerTooltipMode.Always;
                //    myCity.ToolTipText = "Welcome to Lithuania! ;}";
                //    Objects.Markers.Add(myCity);

                //    // add some more points in lithuania
                //    AddLocationLithuania("Kaunas");
                //    AddLocationLithuania("Klaipėda");
                //    AddLocationLithuania("Šiauliai");
                //    AddLocationLithuania("Panevėžys");
                //}
                //else
                //{
                //    Debug.WriteLine("GeoCoderStatusCode = " + status.ToString()); // REQUEST_DENIED I assume
                //}
                //
                //if (Objects.Markers.Count > 0)
                //{
                //    MainMap.ZoomAndCenterMarkers(null);
                //}

                //RegeneratePolygon();

                GenerateGeoGrid();
                Geogrid.IsVisibile = false;
                Geogrid10.IsVisibile = false;

                try
                {
                    var overlay = DeepClone(Objects);
                    Debug.WriteLine("ISerializable status for markers: OK");

                    var overlay2 = DeepClone(Polygons);
                    Debug.WriteLine("ISerializable status for polygons: OK");

                    var overlay3 = DeepClone(Routes);
                    Debug.WriteLine("ISerializable status for routes: OK");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ISerializable failure: " + ex.Message);
#if DEBUG
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
#endif
                }
            }

            routingToolStripMenuItem.Checked = MainMap.Manager.UseRouteCache;
            geocodingToolStripMenuItem.Checked = MainMap.Manager.UseGeocoderCache;

            MainMap.Manager.Mode = AccessMode.ServerOnly;
        } /* MainForm constructor */


        // center markers on start
        private void MainForm_Load(object sender, EventArgs e)
        {
            //trackBarZoom.Value = (int)MainMap.Zoom * 100;
            Activate();
            TopMost = true;
            TopMost = false;
        }

        public static DateTime AssemblyCreationDate()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fi = new FileInfo(asm.Location);
            return fi.LastWriteTime; //itt eredetileg itt .CreationTime volt, de ez igy jobban követi a lokal re-buldeket is B
        }


        private void MainMap_OnMapClick(PointLatLng pointClick, MouseEventArgs e)
        {
            MainMap.FromLocalToLatLng(e.X, e.Y);
        }

        private void MainMap_OnRouteDoubleClick(GMapRoute item, MouseEventArgs e)
        {
            
        }

        private void MainMap_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
           
        }

        private void MainMap_OnPolygonDoubleClick(GMapPolyLine item, MouseEventArgs e)
        {
            
        }

        private void MainMap_OnPolygonClick(GMapPolyLine item, MouseEventArgs e)
        {
            
        }

        private void MainMap_OnMarkerDoubleClick(GMapMarker item, MouseEventArgs e)
        {
            
        }

        public T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                formatter.Serialize(ms, obj);

                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        void Markers_CollectionChanged(object sender, GMap.NET.ObjectModel.NotifyCollectionChangedEventArgs e)
        {
            //textBoxMarkerCount.Text = Objects.Markers.Count.ToString();
        }

        void Routes_CollectionChanged(object sender, GMap.NET.ObjectModel.NotifyCollectionChangedEventArgs e)
        {
            //textBoxRouteCount.Text = Routes.Routes.Count.ToString();
        }

        #region -- performance test --

        double NextDouble(Random rng, double min, double max)
        {
            return min + (rng.NextDouble() * (max - min));
        }

        int _tt;
        void timer_Tick(object sender, EventArgs e)
        {
            var pos = new PointLatLng(NextDouble(_rnd, MainMap.ViewArea.Top, MainMap.ViewArea.Bottom), NextDouble(_rnd, MainMap.ViewArea.Left, MainMap.ViewArea.Right));
            GMapMarker m = new GMarkerGoogle(pos, GMarkerGoogleType.green_pushpin);
            {
                m.ToolTipText = (_tt++).ToString();
                m.ToolTipMode = MarkerTooltipMode.Always;
            }

            Objects.Markers.Add(m);

            if (_tt >= 333)
            {
                _timerPerf.Stop();
                _tt = 0;
            }
        }

        System.Windows.Forms.Timer _timerPerf = new System.Windows.Forms.Timer();
        #endregion


        #region -- transport demo --

        // [jokubokla]: The transport demo doesn't seem to work. Presumably because a public transportation
        // webservice in Vilnius has a new API

        BackgroundWorker _transportWorker = new BackgroundWorker();


        readonly List<VehicleData> _trolleybus = new List<VehicleData>();
        readonly Dictionary<int, GMapMarker> _trolleybusMarkers = new Dictionary<int, GMapMarker>();

        readonly List<VehicleData> _bus = new List<VehicleData>();
        readonly Dictionary<int, GMapMarker> _busMarkers = new Dictionary<int, GMapMarker>();

        bool _firstLoadTrasport = true;
        GMapMarker _currentTransport;

        void transport_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // stops immediate marker/route/polygon invalidations;
            // call Refresh to perform single refresh and reset invalidation state
            MainMap.HoldInvalidation = true;

            lock (_trolleybus)
            {
                foreach (var d in _trolleybus)
                {
                    GMapMarker marker;

                    if (!_trolleybusMarkers.TryGetValue(d.Id, out marker))
                    {
                        marker = new GMarkerGoogle(new PointLatLng(d.Lat, d.Lng), GMarkerGoogleType.red_small);
                        marker.Tag = d.Id;
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                        _trolleybusMarkers[d.Id] = marker;
                        Objects.Markers.Add(marker);
                    }
                    else
                    {
                        marker.Position = new PointLatLng(d.Lat, d.Lng);
                        //(marker as GMarkerGoogle).Bearing = (float?) d.Bearing;
                    }
                    marker.ToolTipText = "Trolley " + d.Line + (d.Bearing.HasValue ? ", bearing: " + d.Bearing.Value.ToString() : string.Empty) + ", " + d.Time;

                    if (_currentTransport != null && _currentTransport == marker)
                    {
                        MainMap.Position = marker.Position;
                        if (d.Bearing.HasValue)
                        {
                            MainMap.Bearing = (float)d.Bearing.Value;
                        }
                    }
                }
            }

            lock (_bus)
            {
                foreach (var d in _bus)
                {
                    GMapMarker marker;

                    if (!_busMarkers.TryGetValue(d.Id, out marker))
                    {
                        marker = new GMarkerGoogle(new PointLatLng(d.Lat, d.Lng), GMarkerGoogleType.green_small);
                        marker.Tag = d.Id;
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                        _busMarkers[d.Id] = marker;
                        Objects.Markers.Add(marker);
                    }
                    else
                    {
                        marker.Position = new PointLatLng(d.Lat, d.Lng);
                        //(marker as GMarkerGoogle).Bearing = (float?) d.Bearing;
                    }
                    marker.ToolTipText = "Bus " + d.Line + (d.Bearing.HasValue ? ", bearing: " + d.Bearing.Value.ToString() : string.Empty) + ", " + d.Time;

                    if (_currentTransport != null && _currentTransport == marker)
                    {
                        MainMap.Position = marker.Position;
                        if (d.Bearing.HasValue)
                        {
                            MainMap.Bearing = (float)d.Bearing.Value;
                        }
                    }
                }
            }

            if (_firstLoadTrasport)
            {
                MainMap.Zoom = 5;
                MainMap.ZoomAndCenterMarkers("objects");
                _firstLoadTrasport = false;
            }
            MainMap.Refresh();
        }

        void transport_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_transportWorker.CancellationPending)
            {
                try
                {
                    #region -- old vehicle demo --
                    lock (_trolleybus)
                    {
                        Stuff.GetVilniusTransportData(TransportType.TrolleyBus, string.Empty, _trolleybus);
                    }

                    lock (_bus)
                    {
                        Stuff.GetVilniusTransportData(TransportType.Bus, string.Empty, _bus);
                    }
                    #endregion

                    _transportWorker.ReportProgress(100);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("transport_DoWork: " + ex.ToString());
                }
                Thread.Sleep(2 * 1000);
            }

            _trolleybusMarkers.Clear();
            _busMarkers.Clear();
        }

        #endregion


        #region -- some functions --
        /// <summary>
        /// generates polygon from rect markers, using marker.Tag to keep point index in polygon
        /// </summary>
        void RegeneratePolygon()
        {
            var polygonPoints = new List<PointLatLng>();

            foreach (var m in Objects.Markers)
            {
                if (m is GMapMarkerRect)
                {
                    m.Tag = polygonPoints.Count;
                    polygonPoints.Add(m.Position);
                }
            }

            if (_polygon == null)
            {
                _polygon = new GMapPolyLine(polygonPoints, "polygon test");
                _polygon.IsHitTestVisible = true;
                Polygons.Polygons.Add(_polygon);
            }
            else
            {
                _polygon.Points.Clear();
                _polygon.Points.AddRange(polygonPoints);

                if (Polygons.Polygons.Count == 0)
                {
                    Polygons.Polygons.Add(_polygon);
                }
                else
                {
                    MainMap.UpdatePolygonLocalPosition(_polygon);
                }
            }
        } /* RegeneratePolygon */


        private void GenerateGeoGrid()
        {
            Geogrid.Lines.Clear();
            Geogrid.ScaleLabels.Clear();
            double lng1 = -180;
            double lng2 = 180;

            for (int i = -90; i < 90; i++)
            {
                double lat = i;
                double lng = 20;
                var p1 = new PointLatLng(lat, lng1);
                var p2 = new PointLatLng(lat, lng2);
                var line = new GMapLine(p1, p2, Color.Black);
                Geogrid.Lines.Add(line);
                var text = new GMapScaleLabel(new PointLatLng(lat, lng), lat.ToString(), new Font("Arial", 12), AnchorType.Left);
                Geogrid.ScaleLabels.Add(text);
            }

            for (int i = -170; i < 180; i++)
            {
                double lng = i;
                double lat = 45;
                var p1 = new PointLatLng(-90, lng);
                var p2 = new PointLatLng(90, lng);
                var line = new GMapLine(p1, p2, Color.Black);
                Geogrid.Lines.Add(line);
                var text = new GMapScaleLabel(new PointLatLng(lat, lng), lng.ToString(), new Font("Arial", 12), AnchorType.Top);
                Geogrid.ScaleLabels.Add(text);
            }

            Geogrid10.Lines.Clear();
            Geogrid10.Labels.Clear();
            for (int i = -90; i < 90; i += 10)
            {
                double lat = i;
                double lng = 20;
                var p1 = new PointLatLng(lat, lng1);
                var p2 = new PointLatLng(lat, lng2);
                var line = new GMapLine(p1, p2, Color.Black);
                Geogrid10.Lines.Add(line);
                var text = new GMapScaleLabel(new PointLatLng(lat, lng), lat.ToString(), new Font("Arial", 12), AnchorType.Left);
                Geogrid10.ScaleLabels.Add(text);
            }

            for (int i = -170; i < 180; i+= 10)
            {
                double lng = i;
                double lat = 45;
                var p1 = new PointLatLng(-90, lng);
                var p2 = new PointLatLng(90, lng);
                var line = new GMapLine(p1, p2, Color.Black);
                Geogrid10.Lines.Add(line);
                var text = new GMapScaleLabel(new PointLatLng(lat, lng), lng.ToString(), new Font("Arial", 12), AnchorType.Top);
                Geogrid10.ScaleLabels.Add(text);
            }

        } /* GenerateGeoGrid */


        //public void AlignGeoLabels()
        //{
        //    foreach (var l in Geogrid.Labels)
        //    {
        //        l.UpdateLocalPosition(); 
        //    }

        //    foreach (var l in Geogrid10.Labels)
        //    {
        //        l.UpdateLocalPosition(); 
        //    }

        //} /* AlignGeoLabels */  


        /// <summary>
        /// adds marker using geocoder
        /// </summary>
        /// <param name="place"></param>
        void AddLocationLithuania(string place)
        {
            GeoCoderStatusCode status;
            var pos = MainMap.GeocodingProvider.GetPoint("Lithuania, " + place, out status);
            if (pos != null && status == GeoCoderStatusCode.OK)
            {
                var m = new GMarkerGoogle(pos.Value, GMarkerGoogleType.green);
                m.ToolTip = new GMapRoundedToolTip(m);

                var mBorders = new GMapMarkerRect(pos.Value);
                {
                    mBorders.InnerMarker = m;
                    mBorders.ToolTipText = place;
                    mBorders.ToolTipMode = MarkerTooltipMode.Always;
                }

                Objects.Markers.Add(m);
                Objects.Markers.Add(mBorders);
            }
        }

        bool TryExtractLeafletjs()
        {
            try
            {
                string launch = string.Empty;

                string[] x = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string f in x)
                {
                    if (f.Contains("leafletjs"))
                    {
                        string fName = f.Replace("Demo.WindowsForms.", string.Empty);
                        fName = fName.Replace(".", "\\");
                        int ll = fName.LastIndexOf("\\");
                        string name = fName.Substring(0, ll) + "." + fName.Substring(ll + 1, fName.Length - ll - 1);

                        //Demo.WindowsForms.leafletjs.dist.leaflet.js

                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(f))
                        {
                            string fileFullPath = MainMap.CacheLocation + name;

                            if (fileFullPath.Contains("gmap.html"))
                            {
                                launch = fileFullPath;
                            }

                            string dir = Path.GetDirectoryName(fileFullPath);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }

                            using (var fileStream = File.Create(fileFullPath, (int)stream.Length))
                            {
                                // Fill the bytes[] array with the stream data
                                byte[] bytesInStream = new byte[stream.Length];
                                stream.Read(bytesInStream, 0, bytesInStream.Length);

                                // Use FileStream object to write to the specified file
                                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(launch))
                {
                    Process.Start(launch);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TryExtractLeafletjs: " + ex);
                return false;
            }
            return true;
        }

        #endregion


        #region -- cache events --

        void OnTileCacheComplete()
        {
            Debug.WriteLine("OnTileCacheComplete");
            long size = 0;
            int db = 0;
            try
            {
                var di = new DirectoryInfo(MainMap.CacheLocation);
                var dbs = di.GetFiles("*.gmdb", SearchOption.AllDirectories);
                foreach (var d in dbs)
                {
                    size += d.Length;
                    db++;
                }
            }
            catch
            {
            }

            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    //textBoxCacheSize.Text = string.Format(CultureInfo.InvariantCulture, "{0} db in {1:00} MB", db, size / (1024.0 * 1024.0));
                    progressBar1.Visible = false;
                    groupBoxProgress.Text = "all tiles saved!";
                };

                if (!IsDisposed)
                {
                    try
                    {
                        Invoke(m);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        void OnTileCacheStart()
        {
            Debug.WriteLine("OnTileCacheStart");

            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    progressBar1.Visible = true;
                    progressBar1.Value = progressBar1.Minimum;
                    groupBoxProgress.Text = "saving tiles...";
                };
                Invoke(m);
            }
        }

        void OnTileCacheProgress(int left)
        {
            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    progressBar1.Visible = true;
                    progressBar1.Value = (progressBar1.Value < progressBar1.Maximum) ?  (progressBar1.Value + 1) : progressBar1.Minimum;
                    groupBoxProgress.Text = left + " tiles to save...";
                };
                Invoke(m);
            }
        }

        #endregion


        #region -- map events --


        void MainMap_OnMarkerLeave(GMapMarker item)
        {
            if (item is GMapMarkerRect)
            {
                _currentRectMarker = null;

                var rc = item as GMapMarkerRect;
                rc.Pen.Color = Color.Blue;

                Debug.WriteLine("OnMarkerLeave: " + item.Position);
            }
            else if (item is GMarkerHFsystem)
            {
                _currentHFmarker = null;
            }
            else if (item is GMarkerLinePoint)
            {
                _currentLinePoint = null;
            }
        }

        void MainMap_OnMarkerEnter(GMapMarker item)
        {
            if (item is GMapMarkerRect)
            {
                var rc = item as GMapMarkerRect;
                rc.Pen.Color = Color.Red;

                _currentRectMarker = rc;
            }
            else if (item is GMarkerHFsystem)
            {
                _currentHFmarker = item as GMarkerHFsystem;
            }
            else if (item is GMarkerLinePoint)
            {
                _currentLinePoint = item as GMarkerLinePoint;
                if (_currentLinePoint.Parent is GMapPolyLine polygon)
                {
                    _currentPolyline = polygon;
                }
            }
            Debug.WriteLine("OnMarkerEnter: " + item.Position);
        }

        void MainMap_OnPolygonLeave(GMapPolyLine item)
        {
            _currentPolyline = null;
            item.Stroke.Color = Color.MidnightBlue;
            Debug.WriteLine("OnPolygonLeave: " + item.Name);
        }

        void MainMap_OnPolygonEnter(GMapPolyLine item)
        {
            _currentPolyline = item;
            item.Stroke.Color = Color.Red;
            Debug.WriteLine("OnPolygonEnter: " + item.Name);
        }

        GMapRoute _currentRoute;
        void MainMap_OnRouteLeave(GMapRoute item)
        {
            _currentRoute = null;
            item.Stroke.Color = Color.MidnightBlue;
            Debug.WriteLine("OnRouteLeave: " + item.Name);
        }

        void MainMap_OnRouteEnter(GMapRoute item)
        {
            _currentRoute = item;
            item.Stroke.Color = Color.Red;
            Debug.WriteLine("OnRouteEnter: " + item.Name);
        }

        void MainMap_OnMapTypeChanged(GMapProvider type)
        {
            comboBoxMapType.SelectedItem = type;

            //trackBarZoom.Minimum = MainMap.MinZoom * 100;
            //trackBarZoom.Maximum = MainMap.MaxZoom * 100;

        }

        void MainMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = false;
            }

            if (e.Button == MouseButtons.Right)
            {
                _isMouseDown = false;
            }
        }

        // add demo circle
        void MainMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var cc = new GMapMarkerCircle(MainMap.FromLocalToLatLng(e.X, e.Y));
            Objects.Markers.Add(cc);
        }

        void MainMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = true;

                if (_currentMarker.IsVisible)
                {
                    _currentMarker.Position = MainMap.FromLocalToLatLng(e.X, e.Y);

                    var px = MainMap.MapProvider.Projection.FromLatLngToPixel(_currentMarker.Position.Lat, _currentMarker.Position.Lng, (int)MainMap.Zoom);
                    var tile = MainMap.MapProvider.Projection.FromPixelToTileXY(px);

                    Debug.WriteLine("MouseDown: geo: " + _currentMarker.Position + " | px: " + px + " | tile: " + tile);
                    statusStrip1.Items[0].Text = _currentMarker.ToGeoString();
                }

                foreach (var m in Objects.Markers)
                {
                    m.IsSelected = false;
                }

                MainMap.CheckSelection(MainMap.FromLocalToLatLng(e.X, e.Y));

            }
            else if (e.Button == MouseButtons.Right)
            {
                if ((_currentHFmarker != null) && _currentHFmarker.IsMouseOver)
                {
                    _isMouseDown = false;
                    MainMap.ClearSelection();

                    MenuItem mCenter = new MenuItem("Center");
                    MenuItem mInfobox = new MenuItem("Infobox");
                    MenuItem mHighlight = new MenuItem("Highlight");
                    MenuItem mCopy = new MenuItem("Copy");
                    MenuItem mDelete = new MenuItem("Delete");

                    mCenter.Click += MCenter_Click;
                    mInfobox.Click += MInfobox_Click;
                    mInfobox.Checked = _currentHFmarker.BallonState == GeoBallonState.geoDisplayBalloon;
                    mHighlight.Click += MHighlight_Click;
                    mHighlight.Checked = _currentHFmarker.Highlight;
                    mDelete.Click += MDelete_Click;
                    mCopy.Click += MCopy_Click;   
                    _tempHFmarker = _currentHFmarker;

                    ContextMenu m = new ContextMenu(new MenuItem[] { mCenter, mInfobox, mHighlight, mCopy, mDelete });
                    m.Show(MainMap, new Point(e.X, e.Y));
                    _isMouseDown = false;
                    _currentHFmarker = null;
                }
                else
                {
                    _currentLatLong = MainMap.FromLocalToLatLng(e.X, e.Y);

                    if (_currentRectMarker != null)
                    {
                        MenuItem mCenter = new MenuItem("Center");
                        MenuItem mCopy = new MenuItem("Copy");
                        MenuItem mDelete = new MenuItem("Delete");

                        mCenter.Click += MRecCenter_Click;
                        mDelete.Click += MRecDelete_Click;
                        mCopy.Click += MRecCopy_Click;
                        _tempHFmarker = _currentHFmarker;

                        ContextMenu m = new ContextMenu(new MenuItem[] { mCenter, mCopy, mDelete });
                        m.Show(MainMap, new Point(e.X, e.Y));
                        _isMouseDown = false;
                        _currentHFmarker = null;

                    }
                    else
                    { 
                        if (_currentPolyline == null)
                        {
                            foreach (var p in Polygons.Polygons)
                            {
                                if (p.IsInside(_currentLatLong))
                                {
                                    _currentPolyline = p;
                                    break;
                                }
                            }
                        }

                        if (_currentPolyline != null)
                        {
                            if (_currentPolyline.IsInside(_currentLatLong))
                            {
                                MainMap.ClearSelection();
                                _currentPolyline.IsSelected = true;

                                MenuItem m1 = new MenuItem("Add point");
                                MenuItem m2 = new MenuItem("Close curve");
                                MenuItem m3 = new MenuItem("Change color");

                                m1.Click += M1P_Click;
                                m2.Click += M2P_Click;
                                m3.Click += M3P_Click;

                                _tempHFmarker = _currentHFmarker;

                                ContextMenu m = new ContextMenu(new MenuItem[] { m1, m2, m3 });
                                m.Show(MainMap, new Point(e.X, e.Y));
                                _isMouseDown = false;
                                MainMap.ClearSelection();
                                MainMap.IsMouseOverPolygon = true;
                            }
                            else MainMap.IsMouseOverPolygon = false;
                        }
                    }
                }
            }

        } /* MainMap_MouseDown */

        private void MHighlight_Click(object sender, EventArgs e)
        {
            if (_tempHFmarker != null)
            {
                foreach (var m in Objects.Markers)
                {
                    if (m is GMarkerHFsystem)
                    {
                        var hf = m as GMarkerHFsystem;
                        if (hf != _tempHFmarker)
                            hf.Highlight = false;
                    }
                }
                
                _tempHFmarker.Highlight = !_tempHFmarker.Highlight;
                _isMouseDown = false;
                MainMap.ClearSelection();
            }
        }

        private void MDelete_Click(object sender, EventArgs e)
        {
            if ((_tempHFmarker != null) && _tempHFmarker.IsVisible)
            {
                Objects.Markers.Remove(_tempHFmarker);
                _tempHFmarker = null;
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }


        private void MInfobox_Click(object sender, EventArgs e)
        {
            if (_tempHFmarker != null)
            {
                if (_tempHFmarker.BallonState == GeoBallonState.geoDisplayBalloon)
                _tempHFmarker.BallonState = GeoBallonState.geoDisplayNone;
                else _tempHFmarker.BallonState = GeoBallonState.geoDisplayBalloon;
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }

        private void MCenter_Click(object sender, EventArgs e)
        {
            if (_tempHFmarker != null)
                MainMap.Position = _tempHFmarker.Position;   
            _isMouseDown = false;
            MainMap.ClearSelection();
        }

        private void MCopy_Click(object sender, EventArgs e)
        {
            if (_tempHFmarker != null)
            {
                _tempHFmarker.CopyToClipboard();
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }


        private void MRecDelete_Click(object sender, EventArgs e)
        {
            if ((_currentRectMarker != null) && _currentRectMarker.IsVisible)
            {
                if (_currentRectMarker.InnerMarker != null)
                    Objects.Markers.Remove(_currentRectMarker.InnerMarker);

                Objects.Markers.Remove(_currentRectMarker);
                _currentRectMarker = null;
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }


        private void MRecCenter_Click(object sender, EventArgs e)
        {
            if (_currentRectMarker != null)
                MainMap.Position = _currentRectMarker.Position;
            _isMouseDown = false;
            MainMap.ClearSelection();
        }

        private void MRecCopy_Click(object sender, EventArgs e)
        {
            if (_currentRectMarker != null)
            {
                _currentRectMarker.CopyToClipboard();
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }




        private void M1P_Click(object sender, EventArgs e)
        {
            if (_currentLatLong != null && _currentPolyline != null)
            {
                int index = _currentPolyline.IndexOfPoint(_currentLatLong);
                if (index >= 0)
                {
                    _currentPolyline.Insert(index, _currentLatLong);
                    MainMap.UpdatePolygonLocalPosition(_currentPolyline);
                }
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }

        private void M2P_Click(object sender, EventArgs e)
        {
            if (_currentPolyline != null)
            {
                _currentPolyline.IsClosed = true;
            }
            _isMouseDown = false;
            MainMap.ClearSelection();
        }

        private void M3P_Click(object sender, EventArgs e)
        {
            //.. change color
            _isMouseDown = false;
            MainMap.ClearSelection();

            var cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                _currentPolyline.LineColor = cd.Color;
            }
        }

        // move current marker with left holding
        void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isMouseDown)
            {
                if (_currentRectMarker == null)
                {
                    var pos = MainMap.FromLocalToLatLng(e.X, e.Y);

                    if (_currentMarker.IsVisible)
                    {
                        _currentMarker.Position = pos;
                        _currentMarker.ToolTipText = pos.ToGeoString();
                        statusStrip1.Items[0].Text = _currentMarker.ToGeoString();
                    }

                    if (_currentHFmarker != null)
                    {
                        _currentHFmarker.Position = pos;

                        if (_currentHFmarker != null)
                            _currentHFmarker.UpdateDefaultInfo();
                    }

                    if (_currentLinePoint != null)
                    {
                        _currentLinePoint.Position = pos;
                    }
                }
                else // move rect marker
                {
                    var pnew = MainMap.FromLocalToLatLng(e.X, e.Y);

                    int? pIndex = (int?)_currentRectMarker.Tag;
                    if (pIndex.HasValue)
                    {
                        if (pIndex < _polygon.Points.Count)
                        {
                            _polygon.Points[pIndex.Value] = pnew;
                            MainMap.UpdatePolygonLocalPosition(_polygon);
                        }
                    }

                    if (_currentMarker.IsVisible)
                    {
                        _currentMarker.Position = pnew;
                        _currentMarker.ToolTipText = pnew.ToGeoString();

                    }
                    _currentRectMarker.Position = pnew;
                    _currentRectMarker.UpdateToolTip();

                    if (_currentRectMarker.InnerMarker != null)
                    {
                        _currentRectMarker.InnerMarker.Position = pnew;
                        _currentRectMarker.InnerMarker.ToolTipText = pnew.ToGeoString();
                    }
                }

                MainMap.Refresh(); // force instant invalidation
            }
        }

        // MapZoomChanged
        void MainMap_OnMapZoomChanged()
        {
            //trackBarZoom.Value = (int)(MainMap.Zoom * 100.0);
            //textBoxZoomCurrent.Text = MainMap.Zoom.ToString();
            checkBoxShowGeoGrid_CheckedChanged(this, EventArgs.Empty);
            toolStripStatusLabel2.Text = "Zoom: " + MainMap.Zoom.ToString();
        }

        // click on some marker
        void MainMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (item is GMapMarkerRect)
                {
                    GeoCoderStatusCode status;
                    var pos = MainMap.GeocodingProvider.GetPlacemark(item.Position, out status);
                    if (status == GeoCoderStatusCode.OK && pos != null)
                    {
                        var v = item as GMapMarkerRect;
                        {
                            v.ToolTipText = pos.Value.Address;
                        }
                        MainMap.Invalidate(false);
                    }
                }
                else
                {
                    foreach (var m in Objects.Markers)
                    {
                        m.IsSelected = false;
                    }

                    item.IsSelected = true;

                    if (item.Tag != null)
                    {
                        if (_currentTransport != null)
                        {
                            _currentTransport.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            _currentTransport = null;
                        }
                        _currentTransport = item;
                        _currentTransport.ToolTipMode = MarkerTooltipMode.Always;
                    }
                }
            }
        }

        // loader start loading tiles
        void MainMap_OnTileLoadStart()
        {
            MethodInvoker m = delegate ()
            {
                // HACK JKU CLEAN
                //panelMenu.Text = "Menu: loading tiles...";
            };
            try
            {
                BeginInvoke(m);
            }
            catch
            {
            }
        }

        // loader end loading tiles
        void MainMap_OnTileLoadComplete(long elapsedMilliseconds)
        {
            // HACK JKU CLEAN
            //MainMap.ElapsedMilliseconds = elapsedMilliseconds;

            MethodInvoker m = delegate ()
            {
                
                // HACK JKU CLEAN
                //panelMenu.Text = "Menu, last load in " + MainMap.ElapsedMilliseconds + "ms";
                //textBoxMemory.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00} MB of {1:0.00} MB", MainMap.Manager.MemoryCache.Size, MainMap.Manager.MemoryCache.Capacity);
            };
            try
            {
                BeginInvoke(m);
            }
            catch
            {
            }
        }

        // current point changed
        void MainMap_OnPositionChanged(PointLatLng point)
        {
            //textBoxLatCurrent.Text = point.Lat.ToString(CultureInfo.InvariantCulture);
            //textBoxLngCurrent.Text = point.Lng.ToString(CultureInfo.InvariantCulture);

        }

        #endregion


        #region -- ui events --

        bool _userAcceptedLicenseOnce;

        // change map type
        private void comboBoxMapType_DropDownClosed(object sender, EventArgs e)
        {
            if (!_userAcceptedLicenseOnce)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "License.txt"))
                {
                    string txt = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "License.txt");
                    
                    var d = new Demo.WindowsForms.Forms.Message();
                    d.richTextBox1.Text = txt;

                    if (DialogResult.Yes == d.ShowDialog())
                    {
                        _userAcceptedLicenseOnce = true;
                        Text += " - license accepted by " + Environment.UserName + " at " + DateTime.Now;
                    }
                }
                else
                {
                    // user deleted License.txt ;}
                    _userAcceptedLicenseOnce = true;
                }
            }

            if (_userAcceptedLicenseOnce)
            {
                MainMap.MapProvider = comboBoxMapType.SelectedItem as GMapProvider;
            }
            else
            {
                MainMap.MapProvider = GMapProviders.OpenStreetMap;
                comboBoxMapType.SelectedItem = MainMap.MapProvider;
            }
        }

        // change mode
        private void comboBoxMode_DropDownClosed(object sender, EventArgs e)
        {
            MainMap.Manager.Mode = (AccessMode)comboBoxMode.SelectedValue;
            MainMap.ReloadMap();
        }

        // goto by geocoder
        private void textBoxGeo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                var status = MainMap.SetPositionByKeywords(textBoxGeo.Text);

                if (status != GeoCoderStatusCode.OK)
                {
                    MessageBox.Show("Geocoder can't find: '" + textBoxGeo.Text + "', reason: " + status.ToString(), "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                textBoxLat.Text = MainMap.Position.Lat.ToString(CultureInfo.InvariantCulture);
                textBoxLng.Text = MainMap.Position.Lng.ToString(CultureInfo.InvariantCulture);
            }
        }

        // go to
        private void btnGoTo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxGeo.Text))
            {
                if (double.TryParse(textBoxLat.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(textBoxLng.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng))
                {
                    MainMap.Position = new PointLatLng(lat, lng);
                }
                return;
            }

            var status = MainMap.SetPositionByKeywords(textBoxGeo.Text);

            if (status != GeoCoderStatusCode.OK)
            {
                MessageBox.Show("Geocoder can't find: '" + textBoxGeo.Text + "', reason: " + status.ToString(), "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                textBoxLat.Text = MainMap.Position.Lat.ToString(CultureInfo.InvariantCulture);
                textBoxLng.Text = MainMap.Position.Lng.ToString(CultureInfo.InvariantCulture);
            }
        }

        // reload map
        private void btnReload_Click(object sender, EventArgs e)
        {
            MainMap.ReloadMap();
        }

        // cache config
        private void checkBoxUseCache_CheckedChanged(object sender, EventArgs e)
        {
            //MainMap.Manager.UseRouteCache = checkBoxUseRouteCache.Checked;
            //MainMap.Manager.UseGeocoderCache = checkBoxUseRouteCache.Checked;
            //MainMap.Manager.UsePlacemarkCache = checkBoxUseRouteCache.Checked;
            //MainMap.Manager.UseDirectionsCache = checkBoxUseRouteCache.Checked;
        }

        // clear cache
        private void btnCacheClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You sure?", "Clear GMap.NET cache?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                try
                {
                    MainMap.Manager.PrimaryCache.DeleteOlderThan(DateTime.Now, null);
                    MessageBox.Show("Done. Cache is clear.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // add test route
        private void btnAddRoute_Click(object sender, EventArgs e)
        {
            var rp = MainMap.RoutingProvider;

            var route = rp.GetRoute(_start, _end, false, false, (int)MainMap.Zoom);
            if (route != null)
            {
                // add route
                var r = new GMapRoute(route.Points, route.Name);
                r.IsHitTestVisible = true;
                Routes.Routes.Add(r);

                // add route start/end marks
                GMapMarker m1 = new GMarkerGoogle(_start, GMarkerGoogleType.green_big_go);
                m1.ToolTipText = "Start: " + route.Name;
                m1.ToolTipMode = MarkerTooltipMode.Always;

                GMapMarker m2 = new GMarkerGoogle(_end, GMarkerGoogleType.red_big_stop);
                m2.ToolTipText = "End: " + _end.ToString();
                m2.ToolTipMode = MarkerTooltipMode.Always;

                Objects.Markers.Add(m1);
                Objects.Markers.Add(m2);

                MainMap.ZoomAndCenterRoute(r);

                //MainMap.Position = _start;
                ClearStartEnd();
            }
        }


        private void btn_addline_Click(object sender, EventArgs e)
        {
            GMapPolyLine polygon = null;

            if (_start.IsEmpty)
            {
                switch (Polygons.Polygons.Count)
                {
                    case 0: return;
                    case 1:
                        polygon = Polygons.Polygons[0];
                        break;
                    default:
                        foreach (var p in Polygons.Polygons)
                        {
                            if (p.IsSelected)
                            {
                                polygon = p;
                                break;
                            }
                        }
                        break;
                }
            }

            if (polygon == null)
            {
                if (_start.IsEmpty)
                {
                    MessageBox.Show("Set start point first");
                    return;
                }
                var points = new List<PointLatLng>();
                points.Add(new PointLatLng(_start.Lat, _start.Lng));

                if (_end.IsEmpty)
                    _end = _currentMarker.Position;

                points.Add(new PointLatLng(_end.Lat, _end.Lng));
                polygon = new GMapPolyLine(points, $"Line-{++_lineindex}");
                polygon.Stroke = new Pen(Color.Blue, 2);
                polygon.Stroke.EndCap = LineCap.ArrowAnchor;
                Polygons.Polygons.Add(polygon);

                ClearStartEnd();
            }
            else
            {
                if ((_currentMarker != null) &&  _currentMarker.IsVisible)
                {
                    polygon.Append(new PointLatLng(_currentMarker.Position.Lat, _currentMarker.Position.Lng));
                }
            }
        }


        // add marker on current position
        private void btnAddMarker_Click(object sender, EventArgs e)
        {
            var m = new GMarkerGoogle(_currentMarker.Position, GMarkerGoogleType.green_pushpin);
            var mBorders = new GMapMarkerRect(_currentMarker.Position);
            {
                mBorders.InnerMarker = m;
                if (_polygon != null)
                {
                    mBorders.Tag = _polygon.Points.Count;
                }
                mBorders.ToolTipMode = checkBox_Infobox.Checked?  MarkerTooltipMode.Always : MarkerTooltipMode.Never;
            }

            Placemark? p = null;
            if (checkBoxPlacemarkInfo.Checked)
            {
                GeoCoderStatusCode status;
                var ret = MainMap.GeocodingProvider.GetPlacemark(_currentMarker.Position, out status);      // get city, street, etc info for current marker position
                if (status == GeoCoderStatusCode.OK && ret != null)
                {
                    p = ret;
                }
            }

            if (p != null)
            {
                mBorders.ToolTipText = p.Value.Address;
            }
            else
            {
                mBorders.ToolTipText = _currentMarker.Position.ToString();
            }

            Objects.Markers.Add(m);
            Objects.Markers.Add(mBorders);

            RegeneratePolygon();
        }

        private void btnAddPushpin_Click(object sender, EventArgs e)
        {
            var m = new GMarkerHFsystem(_currentMarker.Position, GMarkerHFsystemType.Piroskör, 1);
            m.SetDefaultInfo();
            m.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
            Objects.Markers.Add(m);
        }


        // clear markers
        private void btnClearAllMarkers_Click(object sender, EventArgs e)
        {
            ClearStartEnd();
            Objects.Markers.Clear();
            //Routes.Routes.Clear();
            //Polygons.Polygons.Clear();
        }

        // clear routes and polygons
        private void btn_clear_routes_Click(object sender, EventArgs e)
        {
            Routes.Routes.Clear();
            Polygons.Polygons.Clear();
            Polygons.Rays.Clear();
            ClearStartEnd();
        }


        private void ClearStartEnd()
        {
            if (_startMarker != null)
                Objects.Markers.Remove(_startMarker);
            if (_endMarker != null)
                Objects.Markers.Remove(_endMarker);
            _start = PointLatLng.Empty;
            _end = PointLatLng.Empty;
            _startMarker = null;
            _endMarker = null;
        }

        // show current marker
        private void checkBoxCurrentMarker_CheckedChanged(object sender, EventArgs e)
        {
            _currentMarker.IsVisible = checkBox_CurrentMarker.Checked;
        }

        // can drag
        private void checkBoxCanDrag_CheckedChanged(object sender, EventArgs e)
        {
            MainMap.CanDragMap = checkBoxCanDrag.Checked;
        }

        // set route start
        private void btnSetStart_Click(object sender, EventArgs e)
        {
            _start = _currentMarker.Position;
            _startMarker = new GMarkerCross(_start);
            Objects.Markers.Add(_startMarker);
        }

        // set route end
        private void btnSetEnd_Click(object sender, EventArgs e)
        {
            _end = _currentMarker.Position;
            _endMarker = new GMarkerCross(_end);
            Objects.Markers.Add(_endMarker);
        }

        // zoom to max for markers
        private void btnZoomCenter_Click(object sender, EventArgs e)
        {
            MainMap.ZoomAndCenterMarkers("objects");
        }

        // export map data
        private void btnExport_Click(object sender, EventArgs e)
        {
            MainMap.ShowExportDialog();
        }

        // import map data
        private void btnImport_Click(object sender, EventArgs e)
        {
            MainMap.ShowImportDialog();
        }

        // prefetch
        private void btnCachePrefetch_Click(object sender, EventArgs e)
        {
            var area = MainMap.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)MainMap.Zoom; i <= MainMap.MaxZoom; i++)
                {
                    var res = MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                    {
                        using (var obj = new TilePrefetcher())
                        {
                            obj.Overlay = Objects; // set overlay if you want to see cache progress on the map

                            obj.Shuffle = MainMap.Manager.Mode != AccessMode.CacheOnly;

                            obj.Owner = this;
                            obj.ShowCompleteMessage = true;
                            obj.Start(area, i, MainMap.MapProvider, MainMap.Manager.Mode == AccessMode.CacheOnly ? 0 : 100, MainMap.Manager.Mode == AccessMode.CacheOnly ? 0 : 1);
                        }
                    }
                    else if (res == DialogResult.No)
                    {
                        continue;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        // debug tile grid
        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            MainMap.ShowTileGridLines = checkBox_Debug.Checked;
        }

        // launch static map maker
        private void btnSave_Click(object sender, EventArgs e)
        {
            var st = new StaticImage(this);
            st.Owner = this;
            st.Show();
        }

        // key-up events
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            int offset = -22;

            if (e.KeyCode == Keys.Left)
            {
                MainMap.Offset(-offset, 0);
            }
            else if (e.KeyCode == Keys.Right)
            {
                MainMap.Offset(offset, 0);
            }
            else if (e.KeyCode == Keys.Up)
            {
                MainMap.Offset(0, -offset);
            }
            else if (e.KeyCode == Keys.Down)
            {
                MainMap.Offset(0, offset);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (_currentPolyline != null)
                {
                    Polygons.Polygons.Remove(_currentPolyline);
                    _currentPolyline = null;
                }

                if (_currentRoute != null)
                {
                    Routes.Routes.Remove(_currentRoute);
                    _currentRoute = null;
                }

                if (_currentHFmarker != null)
                {
                    Objects.Markers.Remove(_currentHFmarker);
                    _currentHFmarker = null;
                }

                if (_currentRectMarker != null)
                {
                    Objects.Markers.Remove(_currentRectMarker);

                    if (_currentRectMarker.InnerMarker != null)
                    {
                        Objects.Markers.Remove(_currentRectMarker.InnerMarker);
                    }
                    _currentRectMarker = null;


                    RegeneratePolygon();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                MainMap.Bearing = 0;

                if (_currentTransport != null && !MainMap.IsMouseOverMarker)
                {
                    _currentTransport.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    _currentTransport = null;
                }
            }
        }

        // key-press events
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (MainMap.Focused)
            {
                if (e.KeyChar == '+')
                {
                    MainMap.Zoom = ((int)MainMap.Zoom) + 1;
                }
                else if (e.KeyChar == '-')
                {
                    MainMap.Zoom = ((int)(MainMap.Zoom + 0.99)) - 1;
                }
                else if (e.KeyChar == 'a')
                {
                    MainMap.Bearing--;
                }
                else if (e.KeyChar == 'z')
                {
                    MainMap.Bearing++;
                }
            }
        }

        // engage some live demo
        private void RealTimeChanged(object sender, EventArgs e)
        {
            Objects.Markers.Clear();
            Polygons.Polygons.Clear();
            Routes.Routes.Clear();

            // start performance test
            //if (radioButtonPerf.Checked)
            //{
            //    _timerPerf.Interval = 44;
            //    _timerPerf.Start();
            //}
            //else
            //{
            //    // stop performance test
            //    _timerPerf.Stop();
            //}

            //// vehicle demo
            //if (radioButtonVehicle.Checked)
            //{
            //    if (!_transportWorker.IsBusy)
            //    {
            //        _firstLoadTrasport = true;
            //        _transportWorker.RunWorkerAsync();
            //    }
            //}
            //else
            //{
            //    if (_transportWorker.IsBusy)
            //    {
            //        _transportWorker.CancelAsync();
            //    }
            //}
        }

        // http://leafletjs.com/
        // Leaflet is a modern open-source JavaScript library for mobile-friendly interactive maps
        // Leaflet is designed with simplicity, performance and usability in mind. It works efficiently across all major desktop and mobile platforms out of the box
        private void checkBoxTileHost_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTileHost.Checked)
            {
                try
                {
                    MainMap.Manager.EnableTileHost(8844);
                    TryExtractLeafletjs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("EnableTileHost: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MainMap.Manager.DisableTileHost();
            }
        }

        #endregion

        #region -- UNUSED CODE --

        // zoom
        private void UNUSED_trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            //MainMap.Zoom = trackBarZoom.Value / 100.0;
        }

        private void UNUSED_btnZoomUp_Click(object sender, EventArgs e)
        {
            MainMap.Zoom = ((int)MainMap.Zoom) + 1;
        }

        private void UNUSED_btnZoomDown_Click(object sender, EventArgs e)
        {
            MainMap.Zoom = ((int)(MainMap.Zoom + 0.99)) - 1;
        }

        // saves current map view 
        private void UNUSED_btnSaveCurrentMapView_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PNG (*.png)|*.png";
                    sfd.FileName = "GMap.NET image";

                    var tmpImage = MainMap.ToImage();
                    if (tmpImage != null)
                    {
                        using (tmpImage)
                        {
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                tmpImage.Save(sfd.FileName);

                                MessageBox.Show("Image saved: " + sfd.FileName, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Image failed to save: " + ex.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // load gpx file
        private void UNUSED_btnLoadGpxFile(object sender, EventArgs e)
        {
            using (FileDialog dlg = new OpenFileDialog())
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.AddExtension = true;
                dlg.DefaultExt = "gpx";
                dlg.ValidateNames = true;
                dlg.Title = "GMap.NET: open gpx log";
                dlg.Filter = "gpx files (*.gpx)|*.gpx";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string gpx = File.ReadAllText(dlg.FileName);

                        var r = MainMap.Manager.DeserializeGPX(gpx);
                        if (r != null)
                        {
                            if (r.trk.Length > 0)
                            {
                                foreach (var trk in r.trk)
                                {
                                    var points = new List<PointLatLng>();

                                    foreach (var seg in trk.trkseg)
                                    {
                                        foreach (var p in seg.trkpt)
                                        {
                                            points.Add(new PointLatLng((double)p.lat, (double)p.lon));
                                        }
                                    }

                                    var rt = new GMapRoute(points, string.Empty);
                                    {
                                        rt.Stroke = new Pen(Color.FromArgb(144, Color.Red));
                                        rt.Stroke.Width = 5;
                                        rt.Stroke.DashStyle = DashStyle.DashDot;
                                    }
                                    Routes.Routes.Add(rt);
                                }

                                MainMap.ZoomAndCenterRoutes(null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("GPX import: " + ex.ToString());
                        MessageBox.Show("Error importing gpx: " + ex.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }



        // open disk cache location
        private void UNUSED_btnOpenDiskCacheLocation_Click(object sender, EventArgs e)
        {
            try
            {
                string argument = "/select, \"" + MainMap.CacheLocation + "TileDBv5\"";
                Process.Start("explorer.exe", argument);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open: " + ex.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // export mobile gps log to gpx file
        /*
        private void buttonExportToGpx_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "GPX (*.gpx)|*.gpx";
                    sfd.FileName = "mobile gps log";

                    DateTime? date = null;
                    DateTime? dateEnd = null;

                    if (MobileLogFrom.Checked)
                    {
                        date = MobileLogFrom.Value.ToUniversalTime();

                        sfd.FileName += " from " + MobileLogFrom.Value.ToString("yyyy-MM-dd HH-mm");
                    }

                    if (MobileLogTo.Checked)
                    {
                        dateEnd = MobileLogTo.Value.ToUniversalTime();

                        sfd.FileName += " to " + MobileLogTo.Value.ToString("yyyy-MM-dd HH-mm");
                    }

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var log = Stuff.GetRoutesFromMobileLog(_mobileGpsLog, date, dateEnd, 3.3);
                        if (log != null)
                        {
                            if (MainMap.Manager.ExportGPX(log, sfd.FileName))
                            {
                                MessageBox.Show("GPX saved: " + sfd.FileName, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GPX failed to save: " + ex.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */


        #endregion

        private void checkBox_Markers_CheckedChanged(object sender, EventArgs e)
        {
            Objects.IsVisibile = checkBox_Markers.Checked;  
        }

        private void checkBox_Routes_CheckedChanged(object sender, EventArgs e)
        {
            Polygons.IsVisibile = checkBox_Routes.Checked;
        }

        private void checkBox1_Infobox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var m in Objects.Markers)
            {
                m.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
            }

            foreach (var m in Polygons.Lines)
            {
                m.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
            }

            foreach (var m in Polygons.Rays)
            {
                m.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
            }

            MainMap.Refresh();
        }

        private void button_centerMarker_Click(object sender, EventArgs e)
        {
            if (_currentMarker.IsVisible)
            {
                MainMap.Position = _currentMarker.Position;
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if ((_currentHFmarker != null) &&_currentHFmarker.IsVisible)
            {
                Objects.Markers.Remove(_currentHFmarker);
                _currentHFmarker = null;
            }
        }

        private void textBoxLat_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBoxGeo.Text = string.Empty;
        }

        private void textBoxLng_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBoxGeo.Text = string.Empty;
        }

        private void btnMarker_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxGeo.Text))
            {
                if (double.TryParse(textBoxLat.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(textBoxLng.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng))
                {
                    var p = new PointLatLng(lat, lng);
                    //var m = new GMarkerHFsystem(p, GMarkerHFsystemType.Piroskör, 1);
                    var m = new GMarkerGoogle(p, GMarkerGoogleType.pink_pushpin);
                    //m.SetDefaultInfo();
                    m.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
                    Objects.Markers.Add(m);
                }
                return;
            }
            else
            {
                var pos = MainMap.GetPositionBySettlement(textBoxGeo.Text);
                if (!pos.IsEmpty)
                {
                    //var m = new GMarkerHFsystem(pos, GMarkerHFsystemType.Piroskör, 1);
                    //m.SetDefaultInfo();
                    var m = new GMarkerGoogle(pos, GMarkerGoogleType.pink_pushpin, textBoxGeo.Text);
                    m.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
                    m.ToolTipText = textBoxGeo.Text;
                    Objects.Markers.Add(m);
                }
            }

        }

        private void btnCenterCursor_Click(object sender, EventArgs e)
        {
            if (_currentMarker != null)
            {
                MainMap.Position = _currentMarker.Position;
            }
        }

        private void btnCursor_Click(object sender, EventArgs e)
        {
            if ((_currentMarker == null) || !_currentMarker.IsVisible)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxGeo.Text))
            {
                if (double.TryParse(textBoxLat.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                    double.TryParse(textBoxLng.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double lng))
                {
                    _currentMarker.Position = new PointLatLng(lat, lng);
                }
                return;
            }
            else
            {
                var pos = MainMap.GetPositionBySettlement(textBoxGeo.Text);
                if (!pos.IsEmpty)
                {
                    _currentMarker.Position = pos;
                }
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.settlement = textBoxGeo.Text;
            Program.initialLatitude = MainMap.Position.Lat;
            Program.initialLongitude = MainMap.Position.Lng;
            Program.initialZoom = MainMap.Zoom;
            Program.mapProvider = MainMap.MapProvider;
            Program.accesmode = MainMap.Manager.Mode;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStartEnd();
            Objects.Markers.Clear();
            Routes.Routes.Clear();
            Polygons.Polygons.Clear();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void copyPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMap.Copy();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (FileDialog dlg = new SaveFileDialog())
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.AddExtension = true;
                dlg.DefaultExt = "jpeg";
                dlg.ValidateNames = true;
                dlg.FileName = "";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = "Jpeg file (*.jpeg)|*.jpeg";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    MainMap.Save(dlg.FileName);
                }
            }

        } /* saveToolStripMenuItem1_Click */

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GMap.NET demo application\n\n" +
                            "Version: 2.1.71\n" +
                            "Author: Jurgen De Leon Rodriguez\n" +
                            "Company: MainSoft Technology\n" +
                            //"License: GNU General Public License Version 3 (GPLv3)\n\n" +
                            "Updated: 2024-05-06 by E.Kótai (SAGAX) for RH04 system\n\n" +
                            "This application demonstrates the capabilities of GMap.NET library." , 
                            "About GMap.NET", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void routingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMap.Manager.UseRouteCache = !MainMap.Manager.UseRouteCache;
            routingToolStripMenuItem.Checked = MainMap.Manager.UseRouteCache;
        }

        private void geocodingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainMap.Manager.UseGeocoderCache = !MainMap.Manager.UseGeocoderCache;
            geocodingToolStripMenuItem.Checked = MainMap.Manager.UseGeocoderCache;
        }

        private void checkBoxShowCenterMarker_CheckedChanged(object sender, EventArgs e)
        {
            MainMap.ShowCenter = checkBox_CenterMarker.Checked;
        }

        private void checkBoxShowGeoGrid_CheckedChanged(object sender, EventArgs e)
        {
            if (MainMap.Zoom < 5)
            {
                Geogrid10.IsVisibile = checkBox_GeoGrid.Checked;
                if (checkBox_GeoGrid.Checked)
                    GenerateGeoGrid(); 
                Geogrid.IsVisibile = false;
            }
            else
            {
                Geogrid.IsVisibile = checkBox_GeoGrid.Checked;
                if (checkBox_GeoGrid.Checked)
                    GenerateGeoGrid();
                Geogrid10.IsVisibile = false;
            }
        }

        private void ckb_show_scale_CheckedChanged(object sender, EventArgs e)
        {
            MainMap.MapScaleInfoEnabled = checkBox_scale.Checked;
            MainMap.Invalidate();
        }

        private void btn_add_dfline_Click(object sender, EventArgs e)
        {
            if (_start == PointLatLng.Empty)
            {
                MessageBox.Show("A kezdőpontot meg kell adni!", "WARNING");
                return;
            }
            if (double.TryParse(txe_direction.Text, out double degree))
            {
                var dfline = new GMapRay(_start, degree, Color.DarkBlue);
                Polygons.Rays.Add(dfline);
            }
            else
                MessageBox.Show("A szöget meg kell adni!", "WARNING");
        }


        private void ClearAll()
        {
            Objects.Clear();
            Routes.Clear();
            Polygons.Clear();
            foreach (var m in MainMap.Overlays)
                m.Clear();
        }


        /// <summary>
        /// Process macro command
        /// Formatr: command name parameters..
        /// </summary>
        /// <param name="st"></param>
        /// <param name="errorstring"></param>
        /// <returns></returns>
        public bool ProcessMacroCommand(string st, out string errorstring)
        {
            char[] separator = new Char[] { ',', ';', ' ' };
            errorstring = null;

            if (string.IsNullOrEmpty(st))
                return true;

            st = st.Trim();
            if (st.Length == 0)
                return true;

            if (st.StartsWith("clear all"))
            {
                ClearAll();
                return true;
            }

            if (st.StartsWith("clear objects"))
            {
                Objects.Clear();
                return true;
            }

            if (st.StartsWith("clear routes"))
            {
                btn_clear_routes_Click(this, EventArgs.Empty);
                return true;
            }

            if (st.StartsWith("clear polygons"))
            {
                Polygons.Clear();
                return true;
            }

            if (st.StartsWith("clear lines"))
            {
                Polygons.Clear();
                return true;
            }

            if (st.StartsWith("clear markers"))
            {
                _top.Markers.Clear();
                foreach (var m in MainMap.Overlays)
                    m.Markers.Clear();
                return true;
            }


            string[] ls = st.Split(separator);

            switch (ls[0])
            {
                case "line": break;
                case "ray": break;
                case "polyline": break;
                case "image": break;
                case "label": break;
                case "route": break;
                case "marker":
                    if (ls.Length > 5)
                    {
                        var g = GMapMarker.CreateMarker(ls, out errorstring);
                        if (g == null)
                        {
                            errorstring += ": " + st;
                            return false;
                        }

                        Objects.Markers.Add(g);

                        if (g is GMarkerGoogle)
                        {
                            var mBorders = new GMapMarkerRect(g.Position);
                            mBorders.InnerMarker = g as GMarkerGoogle;
                            mBorders.ToolTipMode = checkBox_Infobox.Checked ? MarkerTooltipMode.Always : MarkerTooltipMode.Never;
                            mBorders.ToolTipText = g.Name;
                            Objects.Markers.Add(mBorders);
                        }

                        RegeneratePolygon();
                        MainMap.Invalidate();
                        return true;
                    }
                    break;

                case "show":
                    if (ls.Length > 1)
                    {
                        bool showit = true;
                        if ((ls.Length > 2) && 
                            ((ls[2] == "no") || (ls[2] == "0") || (ls[2] == "off"))) showit = false;

                        switch (ls[1])
                        {
                            case "markers": checkBox_Markers.Checked = showit; break;
                            case "routes" : checkBox_Routes.Checked = showit; break;
                            case "infobox": checkBox_Infobox.Checked = showit;  break;
                            case "center" : checkBox_CenterMarker.Checked = showit; break;
                            case "cursor" : checkBox_CurrentMarker.Checked = showit;  break;
                            case "grid"   : checkBox_Debug.Checked = showit; break;
                            case "geogrid": checkBox_GeoGrid.Checked = showit; break;
                            case "scale"  : checkBox_scale.Checked = showit;  break;
                            default:
                                //MessageBox.Show("Invalid parameter : " + st, "ERROR");
                                errorstring = "Invalid parameter : " + st;
                                return false;
                        }
                    }
                    break;
                default:
                    //MessageBox.Show("Invalid command : " + st, "ERROR");
                    errorstring = "Invalid command : " + st;
                    return false;
            } /* switch */


            return true;
        } /* ProcessMacroCommand */


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
        } /* openToolStripMenuItem_Click */


        private void macroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "gmc";
            dlg.Filter = "GMap macro file (*.gmc)|*.gmc|All file (*.*)|*.*";
            dlg.Title = "Select GMAP Macro file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (var rd = new StreamReader(dlg.FileName))
                {
                    while (!rd.EndOfStream)
                    {
                        string st = rd.ReadLine().ToLower();
                        if (string.IsNullOrWhiteSpace(st))
                            continue;
                        if (st[0] == '#')               // comments
                            continue;

                        if (st.StartsWith("exit"))
                            break;

                        if (!ProcessMacroCommand(st, out string errorstring))
                        {
                            if (!string.IsNullOrEmpty(errorstring))
                                MessageBox.Show(errorstring, "ERROR");
                            break;
                        }
                    } /* while */

                } /* using */

                MacroEditor.MacroText = new StreamReader(dlg.FileName).ReadToEnd();
            }
        } /* macroToolStripMenuItem_Click */

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var frm = new MacroEditor();
            frm.Show();
        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new Demo.WindowsForms.Forms.Message();

            if (DialogResult.Yes == d.ShowDialog())
            {
                _userAcceptedLicenseOnce = true;
                if (!Text.Contains("license"))
                    Text += " - license accepted by " + Environment.UserName + " at " + DateTime.Now;
            }
        }


    } /* class MainForm */

} /* namespace Demo.WindowsForms */
