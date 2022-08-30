using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using pannella.analoguepocket;


namespace Pocket_Updater
{
    public partial class CoreSelector : Form
    {
        private List<Core> _cores;
        private CoreManager _coreManager;
        public CoreSelector()
        {
            InitializeComponent();

            _coreManager = new CoreManager(Directory.GetCurrentDirectory() + "\\auto_update.json");
            _cores = _coreManager.GetCores();

            foreach (Core core in _cores)
            {
                //coresList.Items.Add
                coresList.Items.Add(core, !core.skip);
            }
        }

        private void CoreSelector_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            _readChecklist();
            _coreManager.SaveCores(_cores);
        }

        private void _readChecklist()
        {
            List<Core> newList = new List<Core>();
            for (int i = 0; i <= (coresList.Items.Count - 1); i++)
            {
                Core core = (Core)coresList.Items[i];
                if (!coresList.GetItemChecked(i))
                {
                    core.skip = true;
                }
                newList.Add(core);
            }
            _cores = newList;
        }
    }
}
