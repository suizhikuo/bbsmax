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
    public class DiskFileFilter : FilterBase<DiskFileFilter>
    {

        [FilterItem(FormName="userid")]
        public int? UserID { get; set; }

        [FilterItem(FormName="Username")]
        public string Username { get; set; }

        [FilterItem(FormName="fileName")]
        public string Filename { get; set; }

        [FilterItem(FormName = "CreateDate_1",FormType=FilterItemFormType.BeginDate)]
        public DateTime? CreateDate_1 { get; set; }

        [FilterItem(FormName = "CreateDate_2", FormType = FilterItemFormType.EndDate)]
        public DateTime? CreateDate_2 { get; set; }

        [FilterItem(FormName="Size_1")]
        public long? Size_1 { get; set; }

        [FilterItem(FormName="SizeUnit_1")]
        public FileSizeUnit SizeUnit_1 { get; set; }

        [FilterItem(FormName = "Size_2")]
        public long? Size_2 { get; set; }

        [FilterItem(FormName = "SizeUnit_2")]
        public FileSizeUnit SizeUnit_2 { get; set; }

        [FilterItem( FormName="directoryName" )]
        public string DirectoryName { get; set; }

        [FilterItem(FormName="orderby")]
        public FileOrderBy? Order { get; set; }

        [FilterItem(FormName = "isdesc")]

        public bool? IsDesc { get; set; }

        [FilterItem(FormName="PageSize")]
        public int PageSize { get; set; }

    }
}