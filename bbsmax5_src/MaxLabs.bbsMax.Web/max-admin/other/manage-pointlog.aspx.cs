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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class manage_pointlog : AdminPageBase
    {

        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_PointLog; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("search"))
            {
                m_Filter = PointLogFilter.GetFromForm();

                m_Filter.Apply("filter", "page");
            }
            m_Filter = PointLogFilter.GetFromFilter("filter");
            FillSimpleUsers<PointLog>(this.PointLogList);
        }

        protected string FilterString
        {
            get
            {
                return Request.QueryString["filter"];
            }
        }

        private PointLogCollection m_PointLogList;
        protected PointLogCollection PointLogList
        {
            get
            {
                if (m_PointLogList == null)
                {
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);
                    m_PointLogList = PointLogBO.Instance.GetPointLogs(My, Filter, pageNumber);
                }

                return m_PointLogList;
            }
        }

        protected UserPointCollection PointList
        {
            get
            {
                return AllSettings.Current.PointSettings.EnabledUserPoints;
            }
        }

        private PointLogFilter m_Filter;
        protected PointLogFilter Filter
        {
            get { return m_Filter; }
        }


        private PointLogTypeCollection m_PointTypeList;
        protected PointLogTypeCollection PointTypeList
        {
            get
            {
                if (m_PointTypeList==null)
                {
                    m_PointTypeList = PointLogBO.Instance.GetPointLogTypes();
                }
                return m_PointTypeList;
            }
        }

        protected string GetOperateName(PointLog log)
        {
            PointLogType lt = this.PointTypeList.GetValue(log.OperateID);
            if (lt != null)
                return lt.OperateName;
            return string.Empty;
        }
    }
}