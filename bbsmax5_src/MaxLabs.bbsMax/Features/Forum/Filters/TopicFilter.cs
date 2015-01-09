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
    public sealed class TopicFilter : FilterBase<TopicFilter>, ICloneable
    {
        public TopicFilter()
        { 
        }

        public enum OrderBy
        {
            /// <summary>
            /// 默认排序，根据发表时间（即ID）
            /// </summary>
            TopicID = 0,

            LastReplyDate = 1,

            ReplyCount = 2,

            ViewCount = 3,
        }

        [FilterItem(FormName = "username")]
        public string Username
        {
            get;
            set;
        }

        [FilterItem(FormName = "userid")]
        public int? UserID
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

        [FilterItem(FormName = "ForumID")]
        public int? ForumID
        {
            get;
            set;
        }

        [FilterItem(FormName = "includeStick")]
        public bool IncludeStick
        {
            get;
            set;
        }

        [FilterItem(FormName = "includeValued")]
        public bool IncludeValued
        {
            get;
            set;
        }

        [FilterItem(FormName = "topicID")]
        public int? TopicID
        {
            get;
            set;
        }


        [FilterItem(FormName = "keyword")]
        public string KeyWord
        {
            get;
            set;
        }


        [FilterItem(FormName = "maxViewCount")]
        public int? MaxViewCount
        {
            get;
            set;
        }

        [FilterItem(FormName = "minViewCount")]
        public int? MinViewCount
        {
            get;
            set;
        }

        [FilterItem(FormName = "maxReplyCount")]
        public int? MaxReplyCount
        {
            get;
            set;
        }

        [FilterItem(FormName = "minReplyCount")]
        public int? MinReplyCount
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

        [FilterItem(FormName = "IsDesc")]
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

        [FilterItem(FormName = "SearchMode")]
        public SearchArticleMethod SearchMode
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


        [FilterItem(FormName = "Status")]
        public ThreadStatus? Status
        {
            get;
            set;
        }

        #region ICloneable 成员

        public object Clone()
        {
            TopicFilter filter = new TopicFilter();
            filter.BeginDate = this.BeginDate;
            filter.EndDate = this.EndDate;
            filter.Order = this.Order;
            filter.IsDesc = this.IsDesc;
            filter.PageSize = this.PageSize;
            filter.ForumID = this.ForumID;
            filter.IncludeStick = this.IncludeStick;
            filter.IncludeValued = this.IncludeValued;
            filter.KeyWord = this.KeyWord;
            filter.MaxReplyCount = this.MaxReplyCount;
            filter.MaxViewCount = this.MaxViewCount;
            filter.MinReplyCount = this.MinReplyCount;
            filter.MinViewCount = this.MinViewCount;
            filter.TopicID = this.TopicID;
            filter.UserID = this.UserID;
            filter.Username = this.Username;
            filter.SearchMode = this.SearchMode;
            filter.CreateIP = this.CreateIP;
            filter.Status = this.Status;
            return filter;
        }

        #endregion
    }
}