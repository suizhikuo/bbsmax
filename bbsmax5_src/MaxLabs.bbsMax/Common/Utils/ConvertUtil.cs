//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.IO;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class ConvertUtil
    {
        public static string FormatSize(long byteSize)
        {
            if (byteSize == 0)
                return "0 K";

            string unit = "BKMG";
            if (byteSize >= 1024)
            {
                double size = byteSize;
                int indx = 0;
                do
                {
                    size = size / 1024.00;
                    indx++;
                } while (size >= 1024 && indx < unit.Length - 1);
                return Math.Round(size, 1).ToString() + " " + unit[indx];
            }
            else
                return byteSize.ToString() + " B";
        }

        public static long GetFileSizeValue(long byteSize)
        {
            if (byteSize == 0)
                return 0;

            string unit = "BKMG";
            if (byteSize >= 1024)
            {
                double size = byteSize;
                int indx = 0;
                do
                {
                    size = size / 1024.00;
                    indx++;
                } while (size >= 1024 && indx < unit.Length - 1);
                return (long)Math.Round(size, 1);
            }
            else
                return byteSize;
        }

        public static FileSizeUnit GetFileSizeUnit(long byteSize)
        {
            //return FileSizeUnit.K;

            string unit = "BKMG";
            if (byteSize >= 1024)
            {
                double size = byteSize;
                int indx = 0;
                do
                {
                    size = size / 1024.00;
                    indx++;
                } while (size >= 1024 && indx < unit.Length - 1);

                switch (indx)
                {
 
                    case 1: return FileSizeUnit.K;
                    case 2: return FileSizeUnit.M;
                    case 3: return FileSizeUnit.G;
                    case 4: return FileSizeUnit.T;
                }
            }
            return FileSizeUnit.B;
        }

        public static long GetFileSize(long value,FileSizeUnit fileSizeUnit)
        {
            switch (fileSizeUnit)
            {
                case FileSizeUnit.B: return value;
                case FileSizeUnit.K: return value * 1024;
                case FileSizeUnit.M: return value * 1024 * 1024;
                case FileSizeUnit.G: return value * 1024 * 1024 * 1024;
                default: return value * 1024 * 1024 * 1024 * 1024;
            }
        }
    }
}