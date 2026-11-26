/*=============================================================================
*  SgxMapControl: User control for SgxMapPanel, 
*  providing a map interface with toolbar, drawbar, and status bar.
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
*          Last modification                                       20-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using GMap.NET.MapProviders.HFsystem;
using SgxMap.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static GMap.NET.Entity.OpenStreetMapGraphHopperGeocodeEntity;
using static GMap.NET.Entity.OpenStreetMapRouteEntity;
//using GMap.NET;
//using GMap.NET.Internals;

namespace SgxMap
{
    public partial class SgxMapControl : UserControl
    {
        private object m_monitor = new object();

        private MapTextBox m_textbox;

        private Color m_selected_fillcolor = Color.Blue;
        private Color m_selected_linecolor = Color.Green;
        private Color m_selected_textcolor = Color.Red;
        private int m_selected_linewidth = 1;
        private ArrowPositions m_selected_linestyle = ArrowPositions.None;
        private string m_lastFileName = string.Empty;
        private PointLatLng m_rigthclickpoint;


        public bool ShowToolbar
        {
            get { return TopPanel.Visible; }
            set { TopPanel.Visible = value; }
        }

        public bool ShowDrawbar
        {
            get { return DrawPanel.Visible; }
            set { DrawPanel.Visible = value; }
        }

        public bool ShowStatusbar
        {
            get { return BottomPanel.Visible; }
            set { BottomPanel.Visible = value; }
        }


        public GMapProvider MapProvider
        {
            get { return MainMap.MapProvider; }
            set
            {
                if (MainMap.MapProvider != value)
                {
                    MainMap.MapProvider = value;
                    comboBoxMapType.SelectedItem = value;
                    UpdateLayerButton();
                    trackBar_Zoom.Minimum = (int)MainMap.MinZoom;
                    trackBar_Zoom.Maximum = (int)MainMap.MaxZoom;
                    if ((MainMap.Zoom >= MainMap.MinZoom) && (MainMap.Zoom <= MainMap.MaxZoom))
                        trackBar_Zoom.Value = (int)MainMap.Zoom;
                }
            }
        }


        public double Zoom
        {
            get { return MainMap.Zoom; }
            set
            {
                MainMap.Zoom = value;
                if ((MainMap.Zoom >= MainMap.MinZoom) && (MainMap.Zoom <= MainMap.MaxZoom))
                    trackBar_Zoom.Value = (int)MainMap.Zoom;
            }
        }


        //---------------------------------------------------------------------------------

        public SgxMapControl()
        {
            InitializeComponent();
            CreateLayerMenu();
            LoadControls();
            MainMap.MouseDown += MainMap_MouseDown;
            MainMap.OnShowPlaceInfo += MainMap_ShowPlaceInfo;
            MainMap.OnShowPlaceInfoEx += MainMap_ShowPlaceInfoEx;
            MainMap.OnTextBoxEdit += MainMap_OnTextBoxEdit;
        }


        public void LoadControls()
        {
            comboBoxMapType.ValueMember = "Name";
            comboBoxMapType.DataSource = GMapProviders.List;
            comboBoxMapType.SelectedItem = MainMap.MapProvider;
            trackBar_Zoom.Minimum = (int)MainMap.MinZoom;
            trackBar_Zoom.Maximum = (int)MainMap.MaxZoom;
            if ((MainMap.Zoom >= MainMap.MinZoom) && (MainMap.Zoom <= MainMap.MaxZoom))
                trackBar_Zoom.Value = (int)MainMap.Zoom;
        }


        #region HFsystemMap rétegek

        private System.Windows.Forms.Button btn_layers = null!;
        private ContextMenuStrip menu_layers = null!;

        /// <summary>
        ///     "Rétegek" gomb a térképtípus-választó mellett; csak HFsystemMap (vektoros) térképnél látszik.
        ///     A menüben a rétegek pipával be-/kikapcsolhatók, a térkép azonnal újrarajzolódik.
        /// </summary>
        private void CreateLayerMenu()
        {
            menu_layers = new ContextMenuStrip(components);
            menu_layers.ShowCheckMargin = true;
            menu_layers.ShowImageMargin = false;
            // pipáláskor ne csukódjon be a menü, így több réteg is átállítható egyszerre
            menu_layers.Closing += (s, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                    e.Cancel = true;
            };

            var provider = HFsystemMapProvider.Instance;
            string? group = null;

            foreach (var item in HFsystemLayerInfo.Items)
            {
                if (item.Group != group)
                {
                    if (group != null)
                        menu_layers.Items.Add(new ToolStripSeparator());
                    menu_layers.Items.Add(new ToolStripLabel(item.Group) { Font = new Font(menu_layers.Font, FontStyle.Bold), ForeColor = SystemColors.GrayText });
                    group = item.Group;
                }

                var mi = new ToolStripMenuItem(item.Title)
                {
                    CheckOnClick = true,
                    Checked = provider.IsLayerVisible(item.Layer),
                    Tag = item.Layer
                };
                mi.CheckedChanged += (s, e) =>
                {
                    var m = (ToolStripMenuItem)s!;
                    HFsystemMapProvider.Instance.SetLayerVisible((HFsystemLayers)m.Tag!, m.Checked);
                };
                menu_layers.Items.Add(mi);
            }

            menu_layers.Items.Add(new ToolStripSeparator());
            menu_layers.Items.Add(new ToolStripMenuItem("Mind be", null, (s, e) => SetAllLayers(true)));
            menu_layers.Items.Add(new ToolStripMenuItem("Csak alaptérkép (feliratok nélkül)", null, (s, e) =>
            {
                SetAllLayers(true);
                foreach (var it in menu_layers.Items.OfType<ToolStripMenuItem>())
                {
                    if (it.Tag is HFsystemLayers l && HFsystemLayerInfo.Items.Any(x => x.Layer == l && x.Group == "Feliratok"))
                        it.Checked = false;
                }
            }));

            btn_layers = new System.Windows.Forms.Button
            {
                Text = "Rétegek ▾",
                Location = new System.Drawing.Point(comboBoxMapType.Right + 6, comboBoxMapType.Top - 1),
                Size = new Size(90, 25),
                UseVisualStyleBackColor = true,
                Visible = false,
                TabIndex = 10
            };
            toolTip_draw.SetToolTip(btn_layers, "Térképrétegek be-/kikapcsolása (utak, utcanevek, épületek...)");
            btn_layers.Click += (s, e) => menu_layers.Show(btn_layers, new System.Drawing.Point(0, btn_layers.Height));
            SearchPanel.Controls.Add(btn_layers);

            provider.LayersChanged += HFsystem_LayersChanged;
            HandleDestroyed += (s, e) => HFsystemMapProvider.Instance.LayersChanged -= HFsystem_LayersChanged;
        }

        private void SetAllLayers(bool visible)
        {
            foreach (var it in menu_layers.Items.OfType<ToolStripMenuItem>())
            {
                if (it.Tag is HFsystemLayers)
                    it.Checked = visible;
            }
        }

        private void HFsystem_LayersChanged(object? sender, EventArgs e)
        {
            if (MainMap.MapProvider != HFsystemMapProvider.Instance)
                return;

            // a már kirajzolt csempék a memóriában vannak; töröljük, hogy az új rétegekkel rajzolódjanak újra
            GMaps.Instance.MemoryCache.Clear();
            try
            {
                // nem ReloadMap: az nullázná a "működő zoomszintet", és a hiányzó csempék helyén
                // nem jelenne meg a szülőcsempe nagyított képe (csak üres hibacsempék)
                MainMap.ReloadTiles();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HFsystem_LayersChanged: " + ex.Message);
            }
        }

        private void UpdateLayerButton()
        {
            if (btn_layers != null)
                btn_layers.Visible = MainMap.MapProvider == HFsystemMapProvider.Instance &&
                                     HFsystemMapProvider.Instance.IsVectorTileUrl;
        }

        #endregion

        private void comboBoxMapType_SelectedValueChanged(object sender, EventArgs e)
        {
            MainMap.MapProvider = comboBoxMapType.SelectedItem as GMapProvider;
            UpdateLayerButton();
            trackBar_Zoom.Minimum = (int)MainMap.MinZoom;
            trackBar_Zoom.Maximum = (int)MainMap.MaxZoom;
            if ((MainMap.Zoom >= MainMap.MinZoom) && (MainMap.Zoom <= MainMap.MaxZoom))
                trackBar_Zoom.Value = (int)MainMap.Zoom;
        }

        private void btn_shrinkMap_Click(object sender, EventArgs e)
        {
            Zoom = Math.Max(MainMap.MinZoom, Zoom - 1);
        }

        private void btn_zoomMap_Click(object sender, EventArgs e)
        {
            Zoom = Math.Min(MainMap.MaxZoom, Zoom + 1);
        }

        private void trackBar_Zoom_Scroll(object sender, EventArgs e)
        {
            Zoom = trackBar_Zoom.Value;
        }

        private void btn_findAddress_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(controlBox_address.Text))
            {
                if (MainMap.SetPositionByKeywords(controlBox_address.Text) != GeoCoderStatusCode.OK)
                    MessageBox.Show("Address not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    if (!controlBox_address.Items.Contains(controlBox_address.Text))
                        controlBox_address.Items.Add(controlBox_address.Text);
                }
            }
        }

        public void MainMap_MouseDown(object? sender, MouseEventArgs e)
        {
            label_status.Text = MainMap.GetPlaceInfo(e.X, e.Y);
        }

        public void MainMap_ShowPlaceInfo(int x, int y)
        {
            label_status.Text = MainMap.GetPlaceInfo(x, y);
        }

        public void MainMap_ShowPlaceInfoEx(int x, int y, double bearing, double distance)
        {
            label_status.Text = MainMap.GetPlaceInfo(x, y) + "   bearing: " + bearing.ToString("F2") + "°   distance: " + distance.ToString("F2") + " km";
        }

        private void btn_draw_Click(object sender, EventArgs e)
        {
            var m = new System.Windows.Forms.ContextMenuStrip();


            var m1 = new ToolStripMenuItem("Show center");
            m1.Checked = MainMap.ShowCenter;
            m1.Click += (s, ev) =>
            {
                MainMap.ShowCenter = !MainMap.ShowCenter;
            };
            m.Items.Add(m1);

            var m2 = new ToolStripMenuItem("Show tile grid");
            m2.Checked = MainMap.ShowTileGridLines;
            m2.Click += (s, ev) =>
            {
                MainMap.ShowTileGridLines = !MainMap.ShowTileGridLines;
            };
            m.Items.Add(m2);

            var m3 = new ToolStripMenuItem("Show geo grid");
            m3.Checked = MainMap.ShowGeoGridLines;
            m3.Click += (s, ev) =>
            {
                MainMap.ShowGeoGridLines = !MainMap.ShowGeoGridLines;
            };
            m.Items.Add(m3);

            var m4 = new ToolStripMenuItem("Show helper line");
            m4.Checked = MainMap.HelperLineOption == HelperLineOptions.ShowOnModifierKey;
            m4.Click += (s, ev) =>
            {
                if (m4.Checked)
                    MainMap.HelperLineOption = HelperLineOptions.DontShow;
                else
                    MainMap.HelperLineOption = HelperLineOptions.ShowOnModifierKey;
            };
            m.Items.Add(m4);

            var m5 = new ToolStripMenuItem("Show scale");
            m5.Checked = MainMap.ShowMapScaleInfo;
            m5.Click += (s, ev) =>
            {
                MainMap.ShowMapScaleInfo = !MainMap.ShowMapScaleInfo;
            };
            m.Items.Add(m5);

            m.Show(DrawPanel, new System.Drawing.Point(btn_draw.Left, btn_draw.Bottom));
        } /* btn_draw_Click */

        private void btn_draw_line_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Line;
        }

        private void btn_draw_radius_Click(object sender, EventArgs e)
        {
            //MainMap.MouseMoveStatus = MouseMoveStatusType.Ruler;
            MainMap.MouseMoveStatus = MouseMoveStatusType.CircleRuler;
        }

        private void btn_draw_arrow_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Arrow;
        }

        private void btn_draw_rectangle_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Rectangle;
        }

        private void btn_draw_ellipse_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Ellipse;
        }

        private void btn_draw_textbox_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Textbox;
        }

        private void btn_draw_ruler_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Ruler;
        }

        private void btn_draw_circle_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Circle;
        }


        private void btn_draw_pushpin_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Marker;
        }


        private void btn_draw_rayline_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Ray;
        }


        private void btn_draw_freeform_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Polyline;
        }


        private void textBox_editor_VisibleChanged(object sender, EventArgs e)
        {
            if (!textBox_editor.Visible)
            {
                Monitor.Exit(m_monitor);
            }
        }



        private void textBox_editor_Leave(object sender, EventArgs e)
        {
            //textBox_editor.Visible = false;
        }

        private void MainMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_textbox != null)
                m_textbox.Text = textBox_editor.Text;
            if (textBox_editor.Visible)
                textBox_editor.Visible = false;

            m_rigthclickpoint = MainMap.FromLocalToLatLng(e.X, e.Y);

            if ((e.Button == MouseButtons.Right) && MainMap.FindSelectedLinePoint(e))
            {
                
                return;
            }

            if ((e.Button == MouseButtons.Right) && MainMap.FindSelectedObject(e))
            {
                if (MainMap.SelectedObject is MapBaseMarker)
                {
                    menu_marker.Enabled = true;
                    menu_marker.Show(MainMap, new System.Drawing.Point(e.X, e.Y));
                }
                else if (MainMap.SelectedObject is GeoCircle)
                {
                    menu_rectangle.Enabled = true;
                    menu_rectangle.Show(MainMap, new System.Drawing.Point(e.X, e.Y));
                }
                else if (MainMap.SelectedObject is MapPolyLine)
                {
                    menu_polyline.Enabled = true;
                    menu_polyline.Show(MainMap, new System.Drawing.Point(e.X, e.Y));
                }
                else if (MainMap.SelectedObject is MapTextBox)
                {
                    menu_textbox.Enabled = true;
                    menu_textbox.Show(MainMap, new System.Drawing.Point(e.X, e.Y));
                }
                else if (MainMap.SelectedObject is MapRectangle)
                {
                    menu_rectangle.Enabled = true;
                    menu_rectangle.Show(MainMap, new System.Drawing.Point(e.X, e.Y));
                }
                else if (MainMap.SelectedObject is MapLine)
                {
                    menu_line.Enabled = true;
                    menu_line.Show(MainMap, new System.Drawing.Point(e.X, e.Y));
                }

            }
        }


        private void MainMap_OnTextBoxEdit(MapTextBox textbox)
        {
            m_textbox = textbox;
            var g = MainMap.FromLatLngToLocal(textbox.Position);
            textBox_editor.Location = new System.Drawing.Point((int)g.X, (int)(g.Y + MainMap.Location.Y));
            //textBox_editor.Location = new System.Drawing.Point(textbox.mouse_x, textbox.mouse_y + MainMap.Location.Y);
            textBox_editor.Width = (int)textbox.LocalWidth;
            textBox_editor.Height = (int)textbox.LocalHeight;
            textBox_editor.Visible = true;
            textBox_editor.Text = textbox.Text;
            textBox_editor.Focus();
            Monitor.Enter(m_monitor);
        }

        private void btn_newMap_Click(object sender, EventArgs e)
        {
            MainMap.ClearAll();
            if (textBox_editor.Visible)
                textBox_editor.Visible = false;
        }


        private void btn_openMap_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                LoadFromFile(dlg.FileName);
            }

        }

        private void btn_saveMap_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "";
            dlg.ValidateNames = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SaveToFile(dlg.FileName);
            }
        }


        private void SaveToFile(string filename)
        {
            try
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    //var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    //var layers = new LayerGroup();
                    //layers.Objects = Objects;
                    //layers.Routes = Routes;
                    //layers.Polygons = Polygons;
                    //layers.MapProvider = MainMap.MapProvider.ToString();
                    //layers.Center = MainMap.Position;
                    //layers.Zoom = MainMap.Zoom;
                    //formatter.Serialize(fs, layers);
                    m_lastFileName = filename;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SaveToFile: " + ex);
                MessageBox.Show("Error  in SaveToFile: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadFromFile(string filename)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    //var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    //Objects.Clear();
                    //Routes.Clear();
                    //Polygons.Clear();
                    //var layers = (LayerGroup)formatter.Deserialize(fs);
                    //Objects.AddFrom(layers.Objects);
                    //foreach (var o in Objects.Markers)
                    //{
                    //    if (o is GMapMarkerRect)
                    //    {
                    //        var rc = o as GMapMarkerRect;
                    //        if (rc.InnerMarker == null)
                    //        {
                    //            foreach (var m in Objects.Markers)
                    //            {
                    //                if (m.Position == rc.Position && m != rc)
                    //                {
                    //                    rc.InnerMarker = (GMarkerGoogle)m;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //Routes.AddFrom(layers.Routes);
                    //Polygons.AddFrom(layers.Polygons);
                    //var provider = GMapProviders.TryGetProvider(layers.MapProvider);
                    //if (provider != null)
                    //{
                    //    MainMap.MapProvider = provider;
                    //}
                    //MainMap.Position = layers.Center;
                    //MainMap.Zoom = layers.Zoom;
                    m_lastFileName = filename;
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("LoadFromFile: " + ex);
                MessageBox.Show("Error  in LoadFromFile: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }


        private void btn_fill_down_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            dlg.Color = m_selected_fillcolor;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_selected_fillcolor = dlg.Color;

                if (MainMap.SelectedObject != null)
                    MainMap.SelectedObject.FillColor = m_selected_fillcolor;
            }
        }

        private void btn_fill_color_Click(object sender, EventArgs e)
        {
            if (MainMap.SelectedObject != null)
            {
                MainMap.SelectedObject.FillColor = m_selected_fillcolor;
                MainMap.SelectedObject.IsFilled = true;
            }
        }


        private void btn_linecolor_down_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            dlg.Color = m_selected_linecolor;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                m_selected_linecolor = dlg.Color;
                btn_draw_linecolor_Click(sender, e);
            }
        }

        private void btn_draw_linecolor_Click(object sender, EventArgs e)
        {
            if (MainMap.SelectedObject == null)
                return;

            if (MainMap.SelectedObject is MapLine)
                (MainMap.SelectedObject as MapLine).LineColor = m_selected_linecolor;

            if (MainMap.SelectedObject is MapPolyLine)
                (MainMap.SelectedObject as MapPolyLine).LineColor = m_selected_linecolor;
        }

        private void btn_textcolor_down_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            dlg.Color = m_selected_textcolor;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                m_selected_textcolor = dlg.Color;
                btn_draw_textcolor_Click(sender, e);
            }

        }

        private void btn_draw_textcolor_Click(object sender, EventArgs e)
        {
            if (MainMap.SelectedObject == null)
                return;

            if (MainMap.SelectedObject is MapTextBox)
            {
                (MainMap.SelectedObject as MapTextBox).TextColor = m_selected_textcolor;
            }
        }

        private void btn_draw_linewidth_Click(object sender, EventArgs e)
        {
            menu_linewidth.Enabled = true;
            menu_linewidth.Show(DrawPanel, new System.Drawing.Point(btn_draw_linewidth.Left, btn_draw_linewidth.Bottom));
        }

        private void btn_draw_linearrow_Click(object sender, EventArgs e)
        {
            menu_linestyle.Enabled = true;
            menu_linestyle.Show(DrawPanel, new System.Drawing.Point(btn_draw_linearrow.Left, btn_draw_linearrow.Bottom));
        }

        private void SetSelectedLineWidth(int width)
        {
            m_selected_linewidth = width;
            if (MainMap.SelectedObject == null)
                return;

            if (MainMap.SelectedObject is MapLine)
            {
                (MainMap.SelectedObject as MapLine).Thickness = width;
            }

            if (MainMap.SelectedObject is MapPolyLine)
            {
                (MainMap.SelectedObject as MapPolyLine).Thickness = width;
            }

        }

        private void ptToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(1);
        }

        private void ptToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(2);
        }

        private void ptToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(3);
        }

        private void ptToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(4);
        }

        private void ptToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(5);
        }

        private void ptToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(6);
        }

        private void ptToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            SetSelectedLineWidth(8);
        }


        private void SetSelectedLineStyle(ArrowPositions position)
        {
            m_selected_linestyle = position;

            if (MainMap.SelectedObject == null)
                return;

            if (MainMap.SelectedObject is MapLine)
            {
                (MainMap.SelectedObject as MapLine).ArrowPosition = position;
            }

            if (MainMap.SelectedObject is MapPolyLine)
            {
                (MainMap.SelectedObject as MapPolyLine).ArrowPosition = position;
            }

        }

        private void mi_noarrow_Click(object sender, EventArgs e)
        {
            SetSelectedLineStyle(ArrowPositions.None);
        }

        private void mi_arrow_atbegin_Click(object sender, EventArgs e)
        {
            SetSelectedLineStyle(ArrowPositions.AtBegin);
        }

        private void mi_arrow_atend_Click(object sender, EventArgs e)
        {
            SetSelectedLineStyle(ArrowPositions.AtEnd);
        }

        private void mi_arrow_both_Click(object sender, EventArgs e)
        {
            SetSelectedLineStyle(ArrowPositions.Both);
        }

        private void btn_copyMap_Click(object sender, EventArgs e)
        {
            MainMap.Copy();
        }

        private void btn_exprtMap_Click(object sender, EventArgs e)
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
        }

        private void btn_draw_highlight_Click(object sender, EventArgs e)
        {
            if (MainMap.SelectedObject == null)
                return;
            MainMap.SelectedObject.IsHighlighted = !MainMap.SelectedObject.IsHighlighted;
        }

        private void menu_line_Opening(object sender, CancelEventArgs e)
        {

        }

        private void menu_polyline_Opening(object sender, CancelEventArgs e)
        {

        }

        private void mi_marker_remove_Click(object sender, EventArgs e)
        {
            MainMap.SelectedObject?.Remove();
        }

        private void mi_marker_change_color_Click(object sender, EventArgs e)
        {
            var lg = new ColorDialog();
            if (MainMap.SelectedObject != null)
            {
                var dlg = new ColorDialog();
                dlg.Color = MainMap.SelectedObject.Color;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    MainMap.SelectedObject.Color = dlg.Color;
                }
            }
        }

        private void mi_marker_highlight_Click(object sender, EventArgs e)
        {
            if (MainMap.SelectedObject != null)
                MainMap.SelectedObject.IsHighlighted = !MainMap.SelectedObject.IsHighlighted;
        }

        private void mi_marker_copy_Click(object sender, EventArgs e)
        {
            MainMap.SelectedObject?.CopyToClipboard();
        }

        private void mi_textbox_editText_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapTextBox;
            if (o != null)
            {
                MainMap_OnTextBoxEdit(o);
            }
        }

        private void mi_marker_showInformation_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapBaseMarker;
            if (o != null)
            {
                o.ToolTipMode = MarkerTooltipMode.Always;
            }
        }

        private void mi_marke_showName_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapBaseMarker;
            if (o != null)
            {
                o.ToolTipShow = MarkerToolTipShow.Name;
            }
        }

        private void mi_marker_center_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapBaseMarker;
            if (o != null)
                MainMap.Position = o.Position;
        }

        private void mi_marker_zoomTo_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapBaseMarker;
            if (o != null)
            {
                MainMap.Position = o.Position;
                MainMap.Zoom = MainMap.MaxZoom - 1;
            }
        }

        private void mi_marker_hideAllInformation_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapBaseMarker;
            if (o != null)
                o.ToolTipMode = MarkerTooltipMode.Never;
        }

        private void mi_line_length_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapLine;
            if (o != null)
                o.ShowLength = !o.ShowLength;
        }

        private void mi_polyline_close_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapPolyLine;
            if (o != null)
                o.IsClosed = true;
        }

        private void mi_polyline_length_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapPolyLine;
            if (o != null)
                o.ShowLength = !o.ShowLength;

        }

        private void mi_polyline_addPoint_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapPolyLine;
            if (o != null)
                o.InsertPoint(m_rigthclickpoint);
        }

        private void mi_polyline_removePoint_Click(object sender, EventArgs e)
        {
            var o = MainMap.m_selected_object as MapPolyLine;
            if (o != null)
                o.RemovePoint(m_rigthclickpoint);
        }
    } /* class SgxMapControl */
} /* namespace SgxMap */
