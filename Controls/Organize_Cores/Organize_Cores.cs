using RetroDriven;
using System.Text.Json;
using pannella.analoguepocket;
using System.Net;
using System.ComponentModel;
using Pocket_Updater.Forms.Message_Box;
using System.Diagnostics;
//using Analogue;

namespace Pocket_Updater.Controls.Organize_Cores
{
    public partial class Organize_Cores : UserControl
    {
        private WebClient WebClient;
        public string Current_Dir { get; set; }
        public string updateFile { get; set; }

        private SettingsManager _settings;

        private Dictionary<string, CoreInfo> platforms;
        public Organize_Cores()
        {
            InitializeComponent();
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);

            //Get USB Drives
            PopulateDrives();
            //ReadPlatforms();
            //string Current_Dir = Directory.GetCurrentDirectory();

            //Preferences
            Get_Preferences_Json();

        }

        private void Save_Click(object sender, EventArgs e)
        {
            bool result = DataGrid_Checker();
            if (result == false)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "Core Names and Categories cannot be Blank!";
                form.Show();
            }
            else
            {
                //Enable Preserve Platform Folder
                try
                {
                    Config config = _settings.GetConfig();
                    config.preserve_platforms_folder = true;
                    _settings.UpdateConfig(config);
                    _settings.SaveSettings();
                    Save_Preferences_Json();

                    var Json_Dir = Path.Combine(Pocket_Drive.Text, "Platforms");
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string Row_Name = row.Cells[0].Value.ToString();
                        //System.Diagnostics.Debug.WriteLine(Row_Name);
                        string Row_Category = row.Cells[1].Value.ToString();
                        //System.Diagnostics.Debug.WriteLine(Row_Category);
                        string filename = row.Tag as String;
                        CoreInfo info = platforms[filename];

                        info.platform.name = Row_Name;
                        info.platform.category = Row_Category;

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string fullPath = Path.Combine(Json_Dir, filename);
                        File.WriteAllText(fullPath, JsonSerializer.Serialize(info, options));
                    }

                    Message_Box form = new Message_Box();
                    form.label1.Text = "Core Organization Saved!";
                    form.Show();
                }
                catch (IOException ioex)
                {
                    Message_Box form = new Message_Box();
                    form.label1.Text = ioex.Message;
                    form.Show();
                }
            }
        }

        private void Pocket_Drive_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get Json Data
            dataGridView1.Rows.Clear();
            ReadPlatforms(Pocket_Drive.Text);
        }
        public void ReadPlatforms(string Dir)
        {
            var Dir_Check = Dir + "Platforms";

            Debug.WriteLine(Dir_Check);

            if (Directory.Exists(Dir_Check))
            {

                platforms = new Dictionary<string, CoreInfo>();

                //Get Json Data
                var Json_Dir = Path.Combine(Dir, "Platforms");
                //System.Diagnostics.Debug.WriteLine(Json_Dir);
                string[] Json_Files = (string[])CoreInfo.GetJsonFiles(Json_Dir, "*.json", SearchOption.TopDirectoryOnly);
                foreach (var file in Json_Files)
                {
                    var data = File.ReadAllText(file);
                    //System.Diagnostics.Debug.WriteLine(data);
                    CoreInfo info = JsonSerializer.Deserialize<CoreInfo>(data);
                    var filename = Path.GetFileName(file);
                    platforms.Add(filename, info);
                    var name = info.platform.name;
                    var category = info.platform.category;
                    int index = dataGridView1.Rows.Add(name, category);
                    dataGridView1.Rows[index].Tag = filename;
                    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
                }
                Save.Enabled = true;
            }
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

                        //Get Json Data
                        //ReadPlatforms(Pocket_Drive.Text);
                    }
                }
            }
            catch {
                Message_Box form = new Message_Box();
                form.label1.Text = "Error retrieving Drive Information";
                form.Show();
            }

            if (Pocket_Drive.SelectedIndex == -1)
            {
                Save.Enabled = false;
                //coresList.Enabled = false;
            }
            else
            {
                Save.Enabled = true;
                //coresList.Enabled = true;
            }
        }

        public bool DataGrid_Checker()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var value = dataGridView1.Rows[i].Cells[0].Value;
                var value2 = dataGridView1.Rows[i].Cells[1].Value;

                if (value == null || value2 == null)
                {
                    return false;
                }
            }
            return true;
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            PopulateDrives();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            String Location_Type = comboBox2.SelectedItem.ToString();
            if (Location_Type == "Removable Storage")
            {
                dataGridView1.Rows.Clear();
                PopulateDrives();
                label3.Visible = true;
                Pocket_Drive.Visible = true;
                Button_Refresh.Visible = true;

                if (Pocket_Drive.SelectedIndex == -1)
                {
                  Save.Enabled = false;
                }
                else
                {
                    Save.Enabled = true;
                }
            }
            else
            {
                label3.Visible = false;
                Pocket_Drive.Visible = false;
                Button_Refresh.Visible = false;
            }
            if (Location_Type == "Current Directory")
            {
                //Get Json Data
                string Current = Directory.GetCurrentDirectory()+"\\";
                _settings = new SettingsManager(Current);
                dataGridView1.Rows.Clear();
                ReadPlatforms(Current);

                //Save.Enabled = true;
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
                comboBox2.Refresh();

                if (Update_Location == "Removable Storage")
                {
                    Pocket_Drive.SelectedIndex = Pocket_Drive.FindStringExact(Update_Drive);
                }
            }
        }

        private void Save_Preferences_Json()
        {
            string Json_File = @"updater_preferences.json";

            string Update_Location = comboBox2.SelectedItem.ToString();

            if ((Pocket_Drive.SelectedIndex == -1))
            {
                string[] Entries = new string[] { Update_Location, "" };
                Updater_Preferences.Save_Updater_Json(Entries, Json_File);
            }
            else
            {
                string Update_Drive = Pocket_Drive.SelectedItem.ToString();
                string[] Entries = new string[] { Update_Location, Update_Drive };
                Updater_Preferences.Save_Updater_Json(Entries, Json_File);
            }
        }
    }
}
