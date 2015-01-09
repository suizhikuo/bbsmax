//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class feedfilter : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "好友动态过滤 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "feedfilter"; }
        }

        protected override string NavigationKey
        {
            get { return "feedfilter"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("好友动态过滤");
            
            if (_Request.IsClick("savefriendgroups"))
            {
                SaveFriendGroups();
                return;
            }
            else if (_Request.IsClick("savefriendfeedfilter"))
            {
                SaveFriendFeedFilter();
                return;
            }
            else if (_Request.IsClick("saveappfeedfilters"))
            {
                SaveAppFeedFilters();
                return;
            }
        }

        protected FriendGroupCollection FriendGroupList
        {
            get
            {
                return My.FriendGroups;
            }
        }

        private void SaveFriendGroups()
        {
            string friendGroupIDString = _Request.Get("friendGroups", Method.Post);

            MessageDisplay msgDisplay = CreateMessageDisplay();

            if (!string.IsNullOrEmpty(friendGroupIDString))
            {
                string[] ids = friendGroupIDString.Split(',');
                List<int> friendGroupIDs = new List<int>();

                foreach (FriendGroup friendGroup in FriendBO.Instance.GetFriendGroups(MyUserID))
                {
                    bool isShield = true;
                    foreach (string id in ids)
                    {
                        if (id == friendGroup.GroupID.ToString())
                        {
                            isShield = false;
                            break;
                        }
                    }
                    if (isShield)
                        friendGroupIDs.Add(friendGroup.GroupID);
                }

                try
                {
                    FriendBO.Instance.ShieldFriendGroups(MyUserID, friendGroupIDs, true);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
            }
            else
                FriendBO.Instance.ShieldFriendGroups(MyUserID, new List<int>(), false);
        }

        private void SaveFriendFeedFilter()
        {
            int userID = UserBO.Instance.GetCurrentUserID();

            string feedFilterIDString = _Request.Get("friendFeedFilterIDs", Method.Post);

            MessageDisplay msgDisplay = CreateMessageDisplay();

            if (!string.IsNullOrEmpty(feedFilterIDString))
            {
                string[] ids = feedFilterIDString.Split(',');
                List<int> feedFilterIDs = new List<int>();
                foreach (string id in ids)
                {
                    feedFilterIDs.Add(int.Parse(id));
                }

                if (feedFilterIDs.Count > 0)
                {
                    try
                    {
                        FeedBO.Instance.DeleteFeedFilters(userID, feedFilterIDs);
                    }
                    catch(Exception ex)
                    {
                        msgDisplay.AddException(ex);
                    }
                }
            }
        }

        private void SaveAppFeedFilters()
        {
            int userID = UserBO.Instance.GetCurrentUserID();

            string feedFilterIDString = _Request.Get("appFeedFilterIDs", Method.Post);

            MessageDisplay msgDisplay = CreateMessageDisplay();

            if (!string.IsNullOrEmpty(feedFilterIDString))
            {
                string[] ids = feedFilterIDString.Split(',');
                List<int> feedFilterIDs = new List<int>();
                foreach (string id in ids)
                {
                    feedFilterIDs.Add(int.Parse(id));
                }

                if (feedFilterIDs.Count > 0)
                {
                    try
                    {
                        FeedBO.Instance.DeleteFeedFilters(userID, feedFilterIDs);
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddException(ex);
                    }
                }
            }
        }






        private FeedFilterCollection m_FeedFilterUserList;
        protected FeedFilterCollection FeedFilterUserList
        {
            get
            {
                if (m_FeedFilterUserList == null)
                {
                    m_FeedFilterUserList = new FeedFilterCollection();
                    foreach (FeedFilter feedFilter in UserFeedFilters)
                    {
                        if (feedFilter.FilterType == FilterType.FilterUser)
                        {
                            m_FeedFilterUserList.Add(feedFilter);
                        }
                    }
                }

                return m_FeedFilterUserList;
            }
        }

        private FeedFilterCollection m_FiltratedAppFriendList;
        /// <summary>
        /// 被屏蔽指定应用指定动作的好友
        /// </summary>
        protected FeedFilterCollection FiltratedAppFriendList
        {
            get
            {
                if (m_FiltratedAppFriendList == null)
                {
                    m_FiltratedAppFriendList = new FeedFilterCollection();
                    List<int> userIDs = new List<int>();
                    foreach (FeedFilter feedFilter in UserFeedFilters)
                    {
                        if (feedFilter.FilterType == FilterType.FilterUserAppAction)
                        {
                            if (!userIDs.Contains(feedFilter.FriendUserID.Value))
                            {
                                userIDs.Add(feedFilter.FriendUserID.Value);
                                m_FiltratedAppFriendList.Add(feedFilter);
                            }
                        }
                    }
                }
                return m_FiltratedAppFriendList;
            }
        }

        protected FeedFilterCollection GetActionFeedFilterList(int userID)
        {
            FeedFilterCollection tempFilter = new FeedFilterCollection();
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.FilterType == FilterType.FilterUserAppAction && feedFilter.FriendUserID.Value == userID)
                {
                    tempFilter.Add(feedFilter);
                }
            }
            return tempFilter;
        }


        protected bool HasFriendFeedFilter
        {
            get
            {
                return (FeedFilterUserList.Count + FiltratedAppFriendList.Count) > 0;
            }
        }


        /// <summary>
        /// 获取指定应用的指定动作名称
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public string GetActionName(Guid appID, int actionType)
        {
            AppBase app = AppManager.GetApp(appID);
            if (app == null)
                return "已被删除的动作";
            AppAction appAction = app.AppActions.GetValue(actionType);
            if (appAction != null)
                return appAction.ActionName;
            else
                return "已被删除的动作";
        }

        private FeedFilterCollection m_UserFeedFilters;
        private FeedFilterCollection UserFeedFilters
        {
            get
            {
                if (m_UserFeedFilters == null)
                {
                    m_UserFeedFilters = FeedBO.Instance.GetFeedFilters(MyUserID);
                }
                return m_UserFeedFilters;
            }
        }









        protected bool HasAppFeedFilter
        {
            get
            {
                return AppFeedFilterCount > 0; 
            }
        }

        private int? appFeedFilterCount = null;
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


        private AppBaseCollection m_FiltratedAppList;
        protected AppBaseCollection FiltratedAppList
        {
            get
            {
                if (m_FiltratedAppList == null)
                {
                    List<Guid> appIDs = new List<Guid>();
                    foreach (FeedFilter feedFilter in UserFeedFilters)
                    {
                        if (feedFilter.AppID != Guid.Empty && feedFilter.FriendUserID == null && !appIDs.Contains(feedFilter.AppID))
                        {
                            appIDs.Add(feedFilter.AppID);
                        }
                    }
                    m_FiltratedAppList = AppManager.GetApps(appIDs);
                }

                return m_FiltratedAppList;
            }
        }

        private int? m_AppCount;
        /// <summary>
        /// 系统应用总数
        /// </summary>
        public int AppCount
        {
            get
            {
                if (m_AppCount == null)
                {
                    m_AppCount = AppManager.GetAllApps().Count;
                }
                return m_AppCount.Value;
            }
        }

        protected AppActionCollection GetFiltratedAppActionList(Guid appID)
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

            return appActions;
        }


        public int GetFeedFilterID(Guid appID, int actionType)
        {
            foreach (FeedFilter feedFilter in UserFeedFilters)
            {
                if (feedFilter.AppID == appID && feedFilter.ActionType == actionType && feedFilter.FriendUserID == null)
                    return feedFilter.ID;
            }
            return 0;
        }
    }
}