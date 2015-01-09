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
    public partial class threadexchanges : DialogPageBase
    {
        protected const int pageSize = 10;
        protected ThreadExchangeCollection ExchangeList;
        protected int totalCount;
        protected int totalMoney;
        protected BasicThread thread;
        protected UserPoint TradePoint;
        protected void Page_Load(object sender, EventArgs e)
        {
            int threadID = _Request.Get<int>("threadID", Method.Get, 0);

            if (threadID < 1)
                ShowError(new InvalidParamError("threadID").Message);

            thread = PostBOV5.Instance.GetThread(threadID);
            if (thread == null)
                ShowError("该主题不存在或者已被删除！");

            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(thread.ForumID);
            if (thread.PostUserID != MyUserID && (!permission.Can(My, ForumPermissionSetNode.Action.AlwaysViewContents)))
            {
                if (!thread.IsBuyed(My))
                {
                    ShowError("您还没有购买此主题，不能查看购买记录！");
                }
            }

            TradePoint = ForumPointAction.Instance.GetUserPoint(thread.PostUserID, ForumPointValueType.SellThread, thread.ForumID);

            int pageNumber = _Request.Get<int>("page",Method.Get,1);

            ExchangeList = PostBOV5.Instance.GetThreadExchanges(threadID, pageNumber, pageSize, out totalCount, out totalMoney);

            WaitForFillSimpleUsers<ThreadExchange>(ExchangeList);

            SetPager("list", null, pageNumber, pageSize, totalCount);
        }

    }
}