using Analogue;
using pannella.analoguepocket;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;
using System.Windows.Forms;

namespace Pocket_Updater
{
    public partial class Form1 : Form
    {
        private const string VERSION = "1.3.0";
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

        private void Button_Pocket_Click(object sender, EventArgs e)
        {
            Update_Pocket form = new Update_Pocket();
            form.Show();
        }

        private async void Button_Cores_Click(object sender, EventArgs e)
        {
            List<pannella.analoguepocket.Core> cores = await CoresService.GetCores();
            CoreSelector form = new CoreSelector(cores);
            form.Show();
        }

        private void Button_Settings_Click(object sender, EventArgs e)
        {
            Settings form = new Settings();
            form.Show();
        }

        private void updateLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Updater_Status form = new Updater_Status();

            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                form.textBox1.Text = File.ReadAllText(Current_Dir + "\\Pocket_Updater_Log.txt");
                form.Show();
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
                            DialogResult dialogResult = MessageBox.Show("There is a New Version Available!\n\n Would you like to Download it?", "App Update", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
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
                    MessageBox.Show("No Internet Connection Detected!","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}