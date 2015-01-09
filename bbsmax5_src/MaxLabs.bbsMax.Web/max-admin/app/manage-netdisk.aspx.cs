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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_netdisk : AdminPageBase 
    {
        private DiskFileCollection m_diskFileList;

        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_NetDisk; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("search"))
            {
                Search();
            }
            else if (_Request.IsClick("delete"))
            {
                Delete();
            }

            m_diskFileList = DiskBO.Instance.AdminSearchFiles(MyUserID, Filter,  _Request.Get<int>("page", Method.Get, 1));
            WaitForFillSimpleUsers<DiskFile>(m_diskFileList);
        }

        private void Delete()
        {
            int[] fileids = _Request.GetList<int>("diskFileids",Method.Post,new int[0]);
            DiskBO.Instance.AdminDeleteDiskFiles( MyUserID, fileids);
        }

        private string m_NoPermissionManageRoleNames;
        protected string NoPermissionManageRoleNames
        {
            get
            {
                if (m_NoPermissionManageRoleNames == null)
                {
                    Guid[] roleIDs = AllSettings.Current.BackendPermissions.GetNoPermissionTargetRoleIds(MyUserID, BackendPermissions.ActionWithTarget.Manage_NetDisk);
                    m_NoPermissionManageRoleNames = RoleSettings.GetRoleNames(roleIDs, ",");
                }
                return m_NoPermissionManageRoleNames;
            }
        }

        protected bool HasNoPermissionManageRole
        {
            get
            {
                return NoPermissionManageRoleNames != string.Empty;
            }
        }


        private void Search()
        {
            DiskFileFilter filter = DiskFileFilter.GetFromForm();
            filter.Apply("filter", "page");
           
        }

        private DiskFileFilter m_filter;
        protected DiskFileFilter Filter
        {
            get
            {
                if (m_filter == null)
                {
                    m_filter=DiskFileFilter.GetFromFilter("filter");
                    if (m_filter.PageSize < 1)
                        m_filter.PageSize = Consts.DefaultPageSize;
                    if (m_filter.IsNull)
                    {
                        int userid = _Request.Get<int>("userid", Method.Get,0);
                        if(userid>0)
                            m_filter.UserID = userid;
                    }
                }
                return m_filter;
            }
        }


        protected DiskFileCollection DiskFileList
        {
            get
            {
                return m_diskFileList;
            }
        }
    }
}