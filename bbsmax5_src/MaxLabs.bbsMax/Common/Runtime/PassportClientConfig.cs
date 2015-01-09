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
using MaxLabs.bbsMax.PassportServerInterface;
using System.Web;
using System.Net;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Settings
{
    public class PassportClientConfig : ICloneable
    {
        public PassportClientConfig()
        {
            this.PassportTimeout = 5;
            this.AccessKey = StringUtil.BuiderRandomString(10);
        }

        private ClientInfoHead m_SoapHeader;
        private ClientInfoHead SoapHeader
        {
            get
            {
                ClientInfoHead soapHeader = m_SoapHeader;

                if (soapHeader == null)
                {
                    soapHeader = new ClientInfoHead();
                    soapHeader.ClientID = this.ClientID;
                    soapHeader.AccessKey = this.AccessKey;

                    m_SoapHeader = soapHeader;
                }

                return soapHeader;
            }
        }

        private Service m_PassportService = null;
        public Service PassportService
        {
            get
            {
                Service service = m_PassportService;

                if (service == null)
                {
                    try
                    {
                        service = new Service();
                        service.Url = UrlUtil.JoinUrl(this.PassportRoot, passport_APIFileName);
                        service.ClientInfoHeadValue = SoapHeader;
                        service.Timeout = PassportTimeout *1000;// timeout;
                    }
                    catch// (Exception ex)
                    {
                        throw new Exception("通行证通讯异常，这可能是服务器网络不稳定导致的。");
                    }

                    m_PassportService = service;
                }

                return service;
            }
        }

        private const string passport_APIFileName = "api.asmx";
        public APIResult RegisterPassportClient(string pasportOwnerUsername, string passportOwnerPassword, out int clientID)
        {
            clientID = 0;
            APIResult result = null;
            try
            {
                result = PassportService.Passport_RegiserClient(pasportOwnerUsername, passportOwnerPassword, AllSettings.Current.SiteSettings.BbsName, Globals.FullAppRoot, "client.aspx", this.AccessKey, new int[0], out clientID);
            }
            catch (Exception ex)
            {
                Context.ThrowError(new APIError(ex.Message));
                return null;
            }
            return result;
        }

        /// <summary>
        /// 测试到通行证的通讯是否正常
        /// </summary>
        /// <param name="passportRoot">通行证的所在Url。例如:http://passport.bbsmax.com/</param>
        /// <param name="timeout">通讯超时时间，单位是秒</param>
        /// <returns></returns>
        public bool TestPassportService(string passportRoot, int timeout)
        {
            try
            {
                Service service = new Service();
                service.Url = UrlUtil.JoinUrl(passportRoot, passport_APIFileName);
                service.ClientInfoHeadValue = SoapHeader;
                service.Timeout = PassportTimeout * 1000;// timeout;
                PassportInfoProxy info = service.Passport_GetInfo();

                return info != null;
            }
            catch(Exception ex)
            {
                MaxLabs.WebEngine.Context.ThrowError(new PassportClientError("无法连接" + passportRoot + "上的Passport服务器！<br />原因：" + ex.Message));
                return false;
            }
        }

        public PassportInfoProxy PassportConfig
        {
            get
            {
                if (m_PassportInfo == null)
                {
                    InitPassportInfo();
                }

                return m_PassportInfo;
            }
        }

        public static object locker = new object();

        PassportInfoProxy m_PassportInfo = null;
        private void InitPassportInfo()
        {
            PassportInfoProxy passportInfo = m_PassportInfo;

            if (passportInfo == null)
            {
                lock (locker)
                {
                    if (m_PassportInfo == null)
                    {
                        passportInfo = PassportService.Passport_GetInfo();
                        //m_PassportInfo = passportInfo;
                        this.m_LoginUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.LoginUrl);
                        this.m_LogoutUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.LogoutUrl);
                        this.m_RegisterUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.RegisterUrl);
                        this.m_RecoverPasswordUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.RecoverPasswordUrl);
                        this.m_AvatarGeneratorUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.AvatarGeneratorUrl);

                        this.m_SettingAvatarUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.SettingAvatarUrl);
                        this.m_SettingChangeEmailUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.SettingChangeEmailUrl);
                        this.m_SettingNotifyUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.SettingNotifyUrl);
                        this.m_SettingPasswordUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.SettingPasswordUrl);
                        this.m_SettingPrlofileUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.SettingPrlofileUrl);

                        this.m_CenterUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.CenterUrl);
                        this.m_CenterChatUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.CenterChatUrl);
                        this.m_CenterNotifyUrl = UrlUtil.JoinUrl(this.PassportRoot, passportInfo.CenterNotifyUrl);

                        this.m_CookieName = passportInfo.CookieName;

                        #region 关联设置
                        AllSettings.Current.PhoneValidateSettings.Open = passportInfo.EnablePhoneValidate;
                        AllSettings.Current.NameCheckSettings.EnableRealnameCheck = passportInfo.EnableRealnameCheck;
                        #endregion

                        m_PassportInfo = passportInfo;
                    }
                }
            }
        }

        //public void SaveConfig()
        //{
        //    if ()
        //}

        private string m_RegisterUrl;
        //挂接到通行证后的注册页面地址
        public string RegisterUrl
        {
            get
            {
                if (m_RegisterUrl == null)
                    InitPassportInfo();

                return m_RegisterUrl;
            }
        }

        private string m_CookieName;
        //挂接到通行证后的Cookie名称
        public string CookieName
        {
            get
            {
                if (m_CookieName == null)
                    InitPassportInfo();

                return m_CookieName;
            }
        }

        private string m_LogoutUrl;
        //挂接到通行证后的退出登录页面地址
        public string LogoutUrl
        {
            get
            {
                if (m_LogoutUrl == null)
                    InitPassportInfo();
                
                return m_LogoutUrl;
            }
        }

        private string m_LoginUrl;
        //挂接到通行证后的登录页面地址
        public string LoginUrl
        {
            get
            {
                if (m_LoginUrl == null)
                    InitPassportInfo();
                
                return m_LoginUrl;
            }
        }

        private string m_AvatarGeneratorUrl;
        public string AvatarGeneratorUrl
        {
            get
            {
                if (m_AvatarGeneratorUrl == null)
                    InitPassportInfo();
                
                return m_AvatarGeneratorUrl;
            }
        }

        private string m_RecoverPasswordUrl;
        //挂接到通行证后的找回密码页面地址
        public string RecoverPasswordUrl
        {
            get    
            {
                if (m_RecoverPasswordUrl == null)
                    InitPassportInfo();

                return m_RecoverPasswordUrl;
            }
        }

        private string m_SettingAvatarUrl;
        public string SettingAvatarUrl
        {
            get
            {
                if (m_SettingAvatarUrl == null)
                    InitPassportInfo();

                return m_SettingAvatarUrl;
            }
        }

        private string m_SettingPrlofileUrl;
        public string SettingProfileUrl
        {
            get
            {
                if (m_SettingPrlofileUrl == null)
                    InitPassportInfo();

                return m_SettingPrlofileUrl;
            }
        }

        private string m_SettingPasswordUrl;
        public string SettingPasswordUrl
        {

            get
            {
                if (m_SettingPasswordUrl == null)
                    InitPassportInfo();

                return m_SettingPasswordUrl;
            }
        }


        private string m_SettingChangeEmailUrl;
        public string SettingChangeEmailUrl
        {
            get
            {
                if (m_SettingChangeEmailUrl == null)
                    InitPassportInfo();

                return m_SettingChangeEmailUrl;
            }
        }

        private string m_SettingNotifyUrl;
        public string SettingNotifyUrl
        {
            get
            {
                if (m_SettingNotifyUrl == null)
                    InitPassportInfo();

                return m_SettingNotifyUrl;
            }
        }

        private string m_CenterUrl;
        public string CenterUrl
        {
            get
            {
                if (m_CenterUrl == null)
                    InitPassportInfo();
                return m_CenterUrl;
            }
        }

        private string m_CenterChatUrl;
        public string CenterChatUrl
        {
            get
            {
                if (m_CenterChatUrl == null)
                    InitPassportInfo();
                return m_CenterChatUrl;
            }
        }

        private string m_CenterNotifyUrl;
        public string CenterNotifyUrl
        {
            get
            {
                if (m_CenterNotifyUrl == null)
                    InitPassportInfo();
                return m_CenterNotifyUrl;
            }
        }

        /// <summary>
        /// 是否启用passport服务器对接
        /// </summary>
        public bool EnablePassport { get; set; }

        /// <summary>
        /// passport服务器地
        /// </summary>
        public string PassportRoot { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// 通信密钥
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// WEB服务请求超时时间（单位为妙）
        /// </summary>
        public int PassportTimeout { get; set; }


        #region 重载Equals 和 ==

        /// <summary>
        /// 重载==
        /// </summary>
        public static bool operator ==(PassportClientConfig config1, PassportClientConfig config2)
        {
            if (config1 as object == null)
                return config2 as object == null;

            if (config2 as object == null)
                return config1 as object == null;

            return (
                config1.EnablePassport == config2.EnablePassport
                &&
                config1.PassportRoot == config2.PassportRoot
                &&
                config1.ClientID == config2.ClientID
                &&
                config1.AccessKey == config2.AccessKey
                &&
                config1.PassportTimeout == config2.PassportTimeout
                );
        }

        /// <summary>
        /// 重载!=
        /// </summary>
        public static bool operator !=(PassportClientConfig config1, PassportClientConfig config2)
        {
            if (config1 as object == null)
                return config2 as object != null;

            if (config2 as object == null)
                return config1 as object != null;

            return (
                config1.EnablePassport != config2.EnablePassport
                ||
                config1.PassportRoot != config2.PassportRoot
                ||
                config1.ClientID != config2.ClientID
                ||
                config1.AccessKey != config2.AccessKey
                ||
                config1.PassportTimeout != config2.PassportTimeout
                );
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            PassportClientConfig config = new PassportClientConfig();
            config.AccessKey = this.AccessKey;
            config.ClientID = this.ClientID;
            config.EnablePassport = this.EnablePassport;
            config.PassportRoot = this.PassportRoot;
            config.PassportTimeout = this.PassportTimeout;

            return config;
        }

        #endregion
    }
}