using Masir.Web.Parse;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Components
{
    public class TagDataHelper
    {
        public TagEntity Tag { get; set; }
        public MaTag pTag { get; set; }

        private int m_totalCount;
        public int TotalCount
        {
            get { return m_totalCount; }
            set { m_totalCount = value; }
        }


        public TagDataHelper(TagEntity tag, MaTag pTag)
        {
            this.Tag = tag;
            this.pTag = pTag;
        }

        public DataTable GetDataTable(string methodName)
        {
            MethodInfo method = this.GetType().GetMethod(methodName, BindingFlags.Instance
                 | BindingFlags.IgnoreCase
                 | BindingFlags.Public);

            var fun = (Func<DataTable>)method.CreateDelegate(typeof(Func<DataTable>), this);

            return fun();
        }


    }
}
