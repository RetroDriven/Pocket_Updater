using pannella.analoguepocket;
using Pocket_Updater.Forms.Message_Box;
using System.Data;
using System.Net;

namespace Pocket_Updater.Controls.Image_Packs
{
    public partial class Image_Packs : UserControl
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

            //Tooltips
            toolTip1.SetToolTip(Button_Refresh, "Refresh your Removable Drive List");

            GetPacks();

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
            catch {
                Message_Box form = new Message_Box();
                form.label1.Text = "Error retrieving Drive Information";
                form.Show();            
            }
        }

        public async Task GetPacks()
        {
            try
            {
                packs = await ImagePacksService.GetImagePacks();
                int i = 0;

                foreach (var pack in packs)
                {

                    var owner = pack.owner;
                    var variant = pack.variant;
                    var repo = "https://github.com/" + owner + "/" + pack.repository;

                    int index = dataGridView1.Rows.Add(owner, variant, repo, "Download");
                    i++;

                    //dataGridView1.Rows[index].Tag = owner;
                    //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                }
            }
            catch 
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "No Internet Connection Detected!";
                form.Show();
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

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //Open Github Links
            if (e.ColumnIndex == 2)
            {
                var row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells[2].Value == null) return;
                var url = row.Cells[2].Value.ToString();
                System.Diagnostics.Process.Start("explorer", url);
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
                Message_Box form = new Message_Box();
                form.label1.Text = "Please select Download Location!";
                form.Show();
            }
            else
            {

                //Make Sure Pocket Drive Letter is selected
                String Location_Type = comboBox2.SelectedItem.ToString();
                Pocket_Drive = this.comboBox1.GetItemText(this.comboBox1.SelectedItem);

                if (Location_Type == "Removable Storage" && comboBox1.SelectedIndex == -1)
                {
                    Message_Box form = new Message_Box();
                    form.label1.Text = "Please select your Pocket's Drive Letter!";
                    form.Show();
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
                                    Message_Box form = new Message_Box();
                                    form.label1.Text = "Asset Image Pack Installed!";
                                    form.Show();
                                }
                                else
                                {
                                    Message_Box form = new Message_Box();
                                    form.label1.Text = "Asset Image Pack did not Install. Please try Again!";
                                    form.Show();
                                }
                            }
                            else
                            {
                                Message_Box form = new Message_Box();
                                form.label1.Text = "The Drive Letter Was Not Found!";
                                form.Show();
                            }
                        }
                        else
                        {
                            //Current Directory Selected
                            if (await packs[e.RowIndex].Install(Current_Dir))
                            {
                                Message_Box form = new Message_Box();
                                form.label1.Text = "Asset Image Pack Installed!";
                                form.Show();
                            }
                            else
                            {
                                Message_Box form = new Message_Box();
                                form.label1.Text = "Asset Image Pack did not Install. Please try Again!";
                                form.Show();
                            }
                        }
                    }
                }
            }
        }
    }
}
