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
    public partial class manage_impressiontype : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_ImpressionType; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_AdminForm = AdminImpressionTypeFilter.GetFromFilter("filter");

			if (_Request.IsClick("advancedsearch"))
			{
				SearchTypes();
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

				m_TypeListPageSize = m_AdminForm.PageSize;

                m_TypeList = ImpressionBO.Instance.GetImpressionTypesForAdmin(My, m_AdminForm, pageNumber);

				if (m_TypeList != null)
				{
					m_TotalTypeCount = m_TypeList.TotalRecords;
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

        private AdminImpressionTypeFilter m_AdminForm;

        public AdminImpressionTypeFilter AdminForm
		{
			get { return m_AdminForm; }
		}

        private ImpressionTypeCollection m_TypeList;

		public ImpressionTypeCollection TypeList
		{
			get { return m_TypeList; }
		}

		private int m_TypeListPageSize;

		public int TypeListPageSize
		{
			get { return m_TypeListPageSize; }
		}

		private int m_TotalTypeCount;

		public int TotalTypeCount
		{
			get { return m_TotalTypeCount; }
		}

        private void SearchTypes()
        {
            AdminImpressionTypeFilter filter = AdminImpressionTypeFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteSearch()
		{
			StringList param = new StringList();

			param.Add(m_AdminForm.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));
			param.Add(_Request.Get("deleteArticle", Method.Post, "1"));

            TaskManager.BeginTask(MyUserID, new DeleteImpressionTypeTask(), param.ToString());
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] typeIDs = StringUtil.Split<int>(_Request.Get("typeids", Method.Post));

                    if (typeIDs == null || typeIDs.Length == 0)
                    {
                        msgDisplay.AddError("请至少选择一条要删除的数据");
                        return;
                    }

                    bool success = ImpressionBO.Instance.DeleteImpressionTypes(My, typeIDs);

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