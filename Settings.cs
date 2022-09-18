using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pannella.analoguepocket;

namespace Pocket_Updater
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
          
            Button_Save.Enabled = false;

            //Tooltips
            toolTip1.SetToolTip(pictureBox1, "This is an Optional setting to use a Personal GitHub Token to avoid Rate Limit Issues/Errors.");

        }

        private void Buttons_Save_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Settings Saved!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text != null)
            {
                Button_Save.Enabled = true;
            }
        }
    }
}
