using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web
{
    /// <summary>
    /// 页面站点单个主机接口
    /// </summary>
    public interface IMaSkin : IMaConfig
    {
        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        string GetTemplate(string templateName);

        /// <summary>
        /// 获取模板代码
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool TryGetTemplate(string templateName, out string code);

        /// <summary>
        /// 获取页面代码
        /// </summary>
        /// <param name="htmlStr"></param>
        void PullOnSkin(ref string htmlStr);

        /// <summary>
        /// 是否添加皮肤
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool IsThis(HttpContext context);

        /// <summary>
        /// 添加额外属性
        /// </summary>
        Dictionary<string, object> TemplateList { get; set; }
    }
}
