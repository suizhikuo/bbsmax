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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.Pages_Prop
{
    public class index_aspx : CenterPropPageBase
    {
        protected override string PageName
        {
            get
            {
                return "prop";
            }
        }
        protected override string NavigationKey
        {
            get
            {
                return "prop";
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            string pageSizeMode = _Request.Get("pagesize");

            switch (pageSizeMode)
            {
                case "a":
                    m_PropListPageSize = 10;
                    break;

                case "b":
                    m_PropListPageSize = 20;
                    break;

                case "c":
                    m_PropListPageSize = 40;
                    break;

                default:
                    m_PropListPageSize = 10;
                    break;
            }

            PropBO.Instance.ReplenishProps();

            int pageNumber = _Request.Get<int>("page", 1);

            m_PropList = PropBO.Instance.GetProps(pageNumber, PropListPageSize);

            m_TotalPropCount = m_PropList.TotalRecords;

            m_Status = PropBO.Instance.GetUserPropStatus(My);

            SetPager("pager1", null, pageNumber, m_PropListPageSize, m_TotalPropCount);
            AddNavigationItem("道具");
        }

        protected override string PageTitle
        {
            get
            {
                return string.Concat("道具中心", " - ", base.PageTitle);
            }
        }

        public int MaxPackageSize
        {
            get { return AllSettings.Current.PropSettings.MaxPackageSize.GetValue(My); }
        }

        public int GetUserPoint(UserPointType type)
        {
            return My.ExtendedPoints[(int)type];
        }

        private UserPropStatus m_Status;

        public UserPropStatus Status
        {
            get { return m_Status; }
        }

        private int m_PropListPageSize;

        public int PropListPageSize
        {
            get { return m_PropListPageSize; }
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

        public string GetPriceName(Prop prop)
        {
            return AllSettings.Current.PointSettings.UserPoints[prop.PriceType].Name;
        }

        public string GetPriceUnit(Prop prop)
        {
            return AllSettings.Current.PointSettings.UserPoints[prop.PriceType].UnitName;
        }

        protected override bool EnableFunction
        {
            get { return AllSettings.Current.PropSettings.EnablePropFunction; }
        }
    }
}