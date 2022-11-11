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
            toolTip5.SetToolTip(pictureBox5, "This will enable/disable the downloading of Pre-Release Cores.");
            toolTip6.SetToolTip(pictureBox6, "This will Delete Cores from your Pocket's SD Card that you have unchecked for Updating/Downloading.");

            //Read Settings Json file
            ReadSettingsAsync();
        }

        public async Task ReadSettingsAsync()
        {
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);

            //GitHub Token
            GitHub_Token.Text = _settings.GetConfig().github_token;

            //Preserve Core Images
            if (_settings.GetConfig().preserve_platforms_folder == true)
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
            //Delete Skipped Cores
            if (_settings.GetConfig().delete_skipped_cores == true)
            {
                Skipped.Checked = true;
            }
            else
            {
                Skipped.Checked = false;
            }
            //Pre-release Cores
            List<Core> cores = await CoresService.GetCores();
            foreach (Core core in cores)
            {
                CoreSettings c = _settings.GetCoreSettings(core.identifier);
                if( c.allowPrerelease == true)
                {
                    PreRelease.Checked = true;
                }
                else
                {
                    PreRelease.Checked = false;
                }
            }
        }
        private async void Button_Save_Click(object sender, EventArgs e)
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
            //Delete Skipped Cores
            if (Skipped.Checked == true)
            {
                config.delete_skipped_cores = true;
            }
            else
            {
                config.delete_skipped_cores = false;
            }
            //Pre-Release Cores
            List<Core> cores = await CoresService.GetCores();
            foreach (Core core in cores)
            {
                CoreSettings c = _settings.GetCoreSettings(core.identifier);

                if (PreRelease.Checked == true)
                {
                    c.allowPrerelease = true;
                    _settings.UpdateCore(c, core.identifier);
                }
                else
                {
                    c.allowPrerelease = false;
                    _settings.UpdateCore(c, core.identifier);
                }
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
