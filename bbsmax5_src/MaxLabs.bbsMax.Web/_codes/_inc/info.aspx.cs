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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_pages
{
	public partial class info : BbsPageBase
	{
        protected override string PageName
        {
            get { return "info"; }
        }

        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

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
            get { return false; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        private string m_Mode = null;
        private string m_Message = null;
        private bool? m_HasWarning = null;
        private bool? m_TipLogin = null;

        private bool? m_AutoJump = null;
        private JumpLinkCollection m_JumpLinkList = null;
        private int? m_AutoJumpSeconds = null;
        private string m_AutoJumpUrl = null;

        /// <summary>
        /// 提示的模式
        /// </summary>
        public string Mode
        {
            get
            {
                if (m_Mode == null)
                    m_Mode = (string)Parameters["mode"];

                return m_Mode;
            }
        }

        public string Message
        {
            get
            {
                if (m_Message == null)
                    m_Message = (string)Parameters["message"];

                return m_Message;
            }
        }

        public JumpLinkCollection JumpLinkList
        {
            get
            {
                if (m_JumpLinkList == null)
                    m_JumpLinkList = Parameters == null ? null : Parameters["returnUrls"] as JumpLinkCollection;

                return m_JumpLinkList;
            }
        }

        /// <summary>
        /// 是否成功提示
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                if (string.Compare(Mode, "success", true) == 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 是否错误提示
        /// </summary>
        public bool IsError
        {
            get
            {
                if (string.Compare(Mode, "error", true) == 0)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 是否警告提示
        /// </summary>
        public bool HasWarning
        {
            get
            {
                if (m_HasWarning == null)
                    m_HasWarning = (bool)Parameters["warning"];

                return m_HasWarning.Value;
            }
        }


        /// <summary>
        /// 提示需要登陆
        /// </summary>
        public bool TipLogin
        {
            get
            {
                if (m_TipLogin == null)
                    m_TipLogin = (bool)Parameters["tipLogin"];

                return m_TipLogin.Value;
            }
        }

        #region 自动调转

        public bool AutoJump
        {
            get
            {
                if (m_AutoJump == null)
                {
                    if (JumpLinkList != null && JumpLinkList.Count > 0)
                    {
                        if (IsSuccess && HasWarning == false && AutoJumpSeconds > 0)
                            m_AutoJump = true;
                        else
                            m_AutoJump = false;

                    }
                    else
                        m_AutoJump = false;
                }

                return m_AutoJump.Value;
            }
        }

        public int AutoJumpSeconds
        {
            get
            {
                if (m_AutoJumpSeconds == null)
                {
                    m_AutoJumpSeconds = (int)Parameters["autoJumpSeconds"];
                }
                return m_AutoJumpSeconds.Value;
            }
        }

        public string AutoJumpUrl
        {
            get
            {
                if (m_AutoJumpUrl == null)
                {
                    if (JumpLinkList.Count > 0)
                    {
                        foreach (JumpLink link in JumpLinkList)
                        {
                            m_AutoJumpUrl = link.Link;
                            break;
                        }
                    }
                }
                return m_AutoJumpUrl;
            }
        }

        public string AutoJumpHtml
        {
            get
            {
                if (AutoJump)
                    return string.Concat("<meta http-equiv=\"refresh\" content=\"", AutoJumpSeconds.ToString(), ";url=", AutoJumpUrl, "\" />");
                else
                    return string.Empty;
            }
        }

        #endregion

        private string m_RoleDescription;
        protected string RoleDescription
        {
            get
            {
                if (m_RoleDescription == null)
                {
                    m_RoleDescription = string.Empty;
                    foreach (UserRole role in My.Roles)
                    {
                        if (role.RoleID == Role.NewUsers.RoleID)
                        {
                            if (role.EndDate.Year >= DateTime.MaxValue.Year)
                                m_RoleDescription = "您是见习用户，可能没有权限进行某些操作";
                            else
                            {
                                TimeSpan timeSpan = role.EndDate - DateTimeUtil.Now;
                                if (timeSpan.TotalSeconds > 0)
                                {
                                    string time = "";
                                    if (timeSpan.Days > 0)
                                        time += timeSpan.Days + "天";
                                    if (timeSpan.Hours > 0)
                                        time += timeSpan.Hours + "小时";
                                    if (timeSpan.Minutes > 0)
                                        time += timeSpan.Minutes + "分";
                                    if (timeSpan.Seconds > 0)
                                        time += timeSpan.Seconds + "秒";


                                    m_RoleDescription = "您是见习用户，可能没有权限进行某些操作，离成为正式用户还有" + time;
                                }
                            }
                            break;
                        }
                    }
                }

                return m_RoleDescription;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("login"))
            {
                UserLogin();
            }
        }

        #region  独立登录代码

        private void UserLogin()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("username", "password", "noActive", GetValidateCodeInputName(validateActionName));

            string username = _Request.Get("username", Method.Post, string.Empty, false);
            string password = _Request.Get("password", Method.Post, string.Empty, false);
            bool autoLogin = !string.IsNullOrEmpty(_Request.Get("autologin", Method.Post));
            bool invisible = _Request.Get("invisible", Method.Post) == "true";

            string ip = _Request.IpAddress;

            //如果全局UserLoginType为Username -或者- 后台设置全局UserLoginType为All且用户选择了账号登陆  则为true
            UserLoginType loginType = _Request.Get<UserLoginType>("logintype", Method.Post, UserLoginType.Username);
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
                    Response.Redirect(AutoJumpUrl, true);

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

        protected string validateActionName
        {
            get { return "login"; }
        }

        protected UserLoginType LoginType
        {
            get { return AllSettings.Current.LoginSettings.LoginType; }
        }

        #endregion
    }
}