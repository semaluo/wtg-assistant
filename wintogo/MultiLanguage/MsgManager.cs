using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using wintogo.MultiLanguage;

namespace wintogo
{

    public static class MsgManager
    {
        public static CultureInfo ci = Thread.CurrentThread.CurrentCulture;
        public static System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceLang));
        public static string getResString(string rname, CultureInfo culi)
        {
            return resources.GetString(rname, culi).Replace("\\n", Environment.NewLine);
        }

    }


}
