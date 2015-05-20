using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir
{
    /// <summary>
    /// 配置文件接口
    /// </summary>
    public interface IMaConfig
    {
        /// <summary>
        /// 读取config文件
        /// </summary>
        /// <param name="node"></param>
        void Load(XmlElement node);
    }
}
