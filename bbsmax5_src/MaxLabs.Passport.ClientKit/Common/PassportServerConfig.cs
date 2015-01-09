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
using MaxLabs.Passport.ClientKit;
using MaxLabs.Passport.ClientKit.PassportServerInterface;

namespace MaxLabs.Passport.ClientKit
{
    /// <summary>
    /// Passport服务器配置类
    /// </summary>
    public static class PassportServerConfig
    {
        /// <summary>
        /// 登录的地址
        /// </summary>
        public static string LoginUrl{get;private set;}

        /// <summary>
        /// 退出登录的地址
        /// </summary>
        public static string LogoutUrl{get;private set;}

        /// <summary>
        /// 注册用户的地址
        /// </summary>
        public static string RegisterUrl { get; private set; }

        /// <summary>
        /// cookie的名称
        /// </summary>
        public static string CookieName { get; private set; }

        /// <summary>
        /// 找回密码页面地址
        /// </summary>
        public static string RecoverPasswordUrl { get; private set; }

        /// <summary>
        /// 用户中心的地址
        /// </summary>
        public static string CenterUrl { get; private set; }

        static PassportServerConfig()
        {
            PassportInfoProxy config = AsmxAccess.API.Passport_GetInfo();
            LoginUrl = DataConvertUtil.ConcatUrl( config.LoginUrl);
            LogoutUrl =DataConvertUtil.ConcatUrl( config.LogoutUrl);
            RegisterUrl =DataConvertUtil.ConcatUrl( config.RegisterUrl);
            CookieName = config.CookieName;
            RecoverPasswordUrl =DataConvertUtil.ConcatUrl( config.RecoverPasswordUrl);
            CenterUrl = DataConvertUtil.ConcatUrl(config.CenterUrl);
        }
    }
}