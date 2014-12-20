using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wintogo
{
    public partial class writeprogress : Form
    {
        public writeprogress()
        {
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
                ws = Application.StartupPath + "\r\n程序版本："+Application.ProductVersion+"\r\n" + System.DateTime.Now;
                sw.WriteLine(ws);
                try
                {
                    ws = textBox1.Text;
                    sw.WriteLine(ws);
                }
                catch { }
                sw.Close();
                textBox1.Text = "";
            }
            catch { }

        }

        private void writeprogress_Load(object sender, EventArgs e)
        {
            Random ra = new Random();
            num=ra.Next(0, 9);
            
            linkLabel1.Text = Form1.topicname [num];
            textBox1.Focus();
            //设置光标的位置到文本尾
            textBox1.Select(textBox1.TextLength, 0);
            //滚动到控件光标处
            textBox1.ScrollToCaret();


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void win8PB1_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1.VisitWeb(Form1.topiclink [num]);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
