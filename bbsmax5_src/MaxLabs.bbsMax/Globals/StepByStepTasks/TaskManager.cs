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

namespace MaxLabs.bbsMax.StepByStepTasks
{
    public class TaskManager
    {


        public static bool BeginTask(int userID, StepByStepTaskBase taskAction, string param)
        {
            Type taskType = taskAction.GetType();

            Guid taskID = Guid.NewGuid();

            RunningTask task = new RunningTask(taskAction);

            int totalCount = 0;
            long offset = 0;
            string title;
            if (task.Task.BeforeExecute(userID, param, ref offset, ref totalCount, out title) == false)
                return false;


            int result = StepByStepTaskDao.Instance.BeginTask(taskID, taskType, userID, param, totalCount, offset, title, taskAction.InstanceMode);

            switch (result)
            {
                case 0:
                    return true;

                case 1:
                case 2:
                    return false;
            
                default:
                    return false;
            }
        }

        public static RunningTask GetRunningTask(int userID, Guid taskID)
        {
            return StepByStepTaskDao.Instance.GetRunningTask(userID, taskID);
        }

        public static RunningTaskCollection GetRunningTasks(int operatorUserID)
        {
            return StepByStepTaskDao.Instance.GetRunningTasks(operatorUserID);
        }

        public static RunningTaskCollection GetRunningTasks(int operatorUserID, Type taskType, string param)
        {

            return StepByStepTaskDao.Instance.GetRunningTasks(operatorUserID, taskType, param);
        }

        private static object executeLocker = new object();

        /// <summary>
        /// 执行任务的当前步骤，并返回一个值表示是否继续执行
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="taskID"></param>
        /// <param name="percent"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool ExecuteTaskStep(int operatorUserID, Guid taskID, out int percent, out string title)
        {

            percent = 0;
            title = string.Empty;

            bool result = true;

            lock (executeLocker)
            {

                RunningTask task = TaskManager.GetRunningTask(operatorUserID, taskID);

                if (task == null || task.Task == null)
                {
                    return false;
                }
                else
                {

                    bool isLastStep;
                    if (task.Task.StepExecute(task.UserID, task.Param, ref task.Offset, ref task.TotalCount, ref task.FinishedCount, out task.Title, out isLastStep))
                    {

                        if (isLastStep)
                        {
                            task.Task.AfterExecute(task.UserID, task.Param, true, task.TotalCount, task.FinishedCount, out task.Title);
                            StepByStepTaskDao.Instance.FinishTask(task.TaskID);
                            result = false;
                        }
                        else
                            StepByStepTaskDao.Instance.UpdateRunnintTaskStatus(task.TaskID, task.Param, task.TotalCount, task.FinishedCount, task.Offset, task.Title);
                    }
                    else
                    {
                        task.Task.AfterExecute(task.UserID, task.Param, false, task.TotalCount, task.FinishedCount, out task.Title);
                        result = false;
                    }

                    percent = task.Percent;
                    title = task.Title;

                }

            }

            return result;
        }


        /// <summary>
        /// 检查是否有一个类型的任务在执行
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public static bool IsRunning(IEnumerable<StepByStepTaskBase> taskTypes)
        {
            List<string> types = new List<string>();
            foreach (StepByStepTaskBase task in taskTypes)
            {
                types.Add(task.GetType().FullName);
            }

            if (types.Count == 0)
                return false;

            return StepByStepTaskDao.Instance.IsRunning(types);
        }
    }
}