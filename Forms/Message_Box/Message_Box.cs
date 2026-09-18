using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pocket_Updater.Forms.Message_Box
{
    public partial class Message_Box : Form
    {
        public Message_Box()
        {
            InitializeComponent();
            ApplyModernLayout();
        }

        private void ApplyModernLayout()
        {
            UI.ModernTheme.Apply(this);

            BackColor = UI.ModernTheme.Surface;
            ClientSize = new Size(360, 150);
            Padding = new Padding(20);

            guna2BorderlessForm1.BorderRadius = 14;
            guna2BorderlessForm1.ShadowColor = UI.ModernTheme.BorderStrong;
            guna2Elipse1.BorderRadius = 14;
            guna2DragControl1.TargetControl = this;

            label1.ForeColor = UI.ModernTheme.TextPrimary;
            label1.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(24, 22);
            label1.Size = new Size(312, 58);
            label1.TextAlign = ContentAlignment.MiddleCenter;

            Button_Ok.AutoRoundedCorners = false;
            Button_Ok.BorderRadius = 9;
            Button_Ok.FillColor = UI.ModernTheme.Accent;
            Button_Ok.HoverState.FillColor = UI.ModernTheme.AccentHover;
            Button_Ok.Font = UI.ModernTheme.BodyBoldFont;
            Button_Ok.Size = new Size(92, 38);
            Button_Ok.Location = new Point((ClientSize.Width - Button_Ok.Width) / 2, 96);
        }

        private void Button_Ok_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
