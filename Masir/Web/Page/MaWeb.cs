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
    /// 页面请求起始处理类
    /// </summary>
    public class MaWeb : MaConfig, IMaWeb
    {
        /// <summary>
        /// 获取站点配置信息
        /// </summary>
        public static MaWeb Instance
        {
            get
            {
                return (MaWeb)MaConfigManager.GetConfigInfo("MaWeb");
            }
        }
        /// <summary>
        /// 初始化 构造函数
        /// </summary>
        public MaWeb()
        {
            m_maSiteConfigList = new List<IMaSite>();
            m_maSiteHostList = new Dictionary<string, string>();
        }

        /// <summary>
        /// 站点配置信息
        /// </summary>
        protected List<IMaSite> m_maSiteConfigList;

        /// <summary>
        /// 站点host信息（非必要）
        /// </summary>

        protected Dictionary<string, string> m_maSiteHostList;

        #region 加载配置信息

        /// <summary>
        /// 加载主机站点信息
        /// </summary>
        /// <param name="node"></param>
        public override void Load(System.Xml.XmlElement node)
        {
            base.Load(node);

            XmlNodeList _siteNodeList = node.SelectNodes("MaSite");
            foreach (XmlElement item in _siteNodeList)
            {
                IMaSite _site = MaConfigManager.LoadConfig(item) as IMaSite;
                if (_site!=null)
                {
                    #region 设置站点主机头

                    //设置站点主机头
                    XmlNodeList _hostNodeList = item.SelectNodes("host");
                    foreach (XmlElement host in _hostNodeList)
                    {
                        if (host.Attributes["value"] != null
                            && !string.IsNullOrEmpty(host.Attributes["value"].Value))
                        {
                            _site.SetHost(host.Attributes["value"].Value);
                        }
                    }

                    #endregion
                    m_maSiteHostList[item.Attributes["name"].Value] = "http://" + _hostNodeList[0].Attributes["value"].Value;
                    m_maSiteConfigList.Add(_site);
                }
                else
                {
                    throw new Exception("站点信息配置错误：" + item.Name + ":" + item.OuterXml);
                }
            }
        }

        #endregion

        #region 匹配当前请求站点信息

        /// <summary>
        /// 匹配当前请求站点信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IMaSite GetSite(HttpContext context)
        {
            foreach (IMaSite item in m_maSiteConfigList)
            {
                if (item.IsThis(context))
                {
                    return item;
                }
            }
            return m_maSiteConfigList[0];
        }

        #endregion

        #region 获取host列表

        /// <summary>
        /// 获取host列表
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> GetHost()
        {
            return m_maSiteHostList;
        }
        #endregion
    }
}
