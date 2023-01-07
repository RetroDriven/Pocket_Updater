using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Reflection;
using pannella.analoguepocket;
using RetroDriven;
using System.Text.Json;
using System.Diagnostics;
using static RetroDriven.General;
using Analogue;

namespace Pocket_Updater.Forms.Image_Packs
{
    public partial class Image_Packs : Form
    {
        public string Pocket_Drive { get; set; }
        public string Current_Dir { get; set; }

        private WebClient WebClient;

        private SettingsManager _settings;
        private ImagePack[] packs;

        public Image_Packs()
        {
            InitializeComponent();

            //Get USB Drives
            PopulateDrives();
            Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);

            GetPacks();

            //Grid Formatting
            dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

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
        }

        public async Task GetPacks()
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
                packs = await ImagePacksService.GetImagePacks();
                int i = 0;

                foreach (var pack in packs)
                {

                    var owner = pack.owner;
                    var variant = pack.variant;
                    var repo = "https://github.com/" + owner + "/" + pack.repository;

                    int index = dataGridView1.Rows.Add(owner,repo,variant,"Download");
                    i++;

                    //dataGridView1.Rows[index].Tag = owner;
                    //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                }
            }
        }
        private async void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            //Open Github Links
            if (e.ColumnIndex == 1)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells[0].Value == null) return;
                var url = row.Cells[1].Value.ToString();
                System.Diagnostics.Process.Start("explorer",url);
            }

            //Install Packs
            var senderGrid = (DataGridView)sender;

            //Preserve Platforms
            pannella.analoguepocket.Config config = _settings.GetConfig();
            config.preserve_platforms_folder = true;
            _settings.UpdateConfig(config);
            _settings.SaveSettings();

            //Make Sure Download Location is selected
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Download Location!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else 
            {

                //Make Sure Pocket Drive Letter is selected
                String Location_Type = comboBox2.SelectedItem.ToString();
                Pocket_Drive = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);

                if (Location_Type == "Removable Storage" && comboBox1.SelectedIndex == -1) {

                    MessageBox.Show("Please select your Pocket's Drive Letter!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                //Error Checking is done.....grab the pack
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
                    {
                        //Pocket Drive Selected
                        if (comboBox1.SelectedIndex != -1)
                        {

                            //Make Sure Drive Letter still exists
                            var drives = DriveInfo.GetDrives();
                            if (drives.Where(data => data.Name == Pocket_Drive).Count() == 1)
                            {
                                if (await packs[e.RowIndex].Install(Pocket_Drive))
                                {
                                    MessageBox.Show("Asset Image Pack Installed!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Asset Image Pack did not Install. Please try Again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("The Drive Letter Was Not Found!", "Drive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            //Current Directory Selected
                            if (await packs[e.RowIndex].Install(Current_Dir))
                            {
                                MessageBox.Show("Asset Image Pack Installed!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Asset Image Pack did not Install. Please try Again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
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
                    //Button_Removable.Enabled = false;
                }
                else
                {
                    //Button_Removable.Enabled = true;
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
                //Button_Removable.Enabled = true;
            }
        }
        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            PopulateDrives();
        }
    }
}
