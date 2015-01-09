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
    public class AdminAlbumFilter : FilterBase<AdminAlbumFilter>
    {
        /// <summary>
        /// 相册结果排序键
        /// </summary>
        public enum OrderBy
        {
            /// <summary>
            /// 创建时间
            /// </summary>
            AlbumID = 1,
            /// <summary>
            /// 更新时间
            /// </summary>
            UpdateDate = 2,
            /// <summary>
            /// 图片数
            /// </summary>
            TotalPhotos = 3,
            /// <summary>
            /// 所有者
            /// </summary>
            UserID = 4,

        }

        public AdminAlbumFilter()
        {
            this.IsDesc = true;
            this.PageSize = Consts.DefaultPageSize;
            this.Order = OrderBy.AlbumID;
        }

        [FilterItem(FormName = "username")]
        public string Username
        {
            get;
            set;
        }

        [FilterItem(FormName = "authorid")]
        public int? AuthorID
        {
            get;
            set;
        }

        [FilterItem(FormName = "albumid")]
        public int? AlbumID
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

        [FilterItem(FormName = "name")]
        public string Name
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

        [FilterItem(FormName="order")]
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