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
using System.Reflection;

namespace MaxLabs.bbsMax.Settings
{
    public class Permission<TA1, TA2>
        where TA1 : struct
        where TA2 : struct
    {

        private byte[] m_PermissionItems_ta1;
        private byte[] m_PermissionItems_ta2;

        private bool[] m_DisabledPermissionItems_ta1;
        private bool[] m_DisabledPermissionItems_ta2;

        public Permission()
        {
            FieldInfo[] fieldInfos = typeof(TA1).GetFields();
            FieldInfo[] fieldInfosWithTarget = typeof(TA2).GetFields();

            m_PermissionItems_ta1 = new byte[fieldInfos.Length];
            m_PermissionItems_ta2 = new byte[fieldInfosWithTarget.Length];

            m_DisabledPermissionItems_ta1 = new bool[fieldInfos.Length];
            m_DisabledPermissionItems_ta2 = new bool[fieldInfosWithTarget.Length];
        }

        internal virtual void DoParse(string text, Guid roleID)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            StringTable permissionItems = StringTable.Parse(text);

            FieldInfo[] fieldInfos = typeof(TA1).GetFields();
            FieldInfo[] fieldInfosWithTarget = typeof(TA2).GetFields();

            FieldInfo actionField;
            string actionName;

            byte[] ta1 = new byte[fieldInfos.Length];
            byte[] ta2 = new byte[fieldInfosWithTarget.Length];

            bool[] disabled_ta1 = new bool[fieldInfos.Length];
            bool[] disabled_ta2 = new bool[fieldInfosWithTarget.Length];

            int m1 = 0;
            int m2 = 0;
            for (int i = 0; i < fieldInfos.Length + fieldInfosWithTarget.Length; i ++ )
            {
                bool isTa1 = false;
                if (i < fieldInfos.Length)
                {
                    isTa1 = true;
                    actionField = fieldInfos[i];
                    actionName = actionField.Name;
                }
                else
                {
                    actionField = fieldInfosWithTarget[i - fieldInfos.Length];
                    actionName = "?" + actionField.Name;
                }

                if (actionField.IsDefined(typeof(PermissionItemAttribute), false))
                {

                    bool isAllow;
                    bool isDeny;
                    bool editable;

                    GetPermissionItemAttributeSet(roleID, actionField, out isAllow, out isDeny, out editable);

                    if (editable && permissionItems.ContainsKey(actionName))
                    {
                        string permissionText = permissionItems[actionName];

                        if (isTa1)
                        {
                            switch (permissionText)
                            {
                                case "2": ta1[m1] = 2; break;
                                case "1": ta1[m1] = 1; break;
                                default: ta1[m1] = 0; break;

                            }
                        }
                        else
                        {
                            switch (permissionText)
                            {
                                case "2": ta2[m2] = 2; break;
                                case "1": ta2[m2] = 1; break;
                                default: ta2[m2] = 0; break;

                            }
                        }
                    }
                    else 
                    {
                        if (isTa1)
                        {
                            if (isAllow)
                                ta1[m1] = 1;
                            else if (isDeny)
                                ta1[m1] = 2;
                            else
                                ta1[m1] = 0;
                        }
                        else
                        {
                            if (isAllow)
                                ta2[m2] = 1;
                            else if (isDeny)
                                ta2[m2] = 2;
                            else
                                ta2[m2] = 0;
                        }
                    }

                    if (isTa1)
                    {
                        disabled_ta1[m1] = !editable;
                        m1++;
                    }
                    else
                    {
                        disabled_ta2[m2] = !editable;
                        m2++;
                    }
                }
            }

            m_PermissionItems_ta1 = ta1;
            m_PermissionItems_ta2 = ta2;
            m_DisabledPermissionItems_ta1 = disabled_ta1;
            m_DisabledPermissionItems_ta2 = disabled_ta2;

        }

        internal static Permission<TA1, TA2> Parse(string text, Guid roleID)
        {
            Permission<TA1, TA2> permission = new Permission<TA1, TA2>();
            permission.DoParse(text, roleID);
            return permission;
        }

        private static Dictionary<Guid, Permission<TA1, TA2>> s_DefaultPermissions = new Dictionary<Guid, Permission<TA1, TA2>>();
        private static object s_DefaultPermissionsLocker = new object();

        public static Permission<TA1, TA2> GetDefaultPermission(Guid roleID)
        {
            Permission<TA1, TA2> defaultPermission;

            if (s_DefaultPermissions.TryGetValue(roleID, out defaultPermission) == false)
            {
                lock (s_DefaultPermissionsLocker)
                {
                    if (s_DefaultPermissions.TryGetValue(roleID, out defaultPermission) == false)
                    {
                        defaultPermission = Parse(string.Empty, roleID);
                        s_DefaultPermissions.Add(roleID, defaultPermission);
                    }
                }
            }

            return defaultPermission;
        }

        public override string ToString()
        {

            StringTable permissionItems = new StringTable();

            FieldInfo[] fieldInfos = typeof(TA1).GetFields();
            FieldInfo[] fieldInfosWithTarget = typeof(TA2).GetFields();

            FieldInfo actionField;
            string actionName;

            int m1 = 0;
            int m2 = 0;
            for (int i = 0; i < fieldInfos.Length + fieldInfosWithTarget.Length; i++)
            {
                bool isTa1 = false;
                if (i < fieldInfos.Length)
                {
                    isTa1 = true;
                    actionField = fieldInfos[i];
                    actionName = actionField.Name;
                }
                else
                {
                    actionField = fieldInfosWithTarget[i - fieldInfos.Length];
                    actionName = "?" + actionField.Name;
                }

                if (actionField.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    if (isTa1)
                    {
                        permissionItems.Add(actionName, m_PermissionItems_ta1[m1].ToString());
                        m1++;
                    }
                    else
                    {
                        permissionItems.Add(actionName, m_PermissionItems_ta2[m2].ToString());
                        m2++;
                    }

                }
            }

            return permissionItems.ToString();

        }

        private const byte m_allow = 1;
        private const byte m_deny = 2;
        private const byte m_notSet = 0;

        internal void SetNotSet(TA1 action)
        {
            m_PermissionItems_ta1[(int)(object)action] = m_notSet;
        }

        internal void SetNotSet(TA2 action)
        {
            m_PermissionItems_ta2[(int)(object)action] = m_notSet;
        }

        internal void SetAllow(TA1 action)
        {
            m_PermissionItems_ta1[(int)(object)action] = m_allow;
        }

        internal void SetAllow(TA2 action)
        {
            m_PermissionItems_ta2[(int)(object)action] = m_allow;
        }

        internal bool IsAllowTA1(int actionValue)
        {
            return m_PermissionItems_ta1[actionValue] == m_allow;
        }

        internal bool IsAllowTA2(int actionValue)
        {
            return m_PermissionItems_ta2[actionValue] == m_allow;
        }

        internal void SetDeny(TA1 action)
        {
            m_PermissionItems_ta1[(int)(object)action] = m_deny;
        }

        internal void SetDeny(TA2 action)
        {
            m_PermissionItems_ta2[(int)(object)action] = m_deny;
        }


        internal void SetDisabled(TA1 action)
        {
            m_DisabledPermissionItems_ta1[(int)(object)action] = true;
        }

        internal void SetDisabled(TA2 action)
        {
            m_DisabledPermissionItems_ta2[(int)(object)action] = true;
        }

        internal bool IsDenyTA1(int actionValue)
        {
            return m_PermissionItems_ta1[actionValue] == m_deny;
        }

        internal bool IsDenyTA2(int actionValue)
        {
            return m_PermissionItems_ta2[actionValue] == m_deny;
        }

        internal bool IsDisabledTA1(int actionValue)
        {
            return m_DisabledPermissionItems_ta1[actionValue];
        }

        internal bool IsDisabledTA2(int actionValue)
        {
            return m_DisabledPermissionItems_ta2[actionValue];
        }

        public void GetPermissionItemAttributeSet(Guid roleID, FieldInfo actionField, out bool isAllow, out bool isDeny, out bool editable)
        {

            isAllow = false;
            isDeny = false;
            editable = false;

            PermissionItemAttribute permissionItemAttribute = (PermissionItemAttribute)actionField.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0];

            RoleOption roleOption;

            if (roleID == Role.Everyone.RoleID)
                roleOption = permissionItemAttribute.Everyone;

            else if (roleID == Role.Guests.RoleID)
                roleOption = permissionItemAttribute.Guests;

            else if (roleID == Role.Users.RoleID)
                roleOption = permissionItemAttribute.Users;

            else if (roleID == Role.NewUsers.RoleID)
                roleOption = permissionItemAttribute.NewUsers;

            else if (roleID == Role.RealnameNotProvedUsers.RoleID)
                roleOption = permissionItemAttribute.RealnameNotProvedUsers;

            else if (roleID == Role.EmailNotProvedUsers.RoleID)
                roleOption = permissionItemAttribute.EmailNotProvedUsers;

            else if (roleID == Role.Moderators.RoleID)
                roleOption = permissionItemAttribute.Moderators;

            else if (roleID == Role.CategoryModerators.RoleID)
                roleOption = permissionItemAttribute.CategoryModerators;

            else if (roleID == Role.SuperModerators.RoleID)
                roleOption = permissionItemAttribute.SuperModerators;

            else if (roleID == Role.Administrators.RoleID)
                roleOption = permissionItemAttribute.Administrators;

            else if (roleID == Role.Owners.RoleID)
                roleOption = permissionItemAttribute.Owners;

            else if (roleID == Role.ForumBannedUsers.RoleID)
                roleOption = permissionItemAttribute.ForumBannedUsers;

            else if (roleID == Role.FullSiteBannedUsers.RoleID)
                roleOption = permissionItemAttribute.FullSiteBannedUsers;

            else
                roleOption = RoleOption.DefaultNotset;

            switch (roleOption)
            {
                case RoleOption.AlwaysAllow:
                    isAllow = true;
                    editable = false;
                    break;

                case RoleOption.AlwaysDeny:
                    isDeny = true;
                    editable = false;
                    break;

                case RoleOption.AlwaysNotset:
                    editable = false;
                    break;

                case RoleOption.DefaultAllow:
                    isAllow = true;
                    editable = true;
                    break;

                case RoleOption.DefaultDeny:
                    isDeny = true;
                    editable = true;
                    break;

                case RoleOption.DefaultNotset:
                    editable = true;
                    break;
            }
        }
    }

    /*

    public class PermissionManager
    {
        public static void GetPermissionItemAttributeSet(Guid roleID, FieldInfo actionField, out bool isAllow, out bool isDeny, out bool editable)
        {

            isAllow = false;
            isDeny = false;
            editable = false;

            PermissionItemAttribute permissionItemAttribute = (PermissionItemAttribute)actionField.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0];

            RoleOption roleOption;

            if (roleID == Role.Everyone.RoleID)
                roleOption = permissionItemAttribute.Everyone;

            else if (roleID == Role.Guests.RoleID)
                roleOption = permissionItemAttribute.Guests;

            else if (roleID == Role.Users.RoleID)
                roleOption = permissionItemAttribute.Users;

            else if (roleID == Role.NewUsers.RoleID)
                roleOption = permissionItemAttribute.NewUsers;

            else if (roleID == Role.RealnameNotProvedUsers.RoleID)
                roleOption = permissionItemAttribute.RealnameNotProvedUsers;

            //else if (roleID == Role.InviteLessUsers.RoleID)
            //    roleOption = permissionItemAttribute.InviteLessUsers;

            else if (roleID == Role.EmailNotProvedUsers.RoleID)
                roleOption = permissionItemAttribute.EmailNotProvedUsers;

            else if (roleID == Role.ForumBannedUsers.RoleID)
                roleOption = permissionItemAttribute.BannedUsers;

            else if (roleID == Role.AvatarNotProvedUsers.RoleID)
                roleOption = permissionItemAttribute.AvatarNotProvedUsers;

            else if (roleID == Role.Moderators.RoleID)
                roleOption = permissionItemAttribute.Moderators;

            else if (roleID == Role.CategoryModerators.RoleID)
                roleOption = permissionItemAttribute.CategoryModerators;

            else if (roleID == Role.SuperModerators.RoleID)
                roleOption = permissionItemAttribute.SuperModerators;

            else if (roleID == Role.Administrators.RoleID)
                roleOption = permissionItemAttribute.Administrators;

            else if (roleID == Role.Owners.RoleID)
                roleOption = permissionItemAttribute.Owners;

            else
                roleOption = RoleOption.DefaultNotset;

            switch (roleOption)
            {
                case RoleOption.AlwaysAllow:
                    isAllow = true;
                    editable = false;
                    break;

                case RoleOption.AlwaysDeny:
                    isDeny = true;
                    editable = false;
                    break;

                case RoleOption.AlwaysNotset:
                    editable = false;
                    break;

                case RoleOption.DefaultAllow:
                    isAllow = true;
                    editable = true;
                    break;

                case RoleOption.DefaultDeny:
                    isDeny = true;
                    editable = true;
                    break;

                case RoleOption.DefaultNotset:
                    editable = true;
                    break;
            }
        }
    }

    */
    #region Class PermissionCollection

    public class PermissionCollection<TA1, TA2> : Dictionary<Guid, Permission<TA1, TA2>>, ISettingItem
        where TA1 : struct
        where TA2 : struct
    {
        public static PermissionTargetType[] s_PermissionTargetTypes = null;//new Dictionary<int, PermissionTargetType>();

        static PermissionCollection()
        {
            #region 读取 s_PermissionTargetTypes

            PermissionTargetType? defaultTargetType = null;

            //读取默认的PermissionTargetType，如果每个枚举没有具体指定PermissionTargetType，那么将用这个值
            if (typeof(TA2).IsDefined(typeof(PermissionTargetAttribute), false))
            {
                PermissionTargetAttribute permissionTargetAttribute = (PermissionTargetAttribute)typeof(TA2).GetCustomAttributes(typeof(PermissionTargetAttribute), false)[0];

                defaultTargetType = permissionTargetAttribute.TargetType;
            }

            //得到所有枚举值

            FieldInfo[] fieldInfosWithTarget = typeof(TA2).GetFields();

            PermissionTargetType[] permissionTargetTypes = new PermissionTargetType[fieldInfosWithTarget.Length];

            int i = 0;
            foreach (FieldInfo fieldInfo in fieldInfosWithTarget)
            {
                if (fieldInfo.IsDefined(typeof(PermissionTargetAttribute), false))
                {
                    PermissionTargetAttribute permissionTargetAttribute = (PermissionTargetAttribute)typeof(TA2).GetCustomAttributes(typeof(PermissionTargetAttribute), false)[0];

                    permissionTargetTypes[i] = permissionTargetAttribute.TargetType;

                    i++;
                }

                else if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    if (defaultTargetType != null)
                        permissionTargetTypes[i] = defaultTargetType.Value;

                    else
                        throw new Exception(typeof(TA2).FullName + "或者" + fieldInfo.Name + "没有标记PermissionTarget，这将导致bbsmax无法确定这个权限判断的目标");

                    i++;
                }
                
            }

            s_PermissionTargetTypes = permissionTargetTypes;

            #endregion
        }

        public PermissionTargetType GetPermissionTargetType(int actionIndex)
        {
            return s_PermissionTargetTypes[actionIndex];
        }


        #region ISettingItem 成员

        public string GetValue()
        {
            StringTable table = new StringTable();

            foreach (KeyValuePair<Guid, Permission<TA1, TA2>> pair in this)
            {
                table.Add(pair.Key.ToString(), pair.Value.ToString());
            }

            return table.ToString();
        }

        public void SetValue(string value)
        {
            StringTable table = StringTable.Parse(value);

            if (table != null)
            {
                Clear();

                foreach (DictionaryEntry item in table)
                {
                    Guid roleId = new Guid(item.Key.ToString());
                    Permission<TA1, TA2> permission = Permission<TA1, TA2>.Parse(item.Value.ToString(), roleId);

                    this.Add(roleId, permission);
                }
            }
        }

        #endregion

        public Permission<TA1, TA2> AlwaysGetPermission(Guid roleID)
        {
            Permission<TA1, TA2> permission;

            if (TryGetValue(roleID, out permission) == false)
                permission = Permission<TA1, TA2>.GetDefaultPermission(roleID);

            return permission;
        }
    }

    #endregion
}