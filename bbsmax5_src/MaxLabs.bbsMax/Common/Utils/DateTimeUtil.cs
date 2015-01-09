//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web.Caching;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax
{
    public class DateTimeUtil
    {
        /// <summary>
        /// 系统当前时间（与数据库时间同步）
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return DateTime.Now.AddMilliseconds(TimeIntervalFromDatabase);
            }
        }

        /// <summary>
        /// 从数据库获取的标准UTC时间
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                return Now.AddHours(-DatabaseTimeDifference);
            }
        }

        public static DateTime SQLMinValue
        {
            get
            {
                return new DateTime(1753, 1, 1, 0, 00, 00);
            }
        }

        /// <summary>
        /// 数据库服务器与标准GMT时间的时差(单位小时)
        /// </summary>
        public static float DatabaseTimeDifference
        {
            get;
            set;
        }

        /// <summary>
        /// 程序当前时间与数据库当前时间的相差毫秒数
        /// </summary>
        private static double? m_TimeIntervalFromDatabase = null;

        public static double TimeIntervalFromDatabase
        {
            get
            {
                return m_TimeIntervalFromDatabase != null ? m_TimeIntervalFromDatabase.Value : 0;
            }
            set
            {
                m_TimeIntervalFromDatabase = value;
            }
        }

        private static DateTimeSettings Setting = AllSettings.Current.DateTimeSettings;

        /// <summary>
        /// 获取类似“几分钟前” 的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetFriendlyDate(DateTime dateTime)
        {
            //获取当前用户和服务器时间的时差
            AuthUser user = User.Current;
            //
            return GetFriendlyDate(user, dateTime);

        }

        /// <summary>
        /// 获取类似“几分钟前” 的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetFriendlyDate(AuthUser user, DateTime dateTime)
        {
            float timeDiff = 0.0f;
            //获取当前用户和服务器时间的时差
            timeDiff = UserBO.Instance.GetUserTimeDiffrence(user);
            //
            return GetFriendlyDateTime(dateTime, timeDiff, false);
        }

        /// <summary>
        /// 输出友好时间
        /// </summary>
        /// <param name="dateTime">服务器时间</param>
        /// <param name="TimeDiffrence">用户所在时区和服务器时区时差</param>
        /// <returns></returns>
        private static string GetFriendlyDateTime(DateTime dateTime, float TimeDiffrence, bool outputTime)
        {

            DateTimeSettings Setting = AllSettings.Current.DateTimeSettings;

            TimeSpan t = (Now - dateTime);

            if (SQLMinValue > dateTime || (DateTime.MaxValue - dateTime).TotalHours < TimeDiffrence || dateTime.AddHours(TimeDiffrence) < SQLMinValue)
                return string.Empty;

            dateTime = dateTime.AddHours(TimeDiffrence);

            if (t.TotalDays >= 3 || t.TotalMinutes < 0)
            {
                return outputTime ? FormatDateTime(dateTime,false) : FormatDate(dateTime);
            }

            if (!Setting.EnableSpoken)
            {
                return FormatDateTime(dateTime);
            }
            
            if (t.TotalSeconds < 10)
            {
                return Lang.Common_Now;
            }
            else if (t.TotalSeconds < 60)
            {
                return string.Format(Lang.Common_SecondAgo, (int)t.TotalSeconds);
            }
            else if (t.TotalMinutes < 60)
            {
                return string.Format(Lang.Common_MineteAgo, (int)t.TotalMinutes);
            }
            else if (t.TotalHours < 24)
            {
                return string.Format(Lang.Common_HourAgo, (int)t.TotalHours);
            }

            string timeString = dateTime.ToString("HH:mm");
            if (Now.Day - dateTime.Day == 1)
                return Lang.Common_Yesterday + timeString;
            else if (Now.Day - dateTime.Day == 2)
                return Lang.Common_BeforeYesterday + timeString;

            return outputTime ? FormatDateTime(dateTime,false) : FormatDate(dateTime);

        }


        public static string GetFriendlyDateTime(AuthUser user, DateTime dateTime)
        {
            float timeDiff = 0.0f;
            //获取当前用户和服务器时间的时差
            timeDiff = UserBO.Instance.GetUserTimeDiffrence(user);
            //
            return GetFriendlyDateTime(dateTime, timeDiff, true);
        }

        /// <summary>
        /// 如果是整数的天 或者 小时 或者 分钟 将转换成对应的单位: 如:"3天"
        /// 0返回 "无限期"
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatSecond(long seconds, TimeUnit timeUnit)
        {

            if (seconds == 0)
                return Lang.Common_Indefinitely;

            int secondOfDay = 24 * 3600;
            int secondOfHour = 3600;
            int secondOfMinute = 60;

            StringBuffer buffer = new StringBuffer();
            if (seconds > secondOfDay)
            {
                int day = (int)(seconds / secondOfDay);
                buffer += day + Lang.Common_Day;
                seconds -= day * secondOfDay;
            }

            if (timeUnit == TimeUnit.Day && buffer.Length > 0) return buffer.ToString();

            if (seconds > secondOfHour)
            {
                int hour;
                hour = (int)seconds / secondOfHour;
                buffer += hour + Lang.Common_Hour;
                seconds -= hour * secondOfHour;
            }

            if (timeUnit == TimeUnit.Hour && buffer.Length > 0) return buffer.ToString();

            if (seconds > secondOfMinute)
            {
                int minute;
                minute = (int)seconds / secondOfMinute;
                buffer += minute + Lang.Common_Minute;
                seconds -= minute * secondOfMinute;
            }

            if (timeUnit == TimeUnit.Minute && buffer.Length > 0) return buffer.ToString();

            if (seconds > 0)
                buffer += seconds + Lang.Common_Second;

            return buffer.ToString();
        }

        public static string FormatSecond(long second)
        {
            return FormatSecond(second, TimeUnit.Second);
        }

        /// <summary>
        /// 检查日期的有效性（那些年月日分开的日期， 有时后面的Day会超出当月最大值）
        /// </summary>
        /// <param name="birthYear"></param>
        /// <param name="birthMonth"></param>
        /// <param name="birthday"></param>
        public static DateTime CheckDateTime(int year, int month, int day)
        {
            if (month <= 0 && month > 12) month = 1;
            if (day <= 0) day = 1;
            if (year < DateTimeUtil.SQLMinValue.Year) year = DateTimeUtil.SQLMinValue.Year;

            if (year > 9999) year = 9999;

            if (month > 0 && month < 13)//天数检查
            {
                int temp = DateTime.DaysInMonth(year <= 0 || year > 9999 ? 2000 : year, month);
                if (day > temp)
                {
                    day = (short)temp;
                }
            }

            return new DateTime(year, month, day);
        }

        /// <summary>
        /// 如果是整数的天 或者 小时 或者 分钟 将转换成对应的单位
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public static long FormatSecond(long seconds, out TimeUnit timeUnit)
        {
            if (seconds == 0)
            {
                timeUnit = TimeUnit.Second;
                return 0;
            }
            else if (seconds % (60 * 60 * 24) == 0)
            {
                timeUnit = TimeUnit.Day;
                return seconds / (60 * 60 * 24);
            }
            else if (seconds % (60 * 60) == 0)
            {
                timeUnit = TimeUnit.Hour;
                return seconds / (60 * 60);
            }
            else if (seconds % 60 == 0)
            {
                timeUnit = TimeUnit.Minute;
                return seconds / 60;
            }
            else
            {
                timeUnit = TimeUnit.Second;
                return seconds;
            }
        }


        /// <summary>
        /// 返回分钟数到 时间的转换
        /// </summary>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static string FormatMinute(int minute)
        {
            if (minute == 0) return "0";
            return FormatSecond(minute * 60);
        }

        public static string FormatMinute(int minute, TimeUnit unit)
        {
            if (minute == 0) return "0";
            return FormatSecond(minute * 60, unit);
        }

        /// <summary>
        /// 输出日期和时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormatDateTime(AuthUser user, DateTime time,bool outputSecond)
        {
            double dateDiff = GetTimeDiffrence(user.TimeZone);
            time = time.AddHours(dateDiff);
            return FormatDateTime(time, outputSecond);
        }


        public static string FormatDateTime(DateTime time)
        {
            return FormatDateTime(time, true);
        }

        /// <summary>
        /// 输出日期和时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime time,bool outSecond)
        {
            if (time.Year == 9999)
                return Lang.Common_Indefinitely;
            else if (time.Year < 1800)
                return "";
            if (outSecond)
                return time.ToString( string.Concat(AllSettings.Current.DateTimeSettings.DateFormat, " " , AllSettings.Current.DateTimeSettings.TimeFormat));
            else
                return time.ToString(string.Concat( AllSettings.Current.DateTimeSettings.DateFormat," ", "HH:mm"));
        }



        /// <summary>
        /// 输出日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FormatDate(DateTime date)
        {
            if (date.Year == 9999)
                return Lang.Common_Indefinitely;
            else if (date.Year < 1800)
                return "";

            return date.ToString(AllSettings.Current.DateTimeSettings.DateFormat);
        }

        /// <summary>
        /// 计算时差
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static double GetTimeDiffrence(float timeZone)
        {
            return timeZone - AllSettings.Current.DateTimeSettings.ServerTimeZone;
        }

        public static long GetSeconds(long time, TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Second: return time;
                case TimeUnit.Minute: return time * 60;
                case TimeUnit.Hour: return time * 60 * 60;
                default: return time * 60 * 60 * 24;
            }
        }

        public static DateTime ParseBeginDateTime(string beginDateTime)
        {
            DateTime result = ConvertStringToDateTime(beginDateTime, false);

            if (result.Year == DateTime.MaxValue.Year || result.Year <= SQLMinValue.Year)
                return result;

            result = result.AddHours(-UserBO.Instance.GetUserTimeDiffrence(User.Current));

            return result;
        }

        public static DateTime ParseEndDateTime(string endDateTime)
        {
            DateTime result = ConvertStringToDateTime(endDateTime, true);

            if (result.Year == DateTime.MaxValue.Year || result.Year <= SQLMinValue.Year)
                return result;

            result = result.AddHours(-UserBO.Instance.GetUserTimeDiffrence(User.Current));

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="datetimeString"></param>
        /// <param name="isEndDateTime"></param>
        /// <param name="considerationTiemDiff">是否考虑时差问题</param>
        /// <returns></returns>
        private static DateTime ConvertStringToDateTime(string datetimeString, bool isEndDateTime)
        {
            string[] englishMonth = new string[] { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };

            string patternNumeric = @"^\d{1,2}(?=\D|$)"
                   , patternPlace = @"^\D*\d{1,2}\D*|$"
                   , patternYear = @"(?>(?<=\D|^))\d{4}(?>(?=\D|$))";

            int year
                , month = -1
                , day = 0
                , hour
                , minute
                , secound
                , temp;

            bool isEnglishMonth = false;

            if (isEndDateTime)
            {
                month = 12;
                hour = 23;
                minute = 59;
                secound = 59;
            }
            else
            {
                month = 1;
                day = 1;
                hour = 0;
                minute = 0;
                secound = 0;
            }

            DateTime returnValue;

            if (string.IsNullOrEmpty(datetimeString))
            {
                return isEndDateTime ? DateTime.MaxValue : DateTime.MinValue;
            }

            datetimeString = datetimeString.Trim().ToLower();

            if (!isEndDateTime)
            {
                if (DateTime.TryParse(datetimeString, out returnValue)) { return returnValue; } //如果可以直接转的话就用.NET直接转换
            }

            if (int.TryParse(Regex.Match(datetimeString, patternYear).Value, out year))
            {
                for (int i = 0; i < englishMonth.Length; i++)
                {
                    if (datetimeString.Contains(englishMonth[i]))
                    {
                        month = i + 1;
                        isEnglishMonth = true;
                        datetimeString = Regex.Replace(datetimeString, englishMonth[i] + @"\D*", "");
                        break;
                    }
                }
                datetimeString = Regex.Replace(datetimeString, year + @"\D*", "");
            }
            else
            {
                if (isEndDateTime)
                    return DateTime.MaxValue;
                else
                    return DateTime.MinValue;
                // year = DateTimeUtil.Now.Year;
                //throw new FormatException("无效的日期格式");
            }

            if (!isEnglishMonth)
            {
                if (int.TryParse(Regex.Match(datetimeString, patternNumeric).Value, out temp))
                {
                    month = temp;
                    datetimeString = Regex.Replace(datetimeString, patternPlace, "");
                }
            }

            if (int.TryParse(Regex.Match(datetimeString, patternNumeric).Value, out temp))
            {
                day = temp;
                datetimeString = Regex.Replace(datetimeString, patternPlace, "");
            }
            else
            {
                if (isEndDateTime) day = DateTime.DaysInMonth(year, month);
            }

            if (int.TryParse(Regex.Match(datetimeString, patternNumeric).Value, out temp))
            {
                hour = temp;
                datetimeString = Regex.Replace(datetimeString, patternPlace, "");
            }

            if (int.TryParse(Regex.Match(datetimeString, patternNumeric).Value, out temp))
            {
                minute = temp;
                datetimeString = Regex.Replace(datetimeString, patternPlace, "");
            }

            if (int.TryParse(Regex.Match(datetimeString, patternNumeric).Value, out temp))
            {
                secound = temp;
            }

            DateTime dtResult = new DateTime(year, month, day, hour, minute, secound, isEndDateTime ? 998 : 0);

            return dtResult;
        }

        /// <summary>
        /// 获取本周1的日期  时间为0点
        /// </summary>
        /// <returns></returns>
        public static DateTime GetMonday()
        {
            int dayOfWeek = (int)Now.DayOfWeek;
            DateTime monday;
            if(dayOfWeek == 0)
                monday = Now.AddDays(-6);
            else
                monday = Now.AddDays(1 - dayOfWeek);

            monday = new DateTime(monday.Year, monday.Month, monday.Day);

            return monday;
        }
    }
}