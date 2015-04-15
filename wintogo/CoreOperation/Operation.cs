using System.IO;
using System.Windows.Forms;

namespace wintogo
{
    public class WTGOperation
    {
        public static string ud;
        public static string udstring;
        public static bool iswimboot;
        public static bool framework;
        public static bool san_policy;
        public static bool diswinre;
        public static string imageFilePath;
        public static string wimpart="0";
        public static bool isesd=false;
        public static string imagex= "imagex_x86.exe";
        public static string bcdboot;
        public static bool forceformat = false;
        public static int userSetSize;
        public static bool isfixed;
        public static int win7togo;
        public static string diskpartscriptpath = Application.StartupPath + "\\logs";
        public static bool isuefigpt;
        public static bool isuefimbr;
        public static bool isnotemp;
        /// <summary>
        /// VHD或VHDX
        /// </summary>
        public static string filetype;
        public static bool commonbootfiles;
        public static string scriptTempPath = Path.GetTempPath();
        public static string win8VhdFile="win8.vhd";
    }
}
