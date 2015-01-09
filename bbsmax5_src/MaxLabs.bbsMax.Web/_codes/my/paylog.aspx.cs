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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web
{
    public class paylog : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "充值记录 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "paylog"; }
        }

        protected override string NavigationKey
        {
            get { return "paylog"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("充值记录");

            if (AllSettings.Current.PaySettings.EnablePointRecharge == false)
                ShowError("系统未开启充值功能");

            if (_Request.IsClick("search"))
            {
                PaylogFilter filter = PaylogFilter.GetFromForm();
                filter.Apply("filter", "page");
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
                    m_Filter.State = Convert.ToByte(State);
                }
                return m_Filter;
            }
        }

        protected int PageSize
        {
            get { return 20; }

        }

        private UserPayCollection m_UserPayList;
        protected UserPayCollection UserPayList
        {
            get
            {
                if (m_UserPayList == null)
                {
                    m_UserPayList = PayBO.Instance.GetUserPays(MyUserID, Filter, PageSize, _Request.Get<int>("page", 1));
                }

                return m_UserPayList;
            }
        }

        private string m_State;
        protected string State
        {
            get
            {
                if (m_State == null)
                {
                    m_State = _Request.Get("state", Method.Get, "1");
                }
                return m_State;
            }
        }
    }
}