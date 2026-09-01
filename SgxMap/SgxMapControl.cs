/*=============================================================================
*  SgxMapControl: User control for SgxMapPanel, 
*  providing a map interface with toolbar, drawbar, and status bar.
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
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

    } /* class SgxMapControl */
} /* namespace SgxMap */
