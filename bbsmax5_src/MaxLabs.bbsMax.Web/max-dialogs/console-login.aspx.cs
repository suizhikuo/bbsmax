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
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class console_login : DialogPageBase
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!My.CanLoginConsole)
            {
                ShowError("您的账号“" + My.Username + "”没有登录控制台的权限！");
                return;
            }

            if (_Request.IsClick("login"))
            {
                AdminLogin();
            }
        }

        protected string validateActionName
        {
            get
            {
                return "ManageLogin";
            }
        }

        protected string RawUrl
        {
            get
            {
                string rawurl = _Request.Get("rawurl", Method.Post,string.Empty,false);
                if (string.IsNullOrEmpty(rawurl))
                {
                    rawurl = Request.RawUrl;
                }

                if (string.IsNullOrEmpty(rawurl) || StringUtil.ContainsIgnoreCase(rawurl, "console-login.aspx"))
                {
                    ShowError("请不要直接访问该页面！");
                }
                return rawurl;
            }
        }

        private void AdminLogin()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            ValidateCodeManager.CreateValidateCodeActionRecode(validateActionName);
            if (!CheckValidateCode(validateActionName, msgDisplay))
            {
                ShowError("验证码错误！");
                return;
            }
            string password = _Request.Get("password", Method.Post, string.Empty, false);

            if (!My.CanLoginConsole)
            {
                ShowError("您没有登录控制台的权限！");
                return;
            }

            Guid sessionid;
            if (!UserBO.Instance.AdminLogin(MyUserID, password, out sessionid))
            {
                ShowError("用户名或者密码错误!");
                return;
            }
            else
            {
                Session[AdminSessionKey] = new AdminSessionStruct(sessionid, DateTimeUtil.Now, My.Password);
                Response.Redirect(RawUrl);
            }
        }
    }
}