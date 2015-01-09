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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class mission_detail : CenterMissionPageBase
    {
        protected override string PageTitle
        {
            get { return string.Concat(Mission.Name, " - 任务 - ", base.PageTitle); }
        }

        MissionAction? action;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            int? missionID = _Request.Get<int>("mid");

            if (missionID == null)
            {
                ShowError("缺少必要的页面参数");
            }

            action = _Request.Get<MissionAction>("missionaction", Method.Get);

            if (_Request.IsClick("applymission") || _Request.IsClick("reapplymission")
                || _Request.IsClick("applymissionagain") || _Request.IsClick("getmissionprize")
                || _Request.IsClick("abandonmission") || action != null)
            {
                Process();
            }
            else
            {
                IsNowAction = _Request.Get<bool>("isnowaction", Method.Get, false);
            }

            m_Mission = MissionBO.Instance.GetMission(missionID.Value);

            if (m_Mission == null)
            {
                ShowError("任务不存在");
            }

            m_UserMission = MissionBO.Instance.GetUserMission(MyUserID, missionID.Value);

            m_StepUserMission = m_UserMission;

            if (m_Mission.ChildMissions != null && m_Mission.ChildMissions.Count > 0)
            {
                int[] missionIDs = new int[m_Mission.ChildMissions.Count];

                for (int i = 0; i < missionIDs.Length; i++)
                {
                    missionIDs[i] = m_Mission.ChildMissions[i].ID;
                }

                m_StepUserMission = MissionBO.Instance.GetStepUserMission(MyUserID, missionIDs);
            }

            int pageNumber = _Request.Get<int>("page", 1);
            int pageSize = 20;

            m_UserList = MissionBO.Instance.GetMissionUsers(missionID.Value, pageNumber, pageSize, out m_UserCount);


            WaitForFillSimpleUsers<UserMission>(m_UserList);

            SetPager("pager1", null, pageNumber, pageSize, m_UserCount);

            if (m_UserMission != null)
            {
                AddNavigationItem("我的任务", BbsRouter.GetUrl("mission/current"));
            }
            else
                AddNavigationItem("任务", BbsRouter.GetUrl("mission/index"));

            AddNavigationItem(m_Mission.Name);
        }

        private Mission m_Mission;

        public Mission Mission
        {
            get
            {
                return m_Mission;
            }
        }

        private UserMission m_UserMission;

        public UserMission UserMission
        {
            get
            {
                return m_UserMission;
            }
        }

        private UserMission m_StepUserMission;

        public UserMission StepUserMission
        {
            get
            {
                return m_StepUserMission;
            }
        }

        public Mission StepMission
        {
            get
            {
                int? step = _Request.Get<int>("step");

                if (StepUserMission == null && step == null)
                    return Mission.ChildMissions[0];

                if (step != null)
                    return Mission.ChildMissions[step.Value];

                return StepUserMission.Mission;
            }
        }

        private int m_UserCount;

        public int UserCount
        {
            get { return m_UserCount; }
        }

        private UserMissionCollection m_UserList;

        public UserMissionCollection UserList
        {
            get
            {
                return m_UserList;
            }
        }

        protected bool IsGetPrize
        {
            get
            {
                return _Request.IsClick("getmissionprize");
            }
        }

        private void Process()
        {
            int missionID = _Request.Get<int>("mid", Method.Post, 0);
            if (missionID == 0)
                missionID = _Request.Get<int>("mid", Method.Get, 0);

            bool setNowAction = false;
            if (_Request.IsClick("applymission"))
            {
                setNowAction = true;
                action = MissionAction.Apply;
            }
            else if (_Request.IsClick("reapplymission"))
            {
                setNowAction = true;
                action = MissionAction.Reapply;
            }
            else if (_Request.IsClick("applymissionagain"))
            {
                setNowAction = true;
                action = MissionAction.ApplyAgain;
            }
            else if (_Request.IsClick("abandonmission"))
            {
                action = MissionAction.Abandon;
            }
            else if (_Request.IsClick("getmissionprize"))
            {
                setNowAction = true;
                action = MissionAction.GetPrize;
            }

            MessageDisplay message = CreateMessageDisplay();
            try
            {
                using (new ErrorScope())
                {
                    bool success = false;

                    switch (action.Value)
                    {
                        case MissionAction.Apply:
                        case MissionAction.Reapply:
                        case MissionAction.ApplyAgain:
                            success = MissionBO.Instance.ApplyMission(My, missionID);
                            break;
                        case MissionAction.GetPrize:
                            success = MissionBO.Instance.GetMissionPrize(My, missionID);
                            break;
                        case MissionAction.Abandon:
                            success = MissionBO.Instance.AbandonMission(My, missionID);
                            break;
                    }


                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            message.AddError(error);
                        });

                    }
                    else
                    {
                        if (setNowAction)
                            IsNowAction = true;
                        else
                            IsNowAction = false;
                        //HttpContext.Current.Response.Redirect("/max-templates/default/mission-detail.aspx?missionID=" + missionID);
                    }
                }
            }
            catch (Exception ex)
            {
                message.AddException(ex);
            }
        }

        protected bool IsNowAction { get; set; }

        protected bool HasGetPrize
        {
            get
            {
                if (UserMission != null && UserMission.IsPrized)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        protected bool IsFinished
        {
            get
            {
                if (UserMission != null && UserMission.FinishPercent == 1)
                    return true;
                else
                    return false;
            }
        }

        protected bool IsNow(DateTime date)
        {
            return date.AddSeconds(5) > DateTimeUtil.Now;
        }

        private enum MissionAction
        {
            Apply,
            Reapply,
            ApplyAgain,
            Abandon,
            GetPrize
        }


        public bool IsShowApplyMissionButton
        {
            get
            {
                if (m_UserMission != null) //已经申请过了
                    return false;
                return MissionBO.Instance.CanApplyMission(My, m_Mission);
            }
        }

        public bool IsShowApplyMissionAgainButton
        {
            get
            {
                return MissionBO.Instance.CanApplyMissionAgain(My, m_Mission, m_UserMission);
            }
        }

        public bool IsShowGetMissionPrizeButton
        {
            get
            {
                if (m_UserMission == null)
                    return false;

                if (m_UserMission.FinishPercent == 1 && m_UserMission.IsPrized == false && m_UserMission.Mission.Prize.PrizeTypes.Count > 0)
                    return true;

                return false;
            }
        }

        public bool IsShowAbandonMissionButton
        {
            get
            {
                if (m_UserMission == null)
                    return false;

                if (m_UserMission.Status == MissionStatus.OverTime || (m_UserMission.Status == MissionStatus.Underway && m_UserMission.FinishPercent != 1))
                    return true;

                return false;
            }
        }

        public bool IsShowReApplyMissionButton
        {
            get
            {
                if (m_UserMission == null)
                    return false;

                if (m_UserMission.Status == MissionStatus.Abandon )
                    //|| (m_UserMission.Mission.CycleTime>0 && m_UserMission.Status== MissionStatus.OverTime))
                    return true;

                return false;
            }
        }

        public bool IsShowGetStepMissionPrizeButton
        {
            get
            {
                if (m_StepUserMission == null || m_StepUserMission == m_UserMission)
                    return false;

                if (m_StepUserMission.Status == MissionStatus.Underway && m_StepUserMission.FinishPercent == 1)
                    return true;

                return false;
            }
        }
    }
}