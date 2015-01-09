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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Filters
{
    public class DenouncingFilter : FilterBase<DenouncingFilter>
    {
		public DenouncingFilter()
        {
            this.IsDesc = true;
            this.PageSize = Consts.DefaultPageSize;
        }

        /// <summary>
        /// 举报类型
        /// </summary>
        [FilterItem]
		public DenouncingType Type { get; set; }

        /// <summary>
        /// 举报理由
        /// </summary>
        [FilterItem]
		public DenouncingReason Reason { get; set; }

        /// <summary>
        /// 举报状态
        /// </summary>
        [FilterItem]
        public int? ReportState { get; set; }

		public DateTime? BeginDate { get; set; }

		public DateTime? EndDate { get; set; }

        /// <summary>
        /// 排序规则
        /// </summary>
        [FilterItem]
        public bool IsDesc { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        [FilterItem]
        public int PageSize { get; set; }
    }
}