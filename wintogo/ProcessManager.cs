using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace wintogo
{
    public static class ProcessManager 
    {
        public static WriteProgress wp=new WriteProgress ();
        public delegate void AppendTextCallback(string text);

        #region 解决多线程下控件访问的问题
        //private bool requiresClose = true;

        /// <summary>
        /// 结束进程
        /// </summary>
        /// 
        //public static void ShowForm()
        //{
        //    Invoke(new MethodInvoker(Showd));

        //}
        //public static void End()
        //{
        //    Invoke(new MethodInvoker(DoEnd));

        //}

        //public static void DoEnd()
        //{
        //    wp.Close();
        //}
        //private void Showd()
        //{
        //    wp.ShowDialog();


        //}
        public static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // 这里仅做输出的示例，实际上您可以根据情况取消获取命令行的内容  
            // 参考：process.CancelOutputRead()  
            //if (!wp.IsHandleCreated) { wp.Show(); }
            try
            {

                if (String.IsNullOrEmpty(e.Data) == false)
                    AppendText(e.Data + "\r\n");
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        private static void progress_Exited(object sender, EventArgs e)
        {
            //MessageBox.Show("exz");
            try
            {

                // Invoke an anonymous method on the thread of the form.
                wp.Invoke((MethodInvoker)delegate
                {
                    // Show the current time in the form's title bar.
                    wp.Close();
                    //this.Text = DateTime.Now.ToLongTimeString();
                });

                //End();
                //wp.Invoke ()
                //wp.Close();
            }
            catch(Exception ex)
            { Console.WriteLine(ex); }
        }

        public static void AppendText(string text)
        {
            try
            {
                if (wp.textBox1.Lines.Length == 0 || wp.textBox1.Lines.Length == 1 || text != wp.textBox1.Lines[wp.textBox1.Lines.Length - 2] + "\r\n")
                {
                    //if (text.Contains("Leaving")) { wp.Close(); }
                    //if (wp.textBox1.Lines.Length != 0)
                    //MessageBox.Show(text+"\n/////////////\n"+ wp.textBox1.Lines[wp.textBox1.Lines.Length - 2] + "\r\n");
                    if (wp.textBox1.InvokeRequired)
                    {
                        AppendTextCallback d = new AppendTextCallback(AppendText);
                        wp.textBox1.Invoke(d, text);
                    }
                    else
                    {
                        wp.textBox1.AppendText(text);

                        //this.textBox1.AppendText(text);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                //MessageBox.Show(ex.ToString());
            }
        }

        #endregion
        public static void SyncCMD(string cmd)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            try
            {
                process.StartInfo.FileName = "cmd.exe";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine(cmd);

                process.StandardInput.WriteLine("exit");

                process.WaitForExit();

            }
            catch (Exception ex)
            {
                MessageBox.Show(MsgManager.getResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }
            finally
            {
                process.Close();

            }
        }
        private static void ExecuteCMD(string StartFileName, string StartFileArg)
        {

            Process process = new Process();
            //wp.ShowDialog();

            try
            {
                AppendText("Command:" + StartFileName + StartFileArg + "\r\n");
                process.StartInfo.FileName = StartFileName;
                process.StartInfo.Arguments = StartFileArg;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(progress_Exited);

                process.Start();


                process.BeginOutputReadLine();


            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_Failure")
                //操作失败
                MessageBox.Show(MsgManager.getResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }

        }
        public static void ECMD(string StartFileName, string StartFileArg)
        {
            ExecuteCMD(StartFileName, StartFileArg);
            wp.ShowDialog();

        }

    }
}
