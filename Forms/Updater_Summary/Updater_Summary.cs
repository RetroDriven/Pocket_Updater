using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pocket_Updater.Forms.Updater_Summary
{
    public partial class Updater_Summary : Form
    {
        public Updater_Summary()
        {
            InitializeComponent();
            ApplyModernLayout();
        }

        private void ApplyModernLayout()
        {
            UI.ModernTheme.Apply(this);

            BackColor = UI.ModernTheme.AppBackground;
            guna2BorderlessForm1.BorderRadius = 14;
            guna2BorderlessForm1.ShadowColor = UI.ModernTheme.BorderStrong;

            Panel_Top.AutoSize = false;
            Panel_Top.Height = 52;
            Panel_Top.FillColor = UI.ModernTheme.TopBar;
            Panel_Top.BackColor = UI.ModernTheme.TopBar;
            label4.Font = UI.ModernTheme.SectionFont;
            label4.ForeColor = UI.ModernTheme.TextPrimary;
            label4.Location = new Point(16, 13);

            foreach (var controlBox in new[] { guna2ControlBox1, guna2ControlBox2, guna2ControlBox3 })
            {
                controlBox.FillColor = Color.Transparent;
                controlBox.IconColor = UI.ModernTheme.TextSecondary;
                controlBox.HoverState.FillColor = UI.ModernTheme.SurfaceHover;
                controlBox.Size = new Size(30, 30);
            }
            guna2ControlBox3.HoverState.FillColor = UI.ModernTheme.Danger;

            textBox1.FillColor = Color.FromArgb(8, 13, 23);
            textBox1.BackColor = Color.FromArgb(8, 13, 23);
            textBox1.BorderColor = UI.ModernTheme.Border;
            textBox1.BorderThickness = 1;
            textBox1.BorderRadius = 0;
            textBox1.ForeColor = Color.FromArgb(210, 220, 236);
            textBox1.Font = UI.ModernTheme.MonoFont;
        }

    }
}
