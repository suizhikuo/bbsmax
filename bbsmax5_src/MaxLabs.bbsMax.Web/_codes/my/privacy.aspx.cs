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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class privacy : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "隐私设置 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "privacy"; }
        }
        
        protected override string NavigationKey
        {
            get { return "privacy"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("隐私设置");

            if (_Request.IsClick("button_savefeedprivacy"))
            {
                if (SaveFeedPrivacy())
                    SaveSpacePrivacy();
                return;
            }
            else if (_Request.IsClick("savespaceprivacy"))
            {
                if (SaveSpacePrivacy())
                    SaveFeedPrivacy();
                return;
            }
        }

        private bool SaveFeedPrivacy()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("feedprivacy");

            int userID = MyUserID;
            AppBaseCollection apps = AppManager.GetAllApps();

            //所有动作都加入动态的应用
            //string appIDString = _Request.Get("apps", Method.Post);

            //string[] appIDs;
            //if (!string.IsNullOrEmpty(appIDString))
            //{
            //    appIDs = appIDString.Split(',');
            //}
            //else
            //    appIDs = new string[] { };


            UserNoAddFeedAppCollection userNoAddFeedApps = new UserNoAddFeedAppCollection();

            foreach (AppBase app in apps)
            {
                //bool isAddAllAction = false;
                //foreach (string appID in appIDs)
                //{
                //    if (app.AppID.ToString() == appID)//该应用的所有动作都加入动态
                //    {
                //        isAddAllAction = true;
                //        break;
                //    }
                //}

                //if (!isAddAllAction)
                //{

                    //加入动态的动作
                    string actionIDString = _Request.Get("app_" + app.AppID.ToString() + "_Actions", Method.Post);

                    string[] actionIDs;
                    if (!string.IsNullOrEmpty(actionIDString))
                    {
                        actionIDs = actionIDString.Split(',');
                    }
                    else
                        actionIDs = new string[] { };

                    foreach (AppAction appAction in app.AppActions)
                    {
                        bool isAddFeed = false;
                        foreach (string actionType in actionIDs)
                        {
                            if (appAction.ActionType.ToString() == actionType)
                            {
                                isAddFeed = true;
                                break;
                            }
                        }

                        UserNoAddFeedApp userNoAddFeedApp = new UserNoAddFeedApp();
                        userNoAddFeedApp.AppID = app.AppID;
                        userNoAddFeedApp.ActionType = appAction.ActionType;
                        userNoAddFeedApp.UserID = MyUserID;
                        userNoAddFeedApp.Send = isAddFeed;
                        userNoAddFeedApps.Add(userNoAddFeedApp);
                    }

                //}
            }

            try
            {
                FeedBO.Instance.SetUserNoAddFeedApps(userID, userNoAddFeedApps);
                //msgDisplay.ShowInfo(this);
                //ShowSuccess();
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
                return false;
            }

            return true;
        }

        private bool SaveSpacePrivacy()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("spaceprivacy");

            SpacePrivacyType blogPrivacy = _Request.Get<SpacePrivacyType>("BlogPrivacy", SpacePrivacyType.All);
            SpacePrivacyType boardPrivacy = _Request.Get<SpacePrivacyType>("BoardPrivacy", SpacePrivacyType.All);
            SpacePrivacyType albumPrivacy = _Request.Get<SpacePrivacyType>("AlbumPrivacy", SpacePrivacyType.All);
            SpacePrivacyType doingPrivacy = _Request.Get<SpacePrivacyType>("DoingPrivacy", SpacePrivacyType.All);
            SpacePrivacyType feedPrivacy = _Request.Get<SpacePrivacyType>("FeedPrivacy", SpacePrivacyType.All);
            SpacePrivacyType friendListPrivacy = _Request.Get<SpacePrivacyType>("FriendListPrivacy", SpacePrivacyType.Self);
            SpacePrivacyType informationPrivacy = _Request.Get<SpacePrivacyType>("InformationPrivacy", SpacePrivacyType.All);
            SpacePrivacyType spacePrivacy = _Request.Get<SpacePrivacyType>("SpacePrivacy", SpacePrivacyType.All);
            SpacePrivacyType sharePrivacy = _Request.Get<SpacePrivacyType>("SharePrivacy", SpacePrivacyType.All);

            try
            {
                SpaceBO.Instance.ModifySpacePrivacy(MyUserID, blogPrivacy, feedPrivacy, boardPrivacy, doingPrivacy, albumPrivacy, spacePrivacy, sharePrivacy, friendListPrivacy, informationPrivacy);
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
                return false;
            }

            return true;
        }



        protected AppBaseCollection AppList
        {
            get
            {
                return AppManager.GetAllApps();
            }
        }

        protected AppActionCollection GetAppActionList(AppBase app)
        {
            AppActionCollection appActions = new AppActionCollection();

            foreach (AppAction appAction in app.AppActions)
            {
                if (FeedBO.Instance.IsSiteFeed(app.AppID, appAction.ActionType))
                    continue;

                bool display = true;
                foreach (FeedSendItem tempItem in AllSettings.Current.PrivacySettings.FeedSendItems)
                {
                    if ((tempItem.DefaultSendType == FeedSendItem.SendType.ForceSend || tempItem.DefaultSendType == FeedSendItem.SendType.ForceNotSend)
                        && tempItem.AppID == app.AppID && tempItem.ActionType == appAction.ActionType)
                    {
                        display = false;
                        break;
                    }
                }

                if (display)
                    appActions.Add(appAction);
            }

            return appActions;
        }

        private int? m_AppCount;
        protected int AppCount
        {
            get
            {
                if (m_AppCount == null)
                {
                    m_AppCount = AppList.Count;
                }
                return m_AppCount.Value;
            }
        }


        /// <summary>
        /// 指定类型的动作是否加入动态
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public bool IsAddFeedAction(Guid appID, int actionType)
        {
            return UserNoAddFeedApps.IsAddFeedAction(MyUserID, appID, actionType);
        }

        private UserNoAddFeedAppCollection m_UserNoAddFeedApps;
        private UserNoAddFeedAppCollection UserNoAddFeedApps
        {
            get
            {
                if (m_UserNoAddFeedApps == null)
                {
                    m_UserNoAddFeedApps = FeedBO.Instance.GetUserNoAddFeedApps(MyUserID);
                }
                return m_UserNoAddFeedApps;
            }
        }
    }
}