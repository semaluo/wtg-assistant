﻿using System.Windows.Forms;

namespace wintogo
{
    public partial class error : Form
    {
        string errmsg;
        public error(string errmsg)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = Form1.ci;

            this.errmsg = errmsg;
            InitializeComponent();
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1.VisitWeb("http://www.luobotou.pw/forum-88-1.html");
        }

        private void error_Load(object sender, System.EventArgs e)
        {
            this.Text += Application.ProductName + Application.ProductVersion;
            label1.Text += errmsg;
        }      


        private void button1_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void label3_Click(object sender, System.EventArgs e)
        {
            Form1.VisitWeb("http://bbs.luobotou.org/thread-8670-1-1.html");
        }
    }
}
