using Masir.Web;
using Masir.Web.Page;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Components
{
    public class CommonParser : Masir.Web.Parse.MaParserBase
    {

        public CommonParser()
        {
            m_tagRegexStr = @"<!--Ma_Common{(.*?)}-->";
            /*
             <!--Ma_Common{db=MaoooBrands;tb=View_V4_OA_JSJ_CompanyInfo;top=5;order=ComIndexId desc;where=(feli=43) AND (til>2);ctype=1;tmp=/vMall/temp/test.html}-->
             * db:数据库名
             * tb:表或视图名
             * top:数据条数
             * order:排序            
             * where:条件            
             * tmp:模板路径
             * page:第几页翻页时有效
             * start:列表从第几条开始读取
             * m:通用不满足时指定处理类
             * btn:分页时页码数量（支持0,5,7,10)默认7
             * field:提取字段默认*
             * output:默认html可以为json
             */
        }

        public override void ParseTag(MaUrl urlInfo, IMaSite site, IMaSkin skin, MaPage page, ref StringBuilder pageCode, Web.Parse.MaTag pTag)
        {
            TagEntity _tag = new TagEntity()
            {
                Database = pTag["db"],
                Table = pTag["tb"],
                Count = pTag["top"],
                Order = pTag["order"],
                Where = pTag["where"],
                DataType = pTag["tmp"] == null ? DataTagEnum.Content : pTag["page"] == null ? DataTagEnum.List : DataTagEnum.Pager,
                Template = pTag["tmp"],
                Method = pTag["m"],
                Filed = pTag["field"] ?? "*",
                Output = pTag["output"] ?? "html"
            };

            DataTable _dt;
            switch (_tag.DataType)
            {
                case DataTagEnum.Content:
                    _dt = GetContentDataTable(_tag, pTag);
                    ParseContentTag(_dt, skin, _tag, ref pageCode, pTag);
                    break;
                case DataTagEnum.List:
                    _dt = GetListDataTable(_tag, pTag);
                    ParseListTag(_dt, skin, _tag, ref pageCode, pTag);
                    break;
                case DataTagEnum.Pager:
                    int _totalCount = 0;
                    _dt = GetPagerDataTable(_tag, pTag, out _totalCount);
                    ParsePagerTag(_dt, skin, _tag, ref pageCode, _totalCount, pTag);
                    break;
                default:
                    break;
            }
            pageCode = pageCode.Replace(pTag.TagStr, "");
        }

        public void ParseContentTag(DataTable dt, IMaSkin skin, TagEntity tag, ref StringBuilder pageCode, Web.Parse.MaTag pTag)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(tag.Template))
                {
                    string _tempCode = skin.GetTemplate(tag.Template);

                    string _code = ParseTools.ParseDataTable(dt, _tempCode);

                    pageCode = pageCode.Replace(pTag.TagStr, _code);
                }
                else
                {
                    if (tag.Output == "html")
                    {
                        string _code = ParseTools.ParseDataTable(dt, pageCode.ToString());
                        _code = _code.Replace(pTag.TagStr, string.Empty);
                        pageCode.Clear().Append(_code);
                    }
                    if (tag.Output == "json")
                    {
                        string _code = JSConvert.Dtb2Json(dt);
                        pageCode = pageCode.Replace(pTag.TagStr, _code);
                    }
                }
            }
            else
            {
                //无数据时处理值标签
                var _matches = ParseTools.m_valueTagRegex.Matches(pageCode.ToString());
                foreach (Match item in _matches)
                {
                    string _valueTagStr = item.Groups[0].Value;
                    string _formatStr = item.Groups[2].Value;

                    if (_formatStr == "")
                    {
                        //pageCode = pageCode.Replace(_valueTagStr, "");
                    }
                    else
                    {
                        pageCode = pageCode.Replace(_valueTagStr, ParseTools.FormatTag(item.Groups[3].Value, "", null));
                    }
                }
            }
        }

        public DataTable GetContentDataTable(TagEntity tag, Web.Parse.MaTag pTag)
        {
            StringBuilder _sql = new StringBuilder(string.Format(" SELECT TOP {0} " + tag.Filed + " FROM {1} ", tag.Count, tag.Table));
            if (!string.IsNullOrEmpty(tag.Where))
            {
                _sql.Append(" WHERE " + tag.Where);
            }
            if (!string.IsNullOrEmpty(tag.Order))
            {
                _sql.Append(" ORDER BY " + tag.Order);
            }

            DataTable _dt;
            using (MaDataHelper helper = MaDataHelper.GetDataHelper(tag.Database))
            {
                _dt = helper.SqlGetDataTable(_sql.ToString());
            }
            return _dt;
        }

        public void ParseListTag(DataTable dt, IMaSkin skin, TagEntity tag, ref StringBuilder pageCode, Web.Parse.MaTag pTag)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                string _tempCode = skin.GetTemplate(tag.Template);

                string _code = tag.Output == "html" ? ParseTools.ParseDataTable(dt, _tempCode) : JSConvert.Dtb2Json(dt);

                pageCode = pageCode.Replace(pTag.TagStr, _code);
            }
        }

        public DataTable GetListDataTable(TagEntity tag, Web.Parse.MaTag pTag)
        {
            if (tag.Table.ToLower().IndexOf("sp_") > -1)
            {
                DataTable _dt2;
                using (MaDataHelper helper = MaDataHelper.GetDataHelper(tag.Database))
                {
                    _dt2 = helper.SqlGetDataTable("exec " + tag.Table);
                }
                return _dt2;
            }

            if (!string.IsNullOrEmpty(pTag["m"]))
            {
                var _dataHelper = new TagDataHelper(tag, pTag);
                return _dataHelper.GetDataTable(pTag["m"]);
            }
            if (!string.IsNullOrEmpty(tag.Where))
            {
                tag.Where = " WHERE " + tag.Where;
            }
            if (!string.IsNullOrEmpty(tag.Order))
            {
                tag.Order = " ORDER BY " + tag.Order;
            }

            StringBuilder _sql = new StringBuilder();
            if (string.IsNullOrEmpty(pTag["start"]))
            {
                _sql.Append(string.Format(" SELECT TOP {0} * FROM {1} {2} {3}", tag.Count, tag.Table, tag.Where, tag.Order));
            }
            else
            {
                if (string.IsNullOrEmpty(tag.Order))
                {
                    throw new Exception("必须提供order参数");
                }
                _sql.Append(string.Format("SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER ({1}) TID, " + tag.Filed + " FROM {2} {3}) [Table] WHERE TID>={4}",
                tag.Count,
                tag.Order,
                tag.Table,
                tag.Where,
                pTag["start"]));
            }

            DataTable _dt;
            using (MaDataHelper helper =MaDataHelper.GetDataHelper(tag.Database))
            {
                _dt = helper.SqlGetDataTable(_sql.ToString());
            }
            return _dt;
        }

        public void ParsePagerTag(DataTable dt, IMaSkin skin, TagEntity tag, ref StringBuilder pageCode, int totalCount, Web.Parse.MaTag pTag)
        {
            if (string.IsNullOrEmpty(tag.Order))
            {
                throw new Exception("必须提供order参数");
            }

            int _currentPage = Convert.ToInt32(pTag["page"]);
            string _btnCount = pTag["btn"] ?? "7";

            if (dt != null && dt.Rows.Count > 0)
            {
                string _tempCode = skin.GetTemplate(tag.Template);
                string _code = tag.Output == "html" ? ParseTools.ParseDataTable(dt, _tempCode) : JSConvert.Dtb2Json(dt);
                pageCode = pageCode.Replace(pTag.TagStr, _code);

                PagerNavigation _pagerhelper = new PagerNavigation();
                _pagerhelper.ButtonCount = int.Parse(_btnCount);

                string _barCode = _pagerhelper.GetPageBarCode(totalCount,
                    int.Parse(tag.Count),
                    _currentPage,
                    HttpContext.Current.Request.Url.AbsolutePath,
                    HttpContext.Current.Request.QueryString.ToString());
                pageCode = pageCode.Replace("{$Pagerbar}", _barCode);
            }
            pageCode = pageCode.Replace("{$Pagerbar}", "");
        }

        public DataTable GetPagerDataTable(TagEntity tag, Web.Parse.MaTag pTag, out int totalCount)
        {
            if (!string.IsNullOrEmpty(pTag["m"]))
            {
                var _dataHelper = new TagDataHelper(tag, pTag);
                DataTable dt = _dataHelper.GetDataTable(pTag["m"]);
                totalCount = _dataHelper.TotalCount;
                return dt;
            }

            int _currentPage = Convert.ToInt32(pTag["page"]);
            string _where = string.IsNullOrEmpty(tag.Where) ? "" : " WHERE " + tag.Where;
            string _order = string.IsNullOrEmpty(tag.Order) ? "" : " ORDER BY " + tag.Order;
            var _dt = Data.DataEntityHelper.GetTable(
                tag.Database,
                tag.Table,
                _where,
                new Hashtable(),
                tag.Filed,
                _order,
                (_currentPage - 1) * int.Parse(tag.Count),
                int.Parse(tag.Count),
                out totalCount);
            return _dt;
        }
    }

    public class TagEntity
    {
        /// <summary>
        /// 通用不满足时可以指定独立方法命
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 数据库节点名
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 表、视图、函数名称
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 提取字段
        /// </summary>
        public string Filed { get; set; }

        /// <summary>
        /// 条数
        /// </summary>
        public string Count { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        public DataTagEnum DataType { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 输出方式
        /// </summary>
        public string Output { get; set; }
    }

    public enum DataTagEnum
    {
        Content = 1,
        List,
        Pager
    }
}
