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

#if !SQLite
                casheToolStripMenuItem.Visible = false;
#endif
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
                    bool ok = sgxMapControl1.MainMap.ExportToGMDB(dlg.FileName);
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
                    bool ok = sgxMapControl1.MainMap.ImportFromGMDB(dlg.FileName);
                    if (ok)
                    {
                        MessageBox.Show("Complete!", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        sgxMapControl1.MainMap.ReloadMap();
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


        private void mi_load_macro_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "gmc";
            dlg.Filter = "SgxMap macro file (*.gmc)|*.gmc|All file (*.*)|*.*";
            dlg.Title = "Select SGXMAP Macro file";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (sgxMapControl1.MainMap.LoadMacroFile(dlg.FileName))
                {
                    MacroEditor.MacroText = File.ReadAllText(dlg.FileName);
                }
            }
        }

        private void mi_edit_macro_Click(object sender, EventArgs e)
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
