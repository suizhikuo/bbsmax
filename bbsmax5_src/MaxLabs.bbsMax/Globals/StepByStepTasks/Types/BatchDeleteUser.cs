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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public class BatchDeleteUser:StepByStepTaskBase
    {

        public override StepByStepTaskBase CreateInstance()
        {
            return new BatchDeleteUser();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        private int[] UserIDs
        {
            get;
            set;
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {
            UserIDs = StringUtil.Split<int>(param);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            if (operatorUser == null)
            {
                title = "没有权限";
                return false;
            }

            bool containsSelf = false;

            foreach (int uid in UserIDs)
            {
                if (uid == operatorUserID)
                {
                    containsSelf = true;
                }
            }

            if (containsSelf)
            {
                title = "待删除用户列表里不能包含自己的帐号！";
                return false;
            }


            if (UserIDs.Length > 0)
            {
                totalCount = UserIDs.Length;
                title = "准备批量删除用户，总共" + UserIDs.Length + "个";
                return true;
            }
            else
            {
                title = "没有选择用户！";
            }
            return false;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            
            UserIDs = StringUtil.Split<int>(param);

            int userId = UserIDs[finishedCount];
            User user = UserBO.Instance.GetUser(userId);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            if (user == null)
            {
                title = "ID为" + userId + "的用户不存在";
                finishedCount++;
            }
            else
            {
                if (!UserBO.Instance.CanDeleteUser(operatorUser, userId) || user.IsOwner)
                {
                    title = "你没有权限删除用户" + user.Username;
                    finishedCount++;
                }
                else
                {

                    int dataCount = UserBO.Instance.DeleteUser(operatorUser, userId, (int)offset, DeleteUserTask.DeleteUserSteps.Count);

                    title = string.Format("删除{0}：{1}", user.Username, DeleteUserTask.DeleteUserSteps[(int)offset]);
                    if (dataCount == 0)
                    {
                        offset++;
                    }
                    else
                    {
                        title += "，剩余" + dataCount;
                    }

                    if (offset == DeleteUserTask.DeleteUserSteps.Count)
                    {
                        offset = 0;
                        finishedCount++;
                    }
                }
            }

            isLastStep = finishedCount == UserIDs.Length;
            return true;
        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                UserIDs = StringUtil.Split<int>(param);

                title = "已经成功删除" + UserIDs.Length + "个用户";
            }
            else
            {
                title = "删除用户失败！";
            }
        }
    }
}