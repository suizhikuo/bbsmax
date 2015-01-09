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
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using System.Text;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_deleteoperationlogjob : AdminPageBase //: SettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_DeleteOperationLogJob; }
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

			DeleteOperationLogJobSettings setting = new DeleteOperationLogJobSettings();

            string valueString = _Request.Get("executetime", Method.Post, string.Empty);
            int value;
            if (!int.TryParse(valueString, out value))
            {
                msgDisplay.AddError("executetime",Lang_Error.Feed_FeedJobExecuteTimeFormatError);
            }
            else if (value < 0 || value > 23)
            {
                msgDisplay.AddError("executetime", Lang_Error.Feed_FeedJobInvalidExecuteTimeError);
            }
            else
            {
                setting.ExecuteTime = value;
            }

            
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

		protected DeleteOperationLogJobSettings DeleteOperationLogJobSettings
        {
            get
            {
				return AllSettings.Current.DeleteOperationLogJobSettings;
            }
        }
    }
}