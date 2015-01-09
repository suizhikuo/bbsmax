//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.ExceptableSetting;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public partial class setting_prop : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Setting_Prop;
            }
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_Request.IsClick("savesetting"))
			{
				SaveSetting();
			}
		}

		private void SaveSetting()
		{
			MessageDisplay msgDisplay = CreateMessageDisplay();

			PropSettings settings = new PropSettings();

            settings.EnablePropFunction = _Request.Get<bool>("EnablePropFunction", Method.Post, true);
            settings.MaxPackageSize = new ExceptableItem_Int().GetExceptable("MaxPackageSize", msgDisplay);

            settings.SaveLogDays = _Request.Get<int>("SaveDays", Method.Post, 30);
            settings.DataClearMode = _Request.Get<JobDataClearMode>("DataClearMode", JobDataClearMode.Disabled);
            settings.SaveLogRows = _Request.Get<int>("SaveRows", Method.Post, 10000);

			if (!msgDisplay.HasAnyError())
			{
                SettingManager.SaveSettings(settings);

                string rawUrl = Request.RawUrl;

                BbsRouter.JumpToUrl(rawUrl, "success=1");
			}
		}
    }
}