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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.RegExp;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_email : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Email; }
        }

        private EmailSendServerCollection m_Emails;
        protected EmailSendServerCollection Emails
        {
            get
            {
                return m_Emails==null? EmailSettings.SendServers:m_Emails;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveEmailSettings();
            }
        }

        private void SaveEmailSettings()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("smtpserver","SmtpServerAccount","SmtpServerPassword","SenderEmail","Port");
            int line = 0;
            EmailSendServerCollection emails = new EmailSendServerCollection();
            int[] emailIDs = _Request.GetList<int>("emailids", Method.Post, new int[0]);

            EmailSettings settings = new EmailSettings();

            bool enableSend = _Request.Get<bool>("EnableSendEmail", Method.Post, false);
         
            if (enableSend)
            {
                foreach (int i in emailIDs)
                {
                    bool isnew = _Request.Get<bool>("isnew." + i, Method.Post, false);
                    string keysubfix = isnew ? ".new." + i : "." + i;
                    EmailSendServer email = new EmailSendServer();

                    email.EmailID = i;
                    email.SmtpServer = _Request.Get("smtpserver" + keysubfix, Method.Post);
                    email.SmtpServerAccount = _Request.Get("SmtpServerAccount" + keysubfix, Method.Post);
                    email.SmtpServerPassword = _Request.Get("SmtpServerPassword" + keysubfix, Method.Post);
                    email.SenderEmail =_Request.Get("senderemail"+keysubfix,Method.Post);
                    email.EnableSSL = _Request.Get<bool>("EnableSSL" + keysubfix, Method.Post, false);
                    email.Port = _Request.Get<int>("Port" + keysubfix, Method.Post,0);

                    ValidateEmail(email,msgDisplay,line);

                    emails.Add(email);

                    line++;
                }
                settings.SendServers = emails;

                if (emails.Count == 0)
                {
                    msgDisplay.AddError(new CustomError("开启邮件发送功能时至少填写一个Email帐号"));
                }
            }
            else
            {
                settings.SendServers = AllSettings.Current.EmailSettings.SendServers;
            }

            settings.EnableSendEmail = enableSend;

            if (!msgDisplay.HasAnyError())
            {
                SettingManager.SaveSettings(settings);
            }

            m_Emails = settings.SendServers;
        }

        private EmailRegex emailreg = new EmailRegex();

        private void ValidateEmail( EmailSendServer email,MessageDisplay msgDisplay,int line )
        {
            if ( string.IsNullOrEmpty(email.SmtpServer))
            {
                msgDisplay.AddError(new CustomError("SmtpServer", line, "smtp服务器地址不能为空！"));
            }

            if (string.IsNullOrEmpty(email.SenderEmail))
            {
                msgDisplay.AddError(new CustomError("SenderEmail", line, "发送端Email地址不能为空"));
            }
            else if (!emailreg.IsMatch(email.SenderEmail))
            {
                msgDisplay.AddError(new CustomError("SenderEmail", line, "发送端Email地址格式错误"));
            }
            

            if (string.IsNullOrEmpty(email.SmtpServerAccount))
            {
                msgDisplay.AddError(new CustomError("SmtpServerAccount",line,"邮箱用户名不能为空"));
            }

            if (string.IsNullOrEmpty(email.SmtpServerPassword))
            {
                msgDisplay.AddError(new CustomError("SmtpServerPassword", line, "邮箱密码不能为空"));
            }

            if (email.Port > 65535 || email.Port < 2)
            {
                msgDisplay.AddError(new CustomError("Port", line, "SMTP服务器端口错误"));
            }
        }
    }
}