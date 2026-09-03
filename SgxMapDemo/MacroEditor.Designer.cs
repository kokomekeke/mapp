namespace SgxMapDemo
{
    partial class MacroEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            btn_save = new Button();
            btn_clear = new Button();
            btn_run = new Button();
            btn_close = new Button();
            textBox_editor = new TextBox();
            btn_open = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btn_open);
            panel1.Controls.Add(btn_save);
            panel1.Controls.Add(btn_clear);
            panel1.Controls.Add(btn_run);
            panel1.Controls.Add(btn_close);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 269);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(558, 43);
            panel1.TabIndex = 0;
            // 
            // btn_save
            // 
            btn_save.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_save.Location = new Point(218, 8);
            btn_save.Margin = new Padding(4, 3, 4, 3);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(88, 27);
            btn_save.TabIndex = 1;
            btn_save.Text = "Save";
            btn_save.UseVisualStyleBackColor = true;
            btn_save.Click += btn_save_Click;
            // 
            // btn_clear
            // 
            btn_clear.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_clear.Location = new Point(14, 8);
            btn_clear.Margin = new Padding(4, 3, 4, 3);
            btn_clear.Name = "btn_clear";
            btn_clear.Size = new Size(88, 27);
            btn_clear.TabIndex = 0;
            btn_clear.Text = "New";
            btn_clear.UseVisualStyleBackColor = true;
            btn_clear.Click += btn_clear_Click;
            // 
            // btn_run
            // 
            btn_run.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_run.Location = new Point(362, 8);
            btn_run.Margin = new Padding(4, 3, 4, 3);
            btn_run.Name = "btn_run";
            btn_run.Size = new Size(88, 27);
            btn_run.TabIndex = 2;
            btn_run.Text = "Run";
            btn_run.UseVisualStyleBackColor = true;
            btn_run.Click += btn_run_Click;
            // 
            // btn_close
            // 
            btn_close.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btn_close.Location = new Point(457, 8);
            btn_close.Margin = new Padding(4, 3, 4, 3);
            btn_close.Name = "btn_close";
            btn_close.Size = new Size(88, 27);
            btn_close.TabIndex = 3;
            btn_close.Text = "Close";
            btn_close.UseVisualStyleBackColor = true;
            btn_close.Click += btn_close_Click;
            // 
            // textBox_editor
            // 
            textBox_editor.Dock = DockStyle.Fill;
            textBox_editor.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 238);
            textBox_editor.Location = new Point(0, 0);
            textBox_editor.Margin = new Padding(4, 3, 4, 3);
            textBox_editor.Multiline = true;
            textBox_editor.Name = "textBox_editor";
            textBox_editor.Size = new Size(558, 269);
            textBox_editor.TabIndex = 0;
            // 
            // btn_open
            // 
            btn_open.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn_open.Location = new Point(116, 8);
            btn_open.Margin = new Padding(4, 3, 4, 3);
            btn_open.Name = "btn_open";
            btn_open.Size = new Size(88, 27);
            btn_open.TabIndex = 4;
            btn_open.Text = "Open";
            btn_open.UseVisualStyleBackColor = true;
            btn_open.Click += btn_open_Click;
            // 
            // MacroEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(558, 312);
            Controls.Add(textBox_editor);
            Controls.Add(panel1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MacroEditor";
            Text = "Macro Editor";
            FormClosing += MacroEditor_FormClosing;
            FormClosed += MacroEditor_FormClosed;
            Load += MacroEditor_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.TextBox textBox_editor;
        private System.Windows.Forms.Button btn_save;
        private Button btn_open;
    }
}
