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
    public sealed class AttachmentFilter : FilterBase<AttachmentFilter>, ICloneable
    {
        public AttachmentFilter()
        {

        }

        public enum OrderBy
        {
            /// <summary>
            /// 默认排序，根据发表时间（即ID）
            /// </summary>
            AttachmentID = 0,

            TotalDownload = 1,

            FileSize = 2,

            Price = 3,
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


        [FilterItem(FormName = "FileType")]
        public string FileType
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


        [FilterItem(FormName = "MaxTotalDownload")]
        public int? MaxTotalDownload
        {
            get;
            set;
        }

        [FilterItem(FormName = "MinTotalDownload")]
        public int? MinTotalDownload
        {
            get;
            set;
        }

        [FilterItem(FormName = "MaxFileSize")]
        public long? MaxFileSize
        {
            get;
            set;
        }
        [FilterItem(FormName = "MaxFileSizeUnit")]
        public FileSizeUnit MaxFileSizeUnit
        {
            get;
            set;
        }

        [FilterItem(FormName = "MinFileSize")]
        public long? MinFileSize
        {
            get;
            set;
        }
        [FilterItem(FormName = "MinFileSizeUnit")]
        public FileSizeUnit MinFileSizeUnit
        {
            get;
            set;
        }

        [FilterItem(FormName = "MaxPrice")]
        public int? MaxPrice
        {
            get;
            set;
        }

        [FilterItem(FormName = "MinPrice")]
        public int? MinPrice
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
            AttachmentFilter filter = new AttachmentFilter();
            filter.BeginDate = this.BeginDate;
            filter.EndDate = this.EndDate;
            filter.FileType = this.FileType;
            filter.ForumID = this.ForumID;
            filter.IsDesc = this.IsDesc;
            filter.KeyWord = this.KeyWord;
            filter.MaxFileSize = this.MaxFileSize;
            filter.MaxFileSizeUnit = this.MaxFileSizeUnit;
            filter.MaxPrice = this.MaxPrice;
            filter.MaxTotalDownload = this.MaxTotalDownload;
            filter.MinFileSize = this.MinFileSize;
            filter.MinFileSizeUnit = this.MinFileSizeUnit;
            filter.MinPrice = this.MinPrice;
            filter.MinTotalDownload = this.MinTotalDownload;
            filter.Order = this.Order;
            filter.PageSize = this.PageSize;
            filter.UserID = this.UserID;
            filter.Username = this.Username;
            return filter;
        }

        #endregion
    }
}