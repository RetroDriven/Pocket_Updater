using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RetroDriven;

namespace Pocket_Updater.Forms.Saves
{
    public partial class Saves : Form
    {
        public Saves()
        {
            InitializeComponent();
            PopulateDrives();

            //Tooltips
            toolTip1.SetToolTip(Button_Refresh, "Refresh your Removable Drive List");
        }

        private void Button_Sync_Click(object sender, EventArgs e)
        {
            /*

             //Get List of Local/NAS Save Files
             string[] LocalSaves = (string[])General.GetFilesByExtension(textBox1.Text, "*.sav", SearchOption.AllDirectories);

             foreach (var file in LocalSaves)
             {
                 _ = new FileInfo(file);
                 //System.Diagnostics.Debug.WriteLine(file);
             }

             //Get List of Pocket Save Files
             string[] PocketSaves = (string[])General.GetFilesByExtension(Pocket_Drive.Text, "*.sav", SearchOption.AllDirectories);

             foreach (var filePocket in PocketSaves)
             {
                 _ = new FileInfo(filePocket);
                 //System.Diagnostics.Debug.WriteLine(filePocket);
             }

             //Backup Saves
             if (Backup_Saves.Checked == true)
             {
                 //Backup Local/NAS Save Files
                 string Backup_Dir = textBox1.Text + "\\Saves_Backup\\";
                 General.CreateDir(Backup_Dir);
                 string LocalBackup = Backup_Dir + string.Format("Saves-{0:MM-dd-yyyy_hh-mm-ss}.zip", DateTime.Now);
                 General.CreateZipFile(LocalBackup, LocalSaves);

                 //Backup Pocket Save Files
                 string PocketBackup_Dir = Pocket_Drive.Text + "Saves_Backup\\";
                 General.CreateDir(PocketBackup_Dir);
                 string PocketBackup = PocketBackup_Dir + string.Format("Saves-{0:MM-dd-yyyy_hh-mm-ss}.zip", DateTime.Now);
                 General.CreateZipFile(PocketBackup, PocketSaves);
             }
            */

            RD_Saves.Sync(textBox1.Text, Pocket_Drive.Text + "\\Saves");
            MessageBox.Show("Saves Synced!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void PopulateDrives()
        {
            try
            {
                Pocket_Drive.Items.Clear();
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo d in allDrives.Where(x => x.DriveType == DriveType.Removable))
                {
                    if (d.IsReady == true)
                    {
                        string dl = d.VolumeLabel;
                        string dt = Convert.ToString(d.DriveType);

                        Pocket_Drive.Items.Add(d.Name.Remove(3));
                    }
                    //comboBox1.SelectedIndex = 0;
                    //comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (Pocket_Drive.Items.Count != 0)
                    {
                        Pocket_Drive.SelectedIndex = 0;
                        Pocket_Drive.DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                }
            }
            catch { MessageBox.Show("Error retrieving Drive Information", "Error!"); }
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            PopulateDrives();
        }

        private void Button_Browse_Click(object sender, EventArgs e)
        {
            DialogResult Local_Path = folderBrowserDialog1.ShowDialog();
            if (Local_Path == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();

                if (textBox1.Text != "" && Pocket_Drive.SelectedIndex > -1)
                {
                    Button_Sync.Enabled = true;
                }
                else
                {
                    Button_Sync.Enabled = false;
                }
            }
        }

        private void Pocket_Drive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && Pocket_Drive.SelectedIndex > -1)
            {
                Button_Sync.Enabled = true;
            }
            else
            {
                Button_Sync.Enabled = false;
            }
        }
    }
}
