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
    public sealed class FeedSearchFilter : FilterBase<FeedSearchFilter>, ICloneable
    {
        public FeedSearchFilter() { this.PageSize = Consts.DefaultPageSize ; }

        public enum OrderBy
        {
            /// <summary>
            /// 
            /// </summary>
            ID = 0,
            /// <summary>
            /// 
            /// </summary>
            CreateDate = 1
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


        [FilterItem(FormName = "appaction")]
        public string AppActionString
        {
            get;
            set;
        }

        public Guid? AppID
        {
            get
            {
                if (string.IsNullOrEmpty(AppActionString))
                    return null;

                try
                {
                    return new Guid(AppActionString.Split('.')[0]);
                }
                catch
                {
                    return null;
                }

            }
        }

        public int? AppActionType
        {
            get
            {
                if (string.IsNullOrEmpty(AppActionString))
                    return null;

                try
                {
                    return int.Parse(AppActionString.Split('.')[1]);
                }
                catch
                {
                    return null;
                }

            }
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




        #region ICloneable 成员

        public object Clone()
        {
            FeedSearchFilter filter = new FeedSearchFilter();
            filter.BeginDate = this.BeginDate;
            filter.EndDate = this.EndDate;
            filter.Order = this.Order;
            filter.IsDesc = this.IsDesc;
            filter.PageSize = this.PageSize;
            filter.AppActionString = this.AppActionString;
            filter.UserID = this.UserID;
            filter.Username = this.Username;
            return filter;
        }

        #endregion
    }
}