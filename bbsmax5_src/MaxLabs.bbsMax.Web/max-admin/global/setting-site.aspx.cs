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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_site : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Site; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<SiteSettings>("savesetting");
		}

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "ViewIPFields")
            {
                SiteSettings temp = (SiteSettings)setting;
                MessageDisplay msgDisplay = CreateMessageDisplay("ViewIPFields", "new_ViewIPFields");
                temp.ViewIPFields = new ExceptableSetting.ExceptableItem_Int().GetExceptable("ViewIPFields", msgDisplay);

                if (msgDisplay.HasAnyError())
                {
                    ThrowError<Errors.CustomError>(new Errors.CustomError("",""));
                    return false;
                }
                return true;
            }
            else if (property.Name == "ScopeList")
            {
                SiteSettings temp = (SiteSettings)setting;
                temp.ScopeList = SiteSettings.ScopeList;
                return true;
            }
               
            else
                return base.SetSettingItemValue(setting, property);
        }

        protected string CookieDomainString
        {
            get
            {
                return CookieUtil.CookieDomain;
            }
        }

        public ScopeBaseCollection ScopeItemList
        {
            get
            {
                return SiteSettings.ScopeList;
            }
        }
        public bool ListIsEmpty
        {
            get { return ScopeItemList.Count == 0; }
        }

       
    }
}