//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class MissionTemplateMembers
    {
        #region newmissionList

        public delegate void NewMissionListHeadFootTemplate(bool hasItems, int totalMissions);
        public delegate void NewMissionListItemTemplate(NewMissionListItemParams _this, int i);

        public class NewMissionListItemParams
        {

            public NewMissionListItemParams(Mission mission)
            {
                m_Mission = mission;
            }

            private Mission m_Mission;
            public Mission Mission
            {
                get
                {
                    return m_Mission;
                }
            }

            private string prizeDescriptionKey = "prizeDescriptionKey_{0}_{1}_{2}";
            public string MissionPrizeDescription(string lineFormat, string separator)
            {
                string key = string.Format(prizeDescriptionKey, lineFormat, separator, Mission.Prize.ConvertToString());
                string description;
                if (PageCacheUtil.TryGetValue<string>(key, out description) == false)
                {
                    description = MissionBO.Instance.GetMissionPrizeDescription(Mission.Prize, lineFormat, separator);
                    PageCacheUtil.Set(key, description);
                }

                return description;
            }


            public bool CanApplyMission
            {
                get
                {
                    return MissionBO.Instance.CanApplyMission(User.Current, Mission);
                }
            }
        }


        //[TemplateTag]
        //public void NewMissionList(int page, int pageSize
        //    , NewMissionListHeadFootTemplate head
        //    , NewMissionListHeadFootTemplate foot
        //    , NewMissionListItemTemplate item)
        //{

        //    if (page == 0)
        //        page = 1;

        //    int userID = UserBO.Instance.GetUserID();

        //    int totalCount;
        //    MissionCollection missions = MissionBO.Instance.GetNewMissions(userID, page, pageSize, out totalCount);

        //    bool haveItem = (missions.Count > 0);

        //    head(haveItem, totalCount);

        //    int i = 0;
        //    foreach (Mission mission in missions)
        //    {
        //        NewMissionListItemParams param = new NewMissionListItemParams(mission);
        //        item(param, i++);
        //    }

        //    foot(haveItem, totalCount);
        //}

        #endregion


        #region newmissionuserList

        public delegate void NewMissionUserListHeadFootTemplate(bool hasItems);
        public delegate void NewMissionUserListItemTemplate(int i, UserMission userMission);

        /// <summary>
        /// 最近参与任务的用户
        /// </summary>
        /// <param name="count"></param>
        /// <param name="template"></param>
        [TemplateTag]
        public void NewMissionUserList(int count
            , NewMissionUserListHeadFootTemplate head
            , NewMissionUserListHeadFootTemplate foot
            , NewMissionUserListItemTemplate item)
        {
            UserMissionCollection missionUsers = MissionBO.Instance.GetNewMissionUsers(count);

            List<int> userIDs = new List<int>();
            foreach (UserMission userMission in missionUsers)
            {
                userIDs.Add(userMission.UserID);
            }
            if(userIDs.Count>0)
                UserBO.Instance.GetUsers(userIDs, GetUserOption.WithDeletedUser);

            head(missionUsers.Count > 0);

            int i = 0;
            foreach (UserMission missionUser in missionUsers)
            {
                item(i++, missionUser); 
            }

            foot(missionUsers.Count > 0);
        }

        #endregion

        #region userMissionuserList

        public delegate void UserMissionListHeadFootTemplate(UserMissionListHeadFootParams _this, bool hasItems, int totalMissions);
        public delegate void UserMissionListItemTemplate(UserMissionListItemParams _this, int i);

        public class UserMissionListHeadFootParams
        {
            public UserMissionListHeadFootParams(MissionStatus currentStatus)
            {
                m_CurrentStatus = currentStatus;
            }
            private MissionStatus m_CurrentStatus;
            public MissionStatus CurrentStatus
            {
                get
                {
                    return m_CurrentStatus;
                }
            }

            public string IsSelected(MissionStatus status, string value)
            {
                if (status == CurrentStatus)
                    return value;
                else
                    return string.Empty;
            }
        }
        public class UserMissionListItemParams
        {

            public UserMissionListItemParams(UserMission userMission, MissionStatus currentStatus)
            {
                m_UserMission = userMission;
                m_CurrentStatus = currentStatus;
            }

            private UserMission m_UserMission;
            public UserMission UserMission
            {
                get
                {
                    return m_UserMission;
                }
            }

            private MissionStatus m_CurrentStatus;
            public MissionStatus CurrentStatus
            {
                get
                {
                    return m_CurrentStatus;
                }
            }

            public bool IsDisplayFinishPercent
            {
                get
                {
                    return CurrentStatus != MissionStatus.Finish;
                }
            }

            private int? m_FinishPercent;
            public int FinishPercent
            {
                get
                {
                    if (m_FinishPercent == null)
                    {
                        m_FinishPercent = (int)(UserMission.FinishPercent * 100);
                    }
                    return m_FinishPercent.Value;
                }
            }

            private string prizeDescriptionKey = "prizeDescriptionKey_{0}_{1}_{2}";
            public string MissionPrizeDescription(string lineFormat, string separator)
            {
                string key = string.Format(prizeDescriptionKey, lineFormat, separator, UserMission.Mission.Prize.ConvertToString());
                string description;
                if (PageCacheUtil.TryGetValue<string>(key, out description) == false)
                {
                    description = MissionBO.Instance.GetMissionPrizeDescription(UserMission.Mission.Prize, lineFormat, separator);
                    PageCacheUtil.Set(key, description);
                }

                return description;
            }

            public bool CanDisplayAbandon
            {
                get
                {
                    if (CurrentStatus == MissionStatus.Underway && UserMission.FinishPercent < 1)
                        return true;
                    else
                        return false;
                }
            }

            public bool CanDisplayGetPrize
            {
                get
                {

                    if (CurrentStatus == MissionStatus.Underway && UserMission.FinishPercent == 1 && UserMission.Mission.Prize.PrizeTypes.Count > 0)
                        return true;

                    return false;
                }
            }

        }

        ///// <summary>
        ///// 用户的任务列表
        ///// </summary>
        ///// <param name="status"></param>
        ///// <param name="page"></param>
        ///// <param name="pageSize"></param>
        ///// <param name="template"></param>
        //[TemplateTag]
        //public void UserMissionList(string status, int page, int pageSize
        //    , UserMissionListHeadFootTemplate head
        //    , UserMissionListHeadFootTemplate foot
        //    , UserMissionListItemTemplate item)
        //{
        //    MissionStatus missionStatus;
        //    try
        //    {
        //        missionStatus = (MissionStatus)Enum.Parse(typeof(MissionStatus), status, true);
        //    }
        //    catch
        //    {
        //        missionStatus = MissionStatus.Underway;
        //    }

        //    if (page == 0)
        //        page = 1;

        //    int totalCount;
        //    UserMissionCollection userMissions = MissionBO.Instance.GetUserMissions(UserBO.Instance.GetUserID(), missionStatus, page, pageSize, out totalCount);

        //    List<int> userIDs = new List<int>();
        //    foreach (UserMission userMission in userMissions)
        //    {
        //        userIDs.Add(userMission.UserID);
        //    }

        //    UserBO.Instance.GetUsers(userIDs, GetUserOption.WithDeletedUser);

        //    bool haveItem = (userMissions.Count > 0);

        //    UserMissionListHeadFootParams headFoot = new UserMissionListHeadFootParams(missionStatus);
        //    head(headFoot, haveItem, totalCount);
        //    int i = 0;
        //    foreach (UserMission userMission in userMissions)
        //    {
        //        UserMissionListItemParams param = new UserMissionListItemParams(userMission, missionStatus);
        //        item(param, i++);
        //    }
        //    foot(headFoot, haveItem, totalCount);
        //}

        #endregion

        #region MissionDetail
        public delegate void MissionDetailTemplate(MissionDetailParams _this);

        public class MissionDetailParams
        {
            public MissionDetailParams(Mission mission)
            {
                m_Mission = mission;
            }

            private Mission m_Mission;
            public Mission Mission
            {
                get
                {
                    return m_Mission;
                }
            }

            private string prizeDescriptionKey = "prizeDescriptionKey_{0}_{1}_{2}";
            public string MissionPrizeDescription(string lineFormat, string separator)
            {
                string key = string.Format(prizeDescriptionKey, lineFormat, separator, Mission.Prize.ConvertToString());
                string description;
                if (PageCacheUtil.TryGetValue<string>(key, out description) == false)
                {
                    description = MissionBO.Instance.GetMissionPrizeDescription(Mission.Prize, lineFormat, separator);
                    PageCacheUtil.Set(key, description);
                }

                return description;
            }


            private string m_FinishConditionDescription;
            public string FinishConditionDescription
            {
                get
                {
                    if (m_FinishConditionDescription == null)
                    {
                        m_FinishConditionDescription = Mission.MissionBase.GetFinishConditionDescription(Mission.FinishCondition);
                        if (m_FinishConditionDescription == null)
                            m_FinishConditionDescription = string.Empty;
                    }
                    return m_FinishConditionDescription;
                }
            }
            private string m_ApplyConditionDescription;
            public string ApplyConditionDescription
            {
                get
                {
                    if (m_ApplyConditionDescription == null)
                    {
                        m_ApplyConditionDescription = MissionBO.Instance.GetApplyMissionConditionDescription(Mission.ApplyCondition);
                        if (m_ApplyConditionDescription == null)
                            m_ApplyConditionDescription = string.Empty;
                    }
                    return m_ApplyConditionDescription;
                }
            }

            private string m_DeductPointDescription;
            public string DeductPointDescription
            {
                get
                {
                    if (m_DeductPointDescription == null)
                    {
                        string lineFormat = "{0}{1}{2}";
                        m_DeductPointDescription = MissionBO.Instance.GetPointDescription(Mission.DeductPoint, lineFormat, "<br />", "扣除");
                    }
                    return m_DeductPointDescription;
                }
            }

            public bool IsDisplayDeductPointDescription
            {
                get
                {
                    return DeductPointDescription != string.Empty;
                }
            }

            public bool IsDisplayFinishCondition
            {
                get
                {
                    return FinishConditionDescription != string.Empty;
                }
            }
            public bool IsDisplayApplyCondition
            {
                get
                {
                    return ApplyConditionDescription != string.Empty;
                }
            }
        }

        [TemplateTag]
        public void MissionDetail(int missionID
            , MissionDetailTemplate template)
        {
            Mission mission = MissionBO.Instance.GetMission(missionID);

            if (mission != null)
            {
                MissionDetailParams detail = new MissionDetailParams(mission);
                template(detail);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="isApplied">是否申请过了</param>
        /// <param name="isFinished">是否完成了</param>
        /// <param name="userMission"></param>
        public delegate void UserMissionTemplate2(UserMissionParams _this, bool isApplied, bool isFinished, bool havePrize);

        public class UserMissionParams
        {
            public UserMissionParams(UserMission userMission)
            {
                m_UserMission = userMission;
            }

            private UserMission m_UserMission;
            public UserMission UserMission
            {
                get
                {
                    return m_UserMission;
                }
            }

            private int? m_FinishPercent;
            public int FinishPercent
            {
                get
                {
                    if (m_FinishPercent == null)
                    {
                        if (UserMission == null)
                            m_FinishPercent = 0;
                        else
                            m_FinishPercent = (int)(UserMission.FinishPercent * 100);
                    }
                    return m_FinishPercent.Value;
                }
            }

            public bool IsDisplayStepDescriptions
            {
                get
                {
                    if (UserMission == null)
                        return true;
                    if (!UserMission.Mission.MissionBase.HaveStepDescriptions)
                        return false;
                    if (UserMission.Status == MissionStatus.Underway)
                        return true;
                    return false;
                }
            }
        }

        [TemplateTag]
        public void UserMission(int missionID, UserMissionTemplate2 template)
        {
            UserMission userMission = MissionBO.Instance.GetUserMission(UserBO.Instance.GetCurrentUserID(), missionID);

            bool isApplied, isFinished, havePrize;
            if (userMission == null)
            {
                isApplied = false;
                isFinished = false;
            }
            else
            {
                isApplied = true;
                if (userMission.FinishPercent == 1)
                    isFinished = true;
                else
                    isFinished = false;

            }

            if (MissionBO.Instance.GetMission(missionID).Prize.PrizeTypes.Count > 0)
                havePrize = true;
            else
                havePrize = false;

            UserMissionParams userMissionParams = new UserMissionParams(userMission);

            template(userMissionParams, isApplied, isFinished, havePrize);
        }


        public delegate void MissionStepDescriptionListHeadFootTemplate();
        public delegate void MissionStepDescriptionListItemTemplate(int i, string stepDescription);
        /// <summary>
        /// 执行任务步骤
        /// </summary>
        /// <param name="missionID"></param>
        /// <param name="template"></param>
        [TemplateTag]
        public void MissionStepDescriptionList(string type
            , MissionStepDescriptionListHeadFootTemplate head
            , MissionStepDescriptionListHeadFootTemplate foot
            , MissionStepDescriptionListItemTemplate item)
        {
            MissionBase missionBase = MissionBO.Instance.GetMissionBase(type);
            if (missionBase != null)
            {
                if (missionBase.StepDescriptions != null)
                {
                    int i = 0;
                    head();
                    foreach (string stepDescription in missionBase.StepDescriptions)
                    {
                        item(i++, stepDescription);
                    }
                    foot();
                }
            }
            else
            {
                //TODO:
            }
        }

        #endregion

        #region MissionUserList

        public delegate void MissionUserListItemHeadFootTemplate(bool hasItems, int totalUsers);
        public delegate void MissionUserListItemTemplate(int i, int index, User user, bool isOddLine, bool isSelf);


        [TemplateTag]
        public void MissionUserList(int missionID, int page, int pageSize
            , MissionUserListItemHeadFootTemplate head
            , MissionUserListItemHeadFootTemplate foot
            , MissionUserListItemTemplate item)
        {
            MissionUserList(missionID, page, pageSize, head, foot, null, item);
        }

        public delegate void UserMissionTemplate(int i, UserMission userMission);
        /// <summary>
        /// 任务的参与用户
        /// </summary>
        /// <param name="missionID"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="template"></param>
        private void MissionUserList(int missionID, int page, int pageSize
            , MissionUserListItemHeadFootTemplate head
            , MissionUserListItemHeadFootTemplate foot
            , UserMissionTemplate template
            , MissionUserListItemTemplate userTemplate)
        {

            if (page == 0)
                page = 1;

            int totalCount;
            UserMissionCollection userMissions = MissionBO.Instance.GetMissionUsers(missionID, page, pageSize, out totalCount);

            List<int> userIDs = new List<int>();
            foreach (UserMission userMission in userMissions)
            {
                userIDs.Add(userMission.UserID);
            }
            UserCollection users = UserBO.Instance.GetUsers(userIDs);

            head(totalCount > 0, totalCount);
            int i = 0;
            if (template != null)
            {
                foreach (UserMission userMission in userMissions)
                {
                    template(i++, userMission);
                }
            }
            if (userTemplate == null)
            {
                foot(totalCount > 0, totalCount);
                return;
            }


            if (userIDs.Count == 0)
            {
                foot(totalCount > 0, totalCount);
                return;
            }

            i = 0;
            int index = (page - 1) * pageSize;
            int userID = UserBO.Instance.GetCurrentUserID();
            foreach (User user in users)
            {
                index++;
                userTemplate(i, index, user, i % 2 == 0, user.UserID == userID);
                i++;
            }
            foot(totalCount > 0, totalCount);

        }

        /// <summary>
        /// 刚刚参与任务的用户
        /// </summary>
        /// <param name="missionID"></param>
        /// <param name="template"></param>
        [TemplateTag]
        public void NewMissionUserList(int missionID, int count
            , MissionUserListItemHeadFootTemplate head
            , MissionUserListItemHeadFootTemplate foot
            , UserMissionTemplate item)
        {
            MissionUserList(missionID, 1, count, head, foot, item, null);
        }

        #endregion


        #region MissionBaseList

        public delegate void MissionBasesListHeadFootTemplate(bool hasItems, int totalMissionBases);
        public delegate void MissionBasesListItemTemplate(MissionBasesListItemParams _this, int i);
        public class MissionBasesListItemParams
        {
            public MissionBasesListItemParams(MissionBase missionBase)
            {
                m_MissionBase = missionBase;
            }

            private MissionBase m_MissionBase;
            public MissionBase MissionBase
            {
                get
                {
                    return m_MissionBase;
                }
            }

            public string IsSelected(string type, string value, int index)
            {
                if (string.IsNullOrEmpty(type) && index == 0)
                    return value;
                if (string.Compare(type, MissionBase.Type, true) == 0)
                {
                    return value;
                }
                else
                    return string.Empty;
            }
        }


        [TemplateTag]
        public void MissionBaseList(
              MissionBasesListHeadFootTemplate head
            , MissionBasesListHeadFootTemplate foot
            , MissionBasesListItemTemplate item)
        {
            List<MissionBase> missions = MissionBO.Instance.GetAllMissionBases();

            int i = 0;

            int total = missions.Count;

            head(total > 0, total);
            foreach (MissionBase missionBase in missions)
            {
                MissionBasesListItemParams param = new MissionBasesListItemParams(missionBase);
                item(param, i++);
            }
            foot(total > 0, total);
        }
        #endregion

        #region CreateORUpdateMission

        public delegate void MissionTemplate(bool isEdit, string prizeTypes, string prizeUserGroups, string prizeMedals, string prizeMedalIDs, string applyConditionUserGroups, Mission mission);
        [TemplateTag]
        public void Mission(int missionID, MissionTemplate template)
        {
            bool isEdit = false;
            Mission mission = null;
            StringBuilder prizeTypes = new StringBuilder();
            string prizeUserGroups = null;
            string prizeMedals = null;
            string prizeMedalIDs = null;
            string applyConditionUserGroups = null;
            if (missionID != 0)
            {
                isEdit = true;
                mission = MissionBO.Instance.GetMission(missionID, true);
                foreach (PrivacyType type in mission.Prize.PrizeTypes)
                {
                    prizeTypes.Append((int)type + ",");
                }
                if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.UserGroup))
                {
                    prizeUserGroups = StringUtil.Join(mission.Prize.UserGroups.Keys);
                }
                applyConditionUserGroups = StringUtil.Join(mission.ApplyCondition.UserGroupIDs);
                if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Medal))
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();
                    foreach (PrizeMedal medal in mission.Prize.Medals)
                    {
                        sb.Append(medal.MedalID + "_" + medal.MedalLevelID).Append(",");
                        sb2.Append(medal.MedalID).Append(",");
                    }

                    if (sb.Length > 0)
                    {
                        prizeMedals = sb.ToString(0, sb.Length - 1);
                        prizeMedalIDs = sb2.ToString(0, sb2.Length - 1);
                    }
                    else
                    {
                        prizeMedals = string.Empty;
                        prizeMedalIDs = string.Empty;
                    }
                }
            }
            template(isEdit, prizeTypes.ToString(), prizeUserGroups, prizeMedals, prizeMedalIDs, applyConditionUserGroups, mission);
        }



        [TemplateTag]
        public void UserGroupValidityTime(Mission mission, Guid groupID, bool isEdit, GlobalTemplateMembers.ValidityTimeTemplate template)
        {
            long seconds;
            if (isEdit)
            {
                if (mission.Prize.UserGroups.ContainsKey(groupID))
                    seconds = mission.Prize.UserGroups[groupID];
                else
                    seconds = 0;
            }
            else
                seconds = 0;
            new GlobalTemplateMembers().TimeFormater(seconds, template);
        }

        [TemplateTag]
        public void MedalValidityTime(Mission mission, int medalID, bool isEdit, GlobalTemplateMembers.ValidityTimeTemplate template)
        {
            long seconds = 0;
            if (isEdit)
            {
                foreach (PrizeMedal medal in mission.Prize.Medals)
                {
                    if (medal.MedalID == medalID)
                    {
                        seconds = medal.Seconds;
                        break;
                    }
                }

            }
            else
                seconds = 0;
            new GlobalTemplateMembers().TimeFormater(seconds, template);
        }


        public delegate void MissionBaseTemplate(MissionBase currentMissionBase);

        [TemplateTag]
        public void MissionBase(string type, MissionBaseTemplate template)
        {
            MissionBase missionBase = MissionBO.Instance.GetMissionBase(type);
            if (missionBase != null)
                template(missionBase);
            else if (MissionBO.Instance.GetAllMissionBases().Count > 0)
                template(MissionBO.Instance.GetAllMissionBases()[0]);
        }

        #endregion

        #region missionList

        public delegate void MissionListHeadFootTemplate(bool hasItems, int totalMissions);
        public delegate void MissionListItemTemplate(int i, Mission mission);

        [TemplateTag]
        public void MissionList(int page, int pageSize
            , MissionListHeadFootTemplate head
            , MissionListHeadFootTemplate foot
            , MissionListItemTemplate item)
        {
            if (page == 0)
                page = 1;

            int total;
            MissionCollection missions = MissionBO.Instance.GetMissions(null, page, pageSize, true, out total);

            head(total > 0, total);

            int i = 0;
            foreach (Mission mission in missions)
            {
                item(i++, mission);
            }
            foot(total > 0, total);
        }

        #endregion




        public delegate void NewMissionTemplate(bool haveNewMission, Mission mission);
        /// <summary>
        /// 获取用户待参与的最优先的一个任务
        /// </summary>
        /// <param name="template"></param>
        [TemplateTag]
        public void NewMission(NewMissionTemplate template)
        {
            int userID = UserBO.Instance.GetCurrentUserID();
            Mission mission = MissionBO.Instance.GetNewUserMission(userID);
            template(mission != null, mission);
        }






        public delegate void Mission2Template(int i, Mission mission);
        [TemplateTag]
        public void MissionList(string page, int pageSize, Mission2Template template)
        {
            int pageNumber;
            if (page == null)
                pageNumber = 1;
            else if (!int.TryParse(page, out pageNumber))
                pageNumber = 1;

            int total;
            MissionCollection missions = MissionBO.Instance.GetMissions(null, pageNumber, pageSize, false, out total);

            int i = 0;
            foreach (Mission mission in missions)
            {
                template(i++, mission);
            }
        }




        [TemplateVariable]
        public int AllMissionsCount
        {
            get
            {
                return MissionBO.Instance.AllMissions().Count;
            }
        }



        private string prizeDescriptionKey = "prizeDescriptionKey_{0}_{1}_{2}";
        /// <summary>
        /// 任务奖励
        /// </summary>
        /// <param name="prize"></param>
        /// <returns></returns>
        [TemplateFunction]
        public string GetMissionPrizeDescription(MissionPrize prize, string lineFormat, string separator)
        {
            string key = string.Format(prizeDescriptionKey, lineFormat, separator, prize.ConvertToString());
            string description;
            if (PageCacheUtil.TryGetValue<string>(key, out description) == false)
            {
                description = MissionBO.Instance.GetMissionPrizeDescription(prize, lineFormat, separator);
                PageCacheUtil.Set(key, description);
            }

            return description;
            //return MissionBO.Instance.GetMissionPrizeDescription(prize);
        }

        /// <summary>
        /// 是否可以申请
        /// </summary>
        /// <param name="mission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool CanApplyMission(Mission mission)
        {
            return MissionBO.Instance.CanApplyMission(User.Current, mission);
        }

        ///// <summary>
        ///// 任务参与度
        ///// </summary>
        //[TemplateVariable]
        //public int MissionParticipationPercent
        //{
        //    get
        //    {
        //        return (int)(((float)(AllMissionsCount - NewMissionCount)) / AllMissionsCount * 100);
        //    }
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="isApplied">是否申请过了</param>
        /// <param name="isFinished">是否完成了</param>
        /// <param name="userMission"></param>
        public delegate void UserMissionTemplate3(bool isApplied, bool isFinished, bool havePrize, UserMission userMission);
        [TemplateTag]
        public void UserMission(string missionID, UserMissionTemplate3 template)
        {
            int id;
            if (!int.TryParse(missionID, out id))
            {
                //TODO:
            }
            UserMission userMission = MissionBO.Instance.GetUserMission(UserBO.Instance.GetCurrentUserID(), id);

            bool isApplied, isFinished, havePrize;
            if (userMission == null)
            {
                isApplied = false;
                isFinished = false;
            }
            else
            {
                isApplied = true;
                if (userMission.FinishPercent == 1)
                    isFinished = true;
                else
                    isFinished = false;

            }

            if (MissionBO.Instance.GetMission(id).Prize.PrizeTypes.Count > 0)
                havePrize = true;
            else
                havePrize = false;
            template(isApplied, isFinished, havePrize, userMission);
        }







        /// <summary>
        /// 用户任务完成度
        /// </summary>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public int GetUserMissionFinishPercent(UserMission userMission)
        {
            if (userMission == null)
                return 0;

            return (int)(userMission.FinishPercent * 100);
        }

        /// <summary>
        /// 是否显示申请任务按钮
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowApplyMissionButton(Mission mission, UserMission userMission)
        {
            if (userMission != null) //已经申请过了
                return false;
            return MissionBO.Instance.CanApplyMission(User.Current, mission);
        }

        /// <summary>
        /// 是否显示再次申请任务按钮
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowApplyMissionAgainButton(Mission mission, UserMission userMission)
        {
            return MissionBO.Instance.CanApplyMissionAgain(User.Current, mission, userMission);
        }

        /// <summary>
        /// 是否显示领取任务的按钮
        /// </summary>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowGetMissionPrizeButton(UserMission userMission)
        {
            if (userMission == null)
                return false;
            if (userMission.FinishPercent == 1 && userMission.IsPrized == false && userMission.Mission.Prize.PrizeTypes.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// 是否显示领取任务的按钮
        /// </summary>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowGetMissionPrizeButton(string status, UserMission userMission)
        {
            MissionStatus missionStatus;
            try
            {
                missionStatus = (MissionStatus)Enum.Parse(typeof(MissionStatus), status, true);
            }
            catch
            {
                missionStatus = MissionStatus.Underway;
            }
            return IsShowGetMissionPrizeButton(missionStatus, userMission);
        }

        private bool IsShowGetMissionPrizeButton(MissionStatus missionStatus, UserMission userMission)
        {
            if (missionStatus == MissionStatus.Underway && userMission.FinishPercent == 1 && userMission.Mission.Prize.PrizeTypes.Count > 0)
                return true;

            return false;
        }


        /// <summary>
        /// 是否显示放弃任务按钮
        /// </summary>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowAbandonMissionButton(UserMission userMission)
        {
            if (userMission == null)
                return true;
            if (userMission.Status == MissionStatus.Underway && userMission.FinishPercent != 1)
                return true;
            return false;
        }

        /// <summary>
        /// 是否显示放弃任务按钮
        /// </summary>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowAbandonMissionButton(string status, UserMission userMission)
        {
            MissionStatus missionStatus;
            try
            {
                missionStatus = (MissionStatus)Enum.Parse(typeof(MissionStatus), status, true);
            }
            catch
            {
                missionStatus = MissionStatus.Underway;
            }
            if (missionStatus == MissionStatus.Underway && userMission.FinishPercent < 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 是否显示重新申请任务按钮
        /// </summary>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowReApplyMissionButton(UserMission userMission)
        {
            if (userMission == null)
                return false;
            if (userMission.Status == MissionStatus.Abandon)
                return true;
            return false;
        }

        /// <summary>
        /// 是否显示任务步骤
        /// </summary>
        /// <param name="mission"></param>
        /// <param name="userMission"></param>
        /// <returns></returns>
        [TemplateFunction]
        public bool IsShowMissionProcedures(Mission mission, UserMission userMission)
        {
            if (!mission.MissionBase.HaveStepDescriptions)
                return false;
            if (userMission == null)
                return true;
            if (userMission.Status == MissionStatus.Underway)
                return true;
            return false;
        }
    }
}