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
using System.Data;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;
namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    public partial class list : ForumPageBase
    {
        protected bool IsNormalThreads = true;
        protected bool IsRecycleBin = false;
        protected bool IsUnapprovedThreads = false;
        protected bool IsUnapprovedPostsThreads = false;

        private bool IsDefaultList = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //点击了快速发帖按钮
            if (_Request.IsClick("postButton"))
            {
                ProcessThread();
            }

            ForumBO.Instance.SetForumsLastThread(Forum.SubForumsForList);

            int totalThreads = 0;

            int pageSize = AllSettings.Current.BbsSettings.ThreadsPageSize;

            if (_Request.IsSpider)
            {
                if (ThreadCatalogID != -1)
                    m_ThreadCatalogID = -1;

                if (Action != "list")
                    m_Action = "list";
            }

            //如果要显示某个主题分类的主题
            if (ThreadCatalogID != -1)
            {
                AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(CodeName));

                Action = "";
                if (ThreadCatalogID != 0)
                {
                    ThreadCatalog threadCatalog = ForumBO.Instance.GetThreadCatalog(ThreadCatalogID);
                    if (threadCatalog == null)
                    {
                        Response.End();
                    }

                    AddNavigationItem(string.Concat("[", threadCatalog.ThreadCatalogName, "]"));
                    //navigation += "  [" + threadCatalog.ThreadCatalogName + "]";
                }
                else
                    AddNavigationItem("[其他]");
                    //navigation += "    [其他]";


                threads = PostBOV5.Instance.GetThreads(ForumID, ThreadCatalogID, PageNumber, pageSize, SortType, BeginDate, EndDate, IsDesc, out totalThreads);

                //设置分页控件的显示
                SetPager("ThreadListPager", BbsUrlHelper.GetThreadCatalogUrlForPager(CodeName, ThreadCatalogID) + (parms == null ? "" : "?" + parms), PageNumber, pageSize, totalThreads);
                //url = BbsUrlHelper.GetForumEmoticonUrlForPager(CodeName, ThreadCatalogID, PageNumber, IsGetAllDefaultEmoticon, EmoticonGroupID);
            }
            else
            {
                string navigation = null;

                switch (Action)
                {

                    //精华帖
                    case "valued":

                        navigation = "[精华帖]";
                        threads = PostBOV5.Instance.GetValuedThreads(ForumID, PageNumber, pageSize, SortType, BeginDate, EndDate, IsDesc, true, out totalThreads);

                        break;

                    //投票帖
                    case "poll":

                        navigation = "[投票帖]";
                        threads = PostBOV5.Instance.GetThreads(ForumID, ThreadType.Poll, PageNumber, pageSize, SortType, BeginDate, EndDate, IsDesc, true, out totalThreads);
                        break;

                    //问题帖
                    case "question":

                        navigation = "[提问帖]";
                            threads = PostBOV5.Instance.GetThreads(ForumID, ThreadType.Question, PageNumber, pageSize, SortType, BeginDate, EndDate, IsDesc, true, out totalThreads);
                        break;

                    //辩论帖
                    case "polemize":

                        navigation = "[辩论帖]";
                            threads = PostBOV5.Instance.GetThreads(ForumID, ThreadType.Polemize, PageNumber, pageSize, SortType, BeginDate, EndDate, IsDesc, true, out totalThreads);
                        break;

                    //回收
                    case "recycled":

                        IsNormalThreads = false;
                        IsRecycleBin = true;
                        navigation = "[回收站]";
                        if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled) == false)
                            ShowError("您没有权限进入回收站");
                        threads = PostBOV5.Instance.GetThreadsByStatus(ThreadStatus.Recycled, ForumID, SortType, BeginDate, EndDate, IsDesc, PageNumber, pageSize, out totalThreads);
                        break;

                    //未审核主题
                    case "unapproved":

                        IsNormalThreads = false;
                        IsUnapprovedThreads = true;
                        navigation = "[未审核主题]";
                        if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads) == false)
                            ShowError("您没有权限查看未审核的主题");

                        threads = PostBOV5.Instance.GetThreadsByStatus(ThreadStatus.UnApproved, ForumID, SortType, BeginDate, EndDate, IsDesc, PageNumber, pageSize, out totalThreads);
                        break;

                    //未审核回复
                    case "unapprovedpost":

                        IsNormalThreads = false;
                        IsUnapprovedPostsThreads = true;
                        navigation = "[未审核回复]";
                        if (ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.ApprovePosts) == false)
                            ShowError("您没有权限查看未审核的回复");
                        threads = PostBOV5.Instance.GetUnapprovedPostThreads(ForumID, PageNumber, pageSize);
                        totalThreads = threads.TotalRecords;
                        break;

                    //正常帖子
                    default:

                        IsDefaultList = true;
                        Action = "list";
                        threads = PostBOV5.Instance.GetThreads(ForumID, SortType, BeginDate, EndDate, IsDesc, PageNumber, pageSize, _Request.IsSpider, out totalThreads);
                        break;
                }

                if (navigation == null)
                    AddNavigationItem(Forum.ForumName);
                else
                {
                    AddNavigationItem(Forum.ForumName, BbsUrlHelper.GetForumUrl(CodeName));
                    AddNavigationItem(navigation);
                }

                //设置分页控件的显示
                SetPager("ThreadListPager", BbsUrlHelper.GetForumUrlForPager(CodeName, Action) + (parms == null ? "" : "?" + parms), PageNumber, pageSize, totalThreads);
                //url = BbsUrlHelper.GetForumEmoticonUrlForPager(CodeName, Action, PageNumber, IsGetAllDefaultEmoticon, EmoticonGroupID);
            }

            if (PageNumber > 1 && threads.Count == 0)
            {
                Response.Redirect(AttachQueryString("page=1"));
            }


            //更新用户在在线列表中的状态
            UpdateOnlineStatus(OnlineAction.ViewThreadList, 0, "");
            //OnlineManager.UpdateOnlineUser(MyUserID, ForumID, 0, My.OnlineStatus, OnlineAction.ViewThreadList, Request, Response);

            if (IsNormalThreads)
                PostBOV5.Instance.ProcessKeyword(threads, ProcessKeywordMode.TryUpdateKeyword);
            else
                PostBOV5.Instance.ProcessKeyword(threads, ProcessKeywordMode.FillOriginalText);

            if (CanManageThread)
            {
                List<int> userIDs = new List<int>();
                foreach (BasicThread thread in threads)
                {
                    if (userIDs.Contains(thread.PostUserID) == false)
                        userIDs.Add(thread.PostUserID);
                }

                UserBO.Instance.GetUsers(userIDs, GetUserOption.WithAll);
            }

            WaitForFillSimpleUsers<Moderator>(Forum.Moderators);
            foreach (Forum subForum in Forum.SubForumsForList)
            {
                WaitForFillSimpleUsers<Moderator>(subForum.Moderators);
            }

            SubmitFillUsers();
        }

        protected override string PageTitle
        {
            get
            {
                string pageNumberString = AllSettings.Current.BaseSeoSettings.FormatPageNumber(PageNumber);
                if (string.IsNullOrEmpty(pageNumberString))
                    return base.PageTitle;
                else
                    return string.Concat(Forum.ForumNameText, " - ", pageNumberString, " - ", GetBasePageTitle());
            }
        }

        protected override bool IsShowOnline
        {
            get
            {
                if (IsNormalThreads == false)
                    return false;
                else
                    return base.IsShowOnline;
            }
        }

        protected bool IsCatalogForum
        {
            get
            {
                return Forum.ParentID == 0;
            }
        }


        private string parms = null;
        private void setParms(string parm)
        {
            parms = (parms == null ? "" : (parms + "&")) + parm;
        }

        private bool hasSetSortType = false;
        private ThreadSortField? m_SortType;
        protected ThreadSortField? SortType
        {
            get
            {
                if (m_SortType == null && hasSetSortType == false)
                {
                    hasSetSortType = true;
                    m_SortType = _Request.Get<ThreadSortField>("sorttype", Method.Get);
                    if (m_SortType != null)
                    {
                        setParms("sorttype=" + m_SortType.Value.ToString());
                    }
                }
                return m_SortType;
            }
        }

        protected ThreadSortField DefaultSortType
        {
            get
            {
                return ForumSetting.DefaultThreadSortField;
            }
        }

        private int? days;
        protected int Days
        {
            get
            {
                if (days == null)
                {
                    days = _Request.Get<int>("days", Method.Get, 0);
                    if (days != 0)
                    {
                        setParms("days=" + days);
                    }
                }
                return days.Value;
            }
        }

        protected DateTime? BeginDate
        {
            get
            {
                if (Days == 0)
                    return null;
                else
                    return DateTimeUtil.Now.AddDays(-Days);
            }
        }

        protected DateTime? EndDate
        {
            get
            {
                return null;
            }
        }

        private bool? isDesc;
        protected bool IsDesc
        {
            get
            {
                if (isDesc == null)
                {
                    isDesc = _Request.Get<bool>("isdesc", Method.Get, true);
                    if (_Request.Get("isdesc") != null)
                    {
                        setParms("isdesc=" + isDesc.ToString());
                    }
                }
                return isDesc.Value;
            }
        }

        protected string GetThreadTypeLinks(string linkStyle, string currentLinkStyle)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //if (Can(ForumPermissionSetNode.Action.ViewThread))
            if (IsDefaultList && Days != 0)
                appendThreadTypeLink(sb, "list", "全部", linkStyle, "");
            else
                appendThreadTypeLink(sb, "list", "全部", linkStyle, currentLinkStyle);
            //if (Can(ForumPermissionSetNode.Action.ViewValuedThread))//精华
            //    appendThreadTypeLink(sb, "valued", "精华", linkStyle, currentLinkStyle);

            appendThreadTypeLink(sb, "poll", "投票", linkStyle, currentLinkStyle);
            appendThreadTypeLink(sb, "question", "提问", linkStyle, currentLinkStyle);
            appendThreadTypeLink(sb, "polemize", "辩论", linkStyle, currentLinkStyle);
            return sb.ToString();
        }

        protected string GetThreadTypeLinks2(string linkStyle, string currentLinkStyle)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (IsDefaultList && Days != 0)
                appendThreadTypeLink(sb, "list", "全部", linkStyle, linkStyle);
            else
                appendThreadTypeLink(sb, "list", "全部", linkStyle, currentLinkStyle);

            if (Can(ForumPermissionSetNode.Action.ViewValuedThread))//精华
                appendThreadTypeLink(sb, "valued", "精华", linkStyle, currentLinkStyle);

            //TODO:
            //appendThreadTypeLink(sb, "tuijian", "推荐", linkStyle, currentLinkStyle);
            return sb.ToString();
        }

        private void appendThreadTypeLink(StringBuilder sb, string threadLocationStr, string name, string linkStyle, string currentLinkStyle)
        {
            if (string.Compare(Action, threadLocationStr, true) == 0)
            {
                sb.Append(string.Format(currentLinkStyle, BbsUrlHelper.GetForumUrl(Forum.CodeName, threadLocationStr, 1), name));
            }
            else
                sb.Append(string.Format(linkStyle, BbsUrlHelper.GetForumUrl(Forum.CodeName, threadLocationStr, 1), name));
        }

        protected string SetDayCurreantClass(int days, string className)
        {
            if (Days == days)
                return string.Format("class=\"{0}\"", className);
            else
                return string.Empty;
        }

        private ThreadCollectionV5 threads = null;

        private ThreadCollectionV5 m_StickThreads;
        protected ThreadCollectionV5 StickThreads
        {
            get
            {
                if (m_StickThreads == null)
                {
                    m_StickThreads = new ThreadCollectionV5();
                    if (IsNormalThreads)
                    {
                        foreach (BasicThread thread in threads)
                        {
                            if (thread.ThreadStatus == ThreadStatus.GlobalSticky || thread.ThreadStatus == ThreadStatus.Sticky)
                            {
                                m_StickThreads.Add(thread);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                return m_StickThreads;
            }
        }

        private ThreadCollectionV5 m_NormalThreads;
        protected ThreadCollectionV5 NormalThreads
        {
            get
            {
                if (m_NormalThreads == null)
                {
                    m_NormalThreads = new ThreadCollectionV5();
                    foreach (BasicThread thread in threads)
                    {
                        if (thread.ThreadStatus == ThreadStatus.GlobalSticky || thread.ThreadStatus == ThreadStatus.Sticky)
                        {
                            continue;
                        }
                        m_NormalThreads.Add(thread);
                    }
                }
                return m_NormalThreads;
            }
        }

        protected string GetThreadUrl(BasicThread thread, bool isLastPage)
        {
            if (ThreadCatalogID == -1)
            {
                if (IsNormalThreads)
                {
                    if (IsDefaultList)
                    {
                        if (isLastPage)
                            return BbsUrlHelper.GetThreadUrl(CodeName, thread.RedirectThreadID, thread.ThreadTypeString, thread.TotalPages, PageNumber);
                        else
                            return BbsUrlHelper.GetThreadUrl(CodeName, thread.RedirectThreadID, thread.ThreadTypeString, 1, PageNumber);
                    }
                    else
                    {
                        if (isLastPage)
                            return BbsUrlHelper.GetThreadUrlForExtParms(CodeName, thread.RedirectThreadID, thread.ThreadTypeString, thread.TotalPages, "Action=" + Action + "&page=" + PageNumber);
                        else
                            return BbsUrlHelper.GetThreadUrlForExtParms(CodeName, thread.RedirectThreadID, thread.ThreadTypeString, 1, "Action=" + Action + "&page=" + PageNumber);
                    }
                }
                else
                {
                    if (string.Compare(Action, "recycled", true) == 0)
                    {
                        return BbsUrlHelper.GetThreadUrl(CodeName, thread.RedirectThreadID, thread.TotalPages, thread.ThreadTypeString, "recyclebin");
                    }
                    else if (string.Compare(Action, "unapproved", true) == 0)
                    {
                        return BbsUrlHelper.GetThreadUrl(CodeName, thread.RedirectThreadID, thread.TotalPages, thread.ThreadTypeString, "unapprovethreads");
                    }
                    else
                    {
                        return BbsUrlHelper.GetThreadUrl(CodeName, thread.RedirectThreadID, thread.TotalPages, thread.ThreadTypeString, "unapproveposts");
                    }
                }
            }
            else
            {
                if (isLastPage)
                    return BbsUrlHelper.GetThreadUrlForExtParms(CodeName, thread.RedirectThreadID, thread.ThreadTypeString, thread.TotalPages, "ThreadCatalogID=" + ThreadCatalogID + "&page=" + PageNumber);
                else
                    return BbsUrlHelper.GetThreadUrlForExtParms(CodeName, thread.RedirectThreadID, thread.ThreadTypeString, 1, "ThreadCatalogID=" + ThreadCatalogID + "&page=" + PageNumber);
            }
        }

        private int? m_PageNumber;
        protected int PageNumber
        {
            get
            {
                if (m_PageNumber == null)
                {
                    //取得当前页码
                    m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
                }
                return m_PageNumber.Value;
            }
        }

        private string m_Action;
        protected string Action
        {
            get
            {
                if (m_Action == null)
                {
                    m_Action = _Request.Get("Action", Method.Get, "list").ToLower();
                }
                return m_Action;
            }
            set { m_Action = value; }
        }

        private int? m_ThreadCatalogID;
        protected int ThreadCatalogID
        {
            get
            {
                if (m_ThreadCatalogID == null)
                {
                    m_ThreadCatalogID = _Request.Get<int>("ThreadCatalogID", Method.Get, -1);
                }
                return m_ThreadCatalogID.Value;
            }
        }

        protected bool IsShowCheckBox(BasicThread thread)
        {
            if (ForumID == thread.ForumID &&
                (//Forum.IsModerator(MyUserID) || 
                ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.MoveThreads, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsUp)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog, thread.PostUserID)
                || ForumManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads, thread.PostUserID))
                )
                return true;
            else
                return false;
        }

        protected new string GetThreadLink(BasicThread thread, int subjectLength, string linkStyle)
        {
            if (ThreadCatalogID == -1)
            {
                if (IsNormalThreads)
                {
                    if (IsDefaultList)
                    {
                        return GetThreadLink(thread, subjectLength, 1, null, linkStyle, PageNumber);
                        //return GetThreadLink(thread,subjectLength,1,false,(PageIndex+1));
                    }
                    else
                    {
                        return GetThreadLinkForExtParms(thread, subjectLength, 1, false, string.Concat("Action=", Action, "&page=", PageNumber.ToString()), linkStyle);
                    }
                }
                else
                {
                    if (StringUtil.EqualsIgnoreCase(Action, "recycled"))
                    {
                        return GetThreadLink(thread, subjectLength, 1, "recyclebin", linkStyle, PageNumber);
                    }
                    else if (StringUtil.EqualsIgnoreCase(Action, "unapproved"))
                    {
                        return GetThreadLink(thread, subjectLength, 1, "unapprovethreads", linkStyle, PageNumber);
                    }
                    else
                    {
                        return GetThreadLink(thread, subjectLength, 1, "unapproveposts", linkStyle, PageNumber);
                    }
                }
            }
            else
            {
                return GetThreadLinkForExtParms(thread, subjectLength, 1, false, string.Concat("ThreadCatalogID=", ThreadCatalogID.ToString(), "&page=", PageNumber.ToString()), linkStyle);
            }
        }

        protected new string GetThreadPager(BasicThread thread, string style, string urlStyle)
        {
            if (ThreadCatalogID == -1)
            {
                if (IsNormalThreads)
                {
                    if (IsDefaultList)
                    {
                        return GetThreadPager(thread, style, urlStyle, null, false, null, PageNumber);
                        //return GetThreadLink(thread,subjectLength,1,false,(PageIndex+1));
                    }
                    else
                        return GetThreadPager(thread, style, urlStyle, string.Concat("Action=", Action, "&page=", PageNumber.ToString()), true, null, PageNumber);
                }
                else
                {
                    if (StringUtil.EqualsIgnoreCase(Action, "recycled"))
                    {
                        return GetThreadPager(thread, style, urlStyle, "recyclebin", false, null, PageNumber);
                    }
                    else if (StringUtil.EqualsIgnoreCase(Action, "unapproved"))
                    {
                        return GetThreadPager(thread, style, urlStyle, "unapprovethreads", false, null, PageNumber);
                    }
                    else
                    {
                        return GetThreadPager(thread, style, urlStyle, "unapproveposts", false, null, PageNumber);
                    }
                }
            }
            else
            {
                return GetThreadPager(thread, style, urlStyle, string.Concat("ThreadCatalogID=", ThreadCatalogID.ToString(), "&page=", PageNumber.ToString()), true, null, PageNumber);
            }
        }

        protected bool IsNewThread(BasicThread thread)
        {
            if ((DateTimeUtil.Now - thread.CreateDate).TotalSeconds < AllSettings.Current.BbsSettings.NewThreadTime)
                return true;
            else
                return false;
        }

        protected override bool IsShowModeratorManageLink
        {
            get
            {
                if (IsNormalThreads)
                    return base.IsShowModeratorManageLink;
                else
                    return true;
            }
        }

        private bool? m_IsShowQuicklyPost = null;
        protected bool IsShowQuicklyPost
        {
            get
            {
                if (m_IsShowQuicklyPost == null)
                {
                    if (IsNormalThreads == false)
                        m_IsShowQuicklyPost = false;
                    //没有发帖权限，或者在该版块已经被屏蔽，都不显示快速发帖框
                    else if (!AllSettings.Current.BbsSettings.AllowQuicklyCreateThread)
                    {
                        m_IsShowQuicklyPost = false;
                    }
                    else if (!Can(ForumPermissionSetNode.Action.CreateThread) || Forum.IsShieldedUser(MyUserID))
                    {
                        m_IsShowQuicklyPost = false;
                    }
                    else
                        m_IsShowQuicklyPost = true;

                }
                return m_IsShowQuicklyPost.Value;
            }
        }

        protected string GetThreadCatalogList()
        {
            return GetThreadCatalogList(0, true);
        }

        protected override string MetaDescription
        {
            get
            {
                string pagerString = "";
                if (PageNumber > 1)
                    pagerString = " (第" + PageNumber + "页)";
                if (string.IsNullOrEmpty(Forum.ExtendedAttribute.MetaDescription))
                    return StringUtil.HtmlEncode(StringUtil.CutString(ClearHTML(Forum.Readme), 200)) + pagerString;
                else
                    return Forum.ExtendedAttribute.MetaDescription + pagerString;
            }
        }

        protected override string MetaKeywords
        {
            get
            {
                string pagerString = "";
                if (PageNumber > 1)
                    pagerString = " (第" + PageNumber + "页)";
                return base.MetaKeywords + pagerString;
            }
        }

        protected string GetThreadCatalogLinks(string linkStyle, string currentLinkStyle)
        {

            if (Forum.ThreadCatalogStatus == ThreadCatalogStatus.DisEnable)
                return string.Empty;

            if (string.IsNullOrEmpty(linkStyle))
                linkStyle = "<a href=\"{0}\">{1}</a>";
            if (string.IsNullOrEmpty(currentLinkStyle))
                currentLinkStyle = "<a class=\"threadCatalogSelected\">{1}</a>";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            appendThreadTypeLink(sb, "list", "全部", linkStyle, currentLinkStyle);
            foreach (ThreadCatalog threadCatalog in ThreadCatalogs)
            {
                if (ThreadCatalogID == threadCatalog.ThreadCatalogID)
                    sb.Append(string.Format(currentLinkStyle, BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, threadCatalog.ThreadCatalogID, 1), threadCatalog.ThreadCatalogName));
                //sb.Append("<a class=\"threadCatalogSelected\">" + threadCatalog.ThreadCatalogName + "</a> ");
                else
                    sb.Append(string.Format(linkStyle, BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, threadCatalog.ThreadCatalogID, 1), threadCatalog.ThreadCatalogName));
            }
            if (Forum.ThreadCatalogStatus != ThreadCatalogStatus.EnableAndMust)
            {
                if (ThreadCatalogID == 0)
                    sb.Append(string.Format(currentLinkStyle, BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, 0, 1), "其他"));
                else
                    sb.Append(string.Format(linkStyle, BbsUrlHelper.GetThreadCatalogUrl(Forum.CodeName, 0, 1), "其他"));
            }
            return sb.ToString();
        }


        //快速发帖
        private void ProcessThread()
        {
            string validateCodeAction = "CreateTopic";
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (CheckValidateCode(validateCodeAction, msgDisplay))
            {
                string posterName;
                if (IsLogin == false)
                {
                    if (EnableGuestNickName)
                        posterName = _Request.Get("guestNickName", Method.Post, string.Empty);
                    else
                        posterName = "";
                }
                else
                {
                    posterName = My.Name;
                }

                string subject = _Request.Get("Subject", Method.Post, string.Empty);
                string content = _Request.Get("Editor", Method.Post, string.Empty,false);
                string ipAddress = _Request.IpAddress;

                //string enableItems = _Request.Get("enableItem", Method.Post, string.Empty);

                bool enableHtml = false;
                bool enableMaxCode3 = false;
                if (AllowHtml && AllowMaxcode)
                {
                    enableHtml = _Request.Get<int>("eritoSellect", Method.Post, 0) == 1;
                    //enableHtml = StringUtil.EqualsIgnoreCase(_Request.Get("contentFormat", Method.Post, ""), "enablehtml");
                    if (enableHtml == false)
                        enableMaxCode3 = true;
                }
                else if (AllowHtml)
                    enableHtml = true;
                else if (AllowMaxcode)
                    enableMaxCode3 = true;

                bool enableEmoticons = true;//(enableItems.IndexOf("enableemoticons", StringComparison.OrdinalIgnoreCase) > -1);
                bool enableSignature = true;//(enableItems.IndexOf("enablesignature", StringComparison.OrdinalIgnoreCase) > -1);
                bool enableReplyNotice = true;//(enableItems.IndexOf("enablereplynotice", StringComparison.OrdinalIgnoreCase) > -1);

                int threadCatalogID = _Request.Get<int>("threadCatalogs", Method.Post, 0);


                int threadID = 0, postID = 0;
                bool success = false;
                bool hasCatchError = false;
                try
                {
                    success = PostBOV5.Instance.CreateThread(
                        My, false, enableEmoticons, ForumID, threadCatalogID, 0, subject
                        , string.Empty, 0, posterName, false, false, content, enableHtml, enableMaxCode3, enableSignature
                        , enableReplyNotice, ipAddress, new AttachmentCollection(), out threadID, out postID
                        );

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
                                m_PageNumber = PageNumber;
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
                        if (IsAjaxRequest)
                        {
                            m_PageNumber = PageNumber;

                            AlertSuccess("操作成功");
                            _Request.Clear();
                        }
                        else
                        {
                            string returnUrl = BbsUrlHelper.GetForumUrl(Forum.CodeName, Action, PageNumber);
                            Response.Redirect(returnUrl);
                        }

                    }
                }
            }
        }

        #region  online

        protected override bool IsForumPage
        {
            get
            {
                return true;
            }
        }

        private int? m_ForumOnlineMemberCount;
        protected override int ForumOnlineMemberCount
        {
            get
            {
                if (m_ForumOnlineMemberCount == null)
                {
                    m_ForumOnlineMemberCount = OnlineMemberList.Count;
                }
                return m_ForumOnlineMemberCount.Value;
            }
        }

        private int? m_ForumOnlineGuestCount;
        protected override int ForumOnlineGuestCount
        {
            get
            {
                if (m_ForumOnlineGuestCount == null)
                {
                    m_ForumOnlineGuestCount = OnlineGuestList.Count;
                }
                return m_ForumOnlineGuestCount.Value;
            }
        }

        //在线会员
        private OnlineMemberCollection m_OnlineMemberList;
        protected override OnlineMemberCollection OnlineMemberList
        {
            get
            {
                OnlineMemberCollection onlineMemberList = m_OnlineMemberList;

                if (onlineMemberList == null)
                {
                    if (OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow)
                    {
                        onlineMemberList = OnlineUserPool.Instance.GetOnlineMembers(ForumID);
                        m_OnlineMemberList = onlineMemberList;
                    }
                }
                return onlineMemberList;
            }
        }


        //在线游客
        private OnlineGuestCollection m_OnlineGuestList;
        protected override OnlineGuestCollection OnlineGuestList
        {
            get
            {
                OnlineGuestCollection onlineGuestList = m_OnlineGuestList;

                if (onlineGuestList == null)
                {
                    if (OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow)
                    {
                        onlineGuestList = OnlineUserPool.Instance.GetOnlineGuests(ForumID);
                        m_OnlineGuestList = onlineGuestList;
                    }
                }
                return onlineGuestList;
            }
        }

        #endregion

    }
}