//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Settings;
//using zzbird.Common.Permissions;
//using zzbird.Common.Users;

namespace MaxLabs.bbsMax.Common
{
    public class CommonUtil
    {

        private static Regex regex_FileNameFilter = new Regex("[<>/\";#$*%]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex regex_ImgFormat = new Regex(@"\.(gif|jpg|bmp|png|jpeg)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool IsImage(string fileName)
        {
            if (regex_ImgFormat.Match(fileName).Success)
            {
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// 判断字符串是否包含危及到SQL注入的非法字符,包含的话返回true
        ///// </summary>
        //public static bool IsIllegalString(string str)
        //{
        //    if (regex_FileNameFilter.IsMatch(str))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //判断(E文 数字 下划线)
        static Regex regex_isUserName = new Regex("^[a-zA-Z\\d_]+$", RegexOptions.Compiled);
        public static bool IsUserName(string str)
        {
            return regex_isUserName.IsMatch(str);
        }
        //判断(E文 数字 中文 下划线)
        static Regex regex_isNickName = new Regex(@"^[a-zA-Z\u4e00-\u9fa5\d_]+$", RegexOptions.Compiled);
        public static bool IsNickName(string str)
        {
            return regex_isNickName.IsMatch(str);
        }

        //判断组名(不允许/\<>{}:*?|")
        static Regex regex_CheckGroupName = new Regex(@"^[^\/""{}<>:?*|]+$", RegexOptions.Compiled);
        public static bool IsGroupName(string str)
        {
            return regex_CheckGroupName.IsMatch(str);
        }

        //public static bool IsBanUserName(string userName)
        //{
        //    CreateUserSetting createUserSetting = SettingHelper.CreateUserSetting;
        //    string banUserName = createUserSetting.BanUserName;
        //    banUserName = banUserName.Replace("\n", "");
        //    string[] list = banUserName.Split('\r');

        //    bool isMatch = false;
        //    string regex;
        //    foreach (string str in list)
        //    {
        //        if (str.Trim() == "")
        //            continue;
        //        regex = str;
        //        regex = regex.Replace(".", "\\.");      //.做普通字符处理
        //        regex = regex.Replace("*", ".*");       //*做任意多个任意字符处理
        //        regex = "^" + regex + "$";         //加入正则头尾判断
        //        if (Regex.IsMatch(userName, regex))
        //        {
        //            isMatch = true;
        //            break;
        //        }
        //    }
        //    if (isMatch)
        //        return true;
        //    else
        //        return false;
        //}

        //public static bool IsBanNickName(string nickName)
        //{
        //    CreateUserSetting createUserSetting = SettingHelper.CreateUserSetting;
        //    string bannickName = createUserSetting.BanNickName;
        //    bannickName = bannickName.Replace("\n", "");
        //    string[] list = bannickName.Split('\r');

        //    bool isMatch = false;
        //    string regex;
        //    foreach (string str in list)
        //    {
        //        if (str.Trim() == "")
        //            continue;
        //        regex = str;
        //        regex = regex.Replace(".", "\\.");      //.做普通字符处理
        //        regex = regex.Replace("*", ".*");       //*做任意多个任意字符处理
        //        regex = "^" + regex + "$";         //加入正则头尾判断
        //        if (Regex.IsMatch(nickName, regex))
        //        {
        //            isMatch = true;
        //            break;
        //        }
        //    }
        //    if (isMatch)
        //        return true;
        //    else
        //        return false;
        //}

        //public static bool IsFilterEmail(string email)
        //{
        //    CreateUserSetting createUserSetting = SettingHelper.CreateUserSetting;
        //    string filterEmail = createUserSetting.FilterEmailDomain;
        //    filterEmail = filterEmail.Replace("\n", "");
        //    string[] list = filterEmail.Split('\r');

        //    bool isMatch = false;
        //    string regex;
        //    foreach (string str in list)
        //    {
        //        if (str.Trim() == "")
        //            continue;
        //        regex = str;
        //        regex = regex.Replace(".", "\\.");      //.做普通字符处理
        //        regex = regex.Replace("*", ".*");       //*做任意多个任意字符处理
        //        regex = "^" + regex + "$";         //加入正则头尾判断
        //        if (Regex.IsMatch(email, regex))
        //        {
        //            isMatch = true;
        //            break;
        //        }
        //    }
        //    if (isMatch)
        //        return true;
        //    else
        //        return false;
        //}


        public static string FormatIP(string ip, int fields)
        {
            if (string.IsNullOrEmpty(ip))
                return "(未记录)";

            if (fields > 3)
                return ip;
            else
            {
                if (ip.Contains(":"))
                    return "(不支持ipv6)";

                string[] ipItems = ip.Split('.');
                if (ipItems.Length != 4)
                    return "(未记录)";

                if (fields == 3)
                    return ipItems[0] + "." + ipItems[1] + "." + ipItems[2] + ".*";
                else if (fields == 2)
                    return ipItems[0] + "." + ipItems[1] + ".*.*";
                else if (fields == 1)
                    return ipItems[0] + ".*.*.*";
                else
                    return "*.*.*.*";
            }


        }

        public static string FormatMoney(decimal money)
        {
            return money.ToString("0.00");
        }

        public static string FormatRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("mms://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/"))
                return path;
            else
                return UrlUtil.JoinUrl(Globals.AppRoot, path);
        }

        public static string FormatCommon(string path)
        {
            //if (string.IsNullOrEmpty(path))
            //    return "";
            //if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            //    path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            //    path.StartsWith("mms://", StringComparison.OrdinalIgnoreCase) ||
            //    path.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase) ||
            //    path.StartsWith("/"))
            //    return path;
            //else
            //    return Globals.ApplicationUrl + Const.CommonPath + path;
            return "";
        }

        //public static string FormatCommonSounds(string path)
        //{
        //    if (string.IsNullOrEmpty(path))
        //        return string.Empty;
        //    if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
        //        path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
        //        path.StartsWith("mms://", StringComparison.OrdinalIgnoreCase) ||
        //        path.StartsWith("rtsp://", StringComparison.OrdinalIgnoreCase) ||
        //        path.StartsWith("/"))
        //        return path;
        //    else
        //        return Globals.ApplicationUrl + Const.CommonSoundPath + path;
        //}



        public static string GetDateFormatString(DateFormat dateFormat)
        {
            switch (dateFormat)
            {
                case DateFormat.MMdd:
                    return "MM\\/dd";

                case DateFormat.MMddyyyy:
                    return "MM\\/dd\\/yyyy";

                case DateFormat.yyyyMMdd:
                    return "yyyy\\-MM\\-dd";

                case DateFormat.yyyyMMdd2:
                    return "yyyy\\/MM\\/dd";

                case DateFormat.Default:
                default:
                    return "yyyy\\-MM\\-dd";
            }
        }

        public static string GetTimeFormatString(TimeFormat timeFormat)
        {
            switch (timeFormat)
            {
                case TimeFormat.hhmm:
                    return "HH\\:mm";

                case TimeFormat.hhmmss:
                    return "HH\\:mm\\:ss";

                case TimeFormat.hhmmtt:
                    return "hh\\:mmtt";

                case TimeFormat.hhmmsstt:
                    return "hh\\:mm\\:sstt";

                case TimeFormat.Default:
                default:
                    return "HH\\:mm";
            }
        }

        public static string FormatSize(long byteSize)
        {
            if (byteSize == 0)
                return "0 K";

            string unit = "BKMG";
            if (byteSize >= 1024)
            {
                double size = byteSize;
                int indx = 0;
                do
                {
                    size = size / 1024.00;
                    indx++;
                } while (size >= 1024 && indx < unit.Length - 1);
                return Math.Round(size, 1).ToString() + " " + unit[indx];
            }
            else
                return byteSize.ToString() + " B";
        }

        public static string FormatSizeForSwfUpload(long byteSize)
        {
            if (byteSize == 0)
                return "0 KB";

            string[] unit = new string[] { "B", "K", "MB" };
            if (byteSize >= 1024)
            {
                double size = byteSize;
                int indx = 0;
                do
                {
                    size = size / 1024.00;
                    indx++;
                } while (size >= 1024 && indx < unit.Length - 1);
                return Math.Round(size, 1).ToString() + " " + unit[indx];
            }
            else
                return byteSize.ToString() + " B";
        }

        public static string FormatSecond(long second)
        {
            string str;
            TimeSpan span = TimeSpan.FromSeconds(second);
            if (span.Days > 0)
                str = span.Days.ToString() + "天";
            else
                str = string.Empty;

            if (span.Hours > 0)
                str += span.Hours.ToString() + "时";
            if (span.Minutes > 0)
                str += span.Minutes.ToString() + "分";
            if (span.Seconds > 0)
                str += span.Seconds.ToString() + "秒";

            return str;
        }

        //public static string FormatSize(PermissionByteSize byteSize)
        //{
        //    return FormatSize(byteSize.Value);
        //}

        //public static string HtmlEncode(string text)
        //{
        //    return Bbs3Globals.HtmlEncode(text);
        //}
        //public static string HtmlDecode(string text)
        //{
        //    return Bbs3Globals.HtmlDecode(text);
        //}





        public static StringDictionary ConvertToDictionary(string dictionaryString)
        {
            if (string.IsNullOrEmpty(dictionaryString))
                return new StringDictionary();

            int i = dictionaryString.IndexOf('|');
            if (i < 3)
                return new StringDictionary();

            string[] keyTable = dictionaryString.Substring(0, i).Split(';');

            i++;

            int j;
            int length;
            StringDictionary dictionary = new StringDictionary();

            try
            {
                foreach (string item in keyTable)
                {
                    j = item.IndexOf(':');
                    if (j != -1)
                    {
                        length = Convert.ToInt32(item.Substring(j + 1));
                        dictionary.Add(item.Substring(0, j), dictionaryString.Substring(i, length));

                        i += length;
                    }
                }
            }
            catch { }
            return dictionary;
        }

        public static string ConvertToString(StringDictionary dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
                return string.Empty;

            StringBuilder indexStringBuilder = new StringBuilder();
            StringBuilder valueStringBuilder = new StringBuilder();
            string value;
            bool isFirst = true;
            foreach (DictionaryEntry item in dictionary)
            {
                if (isFirst)
                    isFirst = false;
                else
                    indexStringBuilder.Append(";");


                value = item.Value.ToString();

                indexStringBuilder.Append(item.Key.ToString());
                indexStringBuilder.Append(":");
                indexStringBuilder.Append(value.Length);

                valueStringBuilder.Append(value);
            }

            indexStringBuilder.Append("|");
            indexStringBuilder.Append(valueStringBuilder.ToString());

            return indexStringBuilder.ToString();
        }

        /// <summary>
        /// 剔除前后空格与\r\n,\
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string JsTrim(string text)
        {
            //string temp = text;
            //temp = temp.Replace("\r", "\\r");
            //temp = temp.Replace("\n", "\\r");
            //temp = temp.Replace("'", "\\'");
            //if(quo)
            //{
            //    temp = temp.Replace("\"", "&#34;");
            //}
            //temp.Trim();
            //return temp;
            StringBuilder builder = new StringBuilder(text);
            builder = builder.Replace("\\", "\\\\");
            builder = builder.Replace("/", "\\/");
            builder = builder.Replace("'", "\\'");
            builder = builder.Replace("\"", "\\\"");
            builder = builder.Replace("\r\n", "\r");
            builder = builder.Replace("\r", "\\r");
            return builder.ToString();
        }

        //public static string JsTrim(string text)
        //{
        //    return JsTrim(text, false);
        //}

        public static string SafeUrl(string url)
        {
            StringBuilder builder = new StringBuilder(url);
            builder = builder.Replace("<", "%3C");
            builder = builder.Replace(">", "%3E");
            builder = builder.Replace("\"", "%22");
            builder = builder.Replace("'", "%27");
            return builder.ToString();
        }

        //private static Regex regex_html = new Regex(@"<.*?>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //public static string ClearHTML(string text)
        //{
        //    if (text == null)
        //        return null;
        //    text = regex_html.Replace(text, "");
        //    return text;
        //}
        const string INSTALLED_KEY = "httpcompress.attemptedinstall";
        static readonly object INSTALLED_TAG = new object();
        public static void SetInstalledKey(HttpContext content)
        {
            if (content.Items.Contains(INSTALLED_KEY))
                content.Items[INSTALLED_KEY] = INSTALLED_TAG;
            else
                content.Items.Add(INSTALLED_KEY, INSTALLED_TAG);
        }
        public static bool ContainsInstalledKey(HttpContext content)
        {
            if (content.Items.Contains(INSTALLED_KEY))
                return true;
            else
                return false;
        }
    }
}