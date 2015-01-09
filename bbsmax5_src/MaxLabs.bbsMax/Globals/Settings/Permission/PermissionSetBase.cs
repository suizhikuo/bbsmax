//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public abstract class PermissionSetBase<TA1, TA2> : SettingBase, IPermissionSet
        where TA1 : struct
        where TA2 : struct
    {

        public PermissionSetBase()
        {
            Permissions = new PermissionCollection<TA1, TA2>();
        }

        //public abstract PermissionSetWithTargetType PermissionSetWithTargetType { get; }

        public abstract string Name { get; }

        public virtual bool IsManagement { get { return false; } }

        public virtual bool CanSetDeny { get { return true; } }

        public virtual int GetActionValue(TA1 action)
        {
            return (int)(object)action;
        }

        public virtual int GetActionValue(TA2 actionWithTarget)
        {
            return (int)(object)actionWithTarget;
        }


        public string TypeName { get { return this.GetType().Name; } }

        private string m_NodesTypeName;
        public string NodesTypeName { get { return m_NodesTypeName; } set { m_NodesTypeName = value; } }

        [SettingItem]
        public PermissionCollection<TA1, TA2> Permissions { get; set; }


        #region 用于权限判断的方法

        /// <summary>
        /// 在检查权限之前的检查，如果返回false将直接认为没有权限，如果返回true将继续正常的权限检查
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual bool BeforePermissionCheck(User operatorUser, Guid roleID, TA1 action)
        {
            return true;
        }

        /// <summary>
        /// 在检查权限之前的检查，如果返回false将直接认为没有权限，如果返回true将继续正常的权限检查
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual bool BeforePermissionCheck(User operatorUser, Guid roleID, TA2 action)
        {
            return true;
        }

        //======================================================================================

        /// <summary>
        /// 判断指定用户是否拥有某个动作的权限
        /// </summary>
        /// <param name="my">操作者</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Can(User my, TA1 action)
        {

            //如果是创始人，直接返回true
            if (my.IsOwner)
                return true;

            //管理类权限而我不是管理员
            if (IsManagement && my.IsManager == false)
                return false;

            Permission<TA1, TA2> permission;

            bool allow = false;
            bool beforeCheck;

            int actionIndex = GetActionValue(action);

            foreach (UserRole userRole in my.Roles)
            {
                //不是管理员
                if (IsManagement && userRole.Role.IsManager == false)
                    continue;

                permission = Permissions.AlwaysGetPermission(userRole.RoleID);
                beforeCheck = BeforePermissionCheck(my, userRole.RoleID, action);

                if (CanSetDeny && beforeCheck && permission.IsDenyTA1(actionIndex))
                    return false;

                if (beforeCheck == false)
                    continue;

                if (allow == false && permission.IsAllowTA1(actionIndex))
                {
                    if (CanSetDeny)
                        allow = true;
                    else
                        return true;
                }

            }

            return allow;

        }

        #region 已过时的、传入MyUserID的Can
        /// <summary>
        /// 判断指定用户是否拥有某个动作的权限
        /// </summary>
        /// <param name="operatorUserID">操作者的Id</param>
        /// <param name="action">操作的动作</param>
        /// <returns></returns>
        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public bool Can(int operatorUserID, TA1 action)
        {

            User user = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithGuest);

            if (user == null)
                return false;

            return Can(user, action);
        }

        #endregion

        //======================================================================================

        public bool Can(User my, TA2 action, int targetUserID, int lastEditUserID)
        {
            NoPermissionType reason;
            return Can(my, action, targetUserID, lastEditUserID, out reason);
        }

        #region 已过时的、传入MyUserID的Can

        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public bool Can(int operatorUserID, TA2 action, int targetUserID, int lastEditUserID)
        {
            NoPermissionType reason;
            return Can(operatorUserID, action, targetUserID, lastEditUserID, out reason);
        }

        #endregion

        //======================================================================================

        public bool Can(User my, TA2 action, int targetUserID, int lastEditUserID, out NoPermissionType reason)
        {
            bool result;

            //目标从未修改，或者目标被修改过，且是本人修改的
            if (targetUserID == lastEditUserID || lastEditUserID == my.UserID)
            {
                result = Can(my, action, targetUserID, out reason);

            }
            //目标被修改过，且不是本人修改的
            else
            {
                NoPermissionType tempReason;

                result = Can(my, action, lastEditUserID, out tempReason);

                if (result == false)
                {
                    if (tempReason == NoPermissionType.NoPermission)
                        reason = NoPermissionType.NoPermission;
                    else
                        reason = NoPermissionType.NoPermissionForLastEditor;
                }
                else
                    reason = NoPermissionType.NoPermission;
            }

            return result;
        }

        #region 已过时的、传入MyUserID的Can

        /// <summary>
        /// 如果 有最后编辑者 使用该方法判断 是否有权限
        /// </summary>
        /// <returns></returns>
        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public bool Can(int operatorUserID, TA2 action, int targetUserID, int lastEditUserID, out NoPermissionType reason)
        {

            bool result;

            //目标从未修改，或者目标被修改过，且是本人修改的
            if (targetUserID == lastEditUserID || lastEditUserID == operatorUserID)
            {
                result = Can(operatorUserID, action, targetUserID, out reason);

            }
            //目标被修改过，且不是本人修改的
            else
            {
                NoPermissionType tempReason;

                result = Can(operatorUserID, action, lastEditUserID, out tempReason);

                if (result == false)
                {
                    if (tempReason == NoPermissionType.NoPermission)
                        reason = NoPermissionType.NoPermission;
                    else
                        reason = NoPermissionType.NoPermissionForLastEditor;
                }
                else
                    reason = NoPermissionType.NoPermission;
            }

            return result;
        }

        #endregion

        //======================================================================================

        public bool Can(User my, TA2 action, int targetUserID)
        {
            NoPermissionType reason;
            return Can(my, action, targetUserID, out reason);
        }

        #region 已过时的、传入MyUserID的Can

        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public bool Can(int operatorUserID, TA2 action, int targetUserID)
        {
            NoPermissionType reason;
            return Can(operatorUserID, action, targetUserID, out reason);
        }

        #endregion

        //======================================================================================

        /// <summary>
        /// 判断指定用户对指定目标用户是否拥有某个动作的权限
        /// </summary>
        /// <param name="my">当前用户</param>
        /// <param name="action">操作的动作</param>
        /// <param name="targetUserId">操作的目标用户</param>
        /// <returns></returns>
        public bool Can(User my, TA2 action, int targetUserID, out NoPermissionType reason)
        {
            reason = NoPermissionType.NoPermission;   

            //如果是创始人，直接返回true
            if (my.IsOwner)
                return true;

            //管理类权限而我不是管理员
            if (IsManagement && my.IsManager == false)
                return false;

            int actionIndex = GetActionValue(action);

            #region 检查权限的逻辑

            Permission<TA1, TA2> permission;

            PermissionLimit limit;

            switch (Permissions.GetPermissionTargetType(actionIndex))
            {
                case PermissionTargetType.Content:
                    limit = AllSettings.Current.PermissionSettings.ContentPermissionLimit;
                    break;

                case PermissionTargetType.User:
                    limit = AllSettings.Current.PermissionSettings.UserPermissionLimit;
                    break;

                default:
                    throw new NotSupportedException("Action with target must defined 'PermissionSetWithTargetType'");
            }


            User targetUser = null;
            Role operatorMaxRole = Role.Everyone;
            Role targetMaxRole = Role.Everyone;

            if (limit.LimitType != PermissionLimitType.Unlimited)
            {
                targetUser = UserBO.Instance.GetUser(targetUserID, GetUserOption.WithAll);

                if (limit.LimitType != PermissionLimitType.ExcludeCustomRoles)
                {
                    targetMaxRole = targetUser.MaxRole;

                    operatorMaxRole = my.MaxRole;
                }

            }

            bool allow = false;

            foreach (UserRole userRole in my.Roles)
            {
                //不是管理员
                if (IsManagement && userRole.Role.IsManager == false)
                    continue;

                permission = Permissions.AlwaysGetPermission(userRole.RoleID);

                if (CanSetDeny && permission.IsDenyTA2(actionIndex))
                {
                    reason = NoPermissionType.NoPermission;
                    return false;
                }

                if (false == BeforePermissionCheck(my, userRole.RoleID, action))
                    continue;

                if (allow == false)
                {
                    if (permission.IsAllowTA2(actionIndex))
                    {
                        if (limit.LimitType == PermissionLimitType.Unlimited)
                        {
                            allow = true;
                        }
                        else if (limit.LimitType == PermissionLimitType.RoleLevelLowerMe)
                        {
                            if (operatorMaxRole > targetMaxRole)
                                allow = true;
                        }
                        else if (limit.LimitType == PermissionLimitType.RoleLevelLowerOrSameMe)
                        {
                            if (operatorMaxRole >= targetMaxRole)
                                allow = true;
                        }
                        else if (limit.LimitType == PermissionLimitType.ExcludeCustomRoles)
                        {
                            bool tempAllow = true;

                            List<Guid> excludeRoleIds;

                            if (limit.ExcludeRoles.TryGetValue(userRole.RoleID, out excludeRoleIds))
                            {
                                if (excludeRoleIds != null)
                                {
                                    foreach (UserRole targetUserRole in targetUser.Roles)
                                    {
                                        //不能管理目标用户
                                        if (excludeRoleIds.Contains(targetUserRole.RoleID))
                                        {
                                            tempAllow = false;
                                            break;
                                        }
                                    }
                                }

                            }

                            if (tempAllow == true)
                                allow = true;
                            else
                            {
                                reason = NoPermissionType.NoPermissionForTargetUser;
                                continue;
                            }
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        if (allow == false)
                        {
                            reason = NoPermissionType.NoPermissionForTargetUser;
                            return false;
                        }
                        //有权限，且对这个用户组也有权限，且本类型的权限没有“禁止”的情况，那么可以立即返回true
                        else if (this.CanSetDeny == false)
                            return true;
                    }
                }
            }

            #endregion

            if (allow)
                reason = NoPermissionType.NoPermission;

            return allow;
        }

        #region 已过时的、传入MyUserID的Can

        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public bool Can(int operatorUserID, TA2 action, int targetUserID, out NoPermissionType reason)
        {
            User user = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithGuest);

            if (user == null)
            {
                reason = NoPermissionType.NoPermission;
                return false;
            }

            return Can(user, action, targetUserID, out reason);
        }

        #endregion

        public bool Can(User my, TA2 action, Role targetRole)
        {
            NoPermissionType reason;
            return Can(my, action, targetRole, out reason);
        }

        public bool Can(User my, TA2 action, Role targetRole, out NoPermissionType reason)
        {
            reason = NoPermissionType.NoPermission;

            //如果是创始人，直接返回true
            if (my.IsOwner)
                return true;

            //管理类权限而我不是管理员
            if (IsManagement && my.IsManager == false)
                return false;

            int actionIndex = GetActionValue(action);

            #region 检查权限的逻辑

            Permission<TA1, TA2> permission;

            PermissionLimit limit;

            switch (Permissions.GetPermissionTargetType(actionIndex))
            {
                case PermissionTargetType.Content:
                    limit = AllSettings.Current.PermissionSettings.ContentPermissionLimit;
                    break;

                case PermissionTargetType.User:
                    limit = AllSettings.Current.PermissionSettings.UserPermissionLimit;
                    break;

                default:
                    throw new NotSupportedException("Action with target must defined 'PermissionSetWithTargetType'");

            }

            Role operatorMaxRole = Role.Everyone;

            if (limit.LimitType != PermissionLimitType.Unlimited)
            {
                if (limit.LimitType != PermissionLimitType.ExcludeCustomRoles)
                {
                    operatorMaxRole = my.MaxRole;
                }

            }

            bool allow = false;
            bool beforeCheck;

            foreach (UserRole userRole in my.Roles)
            {
                //不是管理员
                if (IsManagement && userRole.Role.IsManager == false)
                    continue;

                permission = Permissions.AlwaysGetPermission(userRole.RoleID);
                beforeCheck = BeforePermissionCheck(my, userRole.RoleID, action);

                if (CanSetDeny && beforeCheck && permission.IsDenyTA2(actionIndex))
                {
                    reason = NoPermissionType.NoPermission;
                    return false;
                }

                if (beforeCheck == false)
                    continue;

                if (allow == false)
                {
                    if (permission.IsAllowTA2(actionIndex))
                    {
                        //有这个权限，开始判断对具体的这个用户组是否真的有权限

                        if (limit.LimitType == PermissionLimitType.Unlimited)
                        {
                            allow = true;
                        }
                        else if (limit.LimitType == PermissionLimitType.RoleLevelLowerMe)
                        {
                            if (operatorMaxRole > targetRole)
                                allow = true;
                        }
                        else if (limit.LimitType == PermissionLimitType.RoleLevelLowerOrSameMe)
                        {
                            if (operatorMaxRole >= targetRole)
                                allow = true;
                        }
                        else if (limit.LimitType == PermissionLimitType.ExcludeCustomRoles)
                        {
                            List<Guid> excludeRoleIds;

                            if (limit.ExcludeRoles.TryGetValue(userRole.RoleID, out excludeRoleIds))
                            {
                                if (excludeRoleIds != null)
                                {
                                    //不能管理目标用户
                                    if (excludeRoleIds.Contains(targetRole.RoleID))
                                    {
                                        reason = NoPermissionType.NoPermissionForTargetUser;
                                        continue;
                                    }
                                }
                            }
                            allow = true;
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                        //已经是有权限了，但对这个用户组没权限，那么可以立即返回false
                        if (allow == false)
                        {
                            reason = NoPermissionType.NoPermissionForTargetUser;
                            return false;
                        }
                        //有权限，且对这个用户组也有权限，且本类型的权限没有“禁止”的情况，那么可以立即返回true
                        else if (this.CanSetDeny == false)
                            return true;

                    }
                }
            }

            #endregion

            if (allow)
                reason = NoPermissionType.NoPermission;

            return allow;
        }

        #endregion


        #region 具有操作目标的权限项相关的方法

        /// <summary>
        /// 判断指定用户是拥有某个动作的权限（只要可能对系统中的一部分用户拥有权限，也将返回true）
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool HasPermissionForSomeone(User my, TA2 action)
        {

            //如果是创始人，直接返回true
            if (my.IsOwner)
                return true;

            //管理类权限而我不是管理员
            if (IsManagement && my.IsManager == false)
                return false;

            Permission<TA1, TA2> permission;

            bool allow = false;
            bool beforeCheck;

            int actionIndex = GetActionValue(action);

            foreach (UserRole userRole in my.Roles)
            {
                //不是管理员
                if (IsManagement && userRole.Role.IsManager == false)
                    continue;

                //if (false == BeforePermissionCheck(my, userRole.RoleID, action))
                //    continue;

                permission = Permissions.AlwaysGetPermission(userRole.RoleID);
                beforeCheck = BeforePermissionCheck(my, userRole.RoleID, action);

                if (CanSetDeny && beforeCheck && permission.IsDenyTA2(actionIndex))
                    return false;

                if (beforeCheck == false)
                    continue;

                if (allow == false && permission.IsAllowTA2(actionIndex))
                {
                    if (CanSetDeny)
                        allow = true;
                    else
                        return true;
                }
            }

            return allow;
        }

        #region 已过时的、传入MyUserID的重载

        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public bool HasPermissionForSomeone(int operatorUserID, TA2 action)
        {
            User user = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithGuest);

            if (user == null)
                return false;

            return HasPermissionForSomeone(user, action);
        }

        #endregion

        //======================================================================================

        public Guid[] GetNoPermissionTargetRoleIds(User my, TA2 action)
        {
            int actionIndex = GetActionValue(action);

            PermissionTargetType targetType = Permissions.GetPermissionTargetType(actionIndex);

            return GetNoPermissionTargetRoleIds(my, targetType);
        }

        /// <summary>
        /// 通过调用此方法来得到我到底没有权限管理哪些用户组
        /// TODO : 尚未检查 BeforePermissionCheck
        /// </summary>
        public Guid[] GetNoPermissionTargetRoleIds(User my, PermissionTargetType permissionTargetType)
        {
            RoleCollection allRoles = AllSettings.Current.RoleSettings.Roles;

            //User user = UserBO.Instance.GetUser(operatorUserID);

            if (my == null)
                return allRoles.GetKeys();


            //如果是创始人，直接返回null
            if (my.IsOwner)
                return new Guid[0];

            //管理类权限而我不是管理员
            if (IsManagement && my.IsManager == false)
                return allRoles.GetKeys();

            PermissionLimit limit;

            switch (permissionTargetType)
            {
                case PermissionTargetType.Content:
                    limit = AllSettings.Current.PermissionSettings.ContentPermissionLimit;
                    break;

                case PermissionTargetType.User:
                    limit = AllSettings.Current.PermissionSettings.UserPermissionLimit;
                    break;

                default:
                    throw new NotSupportedException("Action with target must defined 'PermissionSetWithTargetType'");
            }


            if (limit.LimitType == PermissionLimitType.Unlimited)
            {
                return new Guid[0] ;
            }

            List<Guid> allowRoleIds = new List<Guid>();

            foreach (UserRole userRole in my.Roles)
            {
                //不是管理员
                if (IsManagement && userRole.Role.IsManager == false)
                    continue;

                RoleCollection allowRoles;

                if (limit.LimitType == PermissionLimitType.RoleLevelLowerMe)
                {
                    allowRoles = AllSettings.Current.RoleSettings.GetRoles(delegate(Role role)
                    {
                        if (role < userRole.Role)
                            return true;
                        return false;
                    });
                }
                else if (limit.LimitType == PermissionLimitType.RoleLevelLowerOrSameMe)
                {
                    allowRoles = AllSettings.Current.RoleSettings.GetRoles(delegate(Role role)
                    {
                        if (role <= userRole.Role)
                            return true;
                        return false;
                    });
                }
                else if (limit.LimitType == PermissionLimitType.ExcludeCustomRoles)
                {

                    List<Guid> excludeRoleIds = null;
                    limit.ExcludeRoles.TryGetValue(userRole.RoleID, out excludeRoleIds);

                    if (excludeRoleIds != null && excludeRoleIds.Count != 0)
                    {
                        allowRoles = AllSettings.Current.RoleSettings.GetRoles(delegate(Role role)
                        {
                            if (excludeRoleIds.Contains(role.RoleID))
                                return false;
                            return true;
                        });
                    }
                    else
                        allowRoles = AllSettings.Current.RoleSettings.Roles;

                }
                else
                    throw new NotSupportedException();
                

                foreach (Role allowRole in allowRoles)
                {
                    if (allowRoleIds.Contains(allowRole.RoleID) == false)
                        allowRoleIds.Add(allowRole.RoleID);
                }

            }

            int noPermissionRolesCount = allRoles.Count - allowRoleIds.Count;

            Guid[] noPermissionRoleIds = new Guid[noPermissionRolesCount];

            int i = 0;
            foreach (Role role in allRoles)
            {
                if (allowRoleIds.Contains(role.RoleID) == false)
                {
                    noPermissionRoleIds[i] = role.RoleID;
                    i++;
                }
            }

            return noPermissionRoleIds;
        }

        #region  已过时的、传入MyUserID的重载

        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public Guid[] GetNoPermissionTargetRoleIds(int operatorUserID, TA2 action)
        {
            User my = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithAll);

            return GetNoPermissionTargetRoleIds(my, action);
        }

        [Obsolete("请直接把My对象传入以获得更好的性能")]
        public Guid[] GetNoPermissionTargetRoleIds(int operatorUserID, PermissionTargetType permissionTargetType)
        {
            User my = UserBO.Instance.GetUser(operatorUserID, GetUserOption.WithAll);

            return GetNoPermissionTargetRoleIds(my, permissionTargetType);
        }

        #endregion

        #endregion


        #region 内部使用，得到权限项的字段

        private FieldInfo[] GetFieldInfos()
        {
            string cachekey = string.Format("PermissionSet/FieldInfos/{0}", typeof(TA1).Name);

            FieldInfo[] fieldInfos;

            if (PageCacheUtil.TryGetValue(cachekey, out fieldInfos) == false)
            {
                fieldInfos = typeof(TA1).GetFields(BindingFlags.Static | BindingFlags.Public);

                PageCacheUtil.Set(cachekey, fieldInfos);
            }

            return fieldInfos;
        }


        private FieldInfo[] GetFieldInfosWithTarget()
        {

            string cachekey = string.Format("PermissionSet/FieldInfos/{0}", typeof(TA2).Name);

            FieldInfo[] fieldInfos;

            if (PageCacheUtil.TryGetValue(cachekey, out fieldInfos) == false)
            {
                fieldInfos = typeof(TA2).GetFields(BindingFlags.Static | BindingFlags.Public);

                PageCacheUtil.Set(cachekey, fieldInfos);
            }

            return fieldInfos;
        }

        #endregion


        #region 获得权限项的名称列表

        public StringCollection GetPermissionItemNames()
        {

            StringCollection permissionItemNames = new StringCollection();

            foreach (FieldInfo fieldInfo in GetFieldInfos())
            {
                string name = null;

                if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    name = ((PermissionItemAttribute)(fieldInfo.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0])).Name;
                }

                if (string.IsNullOrEmpty(name))
                    name = fieldInfo.Name;

                permissionItemNames.Add(name);
            }

            return permissionItemNames;
        }


        public StringCollection GetPermissionItemNamesWithTarget()
        {
            StringCollection permissionItemNames = new StringCollection();

            foreach (FieldInfo fieldInfo in GetFieldInfosWithTarget())
            {
                string name = null;

                if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    name = ((PermissionItemAttribute)(fieldInfo.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0])).Name;
                }

                if (string.IsNullOrEmpty(name))
                    name = fieldInfo.Name;

                permissionItemNames.Add(name);
            }

            return permissionItemNames;
        }

        #endregion

        #region 获得指定用户组的权限项列表

        public List<PermissionItem> GetPermissionItems(Role role)
        {
            Permission<TA1, TA2> permission = Permissions.AlwaysGetPermission(role.RoleID);

            List<PermissionItem> items = new List<PermissionItem>();

            int index = 0;
            foreach (FieldInfo fieldInfo in GetFieldInfos())
            {
                PermissionItem item = new PermissionItem();

                item.FieldName = fieldInfo.Name;

                if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    PermissionItemAttribute permissionItemAttribute = (PermissionItemAttribute)fieldInfo.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0];

                    item.Name = permissionItemAttribute.Name;

                }

                if (string.IsNullOrEmpty(item.Name))
                    item.Name = fieldInfo.Name;

                item.Role = role;
                item.InputName = "TA1_" + fieldInfo.Name + "_" + role.RoleID.ToString("N");

               // TA1 action = (TA1)Enum.Parse(typeof(TA1), fieldInfo.Name);

                if (permission.IsDenyTA1(index))
                    item.IsDeny = true;
                else if (permission.IsAllowTA1(index))
                    item.IsAllow = true;
                else
                    item.IsNotset = true;

                item.IsDisabled = permission.IsDisabledTA1(index);

                items.Add(item);

                index++;
            }

            return items;

        }

        #region 哈希表版本

        public Dictionary<string, PermissionItem> GetPermissionItemTable(Role role)
        {
            Permission<TA1, TA2> permission = Permissions.AlwaysGetPermission(role.RoleID);

            Dictionary<string, PermissionItem> items = new Dictionary<string, PermissionItem>(StringComparer.OrdinalIgnoreCase);

            int index = 0;

            foreach (FieldInfo fieldInfo in GetFieldInfos())
            {
                PermissionItem item = new PermissionItem();

                item.FieldName = fieldInfo.Name;

                if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    PermissionItemAttribute permissionItemAttribute = (PermissionItemAttribute)fieldInfo.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0];

                    item.Name = permissionItemAttribute.Name;

                }

                if (string.IsNullOrEmpty(item.Name))
                    item.Name = fieldInfo.Name;

                item.Role = role;
                item.InputName = "TA1_" + fieldInfo.Name + "_" + role.RoleID.ToString("N");

                // TA1 action = (TA1)Enum.Parse(typeof(TA1), fieldInfo.Name);

                if (permission.IsDenyTA1(index))
                    item.IsDeny = true;
                else if (permission.IsAllowTA1(index))
                    item.IsAllow = true;
                else
                    item.IsNotset = true;

                item.IsDisabled = permission.IsDisabledTA1(index);


                string fileName = null;
                if (fieldInfo.IsDefined(typeof(BackendPageAttribute), false))
                {
                    BackendPageAttribute backendPageAttribute = (BackendPageAttribute)fieldInfo.GetCustomAttributes(typeof(BackendPageAttribute), false)[0];

                    fileName = backendPageAttribute.FileName;
                }

                if (fileName != null)
                    items.Add(fileName, item);

                index++;
            }

            return items;
        }

        #endregion

        public List<PermissionItem> GetPermissionItemsWithTarget(Role role)
        {
            Permission<TA1, TA2> permission = Permissions.AlwaysGetPermission(role.RoleID);

            List<PermissionItem> items = new List<PermissionItem>();

            int index = 0;
            foreach (FieldInfo fieldInfo in GetFieldInfosWithTarget())
            {
                PermissionItem item = new PermissionItem();

                item.FieldName = fieldInfo.Name;

                if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    item.Name = ((PermissionItemAttribute)(fieldInfo.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0])).Name;
                }

                if (string.IsNullOrEmpty(item.Name))
                    item.Name = fieldInfo.Name;

                item.Role = role;
                item.InputName = "TA2_" + fieldInfo.Name + "_" + role.RoleID.ToString("N");

                TA2 action = (TA2)Enum.Parse(typeof(TA2), fieldInfo.Name);

                if (permission.IsDenyTA2(index))
                    item.IsDeny = true;
                else if (permission.IsAllowTA2(index))
                    item.IsAllow = true;
                else
                    item.IsNotset = true;

                item.IsDisabled = permission.IsDisabledTA2(index);

                items.Add(item);

                index++;
            }

            return items;
        }

        #region 哈希表版本

        public Dictionary<string, PermissionItem> GetPermissionItemWithTargetTable(Role role)
        {
            Permission<TA1, TA2> permission = Permissions.AlwaysGetPermission(role.RoleID);

            Dictionary<string, PermissionItem> items = new Dictionary<string, PermissionItem>(StringComparer.OrdinalIgnoreCase);

            int index = 0;

            foreach (FieldInfo fieldInfo in GetFieldInfosWithTarget())
            {
                PermissionItem item = new PermissionItem();

                item.FieldName = fieldInfo.Name;

                if (fieldInfo.IsDefined(typeof(PermissionItemAttribute), false))
                {
                    PermissionItemAttribute permissionItemAttribute = (PermissionItemAttribute)fieldInfo.GetCustomAttributes(typeof(PermissionItemAttribute), false)[0];

                    item.Name = permissionItemAttribute.Name;

                }

                if (string.IsNullOrEmpty(item.Name))
                    item.Name = fieldInfo.Name;

                item.Role = role;
                item.InputName = "TA2_" + fieldInfo.Name + "_" + role.RoleID.ToString("N");

                // TA1 action = (TA1)Enum.Parse(typeof(TA1), fieldInfo.Name);

                if (permission.IsDenyTA2(index))
                    item.IsDeny = true;
                else if (permission.IsAllowTA2(index))
                    item.IsAllow = true;
                else
                    item.IsNotset = true;

                item.IsDisabled = permission.IsDisabledTA2(index);


                string fileName = null;
                if (fieldInfo.IsDefined(typeof(BackendPageAttribute), false))
                {
                    BackendPageAttribute backendPageAttribute = (BackendPageAttribute)fieldInfo.GetCustomAttributes(typeof(BackendPageAttribute), false)[0];

                    fileName = backendPageAttribute.FileName;
                }

                if (fileName != null)
                    items.Add(fileName, item);

                index++;
            }

            return items;
        }

        #endregion

        #endregion

        #region 给指定用户组设置权限项

        /// <summary>
        /// 给指定用户组设置权限项
        /// </summary>
        /// <param name="role"></param>
        /// <param name="items"></param>
        /// <param name="itemsWithTarget"></param>
        public void SetAllPermissionItems(Role role, List<PermissionItem> items, List<PermissionItem> itemsWithTarget)
        {
            Permission<TA1, TA2> permission = new Permission<TA1, TA2>();

            if (items != null)
            {
                foreach (PermissionItem item in items)
                {
                    TA1 action = (TA1)Enum.Parse(typeof(TA1), item.FieldName);

                    bool isAllow;
                    bool isDeny;
                    bool editable;

                    ProcessActionFiled<TA1>(role.RoleID, permission, action, item, out isAllow, out isDeny, out editable);

                    if (isDeny)
                        permission.SetDeny(action);

                    else if (isAllow)
                        permission.SetAllow(action);
                    else if (editable)
                        permission.SetNotSet(action);

                    if (editable == false)
                        permission.SetDisabled(action);
                }
            }

            if (itemsWithTarget != null)
            {
                foreach (PermissionItem item in itemsWithTarget)
                {
                    TA2 action = (TA2)Enum.Parse(typeof(TA2), item.FieldName);

                    bool isAllow;
                    bool isDeny;
                    bool editable;
                    ProcessActionFiled<TA2>(role.RoleID, permission, action, item, out isAllow, out isDeny, out editable);

                    if (isDeny)
                        permission.SetDeny(action);

                    else if (isAllow)
                        permission.SetAllow(action);
                    else if (editable)
                        permission.SetNotSet(action);

                    if (editable == false)
                        permission.SetDisabled(action);
                }
            }

            Permissions[role.RoleID] = permission;
        }

        private void ProcessActionFiled<T>(Guid roleID, Permission<TA1, TA2> permission, T action, PermissionItem item,out bool isAllow,out bool isDeny,out bool editable)
        {
            isAllow = false;
            isDeny = false;
            editable = true;
            FieldInfo[] fieldInfos = action.GetType().GetFields();

            foreach (FieldInfo actionField in fieldInfos)
            {
                if (actionField.Name == item.FieldName)
                {
                    if (actionField.IsDefined(typeof(PermissionItemAttribute), false))
                    {
                        bool tempIsAllow;
                        bool tempIsDeny;

                        permission.GetPermissionItemAttributeSet(roleID, actionField, out tempIsAllow, out tempIsDeny, out editable);

                        if (false == editable)
                        {
                            if (tempIsAllow)
                                isAllow = true;
                            else if (tempIsDeny)
                                isDeny = true;
                        }
                        else
                        {
                            if (item.IsDeny)
                                isDeny = true;
                            else if (item.IsAllow)
                                isAllow = true;
                        }

                        break;
                    }
                }
            }
        }

        #endregion

        #region IPermissionSet 成员

        [SettingItem]
        public int NodeID
        {
            get;
            set;
        }

        public int RealNodeID
        {
            get;
            set;
        }

        public virtual bool HasNodeList
        {
            get { return false; }
        }

        public virtual NodeItemCollection NodeItemList
        {
            get { return new NodeItemCollection(); }
        }

        #endregion
    }

}