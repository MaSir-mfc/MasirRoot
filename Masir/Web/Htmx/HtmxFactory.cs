using Masir.Web.Page;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Masir.Web.Htmx
{
    /// <summary>
    /// 页面请求处理工厂
    /// </summary>
    public class HtmxFactory : IHttpHandlerFactory
    {
        /// <summary>
        /// 页面处理工具
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="pathTranslated"></param>
        /// <returns></returns>
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            if (MaWeb.Instance != null)
            {
                IMaSite _site = MaWeb.Instance.GetSite(context);
                if (_site == null)
                {
                    throw new Exception("WebApp配置错误,请检查Ma.Config配置文件[MaWeb]配置结点！");
                }
                IMaSkin _skin = _site.GetSkinConfig(context);
                MaPage _page = _site.GetRequestConfig(context);
                if (_page == null)
                {
                    throw new Exception("WebApp配置错误,请检查Ma.Config配置文件[MaWeb]配置结点,没有设定默认页面处理类！");
                }
                IHttpHandler _handler = _page.GetHandler(context);

                context.Response.Filter = new HtmxFilter(context.Response.Filter, MaUrl.Current, _page);

                if (_handler is HtmxHandlerBase)
                {
                    HtmxHandlerBase _maHandler = (HtmxHandlerBase)_handler;
                    _maHandler.InitPage(context, MaUrl.Current, MaWeb.Instance, _site, _skin, _page);
                }
                return _handler;
            }
            else
            {
                throw new Exception("WebApp配置错误,请检查Ma.Config配置文件[MaWeb]配置结点");
            }
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
