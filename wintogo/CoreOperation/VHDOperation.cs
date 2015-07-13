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
            //ShouldContinue = true;
            NeedTwiceAttach = false;
            SetVhdProp();
        }
        //public string Win8VHDFileName { get; set; }


        //private bool ShouldContinue { get; set; }
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
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_LetterV", MsgManager.ci));
                er.ShowDialog();
            }
            //if (useiso) { ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\" + "\\isocmd.exe\" -eject 0: "); }
            try
            {
                //File.Delete ()

                FileOperation.DeleteFile(diskpartScriptPath + "\\create.txt");
                FileOperation.DeleteFile(diskpartScriptPath + "\\removex.txt");
                FileOperation.DeleteFile(diskpartScriptPath + "\\detach.txt");
                FileOperation.DeleteFile(diskpartScriptPath + "\\uefi.txt");
                FileOperation.DeleteFile(diskpartScriptPath + "\\uefimbr.txt");
                FileOperation.DeleteFile(diskpartScriptPath + "\\dp.txt");
                FileOperation.DeleteFile(diskpartScriptPath + "\\attach.txt");
                FileOperation.DeleteFile(Environment.GetEnvironmentVariable("TEMP") + "\\win8.vhd");
                FileOperation.DeleteFile(Environment.GetEnvironmentVariable("TEMP") + "\\win8.vhdx");
                FileOperation.DeleteFile(WTGOperation.userSettings.VHDTempPath + "\\win8.vhd");
                FileOperation.DeleteFile(WTGOperation.userSettings.VHDTempPath + "\\win8.vhdx");
            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_DeleteTempError")
                //程序删除临时文件出错！可重启程序或重启电脑重试！\n
                MessageBox.Show(MsgManager.GetResString("Msg_DeleteTempError", MsgManager.ci) + ex.ToString());
                Log.WriteLog("DeleteVHDTemp.log", ex.ToString());
                //shouldcontinue = false;
            }

        }
        public static void DetachVHD(string vhdPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=" + vhdPath);
            sb.AppendLine("detach vdisk");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

        }
        public void DetachVHD()
        {
            DetachVHD(this.VhdPath);
        }
        public static void DetachVHDExtra()
        {
            DiskpartScriptManager dsm = new DiskpartScriptManager(true);
            dsm.Args = "list vdisk";
            dsm.RunDiskpartScript();
            //MessageBox.Show(dsm.outputFilePath);
            string dpoutput = File.ReadAllText(dsm.OutputFilePath, Encoding.Default);
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
            //if (!this.ShouldContinue) return;
            //MsgManager.getResString("FileName_VHD_Dynamic")
            //VHD模式说明.TXT
            using (FileStream fs1 = new FileStream(ud + MsgManager.GetResString("FileName_VHD_Dynamic", MsgManager.ci), FileMode.Create, FileAccess.Write))
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
                    sw1.WriteLine(MsgManager.GetResString("Msg_VHDDynamicSize", MsgManager.ci) + this.VhdSize + "MB\n");
                    sw1.WriteLine(MsgManager.GetResString("Msg_VHDDynamicSize2", MsgManager.ci));
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
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            //GenerateAttachVHDScript(this.vhdPath);
            //ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\attach.txt\"");
        }
        public void CreateVHD()
        {

            //if (!ShouldContinue) return;
            //else ShouldContinue = false;
            StringBuilder sb = new StringBuilder();
            if (choosedFileType == "vhd")
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
                if (userSettings.VHDPartitionType == PartitionTableType.GPT)
                {
                    sb.AppendLine("convert gpt");
                }
                sb.AppendLine("create partition primary");
                sb.AppendLine("format fs=ntfs quick");
                sb.AppendLine("assign letter=v");
                sb.AppendLine("exit");
            }
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            Log.WriteLog("CreateVHDScript.log", sb.ToString());
            //MessageBox.Show(sb.ToString());
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            //try
            //{
            if (!Directory.Exists("V:\\"))
            {
                throw new VHDException(MsgManager.GetResString("Msg_VHDCreationError"));
                //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                //er.ShowDialog();
                //this.ShouldContinue = false;

                //shouldcontinue = false;
                //return;
            }
            //else
            //{
            //    //this.ShouldContinue = true;

            //}
            //}
            //catch
            //{
            //    throw new VHDException(MsgManager.GetResString("Msg_VHDCreationError"));
            //    //创建VHD失败，未知错误
            //    //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
            //    //er.ShowDialog();
            //    //this.ShouldContinue = false;
            //    //shouldcontinue = false;

            //}
            if (choosedFileType != "vhd" && choosedFileType != "vhdx")
            {
                ApplyToVdisk();
            }
            if (choosedFileType != "vhd" && choosedFileType != "vhdx" && !isWimBoot && !File.Exists(@"v:\windows\regedit.exe"))
            {
                throw new VHDException(MsgManager.GetResString("Msg_ApplyError"));
                //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_ApplyError", MsgManager.ci));
                //er.ShowDialog();
                //this.ShouldContinue = false;

            }
            //else
            //{
            //    this.ShouldContinue = true;
            //}
        }
        public void VHDExtra()
        {
            //if (!ShouldContinue) return;

            ImageOperation.ImageExtra(isFramework, isSan_policy, isDiswinre, @"v:\", imageFilePath);
            UEFIAndWin7ToGo();

        }
        public void CopyVHD()
        {
            //if (!ShouldContinue) return;

            if (NeedCopy)
            {
                if (File.Exists(VhdPath))
                {
                    //Application.DoEvents();
                    //Thread.Sleep(100);
                    //Application.DoEvents();
                    BigFileCopy(VhdPath, WTGOperation.ud + userSettings.VHDNameWithoutExt + "." + choosedFileType, 32);
                    //ProcessManager.ECMD(WTGOperation.applicationFilesPath+ "\\fastcopy.exe", " /auto_close \"" + this.VhdPath + "\" /to=\"" + WTGOperation.ud + "\"");
                    //BigFileCopy ()
                    //MsgManager.getResString("Msg_Copy")
                    //复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n
                    ProcessManager.AppendText(MsgManager.GetResString("Msg_Copy", MsgManager.ci));



                }
                if ((WTGOperation.choosedFileType == "vhd" && !this.VhdPath.EndsWith("win8.vhd")) || (WTGOperation.choosedFileType == "vhdx" && !this.VhdPath.EndsWith("win8.vhdx")))
                {
                    //Rename
                    //MsgManager.getResString("Msg_RenameError")
                    //重命名错误
                    try
                    {
                        File.Move(WTGOperation.ud + this.VhdPath.Substring(this.VhdPath.LastIndexOf("\\") + 1), WTGOperation.ud + WTGOperation.userSettings.VHDNameWithoutExt + "." + choosedFileType);
                    }
                    catch (Exception ex) { MessageBox.Show(MsgManager.GetResString("Msg_RenameError", MsgManager.ci) + ex.ToString()); }
                }
            }
        }
        public void WriteBootFiles()
        {
            //if (!this.ShouldContinue) return;
            if (!NeedCopy)//不需要拷贝，不需要二次加载
            {
                WriteBootFilesIntoVHD();
            }
            else
            {
                if (commonBootFiles && WTGOperation.userSettings.VHDNameWithoutExt == "win8")
                {
                    CopyVHDBootFiles();
                }
                else
                {
                    NeedTwiceAttach = true;
                }
            }
        }

        private void WriteBootFilesIntoVHD()
        {
            if (isUefiGpt)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", @"X:\", bcdbootFileName, FirmwareType.UEFI);
                //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");
            }
            else if (isUefiMbr)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", @"X:\", bcdbootFileName, FirmwareType.ALL);
                BootFileOperation.BooticeWriteMBRPBRAndAct("X:");
                //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");
            }
            else if (isWimBoot)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", ud, bcdbootFileName, FirmwareType.BIOS);
                //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");
            }
            else
            {
                if (!commonBootFiles)
                {
                    if (userSettings.NtfsUefiSupport)
                    {
                        BootFileOperation.BcdbootWriteBootFile(@"V:\", ud, bcdbootFileName, FirmwareType.ALL);

                    }
                    else
                    {
                        BootFileOperation.BcdbootWriteBootFile(@"V:\", ud, bcdbootFileName, FirmwareType.BIOS);
                    }
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

        public void Execute()
        {
            CleanTemp();
            try
            {
                if (choosedFileType == "vhd" || choosedFileType == "vhdx")
                {
                    CopyVHD();
                    if (commonBootFiles)
                    {
                        CopyVHDBootFiles();
                    }
                    else
                    {
                        TwiceAttachVHDAndWriteBootFile();
                    }
                }
                else
                {
                    CreateVHD();
                    VHDExtra();
                    WriteBootFiles();
                    DetachVHD();
                    CopyVHD();
                    TwiceAttachVHDAndWriteBootFile();
                    //BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
                    if (VhdType != "fixed") VHDDynamicSizeIns();
                }
            }
            catch (UserCancelException)
            {
                throw;
            }
            catch (VHDException ex)
            {
                ErrorMsg em = new ErrorMsg(ex.Message);
                em.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序出现严重错误！\n" + ex.ToString());
                Log.WriteLog("VHDFatalError.log", ex.ToString());
            }
        }

        private void TwiceAttachVHDAndWriteBootFile()
        {
            if (!NeedTwiceAttach) return;
            //if (!ShouldContinue) return;
            //AttachVHD(StringOperation.Combine(WTGOperation.ud, win8VhdFile));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=" + StringUtility.Combine(WTGOperation.ud, win8VHDFileName));
            sb.AppendLine("attach vdisk");
            sb.AppendLine("sel partition 1");
            sb.AppendLine("assign letter=v");
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            Log.WriteLog("TwiceAttachVhd.log", sb.ToString());

            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            try
            {
                if (!Directory.Exists("V:\\"))
                {
                    Log.WriteLog("TwiceAttachVhdError.log", "二次加载VHD失败");
                }

            }
            catch
            {
                throw new VHDException(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                //创建VHD失败，未知错误
                //ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                //er.ShowDialog();
                //this.ShouldContinue = false;
                //shouldcontinue = false;

            }
            //需要二次加载，一定不是UEFI模式

            BootFileOperation.BcdbootWriteBootFile(@"V:\", ud, bcdbootFileName, FirmwareType.ALL);
            DetachVHD(StringUtility.Combine(ud, win8VHDFileName));
            if (!File.Exists(ud + "\\Boot\\BCD"))
            {

                CopyVHDBootFiles();
                //Log.WriteLog ("TwiceAttachFailed.log")
            }

        }

        #region 私有方法
        private void SetVhdProp()
        {
            try
            {
                if (Path.GetExtension(imageFilePath) == ".vhd" || Path.GetExtension(imageFilePath) == ".vhdx")
                {
                    VhdType = string.Empty;
                    VhdSize = string.Empty;
                    VhdFileName = string.Empty;
                    ExtensionType = string.Empty;
                    VhdPath = imageFilePath;
                    NeedCopy = true;
                }
                else
                {
                    //    ////////////////vhd设定///////////////////////
                    //    string vhd_type = "expandable";
                    //    vhd_size = "";
                    if (isFixedVHD)
                    {
                        VhdType = "fixed";
                    }
                    else
                    {
                        VhdType = "expandable";
                    }
                    if (userSetSize != 0)
                    {
                        VhdSize = (userSetSize * 1024).ToString();
                    }
                    else
                    {
                        if (!isWimBoot)
                        {
                            if (DiskOperation.GetHardDiskFreeSpace(ud) / 1024 >= 21504) { VhdSize = "20480"; }
                            else { VhdSize = (DiskOperation.GetHardDiskFreeSpace(ud) / 1024 - 500).ToString(); }
                        }
                        else
                        {
                            if (DiskOperation.GetHardDiskFreeSpace(ud) / 1024 >= 24576) { VhdSize = "20480"; }
                            else { VhdSize = (DiskOperation.GetHardDiskFreeSpace(ud) / 1024 - 4096).ToString(); }
                        }
                    }

                    //Win8VHDFileName = userSettings.VHDNameWithoutExt + "." + choosedFileType;


                    ////////////////判断临时文件夹,VHD needcopy?///////////////////
                    int vhdmaxsize;
                    if (isFixedVHD)
                    {
                        vhdmaxsize = int.Parse(VhdSize) * 1024 + 1024;
                    }
                    else
                    {
                        vhdmaxsize = 10485670;
                    }

                    if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.userSettings.VHDTempPath.Substring(0, 2) + "\\") <= vhdmaxsize || StringUtility.IsChina(WTGOperation.userSettings.VHDTempPath) || isUefiGpt || isUefiMbr || isWimBoot || isNoTemp)
                    {
                        this.NeedCopy = false;
                        this.VhdPath = StringUtility.Combine(WTGOperation.ud, win8VHDFileName);
                    }
                    else
                    {
                        this.NeedCopy = true;
                        this.VhdPath = StringUtility.Combine(WTGOperation.userSettings.VHDTempPath, win8VHDFileName);
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
            }
            catch (Exception ex)
            {
                Log.WriteLog("SetVhdProp.log", ex.ToString());
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
            }
        }

        private void ApplyToVdisk()
        {

            ImageOperation.AutoChooseWimIndex(ref wimPart, WTGOperation.win7togo);
            ImageOperation.ImageApply(isWimBoot, WTGOperation.isEsd, WTGOperation.imagexFileName, WTGOperation.imageFilePath, WTGOperation.wimPart, @"v:\", WTGOperation.ud);

        }
        /// <summary>
        /// WIN7注册表和FixLetter 。所有VHD都需要FixLetter
        /// </summary>
        private void UEFIAndWin7ToGo()
        {
            if (WTGOperation.win7togo != 0)
            {
                ImageOperation.Win7REG("V:\\");
            }
            //////////////
            //if (isuefigpt)
            //{
            if (userSettings.FixLetter)
            {
                ImageOperation.Fixletter("C:", "V:");

            }
            //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
            //}
        }

        private void CopyVHDBootFiles()
        {
            //this.CopyVHD();
            //if (!needcopyvhdbootfile) return;
            ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + " /e /h /y");

            if (this.ExtensionType == "vhdx")
            {
                ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");
            }
            Log.WriteLog("CopyVHDBootFiles.log", "xcopy.exe" + "\"" + WTGOperation.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");
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

    [Serializable]
    public class VHDException : Exception
    {
        public VHDException() { }
        public VHDException(string message) : base(message) { }
        public VHDException(string message, Exception inner) : base(message, inner) { }
        protected VHDException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
