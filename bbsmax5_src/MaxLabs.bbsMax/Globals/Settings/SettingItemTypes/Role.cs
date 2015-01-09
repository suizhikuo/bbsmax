//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 用户组
    /// </summary>
    public class Role : SettingBase, IPrimaryKey<Guid>, IBatchSave, ICloneable<Role>, IComparable<Role>
    {
        private const string EmptyColor = "#000000";

        public Role(DataReaderWrap wrap)
        {
            RID = wrap.Get<int>("RoleID");
            Name = wrap.Get<string>("Name");
            Title = wrap.Get<string>("Title");
            Color = wrap.Get<string>("Color");
            IconUrlSrc = wrap.Get<string>("IconUrl");
            Type = wrap.Get<RoleType>("RoleType");
            Level = wrap.Get<int>("Level");
            StarLevel = wrap.Get<int>("StarLevel");
            Count = wrap.Get<int>("UserCount");
            RequiredPoint = wrap.Get<int>("RequiredPoint");
            CreateTime = wrap.Get<DateTime>("CreateTime");
            RoleID = Guid.NewGuid();
        }

        public Role()
        {
            Type = RoleType.Custom;
            this.RoleID = Guid.NewGuid();
            this.IsNew = true;
            this.Color = EmptyColor;
        }

        public bool IsNew { get; set; }

        static Role()
        {
            #region 初始化基本组

            Everyone = new Role();
            Everyone.RoleID = new Guid(new byte[] { 104, 44, 156, 246, 46, 116, 146, 76, 158, 11, 149, 118, 28, 31, 71, 229 });
            Everyone.Name = Lang.Role_Everyone;
            Everyone.Type = RoleType.System | RoleType.Basic | RoleType.Virtual | RoleType.Hidden;
            Everyone.Level = 0;
            Everyone.IsNew = false;
            Everyone.IdentityID = 1;

            Guests = new Role();
            Guests.RoleID = new Guid(new byte[] { 252, 205, 216, 0, 238, 189, 221, 72, 144, 19, 170, 27, 223, 245, 51, 176 });
            Guests.Name = Lang.Role_Guests;
            Guests.Type = RoleType.System | RoleType.Basic | RoleType.Virtual | RoleType.Hidden;
            Guests.Level = 1;
            Guests.IsNew = false;
            Guests.IdentityID = 2;

            Users = new Role();
            Users.RoleID = new Guid(new byte[] { 220, 106, 244, 148, 194, 237, 58, 70, 173, 200, 70, 142, 164, 41, 251, 221 });
            Users.Name = Lang.Role_Users;
            Users.Type = RoleType.System | RoleType.Basic | RoleType.Virtual | RoleType.Hidden;
            Users.Level = 2;
            Users.IsNew = false;
            Users.IdentityID = 3;

            //====================================================

            ForumBannedUsers = new Role();
            ForumBannedUsers.RoleID = new Guid(new byte[] { 43, 66, 0, 72, 147, 25, 20, 66, 175, 129, 67, 203, 248, 30, 228, 228 });
            ForumBannedUsers.Name = Lang.Role_BannedUsers;
            ForumBannedUsers.Type = RoleType.System | RoleType.Basic | RoleType.Virtual;
            ForumBannedUsers.Level = 3;
            ForumBannedUsers.IsNew = false;
            ForumBannedUsers.IdentityID = 4;

            FullSiteBannedUsers = new Role();
            FullSiteBannedUsers.RoleID = new Guid(new byte[] { 88, 70, 32, 131, 124, 234, 91, 65, 189, 37, 106, 28, 240, 144, 102, 114 });
            FullSiteBannedUsers.Name = Lang.Role_FullSiteBannedUsers;
            FullSiteBannedUsers.Type = RoleType.System | RoleType.Basic | RoleType.Virtual;
            FullSiteBannedUsers.Level = 4;
            FullSiteBannedUsers.IsNew = false;
            FullSiteBannedUsers.IdentityID = 5;

            NewUsers = new Role();
            NewUsers.RoleID = new Guid(new byte[] { 81, 153, 242, 26, 24, 13, 194, 64, 159, 36, 34, 17, 217, 245, 70, 236 });
            NewUsers.Name = Lang.Role_NewUsers;
            NewUsers.Title = NewUsers.Name;
            NewUsers.Type = RoleType.System | RoleType.Basic;
            NewUsers.Level = 40;
            NewUsers.IsNew = false;
            NewUsers.IdentityID = 6;

            RealnameNotProvedUsers = new Role();
            RealnameNotProvedUsers.RoleID = new Guid(new byte[] { 37, 189, 66, 236, 120, 24, 202, 69, 134, 128, 84, 230, 69, 85, 49, 67 });
            RealnameNotProvedUsers.Name = Lang.Role_RealnameNotProvedUsers;
            RealnameNotProvedUsers.Type = RoleType.System | RoleType.Basic | RoleType.Hidden;
            RealnameNotProvedUsers.Level = 50;
            RealnameNotProvedUsers.IsNew = false;
            RealnameNotProvedUsers.IdentityID = 7;

            EmailNotProvedUsers = new Role();
            EmailNotProvedUsers.RoleID = new Guid(new byte[] { 83, 132, 139, 143, 107, 144, 86, 79, 179, 232, 84, 163, 135, 123, 190, 136 });
            EmailNotProvedUsers.Name = Lang.Role_EmailNotProvedUsers;
            EmailNotProvedUsers.Type = RoleType.System | RoleType.Basic | RoleType.Hidden;
            EmailNotProvedUsers.Level = 70;
            EmailNotProvedUsers.IsNew = false;
            EmailNotProvedUsers.IdentityID = 9;

            #endregion

            #region 初始化等级组

            NoLevel = new Role();
            NoLevel.RoleID = new Guid(new byte[] { 47, 190, 161, 152, 194, 17, 104, 72, 165, 4, 100, 128, 61, 230, 217, 112 });
            NoLevel.Type = RoleType.Level | RoleType.System | RoleType.Virtual;
            NoLevel.RequiredPoint = int.MinValue;
            NoLevel.Name = Lang.Role_Beggar;
            NoLevel.Title = NoLevel.Name;
            NoLevel.IsNew = false;
            NoLevel.IdentityID = 10;
            #endregion

            #region 初始化管理组

            JackarooModerators = new Role();
            JackarooModerators.RoleID = new Guid(new byte[] { 163, 31, 150, 148, 110, 243, 26, 73, 137, 33, 20, 114, 233, 226, 212, 87 });
            JackarooModerators.Name = Lang.Role_JackarooModerators;
            JackarooModerators.Title = JackarooModerators.Name;
            JackarooModerators.Type = RoleType.System | RoleType.Manager | RoleType.Virtual;
            JackarooModerators.Level = 940;
            JackarooModerators.IsNew = false;
            JackarooModerators.IdentityID = 11;

            Moderators = new Role();
            Moderators.RoleID = new Guid(new byte[] { 153, 180, 63, 86, 182, 58, 186, 74, 186, 102, 155, 26, 57, 156, 10, 33 });
            Moderators.Name = Lang.Role_Moderators;
            Moderators.Title = Moderators.Name;
            Moderators.Type = RoleType.System | RoleType.Manager | RoleType.Virtual;
            Moderators.Level = 950;
            Moderators.IsNew = false;
            Moderators.IdentityID = 12;

            CategoryModerators = new Role();
            CategoryModerators.RoleID = new Guid(new byte[] { 107, 10, 99, 29, 105, 178, 210, 78, 176, 81, 162, 223, 201, 115, 236, 157 });
            CategoryModerators.Name = Lang.Role_CategoryModerators;
            CategoryModerators.Type = RoleType.System | RoleType.Manager | RoleType.Virtual;
            CategoryModerators.Title = CategoryModerators.Name;
            CategoryModerators.Level = 960;
            CategoryModerators.IsNew = false;
            CategoryModerators.IdentityID = 13;

            SuperModerators = new Role();
            SuperModerators.RoleID = new Guid(new byte[] { 119, 226, 215, 26, 18, 166, 159, 74, 146, 70, 128, 168, 222, 175, 27, 107 });
            SuperModerators.Name = Lang.Role_SuperModerators;
            SuperModerators.Type = RoleType.System | RoleType.Manager;
            SuperModerators.Level = 970;
            SuperModerators.CanLoginConsole = true;
            SuperModerators.Title = SuperModerators.Name;
            SuperModerators.StarLevel = 16 * 4 * 1;
            SuperModerators.IsNew = false;
            SuperModerators.IdentityID = 14;

            Administrators = new Role();
            Administrators.RoleID = new Guid(new byte[] { 249, 18, 90, 112, 221, 213, 119, 77, 128, 18, 237, 12, 96, 96, 148, 202 });
            Administrators.Name = Lang.Role_Administrators;
            Administrators.Type = RoleType.System | RoleType.Manager;
            Administrators.Level = 980;
            Administrators.Title = Administrators.Name;
            Administrators.CanLoginConsole = true;
            Administrators.StarLevel = 16 * 4 * 2;
            Administrators.IsNew = false;
            Administrators.IdentityID = 15;

            Owners = new Role();
            Owners.RoleID = new Guid(new byte[] { 94, 160, 12, 219, 7, 193, 242, 64, 165, 45, 13, 72, 107, 93, 108, 176 });
            Owners.Name = Lang.Role_Owner;
            Owners.Title = Owners.Name;
            Owners.Type = RoleType.System | RoleType.Manager;
            Owners.Level = 990;
            Owners.StarLevel = 16 * 4 * 3;
            Owners.IsNew = false;
            Owners.IdentityID = 16;
            #endregion

        }

        /// <summary>
        /// 用户组优先级
        /// </summary>
        private int Priority
        {
            get
            {
                if (IsManager)
                    return 7;

                else if (IsNormal)
                    return 5;

                else if (IsLevel)
                    return 3;

                else
                    return 1;
            }
        }

        /// <summary>
        /// 唯一标志
        /// </summary>

        public int RID { get; set; }

        public int Count { get; set; }

        public DateTime CreateTime { get; set; }

        [SettingItem]
        public int IdentityID { get; set; }

        [SettingItem]
        public int MaxIdentityID { get; set; }

        [SettingItem]
        public Guid RoleID { get; set; }

        private bool m_loginConsole;
        /// <summary>
        /// 可否登录控制台
        /// </summary>
        [SettingItem]
        public bool CanLoginConsole
        {
            get
            {
                if (this.RoleID == Owners.RoleID)
                    return true;

                return m_loginConsole;
            }
            set
            {
                m_loginConsole = value;
            }
        }

        /// <summary>
        /// 用户组名称
        /// </summary>
        [SettingItem]
        public string Name { get; set; }

        /// <summary>
        /// 用户组头衔
        /// </summary>
        private string m_title = string.Empty;

        [SettingItem]
        public string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }

        /// <summary>
        /// 用户组显示用的颜色
        /// </summary>
        [SettingItem]
        public string Color { get; set; }

        /// <summary>
        /// 用户组图标URL
        /// </summary>
        [SettingItem]
        public string IconUrlSrc { get; set; }

        public string IconUrl
        {
            get { return UrlUtil.ResolveUrl(this.IconUrlSrc); }
        }

        /// <summary>
        /// 级别，级别越高值越大
        /// </summary>
        [SettingItem]
        public int Level { get; set; }

        /// <summary>
        /// 星级
        /// </summary>
        [SettingItem]
        public int StarLevel { get; set; }

        /// <summary>
        /// 所需的点数
        /// </summary>
        [SettingItem]
        public int RequiredPoint { get; set; }

        /// <summary>
        /// 用户组类型
        /// </summary>
        [SettingItem]
        public RoleType Type { get; set; }

        public Guid ID
        {
            get { return RoleID; }
        }

        #region 内置的用户组

        #region 基本组

        /// <summary>
        /// 任何人
        /// </summary>
        public static Role Everyone { get; private set; }

        /// <summary>
        /// 游客
        /// </summary>
        public static Role Guests { get; private set; }

        /// <summary>
        /// 注册用户
        /// </summary>
        public static Role Users { get; private set; }

        /// <summary>
        /// 见习用户
        /// </summary>
        public static Role NewUsers { get; private set; }

        /// <summary>
        /// 版块屏蔽用户
        /// </summary>
        public static Role ForumBannedUsers { get; private set; }

        /// <summary>
        /// 整站屏蔽用户
        /// </summary>
        public static Role FullSiteBannedUsers { get; private set; }

        /// <summary>
        /// 未通过实名认证的用户
        /// </summary>
        public static Role RealnameNotProvedUsers { get; private set; }

        /// <summary>
        /// Email未认证的用户
        /// </summary>
        public static Role EmailNotProvedUsers { get; private set; }

        ///// <summary>
        ///// 未通过邀请码验证的用户
        ///// </summary>
        //public static Role InviteLessUsers { get; private set; }


        #endregion

        #region 等级组

        /// <summary>
        /// 默认等级用户组
        /// </summary>
        public static Role NoLevel { get; private set; }

        #endregion

        #region 管理组

        /// <summary>
        /// 版主
        /// </summary>
        public static Role Moderators { get; private set; }

        /// <summary>
        /// 分类版主
        /// </summary>
        public static Role CategoryModerators { get; private set; }

        /// <summary>
        /// 超级版主
        /// </summary>
        public static Role SuperModerators { get; private set; }

        /// <summary>
        /// 实习版主
        /// </summary>
        public static Role JackarooModerators { get; private set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public static Role Administrators { get; private set; }

        /// <summary>
        /// 创始人
        /// </summary>
        public static Role Owners { get; private set; }

        #endregion

        #endregion

        #region 扩展属性


        public int TotalMembers { get { return 0; } }

        public bool IsSystem { get { return (this.Type & RoleType.System) == RoleType.System; } }

        public bool IsBasic { get { return (this.Type & RoleType.Basic) == RoleType.Basic; } }

        /// <summary>
        /// 是否等级组
        /// </summary>
        public bool IsLevel { get { return (this.Type & RoleType.Level) == RoleType.Level; } }

        /// <summary>
        /// 是否自定义组
        /// </summary>
        public bool IsNormal { get { return (this.Type & RoleType.Normal) == RoleType.Normal; } }

        /// <summary>
        /// 是否管理组
        /// </summary>
        public bool IsManager { get { return (this.Type & RoleType.Manager) == RoleType.Manager; } }



        /// <summary>
        /// 是否虚拟组
        /// </summary>
        public bool IsVirtualRole { get { return (this.Type & RoleType.Virtual) == RoleType.Virtual; } }

        /// <summary>
        /// 在前台是否隐藏
        /// </summary>
        public bool IsHidden { get { return (this.Type & RoleType.Hidden) == RoleType.Hidden; } }



        #endregion

        #region 模版使用的扩展属性
        /// <summary>
        /// 用户组类型的名称
        /// </summary>
        public string TypeName
        {
            get
            {

                if ((this.Type & RoleType.System) == RoleType.System)
                    return Lang.RoleType_System;

                else if ((this.Type & RoleType.Level) == RoleType.Level)
                    return Lang.RoleType_Level;

                return Lang.RoleType_Custom;
            }
        }


        /// <summary>
        /// 当前用户是否能够删除当前用户组实例
        /// </summary>
        public bool CanDelete
        {
            get
            {
                if ((Type & RoleType.System) == RoleType.System)
                    return false;
                else
                    return true;
            }
        }

        public bool CanEditMembers
        {
            get
            {
                return true;
            }
        }

        public bool CanEditPermissions
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region IPrimaryKey<Guid> 成员

        public Guid GetKey()
        {
            return RoleID;
        }

        #endregion

        #region 重载Equals 和 ==

        public override bool Equals(object obj)
        {
            return RoleID.Equals(obj);
        }

        public override int GetHashCode()
        {
            return RoleID.GetHashCode();
        }

        /// <summary>
        /// 重载==
        /// </summary>
        public static bool operator ==(Role group1, Role group2)
        {
            if (group1 as object == null)
                return group2 as object == null;

            if (group2 as object == null)
                return group1 as object == null;

            return group1.RoleID == group2.RoleID;
        }

        /// <summary>
        /// 重载!=
        /// </summary>
        public static bool operator !=(Role group1, Role group2)
        {
            if (group1 as object == null)
                return group2 as object != null;

            if (group2 as object == null)
                return group1 as object != null;

            return group1.RoleID != group2.RoleID;
        }

        #endregion

        #region ICloneable 成员

        public Role Clone()
        {
            Role r = new Role();

            r.Type = this.Type;
            r.Title = this.Title;
            r.Name = this.Name;
            r.RequiredPoint = this.RequiredPoint;
            r.StarLevel = this.StarLevel;
            r.Level = this.Level;
            r.IsNew = this.IsNew;
            r.IconUrlSrc = this.IconUrlSrc;
            r.Color = this.Color;
            r.RoleID = this.RoleID;
            r.CanLoginConsole = this.CanLoginConsole;

            return r;
        }

        #endregion

        #region IComparable<Role> 成员

        public int CompareTo(Role other)
        {
            if (other == null)
                return 1;

            //不是同一种类型的，直接可以比较出大小
            if (other.Priority > this.Priority)
                return -1;

            else if (other.Priority < this.Priority)
                return 1;

            //同一种类型

            //都是等级组，判断需要的积分
            else if (other.IsLevel)
                return this.RequiredPoint.CompareTo(other.RequiredPoint);

            else
                return this.Level.CompareTo(other.Level);

        }

        public static bool operator >(Role role1, Role role2)
        {
            //两个用户组相等或者role1是null，那都不可能role1比role2大
            if (role1 == role2 || role1 == null)
                return false;

            return role1.CompareTo(role2) > 0;
        }

        public static bool operator <(Role role1, Role role2)
        {
            //两个用户组相等或者role2是null，那都不可能role2比role1大
            if (role1 == role2 || role2 == null)
                return false;

            return role1.CompareTo(role2) < 0;
        }


        public static bool operator >=(Role role1, Role role2)
        {
            if (role1 == null)
            {
                if (role2 == null)
                    return true;
                else
                    return false;
            }

            return role1.CompareTo(role2) >= 0;
        }

        public static bool operator <=(Role role1, Role role2)
        {
            if (role1 == null)
            {
                return true;
            }
            return role1.CompareTo(role2) <= 0;
        }

        #endregion

        //创建
        #region 创建指定类型的空用户组 CreateLevelRole / CreateManagerRole / CreateNormalRole

        public static Role CreateLevelRole()
        {
            Role r = new Role();
            r.Type = RoleType.Level | RoleType.Custom | RoleType.Virtual;
            return r;
        }

        public static Role CreateManagerRole()
        {
            Role r = new Role();
            r.Type = RoleType.Manager | RoleType.Custom;
            return r;
        }

        public static Role CreateNormalRole()
        {
            Role r = new Role();
            r.Type = RoleType.Normal | RoleType.Custom;
            return r;
        }

        #endregion
    }

    /// <summary>
    /// 用户组对象集合
    /// </summary>
    public class RoleCollection : HashedCollectionBase<Guid, Role>, ISettingItem
    {

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (Role item in this)
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
                List<Guid> removeRoleIds = new List<Guid>();
                foreach (Role role in this)
                {
                    if (role.IsLevel && role != Role.NoLevel)
                    {
                        removeRoleIds.Add(role.RoleID);
                    }
                }

                if (removeRoleIds.Count > 0)
                {
                    foreach (Guid removeRoleID in removeRoleIds)
                        this.RemoveByKey(removeRoleID);
                }

                foreach (string item in list)
                {
                    Role field = new Role();
                    field.IsNew = false;
                    field.Parse(item);

                    bool needReset = true;

                    Role oldRole;
                    if (this.TryGetValue(field.RoleID, out oldRole))
                    {
                        if (oldRole.IsSystem)
                        {
                            oldRole.Title = field.Title;
                            oldRole.StarLevel = field.StarLevel;
                            oldRole.Name = field.Name;
                            oldRole.Level = field.Level;
                            oldRole.IconUrlSrc = field.IconUrlSrc;
                            oldRole.Color = field.Color;
                            oldRole.CanLoginConsole = field.CanLoginConsole;

                            needReset = false;
                        }
                    }

                    if (needReset)
                        this.Set(field);
                }
            }

            this.Sort();
        }

        public RoleCollection() { }

        public RoleCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                Add(new Role(wrap));
            }
        }

        #endregion
    }

    /// <summary>
    /// 用户组类型
    /// </summary>
    public enum RoleType : byte
    {
        /// <summary>
        /// 自定义用户组
        /// </summary>
        Custom = 0x1,

        /// <summary>
        /// 系统用户组
        /// </summary>
        System = 0x2,


        //4种类型开始----------------------

        /// <summary>
        /// 基本组
        /// </summary>
        Basic = 0x4,

        /// <summary>
        /// 等级组
        /// </summary>
        Level = 0x8,

        /// <summary>
        /// 普通组(自定义组)
        /// </summary>
        Normal = 0x10,

        /// <summary>
        /// 管理组
        /// </summary>
        Manager = 0x20,

        //4种类型结束----------------------


        /// <summary>
        /// 虚拟用户组
        /// </summary>
        Virtual = 0x40,

        /// <summary>
        /// 隐藏
        /// </summary>
        Hidden = 0x80
    }

    public delegate bool RoleResolver(Role role);
}