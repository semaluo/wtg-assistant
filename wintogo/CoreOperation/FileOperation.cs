using System;
using System.Diagnostics;
using System.IO;
//using System.Threading.Tasks;
namespace wintogo
{
    public static class FileOperation
    {
        public static string GetFileVersion(string path)
        {
            try
            {
                // Get the file version for the notepad.   
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(path);
                return myFileVersionInfo.FileVersion;
                // Print the file name and version number.   
                //textBox1.Text = "File: " + myFileVersionInfo.FileDescription + '\n' +
                //"Version number: " + myFileVersionInfo.FileVersion;
            }
            catch(Exception ex)
            {
                Log.WriteLog("GetFileVersion.log", ex.ToString());
                return ""; 
            }
        }


        //public static void CleanLockStream(string ErrorMsg, string ErrorTitle)
        //{
        //    ListFiles(new DirectoryInfo(Application.StartupPath + "\\files"), ErrorMsg, ErrorTitle);
        //}
        //public static void ListFiles(FileSystemInfo info, string ErrorMsg, string ErrorTitle)
        //{
        //    try
        //    {
        //        if (!info.Exists) return;
        //        DirectoryInfo dir = info as DirectoryInfo;
        //        //不是目录
        //        if (dir == null) return;
        //        FileSystemInfo[] files = dir.GetFileSystemInfos();
        //        for (int i = 0; i < files.Length; i++)
        //        {
        //            FileInfo file = files[i] as FileInfo;
        //            //是文件
        //            if (file != null)
        //            {
        //                //FileInfo file = new FileInfo(@"d:\Hanye.chm");
        //                //MessageBox.Show(file.FullName);
        //                foreach (AlternateDataStreamInfo s in file.ListAlternateDataStreams())
        //                {
        //                    s.Delete();//删除流
        //                }

        //                //Console.WriteLine(file.FullName + "\t " + file.Length);
        //                //if (file.FullName.Substring(file.FullName.LastIndexOf(".")) == ".jpg")
        //                ////此处为显示JPG格式，不加IF可遍历所有格式的文件
        //                //{
        //                //    //this.list1.Items.Add(file);
        //                //    //MessageBox.Show(file.FullName.Substring(file.FullName.LastIndexOf(".")));
        //                //}
        //            }
        //            //对于子目录，进行递归调用
        //            else
        //            {
        //                ListFiles(files[i], ErrorMsg, ErrorTitle);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //NTFS文件流异常\n请放心，此错误不影响正常使用
        //        //GetResString("Msg_ntfsstream");
        //        //MessageBox.Show(getResString("Msg_ntfsstream", ci) + ex.ToString(), getResString("Msg_warning", ci), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        MessageBox.Show(ErrorMsg + ex.ToString(), ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        Log.WriteLog("CleatNtfsStream.log", ex.ToString());

        //    }
        //}
        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

    }
}
