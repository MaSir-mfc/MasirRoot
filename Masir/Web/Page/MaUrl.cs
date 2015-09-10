using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web.Page
{
    /// <summary>
    /// 页面地址处理
    /// </summary>
    public class MaUrl
    {
        #region 获取当前请求URL信息

        internal const string PAGE_REQUEST_URL_INFO_KEY = "MA_PAGE_REQUEST_URL_INFO_KEY";

        /// <summary>
        /// 获取页面地址信息
        /// </summary>
        public static MaUrl Current
        {
            get
            {
                if (HttpContext.Current.Items[PAGE_REQUEST_URL_INFO_KEY] == null)
                {
                    HttpContext.Current.Items[PAGE_REQUEST_URL_INFO_KEY] = new MaUrl(HttpContext.Current);
                }
                return HttpContext.Current.Items[PAGE_REQUEST_URL_INFO_KEY] as MaUrl;
            }
            set
            {
                HttpContext.Current.Items[PAGE_REQUEST_URL_INFO_KEY] = value as MaUrl;
            }
        }

        #endregion

        #region URL相关信息

        private MaDomain m_domain;

        /// <summary>
        /// 域名信息
        /// </summary>
        public MaDomain Domain
        {
            get { return m_domain; }
            set { m_domain = value; }
        }

        private string m_absolutePath;
        /// <summary>
        /// 请求路径,带“/”，小写副本
        /// </summary>
        public string AbsolutePath
        {
            get { return m_absolutePath; }
            set { m_absolutePath = value; }
        }

        private NameValueCollection m_queryString;

        /// <summary>
        /// 请求字符串
        /// </summary>
        public NameValueCollection QueryString
        {
            get { return m_queryString; }
            set { m_queryString = value; }
        }

        #endregion

        #region 属性

        private string m_requestFilePathExtension;


        /// <summary>
        /// 请求文件扩展名
        /// </summary>
        public string RequestFilePathExtension
        {
            get { return m_requestFilePathExtension; }
            set { m_requestFilePathExtension = value; }
        }

        private MaRequestType m_requestType;

        /// <summary>
        /// 页面请求类型
        /// </summary>
        public MaRequestType RequestType
        {
            get { return m_requestType; }
            set { m_requestType = value; }
        }

        private MaRequestView m_requestView;

        /// <summary>
        /// 页面请求视图
        /// </summary>
        public MaRequestView RequestView
        {
            get { return m_requestView; }
            set { m_requestView = value; }
        }

        private string m_templateFile;

        /// <summary>
        /// 对应的模板文件
        /// </summary>
        public string TemplateFile
        {
            get { return m_templateFile; }
            set { m_templateFile = value; }
        }

        private string m_savePath;

        /// <summary>
        /// 对应的物理路径
        /// </summary>
        public string SavePath
        {
            get { return m_savePath; }
            set { m_savePath = value; }
        }
        #endregion

        System.Web.HttpContext m_context;

        #region 构造函数
        /// <summary>
        /// 页面地址构造函数
        /// </summary>
        /// <param name="context"></param>
        public MaUrl(HttpContext context)
        {
            #region 请求地址

            m_context = context;
            Uri _requestURL = context.Request.Url;

            m_domain = new MaDomain(_requestURL.Host);
            m_queryString = context.Request.QueryString;
            m_absolutePath = _requestURL.AbsolutePath.ToLower();

            #endregion

            #region 请求类型

            m_requestFilePathExtension = context.Request.CurrentExecutionFilePathExtension;
            m_requestView = MaRequestView.Put;


            switch (m_requestFilePathExtension)
            {
                case ".htm"://静态页面
                    m_requestView = MaRequestView.Put;
                    m_requestType = MaRequestType.HTM;
                    break;
                case ".html"://动态页面
                    m_requestView = MaRequestView.Put;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".html", ".htm");
                    break;
                case ".htme"://编辑视图
                    m_requestView = MaRequestView.Edit;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".htme", ".htm");
                    break;
                case ".htms"://保存视图
                    m_requestView = MaRequestView.Save;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".htms", ".htm");
                    break;
                case ".htmx"://更新视图
                    m_requestView = MaRequestView.Update;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".htmx", ".htm");
                    break;
                case ".htmd"://删除视图
                    m_requestView = MaRequestView.Delete;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".htmd", ".htm");
                    break;
                case ".htmn"://清空视图
                    m_requestView = MaRequestView.Null;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".htmn", ".htm");
                    break;
                case ".htmc"://缓存视图
                    m_requestView = MaRequestView.Cache;
                    m_requestType = MaRequestType.HTM;
                    m_absolutePath = m_absolutePath.Replace(".htmc", ".htm");
                    break;
                default:
                    if (string.IsNullOrEmpty(context.Request.CurrentExecutionFilePathExtension))
                    {
                        m_requestView = MaRequestView.Put;
                        m_requestType = MaRequestType.HTM;
                    }
                    else
                    {
                        m_requestType = MaRequestType.Other;
                    }
                    break;
            }

            #endregion

            m_templateFile = (m_requestType == MaRequestType.HTM) ? m_absolutePath + "l" : m_absolutePath;
            //模板路径
            if (string.IsNullOrEmpty(context.Request.CurrentExecutionFilePathExtension))
            {
                m_templateFile = m_absolutePath + "index.html";
            }

            //物理路径
            m_savePath = context.Server.MapPath(m_absolutePath);
        }

        #endregion

        public override string ToString()
        {
            return m_context.Request.Url.ToString();
        }
    }

    #region 域名信息

    /// <summary>
    /// 域名信息
    /// </summary>
    public class MaDomain
    {
        private string m_domain;
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain
        {
            get { return m_domain; }
            set { m_domain = value; }
        }

        private string m_tdl;

        /// <summary>
        /// 顶级域名（top level domain，.com）
        /// </summary>
        public string TDL
        {
            get { return m_tdl; }
            set { m_tdl = value; }
        }

        private string m_mainDomain;

        /// <summary>
        /// 主域（二级域名,Masir.com）
        /// </summary>
        public string MainDomain
        {
            get { return m_mainDomain; }
            set { m_mainDomain = value; }
        }

        private string m_secondDomain;

        /// <summary>
        /// 主机名
        /// </summary>
        public string SecondDomain
        {
            get { return m_secondDomain; }
            set { m_secondDomain = value; }
        }

        /// <summary>
        /// 域名信息构造
        /// </summary>
        /// <param name="domain"></param>
        public MaDomain(string domain)
        {
            m_domain = domain.ToLower();
            string[] _domain = m_domain.Split('.');

            if (_domain.Length > 2)
            {
                if ((_domain[_domain.Length - 2] == "com") || (_domain[_domain.Length - 2] == "net") || (_domain[_domain.Length - 2] == "org"))
                {
                    m_tdl = _domain[_domain.Length - 2] + "." + _domain[_domain.Length - 1];
                    m_mainDomain = _domain[_domain.Length - 3] + "." + _domain[_domain.Length - 2] + "." + _domain[_domain.Length - 1];
                    m_secondDomain = m_domain.Replace("." + m_mainDomain, "");
                }
                else
                {
                    m_tdl = _domain[_domain.Length - 1];
                    m_mainDomain = _domain[_domain.Length - 2] + "." + _domain[_domain.Length - 1];
                    m_secondDomain = m_domain.Replace("." + m_mainDomain, "");
                }
            }
            else
            {
                m_mainDomain = m_domain;
            }
        }
    }
    #endregion

    #region 请求试图

    /// <summary>
    ///  页面请求视图，由页面后缀名决定
    /// </summary>
    public enum MaRequestView
    {
        /// <summary>
        /// 发布（正常）试图
        /// </summary>
        Put = 0,
        /// <summary>
        /// 编辑视图,[.htme]
        /// </summary>
        Edit = 1,
        /// <summary>
        /// 更新页面,[.htmx]
        /// </summary>
        Update = 2,
        /// <summary>
        /// 删除页面,[.htmd]
        /// </summary>
        Delete = 3,
        /// <summary>
        /// 保存视图,[.htms]
        /// </summary>
        Save = 4,
        /// <summary>
        /// 清空,[.htmn]
        /// </summary>
        Null = 5,
        /// <summary>
        /// 缓存页面，[.htmc]
        /// </summary>
        Cache = 6
    }

    #endregion

    #region 保存方法

    /// <summary>
    /// 页面保存方法
    /// </summary>
    public enum MaPageSaveWay
    {
        /// <summary>
        /// 没有配置
        /// </summary>
        Null,
        /// <summary>
        /// 静态化处理页面
        /// </summary>
        Save,
        /// <summary>
        /// 不带参数，缓存页面
        /// </summary>
        Cache,
        /// <summary>
        /// 不做处理
        /// </summary>
        None
    }
    #endregion

    #region 请求类型

    /// <summary>
    /// 请求类型
    /// </summary>
    public enum MaRequestType
    {
        /// <summary>
        /// Htm页面
        /// </summary>
        HTM,
        /// <summary>
        /// 图片
        /// </summary>
        PIC,
        /// <summary>
        /// 样式文件
        /// </summary>
        CSS,
        /// <summary>
        /// JS脚本文件
        /// </summary>
        JS,
        /// <summary>
        /// Falsh文件
        /// </summary>
        Falsh,
        /// <summary>
        /// ASPX页面
        /// </summary>
        ASPX,
        /// <summary>
        /// 其他文件
        /// </summary>
        Other,
        /// <summary>
        /// Wap手机页面
        /// </summary>
        Wap
    }
    #endregion
}
