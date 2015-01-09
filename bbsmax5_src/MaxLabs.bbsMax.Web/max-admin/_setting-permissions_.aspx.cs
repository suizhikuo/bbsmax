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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class _setting_permissions_ : AdminPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savepermission"))
                SavePermission();
        }

        private string m_NoPermissionManagerRoles;
        protected string NoPermissionManagerRoles
        {
            get
            {
                if (m_NoPermissionManagerRoles == null)
                {
                    BackendPermissions.ActionWithTarget action;

                    if (PermissionSet.IsManagement)
                        action = BackendPermissions.ActionWithTarget.Setting_Permission_Manager;
                    else
                        action = BackendPermissions.ActionWithTarget.Setting_Permissions_User;

                    Guid[] roleIDs = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(My, action);

                    if (roleIDs.Length == 0)
                        m_NoPermissionManagerRoles = string.Empty;
                    else
                        m_NoPermissionManagerRoles = AllSettings.Current.RoleSettings.GetRoleNames(roleIDs, ",");
                }
                return m_NoPermissionManagerRoles;
            }
        }

        private bool? m_Success;
        protected bool Success
        {
            get
            {
                if (m_Success == null)
                {
                    m_Success = _Request.Get("success", Method.Get, string.Empty).ToLower() == "true";
                }

                return m_Success.Value;
            }
        }

        protected bool IsForumPage
        {
            get
            {
                return Request.CurrentExecutionFilePath.ToLower().IndexOf("manage-forum-detail.aspx") >= 0;
            }
        }

        private void SavePermission()
        {
            m_Success = false;
            MessageDisplay msgDisplay = CreateMessageDisplay();
            if (_Request.Get("inheritType", Method.Post, "False").ToLower() == "true")
            {

                try
                {
                    if (PermissionSetWithNode == null)
                    {
                        msgDisplay.AddError(new InvalidParamError("type").Message);
                    }
                    else
                    {
                        PermissionSetWithNode.DeleteSetting(NodeID);

                        BbsRouter.JumpToUrl(Request.RawUrl, "success=true");
                    }

                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
                return;
            }


            IPermissionSet permissionSet = (IPermissionSet)Activator.CreateInstance(PermissionSet.GetType());

            BackendPermissions.ActionWithTarget action;

            if(permissionSet.IsManagement)
                action = BackendPermissions.ActionWithTarget.Setting_Permission_Manager;
            else
                action = BackendPermissions.ActionWithTarget.Setting_Permissions_User;


            foreach (Role role in RoleSettings.Roles)
            {
                List<PermissionItem> permissionItems = new List<PermissionItem>();

                foreach (PermissionItem item in PermissionSet.GetPermissionItems(role))
                {
                    PermissionItem newItem = item.Clone();

                    if (AllSettings.Current.BackendPermissions.Can(My, action, role))
                    {
                        #region 设置新值

                        string itemValue = _Request.Get(item.InputName, Method.Post);

                        switch (itemValue)
                        {
                            case "d":
                                newItem.IsDeny = true;
                                break;

                            case "a":
                                newItem.IsAllow = true;
                                break;

                            case "n":
                                newItem.IsNotset = true;
                                break;

                            default:
                                break;
                        }

                        #endregion
                    }
                    permissionItems.Add(newItem);
                }

                List<PermissionItem> permissionItemsWithTarget = new List<PermissionItem>();

                foreach (PermissionItem item in PermissionSet.GetPermissionItemsWithTarget(role))
                {
                    PermissionItem newItem = item.Clone();
                    if (AllSettings.Current.BackendPermissions.Can(My, action, role))
                    {
                        #region 设置新值

                        string itemValue = _Request.Get(item.InputName, Method.Post);

                        switch (itemValue)
                        {
                            case "d":
                                newItem.IsDeny = true;
                                break;

                            case "a":
                                newItem.IsAllow = true;
                                break;

                            case "n":
                                newItem.IsNotset = true;
                                break;

                            default:
                                break;
                        }

                        #endregion
                    }
                    permissionItemsWithTarget.Add(newItem);
                }

                permissionSet.SetAllPermissionItems(role, permissionItems, permissionItemsWithTarget);

            }

            permissionSet.NodeID = NodeID;


            if (PermissionSet.HasNodeList)
            {
                if (PermissionSetWithNode == null)
                {
                    msgDisplay.AddError(new InvalidParamError("type").Message);
                }
                else
                {
                    PermissionSetWithNode.SaveSetting(permissionSet);
                    BbsRouter.JumpToUrl(Request.RawUrl, "success=true");
                }
            }
            else
            {
                SettingManager.SaveSettings((SettingBase)permissionSet);

                BbsRouter.JumpToUrl(Request.RawUrl, "success=true");
            }
        }

        protected string Type
        {
            get
            {
                return Parameters["type"].ToString();
            }
        }

        private IPermissionSet m_PermissionSet = null;

        protected bool IsHasNodePermission
        {
            get
            {
                return PermissionSetWithNode != null;
            }
        }

        private NodeItem m_NodeItem;
        protected NodeItem NodeItem
        {
            get
            {
                if (m_NodeItem == null)
                {
                    if (PermissionSet.NodeID != 0)
                    {
                        foreach (NodeItem item in PermissionSet.NodeItemList)
                        {
                            if (item.NodeID == PermissionSet.NodeID)
                            {
                                m_NodeItem = item;
                                break;
                            }
                        }
                    }
                    else
                    {
                        m_NodeItem = new NodeItem();
                    }
                }
                return m_NodeItem;
            }
        }

        protected IPermissionSet PermissionSet
        {
            get
            {
                if (m_PermissionSet == null)
                {
                    m_PermissionSet = SettingManager.GetPermissionSet(Type, NodeID);

                    if (m_PermissionSet == null)
                    {
                        if (string.Compare(_Request.Get("t", Method.Get, "user"), "user", true) == 0)
                            m_PermissionSet = UserPermissionSetList[0];
                        else
                            m_PermissionSet = ManagePermissionSetList[0];
                    }
                }
                
                return m_PermissionSet;
            }
        }

        private IPermissionSetWithNode m_PermissionSetWithNode;
        protected IPermissionSetWithNode PermissionSetWithNode
        {
            get
            {
                if (m_PermissionSetWithNode == null)
                {
                    Type allSettingsType = typeof(AllSettings);

                    FieldInfo fieldInfo;
                    //string type = _Request.Get("type", Method.Get);
                    try
                    {
                        fieldInfo = allSettingsType.GetField(Type, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    }
                    catch
                    {
                        return null;
                    }
                    try
                    {
                        m_PermissionSetWithNode = (IPermissionSetWithNode)fieldInfo.GetValue(AllSettings.Current);
                    }
                    catch
                    { }
                }

                return m_PermissionSetWithNode;
            }
        }


        private List<IPermissionSet> m_UserPermissionSetList;

        protected List<IPermissionSet> UserPermissionSetList
        {
            get
            {
                if (m_UserPermissionSetList == null)
                {

                    List<IPermissionSet> permissionSets = new List<IPermissionSet>();

                    foreach (IPermissionSet permissionSet in SettingManager.GetPermissionSets())
                    {
                        if (permissionSet.IsManagement == false)
                            permissionSets.Add(permissionSet);
                    }

                    m_UserPermissionSetList = permissionSets;
                }

                return m_UserPermissionSetList;
            }
        }

        private List<IPermissionSet> m_ManagePermissionSetList;

        protected List<IPermissionSet> ManagePermissionSetList
        {
            get
            {
                if (m_ManagePermissionSetList == null)
                {

                    List<IPermissionSet> permissionSets = new List<IPermissionSet>();

                    foreach (IPermissionSet permissionSet in SettingManager.GetPermissionSets())
                    {
                        if (permissionSet.IsManagement)
                            permissionSets.Add(permissionSet);
                    }

                    m_ManagePermissionSetList = permissionSets;
                }

                return m_ManagePermissionSetList;
            }
        }

        private RoleCollection m_RoleList;

        protected RoleCollection RoleList
        {
            get
            {
                if (m_RoleList == null)
                {
                    if (PermissionSet.IsManagement)
                        m_RoleList = AllSettings.Current.RoleSettings.GetRoles(delegate(Role role) {
                            return role.IsManager;
                        });

                    else
                        m_RoleList = AllSettings.Current.RoleSettings.Roles;
                }
                return m_RoleList;
            }
        }

        private StringCollection m_PermissionItemNameList = null;

        protected StringCollection PermissionItemNameList
        {
            get
            {
                if (m_PermissionItemNameList == null)
                {

                    m_PermissionItemNameList = PermissionSet.GetPermissionItemNames();

                }

                return m_PermissionItemNameList;
            }
        }

        private StringCollection m_PermissionItemNameListWithTarget = null;

        protected StringCollection PermissionItemNameListWithTarget
        {
            get
            {
                if (m_PermissionItemNameListWithTarget == null)
                {

                    m_PermissionItemNameListWithTarget = PermissionSet.GetPermissionItemNamesWithTarget();

                }

                return m_PermissionItemNameListWithTarget;
            }
        }

        protected List<PermissionItem> GetPermissionItemList(Role role)
        {
            return PermissionSet.GetPermissionItems(role);
        }

        protected List<PermissionItem> GetPermissionItemListWithTarget(Role role)
        {
            return PermissionSet.GetPermissionItemsWithTarget(role);
        }

        protected bool CanDisplayPermissionItems
        {
            get
            {
                return PermissionSet.GetPermissionItemNames().Count > 0;
            }
        }

        protected bool CanDisplayPermissionItemsWithTarget
        {
            get
            {
                return PermissionSet.GetPermissionItemNamesWithTarget().Count > 0;
            }
        }

        protected bool CanDisplayPermissionLimit
        {
            get
            {
                return PermissionSet.IsManagement;
            }
        }

        protected bool CanSetDeny
        {
            get
            {
                return PermissionSet.CanSetDeny;
            }
        }

        protected int NodeID
        {
            get
            {
                return int.Parse(Parameters["nodeID"].ToString());
            }
        }

        protected string Action
        {
            get
            {
                if (Parameters["action"] != null)
                {
                    return Parameters["action"].ToString();
                }
                return string.Empty;
            }
        }

        protected string T
        {
            get
            {
#if !Passport
                if (Type == new ManageForumPermissionSet().TypeName)
                    return "manage";
                else
#endif
                    return "user";
            }
        }
        
    }
}