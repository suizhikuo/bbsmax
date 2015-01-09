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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Settings;

namespace  MaxLabs.bbsMax.Web.max_admin
{
    public partial class testemail : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Email; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("sendmail"))
            {
                SendMail();
                return;
            }
        }

        protected void SendMail()
        {
            string toEmail = _Request.Get("testmail", Method.Post);
            bool enableSSl = _Request.Get<bool>("ssl", Method.Get, false);
            //string content=_Request.Get("content", Method.Post);
            TestEmail email = new TestEmail(toEmail, SMTPServer, Port, Username, Password, Email,enableSSl);
            //TestEmail email = new TestEmail(toEmail);

            try
            {
                email.Send();
            }
            catch(Exception ex)
            {
                m_HasError = true;
                m_ErrorMessage = ex.Message;
            }

            if (!HasError)
            {
                ShowSuccess("已按您的配置尝试发送邮件，请检查是否正常接收到该测试邮件。标题：" + email.Subject);
            }
            else
            {
                ShowError("错误：" + ErrorMessage);
            }

            _isSend = true;
        }


        private bool m_HasError = false;

        protected bool HasError
        {
            get
            {
                return m_HasError;
            }
        }

        private string m_ErrorMessage  =string.Empty;
        protected string ErrorMessage
        {
            get
            {
                return m_ErrorMessage;
            }
        }

        bool _isSend;
        protected bool  IsSend
        {
            get
            {
                return _isSend;
            }
            set
            {
                _isSend = value;
            }
        }

        protected int Port
        {
            get
            {
                return _Request.Get<int>("port", Method.Get, 25);
            }
        }

        protected string SMTPServer
        {
            get
            {
                return _Request.Get("smtp", Method.Get);
            }
        }
        protected string Email
        {
            get
            {
                return _Request.Get("email", Method.Get) ;
            }
        }
        protected string Username
        {
            get
            {
                return _Request.Get("username", Method.Get, string.Empty, false);
            }
        }
        protected string Password
        {
            get
            {
                return _Request.Get("pwd" , Method.Get) ;
            }
        }
    }
}