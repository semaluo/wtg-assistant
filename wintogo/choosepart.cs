using System;
using System.Windows.Forms;

namespace wintogo
{
    public partial class choosepart : Form
    {
        public static int part;
        public choosepart()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = Form1.ci;

            InitializeComponent();
        }

        private void choosepart_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = part;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            part =(int) numericUpDown1.Value ;
            
            this.Close();
        }
    }
}
