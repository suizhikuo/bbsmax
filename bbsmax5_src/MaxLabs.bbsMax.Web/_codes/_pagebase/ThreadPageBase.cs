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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using System.Diagnostics;
using System.Web.Configuration;
using System.Text;
using MaxLabs.bbsMax.Common;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using MaxLabs.bbsMax.PointActions;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web
{
    public abstract class ThreadPageBase : ForumPageBase
    {

        protected override string MetaKeywords
        {
            get
            {
                return Thread.Words;
            }
        }

        private void CreateLog(string tag)
        {
            if ((PageNumber == 1 || Thread.ThreadType != ThreadType.Normal) && Type == string.Empty && LookUserID == -1)
            {

                Thread.ThreadContent = PostBOV5.Instance.GetPost(Thread.ContentID, true);
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Can(ForumPermissionSetNode.Action.VisitThread) == false)
            {
                ShowError((IsLogin ? "" : "(您是游客)") + "您所在的用户组在当前版块没有进入主题页面的权限");
            }

            ProcessSubmits();


            //放在所有判断点击按钮之后
            if (!HadProcessSubmit)
            {
                string otherAction = _Request.Get("otheraction", Method.Get, string.Empty).ToLower();
                ProcessOtherAction(otherAction);
            }

            //搜索页链接过来的 如果是搜索回复内容或者用户回复时 需要定向到该回复
            if (ThreadSearchMode != null)
            {
                if (ThreadSearchMode.Value == SearchMode.UserPost || ThreadSearchMode.Value == SearchMode.Content)
                {
                    int postID = _Request.Get<int>("postID", Method.Get, 0);
                    if (postID == 0)
                    {
                        ShowError(new InvalidParamError("postid"));
                    }
                    else
                    {
                        ProcessSearchMode(postID);
                    }
                }
            }



            if (IsUnapprovePosts)
            {
                if (CanManage(ManageForumPermissionSetNode.Action.ApprovePosts) == false)
                {
                    ShowError(new ThreadNotExistsError());
                }
            }


            #region  处理蜘蛛

            //处理蜘蛛 --------------

            if (_Request.IsSpider)
            {
                if (IsOnlyLookOneUser)
                {
                    m_LookUserID = -1;
                }
                else if (string.IsNullOrEmpty(Type) == false)
                {
                    m_Type = string.Empty;
                }
            }
            //-----------------------

            #endregion





            if (Thread == null)// || Thread.ThreadContent == null)
            {
                ShowError(new ThreadNotExistsError());
            }

            if (Thread.ThreadContent == null)
            {
                CreateLog("1");
                //ShowError(new ThreadNotExistsError());
            }

            if (Thread.ThreadStatus == ThreadStatus.UnApproved)
            {
                if (string.Compare(Type, MyThreadType.MyUnapprovedThread.ToString(), true) == 0)//我的未审核主题
                {
                    if (MyUserID != Thread.PostUserID)
                        ShowError(new ThreadNotExistsError());
                }
                else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
                {
                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, Thread.PostUserID) == false)
                    {
                        ShowError(new ThreadNotExistsError());
                    }
                }
                else
                {
                    ShowError(new ThreadNotExistsError());
                }
            }
            else if (Thread.ThreadStatus == ThreadStatus.Recycled)
            {
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, Thread.PostUserID) == false
                    && CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, Thread.PostUserID) == false)
                {
                    ShowError(new ThreadNotExistsError());
                }
            }


            if (Thread.ThreadType == ThreadType.Move || Thread.ThreadType == ThreadType.Join)
            {
                BasicThread tempThread = PostBOV5.Instance.GetThread(Thread.RedirectThreadID);
                string tempCodeName = "";
                if (tempThread != null)
                {
                    Forum tempForum = ForumBO.Instance.GetForum(tempThread.ForumID, true);
                    if (tempForum != null)
                    {
                        tempCodeName = tempForum.CodeName;
                    }
                    else
                    {
                        ShowError("主题不存在！");
                    }
                }
                else
                {
                    ShowError("主题不存在！");
                }
                string url = BbsUrlHelper.GetThreadUrl(tempCodeName, tempThread.ThreadID, tempThread.ThreadTypeString, PageNumber, Type);

                UrlUtil.Redirect(url);
                //Response.Redirect(url);
            }


            if (ForumID < 0)
            {
                m_forum = ForumBO.Instance.GetForum(Thread.ForumID, false);
                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
                    AddNavigationItem("[回收站]", BbsUrlHelper.GetForumUrl(m_forum.CodeName, "recycled", 1));
                else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
                    AddNavigationItem("[未审核的主题]", BbsUrlHelper.GetForumUrl(m_forum.CodeName, "unapproved", 1));
                else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
                    AddNavigationItem("[未审核的回复]", BbsUrlHelper.GetForumUrl(m_forum.CodeName, "unapprovedpost", 1));

            }
            else
            {
                //如果主题所在版块不正确(如移动主题后用户从收藏夹进入),则处理
                if (ForumID != Thread.ForumID)
                {
                    //如果是正常查看帖子，则可以直接跳转
                    if (string.IsNullOrEmpty(Type))
                    {
                        string codeName = ForumBO.Instance.GetCodeName(Thread.ForumID);
                        Response.Redirect(BbsUrlHelper.GetThreadUrl(codeName, ThreadID, Thread.ThreadTypeString, PageNumber));
                    }
                    else
                        ShowError("主题不存在或者已被删除");
                }



                if (Type == "")
                {
                    if (ForumListThreadCatalogID == -1)
                    {
                        AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(Forum.CodeName, ForumListAction, ForumListPage));
                    }
                    else
                        AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, ForumListThreadCatalogID, ForumListPage));
                }
                else
                    AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(Forum.CodeName));


                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
                    AddNavigationItem("[回收站]", BbsUrlHelper.GetForumUrl(m_forum.CodeName, "recycled", 1));
                else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
                    AddNavigationItem("[未审核的主题]", BbsUrlHelper.GetForumUrl(m_forum.CodeName, "unapproved", 1));
                else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
                    AddNavigationItem("[未审核的回复]", BbsUrlHelper.GetForumUrl(m_forum.CodeName, "unapprovedpost", 1));

            }

            if (IsGetPost)
            {
                AddNavigationItem(Thread.SubjectText, BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, Thread.ThreadTypeString));
                AddNavigationItem("查看单个帖子");
            }
            else
                AddNavigationItem(Thread.SubjectText);

            //SetPageTitle(AllSettings.Current.BaseSeoSettings.FormatPageNumber(PageNumber));
            //SetPageTitle(Thread.SubjectText);

            UpdateOnlineStatus(OnlineAction.ViewThread, ThreadID, Thread.SubjectText);
            //OnlineManager.UpdateOnlineUser(MyUserID, Forum.ForumID, ThreadID, My.OnlineStatus, OnlineAction.ViewThread, Request, Response);



            if (Thread.ThreadContent == null)
            {
                CreateLog("2");

            }

            PostBOV5.Instance.ProcessKeyword(Thread, ProcessKeywordMode.TryUpdateKeyword);


            if (Thread.ThreadContent == null)
            {
                CreateLog("3");

            }
            #region SetPager

            int totalPosts = TotalPosts;

            string emoctionPagerUrl;
            if (LookUserID == -1)
            {
                if (string.IsNullOrEmpty(Type))
                {
                    if (ExtParms != null)
                    {
                        if (Thread.ThreadType == ThreadType.Question && BestPost != null)
                            SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, -1, Thread.ThreadTypeString, ExtParms), PageNumber, PageSize, totalPosts, 1);
                        else
                            SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, -1, Thread.ThreadTypeString, ExtParms), PageNumber, PageSize, totalPosts);

                        emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, -1, PageNumber, Thread.ThreadTypeString, ExtParms, IsGetAllDefaultEmoticon, EmoticonGroupID);
                    }
                    else
                    {
                        if (Thread.ThreadType == ThreadType.Question && BestPost != null)
                            SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, ForumListPage, Thread.ThreadTypeString), PageNumber, PageSize, totalPosts, 1);
                        else
                            SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, ForumListPage, Thread.ThreadTypeString), PageNumber, PageSize, totalPosts);

                        emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, PageNumber, ForumListPage, Thread.ThreadTypeString, IsGetAllDefaultEmoticon, EmoticonGroupID);
                    }

                }
                else
                {
                    if (Thread.ThreadType == ThreadType.Question && BestPost != null)
                        SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, Thread.ThreadTypeString, Type), PageNumber, PageSize, totalPosts, 1);
                    else
                        SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, Thread.ThreadTypeString, Type), PageNumber, PageSize, totalPosts);

                    emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, Type, PageNumber, Thread.ThreadTypeString, IsGetAllDefaultEmoticon, EmoticonGroupID);
                }
            }
            else
            {
                SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, LookUserID, 1, Thread.ThreadTypeString), PageNumber, PageSize, totalPosts);

                emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, LookUserID, PageNumber, 1, Thread.ThreadTypeString, IsGetAllDefaultEmoticon, EmoticonGroupID);
            }

            #endregion


            if (Thread.ThreadContent == null)
            {
                CreateLog("3");
            }
        }

        protected override bool IncludeBase64Js
        {
            get
            {
                return true;
            }
        }

        protected override string PageTitle
        {
            get
            {
                string pageNumberString = AllSettings.Current.BaseSeoSettings.FormatPageNumber(PageNumber);
                if (string.IsNullOrEmpty(pageNumberString))
                    return string.Concat(Thread.SubjectText, " - ", base.PageTitle);
                else
                    return string.Concat(Thread.SubjectText, " - ", pageNumberString, " - ", base.PageTitle);
            }
        }

        /// <summary>
        /// 给分页用的
        /// </summary>
        protected virtual int TotalPosts
        {
            get
            {
                return Thread.TotalRepliesForPage + 1;
            }
        }

        protected override string SignInForumUrl
        {
            get
            {
                return BbsUrlHelper.GetSignInForumUrl(Forum.CodeName, ThreadID);
            }
        }


        protected override void OnLoadComplete(EventArgs e)
        {

            base.OnLoadComplete(e);
        }

        /// <summary>
        /// 是否处理过按钮点击事件
        /// </summary>
        protected bool HadProcessSubmit;
        protected bool UpdateView = true;
        /// <summary>
        /// 所有按钮提交的操作 写在这里
        /// </summary>
        protected virtual void ProcessSubmits()
        {
            ProcessAddRank();
        }

        #region Submits
        private void ProcessAddRank()
        {
            int rank;
            if (_Request.IsClick("rankButton1"))
            {
                rank = 1;
            }
            else if (_Request.IsClick("rankButton2"))
            {
                rank = 2;
            }
            else if (_Request.IsClick("rankButton3"))
            {
                rank = 3;
            }
            else if (_Request.IsClick("rankButton4"))
            {
                rank = 4;
            }
            else if (_Request.IsClick("rankButton5"))
            {
                rank = 5;
            }
            else
            {
                return;
            }
            UpdateView = false;
            HadProcessSubmit = true;

            try
            {
                bool success = PostBOV5.Instance.SetThreadRank(My, ForumID, ThreadID, rank, false);
                if (success == false)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        AlertWarning(error.Message);
                    });
                }
                else
                {
                    AlertSuccess("您已成功对主题进行评级");
                }
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
                AlertWarning(ex.Message);
            }
        }
        #endregion


        /// <summary>
        /// 所有通过链接进行的操作 写在这里
        /// </summary>
        protected virtual void ProcessOtherAction(string otherAction)
        {
            switch (otherAction)
            {
                case "setlovehate":
                    SetPostLoveHate();
                    break;
                //case "buyattachment":
                //    ProcessBuyAttachment();
                //    break;
                case "deleteselfpost":
                    ProcessDeleteSelfPost();
                    break;
                case "deleteselfthread":
                    ProcessDeleteSelfThread();
                    break;
                default:
                    break;
            }
            //ProcessBuyAttachment();
        }

        private void SetPostLoveHate()
        {
            UpdateView = false;
            bool islove = _Request.Get<bool>("islove",Method.Get,true);
            int postID = _Request.Get<int>("postid",Method.Get,0);

            if(postID<1)
            {
                AlertWarning(new InvalidParamError("postid").Message);
                return;
            }
            try
            {
                bool success = PostBOV5.Instance.SetPostLoveHate(MyUserID, postID, islove, false);
                if (success == false)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        AlertWarning(error.Message);
                    });
                }
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex);
                AlertWarning(ex.Message);
            }
        }





        private Forum m_forum;
        protected override Forum Forum
        {
            get
            {
                if (m_forum == null)
                {
                    m_forum = base.Forum;
                    if (m_forum == null)
                    {
                        m_forum = ForumBO.Instance.GetSystemForum(_Request.Get("codename", Method.Get, string.Empty));
                        if (m_forum != null)
                        {
                            if (m_forum.ForumID == (int)SystemForum.RecycleBin)
                            {
                                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) != 0)
                                    ShowError("非法操作！");
                            }
                            else if (m_forum.ForumID == (int)SystemForum.UnapproveThreads || m_forum.ForumID == (int)SystemForum.UnapprovePosts)
                            {
                                if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) != 0 &&
                                    string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) != 0)
                                    ShowError("非法操作！");
                            }
                            else
                            {
                                if (Thread == null)
                                    ShowError("主题不存在或已被删除");
                                Response.Redirect(BbsUrlHelper.GetThreadUrl(Thread.Forum.CodeName, ThreadID, PageNumber));
                                //m_forum = Thread.Forum;
                                //ShowError("非法操作！");
                            }
                        }
                    }
                }

                return m_forum;
            }
        }



        protected virtual void ProcessSearchMode(int postID)
        {
            PostCollectionV5 posts;
            BasicThread thread;

            ThreadType tempType;
            //要定向到 搜索到的回复  所以要取出所有的回复  进行计算该回复所在的页码
            PostBOV5.Instance.GetThreadWithReplies(ThreadID, 1, int.MaxValue, false, false, false, out thread, out posts, out tempType);

            int postIndex = 0;
            int totalCount = 0;
            PostV5 tempPost = null;
            foreach (PostV5 post in posts)
            {
                if (post.IsApproved)
                {
                    if (post.PostID == postID)
                    {
                        tempPost = post;
                        postIndex = totalCount;
                        break;
                    }
                    totalCount++;
                }
            }

            int pageIndex = postIndex / PageSize;

            if (ThreadSearchMode.Value == SearchMode.UserPost)
                Response.Redirect(BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, thread.ThreadID, thread.ThreadTypeString, postID, pageIndex + 1, false));
            else
                Response.Redirect(BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, thread.ThreadID, thread.ThreadTypeString, postID, pageIndex + 1, SearchText, false));
        }


        private int? m_ThreadID;
        protected int ThreadID
        {
            get
            {
                if (m_ThreadID == null)
                {
                    m_ThreadID = _Request.Get<int>("threadid", Method.Get, 0);
                }

                return m_ThreadID.Value;
            }
        }

        protected abstract BasicThread Thread
        {
            get;
        }

        private PostV5 m_ThreadContent;
        protected PostV5 ThreadContent
        {
            get
            {
                if (m_ThreadContent == null)
                {
                    if (Thread.ThreadContent == null)
                    {
                        CreateLog("100");
                    }
                    m_ThreadContent = Thread.ThreadContent;
                }

                return m_ThreadContent;
            }
        }


        private int? m_PageNumber;
        protected int PageNumber
        {
            get
            {
                if (m_PageNumber == null)
                {
                    m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
                }
                return m_PageNumber.Value;
            }
            set
            {
                m_PageNumber = value;
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

        private int? m_LookUserID;
        /// <summary>
        /// 只看某用户  -1 为查看全部
        /// </summary>
        protected int LookUserID
        {
            get
            {
                if (m_LookUserID == null)
                {
                    m_LookUserID = _Request.Get<int>("userid", Method.Get, -1);
                }
                return m_LookUserID.Value;
            }
        }

        /// <summary>
        /// 是否是只看某个用户
        /// </summary>
        protected bool IsOnlyLookOneUser
        {
            get
            {
                return LookUserID != -1;
            }
        }

        private bool? m_IsUnapprovePosts;
        /// <summary>
        /// 审核站里点过来的（未审核的回复）
        /// </summary>
        protected bool IsUnapprovePosts
        {
            get
            {
                if (m_IsUnapprovePosts == null)
                {
                    m_IsUnapprovePosts = (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0);
                }

                return m_IsUnapprovePosts.Value;
            }
        }

        private bool? m_IsMyUnapprovePosts;
        /// <summary>
        /// 审核站里点过来的（未审核的回复）
        /// </summary>
        protected bool IsMyUnapprovePosts
        {
            get
            {
                if (m_IsMyUnapprovePosts == null)
                {
                    m_IsMyUnapprovePosts = (string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0);
                }

                return m_IsMyUnapprovePosts.Value;
            }
        }



        private bool? m_IsGetPost;
        protected bool IsGetPost
        {
            get 
            {
                if (m_IsGetPost == null)
                    m_IsGetPost = (string.Compare(Type, "getpost", true) == 0);

                return m_IsGetPost.Value;
            }
        }

        private bool? m_IsShowThreadContent;
        protected bool IsShowThreadContent
        {
            get
            {
                if (m_IsShowThreadContent == null)
                {
                    if (PageNumber != 1 || IsOnlyLookOneUser || IsUnapprovePosts || IsGetPost || IsMyUnapprovePosts)
                    {
                        m_IsShowThreadContent = false;
                    }
                    else
                        m_IsShowThreadContent = true;
                }
                return m_IsShowThreadContent.Value;
            }
        }


        protected SearchMode? ThreadSearchMode
        {
            get
            {
                string mode = _Request.Get("SearchMode", Method.Get, string.Empty);
                if (StringUtil.EqualsIgnoreCase(mode, SearchMode.UserPost.ToString()))
                    return SearchMode.UserPost;

                else if (StringUtil.EqualsIgnoreCase(mode, SearchMode.Content.ToString()))
                    return SearchMode.Content;

                else
                    return null;
            }
        }

        private string m_SearchText;
        protected string SearchText
        {
            get
            {
                if (m_SearchText == null)
                {
                    m_SearchText = Server.UrlDecode(_Request.Get("SearchText", Method.Get, string.Empty));
                }
                return m_SearchText;
            }
        }

        private Regex regex;
        protected string Highlight(string content)
        {
            string HighlightColor = AllSettings.Current.SearchSettings.HighlightColor;
            if (regex == null)
            {
                if (string.IsNullOrEmpty(SearchText))
                    return content;

                List<string> words = PostBOV5.Instance.GetSearchKeywords(SearchText);

                if (words.Count == 0)
                    return content;

                string partten = null;
                foreach (string word in words)
                {
                    partten += word + "|";
                }

                partten = partten.Substring(0, partten.Length - 1);


                regex = new Regex(@"(?<=(?:^|>)[^<>]*?)((" + partten + "))(?=[^<>]*?(?:<|$))", RegexOptions.IgnoreCase);
            }

            return regex.Replace(content, string.Format(@"<font style=""color:{0};font-weight:Bold"">$1</font>", HighlightColor));

        }


        protected bool IsShowUserExtendProfile(User user, params string[] keys)
        {
            if (user == null || user.UserID <= 0)
                return false;
            
            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(GetUserExtendProfile(user,key)) == false)
                {
                    return true;
                }
            }
            return false;
        }

        protected string GetUserExtendProfile(User user, string key)
        {
            UserExtendedValue extendedValue = user.ExtendedFields.GetValue(key);
            if (extendedValue == null)
                return string.Empty;

            if (user.UserID == MyUserID)
                return extendedValue.Value;

            if (extendedValue.PrivacyType == ExtendedFieldDisplayType.AllVisible)
                return extendedValue.Value;
            else if (extendedValue.PrivacyType == ExtendedFieldDisplayType.FriendVisible)
            {
                if (FriendBO.Instance.IsFriend(MyUserID, user.UserID))
                    return extendedValue.Value;
                else
                    return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }


        private int? m_ForumListPage;
        /// <summary>
        /// 列表页 页码
        /// </summary>
        protected int ForumListPage
        {
            get
            {
                if (m_ForumListPage == null)
                {
                    GetUrlParms();
                }
                return m_ForumListPage.Value;
            }
        }

        private string m_ForumListAction;
        protected string ForumListAction
        {
            get
            {
                if (m_ForumListAction == null)
                {
                    GetUrlParms();
                }
                return m_ForumListAction;
            }
        }

        private int? m_ForumListThreadCatalogID;
        /// <summary>
        /// 
        /// </summary>
        protected int ForumListThreadCatalogID
        {
            get
            {
                if (m_ForumListThreadCatalogID == null)
                {
                    GetUrlParms();
                }
                return m_ForumListThreadCatalogID.Value;
            }
        }

        private string m_ExtParms;
        protected string ExtParms
        {
            get
            {
                if (m_ExtParms == null)
                {
                    m_ExtParms = _Request.Get("extParms", Method.Get);
                }
                return m_ExtParms;
            }
        }

        private void GetUrlParms()
        {
            if (m_ForumListPage != null)
                return;

            string extParms = ExtParms;
            if (extParms != null)//从分类或者投票等列表页进来的
            {
                int page;
                if (int.TryParse(GetExtParm("page", extParms), out page))
                {
                    m_ForumListPage = page;
                }
                else
                    m_ForumListPage = 1;

                m_ForumListAction = GetExtParm("action", extParms, "list");

                int threadCatalogID;
                if (int.TryParse(GetExtParm("ThreadCatalogID", extParms, "-1"),out threadCatalogID))
                {
                    m_ForumListThreadCatalogID = threadCatalogID;
                }
                else
                    m_ForumListThreadCatalogID = -1;
            }
            else//从一般列表页进来的
            {
                m_ForumListAction = "list";
                m_ForumListThreadCatalogID = -1;
                m_ForumListPage = _Request.Get<int>("ListPage", Method.Get, 1);
            }
        }

        /// <summary>
        /// 获取扩展参数中的某个值(page=1&threadid=2&type=list)
        /// SEK
        /// </summary>
        /// <param name="key"></param>
        /// <param name="extParms"></param>
        /// <returns></returns>
        protected string GetExtParm(string key, string extParms)
        {
            return GetExtParm(key, extParms, "");
        }
        /// <summary>
        /// 获取扩展参数中的某个值(page=1&threadid=2&type=list)
        /// SEK
        /// </summary>
        /// <param name="key"></param>
        /// <param name="extParms"></param>
        /// <returns></returns>
        protected string GetExtParm(string key, string extParms, string defaultValue)
        {
            if (extParms == null)
                return defaultValue;

            extParms = System.Web.HttpContext.Current.Server.UrlDecode(extParms);
            string[] s = extParms.Split('&');

            foreach (string str in s)
            {
                string[] str2s = str.Split('=');
                if (str2s.Length > 1)
                {
                    if (StringUtil.EqualsIgnoreCase(str2s[0].Trim(), key.Trim()))
                    {
                        return str2s[1];
                    }
                }
            }
            return defaultValue;
        }

        private string m_ForumUrl;
        protected string ForumUrl
        {
            get
            {
                if (m_ForumUrl == null)
                {
                    if (Type == "")
                    {
                        if (ForumListThreadCatalogID == -1)
                        {
                            m_ForumUrl = BbsUrlHelper.GetForumUrl(Forum.CodeName, ForumListAction, ForumListPage);
                        }
                        else
                            m_ForumUrl = BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, ForumListThreadCatalogID, ForumListPage);
                    }
                    else
                        m_ForumUrl = BbsUrlHelper.GetForumUrl(Forum.CodeName);
                }
                return m_ForumUrl;
            }
        }

        

        private bool? m_IsShowReply = null;
        protected bool IsShowReply
        {
            get
            {
                if (m_IsShowReply == null)
                {
                    //以下情况不允许回复
                    if (Thread.IsLocked ||
                        !string.IsNullOrEmpty(Type) ||
                        !AllowQuicklyReply ||
                        !Can(ForumPermissionSetNode.Action.ReplyThread) ||
                        Forum.IsShieldedUser(MyUserID))
                    {
                        m_IsShowReply = false;
                    }
                    else
                    {
                        if (Thread.ThreadType == ThreadType.Polemize && !Can(ForumPermissionSetNode.Action.CanPolemize))
                            m_IsShowReply = false;
                        else
                            m_IsShowReply = true;
                    }
                }
                return m_IsShowReply.Value;
            }
        }

        protected bool IsOverUpdateSortOrderTime
        {
            get
            {
                return Thread.IsOverUpdateSortOrderTime(ForumSetting);
            }
        }

        private bool? m_IsShowPostIndexAlias;
        protected virtual bool IsShowPostIndexAlias
        {
            get
            {
                if (m_IsShowPostIndexAlias == null)
                {
                    if (LookUserID > 0 || string.IsNullOrEmpty(Type) == false)
                        m_IsShowPostIndexAlias = false;
                    else
                        m_IsShowPostIndexAlias = true;
                }
                return m_IsShowPostIndexAlias.Value;
            }
        }

        /// <summary>
        /// 问题帖的最佳回复
        /// </summary>
        protected virtual PostV5 BestPost
        {
            get
            {
                return null;
            }
        }

        protected string GetCheckBox(PostV5 reply, string checkBoxStyle)
        {
            if (string.IsNullOrEmpty(checkBoxStyle))
                checkBoxStyle = "<input Type=\"checkbox\" name=\"postID\" id=\"postID_{0}\" value=\"{0}\" />";

            if (CanDeletePostsForSomeone)
            {
                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) != 0)
                {
                    if (reply.PostIndex > 0 || string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
                        return string.Format(checkBoxStyle, reply.PostID.ToString());
                    else
                    {
                        if (Thread.ThreadType == ThreadType.Question)
                        {
                            if (BestPost != null)
                            {
                                if (reply.PostID == BestPost.PostID)
                                {
                                    return string.Format(checkBoxStyle, reply.PostID.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }




        protected string GetPostManageLinks(PostV5 reply, string style, string editStyle, string deleteStyle, string shieldStyle, string unshieldStyle, string checkStyle)
        {
            //return openDialog(this.href, refresh)
            string editLink = GetEditPostLink(reply, editStyle);
            string deleteLink = GetDeletePostLink(reply, deleteStyle);
            string shieldLink = GetShieldPostLink(reply, shieldStyle);
            string unShieldLink = GetUnShieldPostLink(reply, unshieldStyle);
            string checkLink = GetCheckBox(reply, checkStyle);

            if (editLink == string.Empty
                && deleteLink == string.Empty
                && shieldLink == string.Empty
                && unShieldLink == string.Empty)
                return string.Empty;

            return string.Format(style, editLink, deleteLink, shieldLink, unShieldLink, checkLink);

        }







        /// <summary>
        /// 获取回复连接
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        protected string GetReplyLink(PostV5 reply, string style)
        {

            if (string.IsNullOrEmpty(Type))
            {
                if (!Thread.IsLocked)
                {
                    //if (Can(ForumPermissionSetNode.Action.ReplyThread))
                        return string.Format(style, BbsUrlHelper.GetReplyPostUrl(Forum.CodeName, Thread.ThreadID, Server.UrlEncode("re:" + reply.PostIndexAlias + " " + reply.Username), reply.PostID));
                }
            }
            return string.Empty;
        }
        protected string GetQuoteLink(PostV5 reply, string style)
        {
            if (string.IsNullOrEmpty(Type))
            {
                if (!Thread.IsLocked)
                {
                    //if (Can(ForumPermissionSetNode.Action.ReplyThread))
                    //{
                        string url;
                        if (Thread == null || Thread.ThreadID < 1)
                            url = BbsUrlHelper.GetCreatPostUrl(string.Empty, "ReQuote", 0, reply.PostID, Type);
                        else
                            url = BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "ReQuote", Thread.ThreadID, reply.PostID, Type);

                        return string.Format(style, url);
                    //}
                }
            }
            return string.Empty;
        }
        protected string GetMarkLink(PostV5 reply, string style)
        {
            if (string.IsNullOrEmpty(Type) && reply.UserID != MyUserID)
            {
                //    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.Marking, reply.UserID) && reply.UserID != MyUserID)
                //    {
                return string.Format(style, Dialog + "/post-rate.aspx?postid=" + reply.PostID + "&PostAlias="+ Server.UrlEncode(reply.PostIndexAlias));
                //    }
            }
            return string.Empty;
        }

        protected string GetCancelMarkLink(PostV5 reply, string style)
        {
            if (string.IsNullOrEmpty(Type))
            {
                if (reply.PostMarks.Count > 0)
                {
                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.CancelRate, reply.UserID))
                    {
                        return string.Format(style, Dialog + "/post-cancelrate.aspx?postid=" + reply.PostID);
                    }
                }
            }
            return string.Empty;
        }



        protected override string GetModeratorActionLinks(string outputFormat, string separator)
        {
            StringBuilder sb = new StringBuilder();

            if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
            {
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, Thread.PostUserID))
                    sb.AppendFormat(outputFormat, "还原主题", Dialog + "/forum/revertthread.aspx?codename=" + Forum.CodeName);
                    //appendModeratorActionLink(sb, outputFormat, "还原主题", ThreadManageAction.RevertThread, Forum.CodeName, separator);

                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, Thread.PostUserID))
                {
                    sb.Append(separator);
                    sb.AppendFormat(outputFormat, "彻底删除主题", Dialog + "/forum/deletethread.aspx?codename=" + Forum.CodeName);
                    //appendModeratorActionLink(sb, outputFormat, "删除主题", ThreadManageAction.DeleteThread, Forum.CodeName, separator);
                }

                return sb.ToString();
            }
            else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
            {
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, Thread.PostUserID))
                {
                    //appendModeratorActionLink(sb, outputFormat, "主题通过审核", ThreadManageAction.CheckThread, Forum.CodeName, separator);
                    //appendModeratorActionLink(sb, outputFormat, "彻底删除主题", ThreadManageAction.DeleteThread, Forum.CodeName, separator);
                    sb.AppendFormat(outputFormat, "主题通过审核", Dialog + "/forum/approvethread.aspx?codename=" + Forum.CodeName);
                    sb.Append(separator);
                    sb.AppendFormat(outputFormat, "彻底删除主题", Dialog + "/forum/deletethread.aspx?codename=" + Forum.CodeName);

                }

                return sb.ToString();
            }
            else if (IsUnapprovePosts)
            {
                return string.Empty;
            }
            else
            {
                if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.JoinThreads))
                {
                    string joinThreadLink = string.Format(outputFormat, "合并主题", Dialog + "/forum/jointhread.aspx?codename=" + Forum.CodeName);
                    string splitThreadLink = string.Format(outputFormat, "分割主题", Dialog + "/forum/splitthread.aspx?codename=" + Forum.CodeName);
                    return base.GetModeratorActionLinks(outputFormat, separator) + separator + joinThreadLink + separator + splitThreadLink;
                }
                else
                    return base.GetModeratorActionLinks(outputFormat, separator);
            }
        }


        protected string GetPostActionLink(string outputFormat, string separator)
        {
            if (IsUnapprovePosts)
            {
                StringBuilder sb = new StringBuilder();
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, Thread.PostUserID))
                {
                    sb.AppendFormat(outputFormat, "选中的回复通过审核", Dialog + "/forum/approvepost.aspx.aspx?codename=" + Forum.CodeName);
                    sb.Append(separator);
                    sb.AppendFormat(outputFormat, "彻底删除选中的回复", Dialog + "/forum/deletepost.aspx?codename=" + Forum.CodeName);
                    sb.Append(separator);
                    sb.AppendFormat(outputFormat, "所有回复通过审核", Dialog + "/forum/approvepostsbythreadid.aspx?codename=" + Forum.CodeName);
                    sb.Append(separator);
                    sb.AppendFormat(outputFormat, "彻底删除所有回复", Dialog + "/forum/deleteunapprovedpostbythreadid.aspx?codename=" + Forum.CodeName);
                }
                return sb.ToString();
            }
            else
            {
                if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts))
                {
                    return string.Format(outputFormat, "删除选中回复", Dialog + "/forum/deletepost.aspx?codename=" + Forum.CodeName);
                }
                return string.Empty;
            }
        }

        private void appendModeratorActionLink(StringBuilder sb, string outputFormat, string actionName, ThreadManageAction action, string CodeName, string separator)
        {
            if (sb.Length > 0)
                sb.Append(separator);
            sb.AppendFormat(outputFormat, actionName, BbsUrlHelper.GetThreadManagerUrl(CodeName.Replace("'", "\\'"), action.ToString()));
        }


        protected string GetUsePropLink(PostV5 reply, string linkStyle)
        {
            if (EnablePropFunction == false)
                return string.Empty;
            //使用道具
            return string.Format(linkStyle, String.Concat(Dialog, "/prop-use.aspx?uid=", reply.UserID, "&tid=", Thread.ThreadID, "&cat=", (reply.PostType == PostType.ThreadContent ? PropTypeCategory.Thread : PropTypeCategory.ThreadReply)), "return openDialog(this.href, refresh)");

        }
        protected string GetReportLink(PostV5 reply, string linkStyle)
        {
            if (reply.PostType != PostType.ThreadContent)
                return string.Format(linkStyle, Dialog + "/report-add.aspx?type=reply&id=" + reply.PostID + "&uid=" + reply.UserID);
            else
                return string.Format(linkStyle, Dialog + "/report-add.aspx?type=topic&id=" + reply.ThreadID + "&uid=" + reply.UserID);
        }
        
 
        protected string GetShieldUserLink(PostV5 reply, string shieldLinkStyle, string unShieldLinkStyle)
        { 
            //如果当前用户拥有屏蔽用户的权限，显示屏蔽用户的链接
            if (reply.UserID != 0)
            {
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.BanUser, reply.UserID))
                {
                    if (Forum.IsShieldedUser(reply.UserID))
                    {
                        return string.Format(shieldLinkStyle, BbsUrlHelper.GetActionShieldUserUrl(reply.UserID), "return openDialog(this.href, refresh)");
                    }
                    else
                    {
                        return string.Format(unShieldLinkStyle, BbsUrlHelper.GetActionShieldUserUrl(reply.UserID), "return openDialog(this.href, refresh)");
                    }
                }
            }

            return string.Empty;
        }

        protected string GetUnShieldPostLink(PostV5 reply, string linkStyle)
        {
            if (reply.IsShielded)
            {
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetPostShield, reply.UserID))
                {
                    return string.Format(linkStyle, Dialog + "/forum/shieldpost.aspx?codename=" + Forum.CodeName + "&shield=false&postids=" + reply.PostID);
                }
            }
            return string.Empty;
        }
        protected string GetShieldPostLink(PostV5 reply, string linkStyle)
        {
            if (reply.IsShielded == false)
            {
                //如果当前用户拥有屏蔽回复的权限，显示屏蔽回复的链接
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetPostShield, reply.UserID))
                {
                    return string.Format(linkStyle, Dialog + "/forum/shieldpost.aspx?codename=" + Forum.CodeName + "&shield=true&postids=" + reply.PostID);
                }
            }
            return string.Empty;
        }

        protected string GetEditPostLink(PostV5 reply, string linkStyle)
        {
            if ((string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0) || (reply.PostType != PostType.ThreadContent && ((reply.UserID == MyUserID && Can(ForumPermissionSetNode.Action.UpdateOwnPost)) || (CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, reply.UserID)))))
            {
                return string.Format(linkStyle, BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editpost", reply.ThreadID, reply.PostID, Type));
            }
            else if (reply.PostType == PostType.ThreadContent && ((reply.UserID == MyUserID && Can(ForumPermissionSetNode.Action.UpdateOwnPost)) || (CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, reply.UserID))))
            {
                return string.Format(linkStyle, BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, string.Concat("edit", Thread.ThreadTypeString), reply.ThreadID, reply.PostID, Type));
            }

            return string.Empty;
        }

        protected string GetDeletePostLink(PostV5 reply, string linkStyle)
        {
            string deleteLinkReturn;
            if (reply.PostType == PostType.ThreadContent)
                deleteLinkReturn = "return openDialog(this.href,function(){location.replace('"+ UrlHelper.GetForumUrl(reply.Forum.CodeName) +"');})";
            else
                deleteLinkReturn = "return openDialog(this.href, refresh)";


            if (string.IsNullOrEmpty(Type) || IsGetPost)
            {
                if (reply.PostType != PostType.ThreadContent)
                {
                    if (reply.UserID == MyUserID)
                    {
                        if (Can(ForumPermissionSetNode.Action.DeleteOwnPosts))
                            return string.Format(linkStyle, Dialog + "/forum/delete-selfpost.aspx?postid=" + reply.PostID, deleteLinkReturn);
                    }
                    else
                    {
                        if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, reply.UserID))
                        {
                            return string.Format(linkStyle, Dialog + "/forum/deletepost.aspx?codename=" + Forum.CodeName + "&threadids=" + ThreadID + "&postids=" + reply.PostID, deleteLinkReturn);
                        }
                    }
                }
                else
                {
                    if (reply.UserID == MyUserID)
                    {
                        if (Can(ForumPermissionSetNode.Action.DeleteOwnThreads))
                            return string.Format(linkStyle, Dialog + "/forum/delete-selfthread.aspx?threadid=" + reply.ThreadID, deleteLinkReturn);
                    }
                    else
                    {
                        if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, reply.UserID))
                        {
                            return string.Format(linkStyle, Dialog + "/forum/deletethread.aspx?codename=" + Forum.CodeName + "&threadids=" + ThreadID, deleteLinkReturn);
                        }
                    }
                }
            }

            return string.Empty;
        }

        protected string GetDeletePostLinkForMobile(PostV5 reply, string linkStyle)
        {
            if (string.IsNullOrEmpty(Type) || IsGetPost)
            {
                if (reply.PostType != PostType.ThreadContent)
                {
                    if (reply.UserID == MyUserID)
                    {
                        if (Can(ForumPermissionSetNode.Action.DeleteOwnPosts))
                            return string.Format(linkStyle, BbsUrlHelper.GetThreadUrl(Forum.CodeName, Thread.ThreadID, Thread.ThreadTypeString, 1) + "?otheraction=deleteselfpost&postid=" + reply.PostID);
                    }
                }
                else
                {
                    if (reply.UserID == MyUserID)
                    {
                        if (Can(ForumPermissionSetNode.Action.DeleteOwnThreads))
                            return string.Format(linkStyle, BbsUrlHelper.GetThreadUrl(Forum.CodeName, Thread.ThreadID, Thread.ThreadTypeString, 1) + "?otheraction=deleteselfthread");
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 给手机版用的
        /// </summary>
        private void ProcessDeleteSelfPost()
        {
            int postID = _Request.Get<int>("postid", Method.Get, 0);
            if (postID > 0)
            {
                PostV5 post = PostBOV5.Instance.GetPost(postID, false);
                if (post != null && post.UserID == MyUserID)
                {
                    using (ErrorScope es = new ErrorScope())
                    {
                        bool success = false;
                        try
                        {
                            success = PostBOV5.Instance.DeletePosts(My, new int[] { postID }, false, true, false, "用户自己删除");
                        }
                        catch (Exception ex)
                        {
                            ShowError(ex.Message);
                        }

                        if (success == false)
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                ShowError(error.Message);
                            });
                        }
                        else
                        {
                            ShowSuccess("删除成功", BbsUrlHelper.GetThreadUrl(Forum.CodeName, ThreadID, Thread.ThreadTypeString, PageNumber, ForumListPage));
                        }
                    }
                }
            }
        }

        private void ProcessDeleteSelfThread()
        {
            if (Thread.PostUserID == MyUserID)
            {
                using (ErrorScope es = new ErrorScope())
                {
                    bool success = false;
                    try
                    {
                        success = PostBOV5.Instance.DeleteThreads(My, new int[] { ThreadID }, false, true, false, true, "用户自己删除");
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message);
                    }

                    if (success == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            ShowError(error.Message);
                        });
                    }
                    else
                        ShowSuccess("删除成功", BbsUrlHelper.GetForumUrl(Forum.CodeName, ForumListAction, ForumListPage));
                }
            }
        }

        /*
        protected string GetThreadActionList(PostV5 reply, string linkStyle, string separator)
        {
            if (string.IsNullOrEmpty(linkStyle))
                linkStyle = "<a href=\"{0}\" {2}>{1}</a>";

            if (separator == null)
                separator = " ";

            bool isShowEdit = true;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //使用道具
            sb.Append(string.Format(linkStyle, UrlUtil.JoinUrl(Globals.GetRelativeUrl(SystemDirecotry.Root), "/max-dialogs/prop-use.aspx?tid=" + Thread.ThreadID + "&cat=" + (reply.PostType == PostType.ThreadContent ? PropTypeCategory.Thread : PropTypeCategory.ThreadReply)), "使用道具", " onclick=\"return openDialog(this.href, refresh)\"") + separator);

            //如果当前用户拥有屏蔽用户的权限，显示屏蔽用户的链接
            if (reply.UserID != 0)
            {
                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.BanUser, reply.UserID))
                {
                    if (Forum.IsShieldedUser(reply.UserID))
                    {
                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldUserUrl(reply.UserID), "解除屏蔽用户", " onclick=\"return openDialog(this.href, refresh)\"") + separator);
                        isShowEdit = false;
                    }
                    else
                    {
                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldUserUrl(reply.UserID), "屏蔽用户", " onclick=\"return openDialog(this.href, refresh)\"") + separator);
                    }
                }
            }
            //如果当前用户拥有屏蔽回复的权限，显示屏蔽回复的链接
            if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetPostShield, reply.UserID))
            {
                if (reply.IsShielded)
                {
                    sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldPostUrl(Forum.CodeName, "RescindShieldPost", reply.ThreadID, reply.PostID, PageNumber-1), "解除屏蔽帖子", string.Empty) + separator);
                    isShowEdit = false;
                }
                else
                {
                    sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldPostUrl(Forum.CodeName, "ShieldPost", reply.ThreadID, reply.PostID, PageNumber-1), "屏蔽帖子", string.Empty) + separator);
                }
            }

            if (isShowEdit)
                appendEditLink(sb, reply, Forum, linkStyle, separator);

            if (string.IsNullOrEmpty(Type))
            {
                appendFinalQuestionLink(sb, reply, Forum, linkStyle, separator);
                appendDeleteLink(sb, reply, Forum, linkStyle, separator);
            }
            if (sb.Length > separator.Length)
                return sb.ToString(0, sb.Length - separator.Length);
            else
                return sb.ToString();
        }

        private void appendEditLink(StringBuilder sb, PostV5 reply, ForumV5 Forum, string linkStyle, string separator)
        {
            if ((string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0) || (reply.PostIndex != 0 && ((reply.UserID == MyUserID && Can(ForumPermissionSetNode.Action.UpdateOwnPost)) || (CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, reply.UserID)))))
            {
                sb.Append(string.Format(linkStyle, BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editpost", reply.ThreadID, reply.PostID, Type), "编辑", string.Empty) + separator);
            }
            else if (reply.PostIndex == 0 && ((reply.UserID == MyUserID && Can(ForumPermissionSetNode.Action.UpdateOwnPost)) || (CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, reply.UserID))))
            {
                sb.Append(string.Format(linkStyle, BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editthread", reply.ThreadID, reply.PostID, Type), "编辑", string.Empty) + separator);
            }
        }

        protected virtual string appendFinalQuestionLink(StringBuilder sb, PostV5 reply, ForumV5 Forum, string linkStyle, string separator)
        {
            return string.Empty;
        }

        //private void appendFinalQuestionLink(StringBuilder sb, PostV5 reply, ForumV5 Forum, string linkStyle, string separator)
        //{
        //    if (IsShowFinalQuestionLink && reply.PostIndex == 0 && (MyUserID == reply.UserID || CanManage(ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, reply.UserID)))
        //    {
        //        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetFinalQuestionUrl(reply.ThreadID, Forum.ForumID), "结帖", string.Empty) + separator);
        //    }
        //}

        private void appendDeleteLink(StringBuilder sb, PostV5 reply, ForumV5 Forum, string linkStyle, string separator)
        {
            if (reply.PostIndex != 0)
            {
                if (reply.UserID == MyUserID)
                {
                    if (Can(ForumPermissionSetNode.Action.DeleteOwnPosts))
                        sb.Append(string.Format(linkStyle, "javascript:Widget.Confirm('确认删除','确认要删除您自己的回复吗？',function(){location.href='" + BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeletePostSelf.ToString(), reply.ThreadID, reply.PostID) + "'});", "删除", string.Empty) + separator);
                }
                else
                {
                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, reply.UserID))
                    {
                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeletePost.ToString(), reply.ThreadID, reply.PostID), "删除", string.Empty) + separator);
                    }
                }
            }
            else
            {
                if (reply.UserID == MyUserID)
                {
                    if (Can(ForumPermissionSetNode.Action.DeleteOwnPosts))
                        sb.Append(string.Format(linkStyle, "javascript:Widget.Confirm('确认删除','确认要删除您自己的主题吗？',function(){location.href='" + BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeleteOwnThread.ToString(), reply.ThreadID) + "'});", "删除", string.Empty) + separator);
                }
                else
                {
                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, reply.UserID))
                    {
                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeleteThread.ToString(), reply.ThreadID), "删除", string.Empty) + separator);
                    }
                }
            }
        }

        */



        public string GetActionTypeName(object actionType)
        {
            ModeratorCenterAction threadActionType = (ModeratorCenterAction)Convert.ToInt32(actionType);
            return PostBOV5.Instance.GetActionName(threadActionType, Thread);
        }

        private Dictionary<int, bool> AllowImageAttachmentPermission = new Dictionary<int, bool>();
        protected bool IsImage(string fileExtName, int userID)
        {
            bool isImage = MaxLabs.bbsMax.Ubb.PostUbbParserV5.isImage(fileExtName);
            if (isImage == false)
                return false;
            //判断权限
            bool allow = false;
            if (AllowImageAttachmentPermission.TryGetValue(userID, out allow) == false)
            {
                if (ForumSetting.AllowImageAttachment.GetValue(UserBO.Instance.GetUser(userID,GetUserOption.WithAll)))
                    allow = true;
                AllowImageAttachmentPermission.Add(userID, allow);
            }

            return allow;
        }







        #region setting

        protected int PageSize
        {
            get
            {
                return BbsSettings.PostsPageSize;
            }
        }

        protected bool DisplayAvatar
        {
            get
            {
                return BbsSettings.DisplayAvatar;
            }
        }

        protected bool AllowQuicklyReply
        {
            get
            {
                return BbsSettings.AllowQuicklyReply;
            }
        }

        protected bool ReplyReturnThreadLastPage
        {
            get
            {
                if (My.ReplyReturnThreadLastPage == null)
                    return ForumSetting.ReplyReturnThreadLastPage;
                else
                    return My.ReplyReturnThreadLastPage.Value;
            }
        }

        #endregion

        #region  权限

        private bool? m_IsShowModeratorManageLink;
        protected override bool IsShowModeratorManageLink
        {
            get
            {
                if (m_IsShowModeratorManageLink == null)
                {
                    if (CanManageThread
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts)
                        || base.IsShowModeratorManageLink
                        )
                        m_IsShowModeratorManageLink = true;
                    else
                        m_IsShowModeratorManageLink = false;
                }
                return m_IsShowModeratorManageLink.Value;
            }
        }

        private bool? m_CanManageThread;
        protected override bool CanManageThread
        {
            get
            {
                if (m_CanManageThread == null)
                {
                    if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SplitThread)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.JoinThreads)
                        || base.CanManageThread
                        )
                        m_CanManageThread = true;
                    else
                        m_CanManageThread = false;
                }
                return m_CanManageThread.Value;
            }
        }



        private bool? m_CanDeletePostsForSomeone = null;
        protected bool CanDeletePostsForSomeone
        {
            get
            {
                if (m_CanDeletePostsForSomeone == null)
                {
                    if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts))
                        m_CanDeletePostsForSomeone = true;
                    else
                        m_CanDeletePostsForSomeone = false;
                }
                return m_CanDeletePostsForSomeone.Value;
            }
        }

        /// <summary>
        /// 是否显示 已被沉帖
        /// </summary>
        protected bool IsShowThreadUpdateSortOrder
        {
            get
            {
                if (Thread.UpdateSortOrder == true)
                    return false;

                if (false == CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder, Thread.PostUserID))
                {
                    return false;
                }
                return true;
            }
        }


        private bool? m_ViewThread;
        protected bool ViewThread
        {
            get
            {
                if (m_ViewThread == null)
                    m_ViewThread = Can(ForumPermissionSetNode.Action.ViewThread);

                return m_ViewThread.Value;
            }
        }

        private bool? m_ViewReply;
        protected bool ViewReply
        {
            get
            {
                if (m_ViewReply == null)
                    m_ViewReply = Can(ForumPermissionSetNode.Action.ViewReply);
                return m_ViewReply.Value;
            }
        }

        private bool? m_ViewAttachment;
        protected bool ViewAttachment
        {
            get
            {
                if (m_ViewAttachment == null)
                    m_ViewAttachment = Can(ForumPermissionSetNode.Action.ViewAttachment);

                return m_ViewAttachment.Value;
            }
        }



        protected bool IsShowContent(PostV5 post)
        {
            if (Forum.IsShieldedUser(post.UserID) || post.IsShielded)
            {
                return AlwaysViewShieldContents(post.UserID);
            }
            else
                return true;

        }


        private bool? m_AlwaysViewContents;
        protected bool AlwaysViewContents
        {
            get
            {
                if (m_AlwaysViewContents == null)
                {
                    if (Can(ForumPermissionSetNode.Action.AlwaysViewContents))
                        m_AlwaysViewContents = true;
                    else
                        m_AlwaysViewContents = false;
                }

                return m_AlwaysViewContents.Value;
            }
        }

        protected bool AlwaysViewShieldContents(int posterUserID)
        {
            if (ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.AlwaysViewContents, posterUserID))
                return true;
            else
                return false;
        }

        private bool? m_CanReplyThread;
        protected bool CanReplyThread
        {
            get
            {
                if (m_CanReplyThread == null)
                {
                    m_CanReplyThread = Can(ForumPermissionSetNode.Action.ReplyThread);
                }
                return m_CanReplyThread.Value;
            }
        }

        protected bool CanSeeContent(PostV5 post)
        {
            return CanSeeContent(post, post.PostType == PostType.ThreadContent);
        }
        protected bool CanSeeContent(PostV5 post, bool isThreadContent)
        {
            if (isThreadContent)//(post.PostIndex == 0)
            {
                if (Thread.IsValued)
                {
                    if (Can(ForumPermissionSetNode.Action.ViewValuedThread))
                        return true;
                    else
                        return false;
                }
                else if (Can(ForumPermissionSetNode.Action.ViewThread))
                    return true;
                else
                    return false;
            }
            else
                return Can(ForumPermissionSetNode.Action.ViewReply);
        }

        protected bool isShowAttachments(PostV5 post)
        {
            if (post.PostType == PostType.ThreadContent && Thread.Price > 0 && Thread.PostUserID != MyUserID && AlwaysViewContents == false && Thread.IsOverSellDays(ForumSetting) == false && Thread.IsBuyed(My) == false)
                return false;

            return true;
        }

        protected bool isShowAttachmentImage(Attachment attachment)
        {
            if (attachment.IsInContent)
                return false;
            if (IsImage(attachment.FileType, attachment.UserID))
            {
                if (attachment.Price > 0)
                {
                    if (attachment.UserID == MyUserID || attachment.IsBuyed(My) || AlwaysViewContents || IsOverSellAttachmentDays(attachment))
                        return true;
                }
                else
                    return true;
            }
            return false;
        }


        protected bool isShowSignature(PostV5 reply)
        {
            if (reply.UserID == 0)
                return false;

            //if (My.IsSpider)
            //    return false;

            if (Forum.IsShieldedUser(reply.UserID))
                return false;

            if (string.IsNullOrEmpty(reply.User.SignatureFormatted))
                return false;

            if (reply.IsShielded)
                return false;

            if (AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(ForumID).Can(reply.UserID, ForumPermissionSetNode.Action.DisplayContent) == false)
                return false;

            if (reply.User.SignaturePropFlag.Available)
                return true;
            
            if (!reply.EnableSignature)
                return false;

            if (AllSettings.Current.BbsSettings.DisplaySignature.GetValue(reply.UserID))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region  积分

        protected UserPoint SellThreadPoint
        {
            get
            {
                return GetSellThreadPoint(Thread.PostUserID);
            }
        }

        protected bool IsShowChargePointLink(UserPoint point)
        {
            return PostBOV5.Instance.IsShowChargePointLink(point);
        }


        Dictionary<int, UserPoint> attachmentPoints = new Dictionary<int, UserPoint>();
        protected UserPoint GetAttachmentPoint(int userID)
        {
            if (attachmentPoints.ContainsKey(userID))
                return attachmentPoints[userID];
            else
            {
                UserPoint userPoint = ForumPointAction.Instance.GetUserPoint(userID, ForumPointValueType.SellAttachment, Forum.ForumID);
                attachmentPoints.Add(userID, userPoint);
                return userPoint;
            }
        }


        private UserPoint m_QuestionPoint;
        protected UserPoint QuestionPoint
        {
            get
            {
                if (m_QuestionPoint == null)
                {
                    m_QuestionPoint = ForumPointAction.Instance.GetUserPoint(Thread.PostUserID, ForumPointValueType.QuestionReward, Forum.ForumID);
                }
                return m_QuestionPoint;
            }
        }

        #endregion





        /// <summary>
        /// 帖内广告
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="position">位置， 上</param>
        /// <returns></returns>
        protected string InPostTopAD(int floor,bool isLastFloor)
        {
            if (PageNumber == 1)
                floor += 1;
            string target = GetTarget(this.CurrentForumID);
            if (AdSettings.EnableDefer)
                return AddAdDeferItem(ADCategory.InPostAd.ID + "_top", AdvertBO.Instance.GetInPostAD(target, ADPosition.Top, floor, isLastFloor));
            return AdvertBO.Instance.GetInPostAD(target, ADPosition.Top, floor, isLastFloor);
        }

        /// <summary>
        /// 帖内广告
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="position">位置， 下</param>
        /// <returns></returns>
        protected string InPostBottomAD(int floor,bool isLastFloor)
        {
            if (PageNumber == 1)
                floor += 1;
            string target = GetTarget(this.CurrentForumID);
            if (AdSettings.EnableDefer)
                return AddAdDeferItem(ADCategory.InPostAd.ID + "_bottom", AdvertBO.Instance.GetInPostAD(target, ADPosition.Bottom, floor, isLastFloor));
            return AdvertBO.Instance.GetInPostAD(target, ADPosition.Bottom, floor, isLastFloor);
        }

        /// <summary>
        /// 帖内广告
        /// </summary>
        /// <param name="forumId"></param>
        /// <param name="position">位置， 右</param>
        /// <returns></returns>
        protected string InPostRightAD(int floor, bool isLastFloor)
        {
            if (PageNumber == 1)
                floor += 1;
            string target = GetTarget(this.CurrentForumID);
            if (AdSettings.EnableDefer)
                return AddAdDeferItem(ADCategory.InPostAd.ID + "_right", AdvertBO.Instance.GetInPostAD(target, ADPosition.Right, floor, isLastFloor));
            return AdvertBO.Instance.GetInPostAD(target, ADPosition.Right, floor, isLastFloor);
        }


        protected override bool ShowLoginDialog
        {
            get
            {
                if (BbsSettings.EnableShowLoginDialog == false)
                    return false;
                if (IsLogin
                    ||Thread.AttachmentType != ThreadAttachType.Image
                    ||ForumPermission.Can(My, ForumPermissionSetNode.Action.ViewAttachment))
                    return false;
                return true;
            }
        }


        protected string UrlEncode(string text)
        {
            return Server.UrlEncode(text);
        }

    }
}