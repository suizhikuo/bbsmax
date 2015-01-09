//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Filters
{
    public class AdminTagFilter : FilterBase<AdminTagFilter>
    {
        public AdminTagFilter()
        {
            this.OrderBy = TagOrderKey.ID;
            this.IsDesc = true;
            this.PageSize = Consts.DefaultPageSize;
        }

        [FilterItem(FormName = "name")]
        public string Name
        {
            get;
            set;
        }

        [FilterItem(FormName = "islock")]
        public bool? IsLock
        {
            get;
            set;
        }

        [FilterItem(FormName = "type")]
        public TagType? Type
        {
            get;
            set;
        }

        [FilterItem(FormName = "totalelementsscopebegin")]
        public int? TotalElementsScopeBegin
        {
            get;
            set;
        }

        [FilterItem(FormName = "totalelementsscopeend")]
        public int? TotalElementsScopeEnd
        {
            get;
            set;
        }

        [FilterItem(FormName = "orderby")]
        public TagOrderKey OrderBy
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