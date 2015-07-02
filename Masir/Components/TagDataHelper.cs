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
    /// <summary>
    /// 
    /// </summary>
    public class TagDataHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public TagEntity Tag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MaTag pTag { get; set; }

        private int m_totalCount;
        /// <summary>
        /// 
        /// </summary>
        public int TotalCount
        {
            get { return m_totalCount; }
            set { m_totalCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pTag"></param>
        public TagDataHelper(TagEntity tag, MaTag pTag)
        {
            this.Tag = tag;
            this.pTag = pTag;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
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
