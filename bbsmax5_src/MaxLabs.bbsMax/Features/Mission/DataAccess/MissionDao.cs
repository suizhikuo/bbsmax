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

using MaxLabs.bbsMax.Enums;

using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class MissionDao : DaoBase<MissionDao>
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="mission"></param>
        /// <returns></returns>
        public abstract int CreateMission(Mission mission);

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="mission"></param>
        public abstract void UpdateMission(Mission mission);

        /// <summary>
        /// 批量更新任务
        /// </summary>
        /// <param name="missionIDs"></param>
        /// <param name="names"></param>
        /// <param name="sortOrders"></param>
        /// <param name="iconUrls"></param>
        /// <param name="beginDates"></param>
        /// <param name="endDates"></param>
        public abstract void UpdateMissions(List<int> missionIDs, List<string> names, List<int?> categoryIDs, List<int> sortOrders, List<string> iconUrls, List<DateTime> beginDates, List<DateTime> endDates, List<bool> isEnables);

        /// <summary>
        /// 获取所有任务
        /// </summary>
        /// <returns></returns>
        public abstract MissionCollection GetAllMissions();

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="missionIDs"></param>
        public abstract void DeleteMissions(IEnumerable<int> missionIDs);


        /// <summary>
        /// 用户申请任务
        /// </summary>
        /// <param name="userMission"></param>
        public abstract int ApplyMission(int userID,int missionID,double finishPercent);


        /// <summary>
        /// 获取用户的任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionID"></param>
        /// <returns></returns>
        public abstract UserMission GetUserMission(int userID, int missionID);

        /// <summary>
        /// 获取任务的参与用户
        /// </summary>
        /// <param name="missionID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalUsers"></param>
        /// <returns></returns>
        public abstract UserMissionCollection GetMissionUsers(int missionID,int pageNumber,int pageSize,out int totalUsers);

        /// <summary>
        /// 获取用户参与的任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract UserMissionCollection GetUserMissions(int userID, int pageNumber, int pageSize,out int totalCount);

        /// <summary>
        /// 获取用户特定类型的任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionIDs"></param>
        /// <returns></returns>
        public abstract UserMissionCollection GetUserMissions(int userID, IEnumerable<int> missionIDs);

        /// <summary>
        /// 设置用户的任务已经奖励过
        /// </summary>
        /// <param name="userID"></param>
        public abstract int SetUserMissionIsPrized(int userID, int missionID);


        /// <summary>
        /// 更新用户任务完成百分比
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionPercents">int:missionID folat:percent</param>
        public abstract void UpdateUserMissionFinishPercents(int userID, Dictionary<int,double> missionPercents);


        /// <summary>
        /// 更新用户任务状态
        /// </summary>
        /// <param name="status"></param>
        public abstract void UpdateUserMissionStatus(int userID, IEnumerable<int> missionIDs, MissionStatus status);


        /// <summary>
        /// 放弃任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="missionID"></param>
        /// <returns></returns>
        public abstract bool AbandonMission(int userID, int missionID);

        /// <summary>
        /// 获取用户待参与的最优先的一个任务
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public abstract Mission GetNewUserMission(int userID);

        /// <summary>
        /// 获取用户新任务，待参与的任务
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        public abstract MissionCollection GetNewMissions(int userID, int? categoryID, int pageNumber, int pageSize, out int totalCount);

        public abstract int GetUserMissionCount(int userID, MissionStatus status);

        /// <summary>
        /// 获取最近参与任务的用户
        /// </summary>
        /// <param name="getCount"></param>
        /// <returns></returns>
        public abstract UserMissionCollection GetNewMissionUsers(int getCount);

        public abstract MissionCategoryCollection GetMissionCategories();

        public abstract MissionCategory GetMissionCategory(int categoryID);

        public abstract bool CreateMissionCategory(int userID, string categoryName, out int newCategoryID);

        public abstract bool UpdateMissionCategory(int categoryID, string categoryName);

        public abstract bool DeleteMissionCategories(int[] ids);

        public abstract UserMission GetStepUserMission(int userID, int[] missionIDs);
    }


    public enum GetPrizeResult
    {
        Success = 0,

        /// <summary>
        /// 任务不存在
        /// </summary>
        MissionNotIsExists = 1,

        /// <summary>
        /// 任务未完成
        /// </summary>
        NotFinish = 2,
    }
}