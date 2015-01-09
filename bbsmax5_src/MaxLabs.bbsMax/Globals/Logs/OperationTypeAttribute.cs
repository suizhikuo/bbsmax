//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Logs
{
	[AttributeUsage(AttributeTargets.Class)]
	public class OperationTypeAttribute:Attribute
	{
		public OperationTypeAttribute(string displayName)
		{
			DisplayName = displayName;
		}

		public OperationTypeAttribute(string displayName, string targetID_DisplayName_1)
		{
			DisplayName = displayName;
			TargetID_DisplayName_1 = targetID_DisplayName_1;
		}

		public OperationTypeAttribute(string displayName, string targetID_DisplayName_1, string targetID_DisplayName_2)
		{
			DisplayName = displayName;
			TargetID_DisplayName_1 = targetID_DisplayName_1;
			TargetID_DisplayName_2 = targetID_DisplayName_2;
		}

		public OperationTypeAttribute(string displayName, string targetID_DisplayName_1, string targetID_DisplayName_2, string targetID_DisplayName_3)
		{
			DisplayName = displayName;
			TargetID_DisplayName_1 = targetID_DisplayName_1;
			TargetID_DisplayName_2 = targetID_DisplayName_2;
			TargetID_DisplayName_3 = targetID_DisplayName_3;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public string TargetID_DisplayName_1
		{
			get;
			private set;
		}

		public string TargetID_DisplayName_2
		{
			get;
			private set;
		}

		public string TargetID_DisplayName_3
		{
			get;
			private set;
		}
	}
}