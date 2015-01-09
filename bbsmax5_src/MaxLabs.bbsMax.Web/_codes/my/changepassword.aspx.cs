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

using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class changepassword : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "更改密码 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "changepassword"; }
        }

        protected override string NavigationKey
        {
            get { return "changepassword"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("更改密码");

            if (_Request.IsClick("changepassword"))
            {
                ChangePassword();
                return;
            }

        }

        private void ChangePassword()
        {
            string oldPassword;
            string newPassword, newPassword2;
            oldPassword = _Request.Get("password", Method.Post, string.Empty, false);
            newPassword = _Request.Get("newpassword", Method.Post, string.Empty, false);
            newPassword2 = _Request.Get("newpassword2", Method.Post, string.Empty, false);

            using (ErrorScope es = new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplayForForm("changePwd", new string[] { "oldpassword", "newpassword", "newpassword2" });

                if (newPassword != newPassword2)
                {
                    msgDisplay.AddError(new PasswordInconsistentError("newpassword2"));
                }
                else
                {
                    if (UserBO.Instance.ResetPassword(My, oldPassword, newPassword) == false)
                    {
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