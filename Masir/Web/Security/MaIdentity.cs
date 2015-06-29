using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Security
{
    /// <summary>
    /// 用户标识
    /// </summary>
    public class MaIdentity : IIdentity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        protected string m_name;
        /// <summary>
        /// 是否验证了用户
        /// </summary>
        protected bool m_isAuthenticated;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="username">用户名</param>
        public MaIdentity(string username)
        {
            m_name = username;
            m_isAuthenticated = true;
        }
        #region IIdentity成员
        /// <summary>
        /// 认证类型
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "RBAC认证类型";
            }
        }
        /// <summary>
        /// 是否验证了用户
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return m_isAuthenticated;
            }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        #endregion
    }
}
