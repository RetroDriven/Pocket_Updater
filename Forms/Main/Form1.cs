using pannella.analoguepocket;
using Pocket_Updater.Forms.Message_Box;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Pocket_Updater
{
    public partial class Form1 : Form
    { 
        private const string VERSION = "1.5.5";
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
                    _ = CheckVersion_Load();
                }
            }
            catch
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "Failed to check for App Updates!";
                form.Show();
            }

        }
        public async Task CheckVersion_Load()
        {
            using (WebClient client = new WebClient())
            {
                if (await CheckVersion())
                {
                    try
                    {
                        Update_Available.Visible = true;
                    }
                    catch
                    {
                        No_Internet.Visible = true;
                    }
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
                    //return SemverUtil.SemverCompare(v, "1.0");

                }
                return false;
            }
            catch (HttpRequestException e)
            {
                return false;
                
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
        private void Update_Pocket_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            update_Pocket1.Visible= true;

        }
        private void Manage_Cores_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            manageCores1.Visible = true;
            manageCores1.Enabled = true;
        }

        private void Hide_Controls()
        {
            update_Pocket1.Visible = false;
            manageCores1.Visible = false;
            organize_Cores1.Visible = false;
            image_Packs1.Visible = false;
            logs1.Visible= false;
            about1.Visible = false;
        }

        private void Organize_Cores_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            organize_Cores1.Visible = true;
        }

        private void Image_Packs_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            image_Packs1.Visible = true;
        }
        private void Logs_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                logs1.textBox1.Text = File.ReadAllText(Current_Dir + "\\Pocket_Updater_Log.txt");
                logs1.textBox1.SelectionStart = logs1.textBox1.Text.Length;
                logs1.textBox1.ScrollToCaret();
                logs1.textBox1.Refresh();
                logs1.textBox1.Select();
            }
            else
            {
                logs1.textBox1.Text = "No Log File Found!";
                logs1.Clear.Enabled = false;
            }
            logs1.Visible = true;
        }

        private void Update_Available_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", RELEASE_URL);
        }

        private void About_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            about1.Visible= true;
        }
    }
}
