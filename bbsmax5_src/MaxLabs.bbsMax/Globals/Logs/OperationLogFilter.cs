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

using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Logs
{
	public class OperationLogFilter : Filters.FilterBase<OperationLogFilter>
	{
		public OperationLogFilter()
        {
            this.PageSize = Consts.DefaultPageSize;
            this.IsDesc = true;
        }

		[FilterItem(FormName = "OperatorName")]
		public string OperatorName
		{
			get;
			set;
		}

		[FilterItem(FormName = "OperatorID")]
		public int? OperatorID
		{
			get;
			set;
		}

		[FilterItem(FormName = "OperatorIP")]
		public string OperatorIP
		{
			get;
			set;
		}

		[FilterItem(FormName = "OperationType")]
		public string OperationType
		{
			get;
			set;
		}

		[FilterItem(FormName = "TargetID_1")]
		public int? TargetID_1
		{
			get;
			set;
		}

		[FilterItem(FormName = "TargetID_2")]
		public int? TargetID_2
		{
			get;
			set;
		}

		[FilterItem(FormName = "TargetID_3")]
		public int? TargetID_3
		{
			get;
			set;
		}

		[FilterItem(FormName = "BeginDate", FormType = FilterItemFormType.BeginDate)]
		public DateTime? BeginDate
		{
			get;
			set;
		}

		[FilterItem(FormName = "EndDate", FormType = FilterItemFormType.EndDate)]
		public DateTime? EndDate
		{
			get;
			set;
		}

		[FilterItem(FormName = "isdesc")]
		public bool IsDesc
		{
			get;
			set;
		}

		[FilterItem(FormName = "pagesize")]
		public int PageSize
		{
			get;
			set;
		}
	}
}