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
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs.V5.forum
{
    public partial class judgementthread : ModeratorCenterDialogPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override bool onClickOk()
        {
            int judgementID = _Request.Get<int>("ddlJudgementThread", Method.Post, 0);
            return PostBOV5.Instance.JudgementThreads(My, ThreadIDs, CurrentForum.ForumID, judgementID, false, EnableSendNotify, true, ActionReason);
        }

        protected JudgementCollection Judgements { get { return AllSettings.Current.JudgementSettings.Judgements; } }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads);
            }
        }
    }
}