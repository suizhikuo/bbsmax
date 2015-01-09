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
    class Contrast
    {
        public static bool ShortArray(ushort[] Array1, ushort[] Array2)
        {
            if (Array1.Length == Array2.Length)
            {
                for (int i = 0; i < Array1.Length; i++)
                {
                    if (Array1[i] != Array2[i] && Array1[i] != Array2[i] + 0x20 && Array1[i] != Array2[i] - 0x20)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
                return false;
        }

        public static bool IsImage(Model_Property model)
        {
            if (model.NameSize == 0)
                return false;
            for (int i = 0; i < model.Name.Length; i++)
            {
                if (i<model.NameSize/2-1)
                {
                    if (model.Name[i] < 0x30 || model.Name[i] > 0x39)
                        return false;
                }
                else
                {
                    if (model.Name[i] != 0)
                        return false;
                }
            }
            return true;
        }

        public static bool IsImageFixed(Model_Property model)
        {
            if (model.Name.Length < 7)
                return false;
            int FixedNo = model.Name.Length - 6;
            for (int i = 0; i < FixedNo; i++)
            {
                if (model.Name[i] < 0x30 || model.Name[i] > 0x39)
                    return false;
            }

            if (model.Name[FixedNo] == 0x46 && model.Name[FixedNo + 1] == 0x69 && model.Name[FixedNo + 2] == 0x78 && model.Name[FixedNo + 3] == 0x65 && model.Name[FixedNo + 4] == 0x64 && model.Name[FixedNo + 5] == 0)
                return true;

            return false;
        }
    }
}