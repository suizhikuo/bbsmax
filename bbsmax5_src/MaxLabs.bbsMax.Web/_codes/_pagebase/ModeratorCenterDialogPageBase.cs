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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Logs;

/*
 版主管理对话框基类
 */
namespace MaxLabs.bbsMax.Web
{
    public class ModeratorCenterDialogPageBase : DialogPageBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!HasPermission)
            {
                ShowError("您没有权限对主题/帖子进行此操作！");
                return;
            }

            checkThread();

            if (_Request.IsClick("ok"))
            {
                bool success;
                try
                {
                    success = onClickOk();
                }
                catch (Exception ex)
                {
                    LogManager.LogException(ex);
                    ShowError(ex.Message);
                    return;
                }
                string msg = string.Empty;
                if (HasUnCatchedError)
                {
                    CatchError(delegate(ErrorInfo err)
                    {
                        msg = err.Message;
                        //errIndex++;
                        //msg += errIndex + "、" + err.Message + "<br />";
                    });
                }
                if (success)
                {
                    if (msg == string.Empty)
                        ShowSuccess("操作成功", true);
                    else
                        ShowWarning(false, "操作成功， 但出现了错误：<br />" + msg);
                }
                else
                {
                    ShowError(msg);
                }
            }
        }

        protected override int CurrentForumID
        {
            get
            {
                return CurrentForum.ForumID;
            }
        }

        protected virtual void checkThread()
        {
            if (this.ThreadList.Count == 0)
            {
                ShowError("请选择要操作的主题");
            }
        }

        protected virtual bool onClickOk()
        {
            return false;
        }

        private Forum m_currentForum;
        public Forum CurrentForum
        {
            get
            {
                if (m_currentForum == null)
                {
                    m_currentForum = ForumBO.Instance.GetForum(CodeName);
                }
                return m_currentForum;
            }
        }

        protected int[] ThreadIDs
        {
            get
            {
                return _Request.GetList<int>("threadids", new int[0]);
            }
        }

        protected string ThreadIDString
        {
            get
            {
                if (ThreadIDs.Length == 0)
                    return string.Empty;
                else
                    return StringUtil.Join(ThreadIDs);
            }
        }

        private string m_CodeName = null;
        protected string CodeName
        {
            get
            {
                if (m_CodeName == null)
                    m_CodeName = _Request.Get("codename");

                return m_CodeName;
            }
        }

        private ThreadCollectionV5 m_ThreadList;
        protected ThreadCollectionV5 ThreadList
        {
            get
            {
                if (m_ThreadList == null)
                {
                    int[] threadids = _Request.GetList<int>("threadids", new int[0]);
                    m_ThreadList = PostBOV5.Instance.GetThreads(threadids);
                }

                return m_ThreadList;
            }
        }

        protected bool EnableSendNotify
        {
            get
            {
                return _Request.Get<bool>("sendNotify", Method.Post, false);
            }
        }

        protected string ActionReason
        {
            get
            {
                return _Request.Get("actionReasonText", Method.Post);
            }
        }

        private ManageForumPermissionSetNode m_forumManagePermission;
        protected ManageForumPermissionSetNode ForumManagePermission
        {
            get
            {
                if (m_forumManagePermission == null)
                {
                    if (CurrentForum != null)
                    {
                        m_forumManagePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(CurrentForum.ForumID);
                    }
                }
                return m_forumManagePermission;
            }
        }

        protected virtual bool HasPermission
        {
            get
            {
                return false;
                //return
                //    CurrentForum.IsModerator(MyUserID)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.MoveThreads)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock)
                //|| ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick)
                //|| ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsUp)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads)
                //|| ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts)
                //;
            }
        }
    }
}