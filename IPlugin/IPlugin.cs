using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPlugin
{
    public interface InterfacePlugin
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string PluginName { get; }
        /// <summary>
        /// 运行插件
        /// </summary>
        void Execute();
    }
}
