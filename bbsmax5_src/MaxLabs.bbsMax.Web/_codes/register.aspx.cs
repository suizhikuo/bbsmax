//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.ValidateCodes;
using System.Web;
using System.Collections;
using MaxLabs.bbsMax.Entities;
using System.Collections.Generic;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class register : BbsPageBase
    {
        protected override string PageTitle
        {
            get { return "注册新帐号 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "register"; }
        }

        protected override string NavigationKey
        {
            get { return "register"; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected string validateCodeAction
        {
            get { return "register"; }
        }

        protected bool IsActiveState
        {
            get
            {
                return !string.IsNullOrEmpty(activeCode);
            }
        }

        private  string activeCode="";

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("注册新帐号");

#if !Passport
            PassportClientConfig setting = Globals.PassportClient;
            if (setting.EnablePassport)
            {
                Response.Redirect(setting.RegisterUrl);
                return;
            }
#endif

            activeCode = _Request.Get("active", Method.Get);

            MessageDisplay msgDisplay = CreateMessageDisplay();
            ValidateEmailAction activingEmailType = _Request.Get<ValidateEmailAction>("type", Method.Get, ValidateEmailAction.None);
            int isSent = _Request.Get("issent", Method.Get, 0);
            if (activingEmailType != ValidateEmailAction.None)
            {
                if (activingEmailType == ValidateEmailAction.ActivingUser)
                {
                    IsActivingUser = true;
                }
                else
                {
                    IsValidateEmail = true;
                }
                m_NeedActive = true;
                int userID = _Request.Get<int>("userid", Method.Get, 0);
                NeedActiveUser = UserBO.Instance.GetUser(userID);
                if (NeedActiveUser == null)
                {
                    ShowError("该用户ID可能已经被删除,请重新注册！");
                }
                m_email = NeedActiveUser.Email;
                CheckEmailLoginLink(m_email);
            }


            //判断用户是否点击了“注册”按钮
            if (_Request.IsClick("register"))
            {
                Register();
            }
            //判断用户是否点击了"重新发送邮件"按钮
            else if (_Request.IsClick("sendmail"))
            {
                ResendEmail();
            }
            //判断是否“激活帐号”操作
            else if (!string.IsNullOrEmpty(activeCode))
            {
                if (_Request.IsClick("activeme"))  //这里加点击按钮判断是因为某些邮箱会抓取邮件里的链接，造成用户自动激活。
                {
                    using (ErrorScope es = new ErrorScope())
                    {
                        if (UserBO.Instance.ActivingUser(activeCode))
                        {
                            //用户激活成功
                            ShowSuccess("恭喜！您的账号" + My.Username + "已成功激活。", IndexUrl);
                        }
                        else
                        {
                            if (es.HasUnCatchedError)
                            {
                                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                {
                                    IsOverrunLimitTimes = true;
                                    msgDisplay.AddError(error);
                                });
                            }
                            else
                            {
                                ShowError(new InvalidActiveCodeError("active", activeCode));
                            }
                        }
                    }
                }
            }

        //判断是否“重发激活邮件”的操作
            else if (activingEmailType == ValidateEmailAction.ActivingUser && isSent != 1)
            {
                NeedSendMailClick = true;
            }
            //判断是否"重新发送验证邮箱的邮件"
            else if (activingEmailType == ValidateEmailAction.ValidateEmail && isSent != 1)
            {
                NeedSendMailClick = true;
            }
        }

        private void ResendEmail()
        {
            m_NeedActive = true;

            MessageDisplay msgDisplay = CreateMessageDisplay();

            int userID = _Request.Get<int>("userid", Method.All, 0);
            ValidateEmailAction emailType = (ValidateEmailAction)_Request.Get<ValidateEmailAction>("type", Method.Get, ValidateEmailAction.ActivingUser);
            string code = _Request.Get("code", Method.Get, string.Empty);

            using (ErrorScope es = new ErrorScope())
            {
                bool suceess;
                //= UserBO.Instance.SendEmail(userID, code, emailType);

                if (emailType == ValidateEmailAction.ActivingUser)
                    suceess = UserBO.Instance.TryActiveUser(userID, code);
                else
                    suceess = UserBO.Instance.TryValidateEmail(userID, code);

                if (suceess == false)
                {
                    if (es.HasUnCatchedError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            IsOverrunLimitTimes = true;
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        ShowError("无效的用户ID或验证码!");
                    }
                }
            }

            //m_NeedActive = true;
            //int userID = _Request.Get<int>("userid", Method.All, 0);
            //ValidateEmailAction emailType = (ValidateEmailAction)_Request.Get<ValidateEmailAction>("resend", Method.Get, ValidateEmailAction.ActivingUser);
            //string code = _Request.Get("code", Method.Get, string.Empty);
            //bool success = UserBO.Instance.SendEmail(userID, code, emailType);
            //return success;
        }

        private void Register()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("username", "password", "password2", "email", "serial", "inviterid", GetValidateCodeInputName(validateCodeAction));
            string password = _Request.Get("password", Method.Post, string.Empty, false);
            if (password != _Request.Get("password2", Method.Post, string.Empty, false))
            {
                _Request.Remove("password2", Method.Post);
                ThrowError(new PasswordInconsistentError("password2"));
            }

            string username = _Request.Get("username", Method.Post, string.Empty, false).Trim();
            string email = _Request.Get("email", Method.Post, string.Empty, false).Trim();
            string inviteCode = _Request.Get("invite", Method.All, string.Empty).Trim();
            int userid = 0;

            //if (string.IsNullOrEmpty(inviteCode)) inviteCode = _Request.Get("invite", Method.Get);

            int? inviterID = _Request.Get<int>("inviterid");

            if (_Request.Get("agree", Method.Post) != "1")
                ThrowError(new NotAgreeError("notagreeerror"));

            //验证码检查
            bool isRightValidateCode = CheckValidateCode(validateCodeAction, msgDisplay);
            //if (!CheckValidateCode(validateCodeAction, msgDisplay))
            //{
            //    return;
            //}

            UserRegisterState state = UserRegisterState.Failure;

            try
            {
                if (!HasUnCatchedError)
                    state = UserBO.Instance.Register(ref userid, username, password, email, _Request.IpAddress, inviteCode, null, isRightValidateCode);
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }

            switch (state)
            {
                case UserRegisterState.Success:
                    BbsRouter.JumpTo("default");
                    ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);
                    break;
                case UserRegisterState.NeedActive:
                    ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);
                    password = SecurityUtil.Encrypt(EncryptFormat.bbsMax, password);
                    //已经发送过邮件了，跳转后,不需要再次提示发送邮件.
                    string reactiveUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ActivingUser, userid, password, true);
                    Response.Redirect(reactiveUrl);
                    break;
                case UserRegisterState.Failure:
                    //失败
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                    break;
            }
        }

        private void CheckEmailLoginLink(string email)
        {
            email = email.Substring(email.IndexOf('@') + 1);

            EmailLinkInfo emailInfo;
            if (EmailLinkInfoList.EmailInfoList.TryGetValue(email, out emailInfo))
            {
                m_emailLoginLink = emailInfo.EmailLoginLink;
                m_emailName = emailInfo.EmailName;
            }
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


        private User m_NeedActiveUser;
        protected User NeedActiveUser
        {
            get { return m_NeedActiveUser; }
            set { m_NeedActiveUser = value; }
        }

        protected bool IAgree
        {
            get
            {
                return _Request.IsClick("iagree")
                    ||
                    _Request.IsClick("register")
                    ||
                    _Request.Get<int>("resend", Method.Get, 0) == 1;
            }
        }

        private bool m_NeedActive = false;
        protected bool NeedActive
        {
            get { return m_NeedActive; }
        }

        private string m_emailLoginLink;
        protected string EmailLoginLink
        {
            get { return m_emailLoginLink; }
        }

        private string m_emailName;
        protected string EmailName
        {
            get { return m_emailName; }
        }

        private string m_email;
        protected string Email
        {
            get { return m_email; }
        }

        protected bool NeedSendMailClick { get; set; }

        protected bool IsOverrunLimitTimes { get; set; }

        protected bool IsActivingUser { get; set; }

        protected bool IsValidateEmail { get; set; }
    }
}