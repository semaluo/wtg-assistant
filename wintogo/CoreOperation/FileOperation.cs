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
                return string.Empty; 
            }
        }


        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

    }
}
