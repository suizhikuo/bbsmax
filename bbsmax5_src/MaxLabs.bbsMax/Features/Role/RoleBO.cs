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
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax
{
    public class RoleBO:BOBase<RoleBO>
    {
        public void DeleteRole(int roleid)
        {
            //roleid在100以内,为系统内置
            if (roleid < 100)
                return;
            RoleDao.Instance.DeleteRole(roleid);

            ClearCache();
        }

        public UserRole GetUserRoleByBothIDs(int userID, Guid roleID)
        {
            return RoleDao.Instance.GetUserRole(userID, roleID);
        }

        public static RoleCollection s_AllRoles;
        public RoleCollection GetAllRoles()
        {
            RoleCollection allroles = s_AllRoles;
            if (allroles == null)
            {
                allroles = RoleDao.Instance.GetAllRoles();
                s_AllRoles = allroles;
            }
            return allroles;
        }

        public Role GetRoleByID(int id)
        {
            return RoleDao.Instance.GetRoleByID(id);
        }

        public void AddOrUpdateRole(int roleid,string name,string title,string color,string iconUrlSrc,RoleType type,int level,int starLevel,int requiredPoint)
        {
            RoleDao.Instance.AddRole(roleid, name, title, color, iconUrlSrc, type, level, starLevel, requiredPoint);

            ClearCache();
        }

        public UserCollection GetRoleMembers(int operatorUserID, int roleID, int pageSize, int pageNumber, out int totalCount)
        {
            RoleSettings roleSetting = AllSettings.Current.RoleSettings;
            //if( ManagePermissionSet.Can(operatorUserID, ManageUserPermissionSet.ActionWithTarget.  )

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = Consts.DefaultPageSize;

    //        Role role = roleSetting.GetRole(roleID);
            Role role = RoleDao.Instance.GetRoleByID(roleID);
            totalCount = 0;
            if (role == null)
            {
                return new UserCollection();
            }
        /*
            if (role.IsLevel)
            {
                int levelValue1, levelValue2 = int.MaxValue;
                levelValue1 = role.RequiredPoint;
                foreach (Role r in roleSetting.GetLevelRoles())
                {
                    if (r.RequiredPoint > role.RequiredPoint)
                    {
                        levelValue2 = r.RequiredPoint;
                        break;
                    }
                }

                return UserDao.Instance.GetRoleMembers(roleSetting.LevelLieOn
                    , new Int32Scope(levelValue1, levelValue2)
                    , pageSize, pageNumber, out totalCount);
            }
        
         */ 
            return RoleDao.Instance.GetRoleMembers(roleID, pageSize, pageNumber, out totalCount);
        }

        private void ClearCache()
        {
            s_AllRoles = null;
            s_BasicRoles = null;
            s_LevelRoles = null;
            s_ManagerRoles = null;
            s_NormalRoles = null;
        }

        public RoleCollection GetRoles(RoleResolver resolver)
        {
            RoleCollection roles = new RoleCollection();

            foreach (Role role in GetAllRoles())
            {
                if (resolver(role))
                    roles.Add(role);
            }

            return roles;
        }

        #region 基本组 GetBasicRoles

        public static RoleCollection s_BasicRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetBasicRoles()
        {
            RoleCollection basicRoles = s_BasicRoles;
            if (basicRoles == null)
            {
                basicRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsBasic);
                });
                s_BasicRoles = basicRoles;
            }

            return basicRoles;
        }

        #endregion

        #region 等级组 GetLevelRoles

        public static RoleCollection s_LevelRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetLevelRoles()
        {
            RoleCollection levelRoles = s_LevelRoles;
            if (levelRoles == null)
            {
                levelRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsLevel);
                });
                s_LevelRoles = levelRoles;
            }

            return levelRoles;
        }

        #endregion

        #region 自定义组 GetNormalRoles

        public static RoleCollection s_NormalRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetNormalRoles()
        {
            RoleCollection normalRoles = s_NormalRoles;
            if (normalRoles == null)
            {
                normalRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsNormal);
                });
                s_NormalRoles = normalRoles;
            }

            return normalRoles;
        }

        #endregion

        #region 管理组 GetManagerRoles

        public static RoleCollection s_ManagerRoles = null;
        /// <summary>
        /// 获取所有管理员用户组（即可以往其中添加成员的用户组）
        /// </summary>
        /// <returns></returns>
        public RoleCollection GetManagerRoles()
        {
            RoleCollection managerRoles = s_ManagerRoles;
            if (managerRoles == null)
            {
                managerRoles = GetRoles(delegate(Role role)
                {
                    return (role.IsManager);
                });

                s_ManagerRoles = managerRoles;
            }

            return managerRoles;
        }

        #endregion


        #region 对用户进行用户组操作

        #region AddUsersToRoles


        ///// <summary>
        ///// 将一组用户加入一组用户组，并检查操作者的权限
        ///// </summary>
        ///// <param name="operatorUserID"></param>
        ///// <param name="usersInRoles"></param>
        ///// <returns></returns>
        //public bool AddUsersToRoles(AuthUser operatorUser, UserRoleCollection userRoles)
        //{
        //    if (operatorUser.UserID <= 0)
        //    {
        //        ThrowError(new NotLoginError());
        //        return false;
        //    }

        //    if (userRoles == null || userRoles.Count == 0)
        //    {
        //        ThrowError(new NoUsersAddToRolesError("userRoles", userRoles));
        //        return false;
        //    }

        //    List<int> userIds = new List<int>();

        //    for (int i = 0; i < userRoles.Count; i ++ )
        //    {
        //        UserRole userRole = userRoles[i];

        //        if (userRole.RoleID == Guid.Empty
        //            ||
        //            userRole.Role == null
        //            ||
        //            ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, userRole.UserID) == false
        //            )
        //            userRoles.RemoveAt(i);

        //        else if (userIds.Contains(userRole.UserID) == false)
        //            userIds.Add(userRole.UserID);
        //    }

        //    if (userRoles.Count == 0)
        //    {
        //        return true;
        //    }

        //    UserDao.Instance.AddUsersToRoles(userRoles);

        //    RemoveUsersCache(userIds);

        //    return true;
        //}

        /// <summary>
        /// 将一组用户加入一组用户组
        /// </summary>
        /// <param name="usersInRoles"></param>
        /// <returns></returns>
        internal bool AddUsersToRoles(UserRoleCollection userRoles)
        {
            if (userRoles == null || userRoles.Count == 0)
            {
                ThrowError(new NoUsersAddToRolesError("userRoles", userRoles));
                return false;
            }

            List<int> userIds = new List<int>();

            for (int i = 0; i < userRoles.Count; i++)
            {
                UserRole userRole = userRoles[i];

                if (userRole.RoleID == Guid.Empty || userRole.Role == null || userRole.Role.IsVirtualRole == true)
                    userRoles.RemoveAt(i);

                else if (userIds.Contains(userRole.UserID) == false)
                    userIds.Add(userRole.UserID);
            }

            UserDao.Instance.AddUsersToRoles(userRoles);

   //         RemoveUsersCache(userIds);

            return true;
        }

        public int AddUsersToRole(AuthUser operatorUser, IEnumerable<int> userIds, Role role, DateTime beginDate, DateTime enddate)
        {
            RoleUserCollection roleusers = new RoleUserCollection();

            RoleUser ru;

            SimpleUserCollection users = UserBO.Instance.GetSimpleUsers(userIds);
            SimpleUser user;

            foreach (int i in userIds)
            {
                if (users.TryGetValue(i, out user))
                {
                    ru = new RoleUser();
                    ru.UserID = i;
                    ru.RoleID = role.RID;
                    ru.BeginDate = beginDate;
                    ru.EndDate = enddate;
                    roleusers.Add(ru);
                }
            }

           return Math.Abs(RoleDao.Instance.AddUsersToRoles(roleusers));
        }

        public int RemoveUsersFromRole(AuthUser operatorUser, IEnumerable<int> userIds, Role role)
        {
            int[] roleIds = new int[] { role.RID };

            // to do 判断用户是否有操作权限.

            return RoleDao.Instance.RemoveUsersFromRoles(userIds, roleIds);
        }
    

        #endregion

        #endregion



    }
}