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
    public class PaylogFilter : FilterBase<PaylogFilter>
    {

        [FilterItem(FormName = "username")]
        public string Username { get; set; }

        [FilterItem(FormType = FilterItemFormType.BeginDate)]
        public DateTime? BeginDate { get; set; }


        [FilterItem(FormType = FilterItemFormType.EndDate)]
        public DateTime? EndDate { get; set; }

        [FilterItem(FormName = "orderno")]
        public string OrderNo { get; set; }

        [FilterItem(FormName = "transactionno")]
        public string TransactionNo { get; set; }

        [FilterItem(FormName = "payment")]
        public byte Payment { get; set; }

        [FilterItem(FormName = "beginamount")]
        public decimal BeginAmount { get; set; }

        [FilterItem(FormName = "endamount")]
        public decimal EndAmount { get; set; }

        [FilterItem(FormName = "paytype")]
        public byte PayType { get; set; }

        [FilterItem(FormName = "beginvalue")]
        public int BeginValue { get; set; }

        [FilterItem(FormName = "endvalue")]
        public int EndValue { get; set; }

        [FilterItem]
        public byte State { get; set; }
    }
}