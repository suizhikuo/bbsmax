//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_pointtransfer : AdminPageBase //: SettingPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_PointTransfer; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savepointTransferRules"))
            {
                SavepointTransferRules();
            }
        }
        private void SavepointTransferRules()
        {
            string[] names = new string[] { "taxRate", "minRemaining" };
            MessageDisplay msgDisplay = CreateMessageDisplay(names);

            PointTransferRuleCollection rules = new PointTransferRuleCollection();
            int i = 0;
            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                int pointID = (int)userPoint.Type;
                bool canTransfer = _Request.Get<bool>("canTransfer." + pointID, Method.Post, false);
                string valueString = _Request.Get("taxRate." + pointID, Method.Post, string.Empty);
                int taxRate = 0;
                if (!int.TryParse(valueString, out taxRate))
                {
                    msgDisplay.AddError("taxRate", i, Lang_Error.User_UserPointTransferTaxRateFormatError);
                }
                else if (taxRate < 0)
                {
                    msgDisplay.AddError("taxRate", i, Lang_Error.User_UserPointTransferTaxRateFormatError);
                }

                valueString = _Request.Get("minRemaining." + pointID, Method.Post, string.Empty);
                int minRemainingValue = 0;
                if (!int.TryParse(valueString, out minRemainingValue))
                {
                    msgDisplay.AddError("minRemaining", i, Lang_Error.User_UserPointTransferMinRemainingValueFormatError);
                }

                PointTransferRule rule = new PointTransferRule();
                rule.CanTransfer = canTransfer;
                rule.MinRemainingValue = minRemainingValue;
                rule.PointType = userPoint.Type;
                rule.TaxRate = taxRate;

                rules.Add(rule);
            }

            if (msgDisplay.HasAnyError())
                return;

            PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);

            try
            {
                setting.PointTransferRules = rules;
                setting.EnablePointTransfer = _Request.IsChecked("enable", Method.Post, false);
                if (!SettingManager.SaveSettings(setting))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error.TatgetName, error.TargetLine, error.Message);
                    });
                }
                else
                    AllSettings.Current.PointSettings = setting;
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }
    }
}