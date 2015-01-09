//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;


using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 注册设置
	/// </summary>
    public class RegisterSettings : SettingBase
    {
        public RegisterSettings()
        {
            EnableRegister = RegisterMode.Open;
            m_EmailVerifyMode = EmailVerifyMode.Disabled;
            DisplayLicenseMode = LicenseMode.Embed;
            RegisterLicenseDisplayTime = 10;
            RegisterLicenseContent = Lang.User_RegisterAgreement;//
            NewUserPracticeTime = 0;
            ActivationExpiresDate = 0;

            ActivationEmailTitle = Rescourses.Lang.Setting_RegisterSetting_ActivationEmailTitle;
            ActivationEmailContent = Lang.Setting_RegisterSetting_ActivationEmailContent;

            EmailValidationTitle = Lang.Setting_RegisterSetting_EmailValidationTitle;
            EmailValidationContent = Lang.Setting_RegisterSetting_EmailValidationContent;

            WelcomeMailTitle = Lang.Setting_RegisterSetting_WelcomeMailTitle;
            WelcomeMailContent = Lang.Setting_RegisterSetting_WelcomeContent;

            EnableWelcomeMail = false;
           

            if(ScopeList==null)
            ScopeList = new ScopeBaseCollection();
        }

        /// <summary>
        /// 开启注册
        /// </summary>
        [SettingItem]
        public RegisterMode EnableRegister { get; set; }

        /// <summary>
        /// 定时关闭注册时间列表
        /// </summary>
        [SettingItem]
        public ScopeBaseCollection ScopeList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SettingItem]
        public bool EnableWelcomeMail { get; set; }

        [SettingItem]
        public string WelcomeMailTitle { get; set; }

        [SettingItem]
        public string WelcomeMailContent { get; set; }


        private EmailVerifyMode m_EmailVerifyMode;
        /// <summary>
        /// Email验证模式
        /// </summary>
        [SettingItem]
        public EmailVerifyMode EmailVerifyMode
        {
            get 
            {
                if (AllSettings.Current.LoginSettings.LoginType == UserLoginType.Email)
                    return EmailVerifyMode.Required;
                return m_EmailVerifyMode;
            }
            set { m_EmailVerifyMode = value; }
        }

        /// <summary>
        /// 显示注册协议
        /// </summary>
        [SettingItem]
        public LicenseMode DisplayLicenseMode { get; set; }



        /// <summary>
        /// 注册协议显示时间（秒）
        /// </summary>
        [SettingItem]
        public int RegisterLicenseDisplayTime { get; set; }

        /// <summary>
        /// 注册协议内容
        /// </summary>
        [SettingItem]
        public string RegisterLicenseContent { get; set; }

        /// <summary>
        /// 新注册用户实习时间（分）
        /// </summary>
        [SettingItem]
        public int NewUserPracticeTime { get; set; }

        /// <summary>
        /// 关闭注册提示
        /// </summary>
        [SettingItem]
        public string ClosedMessage { get; set; }

        /// <summary>
        /// 邮件激活过期时间(小时)
        /// </summary>
        [SettingItem]
        public int ActivationExpiresDate { get; set; }

        /// <summary>
        /// Email激活邮件标题
        /// </summary>
        [SettingItem]
        public string ActivationEmailTitle { get; set; }

        /// <summary>
        /// Email激活邮件内容
        /// </summary>
        [SettingItem]
        public string ActivationEmailContent { get; set; }

        ///// <summary>
        ///// 邀请功能设置
        ///// </summary>
        //[SettingItem]
        //public InvitationSettings InvitationSettings { get; set; }

        /// <summary>
        /// 邮箱验证邮件标题
        /// </summary>
        [SettingItem]
        public string EmailValidationTitle
        {
            get;
            set;
        }

        /// <summary>
        /// 那个邮箱验证邮件的内容
        /// </summary>
        [SettingItem]
        public string EmailValidationContent
        {
            get;
            set;
        }
 
    }	

    /// <summary>
    /// 注册协议显示模式
    /// </summary>
    public enum LicenseMode
    {
        None,
        Independent,
        Embed
    }

    public enum RegisterMode
    { 
        Open=0,

        Closed,

        TimingClosed
    }
}