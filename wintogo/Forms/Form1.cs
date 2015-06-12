using iTuner;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
//using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
namespace wintogo
{
    public partial class Form1 : Form
    {
        public static Action<string> SetText;
        //private UsbManager manager;
        string currentlist;//当前优盘列表
        //int formwide = 623;
        //bool allowesd = false;//可使用ESD文件
        bool autoupdate = true;
        private Thread threadwrite;
        private Thread listUdisks;
        private string efisize = "350";
        List<string> UDList = new List<string>();
        private readonly string releaseUrl = "http://bbs.luobotou.org/app/wintogo.txt";
        private readonly string reportUrl = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=";
        private readonly string settingFilePath = Application.StartupPath + "\\settings.ini";
        
        public Form1()
        {
            ReadConfigFile();
            InitializeComponent();
            //LoadPlugins();
            //wp = new WriteProgress();
            //CheckForIllegalCrossThreadCalls = false;
        }
        private void set_textboxText(string s)
        {
            this.linkLabel2.Text = s;
            //MessageBox.Show("Test");
            //label4.Visible = true;
            this.linkLabel2.Visible = true;
        }



        private void CheckFiles()
        {
            if (StringOperation.IsChina(WTGOperation.scriptTempPath))
            {
                if (StringOperation.IsChina(Application.StartupPath))
                {
                    ErrorMsg er = new ErrorMsg(MsgManager.getResString("IsChinaMsg", MsgManager.ci));
                    er.ShowDialog();
                    Environment.Exit(0);
                }
                else { WTGOperation.scriptTempPath = Application.StartupPath + "\\logs"; }
            }
          

            //List<string> sw_filelist = new List<string>();

            ////string[] sw_fl = new string[13]; ;//software filelist
            ////ArrayList sw_filelist = new ArrayList();
            //sw_filelist.Add("\\files.dat");
            ////sw_filelist.Add("\\files\\san_policy.xml");
            ////sw_filelist.Add("\\files\\imagex_x86.exe");
            ////sw_filelist.Add("\\files\\fbinst.exe");
            ////sw_filelist.Add("\\files\\FastCopy.exe");
            ////sw_filelist.Add("\\files\\bootsect.exe");
            ////sw_filelist.Add("\\files\\BOOTICE.EXE");
            ////sw_filelist.Add("\\files\\bcdboot.exe");
            ////sw_filelist.Add("\\files\\bcdboot7601.exe");
            ////sw_filelist.Add("\\files\\imagex_x64.exe");
            ////sw_filelist.Add("\\files\\usb.reg");
            ////sw_filelist.Add("\\files\\settings.ini");

            //for (int i = 1; i < sw_filelist.Count; i++)
            //{
            //    if (!File.Exists(Application.StartupPath + sw_filelist[i]))
            //    {
            //        //MsgManager.getResString("Msg_filelist")

            //        MessageBox.Show(MsgManager.getResString("Msg_filelist", MsgManager.ci) + Application.StartupPath + sw_filelist[i], MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        VisitWeb("http://bbs.luobotou.org/thread-761-1-1.html");

            //        Environment.Exit(0);
            //        break;
            //    }
            //}
            //解压文件
            UnZipClass.UnZip(Application.StartupPath + "\\files.dat", StringOperation.Combine(Path.GetTempPath(), "WTGA"));


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            #region OldCode
            //using (StreamReader sr=new StreamReader (@"c:\My\b.txt"))
            //{
            //    string fileStr=sr.ReadToEnd();
            //    MatchCollection mc = Regex.Matches(fileStr, @"Index : ([0-9]+)\nName : [\^]");
            //    MessageBox.Show(mc[1].Value);

            //}
            //string fileStr=File.ReadAllText(@"c:\b.txt");
            //UnZipFiles();
            //SerFiles();

            //WTGOperation.ud = "100";
            //vo.WinbootImagex();
            #endregion
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            toolStripMenuItem3.Checked = autoupdate;
            CheckFiles();

            //FileOperation.CleanLockStream(MsgManager.getResString("Msg_ntfsstream", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci));
            //this.Width = (int)((double)this.Width * 0.675);

            SystemDetection();

           
            timer1.Start();
            //set_TextDelegate Set_Text = new set_TextDelegate(set_textboxText); ;
            SetText = set_textboxText;
            //Set_Text  //实例化
            SWOnline swo = new SWOnline(releaseUrl, reportUrl);
            swo.TopicLink = WriteProgress.topicLink;
            swo.TopicName = WriteProgress.topicName;
            swo.Linklabel = linkLabel2;
            swo.SetLinkLabel = SetText;
            Thread threadUpdate = new Thread(swo.Update);
            threadUpdate.Start();
            Thread threadReport = new Thread(swo.Report);
            threadReport.Start();
            Thread threadShowad = new Thread(swo.Showad);
            threadShowad.Start();
            LoadUDList();
            this.Text += Application.ProductVersion;
            //sw.Stop();
            //MessageBox.Show(sw.Elapsed.ToString ());
        }

        //private void UnZipFiles()
        //{
        //    UnZipClass.UnZip(Application.StartupPath + "\\files.dat", StringOperation.Combine(Path.GetTempPath(), "WTGA"));
        //}

        //private void SerFiles()
        //{
            //    using (FileStream fsRead = File.OpenRead(@"E:\MyProgram\wintogo\wintogo\bin\Release\files\imagex_x64.exe"))
            //    {



            //            //byte[] byts = new byte[buffersize * 1024 * 1024];
            //            //int r = 0;
            //            //while ((r = fsRead.Read(byts, 0, byts.Length)) > 0)
            //            //{
            //            //    fsWrite.Write(byts, 0, r);
            //            //    //Console.WriteLine(fsWrite.Position / (double)fsRead.Length * 100);
            //            //    //r = fsRead.Read(byts, 0, byts.Length);
            //            //}


            //}
            //using (FileStream fs = new FileStream(@"E:\MyProgram\wintogo\wintogo\bin\Release\files\imagex_x64.exe", FileMode.Open))
            //{
            //    byte[] byts = new byte[fs.Length];
            //    fs.Read(byts, 0, byts.Length);
            //    BinaryFormatter bf = new BinaryFormatter();
            //    using (FileStream fsWrite =new FileStream(Application.StartupPath + @"\a.bin", FileMode.Create))
            //    {
            //        bf.Serialize(fsWrite, byts);
            //    }


            //}
            //using (FileStream fs = new FileStream(Application.StartupPath + @"\a.bin", FileMode.Open))
            //{
            //    BinaryFormatter bf = new BinaryFormatter();
            //    byte[] byts = bf.Deserialize(fs) as byte[];
            //    using (FileStream fsWrite = new FileStream(Path.GetTempPath() + "\\a.exe", FileMode.Create))
            //    {
            //        fsWrite.Write(byts, 0, byts.Length);
            //    }

            //}
        //}

        private void SystemDetection()
        {
            WTGOperation.bcdboot = "bcdboot.exe";
            string osVersionStr = Environment.OSVersion.ToString();
            if (osVersionStr.Contains("5.1")) //XP 禁用功能
            {
                radiovhd.Enabled = false;
                radiovhdx.Enabled = false;
                radiochuantong.Checked = true;
                //button2.Enabled = false;
                groupBoxadv.Enabled = false;
                checkBoxdiskpart.Checked = false;
                checkBoxdiskpart.Enabled = false;
                bcdboot9200.Checked = false;
                bcdboot7601.Checked = true;
                WTGOperation.bcdboot = "bcdboot7601.exe";
                label4.Visible = true;
                label5.Visible = true;

            }
            else if (osVersionStr.Contains("6.0")) //vista
            {
                radiovhd.Enabled = false;
                radiovhdx.Enabled = false;
                radiochuantong.Checked = true;

                //button2.Enabled = false;
                groupBoxadv.Enabled = false;
                checkBoxdiskpart.Checked = false;
                checkBoxdiskpart.Enabled = false;
                bcdboot9200.Checked = false;
                bcdboot7601.Checked = true;
                WTGOperation.bcdboot = "bcdboot7601.exe";
                label4.Visible = true;
                label5.Visible = true;

            }
            else if (osVersionStr.Contains("6.1"))//WIN7
            {
                label4.Visible = true;
                label5.Visible = true;
                radiovhd.Checked = true; radiovhdx.Enabled = false;
            }
            else if (osVersionStr.Contains("6.2") || osVersionStr.Contains("6.3"))
            {
                radiovhd.Checked = true;
                //WIN8.1 UPDATE1 WIMBOOT
                string dismversion = FileOperation.GetFileVersion(System.Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe");
                if (dismversion.Substring(0, 14) == "6.3.9600.17031" || dismversion.Substring(0, 3) == "6.4" || dismversion.Substring(0, 5) == "10.0.")
                {
                    checkBoxwimboot.Enabled = true;
                    WTGOperation.allowEsd = true;
                }
            }


            if (Environment.Is64BitOperatingSystem)
            {
                WTGOperation.imagex = "imagex_x64.exe";
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
            if (autoup == "0") { autoupdate = false; }
            if (tp != "") { SetTempPath.temppath = tp; }
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
        //private void LoadPlugins()
        //{

        //    string pluginPath = StringOperation.Combine(Application.StartupPath, "plugins");
        //    //MessageBox.Show(pluginPath);
        //    foreach (var item in Directory.GetFiles(pluginPath, "*.dll"))
        //    {
        //        try
        //        {
        //            Assembly asm = Assembly.LoadFile(item);
        //            Type[] types = asm.GetExportedTypes();
        //            Type typeIPlugin = typeof(InterfacePlugin);
        //            foreach (var typeItem in types)
        //            {
        //                if (typeIPlugin.IsAssignableFrom(typeItem) && !typeItem.IsAbstract)
        //                {
        //                    InterfacePlugin iPlugin = (InterfacePlugin)Activator.CreateInstance(typeItem);
        //                    ToolStripItem tsItem = 工具ToolStripMenuItem.DropDownItems.Add(iPlugin.PluginName);
        //                    工具ToolStripMenuItem.Tag = iPlugin;
        //                    tsItem.Click += tsItem_Click;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //MessageBox.Show("Test");
        //            Log.WriteLog("LoadPlugins.log", ex.ToString());
        //        }
        //    }

        //}

        //void tsItem_Click(object sender, EventArgs e)
        //{
        //    InterfacePlugin iPlugin = 工具ToolStripMenuItem.Tag as InterfacePlugin;
        //    if (iPlugin != null)
        //    {
        //        iPlugin.Execute();
        //    }
        //}
        public delegate void OutDelegate(bool isend, object dtSource);
        public void OutText(bool isend, object dtSource)
        {
            if (comboBox1.InvokeRequired)
            {
                OutDelegate outdelegate = new OutDelegate(OutText);
                BeginInvoke(outdelegate, new object[] { isend, dtSource });
                return;
            }
            comboBox1.DataSource = null;
            comboBox1.DataSource = dtSource;
            //foreach (UsbDisk item in (UsbDiskCollection)dtSource)
            //{
            //    MessageBox.Show(item.ToString());
            //}
            //MessageBox.Show(((UsbDiskCollection)dtSource).ToString ());
            if (comboBox1.Items.Count != 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            if (isend) { comboBox1.SelectedIndex = comboBox1.Items.Count - 1; }
        }

        public void GetUdiskInfo()
        {
            //List<string> list = new List<string>() { "a","b"};
            //List<string> listb = new List<string>() { "a", "b" };
            //if (list==listb)
            //{
            //    MessageBox.Show("Test");
            //}

            //UsbManager um = new UsbManager();
            string newlist = string.Empty;
            UsbManager manager = new UsbManager();
            try
            {
                UsbDiskCollection disks = new UsbDiskCollection();
                //UsbDiskCollection disks = manager.GetAvailableDisks();
                UsbDisk udChoose = new UsbDisk(MsgManager.getResString("Msg_chooseud", MsgManager.ci));
                disks.Add(udChoose);
                
                //if (disks == null) { return; }
                foreach (UsbDisk disk in manager.GetAvailableDisks())
                {
                    disks.Add(disk);
                    newlist += disk.ToString();
                   
                }
                if (newlist != currentlist)
                {
                    UDList.Clear();
                    //MsgManager.getResString("Msg_chooseud")
                    //请选择可移动设备
                    UDList.Add(MsgManager.getResString("Msg_chooseud", MsgManager.ci));

                    //foreach (UsbDisk disk in disks)
                    //{

                    //    UDList.Add(disk.ToString());
                    //    //textBox.AppendText(disk.ToString() + CR);
                    //}
                    currentlist = newlist;

                    OutText(false, disks);
                }
            }
            catch (Exception ex) { Log.WriteLog("GetUdiskInfo.log", ex.ToString()); }
            finally { manager.Dispose(); }

        }
        public void LoadUDList()
        {
            //Udlist = new Udlist();
            listUdisks = new Thread(GetUdiskInfo);
            listUdisks.Start();

        }


        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadUDList();
        }





        #region CORE
        private void goWrite()
        {
            //wimpart = ChoosePart.part;//读取选择分卷，默认选择第一分卷
            #region 各种提示
            //各种提示
            if (wimbox.Text.ToLower().Substring(wimbox.Text.Length - 3, 3) != "wim" && wimbox.Text.ToLower().Substring(wimbox.Text.Length - 3, 3) != "esd" && wimbox.Text.ToLower().Substring(wimbox.Text.Length - 3, 3) != "vhd" && wimbox.Text.ToLower().Substring(wimbox.Text.Length - 4, 4) != "vhdx")//不是WIM文件
            {
                //                MsgManager.getResString("Msg_chooseinstallwim")
                //镜像文件选择错误！请选择install.wim！
                MessageBox.Show(MsgManager.getResString("Msg_chooseinstallwim", MsgManager.ci)); return;
            }
            else
            {
                //请选择install.wim文件
                if (!System.IO.File.Exists(wimbox.Text)) { MessageBox.Show(MsgManager.getResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error); return; }//文件不存在.
                WTGOperation.imageFilePath = wimbox.Text;
            }


            if (!Directory.Exists(WTGOperation.ud)) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci) + "!", MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error); return; }//是否选择优盘
            if (DiskOperation.GetHardDiskSpace(WTGOperation.ud) <= 12582912) //优盘容量<12 GB提示
            {
                //MsgManager.getResString("Msg_DiskSpaceWarning") 
                //可移动磁盘容量不足16G，继续写入可能会导致程序出错！您确定要继续吗？
                if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_DiskSpaceWarning", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
            }


            if (DiskOperation.GetHardDiskSpace(WTGOperation.ud) <= numericUpDown1.Value * 1048576)
            {
                //优盘容量小于VHD设定大小，请修改设置！
                //MsgManager.getResString("Msg_DiskSpaceError")
                MessageBox.Show(MsgManager.getResString("Msg_DiskSpaceError", MsgManager.ci), MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //MsgManager.getResString("Msg_ConfirmChoose")
            //请确认您所选择的
            //MsgManager.getResString("Msg_Disk_Space") 盘，容量
            //Msg_FormatTip

            //GB 是将要写入的优盘或移动硬盘\n误格式化，后果自负！

            if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_ConfirmChoose", MsgManager.ci) + WTGOperation.ud.Substring(0, 1) + MsgManager.getResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGOperation.ud) / 1024 / 1024 + MsgManager.getResString("Msg_FormatTip", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk)) { return; }
            if (checkBoxdiskpart.Checked && !checkBoxuefi.Checked && !checkBoxuefimbr.Checked)//勾选重新分区提示
            {
                //MsgManager.getResString("Msg_Repartition")
                //Msg_Repartition
                //您勾选了重新分区，优盘或移动硬盘上的所有文件将被删除！\n注意是整个磁盘，不是一个分区！
                if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_Repartition", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }

                //MsgManager.getResString("Msg_DoWhat")
                //如果您不清楚您在做什么，请立即停止操作！

                if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_DoWhat", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                DiskOperation.DiskPartReformatUD();

                //diskPart();
            }
            else//普通格式化提示
            {
                if (!checkBoxunformat.Checked)
                {
                    //MsgManager.getResString("Msg_FormatWarning")
                    //盘将会被格式化，此操作将不可恢复，您确定要继续吗？\n由于写入时间较长，请您耐心等待！\n写入过程中弹出写入可能无效属于正常现象，选是即可。
                    if (DialogResult.No == MessageBox.Show(WTGOperation.ud.Substring(0, 1) + MsgManager.getResString("Msg_FormatWarning", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                    //if (DialogResult.No == MessageBox.Show("如果您不清楚您在做什么，请立即停止操作！", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                }
            }
            #endregion



            ///////删除旧LOG文件
            Log.DeleteAllLogs();
            //ProcessManager.SyncCMD ("cmd.exe /c del /f /s /q \""+Application .StartupPath +"\\logs\\*.*\"");
            //////////////将程序运行信息写入LOG
            WriteProgramRunInfoToLog();


            if (checkBoxuefi.Checked)
            {
                //UEFI+GPT
                if (System.Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2"))
                {
                    //MsgManager.getResString("Msg_XPUefiError")
                    //XP系统不支持UEFI模式写入
                    MessageBox.Show(MsgManager.getResString("Msg_XPUefiError", MsgManager.ci)); return;
                }
                if (WTGOperation.udstring.Contains("Removable Disk"))
                {
                    //MsgManager.getResString("Msg_UefiError")
                    //此优盘不支持UEFI模式\n只有 Fixed Disk格式支持\n详情请看论坛说明！
                    MessageBox.Show(MsgManager.getResString("Msg_UefiError", MsgManager.ci), MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    VisitWeb("http://bbs.luobotou.org/thread-6506-1-1.html");
                    return;
                }
                //MsgManager.getResString("Msg_UefiFormatWarning")
                //您所选择的是UEFI模式，此模式将会格式化您的整个移动磁盘！\n注意是整个磁盘！！！\n程序将会删除所有优盘分区！
                if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_UefiFormatWarning", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                DiskOperation.GenerateGPTAndUEFIScript(efisize, WTGOperation.ud);
                ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\uefi.txt\"");
                if (radiochuantong.Checked)
                {
                    //UEFI+GPT 传统
                    UefiGptTypical();


                }
                else // UEFI+GPT VHD、VHDX模式
                {

                    UefiGptVhdVhdx();
                }
                removeLetterX();
                Finish f = new Finish();
                f.ShowDialog();

                //MessageBox.Show("UEFI模式写入完成！\n请重启电脑用优盘启动\n如有问题，可去论坛反馈！","完成啦！",MessageBoxButtons .OK ,MessageBoxIcon.Information );
            }
            else if (checkBoxuefimbr.Checked)
            {
                //UEFI+MBR
                if (WTGOperation.udstring.Contains("Removable Disk"))
                {
                    MessageBox.Show(MsgManager.getResString("Msg_UefiError", MsgManager.ci), MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    VisitWeb("http://bbs.luobotou.org/thread-6506-1-1.html");
                    return;
                }
                if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_UefiFormatWarning", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }

                DiskOperation.GenerateMBRAndUEFIScript(efisize, WTGOperation.ud);
                ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\uefimbr.txt\"");

                if (radiochuantong.Checked)
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
                if (WTGOperation.udstring.Contains("Removable Disk") && radiochuantong.Checked)
                {
                    if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_Legacywarning", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
                }

                if (!checkBoxdiskpart.Checked && !checkBoxunformat.Checked)//普通格式化
                {
                    ProcessManager.ECMD("cmd.exe", "/c format " + WTGOperation.ud.Substring(0, 2) + "/FS:ntfs /q /V: /Y");
                    //
                }
                if (WTGOperation.forceformat) //强制格式化
                {
                    System.Diagnostics.Process ud1 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + "\\fbinst.exe", (" " + WTGOperation.ud.Substring(0, 2) + " format -r -f"));//Format disk
                    ud1.WaitForExit();
                }
                #endregion
                ///////////////////////////////////正式开始////////////////////////////////////////////////
                if (radiochuantong.Checked)
                {
                    NonUEFITypical();

                }
                else //非UEFI VHD VHDX
                {
                    NonUEFIVHDVHDX();
                }
            }
        }


        #region 七种写入模式
        private void UefiGptTypical()
        {
            ImageOperation io = new ImageOperation();
            //io.isESD = WTGOperation.isesd;
            io.imageX = WTGOperation.imagex;
            io.imageFile = WTGOperation.imageFilePath;
            //io.imageIndex = WTGOperation.wimpart;
            //io.isWimboot = checkBoxwimboot.Checked;
            //io.win7togo = WTGOperation.win7togo;
            io.AutoChooseWimIndex();
            io.ImageApplyToUD();
            ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, WTGOperation.ud, wimbox.Text);
            BootFileOperation.BcdbootWriteUEFIBootFile(WTGOperation.ud, @"X:\", WTGOperation.bcdboot);
        }

        private void UefiGptVhdVhdx()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();
            //VHDOperation.CleanTemp();
            //vo.CreateVHD();
            //vo.VHDExtra();
            //vo.DetachVHD();
            //vo.CopyVHD();
            //if (!checkBoxfixed.Checked)
            //{
            //    vo.VHDDynamicSizeIns(WTGOperation.ud);
            //}

            if (System.IO.File.Exists(WTGOperation.ud + WTGOperation.win8VhdFile))
            {
                ////finish f = new finish();
                ////f.ShowDialog();
            }
            else
            {
                //                //MsgManager.getResString("Msg_VHDCreationError")
                //VHD文件创建出错！
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
                //MessageBox.Show("Win8 VHD文件不存在！，可到论坛发帖求助！\n建议将程序目录下logs文件夹打包上传，谢谢！","出错啦！",MessageBoxButtons .OK ,MessageBoxIcon.Error );
                //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum-88-1.html");
            }
        }

        private void NonUEFIVHDVHDX()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();
            if (!System.IO.File.Exists(WTGOperation.ud + WTGOperation.win8VhdFile))
            {
                //MsgManager.getResString("Msg_VHDCreationError")
                //Win8 VHD文件不存在！未知错误原因！
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
            }

            else if (!System.IO.File.Exists(WTGOperation.ud + "\\Boot\\BCD"))
            {
                //MsgManager.getResString("Msg_VHDBcdbootError")
                //VHD模式下BCDBOOT执行出错！
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDBcdbootError", MsgManager.ci));
                er.ShowDialog();
            }
            else if (!System.IO.File.Exists(WTGOperation.ud + "bootmgr"))
            {
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_bootmgrError", MsgManager.ci));
                er.ShowDialog();
                //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
            }
            else
            {
                Finish f = new Finish();
                f.ShowDialog();
            }

            //VHDOperation.CleanTemp();
            //vo.CreateVHD();
            //vo.VHDExtra();
            //vo.WriteBootFiles();
            //vo.DetachVHD();
            //vo.CopyVHD();

            //if (!checkBoxfixed.Checked)
            //{
            //    vo.VHDDynamicSizeIns(WTGOperation.ud);

            //}
            //NonUEFIVHDFinish();
        }

        private void UefiMbrVHDVHDX()
        {
            VHDOperation vo = new VHDOperation();
            vo.Execute();
            //VHDOperation.CleanTemp();
            //vo.CreateVHD();
            //vo.VHDExtra();
            //vo.DetachVHD();
            //vo.CopyVHD();
            //if (!checkBoxfixed.Checked)
            //{
            //    vo.VHDDynamicSizeIns(WTGOperation.ud);
            //}
            //if (checkBoxuefimbr.Checked)
            //{
            //    //BootFileOperation.BooticeWriteMBRPBRAndAct("X:");
            //    //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=X: /pbr /install /type=bootmgr /quiet"));//写入引导
            //    //pbr.WaitForExit();
            //    //System.Diagnostics.Process act = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\bootice.exe", " /DEVICE=X: /partitions /activate /quiet");
            //    //act.WaitForExit();
            //}

            if (System.IO.File.Exists(WTGOperation.ud + WTGOperation.win8VhdFile))
            {
                //finish f = new finish();
                //f.ShowDialog();
            }
            else
            {
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
                er.ShowDialog();
                //shouldcontinue = false;
            }
            //removeLetterX();

            Finish f = new Finish();
            f.ShowDialog();
        }



        private void UEFIMBRTypical()
        {
            ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
            //IMAGEX解压
            ImageOperation.ImageApply(checkBoxwimboot.Checked, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.imageFilePath, WTGOperation.wimpart, WTGOperation.ud, WTGOperation.ud);

            //安装EXTRA
            ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, WTGOperation.ud, wimbox.Text);
            //BCDBOOT WRITE BOOT FILE    
            BootFileOperation.BcdbootWriteALLBootFileToXAndAct(WTGOperation.bcdboot, WTGOperation.ud);

        }

        private void NonUEFITypical()
        {
            ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
            ImageOperation.ImageApply(checkBoxwimboot.Checked, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.imageFilePath, WTGOperation.wimpart, WTGOperation.ud, WTGOperation.ud);
            if (WTGOperation.win7togo != 0) { ImageOperation.Win7REG(WTGOperation.ud); }

            BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
            ProcessManager.ECMD(Application.StartupPath + "\\files\\" + WTGOperation.bcdboot, WTGOperation.ud.Substring(0, 3) + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");

            if (WTGOperation.win7togo == 0) { ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, WTGOperation.ud, wimbox.Text); }

            if (!System.IO.File.Exists(WTGOperation.ud + "\\Boot\\BCD"))
            {
                //MsgManager.getResString("Msg_BCDError")
                //引导文件写入出错！boot文件夹不存在！
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_BCDError", MsgManager.ci));
                er.ShowDialog();
                //MessageBox.Show("引导文件写入出错！boot文件夹不存在\n请看论坛教程！", "出错啦", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //System.Diagnostics.Process.Start("http://bbs.luobotou.org/thread-1625-1-1.html");
            }
            else if (!System.IO.File.Exists(WTGOperation.ud + "bootmgr"))
            {
                //MsgManager.getResString("Msg_bootmgrError")
                //文件写入出错！bootmgr不存在！\n请检查写入过程是否中断
                ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_bootmgrError", MsgManager.ci));
                er.ShowDialog();

                //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
            }
            else
            {
                Finish f = new Finish();
                f.ShowDialog();
            }
        }

        #endregion

        private void WriteProgramRunInfoToLog()
        {
            try { Log.WriteLog("Environment.log", "App Version:" + Application.ProductVersion + "\r\nApp Path:" + Application.StartupPath + "\r\nOSVersion:" + System.Environment.OSVersion.ToString() + "\r\nDism Version:" + FileOperation.GetFileVersion(System.Environment.GetEnvironmentVariable("windir") + "\\System32\\dism.exe") + "\r\nWim file:" + wimbox.Text + "\r\nUsb Disk:" + WTGOperation.udstring + "\r\nClassical:" + radiochuantong.Checked.ToString() + "\r\nVHD:" + radiovhd.Checked.ToString() + "\r\nVHDX:" + radiovhdx.Checked.ToString() + "\r\nRe-Partition:" + checkBoxdiskpart.Checked + "\r\nVHD Size Set:" + numericUpDown1.Value.ToString() + "\r\nFixed VHD:" + checkBoxfixed.Checked.ToString() + "\r\nDonet:" + checkBoxframework.Checked.ToString() + "\r\nDisable-WinRE:" + checkBoxdiswinre.Checked.ToString() + "\r\nBlock Local Disk:" + checkBox_san_policy.Checked.ToString() + "\r\nNoTemp:" + checkBoxnotemp.Checked.ToString() + "\r\nUEFI+GPT:" + checkBoxuefi.Checked.ToString() + "\r\nUEFI+MBR:" + checkBoxuefimbr.Checked.ToString() + "\r\nWIMBOOT:" + checkBoxwimboot.Checked.ToString() + "\r\nCommon-VHD-boot-file：" + checkBoxcommon.Checked.ToString() + "\r\nNo-format：" + checkBoxunformat.Checked.ToString()); }
            catch (Exception ex) { MessageBox.Show("Error!\n" + ex.ToString()); }

            if (File.Exists(Environment.GetEnvironmentVariable("windir") + "\\Logs\\DISM\\dism.log"))
            {
                File.Copy(Environment.GetEnvironmentVariable("windir") + "\\Logs\\DISM\\dism.log", Application.StartupPath + "\\logs\\dism.log");
            }
        }


        private void removeLetterX()
        {
            try
            {
                using (FileStream fs0 = new FileStream(WTGOperation.diskpartscriptpath + "\\removex.txt", FileMode.Create, FileAccess.Write))
                {
                    fs0.SetLength(0);
                    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
                    {
                        string ws0 = "";
                        ws0 = "select volume x";
                        sw0.WriteLine(ws0);
                        ws0 = "remove";
                        sw0.WriteLine(ws0);
                        ws0 = "exit";
                        sw0.WriteLine(ws0);
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Log.WriteLog("removeLetterX.log", ex.ToString());
            }
            ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\removex.txt\"");
        }


        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            try { if (threadwrite != null && threadwrite.IsAlive) { MessageBox.Show(MsgManager.getResString("Msg_WriteProcessing", MsgManager.ci)); return; } }
            catch (Exception ex)
            {
                Log.WriteLog("Msg_WriteProcessing.log", ex.ToString());
                //Console.WriteLine(ex);
            }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//



            WTGOperation.udstring = comboBox1.SelectedItem.ToString();
            WTGOperation.iswimboot = checkBoxwimboot.Checked;
            WTGOperation.framework = checkBoxframework.Checked;
            WTGOperation.san_policy = checkBox_san_policy.Checked;
            WTGOperation.diswinre = checkBoxdiswinre.Checked;
            WTGOperation.imageFilePath = wimbox.Text;
            //WTGOperation.wimpart = ChoosePart.part;
            WTGOperation.userSetSize = (int)numericUpDown1.Value;
            WTGOperation.isfixed = checkBoxfixed.Checked;
            WTGOperation.isuefigpt = checkBoxuefi.Checked;
            WTGOperation.isuefimbr = checkBoxuefimbr.Checked;
            WTGOperation.isnotemp = checkBoxnotemp.Checked;
            WTGOperation.commonbootfiles = checkBoxcommon.Checked;
            if (radiovhdx.Checked)
            {
                WTGOperation.win8VhdFile = "win8.vhdx";
            }
            threadwrite = new Thread(new ThreadStart(goWrite));
            threadwrite.Start();

        }

        private void isobutton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (System.IO.File.Exists(openFileDialog1.FileName)) { wimbox.Text = openFileDialog1.FileName; }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath.Length != 3)
            {
                if (folderBrowserDialog1.SelectedPath != "")
                {
                    //MsgManager.getResString("Msg_UDRoot")
                    //请选择优盘根目录
                    MessageBox.Show(MsgManager.getResString("Msg_UDRoot", MsgManager.ci));
                }
                return;

            }
            UDList.Add(folderBrowserDialog1.SelectedPath);
            OutText(true, UDList);

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");
            //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (threadwrite != null && threadwrite.IsAlive)
                {
                    //MsgManager.getResString("Msg_WritingAbort")
                    //正在写入，您确定要取消吗？
                    if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_WritingAbort", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { e.Cancel = true; return; }
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


        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum-88-1.html");
        }



        private void imagex解压写入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            if (!System.IO.File.Exists(wimbox.Text)) { MessageBox.Show(MsgManager.getResString("Msg_chooseinstallwim", MsgManager.ci), MsgManager.getResString("Msg_error", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_ConfirmChoose", MsgManager.ci) + WTGOperation.ud.Substring(0, 1) + MsgManager.getResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGOperation.ud) / 1024 / 1024 + MsgManager.getResString("Msg_FormatTip", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + WTGOperation.imagex, " /apply " + "\"" + wimbox.Text + "\"" + " " + WTGOperation.wimpart + " " + WTGOperation.ud);
            p.WaitForExit();
            //MsgManager.getResString("Msg_Complete")
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));
        }

        private void 写入引导文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }

            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_ConfirmChoose", MsgManager.ci) + WTGOperation.ud.Substring(0, 1) + MsgManager.getResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGOperation.ud) / 1024 / 1024 + MsgManager.getResString("Msg_FormatTip", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            //MessageBox.Show("\"" + Application.StartupPath + "\\files\\" + bcdboot + "\"  " + ud.Substring(0, 3) + "windows  /s  " + ud.Substring(0, 2));
            ProcessManager.ECMD(Application.StartupPath + "\\files\\" + WTGOperation.bcdboot, WTGOperation.ud.Substring(0, 3) + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");

            BootFileOperation.BcdbootWriteALLBootFile(WTGOperation.ud, WTGOperation.ud, WTGOperation.bcdboot);
            //ProcessManager.SyncCMD("\"" + Application.StartupPath + "\\files\\" + bcdboot + "\"  " + ud.Substring(0, 3) + "windows  /s  " + ud.Substring(0, 2)+" /f ALL");

            //System.Diagnostics.Process p1 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + bcdboot, "  " + ud.Substring(0, 3) + "windows  /s  " + ud.Substring(0, 2));
            //p1.WaitForExit();
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));
        }

        private void 设置活动分区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }

            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘

            if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_ConfirmChoose", MsgManager.ci) + WTGOperation.ud.Substring(0, 1) + MsgManager.getResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGOperation.ud) / 1024 / 1024 + MsgManager.getResString("Msg_FormatTip", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
            //System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\bootice.exe", " /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /partitions /activate /quiet");
            //p2.WaitForExit();
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));
        }

        private void 写入磁盘引导ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }

            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_ConfirmChoose", MsgManager.ci) + WTGOperation.ud.Substring(0, 1) + MsgManager.getResString("Msg_Disk_Space", MsgManager.ci) + DiskOperation.GetHardDiskSpace(WTGOperation.ud) / 1024 / 1024 + MsgManager.getResString("Msg_FormatTip", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo)) { return; }
            BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
            //System.Diagnostics.Process booice = System.Diagnostics.Process.Start(WTGOperation.filesPath+ "\\BOOTICE.exe", (" /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
            //booice.WaitForExit();
            //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\BOOTICE.exe", (" /DEVICE=" + WTGOperation.ud.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
            //pbr.WaitForExit();
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                LoadUDList();

            }
        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            bcdboot9200.Checked = false;
            WTGOperation.bcdboot = "bcdboot7601.exe";
        }

        private void bcdboot9200_Click(object sender, EventArgs e)
        {
            bcdboot7601.Checked = false;
            WTGOperation.bcdboot = "bcdboot.exe";
        }

        private void 选择安装分卷ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChoosePart frmf = new ChoosePart();
            frmf.Show();
        }

        private void 强制格式化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WTGOperation.forceformat = true;
        }





        private void wimbox_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            bool mount_successfully = false;
            if (System.IO.File.Exists(openFileDialog1.FileName))
            {

                wimbox.Text = openFileDialog1.FileName;
                WTGOperation.imageFilePath = openFileDialog1.FileName;
                //Regex.Match ("",@".+\.[")
                WTGOperation.filetype = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
                //MessageBox.Show(WTGOperation.filetype);
                if (WTGOperation.filetype == "iso")
                {

                    //ProcessManager.ECMD(Application.StartupPath + "\\isocmd.exe  -i");
                    ProcessManager.SyncCMD("\"" + WTGOperation.filesPath+ "\\isocmd.exe\" -i");
                    ProcessManager.SyncCMD("\"" + WTGOperation.filesPath+ "\\isocmd.exe\" -s");
                    ProcessManager.SyncCMD("\"" + WTGOperation.filesPath+ "\\isocmd.exe\" -NUMBER 1");
                    ProcessManager.SyncCMD("\"" + WTGOperation.filesPath+ "\\isocmd.exe\" -eject 0: ");
                    ProcessManager.SyncCMD("\"" + WTGOperation.filesPath+ "\\isocmd.exe\" -MOUNT 0: \"" + openFileDialog1.FileName + "\"");
                    //mount.WaitForExit();
                    for (int i = 68; i <= 90; i++)
                    {
                        string ascll_to_eng = Convert.ToChar(i).ToString();
                        if (File.Exists(ascll_to_eng + ":\\sources\\install.wim"))
                        {
                            wimbox.Text = ascll_to_eng + ":\\sources\\install.wim";
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
                else if (WTGOperation.filetype == "esd")
                {
                    if (!WTGOperation.allowEsd)
                    {
                        //MsgManager.getResString("Msg_ESDError")
                        //此系统不支持ESD文件处理！");
                        MessageBox.Show(MsgManager.getResString("Msg_ESDError", MsgManager.ci));

                        return;
                    }
                    else
                    {
                        //MessageBox.Show("Test");
                        WTGOperation.isesd = true;
                        checkBoxwimboot.Checked = false;
                        checkBoxwimboot.Enabled = false;
                    }

                }
                else if (WTGOperation.filetype == "vhd")
                {
                    if (!radiovhd.Enabled)
                    {
                        radiovhd.Checked = true;
                        radiochuantong.Enabled = false;
                        radiovhdx.Enabled = false;


                        //checkBoxcommon.Checked = true;
                        //checkBoxcommon.Enabled = false;

                        //MessageBox.Show("此系统不支持VHD文件处理！"); 
                    }
                    else
                    {
                        radiovhd.Checked = true;
                        radiochuantong.Enabled = false;
                        radiovhdx.Enabled = false;
                    }
                }
                else if (WTGOperation.filetype == "vhdx")
                {
                    if (!radiovhd.Enabled)
                    {
                        radiovhdx.Checked = true;
                        radiochuantong.Enabled = false;
                        radiovhd.Enabled = false;

                        checkBoxcommon.Checked = true;
                        checkBoxcommon.Enabled = false;

                        //MessageBox.Show("此系统不支持VHD文件处理！"); 
                    }
                    else
                    {
                        radiovhdx.Checked = true;
                        radiochuantong.Enabled = false;
                        radiovhd.Enabled = false;
                    }

                }
                else
                {
                    WTGOperation.win7togo = ImageOperation.Iswin7(WTGOperation.imagex, wimbox.Text);
                    if (WTGOperation.win7togo != 0) //WIN7 cannot comptible with VHDX disk or wimboot
                    {
                        if (radiovhdx.Checked) { radiovhd.Checked = true; }
                        radiovhdx.Enabled = false;
                        checkBoxwimboot.Checked = false;
                        checkBoxwimboot.Enabled = false;
                    }
                }

            }

        }

        private void label1_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=2427&extra=page%3D1");

        }



        private void 萝卜头IT论坛ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitWeb("http://bbs.luobotou.org");

        }

        private void 创建VHDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            VHDOperation vo = new VHDOperation();
            vo.CreateVHD();

        }

        private void 向V盘写入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + WTGOperation.imagex, " /apply " + "\"" + wimbox.Text + "\"" + " " + WTGOperation.wimpart.ToString() + " " + "v:\\");
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
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            ProcessManager.ECMD("takeown.exe", " /f \"" + WTGOperation.ud + "\\boot\\" + "\" /r /d y && icacls \"" + WTGOperation.ud + "\\boot\\" + "\" /grant administrators:F /t");



            ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + " /e /h /y");


            //copyvhdbootfile();

        }

        private void 复制win8vhdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            VHDOperation vo = new VHDOperation();
            vo.CopyVHD();
            //Copy(System.Environment.GetEnvironmentVariable("TEMP") + "\\" + win8vhdfile, ud);
            //copy cp = new copy(ud);
            //cp.ShowDialog();

            //Copy(System.Environment.GetEnvironmentVariable("TEMP") + "\\" + win8vhdfile, ud + win8vhdfile);

        }

        private void 清理临时文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VHDOperation.CleanTemp();
        }




        private void checkBoxuefi_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxuefimbr.Checked && checkBoxuefi.Checked) { checkBoxuefimbr.Checked = false; checkBoxuefi.Checked = true; }
            checkBoxdiskpart.Enabled = !checkBoxuefi.Checked;
            checkBoxdiskpart.Checked = checkBoxuefi.Checked;
        }

        private void radiovhd_EnabledChanged(object sender, EventArgs e)
        {
            radiochuantong.Checked = true;
        }

        private void bootsectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            //
            System.Diagnostics.Process p1 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\" + "\\bootsect.exe", " /nt60 " + WTGOperation.ud.Substring(0, 2) + " /force /mbr");
            p1.WaitForExit();
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));
        }



        private void diskpart重新分区ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DiskOperation.DiskPartReformatUD();
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
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            //MsgManager.getResString("Msg_XPNotCOMP")
            //XP系统不支持此操作
            //MsgManager.getResString("Msg_ClearPartition")
            //此操作将会清除移动磁盘所有分区的所有数据，确认？
            if (System.Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2")) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            if (DialogResult.No == MessageBox.Show(MsgManager.getResString("Msg_ClearPartition", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }

            //if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 
            //Msg_Complete
            DiskOperation.DiskPartReformatUD();
            //diskPart();
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));

        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void vHD扩容ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (System.Environment.OSVersion.ToString().Contains("5.1") || System.Environment.OSVersion.ToString().Contains("5.2")) { MessageBox.Show("XP系统不支持此操作！"); return; }
            //vhdexpand vdp = new vhdexpand();
            //vdp.Show();
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
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
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
            MessageBox.Show(MsgManager.getResString("Msg_UpdateTip", MsgManager.ci));
        }

        private void checkBoxdiskpart_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxdiskpart.Checked)
            {
                //Msg_Repartition
                MessageBox.Show(MsgManager.getResString("Msg_Repartition", MsgManager.ci), MsgManager.getResString("Msg_warning", MsgManager.ci), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBoxunformat.Checked = false;
                checkBoxunformat.Enabled = false;
            }
            else
            {
                checkBoxunformat.Enabled = true;
            }
        }

        private void 选择安装分卷ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChoosePart frmf = new ChoosePart();
            frmf.Show();

        }


        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            try
            {
                toolTip1.SetToolTip(this.comboBox1, comboBox1.SelectedItem.ToString()); ;
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
            System.Diagnostics.Process.Start(Application.StartupPath + "\\files\\bootice.exe");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + "\\logs");

        }

        private void checkBoxuefimbr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxuefi.Checked && checkBoxuefimbr.Checked) { checkBoxuefi.Checked = false; checkBoxuefimbr.Checked = true; }
            if (checkBoxuefimbr.Checked) { checkBoxdiswinre.Checked = true; }
            checkBoxdiskpart.Enabled = !checkBoxuefimbr.Checked;
            checkBoxdiskpart.Checked = checkBoxuefimbr.Checked;
        }
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



        private void checkBoxunformat_CheckedChanged(object sender, EventArgs e)
        {
            //MsgManager.getResString("Msg_OnlyNonUefi")
            //此选项仅在非UEFI模式下有效！
            if (checkBoxunformat.Checked) { MessageBox.Show(MsgManager.getResString("Msg_OnlyNonUefi", MsgManager.ci)); }
        }

        private void toolStripMenuItemvhdx_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘

            ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + " /e /h /y");
            ProcessManager.ECMD("xcopy.exe", "\"" + WTGOperation.filesPath+ "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");


        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            SetTempPath stp = new SetTempPath();
            stp.Show();

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

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            EfiSize efz = new EfiSize(efisize);
            efz.ShowDialog();
            efisize = efz.EfiSz;

        }

        private void 修复盘符ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) { MessageBox.Show(MsgManager.getResString("Msg_chooseud", MsgManager.ci)); return; }
            WTGOperation.ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            string vhdPath = string.Empty;
            if (File.Exists(vhdPath = WTGOperation.ud + "win8.vhd") || File.Exists(vhdPath = WTGOperation.ud + "win8.vhdx"))
            {
                VHDOperation vo = new VHDOperation();
                vo.AttachVHD(vhdPath);
                ImageOperation.Fixletter("C:", "V:");
                vo.DetachVHD();
            }
            else
            {
                ImageOperation.Fixletter("C:", WTGOperation.ud.Substring(0, 2));
            }
            MessageBox.Show(MsgManager.getResString("Msg_Complete", MsgManager.ci));
        }
    }
}
