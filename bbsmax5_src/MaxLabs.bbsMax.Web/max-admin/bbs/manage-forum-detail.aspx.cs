//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_forum_detail : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                if (IsEditManagePermission)
                    return BackendPermissions.Action.Manage_ForumDetail_EditManagePermission;
                else if (IsEditUsePermission)
                    return BackendPermissions.Action.Manage_ForumDetail_EditUserPermission;
                else if (IsEditSetting)
                    return BackendPermissions.Action.Manage_ForumDetail_EditSetting;
                else if (IsEditRate)
                    return BackendPermissions.Action.Manage_ForumDetail_EditRate;
                else if (IsEditPoint)
                    return BackendPermissions.Action.Manage_ForumDetail_EditPoint;
                else
                    return BackendPermissions.Action.Manage_Forum;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (false == AllSettings.Current.ManageOtherPermissionSet.Can(My, MaxLabs.bbsMax.Settings.ManageOtherPermissionSet.Action.ManageForum))
            //{
            //    ShowError("您没有权限进行该操作");
            //}

            //if (_Request.IsClick("saveforum"))
            //    SaveForum();


            if (IsCreateForum == false && Forum == null && (IsCreateForum || IsEditForum))
            {
                ShowError(new InvalidParamError("forumid"));
            }

        }

        protected bool ShowGloable
        {
            get
            {
                if (IsCreateForum || IsEditForum)
                    return false;
                else
                    return true;
            }
        }

        private bool? m_IsEditForum;
        protected bool IsEditForum
        {
            get
            {
                if (m_IsEditForum == null)
                {
                    setAction();
                }
                return m_IsEditForum.Value;
            }
        }

        private bool? m_IsEditPermission;
        protected bool IsEditPermission
        {
            get
            {
                if (m_IsEditPermission == null)
                    setAction();
                return m_IsEditPermission.Value;
            }
        }

        private bool? m_IsEditPoint;
        protected bool IsEditPoint
        {
            get
            {
                if (m_IsEditPoint == null)
                    setAction();
                return m_IsEditPoint.Value;
            }
        }

        private bool? m_IsEditSetting;
        protected bool IsEditSetting
        {
            get
            {
                if (m_IsEditSetting == null)
                    setAction();
                return m_IsEditSetting.Value;
            }
        }

        private bool? m_IsCreateForum;
        protected bool IsCreateForum
        {
            get
            {
                if (m_IsCreateForum == null)
                    setAction();
                return m_IsCreateForum.Value;
            }
        }

        private bool? m_IsEditUsePermission;
        protected bool IsEditUsePermission
        {
            get
            {
                if (m_IsEditUsePermission == null)
                    setAction();
                return m_IsEditUsePermission.Value;
            }
        }
        private bool? m_IsEditManagePermission;
        protected bool IsEditManagePermission
        {
            get
            {
                if (m_IsEditManagePermission == null)
                    setAction();
                return m_IsEditManagePermission.Value;
            }
        }
        private bool? m_IsEditRate;
        protected bool IsEditRate
        {
            get
            {
                if (m_IsEditRate == null)
                    setAction();
                return m_IsEditRate.Value;
            }
        }

        private void setAction()
        {
            m_IsEditForum = false;
            m_IsEditPermission = false;
            m_IsEditPoint = false;
            m_IsEditSetting = false;
            m_IsCreateForum = false;
            m_IsEditUsePermission = false;
            m_IsEditManagePermission = false;
            m_IsEditRate = false;
            string action = _Request.Get("action", Method.Get, string.Empty);
            if (string.Compare(action, "editforum", true) == 0)
            {
                m_IsEditForum = true;
            }
            else if (string.Compare(action, "editpermission", true) == 0)
            {
                m_IsEditPermission = true;
            }
            else if (string.Compare(action, "editpoint", true) == 0)
            {
                m_IsEditPoint = true;
            }
            else if (string.Compare(action, "editsetting", true) == 0)
            {
                m_IsEditSetting = true;
            }
            else if (string.Compare(action, "editUsePermission", true) == 0)
            {
                m_IsEditUsePermission = true;
            }
            else if (string.Compare(action, "editManagePermission", true) == 0)
            {
                m_IsEditManagePermission = true;
            }
            else if (string.Compare(action, "editrate", true) == 0)
            {
                m_IsEditRate = true;
            }
            else
                m_IsCreateForum = true;
        }
  

        private Forum forum;
        protected Forum Forum
        {
            get
            {
                if (IsCreateForum == false)
                {
                    if (forum == null)
                    {
                        int forumID = _Request.Get<int>("forumID", Method.Get, 0);
                        forum = ForumBO.Instance.GetForum(forumID, false);
                    }
                    return forum;
                }
                return null;
            }
        }

        protected int ForumID
        {
            get
            {
                if (Forum == null)
                    return 0;
                else
                    return Forum.ForumID;
            }
        }

        protected string TreeAction
        {
            get
            {
                if (IsCreateForum)
                    return "editforum";
                else
                    return _Request.Get("action",Method.Get,string.Empty);
            }
        }

        protected string GetLinkClass(string param,string action)
        {
            if (string.Compare(param.Trim(), action.Trim(), true) == 0)
                return "class=\"current\"";
            else
                return string.Empty;
        }

        protected string GetForumsTree(string style1, string style2)
        {
            return ForumBO.Instance.BuildForumsTreeHtml(0, style1, delegate(Forum forum)
            {
                string linkClass = " class=\"current\" ";
                string color = "color:#999;";
                if (CustomForumIDs != null && CustomForumIDs.Contains(forum.ForumID))
                {
                    color = string.Empty;
                }
                if (forum.ForumID != ForumID)
                    linkClass = string.Empty;

                return string.Format(style2, forum.ForumID, forum.ForumName, "{0}", linkClass, color);
            });
        }

        private List<int> m_CustomForumIDs;
        protected List<int> CustomForumIDs
        {
            get
            {
                if (m_CustomForumIDs == null)
                {
                    m_CustomForumIDs = new List<int>();
                    if (IsEditSetting)
                    {
                        foreach (ForumSettingItem item in AllSettings.Current.ForumSettings.Items)
                        {
                            if (item.ForumID > 0)
                                m_CustomForumIDs.Add(item.ForumID);
                        }
                    }
                    else if (IsEditManagePermission)
                    {
                        foreach (ManageForumPermissionSetNode node in AllSettings.Current.ManageForumPermissionSet.Nodes)
                        {
                            if (node.NodeID > 0)
                                m_CustomForumIDs.Add(node.NodeID);
                        }
                    }
                    else if (IsEditUsePermission)
                    {
                        foreach (ForumPermissionSetNode node in AllSettings.Current.ForumPermissionSet.Nodes)
                        {
                            if (node.NodeID > 0)
                                m_CustomForumIDs.Add(node.NodeID);
                        }
                    }
                    else if (IsEditPoint)
                    {
                        foreach (PointAction action in AllSettings.Current.PointActionSettings.PointActions)
                        {
                            if (action.NodeID > 0 && m_CustomForumIDs.Contains(action.NodeID) == false)
                                m_CustomForumIDs.Add(action.NodeID);
                        }
                    }
                    else if (IsEditRate)
                    {
                        foreach (RateSet rateSet in AllSettings.Current.RateSettings.RateSets)
                        {
                            if (rateSet.NodeID > 0)
                                m_CustomForumIDs.Add(rateSet.NodeID);
                        }
                    }
                    else
                        m_CustomForumIDs = null;

                }
                return m_CustomForumIDs;
            }
        }

    }
}