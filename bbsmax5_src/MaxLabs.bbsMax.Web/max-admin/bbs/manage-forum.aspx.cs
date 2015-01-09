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
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MaxLabs.bbsMax.Web.max_pages.admin;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_forum : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Forum; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("saveforums"))
                SaveForums();
        }

        protected bool CanManageModerator
        {
            get
            {
                return AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Manage_Moderator);
            }
        }

        protected int GetBannedUserCount(int forumID)
        {
            return BannedUserProvider.GetBannedUserCount(forumID);
        }

        protected ModeratorCollection GetModerators(int forumID)
        {
            ModeratorCollection moderators = ForumBO.Instance.GetForum(forumID, false).Moderators;
            FillSimpleUsers<Moderator>(moderators);
            return moderators;
        }

        private void SaveForums()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            ForumCollection forums = ForumBO.Instance.GetAllForums();

            List<int> forumIDs = new List<int>();
            List<int> sortOrders = new List<int>();
            foreach (Forum forum in forums)
            {
                if (forum.ForumID > 0)
                {
                    forumIDs.Add(forum.ForumID);
                    int sortOrder = _Request.Get<int>("sortorder_" + forum.ForumID, Method.Post, 0);

                    sortOrders.Add(sortOrder);
                }
            }
            try
            {
                using (new ErrorScope())
                {
                    bool success = ForumBO.Instance.UpdateForums(My, forumIDs, sortOrders);
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }


        protected string GetForumThreadCatalogNames(int forumID,string separator)
        {
            ThreadCatalogCollection threadCatalogs = ForumBO.Instance.GetThreadCatalogs(forumID);
            StringBuilder names = new StringBuilder();
            foreach (ThreadCatalog catalog in threadCatalogs)
            {
                names.Append(catalog.ThreadCatalogName).Append(separator);
            }

            if (names.Length > 0)
                return names.ToString(0,names.Length - separator.Length);

            return "(无)";
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
            ForumBO.Instance.GetTreeForums("l", delegate(Forum forum) { return true; }, out m_Forums, out m_ForumSeparators);
        }

        protected bool CanEdit(Forum forum)
        {
            if (forum.ForumStatus == ForumStatus.Deleted || forum.ForumStatus == ForumStatus.JoinTo || forum.ForumStatus == ForumStatus.Joined)
            {
                return false;
            }
            return true;
        }
    }
}