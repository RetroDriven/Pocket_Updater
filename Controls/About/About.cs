using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pocket_Updater.Controls.About
{
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            ApplyModernLayout();
        }

        private void ApplyModernLayout()
        {
            UI.ModernTheme.Apply(this);
            UI.ModernTheme.ApplyPageChrome(this);

            Panel_Top.FillColor = Color.Transparent;
            label4.Font = UI.ModernTheme.TitleFont;
            label4.ForeColor = UI.ModernTheme.TextPrimary;
            guna2Separator2.FillColor = UI.ModernTheme.Border;

            Panel_Main.BackColor = UI.ModernTheme.AppBackground;
            Panel_Main.Padding = new Padding(0, 4, 0, 0);
            flowLayoutPanel1.BackColor = UI.ModernTheme.Surface;
            flowLayoutPanel1.Padding = new Padding(18);

            tableLayoutPanel2.Padding = new Padding(0);
            tableLayoutPanel2.Margin = new Padding(0, 0, 0, 16);
            foreach (Control control in tableLayoutPanel2.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = UI.ModernTheme.TextSecondary;
                    label.Font = UI.ModernTheme.BodyBoldFont;
                    label.Margin = new Padding(0, 6, 12, 6);
                }
            }

            foreach (var button in new[] { GitHub, Release, ReadMe, guna2Button1 })
            {
                button.AutoRoundedCorners = false;
                button.BorderRadius = 9;
                button.FillColor = UI.ModernTheme.Accent;
                button.HoverState.FillColor = UI.ModernTheme.AccentHover;
                button.Font = UI.ModernTheme.BodyBoldFont;
                button.Size = new Size(96, 36);
            }

            label5.Font = UI.ModernTheme.SectionFont;
            label5.ForeColor = UI.ModernTheme.TextPrimary;
            label5.Margin = new Padding(0, 8, 0, 0);
            guna2Separator1.FillColor = UI.ModernTheme.Border;

            tableLayoutPanel3.Margin = new Padding(0, 8, 0, 0);
            label6.ForeColor = UI.ModernTheme.TextPrimary;
            label7.ForeColor = UI.ModernTheme.TextPrimary;
            label6.Font = UI.ModernTheme.BodyBoldFont;
            label7.Font = UI.ModernTheme.BodyBoldFont;

            foreach (var button in new[] { Matt_GitHub, Josh_GitHub })
            {
                button.AutoRoundedCorners = false;
                button.BorderRadius = 9;
                button.FillColor = UI.ModernTheme.SurfaceRaised;
                button.BorderColor = UI.ModernTheme.BorderStrong;
                button.BorderThickness = 1;
                button.HoverState.FillColor = UI.ModernTheme.SurfaceHover;
                button.Font = UI.ModernTheme.BodyBoldFont;
                button.ForeColor = UI.ModernTheme.TextPrimary;
            }

            foreach (var box in new[] { textBox1, textBox2 })
            {
                box.BackColor = UI.ModernTheme.Surface;
                box.ForeColor = UI.ModernTheme.TextSecondary;
                box.BorderStyle = BorderStyle.None;
                box.Font = UI.ModernTheme.BodyFont;
                box.ReadOnly = true;
            }
        }

        private void GitHub_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater");
        }

        private void Release_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater/releases/latest");
        }

        private void ReadMe_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater/blob/master/README.md");
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/RetroDriven/Pocket_Updater/issues");
        }

        private void Matt_GitHub_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/mattpannella/pocket-updater-utility");
        }

        private void Josh_GitHub_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", "https://github.com/openfpga-cores-inventory/analogue-pocket");
        }
    }
}
