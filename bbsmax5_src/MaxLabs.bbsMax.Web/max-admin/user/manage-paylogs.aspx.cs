//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_paylogs : AdminPageBase
    {
        protected override Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return Settings.BackendPermissions.Action.Setting_PayLogs;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!My.IsOwner)
            {
                ShowError("你没有权限进入该页面！");
                return;
            }

            FillSimpleUsers<UserPay>(this.UserPayList);

            if (_Request.IsClick("search"))
            {
                PaylogFilter filter = PaylogFilter.GetFromForm();
                filter.Apply("filter", "page");
            }
        }

        protected int Pagesize
        {
            get
            {
                return 20;
            }
        }

        private PaylogFilter m_Filter;
        protected PaylogFilter Filter
        {
            get
            {
                if (m_Filter == null)
                {
                    m_Filter = PaylogFilter.GetFromFilter("filter");

                    if (m_Filter.IsNull)
                        m_Filter.State = 1;
                }
                return m_Filter;
            }
        }

        private UserPayCollection m_UserPayList;
        protected UserPayCollection UserPayList
        {
            get
            {
                if (m_UserPayList == null)
                {
                    int pageSize = 20;
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);

                    m_UserPayList = PayBO.Instance.AdminSearchUserPays(My, Filter, pageSize, pageNumber);
                }

                return m_UserPayList;
            }
        }
    }
}