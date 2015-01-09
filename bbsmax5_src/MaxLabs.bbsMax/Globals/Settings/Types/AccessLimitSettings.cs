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

using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 访问限制设置
    /// </summary>
    public class AccessLimitSettings : SettingBase,ICloneable<AccessLimitSettings>
    {
        public AccessLimitSettings()
        {
            //AccessIPLimitMode = LimitMode.Free;
            AccessIPLimitList = new IPMatchRegulation();

            AdminIPLimitMode = LimitMode.Free;
            AdminIPLimitList = new IPMatchRegulation();
        }

        ///// <summary>
        ///// 论坛访问IP限制模式
        ///// </summary>
        //[SettingItem]
        //public LimitMode AccessIPLimitMode { get; set; }

        /// <summary>
        /// 论坛访问IP限制方式
        /// </summary>
        [SettingItem]
        public IPMatchRegulation AccessIPLimitList { get; set; }

        /// <summary>
        /// 管理员控制台访问IP限制模式
        /// </summary>
        [SettingItem]
        public LimitMode AdminIPLimitMode { get; set; }

        /// <summary>
        /// 管理员控制台访问IP限制方式
        /// </summary>
        [SettingItem]
        public IPMatchRegulation AdminIPLimitList { get; set; }


        #region ICloneable<AccessLimitSettings> 成员

        public AccessLimitSettings Clone()
        {
            AccessLimitSettings settings = new AccessLimitSettings();
            settings.AccessIPLimitList = AccessIPLimitList;
            settings.AdminIPLimitList = AdminIPLimitList;
            settings.AdminIPLimitMode = AdminIPLimitMode;

            return settings;
        }

        #endregion
    }
}