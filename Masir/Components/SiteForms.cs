using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Components
{
    /// <summary>
    /// 表单提交数据过滤
    /// </summary>
    public static class SiteForms
    {
        /// <summary>
        /// 过滤表单项首尾空格，并转换成半角
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static NameValueCollection Trims(NameValueCollection form)
        {
            var after = new NameValueCollection();
            foreach (string key in form.AllKeys)
            {
                after[key] = FormsVerify.ToDBC(form[key].Trim());
            }
            return after;
        }

        /// <summary>
        /// 登录表单验证
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool CheckLoginForm(NameValueCollection form)
        {
            if (!FormsVerify.CheckLengh(FormsVerify.IsUsername, form["loginname"], 6, 18)
                || !FormsVerify.CheckLengh(FormsVerify.IsPassword, form["nloginpwd"], 6, 30))//兼容老密码长度
            {
                return false;
            }
            else return true;
        }

        /// <summary>
        /// 注册表单验证
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool CheckRegistForm(NameValueCollection form)
        {
            if (!FormsVerify.CheckLengh(FormsVerify.IsUsername, form["regName"], 6, 20)
                || !FormsVerify.CheckLengh(FormsVerify.IsPassword, form["pwd"], 6, 20))
            {
                return false;
            }
            else return true;
        }


    }
}
