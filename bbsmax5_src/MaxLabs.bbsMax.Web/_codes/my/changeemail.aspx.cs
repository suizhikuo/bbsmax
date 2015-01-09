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

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class changeemail : CenterPageBase
	{
        protected override string PageTitle
        {
            get { return "更改邮箱 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "changeemail"; }
        }

        protected override string NavigationKey
        {
            get { return "changeemail"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("设置邮箱");

            if (_Request.IsClick("emailcheck"))
            {
                ValidateEmail();
                //return;
            }
            else if (_Request.IsClick("updateEmail"))
            {
                UpdateEmail();
                //return;
            }
        }

        private string m_UnValidateEmail;
        protected string UnValidateEmail
        {
            get
            {
                if (m_UnValidateEmail == null)
                {
                    m_UnValidateEmail = UserBO.Instance.GetUnValidatedEmail(MyUserID);
                    if (m_UnValidateEmail == null)
                        m_UnValidateEmail = string.Empty;
                }

                return m_UnValidateEmail;
            }
        }

        private void UpdateEmail()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("updateEmail", new string[] { "email" });
            UserBO.Instance.UpdateEmail(My, _Request.Get("email", Method.Post));
            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
        }

        private void ValidateEmail()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("updateEmail", new string[] { "email", "password" });
            
            string email = _Request.Get("email", Method.Post, string.Empty, false);
            string password = _Request.Get("password", Method.Post, string.Empty, false);
            
            UserBO.Instance.TryChangeEmail(My, password, email);
            
            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                }
                );
            }
        }
	}
}