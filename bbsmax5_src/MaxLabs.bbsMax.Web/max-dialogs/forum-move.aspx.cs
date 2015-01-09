//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.StepByStepTasks;
using System.Collections.Generic;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class forum_move : AdminDialogPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Forum;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Forum == null)
            {
                ShowError(new InvalidParamError("forumid"));
            }

            if (Forum.ParentID == 0)
            {
                ShowError("“" + Forum.ForumName + "”是分类版块，不能进行移动操作");
            }

            if (Forum.ForumStatus == ForumStatus.Deleted || Forum.ForumStatus == ForumStatus.Joined || Forum.ForumStatus == ForumStatus.JoinTo)
            {
                ShowError("您要移动的版块处于不正常状态，不能进行移动操作");
            }

            if (_Request.IsClick("moveforum"))
            {
                MoveForum();
            }

        }

        private Forum m_Forum;
        protected Forum Forum
        {
            get
            {
                if (m_Forum == null)
                {
                    m_Forum = ForumBO.Instance.GetForum(_Request.Get<int>("forumid",Method.Get,0));
                }
                return m_Forum;
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
                if (forum.ForumID == Forum.ForumID)
                    return false;
                else
                    return true;
            }, out m_Forums, out m_ForumSeparators);

        }

        private void MoveForum()
        {
            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                int parentID = _Request.Get<int>("parentForum", Method.Post, 0);

                try
                {
                    success = ForumBO.Instance.MoveFourm(Forum.ForumID, parentID);
                    if (success == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
            }

            if (success)
                Return(true);
           
        }
    }
}