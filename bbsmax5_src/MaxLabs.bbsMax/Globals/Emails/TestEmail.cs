//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Email
{
    /// <summary>
    /// 测试邮件
    /// </summary>
    public class TestEmail : EmailBase
    {
        private string m_ToEmail;
        private string m_SmtpServer, m_SmtpServerAccount, m_SmtpServerPassword, m_sendEmail;
        private int m_SmtpServerPort;
        private bool m_enableSSL;
        public TestEmail(string toEmail, string smtpServer, int smtpServerPort, string smtpServerAccount, string smtpServerPassword, string senderEmail,bool enableSSL)
            : base(toEmail, "这是一封测试邮件", string.Format(@"您收到的这封邮件是使用以下设置发送的：<br />
SMTP服务器：{0}<br />
服务器端口：{1}<br />
SMTP帐号：{2}<br />
SMTP密码：{3}<br />
发送邮箱：{4}<br />", smtpServer, smtpServerPort, smtpServerAccount, smtpServerPassword, senderEmail))
        {
            this.m_ToEmail = toEmail;
            this.m_SmtpServer = smtpServer;
            this.m_SmtpServerPort = smtpServerPort;
            this.m_SmtpServerAccount = smtpServerAccount;
            this.m_SmtpServerPassword = smtpServerPassword;
            this.m_sendEmail = senderEmail;
            this.m_enableSSL = enableSSL;
          
        }

        public override void Send()
        {
            EmailSendServer sendServer = new EmailSendServer();
            sendServer.SmtpServer = m_SmtpServer;
            sendServer.Port = m_SmtpServerPort;
            sendServer.SmtpServerAccount = m_SmtpServerAccount;
            sendServer.SmtpServerPassword = m_SmtpServerPassword;
            sendServer.SenderEmail = m_sendEmail;
            sendServer.EnableSSL = m_enableSSL;
            sendServer.SendMail(this);
        }

        public override bool ThrowExceptionIfSendFail
        {
            get { return true; }
        }
    }
}