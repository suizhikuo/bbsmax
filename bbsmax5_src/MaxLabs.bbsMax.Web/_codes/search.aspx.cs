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
using System.Text.RegularExpressions;
namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    public partial class search : AppBbsPageBase
    {
        protected ThreadCollectionV5 threadList = new ThreadCollectionV5();
        protected PostCollectionV5 postList = new PostCollectionV5();
        protected SearchMode mode = SearchMode.Subject;
        protected string searchText = "";

        protected bool isShowActionButton = false;
        protected string searchString = "";

        protected bool isShowResult = false;
        protected int TotalCount;
        private ThreadCollectionV5 searchThreads;
        private List<int> canVisitForumIDs;

        private int m_MaxResultCount;
        private Guid m_SearchID;// = Guid.Empty;

        protected override string PageTitle
        {
            get { return "搜索 - " + base.PageTitle; }
        }

        protected override string NavigationKey
        {
            get { return "search"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("搜索");

            SearchSettings setting = AllSettings.Current.SearchSettings;

            if (IsLogin == false && setting.EnableGuestSearch == false)
            {
                ShowError("您是游客不允许使用搜索帖子的功能");
            }
            else if (IsLogin && setting.EnableSearch[My] == false)
            {
                ShowError("您所在的用户组不允许使用搜索帖子的功能");
            }

            canVisitForumIDs = ForumBO.Instance.GetForumIdsForVisit(My);

            if (canVisitForumIDs != null && canVisitForumIDs.Count == 0)
            {
                ShowError("您所在的用户组没有搜索帖子的权限");
            }

            //int maxResultCount = 0;

            if (IsLogin == false)
                m_MaxResultCount = setting.GuestMaxResultCount;
            else
                m_MaxResultCount = setting.MaxResultCount[My];

            if (_Request.Get("searchID", Method.Get) != null)
            {
                m_SearchID = _Request.Get<Guid>("searchID", Method.Get, Guid.Empty);
                if (m_SearchID == Guid.Empty)
                {
                    ShowError("非法的searchID");
                }
            }

            if (_Request.IsClick("SearchSubmit"))
            {
                ProcessSearch();
            }
            else
            {
                if (m_SearchID == Guid.Empty)
                {
                    mode = (SearchMode)_Request.Get<int>("Mode", Method.Get, 1);
                    string searchText = _Request.Get("SearchText", Method.Get, string.Empty, false);

                    if (mode == SearchMode.ThreadUserID || mode == SearchMode.PostUserID)
                    {
                        int userID;
                        if (int.TryParse(searchText, out userID) == false)
                        {
                            ShowError("用户ID必须为整数!");
                            return;
                        }

                        User user = UserBO.Instance.GetUser(userID);
                        if (user == null)
                        {
                            ShowError("不存在该用户!");
                            return;
                        }
                        else
                        {
                            if (mode == SearchMode.ThreadUserID)
                                mode = SearchMode.UserThread;
                            else
                                mode = SearchMode.UserPost;

                            m_SearchID = PostBOV5.Instance.SearchTopics(My, null, user.Username, mode, setting.SearchType, null, false, true, m_MaxResultCount);
                        }

                    }
                    else if (mode == SearchMode.Subject || mode == SearchMode.Content)
                    {
                        m_SearchID = PostBOV5.Instance.SearchTopics(My, null, HttpUtility.HtmlEncode(searchText), mode, setting.SearchType, null, false, true, m_MaxResultCount);
                    }
                    else if (mode == SearchMode.UserThread && searchText != string.Empty)
                    {
                        User user = UserBO.Instance.GetUser(searchText);
                        if (user == null)
                        {
                            ShowError("不存在该用户!");
                            return;
                        }
                        m_SearchID = PostBOV5.Instance.SearchTopics(My, null, searchText, mode, setting.SearchType, null, false, true, m_MaxResultCount);
                    }
                    if (m_SearchID != Guid.Empty)
                    {
                        string url = BbsUrlHelper.GetSearchIndexUrl() + "?searchID=" + m_SearchID.ToString();
                        ShowSuccess("搜索完毕, 稍后将转到搜索结果页面", url);
                    }

                }
                else
                {
                    ProcessGetThreads();
                }
            }

        }

        private void ProcessGetThreads()
        {
            SearchSettings setting = AllSettings.Current.SearchSettings;

            isShowResult = true;

            PostBOV5.Instance.SearchTopics(m_SearchID, setting.SearchPageSize, _Request.Get<int>("page", Method.Get, 1), setting.SearchType, m_MaxResultCount, out TotalCount, out searchText, out mode, out threadList, out postList);

            searchString = searchText;
            if (mode == SearchMode.UserPost || mode == SearchMode.UserThread)
            {
                User user = UserBO.Instance.GetUser(int.Parse(searchText));
                searchText = string.Empty;
                if (user != null)
                {
                    searchString = user.Username;
                }
            }

            if (threadList == null && postList == null)
            {
                ShowError("非法的searchID");
            }

            else if (mode == SearchMode.Content || mode == SearchMode.TopicContent || mode == SearchMode.UserPost)
            {
                List<int> threadIDs = new List<int>();
                foreach (PostV5 post in postList)
                {
                    if (threadIDs.Contains(post.ThreadID) == false)
                        threadIDs.Add(post.ThreadID);
                }

                searchThreads = PostBOV5.Instance.GetThreads(threadIDs);

                WaitForFillSimpleUsers<PostV5>(postList, 1);

                CheckThreads();
            }

            List<int> forumIDs = new List<int>();
            if (threadList != null)
            {
                foreach (BasicThread thread in threadList)
                {
                    if (forumIDs.Contains(thread.ForumID) == false)
                    {
                        ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(thread.ForumID);
                        if (managePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads)
                            || managePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled))
                        {
                            isShowActionButton = true;
                            break;
                        }
                        forumIDs.Add(thread.ForumID);
                    }
                }
            }

            if (isShowActionButton == false && postList != null)
            {
                foreach (PostV5 post in postList)
                {
                    if (forumIDs.Contains(post.ForumID) == false)
                    {
                        ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID);

                        if (managePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts))
                        {
                            isShowActionButton = true;
                            break;
                        }
                        forumIDs.Add(post.ForumID);
                    }
                }
            }

            PostBOV5.Instance.ProcessKeyword(threadList, ProcessKeywordMode.TryUpdateKeyword);
            PostBOV5.Instance.ProcessKeyword(postList, ProcessKeywordMode.TryUpdateKeyword);

            SetPager("list", UrlHelper.GetSearchUrl() + "?searchID=" + m_SearchID.ToString()+"&page={0}", _Request.Get<int>("page", Method.Get, 1), setting.SearchPageSize, TotalCount);
        }

        protected int SearchTime
        {
            get
            {
                if (IsLogin)
                    return AllSettings.Current.SearchSettings.SearchTime[My];
                else
                    return AllSettings.Current.SearchSettings.GuestSearchTime;
            }
        }

        private void ProcessSearch()
        {
            SearchSettings setting = AllSettings.Current.SearchSettings;

            mode = (SearchMode)_Request.Get<int>("Mode", Method.Post, 1);

            int[] forumIDs = _Request.GetList<int>("forumIDs", Method.Post, null);

            searchText = _Request.Get("SearchText", Method.Post, string.Empty, false);

            int searchtime = _Request.Get<int>("searchtime", Method.Post, 0);
            bool isbefore = _Request.Get<bool>("isbefore", Method.Post, false);
            bool isdesc = _Request.Get<bool>("isdesc", Method.Post, true);

            DateTime? postDate = null;
            if (searchtime > 0)
            {
                postDate = DateTimeUtil.Now.AddDays(0 - searchtime);
            }

            if (forumIDs != null)
            {
                bool isAll = false;
                foreach (int id in forumIDs)
                {
                    if (id == 0)
                    {
                        isAll = true;
                        break;
                    }
                }
                if (isAll)
                    forumIDs = null;
            }

            Guid searchResultID;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    searchResultID = PostBOV5.Instance.SearchTopics(My, forumIDs, searchText, mode, setting.SearchType, postDate, isbefore, isdesc, m_MaxResultCount);
                }
                catch(Exception ex)
                {
                    AlertError(ex.Message);
                    return;
                }
                if (searchResultID == Guid.Empty)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        //ShowError(error.Message);
                        AlertError(error.Message);
                    });
                }
                else
                {
                    m_SearchID = searchResultID;
                    //isShowResult = true;
                    ProcessGetThreads();
                    //Response.Redirect(BbsUrlHelper.GetSearchIndexUrl() + "?searchID=" + searchResultID.ToString());
                    AlertSuccess(searchResultID.ToString());
                }
            }

            //string url = BbsUrlHelper.GetSearchIndexUrl() + "?searchID=" + searchResultID.ToString();

            //ShowSuccess("搜索完毕, 稍后将转到搜索结果页面", url);
        }

        protected string SearchUrl
        {
            get
            {
                return BbsUrlHelper.GetSearchIndexUrl();
            }
        }

        protected BasicThread GetThread(int threadID)
        { 
            BasicThread thread;
            searchThreads.TryGetValue(threadID, out thread);
            return thread;
        }

        private List<int> m_hasBuyThreadIDs;
        private List<int> HasBuyThreadIDs
        {
            get
            {
                if (m_hasBuyThreadIDs == null)
                    CheckThreads();
                return m_hasBuyThreadIDs;
            }
        }

        private List<int> m_hasRepliedThreadIDs;
        private List<int> HasRepliedThreadIDs
        {
            get
            {
                if (m_hasRepliedThreadIDs == null)
                    CheckThreads();
                return m_hasRepliedThreadIDs;
            }
        }

        private void CheckThreads()
        {
            List<int> threadIDs = new List<int>();
            foreach (BasicThread thread in searchThreads)
            {
                threadIDs.Add(thread.ThreadID);
            }

            PostBOV5.Instance.CheckThreads(MyUserID, threadIDs, out m_hasBuyThreadIDs, out m_hasRepliedThreadIDs);

            foreach (int id in threadIDs)
            {
                if (My.RepliedThreads.ContainsKey(id))
                    continue;

                if (m_hasRepliedThreadIDs.Contains(id))
                    My.RepliedThreads.Add(id, true);
                else
                    My.RepliedThreads.Add(id, false);
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
            ForumBO.Instance.GetTreeForums("--", delegate(Forum forum)
            {
                if (canVisitForumIDs == null)
                    return true;
                else if (canVisitForumIDs.Contains(forum.ForumID))
                    return true;
                else
                    return false;

            }, out m_Forums, out m_ForumSeparators);

        }





        protected bool CanSearchTopicContent
        {
            get
            {
                if (IsLogin == false)
                    return AllSettings.Current.SearchSettings.GuestCanSearchTopicContent;
                else
                    return AllSettings.Current.SearchSettings.CanSearchTopicContent.GetValue(My);
            }
        }
        protected bool CanSearchAllPost
        {
            get
            {
                if (IsLogin == false)
                    return AllSettings.Current.SearchSettings.GuestCanSearchAllPost;
                else
                    return AllSettings.Current.SearchSettings.CanSearchAllPost.GetValue(My);
            }
        }
        protected bool CanSearchUserTopic
        {
            get
            {
                if (IsLogin == false)
                    return AllSettings.Current.SearchSettings.GuestCanSearchUserTopic;
                else
                    return AllSettings.Current.SearchSettings.CanSearchUserTopic.GetValue(My);
            }
        }
        protected bool CanSearchUserPost
        {
            get
            {
                if (IsLogin == false)
                    return AllSettings.Current.SearchSettings.GuestCanSearchUserPost;
                else
                    return AllSettings.Current.SearchSettings.CanSearchUserPost.GetValue(My);
            }
        }



        protected bool IsShowCheckBox(BasicThread thread)
        {
            return AllowManageThread(thread);
        }

        protected string GetPostActionLinks(string outputFormat)
        {
            if (!isShowActionButton)
                return string.Empty;
            return string.Format(outputFormat, Dialog + "/forum/deletepost.aspx", "删除选中回复");

        }
        protected string GetModeratorActionLinks(string outputFormat)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(outputFormat, Dialog + "/forum/recyclethread.aspx", "回收主题");
            sb.AppendFormat(outputFormat, Dialog + "/forum/deletethread.aspx", "删除主题");

            return sb.ToString();
        }

        protected bool AllowManageThread(BasicThread thread)
        {
            ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(thread.ForumID);

            if (managePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads, thread.PostUserID)
                || managePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled, thread.PostUserID))
                return true;

            return false;

        }
        protected bool AllowManagePost(PostV5 post)
        {
            if (post.PostType == PostType.ThreadContent)
            {
                return false;
            }

            ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID);

            if (managePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.DeletePosts, post.UserID))
                return true;

            return false;
        }



        private int maxContentLength = 150;
        protected string HighlightFormatter(string text)
        {

            text = ClearHTML(text);
            if (mode == SearchMode.UserThread || mode == SearchMode.UserPost)
            {
                if (text.Length > maxContentLength)
                    return text.Substring(0, maxContentLength) + "...";
                else
                    return text;
            }
            if ((mode == SearchMode.Content || mode == SearchMode.TopicContent) && text.Length > maxContentLength)
            {
                //截取第一个搜索字符串附近的文本
                int searchTextIndex = text.ToLower().IndexOf(searchText.ToLower());
                int l = (maxContentLength - searchText.Length) / 2;
                int l1 = searchTextIndex;
                int l2 = text.Length - (searchTextIndex + searchText.Length);
                if (l1 > l && l2 > l)
                {
                    text = text.Substring(searchTextIndex - l, maxContentLength) + "...";
                }
                else if (l1 <= l)
                {
                    text = text.Substring(0, maxContentLength) + "...";
                }
                else
                {
                    text = text.Substring(text.Length - maxContentLength) + "...";
                }
                return HighlightSearchText(text);
            }
            else
                return HighlightSearchText(text);
            //return text.Replace(this.searchText, "<font color=" + BBSSetting.HighlightColor + "><strong>" + this.searchText + "</strong></font>");
        }

        Regex regex_Highlight = null;
        protected string HighlightSearchText(string input)
        {
            string highlightColor = AllSettings.Current.SearchSettings.HighlightColor;

            if (regex_Highlight == null)
            {
                List<string> words = PostBOV5.Instance.GetSearchKeywords(searchText);

                if (words.Count == 0)
                    return input;

                string partten = null;
                foreach (string word in words)
                {
                    partten += word + "|";
                }

                partten = partten.Substring(0, partten.Length - 1);


                regex_Highlight = new Regex(@"(?<=(?:^|>)[^<>]*?)((" + partten + "))(?=[^<>]*?(?:<|$))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            }
            return regex_Highlight.Replace(input, "<font color=\"" + highlightColor + "\"><strong>$1</strong></font>");
            //return System.Text.RegularExpressions.Regex.Replace(input, searchText, "<font color=\""+BBSSetting.HighlightColor+"\"><strong>" + searchText + "</strong></font>",System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        protected string GetThreadLastPageUrl(BasicThread thread)
        {
            return BbsUrlHelper.GetThreadUrl(thread.Forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString, thread.TotalPages, 1);
        }

        protected string GetSearchThreadUrl(PostV5 post)
        {
            return BbsUrlHelper.GetSearchThreadUrl(post.Forum.CodeName, post.ThreadID, post.PostID, mode, searchText);
        }

        private Dictionary<int, bool> isAlwaysViewShieldContents = new Dictionary<int, bool>();
        protected bool IsAlwaysViewShieldContents(PostV5 post)
        {
            bool canView;
            if (isAlwaysViewShieldContents.TryGetValue(post.ForumID,out canView) == false)
            {
                canView = post.Forum.ManagePermission.Can(My, ManageForumPermissionSetNode.ActionWithTarget.AlwaysViewContents, post.UserID);
                isAlwaysViewShieldContents.Add(post.ForumID, canView);
            }

            return canView;
        }
        private Dictionary<int, bool> isAlwaysViewContents = new Dictionary<int, bool>();
        protected bool IsAlwaysViewContents(PostV5 post)
        {
            bool canView;
            if (isAlwaysViewContents.TryGetValue(post.ForumID, out canView))
            {
                canView = post.Forum.Permission.Can(My, ForumPermissionSetNode.Action.AlwaysViewContents);
                isAlwaysViewContents.Add(post.ForumID, canView);
            }

            return canView;
        }

        protected bool HasBuyed(PostV5 post)
        {
            if (post.PostType != PostType.ThreadContent)
                return true;

            BasicThread thread = searchThreads.GetValue(post.ThreadID);
            if (thread == null)
                return true;

            if (thread.Price == 0 || thread.PostUserID == MyUserID || thread.IsOverSellDays(thread.Forum.ForumSetting))
                return true;

            return HasBuyThreadIDs.Contains(post.ThreadID);
        }

        private Dictionary<int, bool> viewPost = new Dictionary<int, bool>();
        protected bool CanViewPost(PostV5 post)
        {
            bool can;

            if (viewPost.TryGetValue(post.ForumID, out can) == false)
            {
                if (post.Forum.Permission.Can(My, ForumPermissionSetNode.Action.ViewReply))
                {
                    can = true;
                }
                else
                    can = false;
                viewPost.Add(post.ForumID, can);
            }

            return can;
        }

        protected bool IsShieldedUser(PostV5 post)
        {
            if (UserBO.Instance.IsBanned(post.UserID, post.ForumID))
                return true;

            ForumPermissionSetNode permission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(post.ForumID);

            return permission.Can(post.UserID, ForumPermissionSetNode.Action.DisplayContent) == false;
        }

        private Dictionary<int, bool> canViewThreads = new Dictionary<int, bool>();
        protected bool CanViewThread(int forumID)
        {
            bool can;
            if (canViewThreads.TryGetValue(forumID, out can) == false)
            {
                can = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(forumID).Can(My, ForumPermissionSetNode.Action.ViewThread);
                canViewThreads.Add(forumID, can);
            }

            return can;
        }

        private Dictionary<int, bool> canViewValueThreads = new Dictionary<int, bool>();
        protected bool CanViewValuedThread(PostV5 post)
        {
            if (post.PostType != PostType.ThreadContent)
                return true;

            bool can;

            if (canViewValueThreads.TryGetValue(post.ForumID, out can) == false)
            {
                BasicThread thread = searchThreads.GetValue(post.ThreadID);
                if (thread.IsValued == false)
                    can = true;
                else
                    can = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(post.ForumID).Can(My, ForumPermissionSetNode.Action.ViewValuedThread);

                canViewValueThreads.Add(post.ForumID, can);
            }

            return can;
        }

    }
}