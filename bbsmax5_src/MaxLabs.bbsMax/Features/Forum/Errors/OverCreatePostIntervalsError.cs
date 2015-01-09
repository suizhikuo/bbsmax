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
    public class OverCreatePostIntervalsError : ErrorInfo
    {
        public OverCreatePostIntervalsError(string target, TimeSpan postInterval, DateTime lastPostDate)
            : base(target)
        {
            PostInterval = postInterval;
            LastPostDate = lastPostDate;
        }

        public TimeSpan PostInterval { get; private set; }
        public DateTime LastPostDate { get; private set; }

        public override string Message
        {
            get { return "您发帖速度太快了，请在" + getTime() + "后再重试"; }
        }

        private string getTime()
        {
            int totalSeconds = (int)(PostInterval - (DateTimeUtil.Now - LastPostDate)).TotalSeconds;

            int day = totalSeconds / (60 * 60 * 24);
            int remain = totalSeconds % (60 * 60 * 24);
            int hour = remain / (60 * 60);
            remain = remain % (60 * 60);
            int minute = remain / 60;
            remain = remain % 60;
            int seconds = remain;

            string time = "";

            if (day > 0)
                time = day + "天";
            if (hour > 0)
                time = time + hour + "小时";
            if (minute > 0)
                time = time + minute + "分";
            if (seconds > 0)
                time = time + seconds + "秒";
            else
                time = "1秒";

            return time;
        }

    }
}