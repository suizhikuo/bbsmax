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
    public partial class attachmentexchanges : DialogPageBase
    {
        protected const int pageSize = 10;
        protected AttachmentExchangeCollection ExchangeList;
        protected int totalCount;
        protected int totalMoney;
        protected Attachment attachment;
        protected UserPoint TradePoint;
        protected void Page_Load(object sender, EventArgs e)
        {
            int attachmentID = _Request.Get<int>("attachmentID", Method.Get, 0);

            if (attachmentID < 1)
                ShowError(new InvalidParamError("attachmentID").Message);

            attachment = PostBOV5.Instance.GetAttachment(attachmentID, false);
            if (attachment == null)
                ShowError("该附件不存在或者已被删除！");

            PostV5 post = PostBOV5.Instance.GetPost(attachment.PostID, false);
            if (post == null)
            {
                ShowError("该附件不存在或者已被删除！");
            }

            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(post.ForumID);
            if (post.UserID != MyUserID && (!permission.Can(My, ForumPermissionSetNode.Action.AlwaysViewContents)))
            {
                if (!attachment.IsBuyed(My))
                {
                    ShowError("您还没有购买此附件，不能查看购买记录！");
                }
            }

            TradePoint = ForumPointAction.Instance.GetUserPoint(post.UserID, ForumPointValueType.SellAttachment, post.ForumID);

            int pageNumber = _Request.Get<int>("page",Method.Get,1);

            ExchangeList = PostBOV5.Instance.GetAttachmentExchanges(attachmentID, pageNumber, pageSize, out totalCount, out totalMoney);

            WaitForFillSimpleUsers<AttachmentExchange>(ExchangeList);

            SetPager("list", null, pageNumber, pageSize, totalCount);
        }

    }
}