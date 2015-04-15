using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace wintogo
{
    public  static class BootFileOperation
    {
        public static void BcdbootWriteALLBootFileToXAndAct(string bcdboot,string ud) 
        {
            ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, ud + "windows  /s  x: /f ALL");
            System.Diagnostics.Process p2 = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\bootice.exe", " /DEVICE=x: /partitions /activate  /quiet");
            p2.WaitForExit();
        }
        public static void BooticeWriteMBRPBRAndAct(string targetDisk) 
        {
            BooticeMbr(targetDisk);
            BooticePbr(targetDisk);
            BooticeAct(targetDisk);
        }
        //public static void BooticeWritePbrAndAct
        public  static void BooticeMbr(string targetDisk) 
        {
            System.Diagnostics.Process booice = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", (" /DEVICE=" + targetDisk.Substring(0, 2) + " /mbr /install /type=nt60 /quiet"));//写入引导
            booice.WaitForExit();
        }
        public static void BooticePbr(string targetDisk) 
        {
            System.Diagnostics.Process pbr = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\BOOTICE.exe", (" /DEVICE=" + targetDisk.Substring(0, 2) + " /pbr /install /type=bootmgr /quiet"));//写入引导
            pbr.WaitForExit();
        }
        public static void BooticeAct(string targetDisk)
        {
            System.Diagnostics.Process act = System.Diagnostics.Process.Start(Application.StartupPath + "\\files" + "\\bootice.exe", " /DEVICE=" + targetDisk.Substring(0, 2) + " /partitions /activate /quiet");
            act.WaitForExit();

        }
        public static void BcdbootWriteALLBootFile(string sourceDisk,string targetDisk,string bcdboot) 
        {
            ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, sourceDisk + "windows  /s  " + targetDisk.Substring(0, 2) + " /f ALL");
        }
        public static void BcdbootWriteUEFIBootFile(string sourceDisk, string targetDisk, string bcdboot)
        {
            ProcessManager.ECMD(Application.StartupPath + "\\files\\" + bcdboot, sourceDisk + "windows  /s  " + targetDisk.Substring(0, 2) + " /f UEFI");
        }

    }
}
