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
    public class JoinForumTask : StepByStepTaskBase
    {
#if DEBUG

        const int stepMoveCount = 1;

#else

        const int stepMoveCount = 200;

#endif


        public override StepByStepTaskBase CreateInstance()
        {
            return new JoinForumTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {
            StringTable keyValues = StringTable.Parse(param);

            Forum forum = ForumBO.Instance.GetForum(int.Parse(keyValues["oldForumID"]), false);

            if (forum == null )//|| forum.TotalThreads == 0)
            {
                title = "没有数据可以合并";
                return false;
            }

            totalCount = forum.TotalThreads;

            Forum newForum = ForumBO.Instance.GetForum(int.Parse(keyValues["newForumID"]), false);

            foreach (Forum tempForum in forum.AllSubForums)
            {
                ForumBO.Instance.MoveFourm(tempForum.ForumID, newForum.ForumID);
            }

            ForumBO.Instance.UpdateForumStatus(forum.ForumID, ForumStatus.JoinTo);
            ForumBO.Instance.UpdateForumStatus(newForum.ForumID, ForumStatus.Joined);

            title = "正在准备合并版块 将移动 " + totalCount + " 个主题";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            StringTable keyValues = StringTable.Parse(param);

            int oldForumID = int.Parse(keyValues["oldForumID"]);
            int newForumID = int.Parse(keyValues["newForumID"]);

            int stepCount;
            stepCount = ForumDaoV5.Instance.MoveForumThreads(oldForumID, newForumID, stepMoveCount);

            finishedCount += stepCount;

            //if (stepCount < stepDeleteCount)
            if (stepCount == 0)
            {
                ForumBO.Instance.DeleteForum(oldForumID);
                ForumBO.Instance.UpdateForumStatus(newForumID, ForumStatus.Normal);
                isLastStep = true;
            }
            else
                isLastStep = false;

            title = "合并版块 正在移动主题，总数 " + totalCount + "，已移动 " + finishedCount;

            return true;

        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                title = "合并版块成功，共合并 1个版块 移动" + finishedCount + " 个主题";
            }
            else
                title = "合并版块失败";
        }
    }
}