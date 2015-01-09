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
using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Mail;
using MaxLabs.bbsMax.Email;
using System.Threading;

namespace MaxLabs.bbsMax.Settings
{
    public enum SendEmailResult
    {
        Fail = 0,

        Success = 1,

        InvalidEmailFormat = 2,

        UnknownError = -1
    }

    public class EmailSendServer : SettingBase, IBatchSave//,IComparable<EmailSendServer>
    {
        public EmailSendServer()
        {
            this.Enabled = true;
            this.SenderEmail = string.Empty;
            this.SmtpServer = string.Empty;
            this.SmtpServerAccount = string.Empty;
            this.SmtpServerPassword = string.Empty;
            this.Port = 25;
            this.IsNew = true;
            this.EnableSSL = false;
        }

        [SettingItem]
        public int EmailID { get; set; }

        /// <summary>
        /// 是否允许系统发送Email
        /// </summary>
        [SettingItem]
        public bool Enabled { get; set; }

        [SettingItem]
        public int Port { get; set; }

        /// <summary>
        /// 系统发送Email时所用的发件人地址
        /// </summary>
        [SettingItem]
        public string SenderEmail { get; set; }

        /// <summary>
        /// 用于发送Email的SMTP服务器地址
        /// </summary>
        [SettingItem]
        public string SmtpServer { get; set; }

        /// <summary>
        /// SMTP服务器的登录帐号
        /// </summary>
        [SettingItem]
        public string SmtpServerAccount { get; set; }

        /// <summary>
        /// SMTP服务器的登录密码
        /// </summary>
        [SettingItem]
        public string SmtpServerPassword { get; set; }

        [SettingItem]
        public bool EnableSSL { get; set; }

        public void BeginSendMail(EmailBase email)
        {
            if (string.IsNullOrEmpty(email.ToEmail))
                return;

            WaitCallback callback = new WaitCallback(DoBeginSendMail);
            ThreadPool.QueueUserWorkItem(callback, email);
        }

        private void DoBeginSendMail(object emailObject)
        {
            EmailBase email = emailObject as EmailBase;

            if (email == null)
                return;

            SendMail(email);
        }

        public SendEmailResult SendMail(EmailBase email)
        {
            MailMessage message = null;
            try
            {
                if (ValidateUtil.IsEmail(email.ToEmail) == false)
                    return SendEmailResult.InvalidEmailFormat;

                message = new MailMessage(SenderEmail, email.ToEmail, email.Subject, email.Content);

                SmtpClient client = new SmtpClient(this.SmtpServer);
                client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential(this.SmtpServerAccount, this.SmtpServerPassword);
                client.EnableSsl = this.EnableSSL;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                //RetryCounter++;

                client.Send(message);

                return SendEmailResult.Success;
            }
            catch
            {
                if (email.ThrowExceptionIfSendFail)
                    throw;
                else
                {
                    return SendEmailResult.Fail;
                    //if (RetryCounter <= 5 && RetryCounter < AllSettings.Current.EmailSettings.Emails.Count) //重试次数阀值为5
                    //    Send(toEmail);
                }
            }
            finally
            {
                if (message != null)
                {
                    try
                    {
                        message.Dispose();
                    }
                    catch { }
                }
            }
        }

        ///// <summary>
        ///// 本域优先
        ///// </summary>
        //[SettingItem]
        //public bool SamePriority { get; set;}


        ///// <summary>
        ///// 后缀
        ///// </summary>
        //private string m_Suffix;
        //public string Suffix
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(m_Suffix))
        //        {
        //            if (string.IsNullOrEmpty(this.SenderEmail))
        //                return string.Empty;

        //            int i = this.SenderEmail.IndexOf("@");

        //            if (i == -1)
        //                return string.Empty;
        //            m_Suffix = this.SenderEmail.Substring(i).ToLower();
        //        }
        //        return m_Suffix;
        //    }
        //}

        //public int UseTimes
        //{
        //    get;
        //    set;
        //}

        #region IBatchSave 成员

        public bool IsNew
        {
            get;
            set;
        }

        #endregion

        //#region IComparable<EmailItem> 成员

        //public int CompareTo(EmailSendServer other)
        //{
        //    return 0 - other.UseTimes.CompareTo(this.UseTimes); //
        //}

        //#endregion
    }

    public class EmailSendServerCollection : Collection<EmailSendServer>, ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (EmailSendServer item in this)
            {
                list.Add(item.ToString());
            }
            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    EmailSendServer field = new EmailSendServer();
                    field.Parse(item);

                    this.Add(field);
                }
            }
        }
    }
}