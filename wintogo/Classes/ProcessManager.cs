using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace wintogo
{
    public static class ProcessManager
    {
        public static WriteProgress wp;
        //public delegate void AppendTextCallback(string text);
        //public static double percentage = -1;

        #region 解决多线程下控件访问的问题
        //private bool requiresClose = true;


        public static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            // 这里仅做输出的示例，实际上您可以根据情况取消获取命令行的内容  
            // 参考：process.CancelOutputRead()  
            //if (!wp.IsHandleCreated) { wp.Show(); }

            try
            {

                if (string.IsNullOrEmpty(e.Data) == false)
                { AppendText(e.Data + "\r\n"); }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        private static void progress_Exited(object sender, EventArgs e)
        {

            try
            {
                wp.Invoke(new Action(() =>
                {
                    wp.IsUserClosing = false;
                    wp.Close();
                }));
                //// Invoke an anonymous method on the thread of the form.
                //wp.Invoke((MethodInvoker)delegate
                //{

                //    wp.Close();

                //});

            }
            catch (Exception ex)
            {
                Log.WriteLog("progress_Exited", ex.ToString());

            }
        }

        public static void AppendText(string text)
        {
            try
            {
                if (!wp.IsHandleCreated) return;
                if (wp.textBox1.Lines.Length == 0 || wp.textBox1.Lines.Length == 1 || text != wp.textBox1.Lines[wp.textBox1.Lines.Length - 2] + "\r\n")
                {
                    wp.textBox1.Invoke(new Action(() => { wp.textBox1.AppendText(text); }));
                    //if (text.Contains("Leaving")) { wp.Close(); }
                    //if (wp.textBox1.Lines.Length != 0)
                    //MessageBox.Show(text+"\n/////////////\n"+ wp.textBox1.Lines[wp.textBox1.Lines.Length - 2] + "\r\n");
                    //if (wp.textBox1.InvokeRequired)
                    //{
                    //    AppendTextCallback d = new AppendTextCallback(AppendText);
                    //    wp.textBox1.Invoke(d, text);
                    //}
                    //else
                    //{
                    //    wp.textBox1.AppendText(text);

                    //    //this.textBox1.AppendText(text);
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("AppendText.log", ex.ToString());
                Console.WriteLine(ex);
                //MessageBox.Show(ex.ToString());
            }
        }

        #endregion
        public static void SyncCMD(List<string> cmds)
        {
            Process process = new Process();

            try
            {
                process.StartInfo.FileName = "cmd.exe";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                foreach (var cmd in cmds)
                {
                    process.StandardInput.WriteLine(cmd);
                }
                process.StandardInput.WriteLine("exit");

                process.WaitForExit();

            }
            catch (Exception ex)
            {
                MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }
            finally
            {
                process.Close();

            }
        }
        public static int SyncCMD(string cmd)
        {
            Process process = new Process();
            int exitcode = 1;
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
                exitcode = process.ExitCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }
            finally
            {
                process.Close();

            }
            return exitcode;
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
                MessageBox.Show(MsgManager.GetResString("Msg_Failure", MsgManager.ci) + ex.ToString());
            }

        }
        public static void ECMD(string StartFileName, string StartFileArg)
        {
            wp = new WriteProgress();
            wp.IsUserClosing = true;
            ExecuteCMD(StartFileName, StartFileArg);
            try
            {
                wp.ShowDialog();
            }
            catch (UserCancelException)
            {
                KillProcessByName(Path.GetFileName(StartFileName));
            }

        }
        private static void KillProcessByName(string pName)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process item in ps)
                {
                    if (item.ProcessName == pName)
                    {
                        item.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("KillProcessByName.log", ex.ToString());
            }

        }

    }

    [Serializable]
    public class UserCancelException : Exception
    {
        public UserCancelException() : base("用户取消操作！") { }
        public UserCancelException(string message) : base(message) { }
        public UserCancelException(string message, Exception inner) : base(message, inner) { }
        protected UserCancelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
