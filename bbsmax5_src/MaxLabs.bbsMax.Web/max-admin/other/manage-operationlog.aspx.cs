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

using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
	public partial class manage_operationlog : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_OperationLog; }
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_Request.IsClick("search"))
			{
				m_Filter = OperationLogFilter.GetFromForm();

				m_Filter.Apply("filter", "page");
			}

			m_Filter = OperationLogFilter.GetFromFilter("filter");

			int page = _Request.Get<int>("page", 0);

			m_OperationLogList = Logs.LogManager.GetOperationLogsBySearch(MyUserID, page, m_Filter);

			m_OperationLogTotalCount = m_OperationLogList.TotalRecords;
		}

		private OperationLogFilter m_Filter;

		public OperationLogFilter Filter
		{
			get { return m_Filter; }
		}

		private OperationLogCollection m_OperationLogList;

		public OperationLogCollection OperationLogList
		{
			get { return m_OperationLogList; }
		}

		private int m_OperationLogTotalCount;

		public int OperationLogTotalCount
		{
			get { return m_OperationLogTotalCount; }
		}

		public OperationTypeInfo[] OperationTypeList
		{
			get { return Logs.LogManager.GetOperationTypeInfos(); }
		}
	}
}