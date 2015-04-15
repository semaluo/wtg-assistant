using Microsoft.Win32;
using System;
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


        public static void ImageExtra(bool framework, bool san_policy, bool diswinre, string imageletter, string wimlocation)
        {
            if (framework)
            {
                ProcessManager.ECMD("dism.exe", " /image:" + imageletter.Substring(0, 2) + " /enable-feature /featurename:NetFX3 /source:" + wimlocation.Substring(0, wimlocation.Length - 11) + "sxs");

            }
            if (san_policy)
            {
                ProcessManager.ECMD("dism.exe", " /image:" + imageletter.Substring(0, 2) + " /Apply-Unattend:\"" + Application.StartupPath + "\\files\\san_policy.xml\"");

            }

            if (diswinre)
            {
                File.Copy(Application.StartupPath + "\\files\\unattend.xml", imageletter + "Windows\\System32\\sysprep\\unattend.xml");
            }
        }
        public static void AutoChooseWimIndex(ref string wimpart, int win7togo)
        {
            if (wimpart == "0")
            {//自动判断模式
                if (isesd)
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
        private static void ESDApply(string imageFile, string targetDisk, string wimIndex)
        {
            ProcessManager.ECMD("Dism.exe", " /Apply-Image /ImageFile:\"" + imageFile + "\" /ApplyDir:" + targetDisk.Substring(0, 2) + " /Index:" + wimIndex.ToString());
            //wp.ShowDialog();

        }
        private static void ImageXApply(string imagex, string imageFile, string wimIndex, string targetDisk)
        {
            ProcessManager.ECMD(Application.StartupPath + "\\files\\" + imagex, " /apply " + "\"" + imageFile + "\"" + " " + wimIndex.ToString() + " " + targetDisk);

        }
        public static void ImageApply(bool iswimboot, bool isesd, string imagex, string imageFile, string wimIndex, string targetDisk, string wimbootApplyDir)
        {
            if (iswimboot)
            {
                WimbootApply(imageFile, wimbootApplyDir, wimIndex, targetDisk);
            }
            else
            {
                if (isesd)
                {
                    ESDApply(imageFile, targetDisk, wimIndex);
                }
                else
                {
                    ImageXApply(imagex, imageFile, wimIndex, targetDisk);
                }
            }
        }
        public static int Iswin7(string imagex, string wimfile)
        {
            ProcessManager.SyncCMD("\"" + Application.StartupPath + "\\files\\" + imagex + "\"" + " /info \"" + wimfile + "\" /xml > " + "\"" + Application.StartupPath + "\\logs\\wiminfo.xml\"");
            XmlDocument xml = new XmlDocument();

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            System.Xml.XmlNodeReader reader = null;
            string strFilename = Application.StartupPath + "\\logs\\wiminfo.xml";
            if (System.IO.File.Exists(strFilename) == false)
            {
                //MsgManager.getResString("Msg_wiminfoerror")
                //WIM文件信息获取失败\n将按WIN8系统安装
                MessageBox.Show(strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci));
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
                MessageBox.Show(strFilename + MsgManager.getResString("Msg_wiminfoerror", MsgManager.ci) + ex.ToString());
                return 0;
            }



            return 0;
        }
        public static void Win7REG(string installdrive)
        {
            //installdriver :ud  such as e:\
            try
            {
                ProcessManager.ECMD("reg.exe", " load HKLM\\sys " + installdrive + "WINDOWS\\system32\\config\\system");
                ProcessManager.ECMD("reg.exe", " import " + Application.StartupPath + "\\files\\usb.reg");
                ProcessManager.ECMD("reg.exe", " unload HKLM\\sys");
                Fixletter("C:", installdrive);
                //ProcessManager.SyncCMD("\""+Application.StartupPath + "\\files\\osletter7.bat\" /targetletter:c /currentos:" + ud.Substring(0, 1) + " > \"" + Application.StartupPath + "\\logs\\osletter7.log\"");
            }
            catch (Exception err)
            {
                //MsgManager.getResString("Msg_win7usberror")
                //处理WIN7 USB启动时出现问题
                MessageBox.Show(MsgManager.getResString("Msg_win7usberror", MsgManager.ci) + err.ToString());
            }
        }
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
                    ProcessManager.SyncCMD("reg.exe load HKU\\TEMP " + currentos + "\\Windows\\System32\\Config\\SYSTEM  > \"" + Application.StartupPath + "\\logs\\loadreg.log\"");
                    RegistryKey hklm = Registry.Users;
                    RegistryKey temp = hklm.OpenSubKey("TEMP", true);
                    try
                    {
                        temp.DeleteSubKey("MountedDevices");
                    }
                    catch { }
                    RegistryKey wtgreg = temp.CreateSubKey("MountedDevices");
                    wtgreg.SetValue("\\DosDevices\\" + targetletter, registData, RegistryValueKind.Binary);
                    wtgreg.Close();
                    temp.Close();
                    ProcessManager.SyncCMD("reg.exe unload HKU\\TEMP > \"" + Application.StartupPath + "\\logs\\unloadreg.log\"");



                    //string code = ToHexString(registData);
                    ////for (int i = 0; i < registData.Length; i++) 
                    ////{
                    ////    code += ToHexString(registData);
                    ////}
                    //MessageBox.Show(code);

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        #endregion

        #region 对象方法
        public void AutoChooseWimIndex()
        {
            string tempindex = WTGOperation.wimpart.ToString();
            AutoChooseWimIndex(ref tempindex, win7togo);
            WTGOperation.wimpart = tempindex;
        }
        public void ImageApplyToUD()
        {
            ImageApply(iswimboot, isesd, imageX, imageFile, wimpart, ud, ud);
        }
        #endregion

    }
}
