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

namespace MaxLabs.bbsMax.Entities
{

    public class ScopeBase : SettingBase, IPrimaryKey<Guid>
    {
        public enum ScopeType
        { 
            ScopeBase=0,

            DaysSope,

            WeekScope,

            MonthScope
        }

        public ScopeBase()
        { 
            BeginTime = null;
            EndTime = null;

            InitBeginEnd();
            OperetingDatetime = DateTime.Now;
            Type = ScopeType.ScopeBase;
        }

        [SettingItem]
        public ScopeType Type
        {
            get;
            set;
        }

        public ScopeBase(DateTime? begin, DateTime? end)
        {
            BeginTime = begin;
            EndTime = end;


            InitBeginEnd();
            OperetingDatetime = DateTime.Now;
        }

        private void InitBeginEnd()
        {
            if (BeginTime == null || EndTime == null)
            {
                BeginTime = DateTime.MinValue;
                EndTime = DateTime.MinValue;
            }
            else
            {
                EndTime = EndTime.Value.AddDays(BeinEndDiff);
            }
        }

        
        private Guid m_ID;
        [JsonItem]
        [SettingItem]
        public Guid ID
        {
            get
            {
                if (m_ID == Guid.Empty)
                {
                    m_ID = Guid.NewGuid();
                }
                return m_ID;
            }
            set { m_ID = value; }
        }

        protected string m_Message;
        [JsonItem]
        public virtual string Message 
        {
            get
            {
                if(string.IsNullOrEmpty(m_Message))
                    m_Message = string.Format("每天 {0}", Description());
                return m_Message;
            }
            set
            {
                m_Message = value;
            }
        }

        protected virtual string Description()
        {
            if (BeginTime != DateTime.MinValue && EndTime != DateTime.MinValue)
                return string.Format("{0:D2}:{1:D2}-{2:D2}:{3:D2}", BeginTime.Value.Hour, BeginTime.Value.Minute,EndTime.Value.Hour,EndTime.Value.Minute);
            return string.Empty;
        }

        [JsonItem]
        [SettingItem]
        public string OperetorName { get; set; }

        [JsonItem]
        [SettingItem]
        public DateTime OperetingDatetime
        {
            get;
            set;
        }

        [SettingItem]
        public DateTime? BeginTime { get; set; }

        [SettingItem]
        public DateTime? EndTime { get; set; }

        private int BeinEndDiff
        {
            get { return IsSpanZero(); }
        }

        //如果时间段跨越0点,则返回1,否则返回0.
        private int IsSpanZero()
        {
           if (BeginTime > EndTime)
                    return 1;

            return 0;
        }

        //不指定范围,表示范围为全天.
        

        /// <summary>
        /// 如果输入时间在指定时间范围内,则返回true
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public virtual bool CompareDatetime(DateTime dt)
        {
            
            if(BeginTime==DateTime.MinValue||EndTime==DateTime.MinValue)
                return true;

 //           double beginDiff = (dt - BeginTime.Value).TotalMinutes;
 //           double endDiff = (dt-EndTime.Value).TotalMinutes;

            if (dt > BeginTime.Value && dt < EndTime.Value)
            {
                return true;
            }

            return false;
        }


        #region IPrimaryKey<Guid> 成员

        public Guid GetKey()
        {
            return this.ID;
        }

        #endregion
    }

    public class DayScope : ScopeBase
    {
        public DayScope() 
        {
            Type = ScopeType.DaysSope;
        }

        public DayScope(DateTime dt)
        {
            DefineDay = dt;
            Type = ScopeType.DaysSope;
        }

        public DayScope(DateTime? ts1, DateTime? ts2): base(ts1, ts2)
        {
            Type = ScopeType.DaysSope;
        }

        public DayScope(DateTime dt, DateTime? ts1, DateTime? ts2)
            : base(ts1, ts2)
        {
            DefineDay = dt;
            Type = ScopeType.DaysSope;
        }

        [JsonItem]
        public override string Message
        {
            get
            {
                return string.Format("{0} {1}", DefineDay.ToString("yyyy年MM月dd日"), base.Description());
            }
        }

        [SettingItem]
        public DateTime DefineDay { get; set; }

        private bool IsDefineDay(DateTime dt)
        {
            if (dt.Date == DefineDay.Date)
            {
                return true;
            }

            return false;
        }

        public override bool CompareDatetime(DateTime dt)
        {
            if (IsDefineDay(dt))
            {
                return base.CompareDatetime(dt);
            }
            return false;
        }
    }

    public class WeekScope : ScopeBase
    {
        public WeekScope()
        {
            Type = ScopeType.WeekScope;
        }

        public WeekScope(string week, DateTime? ts1, DateTime? ts2)
            : base(ts1, ts2)
        {
            Type = ScopeType.WeekScope;
            AddWeekDay(week);
        }

        public WeekScope(DateTime? ts1, DateTime? ts2)
            : base(ts1, ts2)
        {
            Type = ScopeType.WeekScope;
        }

       
        [JsonItem]
        [SettingItem]
        public override string Message
        {
            get
            {
                if(string.IsNullOrEmpty(m_Message))
                    m_Message=string.Format("{0} {1}",WeekDayDesc(),base.Description());
                return m_Message;
            }
            set
            {
                m_Message = value;
            }
        }

        public string WeekDayDesc()
        {
            string message="每周";
            foreach (int day in WeekDay)
            {
                message += WeekDayName(day) + ",";
            }
            return message.Substring(0,message.Length-1);
        }

        private string WeekDayName(int day)
        {
            switch (day)
            { 
                case 1:
                    return "周一";
                case 2:
                    return "周二";
                case 3:
                    return "周三";
                case 4:
                    return "周四";
                case 5:
                    return "周五";
                case 6:
                    return "周六";
                case 0:
                    return "周日";
                default:
                    return string.Empty;    
            }       
        }

        private List<int> m_WeekDay;

        [SettingItem]
        public List<int> WeekDay 
        {
            get 
            {
                if (m_WeekDay == null)
                    m_WeekDay = new List<int>();
                return m_WeekDay;
            }
            set { m_WeekDay = value; }
            
        }

        public void AddWeekDay(string week)
        {
            if (week.IndexOf(',') > -1)
            {
                string[] daylist = week.Split(',');
                for (int i = 0; i < daylist.Length; i++)
                {
                    if (false==string.IsNullOrEmpty(daylist[i]))
                        WeekDay.Add(int.Parse(daylist[i]));
                }
            }
            else
            {
                if (false==string.IsNullOrEmpty(week))
                    WeekDay.Add(int.Parse(week));
            }
        }

        private bool IsInWeekDay(DateTime dt)
        {
            int[] list = WeekDay.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == (int)dt.DayOfWeek)
                {
                    return true;
                }
                    
            }
                
            return false;
        }
        public override bool CompareDatetime(DateTime dt)
        {
            if (IsInWeekDay(dt))
            {
                return base.CompareDatetime(dt);
            }

            return false;
        }

    }

    public class MonthScope : ScopeBase
    {
        public MonthScope()
        {
            Type = ScopeType.MonthScope;
        }

        public MonthScope(string monthday, DateTime? ts1, DateTime? ts2)
            : base(ts1, ts2)
        {
            Type = ScopeType.MonthScope;
            AddMonthDay(monthday);
        }

        public MonthScope(DateTime ts1, DateTime ts2)
            : base(ts1, ts2)
        {
            Type = ScopeType.MonthScope;
        }

        [JsonItem]
        [SettingItem]
        public override string Message
        {
            
            get
            {
                if(string.IsNullOrEmpty(m_Message))
                    m_Message=string.Format("{0} {1}", MonthDayDesc(), base.Description());
                return m_Message;
            }
            set
            {
                m_Message = value;
            }
           
        }

        private string MonthDayDesc()
        { 
            string message="每月";
            int[] daylist = MonthDay.ToArray();
            for (int i = 0; i < daylist.Length; i++)
            {
                message += daylist[i].ToString() + "号,";
            }

            return message.Substring(0, message.Length - 1);
        }

        
        private List<int> m_MonthDay;
        [SettingItem]
        public List<int> MonthDay 
        {
            get 
            {
                if (m_MonthDay == null)
                    m_MonthDay = new List<int>();
                return m_MonthDay;
            }
            set { m_MonthDay = value; }
        }

        public void AddMonthDay(string monthday)
        {
            if (monthday.IndexOf(',') > -1)
            {
                string[] daylist = monthday.Split(',');
                for (int i = 0; i < daylist.Length; i++)
                {
                    if (false == string.IsNullOrEmpty(daylist[i]))
                        MonthDay.Add(int.Parse(daylist[i]));
                }
            }
            else
            {
                if (false == string.IsNullOrEmpty(monthday))
                    MonthDay.Add(int.Parse(monthday));
            }
        }

        private bool IsInMonthDay(DateTime dt)
        {
            int[] list = MonthDay.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == dt.Day)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool CompareDatetime(DateTime dt)
        {
            if (IsInMonthDay(dt))
            {
                return base.CompareDatetime(dt);
            }

            return false;
        }
        
    }

    public class ScopeBaseCollection : EntityCollectionBase<Guid, ScopeBase>, ISettingItem
    {
        public ScopeBaseCollection() { }

        public bool CompareDateTime(DateTime dt)
        {
            foreach (ScopeBase item in this)
            {
                if (item.CompareDatetime(dt))
                    return true;
            }

            return false;
        }



        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();
            foreach (ScopeBase item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);
            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    ScopeBase scope = new ScopeBase();
                    scope.Parse(item);
                    if (scope.Type == ScopeBase.ScopeType.DaysSope)
                    {
                        DayScope dayScope = new DayScope();
                        dayScope.Parse(item);
                        this.Add(dayScope);
                    }
                    else if (scope.Type == ScopeBase.ScopeType.WeekScope)
                    {
                        WeekScope weekScope = new WeekScope();
                        weekScope.Parse(item);
                        this.Add(weekScope);
                    }
                    else if (scope.Type == ScopeBase.ScopeType.MonthScope)
                    {
                        MonthScope monthScope = new MonthScope();
                        monthScope.Parse(item);
                        this.Add(monthScope);
                    }
                    else
                    {
                        this.Add(scope);
                    }
                }
            }
        }

        #endregion
    }
}