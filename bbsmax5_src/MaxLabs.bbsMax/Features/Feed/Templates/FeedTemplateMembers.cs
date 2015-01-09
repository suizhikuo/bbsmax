//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class FeedTemplateMembers
    {

        /*

        #region 动态列表

        public delegate void FeedListHeadFootTemplate(FeedListHeadFootParams _this, bool hasItems, bool haveMoreFeeds, int nextCount, bool hasError, bool displayFriendVisibleError, bool displayAdminVisibleInfo);
        public delegate void FeedListItemTemplate(Feed feed, string formatedFeedTitle, string formatedFeedDescription, bool canDisplayShield, bool canDisplayDeleteSelf, bool canDisplayDeleteAnyFeed,bool isSelf,int shieldFriendUserID);


        public class FeedListHeadFootParams
        {

            public FeedListHeadFootParams(FeedType currentFeedType)
            {
                m_CurrentFeedType = currentFeedType;
            }

            private FeedType m_CurrentFeedType;

            private bool? m_CanDisplayFriend;
            /// <summary>
            /// 是否显示好友动态 只有登陆后才显示
            /// </summary>
            public bool CanDisplayFriend
            {
                get
                {
                    if (m_CanDisplayFriend == null)
                    {
                        m_CanDisplayFriend = UserBO.Instance.GetUserID() > 0;
                    }
                    return m_CanDisplayFriend.Value;
                }
            }

            private bool? m_CanDisplaySelf;
            /// <summary>
            /// 是否显示自己动态 只有登陆后才显示
            /// </summary>
            public bool CanDisplaySelf
            {
                get
                {
                    if (m_CanDisplaySelf == null)
                    {
                        m_CanDisplaySelf = UserBO.Instance.GetUserID() > 0;
                    }
                    return m_CanDisplaySelf.Value;
                }
            }

            public string IsSelected(FeedType feedType, string value)
            {
                if (m_CurrentFeedType == feedType)
                    return value;
                else
                    return string.Empty;
            }


        }
        /// <summary>
        /// space-home.aspx
        /// </summary>
        [TemplateTag]
        public void FeedList(
              string feedType
            , int? count
            , string nextCount
            , string appID
            , string actionType
            , GlobalTemplateMembers.CannotDoTemplate cannotDo
            , GlobalTemplateMembers.NodataTemplate nodata
            , FeedListHeadFootTemplate head
            , FeedListHeadFootTemplate foot
            , FeedListItemTemplate item)
        {
            FeedList(feedType, count, nextCount, appID, actionType, 0, cannotDo, nodata, head, foot, item);
        }

        /// <summary>
        /// user-feeds.aspx
        /// </summary>
        [TemplateTag]
        public void FeedList(
              int uid
            , string appID
            , string actionType
            , GlobalTemplateMembers.CannotDoTemplate cannotDo
            , GlobalTemplateMembers.NodataTemplate nodata
            , FeedListHeadFootTemplate head
            , FeedListHeadFootTemplate foot
            , FeedListItemTemplate item)
        {
            FeedList(FeedType.MyFeed.ToString(), int.MaxValue, null, appID, actionType, uid, cannotDo, nodata, head, foot, item);
        }

        [TemplateTag]
        public void FeedList(
              int uid
            , int count
            //, string appID
            //, string actionType
            //, int? feedID
            , GlobalTemplateMembers.CannotDoTemplate cannotDo
            , GlobalTemplateMembers.NodataTemplate nodata
            , FeedListHeadFootTemplate head
            , FeedListHeadFootTemplate foot
            , FeedListItemTemplate item)
        {
            FeedList(FeedType.MyFeed.ToString(), count, null, null, null, uid, cannotDo, nodata, head, foot, item);
        }

        private void FeedList(
              string feedType
            , int? count
            , string nextCount
            , string appID
            , string actionType
            , int uid
            , GlobalTemplateMembers.CannotDoTemplate cannotDo
            , GlobalTemplateMembers.NodataTemplate nodata
            , FeedListHeadFootTemplate head
            , FeedListHeadFootTemplate foot
            , FeedListItemTemplate item)
        {
            if (count == null)
                count = int.MaxValue;
            int getCount;
            if (!int.TryParse(nextCount, out getCount))
            {
                getCount = count.Value;
            }

            Guid appid = Guid.Empty;
            int? action = null;

            if (appID != null)
            {
                try
                {
                    appid = new Guid(appID);
                }
                catch { }
            }


            if (appid != Guid.Empty)
            {
                int a;
                bool success = StringUtil.TryParse<int>(actionType, out a);
                if (success)
                    action = a;
            }


            int loginUserID = UserBO.Instance.GetUserID();
            if (uid < 1)
            {
                uid = loginUserID;
            }

            bool isSelf = (uid == loginUserID);
            //起始FeedID
            int tempFeedID = int.MaxValue;//(feedID == null ? int.MaxValue : feedID.Value);
            bool haveMore = false;

            FeedType currentFeedType = GetFeedType(feedType);

            FeedCollection feeds;// = new List<Feed>();

            bool displayFriendVisibleError = false;
            bool displayAdminVisibleInfo = false;
            bool hasError = false;
            if (currentFeedType == FeedType.AllUserFeed)
            {
                feeds = FeedBO.Instance.GetAllUserFeeds(uid, tempFeedID, getCount, appid, action, out haveMore);
            }
            else if (currentFeedType == FeedType.FriendFeed)
            {
                feeds = FeedBO.Instance.GetFriendFeeds(uid, tempFeedID, getCount, appid, action, out haveMore);
            }
            else
            {

                if (loginUserID != uid)
                {

                    FeedBO.FeedDisplayType feedDisplayType = FeedBO.Instance.GetFeedDisplayType(loginUserID, uid);
                    switch (feedDisplayType)
                    {
                        case FeedBO.FeedDisplayType.AdminVisibleInfo:
                            displayAdminVisibleInfo = true;
                            break;
                        case FeedBO.FeedDisplayType.FriendVisibleError:
                            displayFriendVisibleError = true;
                            hasError = true;
                            break;
                        case FeedBO.FeedDisplayType.OtherError:
                            hasError = true;
                            break;
                        default: break;
                    }
                }

                if (hasError)
                {
                    FeedListHeadFootParams tempHeadFoot = new FeedListHeadFootParams(currentFeedType);
                    head(tempHeadFoot, false, false, 0, true, displayFriendVisibleError, displayAdminVisibleInfo);
                    foot(tempHeadFoot, false, false, 0, true, displayFriendVisibleError, displayAdminVisibleInfo);
                    return;
                }
                else
                    feeds = FeedBO.Instance.GetUserFeeds(loginUserID, uid, tempFeedID, getCount, appid, action, out haveMore);
            }



            int tempCount = feeds.Count;
            FeedListHeadFootParams headFoot = new FeedListHeadFootParams(currentFeedType);
            int nextGetCount;
            if (getCount == int.MaxValue || count.Value == int.MaxValue)
                nextGetCount = int.MaxValue;
            else
                nextGetCount = getCount + count.Value;

            head(headFoot, tempCount > 0, haveMore, nextGetCount, hasError, displayFriendVisibleError, displayAdminVisibleInfo);


            if (tempCount > 0)
            {
                bool canDisplayShield = false, canDisplayDeleteSelf = false, canDisplayDeleteAny = false;
                if (currentFeedType == FeedType.MyFeed && uid == loginUserID)
                {
                    canDisplayDeleteSelf = true;
                }
                //else if (currentFeedType == FeedType.AllUserFeed)
                //{
                //    canDisplayDeleteAny = true;
                //}
                else if (currentFeedType == FeedType.FriendFeed)
                    canDisplayShield = true;

                bool tempCanDisplayShield = canDisplayShield;
                int shieldFriendUserID = 0;
                bool canManageFeed = FeedBO.Instance.ManagePermission.Can(loginUserID,MaxLabs.bbsMax.Settings.ManageSpacePermissionSet.Action.ManageFeed);
                foreach (Feed feed in feeds)
                {
                    if (canDisplayShield && FeedBO.Instance.IsSiteFeed(feed))
                        tempCanDisplayShield = false;
                    if (tempCanDisplayShield)
                    {
                        if (feed.IsSpecial && FriendBO.Instance.IsFriend(loginUserID, feed.TargetUserID))
                            shieldFriendUserID = feed.TargetUserID;
                        else
                        {
                            if (feed.Users.Count > 0)
                                shieldFriendUserID = feed.Users[0].UserID;
                            else
                                shieldFriendUserID = 0;
                        }
                    }

                    if (currentFeedType == FeedType.AllUserFeed)
                    {
                        canDisplayDeleteAny = canManageFeed;
                    }
                    double timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(UserBO.Instance.GetUserID());

                    string title = FeedBO.Instance.FormatFeedTitle(uid, timeDiffrence, currentFeedType, feed);
                    string description = FeedBO.Instance.FormatFeedDescription(uid, feed);
                    item(feed, title, description, tempCanDisplayShield, canDisplayDeleteSelf, canDisplayDeleteAny, isSelf, shieldFriendUserID);
                    tempCanDisplayShield = canDisplayShield;
                }
            }

            foot(headFoot, tempCount > 0, haveMore, nextGetCount,hasError,displayFriendVisibleError,displayAdminVisibleInfo);

            if (tempCount == 0)
                nodata();
        }

        public delegate void NetWorkFeedListHeadFootTemplate(bool hasItems);
        public delegate void NetWorkFeedListItemTemplate(int i, Feed feed, string formatedFeedTitle, string formatedFeedDescription);
        [TemplateTag]
        public void NetWorkFeedList(int count
            ,NetWorkFeedListHeadFootTemplate head
            ,NetWorkFeedListHeadFootTemplate foot
            ,NetWorkFeedListItemTemplate item)
        {
            FeedCollection feeds = FeedBO.Instance.GetNetWorkFeeds(count);
            int uid = UserBO.Instance.GetUserID();

            int getCount = feeds.Count;
            
            int i = 0;
            head(getCount > 0);

            double timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(UserBO.Instance.GetUserID());

            foreach (Feed feed in feeds)
            {
                string title = FeedBO.Instance.FormatFeedTitle(uid,timeDiffrence, FeedType.AllUserFeed, feed);
                string description = FeedBO.Instance.FormatFeedDescription(uid, feed);
                item(i++, feed, title, description);
            }
            foot(getCount > 0);
        }

        private FeedType GetFeedType(string currentFeedTypeString)
        {
            if (UserBO.Instance.GetUserID() == 0)
            {
                return FeedType.AllUserFeed;
            }
            else
            {
                if (currentFeedTypeString == null)
                {
                    return FeedType.FriendFeed;//默认显示好友动态
                }
                else
                {
                    try
                    {
                        return (FeedType)Enum.Parse(typeof(FeedType), currentFeedTypeString.Trim());
                    }
                    catch
                    {
                        return FeedType.FriendFeed;
                    }
                }
            }
        }





        /// <summary>
        /// 返回第一个用户ID
        /// </summary>
        /// <param name="userFeeds"></param>
        /// <returns></returns>
        [TemplateFunction]
        public int GetUserID(UserFeedCollection userFeeds)
        {
            foreach (UserFeed userFeed in userFeeds)
            {
                return userFeed.UserID;
            }
            return 0;
        }



        #endregion

        */

        /// <summary>
        /// 应用图标
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        [TemplateFunction]
        public string GetAppActionIconUrl(Guid appID, int actionType)
        {
            FeedTemplate template = FeedBO.Instance.GetFeedTemplate(appID, actionType);

            if (template != null)
                return template.IconUrl;

            AppBase app = AppManager.GetApp(appID);
            if (app == null)
                return string.Empty;

            AppAction appAction = app.AppActions.GetValue(actionType);
            if (appAction == null)
                return string.Empty;

            return appAction.IconUrl;
        }

        #region App

        public delegate void AppListHeadFootTemplate();
        public delegate void AppListItemTemplate(int i, AppBase app);

        [TemplateTag]
        public void AppList(
              GlobalTemplateMembers.CannotDoTemplate cannotDo
            , GlobalTemplateMembers.NodataTemplate nodata
            , AppListHeadFootTemplate head
            , AppListHeadFootTemplate foot
            , AppListItemTemplate item)
        {
            AppBaseCollection apps = AppManager.GetAllApps();

            int i = 0;

            head();

            foreach (AppBase app in apps)
            {
                item(i++, app);
            }

            if (apps.Count == 0)
                nodata();

            foot();

        }


        public delegate void AppActionListHeadFootTemplate();
        public delegate void AppActionListItemTemplate(int j, AppAction appAction);

        [TemplateTag]
        public void AppActionList(
              AppBase app
            , GlobalTemplateMembers.CannotDoTemplate cannotDo
            , GlobalTemplateMembers.NodataTemplate nodata
            , AppActionListHeadFootTemplate head
            , AppActionListHeadFootTemplate foot
            , AppActionListItemTemplate item)
        {
            AppActionCollection appActions = app.AppActions;

            head();

            int j = 0;

            foreach (AppAction appAction in appActions)
            {
                if (FeedBO.Instance.IsSiteFeed(app.AppID, appAction.ActionType))
                    continue;

                bool display = true;
                foreach (FeedSendItem tempItem in AllSettings.Current.PrivacySettings.FeedSendItems)
                {
                    if ((tempItem.DefaultSendType == FeedSendItem.SendType.ForceSend || tempItem.DefaultSendType == FeedSendItem.SendType.ForceNotSend)
                        && tempItem.AppID == app.AppID && tempItem.ActionType == appAction.ActionType)
                        display = false;

                }

                if (display)
                    item(j++, appAction);
            }

            if (appActions.Count == 0)
                nodata();

            foot();

        }



        private int? appCount;
        /// <summary>
        /// 系统应用总数
        /// </summary>
        [TemplateVariable]
        public int AppCount
        {
            get
            {
                if (appCount == null)
                {
                    appCount = AppManager.GetAllApps().Count;
                }
                return appCount.Value;
            }
        }


        #endregion

        #region FeedFilter
        /// <summary>
        /// 过滤设置 模板
        /// </summary>
        /// <param name="i">当前记录的序号，从0开始计数</param>
        /// <param name="feedFilter">过滤条件</param>
        public delegate void FeedFiltersItemTemplate(int i, FeedFilter feedFilter);
        public delegate void FeedFiltersHeadFootTemplate();


        /// <summary>
        /// 过滤设置 数据
        /// </summary>
        [TemplateTag]
        public void FeedFilterList(FilterType filterType
            , FeedFiltersHeadFootTemplate head
            , FeedFiltersHeadFootTemplate foot
            , FeedFiltersItemTemplate item)
        {
            FeedFilterCollection tempFeedFilters = new FeedFilterCollection();
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (filterType == FilterType.FilterApp || filterType == FilterType.FilterAppAction)
                {
                    if (feedFilter.FilterType == FilterType.FilterApp || feedFilter.FilterType == FilterType.FilterAppAction)
                    {
                        tempFeedFilters.Add(feedFilter);
                    }
                }
                else if (feedFilter.FilterType == filterType)
                {
                    //if(filterType == FilterType.FilterUser)
                    tempFeedFilters.Add(feedFilter);
                }
            }

            int i = 0;

            head();

            foreach (FeedFilter feedFilter in tempFeedFilters)
            {
                item(i++, feedFilter);
            }
            foot();
        }



        /// <summary>
        /// 被屏蔽指定应用指定动作的好友
        /// </summary>
        /// <param name="template"></param>
        [TemplateTag]
        public void FiltratedAppFriendList(
              FeedFiltersHeadFootTemplate head
            , FeedFiltersHeadFootTemplate foot
            , FeedFiltersItemTemplate item
            )
        {
            int i = 0;
            List<FeedFilter> feedFilters = new List<FeedFilter>();
            List<int> userIDs = new List<int>();
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.FilterType == FilterType.FilterUserAppAction)
                {
                    if (!userIDs.Contains(feedFilter.FriendUserID.Value))
                    {
                        userIDs.Add(feedFilter.FriendUserID.Value);
                        feedFilters.Add(feedFilter);
                    }
                }
            }

            head();

            foreach (FeedFilter feedFilter in feedFilters)
            {
                item(i++, feedFilter);
            }

            foot();
        }


        public delegate void ActionFeedFiltersTemplate(int j, FeedFilter feedFilter);
        /// <summary>
        /// 过滤设置 数据  被屏蔽指定应用指定动作
        /// </summary>
        [TemplateTag]
        public void ActionFeedFilterList(int userID
            , FeedFiltersHeadFootTemplate head
            , FeedFiltersHeadFootTemplate foot
            , ActionFeedFiltersTemplate item)
        {
            int j = 0;
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.FilterType == FilterType.FilterUserAppAction && feedFilter.FriendUserID.Value == userID)
                {
                    item(j++, feedFilter);
                }
            }
        }





        /// <summary>
        /// 获取指定应用的指定动作名称
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        [TemplateFunction]
        public string GetActionName(Guid appID, int actionType)
        {
            AppAction appAction = AppManager.GetApp(appID).AppActions.GetValue(actionType);
            if (appAction != null)
                return appAction.ActionName;
            else
                return string.Empty;
        }

        /// <summary>
        /// 被过滤动态的应用
        /// </summary>
        /// <param name="template"></param>
        [TemplateTag]
        public void FiltratedAppList(
              AppListHeadFootTemplate head
            , AppListHeadFootTemplate foot
            , AppListItemTemplate item)
        {
            List<Guid> appIDs = new List<Guid>();
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.AppID != Guid.Empty && feedFilter.FriendUserID == null && !appIDs.Contains(feedFilter.AppID))
                {
                    appIDs.Add(feedFilter.AppID);
                }
            }
            AppBaseCollection apps = AppManager.GetApps(appIDs);

            int i = 0;
            head();
            foreach (AppBase app in apps)
            {
                item(i++, app);
            }
            foot();
        }


        /// <summary>
        /// 应用动作显示模板
        /// </summary>
        /// <param name="i">当前记录的序号，从0开始计数</param>
        /// <param name="app">应用</param>
        public delegate void FiltratedAppActionListItemTemplate(int j, AppAction appAction);
        public delegate void FiltratedAppActionListHeadFootTemplate();

        /// <summary>
        /// 被过滤动态的应用动作
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="template"></param>
        [TemplateTag]
        public void FiltratedAppActionList(Guid appID
            , FiltratedAppActionListHeadFootTemplate head
            , FiltratedAppActionListHeadFootTemplate foot
            , FiltratedAppActionListItemTemplate item)
        {
            AppActionCollection appActions = new AppActionCollection();
            AppBase app = AppManager.GetApp(appID);
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.AppID == appID)
                {
                    if (feedFilter.FilterType == FilterType.FilterApp)
                    {
                        appActions = app.AppActions;
                        break;
                    }
                    else if (feedFilter.FilterType == FilterType.FilterAppAction)
                    {
                        AppAction appAction = app.AppActions.GetValue(feedFilter.ActionType.Value);
                        if (appAction != null)
                            appActions.Add(appAction);
                    }
                }
            }

            int j = 0;
            head();
            foreach (AppAction appAction in appActions)
            {
                item(j++, appAction);
            }
            foot();
        }


        /// <summary>
        /// 获取过滤ID
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        [TemplateFunction]
        public int GetFeedFilterID(Guid appID, int actionType)
        {
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.AppID == appID && feedFilter.ActionType == actionType && feedFilter.FriendUserID == null)
                    return feedFilter.ID;
            }
            return 0;
        }
        #endregion

        #region feedTemplates

        public delegate void FeedTemplateHeadFootItemTemplate(bool hasItems, int totalFeedTemplates, AppBase currentApp);
        public delegate void FeedTemplateListItemTemplate(int i, FeedTemplate feedTemplate, AppBase currentApp);


        /// <summary>
        /// 通知模板数据
        /// </summary>
        [TemplateTag]
        public void FeedTemplateList(string appID
            , GlobalTemplateMembers.CannotDoTemplate canNotDo
            , FeedTemplateHeadFootItemTemplate head
            , FeedTemplateHeadFootItemTemplate foot
            , FeedTemplateListItemTemplate item)
        {
            Guid currentAppID;
            try
            {
                currentAppID = new Guid(appID);
            }
            catch
            {
                currentAppID = new BasicApp().AppID;
                //throw new Exception("非法的AppID");
            }


            FeedTemplateCollection feedTemplates;
            using (ErrorScope errorScope = new ErrorScope())
            {
                feedTemplates = FeedBO.Instance.GetFeedTemplates(currentAppID);
                errorScope.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    canNotDo(error.Message);
                    return;
                });
            }

            int totalCount = feedTemplates.Count;

            AppBase currentApp = AppManager.GetApp(currentAppID);

            head(totalCount > 0, totalCount, currentApp);

            int i = 0;

            foreach (FeedTemplate feedTemplate in feedTemplates)
            {
                item(i++, feedTemplate, currentApp);
            }

            foot(totalCount > 0, totalCount, currentApp);
        }


        [TemplateFunction]
        public string GetActionNameByType(AppBase app, int actionType)
        {
            AppAction appAction = app.AppActions.GetValue(actionType);
            if (appAction != null)
                return appAction.ActionName;
            else
                return string.Empty;
        }

        #endregion


        #region sitefeed 全局动态


        public delegate void SiteFeedListHeadFootTemplate(bool hasItems,int totalFeeds,bool canDelete);
        public delegate void SiteFeedListItemTemplate(int i,Feed feed,bool canDelete,bool canEdit, string formatedFeedTitle, string formatedFeedDescription);

        [TemplateTag]
        public void SiteFeedList(
             SiteFeedListHeadFootTemplate head
            ,SiteFeedListHeadFootTemplate foot
            ,SiteFeedListItemTemplate item)
        {
            FeedCollection feeds = FeedBO.Instance.GetAllSiteFeeds();
            int total = feeds.Count;
            int i = 0;

            AuthUser user = User.Current;
            int loginUserID = user.UserID;

            bool canManageSiteFeed = FeedBO.Instance.ManagePermission.Can(user, BackendPermissions.Action.Manage_Feed_SiteFeed);
            head(total > 0, total, canManageSiteFeed);


            float timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(user);

            foreach (Feed feed in feeds)
            {
                string title = FeedBO.Instance.FormatFeedTitle(loginUserID, timeDiffrence, FeedType.AllUserFeed, feed);
                string description = FeedBO.Instance.FormatFeedDescription(loginUserID, feed);
                item(i++, feed, canManageSiteFeed, canManageSiteFeed, title, description);
            }

            foot(total > 0, total, canManageSiteFeed);
        }

        public delegate void SiteFeedTemplate(bool isEdit,string title, string content, string description, string createDate
            , string errorMessage,bool hasError
            , string image1, string image2, string image3, string image4
            , string link1, string link2, string link3, string link4);
        [TemplateTag]
        public void SiteFeed(int feedID,SiteFeedTemplate template)
        {

            string title = string.Empty, content = string.Empty, description = string.Empty;
            string image1 = string.Empty, image2 = string.Empty, image3 = string.Empty, image4 = string.Empty;
            string link1 = string.Empty, link2 = string.Empty, link3 = string.Empty, link4 = string.Empty;

            if (feedID == 0)
            {
                template(false,title, content, description, DateTimeUtil.Now.ToString(), null, false, image1, image2, image3, image4, link1, link2, link3, link4);
                return;
            }

            using (ErrorScope errorScope = new ErrorScope())
            {
                Feed feed = FeedBO.Instance.GetSiteFeed(feedID);
                errorScope.CatchError<ErrorInfo>(delegate(ErrorInfo error) {

                    template(true,null, null, null, null, error.Message, true, null, null, null, null, null, null, null, null);
                });

                if (errorScope.HasError)
                    return;

                List<string> imageUrls, imageLinks;
                FeedBO.Instance.GetSiteFeed(feed, out title, out content, out description, out imageUrls, out imageLinks);

                for (int i = 0; i < imageUrls.Count; i++)
                {
                    if (i == 0)
                    {
                        image1 = imageUrls[i];
                        link1 = imageLinks[i];
                    }
                    else if (i == 1)
                    {
                        image2 = imageUrls[i];
                        link2 = imageLinks[i];
                    }
                    else if (i == 2)
                    {
                        image3 = imageUrls[i];
                        link3 = imageLinks[i];
                    }
                    else if (i == 3)
                    {
                        image4 = imageUrls[i];
                        link4 = imageLinks[i];
                    }
                }

                template(true,title, content, description, feed.CreateDate.ToString(), null, false, image1, image2, image3, image4, link1, link2, link3, link4);

            }

        }

        #endregion


        #region Feed后台搜索

        public delegate void FeedSearchListHeadFootTemplate(FeedSearchFilter filter, bool hasItems, int totalFeeds, int pageSize);
        public delegate void FeedSearchListItemTemplate(int i, Feed feed, string formatedFeedTitle, string formatedFeedDescription,bool canDelete);

        [TemplateTag]
        public void FeedSearchList(string filter, int page, int defaultPageSize
            , FeedSearchListHeadFootTemplate head
            , FeedSearchListHeadFootTemplate foot
            , FeedSearchListItemTemplate item)
        {
            FeedSearchFilter feedSearchFilter = FeedSearchFilter.GetFromFilter(filter);
            int totalCount;
            FeedCollection feeds = new FeedCollection();
            //int pageSize;

            FeedSearchFilter tempFilter;
            if (feedSearchFilter.IsNull)
            {
                tempFilter = new FeedSearchFilter();
                tempFilter.PageSize = defaultPageSize;
                tempFilter.Order = FeedSearchFilter.OrderBy.ID;
                tempFilter.IsDesc = true;
            }
            else
                tempFilter = feedSearchFilter;
            
            feeds = FeedBO.Instance.SearchFeeds(page, tempFilter, out totalCount);

            head(feedSearchFilter, totalCount > 0, totalCount, tempFilter.PageSize);

            int loginUserID = UserBO.Instance.GetCurrentUserID();
            int i = 0;

            float timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(User.Current);


            foreach (Feed feed in feeds)
            {
                string title = FeedBO.Instance.FormatFeedTitle(loginUserID, timeDiffrence, FeedType.AllUserFeed, feed);
                string description = FeedBO.Instance.FormatFeedDescription(loginUserID, feed);
                item(i++, feed, title, description, true);
            }

            foot(feedSearchFilter, totalCount > 0, totalCount, tempFilter.PageSize);
        }

        #endregion






        //==============================================================


        /// <summary>
        /// 应用显示模板l
        /// </summary>
        /// <param name="i">当前记录的序号，从0开始计数</param>
        /// <param name="app">应用</param>
        public delegate void AppsTemplate(int i, AppBase app);


        /// <summary>
        /// 应用数据
        /// </summary>
        [TemplateTag]
        public void AppList(AppsTemplate template)
        {
            AppBaseCollection apps = AppManager.GetAllApps();

            int i = 0;

            foreach (AppBase app in apps)
            {
                template(i++, app);
            }
        }


        /// <summary>
        /// 应用动作显示模板
        /// </summary>
        /// <param name="i">当前记录的序号，从0开始计数</param>
        /// <param name="app">应用</param>
        public delegate void AppActionsTemplate(int j, AppAction appAction);


        /// <summary>
        /// 应用动作数据
        /// </summary>
        [TemplateTag]
        public void AppActionList(AppBase app, AppActionsTemplate template)
        {
            AppActionCollection appActions = app.AppActions;

            int j = 0;

            foreach (AppAction appAction in appActions)
            {
                template(j++, appAction);
            }
        }


        private UserNoAddFeedAppCollection userNoAddFeedApps;
        private UserNoAddFeedAppCollection UserNoAddFeedApps
        {
            get
            {
                if (userNoAddFeedApps == null)
                {
                    int userID = UserBO.Instance.GetCurrentUserID();
                    userNoAddFeedApps = FeedBO.Instance.GetUserNoAddFeedApps(userID);
                }
                return userNoAddFeedApps;
            }
        }

        /// <summary>
        /// 指定类型的动作是否加入动态
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsAddFeedAction(Guid appID, int actionType)
        {
            int userID = UserBO.Instance.GetCurrentUserID();
            return UserNoAddFeedApps.IsAddFeedAction(userID, appID, actionType);
        }


        ///// <summary>
        ///// 指定应用是否全部动作都加入动态
        ///// </summary>
        ///// <param name="appID"></param>
        ///// <param name="actionType"></param>
        ///// <returns></returns>
        //[TemplateFunction]
        //public bool IsAddFeedApp(Guid appID)
        //{
        //    AppBase app = AppManager.GetApp(appID);
        //    return UserNoAddFeedApps.Count == 0;
        //}


        //==========================================================================




        private FeedFilterCollection userFeedFilters;
        private FeedFilterCollection UserFeedFilters
        {
            get
            {
                if (userFeedFilters == null)
                {
                    int userID = UserBO.Instance.GetCurrentUserID();
                    userFeedFilters = FeedBO.Instance.GetFeedFilters(userID);
                }
                return userFeedFilters;
            }
        }


        private int? friendFeedFilterCount = null;
        [TemplateVariable]
        public int FriendFeedFilterCount
        {
            get
            {
                if (friendFeedFilterCount == null)
                {
                    int count = 0;
                    foreach (FeedFilter feedFilter in UserFeedFilters)
                    {
                        if (feedFilter.FilterType == FilterType.FilterUser || feedFilter.FilterType == FilterType.FilterUserAppAction)
                            count++;
                    }
                    friendFeedFilterCount = count;
                }
                return friendFeedFilterCount.Value;
            }
        }

        private int? appFeedFilterCount = null;
        [TemplateVariable]
        public int AppFeedFilterCount
        {
            get
            {
                if (appFeedFilterCount == null)
                {
                    int count = 0;
                    foreach (FeedFilter feedFilter in UserFeedFilters)
                    {
                        if (feedFilter.FilterType == FilterType.FilterApp || feedFilter.FilterType == FilterType.FilterAppAction)
                            count++;
                    }
                    appFeedFilterCount = count;
                }
                return appFeedFilterCount.Value;
            }
        }

        /// <summary>
        /// 过滤设置 模板
        /// </summary>
        /// <param name="i">当前记录的序号，从0开始计数</param>
        /// <param name="feedFilter">过滤条件</param>
        public delegate void FeedFiltersTemplate(int i, FeedFilter feedFilter);


        /// <summary>
        /// 过滤设置 数据
        /// </summary>
        [TemplateTag]
        public void FeedFilterList(FilterType filterType, FeedFiltersTemplate template)
        {
            FeedFilterCollection tempFeedFilters = new FeedFilterCollection();
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (filterType == FilterType.FilterApp || filterType == FilterType.FilterAppAction)
                {
                    if (feedFilter.FilterType == FilterType.FilterApp || feedFilter.FilterType == FilterType.FilterAppAction)
                    {
                        tempFeedFilters.Add(feedFilter);
                    }
                }
                else if (feedFilter.FilterType == filterType)
                {
                    //if(filterType == FilterType.FilterUser)
                    tempFeedFilters.Add(feedFilter);
                }
            }

            int i = 0;

            foreach (FeedFilter feedFilter in tempFeedFilters)
            {
                template(i++, feedFilter);
            }
        }

        //[TemplateFunction]
        //public string GetNickName(int userID)
        //{
        //    return "admin"+userID;//TODO:获取昵称
        //}


        public delegate void UserIDsTemplate(int i, int userID);


        /// <summary>
        /// 被屏蔽指定应用指定动作的好友ID
        /// </summary>
        /// <param name="template"></param>
        [TemplateTag]
        public void FiltratedUserIDList(UserIDsTemplate template)
        {
            int i = 0;
            List<int> userIDs = new List<int>();
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.FilterType == FilterType.FilterUserAppAction)
                {
                    if (!userIDs.Contains(feedFilter.FriendUserID.Value))
                        userIDs.Add(feedFilter.FriendUserID.Value);
                }
            }

            foreach (int userID in userIDs)
            {
                template(i++, userID);
            }
        }


        // =========================================




    }
}