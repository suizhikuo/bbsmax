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
using System.Text;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class validatecode_role : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_ValidateCode; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savevalidaterole"))
            {
                SaveValidateRole();
            }
        }
        private void SaveValidateRole()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string actiontype = _Request.Get("actiontype", Method.Get, string.Empty);


            string roleIDString = _Request.Get("validateCodeRole", Method.Post, string.Empty);

            if (roleIDString.Length == 0)
                return;


            bool isInvalidActionType = true;

            foreach (ValidateCodeAction action in ValidateCodeManager.GetAllValidateCodeActions())
            {
                if (string.Compare(action.Type, actiontype, true) == 0)
                {
                    if (action.CanSetExceptRoleId == false)
                    {
                        msgDisplay.AddError(string.Format(Lang_Error.ValidateCode_ValidateCodeActionCannotSetExceptRoleID, action.Name));
                        return;
                    }
                    isInvalidActionType = false;
                    break;
                }
            }

            if (isInvalidActionType == true)
            {
                msgDisplay.AddError(new InvalidParamError("actiontype").Message);
            }


            StringList exceptRoleIDs = new StringList();

            foreach (string roleID in roleIDString.Split(','))
            {
                exceptRoleIDs.Add(roleID);
            }


            ValidateCodeCollection tempValidateCodes = new ValidateCodeCollection();

            bool hasAdd = false;

            foreach (ValidateCode validateCode in AllSettings.Current.ValidateCodeSettings.ValidateCodes)
            {
                if (string.Compare(validateCode.ActionType, actiontype, true) == 0)
                {
                    ValidateCode tempValidateCode = (ValidateCode)validateCode.Clone();

                    tempValidateCode.ExceptRoleIds = exceptRoleIDs;

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

                validateCode.ExceptRoleIds = exceptRoleIDs;
                validateCode.ActionType = actiontype;

                tempValidateCodes.Add(validateCode);
            }


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
                        Return(true);
                    }

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }



        private string m_ExceptRoleIDs;

        protected string ExceptRoleIDs
        {
            get
            {
                if (m_ExceptRoleIDs == null)
                {
                    string actiontype = _Request.Get("actiontype", Method.Get, string.Empty);

                    ValidateCode validateCode = AllSettings.Current.ValidateCodeSettings.ValidateCodes.GetValue(actiontype);

                    if (validateCode == null)
                        return string.Empty;

                    StringBuilder exceptRoleIDs = new StringBuilder();

                    foreach (string roleIDString in validateCode.ExceptRoleIds)
                    {
                        exceptRoleIDs.Append(roleIDString).Append(",");
                    }

                    if (exceptRoleIDs.Length > 0)
                        m_ExceptRoleIDs = exceptRoleIDs.ToString(0, exceptRoleIDs.Length - 1);
                    else
                        m_ExceptRoleIDs = string.Empty;
                }

                return m_ExceptRoleIDs;
            }
        }


        protected RoleCollection AllRoleList
        {
            get
            {
                return AllSettings.Current.RoleSettings.Roles;
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