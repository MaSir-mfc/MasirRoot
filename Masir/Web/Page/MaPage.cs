using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Masir.Web.Page
{
    /// <summary>
    /// 页面处理抽象类
    /// </summary>
    public abstract class MaPage : MaConfig
    {
        #region 页面后续处理规则

        MaPageSaveWay m_saveWay;

        /// <summary>
        /// 页面保存方法
        /// </summary>
        public MaPageSaveWay SaveWay
        {
            get
            {
                if (m_saveWay == MaPageSaveWay.Null)
                {
                    return MaPageSaveWay.Cache;
                }
                return m_saveWay;
            }
        }

        int m_updateTime;

        /// <summary>
        /// 页面更新时间
        /// </summary>
        public int UpdateTime
        {
            get
            {
                if (m_updateTime == 0)
                {
                    return 15 * 60;
                }
                return m_updateTime;
            }
        }

        string m_location;

        /// <summary>
        /// 允许缓存的位置
        /// </summary>
        public string Location
        {
            get { return m_location; }
        }
        string m_custom;
        /// <summary>
        /// 自定义缓存健
        /// </summary>
        public string Custom
        {
            get { return m_custom; }
        }
        string m_header;
        /// <summary>
        /// 根据标头缓存
        /// </summary>
        public string Header
        {
            get { return m_header; }
        }
        string m_param;
        /// <summary>
        /// 根据参数缓存
        /// </summary>
        public string Param
        {
            get { return m_param; }
        }

        #endregion

        #region Page对象

        string m_handlerTypeStr;
        Type m_handlerType;

        #endregion

        #region 0、构造函数
        /// <summary>
        /// 设置页面默认信息构造
        /// </summary>
        public MaPage()
        {
            m_saveWay = MaPageSaveWay.Null;
            m_updateTime = 0;
            m_parseList = new List<IMaParser>();
        }
        #endregion

        #region 1、加载配置信息

        /// <summary>
        /// 加载具体页面的配置信息
        /// </summary>
        /// <param name="node"></param>
        public override void Load(XmlElement node)
        {
            base.Load(node);
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item is XmlNode)
                {
                    if (item.Name == "Output")
                    {
                        #region 后续处理

                        if (item.Attributes["way"] != null)
                        {
                            Enum.TryParse<MaPageSaveWay>(item.Attributes["way"].Value, out m_saveWay);
                        }
                        if (item.Attributes["time"] != null)
                        {
                            int.TryParse(item.Attributes["time"].Value, out m_updateTime);
                        }
                        if (item.Attributes["location"] != null)
                        {
                            m_location = item.Attributes["location"].Value;
                        }
                        if (item.Attributes["custom"] != null)
                        {
                            m_custom = item.Attributes["custom"].Value;
                        }
                        if (item.Attributes["header"] != null)
                        {
                            m_header = item.Attributes["header"].Value;
                        }
                        if (item.Attributes["param"] != null)
                        {
                            m_param = item.Attributes["param"].Value;
                        }

                        #endregion
                    }
                    else if (item.Name == "MappedRegexStr")
                    {
                        m_mappedRegexStr = node["MappedRegexStr"].InnerText;
                        m_mappedRegex = new Regex(node["MappedRegexStr"].InnerText, RegexOptions.IgnoreCase);
                    }
                    else if (item.Name == "PageTemplate")
                    {
                        m_templateRegexStr = node["PageTemplate"].InnerText;
                    }
                    else if (item.Name == "Handler")
                    {
                        #region 页面处理程序

                        //页面处理程序
                        if (item.Attributes["type"] != null)
                        {
                            m_handlerTypeStr = item.Attributes["type"].Value;
                            m_handlerType = System.Web.Compilation.BuildManager.GetType(m_handlerTypeStr, false, false);
                            if (m_handlerType == null)
                            {
                                throw new Exception("页面处理程序类型错误：没有找到对应的类型[" + item.Attributes["type"].Value + "]");
                            }
                        }
                        if (m_handlerType == null)
                        {
                            throw new Exception("配置结点没有设置页面处理程序，请检查配置文件");
                        }
                        #endregion
                    }
                    else if (item.Name == "Parsers")
                    {
                        #region 解析者

                        m_clearParsers = item["clear"] != null;

                        foreach (XmlElement addnode in item.SelectNodes("add"))
                        {
                            IMaParser _pw = MaConfigManager.LoadConfig((XmlElement)addnode) as IMaParser;
                            if (_pw != null)
                            {
                                m_parseList.Add(_pw);
                            }
                        }

                        #endregion
                    }
                }
            }
        }
        #endregion
        
        #region 2、设置默认页面处理配置

        /// <summary>
        /// 默认页面配置信息
        /// </summary>
        protected MaPage m_defaultRequestConfig;

        /// <summary>
        /// 设置默认页面处理配置
        /// </summary>
        /// <param name="requestConfig"></param>
        public virtual void SetDefault(MaPage requestConfig)
        {
            m_defaultRequestConfig = requestConfig;
            m_saveWay = (m_saveWay == MaPageSaveWay.Null) ? requestConfig.SaveWay : m_saveWay;
            m_updateTime = (m_updateTime == 0) ? requestConfig.UpdateTime : m_updateTime;

            m_location = string.IsNullOrEmpty(m_location) ? requestConfig.m_location : m_location;
            m_header = string.IsNullOrEmpty(m_header) ? requestConfig.m_header : m_header;
            m_param = string.IsNullOrEmpty(m_param) ? requestConfig.m_param : m_param;
            m_custom = string.IsNullOrEmpty(m_custom) ? requestConfig.m_custom : m_custom;
        }

        #endregion

        #region 3、是否适应当前请求

        /// <summary>
        /// 是否当前页面
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool IsThis(HttpContext context)
        {
            if (m_mappedRegex.Match(MaUrl.Current.AbsolutePath).Success)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 4、获取页面处理方法

        /// <summary>
        /// 获取页面处理方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IHttpHandler GetHandler(HttpContext context)
        {
            if (m_handlerType==null)
            {
                if (m_defaultRequestConfig!=null)
                {
                    return m_defaultRequestConfig.GetHandler(context);
                }
                else
                {
                    throw new Exception("没有配置页面处理程序：请检查配置结点[DefaultPage][MaPage]是否配置了结点[Handler]");
                }
            }
            else
            {
                IHttpHandler _obj = Activator.CreateInstance(m_handlerType) as IHttpHandler;
                return _obj;
            }
        }

        #endregion

        #region 5、模板信息，待完善正则匹配

        #region 属性

        /// <summary>
        /// URL正则规则字符串
        /// </summary>
        string m_mappedRegexStr;
        /// <summary>
        /// URL正则规则匹配字符串
        /// </summary>
        public string MappedRegexStr
        {
            get { return m_mappedRegexStr; }
            set { m_mappedRegexStr = value; }
        }

        /// <summary>
        /// URL正则规则
        /// </summary>
        Regex m_mappedRegex;

        /// <summary>
        /// 模板正则规则
        /// </summary>
        string m_templateRegexStr;
        /// <summary>
        /// 模板正则规则
        /// </summary>
        public string TemplateRegexStr
        {
            get { return m_templateRegexStr; }
            set { m_templateRegexStr = value; }
        }
        #endregion

        #region 获取当前请求对应的模板名称

        /// <summary>
        /// 获取当前请求页面对应的模板名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetTemplateName()
        {
            //待完善正则匹配
            string _template = MaUrl.Current.TemplateFile;

            if (!string.IsNullOrEmpty(m_templateRegexStr))
            {
                _template = m_templateRegexStr;
                if (m_templateRegexStr.Contains("$"))
                {
                    _template = System.Text.RegularExpressions.Regex.Replace(MaUrl.Current.AbsolutePath, m_mappedRegexStr, m_templateRegexStr);
                }
            }
            return _template;
        }

        #endregion

        #endregion

        #region 6、获取页面变量

        /// <summary>
        /// 获取页面变量
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetPageVariable()
        {
            Dictionary<string, string> _info = new Dictionary<string, string>();
            if (m_mappedRegexStr!=null)
            {
                foreach (KeyValuePair<string,string> item in m_variables)
                {
                    if (item.Value.Contains("$"))
                    {
                        string _value = System.Text.RegularExpressions.Regex.Replace(MaUrl.Current.AbsolutePath, m_mappedRegexStr, item.Value, RegexOptions.IgnoreCase);
                        _info.Add(item.Key, _value);
                    }
                    else
                    {
                        _info.Add(item.Key, item.Value);
                    }
                }
            }
            return _info;
        }
        #endregion


        #region 7、解析处理者集合

        List<IMaParser> m_parseList;

        /// <summary>
        /// 解析处理类列表
        /// </summary>
        public List<IMaParser> ParseList
        {
            get { return m_parseList; }
            set { m_parseList = value; }
        }

        bool m_clearParsers = false;

        /// <summary>
        /// 获取解析处理程序集合
        /// </summary>
        /// <returns></returns>
        public virtual List<IMaParser> GetParseList()
        {
            List<IMaParser> _list = new List<IMaParser>();
            if (!m_clearParsers && m_defaultRequestConfig != null)
            {
                foreach (IMaParser item in m_defaultRequestConfig.GetParseList())
                {
                    _list.Add(item);
                }
            }
            foreach (IMaParser item in m_parseList)
            {
                _list.Add(item);
            }
            return _list;
        }

        #endregion
    }
}
