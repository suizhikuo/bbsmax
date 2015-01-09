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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Errors;
#if !Passport
using MaxLabs.bbsMax.PassportServerInterface;
using System.Threading;
#endif

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 好友相关的业务逻辑
    /// </summary>
    public class FriendBO : BOBase<FriendBO>
    {
        #region Events

        public static event AcceptFriend OnAcceptFriend;

        public static event RemoveFriend OnRemoveFriend;

        public static event MoveFriend OnMoveFriend;

        public static event FriendGroupCreated OnFriendGroupCreated;

        public static event FriendGroupDeleted OnFriendGroupDeleted;

        public static event FriendGroupUpdated OnFriendGroupUpdated;

        public static event FriendGroupShielded OnFriendGroupShielded;

        public static event DeleteFromBlacklist OnDeleteFromBlacklist;

        public static event AddUsersToBlacklist OnAddUsersToBlacklist;

        public static event UpdateFriendHot OnUpdateFriendHot;
        #endregion

        #region passport client only

#if !Passport

        /// <summary>
        /// 完全同步好友
        /// </summary>
        /// <param name="userID">用户</param>
        /// <returns></returns>
        public bool Client_SyncAllFriend(User user)
        {
            if (user == null)
                return false;

            List<int> removeGroup = new List<int>();
            List<KeyValuePair<int, string>> renameGroup = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<int, string>> createGroup = new List<KeyValuePair<int, string>>();

            List<int> removeFriends = new List<int>();
            List<KeyValuePair<int, int>> createFriends = new List<KeyValuePair<int,int>>();
            List<KeyValuePair<int, int>> passportFriends = new List<KeyValuePair<int, int>>(user.Friends.Count);

            List<int> createBlackUsers = new List<int>();
            List<int> removeBlackUsers = new List<int>();

            List<FriendProxy> passportBlackList = new List<FriendProxy>();
            FriendGroupProxy[] passportGroups  = Globals.PassportClient.PassportService.Friend_GetFriendGroupsWithFriends(user.UserID);

#region 比较分组

            FriendGroupCollection userFriendGroups = new FriendGroupCollection(user.FriendGroups);

            foreach (FriendGroupProxy groupProxy in passportGroups)  
            {
                if (groupProxy.GroupID == -1) 
                {
                    if (groupProxy.Friends.Length>0)
                        passportBlackList = new List<FriendProxy>( groupProxy.Friends);

                    continue; 
                } //黑名单列表

                foreach (FriendProxy fp in groupProxy.Friends) //复制出好友
                {
                    passportFriends.Add(new KeyValuePair<int, int>(fp.UserID, fp.GroupID));
                }

                if (groupProxy.GroupID == 0) //默认分组
                    continue;

                int j = -1;

                FriendGroup group;
                for (int i = 0; i < userFriendGroups.Count; i++)
                {
                    group = userFriendGroups[i];
                    if (group.GroupID == groupProxy.GroupID)
                    {
                        j = i;
                        if (group.Name != groupProxy.Name)
                        {
                            renameGroup.Add(new KeyValuePair<int,string>( groupProxy.GroupID, groupProxy.Name)); //重命名的
                        }

                        break;
                    }
                }

                if (j == -1)
                {
                    createGroup.Add(new KeyValuePair<int,string>( groupProxy.GroupID, groupProxy.Name)); //新建的
                }
                else
                {
                    userFriendGroups.RemoveAt(j);
                }
            }
              
            foreach (FriendGroup group in userFriendGroups)  //被删除的
            {
                if (group.GroupID == 0) continue;
                removeGroup.Add(group.GroupID);
            }
#endregion
#region 比较好友
            FriendCollection userFriends = user.Friends;

            foreach (Friend f in userFriends)
            {
                int j = -1;

                for (int i = 0; i < passportFriends.Count; i++)
                {
                    KeyValuePair<int, int> friendItem = passportFriends[i];
                    if (friendItem.Key == f.UserID)
                    {
                        if (f.GroupID != friendItem.Value)
                        {
                            //移动分组
                            removeFriends.Add(f.UserID);
                            createFriends.Add(new KeyValuePair<int, int>( friendItem.Value, f.UserID));
                        }
                        j = i;
                        break;
                    }
                }
                
                if(j>-1)
                {
                     passportFriends.RemoveAt(j);
                }
                else
                {
                    removeFriends.Add(f.UserID);
                }
            }

            if (passportFriends.Count > 0)
            {
                foreach (KeyValuePair<int, int> p in passportFriends)
                {
                    createFriends.Add(new KeyValuePair<int,int>(p.Value, p.Key));
                }
            }
#endregion

            foreach( BlacklistItem b in userFriends.Blacklist )
            {
                int j = -1;
                for (int i = 0; i < passportBlackList.Count; i++)
                {
                    if (passportBlackList[i].UserID == b.UserID)
                    {
                        j = i;
                        break;
                    }
                }

                if (j == -1)
                {
                    removeBlackUsers.Add(b.UserID);
                }
            }

            if (passportBlackList.Count > 0)
            {
                foreach (FriendProxy f in passportBlackList)
                {
                    createBlackUsers.Add(f.UserID);
                }
            }


            bool friendChanged = removeGroup.Count > 0
                || renameGroup.Count > 0
                || createGroup.Count > 0
                || createFriends.Count > 0
                || removeGroup.Count > 0
                || createBlackUsers.Count > 0
                || removeBlackUsers.Count > 0;

            //
            if (removeGroup.Count > 0) FriendDao.Instance.DeleteFriendGroups(user.UserID, removeGroup);
            if (renameGroup.Count > 0) FriendDao.Instance.RenameFriendGroups(user.UserID, renameGroup);
            if (createGroup.Count > 0) FriendDao.Instance.AddFriendGroups(user.UserID, createGroup);

            //删除被删除的好友
            if (removeFriends.Count > 0)
                FriendDao.Instance.DeleteFriends(user.UserID, removeFriends);

            if (createFriends.Count > 0)
                FriendDao.Instance.CreateFriends(user.UserID, createFriends);

            if(removeBlackUsers.Count>0)
                FriendDao.Instance.DeleteFromBlackList(user.UserID, removeBlackUsers);

            if (createBlackUsers.Count > 0)
                FriendDao.Instance.AddUsersToBlacklist(user.UserID, createBlackUsers);
          
            if (friendChanged)
                UserBO.Instance.RemoveUserCache(user.UserID);

            return true;
        }

        public bool Client_AcceptFriend(int operatorUserID, int firendUserID, int groupID, int hisGroupID)
        {
            FriendDao.Instance.AcceptAddFriend(operatorUserID, firendUserID, hisGroupID, groupID);
            UpdateFriendCache(operatorUserID, firendUserID);

            AuthUser operatorUser = UserBO.Instance.GetAuthUser(operatorUserID);

            if (operatorUser != null && operatorUser != User.Guest)
            {
                User friendUser = UserBO.Instance.GetUser(firendUserID);
                if(friendUser!=null)
                    FeedBO.Instance.CreateAddFriendFeed(firendUserID, operatorUser.UserID);
            }

            return true;
        }

        public bool Client_DeleteFriends(int operatorID, List<int> friendIDs)
        {

            FriendDao.Instance.DeleteFriends(operatorID, friendIDs);

            UpdateFriendCache(operatorID, friendIDs);

            return true;
        }

        public bool Client_MoveFriends(int operatorID, List<int> friendIDs, int groupID)
        {
            FriendDao.Instance.MoveFriends(operatorID, friendIDs, groupID);

            UpdateFriendCache(operatorID);

            return true;
        }

        public bool Client_UpdateFriendHot(int operatorID, int type, int friendUserID)
        {
            FriendDao.Instance.UpdateFriendHot((HotType)type, operatorID, friendUserID);
            UpdateFriendCache(operatorID);
            return true;
        }

        public bool Client_AddUsersToBlacklist(int operatorID, IEnumerable<int> userIdsToAdd)
        {
            if (FriendDao.Instance.AddUsersToBlacklist(operatorID, userIdsToAdd))
            {
                UpdateFriendCache(operatorID, userIdsToAdd);
                return true;
            }
            return false;
        }

        public bool Client_DeleteFromBlacklist(int operatorID, int userIDInBlacklist)
        {
            FriendDao.Instance.DeleteFromBlackList(operatorID, userIDInBlacklist);
            UpdateFriendCache(operatorID);
            return true;
        }

        public int Client_AddFriendGroup(int operatorID, int groupID, string groupName)
        {
            FriendGroup newGroup;
            return FriendDao.Instance.AddFriendGroup(operatorID, groupID, groupName, int.MaxValue, out newGroup);
        }

        public bool Client_DeleteFriendGroup(int operatorID, int groupID, bool deleteFriends)
        {
            return FriendDao.Instance.DeleteFriendGroup(operatorID, groupID, deleteFriends);
        }

        public bool Client_RenameFriendGroup(int operatorID, int groupID, string groupName)
        {
            int result = FriendDao.Instance.RenameFriendGroup(operatorID, groupID, groupName);
            return result == 0;
        }

        public bool Client_ShieldFriendGroups(int operatorID, IEnumerable<int> groupIDs, bool isShield)
        {
            FriendDao.Instance.ShieldFriendGroups(operatorID, groupIDs, isShield);

            return true;
        }
#endif


        #endregion

        #region Passport Server Only

        public bool Server_AddFriendGroup(int operatorID, string groupName, out FriendGroup friendGroup, out int errorCode)
        {
            friendGroup = null;
            errorCode = 0;

            bool success = CheckFriendGroupInput(operatorID, groupName);
            if (success == false)
                return false;

            int maxGroupCount = AllSettings.Current.FriendSettings.MaxFriendGroupCount;
            errorCode = FriendDao.Instance.AddFriendGroup(operatorID, null, groupName, maxGroupCount, out friendGroup);

            if (ProcessFriendGroupError(errorCode, maxGroupCount, groupName))
            {
                if (OnFriendGroupCreated != null)
                    OnFriendGroupCreated(friendGroup.UserID, friendGroup);

                return true;
            }

            return false;

        }
        
        #endregion

        const int GroupNameMaxLength = 10;

        const string cacheKey_FriendGroups = "FriendGroups/{0}";

        public FriendBO()
        {

        }

        #region 好友分组

        #region 获取

        /// <summary>
        /// 返回好友分组
        /// </summary>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        public FriendGroupCollection GetFriendGroups(int operatorID)
        {

            if (operatorID <= 0)
            {
                //ThrowError(new NotLoginError());
                return new FriendGroupCollection();
            }

            string cacheKey = string.Format(cacheKey_FriendGroups, operatorID);

            FriendGroupCollection groups;

            if (PageCacheUtil.TryGetValue(cacheKey, out groups) == false)
            {

                User user = UserBO.Instance.GetUser(operatorID);

                if (user == null)
                {
                    ThrowError(new UserNotExistsError("operatorID", operatorID));
                    return new FriendGroupCollection();
                }

                groups = FriendDao.Instance.GetFriendGroups(operatorID);

                int defaultFriends = user.TotalFriends;

                foreach(FriendGroup group in groups)
                {
                    defaultFriends -= group.TotalFriends;
                }

                groups.Add(FriendGroup.GetDefaultGroup(operatorID, defaultFriends));
                
                PageCacheUtil.Set(cacheKey, groups);
            }

            return groups;
        }


        /// <summary>
        /// 根据用户ID和分组ID取好友分组
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendGroupID"></param>
        /// <returns></returns>
        public FriendGroup GetFriendGroup(int operatorID, int groupID)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return null;
            }

            //要操作的组ID错误
            if (groupID < 0)
            {
                ThrowError(new InvalidParamError("groupID"));
                return null;
            }

            FriendGroup friendGroup = GetFriendGroups(operatorID).GetValue(groupID);

            return friendGroup;
        }

        #endregion

        #region 创建


        /// <summary>
        /// 创建一个好友分组并返回这个分组的ID（如果返回0表示创建失败）
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public int AddFriendGroup(int operatorID, string groupName)
        {

            bool success = CheckFriendGroupInput(operatorID, groupName);

            if (success == false)
                return 0;


            int result = 0;
            int newGroupID;
            int maxGroupCount = AllSettings.Current.FriendSettings.MaxFriendGroupCount;

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;
            if (settings.EnablePassport)
            {
                FriendGroupProxy friendGroup;
                APIResult apiResult = null;

                try
                {
                    apiResult = settings.PassportService.Friend_CreateFriendGroup(operatorID, groupName, out friendGroup);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return 0;
                }

                result = apiResult.ErrorCode;
                if (apiResult.IsSuccess)
                    newGroupID = friendGroup.GroupID;
                else
                    newGroupID = 0;

                if (result == Consts.ExceptionCode)
                {
                    if (apiResult.Messages.Length > 0)
                    {
                        throw new Exception(apiResult.Messages[0]);
                    }
                }
            }
            else
#endif
            {
                FriendGroup newGroup = null;
                result = FriendDao.Instance.AddFriendGroup(operatorID, null, groupName, maxGroupCount, out newGroup);
                if (newGroup != null)
                {
                    newGroupID = newGroup.GroupID;
                    if (OnFriendGroupCreated != null)
                    {
                        OnFriendGroupCreated(operatorID, newGroup);
                    }
                }
                else
                    newGroupID = 0;
            }


            if (ProcessFriendGroupError(result, maxGroupCount, groupName))
            {
                return newGroupID;
            }
            else
                return result;
        }


        private bool CheckFriendGroupInput(int operatorID, string groupName)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (string.IsNullOrEmpty(groupName))
            {
                ThrowError(new EmptyFriendGroupNameError("groupName"));
                return false;
            }

            if (StringUtil.GetByteCount(groupName) > GroupNameMaxLength)
            {
                ThrowError(new FriendGroupLengthError("groupName", groupName, GroupNameMaxLength));
                return false;
            }

            return true;
        }
        private bool ProcessFriendGroupError(int errorCode, int maxGroupCount, string groupName)
        {
            switch (errorCode)
            {
                case 2:
                    ThrowError(new DuplicateFriendGroupNameError("groupName", groupName));
                    return false;

                case 3:
                    ThrowError(new FriendGroupNumberError(maxGroupCount));
                    return false;

                case 0:
                    return true;

                default:
                    ThrowError(new UnknownError());
                    return false;
            }

        }

        #endregion

        #region 删除

        /// <summary>
        /// 删除一个好友分组
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool DeleteFriendGroup(int operatorID, int groupID, bool deleteFriends)
        {
            if (CheckDeleteFriednGroupData(operatorID, groupID) == false)
            {
                return false;
            }
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {

                APIResult result = null;

                try
                {
                    result = settings.PassportService.Friend_DeleteFriendGroup(operatorID, groupID, deleteFriends);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                    return false;
                }
                if (result.IsSuccess == false)
                {
                    ThrowError<CustomError>(new CustomError(result.Messages[0]));
                    return false;
                }

                return true;
            }
            else
#endif
            {
                bool result = FriendDao.Instance.DeleteFriendGroup(operatorID, groupID, deleteFriends);
                if (result)
                {
                    if (OnFriendGroupDeleted != null)
                    {
                        OnFriendGroupDeleted(operatorID, groupID, deleteFriends);
                    }
                }

                return result;
            }
        }

        public bool Server_DeleteFriendGroup(int operatorID, int groupID, bool deleteFriends)
        {
            if (CheckDeleteFriednGroupData(operatorID, groupID) == false)
            {
                return false;
            }

            bool result = FriendDao.Instance.DeleteFriendGroup(operatorID, groupID, deleteFriends);
            if (result && OnFriendGroupDeleted != null)
                OnFriendGroupDeleted(operatorID, groupID, deleteFriends);
            return result;
        }

        private bool CheckDeleteFriednGroupData(int operatorID, int groupID)
        {
            //未登陆错误
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            //要操作的组ID错误
            if (groupID <= 0)
            {
                ThrowError(new InvalidParamError("groupID"));
                return false;
            }

            return true;
        }

        #endregion

        #region 重命名

        /// <summary>
        /// 重命名一个好友分组
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupID"></param>
        /// <param name="newGroupName"></param>
        /// <returns></returns>
        public bool RenameFriendGroup(int operatorID, int groupID, string groupName)
        {
            int errorCode = 0;
            if (CheckRenameFriendGroupData(operatorID, groupID, groupName) == false)
                return false;
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            
            if (settings.EnablePassport)
            {
                APIResult result = null;
                try
                {
                    result = settings.PassportService.Friend_RenameFriendGroup(operatorID, groupID, groupName);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError( ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                    return false;
                }

                errorCode = result.ErrorCode;
            }
            else
#endif
            {
                errorCode = FriendDao.Instance.RenameFriendGroup(operatorID, groupID, groupName);
                if (errorCode == 0 && OnFriendGroupUpdated != null)
                {

                    FriendGroup newGroup = GetFriendGroup(operatorID, groupID);
                    OnFriendGroupUpdated(operatorID, newGroup);
                }
            }

            return ProcessRenameFriendGroupDataError(errorCode, groupName);
        }

        public bool Server_RenameFriendGroup(int operatorID, int groupID, string groupName, out int errorCode)
        {
            errorCode = 0;

            if (CheckRenameFriendGroupData(operatorID, groupID, groupName) == false)
                return false;

            errorCode = FriendDao.Instance.RenameFriendGroup(operatorID, groupID, groupName);

            if (ProcessRenameFriendGroupDataError(errorCode, groupName))
            {
                if (OnFriendGroupUpdated != null)
                    OnFriendGroupUpdated(operatorID, GetFriendGroup(operatorID, groupID));
                return true;
            }
            return false;
        }

        private bool CheckRenameFriendGroupData(int operatorID, int groupID, string groupName)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            //要操作的组ID错误
            if (groupID <= 0)
            {
                ThrowError(new InvalidParamError("groupID"));
                return false;
            }

            if (string.IsNullOrEmpty(groupName))
            {
                ThrowError(new EmptyFriendGroupNameError("groupName"));
                return false;
            }

            if (StringUtil.GetByteCount(groupName) > GroupNameMaxLength)
            {
                ThrowError(new FriendGroupLengthError("groupName", groupName, GroupNameMaxLength));
                return false;
            }

            return true;
        }

        private bool ProcessRenameFriendGroupDataError(int errorCode, string groupName)
        {
            switch (errorCode)
            {
                case 2:
                    ThrowError(new InvalidParamError("groupID"));
                    return false;

                case 3:
                    ThrowError(new DuplicateFriendGroupNameError("groupName", groupName));
                    return false;

                case 0:
                    return true;
                default:
                    ThrowError(new UnknownError());
                    return false;
            }
        }
        #endregion

        #region 屏蔽

        /// <summary>
        /// 设置一个好友分组是显示还是隐藏
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupID"></param>
        /// <param name="isShield"></param>
        /// <returns></returns>
        public bool ShieldFriendGroup(int operatorID, int groupID, bool isShield)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            //要操作的组ID错误
            if (groupID <= 0)
            {
                ThrowError(new InvalidParamError("groupID"));
                return false;
            }

            List<int> ids = new List<int>();
            ids.Add(groupID);

            ShieldFriendGroups(operatorID, ids, isShield);


            //FriendDao.Instance.ShieldFriendGroups(operatorID, ids, isShield);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupIds">例如：isShield为true时 groupIds表示要被屏蔽的组ID  其他组ID则不屏蔽</param>
        /// <param name="isShield"></param>
        public void ShieldFriendGroups(int operatorID, IEnumerable<int> groupIds, bool isShield)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return;
            }
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {

                List<int> temp = new List<int>();
                foreach (int id in groupIds)
                    temp.Add(id);

                int[] t = new int[temp.Count];
                temp.CopyTo(t, 0);


                APIResult result = null;

                try
                {
                    result = settings.PassportService.Friend_ShieldFriendGroup(operatorID, t, isShield);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
            }
            else
#endif
            {
                FriendDao.Instance.ShieldFriendGroups(operatorID, groupIds, isShield);

                if (OnFriendGroupShielded != null)
                    OnFriendGroupShielded(operatorID, groupIds, isShield);
            }

        }

        public void Server_ShieldFriendGroups(int operatorID, IEnumerable<int> groupIds, bool isShield)
        {
            if (ValidateUtil.HasItems<int>(groupIds) == false)
                return;

            FriendDao.Instance.ShieldFriendGroups(operatorID, groupIds, isShield);

            if (OnFriendGroupShielded != null)
                OnFriendGroupShielded(operatorID, groupIds, isShield);
        }

        #endregion

        #region 判断是否屏蔽

        /// <summary>
        /// 判断当前好友分组是否被屏蔽
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendGroupID"></param>
        /// <returns></returns>
        public bool IsShieldFriendGroup(int operatorID, int friendGroupID)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (friendGroupID < 0)
            {
                ThrowError(new InvalidParamError("friendGroupID"));
                return false;
            }

            FriendGroupCollection friendGroups = GetFriendGroups(operatorID);

            if (friendGroups.GetValue(friendGroupID) == null)
                return false;

            return friendGroups.GetValue(friendGroupID).IsShield;
        }

        #endregion

        #endregion

        #region 好友

        #region 获取
        /// <summary>
        /// 根据用户ID返回该用户的所有好友
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public FriendCollection GetFriends(int userID)
        {
            if (userID <= 0)
            {
                ThrowError(new InvalidParamError("operatorID"));
                return new FriendCollection();
            }

            User user = UserBO.Instance.GetUser(userID);
            if (user != null)
                return user.Friends;
            return new FriendCollection();
        }

        public FriendCollection GetFriendAndBlackList(int userID)
        {
            return FriendDao.Instance.GetFriendsAndBlacklist(userID);
        }


        public FriendCollection GetFriends(int operatorID, IEnumerable<int> friendUserIds)
        {
            FriendCollection result = new FriendCollection();

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return result;
            }

            if (ValidateUtil.HasItems(friendUserIds) == false)
            {
                ThrowError(new NotSelectedFriendsError("friendUserIds"));
                return result;
            }

            User user = UserBO.Instance.GetUser(operatorID);

            if (user == null)
                return result;

            foreach (int friendUserID in friendUserIds)
            {
                Friend friend;
                if (user.Friends.TryGetValue(friendUserID, out friend))
                    result.Add(friend);
            }

            return result;
        }

        /// <summary>
        /// 根据用户ID返回该用户的所有好友ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<int> GetFriendUserIDs(int userID)
        {

            List<int> friendUserIDs = new List<int>();

            if (userID <= 0)
            {
                ThrowError(new InvalidParamError("userID"));
                return friendUserIDs;
            }

            FriendCollection friends = GetFriends(userID);

            foreach (Friend friend in friends)
            {
                friendUserIDs.Add(friend.UserID);
            }

            return friendUserIDs;
        }

        public Friend GetFriend(int operatorID, int friendUserID)
        {

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return null;
            }

            if (friendUserID <= 0)
            {
                ThrowError(new InvalidParamError("friendUserID"));
                return null;
            }

            return GetFriends(operatorID).GetValue(friendUserID);
        }

        public FriendCollection GetFriends(int operatorID, int pageNumber, int pageSize)
        {

            FriendCollection result = new FriendCollection();

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return result;
            }

            FriendCollection friends = GetFriends(operatorID);

            int totalCount = friends.Count;

            result.TotalRecords = totalCount;

            if (pageNumber < 1)
                pageNumber = 1;

            int startIndex = (pageNumber - 1) * pageSize;

            if (startIndex + pageSize > totalCount)
            {
                pageSize = (totalCount % pageSize);
                startIndex = totalCount - pageSize;
            }

            for (int i = startIndex; i < (startIndex + pageSize); i++)
            {
                result.Add(friends[i]);
            }

            return result;

        }

        /// <summary>
        /// 获取指定好友分组中的所有好友
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupID">好友分组的ID。如果指定0表示取“未分类”的好友</param>
        /// <returns></returns>
        public FriendCollection GetFriends(int operatorID, int groupID)
        {
            return GetFriends(operatorID, groupID, 1, int.MaxValue);
        }

        /// <summary>
        /// 分页获取指定好友分组的好友
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="groupID">好友分组的ID。如果指定0表示取“未分类”的好友</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public FriendCollection GetFriends(int operatorID, int groupID, int pageNumber, int pageSize)
        {
            FriendCollection result = new FriendCollection();

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return result;
            }

            //要操作的组ID错误
            if (groupID < 0)
            {
                ThrowError(new InvalidParamError("groupID"));
                return result;
            }


            User operatorUser = User.Current;

            if (operatorUser == null)
            {
                ThrowError(new UserNotExistsError("operatorID", operatorID));
                return result;
            }

            if (operatorUser.TotalFriends == 0)
            {
                result.TotalRecords = 0;
                return result;
            }


            //FriendCollection friends = GetFriends(operatorID);


            FriendGroupCollection friendGroups = GetFriendGroups(operatorID);

            FriendGroup friendGroup;
            if (friendGroups.TryGetValue(groupID, out friendGroup) == false)
            {
                return result;
            }

            if (pageNumber < 1)
                pageNumber = 1;

            int startIndex = (pageNumber - 1) * pageSize;

            if (startIndex + pageSize > friendGroup.TotalFriends)
            {
                pageSize = (friendGroup.TotalFriends % pageSize);
                startIndex = friendGroup.TotalFriends - pageSize;
            }

            int i = 0;
            if (friendGroup.TotalFriends > 0)
            {

                foreach (Friend friend in GetFriends(operatorID))
                {
                    //如果是取“未分类”组，要考虑到一些GroupID不为0，但其GroupID并不存在的情况
                    if (friend.GroupID == groupID
                        || (groupID == 0 && friendGroups.ContainsKey(friend.GroupID) == false))
                    {

                        if (i >= startIndex)
                            result.Add(friend);


                        if (result.Count >= pageSize)
                            break;

                        i++;
                    }
                }

                result.TotalRecords = friendGroup.TotalFriends;

            }


            return result;

        }

        #endregion

        #region 添加

        /// <summary>
        /// 申请添加好友
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        /// <param name="friendGroupID"></param>
        /// <param name="IP"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool TryAddFriend(AuthUser operatorUser, int userIDToAdd, int groupIDToAdd, string message)
        {
            if (CheckAddFriendInput(operatorUser.UserID, userIDToAdd, groupIDToAdd, message) == false)
                return false;

            if (MyInBlacklist(operatorUser.UserID, userIDToAdd))//你在他的黑名单中
            {
                //ThrowError(new InBlacklistError("userIDToAdd", userIDToAdd));
                return true;
            }

            if (InMyBlacklist(operatorUser.UserID, userIDToAdd))//他在你的黑名单中
            {
                ThrowError(new InMyBlacklistError("userIDToAdd", userIDToAdd));
                return true;
            }
            //if (NotifyBO.Instance.CheckIfSentFriendRequest(operatorID, userIDToAdd))//正在验证
            //{
            //    ThrowError(new InVerifyError());
            //    return false;
            //}
           
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {

                APIResult result = null;
                try
                {
                    result = settings.PassportService.Friend_AddFriend(operatorUser.UserID, userIDToAdd, groupIDToAdd, message);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false; 
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }

                if (result.IsSuccess == false)
                {
                    if (result.Messages.Length > 0)
                        ThrowError<CustomError>(new CustomError(result.Messages[0]));
                    return false;
                }
            }
            else
#endif
            {

                FriendNotify notify = new FriendNotify(operatorUser.UserID, groupIDToAdd, message);
                notify.UserID = userIDToAdd;
                NotifyBO.Instance.AddNotify(operatorUser, notify);
            }
            return true;
        }

        public bool Server_TryAddFriend( int userID, int userIDToAdd, int groupIDToAdd, string message)
        {
            if (CheckAddFriendInput(userID, userIDToAdd, groupIDToAdd, message) == false)
                return false;

            if (MyInBlacklist(userID, userIDToAdd))//你在他的黑名单中
            {
                //ThrowError(new InBlacklistError("userIDToAdd", userIDToAdd));
                return true;
            }

            if (InMyBlacklist(userID, userIDToAdd))//他在你的黑名单中
            {
                ThrowError(new InMyBlacklistError("userIDToAdd", userIDToAdd));
                return true;
            }

            FriendNotify notify = new FriendNotify(userID, groupIDToAdd, message);
            notify.UserID = userIDToAdd;
            NotifyBO.Instance.AddNotify(UserBO.Instance.GetAuthUser( userID), notify);

            //这里不需要告诉客户端 加好友
            //只需要告诉客户端加条加好友的FriendNotify 即可  而这个事件 在NotifyBO里做

            return true;
        }



        private bool CheckAddFriendInput(int operatorID, int userIDToAdd, int groupIDToAdd, string message)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (userIDToAdd <= 0)
            {
                ThrowError(new InvalidParamError("userIDToAdd"));
                return false;
            }

            if (groupIDToAdd < 0)
            {
                ThrowError(new InvalidParamError("groupIDToAdd"));
                return false;
            }

            //TODO:权限判断 是否有权限加好友
            if (IsFriend(operatorID, userIDToAdd))//已经是好友
            {
                ThrowError(new AlreadyFriendError());
                return false;
            }

            return true;
        }

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="friendUserID">好友ID</param>
        /// <param name="fromFriendGroupID">发起申请方的分组ID</param>
        /// <param name="toFriendGroupID">接受方的分组ID</param>
        public bool AcceptAddFriend(AuthUser operatorUser, int notifyID, int groupIDToAdd)
        {
            int tryAddUserID, targetGroupID;
            AuthUser user;
            if (CheckAcceptAddFriendInput(operatorUser.UserID, notifyID, groupIDToAdd, out tryAddUserID, out targetGroupID, out user) == false)
                return false;

            //if (!NotifyBO.Instance.CheckIfReceiveFriendRequest(tryAddUserID, operatorID))
            //    return false;

            if (user.TotalFriends < AllSettings.Current.FriendSettings.MaxFriendCount)
            {

#if !Passport
                PassportClientConfig settings = Globals.PassportClient;

                if (settings.EnablePassport)
                {
                    APIResult result = null;
                    try
                    {
                        result = settings.PassportService.Friend_AcceptAddFriend(operatorUser.UserID, notifyID, groupIDToAdd);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                        return false;
                    }

                    if (result.ErrorCode == Consts.ExceptionCode)
                    {
                        if (result.Messages.Length > 0)
                            throw new Exception(result.Messages[0]);

                    }

                    if (result.IsSuccess == false)
                    {
                        if (result.Messages.Length > 0)
                            ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                        return false;
                    }

                    Thread.Sleep(300);
                }
                else
#endif

                {

                    FriendDao.Instance.AcceptAddFriend(operatorUser.UserID, tryAddUserID, targetGroupID, groupIDToAdd);
                    NotifyBO.Instance.DeleteNotify(operatorUser, notifyID);
#if !Passport 
                    FeedBO.Instance.CreateAddFriendFeed(tryAddUserID, operatorUser.UserID);
#endif
                    UpdateFriendCache(operatorUser.UserID, tryAddUserID);

                    if (OnAcceptFriend != null)
                        OnAcceptFriend(operatorUser.UserID, tryAddUserID, groupIDToAdd, targetGroupID);
                }
            }
            else
            {
                ThrowError(new FriendNumberError(AllSettings.Current.FriendSettings.MaxFriendCount));
                return false;
            }

            return true;
        }

        public bool Server_AcceptAddFriend(int operatorID, int notifyID, int groupIDToAdd)
        {
            int tryAddUserID, targetGroupID;
            AuthUser user;
            if (CheckAcceptAddFriendInput(operatorID, notifyID, groupIDToAdd, out tryAddUserID, out targetGroupID, out user) == false)
                return false;


            if (user.TotalFriends < AllSettings.Current.FriendSettings.MaxFriendCount)
            {
                FriendDao.Instance.AcceptAddFriend(operatorID, tryAddUserID, targetGroupID, groupIDToAdd);
                //此时 不能删除该条通知  因为 OnAcceptFriend 事件中 还要调用此通知
                //要在客户端来删除此通知
                NotifyBO.Instance.DeleteNotify(user, notifyID);

                UpdateFriendCache(operatorID, tryAddUserID);

                if (OnAcceptFriend != null)
                    OnAcceptFriend(operatorID, notifyID, groupIDToAdd,targetGroupID);
            }
            else
            {
                ThrowError(new FriendNumberError(AllSettings.Current.FriendSettings.MaxFriendCount));
                return false;
            }

            return true;
        }

        private bool CheckAcceptAddFriendInput(int operatorID, int notifyID, int groupIDToAdd,out int tryAddUserID, out int targetGroupID, out AuthUser user)
        {
            tryAddUserID = 0;
            targetGroupID = 0;
            user = null;

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (groupIDToAdd < 0)
            {
                ThrowError(new InvalidParamError("groupIDToAdd"));
                return false;
            }

            FriendNotify notify;
            using (ErrorScope es = new ErrorScope())
            {
                Notify temp = NotifyBO.Instance.GetNotify(operatorID, notifyID);
                if (temp == null)
                    return false;
                notify = new FriendNotify(temp);

                if (es.HasUnCatchedError)
                {
                    return false;
                }
            }

            targetGroupID = notify.TargetGroupID;
            //获得发起好友申请的UserID
            tryAddUserID = notify.RelateUserID;

            if (tryAddUserID <= 0)
            {
                return false;
            }
            if (notify.TargetGroupID < 0)
            {
                return false;
            }


            user = UserBO.Instance.GetAuthUser(operatorID);

            if (user == null)
            {
                ThrowError(new UserNotExistsError("operatorID", operatorID));
                return false;
            }

            if (MyInBlacklist(operatorID, tryAddUserID))
            {
                ThrowError(new InBlacklistError("tryAddUserID", tryAddUserID));
                return false;
            }
            if (InMyBlacklist(operatorID, tryAddUserID))
            {
                ThrowError(new InMyBlacklistError("tryAddUserID", tryAddUserID));
                return false;
            }

            return true;
        }
        #endregion

        #region 删除

        /// <summary>
        /// 解除好友关系
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        public bool DeleteFriend(int operatorID, int friendUserID)
        {
            return DeleteFriends(operatorID, new int[] { friendUserID });
        }

        /// <summary>
        /// 解除好友关系
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendID"></param>
        public bool DeleteFriends(int operatorID, IEnumerable<int> friendUserIds)
        {
            if (CheckDeleteFriendsInput(operatorID, friendUserIds) == false)
                return false;

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {
                List<int> temp = new List<int>();
                foreach (int id in friendUserIds)
                    temp.Add(id);

                int[] t = new int[temp.Count];
                temp.CopyTo(t, 0);

                APIResult result = null;
                try
                {
                   result = settings.PassportService.Friend_DeleteFriends(operatorID, t);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
                if (result.IsSuccess == false)
                {
                    if (result.Messages.Length > 0)
                        ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return false;
                }

                Thread.Sleep(300);
            }
            else
#endif
            {
                FriendDao.Instance.DeleteFriends(operatorID, friendUserIds);

                UpdateFriendCache(operatorID, friendUserIds);

                if (OnRemoveFriend != null)
                    OnRemoveFriend(operatorID, friendUserIds);
            }

            return true;
        }

        public bool Server_DeleteFriends(int operatorID, IEnumerable<int> friendUserIds)
        {
            if (CheckDeleteFriendsInput(operatorID, friendUserIds) == false)
                return false;

            FriendDao.Instance.DeleteFriends(operatorID, friendUserIds);

            UpdateFriendCache(operatorID, friendUserIds);

            if (OnRemoveFriend != null)
                OnRemoveFriend(operatorID, friendUserIds);

            return true;
        }

        private bool CheckDeleteFriendsInput(int operatorID, IEnumerable<int> friendUserIds)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (ValidateUtil.HasItems(friendUserIds) == false)
            {
                ThrowError(new NotSelectedFriendsError("friendUserIds"));
                return false;
            }

            return true;
        }

        #endregion

        #region 移动

        /// <summary>
        /// 批量移动好友
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserIDs">2,5,6</param>
        /// <param name="friendGroupID"></param>
        /// <returns></returns>
        public bool MoveFriends(int operatorID, IEnumerable<int> friendUserIds, int friendGroupID)
        {
            if (CheckMoveFriendsInput(operatorID, friendUserIds, friendGroupID) == false)
                return false;
            
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {
                List<int> temp = new List<int>();
                foreach (int id in friendUserIds)
                    temp.Add(id);

                int[] t = new int[temp.Count];
                temp.CopyTo(t, 0);

                APIResult result = null;

                try
                {
                    result = settings.PassportService.Friend_MoveFriends(operatorID, t, friendGroupID);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
                if (result.IsSuccess == false)
                {
                    if (result.Messages.Length > 0)
                        ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return false;
                }
            }
            else
#endif
            {
                FriendDao.Instance.MoveFriends(operatorID, friendUserIds, friendGroupID);

                UpdateFriendCache(operatorID);

                if (OnMoveFriend != null)
                    OnMoveFriend(operatorID, friendUserIds, friendGroupID);
            }
            return true;
        }

        public bool Server_MoveFriends(int operatorID, IEnumerable<int> friendUserIds, int friendGroupID)
        {
            if (CheckMoveFriendsInput(operatorID, friendUserIds, friendGroupID) == false)
                return false;

            FriendDao.Instance.MoveFriends(operatorID, friendUserIds, friendGroupID);

            UpdateFriendCache(operatorID);

            if (OnMoveFriend != null)
                OnMoveFriend(operatorID, friendUserIds, friendGroupID);

            return true;

        }

        private bool CheckMoveFriendsInput(int operatorID, IEnumerable<int> friendUserIds, int friendGroupID)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (ValidateUtil.HasItems(friendUserIds) == false)
            {
                ThrowError(new NotSelectedFriendsError("friendUserIds"));
                return false;
            }

            if (friendGroupID < 0)
            {
                ThrowError(new InvalidParamError("friendGroupID"));
                return false;
            }

            return true;
        }

        #endregion

        #region 判断是否好友

        /// <summary>
        /// 判断两个人是否是好友
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendUserID"></param>
        /// <returns></returns>
        public bool IsFriend(int userID, int targetUserID)
        {
			if (userID == 0 || targetUserID == 0)
				return false;

            if (userID < 0)
            {
                ThrowError(new InvalidParamError("userID"));
                return false;
            }

            if (targetUserID < 0)
            {
                ThrowError(new InvalidParamError("targetUserID"));
                return false;
            }

			//先看看第一个用户是否有在缓存中
			User user = UserBO.Instance.GetUserFromCache(userID);

			//从缓存中没有得到第一个用户就接着尝试获取第二个用户
			if (user == null)
				user = UserBO.Instance.GetUserFromCache(targetUserID);

			//两个用户都不在缓存中就只要查询数据库了
			if (user == null)
				user = UserBO.Instance.GetUser(userID);

            return user.Friends.GetValue(targetUserID) != null;
        }

        #endregion

        #region 更新热度

        /// <summary>
        /// 更新好友间的热度
        /// </summary>
        /// <param name="type">类型:评论 留言 访问主页</param>
        /// <param name="userID">当前用户ID</param>
        /// <param name="friendUserID">当前用户的好友的ID</param>
        public bool UpdateFriendHot(int operatorID, HotType type, int friendUserID)
        {
            if (CheckUpdateFriendHotInput(operatorID, type, friendUserID) == false)
                return false;
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {
                APIResult result = null;

                try
                {
                    result = settings.PassportService.Friend_UpdateFriendHot(operatorID, (int)type, friendUserID);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
                if (result.IsSuccess == false)
                {
                    if (result.Messages.Length > 0)
                        ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return false;
                }
            }
            else
#endif
            {
                FriendDao.Instance.UpdateFriendHot(type, operatorID, friendUserID);

                UpdateFriendCache(operatorID);

                if (OnUpdateFriendHot != null)
                    OnUpdateFriendHot(operatorID, type, friendUserID);

            }
            return true;
        }

        public bool Server_UpdateFriendHot(int operatorID, HotType type, int friendUserID)
        {
            if (CheckUpdateFriendHotInput(operatorID, type, friendUserID) == false)
                return false;

            FriendDao.Instance.UpdateFriendHot(type, operatorID, friendUserID);

            UpdateFriendCache(operatorID);

            if (OnUpdateFriendHot != null)
                OnUpdateFriendHot(operatorID, type, friendUserID);

            return true;
        }

        private bool CheckUpdateFriendHotInput(int operatorID, HotType type, int friendUserID)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (friendUserID <= 0)
            {
                ThrowError(new InvalidParamError("friendUserID"));
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region 黑名单

        public Blacklist GetBlacklist(int userID)
        {
            return GetFriends(userID).Blacklist;
        }

        public Blacklist GetBlacklist(int operatorID, int pageNumber, int pageSize)
        {
            Blacklist result = new Blacklist();

            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return result;
            }

            Blacklist blacklist = GetBlacklist(operatorID);

            int totalCount = blacklist.Count;

            result.TotalRecords = totalCount;

            if (pageNumber < 1)
                pageNumber = 1;

            int startIndex = (pageNumber - 1) * pageSize;

            if (startIndex + pageSize > totalCount)
            {
                pageSize = (totalCount % pageSize);
                startIndex = totalCount - pageSize;
            }

            for (int i = startIndex; i < (startIndex + pageSize); i++)
            {
                result.Add(blacklist[i]);
            }

            return result;
        }

        /// <summary>
        /// 把一组用户加入黑名单
        /// </summary>
        /// <param name="operatorID"></param>
        /// <param name="userIdsToAdd"></param>
        /// <returns></returns>
        public bool AddUsersToBlacklist(int operatorID, IEnumerable<int> userIdsToAdd)
        {
            if (CheckAddUsersToBlacklistInput(operatorID, userIdsToAdd) == false)
                return false;
            
#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {
                List<int> temp = new List<int>();
                foreach (int id in userIdsToAdd)
                    temp.Add(id);

                int[] t = new int[temp.Count];
                temp.CopyTo(t, 0);

                APIResult result = null;

                try
                {
                    result = settings.PassportService.Friend_AddUsersToBlacklist(operatorID, t);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
                if (result.IsSuccess == false)
                {
                    if (result.Messages.Length > 0)
                        ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return false;
                }
            }
            else
#endif
            {
                if (FriendDao.Instance.AddUsersToBlacklist(operatorID, userIdsToAdd) == false)
                {
                    return false;
                }
                else
                {
                    UpdateFriendCache(operatorID, userIdsToAdd);
                    if (OnAddUsersToBlacklist != null)
                    {
                        OnAddUsersToBlacklist(operatorID, userIdsToAdd);
                    }
                }
            }
            return true;
            
        }

        public bool Server_AddUsersToBlacklist(int operatorID, IEnumerable<int> userIdsToAdd)
        {
            if (CheckAddUsersToBlacklistInput(operatorID, userIdsToAdd) == false)
                return false;

            if (FriendDao.Instance.AddUsersToBlacklist(operatorID, userIdsToAdd))
            {
                UpdateFriendCache(operatorID, userIdsToAdd);

                if (OnAddUsersToBlacklist != null)
                    OnAddUsersToBlacklist(operatorID, userIdsToAdd);

                return true;
            }
            return false;

        }
        

        public bool AddUserToBlacklist(int operatorID, int userIDToAdd)
        {
            return AddUsersToBlacklist(operatorID, new int[] { userIDToAdd });
        }

        public bool AddUserToBlacklist(int operatorID, string usernameToAdd)
        {

            if (string.IsNullOrEmpty(usernameToAdd))
            {
                ThrowError(new EmptyUsernameError("usernameToAdd"));
                return false;
            }

            if (usernameToAdd.Length > 50)
            {
                ThrowError(new UsernameLengthError("usernameToAdd", usernameToAdd, 50, 1));
                return false;
            }

            int userIDToAdd = UserBO.Instance.GetUserID(usernameToAdd);

            if (userIDToAdd == 0)
            {
                ThrowError(new UserNotExistsError("usernameToAdd", usernameToAdd));
                return false;
            }

            if (userIDToAdd == operatorID)
            {
                ThrowError(new CustomError("", "您不能将自己加入黑名单"));
                return false;
            }

            return AddUserToBlacklist(operatorID, userIDToAdd);
        }



        private bool CheckAddUsersToBlacklistInput(int operatorID, IEnumerable<int> userIdsToAdd)
        {
            if (operatorID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (ValidateUtil.HasItems(userIdsToAdd) == false)
            {
                ThrowError(new NotSelectedUserError("userIdsToAdd"));
                return false;
            }

            foreach (int userID in userIdsToAdd)
            {
                if (userID <= 0)
                {
                    ThrowError(new InvalidParamError("userIdsToAdd"));
                    return false;
                }
            }

            return true;
        }



        public bool DeleteFromBlacklist(int operatorID, int userIDInBlacklist)
        {

#if !Passport
            PassportClientConfig settings = Globals.PassportClient;

            if (settings.EnablePassport)
            {

                APIResult result = null;
                try
                {
                    result = settings.PassportService.Friend_DeleteFromBlacklist(operatorID, userIDInBlacklist);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result.ErrorCode == Consts.ExceptionCode)
                {
                    if (result.Messages.Length > 0)
                        throw new Exception(result.Messages[0]);

                }
                if (result.IsSuccess == false)
                {
                    if (result.Messages.Length > 0)
                        ThrowError<CustomError>(new CustomError("", result.Messages[0]));
                    return false;
                }

                return true;
            }
            else
#endif
            {
                FriendDao.Instance.DeleteFromBlackList(operatorID, userIDInBlacklist);
                UpdateFriendCache(operatorID);

                if (OnDeleteFromBlacklist != null)
                    OnDeleteFromBlacklist(operatorID, userIDInBlacklist);

                return true;
            }
        }


        public bool Server_DeleteFromBlacklist(int operatorID, int userIDInBlacklist)
        {
            FriendDao.Instance.DeleteFromBlackList(operatorID, userIDInBlacklist);
            UpdateFriendCache(operatorID);

            if (OnDeleteFromBlacklist != null)
                OnDeleteFromBlacklist(operatorID, userIDInBlacklist);

            return true;
        }


        /// <summary>
        /// 判断指定的UserID是否在黑名单中
        /// </summary>
        /// <param name="userID">黑名单列表所有者</param>
        /// <param name="targetUserID">被检查者，如果这个用户在黑名单中将返回false</param>
        /// <returns></returns>
        public bool InMyBlacklist(int operatorID, int userIDToCheck)
        {
            if (operatorID <= 0)
                return false;

            if (userIDToCheck <= 0)
                return false;

            if (operatorID == userIDToCheck)
                return false;

            User user = UserBO.Instance.GetUser(operatorID);

            if (user == null)
            {
                return false;
            }

            return user.Blacklist.ContainsKey(userIDToCheck);
        }


        public bool MyInBlacklist(int operatorID, int blacklistOwnerID)
        {
            if (operatorID <= 0)
                return false;

            if (blacklistOwnerID <= 0)
                return false;

            if (operatorID == blacklistOwnerID)
                return false;

            User user = UserBO.Instance.GetUser(blacklistOwnerID);

            if (user == null)
            {
                return false;
            }

            return user.Blacklist.ContainsKey(operatorID);
        }

        #endregion

        #region 缓存操作

        /// <summary>
        /// 更新某个用户的好友列表缓存
        /// </summary>
        /// <param name="userID"></param>
        public bool UpdateFriendCache(int userID)
        {
            if (userID <= 0)
            {
                ThrowError(new InvalidParamError("userID"));
                return false;
            }

            User user = UserBO.Instance.GetUserFromCache(userID);
            
            if (user != null)
            {
                user.Friends = FriendDao.Instance.GetFriendsAndBlacklist(userID);
            }

            return true;
        }


        //TODO : 性能优化，一次性读取
        /// <summary>
        /// 更新一组用户的好友列表缓存
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public bool UpdateFriendCache(int userID, IEnumerable<int> userIds)
        {
            if (ValidateUtil.HasItems(userIds) == false)
            {
                ThrowError(new InvalidParamError("userIds"));
                return false;
            }

            User user = UserBO.Instance.GetUserFromCache(userID);

            if (user != null)
            {
                user.Friends = FriendDao.Instance.GetFriendsAndBlacklist(userID);
            }

            PageCacheUtil.Remove(string.Format(cacheKey_FriendGroups, userID));

            foreach (int userIDItem in userIds)
            {
                user = UserBO.Instance.GetUserFromCache(userIDItem);

                if (user != null)
                {
                    user.Friends = FriendDao.Instance.GetFriendsAndBlacklist(userIDItem);
                }

                PageCacheUtil.Remove(string.Format(cacheKey_FriendGroups, userIDItem));
            }

            return true;
        }

        public bool UpdateFriendCache(int userID, int userID2)
        {
            return UpdateFriendCache(userID, new int[] { userID2 });
        }

        #endregion

    }
}