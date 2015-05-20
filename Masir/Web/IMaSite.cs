using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web
{
    /// <summary>
    /// 页面站点主机接口
    /// </summary>
    public interface IMaSite:IMaConfig
    {
        /// <summary>
        /// 获取当前请求对应的皮肤配置信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IMaSkin GetSkinConfig(HttpContext context);

        /// <summary>
        /// 获取当前请求对应的处理页面配置信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        MaPage GetRequestConfig(HttpContext context);

        /// <summary>
        /// 设置当前站点对应的主机头Host
        /// </summary>
        /// <param name="host"></param>
        void SetHost(string host);

        /// <summary>
        /// 检测当前请求是否适应此站点配置
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool IsThis(HttpContext context);
    }
}
