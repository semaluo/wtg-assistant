using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace wintogo
{
    public static class Log
    {
        /// <summary>
        /// 写入日志文件，可附加
        /// </summary>
        /// <param name="LogName">文件名</param>
        /// <param name="WriteInfo">日志信息</param>
        public static void WriteLog(string LogName, string WriteInfo)
        {
            try
            {
                if (!Directory.Exists(WTGModel.logPath)) { Directory.CreateDirectory(WTGModel.logPath); }
                if (File.Exists(WTGModel.logPath + "\\" + LogName)) { File.Delete(WTGModel.logPath + "\\" + LogName); }
                using (FileStream fs0 = new FileStream(WTGModel.logPath + "\\" + LogName, FileMode.Append, FileAccess.Write))
                {
                    fs0.SetLength(0);
                    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
                    {
                        string ws0 = "";

                        ws0 = Application.ProductName + Application.ProductVersion;
                        sw0.WriteLine(ws0);
                        ws0 = DateTime.Now.ToString();
                        sw0.WriteLine(ws0);
                        ws0 = WriteInfo;
                        sw0.WriteLine(ws0);

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            //sw0.Close();


        }
        public static void DeleteAllLogs()
        {
            ProcessManager.SyncCMD("cmd.exe /c del /f /s /q \"" + WTGModel.logPath + "\\*.*\"");
        }

    }
}