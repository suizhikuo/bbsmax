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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.ValidateCodes;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class validatecode_style : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_ValidateCode; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savevalidatestyle"))
            {
                SaveValidateStyle();
            }
        }

        private bool _success;

        protected bool Success
        {
            get
            {
                return _success;
            }
            set { _success = value; }
        }

        private void SaveValidateStyle()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string actiontype = _Request.Get("actiontype", Method.Get, string.Empty);

            string validateCodeStyle = _Request.Get("validateCodeStyle",Method.Post,string.Empty);

            bool isInvalidActionType = true;

            foreach (ValidateCodeAction action in ValidateCodeManager.GetAllValidateCodeActions())
            {
                if (string.Compare(action.Type, actiontype, true) == 0)
                {
                    isInvalidActionType = false;
                    break;
                }
            }

            if (isInvalidActionType == true)
            {
                msgDisplay.AddError(new InvalidParamError("actiontype").Message);
            }


            ValidateCodeCollection tempValidateCodes = new ValidateCodeCollection();

            bool hasAdd = false;

            foreach (ValidateCode validateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(validateCode.ActionType, actiontype, true) == 0)
                {
                    ValidateCode tempValidateCode = (ValidateCode)validateCode.Clone();

                    tempValidateCode.ValidateCodeType = validateCodeStyle;

                    tempValidateCodes.Add(tempValidateCode);

                    hasAdd = true;
                }
                else
                {
                    tempValidateCodes.Add(validateCode);
                }
            }

            if (hasAdd == false)
            {
                ValidateCode validateCode = new ValidateCode();

                validateCode.ValidateCodeType = validateCodeStyle;
                validateCode.ActionType = actiontype;

                tempValidateCodes.Add(validateCode);
            }


            try
            {
                using (new ErrorScope())
                {

                    ValidateCodeSettings setting = (ValidateCodeSettings)AllSettings.Current.ValidateCodeSettings.Clone();

                    setting.ValidateCodes = tempValidateCodes;

                    Success = SettingManager.SaveSettings(setting);

                    if (!Success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        Return(true);
                    }

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }


        protected List<ValidateCodeType> ValidateCodeTypeList
        {
            get
            {
                return ValidateCodeManager.GetAllValidateCodeTypes();
            }
        }

        protected string GetValidateCodeImageUrl(string type)
        {
            return ValidateCodeManager.GetValidateCodeImageUrl(type, true, null);
        }

        private string m_CurrentValidateCodeStyle;

        protected string CurrentValidateCodeStyle
        {
            get
            {
                if (m_CurrentValidateCodeStyle == null)
                {
                    if (CurrentValidateCodeType != null)
                        m_CurrentValidateCodeStyle = CurrentValidateCodeType.Type;
                    else
                        m_CurrentValidateCodeStyle = string.Empty;
                }

                return m_CurrentValidateCodeStyle;
            }
        }

        private ValidateCodeType m_CurrentValidateCodeType;
        protected ValidateCodeType CurrentValidateCodeType
        {
            get
            {
                if (m_CurrentValidateCodeStyle == null)
                {
                    string actiontype = _Request.Get("actiontype", Method.Get, string.Empty);
                    m_CurrentValidateCodeType = ValidateCodeManager.GetValidateCodeTypeByAction(actiontype);
                }
                return m_CurrentValidateCodeType;
            }
        }

        private string m_CurrentActionName;
        protected string CurrentActionName
        {
            get
            {
                if (m_CurrentActionName == null)
                {
                    string actiontype = _Request.Get("actiontype", Method.Get, string.Empty);
                    foreach (ValidateCodeAction action in ValidateCodeManager.GetAllValidateCodeActions())
                    {
                        if (string.Compare(action.Type, actiontype, true) == 0)
                        {
                            m_CurrentActionName = action.Name;
                            break;
                        }
                    }
                    if (m_CurrentActionName == null)
                        m_CurrentActionName = string.Empty;
                }
                return m_CurrentActionName;
            }
        }
    }
}