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
    public class UpdateVarsDataTask : StepByStepTaskBase
    {

        //const int stepUpdateCount = 1;

        public override StepByStepTaskBase CreateInstance()
        {
            return new UpdateVarsDataTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {

            List<int> values = StringUtil.Split2<int>(param);

            title = "将重新统计";
            if (values.Contains(1))
                title += " 今日帖子数";
            if (values.Contains(2))
                title += " 昨日帖子数";
            if (values.Contains(3))
                title += " 会员数";

            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            List<int> values = StringUtil.Split2<int>(param);

            bool recountUsers = false;
            if (offset == 0)
            {
                bool recountToday = false;
                bool recountYestoday = false;
                if (values.Contains(1))
                    recountToday = true;
                if (values.Contains(2))
                    recountYestoday = true;

                if (recountToday == false && recountYestoday == false)
                {
                    recountUsers = true;
                }
                else
                {
#if !Passport

                    title = "正重新统计帖子数";
                    bool success = PostBOV5.Instance.ReCountTopicsAndPosts(recountToday, recountYestoday);
#else
                    title = "";
                    bool success = true;
#endif

                    if (values.Contains(3))
                        isLastStep = false;
                    else
                        isLastStep = true;

                    offset = 2;
                    finishedCount++;
                    return success;
                }
            }
            else
            {
                if (values.Contains(3))
                    recountUsers = true;
            }

            title = "";
            if (recountUsers)
            {
                title = "正重新统计用户数";
                VarsManager.UpdateUserCount();
            }
            isLastStep = true;
            return true;

        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                title = "重新统计数据成功";
            }
            else
                title = "重新统计数据失败";
        }
    }
}