using Masir.Web.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Web.Parse
{
    /// <summary>
    /// 解析入口类
    /// </summary>
   public class VariableParser:IMaParser
   {
       const string TMP_TAG_REGEX = @"{$(.*)}";

       /// <summary>
       /// 解析入口
       /// </summary>
       /// <param name="urlInfo"></param>
       /// <param name="site"></param>
       /// <param name="skin"></param>
       /// <param name="page"></param>
       /// <param name="pageCode"></param>
       public void Parse(MaUrl urlInfo, IMaSite site, IMaSkin skin, MaPage page, ref StringBuilder pageCode)
       {
           //处理页面临时变量
           foreach (KeyValuePair<string, string> item in Htmx.HtmxHandler.GetThisPageTempVariable())
           {
               pageCode = pageCode.Replace("{$" + item.Key + "}", item.Value);
           }
           //处理页面变量
           foreach (KeyValuePair<string, string> item in page.GetPageVariable())
           {
               pageCode = pageCode.Replace("{$" + item.Key + "}", item.Value);
           }
           //处理皮肤变量
           foreach (KeyValuePair<string, string> item in ((MaConfig)skin).Variables)
           {
               pageCode = pageCode.Replace("{$" + item.Key + "}", item.Value);
           }
           //处理站点变量
           foreach (KeyValuePair<string, string> item in ((MaConfig)site).Variables)
           {
               pageCode = pageCode.Replace("{$" + item.Key + "}", item.Value);
           }
           //处理基本变量
           pageCode = pageCode.Replace("{$_ma_this_domain}", urlInfo.Domain.Domain);
           pageCode = pageCode.Replace("{$_ma_this_root_domain}", urlInfo.Domain.MainDomain);
       }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="node"></param>
       public void Load(System.Xml.XmlElement node)
       {
           
       }

    }
}
