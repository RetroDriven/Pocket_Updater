namespace Pocket_Updater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button_PC_Click(object sender, EventArgs e)
        {
            Updater_PC form = new Updater_PC();
            form.Show();
        }

        private void Button_Log_Click(object sender, EventArgs e)
        {

        }
    }
}