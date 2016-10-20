using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir
{
    /// <summary>
    /// 配置文件信息，运行更目录下的Ma.config文件
    /// </summary>
    public class MaConfig : IMaConfig
    {
        #region 基本属性

        internal string m_name;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        DateTime m_loadTime;

        /// <summary>
        /// 加载时间
        /// </summary>
        public DateTime LoadTime
        {
            get { return m_loadTime; }
        }

        XmlNode m_configNode;

        /// <summary>
        /// 配置结点
        /// </summary>
        public XmlNode ConfigNode
        {
            get { return m_configNode; }
        }

        #endregion
        
        #region 构造函数

        /// <summary>
        /// 初始化请求地址变量构造
        /// </summary>
        public MaConfig()
        {
            m_variables = new Dictionary<string, string>();
        }

        /// <summary>
        /// 根据请求xml获取请求地址变量构造函数
        /// </summary>
        /// <param name="node"></param>
        public MaConfig(XmlElement node)
        {
            Load(node);
            m_variables = new Dictionary<string, string>();
        }

        #endregion
        
        /// <summary>
        /// 页面标签集合
        /// </summary>
        protected Dictionary<string, string> m_variables;
        /// <summary>
        /// 当前配置结点键值对信息
        /// </summary>
        public Dictionary<string, string> Variables
        {
            get { return m_variables; }
            set { m_variables = value; }
        }

        /// <summary>
        /// 提取请求地址变量
        /// </summary>
        /// <param name="node">匹配的xml</param>
        public virtual void Load(XmlElement node)
        {
            m_loadTime = DateTime.Now;
            m_name = node.Name;
            m_configNode = node;

            #region 提取变量信息

            if (node["Variables"] != null)
            {
                foreach (XmlNode item in node["Variables"].SelectNodes("add"))
                {
                    if (item.Attributes["key"] != null
                        && item.Attributes["value"] != null)
                    {
                        m_variables[item.Attributes["key"].Value] = item.Attributes["value"].Value;
                    }
                }
            }

            #endregion
        }
    }
}
