//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;

using MaxLabs.bbsMax.Filters;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_shielduers : AdminPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ForumID != 0 && Forum == null)
            {
                ShowError("版块不存在");
            }

            //if (!(AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(MyUserID, ManageUserPermissionSet.ActionWithTarget.FullSiteBanUser)||Forum.ManagePermission.HasPermissionForSomeone(MyUserID, ManageForumPermissionSetNode.ActionWithTarget.BanUser)))

            if (ForumID == 0)
            {
                if (!AllSettings.Current.BackendPermissions.HasPermissionForSomeone(My, BackendPermissions.ActionWithTarget.Manage_BanUsers))
                {
                    ShowError("没有权限整站屏蔽用户！");
                    return;
                }
            } 
            else if (AllSettings.Current.BackendPermissions.HasPermissionForSomeone(My
             , BackendPermissions.ActionWithTarget.Manage_BanUsers) == false && AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(ForumID).HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser) == false)
            {
                {
                    ShowError("没有权限在版块" + Forum.ForumNameText + "屏蔽用户！");
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

        private List<int> m_NoPermissionForumIDs;
        protected List<int> NoPermissionForumIDs
        {
            get
            {
                if (m_NoPermissionForumIDs == null)
                {
                    m_NoPermissionForumIDs = new List<int>();
                    if (AllSettings.Current.BackendPermissions.HasPermissionForSomeone(My, BackendPermissions.ActionWithTarget.Manage_BanUsers))
                    {
                    }
                    else
                    {
                        ForumCollection forums = ForumBO.Instance.GetAllForums();
                        foreach (Forum forum in forums)
                        {
                            if (AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID).HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser) == false)
                            {
                                m_NoPermissionForumIDs.Add(forum.ForumID);
                            }
                        }
                    }
                }
                return m_NoPermissionForumIDs;
            }
        }

        private void Unban()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int unbanUserCount;
            int[] userIds = _Request.GetList<int>("userids", Method.Post, new int[0]);

            if (ForumID == 0)
            {
                unbanUserCount = UserBO.Instance.UnBanUsers(My, userIds);//解除整站屏蔽
            }
            else
            {
                Forum forum = ForumBO.Instance.GetForum(ForumID);
                unbanUserCount = UserBO.Instance.UnBanUsers(My, userIds, ForumID);//解除屏蔽
            }
        }



        private void ShieldUser()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("username", "banneddate");

            string username = _Request.Get("username", Method.Post,string.Empty,false);
            DateTime bannedDate = DateTimeUtil.ParseEndDateTime(_Request.Get("banneddate", Method.Post));
            User user = UserBO.Instance.GetUser(username);

            if (user == null)
            {
                msgDisplay.AddError( "用户名错误！");
            } 

            if (bannedDate < DateTimeUtil.Now)
            {
                msgDisplay.AddError( "您输入的日期已过时！");
            }

            if (msgDisplay.HasAnyError())
                return;

            if (ForumID > 0)
            {
                if (AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(ForumID).Can(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser,user.UserID))
                {
                    UserBO.Instance.BanUser(My, user.UserID, Forum.ForumID, bannedDate, string.Empty);     
                }
                else
                {
                    msgDisplay.AddError(string.Format("您没有权限在 {0} 版块屏蔽用户 {1}", Forum.ForumName, user.Username));
                }
            }
            else
            {
                //if (AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.FullSiteBanUser, user.UserID))
                  if (AllSettings.Current.BackendPermissions.Can(My
               , BackendPermissions.ActionWithTarget.Manage_BanUsers, user.UserID))
                {
                    UserBO.Instance.BanUser(My, user.UserID, bannedDate, string.Empty);
                  }
                else
                {
                    msgDisplay.AddError(string.Format("您没有权限屏蔽 {0}", user.Username));   
                  }
            }

            _Request.Clear(Method.Post);
        }

        

        protected bool HasPermission(int forumID)
        {
            return false;
        }

        protected Forum Forum
        {
            get
            {
                return ForumBO.Instance.GetForum(ForumID);
            }
        }

        protected int ForumID
        {
            get
            {
                return _Request.Get<int>("forumid", Method.Get, 0);
            }
        }

        protected int PageSize
        {
            get
            {
                return 80;
            }
        }

        private BannedUserCollection m_BannedUserList = null;
        private int m_totalCount = 0;

        protected BannedUserCollection BannedUserList
        {
            get
            {
                if (m_BannedUserList == null)
                {
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);
                    
                    m_BannedUserList = BannedUserProvider.GetBannedInfos(ForumID, PageSize, pageNumber , out m_totalCount);
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
                return m_totalCount;
            }
        }

        protected string GetForumsTree(string style1, string style2, string nopermissionStyle)
        {
            return ForumBO.Instance.BuildForumsTreeHtml(0, style1, delegate(Forum forum)
            {
                string style;
                string linkClass = " class=\"current\" ";
                //string color = "color:#999;";
                if (NoPermissionForumIDs.Contains(forum.ForumID))
                {
                    style = nopermissionStyle;
                    //color = string.Empty;
                }
                else
                    style = style2;
                if (forum.ForumID != ForumID)
                {
                    linkClass = string.Empty;
                }

                return string.Format(style, forum.ForumID, forum.ForumName, "{0}", linkClass);
            });
        }
    }
}