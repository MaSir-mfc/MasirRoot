using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web.Security
{
    /// <summary>
    /// 安全验证
    /// </summary>
    public class MaSecurityModule : IHttpModule
    {
        #region IHttpModule接口
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        public void Init(HttpApplication application)
        {
            //application.AuthenticateRequest+=

        }


        void application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            MaUrl _url = MaUrl.Current;
            if (_url.AbsolutePath == "/")
            {
                return;
            }

            //获得认证用户信息
            OnAuthenticate(context);

            if ((context.User == null) && (MaSecurityConfig.Instance.LoginUrl != _url.AbsolutePath))
            {
                //开放域名跳过
                foreach (var item in MaSecurityConfig.Instance.OpenDoamin)
                {
                    if (_url.Domain.Domain == item)
                    {
                        goto URLCHECK;
                    }
                }
                //开放跳过
                foreach (var item in MaSecurityConfig.Instance.OpenPath)
                {
                    if (_url.AbsolutePath.IndexOf(item) == 0)
                    {
                        goto URLCHECK;
                    }
                }
                //授权目录，跳转登陆页面
                foreach (var item in MaSecurityConfig.Instance.AuthorizationPath)
                {
                    if (_url.AbsolutePath.IndexOf(item) == 0)
                    {
                        #region 授权页面，未经授权的处理方式

                        switch (MaSecurityConfig.Instance.StopType)
                        {
                            case StopBrowseType.Redirect:
                                {
                                    string _loginPage = MaSecurityConfig.Instance.LoginUrl + "?ReturnUrl=" + MaWebUtility.UrlEncode(_url.ToString());
                                    context.Response.Redirect(_loginPage, true);
                                }
                                break;
                            case StopBrowseType.Stop:
                                {
                                    context.Response.Clear();
                                    context.Response.End();
                                }
                                break;
                            case StopBrowseType.Exception:
                                {
                                    throw new Exception("未经授权，禁止访问：" + MaSecurityConfig.Instance.StopInfo);
                                }
                            case StopBrowseType.Info:
                                {
                                    context.Response.Clear();
                                    context.Response.Write(MaSecurityConfig.Instance.StopInfo);
                                    context.Response.End();
                                }
                                break;
                            default:
                                {
                                    string _loginPage = MaSecurityConfig.Instance.LoginUrl + "?ReturnUrl=" + MaWebUtility.UrlEncode(_url.ToString());
                                    context.Response.Redirect(_loginPage, true);
                                }
                                break;
                        }

                        #endregion
                    }
                }
            }

        URLCHECK:

            //进行URL检测
            MaPrincipal _principal = context.User as MaPrincipal;
            if (_principal != null)
            {
                if (!_principal.IsBrowseUrl(_url))
                {
                    throw new Exception("你没有浏览该页面的权限，请与管理员联系");
                }
            }
        }
        #endregion

        #region 认证用户

        private void OnAuthenticate(HttpContext context)
        {
            if (context.User == null)
            {//用户是否认证
                MaUserTicket _ticket = MaSecurityHelper.GetTicketFormCookie();
                if (_ticket != null)
                {
                    if (!_ticket.Expired)
                    {
                        #region 票证有效进行延期
                        if (!string.IsNullOrEmpty(_ticket.Username))
                        {
                            bool _isDefer = false;
                            if (MaSecurityConfig.Instance.ValidateIP)
                            {//需要验证IP
                                if (_ticket.Ip != MaWebUtility.GetIP())
                                {//ip验证不通过，退出登录
                                    MaSecurityHelper.SignOut();
                                    return;
                                }
                                else
                                {
                                    _isDefer = true;
                                }
                            }
                            //设置用户
                            Type type = System.Web.Compilation.BuildManager.GetType(MaSecurityConfig.Instance.MaPrincipalType, false, false);
                            context.User = Activator.CreateInstance(type, new object[] { _ticket.Username }) as MaPrincipal;
                            //更新过期时间
                            if (_isDefer && _ticket.SlidingExpiration)
                            {
                                //更新cookie
                                HttpContext.Current.Response.Cookies.Add(MaSecurityHelper.GetAuthCookie(_ticket));
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        //登录超时
                    }
                }
            }
            else
            {
                //已有用户信息
            }
        }


        #endregion
    }
}
