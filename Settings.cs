using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using pannella.analoguepocket;

namespace Pocket_Updater
{
    public partial class Settings : Form
    {
        SettingsManager _settings;
        public Settings()
        {
            InitializeComponent();
          
            Button_Save.Enabled = false;

            //Tooltips
            toolTip1.SetToolTip(pictureBox1, "This is an Optional setting to use a Personal GitHub Token to avoid Rate Limit Issues/Errors.");
            ReadSettings();
        }

        public void ReadSettings()
        {
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);
            textBox1.Text = _settings.GetConfig().github_token;
        }

        private void Buttons_Save_Click(object sender, EventArgs e)
        {
            string value = textBox1.Text;
            Config config = _settings.GetConfig();
            config.github_token = value;
            _settings.UpdateConfig(config);
            _settings.SaveSettings();
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
