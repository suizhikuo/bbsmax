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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Errors
{
    public class OverTodayAlowAttachmentCountError : ErrorInfo
    {
        public OverTodayAlowAttachmentCountError(int totalAllowCount, int todayAllowCount, int currentCount)
            : base()
        {
            TodayAllowCount = todayAllowCount;
            CurrentCount = currentCount;
            TotalAllowCount = totalAllowCount;
        }

        public int TodayAllowCount { get; private set; }
        public int CurrentCount { get; private set; }
        public int TotalAllowCount { get; private set; }
        public override string Message
        {
            get
            {
                if(TodayAllowCount == 0)
                    return string.Format("超过了您今天允许上传的最大附件个数{0}，您今天已经不能再上传附件", TotalAllowCount);
                else
                    return string.Format("超过了您今天允许上传的最大附件个数{1}，您本次只允许上传{0}个附件", TodayAllowCount, TotalAllowCount);
            }
        }
    }
}