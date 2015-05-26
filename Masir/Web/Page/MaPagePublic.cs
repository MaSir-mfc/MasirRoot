using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir.Web.Page
{
    /// <summary>
    /// 公共页面处理
    /// </summary>
    public class MaPagePublic : IMaConfig
    {
        /// <summary>
        /// 初始化页面处理抽象类构造
        /// </summary>
        public MaPagePublic()
        {
            m_pageList = new List<MaPage>();
        }

        #region 属性

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

        #endregion

        /// <summary>
        /// 实现
        /// </summary>
        /// <param name="node"></param>
        public void Load(XmlElement node)
        {
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
        }
    }
}
