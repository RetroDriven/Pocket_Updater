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
            ApplyModernLayout();

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

        private void ApplyModernLayout()
        {
            UI.ModernTheme.Apply(this);
            UI.ModernTheme.ApplyPageChrome(this);

            Panel_Top.FillColor = Color.Transparent;
            label4.Font = UI.ModernTheme.TitleFont;
            label4.ForeColor = UI.ModernTheme.TextPrimary;
            guna2Separator2.FillColor = UI.ModernTheme.Border;

            panel2.BackColor = UI.ModernTheme.Surface;
            panel2.Padding = new Padding(1);
            textBox1.FillColor = Color.FromArgb(8, 13, 23);
            textBox1.BackColor = Color.FromArgb(8, 13, 23);
            textBox1.BorderColor = UI.ModernTheme.Border;
            textBox1.BorderThickness = 1;
            textBox1.BorderRadius = 10;
            textBox1.ForeColor = Color.FromArgb(210, 220, 236);
            textBox1.Font = UI.ModernTheme.MonoFont;

            panel1.BackColor = UI.ModernTheme.Surface;
            panel1.Height = 66;
            panel1.Padding = new Padding(0, 12, 0, 12);

            Clear.AutoRoundedCorners = false;
            Clear.BorderRadius = 9;
            Clear.FillColor = UI.ModernTheme.SurfaceRaised;
            Clear.BorderColor = UI.ModernTheme.BorderStrong;
            Clear.BorderThickness = 1;
            Clear.ForeColor = UI.ModernTheme.TextPrimary;
            Clear.HoverState.FillColor = Color.FromArgb(76, 34, 46);
            Clear.Font = UI.ModernTheme.BodyBoldFont;
            Clear.Size = new Size(108, 38);
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
