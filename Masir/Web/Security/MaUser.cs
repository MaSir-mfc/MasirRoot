using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Security
{
    /// <summary>
    /// 授权用户信息
    /// </summary>
    public class MaUser : MaPrincipal
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username"></param>
        public MaUser(string username)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public override bool IsInMaRole(string role)
        {
            return false;
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public override bool IsInMaPermission(string permission)
        {
            //throw new NotImplementedException();
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public override bool IsInMaService(string service)
        {
            //throw new NotImplementedException();
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public override bool IsBrowseUrl(MaUrl url)
        {
            //throw new NotImplementedException();
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public override bool IsInMaGroup(string group)
        {
            //throw new NotImplementedException();
            return false;
        }

    }
}
