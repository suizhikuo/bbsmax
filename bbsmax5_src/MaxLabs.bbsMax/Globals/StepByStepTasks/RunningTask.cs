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
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.StepByStepTasks
{
    /// <summary>
    /// 正在运行的任务
    /// </summary>
    public class RunningTask
    {
        public RunningTask(StepByStepTaskBase taskAction)
        {
            m_Task = taskAction;
        }

        public RunningTask(DataReaderWrap readerWrap)
        {
            TaskID = readerWrap.Get<Guid>("TaskID");
            Type = readerWrap.Get<string>("Type");
            UserID = readerWrap.Get<int>("UserID");
            Param = readerWrap.Get<string>("Param");
            TotalCount = readerWrap.Get<int>("TotalCount");
            FinishedCount = readerWrap.Get<int>("FinishedCount");
            Offset = readerWrap.Get<long>("Offset");
            Title = readerWrap.Get<string>("Title");
            LastExecuteTime = readerWrap.Get<DateTime>("LastExecuteTime");
        }

        public Guid TaskID;

        public string Type;

        public int UserID;

        public string Param;

        public int TotalCount;

        public int FinishedCount;

        public long Offset;

        public DateTime LastExecuteTime;

        public string Title;

        //===========================================


        public int Percent
        {
            get
            {
                if (TotalCount == 0 || FinishedCount == 0)
                    return 0;

                if (FinishedCount >= TotalCount)
                    return 100;

                return FinishedCount * 100 / TotalCount;
            }
        }

        private StepByStepTaskBase m_Task = null;

        public StepByStepTaskBase Task
        {
            get
            {
                if (m_Task == null)
                {
                    try
                    {
                        Type taskType = System.Type.GetType(Type);

                        m_Task = (StepByStepTaskBase)Activator.CreateInstance(taskType);
                    }
                    catch
                    {
                        return null;
                    }
                }

                return m_Task;
            }
        }

        public string HandlerUrl
        {
            get
            {
                return BbsRouter.GetUrl("handler/steptask", "TaskID=" + this.TaskID.ToString("N"));
            }
        }

        //public long NextOffset { get; set; }
    }

    public class RunningTaskCollection : Collection<RunningTask>
    {
        public RunningTaskCollection(DataReaderWrap readerWrap)
        {
            RunningTask task;
            while (readerWrap.Next)
            {
                try
                {
                    task = new RunningTask(readerWrap);

                    this.Add(task);
                }
                catch { }
            }
        }
    }
}