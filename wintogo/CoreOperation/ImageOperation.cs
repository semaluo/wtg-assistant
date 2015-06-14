﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace wintogo
{
    public class ImageOperation : WTGOperation
    {
        public string imageFile { get; set; }
        public string imageX { get; set; }
        //public bool isESD { get; set; }
        //public string imageIndex { get; set; }
        //public bool isWimboot { get; set; }
        //public int win7togo { get; set; }

        #region 静态方法

        public static string AutoChooseESDImageIndex(string esdPath)
        {
            string outputFilePath = Path.GetTempFileName();
            StringBuilder args = new StringBuilder();
            args.Append(" /get-wiminfo /wimfile:\"");
            args.Append(esdPath);
            args.Append("\" /english");
            args.Append(" > ");
            args.Append("\"");
            args.Append(outputFilePath);
            args.Append("\"");
            //ProcessManager.RunDism(args.ToString());
            ProcessManager.SyncCMD("dism.exe" + args.ToString());

            string outputFileText = File.ReadAllText(outputFilePath);
            MatchCollection mc = Regex.Matches(outputFileText, @"Index");
            if (mc.Count > 1) { FileOperation.DeleteFile(outputFilePath); return "4"; }
            else { FileOperation.DeleteFile(outputFilePath); return "1"; }
            //Match match = Regex.Match(outputFileText, @"Index :([1-9]).+Windows Technical Preview", RegexOptions.Singleline);
            //MessageBox.Show(match.Groups[1].Value);
            //ProcessManager.ECMD("diskpart.exe", args.ToString());

            //System.Console.WriteLine(File.ReadAllText (this.scriptPath));
            //System.Console.WriteLine(dpargs.ToString());
            //System.Windows.Forms.MessageBox.Show(dpargs.ToString());

            //System.Console.WriteLine(File.ReadAllText (this.outputFilePath));

        }

        /// <summary>
        /// framework3.5，屏蔽本机硬盘，禁用WINRE
        /// </summary>
        /// <param name="framework"></param>
        /// <param name="san_policy"></param>
        /// <param name="diswinre"></param>
        /// <param name="imageletter">可以是有盘盘符或V盘</param>
        /// <param name="wimlocation">WIM文件路径</param>
        public static void ImageExtra(bool framework, bool san_policy, bool diswinre, string imageletter, string wimlocation)
        {
            if (framework)
            {
                StringBuilder args = new StringBuilder();
                args.Append(" /image:");
                args.Append(imageletter.Substring(0, 2));
                args.Append(" /enable-feature /featurename:NetFX3 /source:");
                args.Append(wimlocation.Substring(0, wimlocation.Length - 11));
                args.Append("sxs");
                //ProcessManager.RunDism(args.ToString());
                ProcessManager.ECMD("dism.exe",args.ToString ());
               

            }
            if (san_policy)
            {
                ProcessManager.ECMD("dism.exe", " /image:" + imageletter.Substring(0, 2) + " /Apply-Unattend:\"" + WTGOperation.applicationFilesPath + "\\san_policy.xml\"");

            }

            if (diswinre)
            {
                //try {  }
                if (Directory.Exists(imageletter + "Windows\\System32\\sysprep\\"))
                {
                    File.Copy(WTGOperation.applicationFilesPath + "\\unattend.xml", imageletter + "Windows\\System32\\sysprep\\unattend.xml");
                }
            }
        }
        public static void AutoChooseWimIndex(ref string wimpart, int win7togo)
        {
            if (wimpart == "0")
            {//自动判断模式
                if (isEsd)
                { wimpart = AutoChooseESDImageIndex(imageFilePath); }
                else
                {
                    if (win7togo == 1)
                    {//WIN7 32 bit

                        wimpart = "5";
                    }
                    else if (win7togo == 2)
                    { //WIN7 64 BIT

                        wimpart = "4";
                    }
                    else { wimpart = "1"; }
                }
            }
        }
        private static void WimbootApply(string sourceImageFile, string destinationImageDisk, string wimindex, string applyDir)
        {
            ProcessManager.ECMD("Dism.exe", " /Export-Image /WIMBoot /SourceImageFile:\"" + sourceImageFile + "\" /SourceIndex:" + wimindex.ToString() + " /DestinationImageFile:" + destinationImageDisk + "wimboot.wim");
            ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + destinationImageDisk + "wimboot.wim" + "\" /ApplyDir:" + applyDir.Substring(0, 2) + " /Index:" + wimindex.ToString() + " /WIMBoot");
        }
        private static void DismApplyImage(string imageFile, string targetDisk, string wimIndex)
        {
            
            ProcessManager.ECMD("Dism.exe"," /Apply-Image /ImageFile:\"" + imageFile + "\" /ApplyDir:" + targetDisk.Substring(0, 2) + " /Index:" + wimIndex.ToString());
            //ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + imageFile + "\" /ApplyDir:" + targetDisk.Substring(0, 2) + " /Index:" + wimIndex.ToString());
            //wp.ShowDialog();

        }
        private static void ImageXApply(string imagex, string imageFile, string wimIndex, string targetDisk)
        {
            ProcessManager.ECMD(WTGOperation.applicationFilesPath + "\\" + imagex, " /apply " + "\"" + imageFile + "\"" + " " + wimIndex.ToString() + " " + targetDisk);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iswimboot">是否WIMBOOT</param>
        /// <param name="isesd">是否ESD</param>
        /// <param name="imagex">imagex路径</param>
        /// <param name="imageFile">镜像文件</param>
        /// <param name="wimIndex"></param>
        /// <param name="targetDisk">目标磁盘</param>
        /// <param name="wimbootApplyDir">/ImageFile:\"" + wimbootApplyDir + "wimboot.wim"</param>
        public static void ImageApply(bool iswimboot, bool isesd, string imagex, string imageFile, string wimIndex, string targetDisk, string wimbootApplyDir)
        {
            if (iswimboot)
            {
                WimbootApply(imageFile, wimbootApplyDir, wimIndex, targetDisk);
            }
            else
            {
                if (isesd || WTGOperation.allowEsd)//allowEsd只是判断DISM版本
                {
                    DismApplyImage(imageFile, targetDisk, wimIndex);
                }
                else
                {
                    ImageXApply(imagex, imageFile, wimIndex, targetDisk);
                }
            }
        }
        /// <summary>
        /// 判断是否为WIN7 以及32或64位
        /// </summary>
        /// <param name="imagex">imagex文件名，默认传imagex字段</param>
        /// <param name="wimfile">WIM文件路径</param>
        /// <returns>不是WIN7系统：0，Windows 7 STARTER（表示为32位系统镜像）：1，Windows 7 HOMEBASIC（表示为64位系统镜像）：2</returns>
        public static int Iswin7(string imagex, string wimfile)
        {
            ProcessManager.SyncCMD("\"" + WTGOperation.applicationFilesPath + "\\" + imagex + "\"" + " /info \"" + wimfile + "\" /xml > " + "\"" + WTGOperation.logPath + "\\wiminfo.xml\"");
            XmlDocument xml = new XmlDocument();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNodeReader reader = null;
            
            string strFilename = WTGOperation.logPath + "\\wiminfo.xml";
            if (!File.Exists(strFilename))
            {
                //MsgManager.getResString("Msg_wiminfoerror")
                //WIM文件信息获取失败\n将按WIN8系统安装
                Log.WriteLog("Iswin7.log", strFilename + "文件不存在");
                //MessageBox.Show(strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci));
                return 0;
            }
            try
            {
                doc.Load(strFilename);
                reader = new System.Xml.XmlNodeReader(doc);
                while (reader.Read())
                {
                    if (reader.IsStartElement("NAME"))
                    {

                        //从找到的这个依次往下读取节点
                        System.Xml.XmlNode aa = doc.ReadNode(reader);
                        if (aa.InnerText == "Windows 7 STARTER")
                        {
                            return 1;
                        }
                        else if (aa.InnerText == "Windows 7 HOMEBASIC")
                        {
                            return 2;
                        }
                        else { return 0; }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Iswin7.log", strFilename + "\n" + ex.ToString());
                //MessageBox.Show(strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci) + ex.ToString());
                return 0;
            }



            return 0;
        }
        /// <summary>
        /// WIN7 TO GO注册表处理
        /// </summary>
        /// <param name="installdrive">系统盘所在盘盘符例如E:</param>
        public static void Win7REG(string installdrive)
        {
            try
            {
                ProcessManager.SyncCMD("reg.exe load HKU\\sys " + installdrive + "Windows\\System32\\Config\\SYSTEM  > \"" + WTGOperation.logPath + "\\Win7REGLoad.log\"");
                int errorlevel = ProcessManager.SyncCMD("reg.exe import \"" + applicationFilesPath + "\\usb.reg\" >nul &if %errorlevel% ==0 (echo 注册表导入成功) else (echo 注册表导入失败)" + " > \"" + WTGOperation.logPath + "\\Win7REGImport.log\"");
                ProcessManager.SyncCMD("reg.exe unload HKU\\sys " + " > \"" + WTGOperation.logPath+ "\\Win7REGLoad.log\"");
                Log.WriteLog("ImportReg.log", errorlevel.ToString());
                Fixletter("C:", installdrive);

            }
            catch (Exception err)
            {
                //MsgManager.getResString("Msg_win7usberror")
                //处理WIN7 USB启动时出现问题
                MessageBox.Show(MsgManager.GetResString("Msg_win7usberror", MsgManager.ci) + err.ToString());
            }
        }
        /// <summary>
        /// 修复盘符
        /// </summary>
        /// <param name="targetletter">一般为"C:"</param>
        /// <param name="currentos">例如"V:"</param>
        public static void Fixletter(string targetletter, string currentos)
        {
            try
            {
                byte[] registData;
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey software = hkml.OpenSubKey("SYSTEM", false);
                RegistryKey aimdir = software.OpenSubKey("MountedDevices", false);
                registData = (byte[])aimdir.GetValue("\\DosDevices\\" + currentos);
                if (registData != null)
                {
                    ProcessManager.SyncCMD("reg.exe load HKU\\TEMP " + currentos + "\\Windows\\System32\\Config\\SYSTEM  > \"" + WTGOperation.logPath + "\\loadreg.log\"");
                    RegistryKey hklm = Registry.Users;
                    RegistryKey temp = hklm.OpenSubKey("TEMP", true);
                    try
                    {
                        temp.DeleteSubKey("MountedDevices");
                    }
                    catch (Exception ex)
                    { Log.WriteLog("FixletterDeleteSubKey.log", ex.ToString()); }
                    RegistryKey wtgreg = temp.CreateSubKey("MountedDevices");
                    wtgreg.SetValue("\\DosDevices\\" + targetletter, registData, RegistryValueKind.Binary);
                    wtgreg.Close();
                    temp.Close();
                    ProcessManager.SyncCMD("reg.exe unload HKU\\TEMP > \"" + WTGOperation.logPath + "\\unloadreg.log\"");



                    //string code = ToHexString(registData);
                    ////for (int i = 0; i < registData.Length; i++) 
                    ////{
                    ////    code += ToHexString(registData);
                    ////}
                    //MessageBox.Show(code);

                }
                else
                {
                    Log.WriteLog("registDataNull.log", "\\DosDevices\\null");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Log.WriteLog("Fixletter.log", ex.ToString());
            }
        }
        #endregion

        #region 对象方法
        public void AutoChooseWimIndex()
        {
            string tempindex = WTGOperation.wimPart.ToString();
            AutoChooseWimIndex(ref tempindex, win7togo);
            WTGOperation.wimPart = tempindex;
        }
        public void ImageApplyToUD()
        {
            ImageApply(isWimBoot, isEsd, imageX, imageFile, wimPart, ud, ud);
        }
        #endregion

    }
}
