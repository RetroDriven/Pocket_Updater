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

namespace Pocket_Updater.Forms.Message_Box
{
    public partial class Updates_Message_Box : Form
    {
        private const string API_URL = "https://api.github.com/repos/RetroDriven/Pocket_Updater/releases";
        private const string RELEASE_URL = "https://github.com/RetroDriven/Pocket_Updater/releases/latest";

        public Updates_Message_Box()
        {
            InitializeComponent();
            ApplyModernLayout();
        }

        private void ApplyModernLayout()
        {
            UI.ModernTheme.Apply(this);

            BackColor = UI.ModernTheme.Surface;
            Padding = new Padding(20);
            guna2BorderlessForm1.BorderRadius = 14;
            guna2BorderlessForm1.ShadowColor = UI.ModernTheme.BorderStrong;
            guna2Elipse1.BorderRadius = 14;
            guna2DragControl1.TargetControl = this;

            label1.ForeColor = UI.ModernTheme.TextPrimary;
            label1.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            label1.TextAlign = ContentAlignment.MiddleCenter;

            Button_Yes.AutoRoundedCorners = false;
            Button_Yes.BorderRadius = 9;
            Button_Yes.FillColor = UI.ModernTheme.Accent;
            Button_Yes.HoverState.FillColor = UI.ModernTheme.AccentHover;
            Button_Yes.Font = UI.ModernTheme.BodyBoldFont;

            Button_No.AutoRoundedCorners = false;
            Button_No.BorderRadius = 9;
            Button_No.FillColor = UI.ModernTheme.SurfaceRaised;
            Button_No.BorderColor = UI.ModernTheme.BorderStrong;
            Button_No.BorderThickness = 1;
            Button_No.HoverState.FillColor = UI.ModernTheme.SurfaceHover;
            Button_No.Font = UI.ModernTheme.BodyBoldFont;
            Button_No.ForeColor = UI.ModernTheme.TextPrimary;
        }

        private void Button_Yes_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", RELEASE_URL);
            Close();
        }

        private void Button_No_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
