//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;

using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using System.Collections.Generic;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_roles_manager : RoleSettingPageBase
	{
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Setting_Roles_Manager; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }
		}

        protected bool ShowMemberLink( Role r )
        {
            if( !(
                   r.RoleID == Role.CategoryModerators.RoleID 
                || r.RoleID == Role.Moderators.RoleID 
                || r.RoleID == Role.JackarooModerators.RoleID
                ))
                if (CanManageMember(r))
            return true;

            return false;
        }

        protected PermissionLimit UserPermissionLimit
        {
            get 
            {
                return PermissionSettings.UserPermissionLimit;
            }
        }

        protected PermissionLimit ContentPermissionLimit
        {
            get
            {
                return PermissionSettings.ContentPermissionLimit;
            }
        }

        protected bool CanManageMember( Role role )
        {
            return AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserRole, role);
        }

        public override bool BeforeSaveSettings(RoleSettings roleSettings)
        {

            if (My.IsOwner)
            {
                PermissionSettings settings = SettingManager.CloneSetttings<PermissionSettings>(AllSettings.Current.PermissionSettings);

                settings.ContentPermissionLimit.LimitType = _Request.Get<PermissionLimitType>("ContentPermissionLimit", Method.Post, PermissionLimitType.RoleLevelLowerMe);
                settings.UserPermissionLimit.LimitType = _Request.Get<PermissionLimitType>("UserPermissionLimit", Method.Post, PermissionLimitType.RoleLevelLowerMe);

                if (settings.ContentPermissionLimit.LimitType == PermissionLimitType.ExcludeCustomRoles)
                {
                    string key = "content.{0}.{1}";
                    GetLimitRoleList(key, settings.ContentPermissionLimit);
                }

                if (settings.UserPermissionLimit.LimitType == PermissionLimitType.ExcludeCustomRoles)
                {
                    string key = "user.{0}.{1}";
                    GetLimitRoleList(key, settings.UserPermissionLimit);
                }
                
                SettingManager.SaveSettings(settings);
            }

            return true;
        }

        /// <summary>
        /// 如果LimitType == PermissionLimitType.ExcludeCustomRoles 的时候取得页面上返回的角色表 
        /// </summary>
        /// <param name="formKey"></param>
        /// <param name="limit"></param>
        private void GetLimitRoleList( string formKey, PermissionLimit limit)
        {
            limit.ExcludeRoles.Clear();
            foreach (Role r in this.RoleList)
            {

                limit.ExcludeRoles.Add(r.RoleID, new List<Guid>(RoleList.Count));
                foreach (Role r2 in this.RoleList)
                {
                    if (_Request.Get<bool>(string.Format(formKey, r.RoleID, r2.RoleID), Method.Post, false) == false)
                    {
                        limit.ExcludeRoles[r.RoleID].Add(r2.RoleID);
                    }
                }
            }
        }

        protected override Role CreateRole()
        {
            return Role.CreateManagerRole();
        }

        protected RoleCollection ManagerRoleList
        {
            get
            {
                return RoleSettings.GetManagerRoles();
            }
        }


        protected override RoleCollection RoleList
        {
            get
            {
                return m_RoleList == null ? RoleSettings.GetManagerRoles() : m_RoleList;
            }
            set
            {
                m_RoleList = value;
            }
        }

        /// <summary>
        /// role1是否可以管理role2的用户
        /// </summary>
        /// <param name="role1"></param>
        /// <param name="role2"></param>
        /// <returns></returns>
        public bool CanManageUser(Guid role1, Guid role2)
        {
            return  RoleSettings.CanManageUser(role1, role2);
        }

        /// <summary>
        /// role1是否可以管理role2的内容
        /// </summary>
        /// <param name="role1"></param>
        /// <param name="role2"></param>
        /// <returns></returns>
        public bool CanManageContent(Guid role1, Guid role2)
        {
            return RoleSettings.CanManageContent(role1, role2);
        }
    }
}