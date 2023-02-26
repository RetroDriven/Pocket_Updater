using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pocket_Updater.Controls.About
{
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        private void GitHub_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater");
        }

        private void Release_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater/releases/latest");
        }

        private void ReadMe_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater/blob/master/README.md");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater/issues");
        }

        private void Matt_GitHub_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/mattpannella/pocket-updater-utility");
        }

        private void Josh_GitHub_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/openfpga-cores-inventory/analogue-pocket");
        }
    }
}
