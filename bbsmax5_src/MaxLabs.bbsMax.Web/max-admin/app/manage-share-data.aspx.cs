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
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Filters;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_share_data : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Share; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			m_Filter = ShareFilter.GetFromFilter("filter");

			if (_Request.Get("type") != "share")
				m_Filter.PrivacyType = PrivacyType.SelfVisible;

            if (_Request.IsClick("searchshares"))
                SearchShares();
            else if (_Request.IsClick("deletechecked"))
            {
                DeleteChecked();
            }
            else if (_Request.IsClick("deleteallsearch"))
            {
                DeleteAllSearch();
			}

			int page = _Request.Get<int>("page", Method.Get, 1);

			m_ShareList = ShareBO.Instance.GetSharesForAdmin(MyUserID, m_Filter, page);

			if (m_ShareList != null)
			{
				m_ShareTotalCount = m_ShareList.TotalRecords;

				UserBO.Instance.WaitForFillSimpleUsers<Share>(m_ShareList, 0);
			}
        }

		private ShareFilter m_Filter;

		public ShareFilter Filter
		{
            get
            {
                if (m_Filter.IsNull)
                {
                    int userid = _Request.Get<int>("userid", Method.Get, 0);
                    if (userid > 0)
                    {
                        m_Filter.UserID = userid;
                    }
                }
                return m_Filter;
            }
		}

		private int m_ShareTotalCount;

		public int ShareTotalCount
		{
			get { return m_ShareTotalCount; }
		}

		private ShareCollection m_ShareList;

		public ShareCollection ShareList
		{
			get { return m_ShareList; }
		}

        private string m_NoPermissionManageRoleNames;
        protected string NoPermissionManageRoleNames
        {
            get
            {
                if (m_NoPermissionManageRoleNames == null)
                {
                    Guid[] roleIDs = ShareBO.Instance.ManagePermission.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
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

        private void SearchShares()
        {
            ShareFilter filter = ShareFilter.GetFromForm();

			if (_Request.Get("type") != "share")
				filter.PrivacyType = PrivacyType.SelfVisible;

            filter.Apply("filter", "page");
        }

        private void DeleteChecked()
        {
            int[] shareIDs = StringUtil.Split<int>(_Request.Get("shareids", Method.Post, string.Empty));
            bool updatePoint = (_Request.Get("updatePoint", Method.Post) == "1");

            MessageDisplay msgDisplay = CreateMessageDisplay();


            if (shareIDs == null || shareIDs.Length == 0)
            {
                msgDisplay.AddError("请至少选择一条要删除的数据");
                return;
            }

            try
            {
                using (new ErrorScope())
                {
                    bool success = false;

                    if (_Request.Get("type") != "share")
                        success = FavoriteBO.Instance.DeleteShares(MyUserID, shareIDs, updatePoint);
                    else
                        success = ShareBO.Instance.DeleteShares(MyUserID, shareIDs, updatePoint);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

        }

        private void DeleteAllSearch()
		{
			StringList param = new StringList();

			param.Add(m_Filter.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));
            param.Add(_Request.Get("type"));

			TaskManager.BeginTask(MyUserID, new DeleteShareTask(), param.ToString());
        }
    }
}