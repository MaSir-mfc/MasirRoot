using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Masir.Components
{
    /// <summary>
    /// 文本编码转换
    /// </summary>
    public static class TextHelper
    {
        #region string 字符串编码函数

        const string maKey = @")O[N-]6*^NMK|I,YF}+efc{}JK#JH8>Z'e9M";
        const string maIV = @"L+\~fU)F($#:KL*C4,I+)bkf";

        #region MD5加密

        /// <summary>
        /// MD5加密函数
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5(string str)
        {
            byte[] bt = System.Text.UTF8Encoding.UTF8.GetBytes(str);//UTF8需要对Text的引用
            System.Security.Cryptography.MD5CryptoServiceProvider objMD5;
            objMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] output = objMD5.ComputeHash(bt);
            return BitConverter.ToString(output).Replace("-", "");
        }

        #endregion

        #region 16进制转化

        /// <summary>
        /// 字符串转base16
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToBase16(string str)
        {
            StringBuilder ret = new StringBuilder();
            foreach (byte b in Encoding.Default.GetBytes(str))
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        /// <summary>
        /// base16转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base16ToString(string str)
        {
            try
            {
                byte[] inputByteArray = new byte[str.Length / 2];
                for (int x = 0; x < str.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(str.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                return Encoding.Default.GetString(inputByteArray);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Base64编码解码

        /// <summary>
        /// Base64编码（UTF-8编码）
        /// </summary>
        /// <param name="data">需要编码的数据</param>
        /// <returns></returns>
        public static string Base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodeData = Convert.ToBase64String(encData_byte);
                return encodeData;
            }
            catch (Exception e)
            {

                throw new Exception("Error in base64Encode" + e.Message);
            }
        }

        /// <summary>
        /// Base64解码（UTF-8编码）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new string(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
        #endregion

        #region AES加密解密方法

        /// <summary>
        /// 使用默认密钥加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <returns></returns>
        public static string AESEncrypt(string plainStr)
        {
            return AESEncrypt(plainStr, maKey, maIV);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>密文</returns>
        public static string AESEncrypt(string plainStr, string key, string iv)
        {
            if (string.IsNullOrEmpty(plainStr))
            {
                return string.Empty;
            }
            MaAES maCaes = new MaAES(key, iv);
            string encrypt = maCaes.AESEncrypt(plainStr);
            if (string.IsNullOrEmpty(encrypt))
            {
                return string.Empty;
            }
            else
            {
                return StringToBase16(encrypt);
            }
        }

        /// <summary>
        /// 使用默认密钥解密
        /// </summary>
        /// <param name="decryptStr">密文字符串</param>
        /// <returns>明文</returns>
        public static string AESDecrypt(string decryptStr)
        {
            return AESDecrypt(decryptStr, maKey, maIV);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>明文</returns>
        public static string AESDecrypt(string decryptStr, string key, string iv)
        {
            decryptStr = Base16ToString(decryptStr);
            if (string.IsNullOrEmpty(decryptStr))
            {
                return string.Empty;
            }
            MaAES maCaes = new MaAES(key, iv);
            string decrypt = maCaes.AESDecrypt(decryptStr);
            if (string.IsNullOrEmpty(decrypt))
            {
                return string.Empty;
            }
            else
            {
                return decrypt;
            }
        }

        #endregion

        #region HMACSHA1 签名

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HMACSHA1(string key, string data)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = Encoding.ASCII.GetBytes(key);

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// HMACSHA1 签名
        /// </summary>
        /// <param name="key">签名密钥</param>
        /// <param name="requestMethod">请求模式（Get\Post）</param>
        /// <param name="requestUrl">请求URL</param>
        /// <param name="parameters">请求参数</param>
        /// <returns></returns>
        public static string HMACSHA1Signature(string key, string requestMethod, string requestUrl, Parameters parameters)
        {

            StringBuilder data = new StringBuilder(100);
            data.AppendFormat("{0}&{1}&", requestMethod.ToUpper(), Uri.EscapeDataString(requestUrl));
            //处理参数
            if (parameters != null)
            {
                data.Append(parameters.BuildQueryString(true));
            }
            return HMACSHA1(key, data.ToString());
        }

        /// <summary>
        /// 获取字符串SHA1加密串，兼容 php sha1()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SHA1(string str)
        {
            byte[] StrRes = Encoding.Default.GetBytes(str);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }

        #endregion 

        #region URL加密传参

        /// <summary>
        /// URL解密
        /// </summary>
        /// <typeparam name="T">返回参数类型</typeparam>
        /// <param name="req">参数集合</param>
        /// <param name="paramName">参数名</param>
        /// <returns></returns>
        public static T GetEncrypt<T>(this HttpRequest req, string paramName)
        {
            string _param = req.Params[paramName];

            try
            {
                string _decode = AESEncrypt(_param, "w3@4!e?0", "w3$4!e?0");

                return (T)Convert.ChangeType(_decode, typeof(T));
            }
            catch
            {
                return (T)Convert.ChangeType(_param, typeof(T)); ;
            }
        }

        /// <summary>
        /// URL加密
        /// </summary>
        /// <param name="input">明文字符</param>
        /// <param name="isUrlCode">是否编码</param>
        /// <returns></returns>
        public static string Encrypt(string input, bool isUrlCode = true)
        {
            return isUrlCode ? Uri.EscapeDataString(AESDecrypt(input, "w3@4!e?0", "w3$4!e?0")) : AESDecrypt(input, "w3@4!e?0", "w3$4!e?0");
        }

        #endregion

        #endregion

        #region string 字符串处理函数

        #region 获得两个字符串中间的字符串

        /// <summary>
        ///  获得两个字符串中间的字符串
        /// </summary>
        /// <param name="original">原始字符</param>
        /// <param name="startStr">开始字符</param>
        /// <param name="endStr">结束字符</param>
        /// <returns></returns>
        public static string GetMiddleStr(string original, string startStr, string endStr)
        {
            return GetMiddleStr(original, startStr, endStr, 0);
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="original">原始字符串</param>
        /// <param name="startStr">开始字符串</param>
        /// <param name="endStr">结束字符串</param>
        /// <param name="include">是否包含前后字符串，0：不包含，1：包含前，2：包含后，3：包含前后</param>
        /// <returns></returns>
        public static string GetMiddleStr(string original, string startStr, string endStr, int include)
        {
            string _str = "";

            int s = original.IndexOf(startStr);
            if (s < 0)
            {
                return "";
            }
            original = original.Substring(s + startStr.Length);
            int e = original.IndexOf(endStr);
            if (e > 0)
            {
                _str = original.Substring(0, e);
                switch (include)
                {
                    case 1:
                        _str = startStr + _str;
                        break;
                    case 2:
                        _str = _str + endStr;
                        break;
                    case 3:
                        _str = startStr + _str + endStr;
                        break;
                    default:
                        break;
                }
            }
            return _str;
        }

        #endregion

        #region 验证身份证号码

        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCard(string Id)
        {
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (Id.Length == 18)
            {
                long n = 0;
                if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;//数字验证
                }
                if (address.IndexOf(Id.Remove(2)) == -1)
                {
                    return false;//省份验证
                }

                string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = Id.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
                {
                    return false;//校验码验证
                }
                return true;//符合GB11643-1999标准

            }

            if (Id.Length == 15)
            {
                long n = 0;
                if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
                {
                    return false;//数字验证
                }
                if (address.IndexOf(Id.Remove(2)) == -1)
                {
                    return false;//省份验证
                }
                string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证
                }
                return true;//符合15位身份证标准
            }

            return false;
        }

        #endregion

        #region 验证Ip地址

        /// <summary>
        /// 是否IP地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string str)
        {
            if (str == null || str == string.Empty || str.Length < 7 || str.Length > 15)
                return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str);
        }

        #endregion

        #region 是否是数字

        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool IsNumeric(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);
            foreach (byte c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region 是否是中文

        /// <summary>
        /// 判断是否是中文
        /// </summary>
        /// <param name="CString"></param>
        /// <returns></returns>
        public static bool IsChina(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    BoolValue = true;
                }
            }
            return BoolValue;
        }

        #endregion

        #region 是否手机

        /// <summary>
        /// 校验手机号码是否符合标准。
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return false;

            return Regex.IsMatch(mobile, @"^(13|14|15|16|18|19)\d{9}$");
        }

        #endregion

        #region 生成验证码

        /// <summary>
        /// 创建随机字符
        /// </summary>
        /// <param name="length">创建随机字符</param>
        /// <returns></returns>
        public static string CreateRandomStr(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, Int32.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }


        #endregion

        #region 字符串字节数

        /// <summary>
        /// 字符串所占字节数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetByteLenght(this string str)
        {
            byte[] bytestr = System.Text.Encoding.Unicode.GetBytes(str);
            int j = 0;
            for (int i = 0; i < bytestr.GetLength(0); i++)
            {
                if (i % 2 == 0)
                {
                    j++;
                }
                else
                {
                    if (bytestr[i] > 0)
                    {
                        j++;
                    }
                }
            }
            return j;
        }

        #endregion

        #region 比较2个字符串相识度

        /// <summary>
        /// 比较2个字符的显示度，返回100内的整数（显示度50%返回50）【移位算法】
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static int GetSemblance(string str1, string str2)
        {
            LevenshteinDistance _ld = new LevenshteinDistance(str1, str2);
            _ld.Compute();
            return (int)(double.Parse(_ld.ComputeResult.Rate) * 100);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 加密解密类
    /// </summary>
    class MaAES
    {
        public MaAES(string _key, string _iv)
        {
            key = _key;
            iv = _iv;
        }

        private string key;
        private string m_key;
        /// <summary>
        /// MD5加密后的秘钥
        /// </summary>
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(m_key))
                {
                    m_key = TextHelper.MD5(key);
                }
                return m_key;
            }
        }

        private string iv;
        private string m_iv;
        /// <summary>
        /// 加密后的向量
        /// </summary>
        public string Iv
        {
            get
            {
                if (string.IsNullOrEmpty(m_iv))
                {
                    m_iv = TextHelper.MD5(iv);
                }
                return m_iv;
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <returns>密文</returns>
        public string AESEncrypt(string plainStr)
        {
            byte[] bKey = Encoding.Default.GetBytes(Key);
            byte[] bIV = Encoding.Default.GetBytes(Iv);
            byte[] byteArray = Encoding.Default.GetBytes(plainStr);

            string encrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        encrypt = Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();
            return encrypt;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptStr">密文字符串</param>
        /// <returns>明文</returns>
        public string AESDecrypt(string encryptStr)
        {
            byte[] bKey = Encoding.Default.GetBytes(Key);
            byte[] bIV = Encoding.Default.GetBytes(Iv);
            byte[] byteArray = Convert.FromBase64String(encryptStr);

            string decrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = Encoding.Default.GetString(mStream.ToArray());
                    }
                }
            }
            catch { }
            aes.Clear();

            return decrypt;
        }
    }
}
