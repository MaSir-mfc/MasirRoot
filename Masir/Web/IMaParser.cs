using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web
{
    /// <summary>
    /// 页面解签接口
    /// </summary>
    public interface IMaParser : IMaConfig
    {
        /// <summary>
        /// 页面解签方法
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <param name="site"></param>
        /// <param name="skin"></param>
        /// <param name="page"></param>
        /// <param name="pageCode"></param>
        void Parse(MaUrl urlInfo, IMaSite site, IMaSkin skin, MaPage page, ref StringBuilder pageCode);
    }
}
