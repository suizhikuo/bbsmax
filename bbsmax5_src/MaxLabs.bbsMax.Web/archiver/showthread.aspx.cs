//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Logs;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.archiver
{
    public class showthread : ForumPageBase
    {
        public showthread()
        {

        }

        protected bool IsShowReplies = true;

        private string m_ErrorMessage;
        protected string ErrorMessage
        {
            get { return m_ErrorMessage; }
            private set
            {
                m_ErrorMessage = value;
                //m_Thread = new BasicThread();
                //m_ReplyList = new PostCollectionV5();
            }
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (BaseHasError)
                return;

            AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetArchiverForumUrl(CodeName));
            if (Thread != null)
            {
                AddNavigationItem(Thread.SubjectText);

                //SetPageTitle(AllSettings.Current.BaseSeoSettings.FormatPageNumber(PageNumber));
                //SetPageTitle(Thread.SubjectText);
            }

            if (ViewReply == false)
                IsShowReplies = false;
        }

        protected override bool IncludeBase64Js
        {
            get
            {
                return AllSettings.Current.BaseSeoSettings.EncodePostUrl;
            }
        }

        protected override string PageTitle
        {
            get
            {
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    if (Forum == null)
                        return base.PageTitle;
                    if (Thread == null)
                        return string.Concat("主题不存在", " - ", base.PageTitle);
                    string pageNumberString = AllSettings.Current.BaseSeoSettings.FormatPageNumber(PageNumber);
                    if (string.IsNullOrEmpty(pageNumberString))
                        return string.Concat(Thread.SubjectText, " - ", base.PageTitle);
                    else
                        return string.Concat(Thread.SubjectText, " - ", pageNumberString, " - ", base.PageTitle);
                }
                else
                {
                    return string.Concat(ErrorMessage, " - ", base.PageTitle);
                }
            }
        }

        protected override string IndexUrl
        {
            get
            {
                return BbsUrlHelper.GetArchiverIndexUrl();
            }
        }

        protected override string GetNavigationForumUrl(Forum forum)
        {
            return BbsUrlHelper.GetArchiverForumUrl(forum.CodeName);
        }

        protected override bool CheckPermission()
        {
            if (Forum == null)
            {
                if (Thread == null)
                    return false;

                Response.Redirect(BbsUrlHelper.GetArchiverThreadUrl(Thread.Forum.CodeName, Thread.ThreadID));
                //ErrorMessage = "版块不存在";
                //return false;
            }
            if (false == Forum.CanVisit(My))
            {
                if (!Forum.CanDisplayInList(My))
                {
                    ErrorMessage = "版块不存在！";
                }
                else
                {
                    ErrorMessage = "您没有权限进入该版块！";
                }
                return false;
            }

            //版块类型正常，但需要密码
            if (Forum.ForumType == ForumType.Normal && !string.IsNullOrEmpty(Forum.Password))
            {
                //如果当前用户不拥有“进入版块不需要密码”的权限，继续检查
                if (!Forum.SigninWithoutPassword(My))
                {
                    //检查这个用户之前是否已经通过这个版块的验证，避免重复输入密码
                    if (!My.IsValidatedForum(Forum))
                    {
                        ErrorMessage = "进入该版块需要密码";
                        return false;
                    }
                }
            }

            if (Thread == null || ReplyList == null)
            {
                ErrorMessage = "主题不存在";
                return false;
            }

            if (Thread.ForumID != Forum.ForumID)
            {
                Response.Redirect(UrlHelper.GetArchiverThreadUrl(Thread.Forum.CodeName, Thread.ThreadID));
            }

            if (Can(ForumPermissionSetNode.Action.VisitThread) == false)
            {
                ErrorMessage = "您所在的用户组没有进入主题页的权限";
                return false;
            }

            if (Thread.ThreadType == ThreadType.Move)
            {
                BasicThread tempThread = PostBOV5.Instance.GetThread(Thread.RedirectThreadID);
                string tempCodeName = "";
                if (tempThread != null)
                {
                    Forum tempForum = ForumBO.Instance.GetForum(tempCodeName);
                    if (tempForum != null)
                    {
                        tempCodeName = tempForum.CodeName;
                    }
                    else
                    {
                        ErrorMessage = "主题不存在！";
                        return false;
                    }
                }
                else
                {
                    ErrorMessage = "主题不存在！";
                    return false;
                }
                string url = BbsUrlHelper.GetArchiverThreadUrl(tempCodeName, tempThread.ThreadID);
                Response.Redirect(url);

                return false;
            }
            else if (Thread.ThreadType == ThreadType.Question)
            {
                //PostV5 bestPost;
                //QuestionV5 question = PostBOV5.Instance.GetQuestion(Thread.ThreadID, out bestPost);
                //Question question = PostManager.GetQuestion(thread.ThreadID, false, out bestPost);
                QuestionThread question = null;
                if (Thread is QuestionThread)
                {
                    question = (QuestionThread)Thread;

                    if (PageNumber == 1 && question.BestPost != null)
                    {
                        //把最佳答案插入第2楼
                        m_ReplyList.Insert(1, question.BestPost);
                    }
                }
                if (question == null)
                {
                    ErrorMessage = "主题不存在！";
                }
                if (Thread.IsClosed)
                {
                    IsShowReplies = true;
                }
                else if (question.AlwaysEyeable || ForumPermission.Can(My, ForumPermissionSetNode.Action.AlwaysViewContents) || Thread.IsReplied(My))
                {
                    IsShowReplies = true;
                }
                else
                {
                    IsShowReplies = false;
                }
            }


            return true;
        }

        private BasicThread m_Thread;
        protected BasicThread Thread
        {
            get
            {
                if (m_Thread == null)
                {
                    GetThread();
                }
                return m_Thread;
            }
        }

        private PostCollectionV5 m_ReplyList;
        public PostCollectionV5 ReplyList
        {
            get
            {
                if (m_ReplyList == null)
                {
                    GetThread();
                }
                return m_ReplyList;
            }
        }

        private void GetThread()
        {
            //if (Forum == null)
                //return;

            int threadID = _Request.Get<int>("threadid",Method.Get,0);
            int pageSize = AllSettings.Current.BbsSettings.PostsPageSize;
            try
            {
                using (new ErrorScope())
                {
                    ThreadType tempType;
                    PostBOV5.Instance.GetThreadWithReplies(threadID, ThreadType.Normal, PageNumber, pageSize, false, true, true, false, out m_Thread, out m_ReplyList, out tempType);
                    if (m_Thread == null)
                    {
                        ErrorMessage = "主题不存在";
                    }
                    else
                    {
                        if (m_Thread != null)
                        {
                            //List<int> userIDs = new List<int>();
                            //foreach (PostV5 post in m_ReplyList)
                            //{
                            //    userIDs.Add(post.UserID);
                            //}
                            //UserBO.Instance.GetUsers(userIDs, GetUserOption.WithAll);
                            UserBO.Instance.GetUsers(m_ReplyList.GetUserIds(), GetUserOption.WithAll);

                            SetPager("PostListPager", BbsUrlHelper.GetArchiverThreadUrlForPager(Thread.Forum.CodeName, threadID), PageNumber, pageSize, m_Thread.TotalReplies + 1);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex);
                ErrorMessage = ex.Message;
            }
        }

        private int? m_PageNumber;
        protected int PageNumber
        {
            get
            {
                if(m_PageNumber == null)
                    m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
                return m_PageNumber.Value;
            }
        }

        protected bool IsShowNoPermissionViewReply(int index)
        {
            if (PageNumber == 1 && index == 1)
                return true;
            else if (PageNumber > 1 && index == 0)
                return true;
            return false;
        }


        protected override string MetaDescription
        {
            get
            {
                if (Forum == null)
                    return "版块不存在";
                if (Thread == null || ReplyList == null)
                    return "主题不存在";
                if (ReplyList.Count > 0)
                {
                    PostV5 post = ReplyList[0];
                    if (IsShowContent(post) && CanSeeContent(post, true))
                    {
                        return StringUtil.HtmlEncode(StringUtil.CutString(post.ContentText, 200));
                    }
                    else
                        return StringUtil.HtmlEncode(Thread.SubjectText);
                }
                else
                    return StringUtil.HtmlEncode(Thread.SubjectText);

            }
        }

        protected bool IsShowContent(PostV5 post)
        {
            if (Forum.IsShieldedUser(post.UserID) || post.IsShielded /*|| ForumPermission.Can(post.UserID, ForumPermissionSetNode.Action.DisplayContent) == false*/)
            {
                return AlwaysViewShieldContents(post.UserID);
            }
            else
                return true;
        }

        protected bool AlwaysViewShieldContents(int posterUserID)
        {
            if (ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.AlwaysViewContents, posterUserID))
                return true;
            else
                return false;
        }

        protected bool CanSeeContent(PostV5 post)
        {
            return CanSeeContent(post, post.PostIndex == 0);
        }
        protected bool CanSeeContent(PostV5 post, bool isThreadContent)
        {
            if (isThreadContent)
            {
                if (Thread.IsValued)
                {
                    if (ForumPermission.Can(My, ForumPermissionSetNode.Action.ViewValuedThread))
                        return true;
                    else
                        return false;
                }
                else if (ForumPermission.Can(My, ForumPermissionSetNode.Action.ViewThread))
                    return true;
                else
                    return false;
            }
            else
            {
                if (ForumPermission.Can(My, ForumPermissionSetNode.Action.ViewReply))
                    return true;
                else
                    return false;
            }
        }





        private bool? m_AlwaysViewContents;
        protected bool AlwaysViewContents
        {
            get
            {
                if (m_AlwaysViewContents == null)
                {
                    if (ForumPermission.Can(My, ForumPermissionSetNode.Action.AlwaysViewContents))
                        m_AlwaysViewContents = true;
                    else
                        m_AlwaysViewContents = false;
                }

                return m_AlwaysViewContents.Value;
            }
        }


        //protected bool DisplayContent(int userID)
        //{
        //    return ForumPermission.Can(userID, ForumPermissionSetNode.Action.DisplayContent);
        //}


        private bool? m_ViewThread;
        protected bool ViewThread
        {
            get
            {
                if(m_ViewThread == null)
                    m_ViewThread = ForumPermission.Can(My, ForumPermissionSetNode.Action.ViewThread);
                return m_ViewThread.Value;
            }
        }

        private bool? m_ViewReply;
        protected bool ViewReply
        {
            get
            {
                if (m_ViewReply == null)
                    m_ViewReply = ForumPermission.Can(My, ForumPermissionSetNode.Action.ViewReply);
                return m_ViewReply.Value;
            }
        }

        protected override string MetaKeywords
        {
            get
            {
                if (Thread!=null && Thread.Words != null)
                    return Thread.Words;
                else
                    return base.MetaKeywords;
            }
        }

    }
}