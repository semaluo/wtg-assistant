﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace wintogo
{
    public partial class Win8PB : UserControl
    {
        
        int p1 = 1;
        int p2 = 1;
        int p3 = 1;
        int p4 = 1;
        int p5 = 1;

        int v1 = 10;
        int v2 = 10;
        int v3 = 10;
        int v4 = 10;
        int v5 = 10;

        public Win8PB()
        {
            InitializeComponent();
            //Console.WriteLine("初始化");
        }

        private void Win8PB_Load(object sender, EventArgs e)
        {
            //InitializeComponent();
            //Console.WriteLine("Load");
            //timer1 = new System.Windows.Forms.Timer();
            //timer2 = new System.Windows.Forms.Timer();
            //timer3 = new System.Windows.Forms.Timer();
            //timer4 = new System.Windows.Forms.Timer();
            //timer5 = new System.Windows.Forms.Timer();
            //MessageBox.Show("Test");
            //MessageBox.Show ("")
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            timer4.Enabled = true;
            timer5.Enabled = true;




            timer1.Interval = 50;
            timer2.Interval = 50;
            timer3.Interval = 50;
            timer4.Interval = 50;
            timer5.Interval = 50;
            label1.Left = 500;
            label2.Left = 500;
            label3.Left = 500;
            label4.Left = 500;
            label5.Left = 500;
            timer1.Start();
            timer2.Start();

            timer3.Start();
            timer4.Start();
            timer5.Start();


          
            
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (p1 == 1) 
            {
                //label1.Invoke((MethodInvoker)delegate
                //{
                //    // Show the current time in the form's title bar.
                //    label1.Left += v1;
                //    //this.Text = DateTime.Now.ToLongTimeString();
                //});

                label1.Left += v1;
                v1 = v1 - 1;
                if (v1 == 2) { p1 = 2; }
            }
            else if (p1 == 2)
            {
            //    label1.Invoke((MethodInvoker)delegate
            //    {
            //        // Show the current time in the form's title bar.
            //        label1.Left += v1;
            //        //this.Text = DateTime.Now.ToLongTimeString();
            //    });

                label1.Left += v1;
                if (label1.Left >= 150) 
                {
                    p1 = 3;
                }
            }
            else if (p1 == 3) 
            {
                label1.Left += v1;
                v1 = v1 + 1;
                if (label1.Left > 300&&label5 .Left >300) { label1.Left = 0; p1 = 1; v1 = 10; }
            }
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (p2 == 1 && label1.Left > 48)
            {
                label2.Left += v2;
                v2 = v2 - 1;
                if (v2 == 2) { p2 = 2; }
            }
            else if (p2 == 2)
            {
                label2.Left += v2;
                if (label2.Left >= 150)
                {
                    p2 = 3;
                }
            }
            else if (p2 == 3)
            {
                label2.Left += v2;
                v2= v2 + 1;
                if (label2.Left > 300 && label1.Left > 48&&label1.Left <300) { label2.Left = 0; p2 = 1; v2 = 10; }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (p3 == 1 && label2.Left > 48)
            {

                label3.Left += v3;


                v3 = v3 - 1;
                if (v3 == 2) { p3 = 2; }
            }
            else if (p3 == 2)
            {
                label3.Left += v3;
                if (label3.Left >= 150)
                {
                    p3 = 3;
                }
            }
            else if (p3 == 3)
            {
                label3.Left += v3;
                v3 = v3 + 1;
                if (label3.Left > 300 && label2.Left > 48 && label2.Left < 300) { label3.Left = 0; p3 = 1; v3 = 10; }
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (p4 == 1 && label3.Left > 48)
            {

                label4.Left += v4;


                v4 = v4 - 1;
                if (v4 == 2) { p4 = 2; }
            }
            else if (p4 == 2)
            {
                label4.Left += v4;
                if (label4.Left >= 150)
                {
                    p4 = 3;
                }
            }
            else if (p4 == 3)
            {
                label4.Left += v4;
                v4 = v4 + 1;
                if (label4.Left > 300 && label3.Left > 48 && label3.Left < 300) { label4.Left = 0; p4 = 1; v4 = 10; }
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            if (p5 == 1 && label4.Left > 48)
            {

                label5.Left += v5;


                v5 = v5 - 1;
                if (v5 == 2) { p5 = 2; }
            }
            else if (p5 == 2)
            {
                label5.Left += v5;
                if (label5.Left >= 150)
                {
                    p5 = 3;
                }
            }
            else if (p5 == 3)
            {
                label5.Left += v5;
                v5 = v5 + 1;
                if (label5.Left > 300 && label4.Left > 48 && label4.Left < 300) 
                {
                    label5.Left = 0; p5 = 1; v5 = 10; 
                }
            }
        }

       
    }
}
