//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Enums;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Extensions;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using MaxLabs.WebEngine.Plugin;
using System.Collections;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 任务相关业务逻辑
    /// </summary>
    public class MissionBO : BOBase<MissionBO>
    {

        private const string cacheKey_AllMissions = "mission/AllMissions";
        private const string cacheKey_NewMissionUsers = "mission/NewMissionUsers/{0}";
        private void ClearNewMissionUsersCache()
        {
            CacheUtil.RemoveBySearch("mission/NewMissionUsers/");
        }
        private const string cacheKey_NewUserMission = "mission/NewUserMission/{0}";
        private void ClearNewUserMission(int userID)
        {
            string key = string.Format(cacheKey_NewUserMission, userID);
            CacheUtil.Remove(key);
        }

        /// <summary>
        /// 移除所有与任务相关的缓存
        /// </summary>
        private void ClearAllMissionCache()
        {
            CacheUtil.RemoveBySearch("mission/");
        }

        private static List<MissionBase> missions = new List<MissionBase>();


        public BackendPermissions ManagePermission
        {
            get
            {
                return AllSettings.Current.BackendPermissions;
            }
        }

        /// <summary>
        /// 获取所有已注册的任务类型
        /// </summary>
        /// <returns></returns>
        public List<MissionBase> GetAllMissionBases()
        {
            return missions;
        }

        /// <summary>
        /// 获取某个任务类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MissionBase GetMissionBase(string type)
        {
            foreach (MissionBase mission in missions)
            {
                if (string.Compare(mission.Type, type, true) == 0)
                    return mission;
            }
            return null;
        }

        private object registMissionLocker = new object();
        /// <summary>
        /// 注册一个任务类型
        /// </summary>
        /// <param name="mission"></param>
        public void RegisterMission(MissionBase mission)
        {
            lock (registMissionLocker)
            {
                if (GetMissionBase(mission.Type) == null)
                    missions.Add(mission);
            }
        }

        /// <summary>
        /// 创建一个任务
        /// </summary>
        /// <param name="mission"></param>
        /// <returns></returns>
        public bool CreateMission(int operatorUserID, Mission mission)
        {
            if (false == ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Mission))
            {
                ThrowError<NoPermissionCreateMissionError>(new NoPermissionCreateMissionError());
                return false;
            }

            if (!CheckMission(mission))
                return false;
            MissionDao.Instance.CreateMission(mission);
            ClearAllMissionCache();
            return true;
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="mission"></param>
        /// <returns></returns>
        public bool UpdateMission(int operatorUserID, Mission mission)
        {
            if (false == ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Mission))
            {
                ThrowError<NoPermissionUpdateMissionError>(new NoPermissionUpdateMissionError());
                return false;
            }

            if (!CheckMission(mission))
                return false;
            MissionDao.Instance.UpdateMission(mission);
            ClearAllMissionCache();
            return true;
        }

        private bool CheckMission(Mission mission)
        {

            MissionBase missionBase = GetMissionBase(mission.Type);
            if (missionBase == null)
            {
                ThrowError<MissionBaseNotExistsError>(new MissionBaseNotExistsError("type", mission.Type));
                return false;
            }
            if (string.IsNullOrEmpty(mission.Name))
            {
                ThrowError<EmptyMissionNameError>(new EmptyMissionNameError("name"));
            }
            else if (StringUtil.GetByteCount(mission.Name) > Consts.Mission_MaxMissionNameLength)
            {
                ThrowError<MissionNameLengthError>(new MissionNameLengthError("name", mission.Name, Consts.Mission_MaxMissionNameLength));
            }

            if (StringUtil.GetByteCount(mission.IconUrl) > Consts.Mission_MaxMissionIconUrlLength)
            {
                ThrowError<MissionIconUrlLengthError>(new MissionIconUrlLengthError("iconUrl", mission.IconUrl, Consts.Mission_MaxMissionIconUrlLength));
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.UserGroup) && mission.Prize.UserGroups.Count == 0)
            {
                ThrowError<NeedPrizeUserGroupError>(new NeedPrizeUserGroupError("prize.userGroup"));
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Point))
            {
                bool hasError = true;
                foreach (int value in mission.Prize.Points)
                {
                    if (value > 0)
                    {
                        hasError = false;
                        break;
                    }
                }
                if (hasError)
                    ThrowError<NeedPrizePointError>(new NeedPrizePointError("prize.point"));
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Medal) && mission.Prize.Medals.Count == 0)
            {
                ThrowError<NeedPrizeMedalError>(new NeedPrizeMedalError("prize.medal"));
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.InviteSerial) && mission.Prize.InviteSerialCount < 1)
            {
                ThrowError<InvalidInviteSerialCountError>(new InvalidInviteSerialCountError("prize.inviteSerialCount", mission.Prize.InviteSerialCount));
            }

            Hashtable result = missionBase.CheckValues(mission.FinishCondition);

            if (result != null && result.Count > 0)
            {
                foreach (string key in result.Keys)
                {
                    ThrowError<CustomError>(new CustomError(key, result[key].ToString()));
                }
            }

            if (HasUnCatchedError)
                return false;
            return true;
        }

        /// <summary>
        /// 批量更新任务
        /// </summary>
        /// <param name="missionIDs"></param>
        /// <param name="names"></param>
        /// <param name="sortOrders"></param>
        /// <param name="iconUrls"></param>
        /// <param name="beginDates"></param>
        /// <param name="endDates"></param>
        public bool UpdateMissions(int operatorUserID, List<int> missionIDs, List<string> names, List<int?> categoryIDs, List<int> sortOrders, List<string> iconUrls, List<DateTime> beginDates, List<DateTime> endDates, List<bool> isEnables)
        {

            if (false == ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Mission))
            {
                ThrowError<NoPermissionUpdateMissionError>(new NoPermissionUpdateMissionError());
                return false;
            }

            for (int i = 0; i < missionIDs.Count; i++)
            {
                string name = names[i];
                string iconUrl = iconUrls[i];
                if (string.IsNullOrEmpty(name))
                {
                    ThrowError<EmptyMissionNameError>(new EmptyMissionNameError("name", i));
                }
                else if (StringUtil.GetByteCount(name) > Consts.Mission_MaxMissionNameLength)
                {
                    ThrowError<MissionNameLengthError>(new MissionNameLengthError("name", name, Consts.Mission_MaxMissionNameLength, i));
                }

                if (StringUtil.GetByteCount(iconUrl) > Consts.Mission_MaxMissionIconUrlLength)
                {
                    ThrowError<MissionIconUrlLengthError>(new MissionIconUrlLengthError("iconUrl", iconUrl, Consts.Mission_MaxMissionIconUrlLength, i));
                }
            }

            if (HasUnCatchedError)
                return false;

            MissionDao.Instance.UpdateMissions(missionIDs, names, categoryIDs, sortOrders, iconUrls, beginDates, endDates, isEnables);
            ClearAllMissionCache();

            return true;
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="missionIDs"></param>
        public bool DeleteMissions(int operatorUserID, IEnumerable<int> missionIDs)
        {
            if (false == ManagePermission.Can(operatorUserID, BackendPermissions.Action.Manage_Mission))
            {
                ThrowError<NoPermissionDeleteMissionError>(new NoPermissionDeleteMissionError());
                return false;
            }

            if (missionIDs == null || ValidateUtil.HasItems<int>(missionIDs) == false)
            {
                ThrowError(new NotSelectedMissionsError("missionIDs"));
                return false;
            }

            MissionDao.Instance.DeleteMissions(missionIDs);
            ClearAllMissionCache();

            List<int> tempMissionIDs = new List<int>();

            foreach (int id in missionIDs)
                tempMissionIDs.Add(id);

            Logs.LogManager.LogOperation(
                       new Mission_DeleteMissionByIDs(operatorUserID, UserBO.Instance.GetUser(operatorUserID).Name, IPUtil.GetCurrentIP(), tempMissionIDs)
                   );

            return true;
        }

        /// <summary>
        /// 所有任务 包括未启用的  没缓存
        /// </summary>
        /// <returns></returns>
        public MissionCollection GetAllMissions()
        {
            return MissionDao.Instance.GetAllMissions();
        }

        /// <summary>
        /// 所有已经启用的任务
        /// </summary>
        /// <returns></returns>
        public MissionCollection AllMissions()
        {
            MissionCollection missions;
            if (!CacheUtil.TryGetValue<MissionCollection>(cacheKey_AllMissions, out missions))
            {
                missions = new MissionCollection();
                MissionCollection tempMissions = MissionDao.Instance.GetAllMissions();

                foreach (Mission mission in tempMissions)
                {
                    if (mission.IsEnable)
                        missions.Add(mission);
                }

                CacheUtil.Set<MissionCollection>(cacheKey_AllMissions, missions, CacheTime.Normal, CacheExpiresType.Sliding);
            }
            return missions;
        }




        /// <summary>
        /// 按分页获取任务
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public MissionCollection GetMissions(int? categoryID, int pageNumber, int pageSize, bool includeDisable, out int totalCount)
        {

            if (pageNumber < 1)
                pageNumber = 1;

            MissionCollection allMissions;
            if (includeDisable)
                allMissions = GetAllMissions();
            else
                allMissions = AllMissions();

            MissionCollection tempMissions = new MissionCollection();

            foreach (Mission mission in allMissions)
            {
                if (categoryID == null || mission.CategoryID == categoryID)
                    tempMissions.Add(mission);
            }

            totalCount = tempMissions.Count;

            int count = pageNumber * pageSize > totalCount ? totalCount : pageNumber * pageSize;

            MissionCollection missions = new MissionCollection();
            for (int i = (pageNumber - 1) * pageSize; i < count; i++)
            {
                missions.Add(tempMissions[i]);
            }

            return missions;
        }

        public Mission GetMission(int missionID)
        {
            return GetMission(missionID, false);
        }
        /// <summary>
        /// 获取单个任务
        /// </summary>
        /// <param name="missionID"></param>
        /// <returns></returns>
        public Mission GetMission(int missionID, bool includeDisable)
        {
            if (missionID < 1)
            {
                ThrowError(new InvalidParamError("missionID"));
                return null;
            }

            MissionCollection missions;
            if (includeDisable)
                missions = GetAllMissions();
            else
                missions = AllMissions();

            foreach (Mission mission in missions)
            {
                if (mission.ID == missionID)
                    return mission;
                else
                {
                    if (mission.ChildMissions != null)
                    {
                        foreach (Mission child in mission.ChildMissions)
                        {
                            if (child.ID == missionID)
                                return child;
                        }
                    }
                }
            }
            return null;
        }


        public int GetUserMissionCount(int userID, MissionStatus status)
        {
            return MissionDao.Instance.GetUserMissionCount(userID, status);
        }

        /// <summary>
        /// 获取用户待参与的最优先的一个任务
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Mission GetNewUserMission(int userID)
        {

            if (userID < 1)
            {
                ThrowError(new InvalidParamError("userID"));
                return null;
            }

            int missionID;
            string key = string.Format(cacheKey_NewUserMission, userID);
            if (!CacheUtil.TryGetValue<int>(key, out missionID))
            {
                Mission mission = MissionDao.Instance.GetNewUserMission(userID);
                if (mission == null)
                    missionID = 0;
                else
                    missionID = mission.ID;

                CacheUtil.Set<int>(key, missionID, CacheTime.Normal, CacheExpiresType.Sliding);
            }
            if (missionID == 0)
                return null;
            return GetMission(missionID);
        }

        /// <summary>
        /// 获取用户新任务(待参与的任务)
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        public MissionCollection GetNewMissions(int userID, int? categoryID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;

            if (userID < 1)
            {
                ThrowError(new InvalidParamError("userID"));
                return new MissionCollection();
            }

            MissionCollection missions = MissionDao.Instance.GetNewMissions(userID, categoryID, pageNumber, pageSize, out totalCount);

            MissionCollection allmissions = GetAllMissions();

            for (int j = 0; j < missions.Count; j++)
            {
                for (int i = 0; i < allmissions.Count; i++)
                {
                    if (missions[j].ID == allmissions[i].ID)
                        missions[j] = allmissions[i];
                }
            }

            return missions;
        }

        /// <summary>
        /// 获取最近参与任务的用户
        /// </summary>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public UserMissionCollection GetNewMissionUsers(int getCount)
        {
            UserMissionCollection newMissionUsers;
            string cacheKey = string.Format(cacheKey_NewMissionUsers, getCount);
            if (!CacheUtil.TryGetValue<UserMissionCollection>(cacheKey, out newMissionUsers))
            {
                newMissionUsers = MissionDao.Instance.GetNewMissionUsers(getCount);
                CacheUtil.Set<UserMissionCollection>(cacheKey, newMissionUsers, CacheTime.Normal, CacheExpiresType.Sliding);
            }
            return newMissionUsers;
        }



        /// <summary>
        /// 获取用户的任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionID"></param>
        /// <returns></returns>
        public UserMission GetUserMission(int userID, int missionID)
        {

            if (userID < 1)
            {
                ThrowError(new InvalidParamError("userID"));
                return null;
            }

            if (missionID < 1)
            {
                ThrowError(new InvalidParamError("missionID"));
                return null;
            }

            UserMission userMission = MissionDao.Instance.GetUserMission(userID, missionID);

            if (userMission != null)
            {
                UserMissionCollection userMissions = new UserMissionCollection();
                userMissions.Add(userMission);
                UpdateUserMissionFinishPercents(userID, userMissions);
            }
            return userMission;
        }

        public UserMission GetStepUserMission(int userID, int[] missionIDs)
        {

            if (userID < 1)
            {
                ThrowError(new InvalidParamError("userID"));
                return null;
            }

            UserMission userMission = MissionDao.Instance.GetStepUserMission(userID, missionIDs);

            if (userMission != null)
            {
                UserMissionCollection userMissions = new UserMissionCollection();
                userMissions.Add(userMission);
                UpdateUserMissionFinishPercents(userID, userMissions);
            }
            return userMission;
        }


        public UserMissionCollection GetUserMissions(int userID, string type)
        {

            if (userID < 1)
            {
                ThrowError(new InvalidParamError("userID"));
                return new UserMissionCollection();
            }

            List<int> missionIDs = new List<int>();
            foreach (Mission mission in AllMissions())
            {
                if (string.Compare(mission.Type, type, true) == 0)
                    missionIDs.Add(mission.ID);
            }
            if (missionIDs.Count == 0)
                return new UserMissionCollection();

            return MissionDao.Instance.GetUserMissions(userID, missionIDs);
        }


        /// <summary>
        /// 用户申请任务
        /// </summary>
        /// <param name="userMission"></param>
        public bool ApplyMission(AuthUser operatorUser, int missionID)
        {

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (missionID < 1)
            {
                ThrowError(new InvalidParamError("missionID"));
                return false;
            }

            Mission mission = GetMission(missionID);
            if (mission == null)
            {
                ThrowError<MissionNotExistsError>(new MissionNotExistsError("missionID", missionID));
                return false;
            }

            MissionBase missionBase = GetMissionBase(mission.Type);
            if (missionBase == null)
            {
                ThrowError<MissionBaseNotExistsError>(new MissionBaseNotExistsError("Type", mission.Type));
                return false;
            }

            //检查申请条件
            if (!CanApplyMission(operatorUser, mission))
            {
                return false;
            }

            List<int> missionIDs = mission.ApplyCondition.OtherMissionIDs;

            if (missionIDs.Count > 0)
            {
                UserMissionCollection userMissions = MissionDao.Instance.GetUserMissions(operatorUser.UserID, missionIDs);

                StringBuilder missionNames = new StringBuilder();
                foreach (int id in missionIDs)
                {
                    bool have = false;
                    foreach (UserMission userMission in userMissions)
                    {
                        if (userMission.MissionID == id && userMission.FinishPercent == 1)
                        {
                            have = true;
                            break;
                        }
                    }
                    if (!have)
                    {
                        Mission tempMission = GetMission(id);
                        if (tempMission != null)
                        {
                            missionNames.Append(tempMission.Name + ",");
                        }
                    }
                }
                if (missionNames.Length > 0)
                {
                    ThrowError<ApplyMissionNeedFinishOtherMissionError>(new ApplyMissionNeedFinishOtherMissionError("ApplyMissionNeedFinishOtherMissionError", missionNames.ToString(0, missionNames.Length - 1)));
                    return false;
                }
            }

            bool isFail;
            int result;
            double finishPercent;
            lock (operatorUser)
            {
                if (!CheckApplyMissionPoint(operatorUser, mission))
                    return false;

                finishPercent = missionBase.GetFinishPercent(mission, operatorUser.UserID, mission.CycleTime, DateTimeUtil.Now, mission.FinishCondition, out isFail);

                result = MissionDao.Instance.ApplyMission(operatorUser.UserID, missionID, finishPercent);
            }

            switch (result)
            {
                case 0:
                case 3:
                    break;
                case 1:
                    ThrowError<MissionNotExistsError>(new MissionNotExistsError("MissionNotExistsError", missionID));
                    return false;
                case 2:
                    ThrowError<MissionHadAppliedError>(new MissionHadAppliedError("MissionHadAppliedError"));
                    return false;
                default: break;
            }

            int prizeCount = 0;
            //周期性的任务 上一次完成未领取奖励 自动给其奖励
            if (result == 3)
            {
                prizeCount++;
            }

            if (finishPercent == 1)
            {
                prizeCount++;
            }
            GetPrize(operatorUser.UserID, mission, prizeCount, true);


            ClearNewMissionUsersCache();
            ClearNewUserMission(operatorUser.UserID);
            //更新参与人次
            CacheUtil.Remove(cacheKey_AllMissions);

            operatorUser.ClearTotalMissionsCache();

            FeedBO.Instance.CreateApplyMissionFeed(operatorUser.UserID, mission.ID, mission.Name);

            if (mission.ChildMissions != null && mission.ChildMissions.Count > 0)
            {
                return ApplyMission(operatorUser, mission.ChildMissions[0].ID);
            }

            return true;
        }

        public bool CanApplyMission(AuthUser operatorUser, Mission mission)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            //检查申请条件
            UserRoleCollection userGroups = operatorUser.Roles;

            bool canApply = false;
            foreach (Guid userGroupID in mission.ApplyCondition.UserGroupIDs)
            {
                foreach (UserRole userGroup in userGroups)
                {
                    if (userGroup.RoleID == userGroupID)
                    {
                        canApply = true;
                        break;
                    }
                }
                if (canApply)
                    break;
            }
            if (!canApply && mission.ApplyCondition.UserGroupIDs.Count > 0)
            {
                StringBuilder userGroupNames = new StringBuilder();
                foreach (Guid groupID in mission.ApplyCondition.UserGroupIDs)
                {
                    foreach (Role userGroup in AllSettings.Current.RoleSettings.Roles)
                    {
                        if (userGroup.RoleID == groupID)
                        {
                            userGroupNames.Append(userGroup.Name).Append(",");
                            break;
                        }
                    }
                }
                if (userGroupNames.Length > 0)
                {
                    ThrowError<ApplyMissionNeedUserGroupError>(new ApplyMissionNeedUserGroupError("ApplyMissionNeedUserGroupError", userGroupNames.ToString(0, userGroupNames.Length - 1)));
                    return false;
                }
            }

            if (!CheckApplyMissionPoint(operatorUser, mission))
                return false;

            if (operatorUser.TotalTopics + operatorUser.TotalReplies < mission.ApplyCondition.TotalPosts)
            {
                ThrowError<ApplyMissionNeedTotalTopicsError>(new ApplyMissionNeedTotalTopicsError("ApplyMissionNeedTotalTopicsError", mission.ApplyCondition.TotalPosts, operatorUser.TotalTopics));
                return false;
            }

            if ((operatorUser.TotalOnlineTime / 60) < mission.ApplyCondition.OnlineTime)
            {
                ThrowError<ApplyMissionNeedOnlineTimeError>(new ApplyMissionNeedOnlineTimeError("ApplyMissionNeedOnlineTimeError", mission.ApplyCondition.OnlineTime, (operatorUser.TotalOnlineTime / 60)));
                return false;
            }

            if (mission.ApplyCondition.MaxApplyCount != 0 && mission.ApplyCondition.MaxApplyCount <= mission.TotalUsers)
            {
                ThrowError<ExceededMaxApplyError>(new ExceededMaxApplyError("ExceededMaxApplyError"));
                return false;
            }
            return true;
        }

        private bool CheckApplyMissionPoint(AuthUser operatorUser, Mission mission)
        {
            UserPointCollection points = AllSettings.Current.PointSettings.EnabledUserPoints;

            for (int i = 0; i < points.Count; i++)
            {
                int pointIndex = (int)points[i].Type;
                if (operatorUser.ExtendedPoints[pointIndex] < mission.ApplyCondition.Points[pointIndex])
                {
                    ThrowError<ApplyMissionNoEnoughPointError>(new ApplyMissionNoEnoughPointError("ApplyMissionNoEnoughPointError", points[i].Name, mission.ApplyCondition.Points[pointIndex], operatorUser.ExtendedPoints[pointIndex]));
                    return false;
                }
                if (mission.DeductPoint[pointIndex] > 0 && (operatorUser.ExtendedPoints[pointIndex] - mission.DeductPoint[pointIndex] < points[i].MinValue))
                {
                    ThrowError<ApplyMissionNoEnoughDeductPointtError>(new ApplyMissionNoEnoughDeductPointtError("ApplyMissionNoEnoughDeductPointtError", points[i].Name, mission.DeductPoint[pointIndex], points[i].MinValue, operatorUser.ExtendedPoints[pointIndex]));
                    return false;
                }
            }


            //总积分
            if (operatorUser.Points < mission.ApplyCondition.TotalPoint)
            {
                ThrowError<ApplyMissionNoEnoughTotalPointError>(new ApplyMissionNoEnoughTotalPointError("ApplyMissionNoEnoughTotalPointError", mission.ApplyCondition.TotalPoint, operatorUser.Points));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 对于周期性任务是否可以再次申请（如果以前未申请过返回false）
        /// </summary>
        /// <returns></returns>
        public bool CanApplyMissionAgain(AuthUser operatorUser, Mission mission, UserMission userMission)
        {
            if (mission.CycleTime > 0 && userMission != null)
            {
                if (userMission.CreateDate.AddSeconds(mission.CycleTime) < DateTimeUtil.Now && userMission.Status == MissionStatus.Finish)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 用户领取任务奖励
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionID"></param>
        public bool GetMissionPrize(AuthUser operatorUser, int missionID)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            Mission mission = GetMission(missionID);
            Mission stepmission = null;

            int? nextmissionid = null;

            int result = 0;

            if (mission.ChildMissions.Count == 0)
            {
                result = MissionDao.Instance.SetUserMissionIsPrized(operatorUser.UserID, missionID);
            }
            else
            {
                int[] missionIDs = new int[mission.ChildMissions.Count];

                for (int i = 0; i < missionIDs.Length; i++)
                {
                    missionIDs[i] = mission.ChildMissions[i].ID;
                }

                UserMission usermission = MissionBO.Instance.GetStepUserMission(operatorUser.UserID, missionIDs);

                stepmission = usermission.Mission;

                result = MissionDao.Instance.SetUserMissionIsPrized(operatorUser.UserID, stepmission.ID);

                for (int i = 0; i < missionIDs.Length; i++)
                {
                    if (mission.ChildMissions[i].ID == stepmission.ID && i < missionIDs.Length - 1)
                    {
                        nextmissionid = mission.ChildMissions[i + 1].ID;
                    }
                }

                if (nextmissionid == null && result == 0)
                {
                    result = MissionDao.Instance.SetUserMissionIsPrized(operatorUser.UserID, missionID);
                }
            }

            if (result == 1)
            {
                ThrowError<MissionNotExistsError>(new MissionNotExistsError("MissionNotExistsError", missionID));
                return false;
            }
            else if (result == 2)
            {
                ThrowError<MissionNotFinishError>(new MissionNotFinishError("MissionNotFinishError"));
                return false;
            }
            else if (result == 3)
            {
                ThrowError<MissionHadPrizedError>(new MissionHadPrizedError("MissionHadPrizedError"));
                return false;
            }

            GetPrize(operatorUser.UserID, stepmission != null ? stepmission : mission, 1);

            if (nextmissionid != null)
            {
                ApplyMission(operatorUser, nextmissionid.Value);
            }

            return true;
        }

        private void GetPrize(int userID, Mission mission, int prizeCount)
        {
            GetPrize(userID, mission, prizeCount, false);
        }

        /// <summary>
        ///  领取奖励
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="mission"></param>
        /// <param name="prizeCount"></param>
        /// <param name="deductPoint">仅当申请任务的时候 为true</param>
        private void GetPrize(int userID, Mission mission, int prizeCount, bool deductPoint)
        {
            int[] values = null;
            UserPointCollection points = null;
            bool hasDeductPoints = false;
            if (deductPoint)//扣除积分
            {
                values = new int[8];
                points = AllSettings.Current.PointSettings.UserPoints;
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 8 || mission.DeductPoint.Length < (i + 1))
                        break;

                    if (mission.DeductPoint[i] > 0 && points[i].Enable)
                    {
                        hasDeductPoints = true;
                        values[i] = -mission.DeductPoint[i];
                    }
                }
            }

            if (prizeCount == 0)
            {
                if (hasDeductPoints)
                    UserBO.Instance.UpdateUserPoints(userID, false, false, values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7],"做任务","申请任务“"+mission.Name+"”扣除积分");
                return;
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Point))
            {
                if (!deductPoint)
                {
                    points = AllSettings.Current.PointSettings.UserPoints;
                    values = new int[8];
                }

                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 8 || mission.Prize.Points.Length < (i + 1))
                        break;

                    if (mission.Prize.Points[i] > 0 && points[i].Enable)
                    {
                        values[i] = values[i] + mission.Prize.Points[i] * prizeCount;
                    }
                }
                UserBO.Instance.UpdateUserPoints(userID, false, false, values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], "做任务", "完成任务“" + mission.Name + "”奖励积分");
            }
            else if (hasDeductPoints)
                UserBO.Instance.UpdateUserPoints(userID, false, false, values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7],"做任务","申请任务“"+mission.Name+"”扣除积分");

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.UserGroup))
            {
                UserRoleCollection members = new UserRoleCollection();
                foreach (KeyValuePair<Guid, long> pair in mission.Prize.UserGroups)
                {
                    UserRole member = new UserRole();
                    member.UserID = userID;
                    member.RoleID = pair.Key;
                    member.BeginDate = DateTimeUtil.Now;
                    if (pair.Value > 0)
                        member.EndDate = DateTimeUtil.Now.AddSeconds(pair.Value);
                    else
                        member.EndDate = DateTime.MaxValue;
                    members.Add(member);
                }
                UserBO.Instance.AddUsersToRoles(members);
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Medal))
            {
                Dictionary<int, int> medalIDs = new Dictionary<int, int>();
                List<DateTime> endDates = new List<DateTime>();
                foreach (PrizeMedal prizeMedal in mission.Prize.Medals)
                {
                    medalIDs.Add(prizeMedal.MedalID, prizeMedal.MedalLevelID);

                    if (prizeMedal.Seconds > 0)
                        endDates.Add(DateTimeUtil.Now.AddSeconds(prizeMedal.Seconds));
                    else
                        endDates.Add(DateTime.MaxValue);
                }

                UserBO.Instance.AddMedalsToUser(null, userID, medalIDs, endDates, true);
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.InviteSerial))
            {
                List<int> userIDs = new List<int>();
                userIDs.Add(userID);
                InviteBO.Instance.CreateInviteSerials(userIDs, mission.Prize.InviteSerialCount * prizeCount);
            }

            if (mission.Prize.PrizeTypes.Contains(MissionPrizeType.Prop))
            {
                PropBO.Instance.PrizePropForMission(userID, mission.Name, mission.Prize.Props);
            }

            AuthUser user = UserBO.Instance.GetUserFromCache<AuthUser>(userID);
            if (user != null)
                user.ClearTotalMissionsCache();
        }

        /// <summary>
        /// 获取用户参与的任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public UserMissionCollection GetUserMissions(int userID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;

            if (userID < 1)
            {
                ThrowError(new InvalidParamError("userID"));
                return new UserMissionCollection();
            }

            UserMissionCollection userMissions = MissionDao.Instance.GetUserMissions(userID, pageNumber, pageSize, out totalCount);

            UpdateUserMissionFinishPercents(userID, userMissions);

            return userMissions;
        }

        /// <summary>
        /// 更新用户任务完成百分比
        /// </summary>
        /// <param name="userMissions"></param>
        private void UpdateUserMissionFinishPercents(int userID, UserMissionCollection userMissions)
        {
            Dictionary<int, double> percents = new Dictionary<int, double>();
            List<int> failMissionIDs = new List<int>();
            foreach (UserMission userMission in userMissions)
            {
                if (userMission.FinishPercent < 1 && userMission.Status == MissionStatus.Underway)
                {
                    bool isFail;
                    userMission.FinishPercent = userMission.Mission.MissionBase.GetFinishPercent(userMission.Mission, userMission.UserID, userMission.Mission.CycleTime, userMission.CreateDate, userMission.Mission.FinishCondition, out isFail);
                    if (userMission.FinishPercent == 1 || isFail)//如果失败了 更新失败前完成的百分比
                    {
                        percents.Add(userMission.MissionID, userMission.FinishPercent);
                    }
                    if (isFail)
                        failMissionIDs.Add(userMission.MissionID);
                }
            }
            if (percents.Count > 0)
                MissionDao.Instance.UpdateUserMissionFinishPercents(userID, percents);
            if (failMissionIDs.Count > 0)
                MissionDao.Instance.UpdateUserMissionStatus(userID, failMissionIDs, MissionStatus.OverTime);
        }

        /// <summary>
        /// 获取任务的参与用户
        /// </summary>
        /// <param name="missionID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalUsers"></param>
        /// <returns></returns>
        public UserMissionCollection GetMissionUsers(int missionID, int pageNumber, int pageSize, out int totalUsers)
        {
            return MissionDao.Instance.GetMissionUsers(missionID, pageNumber, pageSize, out totalUsers);
        }

        /// <summary>
        /// 放弃任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionID"></param>
        /// <returns></returns>
        public bool AbandonMission(AuthUser operatorUser, int missionID)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (MissionDao.Instance.AbandonMission(operatorUser.UserID, missionID))
            {
                ClearNewMissionUsersCache();
                ClearNewUserMission(operatorUser.UserID);
                //更新参与人次
                CacheUtil.Remove(cacheKey_AllMissions);

                operatorUser.ClearTotalMissionsCache();

                return true;
            }
            else
            {
                ThrowError(new AbandonMissionError("missionID"));
                return false;
            }
        }


        public string GetPointDescription(int[] points, string lineFormat, string separator, string action)
        {
            StringBuilder pointString = new StringBuilder();
            UserPointCollection userPoints = AllSettings.Current.PointSettings.UserPoints;

            for (int i = 0; i < userPoints.Count; i++)
            {
                if (points.Length == i)
                    break;
                if (points[i] > 0 && userPoints[i].Enable)
                {
                    pointString.AppendFormat(lineFormat, action + userPoints[i].Name, points[i], userPoints[i].UnitName).Append(separator);

                }
            }

            if (pointString.Length > 0)
            {
                return pointString.ToString(0, pointString.Length - separator.Length);
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prize"></param>
        /// <param name="lineFormat">{0}{1}{2}</param>
        /// <param name="separator"><br /></param>
        /// <returns></returns>
        public string GetMissionPrizeDescription(MissionPrize prize, string lineFormat, string separator)
        {
            StringBuilder description = new StringBuilder();

            if (prize.PrizeTypes.Contains(MissionPrizeType.Point))
            {
                description.Append(GetPointDescription(prize.Points, lineFormat, separator, "")).Append(separator);
            }

            if (prize.PrizeTypes.Contains(MissionPrizeType.UserGroup))
            {
                if (prize.UserGroups.Count > 0)
                {
                    RoleCollection groups = AllSettings.Current.RoleSettings.Roles;

                    StringBuilder userGroupNames = new StringBuilder();


                    foreach (Role group in groups)
                    {
                        long seconds;
                        if (prize.UserGroups.TryGetValue(group.RoleID, out seconds))
                        {
                            description.AppendFormat(lineFormat, "加入", group.Name, "(" + DateTimeUtil.FormatSecond(seconds) + ")").Append(separator);
                            //userGroupNames.AppendFormat("{0}({1}),", group.Name, DateTimeUtil.FormatSecond(seconds));
                        }
                    }
                    //if (userGroupNames.Length > 0)
                    //    description.AppendFormat("用户组:{0};", userGroupNames.ToString(0, userGroupNames.Length - 1));
                }
            }

            if (prize.PrizeTypes.Contains(MissionPrizeType.Medal))
            {
                if (prize.Medals.Count > 0)
                {
                    StringBuilder medalNames = new StringBuilder();
                    foreach (PrizeMedal prizeMedal in prize.Medals)
                    {
                        Medal medal = AllSettings.Current.MedalSettings.Medals.GetValue(prizeMedal.MedalID);
                        if (medal == null)
                            continue;

                        MedalLevel level = null;
                        foreach (MedalLevel medalLevel in medal.Levels)
                        {
                            if (medalLevel.ID == prizeMedal.MedalLevelID)
                            {
                                level = medalLevel;
                                break;
                            }
                        }
                        if (level == null)
                            continue;

                        string title;
                        if (level.Name != string.Empty)
                        {
                            title = medal.Name + "(" + level.Name + ")";
                        }
                        else
                            title = medal.Name;

                        description.AppendFormat(lineFormat, "点亮图标", "<img src=\"" + level.LogoUrl + "\" alt=\"" + title + "\" />", "(" + DateTimeUtil.FormatSecond(prizeMedal.Seconds) + ")").Append(separator);
                    }

                }
            }

            if (prize.PrizeTypes.Contains(MissionPrizeType.InviteSerial))
            {
                if (prize.InviteSerialCount > 0)
                {
                    description.AppendFormat(lineFormat, "邀请码", prize.InviteSerialCount, "个").Append(separator);
                    //description.AppendFormat("邀请码:{0}个;",prize.InviteSerialCount);
                }
            }

            if (prize.PrizeTypes.Contains(MissionPrizeType.Prop))
            {
                foreach (int propID in prize.Props.Keys)
                {
                    Prop prop = PropBO.Instance.GetPropByID(propID);

                    if (prop == null)
                        continue;

                    description.AppendFormat(lineFormat, "道具", prop.Name, prize.Props[propID] + "个").Append(separator);
                }
            }

            if (description.Length > 0)
                return description.ToString(0, description.Length - separator.Length);
            else
                return string.Empty;
        }

        /// <summary>
        /// 申请任务条件的描述
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string GetApplyMissionConditionDescription(ApplyMissionCondition condition)
        {
            StringBuilder description = new StringBuilder();

            StringBuilder pointString = new StringBuilder();
            UserPointCollection points = AllSettings.Current.PointSettings.UserPoints;

            if (condition.TotalPoint > 0)
                pointString.AppendFormat(AllSettings.Current.PointSettings.GeneralPointName + ":{0},", condition.TotalPoint);

            for (int i = 0; i < points.Count; i++)
            {
                if (condition.Points.Length == i)
                    break;
                if (condition.Points[i] > 0)
                {
                    pointString.AppendFormat("{0}:{1},", points[i].Name, condition.Points[i]);
                }
            }

            if (pointString.Length > 0)
            {
                description.Append(pointString.ToString(0, pointString.Length - 1) + ";");
            }

            if (condition.UserGroupIDs.Count > 0)
            {
                StringBuilder userGroupNames = new StringBuilder();
                RoleCollection groups = AllSettings.Current.RoleSettings.Roles;

                foreach (Role group in groups)
                {
                    if (condition.UserGroupIDs.Contains(group.RoleID))
                    {
                        userGroupNames.Append(group.Name + ",");
                    }
                }
                if (userGroupNames.Length > 0)
                    description.AppendFormat("用户组:{0};", userGroupNames.ToString(0, userGroupNames.Length - 1));
            }



            if (condition.TotalPosts > 0)
            {
                description.AppendFormat("发帖数:{0};", condition.TotalPosts);
            }

            if (condition.OnlineTime > 0)
            {
                description.AppendFormat("在线时间:{0}小时;", condition.OnlineTime);
            }

            if (condition.OtherMissionIDs.Count > 0)
            {
                StringBuilder missionNames = new StringBuilder();

                foreach (Mission mission in AllMissions())
                {
                    if (condition.OtherMissionIDs.Contains(mission.ID))
                        missionNames.Append(mission.Name + ",");
                }
                if (missionNames.Length > 0)
                    description.AppendFormat("先完成任务:{0};", missionNames.ToString(0, missionNames.Length - 1));
            }

            if (condition.MaxApplyCount > 0)
            {
                description.AppendFormat("申请人次上限:{0}人次;", condition.MaxApplyCount);
            }

            if (description.Length > 0)
                return description.ToString();
            else
                return null;
        }

        public MissionCategoryCollection GetMissionCategories()
        {
            return MissionDao.Instance.GetMissionCategories();
        }

        public MissionCategory GetMissionCategory(int categoryID)
        {
            return MissionDao.Instance.GetMissionCategory(categoryID);
        }

        public bool CreateMissionCategory(int userID, string categoryName, out int newCategoryID)
        {
            newCategoryID = 0;

            if (AllSettings.Current.BackendPermissions.Can(userID, BackendPermissions.Action.Manage_Mission_Category) == false)
            {
                ThrowError<CustomError>(new CustomError("", "没有权限管理任务分类"));
                return false;
            }
            else if (ValidateCategoryName(categoryName) == false)
            {
                return false;
            }

            bool isCreated = MissionDao.Instance.CreateMissionCategory(userID, categoryName, out newCategoryID);

            return isCreated;
        }

        public bool UpdateMissionCategory(int userID, int categoryID, string categoryName)
        {
            if (AllSettings.Current.BackendPermissions.Can(userID, BackendPermissions.Action.Manage_Mission_Category) == false)
            {
                ThrowError<CustomError>(new CustomError("", "没有权限管理任务分类"));
                return false;
            }
            else if (ValidateCategoryName(categoryName) == false)
            {
                return false;
            }

            bool isCreated = MissionDao.Instance.UpdateMissionCategory(categoryID, categoryName);

            return isCreated;
        }

        public bool DeleteMissionCategories(AuthUser operatorUser, int[] ids)
        {
            if (AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_Mission_Category) == false)
            {
                ThrowError<CustomError>(new CustomError("", "没有权限管理任务分类"));
                return false;
            }

            return MissionDao.Instance.DeleteMissionCategories(ids);
        }

        private bool ValidateCategoryName(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                ThrowError(new CustomError("categoryName", "分类名称不能放空"));
                return false;
            }

            if (categoryName[0] == ' ' || categoryName[categoryName.Length - 1] == ' ')
            {
                ThrowError(new CustomError("categoryName", "分类名称前后不能是空格"));
            }

            if (categoryName.Length > 20)
            {
                ThrowError(new CustomError("categoryName", "分类名称超过20个字"));
                return false;
            }

            return true;
        }
    }
}