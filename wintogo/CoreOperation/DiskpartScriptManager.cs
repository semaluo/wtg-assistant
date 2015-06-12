using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wintogo
{
    public class DiskpartScriptManager
    {
        /// <summary>
        /// Diskpart命令
        /// </summary>
        public string args { private get; set; }
        private string scriptPath { get; set; }
        public bool outputToFile { get; set; }
        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string outputFilePath { get; private set; }
        public DiskpartScriptManager()
        {
            this.outputToFile = false;
        }
        public DiskpartScriptManager(bool outputToFile)
        {
            if (outputToFile) this.outputToFile = true;
        }
        private void CreateScriptFile()
        {
            this.scriptPath = Path.GetTempFileName();
            using (FileStream fs = new FileStream(scriptPath, FileMode.Create, FileAccess.Write))
            {
                fs.SetLength(0);
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    string ws = args;
                    //System.Windows.Forms.MessageBox.Show(args);
                    sw.Write(ws);
                }
            }
        }
        /// <summary>
        /// 执行Diskpart命令
        /// </summary>

        public void RunDiskpartScript()
        {
            this.outputFilePath = Path.GetTempFileName();
            CreateScriptFile();
            StringBuilder dpargs = new StringBuilder();
            dpargs.Append(" /s \"");
            dpargs.Append(this.scriptPath);
            dpargs.Append("\"");
            if (this.outputToFile)
            {
                dpargs.Append(" > ");
                dpargs.Append("\"");
                dpargs.Append(this.outputFilePath);
                dpargs.Append("\"");
                ProcessManager.SyncCMD("diskpart.exe" + dpargs.ToString());
            }
            else
            {
                ProcessManager.ECMD("diskpart.exe", dpargs.ToString());
            }
            //System.Console.WriteLine(File.ReadAllText (this.scriptPath));
            //System.Console.WriteLine(dpargs.ToString());
            //System.Windows.Forms.MessageBox.Show(dpargs.ToString());
            
            //System.Console.WriteLine(File.ReadAllText (this.outputFilePath));
            FileOperation.DeleteFile(this.scriptPath);
        }
        /// <summary>
        /// 删除输出文件
        /// </summary>
        public void DeleteOutputFile()
        {
            FileOperation.DeleteFile(this.outputFilePath);
        }

  

    }
}
