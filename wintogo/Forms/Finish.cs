﻿using System;
using System.Globalization;
using System.Windows.Forms;

namespace wintogo

{
    public partial class Finish : Form
    {
        public Finish(TimeSpan ts)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = MsgManager.ci;
            InitializeComponent();
            lblTime.Text = ts.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1.VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=804");
           
        }

        private void finish_Load(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1.VisitWeb("http://bbs.luobotou.org/thread-5258-1-1.html");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-5258-1-1.html");
        }

  
    }
}
