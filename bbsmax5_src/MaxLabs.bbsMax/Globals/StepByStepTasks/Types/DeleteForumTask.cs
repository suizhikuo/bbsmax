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

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public class DeleteForumTask : StepByStepTaskBase
    {
#if DEBUG

        const int stepDeleteCount = 1;

#else

        const int stepDeleteCount = 200;

#endif


        public override StepByStepTaskBase CreateInstance()
        {
            return new DeleteForumTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {
            Forum forum = ForumBO.Instance.GetForum(int.Parse(param), false);

            if (forum == null )//|| forum.TotalThreads == 0)
            {
                title = "没有数据可以删除";
                return false;
            }

            ForumBO.Instance.UpdateForumStatus(forum.ForumID, ForumStatus.Deleted);

            totalCount = forum.TotalThreads;

            title = "将删除版块“" + forum.ForumName + "”及 " + totalCount + " 个主题";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            int forumID = int.Parse(param);

            Forum forum = ForumBO.Instance.GetForum(int.Parse(param), false);

            if (forum == null)
            {
                title = "版块“" + forum.ForumName + "”已经被删除";
                isLastStep = false;
                return false;
            }

            int stepCount;
            stepCount = ForumDaoV5.Instance.DeleteForumThreads(forumID, stepDeleteCount);

            finishedCount += stepCount;

            //if (stepCount < stepDeleteCount)
            if (stepCount == 0)
            {
                ForumBO.Instance.DeleteForum(forumID);
                isLastStep = true;
            }
            else
                isLastStep = false;

            title = "正删除版块“" + forum.ForumName +"”及其主题，主题数 " + totalCount + "，已删 " + finishedCount;

            return true;

        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                title = "删除版块成功，共删除 1 个版块及 " + finishedCount + " 个主题";
            }
            else
                title = "删除版块失败";
        }
    }
}