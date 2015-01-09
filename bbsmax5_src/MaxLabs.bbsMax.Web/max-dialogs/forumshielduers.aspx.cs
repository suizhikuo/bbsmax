//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class forumshielduers : DialogPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Forum == null)
            {
                ShowError(new InvalidParamError("ForumID"));
            }

            if (AllSettings.Current.BackendPermissions.HasPermissionForSomeone(My
                , BackendPermissions.ActionWithTarget.Manage_BanUsers))
            {
            }
            else
            {
                if (AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(Forum.ForumID).HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser) == false)
                {
                    ShowError("没有权限查看屏蔽用户！");
                    return;
                }
            }

            if (_Request.IsClick("shielduser"))
            {
                ShieldUser();
            }
            else if (_Request.IsClick("unban"))
            {
                Unban();
            }
        }

        private Forum m_Forum;
        protected Forum Forum
        {
            get
            {
                if (m_Forum == null)
                {
                    m_Forum = ForumBO.Instance.GetForum(_Request.Get<int>("forumID", Method.Get, 0));
                }

                return m_Forum;
            }
        }

        private void Unban()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int unbanUserCount;
            int[] userIds = _Request.GetList<int>("userids", Method.Post, new int[0]);

            unbanUserCount = UserBO.Instance.UnBanUsers(My, userIds, Forum.ForumID);//解除屏蔽
        }


        private void ShieldUser()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("username", "banneddate");

            string username = _Request.Get("username", Method.Post,string.Empty,false);
            DateTime bannedDate = DateTimeUtil.ParseEndDateTime(_Request.Get("banneddate", Method.Post));
            User user = UserBO.Instance.GetUser(username);

            if (user == null)
            {
                msgDisplay.AddError("用户名错误！");
            }

            if (bannedDate < DateTimeUtil.Now)
            {
                msgDisplay.AddError("您输入的日期已过时！");
            }

            if (msgDisplay.HasAnyError())
                return;


            if (AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(Forum.ForumID).Can(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser, user.UserID))
            {
                UserBO.Instance.BanUser(My, user.UserID, Forum.ForumID, bannedDate, string.Empty);
                _Request.Clear(Method.Post);
            }
            else
            {
                msgDisplay.AddError(string.Format("您没有权限在 {0} 版块屏蔽用户 {1}", Forum.ForumName, user.Username));
            }

        }

        protected int ForumID
        {
            get { return _Request.Get<int>("forumid", Method.Get, 0); }
        }

        protected int PageSize
        {
            get { return 80; }
        }

        private BannedUserCollection m_BannedUserList = null;
        private int m_TotalCount = 0;

        protected BannedUserCollection BannedUserList
        {
            get
            {
                if (m_BannedUserList == null)
                {
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);

                    m_BannedUserList = BannedUserProvider.GetBannedInfos(ForumID, PageSize, pageNumber, out m_TotalCount);
                    FillSimpleUsers<BannedUser>(m_BannedUserList);
                }
                return m_BannedUserList;
            }
        }

        protected int TotalCount
        {
            get
            {
                BannedUserCollection temp = BannedUserList;
                return m_TotalCount;
            }
        }
    }
}