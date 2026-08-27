using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo.WindowsForms.Forms
{
    public partial class AngleCalculator : Form
    {
        public static AngleCalculator Instance = null;


        public AngleCalculator()
        {
            InitializeComponent();
            Instance = this;
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            txe_degree.Text = "";
            txe_minute.Text = "";
            txe_second.Text = "";
            lbl_result.Text = "";
            txe_degree.Focus();
        }

        private void btn_calculate_Click(object sender, EventArgs e)
        {
            try
            {
                double degree = string.IsNullOrEmpty(txe_degree.Text) ? 0.0 : double.Parse(txe_degree.Text);
                double minute = string.IsNullOrEmpty(txe_minute.Text) ? 0.0 : double.Parse(txe_minute.Text);
                double second = string.IsNullOrEmpty(txe_second.Text) ? 0.0 : double.Parse(txe_second.Text);
                double result = (degree * 3600.0 + minute * 60.0 + second) / 3600.0;
                lbl_result.Text = result.ToString("F6");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please enter valid numbers for degree, minute, and second.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbl_result.Text))
                Clipboard.SetText(lbl_result.Text);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
            Instance = null;
        }
    }
}
