//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Text;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Errors
{
    public class InvalidPollItemCountError : ErrorInfo
    {
        public InvalidPollItemCountError(string target, int maxCount, int minCount, int currentCount)
            : base(target) 
        {
            MaxCount = maxCount;
            MinCount = minCount;
            CurrentCount = currentCount;
        }

        public int MaxCount { get; private set; }
        public int MinCount { get; private set; }
        public int CurrentCount { get; private set; }
        public override string Message
        {
            get
            {
                if (CurrentCount > MaxCount)
                    return string.Format("投票选项不能超过{0}项", MaxCount);
                else
                    return string.Format("投票选项不能少于{0}项", MinCount);
            }
        }
    }
}