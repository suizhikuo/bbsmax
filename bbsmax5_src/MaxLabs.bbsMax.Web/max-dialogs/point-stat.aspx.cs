//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class point_stat : AdminDialogPageBase
    {
        protected int Count, ProduceCount,ConsumeCount;
        protected void Page_Load(object sender, EventArgs e)
        {
            m_Filter = PointLogFilter.GetFromFilter("filter");
            UserPointType pointType = _Request.Get<UserPointType>("point", Method.Get, UserPointType.Point1);
            PointLogBO.Instance.GetPointStatInfo(Filter, pointType, out Count, out ProduceCount, out ConsumeCount);

        }
        PointLogFilter m_Filter;
        protected PointLogFilter Filter
        {
            get
            {
                return m_Filter;
            }
        }
    }
}