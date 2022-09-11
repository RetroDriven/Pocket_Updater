using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pannella.analoguepocket;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pocket_Updater
{
    public partial class Update_Pocket : Form
    {
        public string Pocket_Drive { get; set; }
        public string Current_Dir { get; set; }

        private WebClient WebClient;
        private PocketCoreUpdater _updater;
        private ArcadeRomDownloader _romDownloader;

        public Update_Pocket()
        {
            InitializeComponent();

            //Get USB Drives
            PopulateDrives();

            //pass in the directory where we want to download the cores to. (like the root of your sd card, for example)
            //will just use the current directory for now as an example, just like the json downloader is
            //the updater also needs to be given the path to the json file, so i am setting that in the download method

            //string pathToUpdate = Directory.GetCurrentDirectory();
            //_updater = new PocketCoreUpdater(pathToUpdate);
            //_updater.InstallBiosFiles(true); //turns on the option to also download bios files

            //this sets up an event listener, for the 'StatusUpdated' event i create every time i want to send an update message
            //stuff like 'downloading file for core x' or 'finished updating core y' etc
            //_updater.StatusUpdated += updater_StatusUpdated;
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
            string Json_URL = "https://raw.githubusercontent.com/mattpannella/pocket_core_autoupdate_net/main/auto_update.json";
            WebClient = new WebClient();
            //string Current_Dir = Directory.GetCurrentDirectory();

            try
            {
                string updateFile = Drive + "\\auto_update.json";
                Console.WriteLine(updateFile);
                WebClient.DownloadFile(new Uri(Json_URL), updateFile);
                _updater.CoresFile = updateFile; //here we set the location of the json file, for the updater to use
                _romDownloader = new ArcadeRomDownloader(updateFile);
                Button_Removable.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /* ------ STUFF MATT ADDED FOR EXAMPLE ------ */

        //Call this method from a new button and you can run the arcade rom downloder
        public void RunArcadeRomDownloadProcess()
        {
            _romDownloader.DownloadAll();
        }

        //Call this method from a new button and you can run the core updater
        public async Task RunCoreUpdateProcess(string updatePath, string coresJsonPath)
        {
            Button_Removable.Enabled = false;
            Button_Refresh.Enabled = false;
            comboBox1.Enabled = false;
            await _updater.RunUpdates();
            Button_Removable.Enabled = true;
            Button_Refresh.Enabled = true;
            comboBox1.Enabled = true;
            MessageBox.Show("Cores have been Checked/Updated Successfully", "Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void updater_StatusUpdated(object sender, StatusUpdatedEventArgs e)
        {
            //i would not pop up a messagebox here, but I just put it as an example. so if you uncommented that line
            //it would pop up a messagebox every time my updater class sent a status message update
            //you could instead maybe put some kind of text box on your form, and then update it with e.Message in this method
            //to display the updates on the ui
            //MessageBox.Show(e.Message);

            infoTextBox.AppendText(e.Message);
            infoTextBox.AppendText(Environment.NewLine);
        }

        private void updateCoresButton_Click(object sender, EventArgs e)
        {
            //Clear the info box
            infoTextBox.Clear();

            string Location_Type = comboBox2.SelectedItem.ToString();

            //Current Drive Updater
            if (Location_Type == "Current Directory")
            {
                string Current_Dir = Directory.GetCurrentDirectory();

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
                    
                    _updater = new PocketCoreUpdater(Current_Dir);
                    _updater.CoresFile = Current_Dir;
                    _updater.InstallBiosFiles(true); //turns on the option to also download bios files

                    _updater.StatusUpdated += updater_StatusUpdated;

                    Download_Json(Current_Dir);

                    //_updater = new PocketCoreUpdater(Current_Dir);
                    RunCoreUpdateProcess(Current_Dir, Current_Dir);
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
                        Download_Json(pathToUpdate);
                        _updater = new PocketCoreUpdater(pathToUpdate);
                        RunCoreUpdateProcess(Pocket_Drive, Pocket_Drive);
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
                    comboBox1.SelectedIndex = 0;
                    comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
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
    }
}