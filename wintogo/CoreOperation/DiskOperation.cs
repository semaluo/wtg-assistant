using System.IO;
using System.Text;
//using System.Threading.Tasks;

namespace wintogo
{
    public static class DiskOperation
    {
        /// <summary>
        /// WTGOperation.diskpartscriptpath + @"\uefi.txt"
        /// </summary>
        /// <param name="efisize"></param>
        /// <param name="ud"></param>
        /// <returns>return WTGOperation.diskpartscriptpath + "\\uefi.txt";</returns>
        public static string GenerateGPTAndUEFIScript(string efisize, string ud)
        {
            using (FileStream fs0 = new FileStream(WTGOperation.diskpartScriptPath + @"\uefi.txt", FileMode.Create, FileAccess.Write))
            {
                fs0.SetLength(0);
                using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
                {
                    //string ws0 = "";

                    sw0.WriteLine("select volume " + ud.Substring(0, 1));
                    sw0.WriteLine("clean");
                    sw0.WriteLine("convert gpt");
                    sw0.WriteLine("create partition efi size " + efisize);
                    sw0.WriteLine("create partition primary");
                    sw0.WriteLine("select partition 2");
                    sw0.WriteLine("format fs=fat quick");
                    sw0.WriteLine("assign letter=x");
                    sw0.WriteLine("select partition 3");
                    sw0.WriteLine("format fs=ntfs quick");
                    sw0.WriteLine("assign letter=" + ud.Substring(0, 1));
                    sw0.WriteLine("exit");
                }
            }
            //sw0.Close();
            return WTGOperation.diskpartScriptPath + "\\uefi.txt";
        }
        /// <summary>
        /// MBR+UEFI脚本Write到WTGOperation.diskpartscriptpath + @"\uefimbr.txt
        /// </summary>
        /// <param name="efisize">efisize(MB)</param>
        /// <param name="ud">优盘盘符，":"、"\"不必须</param>
        public static void GenerateMBRAndUEFIScript(string efisize, string ud)
        {
            using (FileStream fs0 = new FileStream(WTGOperation.diskpartScriptPath + @"\uefimbr.txt", FileMode.Create, FileAccess.Write))
            {
                fs0.SetLength(0);
                using (StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default))
                {
                    //string ws0 = "";

                    sw0.WriteLine("select volume " + ud.Substring(0, 1));
                    sw0.WriteLine("clean");
                    sw0.WriteLine("convert mbr");
                    sw0.WriteLine("create partition primary size " + efisize);
                    sw0.WriteLine("create partition primary");
                    sw0.WriteLine("select partition 1");
                    sw0.WriteLine("format fs=fat quick");
                    sw0.WriteLine("assign letter=x");
                    sw0.WriteLine("select partition 2");
                    sw0.WriteLine("format fs=ntfs quick");
                    sw0.WriteLine("assign letter=" + ud.Substring(0, 1));
                    sw0.WriteLine("exit");
                }

            }
        }

        public static void DiskPartReformatUD()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select volume " + WTGOperation.ud.Substring(0, 1));
            sb.AppendLine("clean");
            sb.AppendLine("convert mbr");
            sb.AppendLine("create partition primary");
            sb.AppendLine("select partition 1");
            sb.AppendLine("format fs=ntfs quick");
            sb.AppendLine("active");
            sb.AppendLine("assign letter=" + WTGOperation.ud.Substring(0, 1));
            sb.AppendLine("exit");
            DiskpartScriptManager dsm = new DiskpartScriptManager();
            dsm.Args = sb.ToString();
            dsm.RunDiskpartScript();

            //ud = comboBox1.SelectedItem.ToString().Substring(0, 2) + "\\";//优盘
            //if (DialogResult.No == MessageBox.Show("此操作将会清除移动磁盘所有分区的所有数据，确认？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; }
            //if (DialogResult.No == MessageBox.Show("您确定要继续吗？", "警告！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)) { return; } 

            //FileStream fs0 = new FileStream(WTGOperation.diskpartscriptpath + "\\dp.txt", FileMode.Create, FileAccess.Write);
            //fs0.SetLength(0);
            //StreamWriter sw0 = new StreamWriter(fs0, Encoding.Default);
            //string ws0 = "";
            //try
            //{
            //    ws0 = "select volume " + WTGOperation.ud.Substring(0, 1);
            //    sw0.WriteLine(ws0);
            //    ws0 = "clean";
            //    sw0.WriteLine(ws0);
            //    ws0 = "convert mbr";
            //    sw0.WriteLine(ws0);
            //    ws0 = "create partition primary";
            //    sw0.WriteLine(ws0);
            //    ws0 = "select partition 1";
            //    sw0.WriteLine(ws0);
            //    ws0 = "format fs=ntfs quick";
            //    sw0.WriteLine(ws0);
            //    ws0 = "active";
            //    sw0.WriteLine(ws0);
            //    ws0 = "assign letter=" + WTGOperation.ud.Substring(0, 1);
            //    sw0.WriteLine(ws0);
            //    ws0 = "exit";
            //    sw0.WriteLine(ws0);
            //}
            //catch { }
            //sw0.Close();

            //ProcessManager.ECMD("diskpart.exe", " /s \"" + WTGOperation.diskpartscriptpath + "\\dp.txt\"");

            //System.Diagnostics.Process dpc = System.Diagnostics.Process.Start("diskpart.exe", " /s " + Application.StartupPath + "\\dp.txt");
            //dpc.WaitForExit();
        }
        public static long GetHardDiskSpace(string str_HardDiskName)
        {
            long totalSize = new long();
            //str_HardDiskName = str_HardDiskName;
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                //MessageBox.Show(drive.TotalSize.ToString () );
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalSize / 1024;

                }
            }
            return totalSize;
        }
        public static long GetHardDiskFreeSpace(string str_HardDiskName)
        {
            long totalSize = new long();
            //str_HardDiskName = str_HardDiskName;
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                //MessageBox.Show(drive.TotalSize.ToString () );
                if (drive.Name == str_HardDiskName)
                {
                    totalSize = drive.TotalFreeSpace / 1024;

                }
            }
            return totalSize;
        }
    }
}
