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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs.V5.forum
{
    public partial class revertthread : ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override bool onClickOk()
        {
            return PostBOV5.Instance.RestoreThreads(My, CurrentForumID, ThreadIDs, false, EnableSendNotify, true, ActionReason);
        }

        protected override bool HasPermission
        {
            get
            {
                //有回收权限  就能还原
                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled);
            }
        }
    }
}