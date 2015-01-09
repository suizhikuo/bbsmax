//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;



namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class UserGroupTemplateMembers
    {
        //#region 用户组数量

        ///// <summary>
        ///// 所有的用户组数量
        ///// </summary>
        //[TemplateVariable]
        //public int UserGroupCount
        //{
        //    get
        //    {
        //        return UserGroupBO.Instance.GetUserGroupCount();
        //    }
        //}

        //private int? m_AdminUserGroupCount;

        ///// <summary>
        ///// 管理员用户组的数量
        ///// </summary>
        //[TemplateVariable]
        //public int AdminUserGroupCount
        //{
        //    get
        //    {
        //        if (m_AdminUserGroupCount == null)
        //            m_AdminUserGroupCount = UserGroupBO.Instance.GetUserGroupCount(UserGroupTypes.Admin);

        //        return m_AdminUserGroupCount.Value;
        //    }
        //}

        //private int? m_LevelUserGroupCount;

        ///// <summary>
        ///// 级别用户组的数量
        ///// </summary>
        //[TemplateVariable]
        //public int LevelUserGroupCount
        //{
        //    get
        //    {
        //        if (m_LevelUserGroupCount == null)
        //            m_LevelUserGroupCount = UserGroupBO.Instance.GetUserGroupCount(UserGroupTypes.Level);

        //        return m_LevelUserGroupCount.Value;
        //    }
        //}

        //private int? m_MemberUserGroupCount;

        ///// <summary>
        ///// 会员用户组的数量
        ///// </summary>
        //[TemplateVariable]
        //public int MemberUserGroupCount
        //{
        //    get
        //    {
        //        if (m_MemberUserGroupCount == null)
        //            m_MemberUserGroupCount = UserGroupBO.Instance.GetUserGroupCount(UserGroupTypes.Member);

        //        return m_MemberUserGroupCount.Value;
        //    }
        //}

        //private int? m_SystemUserGroupCount;

        ///// <summary>
        ///// 系统用户组的数量
        ///// </summary>
        //[TemplateVariable]
        //public int SystemUserGroupCount
        //{
        //    get
        //    {
        //        if (m_SystemUserGroupCount == null)
        //            m_SystemUserGroupCount = UserGroupBO.Instance.GetUserGroupCount(UserGroupTypes.System);

        //        return m_SystemUserGroupCount.Value;
        //    }
        //}

        //#endregion

        /// <summary>
        /// 用户组数据显示模板
        /// </summary>
        /// <param name="i">当前记录的序号，从0开始计数</param>
        /// <param name="group">用户组数据</param>
        public delegate void UserRolesTemplate(int i, Role group);

        /// <summary>
        /// 显示用户组数据
        /// </summary>
        /// <param name="type">用户组类型</param>
        /// <param name="template">用户组数据显示模板</param>
        [TemplateTag]
        public void UserRoleList(string type, UserRolesTemplate template)
        {
            RoleType roleType;
            bool success = StringUtil.TryParse<RoleType>(type, out roleType);

            int i = 0;

            foreach (Role role in AllSettings.Current.RoleSettings.Roles)
            {
                if (success)
                {
                    if (roleType == role.Type)
                        template(i++, role);
                }
                else
                    template(i++, role);
            }
        }

        //public delegate void UserGroupListTemplate(int i, UserGroup group);
        ///// <summary>
        ///// 显示用户的用户组
        ///// </summary>
        //[TemplateTag]
        //public void UserGroupList(string userID, UserGroupListTemplate template)
        //{
        //    int id = 0;
        //    if (!string.IsNullOrEmpty(userID))
        //        id = StringUtil.GetInt(userID, 0);

        //    int i = 0;
        //    foreach (UserGroup group in UserGroupBO.Instance.GetUserGroups(id))
        //    {
        //        template(i++, group);
        //    }
        //}



    }
}