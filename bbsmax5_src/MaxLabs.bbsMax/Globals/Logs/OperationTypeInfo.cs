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

namespace MaxLabs.bbsMax.Logs
{
	public class OperationTypeInfo : IComparable<OperationTypeInfo>
	{
		public OperationTypeInfo(string type, OperationTypeAttribute info)
		{
			Type = type;
			DisplayName = info.DisplayName;
			TargetID_DisplayName_1 = info.TargetID_DisplayName_1;
			TargetID_DisplayName_2 = info.TargetID_DisplayName_2;
			TargetID_DisplayName_3 = info.TargetID_DisplayName_3;
		}

		public string Type
		{
			get;
			private set;
		}

		public string DisplayName
		{
			get;
			private set;
		}

		public bool TargetID_Enable_1
		{
			get { return this.TargetID_DisplayName_1 != null; }
		}

		public bool TargetID_Enable_2
		{
			get { return this.TargetID_DisplayName_2 != null; }
		}

		public bool TargetID_Enable_3
		{
			get { return this.TargetID_DisplayName_3 != null; }
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

		#region IComparable<OperationTypeInfo> 成员

		public int CompareTo(OperationTypeInfo other)
		{
			return String.Compare(this.DisplayName, other.DisplayName);
		}

		#endregion
	}
}