using System.Net;
using pannella.analoguepocket;
using System.Text.Json;
using System.Windows.Forms;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Linq;

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

        public CoreSelector(List<Core> cores)
        {
            InitializeComponent();
            dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
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
                    string Identifier = core.identifier.Remove(core.identifier.LastIndexOf(".") + 1);
                    string Core_Author = Identifier.Substring(0, (Identifier.Length - 1));
 
                    //array containing the data for the 3 columns
                    object[] rows = {core.platform, Core_Author, !_settingsManager.GetCoreSettings(core.identifier).skip };
                    int index = dataGridView1.Rows.Add(rows);
                    
                    dataGridView1.Rows[index].Tag = core.identifier;
                    dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

                    /*
                    //Duplicate Core Name Handling
                    if (core.platform == "NES")
                    {
                        string NES = core.identifier.Replace(".NES", "");
                        core.platform = core.platform + " (" + NES + ")";
                    }
                    if (core.platform == "Genesis")
                    {
                        string Genesis = core.identifier.Replace(".Genesis", "");
                        core.platform = core.platform + " (" + Genesis + ")";
                    }
                    if (core.platform == "Supervision")
                    {
                        string Supervision = core.identifier.Replace(".Supervision", "");
                        core.platform = core.platform + " (" + Supervision + ")";
                    }
                    */
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
            for (int i = 0; i <= (dataGridView1.Rows.Count - 1); i++)
            {
                //Core core = (Core)coresList.Items[i];
                string id = (string)dataGridView1.Rows[i].Tag;
                //if (!coresList.GetItemChecked(i))
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[2];
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

                for (int le = 0; le < length; le++)
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //_cores
           // foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
             //   DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[2];
               // chk.Value = !(chk.Value == null ? false : (bool)chk.Value); //because chk.Value is initialy null
           // }
        }
    }
}
