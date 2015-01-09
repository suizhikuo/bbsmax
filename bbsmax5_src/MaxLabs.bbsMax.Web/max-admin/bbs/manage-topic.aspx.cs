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
    public partial class manage_topic : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HasSomePermission == false)
            {
				ShowError("您没有权限管理帖子");
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
            else if (_Request.IsClick("restorechecked"))
            {
                RestoreChecked();
            }
            else if (_Request.IsClick("restoresearched"))
            {
                RestorSearchResult();
            }
        }

        protected bool HasPermission(Forum forum)
        {
            ManageForumPermissionSetNode managePermission = PostBOV5.Instance.GetForumPermissonSet(forum);
            
            
            bool has = managePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads);
            if (IsRecycleBin && has == false)
                has = managePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads);
            return has;
        }


        protected bool IsRecycleBin
        {
            get
            {
                return string.Compare(Type, "recycle", true) == 0;
            }
        }

        private string m_Type;
        protected string Type
        {
            get
            {
                if (m_Type == null)
                {
                    m_Type = _Request.Get("type", Method.Get, string.Empty);
                }

                return m_Type;
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
               
                if (managePermission.HasPermissionForSomeone(My, MaxLabs.bbsMax.Settings.ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
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

                    if (IsRecycleBin)
                        m_TopicForm.Status = ThreadStatus.Recycled;

                }
                return m_TopicForm;
            }
		}



        private ThreadCollectionV5 m_ThreadList;

		public ThreadCollectionV5  ThreadList
		{
			get
			{
                if (m_ThreadList == null)
                {
                    m_ThreadList = PostBOV5.Instance.GetThreads(My, TopicForm, _Request.Get<int>("page", Method.Get, 1));

                    PostBOV5.Instance.ProcessKeyword(m_ThreadList, ProcessKeywordMode.FillOriginalText);
                }
                return m_ThreadList;
			}
		}

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

        private void RestoreChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    List<int> topicids = StringUtil.Split2<int>(_Request.Get("topicids", Method.Post));

                    bool success = PostBOV5.Instance.RestoreThreads(My, topicids, false, true, true, string.Empty);
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

        private void RestorSearchResult()
        {
            TopicFilter filter = TopicForm;//TopicFilter.GetFromFilter("filter");

            StringList param = new StringList();

            param.Add(filter.ToString());

            if (TaskManager.BeginTask(MyUserID, new RestoreTopicTask(), param.ToString()))
            {

            }
        }
    }
}