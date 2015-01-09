//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.Settings;
using MaxLabs.Passport.Client;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.plugins
{
    public class PassportClientPlugin : PluginBase
    {
        public override void Initialize()
        {  
            if (Globals.PassportClient.EnablePassport)
            {   
                InstructHandlerManager.RegisterHandler(new UserRegisterHandler());
                InstructHandlerManager.RegisterHandler(new UserChangePasswordHandler());
                InstructHandlerManager.RegisterHandler(new UserChangeAvatarHandler());
                InstructHandlerManager.RegisterHandler(new UserChangeProfileHandler());
                InstructHandlerManager.RegisterHandler(new UserChangeExtendedFieldHandler());
                InstructHandlerManager.RegisterHandler(new UserEmailChangedHandler());
                InstructHandlerManager.RegisterHandler(new UserUnreadNotifyChangeHandler());

                InstructHandlerManager.RegisterHandler(new AccetFriendHandler());
                InstructHandlerManager.RegisterHandler(new DeleteFriendHandler());
                InstructHandlerManager.RegisterHandler(new MoveFriendHandler());
                InstructHandlerManager.RegisterHandler(new UpdateFriendHotHandler());
                InstructHandlerManager.RegisterHandler(new AddUsersToBlacklistHandler());
                InstructHandlerManager.RegisterHandler(new DeleteFromBlacklistHandler());
                InstructHandlerManager.RegisterHandler(new AddFriendGroupHandler());
                InstructHandlerManager.RegisterHandler(new DeleteFriendGroupHandler());
                InstructHandlerManager.RegisterHandler(new RenameFriendGroupHandler());
                InstructHandlerManager.RegisterHandler(new ShieldFriendGroupHandler());
                InstructHandlerManager.RegisterHandler(new UserChatMessageCountChangedHandler());

                InstructHandlerManager.RegisterHandler(new UserBindMobilePhoneHandler());
                InstructHandlerManager.RegisterHandler(new UserUnbindMobilePhoneHandler());
                InstructHandlerManager.RegisterHandler(new UserRealnameCheckedHandler());
                InstructHandlerManager.RegisterHandler(new UserCancelRealnameCheckHandler());
                InstructHandlerManager.RegisterHandler(new UserDoingUpdatedHandler());
                
            }
        }
    }

    public class UserDoingUpdatedHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_DoingUpdated; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            UserBO.Instance.Client_UpdateUserDoing(targetID, datas);
        }
    }

    public class UserEmailChangedHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_EmailChanged; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            UserBO.Instance.Client_UpdateUserEmail(targetID, datas);
        }
    }

    public class UserBindMobilePhoneHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_BindMobilePhone; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            long phone = StringUtil.TryParse<long>(datas, 0);
            UserBO.Instance.Client_UpdateUserMobilePhone(targetID, phone);
        }
    }

    public class UserUnbindMobilePhoneHandler : MaxHandlerBase
    {

        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_UnbindMobilePhone; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            UserBO.Instance.Client_UpdateUserMobilePhone(targetID, 0);
        }
    }

    public class UserLogoutHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_Logout; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            int userID = StringUtil.TryParse<int>(datas);
            UserBO.Instance.Client_Logout(userID);
        }
    }

    public class UserRealnameCheckedHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_RealnameChecked; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            UserBO.Instance.Client_SetUserRealnameChecked(targetID, datas);
        }
    }

    public class UserCancelRealnameCheckHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_CancelRealnameCheck; }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            UserBO.Instance.Client_SetUserRealnameUncheck(targetID);
        }
    }

    public class UserChangePasswordHandler:MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
              return  (int)InstructTypeEnum.User_ChangePassword;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            UserPasswordProxy passowrd = DataReadWrap.Get<UserPasswordProxy>(datas);
            UserBO.Instance.Client_ResetUserPassword(passowrd.UserID, passowrd.Password, (EncryptFormat)passowrd.PasswordFormat);
        }
    }

    public class UserChangeAvatarHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.User_ChangeAvatar;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            UserAvatarProxy avatarInfo = DataReadWrap.Get<UserAvatarProxy>(datas);
            UserBO.Instance.Client_ResetUserAvatar(avatarInfo);
        }
    }

    public class UserChangeProfileHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.User_ChangeProfile;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            UserProfileProxy proxy = DataReadWrap.Get<UserProfileProxy>(datas);
            UserBO.Instance.Client_UpdateUserProfile(proxy);
        }
    }

    public class UserChangeExtendedFieldHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Setting_UpdateExtendedField;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            ExtendedFieldProxy[] proxy = DataReadWrap.Get<ExtendedFieldProxy[]>(datas);
            UserBO.Instance.Client_UpdatePassportUserExtendFieldCache(proxy);
        }
    }

    public class UserRegisterHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_Create; }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.DataForNewUser authUserProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.DataForNewUser>(datas);

            MaxLabs.bbsMax.PassportServerInterface.DataForNewUser data = new MaxLabs.bbsMax.PassportServerInterface.DataForNewUser();

            data.UserID = authUserProxy.UserID;
            data.Email = authUserProxy.Email;
            data.Username = authUserProxy.Username;
            data.Password = authUserProxy.Password;
            data.PasswordFormat = authUserProxy.PasswordFormat;
            data.IPAddress = authUserProxy.IPAddress;
            data.InviterID = authUserProxy.InviterID;
            data.IsActive = authUserProxy.IsActive;
            data.Gender = authUserProxy.Gender;
            data.ExtendedFields = new MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy[]{}; //new MaxLabs.bbsMax.PassportServerInterface.StringKeyValueProxy[0];// authUserProxy.ExtendedFields;
            data.Birthdat = authUserProxy.Birthdat;
            data.AvatarSrc = authUserProxy.AvatarSrc;
            data.AvatarUrl_120px = authUserProxy.AvatarUrl_120px;
            data.AvatarUrl_24px = authUserProxy.AvatarUrl_24px;
            data.AvatarUrl_48px = authUserProxy.AvatarUrl_48px;
            data.MobilePhone = authUserProxy.MobilePhone;
            data.Realname = authUserProxy.Realname;
            data.Signature = authUserProxy.Signature;
            data.SignatureFormat = authUserProxy.SignatureFormat;
            data.TimeZone = authUserProxy.TimeZone;

            UserBO.Instance.Client_Register(data);
        }
    }

    public class UserUnreadNotifyChangeHandler : MaxHandlerBase
    {

        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Notify_UserNotifyCountChanged;
            }
        }

        public override void Execute(int targetID, DateTime instructDateTime, string datas)
        {
            UnreadNotifies unreadNotifies = new UnreadNotifies();

            MaxLabs.Passport.Proxy.UnreadNotifiesProxy proxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.UnreadNotifiesProxy>(datas);

            unreadNotifies.UserID = proxy.UserID;
            foreach (MaxLabs.Passport.Proxy.UnreadNotifyItemProxy item in proxy.Items)
            {
                unreadNotifies[item.TypeID] = item.Count;
            }

            NotifyBO.Instance.Client_SetUserUnreadNotifies(unreadNotifies);
        }
    }

    public class UserChatMessageCountChangedHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get { return (int)InstructTypeEnum.User_ChatMessageChanged; }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.DataForUpdateChatMessageCount counts = DataReadWrap.Get<MaxLabs.Passport.Proxy.DataForUpdateChatMessageCount>(datas);

            Dictionary<int, int> temp = new Dictionary<int, int>();

            for (int i = 0; i < counts.UserIDs.Count; i++)
            {
                temp.Add(counts.UserIDs[i],counts.MessageCounts[i]);
            }

            ChatBO.Instance.Client_UpdateUserUnReadMessageCount(temp);
        }
    }

    #region friend

    //public class AddFriendHandler : MaxHanderBase
    //{
    //    public override int InstructType
    //    {
    //        get
    //        {
    //            return (int)InstructTypeEnum.Friend_Added;
    //        }
    //    }

    //    public override void Execute(int userID, DateTime instructDateTime, string datas)
    //    {
    //        MaxLabs.Passport.Proxy.FriendProxy friendProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendProxy>(datas);
    //        FriendBO.Instance.Client_TryAddFriend(friendProxy.OwnerID, friendProxy.UserID, friendProxy.GroupID, friendProxy.Message);
    //    }
    //}

    public class AccetFriendHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_Accept;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.FriendDataProxy friendProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendDataProxy>(datas);
            FriendBO.Instance.Client_AcceptFriend(friendProxy.UserID, friendProxy.FriendUserID, friendProxy.FriendGroupID, friendProxy.HisFriendGroupID);
        }
    }

    public class DeleteFriendHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_Deleted;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.FriendIDsProxy friendIDsProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendIDsProxy>(datas);
            FriendBO.Instance.Client_DeleteFriends(friendIDsProxy.UserID, friendIDsProxy.FriendIDs);
        }
    }

    public class MoveFriendHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_Moved;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.FriendIDsProxy friendIDsProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendIDsProxy>(datas);
            FriendBO.Instance.Client_MoveFriends(friendIDsProxy.UserID, friendIDsProxy.FriendIDs, friendIDsProxy.FriendGroupID);
        }
    }

    public class UpdateFriendHotHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_UpdateHot;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.FriendHotProxy friendHotProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendHotProxy>(datas);
            FriendBO.Instance.Client_UpdateFriendHot(friendHotProxy.UserID, friendHotProxy.HotType, friendHotProxy.FriendID);
        }
    }

    public class AddUsersToBlacklistHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_AddBlack;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.UserIDsProxy userIDsProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.UserIDsProxy>(datas);
            FriendBO.Instance.Client_AddUsersToBlacklist(userIDsProxy.OperatorUserID, userIDsProxy.UserIDs);
        }
    }

    public class DeleteFromBlacklistHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_DeleteBlack;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            int id = int.Parse(datas);
            FriendBO.Instance.Client_DeleteFromBlacklist(userID, id);
        }
    }



    public class AddFriendGroupHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_GroupCreated;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.FriendGroupProxy friendGroupProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendGroupProxy>(datas);
            FriendBO.Instance.Client_AddFriendGroup(friendGroupProxy.OwnerUserID, friendGroupProxy.GroupID, friendGroupProxy.Name);
        }
    }

    public class DeleteFriendGroupHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_GroupDeleted;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.DataForDeleteFriendGroup deleteFriendGroupProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.DataForDeleteFriendGroup>(datas);
            FriendBO.Instance.Client_DeleteFriendGroup(deleteFriendGroupProxy.OperatorUserID, deleteFriendGroupProxy.GroupID, deleteFriendGroupProxy.DeleteFriends);
        }
    }

    public class RenameFriendGroupHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_GroupUpdated;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.FriendGroupProxy friendGroupProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.FriendGroupProxy>(datas);
            FriendBO.Instance.Client_RenameFriendGroup(friendGroupProxy.OwnerUserID, friendGroupProxy.GroupID, friendGroupProxy.Name);
        }
    }

    public class ShieldFriendGroupHandler : MaxHandlerBase
    {
        public override int InstructType
        {
            get
            {
                return (int)InstructTypeEnum.Friend_GroupShielded;
            }
        }

        public override void Execute(int userID, DateTime instructDateTime, string datas)
        {
            MaxLabs.Passport.Proxy.DataForShieldFriendGroup shieldFriendGroupProxy = DataReadWrap.Get<MaxLabs.Passport.Proxy.DataForShieldFriendGroup>(datas);
            FriendBO.Instance.Client_ShieldFriendGroups(shieldFriendGroupProxy.OperatorUserID, shieldFriendGroupProxy.GroupIDs, shieldFriendGroupProxy.IsShield);
        }
    }
    #endregion


    public abstract class MaxHandlerBase:InstructHandlerBase
    {
        //public override string Key
        //{
        //    get { return Globals.PassportClient.AccessKey; }
        //}
    }

    public enum InstructTypeEnum
    {
        #region 用户指令 100
        User_Create             = 101,
        User_ChangeProfile      = 102,
        User_ChangePassword     = 103,
        User_UpdatePoint        = 104,
        User_ChatMessageChanged = 105,
        User_ChangeAvatar       = 106,
        User_BindMobilePhone    = 107,
        User_UnbindMobilePhone  = 108,
        User_RealnameChecked    = 109,
        User_CancelRealnameCheck= 110,
        User_EmailChanged       = 112,
        User_Logout             = 111,
        User_DoingUpdated = 113,

        #endregion

        #region 通知 200

        Notify_SystemNotifyCreate       = 201,
        notify_SystemNotifyUpdated      = 202,
        Notify_SystemNotifyDeleted      = 203,
        Notify_UserNotifyCountChanged   = 204,
        Notify_UserIgnoreSystemNotify   = 205,

        #endregion

        #region 好友 300


        Friend_GroupCreated = 301,

        Friend_GroupUpdated = 302,

        Friend_GroupDeleted = 303,

        Friend_GroupShielded = 304,
        //Friend_Added = 304,
        Friend_Accept = 305,

        Friend_Deleted = 306,

        Friend_Moved = 307,

        Friend_UpdateHot = 308,

        Friend_AddBlack = 309,

        Friend_DeleteBlack = 310,
        #endregion

        #region 公告 400
        Announcement_ListChanged = 401,
        #endregion

        #region 设置 500

        Setting_UpdateExtendedField = 501,

        #endregion

        #region 网站类指令 900

        Website_Create      = 901,
        Website_Updated     = 902,
        Website_Verified    = 903,
        Website_Deleted     = 904,
        
        #endregion


        Other=99999
    }
}