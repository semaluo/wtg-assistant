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
            //throw new NotImplementedException();
            DecryptESDForm form = new DecryptESDForm();
            form.ShowDialog();
        }
    }
}
