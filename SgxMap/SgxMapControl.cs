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


        public SgxMapControl()
        {
            InitializeComponent();
            LoadControls();
        }



        public void LoadControls()
        {
            comboBoxMapType.ValueMember = "Name";
            comboBoxMapType.DataSource = GMapProviders.List;
            comboBoxMapType.SelectedItem = MainMap.MapProvider;
        }

        private void comboBoxMapType_SelectedValueChanged(object sender, EventArgs e)
        {
            MainMap.MapProvider = comboBoxMapType.SelectedItem as GMapProvider;
        }


    } /* class SgxMapControl */
} /* namespace SgxMap */
