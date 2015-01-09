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
    public sealed class PostFilter : FilterBase<PostFilter>, ICloneable
    {
        public PostFilter()
        { 
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

       

        [FilterItem(FormName = "PostID")]
        public int? PostID
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


        [FilterItem(FormName = "IsUnapproved")]
        public bool? IsUnapproved
        {
            get;
            set;
        }

        #region ICloneable 成员

        public object Clone()
        {
            PostFilter filter = new PostFilter();
            filter.BeginDate = this.BeginDate;
            filter.EndDate = this.EndDate;
            filter.IsDesc = this.IsDesc;
            filter.PageSize = this.PageSize;
            filter.ForumID = this.ForumID;
            filter.KeyWord = this.KeyWord;
            filter.PostID = this.PostID;
            filter.UserID = this.UserID;
            filter.Username = this.Username;
            filter.SearchMode = this.SearchMode;
            filter.CreateIP = this.CreateIP;
            filter.IsUnapproved = this.IsUnapproved;
            return filter;
        }

        #endregion
    }
}