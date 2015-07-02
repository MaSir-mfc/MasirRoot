using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Data
{
    /// <summary>
    /// 数据行类型的实体类，字段属性
    /// </summary>
    public class DataRowEntityFileAttribute : Attribute
    {
        /// <summary>
        /// 用于对数据类型的实体类字段，进行数据行字段属性标注
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="type">字段类型</param>
        public DataRowEntityFileAttribute(string name, Type type)
        {
            this.m_fieldName = name;
            this.m_fieldTitle = name;
            this.m_toStringFormate = string.Empty;
            this.m_fieldType = type;
        }
        /// <summary>
        /// 用于对数据类型的实体类字段，进行数据行字段属性标注
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="title">字段标注</param>
        /// <param name="type">字段类型</param>
        public DataRowEntityFileAttribute(string name, string title, Type type)
        {
            this.m_fieldName = name;
            this.m_fieldTitle = title;
            this.m_toStringFormate = string.Empty;
            this.m_fieldType = type;
        }

        private string m_fieldName;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName
        {
            get { return this.m_fieldName; }
            set { this.m_fieldName = value; }
        }

        private string m_fieldTitle;
        /// <summary>
        /// 字段说明
        /// </summary>
        public string FieldTitle
        {
            get { return m_fieldTitle; }
            set { m_fieldTitle = value; }
        }

        private string m_toStringFormate;
        /// <summary>
        /// 字段格式化模板
        /// </summary>
        public string ToStringFormate
        {
            get { return m_toStringFormate; }
            set { m_toStringFormate = value; }
        }


        private Type m_fieldType;
        /// <summary>
        /// 字段类型
        /// </summary>
        public Type FieldType
        {
            get { return this.m_fieldType; }
            set { this.m_fieldType = value; }
        }
    }
}
