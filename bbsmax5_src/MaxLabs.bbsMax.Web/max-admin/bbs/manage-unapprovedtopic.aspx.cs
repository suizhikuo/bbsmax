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
    public partial class manage_unapprovedtopic : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HasSomePermission == false)
            {
                ShowError("您没有权限审核主题的权限");
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
            else if (_Request.IsClick("approvedchecked"))
            {
                ApproveChecked();
            }
            else if (_Request.IsClick("approvedsearched"))
            {
                ApproveSearchResult();
            }
        }

        protected bool HasPermission(Forum forum)
        {
            ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forum.ForumID);

            return managePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads);
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
               
                if (managePermission.HasPermissionForSomeone(My, MaxLabs.bbsMax.Settings.ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads))
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


        private TopicFilter m_TopicForm;

        public TopicFilter TopicForm
		{
			get 
            {
                if (m_TopicForm == null)
                {
                    m_TopicForm = TopicFilter.GetFromFilter("filter");

                    if (m_TopicForm.IsNull)
                    {
                        m_TopicForm = new TopicFilter();

                        if(IsOwner == false)
                            m_TopicForm.ForumID = FirstHavePermissionForum.ForumID;

                        m_TopicForm.PageSize = Consts.DefaultPageSize;
                        m_TopicForm.IncludeStick = true;
                        m_TopicForm.IncludeValued = true;
                        m_TopicForm.IsDesc = true;

                        int userid = _Request.Get<int>("userid", Method.Get, 0);
                        if (userid > 0)
                            m_TopicForm.UserID = userid;

                        m_TopicForm.Order = TopicFilter.OrderBy.TopicID;
                        m_TopicForm.SearchMode = SearchArticleMethod.Subject;
                    }

                    m_TopicForm.Status = ThreadStatus.UnApproved;

                }
                return m_TopicForm;
            }
		}



        private ThreadCollectionV5 m_ThreadList;
        private Dictionary<int,PostV5> m_ThreadContents;
		public ThreadCollectionV5  ThreadList
		{
            get
            {
                if (m_ThreadList == null)
                {
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);

                    m_ThreadList = PostBOV5.Instance.GetThreads(My, TopicForm, pageNumber);

                    PostBOV5.Instance.ProcessKeyword(m_ThreadList, ProcessKeywordMode.FillOriginalText);

                    List<int> threadIDs = new List<int>();
                    foreach (BasicThread thread in m_ThreadList)
                    {
                        threadIDs.Add(thread.ThreadID);
                    }

                    m_ThreadContents = PostBOV5.Instance.GetThreadContents(threadIDs);

                    PostCollectionV5 tempPosts = new PostCollectionV5();
                    foreach (PostV5 post in m_ThreadContents.Values)
                        tempPosts.Add(post);

                    PostBOV5.Instance.ProcessKeyword(tempPosts, ProcessKeywordMode.FillOriginalText);

                    SetPager("list", null, pageNumber, TopicForm.PageSize, m_ThreadList.TotalRecords);

                }
                return m_ThreadList;
            }
		}

        protected PostV5 GetThreadContent(int threadID)
        {
            PostV5 post;
            if (m_ThreadContents.TryGetValue(threadID, out post))
            {
                return post;
            }
            return null;
        }

        protected string GetContent(int threadID)
        {
            PostV5 post = GetThreadContent(threadID);
            if (post == null)
                return string.Empty;

            if (StringUtil.GetByteCount(post.ContentForSearch) > 300)
                return StringUtil.CutString(post.ContentForSearch, 300);

            return post.ContentForSearch;
        }

        //private PostCollectionV5 m_PostList;
        //public PostCollectionV5 PostList
        //{
        //    get
        //    { 
        //    }
        //}

        public int TotalCount
        {
            get
            {
                return this.ThreadList.TotalRecords;
            }
        }

        protected string GetThreadUrl(BasicThread thread)
        {
            Forum forum = thread.Forum;
            return BbsUrlHelper.GetThreadUrl(forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString);
        }

        protected int PageSize
        {
            get
            {
                return TopicForm.PageSize;
            }
        }

        private void Search()
        {
            TopicFilter filter = TopicFilter.GetFromForm();
            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    List<int> topicids = StringUtil.Split2<int>(_Request.Get("topicids", Method.Post));

                    bool updatePoint = _Request.Get("updatePoint", Method.Post, "1") == "1";
                    bool success = PostBOV5.Instance.DeleteThreads(My, topicids, false, updatePoint, false, false, string.Empty);
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
            TopicFilter filter = TopicForm;//TopicFilter.GetFromFilter("filter");

            StringList param = new StringList();

            param.Add(filter.ToString());
            param.Add(_Request.Get("updatePoint", Method.Post, "1"));

            if (TaskManager.BeginTask(MyUserID, new DeleteTopicTask(), param.ToString()))
            {
                
            }
        }

        private void ApproveChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    List<int> topicids = StringUtil.Split2<int>(_Request.Get("topicids", Method.Post));

                    bool success = PostBOV5.Instance.ApproveThreads(My, topicids, false, true, true, string.Empty);
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


        private void ApproveSearchResult()
        {
            TopicFilter filter = TopicForm;//TopicFilter.GetFromFilter("filter");

            StringList param = new StringList();

            param.Add(filter.ToString());

            if (TaskManager.BeginTask(MyUserID, new ApproveTopicTask(), param.ToString()))
            {

            }
        }

    }
}