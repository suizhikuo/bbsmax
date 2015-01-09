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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_attachment : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HasSomePermission == false)
            {
				ShowError("您没有权限管理附件");
            }
			
            if (_Request.IsClick("search"))
            {
                Search();
            }
            else if (_Request.IsClick("deletechecked"))
            {
                DeleteChecked();
            }
            else if (_Request.IsClick("deletesearched"))
            {
                DeletedSearchResult();
            }

        }

        protected bool HasPermission(Forum forum)
        {
            return AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID).HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads);
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
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, out m_Forums, out m_ForumSeparators);
            //ForumManager.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, false, out m_Forums, out m_ForumSeparators);
        }

        private Forum m_FirstHavePermissionForum;
        protected Forum FirstHavePermissionForum
        {
            get
            {
                if (m_FirstHavePermissionForum == null)
                {
                    checkForumPermission();
                }
                return m_FirstHavePermissionForum;
            }
        }

        private bool? m_HasSomePermission;
        protected bool HasSomePermission
        {
            get
            {
                if (m_HasSomePermission == null)
                {
                    checkForumPermission();
                }
                return m_HasSomePermission.Value;
            }
        }

        private void checkForumPermission()
        {
            foreach (Forum forum in Forums)
            {
                if (AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID).HasPermissionForSomeone(My, MaxLabs.bbsMax.Settings.ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                {
                    m_HasSomePermission = true;
                    m_FirstHavePermissionForum = forum;
                    return;
                }
            }
            m_HasSomePermission = false;
            m_FirstHavePermissionForum = null;
        }

        private bool? m_IsOwner;
        protected bool IsOwner
        {
            get
            {
                if (m_IsOwner == null)
                {
                    m_IsOwner = My.IsOwner;
                }
                return m_IsOwner.Value;
            }
        }


        private AttachmentFilter m_Filter;

        public AttachmentFilter Filter
		{
			get 
            {
                if (m_Filter == null)
                {
                    m_Filter = AttachmentFilter.GetFromFilter("filter");

                    if (m_Filter.IsNull)
                    {
                        m_Filter = new AttachmentFilter();

                        if(IsOwner == false)
                            m_Filter.ForumID = FirstHavePermissionForum.ForumID;

                        m_Filter.PageSize = Consts.DefaultPageSize;
                        m_Filter.IsDesc = true;
                        m_Filter.MaxFileSizeUnit = FileSizeUnit.K;
                        m_Filter.MinFileSizeUnit = FileSizeUnit.K;
                        m_Filter.Order = AttachmentFilter.OrderBy.AttachmentID;
                    }
                }
                return m_Filter;
            }
		}

        private AttachmentCollection m_AttachmentList;

        public AttachmentCollection AttachmentList
		{
			get
			{
                if (m_AttachmentList == null)
                {
                    m_AttachmentList = PostBOV5.Instance.GetAttachments(My, Filter, _Request.Get<int>("page", Method.Get, 1), out m_TotalCount);
                }
                return m_AttachmentList;
			}
		}

        private int m_TotalCount = int.MinValue;
        public int TotalCount
        {
            get
            {
                if (m_TotalCount == int.MinValue)
                {
                    m_AttachmentList = PostBOV5.Instance.GetAttachments(My, Filter, _Request.Get<int>("page", Method.Get, 1), out m_TotalCount);
                }
                return m_TotalCount;
            }
        }

        private PostCollectionV5 m_posts;
        protected PostCollectionV5 Posts
        {
            get
            {
                if (m_posts == null)
                {
                    List<int> postIDs = new List<int>();
                    foreach (Attachment attachment in AttachmentList)
                    {
                        if (false == postIDs.Contains(attachment.PostID))
                        {
                            postIDs.Add(attachment.PostID);
                        }
                    }

                    m_posts = PostBOV5.Instance.GetPosts(postIDs); 
                }

                return m_posts;
            }
        }

        private ThreadCollectionV5 m_Threads;
        protected ThreadCollectionV5 Threads
        {
            get
            {
                if (m_Threads == null)
                {
                    List<int> threadIDs = new List<int>();
                    foreach (PostV5 post in Posts)
                    {
                        if (false == threadIDs.Contains(post.ThreadID))
                        {
                            threadIDs.Add(post.ThreadID);
                        }
                    }

                    m_Threads = PostBOV5.Instance.GetThreads(threadIDs);
                }

                return m_Threads;
            }
        }

        protected string GetAttachmentUrl(int attachmentID)
        {
            return BbsUrlHelper.GetAttachmentUrl(attachmentID);
        }

        protected string GetUserName(int postID)
        {
            foreach (PostV5 post in Posts)
            {
                if (postID == post.PostID)
                    return post.Username;
            }
            return string.Empty;
        }

        protected string GetThreadUrl(int postID)
        {
            foreach (PostV5 post in Posts)
            {
                if (postID == post.PostID)
                {
                    Forum forum = ForumBO.Instance.GetForum(post.ForumID);
                    return BbsUrlHelper.GetThreadUrl(forum.CodeName,post.ThreadID);
                }
            }
            return string.Empty;
        }
        protected string GetThreadSubject(int postID)
        {
            foreach (PostV5 post in Posts)
            {
                if (postID == post.PostID)
                {
                    foreach (BasicThread thread in Threads)
                    {
                        if (thread.ThreadID == post.ThreadID)
                        {
                            return thread.SubjectText;
                        }
                    }
                }
            }
            return string.Empty;
        }

        protected int GetForumID(int postID)
        {
            foreach (PostV5 post in Posts)
            {
                if (postID == post.PostID)
                {
                    return post.ForumID;
                }
            }
            return 0;
        }

        //protected string GetThreadUrl(Thread thread)
        //{
        //    Forum forum = ForumManager.GetForum(thread.ForumID);
        //    return BbsUrlHelper.GetThreadUrl(forum.CodeName, thread.RedirectThreadID);
        //}

        protected string GetFileSize(long fileSize)
        {
            return ConvertUtil.FormatSize(fileSize);
        }

        protected int PageSize
        {
            get
            {
                return Filter.PageSize;
            }
        }

        private void Search()
        {

            AttachmentFilter filter = AttachmentFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    List<int> attachmentIDs = StringUtil.Split2<int>(_Request.Get("attachmentIDs", Method.Post));

                    bool success = PostBOV5.Instance.DeleteAttachments(My, _Request.Get<int>("ForumID", Method.Post, 0), attachmentIDs);

                    if (success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    { 
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }

        private void DeletedSearchResult()
        {
            AttachmentFilter filter = AttachmentFilter.GetFromFilter("filter");

            if (TaskManager.BeginTask(MyUserID, new DeleteAttachmentTask(), filter.ToString()))
            {
                
            }
        }


    }
}