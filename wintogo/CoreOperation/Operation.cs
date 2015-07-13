using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace wintogo
{
    public class WTGOperation
    {
        public static UserSetWTGSettingItems userSettings = new UserSetWTGSettingItems();
        public static List<string> imagePartNames = new List<string>();
        //public static bool appClosrForm = false;
        //public static string userSetEfiSize = "350";
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
        public static string udString;
        public static bool isWimBoot;
        public static bool isFramework;
        public static bool isSan_policy;
        public static bool isDiswinre;
        /// <summary>
        /// 镜像文件路径
        /// </summary>
        public static string imageFilePath;
        /// <summary>
        /// WimIndex
        /// </summary>
        public static string wimPart="0";

        public static bool isEsd=false;
        /// <summary>
        /// 默认为imagex_x86.exe
        /// </summary>
        public static string imagexFileName= "imagex_x86.exe";
        /// <summary>
        /// bcdboot文件名
        /// </summary>
        public static string bcdbootFileName;
        ///// <summary>
        ///// 强制格式化
        ///// </summary>
        //public static bool forceFormat = false;
        /// <summary>
        /// VHD文件用户设定大小
        /// </summary>
        public static int userSetSize;
        public static bool isFixedVHD;
        public static int win7togo;
        /// <summary>
        /// Application.StartupPath + "\\logs";
        /// </summary>
        public static string diskpartScriptPath = Path.GetTempPath();
        public static bool isUefiGpt;
        public static bool isUefiMbr;
        public static bool isNoTemp;

        /// <summary>
        ///  WTGOperation.filetype = Path.GetExtension(openFileDialog1.FileName.ToLower()).Substring(1);
        /// </summary>
        public static string choosedFileType;
        /// <summary>
        /// 是否勾选通用启动文件
        /// </summary>
        public static bool commonBootFiles;
        /// <summary>
        /// Path.GetTempPath();
        /// </summary>
        //public static string scriptTempPath = Path.GetTempPath();
        /// <summary>
        /// win8.vhd
        /// </summary>
        public static string win8VHDFileName = "win8.vhd";
        /// <summary>
        /// Path.GetTempPath() + "\\WTGA";
        /// </summary>
        public static string applicationFilesPath = Path.GetTempPath() + "\\WTGA";
        public static string logPath = Application.StartupPath + "\\logs";
        public static string vhdExtension = "vhd";
    }
}
