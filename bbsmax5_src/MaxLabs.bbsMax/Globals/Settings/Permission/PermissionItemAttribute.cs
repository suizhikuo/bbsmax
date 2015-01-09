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

namespace MaxLabs.bbsMax.Settings
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionItemAttribute : Attribute
    {
        public PermissionItemAttribute()
        {
            Administrators = RoleOption.DefaultAllow;
            Owners = RoleOption.AlwaysAllow;
        }

        public PermissionItemAttribute(string name)
        {
            Name = name;
            Administrators = RoleOption.DefaultAllow;
            Owners = RoleOption.AlwaysAllow;
        }

        public string Name { get; set; }

        public RoleOption Everyone { get; set; }

        public RoleOption Guests { get; set; }

        public RoleOption Users { get; set; }

        public RoleOption NewUsers { get; set; }

        public RoleOption ForumBannedUsers { get; set; }

        public RoleOption FullSiteBannedUsers { get; set; }

        public RoleOption RealnameNotProvedUsers { get; set; }

        public RoleOption AvatarNotProvedUsers { get; set; }

        public RoleOption EmailNotProvedUsers { get; set; }

        public RoleOption InviteLessUsers { get; set; }

        public RoleOption Moderators { get; set; }

        public RoleOption CategoryModerators { get; set; }

        public RoleOption SuperModerators { get; set; }

        public RoleOption Administrators { get; set; }

        public RoleOption Owners { get; set; }
    }

    public enum RoleOption
    {
        /// <summary>
        /// 默认未设置
        /// </summary>
        DefaultNotset = 0,

        /// <summary>
        /// 默认允许
        /// </summary>
        DefaultAllow = 1,

        /// <summary>
        /// 默认禁止
        /// </summary>
        DefaultDeny = 2,

        /// <summary>
        /// 始终未设置
        /// </summary>
        AlwaysNotset = 3,

        /// <summary>
        /// 始终允许
        /// </summary>
        AlwaysAllow = 4,

        /// <summary>
        /// 始终禁止
        /// </summary>
        AlwaysDeny = 5
        
    }
}