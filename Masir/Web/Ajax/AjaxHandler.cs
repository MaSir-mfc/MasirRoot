using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Masir.Web.Ajax
{
    /// <summary>
    /// 异步ajax处理
    /// </summary>
    public class AjaxHandler : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// 反射请求方法
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string methodName = context.Request.PathInfo.Replace("/", "");

            MethodInfo method = this.GetType().GetMethod(methodName, BindingFlags.Instance
                    | BindingFlags.IgnoreCase
                    | BindingFlags.NonPublic);

            var fun = (Func<HttpContext, string>)method.CreateDelegate(typeof(Func<HttpContext, string>), this);

            context.Response.Write(fun(context));
        }

        /// <summary>
        /// 响应json数据转换
        /// </summary>
        /// <param name="success"></param>
        /// <param name="msg"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        protected string JSON(bool success, string msg, params string[] other)
        {
            var p = string.Empty;
            if (other.Length > 0)
            {
                p = "," + string.Join(",", other);
            }
            var result = string.Format("({{\"success\":\"{0}\",\"msg\":\"{1}\"{2}}})", success.ToString().ToLower(), msg, p);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable { get { return false; } }
    }
}
