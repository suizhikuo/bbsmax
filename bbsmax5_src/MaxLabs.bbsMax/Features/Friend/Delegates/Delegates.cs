//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 添加好友
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="hisGroupID"></param>
    /// <param name="groupID"></param>
    /// <param name="hisGroupID">对方的好友组ID</param>
    public delegate void AcceptFriend(int userID,int friendUserID,int groupID,int hisGroupID);

    /// <summary>
    /// 删除好友
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="friendID"></param>
    public delegate void RemoveFriend( int userID , IEnumerable<int> friendIDs);

    /// <summary>
    /// 移动好友
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="friendID"></param>
    /// <param name="newGroupID"></param>
    public delegate void MoveFriend(int userID , IEnumerable<int> friendIDs , int newGroupID);

    /// <summary>
    /// 创建好友分组
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="group"></param>
    public delegate void FriendGroupCreated(int userID, FriendGroup group);

    /// <summary>
    /// 删除好友分组
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="groupID"></param>
    public delegate void FriendGroupDeleted(int userID, int groupID,bool deleteFriend);

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="group"></param>
    public delegate void FriendGroupUpdated(int userID ,FriendGroup group);


    /// <summary>
    /// 屏蔽
    /// </summary>
    public delegate void FriendGroupShielded(int userID, IEnumerable<int> groupIDs, bool isShield);


    public delegate void DeleteFromBlacklist(int userID, int userIDInBlacklist);


    public delegate void AddUsersToBlacklist(int userID, IEnumerable<int> userIdsToAdd);


    public delegate void UpdateFriendHot(int userID, MaxLabs.bbsMax.Enums.HotType type, int friendUserID);

}