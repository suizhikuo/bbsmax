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
    public partial class view_thread : ThreadPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void ProcessSubmits()
        {
            if (_Request.IsClick("postButton"))
            {
                ProcessPost();
            }
            else
                base.ProcessSubmits();
        }


        private BasicThread m_Thread;
        protected override BasicThread Thread
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

        private bool? m_MustRemovePostListFirstOne = null;
        /// <summary>
        /// 是否需要移除m_PostList里的第一个帖子  通常第一页 m_PostList里包含主题内容 要移除
        /// </summary>
        protected virtual bool MustRemovePostListFirstOne
        {
            get
            {
                if (m_MustRemovePostListFirstOne == null)
                {
                    m_MustRemovePostListFirstOne = (IsOnlyLookOneUser == false && PageNumber == 1 && IsGetPost == false);
                }
                return m_MustRemovePostListFirstOne.Value;
            }
        }

        private bool hasRemovedThreadContent;
        private bool hasSetThreadLastPost;
        protected PostCollectionV5 m_PostList;
        protected PostCollectionV5 PostList
        {
            get
            {
                if (m_PostList == null)
                {
                    GetThread();

                }
                if (hasSetThreadLastPost == false)
                {
                    hasSetThreadLastPost = true;
                    if (ThreadLastPostID > 0 && PageNumber != Thread.TotalPages)
                    {
                        PostV5 lastPost = ThreadCachePool.GetPost(ThreadID, ThreadLastPostID);
                        if (lastPost == null)
                            lastPost = PostBOV5.Instance.GetPost(ThreadLastPostID, true);

                        if (lastPost != null)
                        {
                            if (lastPost.FloorNumber > 0)
                                lastPost.PostIndex = lastPost.FloorNumber - 1;
                            else
                                lastPost.PostIndex = Thread.TotalReplies;
                            m_PostList.Add(lastPost);
                        }
                    }
                }
                if (hasRemovedThreadContent == false && MustRemovePostListFirstOne)
                {
                    hasRemovedThreadContent = true;
                    if (m_PostList.Count > 0)
                    {
                        //移除掉主题内容
                        m_PostList.RemoveAt(0);
                    }
                }
                return m_PostList;
            }
        }

 
        protected bool IsLastPost(PostV5 post)
        {
            if (post.PostType == PostType.ThreadContent)
                return true;
            if (post.PostID == LastPostID)
                return true;
            return false;
        }

        private int? m_TotalPosts;
        protected override int TotalPosts
        {
            get
            {
                if (IsOnlyLookOneUser || IsGetPost || IsUnapprovePosts)// string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
                {
                    if (m_TotalPosts == null)
                    {
                        GetThread();
                    }
                    return m_TotalPosts.Value;
                }
                else
                    return base.TotalPosts;
            }
        }

        protected bool GetPosts(ThreadType threadType, out int? totalCount, out BasicThread thread, out PostCollectionV5 posts)
        {
            totalCount = null;
            thread = null;
            posts = null;
            ThreadType realThreadType = threadType;
            if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
            {
                PostBOV5.Instance.GetPosts(ThreadID, false, PageNumber, PageSize, null, true, true, true, true, ref thread, out posts, ref realThreadType);
                if (realThreadType != threadType)
                {
                    BbsRouter.JumpToUrl(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(realThreadType)), "type=" + Type);
                }

                if (thread == null)
                {
                    ShowError("您要查看的主题不存在或者已被删除");
                }

                //totalCount = base.TotalPosts;
                return true;
            }
            else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
            {
                PostBOV5.Instance.GetPosts(ThreadID, false, PageNumber, PageSize, null, true, true, true, true, ref thread, out posts, ref realThreadType);
                //m_Thread.ThreadContent = m_PostList[0];
                totalCount = posts.TotalRecords;
                if (realThreadType != threadType)
                {
                    BbsRouter.JumpToUrl(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(realThreadType)), "type=" + Type);
                }

                if (thread == null)
                {
                    ShowError("您要查看的主题不存在或者已被删除");
                }

                return true;
            }
            else if (string.Compare(Type, MyThreadType.MyUnapprovedThread.ToString(), true) == 0)
            //|| string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0)
            {
                int count;
                PostBOV5.Instance.GetUnapprovedPostThread(ThreadID, MyUserID, PageNumber, PageSize, out thread, out posts, out count);


                if (thread == null)
                {
                    ShowError("您要查看的主题不存在或者已被删除");
                }
                if (posts.Count > 0)
                    thread.ThreadContent = posts[0];
                else
                    ShowError("您要查看的主题不存在或者已被删除");


                totalCount = count;
                return true;
            }
            else if (IsGetPost)
            {
                if (threadType != ThreadType.Normal)
                {
                    BbsRouter.JumpToUrl(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(ThreadType.Normal)), "type=getpost&postid=" + PostID);
                }

                thread = PostBOV5.Instance.GetThread(ThreadID);
                PostV5 post = PostBOV5.Instance.GetPost(PostID, true);
                if (post == null)
                {
                    ShowError("您要查看的帖子不存在或者已被删除");
                }

                posts = new PostCollectionV5();
                posts.Add(post);

                totalCount = 1;

                return true;
            }
            else
                return false;
        }

        protected virtual void GetThread()
        {
            ThreadType realThreadType = ThreadType.Normal;

            if (IsOnlyLookOneUser)
            {
                int totalCount;
                PostBOV5.Instance.GetUserPosts(ThreadID, LookUserID, ThreadType.Normal, PageNumber, PageSize, true, false, out m_Thread, out m_PostList, out realThreadType, out totalCount);

                m_TotalPosts = totalCount;
            }
            else
            {
                if (string.IsNullOrEmpty(Type))
                {
                    PostBOV5.Instance.GetThreadWithReplies(ThreadID, PageNumber, PageSize, true, UpdateView, true, out m_Thread, out m_PostList, out realThreadType);
                }
                else if (GetPosts(ThreadType.Normal, out m_TotalPosts, out m_Thread, out m_PostList))
                {
                }
                else if (IsUnapprovePosts || IsMyUnapprovePosts)
                {
                    m_MustRemovePostListFirstOne = false;
                    int total;
                    PostBOV5.Instance.GetUnapprovedPostThread(ThreadID, null, PageNumber, PageSize, out m_Thread, out m_PostList, out total);
                    m_TotalPosts = total;
                }
                else
                    ShowError(new InvalidParamError("type"));
            }
            //如果不是 投票 问题  辩论  则跳到相应的页面
            if (realThreadType != ThreadType.Normal)
            {
                Response.Redirect(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(realThreadType)));
            }

            PostBOV5.Instance.ProcessKeyword(m_PostList, ProcessKeywordMode.TryUpdateKeyword);

            //if (_Request.IsSpider == false)
            //{
                //List<int> userIDs = new List<int>();
                //foreach(PostV5 post in m_PostList)
                //{
                //    userIDs.Add(post.UserID);
                //}
                //UserBO.Instance.GetUsers(userIDs, GetUserOption.WithAll);
                UserBO.Instance.GetUsers(m_PostList.GetUserIds(), GetUserOption.WithAll);
            //}
        }

        private int? m_LastPostID = null;
        protected int LastPostID
        {
            get
            {
                if (m_LastPostID == null)
                {
                    if (PostList.Count > 0)
                        m_LastPostID = PostList[PostList.Count - 1].PostID;
                    else
                        m_LastPostID = 0;
                }
                return m_LastPostID.Value;
            }
        }


        protected bool IsUnapprovedTopic
        {
            get
            {
                if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0 ||
                    string.Compare(Type, MyThreadType.MyUnapprovedPostThread.ToString(), true) == 0 ||
                    string.Compare(Type, MyThreadType.MyUnapprovedThread.ToString(), true) == 0)
                    return true;
                else
                    return false;
            }
        }


        private bool? m_HasBuyed;
        protected bool HasBuyed
        {
            get
            {
                if (m_HasBuyed == null)
                {
                    if (Thread.Price == 0 || MyUserID == Thread.PostUserID || Can(ForumPermissionSetNode.Action.AlwaysViewContents) || Thread.IsOverSellDays(ForumSetting) || Thread.IsBuyed(My))
                    {
                        m_HasBuyed = true;
                    }
                    else
                        m_HasBuyed = false;
                }

                return m_HasBuyed.Value;
            }
        }


        private string m_MetaDescription;
        protected new string MetaDescription
        {
            get
            {
                if (m_MetaDescription == null)
                {
                    PostV5 post = null;
                    if (PageNumber == 1 && Type == string.Empty && LookUserID == -1)
                        post = Thread.ThreadContent;
                    else if (PostList.Count > 0)
                        post = PostList[0];

                    if (post != null)
                    {
                        if (IsShowContent(post))
                        {
                            m_MetaDescription = StringUtil.CutString(StringUtil.ClearAngleBracket(post.ContentText), 200);
                        }
                        else
                            m_MetaDescription = StringUtil.ClearAngleBracket(Thread.SubjectText);
                    }
                    else
                        m_MetaDescription = StringUtil.ClearAngleBracket(Thread.SubjectText);
                }
                return m_MetaDescription;
            }
        }



        private int? m_PostID;
        protected int PostID
        {
            get
            {
                if (m_PostID == null)
                {
                    m_PostID = _Request.Get<int>("postid", Method.Get, 0);
                }
                return m_PostID.Value;
            }
        }

        private int? m_ThreadLastPostID;
        protected int ThreadLastPostID
        {
            get
            {
                if (m_ThreadLastPostID == null)
                {
                    m_ThreadLastPostID = _Request.Get<int>("lastpostid", Method.Get, 0);
                }

                return m_ThreadLastPostID.Value;
            }
        }



        protected void ProcessPost()
        {
            string validateCodeAction = "ReplyTopic";
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (CheckValidateCode(validateCodeAction, msgDisplay))
            {
                PostType postType = (PostType)_Request.Get<int>("viewPointType", Method.Post, 0);
                bool isReplyPolemize = _Request.Get<bool>("isReplyPolemize", Method.Post, false);

                if (isReplyPolemize && postType == PostType.Normal)
                {
                    msgDisplay.AddError("请选择您的观点！");
                    return;
                }

                string postNickName;
                if (IsLogin == false)
                {
                    if (EnableGuestNickName)
                        postNickName = _Request.Get("guestNickName", Method.Post, "");
                    else
                        postNickName = "";
                }
                else
                {
                    postNickName = My.Name;
                }
                string subject = _Request.Get("Subject", Method.Post, string.Empty);
                string content = _Request.Get("Editor", Method.Post, string.Empty,false);
                string iPAddress = _Request.IpAddress;

                //string enableItems = _Request.Get("enableItem", Method.Post, string.Empty);

                bool enableHTML = false;
                bool enableMaxCode3 = false;
                if (AllowHtml && AllowMaxcode)
                {
                    enableHTML = _Request.Get<int>("eritoSellect", Method.Post, 0) == 1;

                    if (enableHTML == false)
                        enableMaxCode3 = true;
                }
                else if (AllowHtml)
                    enableHTML = true;

                else if (AllowMaxcode)
                    enableMaxCode3 = true;

                bool enableEmoticons = true;// (enableItems.IndexOf("enableemoticons", StringComparison.OrdinalIgnoreCase) > -1);
                bool enableSignature = true;//(enableItems.IndexOf("enablesignature", StringComparison.OrdinalIgnoreCase) > -1);
                bool enableReplyNotice = true;//(enableItems.IndexOf("enablereplynotice", StringComparison.OrdinalIgnoreCase) > -1);

                int postID = 0;
                bool success = false;
                bool hasCatchError = false;
                try
                {
                    success = PostBOV5.Instance.ReplyThread(
                        My, ThreadID, postType, 0, subject, content, enableEmoticons, enableHTML, enableMaxCode3, enableSignature
                        , enableReplyNotice, ForumID, postNickName, iPAddress, 0, new AttachmentCollection(), false, out postID);

                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                    hasCatchError = true;
                }

                if (hasCatchError == false)
                {
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UnapprovedError)
                            {
                                _Request.Clear();
                                AlertWarning(error.Message);
                            }
                            else
                                msgDisplay.AddError(error);
                        });
                    }
                    else
                    {

                        ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);
                        bool returnLastUrl = _Request.Get<int>("tolastpage", Method.Post, 0) == 1;

                        UserBO.Instance.UpdateUserReplyReturnThreadLastPage(My, returnLastUrl);

                        if (IsAjaxRequest)
                        {
                            AlertSuccess("操作成功");//告诉客户端操作成功
                            if (returnLastUrl)
                                PageNumber = PostBOV5.Instance.GetThread(ThreadID).TotalPages;
                            else
                            {
                                PageNumber = PageNumber;
                                m_ThreadLastPostID = postID;
                            }
                            //_Request.Clear();
                        }
                        else
                        {
                            string returnUrl;
                            if (returnLastUrl)
                                returnUrl = BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, ThreadID, postID, PostBOV5.Instance.GetThread(ThreadID).TotalPages, true);
                            else
                                returnUrl = BbsUrlHelper.GetThreadUrl(Forum.CodeName, ThreadID, PostBOV5.Instance.GetThread(ThreadID).ThreadTypeString, PageNumber) + "?lastPostID=" + postID;
                            Response.Redirect(returnUrl);
                        }

                    }
                }
            }
        }


        /// <summary>
        /// 问题帖时 是否显示该贴得分
        /// </summary>
        protected virtual bool IsShowPostGetReward(int postID)
        {
            return false;
        }

    }
}