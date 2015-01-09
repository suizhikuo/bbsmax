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
using MaxLabs.bbsMax;
using System.Collections;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public class DeleteUserTask : StepByStepTaskBase
    {
        List<string> stepNames;


        public static List<string> DeleteUserSteps
        {
            get;
            private set;
        }

        static DeleteUserTask()
        {
            DeleteUserSteps = new List<string>();
            DeleteUserSteps.Add("屏蔽用户");
            DeleteUserSteps.Add("删除动态");
            DeleteUserSteps.Add("删除评论");
            DeleteUserSteps.Add("删除通知");
            DeleteUserSteps.Add("删除邀请码");
            DeleteUserSteps.Add("去除邀请关系");
            DeleteUserSteps.Add("删除网络硬盘");
            DeleteUserSteps.Add("删除自定义表情");
            DeleteUserSteps.Add("删除对话记录");
            DeleteUserSteps.Add("删除记录(DOING)");
            DeleteUserSteps.Add("删除分享、收藏");
            DeleteUserSteps.Add("删除空间、文章访问或者被访记录");
            DeleteUserSteps.Add("删除博客文章");
            DeleteUserSteps.Add("删除博客相册");
            //---------------------------------14
            DeleteUserSteps.Add("删除附件");
            DeleteUserSteps.Add("删除帖子");
            DeleteUserSteps.Add("删除主题");
            //---------------------------------17
            DeleteUserSteps.Add("删除好友");
            DeleteUserSteps.Add("删除空间访问记录");
            DeleteUserSteps.Add("删除用户积分明细");
            DeleteUserSteps.Add("删除主用户记录");
            DeleteUserSteps.Add("重新统计数据");
            DeleteUserSteps.Add("记录系统日志并更新整站缓存");

        }

        public DeleteUserTask()
        {
            stepNames = DeleteUserSteps;
        }

        public override StepByStepTaskBase CreateInstance()
        {
            return new DeleteUserTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemMultipleInstances; }
        }

        private void ProccessParam(string param)
        {
            string[] paramArray = param.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            UserID = int.Parse(paramArray[0]);
            DeletePost = StringUtil.TryParse<bool>(paramArray[1]);
        }

        private bool DeletePost = true;
        private int UserID;

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {
            ProccessParam(param);

            if (operatorUserID == UserID)
            {
                title = "无法删除自己的帐号";
                return false;
            }

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            if (UserBO.Instance.CanDeleteUser(operatorUser, UserID))
            {
                totalCount = stepNames.Count;
                title = string.Format("删除ID为{0}的用户：正在{1}", UserID, stepNames[0]);
                return true;
            }
            else
            {
                totalCount = 0;
                title = "没有权限删除ID为" + UserID + "的ID";
            }
            return false;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {

            ProccessParam(param);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            if (!DeletePost)
            {
                if (finishedCount >= 14 && finishedCount <= 16)
                {
                    title = "跳过帖子删除";
                    finishedCount++;
                    isLastStep = false;
                    return true;
                }
            }
            if (finishedCount < stepNames.Count-1)
            {
                title = string.Format("删除ID为{0}的用户：正在{1}", UserID, stepNames[finishedCount + 1]);
            }
            else
            {
                title = string.Empty;
            }

            int count = UserBO.Instance.DeleteUser(operatorUser, UserID, finishedCount, stepNames.Count);

            if (count == 0)
            {
                finishedCount++;
            }
            else if (count > 0)
            {
                title += "，剩余" + count;
            }

            isLastStep = stepNames.Count == finishedCount;

            return true;
        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            ProccessParam(param);

            if (!success || UserBO.Instance.GetUser(UserID) != null)
            {
                title = "删除用户失败";
            }
            else
            {
                title = "已成功删除ID为" + UserID + "的用户";
            }
        }
    }
}