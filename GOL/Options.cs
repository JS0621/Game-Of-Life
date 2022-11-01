using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL
{
    public partial class Options : Form
    {
        int Interval = 20;
        int UniWidth = 10;
        int UniHeight = 10;
        public Options()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            InitializeComponent();
        }

        //Getters
        public int GetInterval()
        {
            return Interval;
        }

        public int GetWidth()
        {
            return UniWidth;
        }
        public int GetHegiht()
        {
            return UniHeight;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            string interval = textBox1.Text;
            string width = textBox2.Text;
            string height = textBox3.Text;
            if (int.TryParse(interval, out Interval) && int.TryParse(width, out UniWidth) && int.TryParse(height, out UniHeight))
                Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
