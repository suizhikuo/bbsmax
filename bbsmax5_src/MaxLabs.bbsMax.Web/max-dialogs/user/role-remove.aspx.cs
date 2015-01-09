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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class user_role_remove : AdminDialogPageBase
    {
        private int m_RoleID;
        private Role m_Role;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_RoleID = _Request.Get<int>("roleID", Method.Get, 0);

            if (m_RoleID == 0)
            {
                ShowError(new InvalidParamError("roleID"));
                return;
            }

            m_Role = RoleBO.Instance.GetRoleByID(m_RoleID);

            if (m_Role == null)
            {
                ShowError("指定的用户组并不存在");
                return;
            }

            //根据不同类型的用户组检查不同的权限
            if (m_Role.IsLevel && AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_Roles_Level) == false)
            {
                ShowError("您没有管理等级组的权限");
            }

            else if (m_Role.IsNormal && AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_Roles_Other) == false)
            {
                ShowError("您没有管理自定义组的权限");
            }

            else if (m_Role.IsManager && AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Setting_Roles_Manager, m_Role) == false)
            {
                ShowError("您没有管理这个管理员组的权限");
            }

            //if (false == AllSettings.Current.BackendManageUserPermissionSet.Can(My, BackendManageUserPermissionSet.Action.ManageUserGroup))
            //{
            //    ShowError(Lang.PermissionItem_ManageUserPermissionSet_ChangeGroup);
            //}

            if (_Request.IsClick("removerole"))
            {
                //          RoleSettings setting = AllSettings.Current.RoleSettings;

                //        setting.RemoveRole(Role.RoleID);

                //        SettingManager.SaveSettings(setting);

                RoleBO.Instance.DeleteRole(RoleID);
                Return(_Request.Get<int>("roleid", Method.All, 0), true);

            }
        }

        protected int RoleID
        {
            get { return m_RoleID; }
        }

        protected Role Role
        {
            get { return m_Role; }
        }
    }
}