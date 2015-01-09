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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class recoverpassword : BbsPageBase
    {
        protected override string PageTitle
        {
            get { return "找回密码 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "recoverpassword"; }
        }

        protected override string NavigationKey
        {
            get { return "recoverpassword"; }
        }
        
        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("找回密码");

#if !Passport
            PassportClientConfig setting = Globals.PassportClient;
            if (setting.EnablePassport)
            {
                Response.Redirect(setting.RecoverPasswordUrl);
                return;
            }
#endif

            if(_Request.IsClick("RecoverPassword"))
            {
                RecoverPassword();
            }
            else if(_Request.IsClick("resetRecoverPassword"))
            {
                ResetRecoverPassword(); 
            }
        }
      
        public void RecoverPassword()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("email", "username", GetValidateCodeInputName("recoverpassword"));

            using (ErrorScope es = new ErrorScope())
            {
                ValidateCodeManager.CreateValidateCodeActionRecode("recoverpassword");
                
                if (CheckValidateCode("recoverpassword", msgDisplay))
                {
                    string username = _Request.Get("username", Method.Post, string.Empty, false);
                    string email = _Request.Get("email", Method.Post, string.Empty, false);

                    UserBO.Instance.TryRecoverPassword(username, email);

                    if (es.HasUnCatchedError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        ShowSuccess("已经有一封邮件发到你的邮箱，请收取邮件，按照提示进行下一步操作", IndexUrl);
                        //msgDisplay.ShowInfoPage(this);
                    }
                }
            }
        }

        private void ResetRecoverPassword()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("newPassword", "password2");

            string password = _Request.Get("password", Method.Post, string.Empty, false);
            if (_Request.Get("password2", Method.Post, string.Empty, false) == password)
            {
                UserBO.Instance.ResetPasswordBySerial(_Request.Get("serial", Method.Get), password);
            }
            else
            {
                ThrowError(new PasswordInconsistentError("password2"));
            }
            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo errorinfo)
                {
                    msgDisplay.AddError(errorinfo.TatgetName, errorinfo.Message);
                });
            }
            else
            {
                //msgDisplay.ShowInfoPage(this);
                ShowSuccess("您已成功重设了密码！", IndexUrl);
            }
        }
    }
}