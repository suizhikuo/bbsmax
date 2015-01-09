//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public class DeleteFeedJob : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.AfterRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Day; }
        }

        public override bool Enable
        {
            get 
            {
                return Settings.AllSettings.Current.FeedJobSettings.Enable;
            }
        }

        public override void Action()
        {
            try
            {
                if (Settings.AllSettings.Current.FeedJobSettings.ClearMode == MaxLabs.bbsMax.Enums.JobDataClearMode.ClearByRows)
                    FeedBO.Instance.DeleteFeeds(null, Settings.AllSettings.Current.FeedJobSettings.Count);
                else if (Settings.AllSettings.Current.FeedJobSettings.ClearMode == MaxLabs.bbsMax.Enums.JobDataClearMode.ClearByDay)
                    FeedBO.Instance.DeleteFeeds(DateTimeUtil.Now.AddDays(0 - Settings.AllSettings.Current.FeedJobSettings.Day), 0);
                else
                    FeedBO.Instance.DeleteFeeds(DateTimeUtil.Now.AddDays(0 - Settings.AllSettings.Current.FeedJobSettings.Day), Settings.AllSettings.Current.FeedJobSettings.Count);
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

        protected override void SetTime()
        {
            SetDayExecuteTime(Settings.AllSettings.Current.FeedJobSettings.ExecuteTime, 0, 0);
        }
    }

}