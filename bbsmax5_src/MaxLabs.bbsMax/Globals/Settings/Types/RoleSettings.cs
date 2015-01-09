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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 等级用户组升级依赖
    /// </summary>
    public enum LevelLieOn 
    {
        /// <summary>
        /// 总积分
        /// </summary>
        Point,

        /// <summary>
        /// 在线时间
        /// </summary>
        OnlineTime,

        /// <summary>
        /// 主题数
        /// </summary>
        Topic,

        /// <summary>
        /// 发帖数
        /// </summary>
        Post
    }

    public class RoleSettings : SettingBase
    {
        public RoleSettings()
        {
            string iconPath = Globals.GetRelativeUrl(SystemDirecotry.Assets_RoleIcon);

            Roles = new RoleCollection();
            Roles.Add(Role.Everyone);
            Roles.Add(Role.Guests);
            Roles.Add(Role.Users);
            Roles.Add(Role.ForumBannedUsers);
            Roles.Add(Role.FullSiteBannedUsers);

            Roles.Add(Role.NewUsers);
            Roles.Add(Role.EmailNotProvedUsers);
            Roles.Add(Role.RealnameNotProvedUsers);
            //Roles.Add(Role.InviteLessUsers);

            Roles.Add(Role.JackarooModerators);
            Roles.Add(Role.Moderators);
            Roles.Add(Role.CategoryModerators);
            Roles.Add(Role.SuperModerators);
            Roles.Add(Role.Administrators);
            Roles.Add(Role.Owners);

            Roles.Add(Role.NoLevel);

            LevelLieOn = LevelLieOn.Post;
            /*-----------------------默认图标------------------------*/

            Role.CategoryModerators.IconUrlSrc  = UrlUtil.JoinUrl(iconPath, "pips10.gif");
            Role.Administrators.IconUrlSrc      = UrlUtil.JoinUrl(iconPath, "pips10.gif");
            Role.SuperModerators.IconUrlSrc     = UrlUtil.JoinUrl(iconPath, "pips10.gif");
            Role.Owners.IconUrlSrc              = UrlUtil.JoinUrl(iconPath, "pips10.gif");
            Role.Moderators.IconUrlSrc          = UrlUtil.JoinUrl(iconPath, "pips9.gif");
            Role.JackarooModerators.IconUrlSrc  = UrlUtil.JoinUrl(iconPath, "pips8.gif");

            /*-------------------默认等级用户组------------------*/
            Role levelRole = new Role();
            levelRole.Name = "新手上路";
            levelRole.RequiredPoint = 0;
            levelRole.RoleID = new Guid(new byte[] { 74, 115, 163, 71, 186, 53, 83, 71, 183, 179, 236, 16, 210, 32, 146, 103 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips1.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "侠客";
            levelRole.RequiredPoint = 50;
            levelRole.RoleID = new Guid(new byte[] { 248, 152, 64, 183, 181, 77, 53, 77, 182, 73, 89, 85, 162, 252, 146, 41 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips2.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "圣骑士";
            levelRole.RequiredPoint = 200;
            levelRole.RoleID = new Guid(new byte[] { 29, 40, 140, 204, 235, 17, 207, 73, 174, 43, 138, 98, 96, 82, 219, 68 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips3.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "精灵";
            levelRole.RequiredPoint = 500;
            levelRole.RoleID = new Guid(new byte[] { 26, 191, 38, 123, 38, 152, 103, 71, 171, 98, 16, 19, 63, 224, 29, 77 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips4.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "精灵王";
            levelRole.RequiredPoint = 1000;
            levelRole.RoleID = new Guid(new byte[] { 92, 96, 198, 205, 96, 80, 220, 66, 172, 38, 16, 150, 6, 184, 0, 251 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips5.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "风云使者";
            levelRole.RequiredPoint = 5000;
            levelRole.RoleID = new Guid(new byte[] { 165, 75, 133, 177, 234, 6, 16, 66, 162, 12, 235, 64, 5, 68, 248, 140 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips6.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "光明使者";
            levelRole.RequiredPoint = 10000;
            levelRole.RoleID = new Guid(new byte[] { 185, 185, 243, 95, 132, 73, 233, 67, 128, 221, 168, 188, 74, 206, 151, 196 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips7.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "天使";
            levelRole.RequiredPoint = 50000;
            levelRole.RoleID = new Guid(new byte[] { 164, 254, 210, 237, 225, 87, 214, 77, 171, 96, 92, 245, 221, 54, 78, 154 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips8.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);

            levelRole = new Role();
            levelRole.Name = "法老";
            levelRole.RequiredPoint = 100000;
            levelRole.RoleID = new Guid(new byte[] { 188, 159, 219, 20, 145, 63, 6, 67, 184, 52, 226, 115, 211, 207, 90, 98 });
            levelRole.Title = levelRole.Name;
            levelRole.Type = RoleType.Custom | RoleType.Level | RoleType.Virtual;
            levelRole.IconUrlSrc = UrlUtil.JoinUrl(iconPath, "pips9.gif");
            levelRole.IsNew = false;
            Roles.Add(levelRole);
            /*-----------------------end-------------------------*/

            Roles.Sort();

        }

        /// <summary>
        /// 不整体序列化
        /// </summary>
        public override bool Serializable
        {
            get { return false; }
        }

        /// <summary>
        /// 等级组的升级依据
        /// </summary>
        [SettingItem]
        public LevelLieOn LevelLieOn { get; set; }


        /// <summary>
        /// 所有用户组
        /// </summary>
        [SettingItem]
        public RoleCollection Roles { get; set; }


        //以下4个方法用于获得4种不同类型的用户组

        #region 基本组 GetBasicRoles

        private RoleCollection m_BasicRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetBasicRoles()
        {
            if (m_BasicRoles == null)
            {
                m_BasicRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsBasic);
                });
            }

            return m_BasicRoles;
        }

        #endregion

        #region 等级组 GetLevelRoles

        private RoleCollection m_LevelRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetLevelRoles()
        {
            if (m_LevelRoles == null)
            {
                m_LevelRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsLevel);
                });
            }

            return m_LevelRoles;
        }

        #endregion

        #region 自定义组 GetNormalRoles

        private RoleCollection m_NormalRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetNormalRoles()
        {
            if (m_NormalRoles == null)
            {
                m_NormalRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsNormal);
                });
            }

            return m_NormalRoles;
        }

        #endregion

        #region 管理组 GetManagerRoles

        private RoleCollection m_ManagerRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetManagerRoles()
        {
            if (m_ManagerRoles == null)
            {
                RoleCollection roles = GetRoles(delegate(Role role)
                {
                    return (role.IsManager);
                });

                m_ManagerRoles = roles;
            }

            return m_ManagerRoles;
        }

        #endregion


        //以下方法用于辅助

        //获取
        #region 获得所有虚拟的用户组（即不能往其中添加成员的用户组，用户是否隶属于此类用户组依赖于实时计算） GetVirturlRoles

        private RoleCollection m_VirtualRoles = null;
        /// <summary>
        /// 获取所有非虚拟的用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetVirtualRoles()
        {
            if (m_VirtualRoles == null)
            {
                m_VirtualRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsVirtualRole);
                });
            }

            return m_VirtualRoles;
        }

        #endregion


        #region 获得所有非虚拟的用户组（即可以往其中添加成员的用户组） GetNonVirturlRoles

        private RoleCollection m_NonVirtualRoles = null;
        /// <summary>
        /// 获取所有非虚拟的用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetNonVirtualRoles()
        {
            if (m_NonVirtualRoles == null)
            {
                m_NonVirtualRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsVirtualRole == false);
                });
            }

            return m_NonVirtualRoles;
        }

        #endregion

        #region 获取所有用户组，但排除掉指定的用户组 GetRoles

        /// <summary>
        /// 获取所有用户组，但排除掉指定的用户组
        /// </summary>
        /// <param name="exceptRoleIds"></param>
        /// <returns></returns>
        public RoleCollection GetRoles(params Role[] exceptRoles)
        {
            RoleCollection roles = new RoleCollection();

            foreach (Role role in Roles)
            {
                bool isExcept = false;
                foreach (Role exceptRole in exceptRoles)
                {
                    if (role == exceptRole)
                    {
                        isExcept = true;
                        break;
                    }
                }

                if (isExcept == false)
                    roles.Add(role);
            }

            return roles;
        }

        #endregion

        #region 获取给后台作为自动加入的用户组列表。例如：作为任务奖励自动加入用户组，那么只能在这些用户组里面选择 GetRolesForAutoAdd

        /// <summary>
        /// 获取给后台作为自动加入的用户组列表。例如：作为任务奖励自动加入用户组，那么只能在这些用户组里面选择
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetRolesForAutoAdd()
        {
            RoleCollection roles = new RoleCollection();
            foreach (Role r in Roles)
            {

                if ((r.Type & RoleType.Normal) == RoleType.Normal
                    &&
                    (r.Type & RoleType.Virtual) != RoleType.Virtual
                    &&
                    (r.Type & RoleType.System) != RoleType.System
                    )
                {
                    roles.Add(r);
                }
            }

            return roles;
        }

        #endregion

        #region 获取指定条件的用户组（推荐使用匿名委托来指定条件） GetRoles

        /// <summary>
        /// 获取指定条件的用户组（推荐使用匿名委托来指定条件）
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public RoleCollection GetRoles(RoleResolver resolver)
        {
            RoleCollection roles = new RoleCollection();

            foreach (Role role in Roles)
            {
                if (resolver(role))
                    roles.Add(role);
            }

            return roles;
        }

        #endregion

        #region 根据ID查找指定的用户组 GetRole

        /// <summary>
        /// 根据ID查找指定的用户组
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public Role GetRole(Guid roleID)
        {
            return this.Roles.GetValue(roleID);
        }

        #endregion

        public void AddRole(Role role)
        {
            Roles.Add(role);
        }

        public void SetRole(Role role)
        {
            Roles.Set(role);
        }

        public void RemoveRole(Guid roleID)
        {
            Roles.RemoveByKey(roleID);
        }


        public string GetRoleNames(IEnumerable<Guid> roleIDs,string separator)
        {
            if (ValidateUtil.HasItems(roleIDs) == false)
                return string.Empty;

            StringBuilder roleNames = new StringBuilder();

            foreach (Guid roleID in roleIDs)
            {
                Role role;
                if (Roles.TryGetValue(roleID, out role))
                    roleNames.Append(role.Name).Append(separator);
            }

            if (roleNames.Length > 0)
                return roleNames.ToString(0, roleNames.Length - separator.Length);

            return string.Empty;
        }

        public bool CanLoginConsole(Guid roleID)
        {
            Role role = GetRole(roleID);

            if (role == null)
                return false;

            if (role.IsManager && role.CanLoginConsole)
                return true;

            return false;
        }

        /// <summary>
        /// role1是否可以管理role2的用户
        /// </summary>
        /// <param name="role1"></param>
        /// <param name="role2"></param>
        /// <returns></returns>
        public bool CanManageUser(Guid role1, Guid role2)
        {
            PermissionLimit limit = AllSettings.Current.PermissionSettings.UserPermissionLimit;
            return  CanManage(GetRole(role1), GetRole(role2), limit);
        }

        /// <summary>
        /// role1是否可以管理role2的内容
        /// </summary>
        /// <param name="role1"></param>
        /// <param name="role2"></param>
        /// <returns></returns>
        public bool CanManageContent(Guid role1, Guid role2)
        {
            PermissionLimit limit = AllSettings.Current.PermissionSettings.ContentPermissionLimit;
            return CanManage(GetRole(role1), GetRole(role2), limit);
        }

        private  bool CanManage(Role role1, Role role2, PermissionLimit limit)
        {
            if((role1.Type & RoleType.Manager)!= RoleType.Manager ) return false; //组1 不是管理员， 默认返回false
            if ((role2.Type & RoleType.Manager) != RoleType.Manager) return true; //组2 不是管理员， 默认返回true
            switch (limit.LimitType)
            {
                case PermissionLimitType.Unlimited:
                    return true;
                case PermissionLimitType.RoleLevelLowerMe:
                    if (role1.Level > role2.Level) 
                        return true;
                    break;
                case PermissionLimitType.RoleLevelLowerOrSameMe:
                    if (role1.Level >= role2.Level) 
                        return true;
                    break;
                case PermissionLimitType.ExcludeCustomRoles:
                    if (limit.ExcludeRoles.ContainsKey(role1.RoleID))
                    {
                        List<Guid> r = limit.ExcludeRoles[role1.RoleID];
                        return r == null ? true : !r.Contains(role2.RoleID);
                    }
                    else
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}