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
    public partial class manage_impressionrecord : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_ImpressionRecord; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int? typeid = _Request.Get<int>("tid", Method.Get);

            if (typeid != null)
            {
                AdminImpressionRecordFilter filter = AdminImpressionRecordFilter.GetFromForm();

                filter.TypeID = typeid.Value;

                UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();

                scheme.RemoveQuery("tid");
                scheme.AttachQuery("filter", filter.ToString());
                scheme.AttachQuery("page", "1");

                HttpContext.Current.Response.Redirect(scheme.ToString());
            }

            m_AdminForm = AdminImpressionRecordFilter.GetFromFilter("filter");

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

				m_RecordListPageSize = m_AdminForm.PageSize;

                m_RecordList = ImpressionBO.Instance.GetImpressionRecordsForAdmin(My, m_AdminForm, pageNumber);

				if (m_RecordList != null)
				{
					m_TotalRecordCount = m_RecordList.TotalRecords;
				}

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
						ShowError(error);
					});
				}

                UserBO.Instance.WaitForFillSimpleUsers<ImpressionRecord>(m_RecordList, 0);
			}
		}

        private AdminImpressionRecordFilter m_AdminForm;

        public AdminImpressionRecordFilter AdminForm
		{
			get { return m_AdminForm; }
		}

        private ImpressionRecordCollection m_RecordList;

        public ImpressionRecordCollection RecordList
		{
			get { return m_RecordList; }
		}

		private int m_RecordListPageSize;

		public int RecordListPageSize
		{
			get { return m_RecordListPageSize; }
		}

		private int m_TotalRecordCount;

		public int TotalRecordCount
		{
			get { return m_TotalRecordCount; }
		}

        private void SearchTypes()
        {
            AdminImpressionRecordFilter filter = AdminImpressionRecordFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private void DeleteSearch()
		{
			StringList param = new StringList();

			param.Add(m_AdminForm.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));
			param.Add(_Request.Get("deleteArticle", Method.Post, "1"));

            TaskManager.BeginTask(MyUserID, new DeleteImpressionRecordTask(), param.ToString());
        }

        private void DeleteChecked()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                try
                {
                    int[] recordIDs = StringUtil.Split<int>(_Request.Get("recordids", Method.Post));

                    if (recordIDs == null || recordIDs.Length == 0)
                    {
                        msgDisplay.AddError("请至少选择一条要删除的数据");
                        return;
                    }

                    bool success = ImpressionBO.Instance.DeleteImpressionRecords(My, recordIDs);

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