//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 哈希助手类
	/// </summary>
	public class HashUtil
	{
		/// <summary>
		/// 计算流的MD5
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static string GetMD5(Stream stream)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			md5.ComputeHash(stream);

			byte[] hash = md5.Hash;

			StringBuilder result = new StringBuilder();

			foreach (byte byt in hash)
			{
				result.Append(String.Format("{0:X1}", byt));
			}

			return result.ToString();
		}
	}
}