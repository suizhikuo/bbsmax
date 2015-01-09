//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web.Caching;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Max.Installs;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Max.Installs
{
    public sealed class Globals
    {
        private Globals()
        {
        }

        public static Cache Cache
        {
            get
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                    return context.Cache;
                else
                    return HttpRuntime.Cache;
            }
        }

        public static int ToInt32(object value)
        {
            return ToInt32(value, 0);
        }

        public static int ToInt32(object value, int defaultValue)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool ToBoolean(object value)
        {
            return ToBoolean(value, false);
        }

        public static bool ToBoolean(object value, bool defaultValue)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// �ж�textA��textB�Ƿ���ͬ�����Դ�Сд��
        /// </summary>
        public static bool Compare(string textA, string textB)
        {
            return Compare(textA, textB, true);
        }
        /// <summary>
        /// �ж�textA��textB�Ƿ���ͬ��
        /// </summary>
        public static bool Compare(string textA, string textB, bool ignoreCase)
        {
            return string.Compare(textA, textB, ignoreCase) == 0;
        }

        public static string MapPath(string path)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
                return context.Server.MapPath(path);
            else
            {
                path = path.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("~", "");
                return PhysicalPath(path);
            }
        }

        public static string RootPath()
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string newValue = Path.DirectorySeparatorChar.ToString();
            rootPath = rootPath.Replace("/", newValue);
            return rootPath;
        }

        public static string PhysicalPath(string path)
        {
            path =(RootPath().TrimEnd(new char[] { Path.DirectorySeparatorChar }) + Path.DirectorySeparatorChar.ToString() + path.TrimStart(new char[] { Path.DirectorySeparatorChar }));
            return path;
        }

        public static string SafeJS(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Replace("\\", @"\\").Replace("'", @"\'").Replace("\r", @"\r").Replace("\n", @"\n");
        }

        public static DateTime ToDateTime(object timeStamp)
        {
            long timeSatamp = Convert.ToInt64(timeStamp);
            int timezone = Settings.Current.TimeZone;
            DateTime start = new DateTime(1970, 01, 01);
            TimeSpan span = new TimeSpan(timeSatamp * 10000000);
            return start.AddHours(timezone).Add(span);
        }

        public static long ToTimeStamp(DateTime date)
        {
            int timezone = Settings.Current.TimeZone;
            DateTime start = new DateTime(1970, 01, 01);
            start.AddHours(timezone);
            TimeSpan span = date.Subtract(start);
            string timeStamp = span.Ticks.ToString();
            timeStamp = timeStamp.Substring(0, timeStamp.Length - 7);
            return Convert.ToInt64(timeStamp);
        }

        public static string SafeSQL(object value)
        {
            if (value == null || value.ToString() == "")
                return "''";

            Type type = value.GetType();
            if (type == typeof(String))
            {
                return ("'" + value.ToString() + "'");
            }
            else if (typeof(DateTime) == type)
            {
                if ((DateTime)value <= DateTime.MinValue)
                    return "''";
                else
                    return ("'" + value.ToString() + "'");
            }
            else if (type == typeof(Boolean))
            {
                return ((bool)value) ? "1" : "0";
            }
            else if (type == typeof(Enum))
            {
                return ((int)value).ToString();
            }
            else
            {
                return value.ToString();
            }
        }

        public static string SafeString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            else
                return text.Trim();
        }
    }
}