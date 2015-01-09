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

namespace MaxLabs.bbsMax.Emoticons
{
    class Check
    {
        public static string GetFileNamePostfix(string filename)
        {
            return System.Text.RegularExpressions.Regex.Match(filename, "(?=.)[^.]*$").Value.ToUpper();
        }

        public static bool CheckImage(byte[] data)
        {
            if (data[0] == 0xff && data[1] == 0xd8)
                return true;
            if (data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38)
            {
                if (data[4] == 0x37 || data[4] == 0x39)
                {
                    if (data[5] == 0x61)
                        return true;
                }
            }
            return false;
        }
    }
}