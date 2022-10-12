using System.Net;
using pannella.analoguepocket;
using System.Data;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Numerics;
using System.Text.Json.Serialization;
using RetroDriven;
using System.Net.Http.Json;
using System.Threading;
//using Analogue;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace Pocket_Updater
{
    public partial class Organize_Cores : Form
    {

        private WebClient WebClient;
        public string Current_Dir { get; set; }
        public string updateFile { get; set; }

        private SettingsManager _settings;

        public Organize_Cores()
        {
            InitializeComponent();
            string Current_Dir = Directory.GetCurrentDirectory();
            _settings = new SettingsManager(Current_Dir);

            //Get USB Drives
            PopulateDrives();
            //string Current_Dir = Directory.GetCurrentDirectory();

            //Tooltips
            toolTip1.SetToolTip(Button_Refresh, "Refresh your Removable Drive List");

        }

        private void Save_Click(object sender, EventArgs e)
        {
            bool result = DataGrid_Checker();
            if (result == false)
            {
                MessageBox.Show("Core Names and Categories cannot be Blank!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    var Json_Dir = Pocket_Drive.Text + "Platforms";
                    //System.Diagnostics.Debug.WriteLine(Json_Dir);
                    string[] Json_Files = (string[])CoreInfo.GetJsonFiles(Json_Dir, "*.json", SearchOption.TopDirectoryOnly);
                    foreach (var file in Json_Files)
                    {
                        var data = File.ReadAllText(file);
                        CoreInfo info = JsonSerializer.Deserialize<CoreInfo>(data);
                        var name = info.platform.name;
                        var category = info.platform.category;

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {

                            string Row_Name = row.Cells[0].Value.ToString();
                            System.Diagnostics.Debug.WriteLine(Row_Name);
                            string Row_Category = row.Cells[1].Value.ToString();
                            System.Diagnostics.Debug.WriteLine(Row_Category);

                            info.platform.name = Row_Name;
                            info.platform.category = Row_Category;

                            var options = new JsonSerializerOptions { WriteIndented = true };
                            File.WriteAllText(file, JsonSerializer.Serialize(info, options));
                            
                        }
                    }

                    MessageBox.Show("Core Organization Saved!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException ioex)
                {
                    MessageBox.Show(ioex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                        var Json_Dir = Pocket_Drive.Text + "Platforms";
                        //System.Diagnostics.Debug.WriteLine(Json_Dir);
                        string[] Json_Files = (string[])CoreInfo.GetJsonFiles(Json_Dir, "*.json", SearchOption.TopDirectoryOnly);
                        foreach (var file in Json_Files)
                        {
                            var data = File.ReadAllText(file);
                            //System.Diagnostics.Debug.WriteLine(data);
                            CoreInfo info = JsonSerializer.Deserialize<CoreInfo>(data);
                            var name = info.platform.name;
                            var category = info.platform.category;
                            dataGridView1.Rows.Add(name,category);
                        }
                    }
                }
            }
            catch { MessageBox.Show("Error retrieving Drive Information", "Error!"); }

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

        private void Button_Refresh_Click_1(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            PopulateDrives();

            if (Pocket_Drive.SelectedIndex == -1 || dataGridView1.Rows.Count > 0)
            {
                Save.Enabled = false;
            }
            else
            {
                Save.Enabled = true;
            }
        }

        private void Pocket_Drive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Pocket_Drive.SelectedIndex == -1 || dataGridView1.Rows.Count > 0)
            {
                Save.Enabled = false;
            }
            else
            {
                Save.Enabled = true;
            }
        }
        public bool DataGrid_Checker()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var value = dataGridView1.Rows[i].Cells[0].Value.ToString();

                //if (string.IsNullOrWhiteSpace(value))
                if (string.IsNullOrEmpty(value))
                {
                   return false;
                }
            }
            return true;
        }
    }
}
