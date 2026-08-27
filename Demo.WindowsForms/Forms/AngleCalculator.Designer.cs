namespace Demo.WindowsForms.Forms
{
    partial class AngleCalculator
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
            this.txe_degree = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txe_minute = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txe_second = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbl_result = new System.Windows.Forms.Label();
            this.btn_calculate = new System.Windows.Forms.Button();
            this.btn_copy = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txe_degree
            // 
            this.txe_degree.Location = new System.Drawing.Point(12, 35);
            this.txe_degree.Name = "txe_degree";
            this.txe_degree.Size = new System.Drawing.Size(53, 20);
            this.txe_degree.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Fok";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(92, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Perc";
            // 
            // txe_minute
            // 
            this.txe_minute.Location = new System.Drawing.Point(80, 35);
            this.txe_minute.Name = "txe_minute";
            this.txe_minute.Size = new System.Drawing.Size(53, 20);
            this.txe_minute.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Másodperc";
            // 
            // txe_second
            // 
            this.txe_second.Location = new System.Drawing.Point(148, 35);
            this.txe_second.Name = "txe_second";
            this.txe_second.Size = new System.Drawing.Size(53, 20);
            this.txe_second.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Fok:";
            // 
            // lbl_result
            // 
            this.lbl_result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_result.Location = new System.Drawing.Point(56, 86);
            this.lbl_result.Name = "lbl_result";
            this.lbl_result.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lbl_result.Size = new System.Drawing.Size(130, 21);
            this.lbl_result.TabIndex = 7;
            // 
            // btn_calculate
            // 
            this.btn_calculate.Location = new System.Drawing.Point(12, 133);
            this.btn_calculate.Name = "btn_calculate";
            this.btn_calculate.Size = new System.Drawing.Size(63, 23);
            this.btn_calculate.TabIndex = 3;
            this.btn_calculate.Text = "Számold";
            this.btn_calculate.UseVisualStyleBackColor = true;
            this.btn_calculate.Click += new System.EventHandler(this.btn_calculate_Click);
            // 
            // btn_copy
            // 
            this.btn_copy.Location = new System.Drawing.Point(80, 133);
            this.btn_copy.Name = "btn_copy";
            this.btn_copy.Size = new System.Drawing.Size(63, 23);
            this.btn_copy.TabIndex = 4;
            this.btn_copy.Text = "Másol";
            this.btn_copy.UseVisualStyleBackColor = true;
            this.btn_copy.Click += new System.EventHandler(this.btn_copy_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(154, 164);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(63, 23);
            this.btn_close.TabIndex = 6;
            this.btn_close.Text = "Bezár";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(154, 132);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(63, 23);
            this.btn_clear.TabIndex = 5;
            this.btn_clear.Text = "Töröl";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.Location = new System.Drawing.Point(66, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(9, 9);
            this.label6.TabIndex = 12;
            this.label6.Text = "o";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(134, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(9, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "\'";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(203, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "\"";
            // 
            // AngleCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 196);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_copy);
            this.Controls.Add(this.btn_calculate);
            this.Controls.Add(this.lbl_result);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txe_second);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txe_minute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txe_degree);
            this.Name = "AngleCalculator";
            this.Text = "Szög átváltás";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txe_degree;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txe_minute;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txe_second;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbl_result;
        private System.Windows.Forms.Button btn_calculate;
        private System.Windows.Forms.Button btn_copy;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}