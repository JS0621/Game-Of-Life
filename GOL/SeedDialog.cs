using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GOL;

namespace GOL
{
    public partial class SeedDialog : Form
    {
        int seedInt;
        public SeedDialog()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;

            InitializeComponent();
        }
        //Getter to access seed
        public int GetSeed()
        {
            return seedInt;
        }

        //OK Functionality
        private void OK_Click(object sender, EventArgs e)
        {
            string seed = textBox1.Text;
            if (int.TryParse(seed, out seedInt))
            {
                Close();
            }
            else
            {
                //Error
                errorProvider1.SetError(textBox1, "Please Input Integer");
                errorProvider1.Clear();
            }
        }

        //Cancel Functionality
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
