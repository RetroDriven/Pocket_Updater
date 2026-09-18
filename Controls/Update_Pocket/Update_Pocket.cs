using System.Data;
using System.Net;
using Pannella.Services;
using RetroDriven;
using Pocket_Updater.Forms.Message_Box;
using Pocket_Updater.Forms.Updater_Summary;
using Pocket_Updater.Forms.Update_Progress;
using Pocket_Updater.UI;

using Newtonsoft.Json;
using Pannella.Helpers;
using Guna.UI2.WinForms;
using Pannella.Models.Settings;
using Microsoft.VisualBasic;
using Pocket_Updater.Properties;
using System.Runtime;
using System.Net.NetworkInformation;

namespace Pocket_Updater.Controls
{
    public partial class Update_Pocket : UserControl
    {
        public string Pocket_Drive { get; set; }
        public string Current_Dir { get; set; }

        private WebClient WebClient;
        private CoreUpdaterService _updater;
        SettingsService _settings;

        //Initialize Update Status Form Popup
        Updater_Summary Summary = new Updater_Summary();

        public Update_Pocket()
        {
            InitializeComponent();
            ApplyModernLayout();

            textBox1.Clear();

            //Get USB Drives
            PopulateDrives();

            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsService(Current_Dir);

            //Show/Hde Alternate Download
            if (Toggle_Alternate.Checked == true)
            {
                Alternate_Location.Visible = true;
                TextBox2.Visible = true;
            } else
            {
                Alternate_Location.Visible = false;
                TextBox2.Visible = false;
            }

            //Check for Internet
            if (Check_Internet())
            {
                setupUpdater(Current_Dir, Current_Dir);

                //Read Settings Json file
                ReadSettingsAsync();
            }

            Update.Enabled = false;

            //Preferences
            Get_Preferences_Json();
        }

        private void ApplyModernLayout()
        {
            UI.ModernTheme.Apply(this);

            AutoSize = false;
            BackColor = UI.ModernTheme.AppBackground;
            Padding = new Padding(18);

            Panel_Left.FillColor = UI.ModernTheme.AppBackground;
            Panel_Left.BackColor = UI.ModernTheme.AppBackground;
            Panel_Left.BorderThickness = 0;
            Panel_Left.Width = 520;
            Panel_Left.Padding = new Padding(0, 0, 14, 0);

            Panel_Left_Top.BackColor = UI.ModernTheme.Surface;
            Panel_Left_Top.Padding = new Padding(12, 10, 12, 8);
            Panel_Left_Top.Height = 154;

            Panel_Left_Main.BackColor = UI.ModernTheme.AppBackground;
            Panel_Left_Main.Padding = new Padding(0, 14, 0, 0);

            Panel_Left_Bottom.BackColor = UI.ModernTheme.Surface;
            Panel_Left_Bottom.Padding = new Padding(12, 8, 12, 12);

            Panel_Right.FillColor = UI.ModernTheme.Surface;
            Panel_Right.BackColor = UI.ModernTheme.Surface;
            Panel_Right.BorderColor = UI.ModernTheme.Border;
            Panel_Right.BorderThickness = 1;
            Panel_Right.BorderRadius = 14;
            Panel_Right.Padding = new Padding(12);

            Panel_Right_Top.FillColor = Color.Transparent;
            Panel_Right_Top.Height = 48;
            Panel_Right_Top.Padding = new Padding(4, 2, 4, 0);

            Panel_Title.FillColor = Color.Transparent;
            Panel_Title.BackColor = Color.Transparent;
            Panel_Title.Padding = new Padding(4, 0, 0, 0);
            Panel_Title.AutoSize = false;
            Panel_Title.Size = new Size(460, 42);

            Panel_Status_Title.FillColor = Color.Transparent;
            Panel_Status_Title.BackColor = Color.Transparent;
            Panel_Status_Title.Padding = new Padding(4, 0, 0, 0);

            guna2Panel1.FillColor = Color.Transparent;
            guna2Panel1.BackColor = Color.Transparent;
            guna2Panel1.AutoSize = false;
            guna2Panel1.Height = 42;
            guna2Panel1.Padding = new Padding(4, 0, 0, 0);

            label3.Font = UI.ModernTheme.TitleFont;
            label3.ForeColor = UI.ModernTheme.TextPrimary;
            label5.Font = UI.ModernTheme.SectionFont;
            label5.ForeColor = UI.ModernTheme.TextPrimary;
            label4.Font = UI.ModernTheme.TitleFont;
            label4.ForeColor = UI.ModernTheme.TextPrimary;

            guna2Separator1.FillColor = UI.ModernTheme.Border;
            guna2Separator2.FillColor = UI.ModernTheme.Border;
            guna2Separator3.FillColor = UI.ModernTheme.Border;

            tableLayoutPanel1.Padding = new Padding(4, 0, 0, 0);
            tableLayoutPanel1.Margin = new Padding(0, 4, 0, 2);
            tableLayoutPanel2.Padding = new Padding(4, 0, 0, 0);
            tableLayoutPanel2.Margin = new Padding(0, 2, 0, 0);
            label1.Font = UI.ModernTheme.BodyBoldFont;
            label2.Font = UI.ModernTheme.BodyBoldFont;
            label1.ForeColor = UI.ModernTheme.TextSecondary;
            label2.ForeColor = UI.ModernTheme.TextSecondary;

            comboBox2.AutoRoundedCorners = false;
            comboBox2.BorderRadius = 9;
            comboBox2.FillColor = UI.ModernTheme.SurfaceRaised;
            comboBox2.BorderColor = UI.ModernTheme.BorderStrong;
            comboBox2.Font = UI.ModernTheme.BodyFont;
            comboBox2.ForeColor = UI.ModernTheme.TextPrimary;

            comboBox1.AutoRoundedCorners = false;
            comboBox1.BorderRadius = 9;
            comboBox1.FillColor = UI.ModernTheme.SurfaceRaised;
            comboBox1.BorderColor = UI.ModernTheme.BorderStrong;
            comboBox1.Font = UI.ModernTheme.BodyFont;
            comboBox1.ForeColor = UI.ModernTheme.TextPrimary;

            Update.AutoRoundedCorners = false;
            Update.BorderRadius = 10;
            Update.FillColor = UI.ModernTheme.Accent;
            Update.HoverState.FillColor = UI.ModernTheme.AccentHover;
            Update.Font = UI.ModernTheme.BodyBoldFont;
            Update.Size = new Size(110, 38);

            Button_Refresh.AutoRoundedCorners = false;
            Button_Refresh.BorderRadius = 9;
            Button_Refresh.FillColor = UI.ModernTheme.SurfaceRaised;
            Button_Refresh.HoverState.FillColor = UI.ModernTheme.SurfaceHover;
            Button_Refresh.BorderColor = UI.ModernTheme.Border;
            Button_Refresh.BorderThickness = 1;
            Button_Refresh.Size = new Size(40, 36);

            flowLayoutPanel2.Padding = new Padding(0, 4, 0, 0);
            tableLayoutPanel3.Padding = new Padding(6, 0, 0, 0);
            tableLayoutPanel3.Margin = new Padding(0);
            foreach (Control settingControl in tableLayoutPanel3.Controls)
            {
                if (settingControl is Label settingLabel)
                {
                    settingLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
                    settingLabel.ForeColor = UI.ModernTheme.TextSecondary;
                    settingLabel.Margin = new Padding(0, 4, 10, 4);
                }
                else if (settingControl is Guna2ToggleSwitch settingToggle)
                {
                    settingToggle.Margin = new Padding(4, 3, 0, 3);
                    settingToggle.Size = new Size(42, 24);
                }
            }

            tableLayoutPanel4.Padding = new Padding(6, 4, 0, 0);
            tableLayoutPanel4.Margin = new Padding(0, 4, 0, 0);

            Alternate_Location.AutoRoundedCorners = false;
            Alternate_Location.BorderRadius = 9;
            Alternate_Location.FillColor = UI.ModernTheme.SurfaceRaised;
            Alternate_Location.BorderColor = UI.ModernTheme.BorderStrong;
            Alternate_Location.ForeColor = UI.ModernTheme.TextPrimary;
            Alternate_Location.Font = UI.ModernTheme.BodyFont;

            TextBox2.FillColor = Color.Transparent;
            TextBox2.BorderThickness = 0;
            TextBox2.ForeColor = UI.ModernTheme.TextMuted;
            TextBox2.Font = new Font("Segoe UI", 8.75F, FontStyle.Italic, GraphicsUnit.Point);

            Button_Save.AutoRoundedCorners = false;
            Button_Save.BorderRadius = 9;
            Button_Save.FillColor = UI.ModernTheme.Accent;
            Button_Save.HoverState.FillColor = UI.ModernTheme.AccentHover;
            Button_Save.Font = UI.ModernTheme.BodyBoldFont;

            textBox1.FillColor = Color.FromArgb(8, 13, 23);
            textBox1.BackColor = Color.FromArgb(8, 13, 23);
            textBox1.BorderColor = UI.ModernTheme.Border;
            textBox1.BorderThickness = 1;
            textBox1.BorderRadius = 10;
            textBox1.ForeColor = Color.FromArgb(210, 220, 236);
            textBox1.Font = UI.ModernTheme.MonoFont;
            textBox1.Margin = new Padding(0, 6, 0, 8);

            panel1.BackColor = UI.ModernTheme.Surface;
            panel1.Height = 32;
            panel1.Padding = new Padding(0, 8, 0, 0);
            guna2ProgressBar1.FillColor = UI.ModernTheme.SurfaceRaised;
            guna2ProgressBar1.ProgressColor = UI.ModernTheme.Accent;
            guna2ProgressBar1.ProgressColor2 = UI.ModernTheme.Accent;
            guna2ProgressBar1.BorderRadius = 4;

            ToolTip.BackColor = UI.ModernTheme.SurfaceRaised;
            ToolTip.BorderColor = UI.ModernTheme.BorderStrong;
            ToolTip.ForeColor = UI.ModernTheme.TextPrimary;
        }

        public async Task RunCoreUpdateProcess(string updatePath, string coresJsonPath, string LogDir)
        {
            Update.Enabled = false;
            Button_Refresh.Enabled = false;
            comboBox1.Enabled = false;

            await Task.Run(() =>
            {
                _updater.RunUpdates();
            });
            guna2ProgressBar1.ShowText = false;
            guna2ProgressBar1.Value = 0;
            guna2ProgressBar1.Update();

            Update.Enabled = true;
            Button_Refresh.Enabled = true;
            comboBox1.Enabled = true;

            Get_Jsons(updatePath);

            //Write to a Log File
            Write_Log(LogDir, "Pocket_Updater_Log.txt", Summary.textBox1.Text);

            //Close the Updater
            //Close();
        }

        private void updater_StatusUpdated(object sender, Pannella.Models.StatusUpdatedEventArgs e)
        {
            //Show Updater Status in a new Form
            BeginInvoke((Action)(() =>
            {
                textBox1.AppendText(e.Message);
                textBox1.AppendText(Environment.NewLine);
            }));
        }

        private async void Update_Click(object sender, EventArgs e)
        {
            if (!Check_Internet())
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "No Internet Connection Detected!";
                form.Show();
                return;
            }

            Button_Save.Enabled = false;
            Update.Enabled = false;
            Button_Refresh.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;

            try
            {
                Save_Settings("No");
                Current_Dir = Directory.GetCurrentDirectory();
                string locationType = comboBox2.SelectedItem?.ToString() ?? "Current Directory";
                string targetPath = Current_Dir;

                if (locationType == "Removable Storage")
                {
                    if (comboBox1.SelectedIndex < 0)
                    {
                        Message_Box form = new Message_Box();
                        form.label1.Text = "Please select your Pocket's drive letter.";
                        form.ShowDialog();
                        return;
                    }

                    targetPath = comboBox1.SelectedItem?.ToString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(targetPath) || !Directory.Exists(targetPath))
                    {
                        Message_Box form = new Message_Box();
                        form.label1.Text = "The selected Pocket drive was not found.";
                        form.ShowDialog();
                        PopulateDrives();
                        return;
                    }
                }

                Save_Preferences_Json();
                PocketTargetContext.SetSelectedPath(targetPath);

                using (var progress = new ModernUpdateProgressForm(targetPath))
                {
                    progress.ShowDialog(FindForm());
                }

                setupUpdater(targetPath, Current_Dir);
                Get_Jsons(targetPath);
            }
            catch (Exception ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = ex.Message;
                form.Show();
            }
            finally
            {
                Button_Save.Enabled = true;
                Update.Enabled = comboBox2.SelectedItem?.ToString() == "Current Directory" || comboBox1.SelectedIndex >= 0;
                Button_Refresh.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
            }

            await Task.CompletedTask;
        }

        private void updater_ProgressUpdated(object sender, DownloadProgressEventArgs e)
        {
            var percent = e.Progress * 100;
            int val = Convert.ToInt32(percent);

            guna2ProgressBar1.ShowText = true;

            BeginInvoke((Action)(() =>
            {

                    guna2ProgressBar1.Value = val;
                    guna2ProgressBar1.Update();
            }));

        }

        private async Task UpdateCurrentDirectory(string github_token, string currentDirectory)
        {
            try
            {
                //Download_Json(Current_Dir);
                setupUpdater(currentDirectory,currentDirectory);

                //Progress Bar
                HttpHelper.Instance.DownloadProgressUpdate += updater_ProgressUpdated;

                comboBox2.Enabled = false;
                Save_Preferences_Json();
                await RunCoreUpdateProcess(currentDirectory, currentDirectory, currentDirectory);
            }
            catch (Exception ex)
            {
                // TODO: log the exception
                Message_Box form = new Message_Box();
                form.label1.Text = ex.ToString();
                form.ShowDialog();
                Update.Enabled = true;
            }
        }

        private async Task UpdateRemoveableStorage(string github_token, string currentDirectory)
        {
            //string pathToUpdate = Pocket_Drive;
            string pathToUpdate = comboBox1.SelectedItem.ToString();
            comboBox2.SelectedIndex = comboBox2.FindStringExact("Removable Storage");

            try
            {
                //Make Sure Drive Letter still exists
                var drives = DriveInfo.GetDrives();
                if (drives.Where(data => data.Name == pathToUpdate).Count() == 1)
                {
                    setupUpdater(pathToUpdate, currentDirectory);

                    //Progress Bar
                    HttpHelper.Instance.DownloadProgressUpdate += updater_ProgressUpdated;

                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;

                    Save_Preferences_Json();
                    await RunCoreUpdateProcess(pathToUpdate, currentDirectory, currentDirectory);
                }
                else
                {
                    Message_Box form = new Message_Box();
                    form.label1.Text = "The Drive Letter Was Not Found!";
                    form.ShowDialog();
                    Button_Save.Enabled = true;

                    label2.Enabled = true;
                    comboBox1.Enabled = true;
                    Button_Refresh.Enabled = true;
                    Update.Enabled = true;
                    PopulateDrives();
                }
            }
            catch (Exception ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = ex.ToString();
                form.Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pocket_Drive = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);

            if (comboBox1.SelectedIndex == -1)
            {
                Update.Enabled = false;
            }
            else
            {
                Update.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String Location_Type = comboBox2.SelectedItem.ToString();
            if (Location_Type == "Removable Storage")
            {
                PopulateDrives();
                label2.Visible = true;
                comboBox1.Visible = true;
                Button_Refresh.Visible = true;

                if (comboBox1.SelectedIndex == -1)
                {
                    Update.Enabled = false;
                }
                else
                {
                    Update.Enabled = true;
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
                Update.Enabled = true;
            }
        }
        private void _updater_UpdateProcessComplete(object? sender, Pannella.Models.UpdateProcessCompleteEventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                //No Updates Found
                if (e.InstalledCores.Count == 0 && e.InstalledAssets.Count == 0 && string.IsNullOrEmpty(e.FirmwareUpdated) && e.MissingBetaKeys.Count == 0)
                {
                    //Summary.Close();
                    Message_Box form = new Message_Box();
                    form.label1.Text = "No Updates Found!";
                    form.Show();

                    //Status.Close();
                    Update.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    Button_Save.Enabled = true;
                }

                //Updates Found
                if (e.InstalledCores.Count > 0 || e.InstalledAssets.Count > 0 || !string.IsNullOrEmpty(e.FirmwareUpdated) || e.MissingBetaKeys.Count > 0)
                {
                    //Status.Close();
                    Summary = new Updater_Summary();
                    Summary.Show();
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    Button_Save.Enabled = true;
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
                        Summary.textBox1.AppendText(Path.GetFileName(asset));
                        Summary.textBox1.AppendText(", ");
                    }
                    Summary.textBox1.AppendText(Environment.NewLine);
                }

                //Skipped Assets
                if (e.SkippedAssets.Count > 0)
                {
                    Summary.textBox1.AppendText(Environment.NewLine);
                    Summary.textBox1.AppendText("Assets Skipped:(" + e.SkippedAssets.Count + ")");
                    Summary.textBox1.AppendText(Environment.NewLine);
                    Summary.textBox1.AppendText("-----------------------");
                    Summary.textBox1.AppendText(Environment.NewLine);

                    foreach (string asset in e.SkippedAssets)
                    {
                        Summary.textBox1.AppendText(Path.GetFileName(asset));
                        Summary.textBox1.AppendText(", ");
                    }
                    Summary.textBox1.AppendText(Environment.NewLine);
                }
                //JT Beta Key
                if (e.MissingBetaKeys.Count > 0)
                {
                    Summary.textBox1.AppendText(Environment.NewLine);
                    Summary.textBox1.AppendText("Missing/Incorrect JT Beta Key For:");
                    Summary.textBox1.AppendText(Environment.NewLine);
                    Summary.textBox1.AppendText("-----------------------");
                    Summary.textBox1.AppendText(Environment.NewLine);

                    foreach (string core in e.MissingBetaKeys)
                    {
                        Summary.textBox1.AppendText(core);
                        Summary.textBox1.AppendText(Environment.NewLine);
                    }
                    Summary.textBox1.AppendText(Environment.NewLine);
                }
                //Firmware Installed
                if (!string.IsNullOrEmpty(e.FirmwareUpdated))
                {
                    if (e.InstalledCores.Count != 0 || e.InstalledAssets.Count != 0 || e.MissingBetaKeys.Count != 0)
                    {
                        Summary.textBox1.AppendText("-----------------------");
                    }

                    Summary.textBox1.AppendText(Environment.NewLine);
                    //Summary.textBox1.AppendText("New Firmware was Downloaded(" + e.FirmwareUpdated + ") - Restart your Pocket to Install!");
                    Summary.textBox1.AppendText("New Firmware was Downloaded - Restart your Pocket to Install!");
                    //Summary.textBox1.AppendText(e.FirmwareUpdated);
                    Summary.textBox1.AppendText(Environment.NewLine);
                }
            }));
        }
        public void Write_Log(string LogDir, string LogName, string LogSource)
        {
            string DateStamp = string.Format("**Update Run On " + "{0:MM-dd-yy}" + "**", DateTime.Now);
            string Log = LogDir + "\\" + LogName;

            if (File.Exists(Log))
            {
                File.AppendAllText(Log, Environment.NewLine + DateStamp + Environment.NewLine + LogSource);

            }
            else
            {
                File.WriteAllText(Log, Environment.NewLine + DateStamp + Environment.NewLine + LogSource);
            }
        }
        public void SelectUpdateTarget(string targetPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
                return;

            string current = Path.GetFullPath(Directory.GetCurrentDirectory()).TrimEnd('\\');
            string target = Path.GetFullPath(targetPath).TrimEnd('\\');
            if (string.Equals(current, target, StringComparison.OrdinalIgnoreCase))
            {
                int currentIndex = comboBox2.FindStringExact("Current Directory");
                if (currentIndex >= 0) comboBox2.SelectedIndex = currentIndex;
                return;
            }

            int removableIndex = comboBox2.FindStringExact("Removable Storage");
            if (removableIndex >= 0) comboBox2.SelectedIndex = removableIndex;
            PopulateDrives();

            string root = Path.GetPathRoot(targetPath) ?? targetPath;
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                string candidate = comboBox1.Items[i]?.ToString() ?? string.Empty;
                if (string.Equals(Path.GetFullPath(candidate).TrimEnd('\\'), Path.GetFullPath(root).TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
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
            catch
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "Error retrieving Drive Information";
                form.Show();
            }

            if (comboBox1.SelectedIndex == -1)
            {
                Update.Enabled = false;
            }
            else
            {
                Update.Enabled = true;
            }
        }
        public void Get_Jsons(string Dir)
        {
            string assetsDirectory = Path.Combine(Dir, "Assets");
            if (!Directory.Exists(assetsDirectory))
            {
                return;
            }

            //Get List Json Files
            string[] Json_Files = (string[])General.GetFilesByExtension(assetsDirectory, "*.json", SearchOption.AllDirectories);

            foreach (var file in Json_Files)
            {
                _ = new FileInfo(file);
                System.Diagnostics.Debug.WriteLine(file);
            }
        }
        private void Settings_Load(object sender, EventArgs e)
        {

        }
        public async Task ReadSettingsAsync()
        {
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsService(Current_Dir);

            //Alternate Arcade Files
            if (ServiceHelper.SettingsService.GetConfig().skip_alternative_assets == true)
            {
                Toggle_Alternatives.Checked = true;
            }
            else
            {
                Toggle_Alternatives.Checked = false;
            }
            //GitHub Token
            //Alternate_Location.Text = ServiceHelper.SettingsService.GetConfig().use_custom_archive;

            if (ServiceHelper.SettingsService.GetConfig().use_custom_archive == true)
            {
                Toggle_Alternate.Checked = true;

                var url = ServiceHelper.SettingsService.GetConfig().archives[1].url;
                var index = ServiceHelper.SettingsService.GetConfig().archives[1].index;
                Alternate_Location.Text = url;
                //CRC_Json.Text = index;

            }
            else
            {
                Toggle_Alternate.Checked = false;
            }
            //Download Pocket Firmware
            if (ServiceHelper.SettingsService.GetConfig().download_firmware == true)
            {
                Toggle_Firmware.Checked = true;
            }
            else
            {
                Toggle_Firmware.Checked = false;
            }
            //Download Assets
            if (ServiceHelper.SettingsService.GetConfig().download_assets == true)
            {
                Toggle_Assets.Checked = true;
            }
            else
            {
                Toggle_Assets.Checked = false;
            }
            //Preserve Core Images
            if (ServiceHelper.SettingsService.GetConfig().preserve_platforms_folder == true)
            {
                Toggle_Platforms.Checked = true;
            }
            else
            {
                Toggle_Platforms.Checked = false;
            }
            //Delete Skipped Cores
            if (ServiceHelper.SettingsService.GetConfig().delete_skipped_cores == true)
            {
                Toggle_Skipped.Checked = true;
            }
            else
            {
                Toggle_Skipped.Checked = false;
            }
            //Build Jsons
            if (ServiceHelper.SettingsService.GetConfig().build_instance_jsons == true)
            {
                Toggle_Jsons.Checked = true;
            }
            else
            {
                Toggle_Jsons.Checked = false;
            }
            //Jotego Core Rename
            if (ServiceHelper.SettingsService.GetConfig().fix_jt_names == true)
            {
                Toggle_Jotego.Checked = true;
            }
            else
            {
                Toggle_Jotego.Checked = false;
            }
            //CRC Checking
            if (ServiceHelper.SettingsService.GetConfig().crc_check == true)
            {
                Toggle_CRC.Checked = true;
            }
            else
            {
                Toggle_CRC.Checked = false;
            }
            //Backup Saves
            if (ServiceHelper.SettingsService.GetConfig().backup_saves == true)
            {
                Toggle_Backup_Saves.Checked = true;
            }
            else
            {
                Toggle_Backup_Saves.Checked = false;
            }
        }
        private async void Button_Save_Click(object sender, EventArgs e)
        {
            Save_Settings("Yes");
        }

        private async void Save_Settings(string ShowBox)
        {
            if (!Check_Internet())
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "No Internet Connection Detected!";
                form.Show();
            }
            else
            {

                //string value = Alternate_Location.Text;
                Pannella.Models.Settings.Config config = ServiceHelper.SettingsService.GetConfig();

                //GitHub Token
                //config.github_token = value;

                //Alternate Arcade Files
                if (Toggle_Alternatives.Checked == true)
                {
                    config.skip_alternative_assets = true;
                }
                else
                {
                    config.skip_alternative_assets = false;
                }

                //Alternate Download Location
                if (Toggle_Alternate.Checked == true)
                {
                    config.use_custom_archive = true;

                    ServiceHelper.SettingsService.GetConfig().archives[1].url = Alternate_Location.Text;
                }
                else
                {
                    config.use_custom_archive = false;
                }

                //Download Pocket Firmware
                if (Toggle_Firmware.Checked == true)
                {
                    config.download_firmware = true;
                }
                else
                {
                    config.download_firmware = false;
                }
                //Download Assets
                if (Toggle_Assets.Checked == true)
                {
                    config.download_assets = true;
                }
                else
                {
                    config.download_assets = false;
                }
                //Preserve Core Images
                if (Toggle_Platforms.Checked == true)
                {
                    config.preserve_platforms_folder = true;
                }
                else
                {
                    config.preserve_platforms_folder = false;
                }
                //Delete Skipped Cores
                if (Toggle_Skipped.Checked == true)
                {
                    config.delete_skipped_cores = true;
                }
                else
                {
                    config.delete_skipped_cores = false;
                }
                //Build Jsons
                if (Toggle_Jsons.Checked == true)
                {
                    config.build_instance_jsons = true;
                }
                else
                {
                    config.build_instance_jsons = false;
                }
                //Jotego Rename
                if (Toggle_Jotego.Checked == true)
                {
                    config.fix_jt_names = true;
                }
                else
                {
                    config.fix_jt_names = false;
                }
                //CRC Check
                if (Toggle_CRC.Checked == true)
                {
                    config.crc_check = true;
                }
                else
                {
                    config.crc_check = false;
                }
                //Backup Saves
                if (Toggle_Backup_Saves.Checked == true)
                {
                    config.backup_saves = true;
                }
                else
                {
                    config.backup_saves = false;
                }

                //Save Sttings
                SettingsService _settings;
                string Current_Dir = Directory.GetCurrentDirectory();
                _settings = new SettingsService(Current_Dir);

                _settings.UpdateConfig(config);
                _settings.Save();
                //ServiceHelper.SettingsService.UpdateConfig(config);
                //ServiceHelper.SettingsService.Save();

                //Show Message Box
                if (ShowBox == "Yes")
                {

                    Message_Box form = new Message_Box();
                    form.label1.Text = "Settings Saved!";
                    form.Show();
                }
            }
        }

        private void Button_Refresh_Click_1(object sender, EventArgs e)
        {
            PopulateDrives();

            if (comboBox1.SelectedIndex == -1)
            {
                Update.Enabled = false;
            }
            else
            {
                Update.Enabled = true;
            }
        }

        private void Toggle_Alternate_CheckedChanged(object sender, EventArgs e)
        {
            if (Toggle_Alternate.Checked == true)
            {
                Alternate_Location.Visible = true;
                TextBox2.Visible = true;
                //CRC_Json.Visible = true;
                //label15.Visible = true;
                //label17.Visible = true;
                //label18.Visible = true;
            }
            if (Toggle_Alternate.Checked == false)
            {
                Alternate_Location.Visible = false;
                TextBox2.Visible = false;
                //CRC_Json.Visible = false;
                //label15.Visible = false;
                //label17.Visible = false;
                //label18.Visible = false;
            }
        }
        private void Get_Preferences_Json()
        {
            string Json_File = @"updater_preferences.json";

            if (File.Exists(Json_File) == true)
            {
                string Update_Location = Updater_Preferences.Get_Updater_Json("Update Location", Json_File);
                string Update_Drive = Updater_Preferences.Get_Updater_Json("Update Drive Letter", Json_File);

                comboBox2.SelectedIndex = comboBox2.FindStringExact(Update_Location);

                if (Update_Location == "Removable Storage")
                {
                    comboBox1.SelectedIndex = comboBox1.FindStringExact(Update_Drive);
                }
            }
        }

        private void Save_Preferences_Json()
        {
            string Json_File = @"updater_preferences.json";

            string Update_Location = comboBox2.SelectedItem.ToString();

            if ((comboBox1.SelectedIndex == -1))
            {
                string[] Entries = new string[] { Update_Location, "" };
                Updater_Preferences.Save_Updater_Json(Entries, Json_File);
            }
            else
            {
                string Update_Drive = comboBox1.SelectedItem.ToString();
                string[] Entries = new string[] { Update_Location, Update_Drive };
                Updater_Preferences.Save_Updater_Json(Entries, Json_File);
            }
        }
        private void setupUpdater(string path, string config_path)
        {
            try
            {
                ServiceHelper.Initialize(path, config_path, updater_StatusUpdated, _updater_UpdateProcessComplete, true);
                _updater = new CoreUpdaterService(
                    ServiceHelper.UpdateDirectory,
                    ServiceHelper.CoresService.Cores,
                    ServiceHelper.FirmwareService,
                    ServiceHelper.SettingsService,
                    ServiceHelper.CoresService
                );

                _updater.StatusUpdated += updater_StatusUpdated;
                _updater.UpdateProcessComplete += _updater_UpdateProcessComplete;
            }
            catch(Exception ex)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = ex.ToString();
                form.Show();
            }
        }
        public static bool Check_Internet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
