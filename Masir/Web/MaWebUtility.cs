using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Web
{
    /// <summary>
    /// web相关公共处理函数
    /// </summary>
    public static class MaWebUtility
    {
        /// <summary>
        /// 移除html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            string regexstr = @"<[^>]*>";
            return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }

        #region Http 编码 解码

        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        #endregion

        #region 获得物理路径

        /// <summary>
        /// 获得文件物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMapPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }

        /// <summary>
        /// 获得文件物理路径
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isRoot">是否相对根路径</param>
        /// <returns></returns>
        public static string GetMapPath(string path, bool isRoot)
        {
            if (isRoot)
            {
                path = path.Replace("\\", "/");
                if (path.Remove(1) != "~")
                {
                    if (path.Remove(1) != "/")
                    {
                        path = "~/" + path;
                    }
                    else
                    {
                        path = "~" + path;
                    }
                }
            }
            return HttpContext.Current.Server.MapPath(path);
        }

        #endregion
    }
}
