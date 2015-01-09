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
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_photo : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Album; }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
			m_AdminForm = AdminPhotoFilter.GetFromFilter("filter");
            if (m_AdminForm.IsNull)
            {
                int albumID = _Request.Get<int>("albumID", Method.Get, 0);
                if (albumID > 0)
                {
                    m_AdminForm.AlbumID = albumID;
                }
            }

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

			int page = _Request.Get<int>("page", Method.Get, 1);

			m_PhotoList = AlbumBO.Instance.GetPhotosForAdmin(MyUserID, m_AdminForm, page);

			if (m_PhotoList != null)
			{
				m_PhotoTotalCount = m_PhotoList.TotalRecords;

				UserBO.Instance.WaitForFillSimpleUsers<Photo>(m_PhotoList);
			}
		}

		private AdminPhotoFilter m_AdminForm;

		public AdminPhotoFilter AdminForm
		{
			get { return m_AdminForm; }
		}

		private int m_PhotoTotalCount;

		public int PhotoTotalCount
		{
			get { return m_PhotoTotalCount; }
		}

		private PhotoCollection m_PhotoList;

		public PhotoCollection PhotoList
		{
			get { return m_PhotoList; }
		}

		private string m_NoPermissionManageRoleNames;
		protected string NoPermissionManageRoleNames
		{
			get
			{
				if (m_NoPermissionManageRoleNames == null)
				{
                    Guid[] roleIDs = AlbumBO.Instance.ManagePermission.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
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

            AdminPhotoFilter filter = AdminPhotoFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] photoIDs = StringUtil.Split<int>(_Request.Get("photoids", Method.Post));

                    if (photoIDs == null || photoIDs.Length == 0)
                    {
                        msgDisplay.AddError("请至少选择一条要删除的数据");
                        return;
                    }

					bool isUpdatePoint = (_Request.Get("updatePoint", Method.Post) == "1");

                    bool success = AlbumBO.Instance.DeletePhotos(MyUserID, photoIDs, isUpdatePoint);
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

        private void DeleteSearched()
		{
			StringList param = new StringList();

			param.Add(m_AdminForm.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));

			TaskManager.BeginTask(MyUserID, new DeletePhotoTask(), param.ToString());
        }
    }
}