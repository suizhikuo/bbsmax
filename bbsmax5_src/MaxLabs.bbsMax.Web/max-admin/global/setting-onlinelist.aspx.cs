//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_onlinelist : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_OnlineList; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool oldIsShowAllSpidersInSameIP = OnlineSettings.IsShowAllSpidersInSameIP;

            int oldShowSameIPCount = OnlineSettings.ShowSameIPCount;

            SaveSetting<OnlineSettings>("savesetting");

            bool updateSpider = true;
            if (OnlineSettings.IsShowAllSpidersInSameIP || (oldIsShowAllSpidersInSameIP == OnlineSettings.IsShowAllSpidersInSameIP))
                updateSpider = false;

            bool updateNormal = true;
            if (OnlineSettings.ShowSameIPCount >= oldShowSameIPCount)
                updateNormal = false;

            OnlineUserPool.Instance.UpdateSameIpOnlineGuest(updateNormal, updateSpider);
        }

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "RolesInOnline")
            {
                OnlineSettings temp = (OnlineSettings)setting;
                temp.RolesInOnline = OnlineSettings.RolesInOnline;
                return true;
            }
            else if (property.Name == "DefaultOpen")
            {
                OnlineSettings temp = (OnlineSettings)setting;

                string name = null;
                string checkedValue = _Request.Get("OnlineMemberShow", Method.Post, string.Empty);
                if (checkedValue == "ShowAll")
                {
                    name = "DefaultOpen1";
                }
                else if (checkedValue == "ShowMember")
                {
                    name = "DefaultOpen2";
                }
                if (name != null)
                    temp.DefaultOpen = _Request.Get(name, Method.Post, "").ToLower() == "true";
                return true;
            }
            else
                return base.SetSettingItemValue(setting, property);
        }
    }
}