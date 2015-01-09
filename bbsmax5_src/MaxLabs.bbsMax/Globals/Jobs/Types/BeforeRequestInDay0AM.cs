//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 每天0点 执行
    /// </summary>
    public class BeforeRequestInDay0AM : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.BeforeRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Day; }
        }

        public override bool Enable
        {
            get 
            {
                return true;
            }
        }

        public override void Action()
        {

#if !Passport

            try
            {
                ForumBO.Instance.ResetTodayPosts();
                ThreadCachePool.ClearCahceAt0AM();
                UserBO.Instance.ClearMostActiveUsersCache(new ActiveUserType[] { ActiveUserType.DayOnlineTime, ActiveUserType.DayPosts, ActiveUserType.WeekOnlineTime, ActiveUserType.WeekPosts, ActiveUserType.MonthPosts, ActiveUserType.MonthOnlineTime });

                UserBO.Instance.ClearExperiesExtendField();


            }
            catch { }

#endif
        }

        protected override void SetTime()
        {
            SetDayExecuteTime(0, 0, 1);
        }
    }

}