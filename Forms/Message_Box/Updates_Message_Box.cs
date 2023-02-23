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

namespace Pocket_Updater.Forms.Message_Box
{
    public partial class Updates_Message_Box : Form
    {
        private const string API_URL = "https://api.github.com/repos/RetroDriven/Pocket_Updater/releases";
        private const string RELEASE_URL = "https://github.com/RetroDriven/Pocket_Updater/releases/latest";

        public Updates_Message_Box()
        {
            InitializeComponent();
        }

        private void Button_Yes_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", RELEASE_URL);
            Close();
        }

        private void Button_No_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
