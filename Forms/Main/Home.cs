using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pocket_Updater.Forms.Main
{
    public partial class Home : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {

        public Home()
        {
            InitializeComponent();
            UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXI);
            UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.WXI.Darkness);
        }

        private void UpdatePocket_Click(object sender, EventArgs e)
        {
            update_Pocket1.Visible = true;
            update_Pocket1.BringToFront();
        }

        private void UpdatePocket_EnabledChanged(object sender, EventArgs e)
        {

        }
    }
}
