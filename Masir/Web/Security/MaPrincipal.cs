using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Security
{
    /// <summary>
    /// 用户对象的基本功能
    /// </summary>
    public abstract class MaPrincipal : IPrincipal
    {
        #region IPrincipal 成员
        /// <summary>
        /// 
        /// </summary>
        protected MaIdentity m_identity;
        /// <summary>
        /// 获取当前用户的标识，接口成员
        /// </summary>
        public IIdentity Identity
        {
            get { return m_identity; }
        }

        /// <summary>
        /// 确定当前用户是否属于指定的角色，接口成员
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public virtual bool IsInRole(string str)
        {
            string[] _list = str.Split(';');
            foreach (string item in _list)
            {
                string[] _item = item.Split(':');
                string _type=_item[0];
                //判断是否要执行操作
                bool _ibool = false;
                if (_item[0].Length==2)
                {
                    _ibool = true;
                    _type = _item[0].Substring(1,1);
                }
                //计算表达式的值
                bool _expresserValue = false;

                #region 计算表达式的值

                if (_type == "R")
                {//角色认证
                    string[] _role = _item[1].Split(',');
                    foreach (string role in _role)
                    {
                        if (IsInMaRole(role))
                        {
                            _expresserValue = true;
                            break;
                        }
                    }
                }
                else if (_type == "P")
                {//权限认证
                    string[] _permission = _item[1].Split(',');
                    foreach (string permission in _permission)
                    {
                        if (IsInMaPermission(permission))
                        {
                            _expresserValue = true;
                            break;
                        }
                    }
                }
                else if (_type == "G")
                {//部门认证
                    string[] _group = _item[1].Split(',');
                    foreach (string group in _group)
                    {
                        if (IsInMaGroup(group))
                        {
                            _expresserValue = true;
                            break;
                        }
                    }
                }

                #endregion

                if (_ibool)
                {//执行否操作
                    _expresserValue = !_expresserValue;
                }

                if (_expresserValue)
                {
                    //通过验证，返回TRUE
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 角色认证
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public abstract bool IsInMaRole(string role);
        /// <summary>
        /// 权限认证
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public abstract bool IsInMaPermission(string permission);
        /// <summary>
        /// 部门认证
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public abstract bool IsInMaGroup(string role);
        /// <summary>
        /// 服务认证
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public abstract bool IsInMaService(string service);
        /// <summary>
        /// 目录认证
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public abstract bool IsBrowseUrl(Masir.Web.Page.MaUrl url);
        #endregion
    }
}
