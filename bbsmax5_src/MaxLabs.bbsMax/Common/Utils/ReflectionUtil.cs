//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax
{
	/// <summary>
	/// 反射操作助手类
	/// </summary>
	public class ReflectionUtil
	{
		/// <summary>
		/// 根据名称获取类型中对应的方法（忽略大小写）
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="name">方法名</param>
		/// <returns>方法信息</returns>
		public static MethodInfo GetMethodByName(Type type, string name)
		{
			return GetMethodByName(type, name, true);
		}

		/// <summary>
		/// 根据名称获取类型中对应的方法
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="name">方法名</param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>方法信息</returns>
		public static MethodInfo GetMethodByName(Type type, string name, bool ignoreCase)
		{
			foreach (MethodInfo method in type.GetMethods())
			{
				if (string.Compare(method.Name, name, ignoreCase) == 0)
					return method;
			}

			return null;
		}


		/// <summary>
		/// 根据名称获取类型中的属性（忽略大小写）
		/// </summary>
		/// <param name="type">类型信息</param>
		/// <param name="name">属性名</param>
		/// <returns>属性信息</returns>
		public static PropertyInfo GetPropertyByName(Type type, string name)
		{
			return GetPropertyByName(type, name, true);
		}

		/// <summary>
		/// 根据名称获取类型中的属性
		/// </summary>
		/// <param name="type">类型信息</param>
		/// <param name="name">属性名</param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>属性信息</returns>
		public static PropertyInfo GetPropertyByName(Type type, string name, bool ignoreCase)
		{
			foreach (PropertyInfo property in type.GetProperties())
			{
				if (string.Compare(property.Name, name, ignoreCase) == 0)
					return property;
			}

			return null;
		}

		public static ParameterInfo GetParameterByName(MethodInfo methodInfo, string name)
		{
			foreach (ParameterInfo parameter in methodInfo.GetParameters())
			{
				if (StringUtil.EqualsIgnoreCase(parameter.Name, name))
					return parameter;
			}

			return null;
		}

        public static bool HasInterface(Type type, Type interfaceType)
        {

            Type[] iTypes = type.GetInterfaces();

            foreach(Type iType in iTypes)
            {
                if (iType == interfaceType)
                    return true;
            }

            return false;

        }

		public static string GetTypeName(Type type)
		{
			return type.FullName; //TODO:暂时的
		}

        /// <summary>
        /// 获取C#代码格式的完整类型名包含命名空间
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetCSharpTypeName(Type type)
        {
            StringBuilder result = new StringBuilder(type.DeclaringType != null ? type.DeclaringType.FullName : type.Namespace);

            result.Append(".").Append(type.Name);

            if (type.IsNested)
                result.Replace("+", ".");

            if (type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();

                int genericArgsUsage = 0;

                foreach (Match match in Regex.Matches(result.ToString(), "`(?<count>\\d+)"))
                {
                    int count = int.Parse(match.Groups["count"].Value);

                    StringBuilder sb = new StringBuilder();

                    sb.Append("<");

                    int i = genericArgsUsage;

                    for (; i - genericArgsUsage < count; i++)
                    {
                        sb.Append(GetCSharpTypeName(genericArgs[i]));

                        sb.Append(",");
                    }

                    sb.Remove(sb.Length - 1, 1);

                    sb.Append(">");

                    genericArgsUsage = i;

                    result.Replace(match.Value, sb.ToString(), match.Index, match.Length);
                };
            }

            return result.ToString();
        }
	}
}