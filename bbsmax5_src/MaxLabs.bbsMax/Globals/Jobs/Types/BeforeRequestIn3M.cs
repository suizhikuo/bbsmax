//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 每5分钟执行一次  的清理过期数据的任务
    /// </summary>
    public class BeforeRequestIn5M : JobBase
    {

        public override ExecuteType ExecuteType
        {
            get { return ExecuteType.BeforeRequest; }
        }

        public override TimeType TimeType
        {
            get { return TimeType.Interval; }
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
            ////清理主题状态（例如：定时置顶、定时高亮）
            //try
            //{
            //    PostManager.ProcessExperiesTopicStatus();
            //}
            //catch { }

            ////清理主题的过期数据（例如：已到期投票等）
            //try
            //{
            //    PostManager.ClearExpiresDatas();
            //}
            //catch { }

#if !Passport

            try
            {
                ProcessBeforeRequestIn3M();
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }

#endif

            ////解锁已经到期的被锁定头像
            //try
            //{
            //    UserBO.Instance.UnlockUserAvatars();
            //}
            //catch { }
        }

        protected override void SetTime()
        {
            SetIntervalExecuteTime(3 * 60);
        }

#if !Passport

        private void ProcessBeforeRequestIn3M()
        {
            //已经到期的、需要处理的主题状态（例如：到期的置顶、到期的主题高亮）
            TopicStatusCollection experiesTopicStatus;
            
            //需要处理的提问对应的主题ID
            List<int> autoFinalQuestionThreadIds;

            //结贴时，每个板块中的用户得分情况（不同板块的积分策略可能不同，因此此处需要先将不同板块区分开来）
            Dictionary<int, Dictionary<int, int>> autoFinalQuestionForumIDAndRewards;


            //以上是本计划任务所需的所有数据的变量的声明
            //--------------------------------------------------------------

            //查询本计划任务所需的所有数据
            JobDao.Instance.QueryForBeforeRequestIn3M(out experiesTopicStatus, out autoFinalQuestionThreadIds, out autoFinalQuestionForumIDAndRewards);

            //以下开始处理这些数据
            //--------------------------------------------------------------


            //处理过期的主题状态（例如：到期的置顶、到期的主题高亮）
            PostBOV5.Instance.ProcessExperiesTopicStatus(experiesTopicStatus);

            //自动结束提问贴并自动散分
            PostBOV5.Instance.AutoFinalQuestion(autoFinalQuestionThreadIds, autoFinalQuestionForumIDAndRewards);

            ThreadCachePool.UpdateCache(autoFinalQuestionThreadIds);


        }

#endif
    }

}