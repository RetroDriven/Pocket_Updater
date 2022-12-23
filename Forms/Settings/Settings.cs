using pannella.analoguepocket;
using System.IO;
using System.Windows.Forms;

namespace Pocket_Updater
{
    public partial class Settings : Form
    {
        SettingsManager _settings;

        public Settings()
        {
            InitializeComponent();

            //Grid Formatting
            dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                //col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            //Tooltips
            toolTip1.SetToolTip(pictureBox1, "This is an Optional setting to use a Personal GitHub Token to avoid Rate Limit Issues/Errors");

            //Read Settings Json file
            ReadSettingsAsync();
        }

        public async Task ReadSettingsAsync()
        {

            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);

            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);

            //GitHub Token
            GitHub_Token.Text = _settings.GetConfig().github_token;

            //Download Pocket Firmware
            if (_settings.GetConfig().download_firmware == true)
            {
                dataGridView1.Rows.Add(true,"Download Pocket Firmware", "Enable/Disable Downloading Pocket Firmware Updates");
            }
            else
            {
                dataGridView1.Rows.Add(false,"Download Pocket Firmware", "Enable/Disable Downloading Pocket Firmware Updates");
            }
            //Download Assets
            if (_settings.GetConfig().download_assets == true)
            {
                dataGridView1.Rows.Add(true,"Download ROMS/BIOS", "Enable/Disable Downloading Arcade ROMS and Core BIOS");
            }
            else
            {
                dataGridView1.Rows.Add(false,"Download ROMS/BIOS", "Enable/Disable Downloading Arcade ROMS and Core BIOS");
            }
            //Preserve Core Images
            if (_settings.GetConfig().preserve_platforms_folder == true)
            {
                dataGridView1.Rows.Add(true,"Preserve Platforms", "Preserve Custom Core Images, Core Naming, and Category changes made manually");
            }
            else
            {
                dataGridView1.Rows.Add(false,"Preserve Platforms", "Preserve Custom Core Images, Core Naming, and Category changes made manually");
            }
            //Pre-release Cores
            List<Core> cores = await CoresService.GetCores();
            foreach (Core core in cores)
            {
                CoreSettings c = _settings.GetCoreSettings(core.identifier);
                if (c.allowPrerelease == true)
                {
                    dataGridView1.Rows.Add(true,"Download Pre-Release Cores","Enable/Disable Downloadig Alpha/Beta Pre-Release Cores");
                    break;
                }
                else
                {
                    dataGridView1.Rows.Add(false, "Download Pre-Release Cores", "Enable/Disable Downloadig Alpha/Beta Pre-Release Cores");
                    break;
                }
            }
            //Delete Skipped Cores
            if (_settings.GetConfig().delete_skipped_cores == true)
            {
                dataGridView1.Rows.Add(true, "Delete Skipped Cores", "Delete Cores from your Pocket's SD Card that you have unchecked for Downloading");
            }
            else
            {
                dataGridView1.Rows.Add(false, "Delete Skipped Cores", "Delete Cores from your Pocket's SD Card that you have unchecked for Downloading");
            }

        }
        private async void Button_Save_Click(object sender, EventArgs e)
        {
            string value = GitHub_Token.Text;
            Config config = _settings.GetConfig();
            
            //GitHub Token
            config.github_token = value;

            //Download Pocket Firmware
            string Firmware = dataGridView1.Rows[0].Cells[0].Value.ToString();

            if (Firmware == "True")
            {
                config.download_firmware = true;
            }
            else
            {
                config.download_firmware = false;
            }
            //Download Assets
            string Assets = dataGridView1.Rows[1].Cells[0].Value.ToString();

            if (Assets == "True")
            {
                config.download_assets = true;
            }
            else
            {
                config.download_assets = false;
            }
            //Preserve Core Images
            string Preserve = dataGridView1.Rows[2].Cells[0].Value.ToString();

            if (Preserve == "True")
            {
                config.preserve_platforms_folder = true;
            }
            else
            {
                config.preserve_platforms_folder = false;
            }
            //Pre-Release Cores
                List<Core> cores = await CoresService.GetCores();
            foreach (Core core in cores)
            {
                CoreSettings c = _settings.GetCoreSettings(core.identifier);

                string Pre = dataGridView1.Rows[3].Cells[0].Value.ToString();

                if (Pre == "True")
                {
                    c.allowPrerelease = true;
                    _settings.UpdateCore(c, core.identifier);
                    break;
                }
                else
                {
                    c.allowPrerelease = false;
                    _settings.UpdateCore(c, core.identifier);
                    break;
                }
            }
            //Delete Skipped Cores
            string Skipped = dataGridView1.Rows[4].Cells[0].Value.ToString();

            if (Skipped == "True")
            {
                config.delete_skipped_cores = true;
            }
            else
            {
                config.delete_skipped_cores = false;
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
