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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public class manage_userprop : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Manage_Prop;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
			m_AdminForm = UserPropFilter.GetFromFilter("filter");

            int pageNumber = _Request.Get<int>("page", 1);

            if(_Request.IsClick("deletechecked"))
            {
                int[] ids = StringUtil.Split<int>(_Request.Get("PropIDs", MaxLabs.WebEngine.Method.Post, string.Empty));

                if (ids != null && ids.Length > 0)
                {
                    PropBO.Instance.DeleteUserPropsForAdmin(My, ids);

                    JumpTo("interactive/manage-userprop.aspx");
                }
            }
            else if(_Request.IsClick("advancedsearch"))
            {
                UserPropFilter filter = UserPropFilter.GetFromForm();

                filter.Apply("filter", "page");
            }

            m_PropList = PropBO.Instance.GetUserPropsForAdmin(My, m_AdminForm, pageNumber);

            m_TotalPropCount = m_PropList.TotalRecords;

            UserBO.Instance.WaitForFillSimpleUsers<UserProp>(m_PropList);
        }

		private UserPropFilter m_AdminForm;

		public UserPropFilter AdminForm
		{
			get { return m_AdminForm; }
		}

        private UserPropCollection m_PropList;

        public UserPropCollection PropList
        {
            get { return m_PropList; }
        }

        private int m_TotalPropCount;

        public int TotalPropCount
        {
            get { return m_TotalPropCount; }
        }
    }
}