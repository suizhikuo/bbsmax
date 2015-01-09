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
    public sealed class ShareFilter : FilterBase<ShareFilter>, ICloneable
    {
        public enum OrderBy
        {
            /// <summary>
            /// 默认排序，根据发表时间（即ID）
            /// </summary>
            ShareID = 0
        }

		public ShareFilter()
        {
            this.PageSize = Consts.DefaultPageSize;
            this.IsDesc = true;
            this.Order = OrderBy.ShareID;
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

        [FilterItem(FormName = "shareid")]
        public int? ShareID
        {
            get;
            set;
        }

        [FilterItem(FormName = "sharetype")]
        public ShareType? ShareType
        {
            get;
            set;
        }


        [FilterItem(FormName = "privacytype")]
        public PrivacyType? PrivacyType
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




        #region ICloneable 成员

        public object Clone()
        {
            ShareFilter filter = new ShareFilter();
            filter.BeginDate = this.BeginDate;
            filter.EndDate = this.EndDate;
            filter.Order = this.Order;
            filter.IsDesc = this.IsDesc;
            filter.PageSize = this.PageSize;
            filter.ShareType = this.ShareType;
            filter.PrivacyType = this.PrivacyType;
            filter.ShareID = this.ShareID;
            filter.UserID = this.UserID;
            filter.Username = this.Username;
            return filter;
        }

        #endregion
    }
}