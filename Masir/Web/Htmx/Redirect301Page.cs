using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Htmx
{
    /// <summary>
    /// 页面永久重定向
    /// </summary>
    public class Redirect301Page : HtmxHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public override void LoadMainTemplate()
        {
            string url = System.Text.RegularExpressions.Regex.Replace(MaUrl.Current.AbsolutePath, m_maPage.MappedRegexStr, m_maPage.TemplateRegexStr);

            Response.StatusCode = 301;
            Response.Status = "301 Moved Permanently";
            Response.AddHeader("Location", url);
            Response.End();
        }
    }
}
