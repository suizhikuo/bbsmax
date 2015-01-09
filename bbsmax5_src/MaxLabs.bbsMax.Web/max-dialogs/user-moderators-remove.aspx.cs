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
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class user_moderators_remove : AdminDialogPageBase
    {

        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Manage_Moderator;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Forum == null)
            {
                ShowError("不存在该版块");
                return;
            }

            RemoveModerators();
        }


        private void RemoveModerators()
        {
            ForumBO.Instance.RemoveModerator(My, ForumID, UserID);
            Return(true);
        }


        private Forum m_forum;
        private Forum Forum
        {
            get
            {
                if (m_forum == null)
                {
                    m_forum = ForumBO.Instance.GetForum(this.ForumID);
                }
                return m_forum;
            }
        }

        protected int ForumID
        {
            get
            {
                return _Request.Get<int>("forumid", Method.Get, 0);
            }
        }

        protected int UserID
        {
            get
            {
                return _Request.Get<int>("userid", Method.Get, 0);
            }
        }
    }
}