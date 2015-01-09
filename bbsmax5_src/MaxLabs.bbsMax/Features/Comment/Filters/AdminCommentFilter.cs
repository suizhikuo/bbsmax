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
    public class AdminCommentFilter : FilterBase<AdminCommentFilter>
    {
        public enum OrderBy
        {
            /// <summary>
            /// 默认排序，按发表时间排序
            /// </summary>
            CommentID = 0,
            /// <summary>
            /// 评论类型
            /// </summary>
            Type = 1,
            /// <summary>
            /// 发表者
            /// </summary>
            UserID = 2,
            /// <summary>
            /// 被评论者
            /// </summary>
            TargetUserID = 3
        }

        public AdminCommentFilter()
        {
            this.PageSize = Consts.DefaultPageSize;
            this.IsDesc = true;
        }

        /// <summary>
        /// 评论类型
        /// </summary>
        [FilterItem]
        public CommentType Type { get; set; }

        /// <summary>
        /// 评论者用户名
        /// </summary>
        [FilterItem]
        public string Username { get; set; }

        /// <summary>
        /// 被评论者用户名
        /// </summary>
        [FilterItem]
        public string TargetUsername { get; set; }

        /// <summary>
        /// 评论起始时间
        /// </summary>
        [FilterItem(FormName = "BeginDate", FormType = FilterItemFormType.BeginDate)]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 评论结束时间
        /// </summary>
        [FilterItem(FormName = "EndDate", FormType = FilterItemFormType.EndDate)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [FilterItem]
        public string Content { get; set; }

        /// <summary>
        /// 发布者IP
        /// </summary>
        [FilterItem]
        public string IP { get; set; }

        /// <summary>
        /// 是否审核通过
        /// </summary>
        [FilterItem]
        public bool? IsApproved { get; set; }

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