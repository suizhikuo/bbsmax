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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class RoleDao:DaoBase<RoleDao>
    {
        public abstract void DeleteRole(int roleid);

        public abstract void AddRole(int roleID, string name, string title, string color, string iconUrl, RoleType type, int level, int starLevel, int requiredPoint);

        public abstract RoleCollection GetAllRoles();

        public abstract Role GetRoleByID(int id);

        /// <summary>
        /// 将一组用户加入一组用户组
        /// </summary>
        /// <param name="userRoles"></param>
        public abstract int AddUsersToRoles(RoleUserCollection roleUsers);

        /// <summary>
        /// 将一组用户从一组用户组移除
        /// </summary>
        /// <param name="userRoles"></param>
        public abstract int RemoveUsersFromRoles(IEnumerable<int> userIds, IEnumerable<int> roleIds);

        public abstract UserRole GetUserRole(int userID, Guid roleID);

        public abstract UserCollection GetRoleMembers(int roleID, int pageSize, int pageNumber, out int totalCount);
    }
}