using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Masir.Web.Page
{
    /// <summary>
    /// 皮肤处理类
    /// </summary>
    public class MaSkin : MaConfig, IMaSkin
    {
        #region 皮肤基本属性

        string m_skinName = "Defualt";
        string m_imagePath = "/Skin/Default/Images/";
        string m_cssPath = "/Skin/Default/Css/";
        string m_jsPath = "/Skin/Default/Js/";
        string m_templatePath = "/Skin/Default/";
        Dictionary<string, string> m_templateList = new Dictionary<string, string>();
        /// <summary>
        /// 皮肤名称
        /// </summary>
        public string SkinName
        {
            get
            {

                return m_skinName;
            }
            set
            {
                m_skinName = value;
            }
        }

        /// <summary>
        /// 皮肤图片路径
        /// </summary>
        public string ImagePath
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ImagesPath"]))
                {
                    return ConfigurationManager.AppSettings["ImagesPath"];
                }
                return m_imagePath;
            }
            set
            {
                m_imagePath = value;
            }
        }

        /// <summary>
        /// 皮肤样式路径
        /// </summary>
        public string CssPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["StylePath"]))
                {
                    return ConfigurationManager.AppSettings["StylePath"];
                }
                return m_cssPath;
            }
            set
            {
                m_cssPath = value;
            }
        }

        /// <summary>
        /// 皮肤脚本路径
        /// </summary>
        public string JsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["JsPath"]))
                {
                    return ConfigurationManager.AppSettings["JsPath"];
                }
                return m_jsPath;
            }
            set
            {
                m_jsPath = value;
            }
        }

        /// <summary>
        /// 皮肤模板路径
        /// </summary>
        public string TemplatePath
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TemplatePath"]))
                {
                    return ConfigurationManager.AppSettings["TemplatePath"];
                }
                return m_templatePath;
            }
            set
            {
                m_templatePath = value;
            }
        }

        #endregion

        #region 加载配置信息

        /// <summary>
        /// 加载皮肤配置
        /// </summary>
        /// <param name="node"></param>
        public override void Load(System.Xml.XmlElement node)
        {
            base.Load(node);
            foreach (XmlNode item in node)
            {
                if (item is XmlElement)
                {
                    if (item.Name == "Name")
                    {
                        m_skinName = item.InnerText;
                    }
                    else if (item.Name == "ImagePath")
                    {
                        m_imagePath = item.InnerText;
                    }
                    else if (item.Name == "CssPath")
                    {
                        m_cssPath = item.InnerText;
                    }
                    else if (item.Name == "JsPath")
                    {
                        m_jsPath = item.InnerText;
                    }
                    else if (item.Name == "TemplatePath")
                    {
                        m_templatePath = item.InnerText;
                    }
                    else if (item.Name == "TemplateList")
                    {
                        foreach (XmlNode nodes in item.ChildNodes)
                        {
                            m_templateList.Add(nodes.Attributes["key"].Value, nodes.Attributes["value"].Value);
                        }
                    }
                }
            }
        }
        #endregion

        #region 获取模板保存路径

        /// <summary>
        /// 获取模板保存路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetTemplateSavePath(string name)
        {
            string _templatePath = name;
            if (!m_templateList.TryGetValue(name, out _templatePath))
            {
                _templatePath = name;
            }
            //网络路径
            if (_templatePath.IndexOf("\\\\") == 0)
            {
                if (System.IO.File.Exists(_templatePath))
                {
                    return _templatePath;
                }
            }
            //路径加载
            if (_templatePath.IndexOf(":") < 0)
            {
                //获得皮肤路径
                string _skinPath = MaWebUtility.GetMapPath(TemplatePath + _templatePath, true);
                //获得站点路径
                string _sitePath = MaWebUtility.GetMapPath(_templatePath, true);

                _templatePath = _sitePath;
                if (System.IO.File.Exists(_skinPath))
                {//优先皮肤路径
                    _templatePath = _skinPath;
                }
            }
            return _templatePath;
        }

        #endregion

        #region 获取模板代码

        /// <summary>
        /// 获取模板代码，如果没有找到，将抛出异常
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public string GetTemplate(string templateName)
        {
            string _templatePath = GetTemplateSavePath(templateName);
            if (!System.IO.File.Exists(_templatePath))
            {
                throw new Exception("所有区域都没找到模板【" + templateName + "】【" + HttpContext.Current.Request.Url.ToString() + "】");
            }
            return System.IO.File.ReadAllText(_templatePath);
        }

        /// <summary>
        /// 尝试获取模板代码
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool TryGetTemplate(string templateName, out string code)
        {
            string _templatePath = GetTemplateSavePath(templateName);
            if (!System.IO.File.Exists(_templatePath))
            {
                code = string.Empty;
                return false;
            }
            else
            {
                code = System.IO.File.ReadAllText(_templatePath);
                return true;
            }
        }
        #endregion

        #region IMaSkin成员
        /// <summary>
        /// 公共模板地址集合
        /// </summary>
        public Dictionary<string, string> TemplateList
        {
            get
            {
                return this.m_templateList;
            }
            set
            {
                this.m_templateList = value;
            }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlStr"></param>
        public void PullOnSkin(ref string htmlStr)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsThis(HttpContext context)
        {
            return true;
        }
    }
}
