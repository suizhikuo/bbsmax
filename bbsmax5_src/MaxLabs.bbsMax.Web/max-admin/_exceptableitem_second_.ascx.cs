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
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class _exceptableitem_second_ : ExceptableSettigItemPageBase
    {
        protected long GetTimeVale(long seconds)
        {
            TimeUnit timeUnit;
            return DateTimeUtil.FormatSecond(seconds, out timeUnit);
        }
        protected TimeUnit GetTimeUnit(long seconds)
        {
            TimeUnit timeUnit;
            DateTimeUtil.FormatSecond(seconds, out timeUnit);

            return timeUnit;
        }

        private ExceptionItem<long> m_Item;
        protected ExceptionItem<long> Item
        {
            get
            {
                if (m_Item == null)
                {
                    try
                    {
                        m_Item = (ExceptionItem<long>)Parameters["Item"];
                    }
                    catch
                    {
                        m_Item = new ExceptionItem<long>(Guid.Empty,0,0);
                        ExceptionItem<int> temp = (ExceptionItem<int>)Parameters["Item"];

                        m_Item.LevelStatus = temp.LevelStatus;
                        m_Item.RoleID = temp.RoleID;
                        m_Item.SortOrder = temp.SortOrder;
                        m_Item.Value = (long)temp.Value;
                    }
                }
                return m_Item;
            }
        }
    }
}