//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_notify : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Notify; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (_Request.IsClick("advancedsearch"))
            {
                SearchAlbums();
            }
            else if (_Request.IsClick("deletechecked"))
            {
                DeleteChecked();
            }
            else if (_Request.IsClick("deletesearched"))
            {
                DeleteSearched();
            }

            WaitForFillSimpleUsers<Notify>(this.NotifyList);
		}

        protected string GetTypename( int typeid )
        {
            return this.NotifyTypeList[typeid].TypeName;
        }


		private string m_NoPermissionManageRoleNames;
		protected string NoPermissionManageRoleNames
		{
			get
			{
				if (m_NoPermissionManageRoleNames == null)
				{
					Guid[] roleIDs = NotifyBO.Instance.ManagePermission.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
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

        private void SearchAlbums()
        {

            AdminNotifyFilter filter = AdminNotifyFilter.GetFromForm();

            filter.Apply("filter", "page");
        }


        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] notifyIDs = StringUtil.Split<int>(_Request.Get("notifyids", Method.Post));

                    bool success = NotifyBO.Instance.DeleteNotifies(MyUserID, notifyIDs);
                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }

        private NotifyCollection m_NotifyList;
        protected NotifyCollection NotifyList
        {
            get
            {
                if(m_NotifyList==null)
                {
                    int pageNumber=_Request.Get<int>("page",Method.Get,1);
                    m_NotifyList = NotifyBO.Instance.AdminGetNotifiesBySearch( MyUserID, this.NotifyFilter, pageNumber);
                }
                return m_NotifyList;
            }
        }

        private AdminNotifyFilter m_NotifyFilter;
        protected AdminNotifyFilter NotifyFilter
        {
            get
            {
                if(m_NotifyFilter==null)
                m_NotifyFilter = AdminNotifyFilter.GetFromFilter("filter");
                if (m_NotifyFilter.IsNull)
                {
                    int userid = _Request.Get<int>("userid", Method.Get, 0);
                    if (userid > 0)
                    {
                        SimpleUser user = UserBO.Instance.GetSimpleUser(userid);
                        m_NotifyFilter.Owner = user.Username;
                    }
                }
                return m_NotifyFilter;
            }
        }

        private void DeleteSearched()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    AdminNotifyFilter notifyFilter = AdminNotifyFilter.GetFromFilter("filter");

                    if (notifyFilter != null)
                    {

                        bool success = NotifyBO.Instance.DeleteNotifiesBySearch(MyUserID, notifyFilter);

                        if (!success)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            });
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }

    }
}