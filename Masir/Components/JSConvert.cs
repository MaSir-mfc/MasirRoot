using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Masir.Components
{
    /// <summary>
    /// datatable转json
    /// </summary>
    public class JSConvert
    {
        /// <summary>
        /// DataTable转成Json 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="jsonName"></param>
        /// <param name="encodeFields"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable dt, string jsonName, params string[] encodeFields)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        var value = dt.Rows[i][j].ToString();
                        if (encodeFields.Contains(dt.Columns[j].ColumnName))
                        {
                            value = HttpUtility.UrlEncode(value).Replace("+", "%20");
                        }
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + value + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        ///// <summary>   
        ///// 过滤特殊字符   
        ///// </summary>   
        ///// <param name="s"></param>   
        ///// <returns></returns>   
        //private static string String2Json(String s)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        char c = s.ToCharArray()[i];
        //        switch (c)
        //        {
        //            case '\"':
        //                sb.Append("\\\""); break;
        //            case '\\':
        //                sb.Append("\\\\"); break;
        //            case '/':
        //                sb.Append("\\/"); break;
        //            case '\b':
        //                sb.Append("\\b"); break;
        //            case '\f':
        //                sb.Append("\\f"); break;
        //            case '\n':
        //                sb.Append("\\n"); break;
        //            case '\r':
        //                sb.Append("\\r"); break;
        //            case '\t':
        //                sb.Append("\\t"); break;
        //            default:
        //                sb.Append(c); break;
        //        }
        //    }
        //    return sb.ToString();
        //}


        ///// <summary>
        ///// 数据表转List
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public static List<T> ConvertToList<T>(DataTable dt)
        //{
        //    List<T> ts = new List<T>();
        //    Type type = typeof(T);

        //    string tempName = string.Empty;

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        T t = Activator.CreateInstance<T>();

        //        PropertyInfo[] propertys = t.GetType().GetProperties();

        //        foreach (PropertyInfo pi in propertys)
        //        {
        //            tempName = pi.Name;//将属性名称赋值给临时变量  
        //            //检查DataTable是否包含此列（列名==对象的属性名）    
        //            if (dt.Columns.Contains(tempName))
        //            {
        //                // 判断此属性是否有Setter  
        //                if (!pi.CanWrite) continue;//该属性不可写，直接跳出  
        //                //取值  
        //                object value = dr[tempName];
        //                //如果非空，则赋给对象的属性  
        //                if (value != DBNull.Value)
        //                    pi.SetValue(t, value, null);
        //            }
        //        }
        //        //对象添加到泛型集合中  
        //        ts.Add(t);
        //    }
        //    return ts;
        //}  
        /// <summary>
        /// 标准的数据转json
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string Dtb2Json(DataTable dt)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ArrayList dic = new ArrayList();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> drow = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    drow.Add(col.ColumnName, row[col.ColumnName]);
                }
                dic.Add(drow);
            }
            return jss.Serialize(dic);
        }
    }
}
