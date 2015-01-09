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
    public partial class setting_pointexchange : AdminPageBase //: SettingPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_PointExchange; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savepointExchangeProportions"))
            {
                SavepointExchangeProportions();
            }
            else if (_Request.IsClick("addexchangerule"))
            {
                AddExchangeRule();
            }
            else if (_Request.IsClick("savepointExchangeRules"))
            {
                SavePointExchangeRules();
            }
            else if (_Request.Get("action", Method.Get, string.Empty).ToLower() == "delete")
            {
                DeleteExchangeRule();
            }
            else if (_Request.IsClick("saveenable"))
            {
                Enable();
            }
        }


        private void Enable()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            bool enable = _Request.IsChecked("enable", Method.Post, false);

            PointSettings setting = SettingManager.CloneSetttings<PointSettings>(PointSettings);

            setting.EnablePointExchange = enable;

            try
            {
                SettingManager.SaveSettings(setting);
            }
            catch(Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void SavepointExchangeProportions()
        {
            UserPointCollection userPoints = AllSettings.Current.PointSettings.EnabledUserPoints;
            string[] errorNames = new string[userPoints.Count];
            int i = 0;
            foreach (UserPoint userPoint in userPoints)
            {
                errorNames[i] = userPoint.Type.ToString();
                i++;
            }
            MessageDisplay msgDisplay = CreateMessageDisplay(errorNames);

            PointExchangeProportionCollection pointExchangeProportions = new PointExchangeProportionCollection();

            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                string valueString = _Request.Get("pointExchangeProportion." + (int)userPoint.Type,Method.Post,string.Empty);
                int value;
                if (!int.TryParse(valueString, out value))
                {
                    msgDisplay.AddError(userPoint.Type.ToString(), 0, string.Format(Lang_Error.User_UserPointExchangeFormatError, userPoint.Name));
                }
                else if (value < 1)
                {
                    msgDisplay.AddError(userPoint.Type.ToString(), 0, string.Format(Lang_Error.User_UserPointExchangeFormatError, userPoint.Name));
                }
                else
                    pointExchangeProportions.Add(userPoint.Type, value);
                    
            }

            if (msgDisplay.HasAnyError())
                return;

            //PointExchangeProportionCollection tempPointExchangeProportions = AllSettings.Current.UserPointSettings.ExchangeProportions;
            PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);
            setting.ExchangeProportions = pointExchangeProportions;
            try
            {
                if (!SettingManager.SaveSettings(setting))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
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

        private void AddExchangeRule()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int? pointID = _Request.Get<int>("points", Method.Post);
            int? targetPointID = _Request.Get<int>("targetpoints", Method.Post);

            if (pointID == null)
            {
                msgDisplay.AddError(Lang_Error.User_UserPointNotSellectExchangePointError);
                return;
            }
            if (targetPointID == null)
            {
                msgDisplay.AddError(Lang_Error.User_UserPointNotSellectExchangeTargetPointError);
                return;
            }

            UserPointType pointType = (UserPointType)pointID;
            UserPointType targetPointType = (UserPointType)targetPointID;


            try
            {
                if (!PointActionManager.AddPointExchangeRule(pointType, targetPointType))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
            
        }
        private void SavePointExchangeRules()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("taxRate", "minRemaining");

            int[] ids = StringUtil.Split<int>(_Request.Get("ids",Method.Post,string.Empty));
            PointExchangeRuleCollection rules = AllSettings.Current.PointSettings.PointExchangeRules;

            int i = 0;
            PointExchangeRuleCollection tempRules = new PointExchangeRuleCollection();
            foreach(int id in ids)
            {
                string key = _Request.Get("key."+id,Method.Post,string.Empty);
                PointExchangeRule rule = (PointExchangeRule)rules.GetRule(key).Clone();
                if(rule == null)
                    continue;

                string valueString = _Request.Get("taxRate."+id,Method.Post,string.Empty);
                int value;

                if(!int.TryParse(valueString,out value))
                {
                    msgDisplay.AddError("taxRate",i,Lang_Error.User_UserPointExchangeTaxRateFormatError);
                }
                else
                    rule.TaxRate = value;

                valueString = _Request.Get("minRemaining."+id,Method.Post,string.Empty);
                if(!int.TryParse(valueString,out value))
                {
                    msgDisplay.AddError("minRemaining",i,Lang_Error.User_UserPointExchangeRemainingValueFormatError);
                }
                else if (value < rule.UserPoint.MinValue)
                {
                    msgDisplay.AddError("minRemaining", i, Lang_Error.User_UserPointInvalidTradeMaxValueError);
                    rule.MinRemainingValue = value;
                }
                else
                    rule.MinRemainingValue = value;

                tempRules.Add(rule);

                i++;
            }

            if(msgDisplay.HasAnyError())
            {
                return;
            }

            try
            {
                PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);
                setting.PointExchangeRules = tempRules;
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

        private void DeleteExchangeRule()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            string key = _Request.Get("key",Method.Get,string.Empty);
            if (key == string.Empty)
            {
                msgDisplay.AddError(string.Format(Lang_Error.Global_InvalidParamError,"key"));
                return;
            }
            PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);
            PointExchangeRule rule = setting.PointExchangeRules.GetRule(key);
            if (rule == null)
                return;

            setting.PointExchangeRules.Remove(rule);

            try
            {
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