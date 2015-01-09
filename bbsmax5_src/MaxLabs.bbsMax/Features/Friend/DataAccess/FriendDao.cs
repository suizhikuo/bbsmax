//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.Passport.Proxy;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class FriendDao : DaoBase<FriendDao>
    {
        /*
        public abstract void Passport_WriteFriends(int userID, IEnumerable<FriendGroupProxy> groups);
        */
        #region 好友分组


        #region 为了PASSPORT
        public abstract int RenameFriendGroups(int userID, List<KeyValuePair <int, string>> newGroupNames);
        public abstract int CreateFriends(int userID, List<KeyValuePair<int, int>> groupAndFriendUserID);
        public abstract int AddFriendGroups(int userID, List<KeyValuePair <int, string>> friendGroup);
        public abstract int DeleteFriendGroups(int userID, IEnumerable<int> groupIDs);
        #endregion

        public abstract FriendGroupCollection GetFriendGroups(int userID);

        public abstract int AddFriendGroup(int userID, int? groupID, string friendGroupName, int maxGroupCount, out FriendGroup newGroup);



        public abstract bool DeleteFriendGroup(int userID, int groupID, bool deleteFriends);

        public abstract int RenameFriendGroup(int userID, int groupID, string newGroupName);

        public abstract bool ShieldFriendGroup(int userID, int groupID, bool isShield);

        public abstract void ShieldFriendGroups(int userID, IEnumerable<int> groupIds, bool isShield);



        #endregion

        #region 好友

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        /// <param name="fromFriendGroupID"></param>
        /// <param name="toFriendGroupID"></param>
        public abstract void AcceptAddFriend(int operatorID, int tryAddUserID, int tryAddGroupID, int groupIDToAdd);

        /// <summary>
        /// 删除好友关系
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        public abstract void DeleteFriend(int userID, int friendID);

        public abstract void DeleteFriends(int userID, IEnumerable<int> friendUserIds);

        ///// <summary>
        ///// 更新好友分组
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="friendGroup"></param>
        //public abstract void ModifyFriendGroupName(int userID, string friendGroups);

        /// <summary>
        /// 批量更新好友的组
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        /// <param name="groupID"></param>
        public abstract void MoveFriends(int userID, IEnumerable<int> friendUserIDs, int groupID);

        /// <summary>
        /// 单个更新好友的组
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        /// <param name="groupID"></param>
        public abstract void MoveFriend(int userID, int friendUserID, int groupID);

        /// <summary>
        /// 更新好友间的热度
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        public abstract void UpdateFriendHot(HotType type, int userID, int friendUserID);

        /// <summary>
        /// 根据用户ID获取用户所有好友
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public abstract FriendCollection GetFriendsAndBlacklist(int userID);

        /// <summary>
        /// 好友的好友的ID集合
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract IList<int> SelectMayFriendIDs(int userID, int count);

        #endregion

        #region 黑名单

        public abstract bool AddUsersToBlacklist(int userID, IEnumerable<int> userIdsToAdd);
        public abstract void DeleteFromBlackList(int myId, int blackUserID);
        public abstract void DeleteFromBlackList(int myId,IEnumerable<int> blackUserID);

        #endregion
    }
}