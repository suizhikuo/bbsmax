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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.Pages_Prop
{
    public class log_aspx: CenterPropPageBase
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
                return "proplog";
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PropBO.Instance.ReplenishProps();

            int pageNumber = _Request.Get<int>("page", 1);

            PropLogType type = _Request.Get<PropLogType>("type", PropLogType.All);

            m_LogList = PropBO.Instance.GetPropLogs(MyUserID, type, pageNumber, LogListPageSize);

            m_TotalLogCount = m_LogList.TotalRecords;

            m_Status = PropBO.Instance.GetUserPropStatus(My);

            SetPager("pager1", null, pageNumber, LogListPageSize, m_TotalLogCount);


            AddNavigationItem("道具", BbsRouter.GetUrl("prop/index"));
            AddNavigationItem("道具记录");
        }

        protected override string PageTitle
        {
            get
            {
                return string.Concat("道具记录", " - ", base.PageTitle);
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


        public int LogListPageSize
        {
            get { return 10; }
        }

        private PropLogCollection m_LogList;

        public PropLogCollection LogList
        {
            get { return m_LogList; }
        }

        private int m_TotalLogCount;

        public int TotalLogCount
        {
            get { return m_TotalLogCount; }
        }

        private PropLogType? m_SelectedType;

        public string IsSelected(PropLogType type, string output)
        {
            if(m_SelectedType == null)
            {
                m_SelectedType = _Request.Get<PropLogType>("type", MaxLabs.WebEngine.Method.Get);

                if(m_SelectedType == null)
                    m_SelectedType = PropLogType.All;
            }

            return m_SelectedType.Value == type ? output : string.Empty;
        }
    }
}