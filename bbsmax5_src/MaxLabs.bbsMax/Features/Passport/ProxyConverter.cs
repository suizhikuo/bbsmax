//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司 2002 - 2010
// 公司网站  www.bbsmax.com
//
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 实体类到代理类的转换
    /// </summary>
    public static class ProxyConverter
    {
        public static StringDictionary GetStringDictionary(List<StringKeyValueProxy> list)
        {
            StringDictionary result = new StringDictionary();

            foreach (StringKeyValueProxy item in list)
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }

        public static List<StringKeyValueProxy> GetStringKeyValueList(StringDictionary list)
        {
            List<StringKeyValueProxy> result = new List<StringKeyValueProxy>();

            foreach (DictionaryEntry item in list)
            {
                result.Add(new StringKeyValueProxy(item.Key.ToString(), item.Value.ToString()));
            }

            return result;
        }

        #region 用户代理
        private static void ConvertUserProxy(UserProxy userproxy, User user)
        {
            userproxy.UserID = user.UserID;
            userproxy.Username = user.Username;
            userproxy.Email = user.Email;
            userproxy.AvatarSrc = user.AvatarSrc;
            userproxy.AvatarUrl_120px = user.BigAvatarPath;
            userproxy.AvatarUrl_48px = user.AvatarPath;
            userproxy.AvatarUrl_24px = user.SmallAvatarPath;
            userproxy.Realname = user.Realname;
            userproxy.MobilePhone = user.MobilePhone;
            userproxy.Gender = (byte)user.Gender;
            userproxy.Birthdat = user.Birthday;
            userproxy.Points = new int[user.ExtendedPoints.Length];
            for (int i = 0; i < user.ExtendedPoints.Length; i++)
            {
                userproxy.Points[i] = user.ExtendedPoints[i];
            }
        }

        public static UserProxy GetUserProxy(User user)
        {
            if (user == null)
            {
                return null;
            }

            UserProxy userproxy = new UserProxy();
            ConvertUserProxy(userproxy, user);

            return userproxy;
        }

        public static DataForLogin GetAuthUserProxy(AuthUser authUser)
        {
            if (authUser == null)
                return null;

            DataForLogin authuserProxy = new DataForLogin();
            ConvertUserProxy(authuserProxy, authUser);

            authuserProxy.Password = authUser.Password;
            authuserProxy.PasswordFormat = (int)authUser.PasswordFormat;
            authuserProxy.UnreadMessages = authUser.UnreadMessages;
            authuserProxy.UnreadNotifies = GetUnreadNotifiesProxy(authUser.UnreadNotify);
            authuserProxy.IsActive = authUser.IsActive;

            authuserProxy.ExtendedFields = new List<UserExtendedValueProxy>();

            foreach (UserExtendedValue field in authUser.ExtendedFields)
            {
                authuserProxy.ExtendedFields.Add(GetUserExtendedValueProxy(field));
            }

            return authuserProxy;
        }

        public static UserProfileProxy GetUserProfileProxy(AuthUser user)
        {
            if (user == null)
                return null;

            UserProfileProxy proxy = new UserProfileProxy();
            proxy.UserID = user.UserID;
            proxy.Gender = (int)user.Gender;
            proxy.GenderName = user.GenderName;
            proxy.Birthday = user.Birthday;
            proxy.TimeZone = user.TimeZone;
            proxy.Signature = user.Signature;
            proxy.SignatureFormat = (int)user.SignatureFormat;
            proxy.ExtendedFields = new List<UserExtendedValueProxy>();

            foreach (UserExtendedValue item in user.ExtendedFields)
            {
                proxy.ExtendedFields.Add(GetUserExtendedValueProxy(item));
            }

            return proxy;
        }

        public static UserExtendedValueProxy GetUserExtendedValueProxy(UserExtendedValue userExtendedValue)
        {
            if (userExtendedValue == null)
                return null;

            UserExtendedValueProxy proxy = new UserExtendedValueProxy();
            proxy.ExtendedFieldID = userExtendedValue.ExtendedFieldID;
            proxy.PrivacyType = (int)userExtendedValue.PrivacyType;
            proxy.UserID = userExtendedValue.UserID;
            proxy.Value = userExtendedValue.Value;

            return proxy;
        }

        public static MaxLabs.Passport.Proxy.FriendGroupProxy GetFriendGroupProxy(FriendGroup group)
        {
            if (group == null)
                return null;

            MaxLabs.Passport.Proxy.FriendGroupProxy friendGroupProxy = new MaxLabs.Passport.Proxy.FriendGroupProxy();
            friendGroupProxy.GroupID = group.GroupID;
            friendGroupProxy.Name = group.Name;
            friendGroupProxy.TotalFriends = group.TotalFriends;
            friendGroupProxy.IsShield = group.IsShield;
            friendGroupProxy.OwnerUserID = group.UserID;
            return friendGroupProxy;
        }

        public static MaxLabs.Passport.Proxy.DataForDeleteFriendGroup GetDeleteFriendGroupProxy(int userID, int groupID, bool deleteFriend)
        {
            MaxLabs.Passport.Proxy.DataForDeleteFriendGroup deleteFriendGroupProxy = new DataForDeleteFriendGroup();
            deleteFriendGroupProxy.DeleteFriends = deleteFriend;
            deleteFriendGroupProxy.GroupID = groupID;
            deleteFriendGroupProxy.OperatorUserID = userID;

            return deleteFriendGroupProxy;
        }

        public static MaxLabs.Passport.Proxy.DataForShieldFriendGroup GetShieldFriendGroupProxy(int userID, IEnumerable<int> groupIDs, bool isShield)
        {
            MaxLabs.Passport.Proxy.DataForShieldFriendGroup shieldFriendGroupProxy = new DataForShieldFriendGroup();

            List<int> ids = new List<int>();
            foreach (int id in groupIDs)
                ids.Add(id);
            shieldFriendGroupProxy.GroupIDs = ids;
            shieldFriendGroupProxy.IsShield = isShield;
            shieldFriendGroupProxy.OperatorUserID = userID;

            return shieldFriendGroupProxy;
        }

        public static MaxLabs.Passport.Proxy.FriendIDsProxy GetFriendIDsProxy(int userID, IEnumerable<int> friendIDs, int? groupID)
        {
            MaxLabs.Passport.Proxy.FriendIDsProxy friendIDsProxy = new FriendIDsProxy();

            List<int> ids = new List<int>();
            foreach (int id in friendIDs)
                ids.Add(id);
            friendIDsProxy.FriendIDs = ids;
            friendIDsProxy.UserID = userID;
            if (groupID != null)
                friendIDsProxy.FriendGroupID = groupID.Value;

            return friendIDsProxy;
        }

        public static MaxLabs.Passport.Proxy.FriendDataProxy GetFriendNotifyProxy(int userID, int friendUserID, int groupID, int hisGroupID)
        {
            MaxLabs.Passport.Proxy.FriendDataProxy friendNotifyProxy = new FriendDataProxy();

            friendNotifyProxy.FriendUserID = friendUserID;
            friendNotifyProxy.UserID = userID;
            friendNotifyProxy.FriendGroupID = groupID;

            return friendNotifyProxy;
        }

        public static MaxLabs.Passport.Proxy.UserIDsProxy GetUserIDsProxy(int userID, IEnumerable<int> userIdsToAdd)
        {
            MaxLabs.Passport.Proxy.UserIDsProxy userIDsProxy = new UserIDsProxy();

            List<int> ids = new List<int>();
            foreach (int id in userIdsToAdd)
                ids.Add(id);

            userIDsProxy.UserIDs = ids;
            userIDsProxy.OperatorUserID = userID;

            return userIDsProxy;
        }

        public static MaxLabs.Passport.Proxy.FriendHotProxy GetFriendHotProxy(int userID, MaxLabs.bbsMax.Enums.HotType type, int friendUserID)
        {
            MaxLabs.Passport.Proxy.FriendHotProxy friendHotProxy = new FriendHotProxy();

            friendHotProxy.FriendID = friendUserID;
            friendHotProxy.UserID = userID;
            friendHotProxy.HotType = (int)type;

            return friendHotProxy;
        }

        public static FriendProxy GetFriendProxy(MaxLabs.bbsMax.Entities.Friend friend)
        {
            if (friend == null) return null;

            FriendProxy friendProxy = new FriendProxy();
            friendProxy.UserID = friend.UserID;
            friendProxy.OwnerID = friend.OwnerID;
            friendProxy.Hot = friend.Hot;
            friendProxy.GroupID = friend.GroupID;
            friendProxy.CreateDate = friend.CreateDate.AddHours(-DateTimeUtil.DatabaseTimeDifference);
            return friendProxy;
        }

        public static DataForShieldFriendGroup GetShieldFriendGroupProxy(int userID, List<int> groupIDs, bool isShield)
        {
            DataForShieldFriendGroup shieldFriendGroupProxy = new DataForShieldFriendGroup();
            shieldFriendGroupProxy.OperatorUserID = userID;
            shieldFriendGroupProxy.IsShield = isShield;
            shieldFriendGroupProxy.GroupIDs = groupIDs;

            return shieldFriendGroupProxy;
        }

        public static DataForNewUser GetDataForNewUser(AuthUser user)
        {

            if (user == null || user == User.Guest)
                return null;

            DataForNewUser userDataProxy = new DataForNewUser();
            ConvertUserProxy(userDataProxy, user);

            userDataProxy.GenderName = user.GenderName;
            userDataProxy.ExtendedFields = new List<UserExtendedValueProxy>();

            foreach (UserExtendedValue extendedValue in user.ExtendedFields)
            {
                userDataProxy.ExtendedFields.Add(GetUserExtendedValueProxy(extendedValue));
            }

            userDataProxy.IPAddress = user.LastVisitIP;
            userDataProxy.IsActive = user.IsActive;
            userDataProxy.Password = user.Password;
            userDataProxy.PasswordFormat = (int)user.PasswordFormat;
            userDataProxy.Signature = user.Signature;
            userDataProxy.SignatureFormat = (int)user.SignatureFormat;
            userDataProxy.TimeZone = user.TimeZone;
            userDataProxy.InviterID = user.InviterID;

            //getfriend

            FriendGroupCollection friendgroups = user.FriendGroups;
            userDataProxy.FriendGroups = new List<MaxLabs.Passport.Proxy.FriendGroupProxy>();

            foreach (FriendGroup fg in friendgroups)
            {
                MaxLabs.Passport.Proxy.FriendGroupProxy fgp = GetFriendGroupProxy(fg);
                userDataProxy.FriendGroups.Add(fgp);
                FriendCollection friends = FriendBO.Instance.GetFriends(user.UserID, fg.GroupID);

                foreach (MaxLabs.bbsMax.Entities.Friend friend in friends)
                {
                    fgp.Friends.Add(GetFriendProxy(friend));
                }
            }

            //getExtendedFields
            //userDataProxy.ExtendedFields = GetStringKeyValueList(user.ExtendedFields);

            return userDataProxy;
        }

        public static UserAvatarProxy GetDataForChangeAvatar(int userID, string avatarSrc, string smallAvatarPath, string defaultAvatarPath, string bigAvatarPath)
        {
            UserAvatarProxy proxy = new UserAvatarProxy();
            proxy.UserID = userID;
            proxy.AvatarSrc = avatarSrc;
            proxy.BigAvatar = bigAvatarPath;
            proxy.DefaultAvatar = defaultAvatarPath;
            proxy.SmallAvatar = smallAvatarPath;

            return proxy;
        }



        #region 通知
        public static NotifyProxy GetNotifyProxy(Notify notify)
        {
            if (notify == null)
                return null;

            NotifyProxy notifyProxy = new NotifyProxy();

            notifyProxy.NotifyID = notify.NotifyID;
            notifyProxy.Content = notify.Content;
            //notifyProxy.Url = notify.Url;
            notifyProxy.PostDate = notify.UpdateDate;
            notifyProxy.UserID = notify.UserID;
            notifyProxy.UpdateDate = notifyProxy.UpdateDate;

            foreach (NotifyAction action in notify.Actions)
            {
                notifyProxy.Actions.Add(new NotifyActionProxy(action.Title, action.Url, action.IsDialog));
            }


            notifyProxy.TypeName = notify.TypeName;
            notifyProxy.ClientID = notify.ClientID;
            notifyProxy.TypeID = notify.TypeID;
            notifyProxy.Keyword = notify.Keyword;
            notifyProxy.IsRead = notify.IsRead;
            notifyProxy.UpdateDate = notify.UpdateDate;

            notifyProxy.DataTable = new List<StringKeyValueProxy>();

            foreach (string key in notify.DataTable.Keys)
            {
                notifyProxy.DataTable.Add(new StringKeyValueProxy(key, notify.DataTable[key]));
            }

            return notifyProxy;
        }

        public static NotifyTypeProxy GetNoyifyTypeProxy(NotifyType type)
        {
            if (type == null) return null;

            NotifyTypeProxy proxy = new NotifyTypeProxy();

            proxy.TypeID = type.TypeID;
            proxy.TypeName = type.TypeName;
            proxy.Keep = type.Keep;

            return proxy;
        }

        public static List<NotifyTypeProxy> GetNotifyTypeProxyList(IEnumerable<NotifyType> notifyTypes)
        {
            List<NotifyTypeProxy> list = new List<NotifyTypeProxy>();
            foreach (NotifyType nt in notifyTypes)
                list.Add(GetNoyifyTypeProxy(nt));

            return list;
        }

        public static SystemNotifyProxy GetSystemNotifyProxy(SystemNotify notify)
        {

            if (notify == null)
                return null;

            SystemNotifyProxy systemNotifyProxy = new SystemNotifyProxy();

            foreach (NotifyAction action in notify.Actions)
            {
                systemNotifyProxy.Actions.Add(new NotifyActionProxy(action.Title, action.Url, action.IsDialog));
            }

            systemNotifyProxy.BeginDate = notify.BeginDate;
            systemNotifyProxy.ClientID = notify.ClientID;
            systemNotifyProxy.Content = notify.Content;
            systemNotifyProxy.CreateDate = notify.CreateDate;

            systemNotifyProxy.DataTable = new List<StringKeyValueProxy>();

            foreach (string key in notify.DataTable.Keys)
            {
                systemNotifyProxy.DataTable.Add(new StringKeyValueProxy(key, notify.DataTable[key]));
            }

            systemNotifyProxy.DispatcherID = notify.DispatcherID;
            systemNotifyProxy.DispatcherIP = notify.DispatcherIP;
            systemNotifyProxy.EndDate = notify.EndDate;
            systemNotifyProxy.IsRead = notify.IsRead;
            systemNotifyProxy.Keyword = notify.Keyword;
            systemNotifyProxy.NotifyID = notify.NotifyID;
            systemNotifyProxy.PostDate = notify.CreateDate;
            systemNotifyProxy.ReadUserIDs = notify.ReadUserIDs;
            systemNotifyProxy.ReceiveUserIDs = notify.ReceiveUserIDs;
            systemNotifyProxy.ReceiveRoles = notify.ReceiveRoles;
            systemNotifyProxy.Subject = notify.Subject;
            systemNotifyProxy.TypeID = notify.TypeID;
            systemNotifyProxy.TypeName = notify.TypeName;
            systemNotifyProxy.UpdateDate = notify.UpdateDate;
            systemNotifyProxy.Url = notify.Url;
            systemNotifyProxy.UserID = notify.UserID;

            //systemNotifyProxy.ReceiveUserIDs = new int[notify.ReceiveUserIDs.Count];
            //systemNotifyProxy.ReceiveRoles = new Guid[notify.ReceiveRoles.Count];

            //notify.ReceiveRoles.CopyTo(systemNotifyProxy.ReceiveRoles);
            //notify.ReceiveUserIDs.CopyTo(systemNotifyProxy.ReceiveUserIDs);

            //if (notify.ReadUserIDs != null)
            //{
            //    systemNotifyProxy.ReadedUserIDs = notify.ReadUserIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //}
            //else
            //{
            //    systemNotifyProxy.ReadedUserIDs = new string[0];
            //}

            //systemNotifyProxy.BeginDate = notify.BeginDate;
            //systemNotifyProxy.EndDate = notify.EndDate;
            //systemNotifyProxy.Content = notify.Content;
            //systemNotifyProxy.Subject = notify.Subject;

            return systemNotifyProxy;
        }

        public static UnreadNotifiesProxy GetUnreadNotifiesProxy(UnreadNotifies unreadnotifies)
        {
            if (unreadnotifies == null)
                return null;
            UnreadNotifiesProxy proxy = new UnreadNotifiesProxy();
            proxy.Count = unreadnotifies.Count;
            proxy.UserID = unreadnotifies.UserID;
            proxy.Items = new List<UnreadNotifyItemProxy>();

            foreach (UnreadNotifyItem item in unreadnotifies.Items)
            {
                UnreadNotifyItemProxy proxyItem = new UnreadNotifyItemProxy();
                proxyItem.TypeID = item.TypeID;
                proxyItem.Count = item.UnreadCount;
                proxy.Items.Add(proxyItem);
            }

            return proxy;
        }

        #endregion

        public static DataForUpdateChatMessageCount GetUserMessageCountProxy(Dictionary<int, int> counts)
        {
            DataForUpdateChatMessageCount proxy = new DataForUpdateChatMessageCount();

            proxy.UserIDs = new List<int>();
            proxy.MessageCounts = new List<int>();
            foreach (KeyValuePair<int, int> pair in counts)
            {
                proxy.UserIDs.Add(pair.Key);
                proxy.MessageCounts.Add(pair.Value);
            }

            return proxy;
        }

        public static ChatMessageProxy GetChatMessageProxy(ChatMessage message)
        {
            if (message == null)
                return null;
            ChatMessageProxy proxy = new ChatMessageProxy();
            proxy.Content = message.Content;
            proxy.CreateDate = message.CreateDate;
            proxy.CreateIP = message.CreateIP;
            proxy.FromMessageID = message.FromMessageID;
            proxy.IsRead = message.IsRead;
            proxy.IsReceive = message.IsReceive;
            proxy.MessageID = message.MessageID;
            proxy.OriginalContent = message.OriginalContent;
            proxy.TargetUserID = message.TargetUserID;
            proxy.UserID = message.UserID;

            return proxy;
        }

        public static ChatSessionProxy GetChatSessionProxy(ChatSession session)
        {
            if (session == null)
                return null;
            ChatSessionProxy proxy = new ChatSessionProxy();
            proxy.ChatSessionID = session.ChatSessionID;
            proxy.CreateDate = session.CreateDate;
            proxy.LastMessage = session.LastMessage;
            proxy.OwnerID = session.OwnerID;
            proxy.TotalMessages = session.TotalMessages;
            proxy.UnreadMessages = session.UnreadMessages;
            proxy.UpdateDate = session.UpdateDate;
            proxy.UserID = session.UserID;

            return proxy;
        }

        //public static ChatSessionFilterProxy GetChatSessionFilterProxy(ChatSessionFilter filter)
        //{
        //    if (filter == null)
        //        return null;
        //    ChatSessionFilterProxy proxy = new ChatSessionFilterProxy();
        //    proxy.BeginDate = filter.BeginDate;
        //    proxy.Contains = filter.Contains;
        //    proxy.EndDate = filter.EndDate;
        //    proxy.IsDesc = filter.IsDesc;
        //    proxy.PageSize = filter.PageSize;
        //    proxy.ShowAll = filter.ShowAll;
        //    proxy.UserID = filter.UserID;
        //    proxy.Username = filter.Username;

        //    return proxy;
        //}

        public static ExtendedFieldProxy GetExtendedFieldProxy(ExtendedField field)
        {
            if (field == null)
                return null;

            ExtendedFieldProxy proxy = new ExtendedFieldProxy();
            proxy.Description = field.Description;
            proxy.DisplayType = (int)field.DisplayType;
            proxy.FieldTypeName = field.FieldTypeName;
            proxy.IsRequired = field.IsRequired;
            proxy.Key = field.Key;
            proxy.Name = field.Name;
            proxy.Searchable = field.Searchable;
            proxy.Settings = new List<StringKeyValueProxy>();

            foreach (DictionaryEntry item in field.Settings)
            {
                string value;
                if (item.Value == null)
                    value = string.Empty;
                else
                    value = item.Value.ToString();
                StringKeyValueProxy temp = new StringKeyValueProxy(item.Key.ToString(), value);
                proxy.Settings.Add(temp);
            }

            proxy.SortOrder = field.SortOrder;

            return proxy;
        }

        public static ExtendedField GetExtendedField(MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy proxy)
        {
            if (proxy == null)
                return null;

            ExtendedField field = new ExtendedField();
            field.Description = proxy.Description;
            field.DisplayType = (ExtendedFieldDisplayType)proxy.DisplayType;
            field.FieldTypeName = proxy.FieldTypeName;
            field.IsRequired = proxy.IsRequired;
            field.Key = proxy.Key;
            field.Name = proxy.Name;
            field.Searchable = proxy.Searchable;
            field.Settings = new StringTable();

            foreach (MaxLabs.bbsMax.PassportServerInterface.StringKeyValueProxy keyValue in proxy.Settings)
            {
                field.Settings.Add(keyValue.Key, keyValue.Value);
            }
            field.IsPassport = true;
            field.SortOrder = proxy.SortOrder;

            return field;
        }

        #endregion


    }
}