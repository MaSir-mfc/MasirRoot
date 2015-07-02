using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Components
{
    public class PagerNavigation
    {
        private int buttonCount = 10;
        /// <summary>
        /// 翻页按钮数量,默认10(暂支持0;5;7;10)
        /// </summary>
        public int ButtonCount
        {
            get
            {
                return buttonCount;
            }

            set
            {
                buttonCount = value;
            }
        }

        private bool isShowTotal = true;
        /// <summary>
        /// 是否显示总页数当前页信息,默认显示
        /// </summary>
        public bool IsShowTotal
        {
            get
            {
                return isShowTotal;
            }

            set
            {
                isShowTotal = value;
            }
        }

        private bool isShowTopLastStr = true;
        /// <summary>
        /// 是否显示首页末页链接,默认显示
        /// </summary>
        public bool IsShowTopLastStr
        {
            get
            {
                return isShowTopLastStr;
            }

            set
            {
                isShowTopLastStr = value;
            }
        }

        private bool isUseImg = false;
        /// <summary>
        /// 是否用图片代替上一页和下一页配合脚本,默认不采用
        /// </summary>
        public bool IsUseImg
        {
            get
            {
                return isUseImg;
            }

            set
            {
                isUseImg = value;
            }
        }

        private bool withPageIndex = true;
        /// <summary>
        /// Url中是否带当前页码
        /// </summary>
        public bool WithPageIndex
        {
            get
            {
                return withPageIndex;
            }

            set
            {
                withPageIndex = value;
            }
        }
        /// <summary>
        /// 自定义链接
        /// </summary>
        /// <returns></returns>
        public delegate string CustomLinkHandler();
        /// <summary>
        /// 当已经没有上一页时可自定义上一页链接
        /// </summary>
        public event CustomLinkHandler PreLink;
        /// <summary>
        /// 当已经没有下一页时可自定义下一页链接
        /// </summary>
        public event CustomLinkHandler NextLink;
        //可添加自定义事件

        /// <summary>
        /// 分页导航函数
        /// </summary>
        /// <param name="totalRecord">总记录条数</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="currentIndex">当前页索引</param>
        /// <param name="absolutePath">请求绝对路径</param>
        /// <param name="urlParam">请求参数集合</param>
        public string GetPageBarCode(int totalRecord, int pageSize, int currentIndex, string absolutePath, string urlParam)
        {
            string pageUrl = absolutePath;

            string query = string.IsNullOrEmpty(urlParam) ? "" : "?" + urlParam;//string.Empty;

            //if (urlParam != null && urlParam.Count > 0)
            //{
            //    for (int i = 0; i < urlParam.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(urlParam[i]))
            //        {
            //            if (i == 0)
            //                query += "?" + urlParam.Keys[i] + "=" + escape(urlParam[i]);
            //            else
            //                query += "&" + urlParam.Keys[i] + "=" + escape(urlParam[i]);
            //        }
            //    }
            //}
            if (WithPageIndex)
            {
                int t = pageUrl.LastIndexOf('.') - pageUrl.LastIndexOf('_');

                pageUrl = pageUrl.Remove(pageUrl.LastIndexOf('_'), t);
            }

            string pageBarStr = string.Empty;

            int pageCount = 0;

            if (pageSize != 0)
            {
                pageCount = (totalRecord / pageSize);

                pageCount = ((totalRecord % pageSize) != 0 ? pageCount + 1 : pageCount);

                pageCount = (pageCount == 0 ? 1 : pageCount);
            }
            if (currentIndex < 1)
            {
                currentIndex = 1;
            }

            int nextIndex = currentIndex + 1;

            int preIndex = currentIndex - 1;

            if (currentIndex > 1)
            {
                pageBarStr += IsShowTopLastStr ? "<li id='topPage'><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_1") + query + "\">" + (IsUseImg ? "" : "首页") + "</a></li>\r<li id='prePage'><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_" + preIndex) + query + "\">" + (IsUseImg ? "" : "上一页") + "</a></li>\r" : "<li id='prePage'><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_" + preIndex) + query + "\">" + (IsUseImg ? "" : "上一页") + "</a></li>\r";
            }
            else
            {
                if (PreLink != null)
                {
                    pageBarStr += PreLink();
                }
                else
                {
                    pageBarStr += IsShowTopLastStr ? "<li id='topPage'>" + (IsUseImg ? "" : "首页") + "</li>\r<li id='prePage'>" + (IsUseImg ? "" : "上一页") + "</li>\r" : "<li id='prePage'>" + (IsUseImg ? "" : "上一页") + "</li>\r";
                }
            }

            //当前页码居中
            if (buttonCount != 0)
            {
                int startCount = 0;

                int endCount = 0;

                switch (buttonCount)
                {
                    case 10:

                        startCount = (currentIndex + 5) > pageCount ? pageCount - 9 : currentIndex - 4;

                        endCount = currentIndex < 5 ? 10 : currentIndex + 5;

                        break;

                    case 7:
                        startCount = (currentIndex + 3) > pageCount ? pageCount - 7 : currentIndex - 3;

                        endCount = currentIndex < 3 ? 7 : currentIndex + 3;

                        break;

                    case 5:

                        startCount = (currentIndex + 2) > pageCount ? pageCount - 5 : currentIndex - 2;

                        endCount = currentIndex < 2 ? 5 : currentIndex + 2;

                        break;

                    default:

                        throw new Exception("当前页码居中,页码数暂支持0;5;7;10");
                }

                if (startCount < 1)
                {
                    startCount = 1;
                }

                if (pageCount < endCount)
                {
                    endCount = pageCount;
                }

                for (int i = startCount; i <= endCount; i++)
                {
                    pageBarStr += currentIndex == i ? "<li id='currentPage'>" + i + "</li>\r" : "<li><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_" + i) + query + "\">" + i + "</a></li>\r";
                }
            }

            if (currentIndex != pageCount)
            {
                pageBarStr += IsShowTopLastStr ? "<li id='nextPage'><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_" + nextIndex) + query + "\">" + (IsUseImg ? "" : "下一页") + "</a></li>\r<li id='lastPage'><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_" + pageCount) + query + "\">" + (IsUseImg ? "" : "尾页") + "</a></li>\r" : "<li id='nextPage'><a target=\"_self\" href=\"" + pageUrl.Insert(pageUrl.IndexOf("."), "_" + nextIndex) + query + "\">" + (IsUseImg ? "" : "下一页") + "</a></li>\r";
            }
            else
            {
                if (NextLink != null)
                {
                    pageBarStr += NextLink();
                }
                else
                {
                    pageBarStr += IsShowTopLastStr ? "<li id='nextPage'>" + (IsUseImg ? "" : "下一页") + "</li>\r<li id='lastPage'>" + (IsUseImg ? "" : "尾页") + "</li>\r" : "<li id='nextPage'>" + (IsUseImg ? "" : "下一页") + "</li>\r";
                }
            }
            if (isShowTotal)
            {
                pageBarStr += "<li id='totalInfo'>" + currentIndex + "/" + pageCount + "页</li>\r";
            }
            return pageBarStr;
        }

        #region 客户端escape中文字符加密

        public string escape(string s)
        {
            if (IsChinese(s))
            {
                StringBuilder sb = new StringBuilder();

                byte[] ba = System.Text.Encoding.Unicode.GetBytes(s);

                for (int i = 0; i < ba.Length; i += 2)
                {    /**///// BE SURE 2's 
                    sb.Append("%u");

                    sb.Append(ba[i + 1].ToString("X2"));

                    sb.Append(ba[i].ToString("X2"));
                }
                return sb.ToString();
            }
            else
            {
                return s;
            }
        }

        public bool IsChinese(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) > Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = true;
                }

            }
            return BoolValue;
        }

        #endregion

    }
}
