//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Reflection;

using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_managerpermissions : AdminPageBase
    {
        private Guid m_RoleID;
        private Role m_Role;

        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Setting_Permission_Manager; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_RoleID = _Request.Get<Guid>("roleid", Method.Get, Guid.Empty);

            if (m_RoleID == Guid.Empty)
            {
                m_Role = AllSettings.Current.RoleSettings.GetManagerRoles()[0];
                m_RoleID = m_Role.RoleID;
            }
            else
            {
                m_Role = RoleSettings.GetRole(m_RoleID);
            }

            if (m_Role == null)
            {
                ShowError(new InvalidParamError("roleid"));
                return;
            }

            if (_Request.IsClick("savepermission"))
                SavePermission();
        }

        protected Role Role
        {
            get { return m_Role; }
        }

        private Dictionary<string, PermissionItem> m_PermissionItemTable = null;
        private Dictionary<string, PermissionItem> PermissionItemTable
        {
            get
            {
                if (m_PermissionItemTable == null)
                    m_PermissionItemTable = AllSettings.Current.BackendPermissions.GetPermissionItemTable(m_Role);

                return m_PermissionItemTable;
            }
        }

        private Dictionary<string, PermissionItem> m_PermissionItemWithTargetTable = null;
        private Dictionary<string, PermissionItem> PermissionItemWithTargetTable
        {
            get
            {
                if (m_PermissionItemWithTargetTable == null)
                    m_PermissionItemWithTargetTable = AllSettings.Current.BackendPermissions.GetPermissionItemWithTargetTable(m_Role);

                return m_PermissionItemWithTargetTable;
            }
        }

        protected bool ReadOnly
        {
            get { return (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Setting_Permission_Manager, Role) == false); }
        }

        private void SavePermission()
        {
            if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Setting_Permission_Manager, Role) == false)
                ShowError("您没有权限设置“" + Role.Name + "”的操作权限");

            BackendPermissions permission = SettingManager.CloneSetttings<BackendPermissions>(AllSettings.Current.BackendPermissions);

            List<PermissionItem> permissionItems = new List<PermissionItem>();

            List<PermissionItem> permissionItemsWithTarget = new List<PermissionItem>();

            foreach (KeyValuePair<string, PermissionItem> item in PermissionItemTable)
            {
                PermissionItem newItem = item.Value.Clone();

                newItem.IsAllow = _Request.IsChecked("c_" + item.Key, Method.Post, false);

                permissionItems.Add(newItem);
            }

            foreach (KeyValuePair<string, PermissionItem> item in PermissionItemWithTargetTable)
            {
                PermissionItem newItem = item.Value.Clone();

                newItem.IsAllow = _Request.IsChecked("c_" + item.Key, Method.Post, false);

                permissionItemsWithTarget.Add(newItem);
            }

            permission.SetAllPermissionItems(m_Role, permissionItems, permissionItemsWithTarget);

            SettingManager.SaveSettings(permission);

            BbsRouter.JumpToCurrentUrl();

        }


        private List<string> m_FileNameOutputed = new List<string>();

        protected string GetLinkWithoutEndTag(string fileName)
        {
            string className = "disable";
            string isChecked = string.Empty;

            PermissionItem item;
            if (PermissionItemTable.TryGetValue(fileName, out item))
            {
                isChecked = item.IsAllow ? " checked=\"checked\"" : string.Empty;
                className = item.IsAllow ? "checked" : "banned";
            }
            else if (PermissionItemWithTargetTable.TryGetValue(fileName, out item))
            {
                isChecked = item.IsAllow ? " checked=\"checked\"" : string.Empty;
                className = item.IsAllow ? "checked" : "banned";
            }

            else if (fileName.IndexOf('?') != -1)
            {
                fileName = fileName.Substring(0, fileName.IndexOf('?'));

                if (PermissionItemTable.TryGetValue(fileName, out item))
                {
                    isChecked = item.IsAllow ? " checked=\"checked\"" : string.Empty;
                    className = item.IsAllow ? "checked" : "banned";
                }
                else if (PermissionItemWithTargetTable.TryGetValue(fileName, out item))
                {
                    isChecked = item.IsAllow ? " checked=\"checked\"" : string.Empty;
                    className = item.IsAllow ? "checked" : "banned";
                }
            }

            if (m_FileNameOutputed.Contains(fileName))
            {
                return @"<a class=""" + className + @""" href=""javascript:void(0)"" name=""a_" + fileName + @""" onclick=""swCheck('" + fileName + @"')"">";
            }
            else
            {
                m_FileNameOutputed.Add(fileName);
                return @"<input type=""checkbox"" value=""true"" name=""c_" + fileName + @""" style=""display:none""" + isChecked + @" /> 
<a class=""" + className + @""" href=""javascript:void(0)"" name=""a_" + fileName + @""" onclick=""swCheck('" + fileName + @"')"">";
            }
        }

        protected RoleCollection RoleList
        {
            get { return AllSettings.Current.RoleSettings.GetManagerRoles(); }
        }


        //private List<IPermissionSet> m_ManagePermissionSetList;

        //protected List<IPermissionSet> ManagePermissionSetList
        //{
        //    get
        //    {
        //        if (m_ManagePermissionSetList == null)
        //        {

        //            List<IPermissionSet> permissionSets = new List<IPermissionSet>();

        //            foreach (IPermissionSet permissionSet in SettingManager.GetPermissionSets())
        //            {
        //                if (permissionSet.IsManagement)
        //                    permissionSets.Add(permissionSet);
        //            }

        //            m_ManagePermissionSetList = permissionSets;
        //        }

        //        return m_ManagePermissionSetList;
        //    }
        //}
    }
}