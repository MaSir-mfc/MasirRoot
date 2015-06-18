using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir.Data
{
    /// <summary>
    /// 数据库处理基类
    /// </summary>
    public class DataBaseConfig : MaConfig
    {
        /// <summary>
        /// 获得数据库配置信息
        /// </summary>
        public static DataBaseConfig Instance
        {
            get
            {
                return (DataBaseConfig)MaConfigManager.GetConfigInfo("DataBase");
            }
        }

        private Dictionary<string, DataBaseInfo> m_dataBaseList;

        /// <summary>
        /// 连接字符串集合
        /// </summary>
        public Dictionary<string, DataBaseInfo> DataBaseList
        {
            get { return m_dataBaseList; }
            set { m_dataBaseList = value; }
        }

        /// <summary>
        /// 构造初始化连接字符串集合
        /// </summary>
        public DataBaseConfig()
        {
            m_dataBaseList = new Dictionary<string, DataBaseInfo>();
        }

        /// <summary>
        /// 从集合中获得数据连接信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataBaseInfo GetDataBaseInfoByCache(string name)
        {
            DataBaseInfo _info;
            if (!m_dataBaseList.TryGetValue(name,out _info))
            {
                throw new Exception("无该数据库[" + name + "]的连接信息！");
            }
            return _info;
        }

        #region 加载配置文件信息

        /// <summary>
        /// 加载配置文件信息
        /// </summary>
        /// <param name="node"></param>
        public override void Load(System.Xml.XmlElement node)
        {
            base.Load(node);
            foreach (XmlNode item in node.SelectSingleNode("ConnString"))
            {
                #region 链接字符串表示的数据库信息

                //数据库名称
                string _name = item.Name;
                //连接字符串
                string _connStr = item.InnerText;
                if (item.Attributes["IfEncrypt"] != null)
                {
                    if (item.Attributes["IfEncrypt"].Value == "true")
                    {
                        _connStr = Components.TextHelper.AESDecrypt(_connStr);
                    }
                }
                else
                {
                    _connStr = Components.TextHelper.AESDecrypt(_connStr);
                }
                //连接类型
                DataBaseType _type = DataBaseType.Sql;
                if (item.Attributes["Type"] != null)
                {
                    switch (item.Attributes["Type"].Value)
                    {
                        case "Sql":
                            _type = DataBaseType.Sql;
                            break;
                        case "Oracle":
                            _type = DataBaseType.Oracle;
                            break;
                        case "MySql":
                            _type = DataBaseType.MySql;
                            break;
                        case "Db":
                            _type = DataBaseType.Db;
                            break;
                        case "Access":
                            _type = DataBaseType.Access;
                            break;
                        default:
                            _type = DataBaseType.Sql;
                            break;
                    }
                }
                if (string.IsNullOrEmpty(_connStr))
                {
                    continue;
                }
                DataBaseInfo _info = new DataBaseInfo(_name, _connStr, _type);
                if (m_dataBaseList.ContainsKey(_name))
                {
                    throw new Exception("数据库配置[" + Name + "]已经添加了相同的数据库信息[" + _name + "]");
                }
                else
                {
                    m_dataBaseList.Add(_name, _info);
                }

                #endregion
            }
        }
        #endregion
    }
}
