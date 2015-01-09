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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class announcements : BbsPageBase
    {
        protected AnnouncementCollection AnnouncementList;


        protected override string PageTitle
        {
            get { return "公告 - " + base.PageTitle; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("公告");

            AnnouncementList = AnnouncementBO.CurrentAnnouncements;

            WaitForFillSimpleUsers<Announcement>(AnnouncementList);
		}

        protected override string NavigationKey
        {
            get { return "announcements"; }
        }

        protected override bool IncludeBase64Js
        {
            get
            {
                return false;
            }
        }

        protected DateTime DateNow
        {
            get { return DateTimeUtil.Now; }
        }

        protected DateTime DateTimeMaxValue
        {
            get { return DateTime.MaxValue; }
        }

    }
}