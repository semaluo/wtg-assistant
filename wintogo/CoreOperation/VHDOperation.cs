using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wintogo
{
    public class VHDOperation
    {
        public VHDOperation()
        {
            //ShouldContinue = true;
            NeedTwiceAttach = false;
            SetVhdProp();
        }
        private bool NeedTwiceAttach { get; set; }
        //public string WTGModel.diskpartScriptPath { get; set; }
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

                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\create.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\removex.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\detach.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\uefi.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\uefimbr.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\dp.txt");
                FileOperation.DeleteFile(WTGModel.diskpartScriptPath + "\\attach.txt");
                FileOperation.DeleteFile(Environment.GetEnvironmentVariable("TEMP") + "\\win8.vhd");
                FileOperation.DeleteFile(Environment.GetEnvironmentVariable("TEMP") + "\\win8.vhdx");
                FileOperation.DeleteFile(WTGModel.userSettings.VHDTempPath + "\\win8.vhd");
                FileOperation.DeleteFile(WTGModel.userSettings.VHDTempPath + "\\win8.vhdx");
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
            sb.AppendLine("select vdisk file=\"" + vhdPath + "\"");
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
            using (FileStream fs1 = new FileStream(WTGModel.ud + MsgManager.GetResString("FileName_VHD_Dynamic", MsgManager.ci), FileMode.Create, FileAccess.Write))
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

            sb.AppendLine("select vdisk file=\"" + vhdPath + "\"");
            sb.AppendLine("attach vdisk");
            sb.AppendLine("sel partition 1");
            sb.AppendLine("assign letter=v");
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            //GenerateAttachVHDScript(this.vhdPath);
            //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGModel.diskpartScriptPath + "\\attach.txt\"");
        }
        public void CreateVHD()
        {

            //if (!ShouldContinue) return;
            //else ShouldContinue = false;
            StringBuilder sb = new StringBuilder();
            if (WTGModel.choosedImageType == "vhd")
            {
                //sb.AppendLine("create vdisk file=" + this.VhdPath + " type=" + this.VhdType + " maximum=" + this.VhdSize);
                sb.AppendLine("select vdisk file=\"" + VhdPath + "\"");
                sb.AppendLine("attach vdisk");
                sb.AppendLine("assign letter=v");
                sb.AppendLine("exit");
            }
            else
            {
                sb.AppendLine("create vdisk file=\"" + this.VhdPath + "\" type=" + this.VhdType + " maximum=" + this.VhdSize);
                sb.AppendLine("select vdisk file=\"" + this.VhdPath + "\"");
                sb.AppendLine("attach vdisk");
                if (WTGModel.userSettings.VHDPartitionType == PartitionTableType.GPT)
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

            }

            if (WTGModel.choosedImageType != "vhd" && WTGModel.choosedImageType != "vhdx")
            {
                ApplyToVdisk();
            }
            if (WTGModel.choosedImageType != "vhd" && WTGModel.choosedImageType != "vhdx" && !WTGModel.isWimBoot && !File.Exists(@"v:\windows\regedit.exe"))
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

            ImageOperation.ImageExtra(WTGModel.userSettings.InstallDonet35, WTGModel.isSan_policy, WTGModel.userSettings.DisableWinRe, @"v:\", WTGModel.imageFilePath);
            UEFIAndWin7ToGo();

        }
        public void CopyVHD()
        {
            //if (!ShouldContinue) return;

            if (NeedCopy)
            {
                if (File.Exists(VhdPath))
                {
                    //Application.DoEvent                    
                    //s();
                    //Thread.Sleep(100);
                    //Application.DoEvents();
                    //BigFileCopy(VhdPath, WTGModel.ud + WTGModel.userSettings.VHDNameWithoutExt + "." + WTGModel.vhdExtension, 32);
                    //ProcessManager.AppendText(MsgManager.GetResString("Msg_Copy", MsgManager.ci));
                    ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\fastcopy.exe", " /auto_close \"" + this.VhdPath + "\" /to=\"" + WTGModel.ud + "\"", MsgManager.GetResString("Msg_Copy", MsgManager.ci));

                    //BigFileCopy ()
                    //MsgManager.getResString("Msg_Copy")
                    //复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n



                }
                if ((WTGModel.choosedImageType == "vhd" && !this.VhdPath.EndsWith("win8.vhd")) || (WTGModel.choosedImageType == "vhdx" && !this.VhdPath.EndsWith("win8.vhdx")))
                {
                    //Rename
                    //MsgManager.getResString("Msg_RenameError")
                    //重命名错误
                    try
                    {
                        File.Move(WTGModel.ud + this.VhdPath.Substring(this.VhdPath.LastIndexOf("\\") + 1), WTGModel.ud + WTGModel.userSettings.VHDNameWithoutExt + "." + WTGModel.choosedImageType);
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
                if (WTGModel.userSettings.CommonBootFiles && WTGModel.userSettings.VHDNameWithoutExt == "win8")
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
            if (WTGModel.isUefiGpt && !WTGModel.isLegacyUdiskUefi)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", @"X:\", FirmwareType.UEFI);
                //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");
            }
            else if (WTGModel.isUefiMbr)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", @"X:\", FirmwareType.ALL);
                BootFileOperation.BooticeWriteMBRPBRAndAct("X:");
                //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");
            }
            else if (WTGModel.isWimBoot)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.BIOS);
                //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.WTGModel.ud.Substring(0, 2) + " /f ALL");
            }
            else
            {
                if (!WTGModel.userSettings.CommonBootFiles)
                {
                    if (WTGModel.ntfsUefiSupport)
                    {
                        BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.ALL);

                    }
                    else
                    {
                        BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.BIOS);
                    }
                    //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.WTGModel.ud.Substring(0, 2) + " /f ALL");
                }
                else
                {
                    CopyVHDBootFiles();

                }
            }
        }

        public void Execute()
        {
            CleanTemp();
            try
            {
                if (WTGModel.choosedImageType == "vhd" || WTGModel.choosedImageType == "vhdx")
                {
                    CopyVHD();
                    if (WTGModel.userSettings.CommonBootFiles)
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
                    //BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.WTGModel.ud);
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
            //AttachVHD(StringOperation.Combine(WTGOperation.WTGModel.ud, win8VhdFile));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select vdisk file=" + StringUtility.Combine(WTGModel.ud, WTGModel.win8VHDFileName));
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
            if (WTGModel.ntfsUefiSupport)
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.ALL);
            }
            else
            {
                BootFileOperation.BcdbootWriteBootFile(@"V:\", WTGModel.ud, FirmwareType.BIOS);
            }
            DetachVHD(StringUtility.Combine(WTGModel.ud, WTGModel.win8VHDFileName));
            if (!File.Exists(WTGModel.ud + "\\Boot\\BCD"))
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
                if (Path.GetExtension(WTGModel.imageFilePath) == ".vhd" || Path.GetExtension(WTGModel.imageFilePath) == ".vhdx")
                {
                    VhdType = string.Empty;
                    VhdSize = string.Empty;
                    VhdFileName = string.Empty;
                    ExtensionType = string.Empty;
                    VhdPath = WTGModel.imageFilePath;
                    NeedCopy = true;
                }
                else
                {
                    //    ////////////////vhd设定///////////////////////
                    //    string vhd_type = "expandable";
                    //    vhd_size = "";
                    if (WTGModel.isFixedVHD)
                    {
                        VhdType = "fixed";
                    }
                    else
                    {
                        VhdType = "expandable";
                    }
                    if (WTGModel.userSetSize != 0)
                    {
                        VhdSize = (WTGModel.userSetSize * 1024).ToString();
                    }
                    else
                    {
                        long hardDiskFreeSpace = (long)WTGModel.UdObj.Size / 1048576;
                        //MessageBox.Show(hardDiskFreeSpace.ToString());
                        if (!WTGModel.isWimBoot)
                        {
                            if (hardDiskFreeSpace >= 21504) { VhdSize = "20480"; }
                            else
                            {
                                VhdSize = hardDiskFreeSpace == 0 ? "20480" : (hardDiskFreeSpace - 500).ToString();
                            }
                        }
                        else
                        {
                            if (hardDiskFreeSpace >= 24576) { VhdSize = "20480"; }
                            else { VhdSize = hardDiskFreeSpace == 0 ? "20480" : (hardDiskFreeSpace - 4096).ToString(); }
                        }
                    }

                    //Win8VHDFileName = userSettings.VHDNameWithoutExt + "." + choosedFileType;


                    ////////////////判断临时文件夹,VHD needcopy?///////////////////
                    int vhdmaxsize;
                    if (WTGModel.isFixedVHD)
                    {
                        vhdmaxsize = int.Parse(VhdSize) * 1024 + 1024;
                    }
                    else
                    {
                        vhdmaxsize = 10485670;
                    }

                    if (DiskOperation.GetHardDiskFreeSpace(WTGModel.userSettings.VHDTempPath.Substring(0, 2) + "\\") <= vhdmaxsize || StringUtility.IsChina(WTGModel.userSettings.VHDTempPath) || (WTGModel.isUefiGpt && !WTGModel.isLegacyUdiskUefi) || WTGModel.isUefiMbr || WTGModel.isWimBoot || WTGModel.isNoTemp)
                    {
                        this.NeedCopy = false;
                        this.VhdPath = StringUtility.Combine(WTGModel.ud, WTGModel.win8VHDFileName);
                    }
                    else
                    {
                        this.NeedCopy = true;
                        this.VhdPath = StringUtility.Combine(WTGModel.userSettings.VHDTempPath, WTGModel.win8VHDFileName);
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

            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            ImageOperation.ImageApply(WTGModel.isWimBoot, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, @"v:\", WTGModel.ud);

        }
        /// <summary>
        /// WIN7注册表和FixLetter 。所有VHD都需要FixLetter
        /// </summary>
        private void UEFIAndWin7ToGo()
        {
            if (WTGModel.win7togo != 0)
            {
                ImageOperation.Win7REG("V:\\");
            }
            //////////////
            //if (isuefigpt)
            //{
            if (WTGModel.userSettings.FixLetter)
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
            ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + " /e /h /y");

            if (this.ExtensionType == "vhdx")
            {
                ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + "\\boot\\ /e /h /y");
            }
            Log.WriteLog("CopyVHDBootFiles.log", "xcopy.exe" + "\"" + WTGModel.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + "\\boot\\ /e /h /y");
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
