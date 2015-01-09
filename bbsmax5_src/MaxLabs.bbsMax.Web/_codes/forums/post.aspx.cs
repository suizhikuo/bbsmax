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
using MaxLabs.bbsMax.Ubb;
using MaxLabs.bbsMax.ValidateCodes;
using System.Text.RegularExpressions;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    public partial class Post : ForumPageBase
    {

        protected string Content = string.Empty, Subject = string.Empty;
        protected PostV5 CurrentPost;
        protected BasicThread Thread;
        protected string ActionName;
        protected bool IsShowThreadCatalog = true;
        protected double RewardTax;
        protected bool IsShowPollOptions, IsShowPollExpiresDate, IsShowQuestionOptions, IsShowPolemizeOptions;
        protected int SelectCatalogID = 0, CheckedIconID = 0;
        protected int Time, RealReward;
        protected string TimeUnit;

        protected void Page_Load(object sender, System.EventArgs e)
        {

            if (_Request.Get("Review", Method.Post, "") != "")
            {
                m_Review = false; 
                //return;
            }

            if (_Request.IsClick("postButton"))
            {
                ProcessPost();
            }
            if (_Request.IsClick("reviewButton"))
            {
                ProcessReview();
            }

            if (Forum.IsShieldedUser(MyUserID))
                ShowError("您在当前版块已被屏蔽，不能进行此操作");

            AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(Forum.CodeName));

            if (string.IsNullOrEmpty(Type))
            {
            }
            else
            {
                if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
                    AddNavigationItem("[回收站]", UrlHelper.GetRecycledThreadsUrl(CodeName));
                else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
                    AddNavigationItem("[未审核的主题]", UrlHelper.GetUnapprovedThreadsUrl(CodeName));
                else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
                    AddNavigationItem("[未审核的回复]", UrlHelper.GetUnapprovedPostsUrl(CodeName));
            }

            switch (Action)
            {
                case "thread":
                    if (!Can(ForumPermissionSetNode.Action.CreateThread))
                    {
                        if (!Can(ForumPermissionSetNode.Action.CreatePoll))
                        {
                            if (!Can(ForumPermissionSetNode.Action.CreateQuestion))
                            {
                                if (!Can(ForumPermissionSetNode.Action.CreatePolemize))
                                {
                                    if (MyUserID == 0)
                                        ShowError("您是游客，没有发表主题的权限", Request.RawUrl, true);
                                    else
                                        ShowError("您所在的用户组没有发表主题的权限", Request.RawUrl, false);
                                }
                                else
                                {
                                    m_Action = "polemize";
                                }
                            }
                            else
                            {
                                m_Action = "question";
                            }
                        }
                        else
                        {
                            m_Action = "poll";
                        }

                        BbsRouter.JumpToUrl(BbsUrlHelper.GetCreatThreadUrl(Forum.CodeName, Action), string.Empty);
                    }
                    break;
                default: break;
            }

            switch (Action)
            {

                case "reply":

                    if (!Can(ForumPermissionSetNode.Action.ReplyThread))
                    {
                        if (MyUserID == 0)
                            ShowError("您是游客，没有回复主题的权限", Request.RawUrl, true);
                        else
                            ShowError("您所在的用户组没有回复主题的权限", Request.RawUrl, false);
                    }
                    //DisabledItem(false, null);
                    Thread = PostBOV5.Instance.GetThread(ThreadID);
                    if (Thread == null)
                        ShowError("您要回复的主题不存在或已被删除");
                    AddNavigationItem(Thread.SubjectText, BbsUrlHelper.GetThreadUrl(Forum.CodeName, Thread.ThreadID, 1));

                    ActionName = "发表回复";
                    //defaultSubject = "Re:" + thread.Subject;
                    IsShowThreadCatalog = false;
                    UpdateOnlineStatus(OnlineAction.ReplyThread, ThreadID, Thread.SubjectText);
                    break;

                case "replypost":
                    if (!Can(ForumPermissionSetNode.Action.ReplyThread))
                    {
                        if (MyUserID == 0)
                            ShowError("您是游客，没有回复帖子的权限", Request.RawUrl, true);
                        else
                            ShowError("您所在的用户组没有回复帖子的权限", Request.RawUrl, false);
                    }
                    //DisabledItem(false, null);
                    Thread = PostBOV5.Instance.GetThread(ThreadID);
                    if (Thread == null)
                        ShowError("您要回复的主题不存在或已被删除");
                    AddNavigationItem(Thread.SubjectText, BbsUrlHelper.GetThreadUrl(Forum.CodeName, Thread.ThreadID, 1));
                    ActionName = "发表回复";
                    //defaultSubject = Server.UrlDecode(Request.QueryString["PostIndexAlias"].Trim());
                    IsShowThreadCatalog = false;
                    UpdateOnlineStatus(OnlineAction.ReplyThread, ThreadID, Thread.SubjectText);
                    m_Action = "reply";
                    break;
                case "thread":
                    //上面已经检查过发表主题的权限 此处再不检查
                    //DisabledItem(false, null);
                    ActionName = "发表新主题";
                    UpdateOnlineStatus(OnlineAction.CreateThread, 0, "");
                    break;

                case "question":
                    if (!Can(ForumPermissionSetNode.Action.CreateQuestion))
                    {
                        if(IsLogin == false)
                            ShowError("您是游客，没有发表提问帖的权限", Request.RawUrl, true);
                        else
                            ShowError("您所在的用户组没有发表提问帖的权限", Request.RawUrl, false);
                    }
                    //DisabledItem(false, null);
                    IsShowQuestionOptions = true;
                    ActionName = "发表新提问";

                    RewardTax = AllSettings.Current.PointSettings.TradeRate / 100.0;//setting.TradingTax / 100.0;
                    UpdateOnlineStatus(OnlineAction.CreateThread, 0, "");
                    break;
                case "polemize":
                    if (!Can(ForumPermissionSetNode.Action.CreatePolemize))
                    {
                        if (IsLogin == false)
                            ShowError("您是游客，没有发表辩论帖的权限", Request.RawUrl, true);
                        else
                            ShowError("您所在的用户组没有发表辩论帖的权限", Request.RawUrl, false);
                    }
                    //DisabledItem(false, null);
                    IsShowPolemizeOptions = true;
                    ActionName = "发表新辩论";
                    UpdateOnlineStatus(OnlineAction.CreateThread, 0, "");
                    break;
                case "poll":
                    if (!Can(ForumPermissionSetNode.Action.CreatePoll))
                    {
                        if (IsLogin == false)
                            ShowError("您是游客，没有发表投票帖的权限", Request.RawUrl, true);
                        else
                            ShowError("您所在的用户组没有发表投票帖的权限", Request.RawUrl, false);
                    }

                    ActionName = "发表新投票";
                    IsShowPollOptions = true;
                    UpdateOnlineStatus(OnlineAction.CreateThread, 0, "");
                    break;
                case "editpoll":
                case "editquestion":
                case "editpolemize":
                case "editthread":
                    ActionName = "编辑主题";
                    if (IsLogin == false)
                        ShowError("您还未登陆不能进行编辑主题操作");
                    //PostCollectionV5 posts;
                    //ThreadType realType;
                    //if (Type == string.Empty)
                    //{
                        //bool isWrongType = false;
                        //if (Action == "editthread")
                        //{
                        //    PostBOV5.Instance.GetThread(
                        //    PostBOV5.Instance.GetThreadWithReplies(ThreadID, 1, 1, true, false, true, out Thread, out posts, out realType);
                        //    if (realType != ThreadType.Normal)
                        //        isWrongType = true;
                        //}
                        //else if (Action == "editquestion")
                        //{
                        //    QuestionThread question;

                        //    PostBOV5.Instance.GetQuestionWithReplies(ThreadID, 1, 1, true, false, out question, out posts, out realType);
                        //    if (realType != ThreadType.Question)
                        //        isWrongType = true;
                        //    else
                        //        Thread = (BasicThread)question;
                        //}
                        //else if (Action == "editpolemize")
                        //{
                        //    PolemizeThreadV5 polemize;
                        //    int total;
                        //    PostBOV5.Instance.GetPolemizeWithReplies(ThreadID, null, 1, 1, true, false, out polemize, out posts, out realType, out total);
                        //    if (realType != ThreadType.Polemize)
                        //        isWrongType = true;
                        //    else
                        //        Thread = (BasicThread)polemize;
                        //}
                        //else //if (Action == "editpoll")
                        //{
                        //    PollThreadV5 poll;
                        //    PostBOV5.Instance.GetPollWithReplies(ThreadID, 1, 1, true, false, out poll, out posts, out realType);
                        //    if (realType != ThreadType.Poll)
                        //        isWrongType = true;
                        //    else
                        //        Thread = (BasicThread)poll;
                        //}

                        //if (isWrongType)
                        //{
                        //    switch (realType)
                        //    {
                        //        case ThreadType.Normal:
                        //            PostBOV5.Instance.GetThreadWithReplies(ThreadID, 1, 1, true, false, true, out Thread, out posts, out realType);
                        //            break;
                        //        case ThreadType.Poll:
                        //            PollThreadV5 poll;
                        //            PostBOV5.Instance.GetPollWithReplies(ThreadID, 1, 1, true, false, out poll, out posts, out realType);
                        //            Thread = (BasicThread)poll;
                        //            break;
                        //        case ThreadType.Question:
                        //            QuestionThread question;
                        //            PostBOV5.Instance.GetQuestionWithReplies(ThreadID, 1, 1, true, false, out question, out posts, out realType);
                        //            Thread = (BasicThread)question;
                        //            break;
                        //        case ThreadType.Polemize:
                        //            int total;
                        //            PolemizeThreadV5 polemize;
                        //            PostBOV5.Instance.GetPolemizeWithReplies(ThreadID, null, 1, 1, true, false, out polemize, out posts, out realType, out total);
                        //            Thread = (BasicThread)polemize;
                        //            break;
                        //        default: break;
                        //    }
                        //}
                    //}
                    //else
                    //{
                    //    realType = ThreadType.Normal;
                    //    PostBOV5.Instance.GetPosts(ThreadID, false, 1, 1, null, false, true, true, false, ref Thread, out posts, ref realType);
                        
                    //}
                    m_Action = "editthread";

                    Thread = PostBOV5.Instance.GetThread(ThreadID);
                    if (Thread == null)
                        ShowError("您要编辑的主题不存在或已被删除");
                    if (Thread.ThreadContent != null)
                        CurrentPost = Thread.ThreadContent;
                    else
                        CurrentPost = PostBOV5.Instance.GetPost(Thread.ContentID, true);

                    ThreadType realType = Thread.ThreadType;

                    //if(posts!= null && posts.Count>0)
                    //{
                    //    CurrentPost = posts[0];
                    //}

                    if (Thread == null || CurrentPost == null)
                    {
                        ShowError("您要编辑的主题不存在或已被删除", Request.RawUrl, false);
                    }

                    if (Thread.ForumID != ForumID)
                    {
                        ShowError("非法操作");
                    }

                    //如果没有管理权限,则进入
                    if (false == CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads, Thread.PostUserID))
                    {
                        //如果是自己的帖子,则继续进入
                        if (Thread.PostUserID == MyUserID)
                        {
                            //如果自己发表的帖子自己并没有管理权限,则提示没有权限管理自己的帖子
                            if (false == Can(ForumPermissionSetNode.Action.UpdateOwnThread))
                                ShowError("您所在的用户组没有权限编辑自己的主题", BbsUrlHelper.GetForumUrl(Forum.CodeName), false);
                        }
                        //如果不是自己的帖子,直接提示没有权限管理其他用户的帖子
                        else
                            ShowError("您所在的用户组没有权限编辑主题", BbsUrlHelper.GetForumUrl(Forum.CodeName), false);
                    }

                    if (Thread.PostUserID == MyUserID && ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads) == false)//自己的帖子
                    {
                        int intervals = ForumSetting.UpdateOwnPostIntervals[My];
                        if (intervals != 0 && Thread.CreateDate < DateTimeUtil.Now.AddSeconds(0 - intervals))
                        {
                            ShowError(new OverUpdatePostIntervalsError(intervals).Message);
                        }
                    }

                    PostBOV5.Instance.ProcessKeyword(Thread, ProcessKeywordMode.FillOriginalText);
                    PostBOV5.Instance.ProcessKeyword(CurrentPost, ProcessKeywordMode.FillOriginalText);


                    switch (realType)
                    {
                        case ThreadType.Normal:
                            break;
                        case ThreadType.Poll:
                            IsShowPollOptions = true;
                            IsShowPollExpiresDate = true;
                            break;
                        case ThreadType.Question:
                            IsShowQuestionOptions = true;
                            RewardTax = AllSettings.Current.PointSettings.TradeRate / 100.0;//setting.TradingTax / 100.0;
                            RealReward = (int)Math.Ceiling(((double)Question.Reward * (100.0 + AllSettings.Current.PointSettings.TradeRate) / 100.0));
                            break;
                        case ThreadType.Polemize:
                            IsShowPolemizeOptions = true; 
                            break;
                        default: break;
                    }
                    break;
                case "editpost":
                    ActionName = "编辑回复";
                    CurrentPost = PostBOV5.Instance.GetPost(PostID, true);
                    if (CurrentPost == null)
                        ShowError("你要编辑的帖子不存在或者已被删除");

                    Thread = PostBOV5.Instance.GetThread(ThreadID);
                    IsShowThreadCatalog = false;

                    if (CurrentPost.ForumID != ForumID)
                    {
                        ShowError("非法操作");
                    }

                    //如果没有管理权限,则进入
                    if (false == CanManage(ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts, CurrentPost.UserID))
                    {
                        //如果是自己的帖子,则继续进入
                        if (CurrentPost.UserID == MyUserID)
                        {
                            //如果自己发表的帖子自己并没有管理权限,则提示没有权限管理自己的帖子
                            if (false == Can(ForumPermissionSetNode.Action.UpdateOwnPost))
                                ShowError("您所在的用户组没有编辑自己帖子的权限", BbsUrlHelper.GetForumUrl(Forum.CodeName),false);

                        }
                        //如果不是自己的帖子,直接提示没有权限管理其他用户的帖子
                        else
                            ShowError("您所在的用户组没有编辑帖子的权限", BbsUrlHelper.GetForumUrl(Forum.CodeName),false);
                    }
                    if (CurrentPost.UserID == MyUserID && ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts) == false)//自己的帖子
                    {
                        int intervals = ForumSetting.UpdateOwnPostIntervals[My];
                        if (intervals != 0 && CurrentPost.CreateDate < DateTimeUtil.Now.AddSeconds(0 - intervals))
                        {
                            ShowError(new OverUpdatePostIntervalsError(intervals).Message);
                        }
                    }

                    PostBOV5.Instance.ProcessKeyword(CurrentPost, ProcessKeywordMode.FillOriginalText);
                    PostBOV5.Instance.ProcessKeyword(Thread, ProcessKeywordMode.TryUpdateKeyword);

                    break;
                default:
                    //DisabledItem(false, null);
                    if (Action.IndexOf("requote") > -1)//引用
                    {
                        if (!Can(ForumPermissionSetNode.Action.ReplyThread))
                        {
                            if (IsLogin == false)
                                ShowError("您是游客，没有发表回复的权限", Request.RawUrl, true);
                            else
                                ShowError("您所在的用户组没有发表回复的权限", Request.RawUrl, false);

                        }
                        //DisabledItem(false, null);
                        Thread = PostBOV5.Instance.GetThread(ThreadID);
                        if (Thread == null)
                            ShowError("您所要引用的帖子不存在或已被删除");

                        if (Thread.ForumID != ForumID)
                            ShowError("非法操作");

                        AddNavigationItem(Thread.SubjectText, BbsUrlHelper.GetThreadUrl(Forum.CodeName, ThreadID, 1));

                        IsShowThreadCatalog = false;
                        ActionName = "发表回复";
                        //defaultSubject = "Re:" + thread.Subject;

                        CurrentPost = PostBOV5.Instance.GetPost(PostID, false);
                        if (CurrentPost == null)//不存在
                        {
                            ShowError("您所要引用的帖子不存在或已被删除");
                        }
                        else if (CurrentPost.PostType == PostType.ThreadContent)
                        {
                            if (!Can(ForumPermissionSetNode.Action.ViewThread))
                            {
                                ShowError("您所在的用户组没有权限查看主题内容，不能引用该内容");
                            }
                            else if (Thread.IsValued && !Can(ForumPermissionSetNode.Action.ViewValuedThread))
                            {
                                ShowError("您所在的用户组没有权限查看精华主题内容，不能引用该内容");
                            }
                        }

                        string content;
                        if (Forum.IsShieldedUser(CurrentPost.UserID))
                            content = "该用户已被屏蔽";
                        else if (CurrentPost.IsShielded)
                            content = "该帖已被屏蔽";
                        //else if (!Can(ForumPermissionSetNode.Action.DisplayContent))//(!Forum.GetForumPermission(reply.UserID).ContentCanDisplay.Value)
                        //    content = "该用户发言被屏蔽";
                        else if (!CurrentPost.EnableHTML && !CurrentPost.EnableMaxCode3)//(!Forum.ForumPermission.Visible.Access.Create.Format.CanUseHtml&&!Forum.ForumPermission.Visible.Access.Create.Format.CanUseUbb)//(!Forum.Permission.PostFormat.AllowHTML && !Forum.Permission.PostFormat.AllowMaxcode)
                        {
                            content = CurrentPost.Content;
                        }
                        else
                        {
                            content = PostUbbParserV5.ProcessQuote(CurrentPost.Content);
                            content = PostUbbParserV5.FormatRequotePost(content, CurrentPost, Thread);

                        }

                        string replyNickName;
                        if (CurrentPost.UserID == 0)
                        {
                            if (CurrentPost.Username.Trim() == "")
                                replyNickName = "游客";
                            else
                                replyNickName = "游客 <u>" + CurrentPost.Username + "</u>";
                        }
                        else
                        {
                            replyNickName = "会员 <u>" + CurrentPost.Username + "</u>";
                        }

                        if (AllowHtml == false && AllowMaxcode == false)
                        {
                            Content = "[quote]" + StringUtil.ClearAngleBracket(new UbbParser().QuoteToUbb(content)) + "[/quote]";
                        }
                        else
                        {
                            Content = "[quote]<br />" + new UbbParser().QuoteToUbb(content) + "<div style=\"text-align:right;\">-- by " + replyNickName + " (" + CurrentPost.CreateDate + ")</div>[/quote]";

                            if (ContentFormatValue == "enableMaxCode")
                            {
                                Content = HtmlToUbbParser.Html2Ubb(CurrentPost.UserID, Content, false);
                            }
                        }
                    }
                    else
                    {
                    }
                    break;
            }

            if (IsReply)
            {
                if (Thread.IsLocked)
                    ShowError("该主题已被锁定，不能再进行回复操作");
            }

            if ((Action == "editthread" || Action == "editpost") && CurrentPost != null)
            {
                //DisabledItem(true, currentPost);
                if (Type == "")
                    AddNavigationItem(Thread.SubjectText, BbsUrlHelper.GetThreadUrl(Forum.CodeName, Thread.ThreadID, Thread.ThreadTypeString, 1));
                else
                    AddNavigationItem(Thread.SubjectText, BbsUrlHelper.GetThreadUrl(Forum.CodeName, Thread.ThreadID, 1, Thread.ThreadTypeString, Type));

                if (Action == "editpost")
                    IsShowThreadCatalog = false;
                else
                    SelectCatalogID = Thread.ThreadCatalogID;
                CheckedIconID = CurrentPost.IconID;
                Subject = CurrentPost.OriginalSubject;

                Content = CurrentPost.OriginalContent;

                Content = PostUbbParserV5.ParseWhenEdit(CurrentPost.UserID, Content, CurrentPost.EnableMaxCode, CurrentPost.EnableMaxCode3, CurrentPost.EnableHTML, AllowHtml, AllowMaxcode);

                if (Action == "editthread")
                    SelectCatalogID = Thread.ThreadCatalogID;//PostManager.GetThread(currentPost.ThreadID).ThreadCatalogID;
                //checkItem(currentPost);
                //postID = currentPost.PostID;
            }

            if (Action == "question")
            {
                getTime(ForumSetting.QuestionValidDays[My], out Time, out TimeUnit);
            }
            else if (Action == "polemize")
            {
                getTime(ForumSetting.PolemizeValidDays[My], out Time, out TimeUnit);
            }
            else if (Action == "poll")
            {
                getTime(ForumSetting.PollValidDays[My], out Time, out TimeUnit);
            }

            //SetPageTitle(string.Concat("发表新帖子"));

            AddNavigationItem(ActionName);
        }


        protected override string PageTitle
        {
            get
            {
                if (IsReply)
                    return string.Concat("发表回复", "-", base.PageTitle);
                else if (IsEditThread)
                    return string.Concat("编辑主题", "-", base.PageTitle);
                else if (IsEditPost)
                    return string.Concat("编辑回复", "-", base.PageTitle);
                else if (IsCreateThread)
                    return string.Concat("发表主题", "-", base.PageTitle);
                else
                    return base.PageTitle;
            }
        }


        protected int CurrentPostUserID
        {
            get
            {
                if (CurrentPost == null)
                    return 0;
                else
                    return CurrentPost.UserID;
            }
        }



        private string m_Action;
        protected string Action
        {
            get
            {
                if (m_Action == null)
                {
                    m_Action = _Request.Get("action", Method.Get, string.Empty).ToLower();
                }
                return m_Action;
            }
        }

        private string m_Type;
        protected string Type
        {
            get
            {
                if (m_Type == null)
                {
                    m_Type = _Request.Get("type", Method.Get, "");
                }
                return m_Type;
            }
        }

        private bool? m_IsShowNoUpdateSortOrder;
        protected bool IsShowNoUpdateSortOrder
        {
            get
            {
                if (m_IsShowNoUpdateSortOrder == null)
                    m_IsShowNoUpdateSortOrder = IsReply && (Thread.IsOverUpdateSortOrderTime(ForumSetting) == true);
                return m_IsShowNoUpdateSortOrder.Value;
            }
        }



        protected string SellAttachmentDaysInfo
        {
            get
            {
                if (SellAttachmentDays > 0)
                {
                    return DateTimeUtil.FormatSecond(SellAttachmentDays);
                }
                else
                    return string.Empty;
            }
        }

        protected string SellThreadDaysInfo
        {
            get
            {
                if (SellThreadDays > 0)
                    return DateTimeUtil.FormatSecond(SellThreadDays);
                else
                    return string.Empty;
            }
        }

        #region Review
        private bool m_Review = false;
        protected bool Review
        {
            get 
            {
                return m_Review;
            }
        }
        protected string reviewContent;
        protected string reviewSubject;
        public void ProcessReview()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int postUserID;

            bool isV50 = true;
            if (IsEditPost || IsEditThread)
            {
                PostV5 post = PostBOV5.Instance.GetPost(PostID, false); //PostManager.GetPost(PostID).UserID;
                postUserID = post.UserID;
                isV50 = post.IsV5_0;
            }
            else
            {
                postUserID = MyUserID;
            }


            m_Review = true;
            reviewContent = _Request.Get("editor_content", Method.Post, string.Empty, false);

            reviewSubject = ClearHTML(_Request.Get("subject", Method.Post, string.Empty));

            if ((IsEditThread || IsCreateThread) && reviewSubject.Trim() == "")
            {
                msgDisplay.AddError("标题不能为空");
                //ShowAlert("标题不能为空！");
            }

            bool enableHTML = false, enableMaxCode3 = false;
            if (AllowHtml && AllowMaxcode)
            {
                enableHTML = _Request.Get("contentFormat", Method.Post, "").ToLower() == "enablehtml";
                if (enableHTML == false)
                    enableMaxCode3 = true;
            }
            else if (AllowHtml)
                enableHTML = true;
            else if (AllowMaxcode)
                enableMaxCode3 = true;

            bool enableEmoticon = AllowEmoticon && _Request.Get("enableItem", Method.Post, "").ToLower().IndexOf("enableemoticons") > -1;

            AttachmentCollection attachments = new AttachmentCollection();


            //string attachIdsText = _Request.Get("attachIds", Method.Post, string.Empty, false);
            //if (string.IsNullOrEmpty(attachIdsText) == false)
            //{
            //    int[] attachIds = StringUtil.Split<int>(attachIdsText, ',');


            //    GetAttachments(attachIds, "0", postUserID, msgDisplay, ref attachments);
            //}

            //string diskIdsText = _Request.Get("diskFileIDs", Method.Post, string.Empty, false);
            //if (string.IsNullOrEmpty(diskIdsText) == false)
            //{
            //    int[] diskFileIDs = StringUtil.Split<int>(diskIdsText, ',');

            //    GetAttachments(diskFileIDs, "1", postUserID, msgDisplay, ref attachments);
            //}

            GetAttachments(postUserID, msgDisplay, ref attachments);

            reviewContent = PostUbbParserV5.ParseWhenSave(postUserID, enableEmoticon, Forum.ForumID, reviewContent, enableHTML, enableMaxCode3, attachments);
            reviewContent = PostUbbParserV5.ParsePreviewLocalAttachTag(Forum.ForumID, UserBO.Instance.GetUser(postUserID,GetUserOption.WithAll), reviewContent, attachments);

            MatchCollection ms = PostUbbParserV5.regex_AllAttach.Matches(reviewContent);
            List<int> historyAttachmentIDs = new List<int>();

            foreach (Match m in ms)
            {
                bool isHistoryAttach = true;
                int attachID = int.Parse(m.Groups["id"].Value);
                foreach (Attachment attach in attachments)
                {
                    if (attachID == attach.AttachmentID)
                    {
                        isHistoryAttach = false;
                        break;
                    }
                }
                if (isHistoryAttach)
                    historyAttachmentIDs.Add(attachID);
            }

            AttachmentCollection historyAttachs = PostBOV5.Instance.GetAttachments(MyUserID, historyAttachmentIDs);

            foreach (Attachment attach in historyAttachs)
            {
                attachments.Add(attach);
            }

            reviewContent = PostUbbParserV5.ParseWhenDisplay(postUserID, Forum.ForumID, PostID, reviewContent, enableHTML, false, isV50, attachments);


            if (reviewContent.Trim() == "")
            {
                msgDisplay.AddError("内容不能为空");
            }

            if (msgDisplay.HasAnyError())
            {
                m_Review = false;
            }
        }
        #endregion
        
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


        protected int PostIconID
        {
            get
            {
                if (CurrentPost != null)
                {
                    if (IsEditPost || IsEditThread)
                        return CurrentPost.IconID;
                    else
                        return 0;
                }
                return 0;
            }
        }


        private string m_DefaultTextMode;
        protected string DefaultTextMode
        {
            get
            {
                if (m_DefaultTextMode == null)
                {
                    if (Action.IndexOf("requote") > -1)//引用
                        m_DefaultTextMode = "ubb";
                    else
                    {
                        if (CurrentPost != null)
                        {
                            if (CurrentPost.EnableMaxCode3 || CurrentPost.EnableMaxCode)
                                m_DefaultTextMode = "ubb";
                            else
                                m_DefaultTextMode = "html";
                        }
                        else
                            m_DefaultTextMode = "ubb";
                    }
                }
                return m_DefaultTextMode;
            }
        }

        private string m_ContentFormatValue;
        protected string ContentFormatValue
        {
            get
            {
                if (m_ContentFormatValue == null)
                {
                    if (DefaultTextMode == "ubb")
                    {
                        if (AllowMaxcode)
                            m_ContentFormatValue = "enableMaxCode";
                        else if (AllowHtml)
                            m_ContentFormatValue = "enableHtml";
                        else
                            m_ContentFormatValue = string.Empty;
                    }
                    else
                    {
                        if (AllowHtml)
                            m_ContentFormatValue = "enableHtml";
                        else if (AllowMaxcode)
                            m_ContentFormatValue = "enableMaxCode";
                        else
                            m_ContentFormatValue = string.Empty;
                    }
                }

                return m_ContentFormatValue;
            }
        }

        #region 自动保存功能的一些变量
        protected UserTempDataType TempDataType
        {
            get
            {
                switch (Action)
                {
                    case "thread":
                    case "editthread":
                        return UserTempDataType.ThreadContent;

                    case "editpost":
                    case "reply":
                        return UserTempDataType.PostContent;

                    default:
                        return UserTempDataType.None;
                }
            }
        }
        private UserTempData m_TempPostData;
        protected UserTempData TempPostData
        {
            get
            {
                if (this.CanAutoSave)
                {
                    if (TempDataType!= UserTempDataType.None)
                        m_TempPostData = UserTempDataBO.Instance.GetTempData(MyUserID, TempDataType);
                }
                return m_TempPostData;
            }
        }
        protected bool CanAutoSave
        {
            get
            {
                if (IsReply || IsEditPost)
                    return false;
                return true;
            }
        }
        #endregion

        private void getTime(long seconds, out int timeValue, out string timeUnit)
        {
            int days = (int)seconds / (60 * 60 * 24);
            int hours = (int)seconds / (60 * 60);
            int minutes = (int)seconds / 60;
            if (days > 0)
            {
                timeValue = days;
                timeUnit = "天";
            }
            else if (hours > 0)
            {
                timeValue = hours;
                timeUnit = "小时";
            }
            else if (minutes > 0)
            {
                timeValue = minutes;
                timeUnit = "分钟";
            }
            else
            {
                timeValue = (int)seconds;
                timeUnit = "秒";
            }

        }

        private string m_PollItemString;
        protected string PollItemString
        {
            get
            {
                if (m_PollItemString == null)
                {
                    if (Poll == null)
                        m_PollItemString = string.Empty;
                    else
                    {
                        System.Text.StringBuilder pollItems = new System.Text.StringBuilder();
                        foreach (PollItem pollItem in Poll.PollItems)
                            pollItems.Append(pollItem.ItemNameForEdit + "\r\n");

                        m_PollItemString = pollItems.ToString();
                    }
                }
                return m_PollItemString;
            }
        }

        private int? m_PollMultiple;
        protected int PollMultiple
        {
            get
            {
                if (m_PollMultiple == null)
                {
                    if (Poll != null)
                        m_PollMultiple = Poll.Multiple;
                    else
                        m_PollMultiple = BbsSettings.MaxPollItemCount;
                }

                return m_PollMultiple.Value;
            }
        }

        private PollThreadV5 m_Poll;
        protected PollThreadV5 Poll
        {
            get
            {
                if (m_Poll == null)
                {
                    if (Thread != null && Thread is PollThreadV5)
                        m_Poll = (PollThreadV5)Thread;
                }
                return m_Poll;
            }
        }
        private QuestionThread m_Question;
        protected QuestionThread Question
        {
            get
            {
                if (m_Question == null)
                {
                    if (Thread != null && Thread is QuestionThread)
                        m_Question = (QuestionThread)Thread;
                }
                return m_Question;
            }
        }

        private PolemizeThreadV5 m_Polemize;
        protected PolemizeThreadV5 Polemize
        {
            get
            {
                if (m_Polemize == null)
                {
                    if (Thread != null && Thread is PolemizeThreadV5)
                        m_Polemize = (PolemizeThreadV5)Thread;
                }
                return m_Polemize;
            }
        }




        private Dictionary<int, PostIcon>.ValueCollection m_PostIcons;
        protected Dictionary<int, PostIcon>.ValueCollection PostIcons
        {
            get
            {
                if (m_PostIcons == null)
                {
                    if (EnablePostIcon)
                        m_PostIcons = PostBOV5.Instance.GetAllPostIcons().Values;
                    else
                        return null;
                }
                return m_PostIcons;
            }
        }

#region  积分
        private UserPoint m_SellAttachmentPoint;
        protected UserPoint SellAttachmentPoint
        {
            get
            {
                if (m_SellAttachmentPoint == null)
                {
                    GetSellAttachmentPointSetting();
                }
                return m_SellAttachmentPoint;
            }
        }

        private int? m_SellAttachmentPointMaxValue;
        private int? m_SellAttachmentPointMinValue;
        protected string SellAttachmentPointScope
        {
            get
            {
                if (m_SellAttachmentPointMinValue == null)
                {
                    GetSellAttachmentPointSetting();
                }

                if (m_SellAttachmentPointMaxValue == null)
                    return "附件价格允许范围(" + m_SellAttachmentPointMinValue.Value + " 到 无限大)";
                else
                    return "附件价格允许范围(" + m_SellAttachmentPointMinValue.Value + " 到 " + m_SellAttachmentPointMaxValue.Value + ")";
            }
        }
        private void GetQuestionRewardSetting()
        {
            int? maxValue, minRemain;
            int minValue;
            m_QuestionRewardPoint = ForumPointAction.Instance.GetUserPoint(MyUserID, ForumPointValueType.QuestionReward, Forum.ForumID, out minRemain, out minValue, out maxValue);
            m_MaxValue = maxValue;
            m_MinValue = minValue;
        }

        private UserPoint m_QuestionRewardPoint;
        protected UserPoint QuestionRewardPoint
        {
            get
            {
                if (m_QuestionRewardPoint == null)
                {
                    GetQuestionRewardSetting();
                }
                return m_QuestionRewardPoint;
            }
        }
        private int? m_MaxValue;
        private int? m_MinValue;
        protected string QuestionRewardScope
        {
            get
            {
                if (m_MinValue == null)
                {
                    GetQuestionRewardSetting();
                }

                if (m_MaxValue == null)
                    return "允许奖励范围(" + m_MinValue.Value + " 到 无限大)";
                else
                    return "允许奖励范围(" + m_MinValue.Value + " 到 " + m_MaxValue.Value + ")";
            }
        }



        private void GetSellPostPointSetting()
        {
            int? maxValue, minRemain;
            int minValue;
            m_SellPostPoint = ForumPointAction.Instance.GetUserPoint(MyUserID, ForumPointValueType.SellThread, Forum.ForumID, out minRemain, out minValue, out maxValue);
            m_SellPostPointMaxValue = maxValue;
            m_SellPostPointMinValue = minValue;
        }

        private UserPoint m_SellPostPoint;
        protected UserPoint SellPostPoint
        {
            get
            {
                if (m_SellPostPoint == null)
                {
                    GetSellPostPointSetting();
                }
                return m_SellPostPoint;
            }
        }

        private int? m_SellPostPointMaxValue;
        private int? m_SellPostPointMinValue;
        protected string SellPostPointScope
        {
            get
            {
                if (m_SellPostPointMinValue == null)
                {
                    GetSellPostPointSetting();
                }

                if (m_SellPostPointMaxValue == null)
                    return "价格允许范围(" + m_SellPostPointMinValue.Value + " 到 无限大)";
                else
                    return "价格允许范围(" + m_SellPostPointMinValue.Value + " 到 " + m_SellPostPointMaxValue.Value + ")";
            }
        }

        private void GetSellAttachmentPointSetting()
        {
            int? maxValue, minRemain;
            int minValue;
            m_SellAttachmentPoint = ForumPointAction.Instance.GetUserPoint(MyUserID, ForumPointValueType.SellAttachment, Forum.ForumID, out minRemain, out minValue, out maxValue);
            m_SellAttachmentPointMaxValue = maxValue;
            m_SellAttachmentPointMinValue = minValue;
        }

#endregion


        private bool? m_CanUseNetDisk;
        protected bool CanUseNetDisk
        {
            get
            {
                if (m_CanUseNetDisk == null)
                    m_CanUseNetDisk = DiskBO.Instance.CanUseNetDisk(MyUserID);

                return m_CanUseNetDisk.Value;
            }
        }

        protected int MaxPollItemCount
        {
            get
            {
                return BbsSettings.MaxPollItemCount;
            }
        }

        protected bool IsShowSellThread
        {
            get
            {
                if (IsShowPolemizeOptions || IsShowPollOptions || IsShowQuestionOptions || IsEditPost || CanSellThread == false || IsReply)
                    return false;
                else
                    return true;
            }
        }

        protected bool IsReply
        {
            get
            {
                if (Action == "reply" || Action == "requote")
                {
                    return true;
                }
                else
                    return false;
            }
        }
        protected bool IsCreateThread
        {
            get
            {
                if (Action == "question" || Action == "thread" || Action == "polemize" || Action == "poll")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected bool IsEditThread
        {
            get
            {
                if (Action == "editthread")
                    return true;
                else
                    return false;
            }
        }

        protected bool IsEditPost
        {
            get
            {
                if (Action == "editpost")
                    return true;
                else
                    return false;
            }
        }

        private AttachmentCollection m_AttachList = null;
        protected AttachmentCollection AttachList
        {
            get
            {
                if (m_AttachList == null)
                {
                    if (IsLogin == false)//游客
                    {
                        m_AttachList = new AttachmentCollection();
                    }
                    else
                    {
                        if (CurrentPost != null)
                        {
                            if (CurrentPost.Attachments != null)
                            {
                                m_AttachList = new AttachmentCollection();
                                foreach (Attachment attach in CurrentPost.Attachments)
                                {
                                    if (attach.AttachType ==  AttachType.History)
                                        continue;
                                    m_AttachList.Add(attach);
                                }
                            }
                            else
                                m_AttachList = PostBOV5.Instance.GetAttachments(CurrentPost.PostID);

                        }

                        if (m_AttachList == null)
                            m_AttachList = new AttachmentCollection();
                        foreach (TempUploadFile file in FileManager.GetUserTempUploadFiles(MyUserID, "attach", SearchInfo))
                        {
                            Attachment attach = new Attachment();
                            attach.AttachmentID = 0 - file.TempUploadFileID;
                            attach.FileName = file.FileName;
                            attach.FileSize = file.FileSize;
                            attach.AttachType = AttachType.TempAttach;
                            m_AttachList.Add(attach);
                        }
                    }
                }
                return m_AttachList;
            }
        }

        private string M_SearchInfo;
        protected string SearchInfo
        {
            get
            {
                if (M_SearchInfo == null)
                {
                    if (IsEditPost || IsEditThread)
                        M_SearchInfo = "edit";
                    else
                        M_SearchInfo = "create";
                }
                return M_SearchInfo;
            }
        }

        private bool? m_CanSellThread;
        protected bool CanSellThread
        {
            get
            {
                if (m_CanSellThread == null)
                    m_CanSellThread = ForumSetting.EnableSellThread[My];
                return m_CanSellThread.Value;
            }
        }

        private bool? m_HasEditPermission;
        protected bool HasEditPermission
        {
            get
            {
                if (m_HasEditPermission == null)
                {
                    if (IsEditPost)
                        m_HasEditPermission = ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdatePosts);
                    else if (IsEditThread)
                        m_HasEditPermission = ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreads);
                    else
                        m_HasEditPermission = false;
                }
                return m_HasEditPermission.Value;
            }
        }

        protected bool CanSellAttachment
        {
            get
            {
                return ForumSetting.EnableSellAttachment[My];
            }
        }

        private bool? m_CanLockThread;
        protected bool CanLockThread
        {
            get
            {
                if (m_CanLockThread == null)
                {
                    int targetUserID;
                    if (CurrentPost != null)
                        targetUserID = CurrentPost.UserID;
                    else
                        targetUserID = MyUserID;

                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock, targetUserID))
                        m_CanLockThread = true;
                    else
                        m_CanLockThread = false;
                }
                return m_CanLockThread.Value;
            }
        }

        private bool? m_CanStickyThread;
        protected bool CanStickyThread
        {
            get
            {
                if (m_CanStickyThread == null)
                {
                    int targetUserID;
                    if (CurrentPost != null)
                        targetUserID = CurrentPost.UserID;
                    else
                        targetUserID = MyUserID;

                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick, targetUserID))
                        m_CanStickyThread = true;
                    else
                        m_CanStickyThread = false;
                }
                return m_CanStickyThread.Value;
            }
        }

        private bool? m_CanGlobalStickyThread;
        protected bool CanGlobalStickyThread
        {
            get
            {
                if (m_CanGlobalStickyThread == null)
                {
                    int targetUserID;
                    if (CurrentPost != null)
                        targetUserID = CurrentPost.UserID;
                    else
                        targetUserID = MyUserID;

                    if (CanManage(ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick, targetUserID))
                        m_CanGlobalStickyThread = true;
                    else
                        m_CanGlobalStickyThread = false;
                }
                return m_CanGlobalStickyThread.Value;
            }
        }


        private ExtensionList m_AllowFileExtList;
        /// <summary>
        /// 允许的附件扩展名
        /// </summary>
        private ExtensionList AllowFileExtList
        {
            get
            {
                if (m_AllowFileExtList == null)
                {
                    m_AllowFileExtList = ForumSetting.AllowFileExtensions[My];
                }
                return m_AllowFileExtList;
            }
        }

        private string m_AllowFileType;
        /// <summary>
        /// 允许上传的文件格式
        /// </summary>
        protected string AllowFileType
        {
            get
            {
                if (m_AllowFileType == null)
                {
                    string stringExtList = string.Empty;
                    foreach (string ext in AllowFileExtList)
                    {
                        if (ext == "*.*")
                        {
                            m_AllowFileType = ext;
                            break;
                        }
                        stringExtList += "*." + ext + ";";
                    }
                    if (m_AllowFileType == null)
                    {
                        if (!string.IsNullOrEmpty(stringExtList))
                        {
                            stringExtList = stringExtList.Substring(0, stringExtList.Length - 1);
                        }
                        m_AllowFileType = stringExtList;
                    }
                }

                return m_AllowFileType;
            }
        }


        private long? M_SingleFileSize;
        /// <summary>
        /// 上传一个文件最大大小
        /// </summary>
        protected long SingleFileSize
        {
            get
            {
                if (M_SingleFileSize == null)
                {
                    M_SingleFileSize = ForumSetting.MaxSignleAttachmentSize[My];
                    if (M_SingleFileSize.Value == 0)
                        M_SingleFileSize = long.MaxValue;
                }
                return M_SingleFileSize.Value;
            }
        }

        private string m_SingleFileSizeString;
        protected string SingleFileSizeString
        {
            get
            {
                if (m_SingleFileSizeString == null)
                    m_SingleFileSizeString = CommonUtil.FormatSizeForSwfUpload(SingleFileSize);
                return m_SingleFileSizeString;
            }
        }

        private int? m_UsedAttachmentCount;
        protected int UsedAttachmentCount
        {
            get
            {
                if (m_UsedAttachmentCount == null)
                {
                    int count;
                    long fileSize;

                    int? excludePostID = null;
                    if (CurrentPost != null)
                        excludePostID = CurrentPost.PostID;

                    PostBOV5.Instance.GetUserTodayAttachmentInfo(MyUserID, excludePostID, out count, out fileSize);

                    m_UsedAttachmentCount = count;
                }
                return m_UsedAttachmentCount.Value;
            }
        }

        private int? m_MaxAttachmentCountInDay;
        protected int MaxAttachmentCountInDay
        {
            get
            {
                if (m_MaxAttachmentCountInDay == null)
                    m_MaxAttachmentCountInDay = AllSettings.Current.BbsSettings.MaxAttachmentCountInDay[My];
                return m_MaxAttachmentCountInDay.Value;
            }
        }

        private int? m_MaxPostAttachmentCount;
        protected int MaxPostAttachmentCount
        {
            get
            {
                if (m_MaxPostAttachmentCount == null)
                {
                    if (IsCreateThread || IsEditThread)
                    {
                        m_MaxPostAttachmentCount = ForumSetting.MaxTopicAttachmentCount[My];
                    }
                    else
                    {
                        m_MaxPostAttachmentCount = ForumSetting.MaxPostAttachmentCount[My];
                    }
                }
                return m_MaxPostAttachmentCount.Value;
            }
        }

        #region 以下是编辑器功能开关
         
        private bool? m_allowVideo;
        protected bool AllowVideo
        {
            get
            {
                if (m_allowVideo == null)
                    m_allowVideo = ForumSetting.CreatePostAllowVideoTag.GetValue(My);
                return m_allowVideo.Value;
            }
        }

        private bool? m_AllowEmoticon;
        protected bool AllowEmoticon
        {
            get
            {
                if (m_AllowEmoticon == null)
                    m_AllowEmoticon = ForumSetting.CreatePostAllowEmoticon.GetValue(My);
                return m_AllowEmoticon.Value;
            }
        }

        private bool? m_AllowImage;
        protected bool AllowImage
        {
            get
            {
                if (m_AllowImage == null)
                    m_AllowImage = ForumSetting.CreatePostAllowImageTag.GetValue(My);

                return m_AllowImage.Value;
            }
        }

        private bool? m_AllowUrl;
        protected bool AllowUrl
        {
            get
            {
                if (m_AllowUrl==null)
                    m_AllowUrl = ForumSetting.CreatePostAllowUrlTag.GetValue(My);
                
                return m_AllowUrl.Value;
            }
        }

        private bool? m_allowTable;
        protected bool AllowTable
        {
            get
            {
                if (m_allowTable == null)
                    m_allowTable = ForumSetting.CreatePostAllowTableTag.GetValue(My);
                return m_allowTable.Value;
            }
        }
        

        private bool? m_allowAudio;
        protected bool AllowAudio
        {
            get
            {
                if (m_allowAudio == null)
                    m_allowAudio = ForumSetting.CreatePostAllowAudioTag.GetValue(My);

                return m_allowAudio.Value;
            }
        }

        private bool? m_AllowFlash = null;
        protected bool AllowFlash
        {
            get
            {
                if (m_AllowFlash == null)
                    m_AllowFlash = ForumSetting.CreatePostAllowFlashTag.GetValue(My);

                return m_AllowFlash.Value;
            }
        }

        private bool? m_AllowAttachment;
        protected bool AllowAttachment
        {
            get
            {
                if (m_AllowAttachment == null)
                {
                    if (IsLogin == false)
                        m_AllowAttachment = false;
                    else
                        m_AllowAttachment = ForumSetting.AllowAttachment.GetValue(My);
                }
                return m_AllowAttachment.Value;
            }
        }

        protected bool WyswygMode
        {
            get
            {

                return AllSettings.Current.BbsSettings.DefaultTextMode;
            }
        }

        protected bool UseMaxCode
        {
            get
            {
                return ContentFormatValue == "enableMaxCode";
            }
        }

        #endregion

        protected string editorOptions
        {
            get
            {
                if (ForumSetting.EnableHiddenTag)
                    return "hide,";
                return "";
            }
        }


        private void GetAttachments(int postUserID, MessageDisplay msgDisplay, ref AttachmentCollection attachs)
        {
            AttachmentCollection attachments = new AttachmentCollection();
            DiskFileCollection diskFiles = null;

            List<int> tempFileIds = new List<int>(), diskFileIds=new List<int>();

            string[] attachIndexs = _Request.Get("attachIndex", Method.Post,string.Empty).Split( new string[]{ ","}, StringSplitOptions.RemoveEmptyEntries);


            foreach (string i in attachIndexs)
            {
                if (i == "{index}")
                    continue;
                int id = _Request.Get<int>("attachid_" + i, Method.Post, 0);
                int attachType=_Request.Get<int>("attachtype_" + i, Method.Post, 0);
                if ( attachType == 0)
                    tempFileIds.Add(id);
                else if( attachType==1 )
                    diskFileIds.Add(id);
            }

           diskFiles = DiskBO.Instance.GetDiskFiles(diskFileIds);
            
                Attachment attach;

                string extendName = string.Empty;
                foreach (DiskFile file in diskFiles)
                {
                    attach = new Attachment();
                    attach.DiskFileID = file.DiskFileID;
                    attach.FileID = file.FileID;
                    attach.FileSize = file.Size;
                    attach.FileName = _Request.Get("filename_1_" + file.DiskFileID, Method.Post, "未命名", false);
                    attach.Price = _Request.Get("price_1_" + file.DiskFileID, Method.Post, 0);
                    extendName = _Request.Get("extname_1_" + file.DiskFileID, Method.Post, string.Empty);
                    attach.AttachType = AttachType.DiskFile;
                    if (!string.IsNullOrEmpty(attach.FileName) && !string.IsNullOrEmpty(extendName))
                        attach.FileName += "." + extendName;

                    attach.FileType = extendName;
                    attachments.Add(attach);
                }

                foreach (int id in tempFileIds)
                {
                    attach = new Attachment();
                    attach.AttachmentID = id;
                    attach.FileName = _Request.Get("filename_0_" + id, Method.Post, "未命名", false);
                    attach.Price = _Request.Get("price_0_" + id, Method.Post, 0);

                    attach.AttachType = AttachType.TempAttach;

                    extendName = _Request.Get("extname_0_" + id, Method.Post, string.Empty);
                    if (!string.IsNullOrEmpty(attach.FileName) && !string.IsNullOrEmpty(extendName))
                        attach.FileName += "." + extendName;

                    attach.FileType = extendName;
                    attachments.Add(attach);
                }

            foreach(Attachment att in attachments)
            {
                att.PostID = 0;

                if (IsEditPost || IsEditThread)
                    att.UserID = postUserID;
                
                else
                    att.UserID = MyUserID;

                if (att.Price < 0)
                {
                    msgDisplay.AddError("附件售价不能小于0");
                    return;
                }
            }
            attachs.AddRange(attachments);
        }

        public void ProcessPost()
        {
            m_Action = _Request.Get("postaction",Method.Post,string.Empty);

            string validateCodeAction;
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (IsReply || IsEditPost)
                validateCodeAction = "ReplyTopic";
            else
                validateCodeAction = "CreateTopic";

            if (CheckValidateCode(validateCodeAction, msgDisplay))
            {
                string subject = _Request.Get("subject", Method.Post, string.Empty);
                string content = _Request.Get("editor_content", Method.Post, string.Empty, false);

                int iconID = _Request.Get<int>("posticon", Method.Post, 0);
                string ipAddress = _Request.IpAddress;

                string idString = _Request.Get("enableitem", Method.Post, string.Empty).ToLower();

                //---------如果是以新的UBB方式提交----------
                string formatMode = _Request.Get("editorallowtype", Method.Post, "ubb").ToLower();
                bool enableHTML = false;
                bool enableMaxCode3 = false;
                if (AllowHtml && AllowMaxcode)
                {
                    enableHTML = StringUtil.EqualsIgnoreCase(_Request.Get("contentformat", Method.Post, string.Empty), "enablehtml");
                    if (enableHTML == false)
                        enableMaxCode3 = true;
                }
                else if (AllowHtml)
                    enableHTML = true;
                else if (AllowMaxcode)
                    enableMaxCode3 = true;

                bool enableEmoticons = (idString.IndexOf("enableemoticons") > -1);
                bool enableSignature = (idString.IndexOf("enablesignature") > -1);
                bool enableReplyNotice = (idString.IndexOf("enablereplynotice") > -1);


                AttachmentCollection attachments = new AttachmentCollection();
                List<int> tempUploadFileIds = new List<int>();

                string postUsername = null;
                int postUserID;
                if (IsEditPost || IsEditThread)
                {
                    postUserID = 0;
                }
                else
                {
                    postUserID = MyUserID;
                    if(IsLogin)
                        postUsername = My.Name;
                    else
                    {
                        if (EnableGuestNickName)
                            postUsername = _Request.Get("guestnickname", Method.Post, string.Empty);
                        else
                            postUsername = "";
                    }
                }

                //string attachIdsText = _Request.Get("attachIds", Method.Post, string.Empty, false);
                //if (string.IsNullOrEmpty(attachIdsText) == false)
                //{
                //    int[] attachIds = StringUtil.Split<int>(attachIdsText, ',');

                //    GetAttachments(attachIds, "0", postUserID, msgDisplay, ref attachments);
                //}

                //string diskIdsText = _Request.Get("diskFileIDs", Method.Post, string.Empty, false);
                //if (string.IsNullOrEmpty(diskIdsText) == false)
                //{
                //    int[] diskFileIDs = StringUtil.Split<int>(diskIdsText, ',');

                //}

                GetAttachments(postUserID, msgDisplay, ref attachments);

                if (msgDisplay.HasAnyError())
                    return;

                bool success = false;

                try
                {
                    int threadID = ThreadID, postID = PostID;
                    if (IsCreateThread)
                    {
                        #region
                        bool isLocked = _Request.Get<bool>("cbLockThread", Method.Post, false);
                        int threadCatalogID = _Request.Get<int>("threadCatalogs", Method.Post, 0);
                        if (Action == "thread")
                        {
                            int price = _Request.Get<int>("price", Method.Post, 0);
                            success = PostBOV5.Instance.CreateThread(My, false, enableEmoticons, ForumID, threadCatalogID, iconID, subject, string.Empty, price
                                , postUsername, isLocked, false, content, enableHTML, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress
                                , attachments, out threadID, out postID);
                        }
                        else if (Action == "poll")
                        {
                            bool alwaysEyeable = !_Request.IsChecked("cbNoEyeable", Method.Post, false);
                            int multiple = _Request.Get<int>("voteMultiple", Method.Post, 0);
                            TimeSpan expiresDate = GetExpiresDate(ThreadType.Poll);
                            string pollItemString = _Request.Get("vote", Method.Post, string.Empty).Trim();

                            success = PostBOV5.Instance.CreatePoll(pollItemString, multiple, alwaysEyeable, expiresDate
                                , My, false, enableEmoticons, ForumID, threadCatalogID, iconID, subject, string.Empty
                                , postUsername, isLocked, false, content, enableHTML, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress
                                , attachments, out threadID, out postID);
                        }
                        else if (Action == "question")
                        {
                            int reward = _Request.Get<int>("reward", Method.Post, 0);
                            int rewardCount = _Request.Get<int>("rewardCount", Method.Post, 0);
                            bool alwaysEyeable = !_Request.IsChecked("notEyeable", Method.Post, true);

                            TimeSpan expiresDate = GetExpiresDate(ThreadType.Question);

                            success = PostBOV5.Instance.CreateQuestion(reward, rewardCount, alwaysEyeable, expiresDate
                                , My, false, enableEmoticons, ForumID, threadCatalogID, iconID, subject, string.Empty
                                , postUsername, isLocked, false, content, enableHTML, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress
                                , attachments, out threadID, out postID);
                        }
                        else if (Action == "polemize")
                        {
                            string agreeViewPoint = _Request.Get("AgreeViewPoint", Method.Post, string.Empty);
                            string againstViewPoint = _Request.Get("AgainstViewPoint", Method.Post, string.Empty);

                            TimeSpan expiresDate = GetExpiresDate(ThreadType.Polemize);

                            success = PostBOV5.Instance.CreatePolemize(agreeViewPoint, againstViewPoint, expiresDate
                                , My, false, enableEmoticons, ForumID, threadCatalogID, iconID, subject, string.Empty
                                , postUsername, isLocked, false, content, enableHTML, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress
                                , attachments, out threadID, out postID);
                        }

                        if (success)
                        {
                            bool sticky = _Request.Get<bool>("cbStickyThread", Method.Post, false);
                            bool globalStickyThread = _Request.Get<bool>("cbGlobalStickyThread", Method.Post, false);

                            int[] threadIDs = new int[] { threadID };
                            if (globalStickyThread)
                            {
                                PostBOV5.Instance.SetThreadsStickyStatus(My, ForumID, null, threadIDs, ThreadStatus.GlobalSticky, null, false, false, true, "");
                            }
                            else if (sticky)
                            {
                                PostBOV5.Instance.SetThreadsStickyStatus(My, ForumID, null, threadIDs, ThreadStatus.Sticky, null, false, false, true, "");
                            }

                            if (isLocked)//记录日志
                            {
                                string threadLog;
                                PostBOV5.Instance.CreateThreadManageLog(My, _Request.IpAddress, ModeratorCenterAction.LockThread, new int[] { MyUserID }, ForumID
                                    , new int[] { threadID }, new string[] { subject }, string.Empty, true, out threadLog);

                                BasicThread thread = ThreadCachePool.GetThread(threadID);
                                if (string.IsNullOrEmpty(threadLog) == false && thread!=null)
                                {
                                    thread.ThreadLog = threadLog;
                                }
                            }
                        }
                        #endregion
                    }
                    else if (IsEditThread || IsEditPost)
                    {
                        int lastEditorID = MyUserID;
                        string lastEditor = My.Username;

                        #region
                        bool recodeEditLog = _Request.Get("recodeEditLog", Method.Post, "true").ToLower() == "true";

                        if (!recodeEditLog && HasEditPermission)
                        {
                            lastEditorID = 0;
                            lastEditor = string.Empty;
                        }

                        if (IsEditThread)
                        {
                            int threadCatalogID = _Request.Get<int>("threadCatalogs", Method.Post, 0);
                            int price = _Request.Get<int>("price", Method.Post, 0);
                            success = PostBOV5.Instance.UpdateThread(My, ThreadID, threadCatalogID, iconID, subject, price, lastEditorID, lastEditor
                                , content, enableEmoticons, enableHTML, enableMaxCode3, enableSignature, enableReplyNotice, attachments, false
                                );
                        }
                        else
                        {
                            success = PostBOV5.Instance.UpdatePost(My, PostID, iconID, subject, lastEditorID, lastEditor, content, enableEmoticons, enableHTML
                                , enableMaxCode3, enableSignature, enableReplyNotice, attachments, false);
                        }
                        #endregion
                    }
                    else //回复
                    {
                        PostType postType = (PostType)_Request.Get<int>("viewPointType", Method.Post, 0);

                        int parentID = PostID;

                        success = PostBOV5.Instance.ReplyThread(My, ThreadID, postType, iconID, subject, content, enableEmoticons
                            , enableHTML, enableMaxCode3, enableSignature, enableReplyNotice, ForumID, postUsername, ipAddress, parentID
                            , attachments, false, out postID);
                    }


                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UnapprovedError)
                            {
                                //AlertWarning("");
                                //ShowWarning(error.Message, BbsUrlHelper.GetForumUrl(Forum.CodeName));

                                if (IsAjaxRequest)
                                {
                                    m_Message = error.Message;
                                    m_JumpLinks.Add("返回列表页", BbsUrlHelper.GetForumUrl(Forum.CodeName));
                                    m_IsPostAlert = true;
                                    PostReturnUrl = BbsUrlHelper.GetForumUrl(Forum.CodeName);
                                }
                                else
                                {
                                    ShowWarning(error.Message, BbsUrlHelper.GetForumUrl(Forum.CodeName));
                                }

                                //NameObjectCollection paramList = new NameObjectCollection();
                                //paramList.Add("IsPostSuccess", false);
                                //paramList.Add("IsPostAlert", true);
                                //paramList.Add("PostMessage", m_Message);
                                //paramList.Add("JumpLinks", new JumpLinkCollection());
                                //paramList.Add("PostReturnUrl", PostReturnUrl);
                                //Display("~/max-templates/default/_part/post_success.aspx?_max_ajaxids_=post_success", true, paramList);
                            }
                            else
                                msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        if (IsEditPost == false && IsEditThread == false)//编辑的时候 不记录
                            ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);

                        string returnUrl = null;
                        if (Type != "")
                        {
                            SystemForum sf;
                            if (string.Compare(Type, SystemForum.RecycleBin.ToString(), true) == 0)
                                returnUrl = UrlHelper.GetRecycledThreadsUrl(CodeName);
                            else if (string.Compare(Type, SystemForum.UnapprovePosts.ToString(), true) == 0)
                                returnUrl = UrlHelper.GetUnapprovedPostsUrl(CodeName);
                            else if (string.Compare(Type, SystemForum.UnapproveThreads.ToString(), true) == 0)
                                returnUrl = UrlHelper.GetUnapprovedThreadsUrl(CodeName);
                            else
                            {
                                returnUrl = BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, 1);
                            }
                        }

                        BasicThread thread = PostBOV5.Instance.GetThread(threadID);
                        int threadPages = thread.TotalPages;
                        if (IsReply)
                        {
                            bool returnLastUrl = _Request.Get<int>("tolastpage", Method.Post, 0) == 1;

                            int returnPage = _Request.Get<int>("page", Method.Get, 1);

                            string lastUrl = BbsUrlHelper.GetLastThreadUrl(Forum.CodeName, threadID, thread.ThreadTypeString, postID, threadPages, true);
                            returnUrl = BbsUrlHelper.GetThreadUrl(Forum.CodeName, threadID, thread.ThreadTypeString, returnPage);

                            if (returnLastUrl)
                            {
                                m_Message = "回复成功，现在将转入该主题的最后一页";
                                m_JumpLinks.Add("如果没有自动跳转,请点此处", lastUrl);
                                m_JumpLinks.Add("如果要转入该主题的第" + returnPage + "页,请点此处", returnUrl);

                                returnUrl = lastUrl;
                            }
                            else
                            {
                                m_Message = "回复成功，现在将转入该主题的第" + returnPage + "页";
                                m_JumpLinks.Add("如果没有自动跳转,请点此处", returnUrl);
                                m_JumpLinks.Add("如果要转入该主题的最后一页,请点此处", lastUrl);
                            }
                        }
                        else if (IsCreateThread)
                        {
                            returnUrl = BbsUrlHelper.GetThreadUrl(Forum.CodeName, threadID, thread.ThreadTypeString, 1);
                        }
                        else
                        {
                            if (Type == "")
                                returnUrl = BbsUrlHelper.GetThreadUrl(Forum.CodeName, threadID, thread.ThreadTypeString, 1);
                        }




                        JsonBuilder json = new JsonBuilder();
                        json.Set("issuccess", true);
 
                        
                        PostReturnUrl = returnUrl;




                        if (IsReply == false)
                        {
                            m_JumpLinks.Add("如果没有自动跳转,请点此处", returnUrl);
                            m_Message = "操作成功，现在将转入查看主题页";
                        }
                        m_JumpLinks.Add("如果要返回主题列表,请点击此处", BbsUrlHelper.GetForumUrl(Forum.CodeName));
                        m_IsPostSuccess = true;
                        //ShowSuccess("操作成功，现在将转入查看主题页", links);

                        if (IsAjaxRequest == false)
                        {
                            ShowSuccess(m_Message, m_JumpLinks);
                        }
                        else
                        {
                            AjaxPanelContext.SetAjaxResult(json);
                        }
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

            }

        }


        protected string PostReturnUrl
        {
            get;
            set;
        }


        private TimeSpan GetExpiresDate(ThreadType threadType)
        {
            int times = 0;
            string timeUnit = string.Empty;
            if (threadType == ThreadType.Question)
            {
                getTime(ForumSetting.QuestionValidDays[My], out times, out timeUnit);
            }
            else if (threadType == ThreadType.Polemize)
            {
                getTime(ForumSetting.PolemizeValidDays[My], out times, out timeUnit);
            }
            else if (threadType == ThreadType.Poll)
            {
                getTime(ForumSetting.PollValidDays[My], out times, out timeUnit);
            }
            if (times > 0)
            {
                int value = _Request.Get<int>("expiresDays", Method.Post, 0);
                //if (value > times)
                //    ShowError("有效天数不能超过" + Convert.ToInt32(times) + timeUnit);
                if (value <= 0)
                    value = Convert.ToInt32(Time);

                //DateTime expiresDate = DateTimeUtil.Now;
                TimeSpan expiresTime = new TimeSpan();
                switch (timeUnit)
                {
                    case "天":
                        expiresTime = new TimeSpan(value, 0, 0, 0);
                        break;
                    case "小时":
                        expiresTime = new TimeSpan(0, value, 0, 0);
                        break;
                    case "分钟":
                        expiresTime = new TimeSpan(0, 0, value, 0);
                        break;
                    case "秒":
                        expiresTime = new TimeSpan(0, 0, 0, value);
                        break;
                    default: break;
                }
                return expiresTime;
            }
            return TimeSpan.MaxValue;
        }


        private JumpLinkCollection m_JumpLinks = new JumpLinkCollection();
        protected JumpLinkCollection JumpLinks
        {
            get
            {
                return m_JumpLinks;
            }
        }

        private bool m_IsPostSuccess = false;
        protected bool IsPostSuccess
        {
            get
            {
                return m_IsPostSuccess;
            }
        }

        private bool m_IsPostAlert = false;
        protected bool IsPostAlert
        {
            get
            {
                return m_IsPostAlert;
            }
        }

        private string m_Message = string.Empty;
        protected string PostMessage
        {
            get
            {
                return m_Message;
            }
        }

        protected string ProcessCatelogName(string name)
        {
            return ClearHTML(name);
        }

        protected string TradeRate
        {
            get
            {
                return AllSettings.Current.PointSettings.TradeRate.ToString() + "%";
            }
        }
    }
}