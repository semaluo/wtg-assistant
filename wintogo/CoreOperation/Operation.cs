using System.IO;
using System.Windows.Forms;

namespace wintogo
{
    public class WTGOperation
    {
        /// <summary>
        /// 可使用ESD文件
        /// </summary>
        public static bool allowEsd = false;
        /// <summary>
        /// 优盘盘符
        /// </summary>
        public static string ud;
        /// <summary>
        /// 显示在ComboBox中的字符串信息
        /// </summary>
        public static string udstring;
        public static bool iswimboot;
        public static bool framework;
        public static bool san_policy;
        public static bool diswinre;
        /// <summary>
        /// 镜像文件路径
        /// </summary>
        public static string imageFilePath;
        /// <summary>
        /// WimIndex
        /// </summary>
        public static string wimpart="0";

        public static bool isesd=false;
        /// <summary>
        /// 默认为imagex_x86.exe
        /// </summary>
        public static string imagex= "imagex_x86.exe";
        public static string bcdboot;
        public static bool forceformat = false;
        /// <summary>
        /// VHD文件用户设定大小
        /// </summary>
        public static int userSetSize;
        public static bool isfixed;
        public static int win7togo;
        /// <summary>
        /// Application.StartupPath + "\\logs";
        /// </summary>
        public static string diskpartscriptpath = Application.StartupPath + "\\logs";
        public static bool isuefigpt;
        public static bool isuefimbr;
        public static bool isnotemp;
        /// <summary>
        ///  WTGOperation.filetype = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
        /// </summary>
        public static string filetype;
        /// <summary>
        /// 是否勾选通用启动文件
        /// </summary>
        public static bool commonbootfiles;
        /// <summary>
        /// Path.GetTempPath();
        /// </summary>
        public static string scriptTempPath = Path.GetTempPath();
        /// <summary>
        /// win8.vhd
        /// </summary>
        public static string win8VhdFile="win8.vhd";
        public static string filesPath = Path.GetTempPath() + "\\WTGA";
    }
}
