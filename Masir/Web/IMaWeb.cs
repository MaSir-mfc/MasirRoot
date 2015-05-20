using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web
{
    /// <summary>
    /// 页面站点接口
    /// </summary>
    public interface IMaWeb : IMaConfig
    {
        /// <summary>
        /// 匹配当前请求站点信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IMaSite GetSite(HttpContext context);

        /// <summary>
        /// 获取host列表（外加 非必要）
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetHost();
    }
}
