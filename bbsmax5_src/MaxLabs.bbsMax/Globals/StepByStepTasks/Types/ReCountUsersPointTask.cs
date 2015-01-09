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
    public class ReCountUsersPointTask : StepByStepTaskBase
    {
#if DEBUG

        const int stepCount = 1;

#else

        const int stepCount = 200;

#endif


        public override StepByStepTaskBase CreateInstance()
        {
            return new ReCountUsersPointTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {
            totalCount = VarsManager.Stat.TotalUsers;

            offset = 1;

            title = "将重新计算 " + totalCount + " 个用户的积分";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            int endUserID;
            if (UserBO.Instance.ReCountUsersPoints((int)offset, stepCount, out endUserID)) // .DeleteDoingsBySearch(filter, 200);
            {
                isLastStep = endUserID == 0;

                if (isLastStep)
                    finishedCount = totalCount;


                title = "正在重新计算用户积分，总数 " + totalCount + "，已计算 " + finishedCount;

                offset = endUserID + 1;

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
                title = "重新计算用户积分成功，共计算 " + finishedCount + " 个用户";
            }
            else
                title = "重新计算用户积分失败";
        }
    }
}