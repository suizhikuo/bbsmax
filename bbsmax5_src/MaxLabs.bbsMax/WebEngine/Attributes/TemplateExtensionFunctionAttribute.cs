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

namespace MaxLabs.WebEngine
{
	public class TemplateExtensionFunctionAttribute : TemplateMemberAttribute
	{
		public TemplateExtensionFunctionAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// 从方法上获取自定义标签
		/// </summary>
		/// <param name="method">方法的反射信息</param>
		/// <returns>自定义标签实例，或者null</returns>
		public static TemplateExtensionFunctionAttribute GetFromMethod(MethodInfo method)
		{
			Attribute result = Attribute.GetCustomAttribute(method, typeof(TemplateExtensionFunctionAttribute), true);

			if (result == null)
				return null;

			return (TemplateExtensionFunctionAttribute)result;
		}
	}
}