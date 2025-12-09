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
