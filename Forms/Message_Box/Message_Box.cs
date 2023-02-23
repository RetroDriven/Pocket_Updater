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
        }

        private void Button_Ok_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
