//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_rolemembers : AdminPageBase
    {
        private Guid m_RoleID = Guid.Empty;
        private Role m_Role;

        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                if (Role.IsBasic)
                    return BackendPermissions.Action.Setting_Roles_Basic;
                else if (Role.IsLevel)
                    return BackendPermissions.Action.Setting_Roles_Level;
                else if (Role.IsNormal)
                    return BackendPermissions.Action.Setting_Roles_Other;
                else
                    return base.BackedPermissionAction;
            }
        }

        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get
            {
                if (Role.IsManager)
                    return BackendPermissions.ActionWithTarget.Setting_Roles_Manager;
                else
                    return base.BackedPermissionActionWithTarget;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserRole, Role) == false)
            {
                ShowError("您没有管理该用户组成员的权限");
                return;
            }

            if (Role == null)
            {
                ShowError("指定的用户组并不存在");
                return;
            }

            if (_Request.IsClick("removefromrole"))
            {
                RemoveFromRole();
            }
        }

        protected Guid RoleID
        {
            get
            {
                if (m_RoleID == Guid.Empty)
                {
                    m_RoleID = _Request.Get<Guid>("role", Method.Get, Guid.Empty);

                    if (m_RoleID == Guid.Empty)
                    {
                        ShowError(new InvalidParamError("role"));
                    }
                }

                return m_RoleID;
            }
        }

        protected Role Role
        {
            get
            {
                if (m_Role == null)
                {
                    m_Role = AllSettings.Current.RoleSettings.GetRole(RoleID);
                }
                return m_Role;
            }
        }

        private UserCollection m_MemberList;
        private int m_TotalCount;

        protected int TotalCount
        {
            get
            {
                if (m_MemberList == null)
                    m_MemberList = MemberList;
                return m_TotalCount;
            }
        }

        protected UserCollection MemberList
        {
            get
            {
                if (m_MemberList == null)
                {
                    Guid roleId = _Request.Get<Guid>("role", MaxLabs.WebEngine.Method.Get, Guid.Empty);
                    int pageIndex = _Request.Get<int>("page", MaxLabs.WebEngine.Method.Get, 1);
                    m_MemberList = UserBO.Instance.GetRoleMembers(MyUserID, roleId, Consts.DefaultPageSize, pageIndex, out m_TotalCount);
                }
                return m_MemberList;
            }
        }

        protected void RemoveFromRole()
        {
            int[] userIds = StringUtil.Split<int>(_Request.Get("userids", MaxLabs.WebEngine.Method.Post));
            UserBO.Instance.RemoveUsersFromRole(My, userIds, Role);
        }

        protected bool CanChangeMember
        {
            get
            {
                return !Role.IsLevel && !Role.IsVirtualRole;
            }
        }

        protected string GetBeginDate(User user)
        {
            foreach (UserRole ur in user.Roles)
            {
                if (ur.RoleID == Role.RoleID)
                {
                    return OutputDate(ur.BeginDate);
                }
            }
            return "";
        }

        protected string GetEndDate(User user)
        {
            foreach (UserRole ur in user.Roles)
            {
                if (ur.RoleID == Role.RoleID)
                {
                    return OutputDate(ur.EndDate);
                }
            }
            return "";
        }
    }
}