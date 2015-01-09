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
using System.Collections.Generic;

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class navigation_delete : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                if (IsTopLink)
                    return BackendPermissions.Action.Setting_TopLinks;
                else
                    return BackendPermissions.Action.Setting_Navigation;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
                Delete();
            }
        }

        private bool? m_IsTopLink;
        protected bool IsTopLink
        {
            get
            {
                if (m_IsTopLink == null)
                {
                    m_IsTopLink = _Request.Get("IsTopLink", Method.Get, "false").ToLower() == "true";
                }
                return m_IsTopLink.Value;
            }
        }

        protected void Delete()
        {
            int id = _Request.Get<int>("id", Method.Get, 0);

            if (id <= 0)
            {
                ShowError("参数id错误");
            }


            NavigationSettingsBase settings;
            if(IsTopLink)
                settings = SettingManager.CloneSetttings<TopLinkSettings2>(AllSettings.Current.TopLinkSettings2);
            else
                settings = SettingManager.CloneSetttings<NavigationSettings>(AllSettings.Current.NavigationSettings);

            settings.Navigations.RemoveByKey(id);

            try
            {
                SettingManager.SaveSettings(settings);
                if(IsTopLink)
                    AllSettings.Current.TopLinkSettings2.ClearCache();
                else
                    AllSettings.Current.NavigationSettings.ClearCache();
                Return(id);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}