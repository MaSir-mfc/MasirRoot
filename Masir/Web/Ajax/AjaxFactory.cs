using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web.Ajax
{
    /// <summary>
    /// 异步处理工厂方法
    /// </summary>
    public class AjaxFactory : IHttpHandlerFactory
    {
        /// <summary>
        /// 异步请求实现函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="pathTranslated"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            var _path = Regex.Match(url, @"\/(.*?)\.ajax", RegexOptions.IgnoreCase);
            var _className = _path.Groups[1].Value.Replace('/', '.');

            Type handlerType = System.Web.Compilation.BuildManager.GetType(_className, false, true);
            var handler = Activator.CreateInstance(handlerType) as AjaxHandler;
            return handler;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        public void ReleaseHandler(IHttpHandler handler)
        {

        }
    }
}
