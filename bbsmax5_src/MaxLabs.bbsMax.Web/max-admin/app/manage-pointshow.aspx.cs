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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_admin.app
{
    public partial class manage_pointshow : AdminPageBase
    {

        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_PointShow;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (_Request.IsClick("delete"))
            {
                DeletePointShowUsers();
            }
            
            WaitForFillSimpleUsers<PointShowBase>(this.PointShowList);
        }

        protected void DeletePointShowUsers()
        {
            int[] userids = _Request.GetList<int>("userids", new int[0]);

            PointShowBO.Instance.RemoveFomList(My, userids);
        }

        private UserPoint m_pointType;
        protected UserPoint PointType
        {
            get
            {
                if (m_pointType == null)
                {
                    m_pointType =PointSettings.GetUserPoint(PointShowSettings.UsePointType);
                }

                return m_pointType;
            }
        }

        private PointShowCollection m_pointshowList;
        protected PointShowCollection PointShowList
        {
            get
            {
                if (m_pointshowList == null)
                {
                    m_pointshowList = PointShowBO.Instance.GetPointShowList(20, _Request.Get<int>("page", Method.Get, 1));
                }
                return m_pointshowList;
            }
        }
    }
}