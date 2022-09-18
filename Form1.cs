using System.Windows.Forms;

namespace Pocket_Updater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void Button_Cores_Click(object sender, EventArgs e)
        {
            CoreSelector form = new CoreSelector();
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
    }
}