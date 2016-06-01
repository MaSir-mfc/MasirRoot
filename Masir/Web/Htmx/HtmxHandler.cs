using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web.Htmx
{
    /// <summary>
    /// Htmx模板页面处理程序
    /// </summary>
    public class HtmxHandler : HtmxHandlerBase
    {
        /// <summary>
        /// 页面代码
        /// </summary>
        protected StringBuilder m_thisPageCode = new StringBuilder();

        /// <summary>
        /// 页面模板
        /// </summary>
        protected string m_mainTemplate;

        /// <summary>
        /// 重写初始化页面
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        /// <param name="web"></param>
        /// <param name="site"></param>
        /// <param name="skin"></param>
        /// <param name="page"></param>
        public override void InitPage(System.Web.HttpContext context, Page.MaUrl url, IMaWeb web, IMaSite site, IMaSkin skin, Page.MaPage page)
        {
            base.InitPage(context, url, web, site, skin, page);
        }

        /// <summary>
        /// 请求头部信息
        /// </summary>
        /// <param name="context"></param>
        public override void ProcessRequest(HttpContext context)
        {
            if (m_maUrl.RequestType == MaRequestType.HTM)
            {
                //处理数据
                m_mainTemplate = m_maPage.GetTemplateName();

                LoadMainTemplateBegin();
                LoadMainTemplate();
                LoadMainTemplateEnd();

                ParsePageBegin();
                ParsePage();
                ParsePageEnd();
            }
            m_thisPageCode = m_thisPageCode.Replace("</head>", "<meta name=\"handler_time\" content=\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff") + "\" />\r\n</head>");
            Response.Write(m_thisPageCode.ToString());
        }

        #region 解析模板

        /// <summary>
        /// 0、加载主模板开始
        /// </summary>
        public virtual void LoadMainTemplateBegin() { }

        /// <summary>
        /// 1、加载主模板
        /// </summary>
        public virtual void LoadMainTemplate()
        {
            //提取模板
            m_thisPageCode.Append(m_maSkin.GetTemplate(m_mainTemplate));
        }

        /// <summary>
        /// 2、加载主模板结束
        /// </summary>
        public virtual void LoadMainTemplateEnd() { }

        /// <summary>
        /// 3、解析页面开始
        /// </summary>
        public virtual void ParsePageBegin() { }

        /// <summary>
        /// 4、解析页面
        /// </summary>
        public virtual void ParsePage()
        {
            foreach (IMaParser item in m_maPage.GetParseList())
            {
                item.Parse(m_maUrl, m_maSite, m_maSkin, m_maPage, ref m_thisPageCode);
            }
        }

        /// <summary>
        /// 5、解析页面结束
        /// </summary>
        public virtual void ParsePageEnd() { }

        #endregion
    }
}
