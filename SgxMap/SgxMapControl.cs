/*=============================================================================
*  SgxMapControl: User control for SgxMapPanel, 
*  providing a map interface with toolbar, drawbar, and status bar.
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
*          Last modification                                       02-Sep-2026.
=============================================================================*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
//using GMap.NET;
//using GMap.NET.Internals;

namespace SgxMap
{
    public partial class SgxMapControl : UserControl
    {
        private object m_monitor = new object();

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

        public SgxMapControl()
        {
            InitializeComponent();
            LoadControls();
            MainMap.MouseDown += MainMap_MouseDown;
            MainMap.showPlaceInfo += MainMap_ShowPlaceInfo;
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

        private void comboBoxMapType_SelectedValueChanged(object sender, EventArgs e)
        {
            MainMap.MapProvider = comboBoxMapType.SelectedItem as GMapProvider;
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

            m.Show(DrawPanel, new Point(btn_draw.Left, btn_draw.Bottom));
        } /* btn_draw_Click */

        private void btn_drtaw_line_Click(object sender, EventArgs e)
        {
            MainMap.MouseMoveStatus = MouseMoveStatusType.Line;
        }

        private void btn_draw_ruler_Click(object sender, EventArgs e)
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
            if (MainMap.MouseMoveStatus == MouseMoveStatusType.Textbox)
            {
                if (textBox_editor.Visible)
                {
                    if (textBox_editor.Focused)
                        return;
                    textBox_editor.Visible = false;
                    MainMap.MouseMoveStatus = MouseMoveStatusType.None;
                    var t = MainMap.Layers[0].AddTextBox("", textBox_editor.Location.X, textBox_editor.Location.Y - MainMap.Location.Y + textBox_editor.Height, textBox_editor.Text, DefaultFont);
                    t.Color = Color.Red;
                    t.Thickness = 1;
                    return;
                }
                textBox_editor.Visible = true;
                textBox_editor.Text = string.Empty;
                textBox_editor.Location = new Point(e.X, e.Y + MainMap.Location.Y);
                textBox_editor.Focus();
                Monitor.Enter(m_monitor);
            }
        }

        private void btn_newMap_Click(object sender, EventArgs e)
        {
            MainMap.ClearAll();
        }
    } /* class SgxMapControl */
} /* namespace SgxMap */
