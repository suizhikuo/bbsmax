//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Filters
{
    public class AdminBlogArticleFilter : FilterBase<AdminBlogArticleFilter>
    {
        /// <summary>
        /// 文章结果排序键
        /// </summary>
        public enum OrderBy
        {
            /// <summary>
            /// 发布时间
            /// </summary>
            ArticleID = 1,
            /// <summary>
            /// 评论时间
            /// </summary>
            LastCommentDate = 2,
            /// <summary>
            /// 被评论数
            /// </summary>
            TotalComments = 3,
            /// <summary>
            /// 浏览次数
            /// </summary>
            TotalViews = 4,
            /// <summary>
            /// 所有者
            /// </summary>
            UserID = 5,
            /// <summary>
            /// IP地址
            /// </summary>
            CreateIP = 6
        }

        public AdminBlogArticleFilter()
        {
            this.PageSize = Consts.DefaultPageSize;
            this.SearchMode = SearchArticleMethod.Default;
            this.IsDesc = true;
            this.Order = OrderBy.ArticleID;
        }

        [FilterItem(FormName = "Username")]
        public string Username
        {
            get;
            set;
        }

        [FilterItem(FormName = "AuthorID")]
        public int? AuthorID
        {
            get;
            set;
        }

        [FilterItem(FormName = "SearchMode")]
        public SearchArticleMethod SearchMode
        {
            get;
            set;
        }

        [FilterItem(FormName = "SearchKey")]
        public string SearchKey
        {
            get;
            set;
        }

        [FilterItem(FormName = "ArticleID")]
        public int? ArticleID
        {
            get;
            set;
        }

        [FilterItem(FormName = "CreateIP")]
        public string CreateIP
        {
            get;
            set;
        }

        [FilterItem(FormName = "PrivacyType")]
        public PrivacyType? PrivacyType
        {
            get;
            set;
        }

        [FilterItem(FormName = "TotaLViewsScopeBegin")]
        public int? TotaLViewsScopeBegin
        {
            get;
            set;
        }

        [FilterItem(FormName = "TotalViewsScopeEnd")]
        public int? TotalViewsScopeEnd
        {
            get;
            set;
        }

        [FilterItem(FormName = "TotalCommentsScopeBegin")]
        public int? TotalCommentsScopeBegin
        {
            get;
            set;
        }

        [FilterItem(FormName = "TotalCommentsScopeEnd")]
        public int? TotalCommentsScopeEnd
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

        [FilterItem(FormName = "order")]
        public OrderBy Order
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