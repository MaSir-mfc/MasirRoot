using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Masir.Components
{
    /// <summary>
    /// 数据正则验证
    /// </summary>
    public sealed class FormsVerify
    {

        /// <summary>
        /// 检查数据长度
        /// </summary>
        /// <param name="func"></param>
        /// <param name="obj"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool CheckLengh(Func<object, bool> func, object obj, int min, int max)
        {
            if (func(obj))
            {
                int length = StrLength(obj.ToString());
                return length > max || length < min ? false : true;
            }
            else return false;
        }

        /// <summary>
        /// 检查数据是否匹配
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        public static bool Check(object obj, string reg)
        {
            return Regex.IsMatch(obj.ToString(),
                reg, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// 字符串长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int StrLength(string str)
        {
            int len = 0;
            byte[] b;

            for (int i = 0; i < str.Length; i++)
            {
                b = Encoding.Default.GetBytes(str.Substring(i, 1));
                if (b.Length > 1)
                    len += 2;
                else
                    len++;
            }
            return len;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsUsername(object obj)
        {
            if (obj.ToString().IndexOf('@') > -1)
                return IsEmail(obj);
            return Check(obj, @"^[A-Za-z0-9_\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 密码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPassword(object obj)
        {
            return Check(obj, @"^.*[A-Za-z0-9\w_-]+.*$");
        }

        /// <summary>
        /// 是否整数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsIntege(object obj)
        {
            return Check(obj, @"^-?[1-9]\d*$");
        }

        /// <summary>
        /// 是否正整数 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsIntege1(object obj)
        {
            return Check(obj, @"^[1-9]\d*$");
        }

        /// <summary>
        ///是否负整数
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsIntege2(object obj)
        {
            return Check(obj, @"^-[1-9]\d*$");
        }

        /// <summary>
        /// 是否数字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNum(object obj)
        {
            return Check(obj, @"^([+-]?)\d*\.?\d+$");
        }

        /// <summary>
        /// 是否ascii
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAscii(object obj)
        {
            return Check(obj, "^[\x00-\xFF]+$");
        }

        /// <summary>
        /// 是否中文
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsChinese(object obj)
        {
            return Check(obj, @"^[\u4e00-\u9fa5]+$");
        }

        // <summary>
        // 是否日期
        // </summary>
        // <param name="obj"></param>
        // <returns></returns>
        //public static bool IsDate(object obj)
        //{
        //    return Check(obj, @"^\\d{4}(\\-|\\/|\.)\\d{1,2}\\1\\d{1,2}$");
        //}

        /// <summary>
        /// 是否为日期格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDate(object obj)
        {
            try
            {
                DateTime time = Convert.ToDateTime(obj);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否为字母 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsLetter(object obj)
        {
            return Check(obj, "^[A-Za-z]+$");
        }

        /// <summary>
        /// 是否为小写字母 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsLetter1(object obj)
        {
            return Check(obj, "^[a-z]+$");
        }

        /// <summary>
        /// 是否为大写字母 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsLetter2(object obj)
        {
            return Check(obj, "^[A-Z]+$");
        }

        /// <summary>
        /// 是否为邮件格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEmail(object obj)
        {
            return Check(obj, @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
        }

        /// <summary>
        /// 是否为网址
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsUrl(object obj)
        {
            // "^http[s]?:\\/\\/([\\w-]+\\.)+[\\w-]+([\\w-./?%&=]*)?$"
            return Check(obj, @"^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&%_\./-~-]*)?$");
        }

        /// <summary>
        /// 是否为座机
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPhone(object obj)
        {
            //"^[0-9\-()（）]{7,18}$", //电话号码的函数(包括验证国内区号,国际区号,分机号)
            return Check(obj, @"^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$");
        }

        /// <summary>
        /// 是否为手机号
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsMobile(object obj)
        {
            return Check(obj, @"^0?(13|15|18|14)[0-9]{9}$");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsIP(object obj)
        {
            return Check(obj, @"^(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5]).(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5]).(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5]).(0|[1-9]\d?|[0-1]\d{2}|2[0-4]\d|25[0-5])$");
        }


        /// <summary>
        /// 转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32; continue;
                }

                if (c[i] > 65280 && c[i] < 65375)

                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }


    }
}
