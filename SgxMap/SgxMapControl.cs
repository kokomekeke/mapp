/*=============================================================================
*  SgxMapControl: User control for SgxMapPanel, 
*  providing a map interface with toolbar, drawbar, and status bar.
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
*          Last modification                                       14-Sep-2026.
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
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using SgxMap.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static GMap.NET.Entity.OpenStreetMapGraphHopperGeocodeEntity;
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
        private int selected_arrowstale = 0;

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

        private void btn_drtaw_line_Click(object sender, EventArgs e)
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
            //if (MainMap.MouseMoveStatus == MouseMoveStatusType.Textbox)
            //{
            //    if (textBox_editor.Visible)
            //    {
            //        if (textBox_editor.Focused)
            //            return;
            //        textBox_editor.Visible = false;
            //        MainMap.MouseMoveStatus = MouseMoveStatusType.None;
            //        var t = MainMap.Layers[0].AddTextBox("", textBox_editor.Location.X, textBox_editor.Location.Y - MainMap.Location.Y + textBox_editor.Height, textBox_editor.Text, DefaultFont);
            //        if (t != null)
            //        {
            //            t.Color = Color.Red;
            //            t.Thickness = 1;
            //        }
            //        return;
            //    }
            //    textBox_editor.Visible = true;
            //    textBox_editor.Text = string.Empty;
            //    textBox_editor.Location = new Point(e.X, e.Y + MainMap.Location.Y);
            //    textBox_editor.Focus();
            //    Monitor.Enter(m_monitor);
            //}

            //var pos = MainMap.FromLocalToLatLng(e.X, e.Y);
            //var loc = MainMap.FromLatLngToLocal(pos);
            //var l2 = MainMap.FromLatLngToLocalPosition(pos);
            //var l3 = MainMap.Core.FromLatLngToLocal(pos);
            //var g1 = MainMap.FromLatLngToPixel(pos);
            //var g2 = g1;
            //g2.Offset(MainMap.Core.RenderOffset);


            if (m_textbox != null)
                m_textbox.Text = textBox_editor.Text;
            if (textBox_editor.Visible)
                textBox_editor.Visible = false;
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
                MainMap.SelectedObject.FillColor = m_selected_fillcolor;
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

    } /* class SgxMapControl */
} /* namespace SgxMap */
