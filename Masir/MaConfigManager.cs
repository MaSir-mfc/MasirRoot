using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir
{
    /// <summary>
    /// 配置文件读取与缓存类
    /// </summary>
    public class MaConfigManager
    {
        static MemoryCache m_configCache;

        static string m_defaultConfigFile;

        /// <summary>
        /// 获取配置文件构造函数
        /// </summary>
        static MaConfigManager()
        {
            m_configCache = new MemoryCache(new Guid().ToString());
            m_defaultConfigFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Ma.config";

            //读取配置文件
            if (System.IO.File.Exists(m_defaultConfigFile))
            {
                //读取XML信息
                XmlDocument _reader = new XmlDocument();
                _reader.Load(m_defaultConfigFile);
                foreach (XmlNode item in _reader.DocumentElement.ChildNodes)
                {
                    if (item is XmlElement)
                    {
                        ConfigAndCache((XmlElement)item);
                    }
                }
            }
        }

        #region 加载并缓存配置信息
        /// <summary>
        /// 读取配置文件并缓存
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static IMaConfig ConfigAndCache(XmlElement node)
        {
            List<string> _filePaths = new List<string>();
            _filePaths.Add(m_defaultConfigFile);

            #region 读取首个配置文件并得到子配置文件

            XmlElement _node = node;
            if (_node.Attributes["file"] != null)
            {
                string _fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + _node.Attributes["file"].Value);
                if (System.IO.File.Exists(_fileName))
                {
                    XmlDocument _configXml = new XmlDocument();
                    _configXml.Load(_fileName);
                    _node = _configXml.DocumentElement;
                    _filePaths.Add(_fileName);
                }
            }

            #endregion

            #region 处理子节点外部文件

            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(node.OuterXml)))
            {
                while (reader.Read())
                {
                    if (reader.GetAttribute("file") != null)
                    {
                        string _fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + reader.GetAttribute("file"));
                        if (System.IO.File.Exists(_fileName))
                        {
                            _filePaths.Add(_fileName);
                        }
                    }
                }
            }

            #endregion

            #region 获取站点对应的配置文件

            if ((_node.Attributes["type"] != null) && (!string.IsNullOrEmpty(_node.Attributes["type"].Value)))
            {
                //通过反射获得站点配置对象
                Type type = GetType(_node.Attributes["type"].Value);
                if (type != null)
                {
                    IMaConfig _obj = Activator.CreateInstance(type) as IMaConfig;
                    if (_obj != null)
                    {
                        _obj.Load(_node);
                        //缓存配置信息
                        CacheItemPolicy _cachePolicy = new CacheItemPolicy();
                        _cachePolicy.ChangeMonitors.Add(new HostFileChangeMonitor(_filePaths));
                        m_configCache.Set(node.Name, _obj, _cachePolicy);
                    }
                    return _obj;
                }
            }

            #endregion

            return null;
        }
        #endregion

        #region 加载配置信息（没有缓存）

        /// <summary>
        /// 通过配置节点加载配置信息，没有缓存
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IMaConfig LoadConfig(XmlElement node)
        {
            List<string> _filePaths = new List<string>();
            _filePaths.Add(m_defaultConfigFile);
            XmlElement _node = node;
            if (_node.Attributes["file"] != null)
            {
                string _fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + _node.Attributes["file"].Value);
                if (System.IO.File.Exists(_fileName))
                {
                    XmlDocument _configXml = new XmlDocument();
                    _configXml.Load(_fileName);
                    _node = _configXml.DocumentElement;
                    _filePaths.Add(_fileName);
                }
            }

            if ((_node.Attributes["type"] != null)
                && (!string.IsNullOrEmpty(_node.Attributes["type"].Value))
                )
            {
                //通过反射获得站点配置对象
                Type type = GetType(_node.Attributes["type"].Value);
                if (type != null)
                {
                    IMaConfig _obj = Activator.CreateInstance(type) as IMaConfig;
                    if (_obj != null)
                    {
                        _obj.Load(_node);
                    }
                    return _obj;
                }
            }

            return null;
        }

        #endregion

        #region 从配置文件加载配置信息

        /// <summary>
        /// 根据配置键 获得配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IMaConfig GetConfigInfo(string key)
        {
            if (m_configCache.Contains(key))
            {
                return (IMaConfig)m_configCache[key];
            }
            else
            {
                #region 从默认配置文件加载并且缓存

                if (System.IO.File.Exists(m_defaultConfigFile))
                {
                    //读取XML信息
                    XmlDocument _reader = new XmlDocument();
                    _reader.Load(m_defaultConfigFile);
                    foreach (XmlNode item in _reader.DocumentElement.ChildNodes)
                    {
                        if (item is XmlElement)
                        {
                            if ((key == item.Name))
                            {
                                return ConfigAndCache((XmlElement)item);
                            }
                        }
                    }
                }

                #endregion
            }
            return null;
        }
        #endregion

        #region 提取类型

        /// <summary>
        /// 提取类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetType(string name)
        {
            Type _type = null;

            _type = Type.GetType(name, false, false);
            if (_type == null)
            {
                _type = System.Web.Compilation.BuildManager.GetType(name, false, false);
            }
            if (_type == null)
            {
                foreach (System.Reflection.Assembly item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    _type = item.GetType(name, false, false);
                    if (_type != null)
                    {
                        return _type;
                    }
                }
            }

            return _type;
        }

        #endregion
    }
}
