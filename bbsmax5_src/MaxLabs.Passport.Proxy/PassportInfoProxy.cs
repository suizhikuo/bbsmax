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

namespace MaxLabs.Passport.Proxy
{
    public class PassportInfoProxy : ProxyBase
    {
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisterUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 头像生成器地址
        /// </summary>
        public string AvatarGeneratorUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 登出地址
        /// </summary>
        public string LogoutUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 登录地址
        /// </summary>
        public string LoginUrl { get; set; }

        /// <summary>
        /// 取回密码的地址
        /// </summary>
        public string RecoverPasswordUrl { get; set; }

        /// <summary>
        /// 通用COOKIE键
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// 是否开手机验证
        /// </summary>
        public bool EnablePhoneValidate
        {
            get;
            set;
        }

        public bool EnableRealnameCheck
        {
            get;
            set;
        }

        public string SettingAvatarUrl
        {
            get;
            set;
        }

        public string SettingPrlofileUrl
        {
            get;
            set;
        }

        public string SettingChangeEmailUrl
        {
            get;
            set;
        }

        public string SettingNotifyUrl { get; set; }

        public string CenterUrl
        {
            get;
            set;
        }

        public string CenterNotifyUrl { get; set; }

        public string CenterChatUrl { get; set; }

        public string CenterFriendUrl { get; set; }

        public List<ExtendedFieldProxy> ExtendedFields
        {
            get;
            set;
        }

        public string SettingPasswordUrl { get; set; }
    }
}