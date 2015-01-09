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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.Web.max_admin.other
{
    public partial class manage_prop : AdminPageBase
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
            int pageNumber = _Request.Get<int>("page", 0);

            if(_Request.IsClick("disable"))
            {
                int[] ids = StringUtil.Split<int>(_Request.Get("PropIDs", MaxLabs.WebEngine.Method.Post, string.Empty));

                if (ids != null && ids.Length > 0)
                {
                    PropBO.Instance.DisableProps(My, ids);
                }

                JumpTo("interactive/manage-prop.aspx");
            }
            else if(_Request.IsClick("enable"))
            {
                int[] ids = StringUtil.Split<int>(_Request.Get("PropIDs", MaxLabs.WebEngine.Method.Post, string.Empty));

                if (ids != null && ids.Length > 0)
                {
                    PropBO.Instance.EnableProps(My, ids);
                }

                JumpTo("interactive/manage-prop.aspx");
            }
            else if(_Request.IsClick("delete"))
            {
                int[] ids = StringUtil.Split<int>(_Request.Get("PropIDs", MaxLabs.WebEngine.Method.Post, string.Empty));

                if (ids != null && ids.Length > 0)
                {
                    PropBO.Instance.DeleteProps(My, ids);
                }

                JumpTo("interactive/manage-prop.aspx");
            }

            m_PropList = PropBO.Instance.GetPropsForAdmin(My, pageNumber, 10);

            m_TotalPropCount = m_PropList.TotalRecords;
        }

        private PropCollection m_PropList;

        public PropCollection PropList
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