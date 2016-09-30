using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Masir.Web.Parse
{
    /// <summary>
    /// 解析标签抽象类（解析父类）
    /// </summary>
    public abstract class MaParserBase : IMaParser
    {
        /// <summary>
        /// 解析规则
        /// </summary>
        protected string m_tagRegexStr;

        /// <summary>
        /// 标签正则
        /// </summary>
        public string TagRegexStr
        {
            get { return m_tagRegexStr; }
        }

        /// <summary>
        /// 标签处理虚方法
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <param name="site"></param>
        /// <param name="skin"></param>
        /// <param name="page"></param>
        /// <param name="pageCode"></param>
        public virtual void Parse(MaUrl urlInfo, IMaSite site, IMaSkin skin, MaPage page, ref StringBuilder pageCode)
        {
            if (string.IsNullOrEmpty(m_tagRegexStr))
            {
                throw new Exception("解析表情出错，当前类没有标签匹配字符串串，请给[TagRegexStr]赋值");
            }

            //查询的正则表达式
            Regex re = new Regex(m_tagRegexStr, RegexOptions.IgnoreCase);
            MatchCollection matches = re.Matches(pageCode.ToString());
            foreach (Match var in matches)
            {
                MaTag _maTag = new MaTag(var.Value, urlInfo);
                ParseTag(urlInfo, site, skin, page, ref pageCode, _maTag);
            }
        }

        /// <summary>
        /// 标签处理抽象函数
        /// </summary>
        /// <param name="urlInfo"></param>
        /// <param name="site"></param>
        /// <param name="skin"></param>
        /// <param name="page"></param>
        /// <param name="pageCode"></param>
        /// <param name="tag"></param>
        public abstract void ParseTag(MaUrl urlInfo, IMaSite site, IMaSkin skin, MaPage page, ref StringBuilder pageCode, MaTag tag);

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="node"></param>
        public virtual void Load(System.Xml.XmlElement node)
        {

        }
    }
}
