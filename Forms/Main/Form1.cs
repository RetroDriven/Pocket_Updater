using Analogue;
using pannella.analoguepocket;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Pocket_Updater
{
    public partial class Form1 : Form
    { 
        private const string VERSION = "1.4";
        private const string API_URL = "https://api.github.com/repos/RetroDriven/Pocket_Updater/releases";
        private const string RELEASE_URL = "https://github.com/RetroDriven/Pocket_Updater/releases/latest";

        public Form1()
        {
            InitializeComponent();
            
            //Check for Internet Connection and App Updates
            try
            {
                using (WebClient client2 = new WebClient())
                {
                    using (client2.OpenRead("http://www.google.com/"))
                    {
                        _ = CheckVersion_Load();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to check for App Updates!", "No Internet Connection Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private async void Button_Cores_Click(object sender, EventArgs e)
        {

            try
            {
                using (WebClient client = new WebClient())
                {
                    using (client.OpenRead("http://www.google.com/"))
                    {
                        List<pannella.analoguepocket.Core> cores = await CoresService.GetCores();
                        CoreSelector form = new CoreSelector(cores);
                        form.Show();
                    }
                }
            }
            catch
            {
                MessageBox.Show("No Internet Connection Detected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void Button_Settings_Click(object sender, EventArgs e)
        {
            Settings form = new Settings();
            form.Show();
        }

        private void updateLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater_Summary form = new Updater_Summary();

            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                form.textBox1.Text = File.ReadAllText(Current_Dir + "\\Pocket_Updater_Log.txt");
                form.Show();
                form.textBox1.SelectionStart = form.textBox1.Text.Length;
                form.textBox1.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("Pocket Update Log Not Found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public async Task CheckVersion_Load()
        {

            if (await CheckVersion())
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        using (client.OpenRead("http://www.google.com/"))
                        {
                            DialogResult dialogResult = MessageBox.Show("There is a New Version Available!\n\n Would you like to Download it?", "App Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Process.Start("explorer", RELEASE_URL);
                                Close();

                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                //do something else
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("No Internet Connection Detected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        async static Task<bool> CheckVersion()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(API_URL)

                };
                var agent = new ProductInfoHeaderValue("Pocket-Updater", "1.0");
                request.Headers.UserAgent.Add(agent);
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                List<Github.Release>? releases = JsonSerializer.Deserialize<List<Github.Release>>(responseBody);

                string tag_name = releases[0].tag_name;
                string? v = SemverUtil.FindSemver(tag_name);
                if (v != null)
                {
                    return SemverUtil.SemverCompare(v, VERSION);

                }
                return false;
            }
            catch (HttpRequestException e)
            {
                return false;

            }
        }

        private async void checkForAppUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (WebClient client2 = new WebClient())
                {
                    using (client2.OpenRead("http://www.google.com/"))
                    {

                        if (await CheckVersion())
                        {
                            DialogResult dialogResult = MessageBox.Show("There is a New Version Available!\n\n Would you like to Download it?", "App Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Process.Start("explorer", RELEASE_URL);
                                Close();

                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                //do something else
                            }
                        }
                        else
                        {
                            MessageBox.Show("You are using the most recent App (v" + VERSION + ")", "No Updates Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to check for App Updates!", "No Internet Connection Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Update_Pocket form = new Update_Pocket();
            form.Show();
        }

        private async void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    using (client.OpenRead("http://www.google.com/"))
                    {
                        List<pannella.analoguepocket.Core> cores = await CoresService.GetCores();
                        CoreSelector form = new CoreSelector(cores);
                        form.Show();
                    }
                }
            }
            catch
            {
                MessageBox.Show("No Internet Connection Detected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings form = new Settings();
            form.Show();
        }

        private void viewLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater_Summary form = new Updater_Summary();

            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                form.textBox1.Text = File.ReadAllText(Current_Dir + "\\Pocket_Updater_Log.txt");
                form.Show();
                form.textBox1.SelectionStart = form.textBox1.Text.Length;
                form.textBox1.ScrollToCaret();
            }
            else
            {
                MessageBox.Show("No Log File Found!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                try
                {
                    File.Delete(LogFile);
                    MessageBox.Show("Log File Cleared!","",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,"",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No Log File Found!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button_Organize_Click(object sender, EventArgs e)
        {
            Organize_Cores form = new Organize_Cores();
            form.Show();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Organize_Cores form = new Organize_Cores();
            form.Show();
        }

        private void Button_Pocket_Click(object sender, EventArgs e)
        {
            Update_Pocket form = new Update_Pocket();
            form.Show();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
