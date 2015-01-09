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
    public partial class setting_feedjob : AdminPageBase //: SettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_FeedJob; }
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
            MessageDisplay msgDisplay = CreateMessageDisplay("executetime", "clearMode");

            FeedJobSettings setting = new FeedJobSettings();

            setting.Enable = _Request.Get<bool>("enableJob", Method.Post, false);

            string valueString = _Request.Get("executetime", Method.Post, string.Empty);
            int value;
            if (!int.TryParse(valueString, out value))
            {
                msgDisplay.AddError("executetime", Lang_Error.Feed_FeedJobExecuteTimeFormatError);
            }
            else if (value < 0 || value > 23)
            {
                msgDisplay.AddError("executetime", Lang_Error.Feed_FeedJobInvalidExecuteTimeError);
            }
            else
            {
                setting.ExecuteTime = value;
            }

            JobDataClearMode clearMode = _Request.Get<JobDataClearMode>("clearMode", Method.Post, JobDataClearMode.ClearByDay);

            if (clearMode == JobDataClearMode.ClearByDay)
            {
                valueString = _Request.Get("days1", Method.Post, string.Empty);
                if (!int.TryParse(valueString, out value))
                {
                    msgDisplay.AddError("clearMode", Lang_Error.Feed_FeedJobDayFormatError);
                }
                else if (value < 0)
                {
                    msgDisplay.AddError("clearMode", Lang_Error.Feed_FeedJobDayFormatError);
                }
                else
                {
                    setting.Day = value;
                }
            }
            else
            {
                string error = null;
                valueString = _Request.Get("count", Method.Post, string.Empty);
                if (!int.TryParse(valueString, out value))
                {
                    error = "保留条数必须为整数";
                }
                else if (value < 0)
                {
                    error = "保留条数不能小于0";
                }
                else
                {
                    setting.Count = value;
                }

                bool isCombinMode = _Request.Get<bool>("CombinMode", Method.Post, false);
                if (isCombinMode)
                {
                    valueString = _Request.Get("days2", Method.Post, string.Empty);
                    if (!int.TryParse(valueString, out value))
                    {
                        if (error != null)
                            error += "," + Lang_Error.Feed_FeedJobDayFormatError;
                        else
                            error += Lang_Error.Feed_FeedJobDayFormatError;
                    }
                    else if (value < 0)
                    {
                        if (error != null)
                            error += "," + Lang_Error.Feed_FeedJobDayFormatError;
                        else
                            error += Lang_Error.Feed_FeedJobDayFormatError;
                    }
                    else
                    {
                        setting.Day = value;
                    }
                }

                if (isCombinMode)
                    clearMode = JobDataClearMode.CombinMode;

                if (error != null)
                    msgDisplay.AddError("clearMode", error);
            }

            if (msgDisplay.HasAnyError())
                return;

            try
            {
                using (new ErrorScope())
                {
                    setting.ClearMode = clearMode;
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
                        //msgDisplay.ShowInfo(this);
                        _Request.Clear(Method.Post);
                    }

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        protected FeedJobSettings FeedJobSetting
        {
            get
            {
                return AllSettings.Current.FeedJobSettings;
            }
        }

        protected string ClearModeValue
        {
            get
            {
                if (FeedJobSetting.ClearMode == JobDataClearMode.ClearByDay)
                    return FeedJobSetting.ClearMode.ToString();
                else
                    return JobDataClearMode.ClearByRows.ToString();
            }
        }

        protected bool IsCombinMode
        {
            get
            {
                return FeedJobSetting.ClearMode == JobDataClearMode.CombinMode;
            }
        }
    }
}