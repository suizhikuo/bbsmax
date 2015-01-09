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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public partial class setting_pointlogclear : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Manage_PointLog;
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
            MessageDisplay msgDisplay = CreateMessageDisplay("executetime", "days");

            PointLogClearSettings setting = new PointLogClearSettings();

            setting.SaveLogDays = _Request.Get<int>("SaveDays", Method.Post, 30);
            setting.DataClearMode = _Request.Get<JobDataClearMode>("DataClearMode", JobDataClearMode.Disabled);
            setting.SaveLogRows = _Request.Get<int>("SaveRows", Method.Post, 10000);

            if (msgDisplay.HasAnyError())
                return;

            try
            {
                using (new ErrorScope())
                {

                    bool success = SettingManager.SaveSettings(setting);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        string rawUrl = Request.RawUrl;

                        BbsRouter.JumpToUrl(rawUrl, "success=1");
                    }

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        protected PointLogClearSettings Settings
        {
            get
            {
                return AllSettings.Current.PointLogClearSettings;
            }
        }
    }
}