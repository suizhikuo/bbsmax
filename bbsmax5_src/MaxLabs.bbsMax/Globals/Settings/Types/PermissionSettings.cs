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

namespace MaxLabs.bbsMax.Settings
{
    public class PermissionSettings : SettingBase
    {
        public PermissionSettings()
        {
            UserPermissionLimit = new PermissionLimit(PermissionLimitType.RoleLevelLowerMe);
            ContentPermissionLimit = new PermissionLimit(PermissionLimitType.RoleLevelLowerOrSameMe);
            MarkPermissionLimit = new PermissionLimit(PermissionLimitType.RoleLevelLowerMe);
        }

        /// <summary>
        /// 管理用户的限制
        /// </summary>
        [SettingItem]
        public PermissionLimit UserPermissionLimit { get; set; }

        /// <summary>
        /// 管理内容的限制
        /// </summary>
        [SettingItem]
        public PermissionLimit ContentPermissionLimit { get; set; }


        /// <summary>
        /// 评分的限制
        /// </summary>
        [SettingItem]
        public PermissionLimit MarkPermissionLimit { get; set; }
    }
}