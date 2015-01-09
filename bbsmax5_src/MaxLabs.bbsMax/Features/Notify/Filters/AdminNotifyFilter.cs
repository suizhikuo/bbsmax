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
    public class AdminNotifyFilter : FilterBase<AdminNotifyFilter>
    {
        public AdminNotifyFilter() { this.PageSize = Consts.DefaultPageSize; }

        [FilterItem(FormName = "owner")]
        public string Owner
        {
            get;
            set;
        }

        [FilterItem(FormName = "searchmode")]
        public int SearchMode
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

        [FilterItem(FormName = "notifytype")]
        public int? NotifyType
        {
            get;
            set;
        }

        [FilterItem(FormName = "isread")]
        public bool? IsRead
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