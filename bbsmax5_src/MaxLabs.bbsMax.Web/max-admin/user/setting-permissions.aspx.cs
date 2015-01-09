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
    public partial class setting_permissions : AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (_Request.IsClick("savepermission"))
            //    SavePermission();
        }

        /*
        private void SavePermission()
        {
            IPermissionSet permissionSet = (IPermissionSet)Activator.CreateInstance(PermissionSet.GetType());

            foreach (Role role in RoleSettings.Roles)
            {
                List<PermissionItem> permissionItems = new List<PermissionItem>();

                foreach (PermissionItem item in PermissionSet.GetPermissionItems(role))
                {
                    PermissionItem newItem = item.Clone();


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

                    permissionItems.Add(newItem);
                }

                List<PermissionItem> permissionItemsWithTarget = new List<PermissionItem>();

                foreach (PermissionItem item in PermissionSet.GetPermissionItemsWithTarget(role))
                {
                    PermissionItem newItem = item.Clone();

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

                    permissionItemsWithTarget.Add(newItem);
                }

                permissionSet.SetAllPermissionItems(role, permissionItems, permissionItemsWithTarget);

            }

            permissionSet.NodeID = NodeID;

            MessageDisplay msgDisplay = CreateMessageDisplay();

            if (PermissionSet.HasNodeList)
            {
                if (PermissionSetWithNode == null)
                {
                    msgDisplay.AddError(new InvalidParamError("type").Message);
                }
                else
                    PermissionSetWithNode.SaveSetting(permissionSet);
            }
            else
                SettingManager.SaveSettings((SettingBase)permissionSet);
        }
        */
        private IPermissionSet m_PermissionSet = null;

        protected IPermissionSet PermissionSet
        {
            get
            {
                if (m_PermissionSet == null)
                {
                    string type = _Request.Get("type", Method.Get);
                    m_PermissionSet = SettingManager.GetPermissionSet(type, NodeID);

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

        //private IPermissionSetWithNode m_PermissionSetWithNode;
        protected IPermissionSetWithNode GetPermissionSetWithNode(string type)
        {
            return SettingManager.GetPermissionWithNode(type);
        }

        private IPermissionSetWithNode m_PermissionSetWithNode;
        protected IPermissionSetWithNode PermissionSetWithNode
        {
            get
            {
                if (m_PermissionSetWithNode == null)
                {
                    IPermissionSet permissionSet = SettingManager.GetPermissionSet(Type, 0);

                    if (permissionSet == null)
                        return null;

                    if (permissionSet.HasNodeList == false)
                        return null;

                    Type allSettingsType = typeof(AllSettings);

                    PropertyInfo propertyInfo;
                    try
                    {
                        propertyInfo = allSettingsType.GetProperty(Type, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    }
                    catch
                    {
                        return null;
                    }
                    try
                    {
                        m_PermissionSetWithNode = (IPermissionSetWithNode)propertyInfo.GetValue(AllSettings.Current, null);
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

        //private RoleCollection m_RoleList;

        //protected RoleCollection RoleList
        //{
        //    get
        //    {
        //        if (m_RoleList == null)
        //        {
        //            if (PermissionSet.IsManagement)
        //                m_RoleList = AllSettings.Current.RoleSettings.GetRoles(delegate(Role role) {
        //                    return role.IsManager;
        //                });

        //            else
        //                m_RoleList = AllSettings.Current.RoleSettings.Roles;
        //        }
        //        return m_RoleList;
        //    }
        //}

        //private StringCollection m_PermissionItemNameList = null;

        //protected StringCollection PermissionItemNameList
        //{
        //    get
        //    {
        //        if (m_PermissionItemNameList == null)
        //        {

        //            m_PermissionItemNameList = PermissionSet.GetPermissionItemNames();

        //        }

        //        return m_PermissionItemNameList;
        //    }
        //}

        //private StringCollection m_PermissionItemNameListWithTarget = null;

        //protected StringCollection PermissionItemNameListWithTarget
        //{
        //    get
        //    {
        //        if (m_PermissionItemNameListWithTarget == null)
        //        {

        //            m_PermissionItemNameListWithTarget = PermissionSet.GetPermissionItemNamesWithTarget();

        //        }

        //        return m_PermissionItemNameListWithTarget;
        //    }
        //}

        //protected List<PermissionItem> GetPermissionItemList(Role role)
        //{
        //    return PermissionSet.GetPermissionItems(role);
        //}

        //protected List<PermissionItem> GetPermissionItemListWithTarget(Role role)
        //{
        //    return PermissionSet.GetPermissionItemsWithTarget(role);
        //}

        //protected bool CanDisplayPermissionItems
        //{
        //    get
        //    {
        //        return PermissionSet.GetPermissionItemNames().Count > 0;
        //    }
        //}

        //protected bool CanDisplayPermissionItemsWithTarget
        //{
        //    get
        //    {
        //        return PermissionSet.GetPermissionItemNamesWithTarget().Count > 0;
        //    }
        //}

        protected int NodeID
        {
            get
            {
                return _Request.Get<int>("nodeID", Method.Get, 0);
            }
        }

        protected string Type
        {
            get
            {
                return _Request.Get("type", Method.Get,string.Empty);
            }
        }
        
    }
}