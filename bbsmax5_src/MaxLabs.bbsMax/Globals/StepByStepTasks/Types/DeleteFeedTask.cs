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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public class DeleteFeedTask : StepByStepTaskBase
    {
#if DEBUG

        const int stepDeleteCount = 1;

#else

        const int stepDeleteCount = 200;

#endif


        public override StepByStepTaskBase CreateInstance()
        {
            return new DeleteFeedTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {
            FeedSearchFilter filter = FeedSearchFilter.Parse(param);

            int tempTotalCount;
            FeedCollection feeds = FeedBO.Instance.SearchFeeds(1, filter, out tempTotalCount);

            if (feeds == null || feeds.Count == 0)
            {
                title = "没有数据可以删除";
                return false;
            }

            totalCount = tempTotalCount;

            title = "将删除 " + totalCount + " 条动态";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            FeedSearchFilter filter = FeedSearchFilter.Parse(param);

            int stepCount;
            if (FeedBO.Instance.DeleteSearchFeeds(operatorUserID, filter, stepDeleteCount, out stepCount)) // .DeleteDoingsBySearch(filter, 200);
            {

                finishedCount += stepCount;

                isLastStep = stepCount < stepDeleteCount;

                title = "正在删除动态，总数 " + totalCount + "，已删 " + finishedCount;

                return true;
            }
            else
            {
                isLastStep = false;

                title = "发生错误";

                return false;
            }


        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                title = "删除动态成功，共删除 " + finishedCount + " 个动态";

                FeedSearchFilter filter = FeedSearchFilter.Parse(param);

                User operatorUser = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithAll);

                Logs.LogManager.LogOperation(
                    new Feed_DeleteFeedBySearch(operatorUserID, operatorUser.Name, IPUtil.GetCurrentIP(), filter, finishedCount)
                );
            }
            else
                title = "删除动态失败";
        }
    }
}