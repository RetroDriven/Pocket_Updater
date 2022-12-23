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

namespace Pocket_Updater.Controls
{
    public partial class Update_Pocket : DevExpress.XtraEditors.XtraUserControl
    {
        public Update_Pocket()
        {
            InitializeComponent();
            UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.Bezier.MercuryIce);
            //UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXI);
            //UserLookAndFeel.Default.SetSkinStyle(SkinSvgPalette.WXI.Darkness);

        }

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
