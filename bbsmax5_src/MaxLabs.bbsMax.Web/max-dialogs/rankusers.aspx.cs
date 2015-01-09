//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class rankusers : DialogPageBase
    {
        protected const int pageSize = 10;
        protected ThreadRankCollection threadRankList;
        protected int totalCount;
        protected void Page_Load(object sender, EventArgs e)
        {
            int threadID = _Request.Get<int>("threadid", Method.Get, 0);
            if (threadID < 0)
            {
                ShowError(new InvalidParamError("threadid"));
            }

            int pageNumber = _Request.Get<int>("page",Method.Get,1);

            threadRankList = PostBOV5.Instance.GetThreadRanks(threadID, pageNumber, pageSize, out totalCount);

            WaitForFillSimpleUsers<ThreadRank>(threadRankList);

           //SetPager("list", string.Format("rankusers.aspx?threadid={0}&page={1}&isdialog=1", threadID, "{0}"), pageNumber, pageSize, totalCount);
        }

    }
}