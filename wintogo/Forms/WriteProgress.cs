using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace wintogo
{
    public partial class WriteProgress : Form
    {
        public static string[] topicName = new string[10];
        public static string[] topicLink = new string[10];

        public WriteProgress()
        {
            //CultureInfo ca = new System.Globalization.CultureInfo("en");
            //MessageBox.Show(Form1.ci.DisplayName);
            System.Threading.Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            InitializeComponent();
        }
        int num = 0;
        private void writeprogress_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //if (System.IO.Directory .Exists ())
                FileStream fs = new FileStream(Application.StartupPath + "\\logs\\" + DateTime.Now.ToFileTime() + ".log", FileMode.Create, FileAccess.Write);
                fs.SetLength(0);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                string ws = "";
                ws = Application.StartupPath + "\r\n程序版本：" + Application.ProductVersion + "\r\n" + System.DateTime.Now;
                sw.WriteLine(ws);
                ws = textBox1.Text;
                sw.WriteLine(ws);
                sw.Close();
                textBox1.Text = "";
            }
            catch (Exception ex)
            {
                Log.WriteLog("WriteProgressFormClosingError.log", ex.ToString());
            }

        }

        private void writeprogress_Load(object sender, EventArgs e)
        {
            Random ra = new Random();
            num = ra.Next(0, 9);
            try
            {
                linkLabel1.Text = topicName[num];
                textBox1.Focus();
                //设置光标的位置到文本尾
                textBox1.Select(textBox1.TextLength, 0);
                //滚动到控件光标处
                textBox1.ScrollToCaret();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message );
                Log.WriteLog("WriteProgressLoad.log", ex.ToString());
            }


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void win8PB1_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1.VisitWeb(topicLink[num]);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
