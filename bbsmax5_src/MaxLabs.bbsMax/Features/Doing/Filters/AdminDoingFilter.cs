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

using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Filters
{
    public sealed class AdminDoingFilter : FilterBase<AdminDoingFilter>
    {
        public enum OrderBy
        {
            /// <summary>
            /// 默认排序，按发表时间排序
            /// </summary>
            DoingID = 0,
            /// <summary>
            /// 被评论数
            /// </summary>
            TotalComments = 1,
            /// <summary>
            /// 发表者
            /// </summary>
            UserID = 2,
            /// <summary>
            /// IP地址
            /// </summary>
            CreateIP = 3
        }

        public AdminDoingFilter()
        {
            this.Order = OrderBy.DoingID;
            this.IsDesc = true;
            this.PageSize = Consts.DefaultPageSize;
        }

        /// <summary>
        /// 作者ID
        /// </summary>
        [FilterItem]
        public int? UserID { get; set; }

        /// <summary>
        /// 用户名 作者
        /// </summary>
        [FilterItem]
        public string Username { get; set; }

        /// <summary>
        /// 记录内容
        /// </summary>
        [FilterItem]
        public string Content { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        [FilterItem]
        public string IP { get; set; }

        /// <summary>
        /// 起始时间
        /// </summary>
        [FilterItem(FormName = "BeginDate", FormType = FilterItemFormType.BeginDate)]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [FilterItem(FormName = "EndDate", FormType = FilterItemFormType.EndDate)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        [FilterItem]
        public OrderBy Order { get; set; }

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