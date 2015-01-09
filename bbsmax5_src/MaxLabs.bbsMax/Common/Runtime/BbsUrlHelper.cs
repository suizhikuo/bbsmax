//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Common
{
    public class BbsUrlHelper
    {

        public static string GetUserSpaceUrl(int userID)
        {
            return BbsRouter.GetUrl("space/" + userID); 
        }
        public static string GetUserSpaceUrl(string format)
        {
            return BbsRouter.GetUrl("space/" + format);
        }

#if !Passport

        public static string GetArchiverIndexUrl()
        {
//#if V5
//            return Globals.AppRoot + "/archiver/default.aspx";
//#else
            return BbsRouter.GetUrl("archiver/default");
//#endif
            //return UrlManager.GetFriendlyUrl("Archiver_Default.html", true);
        }
        //--------------------------------------------------------------------------------
        public static string GetArchiverForumUrl(string codeName)
        {
//#if V5
//            return Globals.AppRoot + "/archiver/showforum.aspx?codename=" + codeName;
//#else
           // return GetArchiverForumUrl(codeName, 1);
            return BbsRouter.GetUrl(string.Format("archiver/{0}/list-{1}", codeName, 1));
//#endif
            //return string.Format(UrlManager.GetFriendlyUrl("Archiver_ShowForum.html?codename={0}", true), codeName);
        }
        public static string GetArchiverForumUrlForPager(string codeName)
        {
//#if V5
//            return Globals.AppRoot + "/archiver/showforum.aspx?codename=" + codeName + "&page={0}";
//#else
            return BbsRouter.GetUrl(string.Format("archiver/{0}/list-{1}", codeName, "{0}"));
            //string forumUrl = UrlManager.GetFriendlyUrl("Archiver_ShowForum.html?codename={0}&page={1}", true);
//#endif

            //return string.Format(forumUrl, codeName, "{0}");
        }
        public static string GetArchiverForumUrl(string codeName, int page)
        {
//#if V5
//            return Globals.AppRoot + "/archiver/showforum.aspx?codename=" + codeName + "&page=" + page;
//#else
            return BbsRouter.GetUrl(string.Format("archiver/{0}/list-{1}", codeName, 1));
            
            //string forumUrl = UrlManager.GetFriendlyUrl("Archiver_ShowForum.html?codename={0}&page={1}", true);
//#endif
            //return string.Format(forumUrl, codeName, page);
        }

        public static string GetThreadManageLogsUrl(string codeName, int searchType, string keyWord)
        {
            return BbsRouter.GetUrl("threadmanagelogs") + "?codename=" + codeName + "&searchType=" + searchType + "&keyWord=" + keyWord;

            //return BbsRouter.GetUrl(string.Format("threadmanagelogs/{0}/{1}/{2}",codeName,searchType,keyWord));

            //string url = UrlManager.GetFriendlyUrl("ThreadManageLogs.html?codename={0}&searchType={1}&keyWord={2}");

            //return string.Format(url, codeName, searchType.ToString(), keyWord);
        }
        public static string GetThreadManageLogsUrlForPager(string codeName, int searchType, string keyWord)
        {
            return GetThreadManageLogsUrl(codeName, searchType, keyWord) + "&page={0}";
            //return BbsRouter.GetUrl(string.Format("threadmanagelogs/{0}/{1}/{2}/{3}", codeName, searchType, keyWord,"{0}"));
            //string url = UrlManager.GetFriendlyUrl("ThreadManageLogs.html?codename={0}&searchType={1}&keyWord={2}&page={3}");

            //return string.Format(url, codeName, searchType.ToString(), keyWord, "{0}");
        }

        public static string GetForumUrl(string codeName)
        {
            return GetForumUrl(codeName, "list", 1);
            //return BbsRouter.GetUrl(string.Format("{0}/list-1", codeName));
            //return string.Format(UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}", true), codeName);
        }
        public static string GetForumUrlForPager(string codeName, string action)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{1}-{2}", codeName, action, "{0}"));
            //string forumUrl = UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}&action={1}&page={2}", true);

            //return string.Format(forumUrl, codeName, action, "{0}");
        }

        public static string GetForumUrl(string codeName, string action, int page)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{1}-{2}", codeName, action, page));
            //string forumUrl = UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}&action={1}&page={2}", true);

            //return string.Format(forumUrl, codeName, action, page);
        }

        public static string GetThreadCatalogUrlForPager(string codeName, int threadCatalogID)
        {
            return BbsRouter.GetUrl(string.Format("{0}/catalog-{1}-{2}", codeName, threadCatalogID, "{0}"));
            //string catalogUrl = UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}&ThreadCatalogID={1}&page={2}", true);

            //return string.Format(catalogUrl, codeName, threadCatalogID, "{0}");
        }

        public static string GetThreadCatalogUrl(string codeName, int threadCatalogID, int page)
        {
            return BbsRouter.GetUrl(string.Format("{0}/catalog-{1}-{2}", codeName, threadCatalogID, page));
            //string catalogUrl = UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}&ThreadCatalogID={1}&page={2}", true);

            //return string.Format(catalogUrl, codeName, threadCatalogID, page);
        }

        public static string GetForumEmoticonUrlForPager(string codeName, int threadCatalogID, int page, bool IsGetAllDefaultEmoticon, int emoticonGroupID)
        {
            return GetForumUrl(codeName) + "?ThreadCatalogID="+threadCatalogID+"&page="+page+"&IsGetAllDefaultEmoticon="+IsGetAllDefaultEmoticon+"&EmoticonGroupID="+emoticonGroupID+"&emoticonPage={0}";
            //return string.Empty;
            //return BbsRouter.GetUrl(string.Format("forum/{0}/cid-{1}/gde-{2}/gid-{3}-", codeName, action, page));
            //string catalogUrl = UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}&ThreadCatalogID={1}&page={2}&IsGetAllDefaultEmoticon={3}&EmoticonGroupID={4}&emoticonPage={5}", true);

            //return string.Format(catalogUrl, codeName, threadCatalogID, page, IsGetAllDefaultEmoticon, emoticonGroupID, "{0}");
        }
        public static string GetForumEmoticonUrlForPager(string codeName, string action, int page, bool IsGetAllDefaultEmoticon, int emoticonGroupID)
        {
            return GetForumUrl(codeName, action, page) + "?IsGetAllDefaultEmoticon="+IsGetAllDefaultEmoticon+"&EmoticonGroupID="+emoticonGroupID+"&emoticonPage={0}";
            //string catalogUrl = UrlManager.GetFriendlyUrl("ShowForum.html?codename={0}&action={1}&page={2}&IsGetAllDefaultEmoticon={3}&EmoticonGroupID={4}&emoticonPage={5}", true);

            //return string.Format(catalogUrl, codeName, action, page, IsGetAllDefaultEmoticon, emoticonGroupID, "{0}");
        }

        public static string GetNewThreadListUrlForPager()
        {
            return BbsRouter.GetUrl("new/{0}");
            //string url = UrlManager.GetFriendlyUrl("NewThreads.html?page={0}", true);
            //return url;
        }

        //--------------------------------------------------------------------------------

        public static string GetSystemForumUrl()
        {
            return BbsRouter.GetUrl("systemforum");
            //return string.Format(UrlManager.GetFriendlyUrl("ShowSystemForum.html?type={0}", true), systemForumType.ToString());
        }

        public static string GetSystemForumUrl(SystemForum systemForumType)
        {
            return BbsRouter.GetUrl(string.Format("systemforum/{0}", systemForumType.ToString()));
            //return string.Format(UrlManager.GetFriendlyUrl("ShowSystemForum.html?type={0}", true), systemForumType.ToString());
        }
        public static string GetSystemForumUrlForPager(SystemForum systemForumType)
        {
            return BbsRouter.GetUrl(string.Format("systemforum/{0}/{1}", systemForumType.ToString(),"{0}"));
            //string forumUrl = UrlManager.GetFriendlyUrl("ShowSystemForum.html?type={0}&page={1}", true);

            //return string.Format(forumUrl, systemForumType.ToString(), "{0}");
        }

        //public static string GetSystemForumUrl(SystemForum systemForumType, string forumCodeName)
        //{
        //    int forumID = ForumManager.GetForumID(forumCodeName);
        //    return GetSystemForumUrl(systemForumType,forumID);
        //}
        //public static string GetSystemForumUrl(SystemForum systemForumType, string forumIDString)
        //{
        //    return UrlManager.GetFriendlyUrl("ShowSystemForum.html?type=" + systemForumType.ToString() + "&ForumID=" + forumIDString);
        //}
        public static string GetSystemForumUrl(SystemForum systemForumType, string codeName)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{1}-1", codeName, systemForumType.ToString()));
            //return string.Format(UrlManager.GetFriendlyUrl("ShowSystemForum.html?type={0}&Codename={1}", true), systemForumType.ToString(), codeName);
        }
        public static string GetSystemForumUrlForPager(SystemForum systemForumType, string codeName)
        {
            return BbsRouter.GetUrl(string.Format("systemforum/{0}/code-{1}/{2}", systemForumType.ToString(), codeName,"{0}"));
            //string forumUrl = UrlManager.GetFriendlyUrl("ShowSystemForum.html?type={0}&Codename={1}&page={2}", true);

            //return string.Format(forumUrl, systemForumType.ToString(), codeName, "{0}");
        }

        //public static string GetPostMarkUrl(int postID)
        //{
        //    return BbsRouter.GetUrl(string.Format("moderatorcenter/pid-{0}/action-{1}", postID, "rate"));
        //    //return string.Format(UrlManager.GetFriendlyUrl("ModeratorCenter.html?postID={0}&Action=Marking", true), postID);
        //}

        //public static string GetCancelPostMarkUrl(int postID)
        //{
        //    return BbsRouter.GetUrl(string.Format("moderatorcenter/pid-{0}/action-{1}", postID, "cancelrate"));
        //    //return string.Format(UrlManager.GetFriendlyUrl("ModeratorCenter.html?postID={0}&Action=Marking", true), postID);
        //}

        public static string GetModeratorCenterUrl()
        {
            return BbsRouter.GetUrl("moderatorcenter");
        }

        public static string GetModeratorCenterUrl(string codeName, string action)
        {
            return GetThreadManagerUrl(codeName, action);
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}", codeName, action));
            //string forumUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?CodeName={0}&action={1}");
            //return string.Format(forumUrl, codeName, action);
        }
        //public static string GetModeratorCenterManageRoleUrl(string codeName, string action, int roleID)
        //{
        //    return string.Empty;
        //    //string forumUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?CodeName={0}&action={1}&RoleID={2}");
        //    //return string.Format(forumUrl, codeName, action, roleID);
        //}
        /// <summary>
        /// 获得版主管理中心的url
        /// </summary>
        /// <param name="type">type of SystemForum</param>
        /// <param name="codeName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string GetModeratorCenterUrl(SystemForum type, string codeName, string action)
        {
            return GetThreadManagerUrl(codeName, action, type.ToString());
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/type-{2}", codeName, action, type.ToString()));
            //string forumUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?type={0}&CodeName={1}&action={2}");
            //return string.Format(forumUrl, type.ToString(), codeName, action);
        }
        //public static string GetModeratorCenterUrlForPager(string codeName, string action, int roleID)
        //{
        //    return string.Empty;
        //    //string url = UrlManager.GetFriendlyUrl("ModeratorCenter.html?codename={0}&Action={1}&RoleID={2}&page={3}");

        //    //return string.Format(url, codeName, action, roleID, "{0}");
        //}
        public static string GetModeratorCenterSearchUrlForPager(string codeName, string action, int searchType, string keyWord)
        {
            return GetModeratorCenterUrl() + string.Format("?codename={0}&action={1}&searchType={2}&keyWord={3}&page={4}", codeName, action, searchType, keyWord, "{0}");
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/stype-{2}/keyword-{3}/{4}", codeName, action,searchType,keyWord,"{0}"));
            //string url = UrlManager.GetFriendlyUrl("ModeratorCenter.html?codename={0}&action={1}&searchType={2}&keyWord={3}&page={4}");

            //return string.Format(url, codeName, action, searchType.ToString(), keyWord, "{0}");
        }
        public static string GetModeratorCenterSearchUrl(string codeName, string action, int searchType, string keyWord)
        {
            return GetModeratorCenterUrl() + string.Format("?codename={0}&action={1}&searchType={2}&keyWord={3}", codeName, action, searchType, keyWord);
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/stype-{2}/keyword-{3}", codeName, action, searchType, keyWord));
            //string url = UrlManager.GetFriendlyUrl("ModeratorCenter.html?codename={0}&action={1}&searchType={2}&keyWord={3}");

            //return string.Format(url, codeName, action, searchType.ToString(), keyWord);
        }

        public static string GetThreadManagerUrl(string codeName, string action)
        {
            return GetModeratorCenterUrl() + string.Format("?codename={0}&action={1}", codeName, action);
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}", codeName, action));
            //string threadManagerUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?codename={0}&action={1}", true);
            //return string.Format(threadManagerUrl, codeName, action);
        }

        public static string GetThreadManagerUrl(string codeName, string action, string type)
        {
            return GetModeratorCenterUrl() + string.Format("?codename={0}&action={1}&type={2}", codeName, action, type);
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/type-{2}", codeName, action, type));
            //string threadManagerUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?codename={0}&action={1}&type={2}", true);
            //return string.Format(threadManagerUrl, codeName, action, type);
        }

        public static string GetDeletePostUrl(string codeName, string action, int threadID)
        {
            return GetModeratorCenterUrl() + string.Format("?codename={0}&action={1}&threadid={2}", codeName, action, threadID);
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/tid-{2}", codeName, action, threadID));
            //string topicAdminUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?CodeName={0}&Action={1}&ThreadID={2}", true);
            //return string.Format(topicAdminUrl, codeName, action, threadID);
        }
        public static string GetDeletePostUrl(string codeName, string action, int threadID, int postID)
        {
            return GetModeratorCenterUrl() + string.Format("?codename={0}&action={1}&threadid={2}&postid={3}", codeName, action, threadID, postID);
            //return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/tid-{2}/pid-{3}", codeName, action, threadID, postID));
            //string topicAdminUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?CodeName={0}&Action={1}&ThreadID={2}&PostID={3}", true);
            //return string.Format(topicAdminUrl, codeName, action, threadID, postID);
        }
        //public static string GetDeleteOwnPostUrl(string codeName, string action, int threadID)
        //{
        //    string topicAdminUrl = UrlManager.GetFriendlyUrl("ThreadManager.html?CodeName={0}&Action={1}&ThreadID={2}", true);
        //    return string.Format(topicAdminUrl, codeName, action, threadID);
        //}
        public static string GetActionShieldUserUrl(int userID)
        {
            return UrlUtil.JoinUrl(Globals.AppRoot, "max-dialogs/user-shield.aspx?userID=" + userID);
            //string topicAdminUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?CodeName={0}&Action={1}&ThreadID={2}&pageIndex={3}&UserID={4}", true);
            //return string.Format(topicAdminUrl, codeName, action, threadID, pageIndex, userID);
        }
        public static string GetActionShieldPostUrl(string codeName, string action, int threadID, int postID, int pageIndex)
        {
            return BbsRouter.GetUrl(string.Format("moderatorcenter/code-{0}/action-{1}/tid-{2}/pid-{3}/pindex-{4}", codeName, action, threadID, postID, pageIndex));
            //string topicAdminUrl = UrlManager.GetFriendlyUrl("ModeratorCenter.html?CodeName={0}&Action={1}&ThreadID={2}&PostID={3}&pageIndex={4}", true);
            //return string.Format(topicAdminUrl, codeName, action, threadID, postID, pageIndex);
        }
        //--------------------------------------------------------------------------------
        //public static string GetMyThreadsUrl(MyThreadType myThreadType)
        //{
        //    return GetMyThreadsUrl(myThreadType.ToString());
        //}
        //public static string GetMyThreadsUrl(string myThreadType)
        //{
        //    return BbsRouter.GetUrl(string.Format("my/mythreads?type={0}", myThreadType));
        //    //return string.Format(UrlManager.GetFriendlyUrl("ShowMyThreads.html?type={0}", true), myThreadType.ToString());
        //}
        //public static string GetMyThreadsUrlForPager(MyThreadType myThreadType)
        //{
        //    return BbsRouter.GetUrl(string.Format("my/mythreads?type={0}&page={1}", myThreadType.ToString(), "{0}"));
        //    //return string.Format(UrlManager.GetFriendlyUrl("ShowMyThreads.html?type={0}&page={1}", true), myThreadType.ToString(), "{0}");
        //}
        //public static string GetMyThreadsUrl()
        //{
        //    return BbsRouter.GetUrl("my/mythreads");
        //    //return string.Format(UrlManager.GetFriendlyUrl("ShowMyThreads.html?type={0}", true), myThreadType.ToString());
        //}
        //--------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------
        public static string GetUnapprovedPostThreadUrl(string codeName, int threadID, int forumID)
        {
            return GetThreadUrl(codeName, threadID) + "?type=unapprovepost&forumID=" + forumID;

            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{3}/type-{1}/fid-{2}", codeName, "unapprovepost", forumID, threadID));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&type=unapprovepost&forumID={1}&ThreadID={2}", true);
            //return string.Format(threadUrl, codeName, forumID, threadID);
        }
        public static string GetUnapprovedPostThreadUrlForPager(string codeName, int threadID, int forumID)
        {
            return GetThreadUrl(codeName, threadID) + "?type=unapprovepost&forumID=" + forumID + "&page={0}";
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{3}/type-{1}/fid-{2}/{4}", codeName, "unapprovepost", forumID, threadID,"{0}"));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&type=unapprovepost&forumID={1}&ThreadID={2}&page={3}", true);
            //return string.Format(threadUrl, codeName, forumID, threadID, "{0}");
        }

        public static string GetArchiverThreadUrl(string codeName, int threadID)
        {
//#if V5
//            return Globals.AppRoot + "/archiver/showthread.aspx?codename=" + codeName + "&threadid=" + threadID;
//#else
            return BbsRouter.GetUrl(string.Format("archiver/{0}/thread-{1}-{2}", codeName, threadID, 1));
//#endif
            //string threadUrl = UrlManager.GetFriendlyUrl("Archiver_ShowThread.html?codename={0}&ThreadID={1}", true);
            //return string.Format(threadUrl, codeName, threadID);
        }
        public static string GetArchiverThreadUrlForPager(string codeName, int threadID)
        {
//#if V5
//            return Globals.AppRoot + "/archiver/showthread.aspx?codename=" + codeName + "&threadid=" + threadID + "&page={0}";
//#else
            return BbsRouter.GetUrl(string.Format("archiver/{0}/thread-{1}-{2}", codeName, threadID, "{0}"));
            //string threadUrl = UrlManager.GetFriendlyUrl("Archiver_ShowThread.html?codename={0}&ThreadID={1}&page={2}", true);
            //return string.Format(threadUrl, codeName, threadID, "{0}");
//#endif
        }
        public static string GetThreadUrlForPager(string codeName, int threadID, string threadTypeString)
        {
            return GetThreadUrlForPager(codeName, threadID, 1, threadTypeString);
        }


        public static string GetThreadUrlForPager(string codeName, int threadID, int listPage, string threadTypeString)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{4}-{1}-{2}-{3}", codeName, threadID, "{0}", listPage, threadTypeString));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&listPage={3}", true);
            //return string.Format(threadUrl, codeName, threadID, "{0}", listPage);
        }


        //public static string GetThreadUrl(string codeName, int threadID, int page)
        //{
        //    string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}", true);
        //    return string.Format(threadUrl, codeName, threadID, page);
        //}

        public static string GetThreadUrl(string codeName, int threadID)
        {
            return GetThreadUrl(codeName, threadID, 1, 1);
        }

        public static string GetThreadUrl(string codeName, int threadID, int page)
        {
            return GetThreadUrl(codeName, threadID, page, 1);
        }

        public static string GetThreadUrl(string codeName, int threadID, int page, int listPage)
        {
            return BbsRouter.GetUrl(string.Format("{0}/thread-{1}-{2}-{3}", codeName, threadID, page, listPage));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&listPage={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, listPage);
        }


        public static string GetThreadUrl(string codeName, int threadID,string threadType)
        {
            return GetThreadUrl(codeName, threadID, threadType, 1, 1);
        }

        public static string GetThreadUrl(string codeName, int threadID, string threadType, int page)
        {
            return GetThreadUrl(codeName, threadID, threadType, page, 1);
        }

        public static string GetThreadUrl(string codeName, int threadID, string threadType, int page, int listPage)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{4}-{1}-{2}-{3}", codeName, threadID, page, listPage, threadType));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&listPage={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, listPage);
        }

        public static string GetThreadUrlForExtParms(string codeName, int threadID, string threadType, int page, string extParms)
        {
            return GetThreadUrl(codeName, threadID, threadType, page) + "?extParms=" + System.Web.HttpContext.Current.Server.UrlEncode(extParms);

            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/ext-{3}", codeName, threadID, page, System.Web.HttpContext.Current.Server.UrlEncode(extParms)));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&extParms={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, System.Web.HttpContext.Current.Server.UrlEncode(extParms));
        }


        public static string GetThreadUrlForExtParms(string codeName, int threadID, int page, string extParms)
        {
            return GetThreadUrl(codeName, threadID, page) + "?extParms=" + System.Web.HttpContext.Current.Server.UrlEncode(extParms);

            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/ext-{3}", codeName, threadID, page, System.Web.HttpContext.Current.Server.UrlEncode(extParms)));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&extParms={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, System.Web.HttpContext.Current.Server.UrlEncode(extParms));
        }

        public static string GetLastThreadUrl(string codeName, int threadID, int lastPostID, int page, bool isLast)
        {
            string last = isLast ? ("last") : (lastPostID.ToString());
            return BbsRouter.GetUrl(string.Format("{0}/thread-{1}-{2}", codeName, threadID, page)) + "#" + last;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}", true);
            //return string.Format(threadUrl, codeName, threadID, page) + (isLast ? ("#last") : ("#" + lastPostID));
        }
        public static string GetLastThreadUrl(string codeName, int threadID, int lastPostID, int page, string searthText, bool isLast)
        {
            string last = isLast ? ("last") : (lastPostID.ToString());
            return GetThreadUrl(codeName, threadID, page) + "?SearchText=" + searthText + "#" + last;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/keyword-{3}", codeName, threadID, page, searthText)) + "?#" + last;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&SearchText={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, searthText) + (isLast ? ("#last") : ("#" + lastPostID));
        }

        public static string GetLastThreadUrl(string codeName, int threadID, string threadType, int lastPostID, int page, bool isLast)
        {
            string last = isLast ? ("last") : (lastPostID.ToString());

            return BbsRouter.GetUrl(string.Format("{0}/{3}-{1}-{2}", codeName, threadID, page, threadType)) + "#" + last;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}", true);
            //return string.Format(threadUrl, codeName, threadID, page) + (isLast ? ("#last") : ("#" + lastPostID));
        }
        public static string GetLastThreadUrl(string codeName, int threadID, string threadType, int lastPostID, int page, string searthText, bool isLast)
        {
            string last = isLast ? ("last") : (lastPostID.ToString());
            return BbsRouter.GetUrl(string.Format("{0}/{3}-{1}-{2}", codeName, threadID, page, threadType)) + "?SearchText=" + searthText + "#" + last;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/keyword-{3}", codeName, threadID, page, searthText)) + "?#" + last;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&SearchText={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, searthText) + (isLast ? ("#last") : ("#" + lastPostID));
        }

        /// <summary>
        /// 跟有参数type
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="threadID"></param>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetThreadUrl(string codeName, int threadID, string threadType, int page, string type)
        {
            return GetThreadUrl(codeName, threadID, threadType, page) + "?type=" + type;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/type-{3}", codeName, threadID, page, type));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&type={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, type);
        }


        /// <summary>
        ///  只看楼主
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="threadID"></param>
        /// <param name="userID"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetThreadUrl(string codeName, int threadID, int page, int userID, string threadTypeString, int listPage)
        {
            return GetThreadUrl(codeName, threadID, threadTypeString, page, listPage) + "?userid=" + userID;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/lp-{4}/uid-{3}", codeName, threadID, page, userID, listPage));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&userID={3}&listPage={4}", true);
            //return string.Format(threadUrl, codeName, threadID, page, userID, listPage);
        }
        //public static string GetThreadUrlForPager(string codeName, int threadID, int userID)
        //{
        //    return GetThreadUrlForPager(codeName,threadID,userID,1);
        //}
        public static string GetThreadUrlForPager(string codeName, int threadID, int userID, int listPage, string threadTypeString)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{4}-{1}-{2}-{3}", codeName, threadID, "{0}", listPage, threadTypeString)) + "?userid=" + userID;
            //return GetThreadUrl(codeName, threadID, listPage) + "?userid=" + userID + "&page={0}";
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/lp-{4}/uid-{3}", codeName, threadID, "{0}", userID, listPage));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&userID={3}&listPage={4}", true);
            //return string.Format(threadUrl, codeName, threadID, "{0}", userID, listPage);
        }
        public static string GetThreadUrlForPager(string codeName, int threadID, int userID, string threadTypeString, string extParms)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{4}-{1}-{2}-{3}", codeName, threadID, "{0}", 1, threadTypeString)) + "?userid=" + userID + "&extParms=" + System.Web.HttpContext.Current.Server.UrlEncode(extParms);

            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/ext-{4}/uid-{3}", codeName, threadID, "{0}", userID, System.Web.HttpContext.Current.Server.UrlEncode(extParms)));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&userID={3}&extParms={4}", true);
            //return string.Format(threadUrl, codeName, threadID, "{0}", userID, System.Web.HttpContext.Current.Server.UrlEncode(extParms));
        }
        public static string GetThreadUrlForPagerExtParms(string codeName, int threadID, string threadTypeString, string extParms)
        {
            return BbsRouter.GetUrl(string.Format("{0}/{4}-{1}-{2}-{3}", codeName, threadID, "{0}", 1, threadTypeString)) + "?extParms=" + System.Web.HttpContext.Current.Server.UrlEncode(extParms);

            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/ext-{3}", codeName, threadID, "{0}", System.Web.HttpContext.Current.Server.UrlEncode(extParms)));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&extParms={3}", true);
            //return string.Format(threadUrl, codeName, threadID, "{0}", System.Web.HttpContext.Current.Server.UrlEncode(extParms));
        }

        public static string GetThreadUrlForEmoticonPager(string codeName, int threadID, int page, int listPage, string threadTypeString, bool IsGetAllDefaultEmoticon, int emoticonGroupID)
        {
            return GetThreadUrl(codeName, threadID, threadTypeString, page, listPage) + "?IsGetAllDefaultEmoticon="+IsGetAllDefaultEmoticon+"&EmoticonGroupID="+emoticonGroupID+"&emoticonPage={0}";
            //return string.Empty;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&listPage={3}&IsGetAllDefaultEmoticon={4}&EmoticonGroupID={5}&emoticonPage={6}", true);
            //return string.Format(threadUrl, codeName, threadID, page, listPage, IsGetAllDefaultEmoticon, emoticonGroupID, "{0}");
        }
        public static string GetThreadUrlForEmoticonPager(string codeName, int threadID, int userID, int page, int listPage, string threadTypeString, bool IsGetAllDefaultEmoticon, int emoticonGroupID)
        {
            return GetThreadUrl(codeName, threadID, page, userID, threadTypeString, listPage) + "&IsGetAllDefaultEmoticon=" + IsGetAllDefaultEmoticon + "&EmoticonGroupID=" + emoticonGroupID + "&emoticonPage={0}";
            //return string.Empty;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&userID={3}&listPage={4}&IsGetAllDefaultEmoticon={5}&EmoticonGroupID={6}&emoticonPage={7}", true);
            //return string.Format(threadUrl, codeName, threadID, page, userID, listPage, IsGetAllDefaultEmoticon, emoticonGroupID, "{0}");
        }
        public static string GetThreadUrlForEmoticonPager(string codeName, int threadID, int userID, int page, string threadTypeString, string extParms, bool IsGetAllDefaultEmoticon, int emoticonGroupID)
        {
            return GetThreadUrl(codeName, threadID, page, userID, threadTypeString, 1) + "&extParms=" + System.Web.HttpContext.Current.Server.UrlEncode(extParms) + "&IsGetAllDefaultEmoticon=" + IsGetAllDefaultEmoticon + "&EmoticonGroupID=" + emoticonGroupID + "&emoticonPage={0}";
            //return string.Empty;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&userID={3}&extParms={4}&IsGetAllDefaultEmoticon={5}&EmoticonGroupID={6}&emoticonPage={7}", true);
            //return string.Format(threadUrl, codeName, threadID, page, userID, System.Web.HttpContext.Current.Server.UrlEncode(extParms), IsGetAllDefaultEmoticon, emoticonGroupID, "{0}");
        }

        public static string GetThreadUrlForEmoticonPager(string codeName, int threadID, string type, int page, string threadTypeString, bool IsGetAllDefaultEmoticon, int emoticonGroupID)
        {
            return GetThreadUrl(codeName, threadID, page, threadTypeString, type) + "&IsGetAllDefaultEmoticon=" + IsGetAllDefaultEmoticon + "&EmoticonGroupID=" + emoticonGroupID + "&emoticonPage={0}";
            //return string.Empty;
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&type={3}&IsGetAllDefaultEmoticon={4}&EmoticonGroupID={5}&emoticonPage={6}", true);
            //return string.Format(threadUrl, codeName, threadID, page, type, IsGetAllDefaultEmoticon, emoticonGroupID, "{0}");
        }


        /// <summary>
        /// 跟有参数type
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="threadID"></param>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetThreadUrl(string codeName, int threadID, int page, string threadTypeString, string type)
        {
            return GetThreadUrl(codeName, threadID, threadTypeString, page) + "?type=" + type;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/type-{3}", codeName, threadID, page, type));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&type={3}", true);
            //return string.Format(threadUrl, codeName, threadID, page, type);
        }
        public static string GetThreadUrlForPager(string codeName, int threadID, string threadTypeString, string type)
        {
            return GetThreadUrlForPager(codeName, threadID, threadTypeString) + "?type=" + type;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/type-{3}", codeName, threadID, "{0}", type));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&page={2}&type={3}", true);
            //return string.Format(threadUrl, codeName, threadID, "{0}", type);
        }

        public static string GetSearchThreadUrl(string codeName, int threadID, int postID, SearchMode mode, string searchText)
        {
            return GetThreadUrl(codeName, threadID) + "?postID=" + postID + "&page=1&SearchMode=" + mode.ToString() + "&searchText=" + searchText;
            //return BbsRouter.GetUrl(string.Format("showthread/{0}/tid-{1}/{2}/pid-{3}/smode-{4}/keyword-{5}", codeName, threadID, 1, postID,mode.ToString(),searchText));
            //string threadUrl = UrlManager.GetFriendlyUrl("ShowThread.html?codename={0}&ThreadID={1}&postID={2}&page=1&SearchMode={3}&searchText={4}", true);
            //return string.Format(threadUrl, codeName, threadID, postID, mode.ToString(), searchText);
        }
        //--------------------------------------------------------------------------------


        
        //--------------------------------------------------------------------------------
        public static string GetCreatThreadUrl(string codeName, string action)
        {
            return BbsRouter.GetUrl(codeName + "/post") + "?action=" + action;
            //return BbsRouter.GetUrl(string.Format("post/{0}/{1}", codeName, action));
        }

        public static string GetCreatPostUrl(string codeName, string action, int threadID)
        {
            return BbsRouter.GetUrl(codeName + "/post") + "?action=" + action + "&threadid=" + threadID;
            //return BbsRouter.GetUrl(string.Format("post/{0}/{1}/{2}", codeName, action, threadID));
        }

        public static string GetCreatPostUrl(string codeName, string action, int threadID, int postID)
        {
            return GetCreatPostUrl(codeName, action, threadID, postID, null);
        }
        public static string GetCreatPostUrl(string codeName, string action, int threadID, int postID, string type)
        {
            if (string.IsNullOrEmpty(type))
                return BbsRouter.GetUrl(codeName + "/post") + "?action=" + action + "&threadid=" + threadID + "&postID=" + postID;
            //return BbsRouter.GetUrl(string.Format("post/{0}/{1}/{2}/{3}", codeName, action, threadID, postID));
            else
                return BbsRouter.GetUrl(codeName + "/post") + "?action=" + action + "&threadid=" + threadID + "&postID=" + postID + "&type=" + type;
                //return BbsRouter.GetUrl(string.Format("post/{0}/{1}/{2}/{3}/type-{4}", codeName, action, threadID, postID, type));
            //string createPostUrl = UrlManager.GetFriendlyUrl("Post.html?codename={0}&action={1}&threadID={2}&postID={3}", true);
            //return string.Format(createPostUrl, codeName, action, threadID, postID) + (string.IsNullOrEmpty(type) ? "" : "&type=" + type);
        }
        public static string GetReplyPostUrl(string codeName, int threadID, string postIndexAlias, int postID)
        {
            return GetCreatPostUrl(codeName, "replyPost", threadID, postID) + "&PostIndexAlias=" + postIndexAlias;
            //return BbsRouter.GetUrl(string.Format("post/{0}/{1}/{2}/{3}/pia-{4}", codeName, "replyPost", threadID, postID, postIndexAlias));
            //string createPostUrl = UrlManager.GetFriendlyUrl("Post.html?codename={0}&action=replyPost&ThreadID={1}&PostIndexAlias={2}&postID={3}", true);
            //return string.Format(createPostUrl, codeName, threadID, postIndexAlias, postID);
        }
        //----------------------------------------------------------
        public static string GetSearchUrlForPage(int fid, SearchMode mode, string searchText)
        {
            return GetSearchIndexUrl() + "?fid=" + fid + "&mode=" + (int)mode + "&page={0}&searchtext=" + searchText;
            //return BbsRouter.GetUrl(string.Format("search/{0}/{1}/{2}/{3}", fid, (int)mode, searchText, "{0}"));
            //string url = UrlManager.GetFriendlyUrl("Search.html?page={0}&fid={1}&mode={2}&searchText={3}", true);
            //return string.Format(url, "{0}", fid, (int)mode, searchText);
        }
        public static string GetSearchUrl(int fid, SearchMode mode, string searchText)
        {
            return GetSearchIndexUrl() + "?fid=" + fid + "&mode=" + (int)mode + "&searchtext=" + searchText;
            //string url = UrlManager.GetFriendlyUrl("Search.html?fid={0}&mode={1}&searchText={2}", true);
            //return string.Format(url, fid, (int)mode, searchText);
        }
        public static string GetSearchUrl(string searchText, string mode)
        {
            return GetSearchIndexUrl() + "?mode=" + mode + "&searchtext=" + searchText;
            //string searchUrl = UrlManager.GetFriendlyUrl("Search.html?searchText={0}&mode={1}", true);
            //return string.Format(searchUrl, searchText, mode);
        }
        public static string GetSearchIndexUrl()
        {
            return BbsRouter.GetUrl("search");
            //return UrlManager.GetFriendlyUrl("Search.html", true);
        }
        //---------------------------------------------------------------
        public static string GetSignInForumUrl(string codeName)
        {
            return BbsRouter.GetUrl(string.Format("{0}/signinforum", codeName));
            //return string.Format(UrlManager.GetFriendlyUrl("SignInForum.html?CodeName={0}", true), codeName);
        }

        public static string GetSignInForumUrl(string codeName, int threadID)
        {
            return GetSignInForumUrl(codeName) + "?threadid=" + threadID;
            //return string.Format(UrlManager.GetFriendlyUrl("SignInForum.html?CodeName={0}&ThreadID={1}", true), codeName, threadID);
        }
        //public static string GetSignInForumUrl(string codeName, string type)
        //{
        //    return string.Format(UrlManager.GetFriendlyUrl("SignInForum.html?forumID={0}&type={1}", true), codeName, type);
        //}

        public static string GetRssUrl(int forumID, string ticket)
        {
            return BbsRouter.GetUrl("Rss") + string.Format("?forumID={0}&ticket={1}", forumID, ticket);
            //string rssUrl = UrlManager.GetFriendlyUrl("Rss.html?forumID={0}&ticket={1}", true);
            //return string.Format(rssUrl, forumID, ticket);
        }
        public static string GetEmoticonUrl()
        {
            return string.Empty;
            //return UrlManager.GetFriendlyUrl("EmoticonCenter.html", true);
        }

        public static string GetFinalQuestionUrl(int threadID, int forumID)
        {
            return BbsRouter.GetUrl(string.Format("finalquestion/{0}/{1}", forumID, threadID));
            //return string.Format(UrlManager.GetFriendlyUrl("FinalQuestion.html?threadID={0}&forumID={1}", true), threadID, forumID);
        }

        //public static string GetAttachmentUrl(int attachmentID)
        //{
        //    return string.Empty;
        //    //return string.Format(Globals.ApplicationUrl + "/Attachment.aspx?ID={0}", attachmentID);
        //}
        public static string GetAttachmentUrl(object attachmentID)
        {
            return BbsRouter.GetUrl("handler/down") + "?action=attach&id=" + attachmentID.ToString();
            //return string.Format(Globals.ApplicationUrl + "/Attachment.aspx?ID={0}", attachmentID);
        }
        public static string GetDiskAttachmentUrl(object diskFileID)
        {
            return BbsRouter.GetUrl("handler/down") + "?action=attach&diskfileid=" + diskFileID.ToString();
        }
        public static string GetDiskImageAttachmentUrl(object diskFileID)
        {
            return BbsRouter.GetUrl("handler/down") + "?action=attach&mode=image&diskfileid=" + diskFileID.ToString();
        }
        public static string GetV30AttachmentUrl(object attachmentID, bool isInImage)
        {
            return BbsRouter.GetUrl("handler/down") + "?action=attach&m=i&id=" + attachmentID.ToString() + (isInImage ? "&mode=image" : "");
        }
        public static string GetV30AttachmentUrl(object diskFileID, object postID, bool isInImage)
        {
            return BbsRouter.GetUrl("handler/down") + "?action=attach&v=3&diskfileid=" + diskFileID.ToString() + "&postid=" + postID + (isInImage ? "&mode=image" : "");
        }
        //public static string GetImageAttachmentUrl(string attachmentID, bool addRandomNumber)
        //{
        //    return string.Empty;
        //    ////此处加随机数 是因为购买附件时 AJAX提交 后图片没有更新
        //    //string random;
        //    //if (addRandomNumber)
        //    //    random = "&random=" + DateTime.Now.Millisecond;
        //    //else
        //    //    random = "";
        //    //return string.Format(Globals.ApplicationUrl + "/Attachment.aspx?ID={0}&mode=image" + random, attachmentID);
        //}
        public static string GetImageAttachmentUrl(object attachmentID, bool addRandomNumber)
        {
            string random;
            if (addRandomNumber)
                random = "&random=" + DateTime.Now.Millisecond;
            else
                random = "";
            return BbsRouter.GetUrl("handler/down") + "?action=attach&mode=image&id=" + attachmentID.ToString() + random;
            //return string.Format(Globals.ApplicationUrl + "/Attachment.aspx?ID={0}&mode=image" + random, attachmentID);
        }
        //public static string GetEditImageAttachmentUrl(int attachmentID)
        //{
        //    return string.Format(UrlManager.GetFriendlyUrl("OutputAttachment.html?type=edit&ID={0}&mode=image", true), attachmentID);
        //}
        //public static string GetEditAttachmentUrl(int attachmentID)
        //{
        //    return string.Format(UrlManager.GetFriendlyUrl("OutputAttachment.html?type=edit&ID={0}", true), attachmentID);
        //}
        //public static string GetAttachmentUrlByDisckFileID(int diskFileID)
        //{
        //    return string.Format(UrlManager.GetFriendlyUrl("OutputAttachment.html?DiskFileID={0}", true), diskFileID);
        //}

        public static string GetShowPermissionUrl(string forumIDString)
        {
            return string.Empty;
            //return string.Format(UrlManager.GetFriendlyUrl("ShowPermission.html?forumID={0}", true), forumIDString);
        }

        public static string GetShowPointUrl(string forumIDString)
        {
            return string.Empty;
            //return string.Format(UrlManager.GetFriendlyUrl("ShowPoint.html?forumID={0}", true), forumIDString);
        }


        public static string GetOnlinesUrlPager(int type)
        {
            return BbsRouter.GetUrl(string.Format("onlines/{0}/{1}", type, "{0}"));
            //return string.Format(UrlManager.GetFriendlyUrl("Onlines.html?page={0}&type={1}"), "{0}", type);
        }

        public static string GetOnlinesUrl()
        {
            return BbsRouter.GetUrl("onlines");
        }
        public static string GetOnlinesUrl(int page, int type)
        {
            return BbsRouter.GetUrl(string.Format("onlines/{0}/{1}", type, page));
            //return string.Format(UrlManager.GetFriendlyUrl("Onlines.html?page={0}&type={1}"), page, type);
        }

        public static string GetAnnouncementsUrl()
        {
            return BbsRouter.GetUrl("announcement");
        }

        public static string GetAnnouncementUrl(int id)
        {
            return BbsRouter.GetUrl(string.Format("announcement/{0}", id)) + "#" + id;
        }


        public static string GetShowRankUsersUrl(int threadID)
        {
            return BbsRouter.GetUrl(string.Format("rankusers/{0}", threadID));
        }

        public static string GetShowThreadLogsUrl(int threadID)
        {
            return BbsRouter.GetUrl(string.Format("threadlogs/{0}", threadID));
        }

        public static string GetAttachmentExchangesUrl(int attachmentID, int postID)
        {
            return BbsRouter.GetUrl(string.Format("attachmentexchanges/{0}/{1}", attachmentID, postID));
        }

        public static string GetVotedUsersUrl(int threadID)
        {
            return BbsRouter.GetUrl(string.Format("votedusers/{0}", threadID));
        }

        public static string GetNewThreadsUrl()
        {
            return BbsRouter.GetUrl(string.Format("newthreads"));
        }

#endif

    }
}