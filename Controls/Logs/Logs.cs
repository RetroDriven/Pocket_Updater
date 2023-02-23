using Pocket_Updater.Forms.Message_Box;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pocket_Updater.Controls.Logs
{
    public partial class Logs : UserControl
    {
        public Logs()
        {
            InitializeComponent();

            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                textBox1.Text = File.ReadAllText(Current_Dir + "\\Pocket_Updater_Log.txt");
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
                textBox1.Refresh();
                textBox1.Select();
            }
            else
            {
                textBox1.Text = "No Log File Found!";
                Clear.Enabled= false;
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                try
                {
                    Clear.Enabled = false;
                    File.Delete(LogFile);
                    Message_Box form = new Message_Box();
                    form.label1.Text = "Log File Cleared!";
                    form.Show();
                    textBox1.Text = "No Log File Found!";
                }
                catch (Exception ex)
                {
                    Message_Box form = new Message_Box();
                    form.label1.Text = ex.Message;
                    form.Show();
                }
            }
            else
            {
                Message_Box form = new Message_Box();
                form.label1.Text = "No Log File Found!";
                form.Show();
            }
        }
    }
}
