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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class setthreadistop : ModeratorCenterDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override bool onClickOk()
        {
            ThreadStatus threadStatus = _Request.Get<ThreadStatus>("stick", Method.Post, ThreadStatus.Normal);

            List<int> forumIDs = StringUtil.Split2<int>(_Request.Get("forumids", Method.Post, string.Empty));

            if (forumIDs.Contains(CurrentForumID))
            {
                forumIDs.Remove(CurrentForumID);
            }

            DateTime? endTime = null;
            int? time = _Request.Get<int>("time", Method.Post);
            if (time != null)
            {
                string locktimetype = _Request.Get("locktimetype", Method.Post);
                switch (locktimetype)
                {
                    case "0":
                        endTime = DateTimeUtil.Now.AddDays(time.Value);
                        break;
                    case "1":
                        endTime = DateTimeUtil.Now.AddHours(time.Value);
                        break;
                    case "2":
                        endTime = DateTimeUtil.Now.AddMinutes(time.Value);
                        break;
                    default:
                        endTime = null;
                        break;
                }
            }

            return PostBOV5.Instance.SetThreadsStickyStatus(My, CurrentForumID, forumIDs, ThreadIDs, threadStatus, null, false, EnableSendNotify, true, ActionReason);
               
        }

        private bool? m_CheckStick;
        protected bool CheckStick
        {
            get
            {

                if (m_CheckStick == null)
                {
                    if (ThreadList.Count == 1 && (ThreadList[0].ThreadStatus == MaxLabs.bbsMax.Enums.ThreadStatus.Sticky
                        || ThreadList[0].ThreadStatus == MaxLabs.bbsMax.Enums.ThreadStatus.GlobalSticky))
                    {
                        m_CheckStick = false;
                    }
                    else if (ThreadList.Count > 1 && (ThreadList[0].ThreadStatus == MaxLabs.bbsMax.Enums.ThreadStatus.Sticky
                        || ThreadList[0].ThreadStatus == MaxLabs.bbsMax.Enums.ThreadStatus.GlobalSticky))
                    {
                        m_CheckStick = false;
                    }
                    else
                        m_CheckStick = true;
                }

                return m_CheckStick.Value;
            }
        }

        private StickThreadCollection stickThreadsInForums;
        protected bool IsStickForum(int forumID)
        {
            if (ThreadList.Count > 1)
                return false;
            else if (ThreadList.Count == 1)
            {
                if (stickThreadsInForums == null)
                    stickThreadsInForums = PostBOV5.Instance.GetAllStickThreadInForums();

                return stickThreadsInForums.GetValue(ThreadList[0].ThreadID, forumID) != null;
            }
            else
                return false;
        }

        protected override bool HasPermission
        {
            get
            {
                return ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick)
                || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick);
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
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", delegate(Forum forum)
            {
                return true;
            }, out m_Forums, out m_ForumSeparators);
        }

        protected bool HasStickPermission(Forum forum)
        {
            ManageForumPermissionSetNode permission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);
            if (permission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick)
                || permission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick))
            {
                return true;
            }
            else
                return false;
        }
    }
}