using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Masir.Web.Page
{
    /// <summary>
    /// 站点配置读取
    /// </summary>
    public class MaSite : MaConfig, IMaSite
    {
        #region 构造函数
        /// <summary>
        /// 初始化构造
        /// </summary>
        public MaSite()
        {
            m_hostList = new List<string>();
            m_skinList = new List<IMaSkin>();
            m_pageList = new List<MaPage>();
            m_pulicPageConfig = new List<MaPagePublic>();
        }
        #endregion

        #region 属性字段

        /// <summary>
        /// 默认皮肤配置信息
        /// </summary>
        protected IMaSkin m_defaulutSkin;

        /// <summary>
        /// 默认皮肤配置信息
        /// </summary>
        public IMaSkin DefaulutSkin
        {
            get { return m_defaulutSkin; }
            set { m_defaulutSkin = value; }
        }

        /// <summary>
        /// 皮肤配置信息集合
        /// </summary>
        protected List<IMaSkin> m_skinList;

        /// <summary>
        /// 皮肤配置信息集合
        /// </summary>
        public List<IMaSkin> SkinList
        {
            get { return m_skinList; }
            set { m_skinList = value; }
        }

        /// <summary>
        /// 默认页面配置信息
        /// </summary>
        protected MaPage m_defaultPage;

        /// <summary>
        /// 默认页面配置信息
        /// </summary>
        public MaPage DefaultPage
        {
            get { return m_defaultPage; }
            set { m_defaultPage = value; }
        }

        /// <summary>
        /// 页面配置信息集合
        /// </summary>
        protected List<MaPage> m_pageList;

        /// <summary>
        /// 页面配置信息集合
        /// </summary>
        public List<MaPage> PageList
        {
            get { return m_pageList; }
            set { m_pageList = value; }
        }

        /// <summary>
        /// 公共页面配置
        /// </summary>
        protected List<MaPagePublic> m_pulicPageConfig;
        /// <summary>
        /// 公共页面配置
        /// </summary>
        public List<MaPagePublic> PulicPageConfig
        {
            get { return m_pulicPageConfig; }
            set { m_pulicPageConfig = value; }
        }

        #endregion

        #region 加载站点信息

        /// <summary>
        /// 加载站点配置节点信息
        /// </summary>
        /// <param name="node"></param>
        public override void Load(System.Xml.XmlElement node)
        {
            base.Load(node);

            #region 加载皮肤

            //加载默认皮肤
            if (node["DefaultSkin"] != null)
            {
                m_defaulutSkin = MaConfigManager.LoadConfig(node["DefaultSkin"]) as IMaSkin;
            }

            //加载皮肤信息
            foreach (XmlElement item in node.SelectNodes("MaSkin"))
            {
                IMaSkin _skin = MaConfigManager.LoadConfig(item) as IMaSkin;
                if (_skin != null)
                {
                    m_skinList.Add(_skin);
                }
                else
                {
                    throw new Exception("站点皮肤配置错误：" + item.Name);
                }
            }

            #endregion

            #region 加载页面

            //加载默认页面
            if (node["DefaultPage"] != null)
            {
                m_defaultPage = MaConfigManager.LoadConfig(node["DefaultPage"]) as MaPage;
            }

            //加载页面信息
            foreach (XmlElement item in node.SelectNodes("MaPage"))
            {
                MaPage _page = MaConfigManager.LoadConfig(item) as MaPage;
                if (_page != null)
                {
                    //设置页面默认处理配置信息
                    _page.SetDefault(m_defaultPage);
                    //处理页面信息
                    m_pageList.Add(_page);
                }
                else
                {
                    throw new Exception("页面信息配置错误：" + item.Name);
                }
            }
            #endregion

            #region 加载公共配置

            //加载公共页面配置信息
            foreach (XmlElement item in node.SelectNodes("MaPagePublic"))
            {
                MaPagePublic _config = MaConfigManager.LoadConfig(item) as MaPagePublic;
                if (_config != null)
                {
                    m_pulicPageConfig.Add(_config);
                }
            }

            #endregion

        }

        #endregion

        #region 获取当前请求对应的皮肤

        /// <summary>
        /// 获取当前请求对应的皮肤
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IMaSkin GetSkinConfig(HttpContext context)
        {
            foreach (IMaSkin item in m_skinList)
            {
                if (item.IsThis(context))
                {
                    return item;
                }
            }
            return m_defaulutSkin;
        }
        #endregion

        #region 获取当前请求对应的处理页面信息

        /// <summary>
        /// 提取页面处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public MaPage GetRequestConfig(HttpContext context)
        {
            //匹配私有配置
            foreach (MaPage item in m_pageList)
            {
                if (item.IsThis(context))
                {
                    return item;
                }
            }
            //匹配公共配置
            foreach (MaPagePublic item in m_pulicPageConfig)
            {
                foreach (MaPage page in m_pageList)
                {
                    if (page.IsThis(context))
                    {
                        return page;
                    }
                }
            }
            return m_defaultPage;
        }
        #endregion

        #region 检测当前请求是否适应此站点配置

        /// <summary>
        /// 主机集合
        /// </summary>
        List<string> m_hostList;

        /// <summary>
        /// 设置当前站点对应的主机host
        /// </summary>
        /// <param name="host"></param>
        public void SetHost(string host)
        {
            if (!string.IsNullOrEmpty(host))
            {
                m_hostList.Add(host.ToLower());
            }
        }

        /// <summary>
        /// 检测当前请求是否适应此站点配置，host完全匹配模式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsThis(HttpContext context)
        {
            foreach (string item in m_hostList)
            {
                if (item==context.Request.Url.Host)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
