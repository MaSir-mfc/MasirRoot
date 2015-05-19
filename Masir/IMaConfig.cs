using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Masir
{
    public interface IMaConfig
    {
        void Load(XmlElement node);
    }
}
