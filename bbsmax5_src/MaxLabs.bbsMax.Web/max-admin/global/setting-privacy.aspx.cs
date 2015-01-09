//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

using System.Reflection;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_privacy : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Privacy; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<PrivacySettings>("savesetting");
        }

        private FeedSendItemCollection GetFeedSendItems()
        {
            FeedSendItemCollection items = new FeedSendItemCollection();

            foreach (AppBase app in AppList)
            {
                foreach (AppAction action in app.AppActions)
                {
                    if (FeedBO.Instance.IsSiteFeed(app.AppID, action.ActionType))
                        continue;

                    string formName = "app_" + app.AppID.ToString() + "_" + action.ActionType;

                    FeedSendItem item = new FeedSendItem();
                    item.AppID = app.AppID;
                    item.ActionType = action.ActionType;
                    item.DefaultSendType = StringUtil.TryParse<FeedSendItem.SendType>(_Request.Get(formName, Method.Post, string.Empty), FeedSendItem.SendType.Send);

                    items.Add(item);
                }
            }

            return items;
        }

        protected override bool SetSettingItemValue(SettingBase setting, PropertyInfo property)
        {
            if (property.Name == "FeedSendItems")
            {
                PrivacySettings temp = (PrivacySettings)setting;
                temp.FeedSendItems = GetFeedSendItems();
                return true;
            }
            else
                return base.SetSettingItemValue(setting, property);
        }

        protected AppBaseCollection AppList
        {
            get
            {
                return AppManager.GetAllApps();
            }
        }

        protected bool IsSiteFeed(Guid appID,int actionType)
        {
            return FeedBO.Instance.IsSiteFeed(appID, actionType);
        }

        protected string GetSendType(Guid appID,int actionType,string defaultValue)
        {
            foreach (FeedSendItem item in AllSettings.Current.PrivacySettings.FeedSendItems)
            {
                if (item.AppID == appID && item.ActionType == actionType)
                    return item.DefaultSendType.ToString().ToLower();
            }
            return defaultValue;
        }

    }

    
}