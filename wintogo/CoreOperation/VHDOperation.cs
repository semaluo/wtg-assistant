using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{
    public class VHDOperation : WTGOperation
    {
        public VHDOperation()
        {
            ShouldContinue = true;
            NeedTwiceAttach = false;
            SetVhdProp();
        }
        private bool ShouldContinue { get; set; }
        //public int wimpart { get; set; }
        private bool NeedTwiceAttach { get; set; }
        //public string diskpartscriptpath { get; set; }
        /// <summary>
        /// vhd or vhdx
        /// </summary>
        /// 
        public string ExtensionType { get; set; }
        public string VhdPath { get; set; }
        public string VhdSize { get; set; }

        /// <summary>
        /// vhdType: fixed or ex...
        /// </summary>
        public string VhdType { get; set; }
        public bool NeedCopy { get; set; }
        public string VhdFileName { get; set; }


        public static void CleanTemp()
        {

            ProcessManager.SyncCMD("taskkill /f /IM imagex_x86.exe");
            ProcessManager.SyncCMD("taskkill /f /IM imagex_x64.exe");

            //KILL.Start();
            //KILL.WaitForExit();

            if (System.IO.Directory.Exists("V:\\"))
            {
                DetachVHDExtra();

            }
            if (System.IO.Directory.Exists("V:\\"))
            {
                DetachVHDExtra();
                //MsgManager.getResString("Msg_LetterV")
                //盘符V不能被占用！

            }
            if (System.IO.Directory.Exists("V:\\"))
            {
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_LetterV", MsgManager.ci));
                er.ShowDialog();
            }
            //if (useiso) { ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\" + "\\isocmd.exe\" -eject 0: "); }
            try
            {
                //File.Delete ()

                FileOperation.DeleteFile(diskpartscriptpath + "\\create.txt");
                FileOperation.DeleteFile(diskpartscriptpath + "\\removex.txt");
                FileOperation.DeleteFile(diskpartscriptpath + "\\detach.txt");
                FileOperation.DeleteFile(diskpartscriptpath + "\\uefi.txt");
                FileOperation.DeleteFile(diskpartscriptpath + "\\uefimbr.txt");
                FileOperation.DeleteFile(diskpartscriptpath + "\\dp.txt");
                FileOperation.DeleteFile(diskpartscriptpath + "\\attach.txt");
                FileOperation.DeleteFile(System.Environment.GetEnvironmentVariable("TEMP") + "\\win8.vhd");
                FileOperation.DeleteFile(System.Environment.GetEnvironmentVariable("TEMP") + "\\win8.vhdx");
                FileOperation.DeleteFile(SetTempPath.temppath + "\\win8.vhd");
                FileOperation.DeleteFile(SetTempPath.temppath + "\\win8.vhdx");
            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_DeleteTempError")
                //程序删除临时文件出错！可重启程序或重启电脑重试！\n
                MessageBox.Show(MsgManager.getResString("Msg_DeleteTempError", MsgManager.ci) + ex.ToString());
                //shouldcontinue = false;
            }

        }
        public static void DetachVHD(string vhdPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=" + vhdPath);
            sb.AppendLine("detach vdisk");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.args = sb.ToString();
            dsm.RunDiskpartScript();

        }
        public void DetachVHD()
        {
            DetachVHD(this.VhdPath);
        }
        public static void DetachVHDExtra()
        {
            DiskpartScriptManager dsm = new DiskpartScriptManager(true);
            dsm.args = "list vdisk";
            dsm.RunDiskpartScript();
            //MessageBox.Show(dsm.outputFilePath);
            string dpoutput = File.ReadAllText(dsm.outputFilePath, Encoding.Default);
            //MessageBox.Show(dpoutput);
            MatchCollection mc = Regex.Matches(dpoutput, @"([a-z]:\\.*win8\.vhd[x]?)", RegexOptions.IgnoreCase);
            foreach (Match item in mc)
            {
                //MessageBox.Show(item.Groups[1].Value);
                DetachVHD(item.Groups[1].Value);
            }
            dsm.DeleteOutputFile();
        }


        public void VHDDynamicSizeIns()
        {
            if (!this.ShouldContinue) return;
            //MsgManager.getResString("FileName_VHD_Dynamic")
            //VHD模式说明.TXT
            using (FileStream fs1 = new FileStream(ud + MsgManager.getResString("FileName_VHD_Dynamic", MsgManager.ci), FileMode.Create, FileAccess.Write))
            {
                fs1.SetLength(0);
                using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default))
                {
                    //try
                    //{
                    //MsgManager.getResString("Msg_VHDDynamicSize")
                    //您创建的VHD为动态大小VHD，实际VHD容量：
                    ////MsgManager.getResString("Msg_VHDDynamicSize2")
                    //在VHD系统启动后将自动扩充为实际容量。请您在优盘留有足够空间确保系统正常启动！
                    sw1.WriteLine(MsgManager.getResString("Msg_VHDDynamicSize", MsgManager.ci) + this.VhdSize + "MB\n");
                    sw1.WriteLine(MsgManager.getResString("Msg_VHDDynamicSize2", MsgManager.ci));
                }
                //}
                //catch { }
                //sw1.Close();
            }

        }

        public void AttachVHD()
        {
            AttachVHD(this.VhdPath);
        }
        public void AttachVHD(string vhdPath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("select vdisk file=" + vhdPath);
            sb.AppendLine("attach vdisk");
            sb.AppendLine("sel partition 1");
            sb.AppendLine("assign letter=v");
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.args = sb.ToString();
            dsm.RunDiskpartScript();

            //GenerateAttachVHDScript(this.vhdPath);
            //ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\attach.txt\"");
        }
        public void CreateVHD()
        {

            if (!ShouldContinue) return;
            else ShouldContinue = false;
            StringBuilder sb = new StringBuilder();
            if (filetype == "vhd")
            {
                //sb.AppendLine("create vdisk file=" + this.VhdPath + " type=" + this.VhdType + " maximum=" + this.VhdSize);
                sb.AppendLine("select vdisk file=" + this.VhdPath);
                sb.AppendLine("attach vdisk");
                sb.AppendLine("assign letter=v");
                sb.AppendLine("exit");
            }
            else
            {
                sb.AppendLine("create vdisk file=" + this.VhdPath + " type=" + this.VhdType + " maximum=" + this.VhdSize);
                sb.AppendLine("select vdisk file=" + this.VhdPath);
                sb.AppendLine("attach vdisk");
                sb.AppendLine("create partition primary");
                sb.AppendLine("format fs=ntfs quick");
                sb.AppendLine("assign letter=v");
                sb.AppendLine("exit");
            }
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            Log.WriteLog("CreateVHDScript.log", sb.ToString());
            //MessageBox.Show(sb.ToString());
            dsm.args = sb.ToString();
            dsm.RunDiskpartScript();

            try
            {
                if (!System.IO.Directory.Exists("V:\\"))
                {
                    ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
                    er.ShowDialog();
                    this.ShouldContinue = false;

                    //shouldcontinue = false;
                    return;
                }
                else
                {
                    this.ShouldContinue = true;

                }
            }
            catch
            {
                //创建VHD失败，未知错误
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
                this.ShouldContinue = false;
                //shouldcontinue = false;

            }
            if (filetype != "vhd" && filetype != "vhdx")
            {
                ApplyToVdisk();
            }
            if (filetype != "vhd" && filetype != "vhdx" && !iswimboot && !File.Exists(@"v:\windows\regedit.exe"))
            {
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_ApplyError", MsgManager.ci));
                er.ShowDialog();
                this.ShouldContinue = false;

            }
            else
            {
                this.ShouldContinue = true;
            }
        }
        public void VHDExtra()
        {
            if (!ShouldContinue) return;

            ImageOperation.ImageExtra(framework, san_policy, diswinre, @"v:\", imageFilePath);
            UEFIAndWin7ToGo();

        }
        public void CopyVHD()
        {
            if (!ShouldContinue) return;

            if (this.NeedCopy)
            {
                if (System.IO.File.Exists(this.VhdPath))
                {
                    //Application.DoEvents();
                    //Thread.Sleep(100);
                    //Application.DoEvents();

                    ProcessManager.ECMD(WTGOperation.filesPath+ "\\fastcopy.exe", " /auto_close \"" + this.VhdPath + "\" /to=\"" + WTGOperation.ud + "\"");
                    //BigFileCopy ()
                    //MsgManager.getResString("Msg_Copy")
                    //复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n
                    ProcessManager.AppendText(MsgManager.getResString("Msg_Copy", MsgManager.ci));


                    //AppendText("复制文件中...大约需要10分钟~1小时，请耐心等待！");
                    //wp.Show();
                    //Application.DoEvents();
                    //System.Diagnostics.Process cp = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\fastcopy.exe", " /auto_close \"" + Form1.vpath + "\" /to=\"" + ud + "\"");
                    //cp.WaitForExit();
                    //wp.Close();
                }
                if ((WTGOperation.filetype == "vhd" && !this.VhdPath.EndsWith("win8.vhd")) || (WTGOperation.filetype == "vhdx" && !this.VhdPath.EndsWith("win8.vhdx")))
                {
                    //Rename
                    //MsgManager.getResString("Msg_RenameError")
                    //重命名错误
                    try { File.Move(WTGOperation.ud + this.VhdPath.Substring(this.VhdPath.LastIndexOf("\\") + 1), WTGOperation.ud + WTGOperation.win8VhdFile); }
                    catch (Exception ex) { MessageBox.Show(MsgManager.getResString("Msg_RenameError", MsgManager.ci) + ex.ToString()); }
                }
            }
        }
        public void WriteBootFiles()
        {
            if (!this.ShouldContinue) return;
            if (!NeedCopy)
            {
                if (isuefigpt)
                {
                    BootFileOperation.BcdbootWriteUEFIBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);
                    //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");
                }
                else if (isuefimbr)
                {
                    BootFileOperation.BcdbootWriteALLBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);
                    BootFileOperation.BooticeWriteMBRPBRAndAct("X:");
                    //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");
                }
                else if (iswimboot)
                {
                    BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);
                    //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");
                }
                else
                {
                    if (!commonbootfiles)
                    {
                        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);
                        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");
                    }
                    else
                    {
                        CopyVHDBootFiles();
                        //needcopyvhdbootfile = true;
                        //copyvhdbootfile();
                    }
                }
            }
            else
            {
                if (commonbootfiles)
                {
                    CopyVHDBootFiles();
                }
                else
                {
                    NeedTwiceAttach = true;
                }
            }
        }

        public void Execute()
        {
            CleanTemp();
            if (filetype == "vhd" || filetype == "vhdx")
            {
                this.CopyVHD();
                if (commonbootfiles)
                {
                    this.CopyVHDBootFiles();
                }
                else
                {
                    this.TwiceAttachVHDAndWriteBootFile();
                }
            }
            else
            {
                this.CreateVHD();
                this.VHDExtra();
                this.WriteBootFiles();
                this.DetachVHD();
                this.CopyVHD();
                this.TwiceAttachVHDAndWriteBootFile();
                BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
                if (this.VhdType != "fixed") this.VHDDynamicSizeIns();
            }
        }

        private void TwiceAttachVHDAndWriteBootFile()
        {
            if (!ShouldContinue) return;
            //AttachVHD(StringOperation.Combine(WTGOperation.ud, win8VhdFile));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=" + StringOperation.Combine(WTGOperation.ud, win8VhdFile));
            sb.AppendLine("attach vdisk");
            sb.AppendLine("sel partition 1");
            sb.AppendLine("assign letter=v");
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            Log.WriteLog("TwiceAttachVhd.log", sb.ToString());

            dsm.args = sb.ToString();
            dsm.RunDiskpartScript();

            try
            {
                if (!System.IO.Directory.Exists("V:\\"))
                {
                    Log.WriteLog("TwiceAttachVhdError.log", "二次加载VHD失败");
                }

            }
            catch
            {
                //创建VHD失败，未知错误
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
                this.ShouldContinue = false;
                //shouldcontinue = false;

            }
            BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);
            DetachVHD(StringOperation.Combine(WTGOperation.ud, win8VhdFile));
            if (!System.IO.File.Exists(WTGOperation.ud + "\\Boot\\BCD"))
            {

                CopyVHDBootFiles();
                //Log.WriteLog ("TwiceAttachFailed.log")
            }

        }

        #region 私有方法
        private void SetVhdProp()
        {
            if (Path.GetExtension(imageFilePath) == ".vhd" || Path.GetExtension(imageFilePath) == ".vhdx")
            {
                this.VhdType = string.Empty;
                this.VhdSize = string.Empty;
                this.VhdFileName = string.Empty;
                this.ExtensionType = string.Empty;
                this.VhdPath = imageFilePath;
                this.NeedCopy = true;
            }
            else
            {
                //    ////////////////vhd设定///////////////////////
                //    string vhd_type = "expandable";
                //    vhd_size = "";
                if (isfixed)
                {
                    this.VhdType = "fixed";
                }
                else
                {
                    this.VhdType = "expandable";
                }
                if (userSetSize != 0)
                {
                    this.VhdSize = (userSetSize * 1024).ToString();
                }
                else
                {
                    if (!iswimboot)
                    {
                        if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 21504) { this.VhdSize = "20480"; }
                        else { this.VhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 500).ToString(); }
                    }
                    else
                    {
                        if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 24576) { this.VhdSize = "20480"; }
                        else { this.VhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 4096).ToString(); }
                    }
                }
                ////////////////判断临时文件夹,VHD needcopy?///////////////////
                int vhdmaxsize;
                if (WTGOperation.isfixed)
                {
                    vhdmaxsize = System.Int32.Parse(this.VhdSize) * 1024 + 1024;
                }
                else
                {
                    vhdmaxsize = 10485670;
                }

                if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || StringOperation.IsChina(SetTempPath.temppath) || isuefigpt || isuefimbr || iswimboot || isnotemp)
                {
                    this.NeedCopy = false;
                    this.VhdPath = StringOperation.Combine(WTGOperation.ud, win8VhdFile);
                }
                else
                {
                    this.NeedCopy = true;
                    this.VhdPath = StringOperation.Combine(SetTempPath.temppath, win8VhdFile);
                }

            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.ExtensionType);
            sb.AppendLine(this.NeedCopy.ToString());
            sb.AppendLine(this.VhdFileName);
            sb.AppendLine(this.VhdPath);
            sb.AppendLine(this.VhdSize);
            sb.AppendLine(this.VhdType);
            Log.WriteLog("VHDInfo.log", sb.ToString());
            //if (!this.NeedCopy)
            //{
            //    //this.NeedCopy = false;
            //    //usetemp = false;

            //    //this.VhdPath = WTGOperation.ud + this.VhdFileName;
            //}
            //else
            //{
            //    //SetTempPath.temppath + "\\" + win8vhdfile;
            //    //needcopy = true;
            //}
        }

        private void ApplyToVdisk()
        {
            //ImageOperation io = new ImageOperation();
            //io.imageFile = WTGOperation.path;
            //io.imageX = WTGOperation.imagex;
            //io.AutoChooseWimIndex();
            //ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
            //MessageBox.Show(WTGOperation.wimpart);
            //MessageBox.Show(WTGOperation.wimpart);

            ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
            ImageOperation.ImageApply(iswimboot, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.imageFilePath, WTGOperation.wimpart, @"v:\", WTGOperation.ud);

        }
        /// <summary>
        /// WIN7注册表和FixLetter 。所有VHD都需要FixLetter
        /// </summary>
        private void UEFIAndWin7ToGo()
        {
            if (WTGOperation.win7togo != 0) { ImageOperation.Win7REG("V:\\"); }
            //////////////
            //if (isuefigpt)
            //{
            ImageOperation.Fixletter("C:", "V:");
            //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
            //}
        }

        private void CopyVHDBootFiles()
        {
            //this.CopyVHD();
            //if (!needcopyvhdbootfile) return;
            ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + " /e /h /y");

            if (this.ExtensionType == "vhdx")
            {
                ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");
            }
            Log.WriteLog("CopyVHDBootFiles.log", "xcopy.exe" + "\"" + WTGOperation.filesPath+ "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");
        }
        private void BigFileCopy(string source, string target, int buffersize)
        {
            using (FileStream fsRead = File.OpenRead(source))
            {
                using (FileStream fsWrite = File.OpenWrite(target))
                {

                    byte[] byts = new byte[buffersize * 1024 * 1024];
                    int r = 0;
                    while ((r = fsRead.Read(byts, 0, byts.Length)) > 0)
                    {
                        fsWrite.Write(byts, 0, r);
                        //Console.WriteLine(fsWrite.Position / (double)fsRead.Length * 100);
                        //r = fsRead.Read(byts, 0, byts.Length);
                    }

                }
            }
        }
        #endregion
    }
}
