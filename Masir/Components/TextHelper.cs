using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Masir.Components
{
    /// <summary>
    /// 文本编码转换
    /// </summary>
    public class TextHelper
    {
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
