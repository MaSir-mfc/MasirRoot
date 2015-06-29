using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web.Security
{
    /// <summary>
    /// 安全验证工具
    /// </summary>
    public class MaSecurityHelper
    {
        /// <summary>
        /// 根据用户票证获取cookie
        /// </summary>
        /// <param name="userTicket"></param>
        /// <returns></returns>
        public static HttpCookie GetAuthCookie(MaUserTicket userTicket)
        {
            //根据用户票证生成cookie
            HttpCookie cookie = new HttpCookie(MaSecurityConfig.Instance.CookieName, userTicket.ToString());
            cookie.HttpOnly = false;
            cookie.Path = MaSecurityConfig.Instance.CookiePath;
            cookie.Secure = MaSecurityConfig.Instance.RequireSSL;
            if (MaSecurityConfig.Instance.CookieDomain != null)
            {
                cookie.Domain = MaSecurityConfig.Instance.CookieDomain;
            }
            if (userTicket.CreatePersistentCookie)
            {//创建持久cookie
                cookie.Expires = userTicket.Expiration;
            }
            return cookie;
        }

        /// <summary>
        /// 为提供的用户名创建一个身份验证票证，并将其添加到响应的cookie集合或URL
        /// </summary>
        /// <param name="username">已验证身份的用户的名称</param>
        /// <param name="timeOut">过期时间</param>
        /// <param name="createPersistentCookie">持久cookie</param>
        /// <param name="slidingExpiration">是否启用可调过期时间</param>
        public static void SetAuthCookie(string username, int timeOut, bool createPersistentCookie, bool slidingExpiration)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }
            //生成用户票证
            MaUserTicket _userTicket = new MaUserTicket(username, timeOut, slidingExpiration, createPersistentCookie);
            //写入cookie
            HttpContext.Current.Response.Cookies.Add(GetAuthCookie(_userTicket));

            //设置用户
            Type type = System.Web.Compilation.BuildManager.GetType(MaSecurityConfig.Instance.MaPrincipalType, false, false);
            HttpContext.Current.User = Activator.CreateInstance(type, new object[] { username }) as MaPrincipal;
        }

        /// <summary>
        /// 为提供的用户名创建一个身份验证票证，并将其添加到响应的cookie集合或URL
        /// </summary>
        /// <param name="username">已验证身份的用户的名称</param>
        /// <param name="createPersistentCookie">持久cookie</param>
        public static void SetAuthCookie(string username, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }
            //生成用户票证
            MaUserTicket _userTicket = new MaUserTicket(username, MaSecurityConfig.Instance.Timeout, MaSecurityConfig.Instance.SlidingExpiration, createPersistentCookie);
            //写入cookie
            HttpContext.Current.Response.Cookies.Add(GetAuthCookie(_userTicket));

            //设置用户
            Type type = System.Web.Compilation.BuildManager.GetType(MaSecurityConfig.Instance.MaPrincipalType, false, false);
            HttpContext.Current.User = Activator.CreateInstance(type, new object[] { username }) as MaPrincipal;
        }

        /// <summary>
        /// 从浏览器删除Forms身份验证票证
        /// </summary>
        public static void SignOut()
        {
            HttpCookie cookie = new HttpCookie(MaSecurityConfig.Instance.CookieName, "");
            cookie.HttpOnly = false;
            cookie.Path = MaSecurityConfig.Instance.CookiePath;
            cookie.Secure = MaSecurityConfig.Instance.RequireSSL;
            if (MaSecurityConfig.Instance.CookieDomain != null)
            {
                cookie.Domain = MaSecurityConfig.Instance.CookieDomain;
            }
            cookie.Expires = new DateTime(0x7cf, 10, 12);
            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 从cookie获得用户票证
        /// </summary>
        /// <returns></returns>
        public static MaUserTicket GetTicketFormCookie()
        {
            HttpCookie _cookie = HttpContext.Current.Request.Cookies[MaSecurityConfig.Instance.CookieName];
            if (_cookie == null)
            {
                return null;
            }
            try
            {
                MaUserTicket _userTicket = new MaUserTicket(_cookie.Value);
                return _userTicket;
            }
            catch
            {
                SignOut();
                return null;
            }
        }
        /// <summary>
        /// 判断当前用户是否拥有相关权限
        /// </summary>
        /// <param name="str">权限值</param>
        /// <returns></returns>
        public static bool IsHavePotence(string str)
        {
            return HttpContext.Current.User.IsInRole(str);
        }
    }
}
