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

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class login : BbsPageBase
    {
        protected override string PageTitle
        {
            get { return "登录 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "login"; }
        }

        protected override string NavigationKey
        {
            get { return "login"; }
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

        /// <summary>
        /// 不检查必填项
        /// </summary>
        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("登录");

#if !Passport
            PassportClientConfig setting = Globals.PassportClient;
            if (setting.EnablePassport)
            {
                string fromUrl = ReturnUrl;
                if (!fromUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    fromUrl = UrlUtil.JoinUrl(Globals.FullAppRoot, fromUrl);
                }

                Response.Redirect(setting.LoginUrl + "?returnurl=" + HttpUtility.UrlEncode(fromUrl));
                return;
            }
#endif

            if (_Request.IsClick("login"))
            {
                UserLogin();
            }
        }

        protected string validateActionName
        {
            get { return "login"; }
        }

        protected UserLoginType LoginType
        {
            get { return AllSettings.Current.LoginSettings.LoginType; }
        }

        private string m_ReturnUrl = null;
        protected string ReturnUrl
        {
            get
            {
                if (m_ReturnUrl == null)
                {

                    string returnUrl = _Request.Get("returnurl", Method.All, null, false);

                    //如果returnurl参数不存在，则自动取刚才的地址
                    if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
                        returnUrl = Request.UrlReferrer.OriginalString;

                    //如果有提交ReturnUrl且是本主域内的安全地址，
                    //且经过检查是可以回跳的页面，
                    //则登陆成功后自动跳转到returnurl
                    if (UrlUtil.IsUrlInMainDomain(returnUrl) && UrlUtil.IsUrlCanReturn(returnUrl))
                    {
                        m_ReturnUrl = returnUrl;
                    }

                    //不是以上情况，跳转到首页 
                    else
                    {
                        m_ReturnUrl = IndexUrl;
                    }

                }
                return m_ReturnUrl;
            }
        }

        private void UserLogin()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("username", "password", "noActive", GetValidateCodeInputName(validateActionName));

            string username = _Request.Get("username", Method.Post, string.Empty, false);
            string password = _Request.Get("password", Method.Post, string.Empty, false);
            bool autoLogin = !string.IsNullOrEmpty(_Request.Get("autologin", Method.Post));
            bool invisible = _Request.Get("invisible", Method.Post) == "true";

            string ip = _Request.IpAddress;

            //如果全局UserLoginType为Username -或者- 后台设置全局UserLoginType为All且用户选择了账号登陆  则为true
            UserLoginType loginType=_Request.Get<UserLoginType>("logintype", Method.Post, UserLoginType.Username);
            bool isUsernameLogin = (LoginType == UserLoginType.Username || (LoginType == UserLoginType.All && loginType == UserLoginType.Username));

            ValidateCodeManager.CreateValidateCodeActionRecode(validateActionName);

            if (!CheckValidateCode(validateActionName, msgDisplay))
            {
                return;
            }

            using (ErrorScope es = new ErrorScope())
            {
                bool success;
                try
                {
                    success = UserBO.Instance.Login(username, password, ip, autoLogin, isUsernameLogin);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                    success = false;
                }

                if (success)
                {
#if !Passport
                    UpdateOnlineStatus(OnlineAction.OtherAction, 0, "");
                    OnlineUserPool.Instance.Update(My, invisible);
#endif
                    Response.Redirect(ReturnUrl, true);

                }
                else
                {
                    if (es.HasUnCatchedError)
                    {
                        es.CatchError<UserNotActivedError>(delegate(UserNotActivedError err)
                        {
                            Response.Redirect(err.ActiveUrl);
                        });
                        es.CatchError<EmailNotValidatedError>(delegate(EmailNotValidatedError err)
                        {
                            Response.Redirect(err.ValidateUrl);
                        });
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                }
            }
        }
    }
}