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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.StepByStepTasks.Types;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_blogcategory : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Blog; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			m_AdminForm = AdminBlogCategoryFilter.GetFromFilter("filter");

			if (_Request.IsClick("advancedsearch"))
			{
				SearchCategories();
			}
			else if (_Request.IsClick("deletechecked"))
			{
				DeleteChecked();
			}
			else if (_Request.IsClick("deletesearched"))
			{
				DeleteSearch();
			}

			using (ErrorScope es = new ErrorScope())
			{
				int pageNumber = _Request.Get<int>("page", 0);

				m_CategoryListPageSize = m_AdminForm.PageSize;

				m_CategoryList = BlogBO.Instance.GetBlogCategoriesForAdmin(MyUserID, m_AdminForm, pageNumber);

				if (m_CategoryList != null)
				{
					m_TotalCategoryCount = m_CategoryList.TotalRecords;

					UserBO.Instance.WaitForFillSimpleUsers<BlogCategory>(m_CategoryList);
				}

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
						ShowError(error);
					});
				}
			}
		}

		private AdminBlogCategoryFilter m_AdminForm;

		public AdminBlogCategoryFilter AdminForm
		{
			get { return m_AdminForm; }
		}

		private BlogCategoryCollection m_CategoryList;

		public BlogCategoryCollection CategoryList
		{
			get { return m_CategoryList; }
		}

		private int m_CategoryListPageSize;

		public int CategoryListPageSize
		{
			get { return m_CategoryListPageSize; }
		}

		private int m_TotalCategoryCount;

		public int TotalCategoryCount
		{
			get { return m_TotalCategoryCount; }
		}

        private string m_NoPermissionManageRoleNames;
        protected string NoPermissionManageRoleNames
        {
            get
            {
                if (m_NoPermissionManageRoleNames == null)
                {
                    Guid[] roleIDs = BlogBO.Instance.ManagePermission.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
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

        private void SearchCategories()
        {

            AdminBlogCategoryFilter filter = AdminBlogCategoryFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteSearch()
		{
			StringList param = new StringList();

			param.Add(m_AdminForm.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));
			param.Add(_Request.Get("deleteArticle", Method.Post, "1"));

			TaskManager.BeginTask(MyUserID, new DeleteBlogCategoryTask(), param.ToString());
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] categoryIDs = StringUtil.Split<int>(_Request.Get("categoryids", Method.Post));

                    if (categoryIDs == null || categoryIDs.Length == 0)
                    {
                        msgDisplay.AddError("请至少选择一条要删除的数据");
                        return;
                    }

					bool isUpdatePint = (_Request.Get("updatePoint", Method.Post) == "1");
					bool isDeleteArticle = (_Request.Get("deleteArticle", Method.Post) == "1");

                    bool success = BlogBO.Instance.DeleteBlogCategories(MyUserID,categoryIDs, isDeleteArticle, isUpdatePint);

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
    }
}