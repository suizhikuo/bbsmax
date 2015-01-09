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
using System.Collections;
using System.Reflection;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// JSON序列化类
    /// </summary>
    public class JsonBuilder
    {

        #region 内部Json处理过程

        private static void BuildJsonObject(object obj, List<string> excludePropertys, StringBuilder builder, ArrayList parsed)
        {
            if (parsed.Contains(obj)) { builder.Append("{}"); return; }
            parsed.Add(obj);

            if (obj != null)
            {
                if (obj is IEnumerable)
                {
                    BuildJsonArray(obj as IEnumerable, excludePropertys, builder, parsed);
                    return;
                }

                builder.Append("{");
                PropertyInfo[] propertyInfos;
                Type t = obj.GetType();
                object value;
                propertyInfos = t.GetProperties();

                foreach (PropertyInfo pi in propertyInfos)
                {
                    //没有声明JsonItemAttribute，不予转换为Json
                    if (pi.IsDefined(typeof(JsonItemAttribute), false) == false)
                    {
                        continue;
                    }
                    //是否排除
                    else if (excludePropertys.Contains(pi.Name))
                    {
                        continue;
                        // builder.Append("_exclude:\"" + pi.Name + "\",");
                    }
                    else
                    {
                        if (pi.CanRead)
                        {
                            builder.Append(pi.Name);
                            builder.Append(":");
                            value = pi.GetValue(obj, null);
                            if (IsNumber(value))
                            {
                                builder.Append(value.ToString().ToLower());
                            }
                            else if (IsDateTime(value))
                            {
                                builder.Append("\"" + DateTimeUtil.FormatDateTime((DateTime)value) + "\"");//------------
                            }
                            else if (IsString(value))
                            {
                                builder.Append("\"" + ToJavaScriptString(value.ToString()) + "\"");
                            }
                            else if (value is IEnumerable)
                            {
                                BuildJsonArray(value as IEnumerable, excludePropertys, builder, parsed);
                            }
                            else
                            {
                                BuildJsonObject(value, excludePropertys, builder, parsed);
                            }
                            builder.Append(",");
                        }
                    }
                }

                if (builder[builder.Length - 1] == ',')
                    builder.Remove(builder.Length - 1, 1);
                builder.Append("}");
            }
            else
            {
                builder.Append("null");
            }
        }

        private static void BuildJsonArray(IEnumerable ie, List<string> excludePropertys, StringBuilder builder, ArrayList parsed)
        {
            IEnumerator ito = ie.GetEnumerator();
            builder.Append("[");
            bool hasItem = false;
            while (ito.MoveNext())
            {
                hasItem = true;
                if (IsNumber(ito.Current))
                {
                    builder.Append(ito.Current.ToString().ToLower() + ",");
                }
                else if (IsDateTime(ito.Current))
                {
                    builder.Append("\"" + DateTimeUtil.FormatDate((DateTime)ito.Current) + "\"");//------------
                }
                else if (IsString(ito.Current))
                {
                    builder.Append("\"" + ToJavaScriptString(ito.Current.ToString()) + "\",");
                }
                else
                {
                    BuildJsonObject(ito.Current, excludePropertys, builder, parsed);
                    builder.Append(",");
                }
            }

            if (hasItem) builder.Remove(builder.Length - 1, 1);

            builder.Append("]");
        }

        /// <summary>
        /// 直接输出的属性（这里面包括 bool值）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNumber(object obj)
        {
            if (obj is int
                || obj is long
                || obj is float
                || obj is double
                || obj is byte
                || obj is short
                || obj is decimal
                || obj is uint
                || obj is ulong
                || obj is ushort
                || obj is sbyte
                || obj is bool
                )
                return true;
            return false;
        }

        private static bool IsDateTime(object obj)
        {
            return obj is DateTime;
        }

        private static bool IsString(object obj)
        {
            return (obj is string
                || obj is char
                || obj is Guid
                || obj is DateTime
                || obj is Enum
                || obj is StringBuilder
                || obj is Uri
                );
        }

        /// <summary>
        /// 转到JS用的string
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        private static string ToJavaScriptString(string text)
        {
            StringBuilder buffer = new StringBuilder(text);
            buffer.Replace("\\", @"\\");
            buffer.Replace("\n", @"\n");
            buffer.Replace("\r", @"\r");
            buffer.Replace("\"", @"\""");
            buffer.Replace("\f", @"\f");
            buffer.Replace("\t", @"\t");
            return buffer.ToString();
        }

        private static void BuildJson(object obj, List<string> excludePropertys, StringBuilder builder)
        {
            if (IsNumber(obj))
            {
                builder.Append(obj.ToString().ToLower());
                return;
            }
            if (IsDateTime(obj))
            {
                builder.Append("\""+ DateTimeUtil.FormatDateTime((DateTime)obj)+"\"");
                return;
            }
            if (IsString(obj))
            {
                builder.Append("\""+ToJavaScriptString(obj.ToString()) +"\"" );
                return;
            }

            ArrayList parsed = new ArrayList();//记录已经序列化的属性， 否则如果类内部的属性有递归关系， 会死循环
            //StringBuilder builder = new StringBuilder();

            BuildJsonObject(obj, excludePropertys, builder, parsed);

            //return builder.ToString();
        }

        #endregion

        public static string GetJson(object obj, params string[] excludePropertys)
        {
            StringBuilder builder = new StringBuilder();
            if (excludePropertys == null)
                BuildJson(obj, new List<string>(), builder);
            else
                BuildJson(obj, new List<string>(excludePropertys), builder);
            return builder.ToString();
        }

        private Dictionary<string, object> objectsToBuild = new Dictionary<string, object>();
        private Dictionary<string, string[]> allExcludePropertys = new Dictionary<string, string[]>();

        public void Set(string name, object obj, params string[] excludePropertys)
        {
            if (objectsToBuild.ContainsKey(name))
                objectsToBuild[name] = obj;
            else
                objectsToBuild.Add(name, obj);

            if (allExcludePropertys.ContainsKey(name))
            {
                if (excludePropertys == null)
                    allExcludePropertys.Remove(name);
                else
                    allExcludePropertys[name] = excludePropertys;
            }
            else
            {
                if (excludePropertys != null)
                    allExcludePropertys.Add(name, excludePropertys);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");

            string[] excludePropertys = null;
            foreach (KeyValuePair<string, object> pair in objectsToBuild)
            {
                excludePropertys = null;
                allExcludePropertys.TryGetValue(pair.Key, out excludePropertys);

                if (builder.Length != 1)
                    builder.Append(",");

                builder.Append(pair.Key);
                builder.Append(":");

				if (excludePropertys == null)
					BuildJson(pair.Value, new List<string>(), builder);
				else
					BuildJson(pair.Value, new List<string>(excludePropertys), builder);
            }

            builder.Append("}");

            return builder.ToString();
        }
    }
}