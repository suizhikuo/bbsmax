//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;


using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_doing : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Doing; }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
			if (_Request.IsClick("searchdoing"))
				Search();
			else if (_Request.IsClick("deletedoing"))
				DeleteBySelect();
			else if (_Request.IsClick("deletesearch"))
				DeleteBySearch();

			int pageNumber = _Request.Get<int>("page", 0);

			using (ErrorScope es = new ErrorScope())
			{
				m_DoingList = DoingBO.Instance.GetDoingsForAdmin(MyUserID, Filter, pageNumber);

				if (m_DoingList != null)
				{
					UserBO.Instance.WaitForFillSimpleUsers<Doing>(m_DoingList);

					m_DoingTotalCount = m_DoingList.TotalRecords;
				}

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
						ShowError(error);
					});
				}
			}
		}

        private string m_NoPermissionManageRoleNames;
        protected string NoPermissionManageRoleNames
        {
            get
            {
                if (m_NoPermissionManageRoleNames == null)
                {
                    Guid[] roleIDs = DoingBO.Instance.ManagePermission.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
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

		private DoingCollection m_DoingList;

		public DoingCollection DoingList
		{
			get { return m_DoingList; }
		}

		private int m_DoingTotalCount;

		public int DoingTotalCount
		{
			get { return m_DoingTotalCount; }
		}

		private AdminDoingFilter m_Filter;

        /// <summary>
        /// doing搜索过滤器
        /// </summary>
        protected AdminDoingFilter Filter
        {
            get 
			{
                if (m_Filter == null)
                {
                    m_Filter = AdminDoingFilter.GetFromFilter("filter");
                    if (m_Filter.IsNull)
                    {
                        int userid = _Request.Get<int>("userid", Method.Get, 0);
                        if (userid > 0)
                        {
                            m_Filter.UserID = userid;
                        }
                    }
                }
				return m_Filter;
			}
        }


        /// <summary>
        /// 搜集过滤器数据并应用，此时会立即发生页面跳转
        /// </summary>
        private void Search()
        {
            AdminDoingFilter filter = AdminDoingFilter.GetFromForm();
            filter.Apply("filter", "page");
        }

        private void DeleteBySelect()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = StringUtil.Split<int>(_Request.Get("DoingID", Method.All, ""), ',');

			bool updatePoint = _Request.Get("updatePoint", Method.Post, "1") == "1";


            if (ids == null || ids.Length == 0)
            {
                msgDisplay.AddError("请至少选择一条要删除的数据");
                return;
            }

            try
            {
                DoingBO.Instance.DeleteDoings(MyUserID, ids, updatePoint);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void DeleteBySearch()
		{
			StringList param = new StringList();

			param.Add(Filter.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));

			TaskManager.BeginTask(MyUserID, new DeleteDoingTask(), param.ToString());
        }
    }
}