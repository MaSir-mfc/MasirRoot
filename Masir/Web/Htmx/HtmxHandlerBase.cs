using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Masir.Web.Htmx
{
    /// <summary>
    /// 页面处理底层
    /// </summary>
    public abstract class HtmxHandlerBase : IHttpHandler, IRequiresSessionState
    {
        #region 获得当前请求页面代码

        internal const string MA_THIS_PAGE_CODE_KEY = "MA_THIS_PAGE_CODE_KEY";

        /// <summary>
        /// 获得当前请求时段的页面代码
        /// </summary>
        /// <returns></returns>
        public static string GetThisPageCode()
        {
            if (HttpContext.Current.Items[MA_THIS_PAGE_CODE_KEY] == null)
            {
                return "";
            }
            return HttpContext.Current.Items[MA_THIS_PAGE_CODE_KEY].ToString();
        }

        internal const string MA_THIS_PAGE_TEMP_VARIABLE = "MA_THIS_PAGE_TEMP_VARIABLE";

        /// <summary>
        /// 获取当前请求页面的临时变量
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetThisPageTempVariable()
        {
            if (HttpContext.Current.Items[MA_THIS_PAGE_TEMP_VARIABLE] == null)
            {
                Dictionary<string, string> _info = new Dictionary<string, string>();
                HttpContext.Current.Items[MA_THIS_PAGE_TEMP_VARIABLE] = _info;
                return _info;
            }
            return HttpContext.Current.Items[MA_THIS_PAGE_TEMP_VARIABLE] as Dictionary<string, string>;
        }

        #endregion

        #region Http对象

        /// <summary>
        /// 当前请求HttpContext
        /// </summary>
        protected HttpContext Context;
        /// <summary>
        /// 当前请HttpRequest对象
        /// </summary>
        protected HttpRequest Request;
        /// <summary>
        /// 当前请HttpResponse对象
        /// </summary>
        protected HttpResponse Response;

        #endregion

        #region Ma对象
        /// <summary>
        /// 页面地址
        /// </summary>
        protected MaUrl m_maUrl;
        /// <summary>
        /// 页面站点
        /// </summary>
        protected IMaWeb m_maweb;
        /// <summary>
        /// 站点信息
        /// </summary>
        protected IMaSkin m_maSkin;
        /// <summary>
        /// 页面所属站点
        /// </summary>
        protected IMaSite m_maSite;
        /// <summary>
        /// 页面信息
        /// </summary>
        protected MaPage m_maPage;

        #endregion

        #region 初始化页面信息

        /// <summary>
        /// 初始化页面信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        /// <param name="web"></param>
        /// <param name="site"></param>
        /// <param name="skin"></param>
        /// <param name="page"></param>
        public virtual void InitPage(HttpContext context, MaUrl url, IMaWeb web, IMaSite site, IMaSkin skin, MaPage page)
        {
            //Http对象
            Context = context;
            Request = context.Request;
            Response = context.Response;

            //Ma对象
            m_maUrl = url;
            m_maweb = web;
            m_maSite = site;
            m_maSkin = skin;
            m_maPage = page;
        }

        #endregion

        #region IHttpHandler接口
        /// <summary>
        /// 
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }
        /// <summary>
        /// 响应上下文对象
        /// </summary>
        /// <param name="context"></param>
        public abstract void ProcessRequest(HttpContext context);

        #endregion

        #region 内部变量处理

        /// <summary>
        /// 添加临时变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddTempVariable(string name, object value)
        {
            GetThisPageTempVariable()[name] = value.ToString();
        }

        /// <summary>
        /// 添加临时变量
        /// </summary>
        /// <param name="variable"></param>
        public void AddTempVariable(Dictionary<string, object> variable)
        {
            Dictionary<string, string> _thisTempVariable = GetThisPageTempVariable();
            foreach (string item in variable.Keys)
            {
                _thisTempVariable[item] = variable[item].ToString();
            }
        }

        #endregion

    }
}
