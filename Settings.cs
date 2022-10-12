using pannella.analoguepocket;

namespace Pocket_Updater
{
    public partial class Settings : Form
    {
        SettingsManager _settings;
        public Settings()
        {
            InitializeComponent();
          
            //Tooltips
            toolTip1.SetToolTip(pictureBox1, "This is an Optional setting to use a Personal GitHub Token to avoid Rate Limit Issues/Errors.");
            toolTip2.SetToolTip(pictureBox2, "This will preserve any Custom Core Images, Core Naming, and Category changes made manually.");
            toolTip3.SetToolTip(pictureBox3, "This will enable/disable Arcade Rom and Core Bios Files.");
            toolTip4.SetToolTip(pictureBox4, "This will enable/disable the downloading of Pocket Firmware Updates.");
            
            //Read Settings Json file
            ReadSettings();
        }

        public void ReadSettings()
        {
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);
            
            //GitHub Token
            GitHub_Token.Text = _settings.GetConfig().github_token;

            //Preserve Core Images
            if(_settings.GetConfig().preserve_platforms_folder == true)
            {
                Core_Images.Checked = true;
            }
            else
            {
                Core_Images.Checked = false;
            }
            //Download Pocket Firmware
            if (_settings.GetConfig().download_firmware == true)
            {
                Download_Firmware.Checked = true;
            }
            else
            {
                Download_Firmware.Checked = false;
            }
            //Download Assets
            if (_settings.GetConfig().download_assets == true)
            {
                Download_Assets.Checked = true;
            }
            else
            {
                Download_Assets.Checked = false;
            }
        }

        private void Buttons_Save_Click(object sender, EventArgs e)
        {
            string value = GitHub_Token.Text;
            Config config = _settings.GetConfig();
            
            //GitHub Token
            config.github_token = value;
            
            //Preserve Core Images
            if(Core_Images.Checked == true)
            {
                config.preserve_platforms_folder = true;
            }
            else
            {
                config.preserve_platforms_folder = false;
            }
            //Download Pocket Firmware
            if (Download_Firmware.Checked == true)
            {
                config.download_firmware = true;
            }
            else
            {
                config.download_firmware = false;
            }
            //Download Assets
            if (Download_Assets.Checked == true)
            {
                config.download_assets = true;
            }
            else
            {
                config.download_assets = false;
            }

            //Save Sttings
            _settings.UpdateConfig(config);
            _settings.SaveSettings();
            MessageBox.Show("Settings Saved!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        private void Settings_Load(object sender, EventArgs e)
        {

        }

    }
}
