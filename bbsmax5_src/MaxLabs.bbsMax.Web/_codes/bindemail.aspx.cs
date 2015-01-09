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
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages
{
	public partial class bindemail : BbsPageBase
	{
        protected override string PageTitle
        {
            get { return "解决邮箱冲突 - " + base.PageTitle; }
        }

        private string m_Email = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("解决邮箱冲突");

            m_Email = _Request.Get("email", Method.Get);

            if (string.IsNullOrEmpty(m_Email))
            {
                ShowError(new EmptyEmailError("email"));
                return;
            }
            else if (ValidateUtil.IsEmail(m_Email) == false)
            {
                ShowError(new EmailFormatError("email", m_Email));
                return;
            }

            if (_Request.IsClick("bindemail"))
            {
                EmailBind();
            }
        }

        private void EmailBind()
        {

            MessageDisplay msgDisplay = CreateMessageDisplay("username", "password", GetValidateCodeInputName(validateActionName));
            string username = _Request.Get("username", Method.Post, string.Empty, false).Trim();
            string password = _Request.Get("password", Method.Post, string.Empty, false);

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    Success = UserBO.Instance.BindEmail(username, password, m_Email);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }


                if (Success)
                {
                    ShowSuccess("您的账号与Email绑定成功!", BbsRouter.GetUrl("login"));
                }
                else
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
        protected string validateActionName
        {
            get
            {
                return "login";
            }
        }

        private bool m_Success = false;
        public bool Success
        {
            get { return m_Success; }
            set { m_Success = value; }
        }
	}
}