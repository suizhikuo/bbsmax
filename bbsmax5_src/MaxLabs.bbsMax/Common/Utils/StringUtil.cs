//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Web;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
    public delegate int ForEachAction<T>(int index, T value);

    /// <summary>
    /// 字符串助手类
    /// </summary>
    public static class StringUtil
    {
        private static SplitLineRegex splitLineRegex;
        private static AngleBracketRegex angleBracketRegex;
        private static Regex booleanFormatRegex;
        private static ImageRegex imageBracketRegex;
        private static EmoticonRegex emoticonBracketRegex;


        public static string Replace(string text, string oldValue, string newValue)
        {
            return Regex.Replace(text, oldValue, newValue, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 打包一个json对象(实用于简单的类型) 
        /// </summary>
        /// <param name="jsonPropertys"></param>
        /// <returns></returns>
        public static string BuildJsonObject(Dictionary<string, object> jsonPropertys)
        {
            StringBuffer sb=new StringBuffer("{");
            Dictionary<string, object>.Enumerator em = jsonPropertys.GetEnumerator();
            bool isNumber = false;
            while (em.MoveNext())
            {
                isNumber = em.Current.Value is short
                    || em.Current.Value is int
                    || em.Current.Value is long
                    || em.Current.Value is byte
                    || em.Current.Value is float
                    || em.Current.Value is double
                    || em.Current.Value is decimal;
                sb += em.Current.Key + ":";
                if (!isNumber) sb += "\"";
                if (!isNumber)
                    sb += ToJavaScriptString(em.Current.Value.ToString());
                else
                    sb += em.Current.Value;
                if (!isNumber) sb += "\"";
                sb += ",";
            }
            if(sb.Length>1) sb.Remove(sb.Length - 1, 1);
            sb += "}";
            return sb.ToString();
        }

        public static string BuildJsonObject(object obj)
        {
            return JsonBuilder.GetJson(obj, null);
        }

        public static string BuildJsonObject(object obj, params string[] excludePropertys)
        {
            return JsonBuilder.GetJson(obj, excludePropertys);
        }


        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string BuiderRandomString(int length)
        {
            string chars =  "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890" ;
            StringBuilder buider = new StringBuilder(length);

            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                buider.Append(chars[(int)rnd.Next(0, chars.Length)]);
            }
            return buider.ToString();
        }

        /// <summary>
        /// 判断一个字符串是否是有效的布尔值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsBooleanFormat(string text)
        {
            if (booleanFormatRegex == null)
                booleanFormatRegex = new Regex("^(true|false)$", RegexOptions.IgnoreCase);
            return booleanFormatRegex.IsMatch(text);
        }

        /// <summary>
        /// 判断一个字符串是否是有效的整数值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsIntegerFormat(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int totalChars = text.Length;
            for (int i = 0; i < totalChars; i++)
            {
                if (char.IsNumber(text[i]) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 将字符串分行
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] GetLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new string[0];

            if (splitLineRegex == null)
                splitLineRegex = new SplitLineRegex();

            return splitLineRegex.Split(text);
        }

        /// <summary>
        /// string转int
        /// </summary>
        /// <param name="defaultValue">转换失败,返回此值</param>
        public static int GetInt(string input, int defaultValue)
        {
            int result;
            if (!int.TryParse(input, out result))
                result = defaultValue;
            return result;
        }

        /// <summary>
        /// 将一个字符串转为SQL字符串的内容。
        /// 注意返回结果不包含SQL字符串的开始和结束部分的单引号。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSqlString(string text)
        {
            return text.Replace("'", "''");
        }

        /// <summary>
        /// 转到JS用的string
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static string ToJavaScriptString(string text)
        {
            StringBuffer buffer = new StringBuffer(text);
            buffer.Replace("\\", @"\\");
            buffer.Replace("\t", @"\t");
            buffer.Replace("\n", @"\n");
            buffer.Replace("\r", @"\r");
            buffer.Replace("\"", @"\""");
            buffer.Replace("\'", @"\'");
            buffer.Replace("/", @"\/");
            return buffer.ToString();
        }

        /// <summary>
        /// 循环字符串中的每一行
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="action">循环时所要执行的动作，循环过程中执行动作后返回0时，表示退出循环</param>
        public static void ForEachLines(string text, ForEachAction<string> action)
        {
            string[] lines = GetLines(text);

            for (int i = 0; i < lines.Length; i++)
            {
                int result = action(i, lines[i]);

                //当动作返回0时，则退出循环
                if (result == 0)
                    return;
            }
        }

        public static T TryParse<T>(string value)
        {
            return TryParse<T>(value, default(T));
        }

        public static T TryParse<T>(string value, T defaultValue)
        {
            object r = null;

            using (ErrorScope es = new ErrorScope())
            {

                if (TryParse(typeof(T), value, out r))
                {
                    return (T)r;
                }
                else
                    es.IgnoreError<ErrorInfo>();

            }
            return defaultValue;
        }

        public static bool TryParse<T>(string value, out T result)
        {
            object r = null;

            if (TryParse(typeof(T), value, out r))
            {
                result = (T)r;
                return true;
            }

            result = default(T);

            return false;
        }

        /// <summary>
        /// 尝试解析字符串
        /// </summary>
        /// <param name="type">所要解析成的类型</param>
        /// <param name="value">字符串</param>
        /// <param name="result">解析结果，解析失败将返回null</param>
        /// <returns>解析失败将返回具体错误消息，否则将返回null，解析结果通过result获得</returns>
        public static bool TryParse(Type type, string value, out object result)
        {
            if (value == null)
            {
                Context.ThrowError(new Errors.TryParseFailedError(value, type));

                result = null;

                return false;
            }

            bool succeed = false;
            object parsedValue = null;

            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullable)
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
            {
                try
                {
                    parsedValue = Enum.Parse(type, value, true);
                    succeed = true;
                }
                catch
                {
                    Context.ThrowError(new Errors.TryParseFailedError(value, type));
                }
            }
            else if (type == typeof(Guid))
            {
                //TODO:此处需要改善性能

                try
                {
                    parsedValue = new Guid(value);
                    succeed = true;
                }
                catch
                {
                    Context.ThrowError(new Errors.TryParseFailedError(value, type));
                }
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.String:
                        parsedValue = value;
                        succeed = true;
                        break;
                    case TypeCode.Boolean:
                        {
                            if (value == "1")
                            {
                                parsedValue = true;
                                succeed = true;
                            }
                            else if (value == "0")
                            {
                                parsedValue = false;
                                succeed = true;
                            }
                            else
                            {
                                Boolean temp;
                                succeed = Boolean.TryParse(value, out temp);

                                if (succeed)
                                    parsedValue = temp;
                                else
                                    Context.ThrowError(new Errors.TryParseFailedError(value, type));
                            }
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            Byte temp;
                            succeed = Byte.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.Decimal:
                        {
                            Decimal temp;
                            succeed = Decimal.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.Double:
                        {
                            Double temp;
                            succeed = Double.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            Int16 temp;
                            succeed = Int16.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            Int32 temp;
                            succeed = Int32.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            Int64 temp;
                            succeed = Int64.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            SByte temp;
                            succeed = SByte.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.Single:
                        {
                            Single temp;
                            succeed = Single.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            UInt16 temp;
                            succeed = UInt16.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            UInt32 temp;
                            succeed = UInt32.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            UInt64 temp;
                            succeed = UInt64.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    case TypeCode.DateTime:
                        {                            
                            DateTime temp;
                            succeed = DateTime.TryParse(value, out temp);
                            if (succeed)
                            {
                                parsedValue = temp;
                            }
                            else
                                Context.ThrowError(new Errors.TryParseFailedError(value, type));
                        }
                        break;
                    default:
                        Context.ThrowError(new Errors.TryParseTypeNotSupportError(type));
                        break;
                }
            }

            result = parsedValue;

            return succeed;
        }

        /// <summary>
        /// 将字符串列表按逗号分隔符合并
        /// </summary>
        /// <param name="array">所要合并的字符串列表</param>
        /// <returns>合并结果</returns>
        public static string Join(IEnumerable array)
        {
            return Join(array, ",");
        }

        /// <summary>
        /// 将字符串列表按固定分隔符合并
        /// </summary>
        /// <param name="array">所要合并的字符串列表</param>
        /// <param name="separator">字符串的分隔符</param>
        /// <returns>合并结果</returns>
        public static string Join(IEnumerable array, string separator)
        {
            if (array == null)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            foreach (object value in array)
            {
                result.Append(value);
                result.Append(separator);
            }

            if (result.Length > 0)
                result.Remove(result.Length - separator.Length, separator.Length);

            return result.ToString();
        }


        /// <summary>
        /// 将字符串按,分割，并返回int类型的数组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] Split(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new string[0];
            return input.Split(',');
        }

        /// <summary>
        /// 将字符串按固定分隔符分割，并返回int类型的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] Split(string input, char separator)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.Split(separator);
            }
            return new string[0];
        }


        public static T[] Split<T>(string input)
        {
            return Split<T>(input, ',');
        }

        /// <summary>
        /// 将字符串按固定分隔符分割，并返回int类型的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static T[] Split<T>(string input, char separator)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new T[0];
            }
            string[] items = input.Split(new char[]{ separator}, StringSplitOptions.RemoveEmptyEntries);
            T[] result = new T[items.Length];

            for (int i = 0; i < items.Length; i++)
            {
                if (!TryParse<T>(items[i], out result[i]))
                    return new T[0];
            }

            return result;
        }

        public static List<T> Split2<T>(string input)
        {
            return Split2<T>(input, ',');
        }

        /// <summary>
        /// 将字符串按固定分隔符分割，并返回int类型的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<T> Split2<T>(string input, char separator)
        {
            string[] items = input.Split(new char[]{ separator}, StringSplitOptions.RemoveEmptyEntries);
            List<T> result = new List<T>();

            for (int i = 0; i < items.Length; i++)
            {
                T value;
                if (!TryParse<T>(items[i], out value))
                    return new List<T>();

                result.Add(value);
            }

            return result;
        }

        private static Encoding s_EncodingCache = null;

        /// <summary>
        /// 尝试获取GB2312编码并缓存起来，如果运行环境不支持GB2312编码，将缓存系统默认编码
        /// </summary>
        private static Encoding EncodingCache
        {
            get
            {
                if (s_EncodingCache == null)
                {

                    try
                    {
                        s_EncodingCache = Encoding.GetEncoding(936);

                    }
                    catch { }
                    
                    if (s_EncodingCache == null)
                        s_EncodingCache = Encoding.UTF8;

                }

                return s_EncodingCache;
            }
        }

        /// <summary>
        /// 获取字符串的字节长度，默认自动尝试用GB2312编码获取，
        /// 如果当前运行环境支持GB2312编码，英文字母将被按1字节计算，中文字符将被按2字节计算
        /// 如果尝试使用GB2312编码失败，将采用当前系统的默认编码，此时得到的字节长度根据具体运行环境默认编码而定
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns>字符串的字节长度</returns>
        public static int GetByteCount(string text)
        {
            return EncodingCache.GetByteCount(text);
        }

        /// <summary>
        /// 计算行号
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="startIndex">起始位置</param>
        /// <param name="endIndex">结束位置</param>
        /// <returns></returns>
        public static int LineCount(string text, int startIndex, int endIndex)
        {
            int num = 0;

            while (startIndex < endIndex)
            {
                if ((text[startIndex] == '\r') || ((text[startIndex] == '\n') && ((startIndex == 0) || (text[startIndex - 1] != '\r'))))
                {
                    num++;
                }

                startIndex++;
            }

            return num;
        }

        /// <summary>
        /// 忽略大小写的字符串比较
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
            {
                return true;
            }

            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            {
                return false;
            }

            if (s2.Length != s1.Length)
            {
                return false;
            }

            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
        }


        public static bool StartsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[0] == lookfor);
        }

        /// <summary>
        /// 快速判断字符串起始部分
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool StartsWith(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }

            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }

        /// <summary>
        /// 快速判断字符串起始部分
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }
            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }

        public static bool EndsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[text.Length - 1] == lookfor);
        }

        public static bool EndsWith(string target, string lookfor)
        {
            int indexA = target.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }

            return (0 == string.Compare(target, indexA, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }

        /// <summary>
        /// 快递判断字符串结束部分
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(string target, string lookfor)
        {
            int indexA = target.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }

            return (0 == string.Compare(target, indexA, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }

        public static bool Contains(string target, string lookfor)
        {
            if (target.Length < lookfor.Length)
                return false;

            return (0 <= target.IndexOf(lookfor));
        }

        /// <summary>
        /// 忽略大小写判断字符串是否包含
        /// </summary>
        /// <param name="target"></param>
        /// <param name="lookfor"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(string target, string lookfor)
        {
            if (target.Length < lookfor.Length)
                return false;

            return (0 <= target.IndexOf(lookfor, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CutString(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (length < 1)
                return text;

            byte[] buf = EncodingCache.GetBytes(text);

            if (buf.Length <= length)
            {
                return text;
            }

            int newLength = length;
            int[] numArray1 = new int[length];
            byte[] newBuf = null;
            int counter = 0;
            for (int i = 0; i < length; i++)
            {
                if (buf[i] > 0x7f)
                {
                    counter++;
                    if (counter == 3)
                    {
                        counter = 1;
                    }
                }
                else
                {
                    counter = 0;
                }
                numArray1[i] = counter;
            }

            if ((buf[length - 1] > 0x7f) && (numArray1[length - 1] == 1))
            {
                newLength = length + 1;
            }
            newBuf = new byte[newLength];
            Array.Copy(buf, newBuf, newLength);
            return EncodingCache.GetString(newBuf) + "...";

        }

        public static int FirstIndexOf(string source, int startIndex, int length, out string match, params string[] lookfors)
        {
            int index = -1;
            int itemIndex = -1;

            for (int i = 0; i < lookfors.Length; i++)
            {
                int temp = source.IndexOf(lookfors[i], startIndex, length);

                if (index < 0 || (temp >= 0 && temp < index))
                {
                    index = temp;
                    itemIndex = i;
                }
            }

            if (itemIndex >= 0)
                match = lookfors[itemIndex];
            else
                match = null;

            return index;
        }

        /// <summary>
        /// 友好大小
        /// </summary>
        public static string FriendlyCapacitySize(long value)
        {
            if (value < 1024 * 5 && value % 1024 != 0)
            {
                return value + " B";
            }
            else if (value < 1024 * 5 && value % 1024 == 0)
            {
                return (value / 1024) + " KB";
            }
            else if (value >= 1024 * 5 && value < 1024 * 1024)
            {
                return (value / 1024) + " KB";
            }
            else if (value < 1024 * 1024 * 5 && value % (1024 * 1024) != 0)
            {
                return (value / 1024) + " KB";
            }
            else if (value < 1024 * 1024 * 5 && value % (1024 * 1024) == 0)
            {
                return (value / (1024 * 1024)) + " MB";
            }
            else if (value >= 1024 * 1024 * 5 && value < 1024 * 1024 * 1024)
            {
                return (value / (1024 * 1024)) + " MB";
            }
            else
            {
                return (value / (1024 * 1024 * 1024)) + " GB";
            }
        }

        public static string GetSafeFormText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder result = new StringBuilder(text);
            result.Replace("\"", "&quot;");
            result.Replace("<", "&lt;");
            result.Replace(">", "&gt;");

            return result.ToString();
        }

        private static  Regex scriptReg=null;

        /// <summary>
        /// 过滤HTML内容里的脚本
        /// </summary>
        /// <param name="sourceHtml">HTML内容</param>
        /// <returns>返回过滤后的</returns>
        public static string FilterScript( string sourceHtml )
        {
            if (scriptReg == null) scriptReg = new JavascriptRegex();
            sourceHtml = scriptReg.Replace(sourceHtml, string.Empty);
            return sourceHtml;
        }

        /// <summary>
        /// 去除尖括号以及其中的内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ClearAngleBracket(string content) { return ClearAngleBracket(content, false); }
        public static string ClearAngleBracket(string content,bool processImages)
        {
            if (content == null || content.Length == 0)
                return content;

            if (processImages)
            {
                if (imageBracketRegex == null)
                    imageBracketRegex = new ImageRegex();
                if (emoticonBracketRegex == null)
                    emoticonBracketRegex = new EmoticonRegex();

                content = imageBracketRegex.Replace(content, "[表情]");
                content = emoticonBracketRegex.Replace(content, "[图片]");
            }

            if (angleBracketRegex == null)
                angleBracketRegex = new AngleBracketRegex();
            
            return angleBracketRegex.Replace(content, string.Empty);
        }

        /// <summary>
        /// 清除末尾的换行和空格(性能差 用于发表的时候)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ClearEndLineFeedAndBlank(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;
            content = Regex.Replace(content, "<br>", "<br />", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<br/>", "<br />", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<br />", "<br />", RegexOptions.IgnoreCase);//主要作用是把大写转为小写 如"<Br />" 转成 "<br />"
            content = content.Replace("\n","<br />");
            content = content.Replace("\r\n", "<br />");

            string[] contents = Regex.Split(content, "<br />");
            if (contents.Length > 1)
                content = ClearEndLineFeedAndBlank(contents, "<br />");

            //contents = content.Split('\n');

            //content = a(contents, "\n");


            return content.TrimEnd();
        }

        private static string ClearEndLineFeedAndBlank(string[] contents, string spliter)
        {
            StringBuilder result = new StringBuilder();

            bool hasContent = false;
            for (int i = contents.Length - 1; i > -1; i--)
            {
                if (hasContent == false)
                {
                    string temp = contents[i].Replace("&nbsp;", " ");

                    if (temp.Trim() != string.Empty)
                    {
                        result.Insert(0, contents[i].TrimEnd().Replace(" ", "&nbsp;"));
                        hasContent = true;
                    }
                }
                else
                {
                    result.Insert(0, spliter);
                    result.Insert(0, contents[i]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 如果内容只有换行和空格  返回空字符串   否则返回原内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Trim(string content)
        {
            if (string.IsNullOrEmpty(content) == false)
            {
                string tempContent = Regex.Replace(content, "(<br />)|(<br>)|(<br/>)|(&nbsp;)", string.Empty, RegexOptions.IgnoreCase);
                if (tempContent == string.Empty)
                    return string.Empty;
                else
                    return content;
            }

            return content;
        }


        //static readonly string[] excludeHtmlTags = new string[] { "body", "frame", "frameset", "html", "iframe", "style", "ilayer", "layer", "link", "meta", "applet", "form", "input", "select", "textarea" };//, "embed", "object","script"};


        ///// <summary>
        ///// 过滤HTML标签
        ///// </summary>
        ///// <param name="html"></param>
        ///// <returns></returns>
        //public static string ConvertHtmlToSafety(string html)
        //{
        //    string pattern="";
        //    html = FilterScript(html);//脚本
        //    foreach (string s in excludeHtmlTags)
        //    {
        //        pattern=string.Format("</?{0}.*?/?>",s);
        //        if (Regex.IsMatch(html, pattern, RegexOptions.IgnoreCase))
        //        {
        //            html = Regex.Replace(html, pattern, string.Empty, RegexOptions.IgnoreCase);
        //        }
        //    }
        //    return html;
        //}

        /// <summary>
        /// 对字符串进行Html解码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string HtmlDecode(string content)
        {
            return HttpUtility.HtmlDecode(content);
        }

        /// <summary>
        /// 对字符串进行Html编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string HtmlEncode(string content)
        {
            return HttpUtility.HtmlEncode(content);
        }


        /// <summary>
        /// <函数：Decode>
        ///作用：将16进制数据编码转化为字符串，是Encode的逆过程
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public static string HexDecode(string strDecode)
        {
            if (strDecode.IndexOf(@"\u") == -1)
                return strDecode;

            int startIndex = 0;
            if (strDecode.StartsWith(@"\u") == false)
            {
                startIndex = 1;
            }

            string[] codes = Regex.Split(strDecode, @"\\u");

            StringBuilder result = new StringBuilder();
            if (startIndex == 1)
                result.Append(codes[0]);
            for (int i = startIndex; i < codes.Length; i++)
            {
                try
                {
                    if (codes[i].Length > 4)
                    {
                        result.Append((char)short.Parse(codes[i].Substring(0, 4), global::System.Globalization.NumberStyles.HexNumber));
                        result.Append(codes[i].Substring(4));
                    }
                    else
                    {
                        result.Append((char)short.Parse(codes[i].Substring(0, 4), global::System.Globalization.NumberStyles.HexNumber));
                    }
                }
                catch
                {
                    result.Append(codes[i]);
                }
            }

            return result.ToString();
        }



        private static LinkUrlRegex regex_link = null;

        public static string EncodeInnerUrl(string content)
        {
            if (AllSettings.Current.BaseSeoSettings.EncodePostUrl)
            {
                if (regex_link == null)
                    regex_link = new LinkUrlRegex();

                return regex_link.Replace(content, delegate(Match match)
                {
                    string encodedUrl = SecurityUtil.Base64Encode(match.Groups["url"].Value);
                    return string.Concat("<a href=\"javascript:;\" onmouseover=\"return displayUrl(this,'", encodedUrl, "');\" onclick=\"" + "processUrl('", encodedUrl, "');return false;\">");

                });

            }
            return content;
        }
    }
}