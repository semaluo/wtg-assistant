using System;
using System.Windows.Forms;

namespace wintogo
{
    public partial class ChoosePart : Form
    {
        //public static int part;
        public ChoosePart()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            InitializeComponent();
        }

        private void choosepart_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = Int32.Parse(WTGOperation.wimpart);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WTGOperation.wimpart = numericUpDown1.Value.ToString();
            //part =(int) numericUpDown1.Value ;
            
            this.Close();
        }
    }
}
