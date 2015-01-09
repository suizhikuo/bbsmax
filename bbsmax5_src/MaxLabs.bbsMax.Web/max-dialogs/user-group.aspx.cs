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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_group : UserDialogPageBase
    {
        private User m_User;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserBO.Instance.CanEditRole(My, UserID))
            {
                ShowError(Lang.PermissionItem_ManageUserPermissionSet_ChangeGroup);
                return;
            }

            m_User = UserBO.Instance.GetUser(UserID);

            if (m_User == null)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            if (_Request.IsClick("updateusergroup"))
            {
                UpdateUserGroup();
            }
        }

        protected User user
        {
            get { return m_User; }
        }

        protected void UpdateUserGroup()
        {
            MessageDisplay msgdisplay = CreateMessageDisplay();
            UserBO Bo = UserBO.Instance;
            string[] groupIds = StringUtil.Split(_Request.Get("groupids", Method.Post), ',');
            Guid gid;
            bool isRemove = false;


            //bool flagHasNopermissionRole = false ;
            string noPemissionRoleNames = string.Empty;

            //foreach (Role r in RoleList)
            //{
            //    if (r.IsVirtualRole) continue;

            //    isRemove = false;

            //    if (groupIds != null)
            //    {
            //        foreach (string s in groupIds)
            //        {
            //            gid = new Guid(s);
            //            if (gid == r.RoleID)
            //            {
            //                isRemove = true;
            //                break;
            //            }
            //        }
            //    }

            //    if (isRemove == false)
            //    {
            //        if (user.Roles.IsInRole(r.RoleID))
            //            Bo.RemoveUserFromRole(My, user.UserID, r.RoleID);
            //    }
            //}

            //if (flagHasNopermissionRole == true)
            //{
            //    MessageDisplay msgDisplay = CreateMessageDisplay();

            //    msgDisplay.AddError("没有权限从" + noPemissionRoleNames + " 组添加或移除用户,同时也没有权限设置这些用户组的有效时间");
            //    noPemissionRoleNames = noPemissionRoleNames.TrimEnd(',');
            //    return;
            //}

            foreach (Role r in RoleList)
            {
                if (r.IsVirtualRole||!CanChange(r)) continue;

                isRemove = false;

                if (groupIds != null)
                {
                    foreach (string s in groupIds)
                    {
                        gid = new Guid(s);
                        if (gid == r.RoleID)
                        {
                            DateTime beginDate = DateTime.MinValue, endDate = DateTime.MaxValue;

                            beginDate = DateTimeUtil.ParseBeginDateTime(_Request.Get("beginDate" + r.RoleID, Method.Post));
                            endDate = DateTimeUtil.ParseEndDateTime(_Request.Get("endDate" + r.RoleID, Method.Post));
                            Bo.AddUsersToRole(My, new int[]{ user.UserID}, r, beginDate, endDate);

                            isRemove = true;
                            break;
                        }
                    }
                }

                if (isRemove == false)
                {
                    if (user.Roles.IsInRole(r.RoleID))
                        Bo.RemoveUserFromRole(My, user.UserID, r.RoleID);
                }
            }
        }

        protected string GetBeginDate(Guid roleID)
        {
            foreach (UserRole ur in user.Roles)
            {
                if (ur.RoleID == roleID)
                {
                    if (ur.BeginDate.Year > 1900)
                        return ur.BeginDate.ToString();
                    else
                        return string.Empty;
                }
            }
            return string.Empty;

        }

        protected string GetEndDate(Guid roleID)
        {
            foreach (UserRole ur in user.Roles)
            {
                if (ur.RoleID == roleID)
                {
                    if (ur.EndDate.Year < 9999)
                        return ur.EndDate.ToString();
                    else
                        return string.Empty;

                }
            }
            return string.Empty;
        }

        protected UserRole GetRoleInfo(Guid roleID)
        {
            foreach (UserRole ur in user.Roles)
            {
                if (ur.RoleID == roleID)
                {
                    return ur;
                }
            }
            return null;
        }

        //private bool RoleIsChanged( Guid roleID,DateTime beginDate,DateTime endDate)
        //{
        //    Role role = AllSettings.Current.RoleSettings.GetRole(roleID);
        //    if (role.IsVirtualRole)
        //        return false;
        //}

        private Role m_level;

        protected bool ShowVirtualRole( Guid roleid )
        {
            if (roleid == Role.Guests.RoleID || roleid == Role.Users.RoleID || roleid == Role.Everyone.RoleID)
            {
                return false;
            }
            return true;
        }
        protected Role LevelRole
        {
            get
            {
                if (m_level == null)
                {
                    m_level = user.LevelRole;
                }
                return m_level;
            }
        }

        Guid[] NoPermissionRoles;
        protected bool CanChange(Role role)
        {
            ManageUserPermissionSet permission = AllSettings.Current.ManageUserPermissionSet;

            if (!permission.Can(My, ManageUserPermissionSet.ActionWithTarget.EditUserRole, role))
            {
                return false;
            }

            if (NoPermissionRoles == null)
                NoPermissionRoles = permission.GetNoPermissionTargetRoleIds(My, ManageUserPermissionSet.ActionWithTarget.EditUserRole);

            bool flag = false;

            foreach (Guid g in NoPermissionRoles)
            {
                if (role.RoleID == g)
                {
                    flag = true;
                    break;
                }
            }
            return !flag;

        }

        protected bool InRole(Role role)
        {
            return user.Roles.IsInRole(role);
        }

        protected RoleCollection RoleList
        {
            get
            {
                return AllSettings.Current.RoleSettings.Roles;
            }
        }

        protected RoleCollection RealRoleList
        {
            get
            {
                return AllSettings.Current.RoleSettings.GetNonVirtualRoles();
            }
        }

        protected RoleCollection VirtualRoleList
        {
            get
            {
                return AllSettings.Current.RoleSettings.GetVirtualRoles();
            }
        }

        protected RoleCollection LevelRoleList
        {
            get
            {
                return AllSettings.Current.RoleSettings.GetLevelRoles();
            }
        }
    }
}