using IPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecryptionESD
{
    public class DecryptESD:InterfacePlugin
    {

        public string PluginName
        {
            get { return "ESD文件解密"; }
        }
        public void Execute()
        {
            DecryptESDForm form = new DecryptESDForm();
            form.ShowDialog();
        }
        public Version PluginVersion
        {
            get { return new Version("1.1.0.0"); }
        }
        public string Description
        {
            get { return "解密ESD文件"; }
        }

        public string AuthorName
        {
            get { return "nkc3g4"; }
        }

        public string Copyright
        {
            get { return "Copyright  © 2012-2015  nkc3g4"; }
        }

        public string WebSite
        {
            get { return "http://bbs.luobotou.org/forum-88-1.html"; }
        }
    }
}
