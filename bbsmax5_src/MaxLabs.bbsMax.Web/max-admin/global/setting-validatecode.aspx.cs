//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_validatecode : AdminPageBase //: SettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_ValidateCode; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savevalidatecode"))
            {
                SaveValidateCode();
            }
        }

        private void SaveValidateCode()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("Limited");

            ValidateCodeCollection tempValidateCodes = new ValidateCodeCollection();

            int i = 0;

            foreach (ValidateCodeAction action in ValidateCodeActionList)
            {
                ValidateCode validateCode = new ValidateCode();
                validateCode.Enable = _Request.Get<bool>("enable." + action.Type, Method.Post, false);
                validateCode.ActionType = action.Type;

                string valueString = _Request.Get("LimitedTime."+action.Type,Method.Post,string.Empty);

                int limitedTime = 0;

                string error = string.Empty;

                if (valueString.Trim() != string.Empty)
                {
                    if (!int.TryParse(valueString, out limitedTime))
                    {
                        error = Lang_Error.ValidateCode_LimitedTimeError;
                    }
                    else if (limitedTime < 0)
                        error = Lang_Error.ValidateCode_LimitedTimeError;
                }

                int timeType = _Request.Get<int>("limitedTimeType." + action.Type, Method.Post, 0);

                TimeUnit timeUnit = (TimeUnit)timeType;

                validateCode.LimitedTime = (int)DateTimeUtil.GetSeconds(limitedTime,timeUnit);


                valueString = _Request.Get("LimitedCount." + action.Type, Method.Post, string.Empty);

                int limitedCount = 0;

                if (valueString.Trim() != string.Empty)
                {
                    if (!int.TryParse(valueString, out limitedCount))
                    {
                        error = error + Lang_Error.ValidateCode_LimitedCountError;
                    }
                    else if (limitedCount < 0)
                        error = error + Lang_Error.ValidateCode_LimitedCountError;
                }
                if (error != string.Empty)
                {
                    msgDisplay.AddError("limited", i, error);
                    i++;
                    continue;
                }

                i++;

                validateCode.LimitedCount = limitedCount;


                ValidateCode tempValidateCode = AllSettings.Current.ValidateCodeSettings.ValidateCodes.GetValue(action.Type);

                if (tempValidateCode != null)
                {
                    validateCode.ExceptRoleIds = tempValidateCode.ExceptRoleIds;
                    validateCode.ValidateCodeType = tempValidateCode.ValidateCodeType;
                }

                tempValidateCodes.Add(validateCode);
            }


            if (msgDisplay.HasAnyError())
                return;

            try
            {
                using (new ErrorScope())
                {

                    ValidateCodeSettings setting = (ValidateCodeSettings)AllSettings.Current.ValidateCodeSettings.Clone();

                    setting.ValidateCodes = tempValidateCodes;

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
                    }

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        protected ValidateCodeActionCollection ValidateCodeActionList
        {
            get
            {
                return ValidateCodeManager.GetAllValidateCodeActions();
            }
        }

        //protected bool IsEnableValidateCode(string actionType)
        //{
        //    ValidateCodeType type = ValidateCodeManager.GetValidateCodeTypeByAction(actionType);

        //    return type != null;
        //}

        protected bool IsEnable(string actionType)
        {
            ValidateCode validateCode = AllSettings.Current.ValidateCodeSettings.ValidateCodes.GetValue(actionType);

            if (validateCode == null)
                return false;

            return validateCode.Enable;
        }

        protected string GetImageUrl(string actionType)
        {
            return ValidateCodeManager.GetValidateCodeImageUrl(actionType, false, null);
        }



        protected string GetLimitedTime(string actionType)
        {
            foreach (ValidateCode validateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(validateCode.ActionType, actionType, true) == 0)
                {
                    if (validateCode.LimitedTime > 0)
                    {
                        TimeUnit unit;
                        return DateTimeUtil.FormatSecond(validateCode.LimitedTime, out unit).ToString();
                    }
                    break;
                }
            }
            return string.Empty;
        }

        protected string GetLimitedTimeUnit(string actionType)
        {
            foreach (ValidateCode validateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(validateCode.ActionType, actionType, true) == 0)
                {
                    if (validateCode.LimitedTime > 0)
                    {
                        TimeUnit unit;
                        DateTimeUtil.FormatSecond(validateCode.LimitedTime, out unit);

                        return ((int)unit).ToString();
                    }
                    break;
                }
            }
            return string.Empty;
        }

        protected string GetLimitedCount(string actionType)
        {
            foreach (ValidateCode validateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(validateCode.ActionType, actionType, true) == 0)
                {
                    if (validateCode.LimitedCount > 0)
                    {
                        return validateCode.LimitedCount.ToString();
                    }
                    break;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 例外的用户组名
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        protected string GetRoleNames(string actionType)
        {
            ValidateCode validateCode = AllSettings.Current.ValidateCodeSettings.ValidateCodes.GetValue(actionType);

            if (validateCode == null)
                return string.Empty;

            StringBuilder roleNames = new StringBuilder();
            foreach(string roleIDString in validateCode.ExceptRoleIds)
            {
                Guid roleID;
                
                if (StringUtil.TryParse<Guid>(roleIDString, out roleID))
                {
                    Role role = AllSettings.Current.RoleSettings.Roles.GetValue(roleID);

                    if (role != null)
                    {
                        roleNames.Append(role.Name).Append(",");
                    }
                }
            }

            if (roleNames.Length > 0)
                return roleNames.ToString(0, roleNames.Length - 1);
            else
                return string.Empty;
        }
    }
}