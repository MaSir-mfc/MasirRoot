using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Parse
{
    /// <summary>
    /// 代码模板替换类（页面模板替换）
    /// </summary>
    public class TemplateParser : MaParserBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TemplateParser()
        {
            m_tagRegexStr = @"<!--Ma_TMP{(.*?)}-->";
        }

        /// <summary>
        /// 默认模板替换函数
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <param name="site"></param>
        /// <param name="skin"></param>
        /// <param name="page"></param>
        /// <param name="pageCode"></param>
        /// <param name="tag"></param>
        public override void ParseTag(Page.MaUrl urlInfo, IMaSite site, IMaSkin skin, Page.MaPage page, ref StringBuilder pageCode, MaTag tag)
        {
            //模板名称
            string _tName = tag["tmp"];
            string _templateStr;
            if (!skin.TryGetTemplate(_tName, out _templateStr))
            {
                _templateStr = "<!--获取模板文件[" + _tName + "]出错-->";
                throw new Exception("获取模板文件[" + _tName + "]出错，【" + urlInfo.ToString() + "】");
            }
            pageCode = pageCode.Replace(tag.TagStr, _templateStr);

        }
    }
}
