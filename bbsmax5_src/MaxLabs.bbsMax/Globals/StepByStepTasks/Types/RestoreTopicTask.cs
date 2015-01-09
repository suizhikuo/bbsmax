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
    public class RestoreTopicTask : StepByStepTaskBase
    {
#if DEBUG

        const int stepRestoreCount = 1;

#else

        const int stepRestoreCount = 200;

#endif


        public override StepByStepTaskBase CreateInstance()
        {
            return new RestoreTopicTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {

            StringList paramData = StringList.Parse(param);

            TopicFilter filter = TopicFilter.Parse(paramData[0]);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            ThreadCollectionV5 threads = PostBOV5.Instance.GetThreads(operatorUser, filter, 1);

            if (threads == null || threads.Count == 0)
            {
                title = "没有数据可以还原";
                return false;
            }

            totalCount = threads.TotalRecords;

            title = "将还原 " + totalCount + " 个主题";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            StringList paramData = StringList.Parse(param);

            TopicFilter filter = TopicFilter.Parse(paramData[0]);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            int stepCount;
            if (PostBOV5.Instance.RestoreSearchTopics(operatorUser, filter, stepRestoreCount, out stepCount)) // .DeleteDoingsBySearch(filter, 200);
            {

                finishedCount += stepCount;

                isLastStep = stepCount < stepRestoreCount;

                title = "正在还原主题，总数 " + totalCount + "，已还原 " + finishedCount;

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
                title = "还原主题成功，共还原 " + finishedCount + " 个主题";

                StringList paramData = StringList.Parse(param);

                TopicFilter filter = TopicFilter.Parse(paramData[0]);

                User operatorUser = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithAll);

                Logs.LogManager.LogOperation(
                    new Topic_DeleteTopicBySearch(operatorUserID, operatorUser.Name, IPUtil.GetCurrentIP(), filter, finishedCount)
                );
            }
            else
                title = "还原主题失败";
        }
    }
}