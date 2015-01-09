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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_account : UserDialogPageBase
    {
        private SimpleUser m_User;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserBO.Instance.CanEditUserProfile(My, UserID))
            {
                ShowError("没有权限修改用户资料");
                return;
            }

            m_User = UserBO.Instance.GetSimpleUser(UserID);

            if (m_User == null)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            if (_Request.IsClick("updatePassword"))
            {
                UpdateUserPassword();
            }
            else if(_Request.IsClick("updateUsername"))
            {
                UpdateUsername();
            }
        }

        protected new SimpleUser User
        {
            get { return m_User; }
        }

        private void UpdateUsername()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("username",new string[] { "username" }); 

            string newUsername;

            newUsername = _Request.Get("username", Method.All, string.Empty, false);

            UserBO.Instance.AdminTryUpdateUsername(My, UserID, newUsername);

            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
        }

        private void UpdateUserPassword()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("password", new string[] { "password", "newPassword2" });

            string newPassword = _Request.Get("newPassword");
            string newPassword2 = _Request.Get("newPassword2");

            try
            {
                if (newPassword != newPassword2)
                {
                    ThrowError(new PasswordInconsistentError("newpassword2"));
                }
                else
                {
                    //if (UserPO.GetInstance(MyUserID).CanUpdateUserAccount(null))
                    UserBO.Instance.ResetUserPassword(My, UserID, newPassword);
                    //else
                    //{
                    //}
                }

                if (HasUnCatchedError)
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
    }
}