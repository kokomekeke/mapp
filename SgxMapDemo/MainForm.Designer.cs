namespace SgxMapDemo
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            macroToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            casheToolStripMenuItem = new ToolStripMenuItem();
            mi_importCash = new ToolStripMenuItem();
            mi_exportcash = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            mi_use_cash_only = new ToolStripMenuItem();
            sgxMapControl1 = new SgxMap.SgxMapControl();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, macroToolStripMenuItem, casheToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(92, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // macroToolStripMenuItem
            // 
            macroToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadToolStripMenuItem, editToolStripMenuItem });
            macroToolStripMenuItem.Name = "macroToolStripMenuItem";
            macroToolStripMenuItem.Size = new Size(53, 20);
            macroToolStripMenuItem.Text = "Macro";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(100, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(100, 22);
            editToolStripMenuItem.Text = "Edit";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // casheToolStripMenuItem
            // 
            casheToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mi_importCash, mi_exportcash, toolStripMenuItem1, mi_use_cash_only });
            casheToolStripMenuItem.Name = "casheToolStripMenuItem";
            casheToolStripMenuItem.Size = new Size(51, 20);
            casheToolStripMenuItem.Text = "Cashe";
            // 
            // mi_importCash
            // 
            mi_importCash.Name = "mi_importCash";
            mi_importCash.Size = new Size(127, 22);
            mi_importCash.Text = "&Import";
            mi_importCash.ToolTipText = "Load saved cached map from file";
            // 
            // mi_exportcash
            // 
            mi_exportcash.Name = "mi_exportcash";
            mi_exportcash.Size = new Size(127, 22);
            mi_exportcash.Text = "&Export";
            mi_exportcash.ToolTipText = "Save cashed map in file";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(124, 6);
            // 
            // mi_use_cash_only
            // 
            mi_use_cash_only.Name = "mi_use_cash_only";
            mi_use_cash_only.Size = new Size(127, 22);
            mi_use_cash_only.Text = "Cach only";
            mi_use_cash_only.ToolTipText = "Use cashed map only";
            // 
            // sgxMapControl1
            // 
            sgxMapControl1.Dock = DockStyle.Fill;
            sgxMapControl1.Location = new Point(0, 24);
            sgxMapControl1.Name = "sgxMapControl1";
            sgxMapControl1.ShowDrawbar = true;
            sgxMapControl1.ShowStatusbar = true;
            sgxMapControl1.ShowToolbar = true;
            sgxMapControl1.Size = new Size(800, 616);
            sgxMapControl1.TabIndex = 1;
            sgxMapControl1.Zoom = 0D;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 640);
            Controls.Add(sgxMapControl1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "SgxMap Demo";
            FormClosing += MainForm_FormClosing;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        public SgxMap.SgxMapControl sgxMapControl1;
        private ToolStripMenuItem macroToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem casheToolStripMenuItem;
        private ToolStripMenuItem mi_importCash;
        private ToolStripMenuItem mi_exportcash;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mi_use_cash_only;
    }
}
