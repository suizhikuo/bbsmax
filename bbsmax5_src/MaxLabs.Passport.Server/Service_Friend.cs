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
using System.Web.Services;
using MaxLabs.bbsMax;
using MaxLabs.WebEngine;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax.Entities;
using System.Web.Services.Protocols;

namespace MaxLabs.Passport.Server
{
    public partial class Service : ServiceBase
    {
        [WebMethod(Description = @"添加好友
userID:添加者ID  friendUserID:被添加者ID  groupID:好友分组ID  message:附加消息
")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_AddFriend(int userID, int friendUserID, int groupID, string message)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_TryAddFriend(userID, friendUserID, groupID, message);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        [WebMethod(Description = "接受好友")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_AcceptAddFriend(int operatorID, int notifyID, int groupIDToAdd)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_AcceptAddFriend(operatorID, notifyID, groupIDToAdd);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCode = Consts.ExceptionCode;
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        [WebMethod(Description = "创建好友分组")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_CreateFriendGroup(int userID, string groupName, out FriendGroupProxy friendGroup)
        {
            friendGroup = null;
            if (!CheckClient()) return null;
            APIResult result= new APIResult();
            FriendGroup newGroup = null;
            int errorCode;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_AddFriendGroup(userID, groupName, out newGroup, out errorCode);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                    result.ErrorCode = errorCode;
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
                friendGroup = ProxyConverter.GetFriendGroupProxy(newGroup);
            }
            return result;
        }

        [WebMethod(Description = "删除好友分组")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_DeleteFriendGroup(int userID, int groupID, bool deleteFriends)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_DeleteFriendGroup(userID, groupID, deleteFriends);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                    result.ErrorCode = 0;
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }

        [WebMethod(Description = "重命名好友分组")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_RenameFriendGroup(int userID, int groupID, string groupName)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    int errorCode;
                    result.IsSuccess = FriendBO.Instance.Server_RenameFriendGroup(userID, groupID, groupName, out errorCode);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                    result.ErrorCode = errorCode;
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }

        [WebMethod(Description = "屏蔽好友分组")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_ShieldFriendGroup(int userID, List<int> groupIds, bool isShield)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    FriendBO.Instance.Server_ShieldFriendGroups(userID, groupIds, isShield);
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }

        [WebMethod(Description = "删除好友")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_DeleteFriends(int userID, List<int> friendUserIds)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_DeleteFriends(userID, friendUserIds);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }

        [WebMethod(Description = "移动好友")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_MoveFriends(int userID, List<int> friendUserIds, int friendGroupID)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_MoveFriends(userID, friendUserIds, friendGroupID);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }


        [WebMethod(Description = "更新好友热度")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_UpdateFriendHot(int userID, int hotType, int friendUserID)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_UpdateFriendHot(userID, (MaxLabs.bbsMax.Enums.HotType)hotType, friendUserID);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }

        [WebMethod(Description = "添加黑名单")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_AddUsersToBlacklist(int userID, List<int> userIdsToAdd)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_AddUsersToBlacklist(userID, userIdsToAdd);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }


        [WebMethod(Description = "移除黑名单")]
        [SoapHeader("clientinfo")]
        public APIResult Friend_DeleteFromBlacklist(int userID, int userIDInBlacklist)
        {
            if (!CheckClient()) return null;
            APIResult result = new APIResult();
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    result.IsSuccess = FriendBO.Instance.Server_DeleteFromBlacklist(userID, userIDInBlacklist);
                    if (result.IsSuccess == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            result.AddError(error.TatgetName,error.Message);
                        });
                    }
                }
                catch (Exception ex)
                {
                    result.AddError(ex.Message);
                    result.IsSuccess = false;
                    result.ErrorCode = Consts.ExceptionCode;
                }
            }
            return result;
        }




        [WebMethod(Description = "获取用户的好友分组")]
        [SoapHeader("clientinfo")]
        public List<FriendGroupProxy> Friend_GetFriendGroups(int userID)
        {
            if (!CheckClient()) return null;
            List<FriendGroupProxy> groups = new List<FriendGroupProxy>();
            FriendGroupCollection temp = FriendBO.Instance.GetFriendGroups(userID);


            foreach (FriendGroup group in temp)
            {
                groups.Add(ProxyConverter.GetFriendGroupProxy(group));
            }

            return groups;
        }

        [WebMethod(Description = "获取用户的好友分组，并包含分组里的好友")]
        [SoapHeader("clientinfo")]
        public List<FriendGroupProxy> Friend_GetFriendGroupsWithFriends(int userID)
        {
            if (!CheckClient()) return null;
            List<FriendGroupProxy> groups = new List<FriendGroupProxy>();
            FriendGroupCollection temp = FriendBO.Instance.GetFriendGroups(userID);
            FriendGroupProxy blackGroup = new FriendGroupProxy();
           
            blackGroup.GroupID = -1;
            blackGroup.Name = "#black list";

            

            FriendCollection friends = FriendBO.Instance.GetFriendAndBlackList(userID);

            foreach (BlacklistItem b in friends.Blacklist)
            {
                FriendProxy fItem = new FriendProxy();
                fItem.GroupID = b.GroupID;
                fItem.UserID = b.UserID;
                blackGroup.Friends.Add(fItem);
            }

            foreach (FriendGroup fg in temp)
            {
                groups.Add(ProxyConverter.GetFriendGroupProxy(fg));
            }

            while (friends.Count > 0)
            {
                Friend friend = friends[friends.Count - 1];
                FriendProxy fp = ProxyConverter.GetFriendProxy(friend);

                foreach (FriendGroupProxy proxy in groups)
                {
                    if (proxy.GroupID == friend.GroupID)
                    {
                        proxy.Friends.Add(fp);
                    }
                }

                friends.Remove(friend);
            }
            groups.Add(blackGroup);

            return groups;
        }

        [WebMethod(Description = "该好友分组是否被屏蔽")]
        [SoapHeader("clientinfo")]
        public bool Friend_IsShieldFriendGroup(int userID, int friendGroupID)
        {
            if (!CheckClient()) return false;
            return FriendBO.Instance.IsShieldFriendGroup(userID, friendGroupID);
        }

        [WebMethod(Description = "获取用户的好友")]
        [SoapHeader("clientinfo")]
        public List<FriendProxy> Friend_GetFriends(int userID)
        {
            if (!CheckClient()) return null;
            List<FriendProxy> friends = new List<FriendProxy>();
            FriendCollection temp = FriendBO.Instance.GetFriends(userID);

            foreach (Friend friend in temp)
            {
                friends.Add(ProxyConverter.GetFriendProxy(friend));
            }

            return friends;
        }

        [WebMethod(Description = "判断两人是否是好友")]
        [SoapHeader("clientinfo")]
        public bool Friend_IsFriend(int userID, int targetUserID)
        {
            if (!CheckClient()) return false;
            return FriendBO.Instance.IsFriend(userID, targetUserID);
        }


        [WebMethod(Description = "获取黑名单")]
        [SoapHeader("clientinfo")]
        public List<int> Friend_GetBlackUserIDs(int userID)
        {
            if (!CheckClient()) return null;
            List<int> userIDs = new List<int>();
            Blacklist list = FriendBO.Instance.GetBlacklist(userID);
            foreach (BlacklistItem item in list)
            {
                userIDs.Add(item.UserID);
            }

            return userIDs;
        }

        [WebMethod(Description = "目标用户是否在我的黑名单里")]
        [SoapHeader("clientinfo")]
        public bool Friend_InMyBlacklist(int myUserID, int targetUserID)
        {
            if (!CheckClient()) return false;
            return FriendBO.Instance.InMyBlacklist(myUserID, targetUserID);
        }
    }
}