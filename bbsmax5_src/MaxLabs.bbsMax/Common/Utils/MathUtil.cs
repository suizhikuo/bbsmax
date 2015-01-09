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
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax
{
    public class MathUtil
    {
        /// <summary>
        /// 最大公因数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetGreatestCommonDivisor(int a, int b)
        {
            int i;
            int gcd = 1;
            for (i = 1; i <= a && i <= b; i++)
            {
                if (a % i == 0 && b % i == 0)
                    gcd = i;
            }

            return gcd;
        }

        /// <summary>
        /// 最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetLeastCommonMultiple(int a, int b)
        {
            return a * b / GetGreatestCommonDivisor(a, b);
        }

        /// <summary>
        /// 获取百分比
        /// </summary>
        /// <param name="count"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static double GetPercent(int count, int totalCount)
        {
            totalCount = totalCount == 0 ? 1 : totalCount;
            double percent = Math.Round((((float)count / (float)totalCount) * 100), 1);

            return percent;
        }
    }
}