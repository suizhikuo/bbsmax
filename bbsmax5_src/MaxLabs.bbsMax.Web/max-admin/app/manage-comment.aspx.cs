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


using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.StepByStepTasks;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_comment : AdminPageBase
    {
        protected override BackendPermissions.ActionWithTarget BackedPermissionActionWithTarget
        {
            get { return BackendPermissions.ActionWithTarget.Manage_Comment; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			m_Filter = AdminCommentFilter.GetFromFilter("filter");

            if (_Request.IsClick("deletecomment"))
                Delete();
            else if (_Request.IsClick("searchcomment"))
                Search();
            else if (_Request.IsClick("approvecomment"))
                Approve();
            else if (_Request.IsClick("deletesearch"))
                DeleteBySearch();

			using (ErrorScope es = new ErrorScope())
			{
				int pageNumber = _Request.Get<int>("page", 0);

				m_CommentList = CommentBO.Instance.GetCommentsForAdmin(MyUserID, m_Filter, pageNumber);

				if (m_CommentList != null)
				{
					m_CommentTotalCount = m_CommentList.TotalRecords;

					UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);
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

		private AdminCommentFilter m_Filter;

		public AdminCommentFilter Filter
		{
			get { return m_Filter; }
		}

		private CommentCollection m_CommentList;

		public CommentCollection CommentList
		{
			get
			{
				return m_CommentList;
			}
		}

		private int m_CommentTotalCount;

		public int CommentTotalCount
		{
			get
			{
				return m_CommentTotalCount;
			}
		}

        private string m_NoPermissionManageRoleNames;
        protected string NoPermissionManageRoleNames
        {
            get
            {
                if (m_NoPermissionManageRoleNames == null)
                {
                    Guid[] roleIDs = CommentBO.Instance.ManagePermission.GetNoPermissionTargetRoleIds(My, PermissionTargetType.Content);
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

        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = StringUtil.Split<int>(_Request.Get("commentid", Method.All, ""), ',');
            bool updatePoint = (_Request.Get("updatePoint", Method.Post) == "1");

            if (ids == null || ids.Length == 0)
            {
                msgDisplay.AddError("请至少选择一条要删除的数据");
                return;
            }

            try
            {
                CommentBO.Instance.RemoveComments(MyUserID, ids, updatePoint);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void DeleteBySearch()
		{
			StringList param = new StringList();

			param.Add(m_Filter.ToString());
			param.Add(_Request.Get("updatePoint", Method.Post, "1"));

			TaskManager.BeginTask(MyUserID, new DeleteCommentTask(), param.ToString());
        }

        private void Search()
        {
            AdminCommentFilter filter = AdminCommentFilter.GetFromForm();
            filter.Apply("filter", "page");
        }

        public void Approve()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            int[] ids = StringUtil.Split<int>(_Request.Get("CommentID", Method.All, ""), ',');

            try
            {
                CommentBO.Instance.ApproveComments(MyUserID,ids);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }
    }
}