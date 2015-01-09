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
using System.Collections.Specialized;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using System.Net.Mail;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Email
{
    public class EmailBase
    {
        //public delegate void MassMailingCallback(string mailAddress,string errorMessage);

        private string m_SubjectTemplate = null;
        private string m_ContentTempdate = null;
        private MessageRender m_MessageRender = new MessageRender();
        private int m_RetryCounter = 0; //尝试计数器


        private string m_ToEmail;
        private string m_Subject;//标题
        private string m_Content;//内容

        public MessageRender Render
        {
            get { return m_MessageRender; }
        }

        public EmailBase() { }

        public EmailBase(string toEmail, string subjectTemplate, string contentTemplate)
        {
            this.m_ToEmail = toEmail;
            this.m_SubjectTemplate = subjectTemplate;
            this.m_ContentTempdate = contentTemplate;
        }

        public virtual string Subject
        {
            get
            {
                if (string.IsNullOrEmpty(m_Subject))
                    m_Subject = m_MessageRender.Render(m_SubjectTemplate);

                return m_Subject;
            }
        }

        public virtual string Content
        {
            get
            {
                if (string.IsNullOrEmpty(m_Content))
                    m_Content= m_MessageRender.Render(this.m_ContentTempdate);

                return m_Content;
            }
            //set
            //{
            //    m_Content = value;
            //}
        }

        public virtual bool ThrowExceptionIfSendFail
        {
            get { return false; }
        }

        public virtual void Send()
        {
            EmailSettings settings = AllSettings.Current.EmailSettings;

            if (settings.EnableSendEmail == false)
                return;

            if (string.IsNullOrEmpty(m_ToEmail))
                return;

            EmailSendServerCollection servers = settings.SendServers;

            EmailSendServer selected = null;

            if (servers.Count > 1)
            {
                Random rnd = new Random();
                int rndIndex = rnd.Next(0, 10000000) % servers.Count;
                selected = servers[rndIndex];
            }
            else if (servers.Count == 1)
            {
                selected = servers[0];
            }
            else
                return;

            
            selected.BeginSendMail(this);

            //selected.UseTimes++;
            //return selected;
        }


        public string ToEmail
        {
            get { return m_ToEmail; }
        }

        //protected virtual EmailSendServer SendServer { get; set; }


        //private EmailSendServer GetEmailSendServer(string userEmail)
        //{
        //    EmailSendServerCollection emails = AllSettings.Current.EmailSettings.Emails;

        //    EmailSendServer selected = null;

        //    if (emails.Count > 1)
        //    {
        //        Random rnd = new Random();
        //        int rndIndex = rnd.Next(0, 10000000) % emails.Count;
        //        selected = emails[rndIndex];
        //    }
        //    else if (emails.Count == 1)
        //    {
        //        selected = emails[0];
        //    }
        //    else
        //        return null;

        //    selected.UseTimes++;
        //    return selected;
        //}

        //private string _smtpServer = null;
        //public virtual string SmtpServer
        //{
        //    get
        //    {
        //        return this._smtpServer != null ? _smtpServer
        //            : SendServer.SmtpServer
        //            ;
        //    }
        //    set{
        //        this._smtpServer=value;
        //    }
        //}

        //protected bool IsTest;



        //private SendEmailResult Send(EmailSendServer sendServer, string toEmail, bool throwExceptionIfFail)
        //{
        //    MailMessage message = null;
        //    try
        //    {
        //        if (ValidateUtil.IsEmail(toEmail) == false)
        //            return SendEmailResult.InvalidEmailFormat;

        //        if (sendServer == null)
        //            return SendEmailResult.UnknownError;

        //        message = new MailMessage(sendServer.SenderEmail, toEmail, this.Subject, this.Content);

        //        SmtpClient client = new SmtpClient(sendServer.SmtpServer);
        //        client.UseDefaultCredentials = false;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.Credentials = new System.Net.NetworkCredential(sendServer.SmtpServerAccount, sendServer.SmtpServerPassword);

        //        message.IsBodyHtml = true;
        //        message.BodyEncoding = Encoding.UTF8;
        //        //RetryCounter++;

        //        client.Send(message);

        //        return SendEmailResult.Success;
        //    }
        //    catch
        //    {
        //        if (throwExceptionIfFail)
        //            throw;
        //        else
        //        {
        //            return SendEmailResult.Fail;
        //            //if (RetryCounter <= 5 && RetryCounter < AllSettings.Current.EmailSettings.Emails.Count) //重试次数阀值为5
        //            //    Send(toEmail);
        //        }
        //    }
        //    finally
        //    {
        //        if (message != null)
        //        {
        //            try
        //            {
        //                message.Dispose();
        //            }
        //            catch { }
        //        }
        //    }
        //}







        //public virtual void Send(string address)
        //{
        //    if (ValidateUtil.IsEmail(address))
        //        ThreadPool.QueueUserWorkItem(new WaitCallback(SendSync), address);
        //}

        //protected void Send(object email)
        //{
        //    if (email == null)
        //        return;

        //    string emailAddress = email.ToString();

        //    try
        //    {
        //        if (AllSettings.Current.EmailSettings.EnableSendEmail == false)
        //            return;

        //        if (ValidateUtil.IsEmail(emailAddress) == false)
        //            return;

        //        if (SendServer == null)
        //            SendServer = GetEmailSendServer(emailAddress);

        //        if (SendServer == null)
        //        {
        //            LogHelper.CreateErrorLog(new Exception("未设置任何邮件客户端"));
        //            return;
        //        }

        //        MailMessage message = new MailMessage(SendServer.SenderEmail, emailAddress, this.Subject, this.Content);

        //        SmtpClient client = new SmtpClient(SendServer.SmtpServer);
        //        client.UseDefaultCredentials = false;
        //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        client.Credentials = new System.Net.NetworkCredential(SendServer.SmtpServerAccount, SendServer.SmtpServerPassword);

        //        message.IsBodyHtml = true;
        //        message.BodyEncoding = Encoding.UTF8;
        //        m_RetryCounter++;

        //        client.Send(message);
        //    }
        //    catch
        //    {

        //        if (IsTest)
        //            throw;
        //        else
        //        {
        //            if (m_RetryCounter <= 5 && m_RetryCounter < AllSettings.Current.EmailSettings.Emails.Count) //重试次数阀值为5
        //                Send(emailAddress);
        //        }
        //    }
        //    finally
        //    {
        //        message.Dispose();
        //    }
        //}

        ///// <summary>
        ///// 群发， 失败的地址回调MassMailingCallback传回
        ///// </summary>
        ///// <param name="mailAddress">邮件地址数组</param>
        ///// <param name="callback">回调函数， 如果失败就回调这个函数， 传出失败的地址和失败原因</param>
        //public void Send(string[] mailAddress)
        //{
        //    if (mailAddress == null) return;
        //    WaitCallback callback = new WaitCallback(SendSync);
        //    foreach (string s in mailAddress)
        //    {
        //        if (ValidateUtil.IsEmail(s))
        //            ThreadPool.QueueUserWorkItem(callback, s.Trim());
        //    }
        //}

    }
}