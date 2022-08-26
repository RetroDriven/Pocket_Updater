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
    public partial class Updater_Pocket : Form
    {
        public string Pocket_Drive { get; set; }

        private WebClient WebClient;
        private PocketCoreUpdater _updater;
        private ArcadeRomDownloader _romDownloader;

        public Updater_Pocket()
        {
            InitializeComponent();

            //Get USB Drives
            PopulateDrives();


            //pass in the directory where we want to download the cores to. (like the root of your sd card, for example)
            //will just use the current directory for now as an example, just like the json downloader is
            //the updater also needs to be given the path to the json file, so i am setting that in the download method
            string pathToUpdate = Pocket_Drive;
            _updater = new PocketCoreUpdater(pathToUpdate);
            _updater.InstallBiosFiles(true); //turns on the option to also download bios files

            //this sets up an event listener, for the 'StatusUpdated' event i create every time i want to send an update message
            //stuff like 'downloading file for core x' or 'finished updating core y' etc
            _updater.StatusUpdated += updater_StatusUpdated;

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
                Download_Json();
            }
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

        public void Download_Json()
        {
            string Json_URL = "https://raw.githubusercontent.com/mattpannella/pocket_core_autoupdate_net/main/auto_update.json";
            WebClient = new WebClient();
            //string Current_Dir = Directory.GetCurrentDirectory();

            try
            {
                string updateFile = Pocket_Drive + "\\auto_update.json";
                WebClient.DownloadFileAsync(new Uri(Json_URL), updateFile);
                _updater.CoresFile = updateFile; //here we set the location of the json file, for the updater to use
                _romDownloader = new ArcadeRomDownloader(updateFile);
                updateCoresButton.Enabled = true;
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
            updateCoresButton.Enabled = false;
            Button_Refresh.Enabled = false;
            comboBox1.Enabled = false;
            await _updater.RunUpdates();
            updateCoresButton.Enabled = true;
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

            //Run the Updater
            //_updater.RunUpdates();
            RunCoreUpdateProcess(Pocket_Drive, Pocket_Drive);

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
        }

        public void Button_Refresh_Click(object sender, EventArgs e)
        {
            PopulateDrives();
        }

        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pocket_Drive = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);
    }
    }
}