using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Parse
{
    /// <summary>
    /// 解析标签处理
    /// </summary>
    public class MaTag : NameValueCollection
    {
        /// <summary>
        /// 要解析的标签正则
        /// </summary>
        public const string TAG_REGEX = "<!--Ma_(.*?){(.*?)}-->";

        #region 私有变量

        private MaUrl m_urlInfo;
        private string m_tagName;
        private string m_tagStr;
        private string m_tagAttributeStr;

        #endregion

        #region 属性

        /// <summary>
        /// URL地址
        /// </summary>
        public MaUrl UrlInfo
        {
            get { return m_urlInfo; }
        }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string TagName
        {
            get { return m_tagName; }
        }

        /// <summary>
        /// 标签属性串
        /// </summary>
        public string TagAttributeStr
        {
            get { return m_tagAttributeStr; }
        }

        /// <summary>
        /// 标签字符串
        /// </summary>
        public string TagStr
        {
            get { return m_tagStr; }
        }


        #endregion

        #region 构造函数

        /// <summary>
        /// 处理标签构造函数
        /// </summary>
        /// <param name="tagStr"></param>
        /// <param name="urlInfo"></param>
        public MaTag(string tagStr, MaUrl urlInfo)
        {
            m_urlInfo = urlInfo;

            m_tagStr = tagStr;
            m_tagName = System.Text.RegularExpressions.Regex.Replace(tagStr, TAG_REGEX, "$1", System.Text.RegularExpressions.RegexOptions.IgnoreCase).ToUpper();
            //获得属性值
            m_tagAttributeStr = System.Text.RegularExpressions.Regex.Replace(tagStr, TAG_REGEX, "$2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string[] tagArray = m_tagAttributeStr.Trim().Split(';');
            foreach (string var in tagArray)
            {
                if (!string.IsNullOrEmpty(var)
                    && var.Contains("="))
                {
                    string _key = var.Substring(0, var.IndexOf("="));
                    string _value = var.Substring(var.IndexOf("=") + 1);
                    this[_key] = _value;
                }
            }
            //获得动态属性
            if (!string.IsNullOrEmpty(this["regex"]))
            {
                string _url = urlInfo.AbsolutePath;
                if (this["regex"].Substring(0, 1) != "/")
                {//带域名匹配
                    _url = urlInfo.Domain.Domain + "/" + urlInfo.AbsolutePath;
                }
                foreach (string var in this.AllKeys)
                {
                    if (this[var].Substring(0, 1) == "$")
                    {
                        this[var] = System.Text.RegularExpressions.Regex.Replace(_url, this["regex"], this[var]);
                    }
                }
            }
        }

        #endregion
    }
}
