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

namespace MaxLabs.bbsMax.Features.Templates
{
	class MethodBasedTemplateTag
	{
		public MethodBasedTemplateTag(string tagName, MethodInfo[] methodInfos)
		{
			m_TagName = tagName;
			m_MethodInfos = methodInfos;
		}

		private string m_TagName;

		public string TagName
		{
			get { return m_TagName; }
			set { m_TagName = value; }
		}

		private MethodInfo[] m_MethodInfos;

		public MethodInfo[] MethodInfos
		{
			get { return m_MethodInfos; }
		}
	}
}