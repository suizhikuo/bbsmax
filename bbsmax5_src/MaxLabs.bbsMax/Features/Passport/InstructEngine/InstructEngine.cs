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
using MaxLabs.bbsMax.Settings;
using System.Threading;
using MaxLabs.bbsMax.Enums;
using MaxLabs.Passport.Proxy;

namespace MaxLabs.bbsMax.Passport
{
    /// <summary>
    /// PASSPORT 指令引擎
    /// </summary>
    public class InstructEngine
    {
        #region 系统事件接口

        /// <summary>
        /// 进行系统事件挂钩
        /// </summary>
        private void CreateEventHook()
        {
            #region 用户

            UserBO.OnUserPasswordChanged += new UserPasswordChanged(UserBO_OnUserPasswordChanged);
            UserBO.OnUserPointChanged += new UserPointChanged(UserBO_OnUserPointChanged);
            UserBO.OnUserProfileChanged += new UserProfileChanged(UserBO_OnUserProfileChanged);
            UserBO.OnUserExtendFieldChanged += new UserExtendFieldChanged(UserBO_OnUserExtendFieldChanged);
            UserBO.OnUserAvatarChanged += new UserAvatarChanged(UserBO_OnUserAvatarChanged);
            UserBO.OnUserCreated += new UserCreated(UserBO_OnUserCreated);
            UserBO.OnUserRealnameChecked += new UserRealnameChecked(UserBO_OnUserRealnameChecked);
            UserBO.OnUserCancelRealnameCheck += new UserCancelRealnameCheck(UserBO_OnUserCancelRealnameCheck);
            UserBO.OnUserBindMobilePhone += new UserBindMobilePhine(UserBO_OnUserBindMobilePhone);
            UserBO.OnUserUnbindMobilePhone += new UserUnbindMobilePhone(UserBO_OnUserUnbindMobilePhone);
            UserBO.OnUserEmailChanged += new UserEmailChanged(UserBO_OnUserEmailChanged);
            UserBO.OnUserDoingUpdated += new UserDoingUpdated(UserBO_OnUserDoingUpdated);
            UserBO.OnUserLogout += new UserLogout(UserBO_OnUserLogout);

            #endregion

            #region 好友

            FriendBO.OnAcceptFriend += new AcceptFriend(FriendBO_OnAcceptFriend);
            FriendBO.OnMoveFriend += new MoveFriend(FriendBO_OnMoveFriend);
            FriendBO.OnRemoveFriend += new RemoveFriend(FriendBO_OnRemoveFriend);

            FriendBO.OnFriendGroupCreated += new FriendGroupCreated(FriendBO_OnFriendGroupCreated);
            FriendBO.OnFriendGroupDeleted += new FriendGroupDeleted(FriendBO_OnFriendGroupDeleted);
            FriendBO.OnFriendGroupUpdated += new FriendGroupUpdated(FriendBO_OnFriendGroupUpdated);
            FriendBO.OnFriendGroupShielded += new FriendGroupShielded(FriendBO_OnFriendGroupShielded);

            FriendBO.OnDeleteFromBlacklist += new DeleteFromBlacklist(FriendBO_OnDeleteFromBlacklist);
            FriendBO.OnAddUsersToBlacklist += new AddUsersToBlacklist(FriendBO_OnAddUsersToBlacklist);
            FriendBO.OnUpdateFriendHot += new UpdateFriendHot(FriendBO_OnUpdateFriendHot);
            #endregion

            #region 消息

            ChatBO.OnUsersMessageCountChanged += new Chat_UsersMessageCountChanged(ChatBO_OnChatUsersMessageCountChanged);

            #endregion


            #region 系统、用户通知

            NotifyBO.OnUserNotifyCountChanged += new UserNotifyCountChanged(NotifyBO_OnUserNotifyCountChanged);
            NotifyBO.OnUserIgnoreSystemNotify += new UserIgnoreSystemNotify(NotifyBO_OnUserIgnoreSystemNotify);
            NotifyBO.OnSystemNotifyCreated += new SystemNotifyCreate(NotifyBO_OnSystemNotifyCreate);
            NotifyBO.OnSystemNotifyDeleted += new SystemnotifyDeleted(NotifyBO_OnSystemnotifyDeleted);
            NotifyBO.OnSystemNotifyUpdated += new SystemNotifyUpdated(NotifyBO_OnSystemNotifyUpdated);

            #endregion
        }

        void UserBO_OnUserDoingUpdated(int userID, string doing)
        {
            CreateInstruct(userID, InstructType.User_DoingUpdated, doing);
        }

        void UserBO_OnUserEmailChanged(int userID, string newEmail)
        {
            CreateInstruct(userID, InstructType.User_EmailChanged, newEmail);
        }

        void UserBO_OnUserLogout(int userID)
        {
            CreateInstruct(userID, InstructType.User_Logout, userID);
        }

        void UserBO_OnUserUnbindMobilePhone(int userID)
        {
            CreateInstruct(userID, InstructType.User_UnbindMobilePhone, 0);
        }

        void UserBO_OnUserBindMobilePhone(int userID, long phoneNumber)
        {
            CreateInstruct(userID, InstructType.User_BindMobilePhone, phoneNumber);
        }

        void UserBO_OnUserCancelRealnameCheck(int userID)
        {
            CreateInstruct(userID, InstructType.User_CancelRealnameCheck, string.Empty);
        }

        void UserBO_OnUserRealnameChecked(int userID, string realname, string idCardNumber)
        {
            CreateInstruct(userID, InstructType.User_RealnameChecked, realname);
        }


        void FriendBO_OnFriendGroupUpdated(int userID, FriendGroup group)
        {
            CreateInstruct(userID, InstructType.Friend_GroupUpdated, ProxyConverter.GetFriendGroupProxy(group));
        }

        void FriendBO_OnFriendGroupDeleted(int userID, int groupID, bool deleteFriend)
        {
            CreateInstruct(userID, InstructType.Friend_GroupDeleted, ProxyConverter.GetDeleteFriendGroupProxy(userID, groupID, deleteFriend));
        }

        void FriendBO_OnFriendGroupCreated(int userID, FriendGroup group)
        {
            CreateInstruct(userID, InstructType.Friend_GroupCreated, ProxyConverter.GetFriendGroupProxy(group));
        }

        void FriendBO_OnFriendGroupShielded(int userID, IEnumerable<int> groupIDs, bool isShield)
        {
            CreateInstruct(userID, InstructType.Friend_GroupShielded, ProxyConverter.GetShieldFriendGroupProxy(userID, groupIDs, isShield));
        }

        void FriendBO_OnRemoveFriend(int userID, IEnumerable<int> friendIDs)
        {
            CreateInstruct(userID, InstructType.Friend_Deleted, ProxyConverter.GetFriendIDsProxy(userID, friendIDs, null));
        }

        void FriendBO_OnMoveFriend(int userID, IEnumerable<int> friendIDs, int newGroupID)
        {
            CreateInstruct(userID, InstructType.Friend_Moved, ProxyConverter.GetFriendIDsProxy(userID, friendIDs, newGroupID));
        }

        void FriendBO_OnAcceptFriend(int userID, int friendUserID, int groupID, int hisGroupID)
        {
            CreateInstruct(userID, InstructType.Friend_Accept, ProxyConverter.GetFriendNotifyProxy(userID, friendUserID, groupID, hisGroupID));
        }

        void FriendBO_OnDeleteFromBlacklist(int userID, int userIDInBlacklist)
        {
            CreateInstruct(userID, InstructType.Friend_DeleteBlack, userIDInBlacklist);
        }

        void FriendBO_OnAddUsersToBlacklist(int userID, IEnumerable<int> userIdsToAdd)
        {
            CreateInstruct(userID, InstructType.Friend_AddBlack, ProxyConverter.GetUserIDsProxy(userID, userIdsToAdd));
        }

        void FriendBO_OnUpdateFriendHot(int userID, MaxLabs.bbsMax.Enums.HotType type, int friendUserID)
        {
            CreateInstruct(userID, InstructType.Friend_UpdateHot, ProxyConverter.GetFriendHotProxy(userID, type, friendUserID));
        }

        //---

        void ChatBO_OnChatUsersMessageCountChanged(Dictionary<int, int> usersUnreadMessageCount)
        {
            this.CreateInstruct(0, InstructType.User_ChatMessageChanged, ProxyConverter.GetUserMessageCountProxy(usersUnreadMessageCount));
        }

        //---

        void NotifyBO_OnUserIgnoreSystemNotify(int userID, int systemNotifyID)
        {
            CreateInstruct(userID, InstructType.Notify_UserIgnoreSystemNotify, systemNotifyID);
        }

        void NotifyBO_OnSystemNotifyUpdated(SystemNotify notify)
        {
            this.CreateInstruct(0, InstructType.notify_SystemNotifyUpdated, ProxyConverter.GetSystemNotifyProxy(notify));
        }

        void NotifyBO_OnSystemnotifyDeleted(IEnumerable<int> notifyIDs)
        {
            this.CreateInstruct(0, InstructType.Notify_SystemNotifyDeleted, notifyIDs);
        }

        void NotifyBO_OnSystemNotifyCreate(SystemNotify notify)
        {
            this.CreateInstruct(0, InstructType.Notify_SystemNotifyCreate, ProxyConverter.GetSystemNotifyProxy(notify));
        }

        void NotifyBO_OnUserNotifyCountChanged(UnreadNotifies unreads)
        {
            CreateInstruct(unreads.UserID, InstructType.Notify_UserNotifyCountChanged, ProxyConverter.GetUnreadNotifiesProxy(unreads));
        }

        void UserBO_OnUserCreated(AuthUser newUser)
        {
            DataForNewUser data = ProxyConverter.GetDataForNewUser(newUser);
            CreateInstruct(0, InstructType.User_Create, data);
        }

        void UserBO_OnUserAvatarChanged(int userID, string avatarSrc, string smallAvatarPath, string defaultAvatarPath, string bigAvatarPath)
        {
            UserAvatarProxy data = ProxyConverter.GetDataForChangeAvatar(userID, avatarSrc, smallAvatarPath, defaultAvatarPath, bigAvatarPath);
            this.CreateInstruct(userID, InstructType.User_ChangeAvatar, data);
        }

        void UserBO_OnUserProfileChanged(AuthUser user)
        {
            CreateInstruct(user.UserID, InstructType.User_ChangeProfile, ProxyConverter.GetUserProfileProxy(user));
        }

        void UserBO_OnUserExtendFieldChanged(ExtendedFieldCollection extendedFields)
        {
            ExtendedFieldProxy[] fields = new ExtendedFieldProxy[extendedFields.Count];

            int i = 0;
            foreach (ExtendedField field in extendedFields)
            {
                fields[i] = ProxyConverter.GetExtendedFieldProxy(field);
                i++;
            }

            CreateInstruct(0, InstructType.Setting_UpdateUserExtendedField, fields);
        }

        void UserBO_OnUserPasswordChanged(int userID, string password)
        {
            UserPasswordProxy passwordProxy = new UserPasswordProxy();
            passwordProxy.UserID = userID;
            passwordProxy.Password = password;
            passwordProxy.PasswordFormat = (int)EncryptFormat.bbsMax;
            this.CreateInstruct(userID, InstructType.User_ChangePassword, passwordProxy);
        }

        void UserBO_OnUserPointChanged(int userID, int[] points)
        {
            this.CreateInstruct(userID, InstructType.User_UpdatePoint, points);
        }

        #endregion

        private static object locker = new object();

        private InstructEngine()
        {

        }

        public void StopService()
        {
            foreach ( KeyValuePair<int,InstructDriver> d in this.DriverList)
                d.Value.Dispose();
            ServiceClosed = true;
        }

        public void StartService()
        {
            foreach (KeyValuePair<int, InstructDriver> d in this.DriverList)
                d.Value.Restart();
            ServiceClosed = false;

        }

        public bool ServiceClosed
        {
            get;
            private set;

        }

        /// <summary>
        /// 系统初始化的时候执行
        /// </summary>
        public static void Initialize()
        {
            if (m_Instance == null)
            {
                lock (locker)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new InstructEngine();

                        m_Instance.DriverList = new Dictionary<int, InstructDriver>();
                        foreach (PassportClient client in PassportBO.PassportClientList)
                        {
                            m_Instance.DriverList.Add(client.ClientID, new InstructDriver(client));
                        }
                        m_Instance.CreateEventHook();
                    }
                }
            }
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="clientID"></param>
        public void RemoveClient(int clientID)
        {
            lock (locker)
            {
                InstructDriver driver = this.DriverList[clientID];
                if (driver != null)
                {
                    this.DriverList.Remove(clientID);
                    driver.Stop();
                }
            }
        }

        /// <summary>
        /// 创建客户端
        /// 可能出现客户端已经存在的异常
        /// </summary>
        /// <param name="client"></param>
        public void CreateClient(PassportClient client)
        {
            lock (locker)
            {
                Dictionary<int, InstructDriver> drivers = new Dictionary<int, InstructDriver>(this.DriverList);
                drivers.Add(client.ClientID, new InstructDriver(client));
                this.DriverList = drivers;
            }
        }

        private static InstructEngine m_Instance;

        public static InstructEngine Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public Dictionary<int, InstructDriver> DriverList
        {
            get;
            private set;
        }

        public void CreateInstruct(int userID, InstructType type, object datas)
        {
            CreateInstruct(userID, type, datas, false);
        }

        public void CreateInstruct(int userID, InstructType type, object datas, bool isSync)
        {
            ///检查是否开放Passport服务器
            if (!AllSettings.Current.PassportServerSettings.EnablePassportService)
                return;

            Instruct ins = new Instruct();

            string tempData = string.Empty;

            if (Serializer.IsSimpleDataTypes(datas))
                tempData = datas.ToString();
            else
                tempData = Serializer.GetXML(datas);

            ins.Datas = tempData;
            ins.InstructType = type;
            ins.TargetID = userID;
            ins.IsSync = isSync;

            InstructDriver driver;

            foreach (KeyValuePair<int, InstructDriver> item in this.DriverList)
            {
                driver = item.Value;
                if (driver.Client.InstructTypes.Count == 0 || driver.Client.InstructTypes.Contains(type))
                {
                    driver.AppendInstruct(ins);
                }
            }
        }

        /// <summary>
        /// 生成一个
        /// </summary>
        /// <param name="fieldAndData"></param>
        /// <returns></returns>
        private string BuildXmlData(params object[] fieldAndData)
        {
            StringBuilder builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<data>");
            for (int i = 0; i < fieldAndData.Length; i++)
            {
                if (i % 2 == 0)
                {
                    builder.Append("<").Append(fieldAndData[i]).Append(">");
                }
                else if (i % 2 == 1)
                {
                    builder.Append(fieldAndData[i]);
                    builder.Append("</").Append(fieldAndData[i - 1]).Append(">");
                }
            }
            builder.Append("</data>");
            return builder.ToString();
        }
    }
}