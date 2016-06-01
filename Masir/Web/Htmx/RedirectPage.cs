using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Htmx
{
    /// <summary>
    /// 跳转页面
    /// </summary>
    public class RedirectPage : HtmxHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public override void LoadMainTemplate()
        {
            string url = System.Text.RegularExpressions.Regex.Replace(MaUrl.Current.AbsolutePath, m_maPage.MappedRegexStr, m_maPage.TemplateRegexStr);
            url = url.Replace("{#QueryString#}", Request.QueryString.ToString());
            Response.Redirect(url);
            Response.End();
        }
    }
}
