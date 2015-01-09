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
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class _exceptableitem_bool_ : ExceptableSettigItemPageBase
    {
        private ExceptionItem<bool> m_Item;
        protected ExceptionItem<bool> Item
        {
            get
            {
                if (m_Item == null)
                {
                    m_Item = (ExceptionItem<bool>)Parameters["Item"];
                }
                return m_Item;
            }
        }
    }
}