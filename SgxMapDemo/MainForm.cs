/*=============================================================================
*  MainForm: Main form for SgxMapDemo application.
*
* (c) Kótai 2026.
*
* ver 1.00                                                         31-Aug-2026.
=============================================================================*/
using System.Reflection;
using GMap.NET;
using SgxMap;
//using GMap.NET;
//using GMap.NET.Internals;

namespace SgxMapDemo
{
    public partial class MainForm : Form
    {
        public static MainForm? Instance = null;

        public MainForm()
        {
            InitializeComponent();
            Instance = this;

            if (!SgxMapPanel.IsDesignerHosted)
            {
                Text += " - Build=" + AssemblyCreationDate().ToString("yyyy.MM.dd HH:mm");
#if DEBUG
                Text += " - DEBUG";
#endif

                //----------------------------------------
                // Config Map at Startup
                //----------------------------------------
                //sgxMapControl1.MainMap.MapProvider = Program.mapProvider;  //GMapProviders.GoogleMap;
                sgxMapControl1.MainMap.Manager.Mode = Program.accesmode;
                sgxMapControl1.MainMap.Position = new PointLatLng(Program.initialLatitude, Program.initialLongitude);
                sgxMapControl1.MainMap.MinZoom = 0;
                sgxMapControl1.MainMap.MaxZoom = 24;
                sgxMapControl1.MainMap.Zoom = Program.initialZoom;
                sgxMapControl1.MainMap.ScaleMode = ScaleModes.Fractional;
                sgxMapControl1.MapProvider = Program.mapProvider;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static DateTime AssemblyCreationDate()
        {
            var asm = Assembly.GetExecutingAssembly();
            var fi = new FileInfo(asm.Location);
            return fi.LastWriteTime; //itt eredetileg itt .CreationTime volt, de ez igy jobban követi a lokal re-buldeket is B
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.mapProvider = sgxMapControl1.MapProvider;
            Program.initialZoom = sgxMapControl1.Zoom;
            Program.initialLatitude = sgxMapControl1.MainMap.Position.Lat;
            Program.initialLongitude = sgxMapControl1.MainMap.Position.Lng;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "gmc";
            dlg.Filter = "GMap macro file (*.gmc)|*.gmc|All file (*.*)|*.*";
            dlg.Title = "Select GMAP Macro file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (sgxMapControl1.MainMap.LoadMacroFile(dlg.FileName))
                {
                    MacroEditor.MacroText = File.ReadAllText(dlg.FileName);
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = MacroEditor.GetInstance();
            if (frm != null)
            {
                frm.Show();
                frm.BringToFront();
                frm.TopLevel = true;
            }
        }

    } /* class MainForm */
} /* namespace SgxMapDemo */
