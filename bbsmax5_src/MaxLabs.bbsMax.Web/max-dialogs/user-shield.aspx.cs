//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

#if !Passport

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
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class user_shield : DialogPageBase
    {

        bool hasPermission = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (AllSettings.Current.BackendPermissions.HasPermissionForSomeone(My
                , BackendPermissions.ActionWithTarget.Manage_BanUsers))
            {
                hasPermission = true;
            }
            else
            {
                foreach (Forum forum in ForumBO.Instance.GetAllForums())
                {
                    if (forum.ManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser))
                    {
                        hasPermission = true;
                        break;
                    }
                }
                if (hasPermission == false)
                    ShowError(new NoPermissionBanUserError());
            }

            if (User == null)
            {
                ShowError(new UserNotExistsError("userid", userID));
                return;
            }

            if (_Request.IsClick("ok"))
            {
                BanUser();
            }
        }


        private void BanUser()
        {

            MessageDisplay msgDisplay = CreateMessageDisplay();
            string cause = _Request.Get("cause", Method.Post);
            int BannedMode = _Request.Get<int>("shield", Method.Post, 3);
            switch (BannedMode)
            {
                case 0:
                    UserBO.Instance.UnBanUsers(My, new int[] { User.UserID });
                    break;
                case 1:
                    DateTime endDate = DateTimeUtil.ParseEndDateTime(_Request.Get("endDate.0", Method.Post));
                    UserBO.Instance.BanUser(My, User.UserID, endDate, cause);
                    break;
                case 2:

                    Dictionary<int, DateTime> foruminfo = new Dictionary<int, DateTime>();
                    int[] forumIds = _Request.GetList<int>("forums", Method.Post, new int[0]);

                    foreach (int id in forumIds)
                    {
                        foruminfo.Add(id, DateTimeUtil.ParseEndDateTime(_Request.Get("enddate." + id, Method.Post)));
                    }

                    UserBO.Instance.BanUser(My, User.UserID, foruminfo, cause);
                    break;
                default: break;
            }

            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);

                });
            }
            else
            {
                 ShowSuccess("操作成功", true);
            }
        }

        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        protected string GetForumPath(int forumID)
        {

            StringBuffer sb = new StringBuffer();

            ForumCollection parentForums = ForumBO.Instance.GetAllParentForums(forumID);

            foreach (Forum f in parentForums)
            {
                sb += f.ForumID;
                sb += "-";
            }

            sb += forumID;
            return sb.ToString();
        }

        protected bool IsForumBanned
        {
            get
            {
                return BannedUserProvider.IsForumBanned(User.UserID);
            }
        }

        BannedUserCollection m_bannedForum = null;
        protected BannedUserCollection BannedForums
        {
            get
            {
                if (m_bannedForum == null)
                    m_bannedForum = BannedUserProvider.GetUserBannedForumInfo(User.UserID);
                return m_bannedForum;
            }
        }

        public bool FullSiteBanned
        {
            get
            {
                return BannedUserProvider.IsFullSiteBanned(User.UserID);
            }
        }

        private int userID
        {
            get
            {
                return _Request.Get<int>("userid", Method.All, 0);
            }
        }

        public bool IsBanned
        {
            get
            {
                return BannedUserProvider.Contains(User.UserID);
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, out m_Forums, out m_ForumSeparators);
            //ForumManager.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, false, out m_Forums, out m_ForumSeparators);
        }

        protected bool CanBan(int forumID)
        {
            if (CanFullSiteBan)
                return true;
            return AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID).Can(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser, User.UserID);
        }

        private bool? m_CanFullSiteBan;
        protected bool CanFullSiteBan
        {
            get
            {
                if (m_CanFullSiteBan == null)
                {
                    m_CanFullSiteBan = AllSettings.Current.BackendPermissions.Can(My
                 , BackendPermissions.ActionWithTarget.Manage_BanUsers, User.UserID);
                }
                return m_CanFullSiteBan.Value;
            }
        }

        protected bool IsBannedByForum(int forumID)
        {
            foreach (BannedUser bi in this.BannedForums)
            {
                if (bi.ForumID == forumID) return true;
            }
            return false;
        }

        protected string GetBannedEndDate(int forumID)
        {
            foreach (BannedUser bi in this.BannedForums)
            {
                if (bi.ForumID == forumID) return OutputDateTime(bi.EndDate);
            }
            return "";
        }

        private bool m_UserInited = false;
        private User m_User = null;
        protected new User User
        {
            get
            {
                User result = m_User;

                if (m_UserInited == false)
                {
                    if (userID <= 0)
                    {
                        result = UserBO.Instance.GetUser(_Request.Get("username", Method.All, string.Empty, false));
                        if (result != null && result.UserID <= 0)
                            result = null;
                    }
                    else
                    {
                        result = UserBO.Instance.GetUser(userID);
                    }

                    m_User = result;
                    m_UserInited = true;
                }

                return result;
            }
        }

        private BanUserOperationCollection m_BanUserList=null;
        protected BanUserOperationCollection BanUserList
        {
            get 
            {
                if(_Request.Get<int>("tabid", Method.Get, 1)==2)
                    m_BanUserList = LogManager.GetBanUserOperationCollectionByUserID(userID);
                return m_BanUserList;
            }
        }
    }
}

#endif