using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo.WindowsForms.Forms
{
    public partial class MacroEditor : Form
    {
        public static string MacroText = "";
        public string MacroFilename = "";

        private static MacroEditor Instance = null;

        public static MacroEditor GetInstance()
        {
            if (Instance == null)
                Instance = new MacroEditor();
            return Instance;
        }

        public MacroEditor()
        {
            InitializeComponent();
        }

        private void MacroEditor_Load(object sender, EventArgs e)
        {
            textBox_editor.Text = MacroText;
            textBox_editor.Focus();
            if (!string.IsNullOrEmpty(MacroFilename))
                 this.Text = "Macro Editor - " + Path.GetFileName(MacroFilename);
            else this.Text = "Macro Editor";
        }

        private void MacroEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            MacroText = textBox_editor.Text;
        }

        private void MacroEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Instance = null;
        }


        public void Clear()
        {
            textBox_editor.Clear();
            MacroFilename = "";
            this.Text = "Macro Editor";
        }

        public void LoadText()
        {
            textBox_editor.Text = MacroText;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            MacroText = textBox_editor.Text;
            Close();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox_editor.Text))
            {
                var dlg = new SaveFileDialog();
                dlg.DefaultExt = "gmc";
                dlg.CheckPathExists = true;
                dlg.Filter= "GMap macro file (*.gmc)|*.gmc|All file (*.*)|*.*";
                dlg.Title = "Save GMAP Macro in file";
                dlg.FileName = MacroFilename;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var f = new StreamWriter(dlg.FileName))
                            f.Write(textBox_editor.Text);
                        MacroFilename = dlg.FileName;
                        this.Text = "Macro Editor - " + Path.GetFileName(MacroFilename);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ERROR");
                    }
                }
            }
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            var frm = MainForm.Instance;
            if (frm != null)
            {
                for (int i = 0; i < textBox_editor.Lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(textBox_editor.Lines[i]))
                         continue; 
                    if (textBox_editor.Lines[i].StartsWith("#"))
                        continue;
                    if (!frm.ProcessMacroCommand(textBox_editor.Lines[i], out string errorstring))
                    {
                        MessageBox.Show(errorstring, "ERROR");
                        break;
                    }
                }
            }
            MacroText = textBox_editor.Text;
            //Close();
        }

    } /* class MacroEditor */
} /* namespace Demo.WindowsForms.Forms */
