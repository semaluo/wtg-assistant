using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
namespace wintogo
{
    public static class Log
    {
        public static void WriteLog(string LogName, string WriteInfo)
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + "\\logs\\")) { Directory.CreateDirectory(Application.StartupPath + "\\logs\\"); }
                if (File.Exists(Application.StartupPath + "\\logs\\" + LogName)) { File.Delete(Application.StartupPath + "\\logs\\" + LogName); }
                using (FileStream fs0 = new FileStream(Application.StartupPath + "\\logs\\" + LogName, FileMode.Append, FileAccess.Write))
                {
                    fs0.SetLength(0);
                    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
                    {
                        string ws0 = "";

                        ws0 = Application.ProductName + Application.ProductVersion;
                        sw0.WriteLine(ws0);
                        ws0 = System.DateTime.Now.ToString();
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
            ProcessManager.SyncCMD("cmd.exe /c del /f /s /q \"" + Application.StartupPath + "\\logs\\*.*\"");
        }

    }
}