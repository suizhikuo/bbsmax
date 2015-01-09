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
using System.Web.UI;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using System.Text;
using MaxLabs.bbsMax.ValidateCodes;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    /// <summary>
    /// Post 的摘要说明
    /// </summary>
    public partial class view_question : view_thread
    {

        protected override void ProcessSubmits()
        {
            if (_Request.IsClick("UsefulButton"))
            {
                voteQuestionBestPost(true);
                HadProcessSubmit = true;
                UpdateView = false;
            }
            else if (_Request.IsClick("UnUsefulButton"))
            {
                voteQuestionBestPost(false);
                HadProcessSubmit = true;
                UpdateView = false;
            }
            else
                base.ProcessSubmits();
        }

        protected override BasicThread Thread
        {
            get
            {
                return QuestionThread;
            }
        }

        private QuestionThread m_QuestionThread;
        protected QuestionThread QuestionThread
        {
            get
            {
                if (m_QuestionThread == null)
                {
                    GetThread();
                }
                return m_QuestionThread;
            }
        }



        protected override void GetThread()
        {
            if (IsOnlyLookOneUser)
            {
                Response.Redirect(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, 1, LookUserID, PostBOV5.Instance.GetThreadTypeString(ThreadType.Normal), ForumListPage));
            }


            if (IsUnapprovePosts)
            {
                BbsRouter.JumpToUrl(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(ThreadType.Normal)), "type=" + Type);
            }

            if (string.IsNullOrEmpty(Type))
            {
                ThreadType realThreadType;

                PostBOV5.Instance.GetQuestionWithReplies(ThreadID, PageNumber, PageSize, true, UpdateView, out m_QuestionThread, out m_PostList, out realThreadType);

                //如果不是 问题帖 则跳到相应的页面
                if (realThreadType != ThreadType.Question)
                {
                    Response.Redirect(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(realThreadType)));
                }
            }
            else
            {
                BasicThread thread;
                GetPosts(ThreadType.Question, out m_TotalPosts, out thread, out m_PostList);

                m_QuestionThread = (QuestionThread)thread;

            }

            PostCollectionV5 tempPosts;
            if (m_QuestionThread!=null && m_QuestionThread.BestPost != null)
            {
                tempPosts = new PostCollectionV5(m_PostList);
                tempPosts.Add(m_QuestionThread.BestPost);
            }
            else
                tempPosts = m_PostList;

            PostBOV5.Instance.ProcessKeyword(tempPosts, ProcessKeywordMode.TryUpdateKeyword);

            //if (_Request.IsSpider == false)
            //{
                //List<int> userIDs = new List<int>();
                //foreach (PostV5 post in tempPosts)
                //{
                //    userIDs.Add(post.UserID);
                //}
                //UserBO.Instance.GetUsers(userIDs, GetUserOption.WithAll);
            UserBO.Instance.GetUsers(tempPosts.GetUserIds(), GetUserOption.WithAll);
            //}
        }

        private int? m_TotalPosts;
        protected override int TotalPosts
        {
            get
            {
                if (m_TotalPosts == null)
                {
                    return base.TotalPosts;
                    //GetThread();
                    //if (m_TotalPosts == null)
                    //    m_TotalPosts = base.TotalPosts;
                }
                return m_TotalPosts.Value;
            }
        }

   
        protected override PostV5 BestPost
        {
            get
            {
                return QuestionThread.BestPost;
            }
        }


        //private bool? m_IsShowThreadContent;
        //protected bool IsShowThreadContent
        //{
        //    get
        //    {
        //        if (m_IsShowThreadContent == null)
        //        {
        //            m_IsShowThreadContent = IsShowContent(ThreadContent) && CanSeeContent(ThreadContent, true);
        //        }
        //        return m_IsShowThreadContent.Value;
        //    }
        //}
        private bool? m_IsShowBestPostContent;
        protected bool IsShowBestPostContent
        {
            get
            {
                if (m_IsShowBestPostContent == null)
                {
                    if (BestPost == null)
                        m_IsShowBestPostContent = false;
                    else
                        m_IsShowBestPostContent = IsShowContent(BestPost) && CanSeeContent(BestPost, false);
                }
                return m_IsShowBestPostContent.Value;
            }
        }

        private bool? m_IsShowFinalQuestionLink = null;
        protected bool IsShowFinalQuestionLink
        {
            get
            {
                if (m_IsShowFinalQuestionLink == null)
                {
                    if (!Thread.IsClosed && (MyUserID == Thread.PostUserID || CanManage(ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, Thread.PostUserID)))
                        m_IsShowFinalQuestionLink = true;
                    else
                        m_IsShowFinalQuestionLink = false;
                }
                return m_IsShowFinalQuestionLink.Value;
            }
        }

        private bool? m_IsShowPostList;
        protected bool IsShowPostList
        {
            get
            {
                if (m_IsShowPostList == null)
                {
                    if (Thread.IsClosed)
                    {
                        m_IsShowPostList = true;
                    }
                    else if (MyUserID == QuestionThread.PostUserID || QuestionThread.AlwaysEyeable || AlwaysViewContents || Thread.IsReplied(My))
                    {
                        m_IsShowPostList = true;
                    }
                    else
                    {
                        m_IsShowPostList = false;
                    }
                }
                return m_IsShowPostList.Value;
            }
        }

        //protected bool HaveReply
        //{
        //    get
        //    {
        //        if (PageNumber == 1 && PostList.Count > 1)
        //            return true;
        //        else if (PageNumber > 1 && PostList.Count > 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //}

        protected int GetReward(int postID)
        {
            if (QuestionThread.Rewards == null)
                return 0;

            int reward;
            QuestionThread.Rewards.TryGetValue(postID, out reward);
            return reward;
        }

        //private User threadUserProfile;
        //protected User ThreadUserProfile
        //{
        //    get
        //    {
        //        if (threadUserProfile == null)
        //            threadUserProfile = UserBO.Instance.GetUser(Thread.PostUserID, GetUserOption.WithDeletedUser);
        //        return threadUserProfile;
        //    }
        //}

        private ThreadCollectionV5 m_UserQuestionThreads;
        protected ThreadCollectionV5 UserQuestionThreads
        {
            get
            {
                if (m_UserQuestionThreads == null)
                {
                    m_UserQuestionThreads = PostBOV5.Instance.GetUserQuestionThreads(MyUserID, 5, ThreadID);
                    PostBOV5.Instance.ProcessKeyword(m_UserQuestionThreads, ProcessKeywordMode.TryUpdateKeyword);
                }
                return m_UserQuestionThreads;
            }
        }


        private void voteQuestionBestPost(bool isUseful)
        {
            try
            {
                bool success = PostBOV5.Instance.VoteQuestionBestPost(My, ThreadID, isUseful);
                if (success)
                {
                    AlertSuccess("操作成功");
                }
                else
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


        protected override bool IsShowPostGetReward(int postID)
        {
            if (IsUnapprovePosts == false && Thread.IsClosed && GetReward(postID) > 0)
                return true;
            else
                return false;
        }


        //private int? m_LastPostID = null;
        //protected int LastPostID
        //{
        //    get
        //    {
        //        if (m_LastPostID == null)
        //        {
        //            if (PostList.Count > 0)
        //                m_LastPostID = PostList[PostList.Count - 1].PostID;
        //            else
        //                m_LastPostID = 0;
        //        }
        //        return m_LastPostID.Value;
        //    }
        //}


        //protected bool IsUnapprovePosts
        //{
        //    get
        //    {
        //        if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0 ||
        //            string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0 ||
        //            string.Compare(Type, MyThreadType.MyUnapprovedThread.ToString(), true) == 0)
        //            return true;
        //        else
        //            return false;
        //    }
        //}


        //private string m_MetaDescription;
        //protected new string MetaDescription
        //{
        //    get
        //    {
        //        if (m_MetaDescription == null)
        //        {
        //            if (PostList.Count > 0)
        //            {
        //                PostV5 post = PostList[0];
        //                if (!Forum.IsShieldedUser(post.UserID) && !post.IsShielded && IsShowContent(post))
        //                {
        //                    m_MetaDescription = StringUtil.HtmlEncode(StringUtil.CutString(ClearHTML(post.ContentText), 200));
        //                }
        //                else
        //                    m_MetaDescription = StringUtil.HtmlEncode(ClearHTML(Thread.SubjectText));
        //            }
        //            else
        //                m_MetaDescription = StringUtil.HtmlEncode(ClearHTML(Thread.SubjectText));
        //        }
        //        return m_MetaDescription;
        //    }
        //}








//        /// <summary>
//        /// 重写掉更新在线的方法，自己去更新在线信息
//        /// </summary>
//        protected override void UpdateOnline()
//        { }

//        protected Forum subforum;
//        protected SystemForum systemForumType = SystemForum.Normal;

//        protected Role role;

//        //protected zzbird.Common.Medals.Medal medal;

//        protected PollItem pollItem;
//        protected MaxLabs.bbsMax.Entities.Post reply;

//        //回复评分
//        protected PostMark postMark;
//        //附件
//        protected Attachment attachment;

//        protected string disabledHtml, disabledEmoticon, disabledMaxCode, disabledSignature, disabledReplyNotice;


//        private string codeName;
//        protected string CodeName
//        {
//            get
//            {
//                if (codeName == null)
//                {
//                    codeName = GetString("CodeName", "get");
//                }
//                return codeName;
//            }
//            set
//            {
//                codeName = value;
//            }
//        }

//        private string type;
//        protected string Type
//        {
//            get
//            {
//                if (type == null)
//                {
//                    type = GetString("type", "get");
//                }
//                return type;
//            }
//        }

//        //protected bool IsShowOrigionContent
//        //{
//        //    get
//        //    {
//        //        return string.IsNullOrEmpty(Type) == false;
//        //    }
//        //}

//        private int? threadID = null;
//        protected int ThreadID
//        {
//            get
//            {
//                if (threadID == null)
//                {
//                    threadID = GetInt32("ThreadID", "get", 0);
//                    if (threadID <= 0)
//                        ShowError("帖子不存在");
//                }
//                return threadID.Value;
//            }
//        }

//        private string searchText;
//        protected string SearchText
//        {
//            get
//            {
//                if (searchText == null)
//                {
//                    searchText = Server.UrlDecode(GetString("SearchText", "get", string.Empty));
//                }
//                return searchText;
//            }
//        }

//        private int? pageIndex = null;
//        protected int PageIndex
//        {
//            get
//            {
//                if (pageIndex == null)
//                {
//                    pageIndex = GetPageIndex();
//                }
//                return pageIndex.Value;
//            }
//            set
//            {
//                pageIndex = value;
//            }
//        }

//        private int? forumID = null;
//        protected int ForumID
//        {
//            get
//            {
//                if (forumID == null)
//                {
//                    //根据友好名称得到版块的ID
//                    forumID = ForumManager.GetForumID(CodeName);

//                    //非法的版块ID
//                    if (forumID == 0)
//                    {
//                        if (string.Compare(CodeName, SystemForum.UnapproveThreads.ToString(), true) == 0)
//                        {
//                            forumID = -2;
//                        }
//                        else if (string.Compare(CodeName, SystemForum.UnapprovePosts.ToString(), true) == 0)
//                        {
//                            forumID = -3;
//                        }
//                        else
//                        {
//                            ShowError("版块不存在");
//                        }
//                    }
//                }
//                return forumID.Value;
//            }
//            set { forumID = value; }
//        }

//        private int? userID = null;
//        protected int UserID
//        {
//            get
//            {
//                if (userID == null)
//                {
//                    userID = GetInt32("UserID", "get", -1);
//                }
//                return userID.Value;
//            }
//        }

//        private Forum forum;
//        protected Forum Forum
//        {
//            get
//            {
//                if (forum == null)
//                {
//                    forum = ForumManager.GetForum(ForumID);

//                    //版块不存在
//                    if (forum == null)
//                        ShowError("版块不存在！");

//                    //如果Codename是回收站或者审核站，那么Type只能是和他相对应的，避免被普通用户利用而可以查看没有权限进入的版块的帖子
//                    if (ForumID < 0)
//                    {
//                        if (forum.ForumID == (int)SystemForum.RecycleBin)
//                        {
//                            if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) != 0)
//                                ShowError("非法操作！");
//                        }
//                        else if (forum.ForumID == (int)SystemForum.UnapproveThreads)
//                        {
//                            if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) != 0 &&
//                                string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) != 0)
//                                ShowError("非法操作！");
//                        }
//                        else
//                            ShowError("非法操作！");
//                    }
//                    //只有正常版块才检查版块相关的权限
//                    else
//                    {
//                        //zzbird.bbsMax.ForumPermission permission = forum.Permission;

//                        ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forum.ForumID);

//                        //没有进入版块的权限
//                        if (!forum.CanVisitForum(My))
//                            ShowError("您没有权限进入该版块！");

//                        //如果论坛版块类型是链接，则跳转
//                        if (forum.ForumType == ForumType.Link)
//                            Response.Redirect(forum.Password);
//                        //如果论坛版块类型是分类，则停止处理
//                        //else if (Forum.ForumType == ForumType.Catalog)
//                        //    ShowError("版块");
//                        //版块类型正常，但需要密码
//                        if (!string.IsNullOrEmpty(forum.Password))
//                        {
//                            //如果当前用户不拥有“进入版块不需要密码”的权限，继续检查
//                            if (!forum.SigninForumWithoutPassword(My))
//                            {
//                                //检查这个用户之前是否已经通过这个版块的验证，避免重复输入密码
//#if V5
//#else
//                            if (!My.IsValidatedForum(forum))
//                                Response.Redirect(BbsUrlHelper.GetSignInForumUrl(CodeName, ThreadID));
//#endif
//                            }
//                        }


//                        //==============================================

//                    }
//                }
//                return forum;
//            }
//            set
//            {
//                forum = value;
//            }
//        }


//        private Thread thread;
//        protected Thread Thread
//        {
//            get
//            {
//                if (thread == null)
//                {
//                    GetThread();
//                }
//                return thread;
//            }
//        }


//        private List<MaxLabs.bbsMax.Entities.Post> replies;
//        protected List<MaxLabs.bbsMax.Entities.Post> Replies
//        {
//            get
//            {
//                if (replies == null)
//                {
//                    GetThread();
//                }
//                return replies;
//            }
//        }

//        bool updateView = true;
//        int tempPage;
//        string extParms = null;
//        string action = "list";
//        int threadCatalogID = -1;
//        private void GetUrlParms()
//        {
//            if (extParms != null)
//                return;

//            extParms = Request.QueryString["extParms"];
//            if (extParms != null)//从分类或者投票等列表页进来的
//            {
//                try
//                {
//                    tempPage = int.Parse(GetExtParm("page", extParms));
//                }
//                catch
//                {
//                    tempPage = 1;
//                }
//                action = GetExtParm("action", extParms, "list");

//                try
//                {
//                    threadCatalogID = int.Parse(GetExtParm("ThreadCatalogID", extParms, "-1"));
//                }
//                catch
//                {
//                    threadCatalogID = -1;
//                }
//            }
//            else//从一般列表页进来的
//            {
//                try
//                {
//                    tempPage = int.Parse(Request.QueryString["listPage"].Trim());
//                }
//                catch
//                {
//                    tempPage = 1;
//                }
//            }
//        }

//        private bool? m_IsGetPost;
//        /// <summary>
//        /// 只显示一个 回复
//        /// </summary>
//        protected bool IsGetPost
//        {
//            get
//            {
//                if (m_IsGetPost == null)
//                {
//                    m_IsGetPost = string.Compare(_Request.Get("type", Method.Get, string.Empty), "getpost", true) == 0;
//                }
//                return m_IsGetPost.Value;
//            }
//        }

//        protected ThreadType ThreadType
//        {
//            get
//            {
//                if (IsGetPost)
//                    return ThreadType.Normal;
//                else
//                    return Thread.ThreadType;
//            }
//        }

//        protected string FormatDate(DateTime datetime)
//        {
//            if (datetime < DateTimeUtil.Now)
//                return "截止时间:" + datetime.ToString();
//            else
//            {
//                TimeSpan timeSpan = datetime - DateTimeUtil.Now;
//                if (timeSpan.TotalSeconds > 0)
//                {
//                    string time = "";
//                    if (timeSpan.Days > 0)
//                        time += timeSpan.Days + "天";
//                    if (timeSpan.Hours > 0)
//                        time += timeSpan.Hours + "小时";
//                    if (timeSpan.Minutes > 0)
//                        time += timeSpan.Minutes + "分";
//                    if (timeSpan.Seconds > 0)
//                        time += timeSpan.Seconds + "秒";

//                    return "距离截止时间还有" + time;
//                }
//                return "截止时间:" + datetime.ToString();
//            }

//        }


//        private void GetThread()
//        {
//            int totalPosts;

//            GetThreadStatus status = GetThreadStatus.Success;

//            if (IsGetPost)
//            {

//                status = PostManager.GetThread(ThreadID, true, 1, 1, false, out thread, out replies, out totalPosts);


//                int postID = _Request.Get<int>("PostID", Method.Get, 0);

//                MaxLabs.bbsMax.Entities.Post post = PostManager.GetPost(postID);
//                if (post == null)
//                {
//                    ShowError("该帖子不存在或者已被删除");
//                }
//                post.PostMarks = new List<PostMark>();
//                replies.Clear();
//                replies.Add(post);
//            }
//            //如果是正常的查看模式（不是“只看楼主”或“只看该用户”）
//            else if (UserID == -1)
//            {

//                //正常情况
//                if (string.IsNullOrEmpty(Type))
//                {
//                    status = PostManager.GetThread(ThreadID, true, PageIndex, PageSize, updateView, out thread, out replies, out totalPosts);
//                    if (thread != null)
//                        totalPosts = thread.TotalReplies + 1;
//                }
//                //查看回收站的主题，或者查看未审核的主题
//                else if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0 ||
//                       string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
//                {
//                    status = PostManager.GetThread(ThreadID, false, PageIndex, PageSize, false, out thread, out replies, out totalPosts);
//                    if (thread != null)
//                        totalPosts = thread.TotalReplies + 1;
//                }
//                //查看具有未通过审核的回复的主题
//                else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
//                {
//                    PostManager.GetUnapprovedPostThread(ThreadID, PageIndex, PageSize, out thread, out replies, out totalPosts);
//                    foreach (MaxLabs.bbsMax.Entities.Post post in replies)
//                    {
//                        post.PostMarks = new List<PostMark>();
//                    }
//                }

//                else if (string.Compare(Type, MyThreadType.MyUnapprovedThread.ToString(), true) == 0 ||
//                        string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0)
//                {
//                    PostManager.GetMyUnapprovedPostThread(ThreadID, MyUserID, PageIndex, PageSize, out thread, out replies, out totalPosts);
//                    foreach (MaxLabs.bbsMax.Entities.Post post in replies)
//                    {
//                        post.PostMarks = new List<PostMark>();
//                    }
//                }
//                else
//                {
//                    status = PostManager.GetThread(ThreadID, true, PageIndex, PageSize, true, out thread, out replies, out totalPosts);
//                    if (thread != null)
//                        totalPosts = thread.TotalReplies + 1;
//                }
//            }
//            //如果是“只看楼主”或“只看该用户”
//            else
//            {
//                status = PostManager.GetThread(ThreadID, true, UserID, PageIndex, PageSize, true, out thread, out replies, out totalPosts);
//            }


//            if (status != GetThreadStatus.Success)
//                ShowError(status);


//            //如果帖子未通过审核，且
//            if (thread == null)
//                ShowError(GetThreadStatus.NotExists);



//            //检查Post的的的ForumID是否正确  不正确的更新
//            List<int> postIDs = new List<int>();
//            for (int i = 0; i < replies.Count; i++)
//            {
//                MaxLabs.bbsMax.Entities.Post post = replies[i];
//                if (post.ForumID != thread.ForumID)
//                {
//                    post.ForumID = thread.ForumID;
//                    postIDs.Add(post.PostID);
//                }
//            }
//            PostBO.Instance.UpdatePostsForumID(postIDs, thread.ForumID);




//            GetUrlParms();


//            if (Thread.ThreadType != ThreadType.Polemize)
//            {
//                string emoctionPagerUrl;
//                if (UserID == -1)
//                {
//                    if (string.IsNullOrEmpty(Type))
//                    {
//                        if (extParms != null)
//                        {
//                            if (ThreadType == ThreadType.Question && BestPost != null)
//                                SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, -1, extParms == null ? "" : extParms), PageIndex, PageSize, totalPosts, 1);
//                            else
//                                SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, -1, extParms == null ? "" : extParms), PageIndex, PageSize, totalPosts);

//                            emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, -1, PageIndex + 1, extParms == null ? "" : extParms, IsGetAllDefaultEmoticon, EmoticonGroupID);
//                        }
//                        else
//                        {
//                            if (ThreadType == ThreadType.Question && BestPost != null)
//                                SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, tempPage), PageIndex, PageSize, totalPosts, 1);
//                            else
//                                SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, tempPage), PageIndex, PageSize, totalPosts);

//                            emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, PageIndex + 1, tempPage, IsGetAllDefaultEmoticon, EmoticonGroupID);
//                        }

//                    }
//                    else
//                    {
//                        if (ThreadType == ThreadType.Question && BestPost != null)
//                            SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, Type), PageIndex, PageSize, totalPosts, 1);
//                        else
//                            SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, Type), PageIndex, PageSize, totalPosts);

//                        emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, Type, PageIndex + 1, IsGetAllDefaultEmoticon, EmoticonGroupID);
//                    }
//                }
//                else
//                {
//                    SetPager("PostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, UserID, 1), PageIndex, PageSize, totalPosts);

//                    emoctionPagerUrl = BbsUrlHelper.GetThreadUrlForEmoticonPager(CodeName, ThreadID, UserID, PageIndex + 1, 1, IsGetAllDefaultEmoticon, EmoticonGroupID);
//                }

//                if (IsShowReply && (AllowHtml || AllowMaxcode))
//                {
//                    SetPagerAndButtonCount("EmoticonsListPager", emoctionPagerUrl, EmoticonPage - 1, emoticonPageSize, CurrentGroupEmotioconCount, EmoticonPagerButtonCount);
//                }
//            }
//            else
//            {
//                //if (string.Compare(Type, "agree", true) == 0)
//                //{
//                //    SetPager("AgreePostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, "agree"), PageIndex, PageSize, AgreePostCount, "AgreePosts");
//                //}
//                //else if (string.Compare(Type, "against", true) == 0)
//                //{
//                //    SetPager("AgainstPostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, "against"), PageIndex, PageSize, AgainstPostCount);
//                //}
//                //else if (string.Compare(Type, "neutral", true) == 0)
//                //{
//                //    SetPager("NeutralPostListPager", BbsUrlHelper.GetThreadUrlForPager(CodeName, ThreadID, "neutral"), PageIndex, PageSize, NeutralPostCount);
//                //}
//                //else
//                //{


//                //}
//            }
//        }



//        private Poll poll;
//        protected Poll Poll
//        {
//            get
//            {
//                if (poll == null)
//                {
//                    GetPoll();
//                }
//                return poll;
//            }
//        }
//        private Dictionary<int, PollItem>.ValueCollection pollItems;
//        protected Dictionary<int, PollItem>.ValueCollection PollItems
//        {
//            get
//            {
//                if (pollItems == null)
//                {
//                    GetPoll();
//                }
//                return pollItems;
//            }
//        }
//        private int? voteTotalCount = null;
//        protected int VoteTotalCount
//        {
//            get
//            {
//                if (voteTotalCount == null)
//                {
//                    GetPoll();
//                }
//                return voteTotalCount.Value;
//            }
//        }
//        //private int? voteCount = null;
//        //protected int VoteCount
//        //{
//        //    get
//        //    {
//        //        if (voteCount == null)
//        //        {
//        //            GetPoll();
//        //        }
//        //        return voteCount.Value;
//        //    }
//        //}
//        private bool? isExpiresPoll = null;
//        protected bool IsExpiresPoll
//        {
//            get
//            {
//                if (isExpiresPoll == null)
//                {
//                    GetPoll();
//                }
//                return isExpiresPoll.Value;
//            }
//        }
//        private void GetPoll()
//        {
//            if (ThreadType == ThreadType.Poll)
//            {
//                GetPollStatus getPollStatus = PostManager.GetPoll(Thread.ThreadID, true, out poll);

//                if (getPollStatus != GetPollStatus.Success)
//                    ShowError(getPollStatus);

//                if (poll.PollItems == null)
//                    ShowError("没有权限查看或者未知错误！");

//                pollItems = poll.PollItems.Values;

//                int count = 0;
//                foreach (PollItem pollitem in pollItems)
//                {
//                    count += pollitem.PollItemCount;
//                }
//                voteTotalCount = count;
//                //if (voteTotalCount > 0)
//                //    voteCount = voteTotalCount;

//                isExpiresPoll = DateTime.Now > poll.ExpiresDate;
//            }
//        }

//        private MaxLabs.bbsMax.Entities.Post bestPost;
//        protected MaxLabs.bbsMax.Entities.Post BestPost
//        {
//            get
//            {
//                if (bestPost == null && question == null)
//                {
//                    GetQuestion();
//                }
//                return bestPost;
//            }
//        }
//        private Question question;
//        protected Question Question
//        {
//            get
//            {
//                if (question == null)
//                {
//                    GetQuestion();
//                }
//                return question;
//            }
//        }

//        protected Thread tempThread;
//        private List<Thread> userQuestionThreads;
//        protected List<Thread> UserQuestionThreads
//        {
//            get
//            {
//                if (userQuestionThreads == null)
//                {
//                    userQuestionThreads = PostManager.GetUserQuestionThreads(Thread.PostUserID, 5, Thread.ThreadID);
//                }
//                return userQuestionThreads;
//            }
//        }
//        private bool? isShowReplies = null;
//        protected bool IsShowReplies
//        {
//            get
//            {
//                if (isShowReplies == null)
//                {
//                    isShowReplies = true;
//                    GetQuestion();
//                }
//                return isShowReplies.Value;
//            }
//        }
//        private bool? isShowFinalQuestionLink = null;
//        protected bool IsShowFinalQuestionLink
//        {
//            get
//            {
//                if (isShowFinalQuestionLink == null)
//                {
//                    isShowFinalQuestionLink = false;
//                    GetQuestion();
//                }
//                return isShowFinalQuestionLink.Value;
//            }
//        }

//        private void GetQuestion()
//        {
//            if (ThreadType == ThreadType.Question)
//            {
//                question = PostManager.GetQuestion(Thread.ThreadID, false, out bestPost);
//                if (question == null)
//                    ShowError("没有权限查看或者未知错误！");
//                if (Thread.IsClosed)
//                {
//                    isShowReplies = true;
//                }
//                else if (question.AlwaysEyeable || Thread.IsReplied(MyUserID) || AlwaysViewContents)
//                {
//                    isShowReplies = true;
//                }
//                else
//                {
//                    isShowReplies = false;
//                }

//                if (!Thread.IsClosed && (MyUserID == Thread.PostUserID || CanManage(ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, Thread.PostUserID)))//(Forum.Permission.FinalAnyQuestion && Forum.AllowManageOtherUser(Thread.PostUserID))))//thread.PostUserID == UserProfile.UserID)
//                    isShowFinalQuestionLink = true;
//                else
//                    isShowFinalQuestionLink = false;

//            }

//        }

//        private bool? isShowReply = null;
//        protected bool IsShowReply
//        {
//            get
//            {
//                if (isShowReply == null)
//                {
//                    //以下情况不允许回复
//                    if (Thread.IsLocked ||
//                        !string.IsNullOrEmpty(Type) ||
//                        !AllowQuicklyReply ||
//                        !Can(ForumPermissionSetNode.Action.ReplyThread) ||
//                        Forum.IsShieldedUser(MyUserID))
//                    {
//                        isShowReply = false;
//                    }
//                    else
//                    {
//                        if (Thread.ThreadType == ThreadType.Polemize && !Can(ForumPermissionSetNode.Action.CanPolemize))
//                            isShowReply = false;
//                        else
//                            isShowReply = true;
//                    }
//                }
//                return isShowReply.Value;
//            }
//        }

//        private bool? isShowCheckBox = null;
//        protected bool IsShowCheckBox
//        {
//            get
//            {
//                if (isShowCheckBox == null)
//                {
//                    if (ManagePermission.HasPermissionForSomeone(MyUserID, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts))
//                        isShowCheckBox = true;
//                    else
//                        isShowCheckBox = false;
//                }
//                return isShowCheckBox.Value;
//            }
//        }


//        private int? lastPosterID = null;
//        protected int LastPosterID
//        {
//            get
//            {
//                if (lastPosterID == null)
//                {
//                    if (Replies.Count > 0)
//                        lastPosterID = Replies[Replies.Count - 1].PostID;
//                    else
//                        lastPosterID = 0;
//                }
//                return lastPosterID.Value;
//            }
//        }
//        /*
//        #region 收藏夹
//        protected FavoriteDirectory FavDirectory;
//        protected List<FavoriteDirectory> FavDirectories
//        {
//            get
//            {
//                return FavoriteManager.GetFavoriteDirectories(UserProfile.UserID);
//            }
//        }
//        #endregion
//        */
//        //private int? rank = null;
//        //protected int? Rank
//        //{
//        //    get
//        //    {
//        //        if (rank == null)
//        //        {
//        //            if (Request.QueryString["Rank"] != null)
//        //            {
//        //                try
//        //                {
//        //                    rank = int.Parse(Request.QueryString["Rank"]);
//        //                }
//        //                catch
//        //                {
//        //                    return 1;
//        //                }
//        //                if (rank > 5)
//        //                    rank = 5;
//        //                else if (rank < 1)
//        //                    rank = 1;
//        //            }
//        //        }
//        //        return rank;
//        //    }
//        //}
//        protected int PageSize
//        {
//            get
//            {
//                return AllSettings.Current.BbsSettings.PostsPageSize;
//            }
//        }

//        protected MaxLabs.bbsMax.Entities.Post ThreadContent
//        {
//            get
//            {
//                if (Thread.FirstPost == null)
//                {
//                    if (PageIndex == 0)
//                    {
//                        if (Replies.Count > 0)
//                            Thread.FirstPost = Replies[0];
//                        else
//                            ShowError("主题内容不存在！");
//                    }
//                    else
//                    {
//                        Thread.FirstPost = PostManager.GetThreadFirstPost(Thread.ThreadID);
//                        Thread.FirstPost.PostIndex = 0;
//                    }
//                }
//                return Thread.FirstPost;
//            }
//        }

//        protected bool IsShowContent(MaxLabs.bbsMax.Entities.Post post)
//        {
//            if (Forum.IsShieldedUser(post.UserID) || post.IsShielded || Permission.Can(post.UserID, ForumPermissionSetNode.Action.DisplayContent) == false)
//            {
//                return AlwaysViewShieldContents(post.UserID);
//            }
//            else
//                return true;

//        }
//        //protected bool IsShowContent(MaxLabs.bbsMax.Entities.Post post)
//        //{
//        //    if ((!Forum.IsShieldedUser(post.UserID) && !post.IsShielded && Permission.Can(post.UserID, ForumPermissionSetNode.Action.DisplayContent)) || AlwaysViewContents(post.UserID))
//        //        return true;
//        //    else
//        //        return false;
//        //}
//        //protected bool AlwaysViewContents(int posterUserID)
//        //{
//        //    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.AlwaysViewContents, posterUserID))
//        //        return true;
//        //    else
//        //        return false;
//        //}

//        private bool? m_AlwaysViewContents;
//        protected bool AlwaysViewContents
//        {
//            get
//            {
//                if (m_AlwaysViewContents == null)
//                {
//                    if (Permission.Can(My, ForumPermissionSetNode.Action.AlwaysViewContents))
//                        m_AlwaysViewContents = true;
//                    else
//                        m_AlwaysViewContents = false;
//                }

//                return m_AlwaysViewContents.Value;
//            }
//        }
//        protected bool AlwaysViewShieldContents(int posterUserID)
//        {
//            if (ManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.AlwaysViewContents, posterUserID))
//                return true;
//            else
//                return false;
//        }


//        protected string GetBuyAttachmentUrl(int attachmentID)
//        {
//            return AttachQueryString("OtherAction=buyattachment&attachmentID=" + attachmentID);

//            //// string action = "OtherAction=buyattachment";
//            //string url;
//            //if (RawUrl.IndexOf('?') > -1)
//            //{
//            //    url = RawUrl.Substring(0, RawUrl.IndexOf('?'));
//            //}
//            //else
//            //    url = RawUrl;
//            //return url + "?OtherAction=buyattachment&attachmentID=" + attachmentID;
//        }

//        protected bool IsUnapprovePosts
//        {
//            get
//            {
//                if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0 ||
//                    string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0 ||
//                    string.Compare(Type, MyThreadType.MyUnapprovedThread.ToString(), true) == 0)
//                    return true;
//                else
//                    return false;
//            }
//        }
//        protected bool CanSeeContent(MaxLabs.bbsMax.Entities.Post post)
//        {
//            return CanSeeContent(post, post.PostIndex == 0);
//        }
//        protected bool CanSeeContent(MaxLabs.bbsMax.Entities.Post post, bool isThreadContent)
//        {
//            if (isThreadContent)//(post.PostIndex == 0)
//            {
//                if (Thread.IsValued)
//                {
//                    if (Can(ForumPermissionSetNode.Action.ViewValuedThread))
//                        return true;
//                    else
//                        return false;
//                }
//                else if (Can(ForumPermissionSetNode.Action.ViewThread))
//                    return true;
//                else
//                    return false;
//            }
//            return true;
//            //else if (GetThreadContentPermission(Forum).CanSeePostContent.Value)
//            //    return true;
//            //else
//            //    return false;
//        }

//        protected SearchMode? ThreadSearchMode
//        {
//            get
//            {
//                string mode = GetString("SearchMode", "get", string.Empty);
//                if (mode.ToLower() == SearchMode.UserPost.ToString().ToLower())
//                    return SearchMode.UserPost;
//                else if (mode.ToLower() == SearchMode.Content.ToString().ToLower())
//                    return SearchMode.Content;
//                else
//                    return null;
//            }
//        }

//        private bool isClickButton = false;
//        protected void Page_Load(object sender, System.EventArgs e)
//        {
//            //搜索页链接过来的 如果是搜索回复内容或者用户回复时 需要定向到该回复
//            if (ThreadSearchMode != null)
//            {
//                if (ThreadSearchMode.Value == SearchMode.UserPost || ThreadSearchMode.Value == SearchMode.Content)
//                {
//                    Thread thread;
//                    List<MaxLabs.bbsMax.Entities.Post> posts;
//                    int repliesCount;
//                    GetThreadStatus status = PostManager.GetThread(ThreadID, true, 0, int.MaxValue, false, out thread, out posts, out repliesCount);
//                    if (status != GetThreadStatus.Success)
//                        ShowError<GetThreadStatus>(status);

//                    int postID = GetInt32("postID", "get", 0);
//                    if (postID == 0)
//                        ShowError("参数错误，需要参数postID");

//                    //List<MaxLabs.bbsMax.Entities.Post> posts = PostManager.GetPosts(ThreadID);

//                    int totalCount = 0;
//                    int postIndex = 0;
//                    if (thread.ThreadType == ThreadType.Polemize)
//                    {
//                        MaxLabs.bbsMax.Entities.Post tempPost = null;
//                        foreach (MaxLabs.bbsMax.Entities.Post post in posts)
//                        {
//                            if (post.PostID == postID)
//                            {
//                                tempPost = post;
//                                break;
//                            }
//                        }
//                        if (tempPost != null)
//                        {
//                            if (tempPost.PostType == PostType.ThreadContent)
//                            {
//                                Response.Redirect(BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, ThreadID, tempPost.PostID, 1, false));
//                            }
//                            foreach (MaxLabs.bbsMax.Entities.Post post in posts)
//                            {
//                                if (post.IsApproved && post.PostType == tempPost.PostType)
//                                {
//                                    if (post.PostID == postID)
//                                    {
//                                        postIndex = totalCount;
//                                        break;
//                                    }
//                                    totalCount++;
//                                }
//                            }
//                            int pageIndex = (totalCount - 1 - postIndex) / PageSize;

//                            int agreePage = 1, againstPage = 1, neutralPage = 1;
//                            switch (tempPost.PostType)
//                            {
//                                case PostType.Polemize_Against: againstPage = pageIndex + 1; break;
//                                case PostType.Polemize_Agree: agreePage = pageIndex + 1; break;
//                                case PostType.Polemize_Neutral: neutralPage = pageIndex + 1; break;
//                                default: break;
//                            }

//                            string extParmString = "agreePage={0}&againstPage={1}&neutralPage={2}";
//                            string returnUrl = BbsUrlHelper.GetThreadUrlForExtParms(Forum.CodeName, ThreadID, 1, string.Format(extParmString, agreePage.ToString(), againstPage.ToString(), neutralPage.ToString()));
//                            Response.Redirect(returnUrl + "#" + tempPost.PostID.ToString());
//                        }
//                        else
//                            ShowError("帖子不存在");
//                    }
//                    else
//                    {
//                        MaxLabs.bbsMax.Entities.Post tempPost = null;
//                        foreach (MaxLabs.bbsMax.Entities.Post post in posts)
//                        {
//                            if (post.IsApproved)
//                            {
//                                if (post.PostID == postID)
//                                {
//                                    tempPost = post;
//                                    postIndex = totalCount;
//                                    break;
//                                }
//                                totalCount++;
//                            }
//                        }
//                        int? pageIndex = null;
//                        if (thread.ThreadType == ThreadType.Question && thread.IsClosed)
//                        {
//                            MaxLabs.bbsMax.Entities.Post bestPost;
//                            Question question = PostManager.GetQuestion(thread.ThreadID, out bestPost);
//                            if (bestPost != null)
//                            {
//                                if (bestPost.PostID == postID)
//                                    pageIndex = 1;
//                                else if (tempPost == null)
//                                {
//                                    ShowError("帖子不存在");
//                                }
//                                else if (tempPost.PostID > bestPost.PostID)
//                                    postIndex = postIndex - 1;
//                            }
//                        }
//                        if (pageIndex == null)
//                            pageIndex = postIndex / PageSize;

//                        if (ThreadSearchMode.Value == SearchMode.UserPost)
//                            Response.Redirect(BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, ThreadID, postID, pageIndex.Value + 1, false));
//                        else
//                            Response.Redirect(BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, ThreadID, postID, pageIndex.Value + 1, SearchText, false));
//                    }

//                }
//            }

//            //获取扩展参数 extParms
//            GetUrlParms();


//            ProcessAddRank();

//            //如果点下了投票按钮
//            if (IsClick("voteButton"))
//            {
//                isClickButton = true;
//                ProcessVote();
//                return;
//            }

//            //如果点下了快速回复按钮
//            if (IsClick("postButton"))
//            {
//                isClickButton = true;
//                ProcessPost();
//                return;
//            }

//            //==============================================


//            if (IsClick("btnAgree"))
//            {
//                isClickButton = true;
//                ProcessAgreePost();
//                //return;
//            }
//            if (IsClick("btnAgainst"))
//            {
//                isClickButton = true;
//                ProcessAgainstPost();
//                //return;
//            }
//            if (IsClick("btnNeutral"))
//            {
//                isClickButton = true;
//                ProcessNeutralPost();
//                //return;
//            }

//            if (IsClick("AgreePolemize"))
//            {
//                isClickButton = true;
//                ProcessPolemize(ViewPointType.Agree);
//                updateView = false;
//            }
//            if (IsClick("AgainstPolemize"))
//            {
//                isClickButton = true;
//                ProcessPolemize(ViewPointType.Against);
//                updateView = false;
//            }

//            if (IsClick("UsefulButton"))
//            {
//                isClickButton = true;
//                voteQuestionBestPost(true);
//                updateView = false;
//            }
//            if (IsClick("UnUsefulButton"))
//            {
//                isClickButton = true;
//                voteQuestionBestPost(false);
//                updateView = false;
//            }
//            if (IsClick("buyThread"))
//            {
//                isClickButton = true;
//                ProcessBuyThread();
//                updateView = false;
//            }
//            //放在所有判断点击按钮之后
//            if (!isClickButton)
//            {
//                ProcessBuyAttachment();
//            }



//            if (Thread.ThreadType == ThreadType.Move || Thread.ThreadType == ThreadType.Join)
//            {
//                Thread tempThread = PostManager.GetThread(thread.RedirectThreadID);
//                string tempCodeName = "";
//                if (thread != null)
//                {
//                    Forum tempForum = ForumManager.GetForum(tempThread.ForumID);
//                    if (tempForum != null)
//                    {
//                        tempCodeName = tempForum.CodeName;
//                    }
//                    else
//                    {
//                        ShowError("主题不存在！");
//                    }
//                }
//                else
//                {
//                    ShowError("主题不存在！");
//                }
//                string url = BbsUrlHelper.GetThreadUrl(tempCodeName, tempThread.ThreadID, (PageIndex + 1), Type);
//                Response.Redirect(url);
//            }


//            //if (GetString("ViewPointType", "get", null) != null)
//            //{
//            //    ProcessPolemize();
//            //}

//            if (IsGetPost == false && Thread.ThreadType == ThreadType.Polemize)
//            {
//                int agreePage = 1, againstPage = 1, neutralPage = 1;

//                #region 回复反序时（不要删除）
//                //回复反序时

//                if (extParms != null)//从分类或者投票等列表页进来的
//                {
//                    if (agreePageIndex != -1)
//                    {
//                        try
//                        {
//                            agreePage = int.Parse(GetExtParm("agreePage", extParms, "1"));
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                    {
//                        agreePosts = null;
//                    }
//                    if (againstPageIndex != -1)
//                    {
//                        try
//                        {
//                            againstPage = int.Parse(GetExtParm("againstPage", extParms, "1"));
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                    {
//                        againstPosts = null;
//                    }
//                    if (neutralPageIndex != -1)
//                    {
//                        try
//                        {
//                            neutralPage = int.Parse(GetExtParm("neutralPage", extParms, "1"));
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                    {
//                        neutralPosts = null;
//                    }
//                }
//                else
//                {
//                    if (agreePageIndex == -1)
//                    {
//                        agreePosts = null;
//                    }
//                    if (againstPageIndex == -1)
//                    {
//                        againstPosts = null;
//                    }
//                    if (neutralPageIndex == -1)
//                    {
//                        neutralPosts = null;
//                    }
//                }
//                #endregion
//                #region 回复正序时（不要删除）
//                /*
//            if (extParms != null)//从分类或者投票等列表页进来的
//            {
//                if (agreePageIndex == 0)
//                {
//                    try
//                    {
//                        agreePage = int.Parse(GetExtParm("agreePage", extParms, "1"));
//                    }
//                    catch
//                    {
//                    }
//                }
//                else
//                {
//                    agreePage = agreePageIndex + 1;
//                    agreePosts = null;
//                }
//                if (againstPageIndex == 0)
//                {
//                    try
//                    {
//                        againstPage = int.Parse(GetExtParm("againstPage", extParms, "1"));
//                    }
//                    catch
//                    {
//                    }
//                }
//                else
//                {
//                    againstPage = againstPageIndex + 1;
//                    againstPosts = null;
//                }
//                if (neutralPageIndex == 0)
//                {
//                    try
//                    {
//                        neutralPage = int.Parse(GetExtParm("neutralPage", extParms, "1"));
//                    }
//                    catch
//                    {
//                    }
//                }
//                else
//                {
//                    neutralPage = neutralPageIndex + 1;
//                    neutralPosts = null;
//                }
//            }
//            else
//            {
//                if (agreePageIndex > 0)
//                {
//                    agreePosts = null;
//                    agreePage = agreePageIndex + 1;
//                }
//                if (againstPageIndex > 0)
//                {
//                    againstPosts = null;
//                    againstPage = againstPageIndex + 1;
//                }
//                if (neutralPageIndex > 0)
//                {
//                    neutralPage = neutralPageIndex + 1;
//                    neutralPosts = null;
//                }
//            }
//            */
//                #endregion
//                string extParmString = "agreePage={0}&againstPage={1}&neutralPage={2}";
//                SetPager("AgreePostListPager", BbsUrlHelper.GetThreadUrlForExtParms(CodeName, ThreadID, PageIndex + 1, string.Format(extParmString, "{0}", againstPage.ToString(), neutralPage.ToString())), agreePage - 1, PageSize, AgreePostCount);
//                SetPager("AgainstPostListPager", BbsUrlHelper.GetThreadUrlForExtParms(CodeName, ThreadID, PageIndex + 1, string.Format(extParmString, agreePage.ToString(), "{0}", neutralPage.ToString())), againstPage - 1, PageSize, AgainstPostCount);
//                SetPager("NeutralPostListPager", BbsUrlHelper.GetThreadUrlForExtParms(CodeName, ThreadID, PageIndex + 1, string.Format(extParmString, agreePage.ToString(), againstPage.ToString(), "{0}")), neutralPage - 1, PageSize, NeutralPostCount);
//            }


//            /* TODO: bbsMax 3.0 屏蔽IP
//            #region  禁止IP操作
//            if (ManagePermission.AllowManageIPs)
//            {
//                if (IsClick("AddLimitIP"))
//                {
//                    AddLimitIP();
//                }
//                else if (IsClick("DelLimitIP"))
//                {
//                    DelLimitIP();
//                }
//            }
//            if (CommonPermission.AllowShowUserSourceTrack && Request.QueryString["action"] == "GetAdress" && !string.IsNullOrEmpty(Request.QueryString["postID"]))
//            {
//                this.CurrentPostID = GetInt32("postID", "get", 0);
//                return;
//            }
//            #endregion
//            */
//            //========================================================================================



//            if (ForumID < 0)
//            {
//                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
//                    AddNavigationItem("[回收站]", BbsUrlHelper.GetSystemForumUrl(SystemForum.RecycleBin));
//                else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
//                    AddNavigationItem("[未审核的主题]", BbsUrlHelper.GetSystemForumUrl(SystemForum.UnapproveThreads));
//                else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
//                    AddNavigationItem("[未审核的回复]", BbsUrlHelper.GetSystemForumUrl(SystemForum.UnapprovePosts));

//                Forum = ForumManager.GetForum(Thread.ForumID);
//                ForumID = Forum.ForumID;
//                CodeName = Forum.CodeName;
//                //Permission = Forum.Permission;
//            }
//            else
//            {
//                //如果主题所在版块不正确(如移动主题后用户从收藏夹进入),则处理
//                if (ForumID != Thread.ForumID)
//                {
//                    //如果是正常查看帖子，则可以直接跳转
//                    if (string.IsNullOrEmpty(Type))
//                    {
//                        CodeName = ForumManager.GetForum(Thread.ForumID).CodeName;
//                        Response.Redirect(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PageIndex + 1));
//                    }
//                    else
//                        ShowError(GetThreadStatus.NotExists);
//                }



//                if (Forum.ParentID > 0)
//                {
//                    Forum parentForum;
//                    Forum currentForum = Forum;
//                    do
//                    {
//                        parentForum = ForumManager.GetForum(currentForum.ParentID);
//                        if (parentForum == null)
//                            break;
//                        if (parentForum.ForumType != ForumType.Catalog)
//                        {
//                            if (!DisplaySiteNameInNavigation)
//                            {
//                                InsertNavigationItem(1, parentForum.ForumName, BbsUrlHelper.GetForumUrl(parentForum.CodeName));
//                            }
//                            else
//                            {
//                                InsertNavigationItem(2, parentForum.ForumName, BbsUrlHelper.GetForumUrl(parentForum.CodeName));
//                            }
//                        }
//                        currentForum = parentForum;
//                    }
//                    while (parentForum.ParentID > 0);
//                }




//                if (Type == "")
//                {
//                    if (threadCatalogID == -1)
//                    {
//                        AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(Forum.CodeName, action, tempPage));
//                    }
//                    else
//                        AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, threadCatalogID, tempPage));
//                }
//                else
//                    AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(Forum.CodeName));


//                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
//                    AddNavigationItem("[回收站]", BbsUrlHelper.GetSystemForumUrl(SystemForum.RecycleBin, CodeName));
//                else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
//                    AddNavigationItem("[未审核的主题]", BbsUrlHelper.GetSystemForumUrl(SystemForum.UnapproveThreads, CodeName));
//                else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
//                    AddNavigationItem("[未审核的回复]", BbsUrlHelper.GetSystemForumUrl(SystemForum.UnapprovePosts, CodeName));

//            }

//            AddNavigationItem(Thread.Subject);

//            SetPageTitle(Forum.ForumName);
//            SetPageTitle(Thread.Subject);


//            DisabledItem();


//            SetAllForumsList("AllForumsList", Forum);


//            OnlineManager.UpdateOnlineUser(MyUserID, Forum.ForumID, ThreadID, My.OnlineStatus, OnlineAction.ViewThread, Request, Response);


//        }

//        private string forumUrl;
//        protected string ForumUrl
//        {
//            get
//            {
//                if (forumUrl == null)
//                {
//                    if (Type == "")
//                    {
//                        if (threadCatalogID == -1)
//                        {
//                            forumUrl = BbsUrlHelper.GetForumUrl(Forum.CodeName, action, tempPage);
//                        }
//                        else
//                            forumUrl = BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, threadCatalogID, tempPage);
//                    }
//                    else
//                        forumUrl = BbsUrlHelper.GetForumUrl(Forum.CodeName);
//                }
//                return forumUrl;
//            }
//        }
//        protected new string MetaDescription
//        {
//            get
//            {
//                if (Replies.Count > 0)
//                {
//                    MaxLabs.bbsMax.Entities.Post post = Replies[0];
//                    if (!Forum.IsShieldedUser(post.UserID) && !post.IsShielded && IsShowContent(post))
//                    {
//                        return Bbs3Globals.HtmlEncode(Bbs3Globals.CutString(ClearHTML(post.FormattedContent), 200));
//                    }
//                    else
//                        return Bbs3Globals.HtmlEncode(ClearHTML(Thread.Subject));
//                }
//                else
//                    return Bbs3Globals.HtmlEncode(ClearHTML(Thread.Subject));

//            }
//        }



//        private void ProcessAddRank()
//        {
//            int rank;
//            if (IsClick("rankButton1"))
//            {
//                isClickButton = true;
//                rank = 1;
//            }
//            else if (IsClick("rankButton2"))
//            {
//                isClickButton = true;
//                rank = 2;
//            }
//            else if (IsClick("rankButton3"))
//            {
//                isClickButton = true;
//                rank = 3;
//            }
//            else if (IsClick("rankButton4"))
//            {
//                isClickButton = true;
//                rank = 4;
//            }
//            else if (IsClick("rankButton5"))
//            {
//                isClickButton = true;
//                rank = 5;
//            }
//            else
//            {
//                return;
//            }
//            updateView = false;
//            //int threadID = GetInt32("ThreadID", "get", 0);
//            //if (ThreadID == 0)
//            //{
//            //    ShowError("获取主题出错。");
//            //}
//            //string codeName = GetString("ForumCodeName");

//            //Thread t = PostManager.GetThread(threadID);
//            //if (t == null)
//            //{
//            //    ShowError("获取主题出错。");
//            //}

//            if (!IsGuest)
//            {
//                if (Thread.PostUserID == MyUserID)
//                {
//                    ShowAlert("不能为自己的主题评级！");
//                    return;
//                }
//            }
//            //int rank = GetInt32("Rank", "post");
//            //int pageIndex = GetInt32("pageIndex", "get");
//            //int pageSize = GetInt32("pageSize", "get");

//            ThreadRank threadRank = new ThreadRank();
//            threadRank.Rank = rank;
//            threadRank.ThreadID = Thread.ThreadID;
//            threadRank.UserID = MyUserID;

//            CreateUpdateThreadRankStatus status = PostManager.CreateOrUpdateThreadRank(threadRank);
//            if (status == CreateUpdateThreadRankStatus.Success)
//            {
//                if (thread != null)
//                    thread.Rank = rank;
//                ShowInformation<CreateUpdateThreadRankStatus>(status);
//                //string returnUrl = BbsUrlHelper.GetThreadUrl(CodeName, Thread.ThreadID, 1);// +"|" + BbsUrlHelper.GetForumUrl(CodeName);
//                //GoUrl(returnUrl);
//                //ShowInformation<CreateUpdateThreadRankStatus>(status, returnUrl);
//            }
//            else
//                ShowError<CreateUpdateThreadRankStatus>(status);
//        }
//        protected string OtherAction
//        {
//            get
//            {
//                return GetString("otherAction", "get", "").ToLower();
//            }
//        }
//        private void ProcessBuyAttachment()
//        {
//            if (OtherAction != "buyattachment")
//            {
//                return;
//            }
//            updateView = false;
//            int userID = MyUserID;
//            if (userID <= 0)
//            {
//                ShowError("您还没有登陆，不能购买！");
//            }
//            else
//            {
//                int tempAttachmentID = GetInt32("AttachmentID", "get", 0);

//                Attachment tempAttachment = null;
//                MaxLabs.bbsMax.FileSystem.PhysicalFile phyFile = null;
//                //zzbird.Common.Disk.DiskFile diskFile = null;
//                if (tempAttachmentID <= 0)
//                {
//                    ShowError("AttachmentID错误!");
//                }
//                else
//                {

//                    tempAttachment = PostManager.GetAttachment(tempAttachmentID);

//                    if (tempAttachment == null)
//                    {
//                        ShowError("该附件不存在,可能被移动或被删除！");
//                    }
//                    //diskFile = zzbird.Common.Disk.DiskManager.GetDiskFile(tempAttachment.DiskFileID);
//                    phyFile = FileManager.GetFile(tempAttachment.FileID);
//                }
//                if (phyFile == null)
//                {
//                    ShowError("该附件不存在,可能被移动或被删除！");
//                }

//                if (userID == tempAttachment.UserID || tempAttachment.Price == 0)
//                {
//                    return;
//                }
//                if (!tempAttachment.IsBuy(userID))//没购买过
//                {
//                    int trade = Math.Abs(tempAttachment.Price);

//                    UserPoint tradePoint = ForumPointAction.Instance.GetUserPoint(tempAttachment.UserID, ForumPointValueType.SellAttachment, Forum.ForumID);

//                    //ExtendedPoint tradePoint = TradePoint;
//                    if (tradePoint == null)
//                    {
//                        ShowError("系统交易积分错误！");
//                    }

//                    bool success = UserBO.Instance.TradePoint(userID, tempAttachment.UserID, tempAttachment.Price, tradePoint.Type, false, true, null);
//                    if (success == false)
//                    {
//                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
//                        {
//                            ShowError(error.Message);
//                        });
//                    }

//                    //CreateUpdateExtendedPointStatus status = PointManager.TradePoint(
//                    //    userID, UserManager.GetUserProfile(tempAttachment.UserID).NickName, tradePoint.PointID,
//                    //    tempAttachment.Price, false, false, false);//, tradePoint.MinValue - 1);
//                    //if (status != CreateUpdateExtendedPointStatus.Success)
//                    //{
//                    //    ShowError(status);
//                    //}
//                    else
//                    {
//                        //创建交易记录
//                        if (!PostManager.CreateAttachmentExchange(tempAttachment.AttachmentID, userID, tempAttachment.Price))
//                        {
//                            ShowError("购买成功，创建交易记录失败！");
//                        }
//                        else
//                        {
//                            //下载
//                            //更新userprofile
//                            if (My.BuyedAttachments.ContainsKey(tempAttachment.AttachmentID))
//                            {
//                                My.BuyedAttachments[tempAttachment.AttachmentID] = true;
//                            }
//                            //RunJavaScript(this.Page, "alert('购买成功！现在可以下载！')");
//                            //////string returnUrl = string.Empty;
//                            //////if (Request.UrlReferrer != null)
//                            //////{
//                            //////    returnUrl = Request.ServerVariables["Http_REFERER"];
//                            //////}
//                            ////////----性能差，帅帅
//                            //////MaxLabs.bbsMax.Entities.Post p = PostManager.GetPost(tempAttachment.PostID);
//                            ////////Thread t;
//                            //////Forum f;
//                            //////if (p != null)
//                            //////{
//                            //////    f = ForumManager.GetForum(PostManager.GetForumID(p.ThreadID));
//                            //////    if (f != null)
//                            //////    {
//                            //////        returnUrl = BbsUrlHelper.GetThreadUrl(
//                            //////            f.CodeName, p.ThreadID, 1);
//                            //////    }
//                            //////}

//                            Thread.FirstPost = null;
//                            ShowInformation("购买成功,现在可以查看、下载或收藏！", false);

//                        }
//                    }
//                }
//                else
//                {
//                    ShowError("您已经购买此附件，无需再次购买!", string.Empty);
//                }
//            }
//        }

//        protected int SubjectMinLength
//        {
//            get
//            {
//                return ForumSetting.PostSubjectLengths.GetValue(MyUserID).MinValue;
//            }
//        }

//        protected int SubjectMaxLength
//        {
//            get
//            {
//                return ForumSetting.PostSubjectLengths.GetValue(MyUserID).MaxValue;
//            }
//        }


//        protected int ContentMinLength
//        {
//            get
//            {
//                return ForumSetting.PostContentLengths.GetValue(MyUserID).MinValue;
//            }
//        }

//        protected int ContentMaxLength
//        {
//            get
//            {
//                return ForumSetting.PostContentLengths.GetValue(MyUserID).MaxValue;
//            }
//        }

//        //private ExtendedPoint tradePoint;
//        //protected ExtendedPoint TradePoint
//        //{
//        //    get
//        //    {
//        //        if (tradePoint == null)
//        //            tradePoint = PointManager.GetTradePoint();
//        //        return tradePoint;
//        //    }
//        //}
//        //protected string TradeName
//        //{
//        //    get
//        //    {
//        //        if (TradePoint != null)
//        //            return TradePoint.PointName;
//        //        return "";
//        //    }
//        //}
//        //protected string TradeUnit
//        //{
//        //    get
//        //    {
//        //        if (TradePoint != null)
//        //            return TradePoint.PointUnit;
//        //        return "";
//        //    }
//        //}

//        private User threadUserProfile;
//        protected User ThreadUserProfile
//        {
//            get
//            {
//                if (threadUserProfile == null)
//                    threadUserProfile = UserBO.Instance.GetUser(Thread.PostUserID);
//                return threadUserProfile;
//            }
//        }

//        protected string GetUserPostsLink(int UserID)
//        {
//            if (string.IsNullOrEmpty(Type))
//                return "<a href=\"" + BbsUrlHelper.GetThreadUrl(Forum.CodeName, ThreadID, 1, UserID, 1) + "\">只看该用户</a>";
//            else
//                return string.Empty;

//        }
//        protected bool PostEnableReplyNotice
//        {
//            get
//            {
//                return Can(ForumPermissionSetNode.Action.PostEnableReplyNotice);
//            }
//        }

//        protected bool ShowSignatureInPost
//        {
//            get
//            {
//                return ForumSetting.ShowSignatureInPost.GetValue(MyUserID);
//            }
//        }

//        private void DisabledItem()
//        {
//            string checkedString = "checked=\"checked\"", disabledString = "disabled=\"disabled\"";

//            if (AllowEmoticon)
//                disabledEmoticon = checkedString;
//            else
//                disabledEmoticon = disabledString;

//            if (AllowHtml)
//                disabledHtml = checkedString;
//            else
//                disabledHtml = disabledString;

//            if (AllowMaxcode)
//                disabledMaxCode = checkedString;
//            else
//                disabledMaxCode = disabledString;

//            if (Can(ForumPermissionSetNode.Action.PostEnableReplyNotice))
//                disabledReplyNotice = checkedString;
//            else
//                disabledReplyNotice = disabledString;

//            if (ForumSetting.ShowSignatureInPost.GetValue(MyUserID))
//                disabledSignature = checkedString;
//            else
//                disabledSignature = disabledString;
//        }

//        private void ProcessBuyThread()
//        {
//            if (IsGuest)
//                ShowError("您还没有登陆，不能购买！");
//            else if (Thread.Price < 1)
//                ShowAlert("该主题不需要购买");
//            else if (Thread.PostUserID == MyUserID)
//                ShowAlert("自己的主题不需要购买");
//            else if (Thread.IsBuyed)
//                ShowAlert("您已经购买过该主题");
//            else
//            {
//                UserPoint tradePoint = ForumPointAction.Instance.GetUserPoint(Thread.PostUserID, ForumPointValueType.SellThread, Forum.ForumID);
//                if (tradePoint == null)
//                {
//                    ShowError("系统没有设置交易积分！");
//                }

//                bool success = UserBO.Instance.TradePoint(MyUserID, Thread.PostUserID, Thread.Price, tradePoint.Type, false, true, null);
//                if (success == false)
//                {
//                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
//                    {
//                        ShowError(error.Message);
//                    });
//                }

//                //CreateUpdateExtendedPointStatus status = PointManager.TradePoint(
//                //    UserProfile.UserID, UserManager.GetUserProfile(Thread.PostUserID).NickName, tradePoint.PointID,
//                //    Thread.Price, false, false, false);//, tradePoint.MinValue - 1);
//                //if (status != CreateUpdateExtendedPointStatus.Success)
//                //{
//                //    ShowError(status);
//                //}
//                else
//                {
//                    success = PostManager.CreateThreadExchange(Thread.ThreadID, MyUserID, Thread.Price);
//                    if (success)
//                    {
//                        if (My.BuyedThreadIDs.ContainsKey(Thread.ThreadID))
//                        {
//                            My.BuyedThreadIDs[Thread.ThreadID] = true;
//                        }
//                        //Thread.FirstPost = null;
//                        ShowInformation("购买成功！", false);
//                    }
//                    else
//                    {
//                        ShowError("创建交易记录失败！");
//                    }
//                }
//            }
//        }

//        private bool GetVoteErrorMessage = false;
//        private string m_VoteErrorMessage;
//        protected string VoteErrorMessage
//        {
//            get
//            {
//                if (GetVoteErrorMessage == false)
//                {
//                    GetVoteErrorMessage = true;
//                    string name = GetValidateCodeInputName("Vote");
//                    m_VoteErrorMessage = GetErrorMessage(name);
//                    if (m_VoteErrorMessage == null)
//                        m_VoteErrorMessage = GetErrorMessage("Vote");
//                }
//                return m_VoteErrorMessage;
//            }
//        }

//        private void ProcessVote()
//        {
//            if (false == CheckValidateCode("Vote", GetString("ReturnUrl")))
//            {
//                return;
//            }
//            MessageDisplay msgDisplay = CreateMessageDisplay("vote");
//            List<int> itemIDs = GetIdList("pollItem", "post");
//            VoteStatus status = PostManager.Vote(itemIDs, ThreadID, MyUserID, My.Username);
//            if (status == VoteStatus.Success)
//            {
//                ValidateCodeManager.CreateValidateCodeActionRecode("Vote");
//                //ClearValidateSession("vote");
//                string returnUrl = BbsUrlHelper.GetThreadUrl(Forum.CodeName, ThreadID, 1); //+ "|" + BbsUrlHelper.GetForumUrl(Forum.CodeName);

//                ShowInformation(status);
//            }
//            else
//            {
//                msgDisplay.AddError("vote", Bbs3Language.GetText<VoteStatus>(status));
//                //ShowError(status);
//            }
//        }

//        private void ProcessPost()
//        {
//            if (false == CheckValidateCode("ReplyTopic", GetString("ReturnUrl")))
//            {
//                return;
//            }
//            /* 
//            if (EnableQuickReplyValidateCode)
//            {
//                ValidateCodeCheck(GetString("validateCodePost", "post", string.Empty), "reply", GetString("ReturnUrl"));
//            }
//            */

//            MessageDisplay msgDisplay = CreateMessageDisplay();

//            MaxLabs.bbsMax.Entities.Post post = new MaxLabs.bbsMax.Entities.Post();
//            post.UserID = MyUserID;
//            if (IsGuest)
//            {
//                if (EnableGuestNickName)
//                    post.NickName = GetString("guestNickName", "post", "");
//                else
//                    post.NickName = "";
//            }
//            else
//            {
//                post.NickName = My.Username;
//            }
//            post.TempSubject = CommonUtil.HtmlEncode(GetString("Subject"));
//            post.TempContent = GetString("Editor", "post", string.Empty);
//            post.IPAddress = RequestUtil.GetIpAddress(Request);

//            //string formatMode = GetString("formatmode", "post").ToLower();
//            string enableItems = _Request.Get("enableItem", Method.Post, string.Empty).ToLower();

//            //post.EnableHTML = (enableItems.IndexOf("enablehtml") > -1 && AllowHtml);
//            //post.EnableMaxCode3 = (enableItems.IndexOf("enablemaxcode") > -1 && AllowMaxcode);

//            if (AllowHtml && AllowMaxcode)
//            {
//                post.EnableHTML = _Request.Get("contentFormat", Method.Post, "").ToLower() == "enablehtml";
//                if (post.EnableHTML == false)
//                    post.EnableMaxCode3 = true;
//            }
//            else if (AllowHtml)
//                post.EnableHTML = true;
//            else if (AllowMaxcode)
//                post.EnableMaxCode3 = true;

//            post.EnableEmoticons = (enableItems.IndexOf("enableemoticons") > -1);
//            post.EnableSignature = (enableItems.IndexOf("enablesignature") > -1);
//            post.EnableReplyNotice = (enableItems.IndexOf("enablereplynotice") > -1);

//            post.AllAttachments = new AttachmentCollection();
//            post.ThreadID = ThreadID;
//            int postID;


//            CreateUpdatePostStatus status = PostManager.ReplyThread(post, out postID);
//            if (status == CreateUpdatePostStatus.Success)
//            {
//                ValidateCodeManager.CreateValidateCodeActionRecode("ReplyTopic");
//                //ClearValidateSession("createpost");
//                if (IsAjax)
//                {
//                    ShowInformation<CreateUpdatePostStatus>(status);
//                    PageIndex = PostManager.GetThread(ThreadID).TotalPages - 1;
//                }
//                else
//                {
//                    string returnUrl = BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, ThreadID, postID, PostManager.GetThread(ThreadID).TotalPages, true);// +"|" + BbsUrlHelper.GetForumUrl(Forum.CodeName);
//                    GoUrl(returnUrl);
//                }
//            }
//            else if (status == CreateUpdatePostStatus.SuccessButUnapproved)
//            {
//                //ClearValidateSession("createpost");
//                ShowAlert<CreateUpdatePostStatus>(status);
//            }
//            else
//            {
//                CatchError<ErrorInfo>(delegate(ErrorInfo error)
//                {
//                    msgDisplay.AddError(error);
//                });

//                if (msgDisplay.HasAnyError() == false)
//                    msgDisplay.AddError(Bbs3Language.GetText<CreateUpdatePostStatus>(status));
//                //ShowError<CreateUpdatePostStatus>(status);
//            }
//        }

//        protected string GetReward(int postID)
//        {
//            int reward;
//            Question.Rewards.TryGetValue(postID, out reward);
//            return reward.ToString();
//        }

//        protected string GetVoteType(Poll poll)
//        {
//            if (poll.Multiple < 2)
//                return "单选";
//            else
//            {
//                int count;
//                if (poll.PollItems.Count > poll.Multiple)
//                    count = poll.Multiple;
//                else
//                    count = poll.PollItems.Count;
//                return "多选，最多只能选" + count + "项";
//            }
//        }

//        protected string GetVoteImageWidthPercent(PollItem pollItem, int voteTotalCount, int widthPercent)
//        {
//            voteTotalCount = voteTotalCount == 0 ? 1 : voteTotalCount;
//            return (((float)pollItem.PollItemCount / (float)voteTotalCount) * widthPercent).ToString();
//        }
//        //protected string GetVotePercent(PollItem pollItem, int voteTotalCount)
//        //{
//        //    voteTotalCount = voteTotalCount == 0 ? 1 : voteTotalCount;
//        //    string votePercentString = Math.Round((((float)pollItem.PollItemCount / (float)voteTotalCount) * 100),2).ToString();
//        //    int dotIndex = votePercentString.IndexOf('.');

//        //    if (dotIndex == -1)
//        //        votePercentString = votePercentString + ".00";
//        //    else
//        //    {
//        //        int lenth = votePercentString.Length - dotIndex;
//        //        if (lenth == 1)
//        //            votePercentString += "0";
//        //        else if (lenth > 2)
//        //            votePercentString = votePercentString.Substring(0, dotIndex + 3);
//        //    }

//        //    return votePercentString;
//        //}

//        //protected string GetActionButtons(string outputFormat)
//        //{
//        //    System.Text.StringBuilder sb=new System.Text.StringBuilder();
//        //    sb.Append("<input Type=\"hidden\" name=\"ThreadID\" value=\"" + ThreadID + "\" />");

//        //    string cbAll = "<input Type=\"checkbox\" name=\"checkAllBox\" id=\"checkAllBox\" onclick=\"checkAll('postID','checkAllBox')\" /><label for=\"checkAllBox\">全选</label>";
//        //    ForumPermission Permission = Permission;

//        //    if (Type == "" && Forum.AllowManageOtherUser(thread.PostUserID))
//        //    {
//        //        sb.Append(base.GetActionButtons(Forum, outputFormat));
//        //        if (Forum.ForumID > 0)
//        //        {
//        //            if (Forum.HaveModeratorPermission && Permission.JoinThreads && thread.ThreadType != ThreadType.Poll)
//        //                sb.Append(string.Format(outputFormat, "合并主题", GetClickString("join", Forum)) + " ");
//        //            if (Forum.HaveModeratorPermission && Permission.SplitThread && thread.ThreadType != ThreadType.Poll && thread.TotalReplies > 0)
//        //                sb.Append(string.Format(outputFormat, "分割主题", GetClickString("split", Forum)) + " ");
//        //        }
//        //    }
//        //    bool addCBAll=false;
//        //    if (Forum.ForumID > 0 && Forum.HaveModeratorPermission && Permission.DeletePosts)//&& thread.TotalReplies > 0)
//        //    {
//        //        addCBAll=true;
//        //        sb.Append(string.Format(outputFormat, "删除回复", GetDelPostClickString("delpost", Forum)) + " ");
//        //    }
//        //    if (Type == "unapprovepost" &&((Forum.ForumID>0&&Permission.ApprovePosts&&Forum.HaveModeratorPermission)||(Forum.ForumID==(int)SystemForum.UnapproveThreads&&Forum.IsModerator)))
//        //    {
//        //        addCBAll=true;
//        //        sb.Append(string.Format(outputFormat, "审核回复", GetDelPostClickString("approvepost", Forum)) + " ");
//        //    }
//        //    else if ((Type == "unapprovethread" || Type == "unapprovebin") && ((Forum.ForumID>0&&Permission.ApproveThreads&&Forum.HaveModeratorPermission)||(Forum.ForumID==(int)SystemForum.UnapproveThreads&&Forum.IsModerator)))
//        //    {
//        //        sb.AppendFormat( outputFormat, "审核主题", GetClickString("approvethread", Forum) + " ");
//        //        sb.AppendFormat( outputFormat, "清除主题", GetClickString("delthread", Forum) + " ");
//        //    }
//        //    if (Type == "recyclebin" &&((Forum.ForumID>0&&Forum.HaveModeratorPermission && Permission.DeleteThreads)||(Forum.ForumID==(int)SystemForum.RecycleBin&&Forum.IsModerator)))
//        //        sb.Append( string.Format(outputFormat, "删除主题", GetClickString("delthread", Forum)) + " ");
//        //    if(addCBAll)
//        //        return cbAll+" "+sb.ToString();
//        //    else
//        //        return sb.ToString();
//        //}


//        //private string GetDelPostClickString(string action, Forum Forum)
//        //{
//        //    if (Type != "")
//        //        return "postDelPostAction('" + BbsUrlHelper.GetThreadManagerUrl(Forum.CodeName, action) + "&Type=" + Type + "&ForumID=" + GetInt32("ForumID", "get") + "')";
//        //    else
//        //        return "postDelPostAction('" + BbsUrlHelper.GetThreadManagerUrl(Forum.CodeName, action) + "')";
//        //}


//        protected string GetThreadActionList(MaxLabs.bbsMax.Entities.Post reply, string linkStyle, string separator)
//        {
//            if (string.IsNullOrEmpty(linkStyle))
//                linkStyle = "<a href=\"{0}\" {2}>{1}</a>";

//            if (separator == null)
//                separator = " ";

//            bool isShowEdit = true;
//            System.Text.StringBuilder sb = new System.Text.StringBuilder();

//            //使用道具
//            sb.Append(string.Format(linkStyle, "地址", "使用道具", " onclick=\"return openDialog(this.href, refresh)\"") + separator);

//            //如果当前用户拥有屏蔽用户的权限，显示屏蔽用户的链接
//            if (reply.UserID != 0)
//            {
//                //if (Permission.SetUserShield && Forum.HaveModeratorPermission && Forum.AllowManageOtherUser(reply.Poster.UserID))
//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.BanUser, reply.Poster.UserID))
//                {
//                    if (Forum.IsShieldedUser(reply.UserID))
//                    {
//                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldUserUrl(reply.UserID), "解除屏蔽用户", " onclick=\"return openDialog(this.href, refresh)\"") + separator);
//                        isShowEdit = false;
//                    }
//                    else
//                    {
//                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldUserUrl(reply.UserID), "屏蔽用户", " onclick=\"return openDialog(this.href, refresh)\"") + separator);
//                    }
//                }
//            }
//            //如果当前用户拥有屏蔽回复的权限，显示屏蔽回复的链接
//            //if (Permission.SetPostShield && Forum.HaveModeratorPermission && Forum.AllowManageOtherUser(reply.Poster.UserID))
//            if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetPostShield, reply.UserID))
//            {
//                if (reply.IsShielded)
//                {
//                    sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldPostUrl(Forum.CodeName, "RescindShieldPost", reply.ThreadID, reply.PostID, PageIndex), "解除屏蔽帖子", string.Empty) + separator);
//                    //sb.Append("<a href=\"" + BbsUrlHelper.GetActionShieldPostUrl(Forum.CodeName, "RescindShieldPost", reply.ThreadID, reply.PostID, PageIndex) + "\">解除屏蔽帖子</a> ");
//                    isShowEdit = false;
//                }
//                else
//                {
//                    sb.Append(string.Format(linkStyle, BbsUrlHelper.GetActionShieldPostUrl(Forum.CodeName, "ShieldPost", reply.ThreadID, reply.PostID, PageIndex), "屏蔽帖子", string.Empty) + separator);
//                    //sb.Append("<a href=\"" + BbsUrlHelper.GetActionShieldPostUrl(Forum.CodeName, "ShieldPost", reply.ThreadID, reply.PostID, PageIndex) + "\">屏蔽帖子</a> ");
//                }
//            }

//            if (isShowEdit)
//                appendEditLink(sb, reply, Forum, linkStyle, separator);

//            if (string.IsNullOrEmpty(Type))
//            {


//                appendFinalQuestionLink(sb, reply, Forum, linkStyle, separator);

//                appendDeleteLink(sb, reply, Forum, linkStyle, separator);


//            }
//            if (sb.Length > separator.Length)
//                return sb.ToString(0, sb.Length - separator.Length);
//            else
//                return sb.ToString();

//            //return sb.ToString();
//        }

//        /// <summary>
//        /// 获取回复连接
//        /// </summary>
//        /// <param name="reply"></param>
//        /// <param name="style"></param>
//        /// <returns></returns>
//        protected string GetReplyLink(MaxLabs.bbsMax.Entities.Post reply, string style)
//        {

//            if (string.IsNullOrEmpty(Type))
//            {
//                if (!Thread.IsLocked)
//                {
//                    if (Can(ForumPermissionSetNode.Action.ReplyThread))
//                        return string.Format(style, BbsUrlHelper.GetReplyPostUrl(Forum.CodeName, Thread.ThreadID, Server.UrlEncode("re:" + reply.PostIndexAlias + " " + reply.NickName), reply.PostID));
//                }
//            }
//            return string.Empty;
//        }
//        protected string GetQuoteLink(MaxLabs.bbsMax.Entities.Post reply, string style)
//        {
//            if (string.IsNullOrEmpty(Type))
//            {
//                if (!Thread.IsLocked)
//                {
//                    if (Can(ForumPermissionSetNode.Action.ReplyThread))
//                    {
//                        string url;
//                        if (Thread == null || Thread.ThreadID < 1)
//                            url = BbsUrlHelper.GetCreatPostUrl(string.Empty, "ReQuote", 0, reply.PostID, Type);
//                        else
//                            url = BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "ReQuote", Thread.ThreadID, reply.PostID, Type);

//                        return string.Format(style, url);
//                    }
//                }
//            }
//            return string.Empty;
//        }
//        protected string GetMarkLink(MaxLabs.bbsMax.Entities.Post reply, string style)
//        {
//            if (string.IsNullOrEmpty(Type) && reply.UserID != MyUserID)
//            {
//                //    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.Marking, reply.UserID) && reply.UserID != MyUserID)
//                //    {
//                return string.Format(style, BbsUrlHelper.GetPostMarkUrl(reply.PostID));
//                //    }
//            }
//            return string.Empty;
//        }

//        protected string GetCancelMarkLink(MaxLabs.bbsMax.Entities.Post reply, string style)
//        {
//            if (string.IsNullOrEmpty(Type))
//            {
//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.CancelRate, reply.UserID))
//                {
//                    if (reply.PostMarks.Count > 0)
//                    {
//                        return string.Format(style, BbsUrlHelper.GetCancelPostMarkUrl(reply.PostID));
//                    }
//                }
//            }
//            return string.Empty;
//        }


//        protected string GetCheckBox(MaxLabs.bbsMax.Entities.Post reply, string checkBoxStyle)
//        {
//            if (string.IsNullOrEmpty(checkBoxStyle))
//                checkBoxStyle = "<input Type=\"checkbox\" name=\"postID\" id=\"postID_{0}\" value=\"{0}\" />";
//            //if (Forum.HaveModeratorPermission && IsShowCheckBox && Forum.AllowManageOtherUser(reply.Poster.UserID))
//            if (IsShowCheckBox)
//            {
//                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) != 0)
//                {
//                    if (reply.PostIndex > 0 || (/*string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0 || */string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0))
//                        return string.Format(checkBoxStyle, reply.PostID.ToString());
//                    else
//                    {
//                        if (ThreadType == ThreadType.Question)
//                        {
//                            if (BestPost != null)
//                            {
//                                if (reply.PostID == BestPost.PostID)
//                                {
//                                    return string.Format(checkBoxStyle, reply.PostID.ToString());
//                                }
//                            }
//                        }
//                    }
//                    //sb.Append("<input Type=\"checkbox\" name=\"postID\" id=\"postID_" + reply.PostID + "\" value=\"" + reply.PostID + "\" /> ");
//                }
//            }
//            return "";
//        }

//        private void appendEditLink(StringBuilder sb, MaxLabs.bbsMax.Entities.Post reply, Forum Forum, string linkStyle, string separator)
//        {
//            if ((string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0) || (reply.PostIndex != 0 && ((reply.UserID == MyUserID && Can(ForumPermissionSetNode.Action.UpdateOwnPost)) || (CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, reply.UserID)))))
//            {
//                sb.Append(string.Format(linkStyle, BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editpost", reply.ThreadID, reply.PostID, Type), "编辑", string.Empty) + separator);
//                //sb.Append("<a href=\"");
//                //sb.Append(BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editpost", reply.ThreadID, reply.PostID,Type));
//                //sb.Append("\">编辑</a> ");
//            }
//            else if (reply.PostIndex == 0 && ((reply.UserID == MyUserID && Can(ForumPermissionSetNode.Action.UpdateOwnPost)) || (CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, reply.UserID))))
//            {
//                sb.Append(string.Format(linkStyle, BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editthread", reply.ThreadID, reply.PostID, Type), "编辑", string.Empty) + separator);
//                //sb.Append("<a href=\"");
//                //sb.Append(BbsUrlHelper.GetCreatPostUrl(Forum.CodeName, "editthread", reply.ThreadID, reply.PostID,Type));
//                //sb.Append("\">编辑</a> ");
//            }
//        }

//        private void appendDeleteLink(StringBuilder sb, MaxLabs.bbsMax.Entities.Post reply, Forum Forum, string linkStyle, string separator)
//        {
//            if (reply.PostIndex != 0)
//            {
//                if (reply.UserID == MyUserID)
//                {
//                    if (Can(ForumPermissionSetNode.Action.DeleteOwnPosts))
//                        sb.Append(string.Format(linkStyle, "javascript:Widget.Confirm('确认删除','确认要删除您自己的回复吗？',function(){location.href='" + BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeletePostSelf.ToString(), reply.ThreadID, reply.PostID) + "'});", "删除", string.Empty) + separator);
//                    //sb.Append("<a href=\"###\" onclick=\"if(confirm('确认要删除您自己的回复吗？')){location.href ='");
//                    //sb.Append(BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeletePostSelf.ToString(), reply.ThreadID,reply.PostID));
//                    //sb.Append("';}else{return false;} \">删除</a> ");
//                }
//                else
//                {
//                    //if (Forum.HaveModeratorPermission && Permission.DeletePosts && Forum.AllowManageOtherUser(reply.UserID))
//                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, reply.UserID))
//                    {
//                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeletePost.ToString(), reply.ThreadID, reply.PostID), "删除", string.Empty) + separator);
//                    }
//                }
//            }
//            else
//            {
//                if (reply.UserID == MyUserID)
//                {
//                    if (Can(ForumPermissionSetNode.Action.DeleteOwnPosts))
//                        sb.Append(string.Format(linkStyle, "javascript:Widget.Confirm('确认删除','确认要删除您自己的主题吗？',function(){location.href='" + BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeleteOwnThread.ToString(), reply.ThreadID) + "'});", "删除", string.Empty) + separator);
//                    //sb.Append("<a href=\"###\" onclick=\"if(confirm('确认要删除您自己的主题吗？')){location.href ='");
//                    //sb.Append( BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeleteOwnThread.ToString(), reply.ThreadID));
//                    //sb.Append("';}else{return false;} \">删除</a> ");
//                }
//                else
//                {
//                    //if (Forum.HaveModeratorPermission && Permission.DeleteThreads && Forum.AllowManageOtherUser(reply.UserID))
//                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, reply.UserID))
//                    {
//                        sb.Append(string.Format(linkStyle, BbsUrlHelper.GetDeletePostUrl(Forum.CodeName, ThreadManageAction.DeleteThread.ToString(), reply.ThreadID), "删除", string.Empty) + separator);
//                    }
//                }
//            }
//        }

//        protected bool isShowFinalQuestionButton//(MaxLabs.bbsMax.Entities.Post reply)
//        {
//            get
//            {
//                if (IsShowFinalQuestionLink && (MyUserID == ThreadContent.UserID || CanManage(ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, ThreadContent.UserID)))
//                {
//                    return true;
//                    //sb.Append(string.Format(linkStyle, BbsUrlHelper.GetFinalQuestionUrl(reply.ThreadID, Forum.ForumID), "结帖") + separator);
//                }
//                else
//                    return false;
//            }
//        }

//        private void appendFinalQuestionLink(StringBuilder sb, MaxLabs.bbsMax.Entities.Post reply, Forum Forum, string linkStyle, string separator)
//        {
//            if (IsShowFinalQuestionLink && reply.PostIndex == 0 && (MyUserID == reply.UserID || CanManage(ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, reply.UserID)))
//            {
//                sb.Append(string.Format(linkStyle, BbsUrlHelper.GetFinalQuestionUrl(reply.ThreadID, Forum.ForumID), "结帖", string.Empty) + separator);
//                //sb.Append("<a href=\"");
//                //sb.Append(BbsUrlHelper.GetFinalQuestionUrl(reply.ThreadID, Forum.ForumID));
//                //sb.Append("\">结帖</a> ");
//            }
//        }

//        private void voteQuestionBestPost(bool isUseful)
//        {
//            QuestionStatus status = PostManager.VoteQuestionBestPost(Thread.ThreadID, isUseful);
//            if (status != QuestionStatus.Success)
//            {
//                ShowError<QuestionStatus>(status);
//            }
//            else
//            {
//                question = null;
//                ShowInformation<QuestionStatus>(status);
//            }
//        }


//        public string GetActionTypeName(int actionType)
//        {
//            ModeratorCenterAction threadActionType = (ModeratorCenterAction)actionType;
//            return PostManager.GetActionName(threadActionType, Thread);
//        }

//        protected bool IsShowModeratorActionLinks
//        {
//            get
//            {
//                //为了节约性能，避免下面较为复杂的比较，如果检查到 Type 为空（表示完全正常状态）的帖子，如果没有管理权限就直接返回空
//                if (string.IsNullOrEmpty(Type))
//                    return Forum.HasSomeManagePermission(ManagePermission, Thread.PostUserID);
//                //return Forum.HaveModeratorPermission && Forum.AllowManageOtherUser(Thread.PostUserID);


//                //如果从回收站或者审核站进入，以下情况显示管理链接： 要么是回收站版主，要么帖子所在版块拥有管理权限（不需要检查是不是有审核或还原权限，因为如果他没有这个权限他根本不可能进入这个页面）
//                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0 ||
//                    string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0 ||
//                    string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
//                {
//                    if (Forum.HasSomeManagePermission(ManagePermission, Thread.PostUserID) ||
//                        ForumManager.GetSystemForum(((SystemForum)Enum.Parse(typeof(SystemForum), Type, true))).IsModerator)
//                        return true;
//                }
//                return false;
//            }
//        }

//        private void appendModeratorActionLink(StringBuilder sb, string outputFormat, string actionName, ThreadManageAction action, string CodeName, string separator)
//        {
//            if (sb.Length > 0)
//                sb.Append(separator);
//            sb.AppendFormat(outputFormat, actionName, BbsUrlHelper.GetThreadManagerUrl(CodeName, action.ToString()));
//        }

//        protected string GetModeratorActionLinks(string outputFormat, string separator)
//        {

//            //为了节约性能，避免下面较为复杂的比较，如果检查到 Type 为空（表示完全正常状态）的帖子，如果没有管理权限就直接返回空
//            if (string.IsNullOrEmpty(Type) && Forum.HasSomeManagePermission(ManagePermission, Thread.PostUserID) == false)
//                return string.Empty;

//            if (string.IsNullOrEmpty(outputFormat))
//                outputFormat = @"<a href=""javascript:manage('{1}')"">{0}</a> ";

//            //StringBuilder sb = new StringBuilder("<input Type=\"hidden\" name=\"ThreadID\" value=\"" + thread.ThreadID + "\" />");

//            StringBuilder sb = new StringBuilder();

//            string tmpCodeName = GetString("codename", "get");
//            if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
//            {
//                //Forum recycleBin = ForumManager.GetSystemForum(SystemForum.RecycleBin);
//                //if ((Forum.HaveModeratorPermission && Forum.AllowManageOtherUser(Thread.PostUserID)) || recycleBin.IsModerator)
//                //{
//                //if (Permission.SetThreadsRecycled || recycleBin.IsModerator)
//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "还原主题", ThreadManageAction.RevertThread, tmpCodeName, separator);
//                //if (Permission.DeleteThreads || recycleBin.IsModerator)
//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "删除主题", ThreadManageAction.DeleteThread, tmpCodeName, separator);
//                //}
//            }
//            else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
//            {
//                //Forum unapproveThreads = ForumManager.GetSystemForum(SystemForum.UnapproveThreads);
//                //if ((Forum.HaveModeratorPermission && Forum.AllowManageOtherUser(Thread.PostUserID) && Permission.ApproveThreads) || unapproveThreads.IsModerator)
//                //{
//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, Thread.PostUserID))
//                {
//                    appendModeratorActionLink(sb, outputFormat, "主题通过审核", ThreadManageAction.CheckThread, tmpCodeName, separator);
//                    appendModeratorActionLink(sb, outputFormat, "彻底删除主题", ThreadManageAction.DeleteThread, tmpCodeName, separator);
//                }
//                //}
//            }
//            //else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
//            //{
//            //    Forum unapprovePosts = ForumManager.GetSystemForum(SystemForum.UnapprovePosts);
//            //    if ((Forum.HaveModeratorPermission && Forum.AllowManageOtherUser(Thread.PostUserID) && Permission.ApprovePosts) || unapprovePosts.IsModerator)
//            //    {
//            //        appendModeratorActionLink(sb, outputFormat, "选中的回复通过审核", ThreadManageAction.ApprovePost, tmpCodeName, separator);
//            //        appendModeratorActionLink(sb, outputFormat, "彻底删除选中的回复", ThreadManageAction.DeleteUnapprovedPost, tmpCodeName, separator);
//            //        appendModeratorActionLink(sb, outputFormat, "所有回复通过审核", ThreadManageAction.ApprovePostByThreadIDs, tmpCodeName, separator);
//            //        appendModeratorActionLink(sb, outputFormat, "彻底删除所有回复", ThreadManageAction.DeleteUnapprovedPostByThreadIDs, tmpCodeName, separator);
//            //    }
//            //}
//            else
//            {
//                //if (!Forum.HaveModeratorPermission)
//                //    return string.Empty;

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "回收主题", ThreadManageAction.RecycleThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "删除主题", ThreadManageAction.DeleteThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.MoveThreads, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "移动主题", ThreadManageAction.MoveThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "设置锁定", ThreadManageAction.LockThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle))
//                    appendModeratorActionLink(sb, outputFormat, "高亮显示", ThreadManageAction.SetThreadSubjectStyle, CodeName, separator);

//                if (ManagePermission.HasPermissionForSomeone(MyUserID, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick) || ManagePermission.HasPermissionForSomeone(MyUserID, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick))
//                    appendModeratorActionLink(sb, outputFormat, "设置置顶", ThreadManageAction.SetThreadIsTop, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.Action.SetThreadsUp))
//                    appendModeratorActionLink(sb, outputFormat, "提升主题", ThreadManageAction.UpThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "自动沉帖", ThreadManageAction.SetThreadNotUpdateSortOrder, CodeName, separator);

//                if (ManagePermission.HasPermissionForSomeone(MyUserID, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued))
//                    appendModeratorActionLink(sb, outputFormat, "设置精华", ThreadManageAction.SetThreadElite, CodeName, separator);

//                if (Forum.ThreadCatalogStatus != ThreadCatalogStatus.DisEnable && CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "设置分类", ThreadManageAction.UpdateThreadCatalog, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.JoinThreads, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "合并主题", ThreadManageAction.JoinThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SplitThread, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "分割主题", ThreadManageAction.SplitThread, CodeName, separator);

//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads, Thread.PostUserID))
//                    appendModeratorActionLink(sb, outputFormat, "鉴定主题", ThreadManageAction.JudgementThread, CodeName, separator);


//                //if (Permission.DeletePosts)
//                //{
//                //    //sb.Append(" &nbsp;| &nbsp;");
//                //    appendModeratorActionLink(sb, outputFormat, "删除选中回复", ThreadManageAction.DeletePost, CodeName, separator);
//                //}


//            }
//            return sb.ToString();
//        }

//        protected string GetPostActionLink(string outputFormat, string separator)
//        {
//            if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
//            {
//                StringBuilder sb = new StringBuilder();
//                Forum unapprovePosts = ForumManager.GetSystemForum(SystemForum.UnapprovePosts);
//                if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads, Thread.PostUserID) || unapprovePosts.IsModerator)
//                {
//                    string tmpCodeName = GetString("codename", "get");
//                    appendModeratorActionLink(sb, outputFormat, "选中的回复通过审核", ThreadManageAction.ApprovePost, tmpCodeName, separator);
//                    appendModeratorActionLink(sb, outputFormat, "彻底删除选中的回复", ThreadManageAction.DeleteUnapprovedPost, tmpCodeName, separator);
//                    appendModeratorActionLink(sb, outputFormat, "所有回复通过审核", ThreadManageAction.ApprovePostByThreadIDs, tmpCodeName, separator);
//                    appendModeratorActionLink(sb, outputFormat, "彻底删除所有回复", ThreadManageAction.DeleteUnapprovedPostByThreadIDs, tmpCodeName, separator);
//                }
//                return sb.ToString();
//            }
//            else
//            {
//                //if (!Forum.HaveModeratorPermission)
//                //    return string.Empty;
//                if (ManagePermission.HasPermissionForSomeone(MyUserID, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts))
//                {
//                    return string.Format(outputFormat, "删除选中回复", BbsUrlHelper.GetThreadManagerUrl(CodeName, ThreadManageAction.DeletePost.ToString()));
//                }
//                return string.Empty;
//            }
//        }

//        protected bool IsShowThreadUpdateSortOrder
//        {
//            get
//            {
//                if (Thread.UpdateSortOrder == true)
//                    return false;
//                if (false == CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder, Thread.PostUserID))
//                {
//                    return false;
//                }
//                return true;
//            }
//        }

//        //protected MaxLabs.bbsMax.Entities.Post ThreadPost
//        //{
//        //    get
//        //    {
//        //        return Replies[0];
//        //    }
//        //}

//        #region 辩论

//        private Polemize polemize;
//        private List<MaxLabs.bbsMax.Entities.Post> agreePosts;
//        private List<MaxLabs.bbsMax.Entities.Post> againstPosts;
//        private List<MaxLabs.bbsMax.Entities.Post> neutralPosts;

//        private int agreePostCount = -1;
//        private int againstPostCount = -1;
//        private int neutralPostCount = -1;

//        protected int PolemizeCount
//        {
//            get
//            {
//                return Polemize.AgreeCount + Polemize.AgainstCount + Polemize.NeutralCount;
//            }
//        }
//        protected int AgreePostCount
//        {
//            get
//            {
//                if (polemize == null)
//                {
//                    GetPolemizePosts();//polemize.NeutralCount
//                }
//                return agreePostCount;
//            }
//        }
//        protected int AgainstPostCount
//        {
//            get
//            {
//                if (polemize == null)
//                {
//                    GetPolemizePosts();
//                }
//                return againstPostCount;
//            }
//        }
//        protected int NeutralPostCount
//        {
//            get
//            {
//                if (polemize == null)
//                {
//                    GetPolemizePosts();
//                }
//                return neutralPostCount;
//            }
//        }
//        protected Polemize Polemize
//        {
//            get
//            {
//                if (polemize == null)
//                {
//                    GetPolemizePosts();
//                }
//                return polemize;
//            }
//        }


//        int agreePageIndex = 0;
//        int againstPageIndex = 0;
//        int neutralPageIndex = 0;
//        private void GetPolemizePosts()
//        {
//            if (Thread.ThreadType == ThreadType.Polemize)
//            {
//                string extParms = Request.QueryString["extParms"];
//                #region 回复正序时
//                if (extParms != null)//从分类或者投票等列表页进来的
//                {
//                    if (agreePageIndex != -1)
//                    {
//                        try
//                        {
//                            agreePageIndex = int.Parse(GetExtParm("agreePage", extParms, "1")) - 1;
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                        agreePageIndex = 0;

//                    if (againstPageIndex != -1)
//                    {
//                        try
//                        {
//                            againstPageIndex = int.Parse(GetExtParm("againstPage", extParms, "1")) - 1;
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                        againstPageIndex = 0;

//                    if (neutralPageIndex != -1)
//                    {
//                        try
//                        {
//                            neutralPageIndex = int.Parse(GetExtParm("neutralPage", extParms, "1")) - 1;
//                        }
//                        catch
//                        {
//                        }
//                    }
//                    else
//                        neutralPageIndex = 0;
//                }
//                else
//                {
//                    if (agreePageIndex == -1)
//                    {
//                        agreePageIndex = 0;
//                    }
//                    if (againstPageIndex == -1)
//                    {
//                        againstPageIndex = 0;
//                    }
//                    if (neutralPageIndex == -1)
//                    {
//                        neutralPageIndex = 0;
//                    }
//                }
//                #endregion
//                #region 回复反序时（不要删除）
//                /*
//            if (extParms != null)//从分类或者投票等列表页进来的
//            {
//                if (agreePageIndex == 0)
//                {
//                    try
//                    {
//                        agreePageIndex = int.Parse(GetExtParm("agreePage", extParms, "1")) - 1;
//                    }
//                    catch
//                    {
//                    }
//                }
//                if (againstPageIndex == 0)
//                {
//                    try
//                    {
//                        againstPageIndex = int.Parse(GetExtParm("againstPage", extParms, "1")) - 1;
//                    }
//                    catch
//                    {
//                    }
//                }
//                if (neutralPageIndex == 0)
//                {
//                    try
//                    {
//                        neutralPageIndex = int.Parse(GetExtParm("neutralPage", extParms, "1")) - 1;
//                    }
//                    catch
//                    {
//                    }
//                }
//            }
//            */
//                #endregion
//                //    else
//                //    {
//                PostManager.GetPolemizePosts(agreePageIndex, againstPageIndex, neutralPageIndex, PageSize, Thread.ThreadID, out agreePosts, out agreePostCount, out againstPosts, out againstPostCount, out neutralPosts, out neutralPostCount, out polemize);
//                //}
//            }
//        }
//        protected List<MaxLabs.bbsMax.Entities.Post> AgreePosts
//        {
//            get
//            {
//                if (agreePosts == null)
//                {
//                    GetPolemizePosts();
//                }
//                return agreePosts;
//            }
//        }

//        protected List<MaxLabs.bbsMax.Entities.Post> AgainstPosts
//        {
//            get
//            {
//                if (againstPosts == null)
//                {
//                    GetPolemizePosts();
//                }
//                return againstPosts;
//            }
//        }

//        protected List<MaxLabs.bbsMax.Entities.Post> NeutralPosts
//        {
//            get
//            {
//                if (neutralPosts == null)
//                {
//                    GetPolemizePosts();
//                }
//                return neutralPosts;
//            }
//        }

//        private void ProcessAgreePost()
//        {
//            /* 
//            if (EnableQuickReplyValidateCode)
//            {
//                ValidateCodeCheck(GetString("validateCodeAgreePost", "post", string.Empty), "reply", GetString("ReturnUrl"));
//            }
//            */
//            string content = GetString("tbAgreeContent");
//            ProcessPost(content, PostType.Polemize_Agree, "Agree");

//        }
//        private void ProcessAgainstPost()
//        {
//            /* 
//            if (EnableQuickReplyValidateCode)
//            {
//                ValidateCodeCheck(GetString("validateCodeAgainstPost", "post", string.Empty), "reply", GetString("ReturnUrl"));
//            }
//            */

//            string content = GetString("tbAgainstContent");
//            ProcessPost(content, PostType.Polemize_Against, "Against");

//        }
//        private void ProcessNeutralPost()
//        {
//            /* 
//            if (EnableQuickReplyValidateCode)
//            {
//                ValidateCodeCheck(GetString("validateCodeNeutralPost", "post", string.Empty), "reply", GetString("ReturnUrl"));
//            }
//            */
//            string content = GetString("tbNeutralContent");
//            ProcessPost(content, PostType.Polemize_Neutral, "Neutral");

//        }

//        private string GetMessage(string type)
//        {
//            string vname = ValidateCodeManager.GetValidateCodeInputName("ReplyTopic", type);
//            string message = GetErrorMessage(vname);
//            if (message == null)
//                message = GetErrorMessage("error_" + type);

//            return message;
//        }
//        private string m_Error_Against;
//        protected string Error_Against
//        {
//            get
//            {
//                if (m_Error_Against == null)
//                {
//                    m_Error_Against = GetMessage("Against");
//                }
//                return m_Error_Against;
//            }
//        }
//        private string m_Error_Agree;
//        protected string Error_Agree
//        {
//            get
//            {
//                if (m_Error_Agree == null)
//                {
//                    m_Error_Agree = GetMessage("Agree");
//                }
//                return m_Error_Agree;
//            }
//        }
//        private string m_Error_Neutral;
//        protected string Error_Neutral
//        {
//            get
//            {
//                if (m_Error_Neutral == null)
//                {
//                    m_Error_Neutral = GetMessage("Neutral");
//                }
//                return m_Error_Neutral;
//            }
//        }

//        private void ProcessPost(string content, PostType postType, string id)
//        {
//            if (false == CheckValidateCode("ReplyTopic", id, GetString("ReturnUrl")))
//            {
//                return;
//            }

//            MaxLabs.bbsMax.Entities.Post post = new MaxLabs.bbsMax.Entities.Post();
//            post.PostType = postType;
//            post.TempSubject = "";//"re:" + Thread.PrimitiveSubject;

//            post.UserID = MyUserID;
//            post.NickName = My.Username;
//            post.TempContent = content;
//            post.IPAddress = RequestUtil.GetIpAddress(Request);


//            if (AllowHtml && AllowMaxcode)
//                post.EnableHTML = true;
//            else if (AllowHtml)
//                post.EnableHTML = true;
//            else if (AllowMaxcode)
//                post.EnableMaxCode3 = true;

//            //post.EnableHTML = AllowHtml;
//            //post.EnableMaxCode3 = AllowMaxcode;

//            //post.EnableEmoticons = false;
//            post.EnableSignature = true;
//            post.EnableReplyNotice = false;

//            post.ThreadID = ThreadID;
//            post.AllAttachments = new AttachmentCollection();
//            int postID;

//            string formName;
//            switch (postType)
//            {
//                case PostType.Polemize_Against: formName = "error_Against"; break;
//                case PostType.Polemize_Agree: formName = "error_Agree"; break;
//                case PostType.Polemize_Neutral: formName = "error_Neutral"; break;
//                default: formName = string.Empty; break;
//            }

//            MessageDisplay msgDisplay = CreateMessageDisplay(formName);

//            CreateUpdatePostStatus status = PostManager.ReplyThread(post, out postID);
//            if (status == CreateUpdatePostStatus.Success || status == CreateUpdatePostStatus.SuccessButUnapproved)
//            {
//                ValidateCodeManager.CreateValidateCodeActionRecode("ReplyTopic");
//                if (IsAjax)
//                {
//                    ShowInformation<CreateUpdatePostStatus>(status);

//                    polemize = null;
//                    #region 回复正序时（不要删除）
//                    /* 回复正序时 （不要删除）
//                int count;
//                if (postType == PostType.Polemize_Agree)
//                {
//                    count = AgreePostCount;
//                    agreePageIndex = (count % PageSize == 0 ? (count / PageSize) : (count / PageSize + 1)) - 1;
//                }
//                else if (postType == PostType.Polemize_Against)
//                {
//                    count = AgainstPostCount;
//                    againstPageIndex = (count % PageSize == 0 ? (count / PageSize) : (count / PageSize + 1)) - 1;
//                }
//                else if (postType == PostType.Polemize_Neutral)
//                {
//                    count = NeutralPostCount;
//                    neutralPageIndex = (count % PageSize == 0 ? (count / PageSize) : (count / PageSize + 1)) - 1;
//                }
//                */
//                    #endregion
//                    #region 回复反序时
//                    agreePageIndex = -1;
//                    againstPageIndex = -1;
//                    neutralPageIndex = -1;
//                    #endregion
//                    //=============
//                    Thread.TotalReplies += 1;
//                }
//                else
//                {
//                    int agreePage = 1, againstPage = 1, neutralPage = 1;
//                    #region 回复正序时（不要删除）
//                    /* 回复正序时 （不要删除）
//                if (extParms != null)//从分类或者投票等列表页进来的
//                {
//                    try
//                    {
//                        agreePage = int.Parse(GetExtParm("agreePage", extParms, "1"));
//                    }
//                    catch
//                    {
//                    }
//                    try
//                    {
//                        againstPage = int.Parse(GetExtParm("againstPage", extParms, "1"));
//                    }
//                    catch
//                    {
//                    }
//                    try
//                    {
//                        neutralPage = int.Parse(GetExtParm("neutralPage", extParms, "1"));
//                    }
//                    catch
//                    {
//                    }
//                }
//                int count;
//                if (postType == PostType.Polemize_Agree)
//                {
//                    count = AgreePostCount;
//                    agreePage = (count % PageSize == 0 ? (count / PageSize) : (count / PageSize + 1));
//                }
//                else if (postType == PostType.Polemize_Against)
//                {
//                    count = AgainstPostCount;
//                    againstPage = (count % PageSize == 0 ? (count / PageSize) : (count / PageSize + 1));
//                }
//                else if (postType == PostType.Polemize_Neutral)
//                {
//                    count = NeutralPostCount;
//                    neutralPage = (count % PageSize == 0 ? (count / PageSize) : (count / PageSize + 1));
//                }
//                */
//                    #endregion
//                    string extParmString = "agreePage={0}&againstPage={1}&neutralPage={2}";
//                    string returnUrl = BbsUrlHelper.GetThreadUrlForExtParms(CodeName, ThreadID, PageIndex + 1, string.Format(extParmString, agreePage.ToString(), againstPage.ToString(), neutralPage.ToString()));
//                    //string returnUrl = BbsUrlHelper.GetThreadUrl(Forum.CodeName, ThreadID, 1, 1);// +"|" + BbsUrlHelper.GetForumUrl(Forum.CodeName);
//                    GoUrl(returnUrl);
//                }
//            }
//            else
//            {
//                CatchError<ErrorInfo>(delegate(ErrorInfo error)
//                {
//                    msgDisplay.AddError(formName, error.Message);
//                });

//                if (msgDisplay.HasAnyError() == false)
//                    msgDisplay.AddError(formName, Bbs3Language.GetText<CreateUpdatePostStatus>(status));

//                //ShowError<CreateUpdatePostStatus>(status);
//            }
//        }

//        private void ProcessPolemize(ViewPointType viewPointType)
//        {
//            if (IsGuest)
//            {
//                ShowError("游客不能支持观点！");
//            }

//            //viewPointType = ViewPointType.Agree;

//            //try
//            //{
//            //    viewPointType = (ViewPointType)Enum.Parse(typeof(ViewPointType), GetString("ViewPointType"), true);
//            //}
//            //catch
//            //{
//            //    ShowError("参数错误！");
//            //}

//            PolemizeStatus status = PostManager.Polemize(ThreadID, viewPointType);
//            if (status == PolemizeStatus.HaveAgreeViewPoint)
//            {
//                if (viewPointType == ViewPointType.Agree)
//                    ShowError("您已经支持过该观点！");
//                else
//                    ShowError("您已经支持过正方观点，不能再支持其它方观点！");
//            }
//            else if (status == PolemizeStatus.HaveAgainstViewPoint)
//            {
//                if (viewPointType == ViewPointType.Against)
//                    ShowError("您已经支持过该观点！");
//                else
//                    ShowError("您已经支持过反方观点，不能再支持其它方观点！");
//            }
//            else if (status == PolemizeStatus.HaveNeutralViewPoint)
//            {
//                ShowError("您已经支持过中方观点，不能再支持其它方观点！");
//            }
//            else if (status != PolemizeStatus.Success)
//            {
//                ShowError<PolemizeStatus>(status);
//            }
//            else
//                ShowInformation<PolemizeStatus>(status);//,2, Request.UrlReferrer.ToString());
//        }
//        #endregion



//        protected bool isShowSignature(MaxLabs.bbsMax.Entities.Post reply)
//        {
//            if (reply.UserID == 0)
//                return false;

//            if (reply.PostType == PostType.ThreadContent)
//            {
//                if (false == ForumSetting.ShowSignatureInThread.GetValue(reply.UserID))
//                    return false;
//            }
//            else
//            {
//                if (false == ForumSetting.ShowSignatureInPost.GetValue(reply.UserID))
//                    return false;
//            }
//            if (Permission.Can(reply.UserID, ForumPermissionSetNode.Action.DisplayContent) && Forum.IsShieldedUser(reply.UserID) == false && reply.IsShielded == false && DisplaySignature && reply.EnableSignature && reply.User.SignatureFormatted.Trim() != string.Empty)
//            {
//                return true;
//            }
//            return false;
//        }

//        private Dictionary<int, bool> AllowImageAttachmentPermission = new Dictionary<int, bool>();
//        protected bool IsImage(string fileExtName, int userID)
//        {
//            bool isImage = MaxLabs.bbsMax.Ubb.PostUbbParser.isImage(fileExtName);
//            if (isImage == false)
//                return false;
//            //判断权限
//            bool allow = false;
//            if (AllowImageAttachmentPermission.TryGetValue(userID, out allow) == false)
//            {
//                if (ForumSetting.AllowImageAttachment.GetValue(userID))
//                    allow = true;
//                AllowImageAttachmentPermission.Add(userID, allow);
//            }

//            return allow;
//        }




//        #region 收藏夹
//        /*
//    private void ProcessCreateFavLink()
//    {
//        FavoriteLink fl = new FavoriteLink();
//        fl.DirectoryID = GetInt32("FavGroups", "post", 0);
//        fl.PostUserID = Thread.PostUserID;
//        fl.Subject = Thread.Subject;
//        fl.Url = BbsUrlHelper.GetThreadUrl(CodeName, Thread.ID, 1);
//        fl.PostDate = Thread.CreateDate;
//        fl.PostNickName = Thread.PostNickName;
//        CreateUpdateFavoriteLinkStatus status = FavoriteManager.CreateFavoriteLink(UserProfile.UserID, fl);
//        if (status == CreateUpdateFavoriteLinkStatus.Success)
//        {
//            ShowInformation("收藏主题成功!");
//        }
//        else
//        {
//            ShowError(status);
//        }
//    }
//    */
//        #endregion

//        #region 推荐
//        /*
//    private void ProcessCommend()
//    {
//        if (Logged)
//        {
//            string emailString = GetString("commendEmail", "post", string.Empty);

//            if (emailString == string.Empty)
//            {
//                ShowAlert("邮件地址不能为空！");
//            }

//            string description = GetString("commendDescription", "post", string.Empty);
//            string threadUrl = BbsUrlHelper.GetThreadUrl(CodeName, Thread.ID, 1);
//            //发送Email
//            Email email = new Email();
//            email.Subject = zzbird.Common.TextFormater.ProcessMessageTags(BBSSetting.CommendThreadEmailSubject, UserProfile.UserID);
//            email.Subject = email.Subject.Replace("$subject", Thread.Subject);

//            email.Content = zzbird.Common.TextFormater.ProcessMessageTags(BBSSetting.CommendThreadEmailContent, UserProfile.UserID, threadUrl);
//            email.Content = email.Content.Replace("$subject", Thread.Subject);
//            email.Content = email.Content.Replace("$reason", description);

//            //email.Subject = Regex.Replace(BBSSetting.CommendThreadEmailSubject,@"\$nickName",UserProfile.NickName,RegexOptions.IgnoreCase);
//            //email.Content = Regex.Replace(BBSSetting.CommendThreadEmailContent, @"\$nickName", UserProfile.NickName, RegexOptions.IgnoreCase);//"文章： <a href=\"" + threadUrl + "\">" + Thread.Subject + "</a><br>" + description;
//            //email.Content = Regex.Replace(email.Content, @"\$ThreadSubject", Thread.Subject, RegexOptions.IgnoreCase);
//            //email.Content = Regex.Replace(email.Content, @"\$ThreadUrl", threadUrl, RegexOptions.IgnoreCase);
//            //email.Content = Regex.Replace(email.Content, @"\$reason", description, RegexOptions.IgnoreCase);
//            email.ReceiveAddress = emailString;

//            CreateUpdateEmailStatus createUpdateEmailStatus = EmailManager.SendEmail(email);
//            if (createUpdateEmailStatus != CreateUpdateEmailStatus.Success)
//            {
//                if (createUpdateEmailStatus == CreateUpdateEmailStatus.Disabled)
//                {
//                    ShowAlert("论坛邮件没有启用，推荐失败!。");
//                }
//                else
//                {
//                    ShowError("邮件发送失败。");
//                }
//            }
//            else
//            {
//                ShowInformation("向好友推荐主题成功。");
//            }
//        }
//        else
//        {
//            ShowAlert("您没有登陆，无法向好友推荐主题！");
//        }
//    }
//    */
//        #endregion


//        #region 加好友，发短消息
//        private List<int> currentThreadUserIDs = new List<int>();

//        /// <summary>
//        /// 判断当前主题所有用户是否重复
//        /// </summary>
//        /// <param name="userID"></param>
//        /// <returns></returns>
//        public bool CurrentThreadUserID(int userID)
//        {
//            if (currentThreadUserIDs.Contains(userID))
//            {
//                return true;
//            }
//            else
//            {
//                currentThreadUserIDs.Add(userID);
//                return false;
//            }

//        }


//        #endregion


//        #region IP操作
//        private int currentPostID;
//        protected int CurrentPostID
//        {
//            get { return currentPostID; }
//            set { currentPostID = value; }
//        }

//        protected string CurrentViewIP
//        {
//            get
//            {
//                if (CurrentPostID != 0)
//                    return CommonUtil.FormatIP(PostManager.GetPost(this.CurrentPostID).IPAddress, AllowViewIPFields);
//                else
//                    return string.Empty;
//            }
//        }

//        protected string CurrentViewLocation
//        {
//            get
//            {
//                if (CurrentPostID != 0)
//                    return RequestUtil.GetLocation(PostManager.GetPost(this.CurrentPostID).IPAddress);
//                else
//                    return string.Empty;
//            }
//        }
//        /*
//        protected string IPAction
//        {
//            get
//            {
//                if (CurrentPostID != 0)
//                {
//                    if (IPHelper.IsIPInList(ProviderHelper.GetInstance().GetPost(this.CurrentPostID).IPAddress, SettingHelper.IPSetting.IPs))
//                        return "Del";
//                    else
//                        return "Add";
//                }
//                else
//                    return string.Empty;
//            }
//        }

//        private void AddLimitIP()
//        {
//            if (!ManagePermission.AllowManageIPs)
//                ShowError("没有权限!");

//            int postID = GetInt32("CurrentPostID", "post", 0);
//            string ipAdress = ProviderHelper.GetInstance().GetPost(postID).IPAddress;

//            ManageIPStatus manageIPStatus = CommonManager.AddLimitIP(ipAdress);
//            if (manageIPStatus == ManageIPStatus.Success)
//                ShowInformation("此IP添加到禁止列表成功！", true);
//            else
//                ShowError(manageIPStatus);
//        }

//        private void DelLimitIP()
//        {
//            if (!ManagePermission.AllowManageIPs)
//                ShowError("没有权限!");

//            int postID = GetInt32("CurrentPostID", "post", 0);
//            string ipAdress = ProviderHelper.GetInstance().GetPost(postID).IPAddress;

//            ManageIPStatus manageIPStatus = CommonManager.DelLimitIP(ipAdress);
//            if (manageIPStatus == ManageIPStatus.Success)
//                ShowInformation("解除此IP屏蔽成功！", true);
//            else
//                ShowError(manageIPStatus);
//        }
//        */
//        #endregion



//        private bool? m_CanReplyThread;
//        protected bool CanReplyThread
//        {
//            get
//            {
//                if (m_CanReplyThread == null)
//                {
//                    m_CanReplyThread = Can(ForumPermissionSetNode.Action.ReplyThread);
//                }
//                return m_CanReplyThread.Value;
//            }
//        }



//        private bool? m_CanCreateThread;
//        protected bool CanCreateThread
//        {
//            get
//            {
//                if (m_CanCreateThread == null)
//                {
//                    m_CanCreateThread = Can(ForumPermissionSetNode.Action.CreateThread);
//                }
//                return m_CanCreateThread.Value;
//            }
//        }


//        private bool? m_CanCreateQuestion;
//        protected bool CanCreateQuestion
//        {
//            get
//            {
//                if (m_CanCreateQuestion == null)
//                {
//                    m_CanCreateQuestion = Can(ForumPermissionSetNode.Action.CreateQuestion);
//                }
//                return m_CanCreateQuestion.Value;
//            }
//        }


//        private bool? m_CanCreatePoll;
//        protected bool CanCreatePoll
//        {
//            get
//            {
//                if (m_CanCreatePoll == null)
//                {
//                    m_CanCreatePoll = Can(ForumPermissionSetNode.Action.CreatePoll);
//                }
//                return m_CanCreatePoll.Value;
//            }
//        }


//        private bool? m_CanCreatePolemize;
//        protected bool CanCreatePolemize
//        {
//            get
//            {
//                if (m_CanCreatePolemize == null)
//                {
//                    m_CanCreatePolemize = Can(ForumPermissionSetNode.Action.CreatePolemize);
//                }
//                return m_CanCreatePolemize.Value;
//            }
//        }

//        protected bool DisplayContent(int userID)
//        {
//            return Permission.Can(userID, ForumPermissionSetNode.Action.DisplayContent);
//        }

//        protected bool ViewValuedThread
//        {
//            get
//            {
//                return Can(ForumPermissionSetNode.Action.ViewValuedThread);
//            }
//        }

//        protected bool ViewThread
//        {
//            get
//            {
//                return Can(ForumPermissionSetNode.Action.ViewThread);
//            }
//        }

//        protected bool ViewAttachment
//        {
//            get
//            {
//                return Can(ForumPermissionSetNode.Action.ViewAttachment);
//            }
//        }

//        protected bool ViewPollDetail
//        {
//            get
//            {
//                return Can(ForumPermissionSetNode.Action.ViewPollDetail);
//            }
//        }

//        private UserPoint m_SellThreadPoint;
//        protected UserPoint SellThreadPoint
//        {
//            get
//            {
//                if (m_SellThreadPoint == null)
//                {
//                    m_SellThreadPoint = ForumPointAction.Instance.GetUserPoint(Thread.PostUserID, ForumPointValueType.SellThread, Forum.ForumID);
//                }
//                return m_SellThreadPoint;
//            }
//        }

//        //private UserPoint m_SellAttachmentPoint;
//        Dictionary<int, UserPoint> attachmentPoints = new Dictionary<int, UserPoint>();
//        protected UserPoint GetAttachmentPoint(int userID)
//        {
//            if (attachmentPoints.ContainsKey(userID))
//                return attachmentPoints[userID];
//            else
//            {
//                UserPoint userPoint = ForumPointAction.Instance.GetUserPoint(userID, ForumPointValueType.SellAttachment, Forum.ForumID);
//                attachmentPoints.Add(userID, userPoint);
//                return userPoint;
//            }
//        }

//        private UserPoint m_QuestionPoint;
//        protected UserPoint QuestionPoint
//        {
//            get
//            {
//                if (m_QuestionPoint == null)
//                {
//                    m_QuestionPoint = ForumPointAction.Instance.GetUserPoint(Thread.PostUserID, ForumPointValueType.QuestionReward, Forum.ForumID);
//                }
//                return m_QuestionPoint;
//            }
//        }

//        protected Medal medal;
//        protected MedalCollection Medals
//        {
//            get
//            {
//                return AllSettings.Current.MedalSettings.Medals;
//            }
//        }

//        protected bool IsShowMedal(User user)
//        {
//            if (Medals.Count == 0)
//                return false;

//            //if (user.UserID == MyUserID)
//            //{
//            //    return true;
//            //}
//            //else
//            //{
//            foreach (Medal medal in Medals)
//            {
//                if (medal.GetMedalLevel(user) != null)
//                    return true;
//            }

//            return false;
//            //}
//        }

//        private Dictionary<int, string> medalImages = new Dictionary<int, string>();
//        protected string GetMedals(string imgFormat, string imgFormat2, User user)
//        {
//            string imgstring;
//            if (medalImages.TryGetValue(user.UserID, out imgstring))
//                return imgstring;

//            StringBuilder imgs = new StringBuilder();
//            foreach (Medal medal in Medals)
//            {
//                MedalLevel medalLevel = medal.GetMedalLevel(user);
//                if (medalLevel != null)
//                {
//                    string title;
//                    if (medalLevel.Name != string.Empty)
//                    {
//                        title = medal.Name + "(" + medalLevel.Name + ")";
//                    }
//                    else
//                        title = medal.Name;

//                    imgs.Append(string.Format(imgFormat, medalLevel.LogoUrl, title));
//                }
//            }
//            //if (user.UserID == MyUserID)
//            //{
//            //    foreach (Medal medal in Medals)
//            //    {
//            //        MedalLevel medalLevel = medal.GetMedalLevel(user);
//            //        if (medalLevel == null)
//            //        {
//            //            if (medal.Levels.Count == 0)
//            //                continue;
//            //            medalLevel = medal.Levels[0];
//            //            imgs.Append(string.Format(imgFormat2, medalLevel.LogoUrl, medalLevel.Name));
//            //        }
//            //    }
//            //}

//            imgstring = imgs.ToString();

//            medalImages.Add(user.UserID, imgstring);

//            return imgstring;
//        }



//        #region forumSetting 设置

//        private ForumSettingItem m_ForumSetting;
//        protected ForumSettingItem ForumSetting
//        {
//            get
//            {
//                if (m_ForumSetting == null)
//                    m_ForumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(Forum.ForumID);
//                return m_ForumSetting;
//            }
//        }

//        private bool? m_AllowHtml;
//        public bool AllowHtml
//        {
//            get
//            {
//                if (m_AllowHtml == null)
//                    m_AllowHtml = ForumSetting.CreatePostAllowHTML.GetValue(MyUserID);

//                return m_AllowHtml.Value;
//            }
//        }

//        private bool? m_AllowMaxcode;
//        public bool AllowMaxcode
//        {
//            get
//            {
//                if (m_AllowMaxcode == null)
//                    m_AllowMaxcode = ForumSetting.CreatePostAllowMaxcode.GetValue(MyUserID);

//                return m_AllowMaxcode.Value;
//            }
//        }

//        private bool? m_AllowAudioTag;
//        public bool AllowAudioTag
//        {
//            get
//            {
//                if (m_AllowAudioTag == null)
//                    m_AllowAudioTag = ForumSetting.CreatePostAllowAudioTag.GetValue(MyUserID);

//                return m_AllowAudioTag.Value;
//            }
//        }

//        private bool? m_AllowEmoticon;
//        public bool AllowEmoticon
//        {
//            get
//            {
//                if (m_AllowEmoticon == null)
//                    m_AllowEmoticon = ForumSetting.CreatePostAllowEmoticon.GetValue(MyUserID);

//                return m_AllowEmoticon.Value;
//            }
//        }

//        private bool? m_AllowFlashTag;
//        public bool AllowFlashTag
//        {
//            get
//            {
//                if (m_AllowFlashTag == null)
//                    m_AllowFlashTag = ForumSetting.CreatePostAllowFlashTag.GetValue(MyUserID);
//                return m_AllowFlashTag.Value;
//            }
//        }

//        private bool? m_AllowImageTag;
//        public bool AllowImageTag
//        {
//            get
//            {
//                if (m_AllowImageTag == null)
//                    m_AllowImageTag = ForumSetting.CreatePostAllowImageTag.GetValue(MyUserID);

//                return m_AllowImageTag.Value;
//            }
//        }
//        private bool? m_AllowTableTag;
//        public bool AllowTableTag
//        {
//            get
//            {
//                if (m_AllowTableTag == null)
//                    m_AllowTableTag = ForumSetting.CreatePostAllowTableTag.GetValue(MyUserID);
//                return m_AllowTableTag.Value;
//            }
//        }

//        private bool? m_AllowUrlTag;
//        public bool AllowUrlTag
//        {
//            get
//            {
//                if (m_AllowUrlTag == null)
//                    m_AllowUrlTag = ForumSetting.CreatePostAllowUrlTag.GetValue(MyUserID);

//                return m_AllowUrlTag.Value;
//            }
//        }

//        private bool? m_AllowVideoTag;
//        public bool AllowVideoTag
//        {
//            get
//            {
//                if (m_AllowVideoTag == null)
//                    m_AllowVideoTag = ForumSetting.CreatePostAllowVideoTag.GetValue(MyUserID);

//                return m_AllowVideoTag.Value;

//            }
//        }

//        #endregion


//        #region 版块权限
//        /// <summary>
//        /// 检查当前登陆用户权限
//        /// </summary>
//        /// <param name="action"></param>
//        /// <returns></returns>
//        public bool Can(ForumPermissionSetNode.Action action)
//        {
//            return Permission.Can(My, action);
//        }

//        //public bool Can(ForumPermissionSetNode.ActionWithTarget action, int targetUserID)
//        //{
//        //    return Permission.Can(My, action, targetUserID);
//        //}

//        public bool CanManage(ManageForumPermissionSetNode.Action action)
//        {
//            return ManagePermission.Can(My, action);
//        }

//        public bool CanManage(ManageForumPermissionSetNode.ActionWithTarget action, int targetUserID)
//        {
//            return ManagePermission.Can(My, action, targetUserID);
//        }

//        public bool CanManage(ManageForumPermissionSetNode.ActionWithTarget action, int targetUserID, int lastEditUserID)
//        {
//            return ManagePermission.Can(My, action, targetUserID, lastEditUserID);
//        }

//        private ForumPermissionSetNode m_Permission;
//        public ForumPermissionSetNode Permission
//        {
//            get
//            {
//                if (m_Permission == null)
//                    m_Permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(Forum.ForumID);

//                return m_Permission;
//            }
//        }

//        private ManageForumPermissionSetNode m_ManagePermission;
//        public ManageForumPermissionSetNode ManagePermission
//        {
//            get
//            {
//                if (m_ManagePermission == null)
//                    m_ManagePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(Forum.ForumID);
//                return m_ManagePermission;
//            }
//        }
//        #endregion


//        protected override string MetaKeywords
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(Forum.ExtendedAttribute.MetaKeywords))
//                    return base.MetaKeywords;
//                else
//                    return Forum.ExtendedAttribute.MetaKeywords;
//            }
//        }

//        protected override string TitleAttach
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(Forum.ExtendedAttribute.TitleAttach))
//                    return base.TitleAttach;
//                else
//                    return Forum.ExtendedAttribute.TitleAttach;
//            }
//        }

//        private Regex regex;
//        protected string Highlight(string content)
//        {
//            string HighlightColor = AllSettings.Current.SearchSettings.HighlightColor;
//            if (regex == null)
//            {
//                List<string> words = PostBO.Instance.GetSearchKeywords(SearchText);

//                if (words.Count == 0)
//                    return content;

//                string partten = null;
//                foreach (string word in words)
//                {
//                    partten += word + "|";
//                }

//                partten = partten.Substring(0, partten.Length - 1);


//                regex = new Regex(@"(?<=(?:^|>)[^<>]*?)((" + partten + "))(?=[^<>]*?(?:<|$))", RegexOptions.IgnoreCase);
//            }

//            return regex.Replace(content, string.Format(@"<font style=""color:{0};font-weight:Bold"">$1</font>", HighlightColor));

//        }

//        protected bool isShowAttachments(MaxLabs.bbsMax.Entities.Post post)
//        {
//            if (post.PostType == PostType.ThreadContent && Thread.Price > 0 && Thread.PostUserID != MyUserID && AlwaysViewContents == false && Thread.IsBuyed == false)
//                return false;

//            return true;
//        }


    }
}