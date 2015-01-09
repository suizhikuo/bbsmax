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
using System.Security.Cryptography;

namespace MaxLabs.bbsMax.Emoticons
{
    class MD5
    {
        /// <summary>
        /// ��ȡMD5ֵ��32λ��
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static string GetMD5(byte[] fs)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] md5byte = md5.ComputeHash(fs);
            string str = string.Empty;
            int i, j;
            foreach (byte b in md5byte)
            {
                i = Convert.ToInt32(b);
                j = i >> 4;
                str += (Convert.ToString(j, 16));
                j = ((i << 4) & 0x00ff) >> 4;
                str += (Convert.ToString(j, 16));
            }
            return str.ToUpper();
        }
    }
}