using System.Data;
using System.Net;
using System.Reflection;
using pannella.analoguepocket;
using RetroDriven;

namespace Pocket_Updater
{
    public partial class Update_Pocket : Form
    {
        public string Pocket_Drive { get; set; }
        public string Current_Dir { get; set; }

        private WebClient WebClient;
        private PocketCoreUpdater _updater;
        private SettingsManager _settings;

        //Initialize Update Status Form Popup
        Updater_Status Status = new Updater_Status();

        //Initialize Update Summery Form Popup
        Updater_Summary Summary = new Updater_Summary();

        public Update_Pocket()
        {
            InitializeComponent();

            //Get USB Drives
            PopulateDrives();
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);

            //Tooltips
            toolTip1.SetToolTip(Button_Refresh, "Refresh your Removable Drive List");

        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://google.com"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public void Download_Json(string Drive)
        {
            string Json_URL = "https://raw.githubusercontent.com/mattpannella/pocket_core_autoupdate_net/main/pocket_updater_cores.json";
            WebClient = new WebClient();
            //string Current_Dir = Directory.GetCurrentDirectory();

            try
            {
                string updateFile = Drive + "\\pocket_updater_cores.json";
                Console.WriteLine(updateFile);
                WebClient.DownloadFile(new Uri(Json_URL), updateFile);
                Button_Removable.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* ------ STUFF MATT ADDED FOR EXAMPLE ------ */

        //Call this method from a new button and you can run the core updater
        public async Task RunCoreUpdateProcess(string updatePath, string coresJsonPath, string LogDir)
        {
            Button_Removable.Enabled = false;
            Button_Refresh.Enabled = false;
            comboBox1.Enabled = false;
            //GitHub Token
            //_updater.SetGithubApiKey("apikey");
            await _updater.RunUpdates();
            Button_Removable.Enabled = true;
            Button_Refresh.Enabled = true;
            comboBox1.Enabled = true;

            Get_Jsons(updatePath);

            //Write to a Log File
            Write_Log(LogDir, "Pocket_Updater_Log.txt", Summary.textBox1.Text);

            //Close the Updater
            Close();
        }

        private void updater_StatusUpdated(object sender, StatusUpdatedEventArgs e)
        {
            //Show Updater Status in a new Form
            Status.textBox1.AppendText(e.Message);
            Status.textBox1.AppendText(Environment.NewLine);
        }

        private async void updateCoresButton_Click(object sender, EventArgs e)
        {
            //Clear the info box
            //infoTextBox.Clear();

            string Location_Type = comboBox2.SelectedItem.ToString();
            string Current_Dir = Directory.GetCurrentDirectory();
            string github_token = _settings.GetConfig().github_token;

            //Current Drive Updater
            if (Location_Type == "Current Directory")
            {
                //Check for an Internet Connection
                bool result = CheckForInternetConnection();

                if (result == false)
                {
                    string message = "No Internet Connection Detected!";
                    string title = "Error";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result2 = MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);
                }
                else
                {
                    //Download_Json(Current_Dir);
                    _updater = new PocketCoreUpdater(Current_Dir);
                    await _updater.Initialize();
                    //_updater.DownloadAssets(true); //turns on the option to also download bios files

                    _updater.SetGithubApiKey(_settings.GetConfig().github_token);
                    _updater.DownloadFirmware(_settings.GetConfig().download_firmware);
                    _updater.DownloadAssets(_settings.GetConfig().download_assets);
                    _updater.PreservePlatformsFolder(_settings.GetConfig().preserve_platforms_folder);

                    if (github_token != null)
                    {
                        _updater.SetGithubApiKey(github_token);
                    }
                    
                    Status.Show();
                    
                    _updater.StatusUpdated += updater_StatusUpdated;
                    _updater.UpdateProcessComplete += _updater_UpdateProcessComplete;

                    RunCoreUpdateProcess(Current_Dir, Current_Dir, Current_Dir);
                }
            }
            //Removable Drive Updater
            if (Location_Type == "Removable Storage")
            {
                string pathToUpdate = Pocket_Drive;

                //Check for an Internet Connection
                bool internet = CheckForInternetConnection();

                if (internet == false)
                {
                    string message = "No Internet Connection Detected!";
                    string title = "Error";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result2 = MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);
                }
                else
                {
                    //Make Sure Drive Letter still exists
                    var drives = DriveInfo.GetDrives();
                    if (drives.Where(data => data.Name == Pocket_Drive).Count() == 1)
                    {
                        //Download_Json(pathToUpdate);
                        // string Current_Dir = Directory.GetCurrentDirectory();
                        _updater = new PocketCoreUpdater(pathToUpdate, Current_Dir);
                        await _updater.Initialize();
                        //_updater.CoresFile = pathToUpdate;
                        //_updater.DownloadAssets(true); //turns on the option to also download bios files

                        //Get Config Settings
                        _updater.SetGithubApiKey(_settings.GetConfig().github_token);
                        _updater.DownloadFirmware(_settings.GetConfig().download_firmware);
                        _updater.DownloadAssets(_settings.GetConfig().download_assets);
                        _updater.PreservePlatformsFolder(_settings.GetConfig().preserve_platforms_folder);

                        if (github_token != null)
                        {
                            _updater.SetGithubApiKey(github_token);
                        }

                        Status.Show();

                        _updater.StatusUpdated += updater_StatusUpdated;
                        _updater.UpdateProcessComplete += _updater_UpdateProcessComplete;

                        RunCoreUpdateProcess(pathToUpdate, Current_Dir, Current_Dir);
                    }
                    else
                    {
                        MessageBox.Show("The Drive Letter Was Not Found!", "Drive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label2.Enabled = true;
                        comboBox1.Enabled = true;
                        Button_Refresh.Enabled = true;
                        Button_Removable.Enabled = true;
                        PopulateDrives();
                    }
                }
            }
        }
        private void _updater_UpdateProcessComplete(object? sender, UpdateProcessCompleteEventArgs e)
        {

            //No Updates Found
            if (e.InstalledCores.Count == 0 && e.InstalledAssets.Count == 0 && e.FirmwareUpdated == "")
            {
                Summary.Close();
                MessageBox.Show("No Updates Found!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Status.Close();
            }

            //Updates Found
            if (e.InstalledCores.Count > 0  || e.InstalledAssets.Count > 0 || e.FirmwareUpdated != "")
            {
                MessageBox.Show("Updates Complete!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Status.Close();
                Summary.Show();
                Summary.textBox1.SelectionLength = 0;
                Summary.textBox1.SelectionStart = 0;
                Summary.textBox1.ScrollToCaret();
            }

            //Cores Installed
            if (e.InstalledCores.Count > 0)
            {
                Summary.textBox1.AppendText("Cores Updated:(" + e.InstalledCores.Count + ")");
                Summary.textBox1.AppendText(Environment.NewLine);
                Summary.textBox1.AppendText("-----------------------");
                Summary.textBox1.AppendText(Environment.NewLine);

                foreach (Dictionary<string, string> core in e.InstalledCores)
                {
                    Summary.textBox1.AppendText(core["platform"] + " v" + core["version"]);
                    Summary.textBox1.AppendText(Environment.NewLine);
                }
                Summary.textBox1.AppendText(Environment.NewLine);
            }

            //Assets Downloaded
            if (e.InstalledAssets.Count > 0)
            {
                Summary.textBox1.AppendText("Assets Updated:(" + e.InstalledAssets.Count + ")");
                Summary.textBox1.AppendText(Environment.NewLine);
                Summary.textBox1.AppendText("-----------------------");
                Summary.textBox1.AppendText(Environment.NewLine);

                foreach (string asset in e.InstalledAssets)
                {
                    Summary.textBox1.AppendText(asset);
                    Summary.textBox1.AppendText(", ");
                }
                Summary.textBox1.AppendText(Environment.NewLine);
            }

            //Firmware Installed
            if (e.FirmwareUpdated != "")
            {
                Summary.textBox1.AppendText("-----------------------");
                Summary.textBox1.AppendText(Environment.NewLine);
                Summary.textBox1.AppendText("New Firmware was Downloaded(" + e.FirmwareUpdated + ") - Restart your Pocket to Install!");
                Summary.textBox1.AppendText(Environment.NewLine);
            }
        }

        public void Write_Log(string LogDir,string LogName,string LogSource)
        {
            string DateStamp = string.Format("**Update Run On " + "{0:MM-dd-yy}" + "**", DateTime.Now);
            string Log = LogDir + "\\" + LogName;

            if (File.Exists(Log))
            {
                File.AppendAllText(Log,Environment.NewLine + DateStamp + Environment.NewLine + LogSource);

            }
            else
            {
                File.WriteAllText(Log,Environment.NewLine + DateStamp + Environment.NewLine + LogSource);
            }
        }
        public void PopulateDrives()
        {
            try
            {
                comboBox1.Items.Clear();
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo d in allDrives.Where(x => x.DriveType == DriveType.Removable))
                {
                    if (d.IsReady == true)
                    {
                        string dl = d.VolumeLabel;
                        string dt = Convert.ToString(d.DriveType);

                        comboBox1.Items.Add(d.Name.Remove(3));
                    }
                    //comboBox1.SelectedIndex = 0;
                    //comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (comboBox1.Items.Count != 0)
                    {
                        comboBox1.SelectedIndex = 0;
                        comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                }
            }
            catch { MessageBox.Show("Error retrieving Drive Information", "Error!"); }

            if (comboBox1.SelectedIndex == -1)
            {
                Button_Removable.Enabled = false;
            }
            else
            {
                Button_Removable.Enabled = true;
            }
        }

        public void Button_Refresh_Click(object sender, EventArgs e)
        {
            PopulateDrives();

            if (comboBox1.SelectedIndex == -1)
            {
                Button_Removable.Enabled = false;
            }
            else
            {
                Button_Removable.Enabled = true;
            }
        }

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pocket_Drive = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);

            if (comboBox1.SelectedIndex == -1)
            {
                Button_Removable.Enabled = false;
            }
            else
            {
                Button_Removable.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String Location_Type = comboBox2.SelectedItem.ToString();
            if (Location_Type == "Removable Storage")
            {
                label2.Visible = true;
                comboBox1.Visible = true;
                Button_Refresh.Visible = true;

                if (comboBox1.SelectedIndex == -1)
                {
                    Button_Removable.Enabled = false;
                }
                else
                {
                    Button_Removable.Enabled = true;
                }
            }
            else
            {
                label2.Visible = false;
                comboBox1.Visible = false;
                Button_Refresh.Visible = false;
            }
            if (Location_Type == "Current Directory")
            {
                Button_Removable.Enabled = true;
            }
        }

        public void Get_Jsons(string Dir)
        {
            //Get List Json Files
            string[] Json_Files = (string[])General.GetFilesByExtension(Dir + "\\Assets", "*.json", SearchOption.AllDirectories);

            foreach (var file in Json_Files)
            {
                _ = new FileInfo(file);
                System.Diagnostics.Debug.WriteLine(file);
            }
        }
    }
}