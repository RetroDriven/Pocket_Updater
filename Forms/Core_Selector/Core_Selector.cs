using System.Net;
using pannella.analoguepocket;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pocket_Updater
{
    public partial class CoreSelector : Form
    {
        private List<Core> _cores;

        private SettingsManager _settingsManager;
        private WebClient WebClient;
        // private PocketCoreUpdater _updater;
        public string Current_Dir { get; set; }
        public string updateFile { get; set; }
<<<<<<< Updated upstream

=======
        
        CheckBox headerCheckBox = new CheckBox();
        
>>>>>>> Stashed changes
        public CoreSelector(List<Core> cores)
        {
            InitializeComponent();

            //string pathToUpdate = Directory.GetCurrentDirectory();
            //_updater = new PocketCoreUpdater(pathToUpdate);

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
                //Download_Json();
            }

            _settingsManager = new SettingsManager(Directory.GetCurrentDirectory(), _cores);

            foreach (Core core in _cores)
            {
                if (_settingsManager.GetCoreSettings(core.identifier) != null)
                {
                    //Duplicate Core Name Handling
                    if (core.platform == "NES")
                    {
                        string NES = core.identifier.Replace(".NES", "");
                        core.platform = core.platform + " (" + NES + ")";
                    }
                    coresList.Items.Add(core, !_settingsManager.GetCoreSettings(core.identifier).skip);
                }
                else
                {
                    coresList.Items.Add(core, true);
                }
            }
        }

        private void CoreSelector_Load(object sender, EventArgs e)
        {
            /*
            if (coresList.CheckedIndices.Count > 1)
            {
                checkBox_All.Checked = false;
            }
            */
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
            for (int i = 0; i <= (coresList.Items.Count - 1); i++)
            {
                Core core = (Core)coresList.Items[i];
                if (!coresList.GetItemChecked(i))
                {
                    _settingsManager.DisableCore(core.identifier);
                }
                else
                {
                    _settingsManager.EnableCore(core.identifier);
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
                //_updater.CoresFile = updateFile; //here we set the location of the json file, for the updater to use
                WebClient.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkBox_All_CheckedChanged(object sender, EventArgs e)
        {
                if (checkBox_All.Checked)
            {
                int length = coresList.Items.Count;

<<<<<<< Updated upstream
                for (int le = 0; le < length; le++)
=======
        private void Select_All()
        {
            Point headerCellLocation = this.dataGridView1.GetCellDisplayRectangle(0, -1, true).Location;

            //Place the Header CheckBox in the Location of the Header Cell
            //headerCheckBox.Checked = true;
            headerCheckBox.Location = new Point(headerCellLocation.X + 67, headerCellLocation.Y + 9);
            headerCheckBox.BackColor = Color.White;
            headerCheckBox.Size = new Size(20, 20);
            headerCheckBox.Click += new EventHandler(HeaderCheckBox_Clicked);
            dataGridView1.Controls.Add(headerCheckBox);
            //Assign Click event to the DataGridView Cell.
            dataGridView1.CellContentClick += new DataGridViewCellEventHandler(DataGridView_CellClick);
        }
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Check to ensure that the row CheckBox is clicked.
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                //Loop to verify whether all row CheckBoxes are checked or not.
                bool isChecked = true;
                foreach (DataGridViewRow row in dataGridView1.Rows)
>>>>>>> Stashed changes
                {
                    coresList.SetSelected(le, true);
                    coresList.SetItemChecked(le, true);
                }
                checkBox_All.Checked = true;

            }
            else
            {
              
                int length = coresList.Items.Count;

                for (int le = 0; le < length; le++)
                {
                    coresList.SetSelected(le, false);
                    coresList.SetItemChecked(le, false);
                }
                
                checkBox_All.Checked = false;

            }
        }
    }
}
