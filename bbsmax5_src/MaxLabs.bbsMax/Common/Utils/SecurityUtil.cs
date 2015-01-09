//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Web.Security;
using System.Web;

namespace MaxLabs.bbsMax
{
    public class SecurityUtil
	{
        public SecurityUtil()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		public static string Encrypt(EncryptFormat format, string text)
		{
			string encryptedText;
			switch (format)
			{
				case EncryptFormat.MD5:
					encryptedText = MD5(text);
					break;
				case EncryptFormat.SHA1:
					encryptedText = SHA1(text);
					break;
				case EncryptFormat.bbsMax:
                    encryptedText = MD5(text);
                    break;
                case EncryptFormat.Dvbbs:
                    encryptedText = ASPMD5(text).Substring(8,16);
                    break;                
				default:
					encryptedText = text;
					break;
			}
			return encryptedText;
		}

        public static string MD5(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Consts.EmptyMD5;

            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");
        }

        /// <summary>
        /// 获取内容的安全码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetContentSecurityCode(string content)
        {
            string key = Globals.SafeString;
            return MD5(content + key);
        }

        public static string ASPMD5(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Consts.EmptyMD5;

            Byte[] byteToHash = System.Text.Encoding.GetEncoding("gb2312").GetBytes(text);
            //使用直接创建的   MD5   类的实例创建   String   2 的哈希值   
            byte[] hashvalue = (new MD5CryptoServiceProvider()).ComputeHash(byteToHash);
            //将byte数组转化为string   
            return BitConverter.ToString(hashvalue).Replace("-","");
        }

		public static string SHA1(string text)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");
		}

        const string KEY_64 = "!bbsmax!";
        const string IV_64 = "!zzbird!"; //注意了，是8个字符，64位 

        public static string DesEncode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

        }

        public static string DesDecode(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        public static string Base64Encode(string Message)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Message));
        }
        public static string Base64Decode(string Message)
        {
            byte[] outbyte = Convert.FromBase64String(Message);
            return System.Text.Encoding.UTF8.GetString(outbyte);
        }

        /// <summary>
        /// 输入的密码与数据库密码对比
        /// </summary>
        /// <param name="format">加密格式</param>
        /// <param name="inputPassword">用户输入的密码</param>
        /// <param name="userPassword">数据库存储的密码</param>
        /// <returns></returns>
        public static bool ComparePassword(EncryptFormat format, string inputPassword, string userPassword)
        {
            if (ComparePasswordWithoutEncode(format, inputPassword, userPassword))
                return true;

            else if (ComparePasswordWithoutEncode(format, HttpUtility.HtmlEncode(inputPassword), userPassword))
                return true;

            return false;
        }

        private static bool ComparePasswordWithoutEncode(EncryptFormat format, string inputPassword, string userPassword)
        {
            string encryptedUserPassowrd;
            if (EncryptFormat.MolyX == format)//molyx的加密方法MD5(MD5(password)+salt)
                encryptedUserPassowrd = userPassword.Substring(0, 5) + MD5(MD5(inputPassword).ToLower() + userPassword.Substring(0, 5));
            else if (EncryptFormat.Discuz == format)
                encryptedUserPassowrd = userPassword.Substring(0, 6) + MD5(MD5(inputPassword).ToLower() + userPassword.Substring(0, 6));
            else
                encryptedUserPassowrd = Encrypt(format, inputPassword);

            if (string.Compare(encryptedUserPassowrd, userPassword, format != EncryptFormat.ClearText) == 0)
                return true;

            return false;
        }
	}

    /// <summary>
    /// 密码加密方式
    /// </summary>
    public enum EncryptFormat : byte
    {
        /// <summary>
        /// 明文
        /// </summary>
        ClearText = 0,
        /// <summary>
        /// MD5加密
        /// </summary>
        MD5 = 1,
        /// <summary>
        /// SHA1加密
        /// </summary>
        SHA1 = 2,
        /// <summary>
        /// bbsMax专用格式(客户端MD5+服务器端MD5)
        /// </summary>
        bbsMax = 3,
        /// <summary>
        /// 动网论坛格式
        /// </summary>
        Dvbbs = 4,
        /// <summary>
        /// 印象论坛格式
        /// </summary>
        NowBoard = 5,
        /// <summary>
        /// 魔力论坛格式 
        /// </summary>
        MolyX = 6,
        /// <summary>
        /// DZ6.1的加密格式
        /// </summary>
        Discuz = 7,
    }
}