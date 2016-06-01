using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Masir.Web.Security
{
    /// <summary>
    /// Web安全配置信息
    /// </summary>
    public class MaSecurityConfig : MaConfig
    {
        /// <summary>
        /// 获得配置信息
        /// </summary>
        public static MaSecurityConfig Instance
        {
            get
            {
                return MaConfigManager.GetConfigInfo("MaSecurity") as MaSecurityConfig;
            }
        }

        #region 页面相关

        private string m_loginUrl;
        /// <summary>
        /// 登录页面地址
        /// </summary>
        public string LoginUrl
        {
            get
            {
                return m_loginUrl;
            }
        }
        private string m_defaultUrl;
        /// <summary>
        /// 定义在身份验证之后用于重定向的默认 URL。
        /// </summary>
        public string DefaultUrl
        {
            get
            {
                return m_defaultUrl;
            }
        }

        #endregion

        #region Cookie相关属性

        private string m_key;
        /// <summary>
        /// 加密键值
        /// </summary>
        public string Key
        {
            get
            {
                return m_key;
            }
        }

        private string m_cookieName;
        /// <summary>
        /// 认证Cookie名称
        /// </summary>
        public string CookieName
        {
            get
            {
                return m_cookieName;
            }
        }

        private bool m_requireSSL;
        /// <summary>
        /// 指定是否需要 SSL 连接来传输身份验证 Cookie。
        /// </summary>
        public bool RequireSSL
        {
            get
            {
                return m_requireSSL;
            }
        }
        private bool m_slidingExpiration;
        /// <summary>
        /// 指定是否启用可调过期时间。可调过期将 Cookie 的当前身份验证时间重置为在单个会话期间收到每个请求时过期。
        /// </summary>
        public bool SlidingExpiration
        {
            get
            {
                return m_slidingExpiration;
            }
        }
        private int m_timeout;
        /// <summary>
        /// 指定 Cookie 过期前逝去的时间（以整数分钟为单位）。
        /// </summary>
        public int Timeout
        {
            get
            {
                return m_timeout;
            }
        }

        private string m_cookiePath;
        /// <summary>
        /// Cookie路径
        /// </summary>
        public string CookiePath
        {
            get
            {
                return m_cookiePath;
            }
        }
        private HttpCookieMode m_cookieMode;
        /// <summary>
        /// 定义是否使用 Cookie 以及 Cookie 的行为。
        /// </summary>
        public HttpCookieMode CookieMode
        {
            get
            {
                return m_cookieMode;
            }
        }
        private string m_cookieDomain;
        /// <summary>
        /// Cookie 中设置的可选域
        /// </summary>
        public string CookieDomain
        {
            get
            {
                if (CookieUseCurrentRootDomain || string.IsNullOrEmpty(m_cookieDomain))
                {
                    return MaUrl.Current.Domain.MainDomain;
                }
                return m_cookieDomain;
            }
        }
        private bool m_cookieUseCurrentRootDomain;
        /// <summary>
        /// 是否使用当前根域
        /// </summary>
        public bool CookieUseCurrentRootDomain
        {
            get { return m_cookieUseCurrentRootDomain; }
            set { m_cookieUseCurrentRootDomain = value; }
        }

        private bool m_validateIP;
        /// <summary>
        /// 指定是否验证IP信息
        /// </summary>
        public bool ValidateIP
        {
            get
            {
                return m_validateIP;
            }
        }

        #endregion

        #region 验证类相关

        //private bool m_checkUrl;
        ///// <summary>
        ///// 是否验证URL
        ///// </summary>
        //public bool CheckUrl
        //{
        //    get { return m_checkUrl; }
        //    set { m_checkUrl = value; }
        //}

        private string m_maPrincipalType;
        /// <summary>
        /// 用户的类型
        /// </summary>
        public string MaPrincipalType
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MaPrincipalType"]))
                {
                    return ConfigurationManager.AppSettings["MaPrincipalType"];
                }
                return m_maPrincipalType;
            }
        }

        private List<string> m_openDoamin;
        /// <summary>
        /// 开放域名
        /// </summary>
        public List<string> OpenDoamin
        {
            get { return m_openDoamin; }
        }

        private List<string> m_authorizationPath;
        /// <summary>
        /// 需要授权的目录
        /// </summary>
        public List<string> AuthorizationPath
        {
            get
            {
                return m_authorizationPath;
            }
        }
        private List<string> m_openPath;
        /// <summary>
        /// 开放的路径
        /// </summary>
        public List<string> OpenPath
        {
            get
            {
                return m_openPath;
            }
        }

        private List<string[]> m_securityIP;
        /// <summary>
        /// 安全IP范围
        /// </summary>
        public List<string[]> SecurityIP
        {
            get
            {
                return m_securityIP;
            }
        }

        #endregion

        #region 未授权，页面终止方式

        string m_stopInfo;
        /// <summary>
        /// 终止信息
        /// </summary>
        public string StopInfo
        {
            get { return m_stopInfo; }
            set { m_stopInfo = value; }
        }

        StopBrowseType m_stopType;
        /// <summary>
        /// 终止流量的类型
        /// </summary>
        public StopBrowseType StopType
        {
            get { return m_stopType; }
            set { m_stopType = value; }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 变量初始化构造函数
        /// </summary>
        public MaSecurityConfig()
        {
            m_loginUrl = "login.html";//登录路径
            m_defaultUrl = "default.html";//定义在身份验证之后用于重定向的默认 URL。

            m_cookieName = "MaRBACName";//Cookie 名称
            m_requireSSL = false;//指定是否需要 SSL 连接来传输身份验证 Cookie。
            m_slidingExpiration = true;//指定是否启用可调过期时间。可调过期将 Cookie 的当前身份验证时间重置为在单个会话期间收到每个请求时过期。
            m_timeout = 30;//指定 Cookie 过期前逝去的时间（以整数分钟为单位）。
            m_cookiePath = "/";//Cookie路径
            m_cookieMode = HttpCookieMode.UseDeviceProfile;//定义是否使用 Cookie 以及 Cookie 的行为。
            m_validateIP = false;

            m_maPrincipalType = "Masir.WebSite.Security.MaPrincipal";

            m_openDoamin = new List<string>();
            m_authorizationPath = new List<string>();
            m_openPath = new List<string>();

            m_securityIP = new List<string[]>();
            m_securityIP.Add(new string[] { "127.0.0.1", "255.255.255.255" });

            m_stopInfo = "未授权请求";
            m_stopType = StopBrowseType.Redirect;

            m_cookieUseCurrentRootDomain = true;
        }
        #endregion

        #region 加载配置文件信息
        /// <summary>
        /// 加载安全配置信息
        /// </summary>
        /// <param name="node"></param>
        public override void Load(XmlElement node)
        {
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Name == "Cookie")
                {
                    #region Cookie信息
                    if (item["Key"] != null)
                    {
                        m_key = item["Key"].InnerText;
                    }
                    if (item["Name"] != null)
                    {
                        m_cookieName = item["Name"].InnerText;
                    }
                    if (item["RequireSSL"] != null)
                    {
                        m_requireSSL = bool.Parse(item["RequireSSL"].InnerText);
                    }
                    if (item["SlidingExpiration"] != null)
                    {
                        m_slidingExpiration = bool.Parse(item["SlidingExpiration"].InnerText);
                    }
                    if (item["Timeout"] != null)
                    {
                        m_timeout = int.Parse(item["Timeout"].InnerText);
                    }
                    if (item["CookiePath"] != null)
                    {
                        m_cookiePath = item["CookiePath"].InnerText;
                    }
                    if (item["CookieMode"] != null)
                    {
                        //_obj.m_key = item["CookieMode"].InnerText;
                    }
                    if (item["Domain"] != null)
                    {
                        m_cookieDomain = item["Domain"].InnerText;
                    }
                    if (item["UseCurrentRootDomain"] != null)
                    {
                        bool.TryParse(item["UseCurrentRootDomain"].InnerText, out m_cookieUseCurrentRootDomain);
                    }
                    if (item["ValidateIP"] != null)
                    {
                        bool.TryParse(item["ValidateIP"].InnerText, out m_validateIP);
                    }
                    #endregion
                }
                else if (item.Name == "MaUserType")
                {
                    m_maPrincipalType = item.InnerText;
                }
                else if (item.Name == "StopInfo")
                {
                    m_stopInfo = item.InnerText;
                }
                else if (item.Name == "StopType")
                {
                    if (!Enum.TryParse<StopBrowseType>(item.InnerText, out m_stopType))
                    {
                        m_stopType = StopBrowseType.Redirect;
                    }
                }
                else if (item.Name == "LoginUrl")
                {
                    m_loginUrl = item.InnerText;
                }
                else if (item.Name == "DefaultUrl")
                {
                    m_defaultUrl = item.InnerText;
                }
                //需要开放的域名
                else if (item.Name == "OpenDomain")
                {
                    List<string> _list = new List<string>();
                    foreach (XmlNode item1 in item.SelectNodes("Add"))
                    {
                        _list.Add(item1.Attributes["value"].Value.ToLower());
                    }
                    m_openDoamin = _list;
                }
                //需要授权的目录
                else if (item.Name == "AuthorizationPath")
                {
                    List<string> _list = new List<string>();
                    foreach (XmlNode item1 in item.SelectNodes("Add"))
                    {
                        _list.Add(item1.Attributes["value"].Value.ToLower());
                    }
                    m_authorizationPath = _list;
                }
                //需要开放的目录
                else if (item.Name == "OpenPath")
                {
                    List<string> _list = new List<string>();
                    foreach (XmlNode item1 in item.SelectNodes("Add"))
                    {
                        _list.Add(item1.Attributes["value"].Value.ToLower());
                    }
                    m_openPath = _list;
                }
                //安全IP信息
                else if (item.Name == "SecurityIP")
                {
                    List<string[]> _list = new List<string[]>();
                    foreach (XmlNode item1 in item.SelectNodes("Add"))
                    {
                        string[] _ip = new string[] { item1.Attributes["ip"].Value, item1.Attributes["mask"].Value };
                        _list.Add(_ip);
                    }
                    m_securityIP = _list;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 终止请求浏览类型
    /// </summary>
    public enum StopBrowseType : int
    {
        /// <summary>
        /// 重定向到登录页
        /// </summary>
        Redirect = 0,
        /// <summary>
        /// 终止请求，返回空
        /// </summary>
        Stop = 1,
        /// <summary>
        /// 抛出异常
        /// </summary>
        Exception = 2,
        /// <summary>
        /// 输出信息
        /// </summary>
        Info = 3
    }
}
