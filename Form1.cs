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

        private void Button_Pocket_Click(object sender, EventArgs e)
        {
            Updater_Pocket form = new Updater_Pocket();
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CoreSelector form = new CoreSelector();
            form.Show();
        }
    }
}