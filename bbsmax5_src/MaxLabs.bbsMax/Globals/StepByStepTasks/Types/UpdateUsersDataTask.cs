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
    public class UpdateUsersDataTask : StepByStepTaskBase
    {

        const int stepUpdateCount = 1000;

        public override StepByStepTaskBase CreateInstance()
        {
            return new UpdateUsersDataTask();
        }

        public override TaskType InstanceMode
        {
            get { return TaskType.SystemSingleInstance; }
        }

        public override bool BeforeExecute(int operatorUserID, string param, ref long offset, ref int totalCount, out string title)
        {

            title = "正在准备更新用户数据";
            totalCount = VarsManager.Stat.TotalUsers;
            return true;
        }

        public override bool StepExecute(int operatorUserID, string param, ref long offset, ref int totalCount, ref int finishedCount, out string title, out bool isLastStep)
        {
            List<int> dataTypes = StringUtil.Split2<int>(param);

            bool updatePostCount = dataTypes.Contains(1); 
            bool updateBlogCount = dataTypes.Contains(2);  
            bool updateInviteCount = dataTypes.Contains(8);  
            bool updateCommentCount = dataTypes.Contains(3);  
            bool updatePictureCount = dataTypes.Contains(4);  
            bool updateShareCount = dataTypes.Contains(5);
            bool updateDoingCount = dataTypes.Contains(6);
            bool updateDiskFileCount = dataTypes.Contains(7);

            int startUserID = (int)offset;
            startUserID = startUserID > 0 ? startUserID : 1;

            startUserID = UserBO.Instance.UpdateUsersDatas(startUserID, stepUpdateCount, updatePostCount, updateBlogCount, updateInviteCount, updateCommentCount, updatePictureCount, updateShareCount, updateDoingCount, updateDiskFileCount);

            if (startUserID == -1)
            {
                isLastStep = true;
                CacheUtil.RemoveBySearch("User/");
                finishedCount = VarsManager.Stat.TotalUsers;
            }
            else
            {
                isLastStep = false;
                finishedCount += stepUpdateCount;
            }

            offset = startUserID;



            title = "正在更新用户数据(" + finishedCount + "/" + VarsManager.Stat.TotalUsers + ")";

            return true;

        }

        public override void AfterExecute(int operatorUserID, string param, bool success, int totalCount, int finishedCount, out string title)
        {
            if (success)
            {
                title = "更新用户数据成功，共更新 " + finishedCount + " 个用户";
            }
            else
                title = "更新用户数据成功";
        }
    }
}