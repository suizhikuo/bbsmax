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
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public abstract class StepByStepTaskDao : DaoBase<StepByStepTaskDao>
    {

        public abstract RunningTaskCollection GetRunningTasks(int userID);

        public abstract RunningTaskCollection GetRunningTasks(int userID, Type taskType, string param);

        public abstract RunningTask GetRunningTask(int userID, Guid taskID);

        public abstract int BeginTask(Guid taskID, Type type, int userID, string param, int totalCount, long offset, string title, TaskType instanceMode);

        //public abstract void UpdateTaskStatus(Guid taskID, string param, long totalCount, long offset);

        public abstract void UpdateRunnintTaskStatus(Guid taskID, string param, int totalCount, int finishedCount, long offset, string title);

        public abstract void FinishTask(Guid taskID);

        public abstract bool IsRunning(IEnumerable<string> taskTypes);
    }
}