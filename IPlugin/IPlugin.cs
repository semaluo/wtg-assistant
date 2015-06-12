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
        /// 插件版本，Version类型
        /// </summary>
        Version PluginVersion { get; }
        /// <summary>
        /// 插件简介
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 作者名
        /// </summary>
        string AuthorName { get; }
        /// <summary>
        /// 版权信息，例如Copyright  © 2012-2015    nkc3g4
        /// </summary>
        string Copyright { get; }
        /// <summary>
        /// 网站
        /// </summary>
        string WebSite { get; }
        /// <summary>
        /// 运行插件
        /// </summary>
        void Execute();
    }
}
