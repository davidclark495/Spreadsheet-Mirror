using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void CalcButton_Click(object sender, EventArgs e)
        {
            if (!checkInputSanity())
                return;
            
            double tipTotal = Double.Parse(BillText.Text) * Double.Parse(TipPercentText.Text)/100.0;
            TipAmountText.Text = "" + tipTotal;
            TotalAmountText.Text = "$" + (Double.Parse(BillText.Text) + Double.Parse(TipAmountText.Text));
        }

        private bool checkInputSanity()
        {
            if (Double.TryParse(BillText.Text, out double unused1) &&
                Double.TryParse(TipPercentText.Text, out double unused2))
            {
                CalcButton.Enabled = true;
                return true;
            }
            else
            {
                CalcButton.Enabled = false;
                return false;
            }
        }

        private void TotalLabel_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void BillText_TextChanged(object sender, EventArgs e)
        {
            checkInputSanity();
        }

        private void TipPercentText_TextChanged(object sender, EventArgs e)
        {
            checkInputSanity();
        }
    }
}
