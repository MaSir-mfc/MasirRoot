#region Header
/**
 * IJsonWrapper.cs
 *   Interface that represents a type capable of handling all kinds of JSON
 *   data. This is mainly used when mapping objects through JsonMapper, and
 *   it's implemented by JsonData.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System.Collections;
using System.Collections.Specialized;


namespace LitJson
{
    /// <summary>
    /// 
    /// </summary>
    public enum JsonType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Object,
        /// <summary>
        /// 
        /// </summary>
        Array,
        /// <summary>
        /// 
        /// </summary>
        String,
        /// <summary>
        /// 
        /// </summary>
        Int,
        /// <summary>
        /// 
        /// </summary>
        Long,
        /// <summary>
        /// 
        /// </summary>
        Double,
        /// <summary>
        /// 
        /// </summary>
        Boolean
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IJsonWrapper : IList, IOrderedDictionary
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsArray   { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsBoolean { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsDouble  { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsInt     { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsLong    { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsObject  { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsString  { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool     GetBoolean ();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        double   GetDouble ();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int      GetInt ();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        JsonType GetJsonType ();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        long     GetLong ();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string   GetString ();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void SetBoolean  (bool val);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void SetDouble   (double val);
        void SetInt      (int val);
        void SetJsonType (JsonType type);
        void SetLong     (long val);
        void SetString   (string val);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string ToJson ();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void   ToJson (JsonWriter writer);
    }
}
