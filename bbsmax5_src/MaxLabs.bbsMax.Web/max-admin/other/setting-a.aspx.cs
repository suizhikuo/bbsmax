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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_a : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_A; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<AdvertSettings>("savesetting");
        }

        private AdvertCollection m_AllAdverts;
        private AdvertCollection AllAdverts
        {
            get
            {
                if (m_AllAdverts == null)
                {
                    m_AllAdverts = AdvertBO.Instance.GetAdverts();
                }

                return m_AllAdverts;
            }
        }

        protected int GetCount( int categoryID,string position )
        {
            int c = 0;
            ADPosition pos = StringUtil.TryParse<ADPosition>(position, ADPosition.None);
            foreach (Advert ad in AllAdverts)
            {
                if (ad.CategoryID == categoryID)
                {
                    if (ad.Position == pos)
                        c++;
                }
            }
            
            return c;
        }
    }
}