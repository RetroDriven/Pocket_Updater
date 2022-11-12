using System.Net;
using pannella.analoguepocket;
using System.Text.Json;
using System.ComponentModel;

namespace Pocket_Updater
{
    public partial class CoreSelector : Form
    {
        private List<Core> _cores;

        private SettingsManager _settingsManager;
        private WebClient WebClient;
        public string Current_Dir { get; set; }
        public string updateFile { get; set; }

        CheckBox headerCheckBox = new CheckBox();
        public CoreSelector(List<Core> cores)
        {
            InitializeComponent();
            Select_All();

            dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;

            bool result = CheckForInternetConnection();
            _cores = cores;

            if (result == false)
            {
                string message = "No Internet Connection Detected!";
                string title = "Error";
                Button_Save.Enabled = false;
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result2 = MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);
            }
            else
            {
                Button_Save.Enabled = true;
            }

            _settingsManager = new SettingsManager(Directory.GetCurrentDirectory(), _cores);

            foreach (Core core in _cores)
            {
                if (_settingsManager.GetCoreSettings(core.identifier) != null)
                {
                    string Identifier = core.identifier.Remove(core.identifier.LastIndexOf(".") + 1);
                    string Core_Author = Identifier.Substring(0, (Identifier.Length - 1));
 
                    //array containing the data for the 3 columns
                    object[] rows = { !_settingsManager.GetCoreSettings(core.identifier).skip, core.platform, Core_Author};
                    int index = dataGridView1.Rows.Add(rows);
                    
                    dataGridView1.Rows[index].Tag = core.identifier;
                    dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Button_Save.Enabled = false;
            _readChecklist();
            _settingsManager.SaveSettings();
            MessageBox.Show("Core Selection Has Been Saved!", "Cores Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Button_Save.Enabled = true;
            Close();
        }

        private void _readChecklist()
        {
            for (int i = 0; i <= (dataGridView1.Rows.Count - 1); i++)
            {
                string id = (string)dataGridView1.Rows[i].Tag;
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                if ((bool)cell.Value == false)
                {
                    _settingsManager.DisableCore(id);
                }
                else
                {
                    _settingsManager.EnableCore(id);
                }
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
            string Json_URL = "https://raw.githubusercontent.com/mattpannella/pocket_core_autoupdate_net/main/pocket_updater_cores.json";
            WebClient = new WebClient();
            string Current_Dir = Directory.GetCurrentDirectory();
            Console.WriteLine(Current_Dir);

            try
            {
                string updateFile = Current_Dir + "\\pocket_updater_cores.json";
                WebClient.DownloadFile(Json_URL, updateFile);
                string json = File.ReadAllText(updateFile);
                _cores = JsonSerializer.Deserialize<List<Core>>(json);
                WebClient.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Select_All()
        {
            Point headerCellLocation = this.dataGridView1.GetCellDisplayRectangle(0, -1, true).Location;

            headerCheckBox.Location = new Point(headerCellLocation.X + 77, headerCellLocation.Y + 9);
            headerCheckBox.BackColor = Color.White;
            headerCheckBox.Size = new Size(20, 20);
            headerCheckBox.Click += new EventHandler(HeaderCheckBox_Clicked);
            dataGridView1.Controls.Add(headerCheckBox);
        }
        private void HeaderCheckBox_Clicked(object sender, EventArgs e)
        {
            //Necessary to end the edit mode of the Cell.
            dataGridView1.EndEdit();

            //Loop and check and uncheck all row CheckBoxes based on Header Cell CheckBox.
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCheckBoxCell checkBox = (row.Cells[0] as DataGridViewCheckBoxCell);
                checkBox.Value = headerCheckBox.Checked;
            }
        }
    }
}
