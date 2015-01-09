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
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_blog : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Blog; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			m_AdminForm = AdminBlogArticleFilter.GetFromFilter("filter");
            int userid = _Request.Get<int>("userid", Method.Get, 0);
            if (userid > 0)
                m_AdminForm.AuthorID = userid;

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
                //DeletedSearchResult();
            }

			using (ErrorScope es = new ErrorScope())
			{
				int pageNumber = _Request.Get<int>("page", 0);

				m_ArticleListPageSize = m_AdminForm.PageSize;

				m_ArticleList = BlogBO.Instance.GetBlogArticlesForAdmin(MyUserID, m_AdminForm, pageNumber);

				if (m_ArticleList != null)
				{
					m_TotalArticleCount = m_ArticleList.TotalRecords;

					UserBO.Instance.WaitForFillSimpleUsers<BlogArticle>(m_ArticleList);
				}

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
						ShowError(error);
					});
				}
			}
        }

		private AdminBlogArticleFilter m_AdminForm;
		
		public AdminBlogArticleFilter AdminForm
		{
			get { return m_AdminForm; }
		}

		private BlogArticleCollection m_ArticleList;

		public BlogArticleCollection ArticleList
		{
			get
			{
				return m_ArticleList;
			}
		}

		private int m_ArticleListPageSize;

		public int ArticleListPageSize
		{
			get
			{
				return m_ArticleListPageSize;
			}
		}

		private int m_TotalArticleCount;

		public int TotalArticleCount
		{
			get
			{
				return m_TotalArticleCount;
			}
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


        private void SearchAlbums()
        {

            AdminBlogArticleFilter filter = AdminBlogArticleFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] articleIDs = StringUtil.Split<int>(_Request.Get("articleids", Method.Post));

                    if (articleIDs == null || articleIDs.Length == 0)
                    {
                        msgDisplay.AddError("请至少选择一条要删除的数据");
                        return;
                    }

                    bool success = BlogBO.Instance.DeleteBlogArticles(MyUserID, articleIDs, true);

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

			TaskManager.BeginTask(MyUserID, new DeleteBlogArticleTask(), param.ToString());
        }


    }
}