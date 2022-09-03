using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pannella.analoguepocket;
using System.IO;

namespace Pocket_Updater
{
    public partial class CoreSelector : Form
    {
        private List<Core> _cores;
        private CoreManager _coreManager;
        private WebClient WebClient;
        private PocketCoreUpdater _updater;
        public string Current_Dir { get; set; }
        public string updateFile { get; set; }

        public CoreSelector()
        {
            InitializeComponent();

            string pathToUpdate = Directory.GetCurrentDirectory();
            _updater = new PocketCoreUpdater(pathToUpdate);

            bool result = CheckForInternetConnection();

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
                Download_Json();
            }


            _coreManager = new CoreManager(Directory.GetCurrentDirectory() + "\\auto_update.json");
            _cores = _coreManager.GetCores();

            foreach (Core core in _cores)
            {
                //coresList.Items.Add
                coresList.Items.Add(core, !core.skip);
            }
        }

        private void CoreSelector_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button_Save.Enabled = false;
            _readChecklist();
            _coreManager.SaveCores(_cores);
            MessageBox.Show("Core Selection Has Been Saved!", "Cores Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Button_Save.Enabled=true;
        }

        private void _readChecklist()
        {
            List<Core> newList = new List<Core>();
            for (int i = 0; i <= (coresList.Items.Count - 1); i++)
            {
                Core core = (Core)coresList.Items[i];
                if (!coresList.GetItemChecked(i))
                {
                    core.skip = true;
                }
                newList.Add(core);
            }
            _cores = newList;
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
            string Json_URL = "https://raw.githubusercontent.com/mattpannella/pocket_core_autoupdate_net/main/auto_update.json";
            WebClient = new WebClient();
            string Current_Dir = Directory.GetCurrentDirectory();
            Console.WriteLine(Current_Dir);

            try
            {
                string updateFile = Current_Dir + "\\auto_update.json";
                WebClient.DownloadFile(Json_URL, updateFile);
                //_updater.CoresFile = updateFile; //here we set the location of the json file, for the updater to use
                WebClient.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
