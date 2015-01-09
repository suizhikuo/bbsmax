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

using MaxLabs.bbsMax.Filters;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_user: AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_User; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (_Request.IsClick("searchusers"))
            {
                SeachUser();
            }
            else if (_Request.IsClick("delete"))
            {
                Operation("delete");
            }
            else if (_Request.IsClick("active"))
            {
                Operation("active");
            }
        }

        protected bool CanEdit(int targetUserID)
        {
            return UserBO.Instance.CanEditUserProfile(My, targetUserID);
        }

        private void SeachUser()
        {
            AdminUserFilter filter;
            filter = AdminUserFilter.GetFromForm();

            string time = _Request.Get("regdate_1", Method.Post);
            if (!string.IsNullOrEmpty(time))
            {
                filter.RegisterDate_1 = DateTimeUtil.ParseBeginDateTime(time);
            }

            time = _Request.Get("regdate_2", Method.Post);
            if (!string.IsNullOrEmpty(time))
            {
                filter.RegisterDate_2 = DateTimeUtil.ParseEndDateTime(time);
            }

            time = _Request.Get("visitdate_1", Method.Post);
            if (!string.IsNullOrEmpty(time))
            {
                filter.LastVisitDate_1 = DateTimeUtil.ParseBeginDateTime(time);
            }
            time = _Request.Get("visitdate_2", Method.Post);
            if (!string.IsNullOrEmpty(time))
            {
                filter.LastVisitDate_2 = DateTimeUtil.ParseEndDateTime(time);
            }
            
            filter.Apply("filter","page", "mode",_Request.Get("mode", Method.Post));
        }

        private void Operation(string operationType)
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            string selectedIDs;
            int[] userIDs;

            selectedIDs   = _Request.Get("userids", Method.Post);
            userIDs       = StringUtil.Split<int>(selectedIDs);

            switch (operationType)
            {
                case "delete":
                    //UserBO.Instance.DeleteUsers( MyUserID, userIDs);
                        break;
                case "active":
                        UserBO.Instance.AdminActivingUsers(My, userIDs, true);
                        break;
                case "emailvalidated":
                        break;
                default:
                        break;
            }

            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error) {
                    msgDisplay.AddError(error);
                });
            }
        }

        protected string NoPermissionRoleList
        {
            get
            {
                Guid[] roleIds;
                roleIds = AllSettings.Current.ManageUserPermissionSet.GetNoPermissionTargetRoleIds(My, PermissionTargetType.User);
                string Roles = AllSettings.Current.RoleSettings.GetRoleNames(roleIds," ");
                return Roles;
            }
        }

        protected bool HasNoPermissionRole
        {
            get
            {
                Guid[] roles = AllSettings.Current.ManageUserPermissionSet.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
                return roles != null && roles.Length > 0;
            }
        }
    }
}