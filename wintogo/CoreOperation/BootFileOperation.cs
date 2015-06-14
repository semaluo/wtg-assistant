using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace wintogo
{
    public enum FirmwareType
    {
        BIOS,
        UEFI,
        ALL
    }
    public  static class BootFileOperation
    {
        ///// <summary>
        ///// windows  /s  x: /f ALL
        ///// </summary>
        ///// <param name="bcdboot"></param>
        ///// <param name="ud"></param>
        //public static void BcdbootWriteALLBootFileToXAndAct(string bcdboot,string ud) 
        //{
        //    ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, ud + "windows  /s  x: /f ALL");
        //    System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(WTGOperation.applicationFilesPath+ "\\bootice.exe", " /DEVICE=x: /partitions /activate  /quiet");
        //    p2.WaitForExit();
        //}
        public static void BooticeWriteMBRPBRAndAct(string targetDisk) 
        {
            BooticeMbr(targetDisk);
            BooticePbr(targetDisk);
            BooticeAct(targetDisk);
        }
        //public static void BooticeWritePbrAndAct
        public  static void BooticeMbr(string targetDisk) 
        {
            System.Diagnostics.Process booice = System.Diagnostics.Process.Start(WTGOperation.applicationFilesPath+ "\\BOOTICE.exe", (" /DEVICE=" + targetDisk.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
            booice.WaitForExit();
        }
        public static void BooticePbr(string targetDisk) 
        {
            System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(WTGOperation.applicationFilesPath+ "\\BOOTICE.exe", (" /DEVICE=" + targetDisk.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
            pbr.WaitForExit();
        }
        public static void BooticeAct(string targetDisk)
        {
            System.Diagnostics.Process act = System.Diagnostics.Process.Start(WTGOperation.applicationFilesPath+ "\\bootice.exe", " /DEVICE=" + targetDisk.Substring(0, 2) + " /partitions /activate /quiet");
            act.WaitForExit();

        }
        ///// <summary>
        ///// /f ALL参数
        ///// </summary>
        ///// <param name="sourceDisk">指定 windows 系统根目录</param>
        ///// <param name="targetDisk">该参数用于指定要将启动环境文件复制到哪个目标系统分区。</param>
        ///// <param name="bcdboot">bcdboot文件名，默认传bcdboot字段</param>
        //public static void BcdbootWriteALLBootFile(string sourceDisk,string targetDisk,string bcdboot) 
        //{
        //    ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, sourceDisk + "windows  /s  " + targetDisk.Substring(0, 2) + " /f BIOS");
        //}
        ///// <summary>
        ///// /f UEFI参数
        ///// </summary>
        ///// <param name="sourceDisk">指定 windows 系统根目录</param>
        ///// <param name="targetDisk">该参数用于指定要将启动环境文件复制到哪个目标系统分区。</param>
        ///// <param name="bcdboot">bcdboot文件名，默认传bcdboot字段</param>
        //public static void BcdbootWriteUEFIBootFile(string sourceDisk, string targetDisk, string bcdboot)
        //{
        //    ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, sourceDisk + "windows  /s  " + targetDisk.Substring(0, 2) + " /f UEFI");
        //}


        /// <summary>
        /// BCDBOOT写入引导文件
        /// </summary>
        /// <param name="sourceDisk">例如E:\</param>
        /// <param name="targetDisk">例如E:</param>
        /// <param name="bcdbootFileName">例如bcdboot.exe</param>
        /// <param name="fwType"></param>
        public static void BcdbootWriteBootFile(string sourceDisk,string targetDisk,string bcdbootFileName,FirmwareType fwType)
        {
            StringBuilder args = new StringBuilder();
            args.Append(sourceDisk);
            args.Append("windows /s ");
            args.Append(targetDisk.Substring(0,2));
            if (fwType==FirmwareType.ALL)
            {
                args.Append(" /f all ");
            }
            else if(fwType==FirmwareType.BIOS)
            {
                args.Append(" /f bios ");
            }
            else
            {
                args.Append(" /f uefi ");

            }
            args.Append(" /l zh-ch ");
            args.Append(" /v ");
            ProcessManager.ECMD(WTGOperation.applicationFilesPath + "\\" + bcdbootFileName, args.ToString());
        }
    }
}
