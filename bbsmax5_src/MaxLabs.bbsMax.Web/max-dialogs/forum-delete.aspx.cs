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
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class forum_delete : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Forum; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Forum == null)
            {
                ShowError(new InvalidParamError("forumid"));
                return;
            }

            if (Forum.ForumStatus == ForumStatus.Joined || Forum.ForumStatus == ForumStatus.JoinTo)
            {
                ShowError("您要删除的版块处于不正常状态，不能进行删除操作");
            }

            if (Forum.ParentID == 0 && Forum.AllSubForums.Count > 0)
            {
                //当只有一个分类时  如果这个分类还有子版块  那么不允许删除这个分类
                //if (ForumManager.GetRootForums().Count == 1)
                if(ForumBO.Instance.GetCategories().Count == 1)
                {
                    ShowError("您要删除的分类版块有子版块，不能进行删除操作");
                }
            }

            if (_Request.IsClick("deleteforum"))
            {
                DeleteForum();
            }



        }

        private Forum m_Forum;
        protected Forum Forum
        {
            get
            {
                if (m_Forum == null)
                {
                    m_Forum = ForumBO.Instance.GetForum(_Request.Get<int>("forumid", Method.Get, 0));
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

        private void DeleteForum()
        {
            //using (new ErrorScope())
            //{
            bool beginTask = false;

            MessageDisplay msgDisplay = CreateMessageDisplay();

            try
            {
                int parentID = _Request.Get<int>("parentForum", Method.Post, 0);
                foreach (Forum forum in Forum.AllSubForums)
                {
                    ForumBO.Instance.MoveFourm(forum.ForumID, parentID);
                }

                beginTask = TaskManager.BeginTask(MyUserID, new DeleteForumTask(), Forum.ForumID.ToString());

            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }
            //}
            if (beginTask)
                Return(true);

        }
    }
}