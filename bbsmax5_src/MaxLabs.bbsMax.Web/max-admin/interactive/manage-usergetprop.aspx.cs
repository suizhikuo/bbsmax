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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public partial class manage_usergetprop : AdminPageBase
    {
        protected override Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return Settings.BackendPermissions.Action.Manage_PropGet;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("search"))
            {
                m_Filter = UserGetPropFilter.GetFromForm();

                m_Filter.Apply("filter", "page");
            }

            m_Filter = UserGetPropFilter.GetFromFilter("filter");

            int page = _Request.Get<int>("page", 0);
            m_Collection = LogManager.GetUserGetPropCollection(Filter, page);
            m_TotalCount = m_Collection.TotalRecords;
        }

        private UserGetPropFilter m_Filter;
        protected UserGetPropFilter Filter
        {
            get { return m_Filter; }
        }

        private UserGetPropCollection m_Collection;
        protected UserGetPropCollection Collection
        {
            get { return m_Collection; }
        }

        private int m_TotalCount;
        protected int TotalCount
        {
            get { return m_TotalCount; }
        }
    }
}