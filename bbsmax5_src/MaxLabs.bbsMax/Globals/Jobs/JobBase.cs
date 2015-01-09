//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;

namespace MaxLabs.bbsMax.Jobs
{
    /// <summary>
    /// 任务 类型
    /// </summary>
    public abstract class JobBase
    {
        public JobBase()
        {
            bool hasError = false;

            switch (TimeType)
            {
                case TimeType.Interval:
                    hasError = HasError<long?>(ref m_IntervalSeconds);
                    break;

                case TimeType.Hour:
                case TimeType.Day:
                    hasError = HasError<TimeSpan?>(ref m_ExecuteTime);
                    break;

                case TimeType.Week:
                    hasError = HasError<DayOfWeek?>(ref m_DayOfWeek);
                    if(hasError == false)
                        hasError = HasError<TimeSpan?>(ref m_ExecuteTime);
                    break;

                case TimeType.Month:
                    hasError = HasError<TimeSpan?>(ref m_ExecuteTime);
                    break;

                case TimeType.Year:
                    hasError = HasError<int?>(ref m_Month);
                    if (hasError == false)
                        hasError = HasError<TimeSpan?>(ref m_ExecuteTime);
                    break;

                default: break;

            }

            if (hasError)
            {
                throw new Exception(@"任务“" + Type + @"”还未设置执行时间或者执行时间不合法；
该任务执行时间类型是：" + TimeType.ToString() + "，请检查SetTime()方法。");
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get { return this.GetType().Name; } }

        public object Locker = new object();

        public bool isExecuteing = false;

        private DateTime? m_LastExecuteTime;
        /// <summary>
        /// 上次执行时间
        /// </summary>
        public DateTime LastExecuteTime 
        {
            get 
            {
                if (m_LastExecuteTime == null)
                {
                    JobManager.SetJobsLastExecuteTime();
                }
                return m_LastExecuteTime.Value;
            }

            set { m_LastExecuteTime = value; } 
        }

        /// <summary>
        /// 页面请求前执行，还是请求后执行
        /// </summary>
        public abstract ExecuteType ExecuteType { get; }

        /// <summary>
        /// 执行时间类型
        /// </summary>
        public abstract TimeType TimeType { get; }

        /// <summary>
        /// 任务是否启用
        /// </summary>
        public abstract bool Enable { get; }


        /// <summary>
        /// 设置执行时间
        /// TimeType为Interval时 SetTime()里面调用SetIntervalSeconds(long seconds)；
        /// TimeType为Hour    时 SetTime()里面调用SetHourExecuteTime(int minutes,int seconds)；
        /// TimeType为Day     时 SetTime()里面调用SetDayExecuteTime(int hours, int minutes, int seconds)；
        /// TimeType为Week    时 SetTime()里面调用SetWeekExecuteTime(DayOfWeek dayOfWeek,int hours, int minutes, int seconds)；
        /// TimeType为Month   时 SetTime()里面调用SetMonthExecuteTime(int day,int hours, int minutes, int seconds)；
        /// TimeType为Year    时 SetTime()里面调用SetYearExecuteTime(int month,int day, int hours, int minutes, int seconds)；
        /// </summary>
        protected abstract void SetTime();

        /// <summary>
        /// 该任务要执行的动作
        /// </summary>
        public abstract void Action();


        private long? m_IntervalSeconds;
        /// <summary>
        /// 间隔时间执行  单位秒
        /// </summary>
        public long IntervalSeconds
        {
            get
            {
                if (m_IntervalSeconds == null)
                {
                    SetTime();
                }
                if (m_IntervalSeconds == null)
                    return 0;
                else
                    return m_IntervalSeconds.Value;
            }
        }



        private TimeSpan? m_ExecuteTime;
        public TimeSpan ExecuteTime
        {
            get
            {
                if (m_ExecuteTime == null)
                {
                    SetTime();
                }
                if (m_ExecuteTime == null)
                    return new TimeSpan();
                else
                    return m_ExecuteTime.Value;
            }
        }
    

        private DayOfWeek? m_DayOfWeek;

        public DayOfWeek DayOfWeek
        {
            get
            {
                if (m_DayOfWeek == null)
                {
                    SetTime();
                }
                return m_DayOfWeek.Value;
            }
        }

        private int? m_Month;
        public int Month
        {
            get
            {
                if (m_Month == null)
                {
                    SetTime();
                }
                return m_Month.Value;
            }
        }

        private bool HasError<T>(ref T obj)
        {
            if (obj == null)
                SetTime();

            return obj == null;
        }


        /// <summary>
        /// TimeType为Interval时 在SetTime()中请调用此方法 设置执行时间
        /// </summary>
        /// <param name="seconds"></param>
        protected void SetIntervalExecuteTime(long seconds)
        {
            m_IntervalSeconds = seconds;
        }

        /// <summary>
        /// TimeType为Hour时 在SetTime()中请调用此方法 设置执行时间
        /// </summary>
        protected void SetHourExecuteTime(int minutes,int seconds)
        {
            if (minutes > 59)
                minutes = 59;
            if (seconds > 59)
                seconds = 59;

            m_ExecuteTime = new TimeSpan(0, minutes, seconds);
        }

        /// <summary>
        /// TimeType为Day时 在SetTime()中请调用此方法 设置执行时间
        /// </summary>
        protected void SetDayExecuteTime(int hours, int minutes, int seconds)
        {
            if (hours > 23)
                hours = 23;
            if (minutes > 59)
                minutes = 59;
            if (seconds > 59)
                seconds = 59;

            m_ExecuteTime = new TimeSpan(hours, minutes, seconds);
        }

        /// <summary>
        /// TimeType为Week时 在SetTime()中请调用此方法 设置执行时间
        /// </summary>
        protected void SetWeekExecuteTime(DayOfWeek dayOfWeek,int hours, int minutes, int seconds)
        {
            m_DayOfWeek = dayOfWeek;
            SetDayExecuteTime(hours,minutes,seconds);
        }

        /// <summary>
        /// TimeType为Month时 在SetTime()中请调用此方法 设置执行时间
        /// </summary>
        protected void SetMonthExecuteTime(int day,int hours, int minutes, int seconds)
        {
            if (day > 31)
                day = 31;
            if (hours > 23)
                hours = 23;
            if (minutes > 59)
                minutes = 59;
            if (seconds > 59)
                seconds = 59;

            m_ExecuteTime = new TimeSpan(day, hours, minutes, seconds);
        }

        /// <summary>
        /// TimeType为Year时 在SetTime()中请调用此方法 设置执行时间
        /// </summary>
        protected void SetYearExecuteTime(int month,int day, int hours, int minutes, int seconds)
        {
            if (month > 12)
                month = 12;
            if (day > 31)
                day = 31;
            if (hours > 23)
                hours = 23;
            if (minutes > 59)
                minutes = 59;
            if (seconds > 59)
                seconds = 59;

            m_Month = month;
            m_ExecuteTime = new TimeSpan(day, hours, minutes, seconds);
        }
    }

    /// <summary>
    /// 任务执行时间类型
    /// </summary>
    public enum TimeType
    {
        /// <summary>
        /// 间隔一定时间执行
        /// </summary>
        Interval = 0,

        /// <summary>
        /// 每个小时里的固定时间执行
        /// </summary>
        Hour = 1,

        /// <summary>
        /// 每天固定时间执行
        /// </summary>
        Day = 2,

        /// <summary>
        /// 每周的固定时间执行
        /// </summary>
        Week = 3,

        /// <summary>
        /// 每月的固定时间执行
        /// </summary>
        Month = 4,

        /// <summary>
        /// 每年的固定时间执行
        /// </summary>
        Year = 5,
    }

    public enum ExecuteType
    { 
        /// <summary>
        /// 页面请求前执行
        /// </summary>
        BeforeRequest,


        /// <summary>
        /// 页面请求后执行，通过在页面底部自动输出隐藏的iframe来执行
        /// </summary>
        AfterRequest,

        /// <summary>
        /// 在线程里执行
        /// </summary>
        InThread,

    }
}