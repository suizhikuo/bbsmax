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

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class approvepost : ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ValidateUtil.HasItems<int>(PostIds) == false)
                ShowError("请选择要审核的帖子");
        }

        protected override bool onClickOk()
        {
            return PostBOV5.Instance.ApprovePosts(My, PostIds, EnableSendNotify, true, ActionReason);
        }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.ApprovePosts);
            }
        }

        protected int[] PostIds
        {
            get
            {
                return _Request.GetList<int>("postids", new int[0]);
            }
        }
        protected string PostIDString
        {
            get
            {
                return StringUtil.Join(PostIds);
            }
        }
    }
}