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
    public partial class manage_unapprovedpost : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HasSomePermission == false)
            {
                ShowError("您没有权限审核帖子的权限");
            }
			
            if (_Request.IsClick("search"))
            {
                Search();
            }
            else if (_Request.IsClick("deletechecked"))
            {
                DeleteChecked();
            }
            else if (_Request.IsClick("approvedchecked"))
            {
                ApprovedChecked();
            }
            else if (_Request.IsClick("deletesearched"))
            {
                DeletedSearchResult();
            }
            else if (_Request.IsClick("approvedsearched"))
            {
                ApprovedSearchResult();
            }
        }

        protected bool HasPermission(Forum forum)
        {
            ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);

            return managePermission.Can(My, ManageForumPermissionSetNode.Action.ApprovePosts);
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
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", null, out m_Forums, out m_ForumSeparators);
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

                ManageForumPermissionSetNode managePermission = PostBOV5.Instance.GetForumPermissonSet(forum);
               
                if (managePermission.Can(My, MaxLabs.bbsMax.Settings.ManageForumPermissionSetNode.Action.ApprovePosts))
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


        private PostFilter m_PostForm;

        public PostFilter PostForm
		{
			get 
            {
                if (m_PostForm == null)
                {
                    m_PostForm = PostFilter.GetFromFilter("filter");

                    if (m_PostForm.IsNull)
                    {
                        m_PostForm = new PostFilter();

                        if(IsOwner == false)
                            m_PostForm.ForumID = FirstHavePermissionForum.ForumID;

                        m_PostForm.PageSize = Consts.DefaultPageSize;
                        m_PostForm.IsDesc = true;

                        int userid = _Request.Get<int>("userid", Method.Get, 0);
                        if (userid > 0)
                            m_PostForm.UserID = userid;

                        m_PostForm.SearchMode = SearchArticleMethod.Default;
                    }

                    m_PostForm.IsUnapproved = true;
                }
                return m_PostForm;
            }
		}

        private int? m_pageNumber;
        protected int PageNumber
        {
            get
            {
                if (m_pageNumber == null)
                {
                    m_pageNumber = _Request.Get<int>("page", Method.Get, 1);
                }
                return m_pageNumber.Value;
            }
        }

        private PostCollectionV5 m_PostList;
        protected PostCollectionV5 PostList
        {
            get
            {
                if (m_PostList == null)
                {
                    GetData();
                }
                return m_PostList;
            }
        }

        private ThreadCollectionV5 m_ThreadList;
        protected ThreadCollectionV5 ThreadList
        {
            get
            {
                if (m_ThreadList == null)
                {
                    GetData();
                }
                return m_ThreadList;
            }
        }

        private void GetData()
        {
            int count;
            m_PostList = PostBOV5.Instance.GetPosts(My, PostForm, PageNumber, out count);
            m_TotalCount = count;

            PostBOV5.Instance.ProcessKeyword(m_PostList, ProcessKeywordMode.FillOriginalText);

            List<int> threadIDs = new List<int>();
            foreach (PostV5 post in m_PostList)
            {
                if (threadIDs.Contains(post.ThreadID) == false)
                    threadIDs.Add(post.ThreadID);
            }

            m_ThreadList = PostBOV5.Instance.GetThreads(threadIDs);

            PostBOV5.Instance.ProcessKeyword(m_ThreadList, ProcessKeywordMode.FillOriginalText);

            SetPager("list", null, PageNumber, PostForm.PageSize, count);
        }

        protected string GetThreadSubject(int threadID)
        {
            BasicThread thread = ThreadList.GetValue(threadID);
            if (thread == null)
                return string.Empty;

            return thread.SubjectText;
        }

        protected string GetThreadUrl(int threadID)
        {
            BasicThread thread = ThreadList.GetValue(threadID);
            if (thread == null)
                return string.Empty;

            Forum forum = thread.Forum;
            return BbsUrlHelper.GetThreadUrl(forum.CodeName, thread.RedirectThreadID, 1, thread.ThreadTypeString, "unapproveposts");
        }

        protected string GetPostContent(PostV5 post)
        {
            if (StringUtil.GetByteCount(post.ContentForSearch) > 300)
                return StringUtil.CutString(post.ContentForSearch, 300);

            return post.ContentForSearch;
        }

      

        private int? m_TotalCount;
        public int TotalCount
        {
            get
            {
                if (m_TotalCount == null)
                    GetData();

                return m_TotalCount.Value;
            }
        }


        protected int PageSize
        {
            get
            {
                return PostForm.PageSize;
            }
        }

        private void Search()
        {
            PostFilter filter = PostFilter.GetFromForm();
            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    List<int> postIDs = StringUtil.Split2<int>(_Request.Get("postids", Method.Post));

                    bool updatePoint = _Request.Get("updatePoint", Method.Post, "1") == "1";

                    //TODO:
                    // updatePoint ??
                    bool success = PostBOV5.Instance.DeletePosts(My, postIDs, false, true, false, string.Empty);
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            msgDisplay.AddError(err);
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
            PostFilter filter = PostForm;//TopicFilter.GetFromFilter("filter");

            StringList param = new StringList();

            param.Add(filter.ToString());
            param.Add(_Request.Get("updatePoint", Method.Post, "1"));

            if (TaskManager.BeginTask(MyUserID, new DeletePostTask(), param.ToString()))
            {
                
            }
        }

        private void ApprovedChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    List<int> postIDs = StringUtil.Split2<int>(_Request.Get("postids", Method.Post));


                    bool success = PostBOV5.Instance.ApprovePosts(My, postIDs, true, true, string.Empty);
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo err)
                        {
                            msgDisplay.AddError(err);
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

        private void ApprovedSearchResult()
        {
            PostFilter filter = PostForm;//TopicFilter.GetFromFilter("filter");

            StringList param = new StringList();

            param.Add(filter.ToString());

            if (TaskManager.BeginTask(MyUserID, new ApprovePostTask(), param.ToString()))
            {

            }
        }

    }
}