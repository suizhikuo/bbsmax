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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Jobs
{
    public class JobManager
    {


        private static DateTime lastCheckTime_AfterRequest = DateTime.MinValue;

        private readonly static List<JobBase> s_AllJobs = new List<JobBase>();

        private static List<JobBase> s_BeforeRequestJobs = null;
        private static List<JobBase> s_AfterRequestJobs = null;
        private static List<JobBase> s_InThreadJobs = null;

        //private static object registerJobLocker = new object();
        /// <summary>
        /// 注册一个任务类型
        /// </summary>
        /// <param name="mission"></param>
        public static void RegisterJob(JobBase job)
        {
            s_AllJobs.Add(job);
        }

        #region 三个方法分别获取三种类型的任务

        /// <summary>
        /// 获得所有在请求之前就要处理的Job
        /// </summary>
        /// <returns></returns>
        public static List<JobBase> GetBeforeRequestJobs()
        {
            if (s_BeforeRequestJobs == null)
            {
                List<JobBase> beforeRequestJobs = new List<JobBase>();

                foreach (JobBase job in s_AllJobs)
                {
                    if (job.ExecuteType == ExecuteType.BeforeRequest)
                        beforeRequestJobs.Add(job);
                }

                s_BeforeRequestJobs = beforeRequestJobs;
            }
            return s_BeforeRequestJobs;
        }

        /// <summary>
        /// 获得所有在页面请求后需要处理的Job
        /// </summary>
        /// <returns></returns>
        public static List<JobBase> GetAfterRequestJobs()
        {
            if (s_AfterRequestJobs == null)
            {
                List<JobBase> beforeRequestJobs = new List<JobBase>();

                foreach (JobBase job in s_AllJobs)
                {
                    if (job.ExecuteType == ExecuteType.AfterRequest)
                        beforeRequestJobs.Add(job);
                }

                s_AfterRequestJobs = beforeRequestJobs;
            }
            return s_AfterRequestJobs;
        }

        /// <summary>
        /// 获得所有在线程中执行的Job
        /// </summary>
        /// <returns></returns>
        public static List<JobBase> GetInThreadJobs()
        {
            if (s_InThreadJobs == null)
            {
                List<JobBase> inThreadJobs = new List<JobBase>();

                foreach (JobBase job in s_AllJobs)
                {
                    if (job.ExecuteType == ExecuteType.InThread)
                        inThreadJobs.Add(job);
                }

                s_InThreadJobs = inThreadJobs;
            }
            return s_InThreadJobs;
        }

        #endregion

        #region 后台线程运行的任务相关的方法

        public static void StartJobThread()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(InThreadJobWorker));
            thread.Start();
            thread.IsBackground = true;
        }

        private static void InThreadJobWorker()
        {
            while (true)
            {
                try
                {
                    ExecuteInThreadJobs();
                }
                catch
                { }

                System.Threading.Thread.Sleep(100);
            }
        }

        #endregion

        #region 执行三种类型的任务

        /// <summary>
        /// 执行任务 自动检查 当前时间需要执行的任务 并执行
        /// </summary>
        /// <param name="executeType"></param>
        public static void ExecuteBeforeRequestJobs()
        {
            foreach (JobBase job in GetBeforeRequestJobs())
            {
                if (job.Enable == false)
                    continue;

                if (IsExecuteTime(job))
                {
                    ExecuteJob(job);
                }
            }
        }

        /// <summary>
        /// 执行任务 自动检查 当前时间需要执行的任务 并执行
        /// </summary>
        /// <param name="executeType"></param>
        public static void ExecuteAfterRequestJobs()
        {
            foreach (JobBase job in GetAfterRequestJobs())
            {
                if (job.Enable == false)
                    continue;

                if (IsExecuteTime(job))
                    ExecuteJob(job);
            }
        }

        public static void ExecuteInThreadJobs()
        {
            foreach (JobBase job in GetInThreadJobs())
            {
                if (job.Enable == false)
                    continue;

                if (IsExecuteTime(job))
                    ExecuteJob(job);
            }
        }

        #endregion

        #region 当前是否有需要马上执行的页面请求后类型的任务

        /// <summary>
        /// 当前是否有需要马上执行的页面请求后类型的任务
        /// </summary>
        /// <param name="executeType"></param>
        /// <returns></returns>
        public static bool IsAfterRequestJobsExecuteTime()
        {
            //每30秒检查一次
            if (lastCheckTime_AfterRequest.AddSeconds(30) >= DateTimeUtil.Now)
            {
                return false;
            }

            lastCheckTime_AfterRequest = DateTimeUtil.Now;

            foreach (JobBase job in GetAfterRequestJobs())
            {
                if (job.Enable == false)
                    continue;

                if (IsExecuteTime(job))
                    return true;

            }
            return false;
        }

        #endregion

        #region 检查某个任务是否到了可以执行的时间

        /// <summary>
        /// 检查某个人物是否到了可以执行的时间
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        private static bool IsExecuteTime(JobBase job)
        {
            DateTime executeTime;
            switch (job.TimeType)
            {
                case TimeType.Interval:
                    if (job.LastExecuteTime.AddSeconds(job.IntervalSeconds) <= DateTimeUtil.Now)
                        return true;
                    break;

                case TimeType.Hour:
                    executeTime = new DateTime(job.LastExecuteTime.Year, job.LastExecuteTime.Month, job.LastExecuteTime.Day, job.LastExecuteTime.Hour, job.ExecuteTime.Minutes, job.ExecuteTime.Seconds);
                    if (executeTime.AddHours(1) <= DateTimeUtil.Now)
                        return true;
                    break;

                case TimeType.Day:
                    executeTime = new DateTime(job.LastExecuteTime.Year, job.LastExecuteTime.Month, job.LastExecuteTime.Day, job.ExecuteTime.Hours, job.ExecuteTime.Minutes, job.ExecuteTime.Seconds);
                    if (executeTime.AddDays(1) <= DateTimeUtil.Now)
                        return true;
                    break;

                case TimeType.Week:
                    //job.LastExecuteTime.DayOfWeek
                    //executeTime = new DateTime(job.LastExecuteTime.Year, job.LastExecuteTime.Month, job.LastExecuteTime.Day, job.ExecuteTime.Hours, job.ExecuteTime.Minutes, job.ExecuteTime.Seconds);
                    if (DateTimeUtil.Now.DayOfWeek == job.DayOfWeek)
                    {
                        if (job.LastExecuteTime.Year == DateTimeUtil.Now.Year && job.LastExecuteTime.Month == DateTimeUtil.Now.Month && job.LastExecuteTime.Day == DateTimeUtil.Now.Day)
                        { }
                        else
                        {
                            executeTime = new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day, job.ExecuteTime.Hours, job.ExecuteTime.Minutes, job.ExecuteTime.Seconds);
                            if (executeTime <= DateTimeUtil.Now)
                                return true;
                        }
                    }
                    break;

                case TimeType.Month:
                    executeTime = new DateTime(job.LastExecuteTime.Year, job.LastExecuteTime.Month, job.ExecuteTime.Days, job.ExecuteTime.Hours, job.ExecuteTime.Minutes, job.ExecuteTime.Seconds);
                    if (executeTime.AddMonths(1) <= DateTimeUtil.Now)
                        return true;
                    break;

                case TimeType.Year:
                    executeTime = new DateTime(job.LastExecuteTime.Year, job.Month, job.ExecuteTime.Days, job.ExecuteTime.Hours, job.ExecuteTime.Minutes, job.ExecuteTime.Seconds);
                    if (executeTime.AddYears(1) <= DateTimeUtil.Now)
                        return true;
                    break;
            }
            return false;
        }

        #endregion

        #region 执行某个任务

        /// <summary>
        /// 执行某个任务
        /// </summary>
        /// <param name="job"></param>
        private static void ExecuteJob(JobBase job)
        {
            lock (job.Locker)
            {
                if (job.isExecuteing)
                    return;
                job.isExecuteing = true;
            }



            try
            {
                job.Action();

                //job.Action();
                switch (job.TimeType)
                {
                    //间隔时间类型 上次执行时间 始终记录 执行时候的时间
                    case TimeType.Interval:
                        job.LastExecuteTime = DateTimeUtil.Now;
                        if (job.IntervalSeconds > 10 * 60)
                        {
                            RecordExecuteTime(job.Type, job.LastExecuteTime);
                        }
                        break;

                    default: job.LastExecuteTime = DateTimeUtil.Now;
                        RecordExecuteTime(job.Type, job.LastExecuteTime);
                        break;
                }
            }
            catch(Exception ex)
            {
                try
                {
                    LogHelper.CreateErrorLog(ex);
                }
                catch { }
            }

            job.isExecuteing = false;
        }

        #endregion

        #region 记录某个任务的执行时间

        private static void RecordExecuteTime(string jobType, DateTime time)
        {
            try
            {
                JobDao.Instance.SetJobLastExecuteTime(jobType, time);
            }
            catch { }
        }

        #endregion

        #region 设置每个JOB上次执行时间

        /// <summary>
        /// 设置每个JOB上次执行时间
        /// </summary>
        public static void SetJobsLastExecuteTime()
        {
            Dictionary<string, DateTime> times = JobDao.Instance.GetAllJobStatus();
            foreach (JobBase job in s_AllJobs)
            {
                DateTime time;
                if (times.TryGetValue(job.Type, out time))
                {
                    job.LastExecuteTime = time;
                }
                else
                    job.LastExecuteTime = DateTime.MinValue;
            }
        }

        #endregion

    }
}