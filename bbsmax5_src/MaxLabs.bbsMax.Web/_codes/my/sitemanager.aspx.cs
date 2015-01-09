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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web
{
    public class sitemanager:CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "我的网站 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "sites"; }
        }

        protected int PageSize
        {
            get { return 20; }
           
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("我的网站");
        }

        private UserWebsiteCollection m_mywebsiteList;
        protected UserWebsiteCollection MyWebsiteList
        {
            get
            {
                if (m_mywebsiteList ==null)
                {
                    m_mywebsiteList = WebsiteBO.Instance.GetUserWebsites(MyUserID, PageSize, _Request.Get<int>("page",1));
                }
                
                return m_mywebsiteList;
            }
        }
    }
}