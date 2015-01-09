//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class mission : CenterMissionPageBase
    {

        protected override string PageTitle
        {
            get { return "任务 - " + base.PageTitle; }
        }

        protected override string NavigationKey
        {
            get { return "mission"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (_Request.IsClick("apply"))
            {
                int? missionID = _Request.Get<int>("missionid");

                if (missionID != null)
                {
                    bool success;
                    using (ErrorScope es = new ErrorScope())
                    {
                        string errorMessage = null;
                        try
                        {
                            success = MissionBO.Instance.ApplyMission(My, missionID.Value);
                            if (success == false)
                            {
                                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                {
                                    errorMessage = error.Message;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.LogException(ex);
                            success = false;
                            errorMessage = ex.Message;
                        }

                        if (errorMessage != null)
                            ShowError(errorMessage);
                    }

                    if (success)
                        BbsRouter.JumpTo("mission/detail", "mid=" + missionID.Value + "&isnowaction=true");
                }
            }

            int pageNumber = _Request.Get<int>("page", 1);
            int pageSize = 5;
            int? categoryID = _Request.Get<int>("cid");

            m_MissionList = MissionBO.Instance.GetNewMissions(MyUserID, categoryID, pageNumber, pageSize, out m_MissionCount);

            m_CategoryList = MissionBO.Instance.GetMissionCategories();

            SetPager("pager1", null, pageNumber, pageSize, m_MissionCount);
            AddNavigationItem("任务");
        }

        private int m_MissionCount;
        public int MissionCount
        {
            get
            {
                return m_MissionCount;
            }
        }

        private MissionCollection m_MissionList;
        public MissionCollection MissionList
        {
            get
            {
                return m_MissionList;
            }
        }

        private MissionCategoryCollection m_CategoryList;

        public MissionCategoryCollection CategoryList
        {
            get
            {
                return m_CategoryList;
            }
        }
    }
}