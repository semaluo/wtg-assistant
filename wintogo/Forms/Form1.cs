using iTuner;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using wintogo.Forms;

namespace wintogo
{
    public partial class Form1 : Form
    {

        string currentList;//当前优盘列表
        private UsbDiskCollection diskCollection = new UsbDiskCollection();

        bool autoCheckUpdate = true;
        private Thread tWrite;
        private Thread tListUDisks;
        Stopwatch writeSw = new Stopwatch();
        private int udSizeInMB = 0;
        private readonly string releaseUrl = "http://bbs.luobotou.org/app/wintogo.txt";
        private readonly string reportUrl = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=";
        private readonly string settingFilePath = Application.StartupPath + "\\settings.ini";


        public Form1()
        {
            ReadConfigFile();

            InitializeComponent();
            txtVhdTempPath.Text = WTGModel.vhdTempPath;
        }



        private void FileValidation()
        {
            if (StringUtility.IsChina(WTGModel.diskpartScriptPath))
            {
                if (StringUtility.IsChina(Application.StartupPath))
                {
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("IsChinaMsg", MsgManager.ci));
                    er.ShowDialog();
                    Environment.Exit(0);
                }
                else
                {
                    WTGModel.diskpartScriptPath = WTGModel.logPath;
                }
            }
            ProcessManager.SyncCMD("taskkill /f /IM BOOTICE.exe");
            //ProcessManager.KillProcessByName("bootice.exe");
            //解压文件
            UnZipClass.UnZip(Application.StartupPath + "\\files.dat", WTGModel.applicationFilesPath);
        }

        private void SystemDetection()
        {
            WTGModel.bcdbootFileName = "bcdboot.exe";
            string osVersionStr = Environment.OSVersion.ToString();
            if (osVersionStr.Contains("5.1")) //XP 禁用功能
            {
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
                radiobtnLegacy.Checked = true;
                groupBoxAdv.Enabled = false;
                checkBoxDiskpart.Checked = false;
                checkBoxDiskpart.Enabled = false;
                //bcdboot9200.Checked = false;
                //bcdboot7601.Checked = true;
                //WTGModel.bcdbootFileName = "bcdboot7601.exe";
                labelDisFuncEM.Visible = true;
                labelDisFunc.Visible = true;
                WTGModel.CurrentOS = OS.XP;
            }
            else if (osVersionStr.Contains("6.0")) //vista
            {
                radiobtnVhd.Enabled = false;
                radiobtnVhdx.Enabled = false;
                radiobtnLegacy.Checked = true;
                groupBoxAdv.Enabled = false;
                checkBoxDiskpart.Checked = false;
                checkBoxDiskpart.Enabled = false;
                //bcdboot9200.Checked = false;
                //bcdboot7601.Checked = true;
                //WTGModel.bcdbootFileName = "bcdboot7601.exe";
                labelDisFuncEM.Visible = true;
                labelDisFunc.Visible = true;
                WTGModel.CurrentOS = OS.Vista;

            }
            else if (osVersionStr.Contains("6.1"))//WIN7
            {
                labelDisFuncEM.Visible = true;
                labelDisFunc.Visible = true;
                radiobtnVhd.Checked = true;
                radiobtnVhdx.Enabled = false;
                WTGModel.CurrentOS = OS.Win7;

            }
            else if (osVersionStr.Contains("6.2") || osVersionStr.Contains("6.3"))//WIN8及以上
            {
                radiobtnVhd.Checked = true;
                //WIN8.1 UPDATE1 WIMBOOT  已修复WIN10版本号问题
                string dismversion = FileOperation.GetFileVersion(System.Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe");
                if (dismversion.Substring(0, 14) == "6.3.9600.17031" || dismversion.Substring(0, 3) == "6.4")
                {
                    checkBoxWimboot.Enabled = true;
                    WTGModel.allowEsd = true;
                    labelDisFunc.Visible = true;
                    labelDisFuncEM.Visible = true;
                    WTGModel.CurrentOS = OS.Win8_1_with_update;

                }
                else if (dismversion.Substring(0, 3) == "10.")
                {
                    checkBoxWimboot.Enabled = true;
                    checkBoxCompactOS.Enabled = true;
                    WTGModel.allowEsd = true;
                    WTGModel.CurrentOS = OS.Win10;

                }
                else
                {
                    WTGModel.CurrentOS = OS.Win8_without_update;
                }
            }

        }

        private void ReadConfigFile()
        {
            string autoup;
            string tp;
            string language;
            autoup = IniFile.ReadVal("Main", "AutoUpdate", settingFilePath);
            tp = IniFile.ReadVal("Main", "TempPath", settingFilePath);
            language = IniFile.ReadVal("Main", "Language", settingFilePath);
            if (autoup == "0") { autoCheckUpdate = false; }
            if (!string.IsNullOrEmpty(tp))
            {
                WTGModel.vhdTempPath = tp;
            }
            else
            {
                WTGModel.vhdTempPath = Path.GetTempPath();
            }
            if (language == "EN")
            {
                MsgManager.ci = new System.Globalization.CultureInfo("en");

                Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            }
            else if (language == "ZH-HANS")
            {
                MsgManager.ci = new System.Globalization.CultureInfo("zh-cn");

                Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            }
            else if (language == "ZH-HANT")
            {
                MsgManager.ci = new System.Globalization.CultureInfo("zh-Hant");

                Thread.CurrentThread.CurrentUICulture = MsgManager.ci;

            }
        }

        #region Udisk
        public delegate void OutDelegate(bool isend, object dtSource);
        public void OutText(bool isend, object dtSource)
        {
            //MessageBox.Show("Test");
            if (comboBoxUd.InvokeRequired)
            {
                OutDelegate outdelegate = new OutDelegate(OutText);
                BeginInvoke(outdelegate, new object[] { isend, dtSource });
                return;
            }
            comboBoxUd.DataSource = null;
            comboBoxUd.DataSource = dtSource;
            if (comboBoxUd.Items.Count != 0)
            {
                comboBoxUd.SelectedIndex = 0;
            }
            if (isend)
            {
                comboBoxUd.SelectedIndex = comboBoxUd.Items.Count - 1;
            }
        }

        private void GetUdiskInfo()
        {

            string newlist = string.Empty;
            UsbManager manager = new UsbManager();
            try
            {
                diskCollection.Clear();
                //UsbDiskCollection disks = manager.GetAvailableDisks();
                UsbDisk udChoose = new UsbDisk(MsgManager.GetResString("Msg_chooseud", MsgManager.ci));
                diskCollection.Add(udChoose);

                //if (disks == null) { return; }
                foreach (UsbDisk disk in manager.GetAvailableDisks())
                {
                    diskCollection.Add(disk);
                    newlist += disk.ToString();

                }
                if (newlist != currentList)
                {

                    currentList = newlist;

                    OutText(false, diskCollection);
                }
            }
            catch (Exception ex) { Log.WriteLog("GetUdiskInfo.log", ex.ToString()); }
            finally
            {

                manager.Dispose();
            }

        }

        private void LoadUDList()
        {
            //Udlist = new Udlist();
            tListUDisks = new Thread(GetUdiskInfo);
            tListUDisks.Start();

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0)
            {
                LoadUDList();

            }
        }
        #endregion

        #region CORE

        private void GoWrite()
        {

            try
            {
                //wimpart = ChoosePart.part;//读取选择分卷，默认选择第一分卷
                #region 各种提示
                //各种提示
                if (lblWim.Text.ToLower().Substring(lblWim.Text.Length - 3, 3) != "wim" && lblWim.Text.ToLower().Substring(lblWim.Text.Length - 3, 3) != "esd" && lblWim.Text.ToLower().Substring(lblWim.Text.Length - 3, 3) != "vhd" && lblWim.Text.ToLower().Substring(lblWim.Text.Length - 4, 4) != "vhdx")//不是WIM文件
                {
                    //镜像文件选择错误！请选择install.wim！
                    MessageBox.Show(MsgManager.GetResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    //请选择install.wim文件
                    if (!File.Exists(lblWim.Text))
                    {
                        MessageBox.Show(MsgManager.GetResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }//文件不存在.
                    WTGModel.imageFilePath = lblWim.Text;
                }


                if (!Directory.Exists(WTGModel.ud))
                {
                    MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci) + "!", MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }//是否选择优盘
                if (DiskOperation.GetHardDiskSpace(WTGModel.ud) <= 12582912) //优盘容量<12 GB提示
                {
                    //MsgManager.getResString("Msg_DiskSpaceWarning") 
                    //可移动磁盘容量不足16G，继续写入可能会导致程序出错！您确定要继续吗？
                    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_DiskSpaceWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                    {
                        return;
                    }
                }


                if (DiskOperation.GetHardDiskSpace(WTGModel.ud) <= numericUpDown1.Value * 1048576)
                {
                    //优盘容量小于VHD设定大小，请修改设置！
                    //MsgManager.getResString("Msg_DiskSpaceError")
                    MessageBox.Show(MsgManager.GetResString("Msg_DiskSpaceError", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //MsgManager.getResString("Msg_ConfirmChoose")
                //请确认您所选择的
                //MsgManager.getResString("Msg_Disk_Space") 盘，容量
                //Msg_FormatTip

                //GB 是将要写入的优盘或移动硬盘\n误格式化，后果自负！
                StringBuilder formatTip = new StringBuilder();
                formatTip.AppendLine(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci));
                formatTip.AppendFormat(WTGModel.udString);
                formatTip.AppendLine(MsgManager.GetResString("Msg_FormatTip", MsgManager.ci));
                if (checkBoxDiskpart.Checked && !checkBoxUefigpt.Checked && !checkBoxUefimbr.Checked)//勾选重新分区提示
                {
                    formatTip.AppendLine(MsgManager.GetResString("Msg_Repartition", MsgManager.ci));
                    FormatAlert fa = new FormatAlert(formatTip.ToString());
                    //MsgManager.getResString("Msg_Repartition")
                    //您勾选了重新分区，优盘或移动硬盘上的所有文件将被删除！\n注意是整个磁盘，不是一个分区！
                    if (DialogResult.Yes != fa.ShowDialog())
                    {
                        return;
                    }

                    DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);
                }
                else//普通格式化提示
                {
                    if (!WTGModel.doNotFormat)
                    {
                        formatTip.AppendLine(MsgManager.GetResString("Msg_FormatWarning", MsgManager.ci));
                        FormatAlert fa = new FormatAlert(formatTip.ToString());
                        if (DialogResult.Yes != fa.ShowDialog())
                        {
                            return;
                        }
                    }
                    else
                    {
                        FormatAlert fa = new FormatAlert(formatTip.ToString());
                        if (DialogResult.Yes != fa.ShowDialog())
                        {
                            return;
                        }
                    }
                }
                #endregion

                SystemSleepManagement.PreventSleep();

                //删除旧LOG文件
                VHDOperation.CleanTemp();
                Log.DeleteAllLogs();
                ProcessManager.KillProcessByName("bootice.exe");
                WriteProgramRunInfoToLog();

                writeSw.Restart();

                if (checkBoxUefigpt.Checked)
                {
                    //UEFI+GPT
                    if (Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2"))
                    {
                        //MsgManager.getResString("Msg_XPUefiError")
                        //XP系统不支持UEFI模式写入
                        MessageBox.Show(MsgManager.GetResString("Msg_XPUefiError", MsgManager.ci)); return;
                    }
                    if (WTGModel.udString.Contains("Removable Disk"))
                    {
                        //普通优盘UEFI
                        WTGModel.isLegacyUdiskUefi = true;
                        RemoveableDiskUefiGpt();
                        FinishSuccessful();


                    }
                    else
                    {
                        //MsgManager.getResString("Msg_UefiFormatWarning")
                        //您所选择的是UEFI模式，此模式将会格式化您的整个移动磁盘！\n注意是整个磁盘！！！\n程序将会删除所有优盘分区！
                        //if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_UefiFormatWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }



                        DiskOperation.DiskPartGPTAndUEFI(WTGModel.efiPartitionSize.ToString(), WTGModel.ud, WTGModel.partitionSize);

                        //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\uefi.txt\"");
                        if (radiobtnLegacy.Checked)
                        {
                            //UEFI+GPT 传统
                            UefiGptTypical();


                        }
                        else // UEFI+GPT VHD、VHDX模式
                        {

                            UefiGptVhdVhdx();
                        }
                    }
                }
                else if (checkBoxUefimbr.Checked)
                {
                    //UEFI+MBR
                    if (WTGModel.udString.Contains("Removable Disk"))
                    {
                        MessageBox.Show(MsgManager.GetResString("Msg_UefiError", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        VisitWeb("http://bbs.luobotou.org/thread-6506-1-1.html");
                        return;
                    }
                    //if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_UefiFormatWarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                    DiskpartScriptManager dsm = new DiskpartScriptManager();
                    DiskOperation.GenerateMBRAndUEFIScript(WTGModel.efiPartitionSize.ToString(), WTGModel.ud, WTGModel.partitionSize);

                    //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\uefimbr.txt\"");

                    if (radiobtnLegacy.Checked)
                    {
                        UEFIMBRTypical();

                    }
                    else //uefi MBR VHD、VHDX模式
                    {
                        UefiMbrVHDVHDX();
                    }

                    //MessageBox.Show("UEFI模式写入完成！\n请重启电脑用优盘启动\n如有问题，可去论坛反馈！", "完成啦！", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else //非UEFI模式
                {
                    //传统
                    #region 格式化
                    if (WTGModel.udString.Contains("Removable Disk") && radiobtnLegacy.Checked)
                    {
                        if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_Legacywarning", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                    }

                    if (!checkBoxDiskpart.Checked && !WTGModel.doNotFormat)//普通格式化
                    {
                        ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");
                        //
                    }
                    #endregion
                    ///////////////////////////////////正式开始////////////////////////////////////////////////
                    if (radiobtnLegacy.Checked)
                    {
                        NonUEFITypical(false);

                    }
                    else //非UEFI VHD VHDX
                    {
                        NonUEFIVHDVHDX(false);
                    }
                }

            }
            catch (UserCancelException ex)
            {
                ErrorMsg em = new ErrorMsg(ex.Message);
                em.ShowDialog();
            }
            catch (Exception ex)
            {

                ErrorMsg em = new ErrorMsg(ex.Message);
                em.ShowDialog();
            }
            finally
            {
                writeSw.Stop();
                SystemSleepManagement.ResotreSleep();
            }
        }
        private void TakeOwn(string sSourcePath)
        {
            //在指定目录及子目录下查找文件,在list中列出子目录及文件
            DirectoryInfo Dir = new DirectoryInfo(sSourcePath);
            DirectoryInfo[] DirSub = Dir.GetDirectories();
            if (DirSub.Length <= 0)
            {
                foreach (FileInfo f in Dir.GetFiles("*.*", SearchOption.TopDirectoryOnly)) //查找文件
                {
                    TakeOwn(f);

                }
            }
            int t = 1;
            foreach (DirectoryInfo d in DirSub)//查找子目录 
            {
                TakeOwn(Dir + @"\" + d.ToString());
                TakeOwn(d);
                if (t == 1)
                {
                    foreach (FileInfo f in Dir.GetFiles("*.*", SearchOption.TopDirectoryOnly)) //查找文件
                    {
                        TakeOwn(f);
                    }
                    t = t + 1;
                }
            }
        }
        private void TakeOwn(DirectoryInfo di)
        {
            DirectorySecurity dirSecurity = di.GetAccessControl();
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            di.SetAccessControl(dirSecurity);
        }
        private void TakeOwn(FileInfo fi)
        {
            FileSecurity fileSecurity = fi.GetAccessControl();
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            fi.SetAccessControl(fileSecurity);
        }
        #region 七种写入模式
        private void RemoveableDiskUefiGpt()
        {
            string tempFileName = WTGModel.diskpartScriptPath + "\\" + Guid.NewGuid().ToString() + ".txt";
            Process diskInfo = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", " /diskinfo /find: /usbonly /file=" + tempFileName);
            diskInfo.WaitForExit();

            string tempUdiskInfo = File.ReadAllText(tempFileName);
            Match match = Regex.Match(tempUdiskInfo, @"SET DRIVE([0-9])DESC=(.+)\r\nSET DRIVE([0-9])SIZE=(.+)\r\nSET DRIVE([0-9])LETTER=" + WTGModel.ud.Substring(0, 2).ToUpper(), RegexOptions.ECMAScript);
            string UdiskNumber = match.Groups[1].Value;
            if (DialogResult.No == MessageBox.Show(match.Groups[2].Value + "\n" + MsgManager.GetResString("Msg_TwiceConfirm", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
            //59055800320
            long udiskSize = long.Parse(match.Groups[4].Value);
            string dptFile = string.Empty;
            if (udiskSize > 118111600640)
            {
                dptFile = "110G";
            }
            else if (udiskSize > 59055800320)
            {
                dptFile = "55G";
            }
            else if (udiskSize > 28991029248)
            {
                dptFile = "27G";
            }
            else if (udiskSize > 13958643712)
            {
                dptFile = "13G";
            }
            else
            {
                throw new NotSupportedException("Your Usb Key is not suppotred");
            }
            //MessageBox.Show(WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt");
            Process p1 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt");
            p1.WaitForExit();
            ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");
            WTGModel.ntfsUefiSupport = true;

            if (radiobtnLegacy.Checked)
            {
                NonUEFITypical(true);

            }
            else //非UEFI VHD VHDX
            {
                NonUEFIVHDVHDX(true);
            }
            ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.ud.Substring(0, 2) + "\\EFI\\*.*" + "\"" + " \"" + WTGModel.diskpartScriptPath + "\\EFI\\\" /e /h /y");

            Process p2 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-2.dpt");
            p2.WaitForExit();
            ProcessManager.ECMD("cmd.exe", "/c format " + WTGModel.ud.Substring(0, 2) + "/FS:fat /q /V: /Y");
            ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.diskpartScriptPath + "\\EFI\\*.*" + "\"" + " \"" + WTGModel.ud.Substring(0, 2) + "\\EFI\\\" /e /h /y");

            Process p3 = Process.Start(WTGModel.applicationFilesPath + "\\bootice.exe", "/DEVICE=" + UdiskNumber + " /partitions  /quiet /restore_dpt=" + WTGModel.applicationFilesPath + "\\DPTs\\" + dptFile + "-1.dpt");
            p3.WaitForExit();
            ////takeown /f e:\boot /r
            //ProcessManager.SyncCMD("takeown.exe /f " + WTGModel.ud.Substring(0, 2) + "\\EFI /r");
            //ProcessManager.SyncCMD("takeown.exe /f " + WTGModel.ud.Substring(0, 2) + "\\Boot /r");

            //ProcessManager.SyncCMD("cacls.exe " + WTGModel.ud.Substring(0, 2) + "\\EFI /t /e /c /g everyone:f");
            //ProcessManager.SyncCMD("cacls.exe " + WTGModel.ud.Substring(0, 2) + "\\Boot /t /e /c /g everyone:f");
            //FileOperation.DeleteFolder(WTGModel.ud.Substring(0, 2) + "\\EFI");
            //FileOperation.DeleteFolder(WTGModel.ud.Substring(0, 2) + "\\Boot");



        }



        private void UefiGptTypical()
        {
            ImageOperation io = new ImageOperation();
            io.imageX = WTGModel.imagexFileName;
            io.imageFile = WTGModel.imageFilePath;
            io.AutoChooseWimIndex();
            io.ImageApplyToUD();
            ImageOperation.ImageExtra(WTGModel.installDonet35, checkBoxSan_policy.Checked, WTGModel.disableWinRe, WTGModel.ud, lblWim.Text);
            BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, @"X:\", FirmwareType.UEFI);
            RemoveLetterX();
            FinishSuccessful();
        }

        private void UefiGptVhdVhdx()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();

            RemoveLetterX();

            if (File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
            {
                FinishSuccessful();
            }
            else
            {

                //VHD文件创建出错！
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
                //MessageBox.Show("Win8 VHD文件不存在！，可到论坛发帖求助！\n建议将程序目录下logs文件夹打包上传，谢谢！","出错啦！",MessageBoxButtons .OK ,MessageBoxIcon.Error );

            }
        }

        private void NonUEFIVHDVHDX(bool legacyUdiskUefi)
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();
            if (!legacyUdiskUefi)
            {
                BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
            }
            if (!File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
            {
                //MsgManager.getResString("Msg_VHDCreationError")
                //Win8 VHD文件不存在！未知错误原因！
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
            }

            else if (!File.Exists(WTGModel.ud + "\\Boot\\BCD"))
            {
                //MsgManager.getResString("Msg_VHDBcdbootError")
                //VHD模式下BCDBOOT执行出错！
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDBcdbootError", MsgManager.ci));
                er.ShowDialog();
            }
            else if (!File.Exists(WTGModel.ud + "bootmgr"))
            {
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci));
                er.ShowDialog();
                //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
            }
            else
            {
                if (!legacyUdiskUefi)
                {
                    FinishSuccessful();
                }
            }

        }
        private void NonUEFITypical(bool legacyUdiskUefi)
        {
            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            ImageOperation.ImageApply(checkBoxWimboot.Checked, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);
            if (WTGModel.win7togo != 0)
            {
                ImageOperation.Win7REG(WTGModel.ud);
            }
            if (WTGModel.win7togo == 0)
            {
                ImageOperation.ImageExtra(WTGModel.installDonet35, checkBoxSan_policy.Checked, WTGModel.disableWinRe, WTGModel.ud, lblWim.Text);
            }

            if (!legacyUdiskUefi)
            {
                BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
                ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\" + WTGModel.bcdbootFileName, WTGModel.ud.Substring(0, 3) + "windows  /s  " + WTGModel.ud.Substring(0, 2) + " /f ALL");


                if (!System.IO.File.Exists(WTGModel.ud + "bootmgr"))
                {
                    //MsgManager.getResString("Msg_bootmgrError")
                    //文件写入出错！bootmgr不存在！\n请检查写入过程是否中断
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_bootmgrError", MsgManager.ci));
                    er.ShowDialog();

                    //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
                }
                else if (!System.IO.File.Exists(WTGModel.ud + "\\Boot\\BCD"))
                {
                    //MsgManager.getResString("Msg_BCDError")
                    //引导文件写入出错！boot文件夹不存在！
                    ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_BCDError", MsgManager.ci));
                    er.ShowDialog();
                    //MessageBox.Show("引导文件写入出错！boot文件夹不存在\n请看论坛教程！", "出错啦", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-1625-1-1.html");
                }
                else
                {
                    FinishSuccessful();
                }
            }
        }


        private void UefiMbrVHDVHDX()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();

            RemoveLetterX();

            if (System.IO.File.Exists(WTGModel.ud + WTGModel.win8VHDFileName))
            {
                FinishSuccessful();
                //finish f = new finish();
                //f.ShowDialog();
            }
            else
            {
                ErrorMsg er = new ErrorMsg(MsgManager.GetResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
                //shouldcontinue = false;
            }
            //removeLetterX();
            //Finish f = new Finish();
            //f.ShowDialog();
        }

        private void FinishSuccessful()
        {
            if (WTGModel.noDefaultDriveLetter && !WTGModel.udString.Contains("Removable Disk"))
            {
                DiskOperation.SetNoDefaultDriveLetter(WTGModel.ud);
            }
            writeSw.Stop();

            Finish f = new Finish(writeSw.Elapsed);
            f.ShowDialog();
        }

        private void UEFIMBRTypical()
        {
            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            //IMAGEX解压
            ImageOperation.ImageApply(checkBoxWimboot.Checked, WTGModel.isEsd, WTGModel.imagexFileName, WTGModel.imageFilePath, WTGModel.wimPart, WTGModel.ud, WTGModel.ud);

            //安装EXTRA
            ImageOperation.ImageExtra(WTGModel.installDonet35, checkBoxSan_policy.Checked, WTGModel.disableWinRe, WTGModel.ud, lblWim.Text);
            //BCDBOOT WRITE BOOT FILE  
            BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, @"X:\", FirmwareType.ALL);
            //BootFileOperation.BcdbootWriteALLBootFileToXAndAct(WTGOperation.bcdbootFileName, WTGOperation.ud);

            BootFileOperation.BooticeAct(@"X:\");
            RemoveLetterX();
            FinishSuccessful();

        }



        #endregion

        private void WriteProgramRunInfoToLog()
        {
            try
            {
                //StringBuilder sb = new StringBuilder();
                //sb.Append("App Version:");
                //sb.Append(Application.ProductVersion);



                Log.WriteLog("Environment.log", "App Version:" +
                    Application.ProductVersion +
                    "\r\nApp Path:" + Application.StartupPath +
                    "\r\nOSVersion:" + Environment.OSVersion.ToString() +
                    "\r\nDism Version:" + FileOperation.GetFileVersion(System.Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe") +
                    "\r\nWim file:" + lblWim.Text +
                    "\r\nUsb Disk:" + WTGModel.udString +
                    "\r\nClassical:" + radiobtnLegacy.Checked.ToString() +
                    "\r\nVHD:" + radiobtnVhd.Checked.ToString() +
                    "\r\nVHDX:" + radiobtnVhdx.Checked.ToString() +
                    "\r\nRe-Partition:" + checkBoxDiskpart.Checked +
                    "\r\nVHD Size Set:" + numericUpDown1.Value.ToString() +
                    "\r\nFixed VHD:" + checkBoxFixed.Checked.ToString() +
                    "\r\nDonet:" + WTGModel.installDonet35.ToString() +
                    "\r\nDisable-WinRE:" + WTGModel.disableWinRe.ToString() +
                    "\r\nBlock Local Disk:" + checkBoxSan_policy.Checked.ToString() +
                    "\r\nNoTemp:" + checkBoxNotemp.Checked.ToString() +
                    "\r\nUEFI+GPT:" + checkBoxUefigpt.Checked.ToString() +
                    "\r\nUEFI+MBR:" + checkBoxUefimbr.Checked.ToString() +
                    "\r\nWIMBOOT:" + checkBoxWimboot.Checked.ToString() +
                    "\r\nCompactOS:" + checkBoxCompactOS.Checked.ToString() +
                    "\r\nNo-format:" + WTGModel.doNotFormat.ToString());
            }
            catch (Exception ex) { MessageBox.Show("Error!\n" + ex.ToString()); }

            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\Logs\\DISM\\dism.log"))
            {
                File.Copy(Environment.GetEnvironmentVariable("windir") + "\\Logs\\DISM\\dism.log", WTGModel.logPath + "\\dism.log");
            }
        }


        private void RemoveLetterX()
        {
            try
            {
                DiskpartScriptManager dsm = new DiskpartScriptManager();
                StringBuilder args = new StringBuilder();
                args.AppendLine("select volume x");
                args.AppendLine("remove");
                args.AppendLine("exit");
                dsm.Args = args.ToString();
                dsm.RunDiskpartScript();
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Log.WriteLog("removeLetterX.log", ex.ToString());
            }
            //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartScriptPath + "\\removex.txt\"");
        }


        #endregion



        #region MenuItemClick/Miscellaneous
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");
        }
        private void 打开程序运行目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Application.StartupPath);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string adLink = linkLabel2.Tag as string;
            if (!string.IsNullOrEmpty(adLink)) VisitWeb(adLink);
            //VisitWeb(adlink);
            //System.Diagnostics.Process.Start(adlink);
        }

        //private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    VisitWeb("http://bbs.luobotou.org/forum-88-1.html");
        //}

        private void imagex解压写入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            if (!System.IO.File.Exists(lblWim.Text)) { MessageBox.Show(MsgManager.GetResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.GetResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\" + WTGModel.imagexFileName, " /apply " + "\"" + lblWim.Text + "\"" + " " + WTGModel.wimPart + " " + WTGModel.ud);
            p.WaitForExit();
            //MsgManager.getResString("Msg_Complete")
            MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
        }

        private void 写入引导文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            try
            {
                ProcessManager.ECMD(WTGModel.applicationFilesPath + "\\" + WTGModel.bcdbootFileName, WTGModel.ud.Substring(0, 3) + "windows  /s  " + WTGModel.ud.Substring(0, 2) + " /f ALL");

                BootFileOperation.BcdbootWriteBootFile(WTGModel.ud, WTGModel.ud, FirmwareType.BIOS);
                MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
            }
            catch (UserCancelException ex)
            {
                ErrorMsg em = new ErrorMsg(ex.Message);
                em.Show();
            }
        }

        private void 设置活动分区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘

            if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
            //System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\bootice.exe", " /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /partitions /activate /quiet");
            //p2.WaitForExit();
            MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
        }

        private void 写入磁盘引导ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }

            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ConfirmChoose", MsgManager.ci) + WTGModel.ud.Substring(0, 1) + MsgManager.GetResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGModel.ud) / 1024 / 1024 + MsgManager.GetResString("Msg_FormatTip", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            BootFileOperation.BooticeWriteMBRPBRAndAct(WTGModel.ud);
            //System.Diagnostics.Process booice = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
            //booice.WaitForExit();
            //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\BOOTICE.exe", (" /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
            //pbr.WaitForExit();
            MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
        }



        //private void toolStripMenuItem2_Click(object sender, EventArgs e)
        //{
        //    //bcdboot9200.Checked = false;
        //    //WTGModel.bcdbootFileName = "bcdboot7601.exe";
        //}

        //private void bcdboot9200_Click(object sender, EventArgs e)
        //{
        //    //bcdboot7601.Checked = false;
        //    //WTGModel.bcdbootFileName = "bcdboot.exe";
        //}


        private void wimbox_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            bool mount_successfully = false;
            if (File.Exists(openFileDialog1.FileName))
            {

                lblWim.Text = openFileDialog1.FileName;
                WTGModel.imageFilePath = openFileDialog1.FileName;
                //Regex.Match ("",@".+\.[")
                WTGModel.choosedImageType = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
                //MessageBox.Show(WTGOperation.filetype);
                if (WTGModel.choosedImageType == "iso")
                {

                    //ProcessManager.ECMD(Application.StartupPath + "\\isocmd.exe  -i");
                    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -i");
                    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -s");
                    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -NUMBER 1");
                    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -eject 0: ");
                    ProcessManager.SyncCMD("\"" + WTGModel.applicationFilesPath + "\\isocmd.exe\" -MOUNT 0: \"" + openFileDialog1.FileName + "\"");
                    //mount.WaitForExit();
                    for (int i = 68; i <= 90; i++)
                    {
                        string ascll_to_eng = Convert.ToChar(i).ToString();
                        if (File.Exists(ascll_to_eng + ":\\sources\\install.wim"))
                        {
                            lblWim.Text = ascll_to_eng + ":\\sources\\install.wim";
                            mount_successfully = true;
                            break;
                        }
                    }
                    if (!mount_successfully)
                    {
                        MessageBox.Show("虚拟光驱加载失败，请手动加载，之后选择install.wim");
                    }
                    else
                    {
                        //useiso = true;
                    }
                }
                else if (WTGModel.choosedImageType == "esd")
                {
                    if (!WTGModel.allowEsd)
                    {
                        //MsgManager.getResString("Msg_ESDError")
                        //此系统不支持ESD文件处理！");
                        MessageBox.Show(MsgManager.GetResString("Msg_ESDError", MsgManager.ci));

                        return;
                    }
                    else
                    {
                        //MessageBox.Show("Test");
                        WTGModel.isEsd = true;
                        checkBoxWimboot.Checked = false;
                        checkBoxWimboot.Enabled = false;
                    }

                }
                else if (WTGModel.choosedImageType == "vhd")
                {
                    if (!radiobtnVhd.Enabled)
                    {
                        radiobtnVhd.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhdx.Enabled = false;


                        //checkBoxcommon.Checked = true;
                        //checkBoxcommon.Enabled = false;

                        //MessageBox.Show("此系统不支持VHD文件处理！"); 
                    }
                    else
                    {
                        radiobtnVhd.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhdx.Enabled = false;
                    }
                }
                else if (WTGModel.choosedImageType == "vhdx")
                {
                    if (!radiobtnVhd.Enabled)
                    {
                        radiobtnVhdx.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhd.Enabled = false;

                        checkBoxCompactOS.Checked = true;
                        checkBoxCompactOS.Enabled = false;

                        //MessageBox.Show("此系统不支持VHD文件处理！"); 
                    }
                    else
                    {
                        radiobtnVhdx.Checked = true;
                        radiobtnLegacy.Enabled = false;
                        radiobtnVhd.Enabled = false;
                    }

                }
                else
                {
                    WTGModel.win7togo = ImageOperation.Iswin7(WTGModel.imagexFileName, lblWim.Text);
                    if (WTGModel.win7togo != 0) //WIN7 cannot comptible with VHDX disk or wimboot
                    {
                        if (radiobtnVhdx.Checked) { radiobtnVhd.Checked = true; }
                        radiobtnVhdx.Enabled = false;
                        checkBoxWimboot.Checked = false;
                        checkBoxWimboot.Enabled = false;
                    }
                }
                if (Regex.IsMatch(WTGModel.choosedImageType, @"wim|esd") && WTGModel.CurrentOS != OS.XP && WTGModel.CurrentOS != OS.Vista)
                {
                    comboBoxParts.Items.Clear();
                    comboBoxParts.Items.Add("0 : 自动选择");
                    comboBoxParts.Items.AddRange(ImageOperation.DismGetImagePartsInfo(lblWim.Text).ToArray());
                    comboBoxParts.SelectedIndex = 0;
                    //////////////////////////////////dism / Get - ImageInfo
                    //////////////////////////////////#warning 为实现此代码
                }

            }

        }

        //private void label1_Click(object sender, EventArgs e)
        //{
        //    VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427&extra=page%3D1");

        //}

        private void 萝卜头IT论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org");

        }

        private void 创建VHDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            VHDOperation vo = new VHDOperation();
            vo.CreateVHD();

        }

        private void 向V盘写入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageOperation.AutoChooseWimIndex(ref WTGModel.wimPart, WTGModel.win7togo);
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\" + WTGModel.imagexFileName, " /apply " + "\"" + lblWim.Text + "\"" + " " + WTGModel.wimPart.ToString() + " " + "v:\\");
            p.WaitForExit();

        }

        private void 卸载V盘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VHDOperation vo = new VHDOperation();
            vo.DetachVHD();

        }

        private void 复制VHD启动文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //MsgManager.getResString("Msg_chooseud")
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            try
            {
                ProcessManager.ECMD("takeown.exe", " /f \"" + WTGModel.ud + "\\boot\\" + "\" /r /d y && icacls \"" + WTGModel.ud + "\\boot\\" + "\" /grant administrators:F /t");
                ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + " /e /h /y");
            }
            catch (UserCancelException ex)
            {
                ErrorMsg em = new ErrorMsg(ex.Message);
                em.Show();
            }

            //copyvhdbootfile();

        }

        private void 复制win8vhdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            VHDOperation vo = new VHDOperation();
            vo.CopyVHD();

        }

        private void 清理临时文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VHDOperation.CleanTemp();
        }

        private void checkBoxuefi_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUefimbr.Checked && checkBoxUefigpt.Checked) { checkBoxUefimbr.Checked = false; checkBoxUefigpt.Checked = true; }
            checkBoxDiskpart.Enabled = !checkBoxUefigpt.Checked;
            checkBoxDiskpart.Checked = checkBoxUefigpt.Checked;
        }

        private void radiovhd_EnabledChanged(object sender, EventArgs e)
        {
            radiobtnLegacy.Checked = true;
        }

        private void bootsectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            //
            System.Diagnostics.Process p1 = System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\" + "\\bootsect.exe", " /nt60 " + WTGModel.ud.Substring(0, 2) + " /force /mbr");
            p1.WaitForExit();
            MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
        }

        private void diskpart重新分区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);
            //diskPart();


        }

        private void linkLabel3_LinkClicked_2(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-3566-1-1.html");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-3566-1-1.html");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-6098-1-1.html");
        }

        private void 在线帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");

        }

        private void 官方论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum-88-1.html");

        }

        private void diskpart重新分区ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MsgManager.getResString("Msg_chooseud")
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            //MsgManager.getResString("Msg_XPNotCOMP")
            //XP系统不支持此操作
            //MsgManager.getResString("Msg_ClearPartition")
            //此操作将会清除移动磁盘所有分区的所有数据，确认？
            if (System.Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2")) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_ClearPartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }

            //if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 
            //Msg_Complete
            try
            {
                DiskOperation.DiskPartRePartitionUD(WTGModel.partitionSize);
                //diskPart();
                MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
            }
            catch (Exception ex)
            {
                ErrorMsg em = new ErrorMsg(ex.Message);
                em.Show();
            }

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

  
        private void 打开程序运行目录ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath);
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("萝卜头IT论坛 nkc3g4制作\nQQ:1443112740\nEmail:microsoft5133@126.com","关于");
            AboutBox abx = new AboutBox();
            abx.Show();
        }

        private void vHDUEFIBCDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
        }

        private void 自动检查更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SWOnline swo = new SWOnline(releaseUrl, reportUrl);
            Thread threadUpdate = new Thread(swo.Update);
            threadUpdate.Start();

            //threadupdate = new Thread(SWOnline.update);
            //threadupdate.Start();
            //MsgManager.getResString("Msg_UpdateTip")
            //若无弹出窗口，则当前程序已是最新版本！
            MessageBox.Show(MsgManager.GetResString("Msg_UpdateTip", MsgManager.ci));
        }

        private void checkBoxdiskpart_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDiskpart.Checked)
            {
                //Msg_Repartition
                //MessageBox.Show(MsgManager.GetResString("Msg_Repartition", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                WTGModel.doNotFormat = false;
                //checkBoxunformat.Enabled = false;
            }
            else
            {
                ///*checkBoxunformat*/.Enabled = true;
            }
        }

 

        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip1.SetToolTip(this.comboBoxUd, comboBoxUd.SelectedItem.ToString()); ;
            }
            catch (Exception ex)
            {
                Log.WriteLog("comboBox1_MouseHover.log", ex.ToString());
            }
        }

        private void wIN7USBBOOTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";
            ImageOperation.Win7REG(@"V:\\");
        }

        private void bOOTICEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(WTGModel.applicationFilesPath + "\\BOOTICE.EXE");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(WTGModel.logPath);

        }

        private void checkBoxuefimbr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUefigpt.Checked && checkBoxUefimbr.Checked) { checkBoxUefigpt.Checked = false; checkBoxUefimbr.Checked = true; }
            if (checkBoxUefimbr.Checked) { WTGModel.disableWinRe = true; }
            checkBoxDiskpart.Enabled = !checkBoxUefimbr.Checked;
            checkBoxDiskpart.Checked = checkBoxUefimbr.Checked;
        }



        private void toolStripMenuItemvhdx_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            try
            {
                ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + " /e /h /y");
                ProcessManager.ECMD("xcopy.exe", "\"" + WTGModel.applicationFilesPath + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGModel.ud + "\\boot\\ /e /h /y");
            }
            catch (Exception ex)
            {
                ErrorMsg err = new ErrorMsg(ex.Message);
                err.Show();
            }

        }


        private void toolStripMenuItem3_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripMenuItem3.Checked)
            {
                IniFile.WriteVal("Main", "AutoUpdate", "1", settingFilePath);
            }
            else
            {
                IniFile.WriteVal("Main", "AutoUpdate", "0", settingFilePath);

            }
        }


        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-1625-1-1.html");

        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("This program will restart,continue?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }

            IniFile.WriteVal("Main", "Language", "EN", settingFilePath);
            Application.Restart();
        }

        private void chineseSimpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("程序将会重启，确认继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            IniFile.WriteVal("Main", "Language", "ZH-HANS", settingFilePath);
            Application.Restart();

        }

        private void 繁体中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("程序將會重啟，確認繼續？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            IniFile.WriteVal("Main", "Language", "ZH-HANT", settingFilePath);
            Application.Restart();

        }



        private void 修复盘符ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxUd.SelectedIndex == 0) { MessageBox.Show(MsgManager.GetResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            string vhdPath = string.Empty;
            if (File.Exists(vhdPath = WTGModel.ud + "win8.vhd") || File.Exists(vhdPath = WTGModel.ud + "win8.vhdx"))
            {
                VHDOperation vo = new VHDOperation();
                vo.AttachVHD(vhdPath);
                ImageOperation.Fixletter("C:", "V:");
                vo.DetachVHD();
            }
            else
            {
                ImageOperation.Fixletter("C:", WTGModel.ud.Substring(0, 2));
            }
            MessageBox.Show(MsgManager.GetResString("Msg_Complete", MsgManager.ci));
        }

        private void linkLabel3_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-3566-1-1.html");

        }

        private void linkLabel5_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/thread-6098-1-1.html");

        }

        //private void wTG高级设定选项ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    WTGSettings ws = new WTGSettings();
        //    ws.Show();
        //}
        #endregion

        public static void VisitWeb(string url)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                string s = key.GetValue("").ToString();

                Regex reg = new Regex("\"([^\"]+)\"");
                MatchCollection matchs = reg.Matches(s);

                string filename = "";
                if (matchs.Count > 0)
                {
                    filename = matchs[0].Groups[1].Value;
                    System.Diagnostics.Process.Start(filename, url);
                }
            }
            catch (Exception ex)
            {
                //MsgManager.getResString("Msg_FatalError")
                //程序遇到严重错误\n官方支持论坛：bbs.luobotou.org\n
                MessageBox.Show("程序遇到严重错误\nFATAL ERROR!官方支持论坛：bbs.luobotou.org\n" + ex.ToString());

            }


        }
        #region UserControls

        private void Form1_Load(object sender, EventArgs e)
        {

            //MessageBox.Show(comboBox1.Size.Height.ToString()+" "+btnBrowser.Size.Height.ToString());
            toolStripMenuItem3.Checked = autoCheckUpdate;
            FileValidation();
            SystemDetection();

            timer1.Start();//UdList 刷新

            SWOnline swo = new SWOnline(releaseUrl, reportUrl);
            swo.TopicLink = WriteProgress.topicLink;
            swo.TopicName = WriteProgress.topicName;
            swo.Linklabel = linkLabel2;

            if (Assembly.GetExecutingAssembly().GetName().Version.Revision == 9)
            {
                Text += "Preview Bulit:" + File.GetLastWriteTime(GetType().Assembly.Location);
            }
            else
            {
                Text += Application.ProductVersion;
                if (autoCheckUpdate)
                {
                    Thread threadUpdate = new Thread(swo.Update);
                    threadUpdate.Start();
                }
            }


            Thread threadReport = new Thread(swo.Report);
            threadReport.Start();
            if (Assembly.GetExecutingAssembly().GetName().Version.Revision == 0)
            {
                Thread threadShowad = new Thread(swo.Showad);
                threadShowad.Start();
            }

            ///JieMian
            LoadUDList();
            comboBoxParts.Items.Clear();
            comboBoxParts.Items.Add("0 : 自动选择");
            comboBoxParts.SelectedIndex = 0;
            comboBoxVhdPartitionType.SelectedIndex = 0;
            //txtVhdTempPath.text

        }
        private void button1_Click(object sender, EventArgs e)
        {

            //MessageBox.Show(((UsbDisk)comboBox1.SelectedItem).Size.ToString());
            //MessageBox.Show((DiskOperation.GetHardDiskFreeSpace("F:\\")*1024).ToString());
            try
            {
                if (tWrite != null && tWrite.IsAlive)
                {
                    MessageBox.Show(MsgManager.GetResString("Msg_WriteProcessing", MsgManager.ci));
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Msg_WriteProcessing.log", ex.ToString());
                //Console.WriteLine(ex);
            }
            WTGModel.ud = comboBoxUd.SelectedItem.ToString().Substring(0, 2) + "\\";//
            WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
            WTGModel.udString = comboBoxUd.SelectedItem.ToString();
            WTGModel.isWimBoot = checkBoxWimboot.Checked;
            WTGModel.isSan_policy = checkBoxSan_policy.Checked;
            WTGModel.imageFilePath = lblWim.Text;
            WTGModel.isCompactOS = checkBoxCompactOS.Checked;
            WTGModel.userSetSize = (int)numericUpDown1.Value;
            WTGModel.isFixedVHD = checkBoxFixed.Checked;
            WTGModel.isUefiGpt = checkBoxUefigpt.Checked;
            WTGModel.isUefiMbr = checkBoxUefimbr.Checked;
            WTGModel.isNoTemp = checkBoxNotemp.Checked;
            WTGModel.ntfsUefiSupport = checkBoxNtfsUefi.Checked;
            WTGModel.doNotFormat = checkBoxDoNotFormat.Checked;
            WTGModel.vhdNameWithoutExt = txtVhdNameWithoutExt.Text;
            WTGModel.installDonet35 = checkBoxDonet.Checked;
            WTGModel.fixLetter = checkBoxFixLetter.Checked;
            WTGModel.noDefaultDriveLetter = checkBoxNoDefaultLetter.Checked;
            WTGModel.disableWinRe = checkBoxDisWinre.Checked;
            //WTGModel.partitionSize1 = txtPartitionSize1.Text;
            //WTGModel.partitionSize2 = txtPartitionSize2.Text;
            //WTGModel.partitionSize3 = txtPartitionSize3.Text;
            WTGModel.efiPartitionSize = txtEfiSize.Text;
            WTGModel.vhdPartitionType = comboBoxVhdPartitionType.SelectedText;
            WTGModel.vhdTempPath = txtVhdTempPath.Text;
            WTGModel.partitionSize = new string[3];
            WTGModel.partitionSize[0] = txtPartitionSize1.Text;
            WTGModel.partitionSize[1] = txtPartitionSize2.Text;
            WTGModel.partitionSize[2] = txtPartitionSize3.Text;
            WTGModel.wimPart = comboBoxParts.SelectedItem.ToString().Substring(0, 1);
            if (radiobtnVhdx.Checked)
            {
                WTGModel.vhdExtension = "vhdx";
            }
            WTGModel.win8VHDFileName = WTGModel.vhdNameWithoutExt + "." + WTGModel.vhdExtension;
            tWrite = new Thread(new ThreadStart(GoWrite));
            tWrite.Start();

        }

        private void isobutton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (File.Exists(openFileDialog1.FileName)) { lblWim.Text = openFileDialog1.FileName; }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath.Length != 3)
            {
                if (folderBrowserDialog1.SelectedPath.Length != 0)
                {
                    //MsgManager.getResString("Msg_UDRoot")
                    //请选择优盘根目录
                    MessageBox.Show(MsgManager.GetResString("Msg_UDRoot", MsgManager.ci));
                }
                return;

            }
            UsbDisk udM = new UsbDisk(folderBrowserDialog1.SelectedPath);
            //udM.Volume = folderBrowserDialog1.SelectedPath;
            diskCollection.Add(udM);
            //UDList.Add(folderBrowserDialog1.SelectedPath);
            OutText(true, diskCollection);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (tWrite != null && tWrite.IsAlive)
                {
                    //MsgManager.getResString("Msg_WritingAbort")
                    //正在写入，您确定要取消吗？
                    if (DialogResult.No == MessageBox.Show(MsgManager.GetResString("Msg_WritingAbort", MsgManager.ci), MsgManager.GetResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { e.Cancel = true; return; }
                    Environment.Exit(0);
                    //threadwrite.Abort();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Form1_FormClosing.log", ex.ToString());
                //Console.WriteLine(ex);
            }
            VHDOperation.CleanTemp();
            try
            {
                Directory.Delete(Path.GetTempPath() + "\\WTGA", true);
            }
            catch (Exception ex)
            {
                Log.WriteLog("DeleteTempPath.log", ex.ToString());
            }

        }
        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }




        private void 错误提示测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TakeOwn(@"E:\boot");
            TakeOwn(@"E:\EFI");

            //btnBrowser.Height = comboBoxUd.Height;
            //MessageBox.Show(btnBrowser.Height.ToString());
            //MessageBox.Show(comboBoxUd.Height.ToString());

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //jss.Serialize()

            //BootFileOperation.BcdbootWriteBootFile("h:", "h:", FirmwareType.BIOS);
        }

        private void radiochuantong_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
            checkBoxFixed.Enabled = false;
            checkBoxNotemp.Enabled = false;
            lblVhdSize.Enabled = false;
            lblGB.Enabled = false;
            txtVhdNameWithoutExt.Enabled = false;
        }

        private void radiovhd_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            checkBoxFixed.Enabled = true;
            checkBoxNotemp.Enabled = true;
            lblVhdSize.Enabled = true;
            lblGB.Enabled = true;
            txtVhdNameWithoutExt.Enabled = true;
        }

        private void radiovhdx_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = true;
            checkBoxFixed.Enabled = true;
            checkBoxNotemp.Enabled = true;
            lblVhdSize.Enabled = true;
            lblGB.Enabled = true;
            txtVhdNameWithoutExt.Enabled = true;
        }

        #endregion



        private void txtVhdTempPath_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip1.SetToolTip(txtVhdTempPath, txtVhdTempPath.Text); ;
            }
            catch (Exception ex)
            {
                Log.WriteLog("txtVhdTempPath_MouseHover.log", ex.ToString());
            }
        }

        private void btnVhdTempPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                txtVhdTempPath.Text = folderBrowserDialog1.SelectedPath;
            }

        }

        private void txtPartitionSize1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
            else
            {
                checkBoxDiskpart.Checked = true;
            }
        }

        private void txtEfiSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void txtPartitionSize2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
            else
            {
                checkBoxDiskpart.Checked = true;
            }
        }


        private void txtPartitionSize3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void linklblRestoreMultiPartition_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtEfiSize.Text = "350";
            txtPartitionSize1.Text = "0";
            txtPartitionSize2.Text = "0";

        }



        private void comboBoxUd_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(comboBoxUd.SelectedIndex.ToString());

            if (comboBoxUd.SelectedIndex != 0 && comboBoxUd.SelectedIndex != -1)
            {
                WTGModel.UdObj = (UsbDisk)comboBoxUd.SelectedItem;
                udSizeInMB = (int)(WTGModel.UdObj.DiskSize / 1048576);
                txtPartitionSize3.Text = udSizeInMB.ToString();
                if (!WTGModel.UdObj.DriveType.Contains("Removable Disk"))
                {
                    txtPartitionSize1.Enabled = true;
                    txtPartitionSize2.Enabled = true;
                    txtPartitionSize3.Enabled = true;
                }
                else
                {
                    txtPartitionSize1.Enabled = false;
                    txtPartitionSize2.Enabled = false;
                    txtPartitionSize3.Enabled = false;

                }
            }
            else
            {
                txtPartitionSize1.Enabled = false;
                txtPartitionSize2.Enabled = false;
                txtPartitionSize3.Enabled = false;

            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void txtPartitionSize1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartitionSize1.Text))
            {
                txtPartitionSize1.Text = "0";
            }
            int remain = udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize1.Text);
            if (remain < 0)
            {
                txtPartitionSize1.Text = (udSizeInMB - int.Parse(txtPartitionSize2.Text)).ToString();
                txtPartitionSize3.Text = "0";
            }
            else
            {
                txtPartitionSize3.Text = remain.ToString();
            }
        }

        private void txtPartitionSize2_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPartitionSize2.Text))
            {
                txtPartitionSize2.Text = "0";
            }
            int remain = udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize1.Text);
            if (remain < 0)
            {
                txtPartitionSize2.Text = (udSizeInMB - int.Parse(txtPartitionSize1.Text)).ToString();
                txtPartitionSize3.Text = "0";

            }
            else
            {
                txtPartitionSize3.Text = (udSizeInMB - int.Parse(txtPartitionSize2.Text) - int.Parse(txtPartitionSize1.Text)).ToString();
            }

        }

        private void linklblTabPage4Resotre_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            comboBoxVhdPartitionType.SelectedIndex = 1;
            txtVhdTempPath.Text = Path.GetTempPath();
        }
    }
}
