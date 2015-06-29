using Masir.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Security
{
    /// <summary>
    /// 用户票证
    /// </summary>
    public sealed class MaUserTicket
    {
        #region 票证信息
        /// <summary>
        /// 加密后的字符
        /// </summary>
        private string m_encryptValue;
        /// <summary>
        /// 解密后的字符
        /// </summary>
        private string m_decryptValue;
        /// <summary>
        /// 用户名
        /// </summary>
        private string m_username;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username
        {
            get { return m_username; }
        }
        /// <summary>
        /// 客户最后访问IP
        /// </summary>
        private string m_ip;
        /// <summary>
        /// 最后更新IP
        /// </summary>
        public string Ip
        {
            get { return m_ip; }
        }

        /// <summary>
        /// 客户最后访问时间
        /// </summary>
        private DateTime m_time;
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime Time
        {
            get { return m_time; }
        }

        private bool m_slidingExpiration = false;
        /// <summary>
        /// 指定是否启用可调过期时间。可调过期将Cookie的当前身份验证时间重置为在单个回话期间收到每个请求时过期
        /// </summary>
        public bool SlidingExpiration
        {
            get { return m_slidingExpiration; }
        }


        private int m_timeout;
        /// <summary>
        /// 指定cookie过期前逝去的时间（以整数分钟为单位）
        /// </summary>
        public int Timeout
        {
            get { return m_timeout; }
        }

        /// <summary>
        /// 票证过期的时间
        /// </summary>
        private DateTime m_expiration;
        /// <summary>
        /// 票证过期的时间
        /// </summary>
        public DateTime Expiration
        {
            get
            {
                if (m_slidingExpiration)
                {//调用过期时间
                    m_expiration = DateTime.Now.AddMinutes((double)m_timeout);
                }
                return m_expiration;
            }
        }
        /// <summary>
        /// 票证是否已过期
        /// </summary>
        public bool Expired
        {
            get { return DateTime.Now > Expiration; }
        }

        private bool m_createPersistentCookie = false;
        /// <summary>
        /// 若要创建持久Cookie（跨浏览器回话保存的cookie），则为true；否则为false
        /// </summary>
        public bool CreatePersistentCookie
        {
            get { return m_createPersistentCookie; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code">票证代码</param>
        public MaUserTicket(string code)
        {
            m_encryptValue = code;
            m_decryptValue = TextHelper.AESDecrypt(m_encryptValue);
            string[] valueArrage = m_decryptValue.Split('|');
            m_username = TextHelper.AESDecrypt(valueArrage[0]);//用户名
            m_ip = valueArrage[1];//最后IP
            m_expiration = DateTime.Parse(valueArrage[2]);//过期时间
            m_time = DateTime.Parse(valueArrage[3]);//最后更新时间
            m_slidingExpiration = bool.Parse(valueArrage[4]);//是否调用过期
            m_createPersistentCookie = bool.Parse(valueArrage[5]);//是否调用过期
            m_timeout = int.Parse(valueArrage[6]);//过期时间间隔        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="outTime">过期时间</param>
        public MaUserTicket(string username,int outTime)
        {
            m_username = username;
            m_timeout = outTime;
            m_expiration = DateTime.Now.AddMinutes((double)outTime);
            m_ip = MaWebUtility.GetIP();        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="outTime">过期时间</param>
        /// <param name="slidingExpiration">是否启用可调过期时间</param>
        /// <param name="createPersistentCookie">是否创建持久cookie</param>
        public MaUserTicket(string username, int outTime, bool slidingExpiration, bool createPersistentCookie)
        {
            m_username = username;
            m_timeout = outTime;
            m_expiration = DateTime.Now.AddMinutes((double)outTime);
            m_slidingExpiration = slidingExpiration;
            m_createPersistentCookie = createPersistentCookie;
            m_ip = MaWebUtility.GetIP();
        }

        #endregion
        /// <summary>
        /// 获得票证代码
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _value = TextHelper.AESEncrypt(m_username) + "|" + m_ip + "|" + Expiration.ToString() + "|" + DateTime.Now.ToString() + "|" + m_slidingExpiration.ToString() + "|" + m_createPersistentCookie.ToString() + "|" + m_timeout.ToString();
            return TextHelper.AESEncrypt(_value);
        }
    }
}
