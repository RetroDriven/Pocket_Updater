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
using System.ComponentModel;

namespace Pocket_Updater
{
    public partial class Organize_Cores : Form
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

            //Grid Formatting
            dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                //col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            //Get USB Drives
            PopulateDrives();
            //ReadPlatforms();
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
                    

                    MessageBox.Show("Core Organization Saved!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                catch (IOException ioex)
                {
                    MessageBox.Show(ioex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ReadPlatforms(string Dir)
        {
            var Dir_Check = Dir + "Platforms";

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

        private void Pocket_Drive_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get Json Data
            dataGridView1.Rows.Clear();
            ReadPlatforms(Pocket_Drive.Text);
        }
    }
}
