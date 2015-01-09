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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class mission_users : CenterMissionPageBase
    {

        protected override string PageTitle
        {
            get { return "我的任务 - " + base.PageTitle; }
        }

        protected override string NavigationKey
        {
            get { return "mymission"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (_Request.IsClick("getprize"))
            {
                int? missionID = _Request.Get<int>("missionid");

                if (missionID != null)
                {
                    MissionBO.Instance.GetMissionPrize(My, missionID.Value);

                    BbsRouter.JumpToCurrentUrl();
                }
            }
            else if (_Request.IsClick("abandon"))
            {
                int? missionID = _Request.Get<int>("missionid");

                if (missionID != null)
                {
                    MissionBO.Instance.AbandonMission(My, missionID.Value);

                    BbsRouter.JumpToCurrentUrl();
                }
            }

            int pageNumber = _Request.Get<int>("page", 1);
            int pageSize = 5;

            m_MissionList = MissionBO.Instance.GetUserMissions(MyUserID, pageNumber, pageSize, out m_MissionCount);

            SetPager("pager1", null, pageNumber, pageSize, m_MissionCount);

            AddNavigationItem("任务", BbsRouter.GetUrl("mission/index"));
            AddNavigationItem("我的任务");
        }

        private int m_MissionCount;

        public int MissionCount
        {
            get
            {
                return m_MissionCount;
            }
        }

        private UserMissionCollection m_MissionList;

        public UserMissionCollection MissionList
        {
            get
            {
                return m_MissionList;
            }
        }

        protected bool IsFriend(int userID)
        {
            return FriendBO.Instance.IsFriend(MyUserID, userID);
        }
    }
}