﻿#region VHDOperation
//private void createVHD(VHDOperation vo)
//{
//    if (WTGOperation.filetype == "vhd" || WTGOperation.filetype == "vhdx")
//    {
//        //vo.vhdPath = openFileDialog1.FileName;
//        //vpath = openFileDialog1.FileName;
//        //if (Directory.Exists(Application.StartupPath + "\\VHD"))
//        //{ Directory.Delete(Application.StartupPath + "\\VHD"); }
//        //Directory.CreateDirectory(Application.StartupPath + "\\VHD");
//        this.AttachVHD();
//        //ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");
//        //
//    }
//    else
//    {

//        vo.SetVhdProp();
//        vo.ApplyToVdisk();
//        vo.UEFIAndWin7ToGo();
//    }
//    vo.WriteBootFiles();
//    #region OLDCODE
//    ////    ////////////////vhd设定///////////////////////
//    ////    string vhd_type = "expandable";
//    ////    vhd_size = "";
//    //    if (checkBoxfixed.Checked)
//    //    {
//    //        vo.vhdType = "fixed";
//    //    }
//    //    else 
//    //    {
//    //        vo.vhdType = "expandable";
//    //    }
//    //    if (numericUpDown1.Value != 0)
//    //    {
//    //        vo.vhdSize  = (numericUpDown1.Value * 1024).ToString();
//    //    }
//    //    else
//    //    {
//    //        if (!checkBoxwimboot.Checked)
//    //        {
//    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 21504) { vo.vhdSize = "20480"; }
//    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 500).ToString(); }
//    //        }
//    //        else
//    //        {
//    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 24576) { vo.vhdSize = "20480"; }
//    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 4096).ToString(); }
//    //        }
//    //    }
//    //    //needcopy = false;
//    //    WTGOperation.wimpart = ChoosePart.part;
//    //    ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
//    //    ////win7////
//    //    //int win7togo = iswin7(win8iso);
//    //    //if (win7togo != 0 && radiovhdx.Checked) { MessageBox.Show("WIN7 WTG系统不支持VHDX模式！"); return; }
//    //    //if (wimpart == 0)
//    //    //{//自动判断模式

//    //    //    if (win7togo == 1)
//    //    //    {//WIN7 32 bit

//    //    //        wimpart = 5;
//    //    //    }
//    //    //    else if (win7togo == 2)
//    //    //    { //WIN7 64 BIT

//    //    //        wimpart = 4;
//    //    //    }
//    //    //    else { wimpart = 1; }
//    //    //}
//    //    //MessageBox.Show(wimpart.ToString());
//    //    //////////////

//    //    ////////////////判断临时文件夹,VHD needcopy?///////////////////
//    //    int vhdmaxsize;
//    //    if (checkBoxfixed.Checked)
//    //    {
//    //        vhdmaxsize = System.Int32.Parse(vo.vhdSize ) * 1024 + 1024;
//    //    }
//    //    else
//    //    {
//    //        vhdmaxsize = 10485670;
//    //    }
//    //    if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || StringOperation.IsChina(SetTempPath.temppath) || checkBoxuefi.Checked || checkBoxuefimbr.Checked || checkBoxwimboot.Checked || checkBoxnotemp.Checked)
//    //    {
//    //        vo.needcopy = false;
//    //        //usetemp = false;
//    //    }
//    //    else 
//    //    {
//    //        vo.needcopy = true;
//    //        //usetemp = true;
//    //    }
//    //    if (vo.needcopy)
//    //    {
//    //        vo.needcopy = false;
//    //        //usetemp = false;
//    //        vo.vhdPath = WTGOperation.ud + win8vhdfile;
//    //    }
//    //    else
//    //    {
//    //        vo.vhdPath = Path.Combine(SetTempPath.temppath, win8vhdfile);
//    //        //SetTempPath.temppath + "\\" + win8vhdfile;
//    //        //needcopy = true;
//    //    }


//    //    /////////////////////////////////////////////////////

//    //    FileStream fs = new FileStream(diskpartscriptpath + "\\create.txt", FileMode.Create, FileAccess.Write);
//    //    fs.SetLength(0);
//    //    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
//    //    string ws = "";
//    //    try
//    //    {
//    //        ws = "create vdisk file=" + vo.vhdPath + " type=" + vhd_type + " maximum=" + vhd_size;
//    //        sw.WriteLine(ws);
//    //        ws = "select vdisk file=" + vo.vhdPath;
//    //        sw.WriteLine(ws);
//    //        ws = "attach vdisk";
//    //        sw.WriteLine(ws);
//    //        ws = "create partition primary";
//    //        sw.WriteLine(ws);
//    //        ws = "format fs=ntfs quick";
//    //        sw.WriteLine(ws);
//    //        ws = "assign letter=v";
//    //        sw.WriteLine(ws);
//    //        ws = "exit";
//    //        sw.WriteLine(ws);
//    //    }
//    //    catch { }
//    //    sw.Close();
//    //    ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");


//    //    try
//    //    {
//    //        if (!System.IO.Directory.Exists("V:\\"))
//    //        {
//    //            ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
//    //            er.ShowDialog();
//    //            shouldcontinue = false;
//    //            return;
//    //        }
//    //    }
//    //    catch
//    //    {
//    //        //创建VHD失败，未知错误
//    //        ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
//    //        er.ShowDialog();
//    //        shouldcontinue = false;

//    //    }
//    //ImageOperation.ImageApply(checkBoxwimboot.Checked, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.path, WTGOperation.wimpart, WTGOperation.ud, @"v:\");
//    //    //if (checkBoxwimboot.Checked)
//    //    //{
//    //    //    ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + WTGOperation.ud + "wimboot.wim");
//    //    //    ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + WTGOperation.ud + "wimboot.wim" + "\" /ApplyDir:v: /Index:" + wimpart.ToString() + " /WIMBoot");

//    //    //}
//    //    //else
//    //    //{
//    //    //    if (isesd)
//    //    //    {
//    //    //        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:v: /Index:" + wimpart.ToString());


//    //    //    }
//    //    //    else
//    //    //    {
//    //    //        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + "v:\\");

//    //    //    }
//    //    //}

//    //////////////
//    //if (WTGOperation.win7togo != 0) { ImageOperation.Win7REG("V:\\"); }
//    ////////////////
//    //if (checkBoxuefi.Checked)
//    //{
//    //    ImageOperation.Fixletter("C:", "V:");
//    //    //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
//    //}
//    //}

//    //if (!usetemp)
//    //{
//    //    if (checkBoxuefi.Checked)
//    //    {
//    //        BootFileOperation.BcdbootWriteUEFIBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);

//    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");


//    //    }
//    //    else if (checkBoxuefimbr.Checked)
//    //    {
//    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);
//    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");


//    //    }
//    //    else if (checkBoxwimboot.Checked)
//    //    {
//    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

//    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");


//    //    }
//    //    else
//    //    {
//    //        if (!checkBoxcommon.Checked)
//    //        {
//    //            BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

//    //            //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");

//    //        }
//    //        else
//    //        {
//    //            needcopyvhdbootfile = true;
//    //            //copyvhdbootfile();
//    //        }
//    //    }
//    //}
//    //else
//    //{
//    //    needcopyvhdbootfile = true;
//    //}
//    #endregion
//}
//private void copyVHD()
//{


//    if (usetemp)
//    {
//        if (System.IO.File.Exists(vo.vhdPath ))
//        {
//            //Application.DoEvents();
//            //Thread.Sleep(100);
//            //Application.DoEvents();

//            ProcessManager.ECMD(Application.StartupPath + "\\files" + "\\fastcopy.exe", " /auto_close \"" + vo.vhdPath  + "\" /to=\"" + WTGOperation.ud + "\"");
//            //MsgManager.getResString("Msg_Copy")
//            //复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n
//            ProcessManager.AppendText (MsgManager.getResString("Msg_Copy", MsgManager.ci));


//            //AppendText("复制文件中...大约需要10分钟~1小时，请耐心等待！");
//            //wp.Show();
//            //Application.DoEvents();
//            //System.Diagnostics.Process cp = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\fastcopy.exe", " /auto_close \"" + Form1.vpath + "\" /to=\"" + ud + "\"");
//            //cp.WaitForExit();
//            //wp.Close();
//        }
//        if ((WTGOperation.filetype == "vhd" && !vo.vhdPath.EndsWith("win8.vhd")) || (WTGOperation.filetype == "vhdx" && !vo.vhdPath.EndsWith("win8.vhdx")))
//        {
//            //Rename
//            //MsgManager.getResString("Msg_RenameError")
//            //重命名错误
//            try { File.Move(WTGOperation.ud + vo.vhdPath.Substring(vo.vhdPath.LastIndexOf("\\") + 1), WTGOperation.ud + Form1.win8vhdfile); }
//            catch (Exception ex) { MessageBox.Show(MsgManager.getResString("Msg_RenameError", MsgManager.ci) + ex.ToString()); }
//        }

//        //copy cp = new copy(ud);
//        //cp.ShowDialog();

//    }

//}
//private void VHDExtra()
//{
//    ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, "v:", wimbox.Text);
//    //try
//    //{
//    //    ////////////.net 3.5//////////////////
//    //    if (checkBoxframework.Checked)
//    //    {
//    //        ProcessManager.ECMD("dism.exe", " /image:v: /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
//    //        

//    //    }
//    //    /////////////////屏蔽本机硬盘///////////////////////////////////
//    //    if (checkBox_san_policy.Checked)
//    //    {
//    //        ProcessManager.ECMD("dism.exe", " /image:v: /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
//    //        
//    //    }
//    //    /////////////////////禁用WINRE//////////////////////////////
//    //    if (checkBoxdiswinre.Checked)
//    //    {
//    //        File.Copy(Application.StartupPath + "\\files\\unattend.xml", "v:\\Windows\\System32\\sysprep\\unattend.xml");
//    //    }
//    //    //////////////
//    //}
//    //catch (Exception ex)
//    //{
//    //    MessageBox.Show(ex.ToString());
//    //}

//}

//private void copyVHDBootFiles()
//{
//    vo.CopyVHD();

//    ProcessManager.ECMD("xcopy.exe", "\"" + Application.StartupPath + "\\files" + "\\" + "vhd" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + " /e /h /y");

//    if (radiovhdx.Checked)
//    {
//        ProcessManager.ECMD("xcopy.exe", "\"" + Application.StartupPath + "\\files" + "\\" + "vhdx" + "\\" + "*.*" + "\"" + " " + WTGOperation.ud + "\\boot\\ /e /h /y");

//    }
//    BootFileOperation.BooticeWriteMBRPBRAndAct(WTGOperation.ud);
//    ///////////////////////////////////////////////////////
//    //System.Diagnostics.Process booice = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
//    //booice.WaitForExit();
//    //System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
//    //pbr.WaitForExit();
//    //System.Diagnostics.Process act = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\bootice.exe", " /DEVICE=" + ud.Substring(0, 2) + " /partitions /activate /quiet");
//    //act.WaitForExit();

//}
//private void detachVHDExtra() 
//{
//    if (File.Exists(diskpartscriptpath + "\\vdisklist.txt")) { File.Delete(diskpartscriptpath + "\\vdisklist.txt"); }
//    ///////////////////detach vdisk/////////////////////
//    FileStream fs1 = new FileStream(diskpartscriptpath + "\\vdisklist.txt", FileMode.Create, FileAccess.Write);
//    fs1.SetLength(0);
//    StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
//    string ws = "";
//    try
//    {

//        ws = "list vdisk";
//        sw1.WriteLine(ws);
//    }
//    catch { }
//    sw1.Close();
//    ProcessManager.SyncCMD("diskpart.exe /s \"" + diskpartscriptpath + "\\vdisklist.txt" + "\" > " + "\"" + diskpartscriptpath + "\\vhdlist.txt" + "\"");
//    FileStream fs2 = new FileStream(diskpartscriptpath + "\\vhdlist.txt", FileMode.Open);
//    StreamReader sr1 = new StreamReader(fs2,Encoding .Default );
//    string dpoutput = sr1.ReadToEnd();
//    int currentindex=0;
//    int win8vhdindex=0;
//    string vhdname = "";
//    if (dpoutput.Contains("win8.vhdx")) 
//    {
//        vhdname = "win8.vhdx";

//    }
//    else if (dpoutput.Contains("win8.vhd")) 
//    {
//        vhdname = "win8.vhd";
//    }
//    currentindex = dpoutput.IndexOf(vhdname) - 1;
//    win8vhdindex = currentindex;
//    while (currentindex > 0) 
//    {
//        if (dpoutput[currentindex].ToString () == ":") { break; }
//        currentindex--;
//    }
//    string vhdmountpath = "";
//    if (currentindex > 0)
//    {
//        vhdmountpath = dpoutput.Substring(currentindex - 1, win8vhdindex - currentindex + 2) + vhdname;
//        detachVHD(vhdmountpath);
//    }

//    //MessageBox.Show(vhdmountpath);


//}
//private void VHDDynamicSizeIns()
//{
//    //MsgManager.getResString("FileName_VHD_Dynamic")
//    //VHD模式说明.TXT
//    FileStream fs1 = new FileStream(ud + MsgManager.getResString("FileName_VHD_Dynamic", MsgManager.ci), FileMode.Create, FileAccess.Write);
//    fs1.SetLength(0);
//    StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
//    string ws1 = "";
//    try
//    {
//        //MsgManager.getResString("Msg_VHDDynamicSize")
//        //您创建的VHD为动态大小VHD，实际VHD容量：
//        ////MsgManager.getResString("Msg_VHDDynamicSize2")
//        //在VHD系统启动后将自动扩充为实际容量。请您在优盘留有足够空间确保系统正常启动！
//        ws1 = MsgManager.getResString("Msg_VHDDynamicSize", MsgManager.ci) + vhd_size + "MB\n";
//        sw1.WriteLine(ws1);
//        ws1 = MsgManager.getResString("Msg_VHDDynamicSize2", MsgManager.ci);
//        sw1.WriteLine(ws1);
//    }
//    catch { }
//    sw1.Close();

//}

#endregion

#region OLDCODE
////判断是否WIN7，自动选择安装分卷
////int win7togo = iswin7(win8iso);


//if (wimpart == 0)
//{//自动判断模式

//    if (win7togo == 1)
//    {//WIN7 32 bit

//        wimpart = 5;
//    }
//    else if (win7togo == 2)
//    { //WIN7 64 BIT

//        wimpart = 4;
//    }
//    else { wimpart = 1; }
//}

//if (checkBoxwimboot.Checked)
//{
//    ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + ud + "wimboot.wim");
//    
//    ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + ud + "wimboot.wim" + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString() + " /WIMBoot");
//    

//}
//else
//{
//    if (isesd)
//    {
//        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString());
//        
//    }
//    else
//    {
//        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + ud);
//        
//    }
//}


//if (checkBoxframework.Checked)
//{
//    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
//    

//}
//if (checkBox_san_policy.Checked)
//{
//    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
//    

//}

//if (checkBoxdiswinre.Checked)
//{
//    File.Copy(Application.StartupPath + "\\files\\unattend.xml", ud + "Windows\\System32\\sysprep\\unattend.xml");
//}


//ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, ud + "windows  /s  x: /f ALL");
//
//System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\bootice.exe", " /DEVICE=x: /partitions /activate  /quiet");
//p2.WaitForExit();

//finish f = new finish();
//f.ShowDialog();

#endregion
//using (FileStream fs = new FileStream(diskpartscriptpath + "\\create.txt", FileMode.Create, FileAccess.Write))
//{
//    fs.SetLength(0);
//    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
//    {
//        string ws = "";

//        ws = "create vdisk file=" + this.vhdPath + " type=" + this.vhdType + " maximum=" + this.vhdSize;
//        sw.WriteLine(ws);
//        ws = "select vdisk file=" + this.vhdPath;
//        sw.WriteLine(ws);
//        ws = "attach vdisk";
//        sw.WriteLine(ws);
//        ws = "create partition primary";
//        sw.WriteLine(ws);
//        ws = "format fs=ntfs quick";
//        sw.WriteLine(ws);
//        ws = "assign letter=v";
//        sw.WriteLine(ws);
//        ws = "exit";
//        sw.WriteLine(ws);

//    }
//}
//ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");


//private string GetRegistData(string name)
//{
//    string registData;
//    RegistryKey hkml = Registry.LocalMachine;
//    RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
//    RegistryKey aimdir = software.OpenSubKey("XXX", true);
//    registData = aimdir.GetValue(name).ToString();
//    return registData;
//} 

//    ImageOperation.ImageExtra(checkBoxframework.Checked, checkBox_san_policy.Checked, checkBoxdiswinre.Checked, "v:", wimbox.Text);
//    //try
//    //{
//    //    ////////////.net 3.5//////////////////
//    //    if (checkBoxframework.Checked)
//    //    {
//    //        ProcessManager.ExecuteCMD("dism.exe", " /image:v: /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
//    //        wp.ShowDialog();

//    //    }
//    //    /////////////////屏蔽本机硬盘///////////////////////////////////
//    //    if (checkBox_san_policy.Checked)
//    //    {
//    //        ProcessManager.ExecuteCMD("dism.exe", " /image:v: /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
//    //        wp.ShowDialog();
//    //    }
//    //    /////////////////////禁用WINRE//////////////////////////////
//    //    if (checkBoxdiswinre.Checked)
//    //    {
//    //        File.Copy(Application.StartupPath + "\\files\\unattend.xml", "v:\\Windows\\System32\\sysprep\\unattend.xml");
//    //    }
//    //    //////////////
//    //}
//    //catch (Exception ex)
//    //{
//    //    MessageBox.Show(ex.ToString());
//    //}

//}
//private void createVHD()
//{
//    if (WTGOperation.filetype == "vhd" || WTGOperation.filetype == "vhdx")
//    {
//        this.AttachVHD();
//    }
//    else
//    {

//        this.SetVhdProp();
//        this.ApplyToVdisk();
//        this.UEFIAndWin7ToGo();
//    }
//    this.WriteBootFiles();
//    #region OLDCODE
//    ////    ////////////////vhd设定///////////////////////
//    ////    string vhd_type = "expandable";
//    ////    vhd_size = "";
//    //    if (checkBoxfixed.Checked)
//    //    {
//    //        vo.vhdType = "fixed";
//    //    }
//    //    else 
//    //    {
//    //        vo.vhdType = "expandable";
//    //    }
//    //    if (numericUpDown1.Value != 0)
//    //    {
//    //        vo.vhdSize  = (numericUpDown1.Value * 1024).ToString();
//    //    }
//    //    else
//    //    {
//    //        if (!checkBoxwimboot.Checked)
//    //        {
//    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 21504) { vo.vhdSize = "20480"; }
//    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 500).ToString(); }
//    //        }
//    //        else
//    //        {
//    //            if (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 >= 24576) { vo.vhdSize = "20480"; }
//    //            else { vo.vhdSize = (DiskOperation.GetHardDiskFreeSpace(WTGOperation.ud) / 1024 - 4096).ToString(); }
//    //        }
//    //    }
//    //    //needcopy = false;
//    //    WTGOperation.wimpart = ChoosePart.part;
//    //    ImageOperation.AutoChooseWimIndex(ref WTGOperation.wimpart, WTGOperation.win7togo);
//    //    ////win7////
//    //    //int win7togo = iswin7(win8iso);
//    //    //if (win7togo != 0 && radiovhdx.Checked) { MessageBox.Show("WIN7 WTG系统不支持VHDX模式！"); return; }
//    //    //if (wimpart == 0)
//    //    //{//自动判断模式

//    //    //    if (win7togo == 1)
//    //    //    {//WIN7 32 bit

//    //    //        wimpart = 5;
//    //    //    }
//    //    //    else if (win7togo == 2)
//    //    //    { //WIN7 64 BIT

//    //    //        wimpart = 4;
//    //    //    }
//    //    //    else { wimpart = 1; }
//    //    //}
//    //    //MessageBox.Show(wimpart.ToString());
//    //    //////////////

//    //    ////////////////判断临时文件夹,VHD needcopy?///////////////////
//    //    int vhdmaxsize;
//    //    if (checkBoxfixed.Checked)
//    //    {
//    //        vhdmaxsize = System.Int32.Parse(vo.vhdSize ) * 1024 + 1024;
//    //    }
//    //    else
//    //    {
//    //        vhdmaxsize = 10485670;
//    //    }
//    //    if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || StringOperation.IsChina(SetTempPath.temppath) || checkBoxuefi.Checked || checkBoxuefimbr.Checked || checkBoxwimboot.Checked || checkBoxnotemp.Checked)
//    //    {
//    //        vo.needcopy = false;
//    //        //usetemp = false;
//    //    }
//    //    else 
//    //    {
//    //        vo.needcopy = true;
//    //        //usetemp = true;
//    //    }
//    //    if (vo.needcopy)
//    //    {
//    //        vo.needcopy = false;
//    //        //usetemp = false;
//    //        vo.vhdPath = WTGOperation.ud + win8vhdfile;
//    //    }
//    //    else
//    //    {
//    //        vo.vhdPath = Path.Combine(SetTempPath.temppath, win8vhdfile);
//    //        //SetTempPath.temppath + "\\" + win8vhdfile;
//    //        //needcopy = true;
//    //    }


//    //    /////////////////////////////////////////////////////

//    //    FileStream fs = new FileStream(diskpartscriptpath + "\\create.txt", FileMode.Create, FileAccess.Write);
//    //    fs.SetLength(0);
//    //    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
//    //    string ws = "";
//    //    try
//    //    {
//    //        ws = "create vdisk file=" + vo.vhdPath + " type=" + vhd_type + " maximum=" + vhd_size;
//    //        sw.WriteLine(ws);
//    //        ws = "select vdisk file=" + vo.vhdPath;
//    //        sw.WriteLine(ws);
//    //        ws = "attach vdisk";
//    //        sw.WriteLine(ws);
//    //        ws = "create partition primary";
//    //        sw.WriteLine(ws);
//    //        ws = "format fs=ntfs quick";
//    //        sw.WriteLine(ws);
//    //        ws = "assign letter=v";
//    //        sw.WriteLine(ws);
//    //        ws = "exit";
//    //        sw.WriteLine(ws);
//    //    }
//    //    catch { }
//    //    sw.Close();
//    //    ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\create.txt\"");


//    //    try
//    //    {
//    //        if (!System.IO.Directory.Exists("V:\\"))
//    //        {
//    //            ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
//    //            er.ShowDialog();
//    //            shouldcontinue = false;
//    //            return;
//    //        }
//    //    }
//    //    catch
//    //    {
//    //        //创建VHD失败，未知错误
//    //        ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
//    //        er.ShowDialog();
//    //        shouldcontinue = false;

//    //    }
//    //ImageOperation.ImageApply(checkBoxwimboot.Checked, WTGOperation.isesd, WTGOperation.imagex, WTGOperation.path, WTGOperation.wimpart, WTGOperation.ud, @"v:\");
//    //    //if (checkBoxwimboot.Checked)
//    //    //{
//    //    //    ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + WTGOperation.ud + "wimboot.wim");
//    //    //    ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + WTGOperation.ud + "wimboot.wim" + "\" /ApplyDir:v: /Index:" + wimpart.ToString() + " /WIMBoot");

//    //    //}
//    //    //else
//    //    //{
//    //    //    if (isesd)
//    //    //    {
//    //    //        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:v: /Index:" + wimpart.ToString());


//    //    //    }
//    //    //    else
//    //    //    {
//    //    //        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + "v:\\");

//    //    //    }
//    //    //}

//    //////////////
//    //if (WTGOperation.win7togo != 0) { ImageOperation.Win7REG("V:\\"); }
//    ////////////////
//    //if (checkBoxuefi.Checked)
//    //{
//    //    ImageOperation.Fixletter("C:", "V:");
//    //    //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:v  > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
//    //}
//    //}

//    //if (!usetemp)
//    //{
//    //    if (checkBoxuefi.Checked)
//    //    {
//    //        BootFileOperation.BcdbootWriteUEFIBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);

//    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f UEFI");


//    //    }
//    //    else if (checkBoxuefimbr.Checked)
//    //    {
//    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", @"X:\", WTGOperation.bcdboot);
//    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  x: /f ALL");


//    //    }
//    //    else if (checkBoxwimboot.Checked)
//    //    {
//    //        BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

//    //        //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");


//    //    }
//    //    else
//    //    {
//    //        if (!checkBoxcommon.Checked)
//    //        {
//    //            BootFileOperation.BcdbootWriteALLBootFile(@"V:\", WTGOperation.ud, WTGOperation.bcdboot);

//    //            //ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, "  " + "V:\\" + "windows  /s  " + WTGOperation.ud.Substring(0, 2) + " /f ALL");

//    //        }
//    //        else
//    //        {
//    //            needcopyvhdbootfile = true;
//    //            //copyvhdbootfile();
//    //        }
//    //    }
//    //}
//    //else
//    //{
//    //    needcopyvhdbootfile = true;
//    //}
//    #endregion
//}

#region detachVHDExtra
//FileOperation.DeleteFile(diskpartscriptpath + "\\vdisklist.txt");
/////////////////////detach vdisk/////////////////////
//File.WriteAllText(diskpartscriptpath + "\\vdisklist.txt", "list vdisk", Encoding.Default);
////FileStream fs1 = new FileStream(diskpartscriptpath + "\\vdisklist.txt", FileMode.Create, FileAccess.Write);
////fs1.SetLength(0);
////StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
////string ws = "";
////try
////{

////    ws = "list vdisk";
////    sw1.WriteLine(ws);
////}
////catch { }
////sw1.Close();
//ProcessManager.SyncCMD("diskpart.exe /s \"" + diskpartscriptpath + "\\vdisklist.txt" + "\" > " + "\"" + diskpartscriptpath + "\\vhdlist.txt" + "\"");
////FileStream fs2 = new FileStream(diskpartscriptpath + "\\vhdlist.txt", FileMode.Open);
////StreamReader sr1 = new StreamReader(fs2, Encoding.Default);
////string dpoutput = sr1.ReadToEnd();


//int currentindex = 0;
//int win8vhdindex = 0;
//string vhdname = "";
//if (dpoutput.Contains("win8.vhdx"))
//{
//    vhdname = "win8.vhdx";

//}
//else if (dpoutput.Contains("win8.vhd"))
//{
//    vhdname = "win8.vhd";
//}
//currentindex = dpoutput.IndexOf(vhdname) - 1;
//win8vhdindex = currentindex;
//while (currentindex > 0)
//{
//    if (dpoutput[currentindex].ToString() == ":") { break; }
//    currentindex--;
//}
////string vhdmountpath = "";
//if (currentindex > 0)
//{
//    this.vhdPath = dpoutput.Substring(currentindex - 1, win8vhdindex - currentindex + 2) + vhdname;
//    //vhdmountpath = 
//    detachVHD();
//}

//MessageBox.Show(vhdmountpath);
#endregion
#region cleanTempOldCode
//int vhdmaxsize;
//if (checkBoxfixed.Checked)
//{
//    vhdmaxsize = System.Int32.Parse(vhd_size) * 1048576 + 1024;
//}
//else
//{
//    vhdmaxsize = 10485670;
//}
//if (DiskOperation.GetHardDiskFreeSpace(SetTempPath.temppath.Substring(0, 2) + "\\") <= vhdmaxsize || IsChina(SetTempPath.temppath) || checkBoxuefi.Checked || checkBoxuefimbr.Checked || checkBoxwimboot.Checked)
//{
//    usetemp = false;
//}

//if (!usetemp)
//{
//    vpath = ud + win8vhdfile;
//}
//else
//{
//    vpath = SetTempPath.temppath + "\\" + win8vhdfile;
//    //needcopy = true;
//}
//detachVHD(vpath);
#endregion
//public void GenerateAttachVHDScript(string vpath)
//{
//    StringBuilder sb = new StringBuilder();

//    sb.AppendLine("select vdisk file=" + vpath);
//    sb.AppendLine("attach vdisk");
//    sb.AppendLine("sel partition 1");
//    sb.AppendLine("assign letter=v");
//    sb.AppendLine("exit");
//    DiskpartScriptManager dsm = new DiskpartScriptManager();
//    dsm.args = sb.ToString();
//    dsm.RunDiskpartScript(false);

//    //using (FileStream fs0 = new FileStream(diskpartscriptpath + @"\attach.txt", FileMode.Create, FileAccess.Write))
//    //{
//    //    fs0.SetLength(0);
//    //    using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
//    //    {
//    //        //string ws0 = "";
//    //        //StringBuilder sb = new StringBuilder();
//    //        //sb.Append("select vdisk file=" + vpath + "\n");
//    //        //try
//    //        //{
//    //        //ws0 = "select vdisk file=" + vpath;
//    //        sw0.WriteLine("select vdisk file=" + vpath);
//    //        sw0.WriteLine("attach vdisk");
//    //        sw0.WriteLine("sel partition 1");
//    //        sw0.WriteLine("assign letter=v");
//    //        sw0.WriteLine("exit");
//    //    }
//    //}
//}
//private void ad()
//{
//    string pageHtml1;
//    //MessageBox.Show("Test");
//    try
//    {
//        //MessageBox.Show("Test");
//        WebClient MyWebClient = new WebClient();

//        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

//        Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

//        pageHtml1 = Encoding.UTF8.GetString(pageData);
//        // MessageBox.Show(pageHtml1);
//        int index = pageHtml1.IndexOf("announcement=");
//        int indexbbs = pageHtml1.IndexOf("bbs=");
//        // MessageBox.Show(pageHtml1.Substring(index + 13, 1));
//        //MessageBox.Show(MsgManager.ci.EnglishName );
//        //CultureInfo ca = new CultureInfo("en");
//        if (pageHtml1.Substring(index + 13, 1) != "0" && MsgManager.ci.EnglishName != "English")
//        {
//            if (pageHtml1.Substring(indexbbs + 4, 1) == "1")
//            {
//                string pageHtml;


//                WebClient MyWebClient1 = new WebClient();

//                MyWebClient1.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

//                Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/portal.php"); //从指定网站下载数据

//                pageHtml = Encoding.UTF8.GetString(pageData1);
//                //MessageBox.Show(pageHtml);
//                int index1 = pageHtml.IndexOf("<ul><li><a href=");
//                for (int i = 0; i < 10; i++)
//                {
//                    int LinkStartIndex = pageHtml.IndexOf("<li><a href=", index1) + 13;
//                    int LinkEndIndex = pageHtml.IndexOf("\"", LinkStartIndex);
//                    int TitleStartIndex = pageHtml.IndexOf("title=", LinkEndIndex) + 7;
//                    int TitleEndIndex = pageHtml.IndexOf("\"", TitleStartIndex);

//                    topiclink[i] = pageHtml.Substring(LinkStartIndex, LinkEndIndex - LinkStartIndex);
//                    topicname[i] = pageHtml.Substring(TitleStartIndex, TitleEndIndex - TitleStartIndex);
//                    //MessageBox.Show(topiclink[i] + topicname[i]);
//                    index1 = LinkEndIndex;
//                    //topicstring 
//                    //int adprogram = index1 + Application.ProductName.Length + 1;

//                }
//                //string portal_block = pageHtml.Substring;
//                //String adtitle;
//                ////MessageBox.Show(adprogram.ToString() + " " + startindex);
//                //adtitle = pageHtml.Substring(adprogram, startindex - adprogram);

//                //adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
//                //linkLabel2.Invoke(Set_Text, new object[] { adtitle });
//                //MessageBox.Show("");

//                //MessageBox.Show(adtitle + "     " + adlink);

//            }

//            {

//                //MessageBox.Show("Test");
//                string pageHtml;
//                WebClient MyWebClient1 = new WebClient();

//                MyWebClient1.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。
//                MyWebClient1.Encoding = Encoding.UTF8;
//                pageHtml = MyWebClient1.DownloadString("http://bbs.luobotou.org/app/announcement.txt");

//                //Byte[] pageData1 = MyWebClient1.DownloadData("http://bbs.luobotou.org/app/announcement.txt"); //从指定网站下载数据
//                //pageHtml = Encoding.UTF8.GetString(pageData1);
//                //MessageBox.Show(pageHtml);
//                //int index1 = pageHtml.IndexOf(Application.ProductName);
//                //int startindex = pageHtml.IndexOf("~", index1);
//                //int endindex = pageHtml.IndexOf("结束", index1);
//                //int adprogram = index1 + Application.ProductName.Length + 1;
//                Match match = Regex.Match(pageHtml, Application.ProductName + "=(.+)~(.+)结束");
//                //Set_Text(match.Groups[1].Value);
//                adlink = match.Groups[2].Value;
//                String adtitle;
//                adtitle = match.Groups[1].Value;
//                ////MessageBox.Show(adprogram.ToString() + " " + startindex);
//                //adtitle = pageHtml.Substring(adprogram, startindex - adprogram);
//                //adtitles = adtitle;
//                //adlink = pageHtml.Substring(startindex + 1, endindex - startindex - 1);
//                //Set_Text(adtitle);
//                linkLabel2.Invoke(Set_Text, new object[] { adtitle });
//                //linkLabel2(Set_Text);
//                //MessageBox.Show("");
//                //writeprogress .linklabel1
//                //MessageBox.Show(adtitle + "     " + adlink);

//            }
//        }
//    }
//    catch { }


//}
//public void detachVHD()
//{
//    if (!ShouldContinue) return;
//    else ShouldContinue = false;
//    FileOperation.DeleteFile(diskpartscriptpath + "\\detach.txt");
//    ///////////////////detach vdisk/////////////////////
//    FileStream fs1 = new FileStream(diskpartscriptpath + "\\detach.txt", FileMode.Create, FileAccess.Write);
//    fs1.SetLength(0);
//    StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default);
//    string ws = "";
//    try
//    {
//        ws = "select vdisk file=" + VhdPath;
//        sw1.WriteLine(ws);
//        ws = "detach vdisk";
//        sw1.WriteLine(ws);
//    }
//    catch { }
//    sw1.Close();
//    ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\detach.txt\"");
//    ShouldContinue = true;
//}

//private void diskPart() 
//{

//    //ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
//    //if (DialogResult.No == MessageBox.Show("此操作将会清除移动磁盘所有分区的所有数据，确认？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
//    //if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 

//    FileStream fs0 = new FileStream(WTGOperation . diskpartscriptpath + "\\dp.txt", FileMode.Create, FileAccess.Write);
//    fs0.SetLength(0);
//    StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default);
//    string ws0 = "";
//    try
//    {
//        ws0 = "select volume " + WTGOperation.ud.Substring(0, 1);
//        sw0.WriteLine(ws0);
//        ws0 = "clean";
//        sw0.WriteLine(ws0);
//        ws0 = "convert mbr";
//        sw0.WriteLine(ws0);
//        ws0 = "create partition primary";
//        sw0.WriteLine(ws0);
//        ws0 = "select partition 1";
//        sw0.WriteLine(ws0);
//        ws0 = "format fs=ntfs quick";
//        sw0.WriteLine(ws0);
//        ws0 = "active";
//        sw0.WriteLine(ws0);
//        ws0 = "assign letter=" + WTGOperation.ud.Substring(0, 1);
//        sw0.WriteLine(ws0);
//        ws0 = "exit";
//        sw0.WriteLine(ws0);
//    }
//    catch { }
//    sw0.Close();

//    ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\dp.txt\"");

//    //System.Diagnostics.Process dpc = System.Diagnostics.Process.Start("diskpart.exe", " /s " + Application.StartupPath + "\\dp.txt");
//    //dpc.WaitForExit();
//}
//private void 错误提示测试ToolStripMenuItem_Click(object sender, EventArgs e)
//      {
//          //DiskpartScriptManager dsm = new DiskpartScriptManager();
//          //dsm.args = "list disk";
//          //dsm.RunDiskpartScript(true);
//          //MessageBox.Show(dsm.CreateScriptFile()); 
//          //vo.detachVHDExtra();

//          //ProcessManager.ECMD(Application.StartupPath + "\\files" + "\\fastcopy.exe", " /auto_close \"" + Form1.vpath + "\" /to=\"" + ud + "\"");
//          ////MsgManager.getResString("Msg_Copy")
//          ////复制文件中...大约需要10分钟~1小时，请耐心等待！\r\n
//          //AppendText(MsgManager.getResString("Msg_Copy", MsgManager.ci));

//          //
//          ////AppendText("test");
//          ////
//          ////WindowsImageContainer wic = new WindowsImageContainer("", WindowsImageContainer.CreateFileMode.OpenAlways, WindowsImageContainer.CreateFileAccess.Read);

//          //IImage im=null;

//          //im.Apply("");
//          //wic.a
//          //copyfile = new Thread(fc);
//          //copyfile.Start();

//          //System.Diagnostics.Process.Start("c:\\windows\\system32\\bcdboot.exe");
//          //MessageBox.Show(Environment.GetEnvironmentVariable("windir") + "\\system32\\bcdboot.exe");
//          //ProcessManager.ECMD(Environment.GetEnvironmentVariable("windir") + "\\system32\\bcdboot.exe", "  " + "V:\\" + "windows  /s  x: /f UEFI");
//          //
//          //MessageBox.Show(Application.ProductName);
//          //Fixletter("C:","J:");
//          //error ex = new error("测试错误信息！TEST!");
//          //ex.Show();
//      }

//private void ProcessManager.ECMD(string StartFileName, string StartFileArg)
//{

//    Process process = new Process();
//    //

//    try
//    {
//        AppendText("Command:" + StartFileName + StartFileArg+"\r\n");
//        process.StartInfo.FileName = StartFileName;
//        process.StartInfo.Arguments = StartFileArg;
//        process.StartInfo.UseShellExecute = false;
//        process.StartInfo.RedirectStandardInput = true;
//        process.StartInfo.RedirectStandardOutput = true;
//        process.StartInfo.RedirectStandardError = true;
//        process.StartInfo.CreateNoWindow = true;
//        process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
//        process.EnableRaisingEvents = true;
//        process.Exited += new EventHandler(progress_Exited);

//        process.Start();


//        process.BeginOutputReadLine();


//    }
//    catch (Exception ex)
//    {
//        //MsgManager.getResString("Msg_Failure")
//        //操作失败
//        MessageBox.Show(MsgManager.getResString("Msg_Failure", MsgManager.ci) + ex.ToString());
//    }

//}
//private void report()
//{
//    string pageHtml;
//    try
//    {

//        WebClient MyWebClient = new WebClient();

//        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

//        Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

//        pageHtml = Encoding.Default.GetString(pageData);
//        //MessageBox.Show(pageHtml);
//        int index = pageHtml.IndexOf("webreport=");

//        if (pageHtml.Substring(index + 10, 1) == "1")
//        {
//            string strURL = "http://myapp.luobotou.org/statistics.aspx?name=wtg&ver=" + Application.ProductVersion;
//            System.Net.HttpWebRequest request;
//            // 创建一个HTTP请求
//            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
//            //request.Method="get";
//            System.Net.HttpWebResponse response;
//            response = (System.Net.HttpWebResponse)request.GetResponse();
//            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
//            string responseText = myreader.ReadToEnd();
//            myreader.Close();

//        }


//    }
//    catch (WebException webEx)
//    {

//        Console.WriteLine(webEx.Message.ToString());

//    }
//}
//private void update()
//{
//    string autoup = IniFile.ReadVal("Main", "AutoUpdate", Application.StartupPath + "\\files\\settings.ini");
//    if (autoup == "0") { return; }
////if (IsRegeditExit(Application.ProductName)) { if ((GetRegistData("nevercheckupdate")) == "1") { return; } }

//    string pageHtml;
//    try
//    {

//        WebClient MyWebClient = new WebClient();
//        //MyWebClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

//        MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于对向Internet资源的请求进行身份验证的网络凭据。

//        Byte[] pageData = MyWebClient.DownloadData("http://bbs.luobotou.org/app/wintogo.txt"); //从指定网站下载数据

//        pageHtml = Encoding.UTF8 .GetString(pageData);
//       //essageBox.Show(pageHtml );
//        int index = pageHtml.IndexOf("~");
//        String ver;

//        ver = pageHtml.Substring(index + 1, 7);
//        if (ver != Application.ProductVersion)
//        {
//            try
//            {
//                Update frmf = new Update(ver);
//                frmf.ShowDialog();
//            }
//            catch { }

//        }

//    } 
//    catch (WebException webEx)
//    {

//        Console.WriteLine(webEx.Message.ToString());

//    }
//}

#region OLDCODE
//int win7togo = iswin7(win8iso);
//if (wimpart == 0)
//{//自动判断模式

//    if (win7togo == 1)
//    {//WIN7 32 bit

//        wimpart = 5;
//    }
//    else if (win7togo == 2)
//    { //WIN7 64 BIT

//        wimpart = 4;
//    }
//    else { wimpart = 1; }
//}
//if (checkBoxwimboot.Checked)
//{
//    ImageOperation.WimbootApply(win8iso, ud,wimpart );
//    //ProcessManager.ECMD("Dism.exe"," /Export-Image /WIMBoot /SourceImageFile:\""+win8iso+"\" /SourceIndex:"+wimpart .ToString ()+" /DestinationImageFile:"+ud+"wimboot.wim");
//    //
//    //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + ud + "wimboot.wim" + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString() + " /WIMBoot");
//    //

//}
//else
//{
//    if (isesd)
//    {
//        ImageOperation.ESDApply(win8iso, ud, wimpart);
//        //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString());
//        //
//    }
//    else
//    {
//        ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + ud);
//        
//    }
//}
/////////////
//try
//{
//    System.Diagnostics.Process booice = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
//    booice.WaitForExit();
//    System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", (" /DEVICE=" + ud.Substring(0, 2) + " /pbr /install /type=bootmgr  /quiet"));//写入引导
//    pbr.WaitForExit();
//    System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", " /DEVICE=" + ud.Substring(0, 2) + " /partitions /activate  /quiet");
//    p2.WaitForExit();
//}
//catch (Exception ex) { MessageBox.Show(ex.ToString()); }
//if (checkBoxframework.Checked)
//{
//    if (win7togo == 0)
//    {
//        ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
//        
//    }

//}
/////////////////////////////////////////////
//if (checkBox_san_policy.Checked)
//{
//    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
//    
//}
/////////////////////////////////////////////
//if (checkBoxdiswinre.Checked)
//{
//    File.Copy(Application.StartupPath + "\\files\\unattend.xml", ud + "Windows\\System32\\sysprep\\unattend.xml");
//}
///////////////////////////////////////////
#endregion
#region DetachVHD
//FileOperation.DeleteFile(diskpartscriptpath + "\\detach.txt");
///////////////////detach vdisk/////////////////////
//using (FileStream fs1 = new FileStream(diskpartscriptpath + "\\detach.txt", FileMode.Create, FileAccess.Write))
//{
//    fs1.SetLength(0);
//    using (StreamWriter sw1 = new StreamWriter(fs1, Encoding.Default))
//    {
//        sw1.WriteLine("select vdisk file=" + vhdPath);
//        sw1.WriteLine("detach vdisk");
//    }
//}
//ProcessManager.ECMD("diskpart.exe", " /s \"" + diskpartscriptpath + "\\detach.txt\"");
#endregion
      //private void NonUEFIVHDFinish()
      //  {

      //      if (!System.IO.File.Exists(WTGOperation.ud + WTGOperation.win8VhdFile))
      //      {
      //          //MsgManager.getResString("Msg_VHDCreationError")
      //          //Win8 VHD文件不存在！未知错误原因！
      //          ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDCreationError", MsgManager.ci));
      //          er.ShowDialog();
      //          //MessageBox.Show("Win8 VHD文件不存在！可到论坛发帖求助！\n建议将logs文件夹打包上传！");
      //          //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum-88-1.html");                
      //      }

      //      else if (!System.IO.File.Exists(WTGOperation.ud + "\\Boot\\BCD"))
      //      {
      //          //MsgManager.getResString("Msg_VHDBcdbootError")
      //          //VHD模式下BCDBOOT执行出错！
      //          ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_VHDBcdbootError", MsgManager.ci));
      //          er.ShowDialog();

      //          //MessageBox.Show("VHD模式下BCDBOOT执行出错！\nboot文件夹不存在\n请看论坛教程！","出错啦",MessageBoxButtons .OK ,MessageBoxIcon.Error );
      //          //System.Diagnostics.Process.Start("http://bbs.luobotou.org/forum.php?mod=viewthread&tid=8561");
      //      }
      //      else if (!System.IO.File.Exists(WTGOperation.ud + "bootmgr"))
      //      {
      //          ErrorMsg er = new ErrorMsg(MsgManager.getResString("Msg_bootmgrError", MsgManager.ci));
      //          er.ShowDialog();

      //          //MessageBox.Show("文件写入出错！bootmgr不存在！\n请检查写入过程是否中断\n如有疑问，请访问官方论坛！");
      //      }
      //      else
      //      {
      //          Finish f = new Finish();
      //          f.ShowDialog();
      //      }
      //  }


//private void Fixletter(string targetletter, string currentos) 
//{
//    try
//    {
//        byte[] registData;
//        RegistryKey hkml = Registry.LocalMachine;
//        RegistryKey software = hkml.OpenSubKey("SYSTEM", false);
//        RegistryKey aimdir = software.OpenSubKey("MountedDevices", false);
//        registData = (byte[])aimdir.GetValue("\\DosDevices\\" + currentos);
//        if (registData != null)
//        {
//            ProcessManager.SyncCMD("reg.exe load HKU\\TEMP " + currentos + "\\Windows\\System32\\Config\\SYSTEM  > \"" + Application.StartupPath + "\\logs\\loadreg.log\"");
//            RegistryKey hklm = Registry.Users;
//            RegistryKey temp = hklm.OpenSubKey("TEMP", true);
//            try
//            {
//                temp.DeleteSubKey("MountedDevices");
//            }
//            catch { }
//            RegistryKey wtgreg = temp.CreateSubKey("MountedDevices");
//            wtgreg.SetValue("\\DosDevices\\" + targetletter, registData, RegistryValueKind.Binary);
//            wtgreg.Close();
//            temp.Close();
//            ProcessManager.SyncCMD("reg.exe unload HKU\\TEMP > \"" + Application.StartupPath + "\\logs\\unloadreg.log\"");



//            //string code = ToHexString(registData);
//            ////for (int i = 0; i < registData.Length; i++) 
//            ////{
//            ////    code += ToHexString(registData);
//            ////}
//            //MessageBox.Show(code);

//        }
//    }
//    catch (Exception ex) { MessageBox.Show(ex.ToString()); }
//}

//private void ForeachDisk(string path) 
//{
//    DirectoryInfo dir = new DirectoryInfo(path);
//    try
//    {
//        foreach(FileInfo  d in dir.GetFiles())
//        {
//            MessageBox.Show(d.FullName);
//        }

//    }
//    catch { }
//}

//public void Win7REG(string installdrive) 
//{
//    //installdriver :ud  such as e:\
//    try
//    {
//        ProcessManager.ECMD("reg.exe", " load HKLM\\sys " + installdrive + "WINDOWS\\system32\\config\\system");
//        ProcessManager.ECMD("reg.exe", " import " + Application.StartupPath + "\\files\\usb.reg");
//        ProcessManager.ECMD("reg.exe", " unload HKLM\\sys");
//        Fixletter("C:", installdrive);
//        //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:" + ud.Substring(0, 1) + " > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
//    }
//    catch(Exception err) 
//    {
//        //MsgManager.getResString("Msg_win7usberror")
//        //处理WIN7 USB启动时出现问题
//        MessageBox.Show(MsgManager.getResString("Msg_win7usberror", MsgManager.ci) + err.ToString());
//    }
//}
//        #region not used USBDRIVER
//        //public void USBDrive()
//        //{
//        //    WindowsImageContainer image1 = new WindowsImageContainer("h:\\sources\\install.wim", WindowsImageContainer.CreateFileMode.OpenExisting, WindowsImageContainer.CreateFileAccess.Read);

//        //    manager = new UsbManager();
//        //    UsbDiskCollection disks = manager.GetAvailableDisks();
//        //    foreach (UsbDisk disk in disks)
//        //    {
//        //        MessageBox.Show(disk.ToString());
//        //        //textBox.AppendText(disk.ToString() + CR);
//        //    }
//        //    //manager.StateChanged += new UsbStateChangedEventHandler(DoStateChanged);

//        //}
//        private void DoStateChanged(UsbStateChangedEventArgs e)
//        {
//            MessageBox.Show(e.State.ToString ());

//            //foreach (UsbDisk disk in disks)
//            //{
//            //    MessageBox.Show(disk.ToString());
//            //    //textBox.AppendText(disk.ToString() + CR);
//            //}

//            //textBox.AppendText(e.State + " " + e.Disk.ToString() + CR);
//        }

//        public static string GetDriveInfoDetail(string driveName)
//        {
//            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_DiskDrive  WHERE Name = '{0}'", driveName.Substring(0, 2)));

//            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

//            List<ulong> driveInfoList = new List<ulong>(2);

//            ManagementClass mc = new ManagementClass("Win32_DiskDrive");
//            ManagementObjectCollection moc = mc.GetInstances();

//            foreach (ManagementObject mobj in moc)
//            {
//                MessageBox.Show(mobj["DeviceID"].ToString());


//                return (mobj["Index"].ToString());
//                //Console.WriteLine("File system: " + mobj["FileSystem"]);

//                //Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);

//                //Console.WriteLine("Size: " + mobj["Size"]);
//            }
//            return "ERROR";
//        }
//        public static string GetDriveWin32_DiskPartition(string driveName)
//        {
//            //MessageBox.Show(GetDriveInfoDetail(driveName));
//            //WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_PhysicalMedia   WHERE Name = '{0}'", GetDriveInfoDetail(driveName)));
//            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_DiskPartition   "));
//            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

//            List<ulong> driveInfoList = new List<ulong>(2);

//            ManagementClass mc = new ManagementClass("Win32_DiskPartition");
//            ManagementObjectCollection moc = mc.GetInstances();
//            foreach (ManagementObject mobj in moc)
//            {
//                //MessageBox.Show("");
//                MessageBox.Show(mobj["Name"].ToString());
//                //MessageBox.Show("");

//                //return (mobj["Model"].ToString());

//                //Console.WriteLine("File system: " + mobj["FileSystem"]);

//                //Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);

//                //Console.WriteLine("Size: " + mobj["Size"]);
//            }
//            return "ERROR";
//        }
//        public void Testdrive() 
//        {

//foreach(ManagementObject drive in new ManagementObjectSearcher(
//    "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
//{
//    // assoMsgManager.ciate physical disks with partitions

//    foreach(ManagementObject partition in new ManagementObjectSearcher(
//        "ASSOMsgManager.ciATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]+ "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
//    {
//        Console.WriteLine("Partition=" + partition["Name"]);

//        // assoMsgManager.ciate partitions with logical disks (drive letter volumes)

//        foreach(ManagementObject disk in new ManagementObjectSearcher(
//            "ASSOMsgManager.ciATORS OF {Win32_DiskPartition.DeviceID='"+ partition["DeviceID"]+ "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
//        {
//            //MessageBox.Show(disk["Name"].ToString ());
//            Console.WriteLine("Disk=" + disk["Name"]);
//        }
//    }

//    // this may display nothing if the physical disk

//    // does not have a hardware serial number

//    MessageBox.Show ("Serial="+ new ManagementObject("Win32_PhysicalMedia.Tag='"+ drive["DeviceID"] + "'")["SerialNumber"]);
//}

//        }
//        public static string GetDriveWin32_LogicalDisk(string driveName)
//        {
//            //MessageBox.Show(GetDriveInfoDetail(driveName));
//            //WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_PhysicalMedia   WHERE Name = '{0}'", GetDriveInfoDetail(driveName)));
//            WqlObjectQuery wqlObjectQuery = new WqlObjectQuery(string.Format("SELECT * FROM Win32_LogicalDiskToPartition     "));
//            ManagementObjectSearcher managerSearch = new ManagementObjectSearcher(wqlObjectQuery);

//            List<ulong> driveInfoList = new List<ulong>(2);

//            ManagementClass mc = new ManagementClass("Win32_LogicalDiskToPartition");
//            ManagementObjectCollection moc = mc.GetInstances();
//            foreach (ManagementObject mobj in moc)
//            {
//                //MessageBox.Show("");
//                MessageBox.Show(mobj["Dependent"].ToString());
//                MessageBox.Show(mobj["Antecedent"].ToString());


//                //MessageBox.Show("");

//                //return (mobj["Model"].ToString());

//                //Console.WriteLine("File system: " + mobj["FileSystem"]);

//                //Console.WriteLine("Free disk space: " + mobj["FreeSpace"]);

//                //Console.WriteLine("Size: " + mobj["Size"]);
//            }
//            return "ERROR";
//        }
//        #endregion
//public int  Iswin7(string wimfile) 
//{
//    ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\"+imagex+"\"" + " /info \"" + wimfile + "\" /xml > " + "\""+Application.StartupPath + "\\logs\\wiminfo.xml\"");
//    XmlDocument xml = new XmlDocument();

//   System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
//    System.Xml.XmlNodeReader reader = null;
//    string strFilename = Application.StartupPath + "\\logs\\wiminfo.xml";
//    if (System.IO.File.Exists(strFilename) == false)
//    {
//        //MsgManager.getResString("Msg_wiminfoerror")
//        //WIM文件信息获取失败\n将按WIN8系统安装
//        MessageBox.Show(this, strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci), this.Text);
//        return 0;
//    }
//    try
//    {
//        doc.Load(strFilename);
//        reader = new System.Xml.XmlNodeReader(doc);
//        while (reader.Read())
//        {
//            if (reader.IsStartElement("NAME"
//                ))
//            {

//                //从找到的这个依次往下读取节点
//                System.Xml.XmlNode aa = doc.ReadNode(reader);
//                //MessageBox.Show(aa.InnerText);
//                //MessageBox.Show(aa.InnerText);
//                if (aa.InnerText == "Windows 7 STARTER")
//                {

//                    return 1;
//                    //break;
//                }
//                else if (aa.InnerText == "Windows 7 HOMEBASIC")
//                {
//                    //MessageBox.Show(aa.InnerText); 
//                    return 2;
//                }

//                else { return 0; }


//            }
//        }
//    }
//    catch (Exception  ex)
//    {
//        MessageBox.Show(this, strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci) + ex.ToString(), this.Text);
//        return 0;
//    }



//    return 0;
//}

//public string ReadSel(ArrayList text)
//{
//    //object ddd=new object[];
//    if (comboBox1.InvokeRequired)
//    {
//        OutDelegate outdelegate = new OutDelegate(ReadSel);
//        this.BeginInvoke(outdelegate, new object[] { text });
//        return 0;
//    }
//    //comboBox1.Items.Clear();
//    comboBox1.DataSource = null;
//    comboBox1.DataSource = text;
//    if (comboBox1.Items.Count != 0)
//    {
//        comboBox1.SelectedIndex = 0;
//    }

//    //comboBox1.AppendText("\t\n");
//}

#region OLDCODE
//ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, WTGOperation.ud + "windows  /s  x: /f UEFI");

//ImageOperation.AutoChooseWimIndex(ref wimpart, win7togo);
//判断是否WIN7，自动选择安装分卷
//int win7togo = iswin7(win8iso);
//if (wimpart==0)
//{//自动判断模式

//    if (win7togo == 1)
//    {//WIN7 32 bit

//        wimpart = 5;
//    }
//    else if (win7togo == 2)
//    { //WIN7 64 BIT

//        wimpart = 4;
//    }
//    else { wimpart = 1; }
//}
//IMAGEX解压
//io.ImageApplyToUD (checkBoxwimboot.Checked,isesd,)
//ImageOperation.ImageApply(checkBoxwimboot.Checked, isesd, imagex, win8iso, wimpart, WTGOperation.ud);
//if (checkBoxwimboot.Checked)
//{
//    ImageOperation.WimbootApply(win8iso, ud, wimpart);
//    //ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + win8iso + "\" /SourceIndex:" + wimpart.ToString() + " /DestinationImageFile:" + ud + "wimboot.wim");
//    //
//    //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + ud + "wimboot.wim" + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString() + " /WIMBoot");
//    //

//}
//else
//{
//    //dism /apply-image /imagefile:9600.17050.winblue_refresh.140317-1640_x64fre_client_Professional_zh-cn-ir3_cpra_x64frer_zh-cn_esd.esd /index:4 /applydir:G:\

//    if (isesd) 
//    {
//        ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + win8iso + "\" /ApplyDir:" + ud.Substring(0, 2) + " /Index:" + wimpart.ToString());
//        

//    }
//    else
//    {
//        ProcessManager.ECMD(Application.StartupPath + "\\files\\"+imagex, " /apply " + "\"" + win8iso + "\"" + " " + wimpart.ToString() + " " + ud);
//        
//    }
//}
//安装EXTRA
//if (checkBoxframework.Checked)
//{
//    ProcessManager.ECMD("dism.exe"," /image:" + ud.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimbox.Text.Substring(0, wimbox.Text.Length - 11) + "sxs");
//    

//}
//if (checkBox_san_policy.Checked)//屏蔽本机硬盘
//{
//    ProcessManager.ECMD("dism.exe", " /image:" + ud.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");
//    

//}

//if (checkBoxdiswinre.Checked)
//{

//    File.Copy(Application.StartupPath + "\\files\\unattend.xml", ud + "Windows\\System32\\sysprep\\unattend.xml");
//}
//BCDBOOT WRITE BOOT FILE    

#endregion