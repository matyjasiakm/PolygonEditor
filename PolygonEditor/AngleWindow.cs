using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GK1
{
    public partial class AngleWindow : Form
    {
        public int d;
        public AngleWindow(int deegre)
        {
            InitializeComponent();
            d = deegre;
            textBox1.Text = deegre.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (int.TryParse(textBox1.Text, out int p))
            {
                DialogResult = DialogResult.OK;
                d = p;
            }
            else
            {
                MessageBox.Show(this,"Value must be intiger!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            
        }
    }
}
