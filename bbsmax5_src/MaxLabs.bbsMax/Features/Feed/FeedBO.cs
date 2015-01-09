//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 用户动态相关业务逻辑
    /// </summary>
    public class FeedBO : BOBase<FeedBO>
    {

        private const string cacheKey_AllFeedTemplates = "Feed/AllFeedTemplates";
        private const string cacheKey_UserFeedFilters = "Feed/FeedFilters/{0}";
        private const string cacheKey_AllUserFeeds = "Feed/List/AllUserFeeds";
        private const string cacheKey_SearchFeedCount = "Feed/List/SearchFeedCount/{0}";


        /// <summary>
        /// 系统自带变量 动态模板中的 发起动态的用户
        /// </summary>
        private const string ActorUserTemplateTag = "{actor}";
        /// <summary>
        /// 系统自带变量 动态模板中的 动态目标用户
        /// </summary>
        private const string TargetUserTemplateTag = "{targetUser}";

        /// <summary>
        /// 动态发生时间
        /// </summary>
        private const string DateTimeTemplateTag = "{dateTime}";

        //以下要修改 必须相应的修改 MaxLabs.bbsMax.RegExp中 SiteFeed 的正则
        private const string SiteFeedTitleMark = "<!--bbsmax-SiteFeedTitleStart-->{0}<!--bbsmax-SiteFeedTitleEnd-->";
        private const string SiteFeedContentMark = "<!--bbsmax-SiteFeedContentStart-->{0}<!--bbsmax-SiteFeedContentEnd-->";
        private const string SiteFeedDescriptionMark = "<!--bbsmax-SiteFeedDescriptionStart-->{0}<!--bbsmax-SiteFeedDescriptionEnd-->";
        private const string SiteFeedImageMark = "<!--bbsmax-SiteFeedImageStart-->{0}<!--bbsmax-SiteFeedImageEnd-->";

        //private static Regex SiteFeedTitleRegex = new Regex(SiteFeedTitleMark.Replace("{0}", @"(?is)(.*?|\s*?)"), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex SiteFeedContentRegex = new Regex(SiteFeedContentMark.Replace("{0}", @"(?is)(.*?|\s*?)"), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex SiteFeedDescriptionRegex = new Regex(SiteFeedDescriptionMark.Replace("{0}", @"(?is)(.*?|\s*?)"), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static Regex SiteFeedImageRegex = new Regex(SiteFeedImageMark.Replace("{0}", @"(?is)(.*?|\s*?)"), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex SiteFeedImageLinkRegex = new Regex(@"<a\s+href=""(?is)(.*?|\s*?)""><img\s+src=""(?is)(.*?|\s*?)""\s+class=""summaryimg""\s+/></a>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public FeedBO()
        {
        }

        public BackendPermissions ManagePermission
        {
            get
            {
                return AllSettings.Current.BackendPermissions;
            }
        }

        public FeedCollection GetAllCachedFeed()
        {
            FeedCollection feeds;
            CacheUtil.TryGetValue<FeedCollection>(cacheKey_AllUserFeeds, out feeds);

            return feeds;
        }

        /// <summary>
        /// 最新的全站动态
        /// </summary>
        private FeedCollection AllUserFeeds
        {
            get
            {
                FeedCollection allUserFeeds;
                if (!CacheUtil.TryGetValue<FeedCollection>(cacheKey_AllUserFeeds, out allUserFeeds))
                {
                    allUserFeeds = FeedDao.Instance.GetAllUserFeeds(int.MaxValue, Consts.CacheAllUserFeedsCount, Guid.Empty, null);
                    CacheUtil.Set<FeedCollection>(cacheKey_AllUserFeeds, allUserFeeds, CacheTime.Short, CacheExpiresType.Absolute);
                }
                return allUserFeeds;
            }
        }

        private void ClearAllUserFeedsCache()
        {
            CacheUtil.RemoveBySearch("Feed/List/");
        }


        /// <summary>
        /// 获取所有应用通知模板
        /// </summary>
        /// <returns></returns>
        public FeedTemplateCollection GetAllFeedTemplates()
        {
            FeedTemplateCollection feedTemplates;
            if (!CacheUtil.TryGetValue<FeedTemplateCollection>(cacheKey_AllFeedTemplates, out feedTemplates))
            {
                feedTemplates = FeedDao.Instance.GetFeedTemplates();
                CacheUtil.Set<FeedTemplateCollection>(cacheKey_AllFeedTemplates, feedTemplates, CacheTime.Long, CacheExpiresType.Sliding);
            }
            return feedTemplates;

        }


        /// <summary>
        /// 获取某个应用的所有模板
        /// </summary>
        /// <param name="appID">应用ID</param>
        /// <returns></returns>
        public FeedTemplateCollection GetFeedTemplates(Guid appID)
        {
            AppBase app = AppManager.GetApp(appID);
            if (app == null)
                return null;

            FeedTemplateCollection allFeedTemplates = GetAllFeedTemplates();
            FeedTemplateCollection tempFeedTemplates = new FeedTemplateCollection();

            //检查数据库中的模板 如果不存在 就使用应用内置默认模板
            foreach (AppAction appAction in app.AppActions)
            {
                FeedTemplate feedTemplate = allFeedTemplates.GetValue(appID, appAction.ActionType);

                //如果数据库中不存在  就使用应用内置默认模板
                if (feedTemplate == null)
                {
                    feedTemplate = new FeedTemplate();
                    feedTemplate.AppID = appID;
                    feedTemplate.ActionType = appAction.ActionType;
                    feedTemplate.Description = appAction.DescriptionTemplate;
                    feedTemplate.IconSrc = appAction.IconSrc;
                    feedTemplate.Title = appAction.TitleTemplate;
                }

                tempFeedTemplates.Add(feedTemplate);

            }
            return tempFeedTemplates;
        }

        /// <summary>
        /// 获取指定应用指定动作的动态模板
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public FeedTemplate GetFeedTemplate(Guid appID, int actionType)
        {
            return GetAllFeedTemplates().GetValue(appID, actionType);
        }


        /// <summary>
        /// 修改通知模板,如果不存在将插入
        /// </summary>
        /// <param name="feedTemplete"></param>
        /// <returns></returns>
        public bool SetTemplates(int operatorUserID, Guid appID, FeedTemplateCollection feedTemplates)
        {
            if (ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_Template) == false)
            {
                ThrowError<NoPermissionSetFeedTemplateError>(new NoPermissionSetFeedTemplateError());
                return false;
            }

            if (feedTemplates.Count == 0)
                return true;

            AppBase app = AppManager.GetApp(appID);

            int i = 0;
            foreach (FeedTemplate feedTemplate in feedTemplates)
            {
                AppAction appAction = app.AppActions.GetValue(feedTemplate.ActionType);
                if (appAction == null)
                    continue;

                if (StringUtil.GetByteCount(feedTemplate.IconSrc) > Consts.Feed_TemplateIconUrlLength)
                {
                    ThrowError<FeedIconUrlLength>(new FeedIconUrlLength("FeedTemplate.IconUrl", Consts.Feed_TemplateIconUrlLength, feedTemplate.IconSrc, i));
                }
                if (StringUtil.GetByteCount(feedTemplate.Title) > Consts.Feed_TemplateTitleLength)
                {
                    ThrowError<FeedTitleLengthError>(new FeedTitleLengthError("FeedTemplate.Title", Consts.Feed_TemplateTitleLength, feedTemplate.Title, i));
                }
                if (StringUtil.GetByteCount(feedTemplate.Description) > Consts.Feed_TemplateDescriptionLength)
                {
                    ThrowError<FeedDescriptionLengthError>(new FeedDescriptionLengthError("FeedTemplate.Description", Consts.Feed_TemplateDescriptionLength, feedTemplate.Description, i));
                }

                if (string.IsNullOrEmpty(feedTemplate.IconSrc))
                {
                    feedTemplate.IconSrc = appAction.IconSrc;
                }

                if (string.IsNullOrEmpty(feedTemplate.Description))
                    feedTemplate.Description = appAction.DescriptionTemplate == null ? "" : appAction.DescriptionTemplate;

                if (string.IsNullOrEmpty(feedTemplate.Title))
                {
                    feedTemplate.Title = appAction.TitleTemplate;
                }
                i++;

            }

            if (HasUnCatchedError)
                return false;

            FeedDao.Instance.SetTemplates(feedTemplates);

            CacheUtil.Remove(cacheKey_AllFeedTemplates);
            return true;
        }


        public FeedDisplayType GetFeedDisplayType(int operatorUserID,int targetUserID)
        {
            User user = UserBO.Instance.GetUser(targetUserID);
            if (user == null)
                return FeedDisplayType.OtherError;

            SpacePrivacyType spacePrivacyType = user.SharePrivacy;
            if (spacePrivacyType == SpacePrivacyType.Friend)
            {
                if (!FriendBO.Instance.IsFriend(operatorUserID, targetUserID))
                {
                    if (ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_Data))
                        return FeedDisplayType.AdminVisibleInfo;
                    else
                    {
                        return FeedDisplayType.FriendVisibleError;
                    }
                }
            }
            return FeedDisplayType.CanVisible;
        }
        public enum FeedDisplayType
        {
            /// <summary>
            /// 可见
            /// </summary>
            CanVisible,

            /// <summary>
            /// 显示 您是管理员但仍有权限查看 的信息 
            /// </summary>
            AdminVisibleInfo,

            /// <summary>
            /// 显示仅好友可见的错误
            /// </summary>
            FriendVisibleError,

            /// <summary>
            /// 其它错误
            /// </summary>
            OtherError
        }

        /// <summary>
        /// 格式化 动态标题  
        /// </summary>
        /// <param name="userID">如果是全站动态和好友动态为当前登陆用户ID，如果是某人的个人动态则是那个人的UserID </param>
        /// <param name="feed"></param>
        /// <param name="feedType">动态类型：全站动态，个人动态，好友动态</param>
        /// <returns></returns>
        public string FormatFeedTitle(int userID, float timeDiffrence, FeedType feedType, Feed feed)
        {
            string title = BbsRouter.ReplaceUrlTag(feed.Title);

            //全局动态
            if (feed.AppID == Consts.App_BasicAppID && feed.ActionType == (int)AppActionType.SiteFeed)
            {
                string dateString;
                if (feed.CreateDate > DateTimeUtil.Now)
                    dateString = "现在";
                else
                    dateString = DateTimeUtil.GetFriendlyDate(feed.CreateDate);

                Match match = Pool<SiteFeedTitleRegex>.Instance.Match(title);
                //string title = feed.Title;
                if (match.Success)
                {
                    title = title.Replace(match.Value,match.Groups[1].Value);
                }

                title = Regex.Replace(title, DateTimeTemplateTag, dateString, RegexOptions.IgnoreCase);
                return title;
            }

            bool exchangePosition = false; //是否调换 {actor} 和 {targetUser} 的位置

            DateTime date = DateTime.MinValue;

            string url = BbsRouter.GetUrl("space/{0}");

            StringBuilder userString = new StringBuilder();
            foreach (UserFeed userFeed in feed.Users)
            {
                if (feed.IsSpecial && feed.TargetUserID == userFeed.UserID)//加好友  把自己的那个去掉
                    continue;
                
				if (feedType == FeedType.FriendFeed && userFeed.UserID == userID)
                    continue;

                userString.Append("<a class=\"fn\" href=\"" + string.Format(url, userFeed.UserID) + "\">" + userFeed.Realname + "</a>,");
                
				if (userFeed.CreateDate > date)
                    date = userFeed.CreateDate;
            }

            if (feedType == FeedType.AllUserFeed)
                exchangePosition = false;
            else if (feedType == FeedType.FriendFeed)
            {
                exchangePosition = true;
            }
            else
                exchangePosition = false;


            if (userString.Length > 0)
            {
                string dateString = DateTimeUtil.GetFriendlyDate(date);

                string str = userString.ToString(0, userString.Length - 1);
                //string title = feed.Title;
                if (feed.IsSpecial && exchangePosition || (feedType == FeedType.MyFeed && feed.TargetUserID == userID))// || feed.TargetUserID == userID)
                {

                    title = Regex.Replace(title, TargetUserTemplateTag, str, RegexOptions.IgnoreCase);
                    title = Regex.Replace(title, ActorUserTemplateTag, "<a class=\"fn\" href=\"" + string.Format(url,feed.TargetUserID) + "\">" + feed.TargetNickname + "</a>", RegexOptions.IgnoreCase);
                    title = Regex.Replace(title, DateTimeTemplateTag, dateString, RegexOptions.IgnoreCase);
                }
                else
                {
                    title = Regex.Replace(title, ActorUserTemplateTag, str, RegexOptions.IgnoreCase);
                    title = Regex.Replace(title, TargetUserTemplateTag, "<a class=\"fn\" href=\"" + string.Format(url, feed.TargetUserID) + "\">" + feed.TargetNickname + "</a>", RegexOptions.IgnoreCase);
                    title = Regex.Replace(title, DateTimeTemplateTag, dateString, RegexOptions.IgnoreCase);
                }
                return title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 格式化 动态描述
        /// </summary>
        /// <param name="userID">当前登陆用户ID</param>
        /// <param name="feed"></param>
        /// <returns></returns>
        public string FormatFeedDescription(int userID, Feed feed)
        {
            string description = BbsRouter.ReplaceUrlTag(feed.Description);

            if (feed.AppID == Consts.App_BasicAppID && feed.ActionType == (int)AppActionType.SiteFeed)
            {
                description = Pool<SiteFeedContentRegex>.Instance.Replace(description, "$1");
                description = Pool<SiteFeedDescriptionRegex>.Instance.Replace(description, "$1");
                description = Pool<SiteFeedImageRegex>.Instance.Replace(description, "$1");
                return description;
            }
			return Regex.Replace(description, TargetUserTemplateTag, "<a href=\"" + BbsRouter.GetUrl("space/" + feed.TargetUserID) + "\">" + feed.TargetNickname + "</a>", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否是全局动态
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        public bool IsSiteFeed(Feed feed)
        {
            return (feed.AppID == Consts.App_BasicAppID && feed.ActionType == (int)AppActionType.SiteFeed);
        }

        /// <summary>
        /// 是否是全局动态
        /// </summary>
        public bool IsSiteFeed(Guid appID,int actionType)
        {
            return (appID == Consts.App_BasicAppID && actionType == (int)AppActionType.SiteFeed);
        }

        /// <summary>
        /// 随便看看动态
        /// </summary>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public FeedCollection GetNetWorkFeeds(int getCount)
        {
            bool haveMore;
            return GetAllUserFeeds(ExecutorID, getCount, out haveMore);
        }

        /// <summary>
        /// 获取最新的全站动态
        /// </summary>
        /// <param name="userID">当前登陆用户</param>
        /// <param name="getCount"></param>
        /// <param name="haveMore"></param>
        /// <returns></returns>
        public FeedCollection GetAllUserFeeds(int userID,int getCount, out bool haveMore)
        {
            return GetAllUserFeeds(userID,int.MaxValue, getCount, Guid.Empty, null, out haveMore);
        }
        /// <summary>
        /// 获取ID 小于 beforeFeedID 的全站动态 如要获取最新的 请设置beforeFeedID 为 int.MaxValue
        /// </summary>
        /// <param name="userID">当前登陆用户</param>
        /// <param name="beforeFeedID">起始FeedID</param>
        /// <param name="getCount"></param>
        /// <param name="appID">指定应用（如果是所有应用就设为Guid.Empty）</param>
        /// <param name="actionType">指定应用动作（如果是所有动作就设为null）</param>
        /// <returns></returns>
        public FeedCollection GetAllUserFeeds(int userID,int beforeFeedID, int getCount, Guid appID, int? actionType, out bool haveMore)
        {

            List<int> friendUserIDs = FriendBO.Instance.GetFriendUserIDs(userID);

            if (getCount != int.MaxValue)
                getCount = getCount + 1;//多取一条出来  用来判断是否有更多动态
            haveMore = false;

            FeedCollection feeds = new FeedCollection();

            BasicApp basicApp = new BasicApp();

            Feed beforeFeed = null;
            if (beforeFeedID != int.MaxValue)
            {
                beforeFeed = AllUserFeeds.GetValue(beforeFeedID);
            }

            int count = 0;

            for (int i = 0; i < AllUserFeeds.Count; i++)
            {
                if (count == getCount)
                    break;

                Feed feed = null;
                try
                {
                    feed = (Feed)AllUserFeeds[i].Clone();
                }
                catch { }
                if (feed == null)
                    continue;

                //全局动态
                if (feed.AppID == basicApp.AppID && feed.ActionType == (int)AppActionType.SiteFeed)
                {
                    if (beforeFeed == null || feed.CreateDate < beforeFeed.CreateDate)
                    {
                        feeds.Add(feed);
                        count++;
                    }
                    continue;
                }

                if (feed.ID < beforeFeedID)
                {
                    if (appID != Guid.Empty && actionType != null)
                    {
                        if (feed.AppID != appID || feed.ActionType != actionType.Value)
                            continue;
                    }
                    if (!IsVisibleFeed(userID, feed, false, friendUserIDs))
                        continue;

                    if (feed.IsSpecial)
                    {
                        if (feed.Users.Count == 1 && feed.Users[0].UserID == feed.TargetUserID)
                        { }
                        else if (feed.Users.Count > 0)
                        {
                            feeds.Add(feed);
                            count++;
                        }
                    }
                    else
                    {
                        if (feed.Users.Count > 0)
                        {
                            feeds.Add(feed);
                            count++;
                        }
                    }
                }
            }
            //如果缓存中不够  就从数据库中取
            if (AllUserFeeds.Count >= Consts.CacheAllUserFeedsCount && count < getCount)
            {
                if (feeds.Count > 0)
                    beforeFeedID = feeds[feeds.Count - 1].ID;
                FeedCollection tempFeeds = FeedDao.Instance.GetAllUserFeeds(beforeFeedID, getCount - count, appID, actionType);
                if (feeds.Count + tempFeeds.Count == getCount)
                {
                    haveMore = true;
                    tempFeeds.RemoveAt(tempFeeds.Count - 1);//移除多取的那条
                }

                foreach (Feed feed in tempFeeds)
                { 
                    //全局动态
                    if (feed.AppID == basicApp.AppID && feed.ActionType == (int)AppActionType.SiteFeed)
                    {
                        feeds.Add(feed);
                        continue;
                    }

                    if(IsVisibleFeed(userID,feed,false,friendUserIDs))
                        feeds.Add(feed);
                }
            }
            else
            {
                if (feeds.Count == getCount)
                {
                    haveMore = true;
                    feeds.RemoveAt(getCount - 1);//移除多取的那条
                }
            }
            return feeds;
        }

        public FeedCollection SearchFeeds(int pageNumber, FeedSearchFilter filter, out int totalCount)
        {
            FeedSearchFilter feedSearchFilter = ProcessFeedSearchFilter(filter);
            if (feedSearchFilter == null)
            {
                totalCount = 0;
                return new FeedCollection();
            }

            if (pageNumber < 1)
                pageNumber = 1;

            string cacheKey = string.Format(cacheKey_SearchFeedCount,feedSearchFilter.ToString());

            bool hasCountCache = false;
            if (CacheUtil.TryGetValue<int>(cacheKey, out totalCount))
            {
                hasCountCache = true;
            }
            else
            {
                totalCount = -1;
            }

            FeedCollection feeds = FeedDao.Instance.SearchFeeds(pageNumber, feedSearchFilter, ref totalCount);

            if (!hasCountCache)
            {
                CacheUtil.Set<int>(cacheKey,totalCount,CacheTime.Normal,CacheExpiresType.Sliding);
            }
            return feeds;
        }

        public bool DeleteSearchFeeds(int operatorUserID, FeedSearchFilter filter,int deleteTopCount, out int deletedCount)
        {
            deletedCount = 0;
            if (ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_Data) == false)
            {
                ThrowError<NoPermissionDeleteFeedError>(new NoPermissionDeleteFeedError());
                return false;
            }

            FeedSearchFilter feedSearchFilter = ProcessFeedSearchFilter(filter);
            if (feedSearchFilter == null)
                return true;

            FeedDao.Instance.DeleteSearchFeeds(feedSearchFilter, deleteTopCount, out deletedCount);

            ClearAllUserFeedsCache();
            return true;
        }

        private FeedSearchFilter ProcessFeedSearchFilter(FeedSearchFilter filter)
        {
            FeedSearchFilter feedSearchFilter = (FeedSearchFilter)filter.Clone();

            User user = null;
            if (!string.IsNullOrEmpty(feedSearchFilter.Username))
            {
                user = UserBO.Instance.GetUser(feedSearchFilter.Username);
                if (user == null)
                    return null;
            }
            if (user != null)
            {
                if (feedSearchFilter.UserID != null && feedSearchFilter.UserID.Value != user.UserID)
                    return null;
                else
                    feedSearchFilter.UserID = user.UserID;
            }

            return feedSearchFilter;
        }

        /// <summary>
        /// 根据隐私类型判断 该条动态是否可见
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feed"></param>
        /// <param name="isFriendFeed">是否是获取好友动态</param>
        /// <param name="friendUserIDs"></param>
        /// <returns></returns>
        private bool IsVisibleFeed(int userID,Feed feed,bool isFriendFeed,List<int> friendUserIDs)
        {
            //指定用户可见
            if (feed.PrivacyType == PrivacyType.AppointUser)
            {
                if (!feed.VisibleUserIDs.Contains(userID))
                {
                    if (feed.Users.Count == 1 && feed.Users[0].UserID == userID)
                    { }
                    else
                        return false ;
                }
            }
            //好友可见
            else if (!isFriendFeed && feed.PrivacyType == PrivacyType.FriendVisible)
            {
                UserFeedCollection userFeeds = new UserFeedCollection();
                foreach (UserFeed userFeed in feed.Users)
                {
                    if (friendUserIDs.Contains(userFeed.UserID) || userFeed.UserID==userID)
                    {
                        userFeeds.Add(userFeed);
                    }
                }
                if (userFeeds.Count == 0)
                    return false;
                feed.Users = userFeeds;
            }
            //需要密码可见
            else if (feed.PrivacyType == PrivacyType.NeedPassword)
            {
                if (feed.Description != string.Empty)
                    feed.Description = "需要密码可见";
            }
            return true;
        }


        /// <summary>
        /// 获取没有被屏蔽所有动态的好友ID
        /// </summary>
        /// <returns></returns>
        private List<int> GetNoFiltratedFriendUserIDs(int userID)
        {
            FriendCollection friends;
            using (new BbsContext(false))
            {
                friends = FriendBO.Instance.GetFriends(userID);
            }
            if (friends == null || friends.Count == 0)
                return new List<int>();

            List<int> friendUserIDs = new List<int>();

            foreach (Friend friend in friends)
            {
                FriendGroup friendGroup = FriendBO.Instance.GetFriendGroup(friend.OwnerID, friend.GroupID);


                if (friendGroup != null && friendGroup.IsShield)
                {
                    continue;
                }
                friendUserIDs.Add(friend.UserID);
            }

            FeedFilterCollection feedFilters = GetFeedFilters(userID);

            foreach (FeedFilter feedFilter in feedFilters)
            {
                if (feedFilter.FilterType == FilterType.FilterUser)
                {
                    if (friendUserIDs.Contains(feedFilter.FriendUserID.Value))
                        friendUserIDs.Remove(feedFilter.FriendUserID.Value);
                }
            }
            return friendUserIDs;

        }

        /// <summary>
        /// 被过滤所有动态的好友ID,（不包括好友组里屏蔽的）
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<int> GetFiltratedFriendUserIDs(int userID)
        {
            List<int> friendUserIDs;

            string cacheKey = string.Format("FiltratedFriendUserIDs-{0}-Max", userID);

            if (PageCacheUtil.TryGetValue(cacheKey, out friendUserIDs) == false)
            {
                friendUserIDs = new List<int>();
                FeedFilterCollection feedFilters = GetFeedFilters(userID);

                foreach (FeedFilter feedFilter in feedFilters)
                {
                    if (feedFilter.FilterType == FilterType.FilterUser)
                    {
                        friendUserIDs.Add(feedFilter.FriendUserID.Value);
                    }
                }

                PageCacheUtil.Set(cacheKey, friendUserIDs);
            }

            return friendUserIDs;
        }

        /// <summary>
        /// 获取好友的最新动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="getCount"></param>
        /// <param name="haveMore"></param>
        /// <returns></returns>
        public FeedCollection GetFriendFeeds(int userID, int getCount, out bool haveMore)
        {
            return GetFriendFeeds(userID, int.MaxValue, getCount, Guid.Empty, null, out haveMore);
        }
        /// <summary>
        /// 获取ID 小于 beforeFeedID 的好友动态 如要获取最新的 请设置beforeFeedID 为 int.MaxValue
        /// </summary>
        /// <param name="userID">当前登陆用户</param>
        /// <param name="feedID">通知ID,取小于该ID的动态</param>
        /// <param name="getCount">获取条数</param>
        /// <param name="appID">指定应用（如果是所有应用就设为Guid.Empty）</param>
        /// <param name="actionType">指定应用动作（如果是所有动作就设为null）</param>
        public FeedCollection GetFriendFeeds(int userID, int beforeFeedID, int getCount, Guid appID, int? actionType, out bool haveMore)
        {
            haveMore = false;

            if (getCount != int.MaxValue)
                getCount = getCount + 1;//多取一条出来  用来判断是否有更多动态

            
            List<int> friendUserIDs = GetNoFiltratedFriendUserIDs(userID);
            //这里friendUserIDs 如果为0项  不能直接返回   因为还要获取全局动态

            FeedFilterCollection feedFilters = GetFeedFilters(userID);

            int count = 0;

            FeedCollection feeds = new FeedCollection();

            BasicApp basicApp = new BasicApp();

            Feed beforeFeed = null;
            if(beforeFeedID!=int.MaxValue)
                beforeFeed = AllUserFeeds.GetValue(beforeFeedID);

            for (int i = 0; i < AllUserFeeds.Count; i++)
            {
                if (count == getCount)
                    break;

                Feed feed = null;
                try
                {
                    feed = (Feed)AllUserFeeds[i].Clone();
                }
                catch { }
                if (feed == null)
                    continue;

                if (appID != Guid.Empty && actionType != null)
                {
                    if (feed.AppID != appID || feed.ActionType != actionType.Value)
                        continue;
                }

                //全局动态
                if (feed.AppID == basicApp.AppID && feed.ActionType == (int)AppActionType.SiteFeed)
                {
                    if (beforeFeed == null || feed.CreateDate < beforeFeed.CreateDate)
                    {
                        feeds.Add(feed);
                        count++;
                    }
                    continue;
                }

                if (feed.ID < beforeFeedID)
                {
                    if (feed.TargetUserID == userID)
                        continue;


                    if (!IsVisibleFeed(userID, feed, true, friendUserIDs))
                        continue;

                    //如果是加好友 并且TargetUserID是好友特殊处理
                    if (feed.IsSpecial)
                    {
                        if (friendUserIDs.Contains(feed.TargetUserID))
                        {
                            if (!IsFiltrateFeed(feed.AppID, feed.ActionType, feed.TargetUserID, feedFilters))
                            {
                                Feed tempFeed = (Feed)feed.Clone();

                                tempFeed.Users = new UserFeedCollection();
                                for (int j = 0; j < feed.Users.Count; j++)
                                {
                                    UserFeed userFeed;
                                    try
                                    {
                                        userFeed = feed.Users[j];
                                    }
                                    catch { continue; }

                                    if (friendUserIDs.Contains(userFeed.UserID))
                                    {
                                        if (IsFiltrateFeed(feed.AppID, feed.ActionType, userFeed.UserID, feedFilters))
                                        {
                                            continue;
                                        }
                                    }

                                    //2009-7-8 SEK
                                    if (userFeed.UserID == userID)
                                        continue;

                                    tempFeed.Users.Add(userFeed);
                                }
                                if (tempFeed.Users.Count > 1)//0)
                                {
                                    feeds.Add(tempFeed);
                                    count++;
                                }
                                continue;
                            }
                        }
                    }

                    UserFeedCollection userFeeds = new UserFeedCollection();
                    if (feed.Users == null)
                        continue;

                    for (int j = 0; j < feed.Users.Count; j++)
                    {
                        UserFeed userFeed;
                        try
                        {
                            userFeed = feed.Users[j];
                        }
                        catch { continue; }

                        if (friendUserIDs.Contains(userFeed.UserID))
                        {
                            if (!IsFiltrateFeed(feed.AppID, feed.ActionType, userFeed.UserID, feedFilters))
                            {
                                userFeeds.Add(userFeed);
                            }
                        }
                    }

                    if (userFeeds.Count > 0)
                    {
                        Feed tempFeed = (Feed)feed.Clone();
                        tempFeed.Users = userFeeds;
                        feeds.Add(tempFeed);
                        count++;
                    }
                }
            }

            //如果缓存中不够  就从数据库中取
            if (AllUserFeeds.Count >= Consts.CacheAllUserFeedsCount && count < getCount)
            {
                if (feeds.Count > 0)
                    beforeFeedID = feeds[feeds.Count - 1].ID;
                FeedCollection tempFeeds = FeedDao.Instance.GetFriendFeeds(userID, friendUserIDs, beforeFeedID, getCount - count, appID, actionType);

                if (feeds.Count + tempFeeds.Count == getCount)
                {
                    haveMore = true;
                    tempFeeds.RemoveAt(tempFeeds.Count - 1);//移除多取的那条
                }

                foreach (Feed feed in tempFeeds)
                {
                    //全局动态
                    if (feed.AppID == basicApp.AppID && feed.ActionType == (int)AppActionType.SiteFeed)
                    {
                        feeds.Add(feed);
                        continue;
                    }

                    if (!IsVisibleFeed(userID, feed, true, friendUserIDs))
                        continue;

                    if (feed.IsSpecial && friendUserIDs.Contains(feed.TargetUserID))
                    {
                        if (!IsFiltrateFeed(feed.AppID, feed.ActionType, feed.TargetUserID, feedFilters))
                        {
                            bool noAdd = false;
                            foreach (UserFeed userFeed in feed.Users)
                            {
                                if (feed.ActionType == (int)AppActionType.AddFriend && userFeed.UserID == userID)
                                {
                                    noAdd = true;
                                    break;
                                }

                                if (friendUserIDs.Contains(userFeed.UserID))
                                {
                                    if (IsFiltrateFeed(feed.AppID, feed.ActionType, userFeed.UserID, feedFilters))
                                    {
                                        feed.Users.Remove(userFeed);//该好友已经被过滤动态
                                    }
                                }
                            }
                            if (noAdd)
                                continue;

                            feeds.Add(feed);
                            continue;
                        }
                    }
                    UserFeedCollection userFeeds = new UserFeedCollection();
                    foreach (UserFeed userFeed in feed.Users)
                    {
                        if (!IsFiltrateFeed(feed.AppID, feed.ActionType, userFeed.UserID, feedFilters))
                            userFeeds.Add(userFeed);
                    }
                    if (userFeeds.Count > 0)
                    {
                        feed.Users = userFeeds;
                        feeds.Add(feed);
                    }
                }
            }
            else
            {
                if (feeds.Count == getCount)
                {
                    haveMore = true;
                    feeds.RemoveAt(getCount - 1);//移除多取的那条
                }
            }
            return feeds;
        }


        /// <summary>
        /// 获取用户的最新动态
        /// </summary>
        /// <param name="userID">当前登陆用户ID</param>
        /// <param name="targetUserID">要获取动态的用户</param>
        /// <param name="getCount">获取条数</param>
        /// <param name="haveMore">是否有更多动态</param>
        /// <returns></returns>
        public FeedCollection GetUserFeeds(int userID, int targetUserID, int getCount, out bool haveMore)
        {
            return GetUserFeeds(userID,targetUserID,  int.MaxValue, getCount, Guid.Empty, null, out haveMore);
        }
        /// <summary>
        ///  获取用户动态
        /// </summary>
        /// <param name="userID">当前登陆用户ID</param>
        /// <param name="targetUserID">要获取动态的用户</param>
        /// <param name="feedID">ID,取小于该ID的动态</param>
        /// <param name="getCount">获取条数</param>
        /// <param name="appID">指定应用（如果是所有应用就设为Guid.Empty）</param>
        /// <param name="actionType">指定应用动作（如果是所有动作就设为null）</param>
        /// <returns></returns>
        public FeedCollection GetUserFeeds(int userID,int targetUserID, int beforeFeedID, int getCount, Guid appID, int? actionType, out bool haveMore)
        {
            haveMore = false;

            List<int> friendUserIDs = FriendBO.Instance.GetFriendUserIDs(userID);

            if (getCount != int.MaxValue)
                getCount = getCount + 1;//多取一条出来  用来判断是否有更多动态

            int count = 0;

            FeedCollection feeds = new FeedCollection();
            for (int i = 0; i < AllUserFeeds.Count; i++)
            {
                if (count == getCount)
                    break;

                Feed feed;
                try
                {
                    feed = (Feed)AllUserFeeds[i].Clone();
                }
                catch { continue; }


                if (feed.ID < beforeFeedID)
                {

                    if (appID != Guid.Empty && actionType != null)
                    {
                        if (feed.AppID != appID || feed.ActionType != actionType.Value)
                            continue;
                    }
                    if (!IsVisibleFeed(userID, feed, false, friendUserIDs))
                        continue;

                    if (feed.IsSpecial && feed.TargetUserID == targetUserID)
                    {
                        if (feed.Users.GetValue(feed.ID, targetUserID) != null)
                        {
                            feeds.Add(feed);
                            count++;
                        }
                        continue;
                    }

                    UserFeedCollection userFeeds = new UserFeedCollection();

                    for (int j = 0; j < feed.Users.Count; j++)
                    {
                        UserFeed userFeed;
                        try
                        {
                            userFeed = feed.Users[j];
                        }
                        catch { continue; }
                        if (userFeed.UserID == targetUserID && userFeed.UserID != feed.TargetUserID)
                            userFeeds.Add(userFeed);
                    }

                    if (userFeeds.Count > 0)
                    {
                        Feed tempFeed = (Feed)feed.Clone();
                        tempFeed.Users = userFeeds;
                        feeds.Add(tempFeed);
                        count++;
                    }

                }
            }

            //如果缓存中不够  就从数据库中取11
            if (AllUserFeeds.Count >= Consts.CacheAllUserFeedsCount && count < getCount)
            {
                if (feeds.Count > 0)
                    beforeFeedID = feeds[feeds.Count - 1].ID;
                FeedCollection tempFeeds = FeedDao.Instance.GetUserFeeds(targetUserID, beforeFeedID, getCount - count, appID, actionType);

                if (feeds.Count + tempFeeds.Count == getCount)
                {
                    haveMore = true;
                    tempFeeds.RemoveAt(tempFeeds.Count - 1);//移除多取的那条
                }

                //if (feeds.Count == 0)
                //    return tempFeeds;

                foreach (Feed feed in tempFeeds)
                {
                    if (!IsVisibleFeed(userID, feed, false, friendUserIDs))
                        continue;

                    if (feed.IsSpecial && feed.TargetUserID == targetUserID)
                    {
                        if (feed.Users.GetValue(feed.ID, targetUserID) != null)
                        {
                            feeds.Add(feed);
                        }
                        continue;
                    }

                    feeds.Add(feed);
                }
            }
            else
            {
                if (feeds.Count == getCount)
                {
                    haveMore = true;
                    feeds.RemoveAt(getCount - 1);//移除多取的那条
                }
            }
            return feeds;
        }


        /// <summary>
        /// 当前的动作是否被过滤
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="friendUserID"></param>
        /// <param name="feedFilters"></param>
        /// <returns></returns>
        private bool IsFiltrateFeed(Guid appID, int actionType, int friendUserID, FeedFilterCollection feedFilters)
        {
            bool isFiltrated = false;
            foreach (FeedFilter feedFilter in feedFilters)
            {
                if (feedFilter.FilterType == FilterType.FilterApp)
                {
                    if (appID == feedFilter.AppID)
                    {
                        isFiltrated = true;
                        break;
                    }
                }
                else if (feedFilter.FilterType == FilterType.FilterAppAction)
                {
                    if (appID == feedFilter.AppID && actionType == feedFilter.ActionType)
                    {
                        isFiltrated = true;
                        break;
                    }
                }
                else if (feedFilter.FilterType == FilterType.FilterUserAppAction)
                {
                    if (friendUserID == feedFilter.FriendUserID.Value && appID == feedFilter.AppID && actionType == feedFilter.ActionType)
                    {
                        isFiltrated = true;
                        break;
                    }
                }
                else//(feedFilter.FilterType == FilterType.FilterUser)
                {
                    if (friendUserID == feedFilter.FriendUserID.Value)
                    {
                        isFiltrated = true;
                        break;
                    }
                }
            }
            return isFiltrated;
        }


        public Feed GetFeed(int feedID)
        {
            int[] feedIDs = new int[] { feedID };
            FeedCollection feeds = GetFeeds(feedIDs);
            if (feeds.Count > 0)
            {
                return feeds[0];
            }
            return null;
        }
        public FeedCollection GetFeeds(IEnumerable<int> feedIDs)
        {
            if (!ValidateUtil.HasItems<int>(feedIDs))
                return new FeedCollection();
            else
                return FeedDao.Instance.GetFeeds(feedIDs);
        }



        object userFeedslocker = new object();
        private void UpdateUserFeedsCache(Feed feed, UserFeed userFeed)
        {
            if (feed.ID == 0)//该动作被设置成隐私 不加入动态
                return;
            int count = Consts.CacheAllUserFeedsCount;
            lock (userFeedslocker)
            {
                Feed tempFeed = AllUserFeeds.GetValue(feed.ID);
                if (tempFeed == null)
                {
                    if (AllUserFeeds.Count == count)
                    {
                        Feed oldestFeed = AllUserFeeds[count - 1];
                        AllUserFeeds.Remove(oldestFeed);
                    }
                    feed.Users = new UserFeedCollection();

                    if (feed.IsSpecial)
                    {
                        UserFeed tempUserFeed = new UserFeed();
                        tempUserFeed.FeedID = feed.ID;
                        tempUserFeed.UserID = feed.TargetUserID;
                        tempUserFeed.Realname = feed.TargetNickname;
                        tempUserFeed.CreateDate = userFeed.CreateDate;
                        feed.Users.Add(tempUserFeed);
                    }
                    feed.Users.Add(userFeed);

                    if (AllUserFeeds.Count == 0)
                    {
                        AllUserFeeds.Insert(0, feed);
                    }
                    else
                    {
                        //由于全局动态 的创建时间 可以是将来的某个时间 在这个时间之前始终显示在所有动态之前
                        //所以需要以下处理
                        for (int i = 0; i < AllUserFeeds.Count; i++)
                        {
                            Feed oldFeed = AllUserFeeds[i];

                            if (feed.CreateDate > oldFeed.CreateDate)
                            {
                                AllUserFeeds.Insert(i, feed);
                                break;
                            }
                        }
                    }

                }
                else if (tempFeed.Users == null)
                {
                    tempFeed.Users = new UserFeedCollection();

                    if (tempFeed.IsSpecial)
                    {
                        UserFeed tempUserFeed = new UserFeed();
                        tempUserFeed.FeedID = feed.ID;
                        tempUserFeed.UserID = feed.TargetUserID;
                        tempUserFeed.Realname = feed.TargetNickname;
                        tempUserFeed.CreateDate = userFeed.CreateDate;
                        tempFeed.Users.Add(tempUserFeed);
                    }

                    tempFeed.Users.Add(userFeed);

                    tempFeed.Title = feed.Title;
                    tempFeed.Description = feed.Description;
                    tempFeed.CommentInfo = feed.CommentInfo;
                    tempFeed.CommentTargetID = feed.CommentTargetID;
                }
                else
                {
                    tempFeed.Title = feed.Title;
                    tempFeed.Description = feed.Description;
                    tempFeed.CommentInfo = feed.CommentInfo;
                    tempFeed.CommentTargetID = feed.CommentTargetID;

                    UserFeed tempUserFeed = tempFeed.Users.GetValue(feed.ID, userFeed.UserID);
                    if (tempUserFeed == null)
                    {
                        tempFeed.Users.Add(userFeed);
                    }
                    else
                        tempUserFeed.CreateDate = userFeed.CreateDate;
                }
            }

        }

        /// <summary>
        /// 移除动态的指定用户
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedIDs"></param>
        private void ClearFeedCache(int userID, IEnumerable<int> feedIDs)
        {
            lock (userFeedslocker)
            {
                foreach (int feedID in feedIDs)
                {
                    Feed feed = AllUserFeeds.GetValue(feedID);
                    if (feed != null && feed.Users != null)
                    {
                        UserFeed userFeed = feed.Users.GetValue(feedID, userID);
                        if (userFeed != null)
                        {
                            feed.Users.Remove(userFeed);
                            if (feed.Users.Count == 0)
                                AllUserFeeds.Remove(feed);
                            else if (feed.IsSpecial)
                            {
                                if (feed.Users.Count == 1 && feed.Users[0].UserID == feed.TargetUserID)
                                    AllUserFeeds.Remove(feed);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 移除动态的所有用户 并移除动态
        /// </summary>
        /// <param name="feedIDs"></param>
        private void ClearFeedCache(IEnumerable<int> feedIDs)
        {
            lock (userFeedslocker)
            {
                foreach (int feedID in feedIDs)
                {
                    Feed feed = AllUserFeeds.GetValue(feedID);
                    if (feed != null)
                        AllUserFeeds.Remove(feed);
                }
            }
        }




        /// <summary>
        /// 删除自己的动态
        /// </summary>
        public bool DeleteFeed(int operatorUserID, int targetUserID, int feedID)
        {
            List<int> feedIDs = new List<int>();
            feedIDs.Add(feedID);
            return DeleteFeeds(operatorUserID, targetUserID, feedIDs);
        }
        /// <summary>
        /// 删除用户的动态
        /// </summary>
        /// <param name="feedID"></param>
        public bool DeleteFeeds(int operatorUserID, int targetUserID, IEnumerable<int> feedIDs)
        {
            if (operatorUserID != targetUserID && ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_Data) == false)
            {
                ThrowError<NoPermissionDeleteFeedError>(new NoPermissionDeleteFeedError());
                return false;
            }
            if (ValidateUtil.HasItems<int>(feedIDs) == false)
            {
                ThrowError<NotSelectedError>(new NotSelectedFeedsError("feedIDs"));
                return false;
            }

            FeedDao.Instance.DeleteFeeds(targetUserID, feedIDs);
            ClearFeedCache(targetUserID, feedIDs);

            List<int> tempFeedIDs = new List<int>();
            foreach (int id in feedIDs)
                tempFeedIDs.Add(id);

            Logs.LogManager.LogOperation(
                   new Feed_DeleteUserFeedByIDs(operatorUserID, UserBO.Instance.GetUser(operatorUserID).Name, targetUserID, UserBO.Instance.GetUser(targetUserID).Name, IPUtil.GetCurrentIP(), tempFeedIDs)
               );

            return true;
        }


        /// <summary>
        /// 删除这些动态关联的所有用户 （管理员才有权限）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedIDs"></param>
        public bool DeleteAnyFeed(int operatorUserID, int feedID)
        {
            List<int> feedIDs = new List<int>();
            feedIDs.Add(feedID);
            return DeleteAnyFeeds(operatorUserID, feedIDs);
        }
        /// <summary>
        /// 删除这些动态关联的所有用户 （管理员才有权限）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedIDs"></param>
        public bool DeleteAnyFeeds(int operatorUserID, IEnumerable<int> feedIDs)
        {
            if (ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_Data) == false)
            {
                ThrowError<NoPermissionDeleteFeedError>(new NoPermissionDeleteFeedError());
                return false;
            }

            if (!ValidateUtil.HasItems<int>(feedIDs))
            {
                ThrowError(new NotSelectedFeedsError("NoSelectedError"));
                return false;
            }

            FeedDao.Instance.DeleteFeeds(null, feedIDs);
            ClearFeedCache(feedIDs);

            List<int> tempFeedIDs = new List<int>();

            foreach (int id in feedIDs)
                tempFeedIDs.Add(id);

            Logs.LogManager.LogOperation(
                   new Feed_DeleteFeedByIDs(operatorUserID, UserBO.Instance.GetUser(operatorUserID).Name, IPUtil.GetCurrentIP(), tempFeedIDs)
               );
            return true;
        }


        /// <summary>
        /// 删除指定时间以前的所有动态 并且始终保留制定条数
        /// </summary>
        /// <param name="dateTime">如果为null 则按保留条数进行删除</param>
        /// <param name="count">保留条数 如果为0则按时间进行删除</param>
        public bool DeleteFeeds(DateTime? dateTime, int count)
        {
            FeedDao.Instance.DeleteFeeds(dateTime, count);
            ClearAllUserFeedsCache();
            return true;
        }



        /// <summary>
        /// 获取用户 不加入动态的设置
        /// </summary>
        /// <param name="userID"></param>
        public UserNoAddFeedAppCollection GetUserNoAddFeedApps(int userID)
        {
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userid"));
                return new UserNoAddFeedAppCollection();
            }
            return FeedDao.Instance.GetUserNoAddFeedApps(userID);
        }


        /// <summary>
        /// 用户设置自己的动态不加入通知
        /// </summary>
        /// <param name="userNoAddFeedApp">不记录动态的应用</param>
        public bool SetUserNoAddFeedApps(int userID, UserNoAddFeedAppCollection userNoAddFeedApps)
        {
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userid"));
                return false;
            }
            FeedDao.Instance.SetUserNoAddFeedApps(userID, userNoAddFeedApps);
            return true;
        }





        /// <summary>
        /// 获取 过滤好友动态 设置
        /// </summary>
        public FeedFilterCollection GetFeedFilters(int userID)
        {
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userid"));
                return new FeedFilterCollection();
            }

            string cacheKey = string.Format(cacheKey_UserFeedFilters, userID);
            FeedFilterCollection feedFilters;

            if (!CacheUtil.TryGetValue<FeedFilterCollection>(cacheKey, out feedFilters))
            {
                feedFilters = FeedDao.Instance.GetFeedFilters(userID);
                CacheUtil.Set<FeedFilterCollection>(cacheKey, feedFilters, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            return feedFilters;
        }





        /// <summary>
        /// 删除过滤好友动态设置
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedFilterIDs"></param>
        public bool DeleteFeedFilters(int userID, IEnumerable<int> feedFilterIDs)
        {
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userid"));
                return false;
            }
            FeedDao.Instance.DeleteFeedFilters(userID, feedFilterIDs);
            ClearFeedFilterCache(userID);
            return true;
        }

        /// <summary>
        /// 移除 过滤好友动态设置 的缓存
        /// </summary>
        /// <param name="userID"></param>
        private void ClearFeedFilterCache(int userID)
        {
            string cacheKey = string.Format(cacheKey_UserFeedFilters, userID);
            CacheUtil.Remove(cacheKey);
        }







        //public int CreateFeed(int userID, Guid appID, int actionType, string title, string description, int targetUserID, int? targetID, bool isSpecail)
        //{
        //    Feed feed = new Feed();
        //    feed.AppID = appID;
        //    feed.ActionType = actionType;
        //    feed.CreateDate = DateTimeUtil.Now;
        //    feed.Description = description;
        //    feed.TargetID = (targetID == null ? 0 : targetID.Value);
        //    feed.TargetUserID = targetUserID;
        //    feed.TargetNickname = UserBO.Instance.GetUser(targetUserID).Realname;
        //    feed.Title = title;
        //    feed.IsSpecial = isSpecail;

        //    UserFeed userFeed = new UserFeed();
        //    userFeed.CreateDate = DateTimeUtil.Now;
        //    userFeed.Realname = UserBO.Instance.GetUser(userID).Realname;
        //    userFeed.UserID = userID;

        //    return CreateFeed(feed, userFeed);
        //}

        /// <summary>
        /// 添加一个动态
        /// </summary>
        /// <param name="feed"></param>
        private int CreateFeed(Feed feed, UserFeed userFeed, bool canJoin)
        {
            FeedSendItem.SendType sendType = FeedSendItem.SendType.Send;

            foreach (FeedSendItem item in AllSettings.Current.PrivacySettings.FeedSendItems)
            {
                if (item.AppID == feed.AppID && item.ActionType == feed.ActionType)
                {
                    sendType = item.DefaultSendType;
                }
            }

            int feedID = FeedDao.Instance.CreateFeed(feed, userFeed, sendType, canJoin);
            if (feedID > 0)
            {
                feed.ID = feedID;
                userFeed.FeedID = feedID;
                UpdateUserFeedsCache(feed, userFeed);
            }
            return feedID;
        }


        /// <summary>
        /// 解除屏蔽好友动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        public bool UnFiltrateFeed(int friendUserID)
        {
            int userID = ExecutorID;
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userID"));
                return false;
            }
            if (friendUserID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("friendUserID"));
                return false;
            }
            FeedFilterCollection feedFilters = GetFeedFilters(userID);
            FeedFilter feedFilter = null;
            foreach (FeedFilter tempFeedFilter in feedFilters)
            {
                if (tempFeedFilter.FilterType == FilterType.FilterUser)
                {
                    if (tempFeedFilter.FriendUserID == friendUserID)
                    {
                        feedFilter = tempFeedFilter;
                        break;
                    }
                }
            }
            if (feedFilter != null)
            {
                List<int> feedFilterIDs = new List<int>();
                feedFilterIDs.Add(feedFilter.ID);
                FeedDao.Instance.DeleteFeedFilters(userID, feedFilterIDs);
                feedFilters.Remove(feedFilter);
            }
            return true;
        }

        /// <summary>
        /// 屏蔽指定好友的所有动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        public bool FiltrateFeed(int operatorID, int friendUserID)
        {
            //int userID = ExecutorID;
            if (friendUserID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("friendUserID"));
                return false;
            }

            if (!FriendBO.Instance.IsFriend(operatorID, friendUserID))
            {
                ThrowError<ShieldFeedNotFriendError>(new ShieldFeedNotFriendError("friendUserID", UserBO.Instance.GetUser(friendUserID).Name));
                return false;
            }

            FeedFilter feedFilter = new FeedFilter();
            feedFilter.AppID = Guid.Empty;
            feedFilter.ActionType = null;
            feedFilter.FriendUserID = friendUserID;
            feedFilter.UserID = operatorID;

            return CreateFeedFilter(feedFilter);
        }

        /// <summary>
        /// 屏蔽所有好友的指定类型的动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        public bool FiltrateFeed(Guid appID, int actionType)
        {
            int userID = ExecutorID;
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userID"));
                return false;
            }

            FeedFilter feedFilter = new FeedFilter();
            feedFilter.AppID = appID;
            feedFilter.ActionType = actionType;
            feedFilter.FriendUserID = null;
            feedFilter.UserID = userID;

            return CreateFeedFilter(feedFilter);
        }

        /// <summary>
        /// 屏蔽指定好友的指定类型动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        public bool FiltrateFeed(int friendUserID, Guid appID, int actionType)
        {
            int userID = ExecutorID;
            if (userID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("userID"));
                return false;
            }

            if (!FriendBO.Instance.IsFriend(userID, friendUserID))
            {
                User user = UserBO.Instance.GetUser(friendUserID);
                if(user == null)
                {
                    ThrowError<InvalidParamError>(new InvalidParamError("frienduserid"));
                    return false;
                }
                ThrowError<ShieldFeedNotFriendError>(new ShieldFeedNotFriendError("ShieldFeedNotFriendError",user.Name));
                return false;
            }

            FeedFilter feedFilter = new FeedFilter();
            feedFilter.AppID = appID;
            feedFilter.ActionType = actionType;
            feedFilter.FriendUserID = friendUserID;
            feedFilter.UserID = userID;

            return CreateFeedFilter(feedFilter);
        }

        private bool CreateFeedFilter(FeedFilter feedFilter)
        {
            FeedDao.Instance.CreateFeedFilter(feedFilter);
            ClearFeedFilterCache(feedFilter.UserID);
            return true;
        }


         /// <summary>
        /// 更新动态隐私类型
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="targetID"></param>
        /// <param name="privacyType"></param>
        /// <param name="visibleUserIDs">可见的用户</param>
        public bool UpdateFeedPrivacyType(AppActionType actionType, int targetID, PrivacyType privacyType, List<int> visibleUserIDs)
        {
            return UpdateFeedPrivacyType(Consts.App_BasicAppID, (int)actionType, targetID, privacyType, visibleUserIDs);
        }
        /// <summary>
        /// 更新动态隐私类型
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetID"></param>
        /// <param name="privacyType"></param>
        /// <param name="visibleUserIDs">可见的用户</param>
        public bool UpdateFeedPrivacyType(Guid appID, int actionType, int targetID, PrivacyType privacyType, List<int> visibleUserIDs)
        {

            FeedDao.Instance.UpdateFeedPrivacyType(appID,actionType,targetID,privacyType,visibleUserIDs);

            //更新缓存
            for (int i = 0; i < AllUserFeeds.Count; i++)
            {
                Feed feed;
                try
                {
                    feed = AllUserFeeds[i];
                }
                catch { continue; }


                if (feed.AppID == appID && feed.ActionType == actionType && feed.TargetID == targetID)
                {
                    feed.PrivacyType = privacyType;
                    feed.VisibleUserIDs = visibleUserIDs;
                    break;
                }
            }

            return true;
        }

         /// <summary>
        /// 删除评论类型的动态  （如删除了评论则删除相应的动态,删除文章不能使用此方法）
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="targetID">文章ID</param>
        /// <param name="userID">评论者ID</param>
        /// <returns></returns>
        public bool DeleteFeed(AppActionType actionType, int targetID, int userID)
        {
            Dictionary<int, List<int>> deleteIDs = new Dictionary<int, List<int>>();

            List<int> userIDs = new List<int>();
            userIDs.Add(userID);

            deleteIDs.Add(targetID, userIDs);

            return DeleteFeeds(actionType, deleteIDs);
        }
        /// <summary>
        /// 删除评论类型的动态  （如删除了评论则删除相应的动态,删除文章不能使用此方法）
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="deleteIDs">key:文章ID value:评论者ID</param>
        /// <returns></returns>
        public bool DeleteFeeds(AppActionType actionType, Dictionary<int, List<int>> deleteIDs)
        {
            return DeleteFeeds(Consts.App_BasicAppID, (int)actionType, deleteIDs);
        }

        /// <summary>
        /// 删除文章类型的动态  （如删除了文章则删除相应的动态,删除评论不能使用此方法）
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetID">文章ID</param>
        /// <returns></returns>
        public bool DeleteFeed(AppActionType actionType, int targetID)
        {
            return DeleteFeeds(Consts.App_BasicAppID, (int)actionType, new int[] { targetID });
        }

        /// <summary>
        /// 删除文章类型的动态  （如删除了文章则删除相应的动态,删除评论不能使用此方法）
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetIDs">文章ID</param>
        /// <returns></returns>
        public bool DeleteFeeds(AppActionType actionType, IEnumerable<int> targetIDs)
        {
            return DeleteFeeds(Consts.App_BasicAppID, (int)actionType, targetIDs);
        }


        /// <summary>
        /// 删除文章类型的动态  （如删除了文章则删除相应的动态,删除评论不能使用此方法）
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetIDs">文章ID</param>
        /// <returns></returns>
        public bool DeleteFeeds(Guid appID, int actionType, IEnumerable<int> targetIDs)
        {
            return DeleteFeeds(appID, actionType, targetIDs, null);
        }

        /// <summary>
        /// 删除评论类型的动态  （如删除了评论则删除相应的动态,删除文章不能使用此方法）
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="deleteIDs">key:文章ID value:评论者ID</param>
        /// <returns></returns>
        public bool DeleteFeeds(Guid appID, int actionType, Dictionary<int,List<int>> deleteIDs)
        {
            List<int> targetIDs = new List<int>();
            List<List<int>> userIDs = new List<List<int>>();
            foreach (KeyValuePair<int, List<int>> pair in deleteIDs)
            {
                if (pair.Value.Count > 0)
                {
                    targetIDs.Add(pair.Key);
                    userIDs.Add(pair.Value);
                }
            }

            if (targetIDs.Count == 0)
                return true;

            return DeleteFeeds(appID, actionType, targetIDs, userIDs);
        }

        /// <summary>
        /// 删除动态  （如删除了文章则删除相应的动态）
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <param name="targetIDs">文章ID</param>
        /// <param name="userIDs">文章ID对应的评论者的userID</param>
        /// <returns></returns>
        private bool DeleteFeeds(Guid appID, int actionType, IEnumerable<int> targetIDs, IEnumerable<List<int>> userIDs)
        {
            if (ValidateUtil.HasItems<int>(targetIDs) == false)
                return true;

            FeedDao.Instance.DeleteFeeds(appID, actionType, targetIDs, userIDs);

            Dictionary<int, int> feedIDs = new Dictionary<int, int>();
            //更新缓存

            foreach (int targetID in targetIDs)
            {
                for (int i = 0; i < AllUserFeeds.Count; i++)
                {
                    Feed feed;
                    try
                    {
                        feed = AllUserFeeds[i];
                    }
                    catch { continue; }


                    if (feed.AppID == appID && feed.ActionType == actionType && feed.TargetID == targetID)
                    {
                        feedIDs.Add(targetID, feed.ID);
                        break;
                    }
                }
            }

            if (userIDs == null)
                ClearFeedCache(feedIDs.Values);
            else
            {
                int i = 0;
                foreach (List<int> tempUserIDs in userIDs)
                {
                    int j = 0;
                    int? targetID = null;
                    foreach (int tempTargetID in targetIDs)
                    {
                        if (i == j)
                        {
                            targetID = tempTargetID;
                            break;
                        }
                        j++;
                    }
                    if (targetID == null)
                        break;

                    foreach (int userID in tempUserIDs)
                    {
                        int feedID;
                        if (feedIDs.TryGetValue(targetID.Value, out feedID))
                        {
                            ClearFeedCache(userID, new int[] { feedID });
                        }
                    }
                    i++;
                }
            }

            return true;
        }


        /// <summary>
        /// 获取全局动态的原始标题 内容 图片等
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="description"></param>
        /// <param name="images"></param>
        public void GetSiteFeed(Feed feed,out string title,out string content,out string description,out List<string> imageUrls,out List<string> imageLinks)
        {
            Match match = Pool<SiteFeedTitleRegex>.Instance.Match(feed.Title);
            if (match.Success)
                title = match.Groups[1].Value;
            else
                title = string.Empty;

            match = Pool<SiteFeedContentRegex>.Instance.Match(feed.Description);
            if (match.Success)
                content = match.Groups[1].Value;
            else
                content = string.Empty;

            match = Pool<SiteFeedDescriptionRegex>.Instance.Match(feed.Description);
            if (match.Success)
                description = match.Groups[1].Value;
            else
                description = string.Empty;

            match = Pool<SiteFeedImageRegex>.Instance.Match(feed.Description);
            string imageString;
            if (match.Success)
                imageString = match.Groups[1].Value;
            else
                imageString = string.Empty;

            imageUrls = new List<string>();
            imageLinks = new List<string>();
            if (imageString != string.Empty)
            {
                MatchCollection mc = SiteFeedImageLinkRegex.Matches(feed.Description);
                foreach (Match m in mc)
                {
                    imageUrls.Add(m.Groups[2].Value);
                    imageLinks.Add(m.Groups[1].Value);
                }
            }

        }

        /// <summary>
        /// 获取所有全局动态
        /// </summary>
        /// <returns></returns>
        public FeedCollection GetAllSiteFeeds()
        {
            bool haveMore;
            return GetAllUserFeeds(ExecutorID, int.MaxValue, int.MaxValue, Consts.App_BasicAppID, (int)AppActionType.SiteFeed, out haveMore);
        }

        public Feed GetSiteFeed(int feedID)
        {
            if (feedID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("feedID"));
                return null;
            }
            Feed feed = GetFeed(feedID);
            if (feed == null)
            {
                ThrowError<FeedNotExistsError>(new FeedNotExistsError("feedID", feedID));
                return null;
            }
            if (feed.AppID != Consts.App_BasicAppID || feed.ActionType != (int)AppActionType.SiteFeed)
            {
                ThrowError<NotSiteFeedError>(new NotSiteFeedError("feedID", feedID));
                return null;
            }
            return feed;
        }

        //public Feed GetFeed(int feedID)
        //{
        //    return FeedDao.Instance.GetFeed(feedID);
        //}

        public bool EditSiteFeed(int operatorUserID, int feedID, string title, string content, string description, DateTime createDate, List<string> imageUrls, List<string> imageLinks)
        {
            if (ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_SiteFeed) == false)
            {
                ThrowError<NoPermissionManageSiteFeedError>(new NoPermissionManageSiteFeedError());
                return false;
            }

            if (feedID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("feedID"));
                return false;
            }

            Feed feed = GetFeed(feedID);
            if (feed == null)
            {
                ThrowError<FeedNotExistsError>(new FeedNotExistsError("feedID", feedID));
                return false;
            }

            if (feed.AppID != Consts.App_BasicAppID || feed.ActionType != (int)AppActionType.SiteFeed)
            {
                ThrowError<NotSiteFeedError>(new NotSiteFeedError("feedID", feedID));
                return false;
            }

            bool success = CreateSiteFeed(operatorUserID, title, content, description, createDate, imageUrls, imageLinks);
            if (success)
            {
                return DeleteAnyFeed(operatorUserID, feedID);
            }
            else
                return false;
        }

       


        //=========添加动态======================
        #region 内部应用 添加动态

        /// <summary>
        /// 添加全局动态
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="description"></param>
        /// <param name="CreateDate"></param>
        /// <param name="imgs">图片 最多4张 key:图片地址 value:链接地址</param>
        public bool CreateSiteFeed(int operatorUserID, string title, string content, string description, DateTime createDate, List<string> imageUrls,List<string> imageLinks)
        {
            if (ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Feed_SiteFeed) == false)
            {
                ThrowError<NoPermissionCreateSiteFeedError>(new NoPermissionCreateSiteFeedError());
                return false;
            }

            if (string.IsNullOrEmpty(title))
            {
                ThrowError<EmptySiteFeedTitleError>(new EmptySiteFeedTitleError("title"));
            }

            if (string.IsNullOrEmpty(content))
            {
                ThrowError<EmptySiteFeedContentError>(new EmptySiteFeedContentError("content"));
            }

            string link = "<a href=\"{0}\"><img src=\"{1}\" class=\"summaryimg\" /></a>";
            StringBuilder images = new StringBuilder();
            for (int i = 0; i < imageUrls.Count;i++)
            {
                if (i > 3)
                    break;
                images.Append(string.Format(link, imageLinks[i], imageUrls[i]));

            }

            string imageString = images.ToString();

            List<string> titleDatas = new List<string>();
            titleDatas.Add(string.Format(SiteFeedTitleMark, title));
            List<string> descriptionDatas = new List<string>();
            descriptionDatas.Add(string.Format(SiteFeedContentMark, content));
            descriptionDatas.Add(string.Format(SiteFeedDescriptionMark, description));
            descriptionDatas.Add(string.Format(SiteFeedImageMark, imageString));

            if (createDate < DateTimeUtil.Now)
                createDate = DateTimeUtil.Now;

            if (HasUnCatchedError)
                return false;

            CreateAppFeed(Consts.Feed_SiteFeedUserID, 0, null, 0, PrivacyType.AllVisible, null, false, new BasicApp().AppID, (int)AppActionType.SiteFeed, titleDatas, descriptionDatas, createDate);
            return true;
        }


        /// <summary>
        /// 开通空间 注册成功后添加动态
        /// </summary>
        /// <param name="actorUserID"></param>
        public void CreateOpenSpaceFeed(int userID)
        {
            CreateAppFeed(userID, AppActionType.OpenSpace);
        }

        /// <summary>
        /// 用户更新资料添加动态
        /// </summary>
        /// <param name="actorUserID"></param>
        public void CreateUpdateUserProfileFeed(int userID)
        {
            CreateAppFeed(userID, AppActionType.UpdateUserProfile);
        }

        /// <summary>
        /// 用户更新头像添加动态
        /// </summary>
        /// <param name="actorUserID"></param>
        public void CreateUpdateAvatarFeed(int userID)
        {
            CreateAppFeed(userID, AppActionType.UpdateAvatar);
        }

        private void CreateAppFeed(int actorUserID, AppActionType actionType)
        {
            CreateAppFeed(actorUserID, 0, null, 0, false, new BasicApp().AppID, (int)actionType, null, null);
        }
        
        
        
        /// <summary>
        /// 添加好友时候 添加一条动态
        /// </summary>
        /// <param name="targetUserID">接受好友请求的 用户的UserID</param>
        /// <param name="actorUserID">申请人的UserID</param>
        public void CreateAddFriendFeed(int targetUserID,int actorUserID)
        {
            BasicApp app = new BasicApp();
            CreateAppFeed(actorUserID, targetUserID, null, 0, true, app.AppID, (int)AppActionType.AddFriend, null, null);

        }

        /// <summary>
        /// 更新心情时 添加动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="doingID"></param>
        /// <param name="content"></param>
        public void CreateUpdateDoingFeed(int userID, int doingID, string content)
        {
            //string url = UrlUtil.JoinUrl(Globals.AppRoot,"max-dialogs/comment-add.aspx?targetid="+doingID+"&type=doing");
            //string link = "<a href=\""+ url +"\"  onclick=\"return openDialog(this.href,this, function(){})\">回复</a>";
           
            List<string> titleDatas = new List<string>();
            titleDatas.Add(content);
            //titleDatas.Add(link);

            //此处 targetID 用userID  是为了避免 两个不同用户的更新心情的 动态合并    2009-7-13  sek
            CreateAppFeed(userID, 0, userID, doingID, false, new BasicApp().AppID, (int)AppActionType.UpdateDoing, titleDatas, null);

        }

        /// <summary>
        /// 评论分享时 添加动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="targetID"></param>
        /// <param name="catagory"></param>
        public void CreatShareCommentFeed(int userID, int targetUserID, int shareID, ShareType catagory)
        {
            string type = "分享的";
            string title = "<a href=\"$url(space/" + targetUserID + "/share/share-" + shareID + ")\" target=\"_blank\">" + ShareBO.Instance.GetShareTypeName(catagory) + "</a>";

            CreatCommentFeed(userID, targetUserID, shareID, type, title, string.Empty);
        }

        public void CreatPictureCommentFeed(int userID, int targetUserID, int pictureID,string pictureUrl, string picutureTitle,string comment)
        {
            string type = "的图片";

            string content = @"
<div class=""feed_content"">
<a href="""+ UrlHelper.GetPhotoUrlTag(pictureID) +@""" target=""_blank""><img src=""{2}"" class=""summaryimg""></a>
<div class=""detail"">
{0}</div>
<div class=""quote"">
<span class=""q"">{1}</span>
</div>
</div>
";
            content = string.Format(content, picutureTitle, comment, pictureUrl);

            CreatCommentFeed(userID, targetUserID, pictureID, type, string.Empty, content);
        }
        /// <summary>
        /// 评论日志时添加动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="shareID"></param>
        /// <param name="catagory"></param>
        public void CreatBlogCommentFeed(int userID, int targetUserID, int articleID, string title)
        {
            string type = "的日志";
            string tempTitle = "<a href=\"" + UrlHelper.GetBlogArticleUrlTag(articleID) + "\" target=\"_blank\">" + title + "</a>";

            CreatCommentFeed(userID, targetUserID, articleID, type, title, string.Empty);
        }

        public void CreatCommentFeed(int userID, int targetUserID, int targetID, string type, string title, string content)
        {
            //"{actor} 评论了 {targetUser} {type} {title}   {dateTime}";

            List<string> titleDatas = new List<string>();
            titleDatas.Add(type);
            titleDatas.Add(title);
            List<string> descriptionDatas  = new List<string>();
            descriptionDatas.Add(content);

            CreateAppFeed(userID, targetUserID, targetID, targetID, false, new BasicApp().AppID, (int)AppActionType.AddComment, titleDatas, descriptionDatas);
        }

        /// <summary>
        /// 留言时添加动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="targetUserID"></param>
        public void CreateLeveMessageFeed(int userID,int targetUserID)
        {
            CreateAppFeed(userID, targetUserID, 0, 0, false, new BasicApp().AppID, (int)AppActionType.LeaveMessage, null, null);
        }

        /// <summary>
        /// 添加分享动态
        /// </summary>
        /// <param name="actorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="sharID"></param>
        /// <param name="catagory"></param>
        /// <param name="content"></param>
        /// <param name="description"></param>
        public void CreateShareFeed(int actorUserID, int targetUserID, int sharID, ShareType catagory, PrivacyType privacyType, string content, string description)
        {
            //只有自己可见 不添加动态
            if (privacyType == PrivacyType.SelfVisible)
                return;
            
			string catagoryName = ShareBO.Instance.GetShareTypeName(catagory);
            //string commentLink = "<a href=\"$url(space/"+actorUserID+"/share/share-" + sharID+")\" target=\"_blank\">评论</a>";
            
			List<string> titleDatas = new List<string>();
            
			titleDatas.Add(catagoryName);
            //titleDatas.Add(commentLink);

            List<string> descriptionDatas = new List<string>();
            
			descriptionDatas.Add(content);
            descriptionDatas.Add(description);
            
			CreateAppFeed(actorUserID, targetUserID, sharID, sharID, privacyType, new List<int>(), false, new BasicApp().AppID, (int)AppActionType.Share, titleDatas, descriptionDatas, DateTimeUtil.Now);
        }


        public string GetShareFeedContent(ShareContent share, int shareID)
        {
            string url = @"
<div class=""detail"">
<a href=""{0}"" target=""_blank"">{1}</a>
</div>
";
            string media = @"
<div class=""videoplayer"">
<img src=""{0}"" alt="""" width=""120"" height=""90"" >
<a class=""video-play"" href=""javascript:;"" onclick=""javascript:showFlash('','{1}', '{2}', this.parentNode, '{3}');"">点击播放</a>
</div>
";
            if (share.Catagory == ShareType.Video)
            {
                string imgUrl;
                if (string.IsNullOrEmpty(share.ImgUrl))
                {
                    imgUrl = string.Format("{0}/max-assets/images/{1}", Globals.AppRoot, "default_media.gif");
                }
                else
                {
                    imgUrl = share.ImgUrl;
                }
                return string.Format(url, share.URL, string.IsNullOrEmpty(share.Title) ? share.URL : share.Title) + string.Format(media, imgUrl, share.Domain, share.Content, shareID);
            }
            else if (share.Catagory == ShareType.Flash)
                return string.Format(url, share.URL, string.IsNullOrEmpty(share.Title) ? share.URL : share.Title) + string.Format(media, "default_flash.gif", "flash", share.Content, shareID, Globals.AppRoot);
            else if (share.Catagory == ShareType.Music)
                return string.Format(url, share.URL, string.IsNullOrEmpty(share.Title) ? share.URL : share.Title) + string.Format(media, "default_music.gif", "music", share.Content, shareID, Globals.AppRoot);
            else if (share.Catagory == ShareType.URL)
                return string.Format(url, share.URL, string.IsNullOrEmpty(share.Title) ? share.URL : share.Title);
            else
                return string.Empty;
        }

        /// <summary>
        /// 发表日志时添加动态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="articleID"></param>
        /// <param name="imgurl"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public void CreateWriteArticleFeed(int userID, int articleID,PrivacyType privacyType,List<int> visibleUserIDs,string imgUrl, string title, string content)
        {
            //只有自己可见 不添加动态
            if (privacyType == PrivacyType.SelfVisible || privacyType == PrivacyType.NeedPassword)
                return;

            string readArticleUrl = UrlHelper.GetBlogArticleUrlTag(articleID);

			string img;

            if (string.IsNullOrEmpty(imgUrl))
                img = string.Empty;
            else
                img = string.Format(@"<a href=""{1}"" target=""_blank""><img src=""{0}"" class=""summaryimg"" onload=""imageScale(this, 100, 100)"" onerror=""this.style.display = 'none';""></a>", imgUrl, readArticleUrl);
            
			string tempContent = @"
<div class=""detail"">
<b><a href=""{2}"" target=""_blank"">{0}</a></b>
<br>
{1}
</div>
";
            content = StringUtil.ClearAngleBracket(content);
            content = StringUtil.CutString(content, Consts.Feed_ContentSummaryLength);
            tempContent = img + string.Format(tempContent, title, content, readArticleUrl);

            List<string> descriptionDatas = new List<string>();
            descriptionDatas.Add(tempContent);

            CreateAppFeed(userID, 0, articleID, articleID, privacyType, visibleUserIDs, false, new BasicApp().AppID, (int)AppActionType.WriteArticle, null, descriptionDatas, DateTimeUtil.Now);
        }

        public void CreateTopicFeed(int userID, int threadID, Forum forum, string title, string content)
        {
            if (forum.ForumType != ForumType.Link && string.IsNullOrEmpty(forum.Password) == false)
                return;

            string tempContent = @"
<div class=""detail"">
<b><a href=""" + UrlHelper.GetThreadUrlTag(forum.CodeName, threadID) + @""" target=""_blank"">{0}</a></b>
<br>
{1}
</div>
";
            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            title = keyword.Replace(title);
            content = keyword.Replace(content);

            content = StringUtil.ClearAngleBracket(content);
            content = StringUtil.CutString(content, Consts.Feed_ContentSummaryLength);
            tempContent = string.Format(tempContent, title, content);

            List<string> descriptionDatas = new List<string>();
            descriptionDatas.Add(tempContent);

            CreateAppFeed(userID, 0, threadID, threadID, false, new BasicApp().AppID, (int)AppActionType.CreateTopic, null, descriptionDatas);
        }

        
        /// <summary>
        /// 上传图片时添加动态
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="ablumID">相册ID</param>
        /// <param name="ablumName">相册名称</param>
        /// <param name="picturesCount">当前相册图片总数</param>
        /// <param name="pictures">最新的4张图片 int:图片ID,string:图片地址</param>
        /// <param name="privacyType">隐私类型</param>
        /// <param name="visibleUserIDs">可见用户ID</param>
        public void CreateUploadPictureFeed(int userID, int ablumID, string ablumName, int picturesCount, Dictionary<int, string> pictures, PrivacyType privacyType, List<int> visibleUserIDs)
        {
            if (privacyType == PrivacyType.SelfVisible)
                return;
            StringBuilder imgString = new StringBuilder();
            int i = 0;
            foreach (KeyValuePair<int, string> pair in pictures)
            {
                if (i == 4)
                    break;

                imgString.Append(string.Format(@"<a href=""{1}""><img src=""{0}"" /></a>&nbsp;", AlbumBO.Instance.GetPhotoThumbSrc(pair.Value), UrlHelper.GetPhotoUrlTag(pair.Key))
                );

                i++;
            }

            string ablum = string.Format(@"<a href=""{1}"">{0}</a>", ablumName, UrlHelper.GetAblumUrlTag(ablumID));

            List<string> descriptionDatas = new List<string>();
            descriptionDatas.Add(imgString.ToString());
            descriptionDatas.Add(ablum);
            descriptionDatas.Add(picturesCount.ToString());

            CreateAppFeed(userID, 0, ablumID, ablumID, privacyType, visibleUserIDs, false, new BasicApp().AppID, (int)AppActionType.UploadPicture, null, descriptionDatas, DateTimeUtil.Now);
        }

        /// <summary>
        /// 参与任务时添加动态
        /// </summary>
        public void CreateApplyMissionFeed(int userID,int missionID,string missionName)
        {
            List<string> titalDatas = new List<string>();

            string title = string.Format(@"<a href=""{1}"" target=""_blank"">{0}</a>", missionName, UrlHelper.GetMissionDetailUrlTag(missionID));
            
			titalDatas.Add(title);
            
			CreateAppFeed(userID, 0, missionID, missionID, false, new BasicApp().AppID, (int)AppActionType.ApplyMission, titalDatas, null);
        }


        /// <summary>
        /// 增加自己的竞价
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="point">积分</param>
        /// <param name="description">上榜宣言</param>
        public void CreateBidUpSelfFeed(int userID, int point, string description)
        {
            string link = "<a href=\"" + UrlHelper.GetMemberUrlTag("show") + "\" target=\"_blank\">竞价排行榜</a>";

            List<string> titleDatas = new List<string>();
            titleDatas.Add(point.ToString());
            titleDatas.Add(link);
            
            List<string> descriptionDatas = new List<string>();
            descriptionDatas.Add(description);
            
            CreateAppFeed(userID, 0, null, 0, false, Consts.App_BasicAppID, (int)AppActionType.BidUpSelf, titleDatas, descriptionDatas);
        }

        /// <summary>
        /// 增加好友的竞价
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="targetUserID">好友ID</param>
        /// <param name="point">积分</param>
        public void CreateBidUpFriendFeed(int userID,int targetUserID,int point)
        {
            string link = "<a href=\"" + UrlHelper.GetMemberUrlTag("show") + "\" target=\"_blank\">竞价排行榜</a>";
            
			List<string> titleDatas = new List<string>();

            titleDatas.Add(point.ToString());
            titleDatas.Add(link);
            
			CreateAppFeed(userID, targetUserID, null, 0, false, Consts.App_BasicAppID, (int)AppActionType.BidUpFriend, titleDatas, null);
        }


        private int CreateAppFeed(int userID, int targetUserID, int? targetID, int commentTargetID, bool isSpecail, Guid appID, int actionType, IEnumerable<string> titleTemplateDatas, IEnumerable<string> descriptionTemplateDatas)
        {
            return CreateAppFeed(userID, targetUserID, targetID, commentTargetID, PrivacyType.AllVisible, null, isSpecail, appID, actionType, titleTemplateDatas, descriptionTemplateDatas, DateTimeUtil.Now);
        }


#endregion
        /// <summary>
        /// 添加动态
        /// </summary>
        /// <param name="userID">引发动态的人</param>
        /// <param name="targetUserID">目标用户如a评论了b的日志 就是b的UserID 如果是c发表了日志就为0</param>
        /// <param name="targetID">评论了日志 那就是日志ID</param>
        /// <param name="isSpecail">通常为false 加好友为true</param>
        /// <param name="appID">应用ID</param>
        /// <param name="actionType">动作类型</param>
        /// <param name="titleTemplateDatas">标题模板标签对应的数据，注意顺序</param>
        /// <param name="descriptionTemplateDatas">内容模板标签对应的数据，注意顺序</param>
        /// <returns>返回FeedID</returns>
        public int CreateAppFeed(int userID, int targetUserID, int? targetID, int commentTargetID, PrivacyType privacyType, List<int> visibleUserIDs, bool isSpecail, Guid appID, int actionType, IEnumerable<string> titleTemplateDatas, IEnumerable<string> descriptionTemplateDatas, DateTime createDate)
        {
            if (userID == targetUserID)//对自己的操作 不添加动态
                return 0;

            AppBase app = AppManager.GetApp(appID);
            if (app == null)
                return 0;


            AppAction appAction = app.AppActions.GetValue(actionType);
            if (appAction == null)
                return 0;

            bool canJoin = appAction.CanJoin;

            //foreach (FeedSendItem item in AllSettings.Current.PrivacySettings.FeedSendItems)
            //{
            //    if (item.AppID == appID && item.ActionType == actionType)
            //    {
            //        if (item.DefaultSendType == FeedSendItem.SendType.ForceNotSend)
            //            return 0;
            //        if (item.DefaultSendType == FeedSendItem.SendType.ForceSend)
            //        {
            //            break;
            //        }

            //        UserNoAddFeedAppCollection noAddFeedApps = GetUserNoAddFeedApps(userID);
            //        foreach (UserNoAddFeedApp userNoAddFeedApp in noAddFeedApps)
            //        {
            //            if (userNoAddFeedApp.AppID == appID && userNoAddFeedApp.ActionType == actionType)
            //            {
            //                //不发送
            //                return 0;
            //            }
            //        }
            //        if (item.DefaultSendType == FeedSendItem.SendType.NotSend)
            //            return 0;

            //        if (isSpecail)
            //        {
            //            UserNoAddFeedAppCollection noAddFeedApps = GetUserNoAddFeedApps(targetUserID);
            //            foreach (UserNoAddFeedApp userNoAddFeedApp in noAddFeedApps)
            //            {
            //                if (userNoAddFeedApp.AppID == appID && userNoAddFeedApp.ActionType == actionType)
            //                {
            //                    //不发送
            //                    return 0;
            //                }
            //            }
            //        }

            //        break;
            //    }
                
            //}

            FeedTemplate template = GetFeedTemplate(appID, actionType);

            string title;
            if (template == null)
                title = appAction.TitleTemplate;
            else
                title = template.Title;
            //替换标题模板标签
            if (titleTemplateDatas != null && appAction.TitleTemplateTags != null)
            {
                int i = 0;
                foreach (string tag in appAction.TitleTemplateTags)
                {
                    int j = 0;
                    foreach (string data in titleTemplateDatas)
                    {
                        if (i == j)
                        {
                            title = Regex.Replace(title, tag, data, RegexOptions.IgnoreCase);
                            break;
                        }
                        j++;
                    }
                    i++;
                }
            }

            string description;
            if (template == null)
                description = appAction.DescriptionTemplate;
            else
                description = template.Description;

            //替换简介模板标签
            if (descriptionTemplateDatas != null && appAction.DescriptionTemplateTags != null)
            {
                int i = 0;
                foreach (string tag in appAction.DescriptionTemplateTags)
                {
                    int j = 0;
                    foreach (string data in descriptionTemplateDatas)
                    {
                        if (i == j)
                        {
                            description = Regex.Replace(description, tag, data, RegexOptions.IgnoreCase);
                            break;
                        }
                        j++;
                    }
                    i++;
                }
            }

            Feed feed = new Feed();
            feed.AppID = appID;
            feed.ActionType = actionType;
            feed.PrivacyType = privacyType;
            feed.VisibleUserIDs = visibleUserIDs;
            feed.CreateDate = createDate;
            feed.CommentTargetID = commentTargetID;
            feed.CommentInfo = null;
            feed.Description = (description == null ? "" : description);
            feed.TargetID = targetID;//(targetID == null ? 0 : targetID.Value);
            if (targetUserID != 0)
            {
                User targetUser = UserBO.Instance.GetUser(targetUserID);
                if (targetUser == null)
                    return 0;
                feed.TargetNickname = targetUser.Name;
                if (feed.TargetNickname == null)
                    feed.TargetNickname = "该用户已被删除";
            }
            else
                feed.TargetNickname = string.Empty;

            feed.IsSpecial = isSpecail;
            feed.TargetUserID = targetUserID;
            feed.Title = title;

            string nickName;
            if (userID > 0)
            {
                User user = UserBO.Instance.GetUser(userID);
                nickName = (user == null ? string.Empty : user.Name);
            }
            else
                nickName = string.Empty;

            UserFeed userFeed = new UserFeed();
            userFeed.CreateDate = createDate;
            userFeed.Realname = nickName;
            userFeed.UserID = userID;

            return CreateFeed(feed, userFeed, canJoin);

        }








    }
}