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
using MaxLabs.bbsMax.Templates;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class login : DialogPageBase
    {
        protected override string PageName
        {
            get { return "login"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("login"))
            {
                UserLogin();
            }
        }


        protected UserLoginType LoginType
        {
            get { return AllSettings.Current.LoginSettings.LoginType; }
        }

        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        /// <summary>
        /// 不检查访问控制
        /// </summary>
        protected override bool NeedCheckAccess
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected override bool NeedLogin
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 不检查必填项
        /// </summary>
        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected string validateActionName
        {
            get
            {
                return "login";
            }
        }

        private void UserLogin()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("username", "password", "noActive", GetValidateCodeInputName(validateActionName));
            string email = _Request.Get("email", Method.Post, string.Empty, false);
            string username = _Request.Get("username", Method.Post, string.Empty, false);
            string password = _Request.Get("password", Method.Post, string.Empty, false);
            bool autoLogin = !string.IsNullOrEmpty(_Request.Get("autologin", Method.Post));
            bool invisible = _Request.Get("invisible", Method.Post) == "true";

            string account = username == string.Empty ? email : username;

            string ip = _Request.IpAddress;

            //如果全局UserLoginType为Username -或者- 全局UserLoginType为All且用户选择了账号登陆  则为true 
            bool IsUsernameLogin = (LoginType == UserLoginType.Username || (LoginType == UserLoginType.All && _Request.Get<int>("logintype", Method.Post,0) == 0));

            ValidateCodeManager.CreateValidateCodeActionRecode(validateActionName);
            if (!CheckValidateCode(validateActionName, msgDisplay))
            {
                return;
            }

            try
            {
                Success = UserBO.Instance.Login(account, password, ip, autoLogin, IsUsernameLogin);
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
                Success = false;
            }

            if (Success)
            {
#if !Passport
                UpdateOnlineStatus(OnlineAction.OtherAction, 0, "");
                OnlineUserPool.Instance.Update(My, invisible);
                ShowSuccess("登录成功", true);
#endif

            }
            else
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
        }


        protected bool Success
        {
            get;
            set;
        }

        private string m_LoginDescription;
        protected string LoginDescription
        {
            get
            {
                if (m_LoginDescription == null)
                {
                    LoginReferrer? loginRef = _Request.Get<LoginReferrer>("LoginReferrer", Method.Get);
                    if (loginRef == null)
                        m_LoginDescription = string.Empty;
                    else if (loginRef.Value == LoginReferrer.ViewAttachImage)
                        m_LoginDescription = @"
本帖含有精彩图片，您必须登录后才能查看
";
                }

                return m_LoginDescription;
            }
        }
    }
}