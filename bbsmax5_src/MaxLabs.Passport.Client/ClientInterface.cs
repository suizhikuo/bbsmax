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
using System.Web.Services;
using System.Xml;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;
using MaxLabs.Passport.Proxy;
using System.Xml.Serialization;

namespace MaxLabs.Passport.Client
{
    ///// <summary>
    ///// service 的摘要说明
    ///// </summary>
    //[WebService(Namespace = "http://bbsmax.com/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
    public class ClientInterface : System.Web.UI.Page
    {
        private static long LastInstructID;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string key = Request.Headers["key"];
            if (!string.IsNullOrEmpty(key))
            {
                using (StreamReader sr = new StreamReader(Request.InputStream, Encoding.UTF8))
                {
                    string content = sr.ReadToEnd();
                    InstructProxy[] instructs = null;
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            instructs = DataReadWrap.Get<InstructProxy[]>(content);
                        }
                        catch
                        {
                            return;
                        }

                        ReceiveInstruct(key, instructs);
                    }
                }
            }
        }

        public void ReceiveInstruct(string key, InstructProxy[] instructs)
        {
            foreach (InstructProxy ins in instructs)
            {
                if (LastInstructID >= ins.InstructID)
                    return;
                LastInstructID = ins.InstructID;

                InstructHandlerManager.ExecuteInstruct(key, ins.InstructType, ins.TargetID, ins.CreateDateTime, ins.Datas);
            }
        }
    }

    public class DataReadWrap
    {
        public static T Get<T>(string instructData)
        {
            object obj = null;
            XmlSerializer fommatter = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(instructData))
            {
                obj = fommatter.Deserialize(reader);
            }
            return (T)obj;
        }
    }

    static class StringConverter
    {
        #region 数据类型转换
        public static T TryParse<T>(string value)
        {
            return TryParse<T>(value, default(T));
        }

        public static T TryParse<T>(string value, T defaultValue)
        {
            object r = null;

            if (TryParse(typeof(T), value, out r))
            {
                return (T)r;
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
                parsedValue = Enum.Parse(type, value, true);
                succeed = true;
            }
            else if (type == typeof(Guid))
            {
                parsedValue = new Guid(value); succeed = true;
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
                            Boolean temp;
                            succeed = Boolean.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            Byte temp;
                            succeed = Byte.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Decimal:
                        {
                            Decimal temp;
                            succeed = Decimal.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Double:
                        {
                            Double temp;
                            succeed = Double.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            Int16 temp;
                            succeed = Int16.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;

                        }
                        break;
                    case TypeCode.Int32:
                        {
                            Int32 temp;
                            succeed = Int32.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            Int64 temp;
                            succeed = Int64.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            SByte temp;
                            succeed = SByte.TryParse(value, out temp);

                            if (succeed)
                                parsedValue = temp;
                        }
                        break;
                    case TypeCode.Single:
                        {
                            Single temp;
                            succeed = Single.TryParse(value, out temp);

                            if (succeed) parsedValue = temp;
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            UInt16 temp;
                            succeed = UInt16.TryParse(value, out temp);

                            if (succeed) parsedValue = temp;
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            UInt32 temp;
                            succeed = UInt32.TryParse(value, out temp);

                            if (succeed) parsedValue = temp;
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            UInt64 temp;
                            succeed = UInt64.TryParse(value, out temp);

                            if (succeed) parsedValue = temp;
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
                        }
                        break;
                    default:
                        break;
                }
            }

            result = parsedValue;

            return succeed;
        }

        #endregion
    }
}