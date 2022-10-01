using System.Net;
using pannella.analoguepocket;
using System.Text.Json;
using System.Windows.Forms;

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
    }
}
