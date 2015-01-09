//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Logs;
using MaxLabs.WebEngine.Plugin;
#if !Passport
using MaxLabs.bbsMax.PassportServerInterface;
#endif
using MaxLabs.Passport.Proxy;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Common;
using System.Diagnostics;

namespace MaxLabs.bbsMax
{
    public partial class UserBO : BOBase<UserBO>
    {
        #region events

        public static event UserPointChanged OnUserPointChanged;
        public static event UserProfileChanged OnUserProfileChanged;
        public static event UserExtendFieldChanged OnUserExtendFieldChanged;
        public static event UserPasswordChanged OnUserPasswordChanged;
        public static event UserAvatarChanged OnUserAvatarChanged;
        public static event UserCreated OnUserCreated;
        public static event UserLogout OnUserLogout;
        public static event UserRealnameChecked OnUserRealnameChecked;
        public static event UserCancelRealnameCheck OnUserCancelRealnameCheck;
        public static event UserBindMobilePhine OnUserBindMobilePhone;
        public static event UserUnbindMobilePhone OnUserUnbindMobilePhone;
        public static event UserEmailChanged OnUserEmailChanged;
        public static event UserDoingUpdated OnUserDoingUpdated;

        #endregion

        #region Passport client only

#if !Passport


        public void Client_UpdateUserDoing(int userID, string doing)
        {

            UserDao.Instance.UpdateUserDoing(userID, doing);

            AuthUser user = GetUserFromCache<AuthUser>(userID);
            if (user != null)
            {
                user.Doing = doing;
                user.DoingDate = DateTime.Now;
            }
        }

        public void Client_ResetUserPassword(int userID, string newPassword, EncryptFormat format)
        {
            AuthUser user = GetUserFromCache<AuthUser>(userID);
            if (user != null)
            {
                user.Password = newPassword;
                user.PasswordFormat = format;
            }
            UserDao.Instance.ResetUserPassword(userID, newPassword, EncryptFormat.bbsMax);
        }

        public void Client_Logout(int userID)
        {

            RemoveUserCache(userID);
            
            //TODO 在线列表移除
        }

        public void Client_ResetUserAvatar(UserAvatarProxy avatarProxy)
        {
            SimpleUser user = GetUser(avatarProxy.UserID);
            //string avatarStr = newAvatars[0];

            if (user != null)
            {
                
                user.AvatarSrc = avatarProxy.AvatarSrc;
                //user.SmallAvatarPath = null;
                //user.AvatarPath = null;
                //user.BigAvatar = null;
                UserDao.Instance.UpdateAvatar(avatarProxy.UserID, user.AvatarPropFlag.GetStringForSave(), true);
            }
            
        }

        public void Client_UpdateUserProfile(UserProfileProxy proxy)
        {
            //StringDictionary extendedFields = ProxyConverter.GetStringDictionary(proxy.ExtendedFields);

            UserExtendedValueCollection extendedFields = new UserExtendedValueCollection();

            foreach (MaxLabs.Passport.Proxy.UserExtendedValueProxy extendedValue in proxy.ExtendedFields)
            {
                extendedFields.Add(GetUserExtendedValue(extendedValue));
            }

            AuthUser user = GetAuthUser(proxy.UserID);

            if (user != null)
            {
                string signature=proxy.Signature;
                user.SignaturePropFlag.OriginalData = signature;
                signature = user.SignaturePropFlag.GetStringForSave();

                UserDao.Instance.UpdateUserProfile(proxy.UserID,
                    (MaxLabs.bbsMax.Enums.Gender)proxy.Gender,
                    (short)proxy.Birthday.Year, (short)proxy.Birthday.Month, (short)proxy.Birthday.Day,
                    signature, (SignatureFormat)proxy.SignatureFormat,
                    proxy.TimeZone,
                    extendedFields);
                RemoveUserCache(proxy.UserID);
            }
        }
        
        public UserExtendedValue GetUserExtendedValue(MaxLabs.Passport.Proxy.UserExtendedValueProxy proxy)
        {
            if (proxy == null)
                return null;

            UserExtendedValue extendedValue = new UserExtendedValue();
            extendedValue.Value = proxy.Value;
            extendedValue.ExtendedFieldID = proxy.ExtendedFieldID;
            extendedValue.PrivacyType = (ExtendedFieldDisplayType)proxy.PrivacyType;
            extendedValue.UserID = proxy.UserID;

            return extendedValue;
        }
        private UserExtendedValue GetUserExtendedValue2(MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy proxy)
        {
            if (proxy == null)
                return null;

            UserExtendedValue extendedValue = new UserExtendedValue();
            extendedValue.Value = proxy.Value;
            extendedValue.ExtendedFieldID = proxy.ExtendedFieldID;
            extendedValue.PrivacyType = (ExtendedFieldDisplayType)proxy.PrivacyType;
            extendedValue.UserID = proxy.UserID;

            return extendedValue;
        }

        public void Client_Register(MaxLabs.bbsMax.PassportServerInterface.DataForNewUser data)
        {
            int userID = data.UserID;
            int[] defaultPoints = new int[8];
            UserPointCollection points = AllSettings.Current.PointSettings.UserPoints;
            for (int i = 0; i < 8; i++)
            {
                defaultPoints[i] = points[i].Enable ? points[i].InitialValue : 0;
            }

            UserExtendedValueCollection userExtendedValues = new UserExtendedValueCollection();
            foreach (MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy kv in data.ExtendedFields)
            {
                userExtendedValues.Add(GetUserExtendedValue2(kv));
            }

            UserRegisterErrorCode result = UserRegisterErrorCode.Unknown;
            result = (UserRegisterErrorCode)UserDao.Instance.Register(ref userID,
                data.Username,
                data.Email,
                data.Password,
                (EncryptFormat)data.PasswordFormat,
                null,
                data.IPAddress,
                null, data.InviterID,
                data.IsActive,
                defaultPoints,
                0);

            if (string.IsNullOrEmpty(data.AvatarSrc) == false)
                UserDao.Instance.UpdateAvatar(data.UserID, data.AvatarSrc, true);

            UserDao.Instance.UpdateUserProfile(data.UserID, (MaxLabs.bbsMax.Enums.Gender)(int)data.Gender,
                (short)data.Birthdat.Year, (short)data.Birthdat.Month, (short)data.Birthdat.Day,
                data.Signature, (SignatureFormat)data.SignatureFormat,
                data.TimeZone, userExtendedValues);
            


            //if (result == UserRegisterErrorCode.Success)
            //{
            //    return true;
            //}
        }

        public void Client_UpdateUserMobilePhone(int userID, long mobilePhone)
        {

            AuthUser user = GetUserFromCache<AuthUser>(userID);
            if (user != null)
            {
                user.MobilePhone = mobilePhone;
            }
            UserDao.Instance.UpdateUserPhone(userID, mobilePhone);
        }

        /// <summary>
        /// 映射用户属性
        /// </summary>
        /// <param name="localUser"></param>
        /// <param name="passportUser"></param>
        private void Client_MappingUserFields(AuthUser localUser, MaxLabs.bbsMax.PassportServerInterface.DataForLogin passportUser)
        {
            //同步用户信息
            localUser.UserID = passportUser.UserID;
            localUser.Username = passportUser.Username;
            localUser.Password = passportUser.Password;
            localUser.PasswordFormat = (EncryptFormat)passportUser.PasswordFormat;
            localUser.Email = passportUser.Email;
            localUser.UnreadMessages = passportUser.UnreadMessages;
            localUser.Gender = (MaxLabs.bbsMax.Enums.Gender)passportUser.Gender;

            UnreadNotifies unreadnotifies = new UnreadNotifies();


            foreach (MaxLabs.bbsMax.PassportServerInterface.UnreadNotifyItemProxy item in passportUser.UnreadNotifies.Items)
                unreadnotifies[item.TypeID] = item.Count;

            localUser.UnreadNotify = unreadnotifies;
        }

        /// <summary>
        /// 映射数据库记录
        /// (返回的bool值并不是表示是否创建了这个用户，
        /// 而是表示本次是否真的尝试映射用户操作，无论成功与否。
        /// 一般来说每个页面的第一次调用会返回true，以后调用false)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private void Client_MappingUserRecord(int userID)
        {
            string key = "User/Map/" + userID;

            if (PageCacheUtil.Contains(key) == false)
            {
                PageCacheUtil.Set(key, true);

                MaxLabs.bbsMax.PassportServerInterface.DataForNewUser passportUserData = null;

                try
                {
                    passportUserData = Globals.PassportClient.PassportService.User_GetDataForNewUser(userID, true);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    LogHelper.CreateErrorLog(ex);
                    return;
                }

                if (passportUserData == null)
                    return;// null;

                //AuthUser user;
                //通行证中存在这个用户那么自动注册
                Client_Register(passportUserData);

            }

            //user = UserDao.Instance.GetAuthUser(userID);
            //return user;
        }
#endif

        #endregion

        public const string cookieKey_User = "bbsmax_user";
        private const int AdminSessionExpiresMinute = 15;
        private const string cacheKey_CurrentUserID = "User/CurrentID";
        private const string cacheKey_User_StartAs = "User/Item/";
        private const string cacheKey_UserID_StartAs = "User/UserID/";
        //private const string cacheKey_User_Byname_StartAs = "User/Item/N";
        private const string cacheKey_NotifySetting = "User/NotifySetting/{0}";
        private const string cacheKey_FillSimpleUserIds = "FillSimpleUserIds";
        private const string cacheKey_AdminSession = "admin/session/{0}";

        #region 获得当前已登录用户

        /// <summary>
        /// 返回当前用户实例，未登录就会返回User.Guest，永远都不可能返回null，
        /// </summary>
        /// <returns></returns>
        public AuthUser GetCurrentUser()
        {
            int userID = GetCurrentUserID();

            return GetAuthUser(userID);
        }


        /// <summary>
        /// 如果用户存在会返回用户实例，不存在就会返回User.Guest，永远都不可能返回null，
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public AuthUser GetAuthUser(int userID)
        {
            return GetAuthUser(userID, false);
        }

        /// <summary>
        /// 如果用户存在会返回用户实例，不存在就会返回User.Guest，永远都不可能返回null，
        /// </summary>
        public AuthUser GetAuthUser(int userID, bool pageCacheOnly)
        {
            if (userID <= 0)
            {
                return User.Guest;
            }

            bool needProcessKeyword;
            AuthUser user = GetUserFromCache<AuthUser>(userID, out needProcessKeyword);

            if (user == null)
            {
                user = UserDao.Instance.GetAuthUser(userID);
#if !Passport
                PassportClientConfig passportSetting = Globals.PassportClient;

                //挂接到了通行证
                if (passportSetting.EnablePassport)
                {
                    //得到通行证中的用户数据
                    MaxLabs.bbsMax.PassportServerInterface.DataForLogin data = null;
                    try
                    {
                        data = passportSetting.PassportService.User_GetDataForLogin(userID);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));

                        LogHelper.CreateErrorLog(ex);

                        return User.Guest;
                    }
                    //如果本地数据库不存在这个用户
                    if (user == null)
                    {
                        //通行证中也不存在这个用户，那必然是游客，可以直接返回
                        if (data == null)
                        {
                            user = User.Guest;

                            //游客的缓存仅加入页面级别缓存，以避免有人伪造cookie来请求不存在的UserID导致内存耗尽
                            string key = cacheKey_User_StartAs + userID;
                            PageCacheUtil.Set(key, user);

                            return user;
                        }

                        Client_MappingUserRecord(userID);
                        user = UserDao.Instance.GetAuthUser(userID);
                    }

                    AddUserToCache(user, pageCacheOnly);
                    if (data.FriendVersion != user.FirendVersion)
                    {
                        FriendBO.Instance.Client_SyncAllFriend(user);
                    }

                    ValidateUserExtendedProfiles(user, data);

                    needProcessKeyword = true;


                    //同步一些数据
                    Client_MappingUserFields(user, data);

                }
                else
#endif
                {
                    if (user == null)
                    {
                        user = User.Guest;

                        //已删除用户的缓存仅加入页面级别缓存，以避免有人伪造cookie来请求不存在的UserID导致内存耗尽
                        string key = cacheKey_User_StartAs + userID;
                        PageCacheUtil.Set(key, user);

                        return user;
                    }
                    else
                        needProcessKeyword = true;

                    AddUserToCache(user, pageCacheOnly);

                }
            }

            if (needProcessKeyword)
                ProcessKeyword(user, ProcessKeywordMode.TryUpdateKeyword);

            return user;
        }

        private void ValidateUserExtendedProfiles(AuthUser localUser, MaxLabs.bbsMax.PassportServerInterface.DataForLogin serverUser)
        {
            UserExtendedValueCollection insertFields = new UserExtendedValueCollection();
            UserExtendedValueCollection updateFields = new UserExtendedValueCollection();

            foreach (MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy extendedField in serverUser.ExtendedFields)
            {
                bool has = false;
                foreach (UserExtendedValue localField in localUser.ExtendedFields)
                {
                    if (extendedField.ExtendedFieldID == localField.ExtendedFieldID)
                    {
                        has = true;
                        if (extendedField.PrivacyType != (int)localField.PrivacyType
                            || extendedField.Value != localField.Value)
                        {
                            updateFields.Add(GetUserExtendedValue2(extendedField));
                        }
                        break;
                    }
                }

                if (has == false)
                    insertFields.Add(GetUserExtendedValue2(extendedField));
            }

            if (insertFields.Count > 0 || updateFields.Count > 0)
                UserDao.Instance.UpdateUserProfile(localUser.UserID, insertFields, updateFields);
        }

        public AuthUser GetAuthUserByEmail(string email, out bool duplicateEmail)
        {
            return GetAuthUserByEmail(email, false, out duplicateEmail);
        }

        public AuthUser GetAuthUserByEmail(string email, bool pageCacheOnly, out bool duplicateEmail)
        {
            duplicateEmail = false;

            if (string.IsNullOrEmpty(email))
                return User.Guest;

            int userID = GetUserIDFromPageCache(email, UserLoginType.Email);

            if (userID != -1)
                return GetAuthUser(userID);

            AuthUser user = UserDao.Instance.GetAuthUserByEmail(email, out duplicateEmail);

            //===================
#if !Passport
            PassportClientConfig passportSetting = Globals.PassportClient;

            //挂接到了通行证
            if (passportSetting.EnablePassport)
            {
                //得到通行证中的用户数据
                MaxLabs.bbsMax.PassportServerInterface.DataForLogin data = null;

                try
                {
                    data = passportSetting.PassportService.User_GetDataForLoginByEmail(email, out duplicateEmail);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    LogHelper.CreateErrorLog(ex);
                    return User.Guest;
                }

                if (duplicateEmail)
                {
                    return null;
                }

                //如果本地数据库不存在这个用户
                if (user == null)
                {
                    //通行证中也不存在这个用户，那必然是游客，可以直接返回
                    if (data == null)
                    {
                        //把当前用户名对应的UserID是0这一信息放入页面级缓存，以在下次调用本方法时可以迅速判断
                        SetUserIDPageCache(email, 0, UserLoginType.Email);

                        return User.Guest;
                    }

                    userID = data.UserID;

                    Client_MappingUserRecord(userID);
                    user = UserDao.Instance.GetAuthUser(userID);
                }
                else if (data == null)
                {
                    SetUserIDPageCache(email, 0, UserLoginType.Email);
                    return User.Guest;
                }

                //needProcessKeyword = true;
                AddUserToCache(user, pageCacheOnly);

                if (data.FriendVersion != user.FirendVersion)
                {
                    FriendBO.Instance.Client_SyncAllFriend(user);
                }

                Client_MappingUserFields(user, data);

            }
            else
#endif
            {
                if (duplicateEmail)
                {
                    return null;
                }

                //不存在这个用户
                if (user == null || user.UserID <= 0)
                {
                    //把当前用户名对应的UserID是0这一信息放入页面级缓存，以在下次调用本方法时可以迅速判断
                    SetUserIDPageCache(email, 0, UserLoginType.Email);
                    return User.Guest;
                }


                AddUserToCache(user, pageCacheOnly);
                SetUserIDPageCache(email, user.UserID, UserLoginType.Email);
                //needProcessKeyword = true;

            }
            //===================


            ProcessKeyword(user, ProcessKeywordMode.TryUpdateKeyword);

            return user;
        }

        public AuthUser GetAuthUser(string username)
        {
            return GetAuthUser(username, false);
        }

        public AuthUser GetAuthUser(string username, bool pageCacheOnly)
        {

            if (string.IsNullOrEmpty(username))
                return User.Guest;

            int userID = GetUserIDFromPageCache(username, UserLoginType.Username);

            if (userID != -1)
                return GetAuthUser(userID);

            //bool needProcessKeyword;
            AuthUser user = UserDao.Instance.GetAuthUser(username);


            //===================
#if !Passport
            PassportClientConfig passportSetting = Globals.PassportClient;

            //挂接到了通行证
            if (passportSetting.EnablePassport)
            {
                //得到通行证中的用户数据
                MaxLabs.bbsMax.PassportServerInterface.DataForLogin data = null;

                try
                {
                    data = passportSetting.PassportService.User_GetDataForLoginByUserame(username);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    LogHelper.CreateErrorLog(ex);
                    return User.Guest;
                }

                //如果本地数据库不存在这个用户
                if (user == null)
                {
                    //通行证中也不存在这个用户，那必然是游客，可以直接返回
                    if (data == null)
                    {
                        //把当前用户名对应的UserID是0这一信息放入页面级缓存，以在下次调用本方法时可以迅速判断
                        SetUserIDPageCache(username, 0, UserLoginType.Email);

                        return User.Guest;
                    }
                    userID = data.UserID;

                    Client_MappingUserRecord(userID);
                    user = UserDao.Instance.GetAuthUser(userID);
                }
                else
                {
                    if (data == null)//本地有这个用户，但passport没有这个用户
                    {
                        SetUserIDPageCache(username, 0, UserLoginType.Email);

                        return User.Guest;//同样返回游客
                    }
                }

                //needProcessKeyword = true;
                AddUserToCache(user, pageCacheOnly);
                if (data.FriendVersion != user.FirendVersion)
                {
                    FriendBO.Instance.Client_SyncAllFriend(user);
                }
                //同步一些数据
                Client_MappingUserFields(user, data);

            }
            else
#endif
            {
                //不存在这个用户
                if (user == null || user.UserID <= 0)
                {
                    //把当前用户名对应的UserID是0这一信息放入页面级缓存，以在下次调用本方法时可以迅速判断
                    SetUserIDPageCache(username, 0, UserLoginType.Username);
                    return User.Guest;
                }


                AddUserToCache(user, pageCacheOnly);
                SetUserIDPageCache(username, user.UserID, UserLoginType.Username);
                //needProcessKeyword = true;

            }
            //===================

            //if (needProcessKeyword)
            ProcessKeyword(user, ProcessKeywordMode.TryUpdateKeyword);
            return user;
        }

#if !Passport
        /// <summary>
        /// 映射用户属性
        /// </summary>
        /// <param name="localUser"></param>
        /// <param name="passportUser"></param>
        private void MappingPassportUser(AuthUser localUser, MaxLabs.bbsMax.PassportServerInterface.DataForLogin passportUser)
        {
            //同步用户信息
            localUser.UserID = passportUser.UserID;
            localUser.Username = passportUser.Username;
            localUser.Password = passportUser.Password;
            localUser.PasswordFormat = (EncryptFormat)passportUser.PasswordFormat;
            localUser.Email = passportUser.Email;
            localUser.UnreadMessages = passportUser.UnreadMessages;
            localUser.Gender = (MaxLabs.bbsMax.Enums.Gender)passportUser.Gender;
            localUser.MobilePhone = passportUser.MobilePhone;
            localUser.Realname = passportUser.Realname;

            UnreadNotifies unreadnotifies = new UnreadNotifies();


            foreach (MaxLabs.bbsMax.PassportServerInterface.UnreadNotifyItemProxy item in passportUser.UnreadNotifies.Items)
                unreadnotifies[item.TypeID] = item.Count;

            localUser.UnreadNotify = unreadnotifies;
        }

        /// <summary>
        /// 映射数据库记录
        /// (返回的bool值并不是表示是否创建了这个用户，
        /// 而是表示本次是否真的尝试映射用户操作，无论成功与否。
        /// 一般来说每个页面的第一次调用会返回true，以后调用false)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        private void MappingDatabase(int userID)
        {
            string key = "User/Map/" + userID;

            if (PageCacheUtil.Contains(key) == false)
            {
                PageCacheUtil.Set(key, true);

                MaxLabs.bbsMax.PassportServerInterface.DataForNewUser passportUserData = null;

                try
                {
                    passportUserData = Globals.PassportClient.PassportService.User_GetDataForNewUser(userID, true);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return;
                }

                if (passportUserData == null)
                    return;// null;

                //AuthUser user;
                //通行证中存在这个用户那么自动注册
                Client_Register(passportUserData);

            }

            //user = UserDao.Instance.GetAuthUser(userID);
            //return user;
        }
#endif


        public string GetAuthCookie()
        {
            HttpCookie userCookie = CookieUtil.Get(cookieKey_User);

            if (userCookie == null)//游客
                return string.Empty;

            return userCookie.Value;
        }

        public int GetCurrentUserID()
        {
            if (HttpContext.Current == null)
                return Consts.GuestID;

            if (HttpContext.Current.Items.Contains(cacheKey_CurrentUserID))
                return (int)HttpContext.Current.Items[cacheKey_CurrentUserID];

            HttpCookie userCookie = CookieUtil.Get(cookieKey_User);

            if (userCookie == null)
                return 0;

            if (string.IsNullOrEmpty(userCookie.Value))
                return 0;

            int userID = GetUserID(userCookie.Value, false);

            HttpContext.Current.Items[cacheKey_CurrentUserID] = userID;

            return userID;
        }

        public int GetUserID(string ticket, bool setAsPageCache)
        {
            int userID;
            if (string.IsNullOrEmpty(ticket))
            {
                userID = 0;
            }
            else
            {
                KeyValuePair<int, string> userIDandPassword = DecodeCookie(ticket);
                userID = userIDandPassword.Key;

                if (userID != 0)
                {
                    string password = userIDandPassword.Value;

                    AuthUser user = GetAuthUser(userID);

                    if (user == null || !user.IsActive || string.Compare(user.Password, password, true) != 0)
                        userID = 0;
                }

                if (setAsPageCache)
                    HttpContext.Current.Items[cacheKey_CurrentUserID] = userID;
            }


            return userID;
        }

        #region 加密,解密cookie
        /// <summary>
        /// 加密用户ID,用户密码,cookie密钥,生成cookie
        /// </summary>
        public string EncodeCookie(int userID, string password)
        {
            return SecurityUtil.DesEncode(string.Concat(userID.ToString(), "|", password));
        }

        /// <summary>
        /// 解密cookie,返回用户ID和加密后的密码信息
        /// </summary>
        public KeyValuePair<int, string> DecodeCookie(string cookie)
        {
            int userID;
            string password = null;
            try
            {
                string userKey = SecurityUtil.DesDecode(cookie);
                int decollatorIndex = userKey.IndexOf('|');
                //有cookie记录的游客
                if (decollatorIndex == -1)
                {
                    userID = 0;
                }
                else
                {
                    //已登陆用户
                    string userIDString = userKey.Substring(0, decollatorIndex);
                    try
                    {
                        userID = int.Parse(userIDString);

                        if (userID > 0)
                            password = userKey.Substring(decollatorIndex + 1);

                        else if (userID < 0)
                            userID = 0;
                    }
                    catch
                    {
                        userID = 0;
                    }
                }
            }
            catch
            {
                userID = 0;
            }

            if (password == null)
                password = string.Empty;

            return new KeyValuePair<int, string>(userID, password);
        }
        #endregion

        #endregion

        #region 管理员控制台登陆相关

        public ConsoleLoginLogCollection GetConsoleLoginLogs(int count)
        {
            return UserDao.Instance.GetConsoleLoginLogs(count);
        }

        public void AdminLogout(int userID)
        {
            UserDao.Instance.AdminLogout(userID);
        }

        public bool HasAdminSession(int userID, out Guid sessionID, string Ip)
        {

            return UserDao.Instance.HasAdminSession(userID, AdminSessionExpiresMinute, Ip, out  sessionID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        [Obsolete]
        public bool AdminLogin(string username, string password, out Guid sessionID)
        {
            SimpleUser user = GetUser(username);
            if (user != null)
                return AdminLogin(user.UserID, password, out sessionID);
            else
                return AdminLogin(0, password, out sessionID);
        }


        public bool AdminLogin(int userID, string password, out Guid sessionID)
        {
            sessionID = Guid.Empty;

            AuthUser user = GetAuthUser(userID);

            if (user == null || user == User.Guest)
                return false;

            if (!user.CanLoginConsole)
                return false;

            if (GetCurrentUserID() != user.UserID)
                SetUserLogin(user, password, user.Password, false);

            //加密后结果和数据库结果对比 //TODO:密码错误日志
            if (!SecurityUtil.ComparePassword(user.PasswordFormat, password, user.Password))
            {
                return false;
            }

            string ip = IPUtil.GetCurrentIP();

            sessionID = UserDao.Instance.CreateAdminSession(user.UserID, ip);

            Logs.LogManager.LogOperation(new Logs.AdminLoginOperation(user.UserID, user.Username, ip));

            return true;
        }

        #endregion

        #region 缓存操作

        /// <summary>
        /// 根据username尝试从页面级缓存中获得userID，如果缓存不存在，将返回-1
        /// </summary>
        private int GetUserIDFromPageCache(string username, UserLoginType loginType)
        {
            string prefix;
            switch (loginType)
            {
                case UserLoginType.Email:
                    prefix = "email_";
                    break;
                case UserLoginType.Username:
                    prefix = "username_";
                    break;
                default:
                    prefix = string.Empty;
                    break;
            }

            string key = string.Concat(cacheKey_UserID_StartAs, prefix, username);

            int userID;

            if (PageCacheUtil.TryGetValue<int>(key, out userID) == false)
                userID = -1;

            return userID;
        }

        private void SetUserIDPageCache(string username, int mapToUserID, UserLoginType loginType)
        {
            string prefix;
            switch (loginType)
            {
                case UserLoginType.Email:
                    prefix = "email_";
                    break;
                case UserLoginType.Username:
                    prefix = "username_";
                    break;
                default:
                    prefix = string.Empty;
                    break;
            }

            string key = string.Concat(cacheKey_UserID_StartAs, prefix, username);

            PageCacheUtil.Set(key, mapToUserID);
        }


        public void RemoveAllUserCache()
        {
            CacheUtil.RemoveBySearch("User/");
        }

        public void AddSimpleUserToCache(SimpleUser simpleUser)
        {
            if (simpleUser != null)// && simpleUser.UserID > 0)
            {
                //if (ContainsUserCache(simpleUser.UserID, true) == false)
                //{
                string key = cacheKey_User_StartAs + simpleUser.UserID;

                CacheUtil.Set<SimpleUser>(key, simpleUser, CacheTime.Short);
                PageCacheUtil.Set(key, simpleUser);
                //}
            }
        }

        //private void OnUserCacheRemove(string key, object cacheItem, System.Web.Caching.CacheItemRemovedReason reason)
        //{
        //    User user = cacheItem as User;

        //    if (user != null)
        //    {
        //        if (user.IsOnline)
        //        {
        //            CacheUtil.Set<User>(key, user, CacheTime.Default, CacheExpiresType.Sliding, null, System.Web.Caching.CacheItemPriority.NotRemovable, OnUserCacheRemove);
        //        }
        //    }
        //}

        public void AddUserToCache(User user)
        {
            AddUserToCache(user, false);
        }

        public void AddUserToCache(User user, bool pageCacheOnly)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items["max_test2"] == null)
                    HttpContext.Current.Items["max_test2"] = user.ToString() + "|" + user.UserID + "|" + pageCacheOnly;
                else
                    HttpContext.Current.Items["max_test2"] += "{***}" + user.ToString() + "|" + user.UserID + "|" + pageCacheOnly;

            }

            if (user != null)// && user.UserID > 0)
            {
                //if (ContainsUserCache(user.UserID, false) == false)
                //{
                string key = cacheKey_User_StartAs + user.UserID;

                //CacheUtil.Set<User>(key, user, cacheTime, CacheExpiresType.Sliding, null, System.Web.Caching.CacheItemPriority.NotRemovable, OnUserCacheRemove);
                if (pageCacheOnly == false || CacheUtil.Contains(key))
                {
                    if (HttpContext.Current != null)
                    {
                        if (HttpContext.Current.Items["max_test3"] == null)
                            HttpContext.Current.Items["max_test3"] = user.ToString() + "|" + user.UserID + "|" + pageCacheOnly;
                        else
                            HttpContext.Current.Items["max_test3"] += "{***}" + user.ToString() + "|" + user.UserID + "|" + pageCacheOnly;

                    }

                    CacheUtil.Set<User>(key, user);
                }

                PageCacheUtil.Set(key, user);
                //}
            }
        }

        /// <summary>
        /// 移除用户缓存
        /// </summary>
        public void RemoveUserCache(int userID)
        {
            string key = cacheKey_User_StartAs + userID;
            PageCacheUtil.Remove(key);
            CacheUtil.Remove(key);
        }

        /// <summary>
        /// 移除一组用户缓存
        /// </summary>
        /// <param name="userIds"></param>
        public void RemoveUsersCache(IEnumerable<int> userIds)
        {
            foreach (int userID in userIds)
                RemoveUserCache(userID);
        }

        /// <summary>
        /// 检查指定的用户ID是否已经缓存
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool ContainsUserCache(int userID, bool includeSimpleUserCache)
        {
            string key = cacheKey_User_StartAs + userID;

            if (includeSimpleUserCache)
                return CacheUtil.Contains<SimpleUser>(key);

            return CacheUtil.Contains<User>(key);
        }


        /// <summary>
        /// 从缓存中取出指定的用户实例。如果不存在，将返回null
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public T GetUserFromCache<T>(int userID) where T : SimpleUser
        {
            T user = null;
            string key = cacheKey_User_StartAs + userID;

            if (!PageCacheUtil.TryGetValue<T>(key, out user)) //HttpContext.Current.Items
            {
                if (CacheUtil.TryGetValue<T>(key, out user)) //Cache
                {
                    PageCacheUtil.Set(key, user);
                }
            }

            //if (user is User)
            //    ProcessKeyword(user as User, ProcessKeywordMode.TryUpdateKeyword);
            return user;
        }

        /// <summary>
        /// 从缓存中取出指定的用户实例。如果不存在，将返回null
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="createdPageCache">本次操作是否创建了新的页面级缓存</param>
        /// <returns></returns>
        public T GetUserFromCache<T>(int userID, out bool createdPageCache) where T : SimpleUser
        {
            T user = null;
            string key = cacheKey_User_StartAs + userID;

            if (!PageCacheUtil.TryGetValue<T>(key, out user)) //HttpContext.Current.Items
            {
                if (CacheUtil.TryGetValue<T>(key, out user)) //Cache
                {
                    PageCacheUtil.Set(key, user);
                    createdPageCache = true;
                }
                else
                    createdPageCache = false;
            }
            else
                createdPageCache = false;

            //if (user is User)
            //    ProcessKeyword(user as User, ProcessKeywordMode.TryUpdateKeyword);
            return user;
        }


        /// <summary>
        /// 从缓存中取出指定的用户实例。如果不存在，将返回null
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User GetUserFromCache(int userID)
        {
            User user = null;
            string key = cacheKey_User_StartAs + userID;

            if (!PageCacheUtil.TryGetValue<User>(key, out user)) //HttpContext.Current.Items
            {
                if (CacheUtil.TryGetValue<User>(key, out user)) //Cache
                {
                    PageCacheUtil.Set(key, user);
                }
            }

            //ProcessKeyword(user, ProcessKeywordMode.TryUpdateKeyword);
            return user;
        }

        /// <summary>
        /// 从缓存中取出指定的简单用户实例。如果不存在，将返回null
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public SimpleUser GetSimpleUserFromCache(int userID)
        {
            SimpleUser user = null;

            string key = cacheKey_User_StartAs + userID;

            if (PageCacheUtil.TryGetValue<SimpleUser>(key, out user) == false)
            {
                if (CacheUtil.TryGetValue<SimpleUser>(key, out user))
                {
                    PageCacheUtil.Set(key, user);
                }
            }

            return user;
        }

        #endregion

        #region 通过邮箱激活用户账号

        private bool SendActiveEmail(int userID, string username, string email)
        {
            //UserID必须大于0
            if (userID <= 0)
                return false;

            //以下情况不能重发激活邮件
            if (string.IsNullOrEmpty(email)) //没有填写Email
                return false;

            bool success = false;

            MaxSerial serial = SerialBO.Instance.CreateSerial(userID, DateTimeUtil.Now.AddDays(1), SerialType.ActivingAccount, null, out success);

            if (success == false)
            {
                ThrowError(new SendEmailTimesLimitError());
                return false;
            }

            try
            {
                ActiveEmail emailToSend = new ActiveEmail(email, username, serial.Serial.ToString());
                emailToSend.Send();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重新发送帐号激活邮件
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool TryActiveUser(int userID, string code)
        {
            AuthUser user = GetAuthUser(userID);

            if (user ==User.Guest)
            {
                ThrowError(new UserNotExistsError("userid", userID));
                return false;
            }
            if (user.IsActive)
            {
                ThrowError(new CustomError( "用户已经激活。"));
                return false;
            }
            if (code != SecurityUtil.SHA1(user.Password + Globals.SafeString))
            {
                ThrowError(new CustomError("code", "验证码错误"));
                return false;
            }

            return SendActiveEmail(userID, user.Username, user.Email);
        }

        /// <summary>
        /// 激活用户
        /// </summary>
        /// <param name="activeSerial">激活码</param>
        /// <returns></returns>
        public bool ActivingUser(string activeSerial)
        {
            if (AllSettings.Current.RegisterSettings.EmailVerifyMode != EmailVerifyMode.Required)
            {
                ThrowError(new CustomError("activeSerial", "系统未开启账号激活功能"));
                return false;
            }

            Guid serialGuid;

            try
            {
                serialGuid = new Guid(activeSerial);
            }
            catch
            {
                ThrowError(new InvalidActiveCodeError("activeSerial", activeSerial));
                return false;
            }

            int userID;
            int result = UserDao.Instance.ActivingUsers(serialGuid, out userID);

            switch (result)
            {

                case 1: //激活成功
                    UserBO.Instance.RemoveUserCache(userID);
                    AuthUser user = GetAuthUser(userID);
                    SetUserLogin(user, null, user.Password, false);

                    if (OnUserCreated != null)
                    {
                        OnUserCreated(user);
                    }

                    return true;

                case 2:
                    ThrowError(new InvalidActiveCodeError("activeSerial", activeSerial));
                    return false;

                case 3:
                    ThrowError(new CustomError("activeSerial", "该激活码要激活的用户在之前就已经激活，无需重复激活"));
                    return false;

                default:
                    ThrowError(new UnknownError());
                    return false;
            }
        }


        #endregion

        #region  邮箱验证功能

        public bool SendValidateEmail(int userID, string username, string email)
        {
            if (userID <= 0)
                return false;

            //以下情况不能重发激活邮件
            if (string.IsNullOrEmpty(email)) //没有填写Email
                return false;

            bool success = false;
            MaxSerial serial = SerialBO.Instance.CreateSerial(userID, DateTimeUtil.Now.AddDays(1), SerialType.ValidateEmail, email, out success);
            if (success == false)
            {
                ThrowError(new SendEmailTimesLimitError());
                return false;
            }

            try
            {
                EmailBase emailToSend = new EmailValidateEmail(email, username, serial.Serial.ToString());
                emailToSend.Send();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool TryValidateEmail(int userID, string code)
        {
            if (userID <= 0)
                return false;

            AuthUser user = GetAuthUser(userID, true);

            if (user == User.Guest ||
                user.EmailValidated ||
                user.Email == string.Empty ||
                code != SecurityUtil.SHA1(user.Password + Globals.SafeString)
                )
                return false;

            return SendValidateEmail(userID, user.Username, user.Email);
        }

        /// <summary>
        /// 重新验证我的邮箱
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="email">email</param>
        public void TryChangeEmail(AuthUser operatorUser, string password, string email)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
            }
            if (string.IsNullOrEmpty(email))
            {
                ThrowError(new EmptyEmailError("email"));
            }

            if (HasUnCatchedError) return;

            //User user = GetUser(userID);
            if (!SecurityUtil.ComparePassword(operatorUser.PasswordFormat, password, operatorUser.Password))
            {
                ThrowError(new PasswordError("password"));
                return;
            }

            //Regex regex = Pool<EmailRegex>.Instance;

            if (ValidateUtil.IsEmail(email) == false)
            {
                ThrowError(new EmailFormatError("email", email));
                return;
            }

            if (operatorUser.Email.Trim().Equals(email.Trim(), StringComparison.OrdinalIgnoreCase) == false)
            {
                ValidateEmail(email, "email");
            }

            if (HasUnCatchedError)
                return;

#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                APIResult result = null;
                
                try
                {
                    result = Globals.PassportClient.PassportService.User_SendEmailValidateCode(operatorUser.UserID, email);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                }

                if (result != null)
                {
                    if (result.IsSuccess == false)
                    {
                        for (int i = 0; i < result.Messages.Length; i++)
                        {
                            ThrowError(new CustomError(result.ErrorTargets[i], result.Messages[i]));
                        }
                    }
                }
            }
            else
#endif
            {
                SendValidateEmail(operatorUser.UserID, operatorUser.Username, email);
            }
        }

        /// <summary>
        /// 根据邮箱验证码重设用户邮箱
        /// </summary>
        /// <param name="ValidateCode"></param>
        /// <returns></returns>
        public bool ResetEmailByValidateCode(string validateCode)
        {
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                bool success = false;

                try
                {
                  success = Globals.PassportClient.PassportService.User_ResetEmailByValidateCode(validateCode);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                }
                return success;
            }
            else
#endif
            {
                int userID;
                string email = "";
                MaxSerial serial = SerialBO.Instance.GetSerial(validateCode, SerialType.ValidateEmail);
                if (serial == null)
                {
                    return false;
                }

                email = serial.Data;
                userID = serial.OwnerUserId;

                if (userID > 0)
                {
                    User user = GetUser(userID);

                    UserDao.Instance.ValidateUserEmail(userID, email);

                    //如果用户没有认证过邮箱的话就加分
                    if (!user.EmailValidated)
                        PointManager.UpdateUserPoint(userID, UserPoints.ValidateEmail, null);

                    RemoveUserFromRole(userID, Role.EmailNotProvedUsers.RoleID); //从邮箱未验证用户组移除
                    SerialBO.Instance.DeleteSerial(userID, SerialType.ValidateEmail);

                    if (OnUserEmailChanged != null)
                        OnUserEmailChanged(user.UserID, email);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 获得指定用户的“尚未通过验证”的email地址
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetUnValidatedEmail(int userID)
        {
            return UserDao.Instance.GetUnValidatedEmail(userID);
        }

        #endregion

        #region  邮件发送

        /// <summary>
        /// 群发邀请邮件
        /// </summary>
        /// <param name="addresses">地址数组</param>
        /// <param name="inviteCode">邀请码</param>
        /// <param name="message">附加信息</param>
        /// <param name="userName">用户名</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        public bool MassEmailingInvite(AuthUser operatorUser, string[] addresses, string inviteCode, string message)
        {

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (message != null)
            {
                if (message.Length > 140)
                {
                    ThrowError(new CustomError("message", "您输入的附言太长，请不要超过140个字符"));
                    return false;
                }
            }

            //StringBuffer errorAddressString = new StringBuffer();
            //message = StringUtil.HtmlEncode(message);

            foreach (string address in addresses)
            {
                InviteEmail inviteEmail = new InviteEmail(address, message, inviteCode
                    , operatorUser.Username, operatorUser.UserID);

                inviteEmail.Send();
            }
            return true;
            //if (errorAddressString.InnerBuilder.Length == 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    ThrowError(new MassEmailingError(new string[] { errorAddressString.ToString() }));
            //    return false;
            //}
        }

        ///// <summary>
        ///// 群发邮件
        ///// </summary>
        ///// <param name="targetUserIds"></param>
        ///// <returns></returns>
        //public void MassEmailing(int operatorUserIDs, int[] targetUserIds, EmailBase EmailSender)
        //{
        //    if (operatorUserIDs > 0)
        //    {
        //        UserCollection Users = GetUsers(targetUserIds);
        //        foreach (User user in Users)
        //        {

        //            EmailSender.Send(user.Email);
        //        }
        //    }
        //    else
        //    {
        //        ThrowError(new NotLoginError());
        //    }

        //}


        #endregion

        #region 用户注册

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="email">Email</param>
        /// <param name="ip">IP</param>
        /// <param name="serial">邀请码</param>
        /// <param name="inviterUserID">邀请者ID</param>
        /// <returns></returns>
        public UserRegisterState Register(string username, string password, string email, string ip, string serial, int? inviterUserID)
        {
            int userID = 0;
            return Register(ref userID, username, password, email, ip, serial, null, true);
        }

        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="userID">指定用户ID</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="email">Email</param>
        /// <param name="ip">IP</param>
        /// <param name="serial">邀请码</param>
        /// <param name="sendSuccessEmail">注册成功是否发送欢迎邮件（为空表示取系统设置）</param>
        /// <returns></returns>
        public UserRegisterState Register(ref int userID, string username, string password, string email, string ip, string serial, bool? sendSuccessEmail, bool isRightValidateCode)
        {
            User inviteUser = null;//推荐人
            ErrorInfo inviteCodeError;
            Guid serialValue = Guid.Empty;
            //bool addToUninviteRole=false;
            Guid addRoleForInvite = Guid.Empty;

            int[] defaultPoints = new int[8];
            string truePassword = password;
            //相同IP地址注册时间间隔
            int SameIPInterval = 0;
            username = username.Trim();
            ValidateUsername(username, "username");
            if (string.IsNullOrEmpty(password))
                Context.ThrowError(new EmptyPasswordError("password"));
            ValidateEmail(email, "email");

            string tempPassword = password;

            EncryptFormat passwordFormat = EncryptFormat.bbsMax;
            password = SecurityUtil.Encrypt(passwordFormat, password);
            RegisterSettings settings = AllSettings.Current.RegisterSettings;


            //检查后台是否关闭用户注册
            if (settings.EnableRegister == RegisterMode.Closed ||
                (settings.EnableRegister == RegisterMode.TimingClosed && settings.ScopeList.CompareDateTime(DateTimeUtil.Now)))
            {
                ThrowError(new RegisterDisableError(settings.ClosedMessage));
                return UserRegisterState.Failure;
            }

            //IP地址检查
            CanRegister(ip);
            //相同IP地址注册时间间隔检查
            SameIPInterval = AllSettings.Current.RegisterLimitSettings.TimeSpanForContinuousRegister;


            InvitationSettings invitationSettings = AllSettings.Current.InvitationSettings;
            //邀请码检查
            if (invitationSettings.InviteMode != InviteMode.Close)
            {
                inviteCodeError = InviteBO.Instance.ValidateInvideCode(serial, out inviteUser);
                if (inviteCodeError != null)//邀请码检查
                {
                    if (invitationSettings.InviteMode == InviteMode.InviteLinkRequire
                        || invitationSettings.InviteMode == InviteMode.InviteSerialRequire
                        || string.IsNullOrEmpty(serial) == false)//如果是必须得到邀请的话，或者邀请码有输入
                    {
                        ThrowError(inviteCodeError);
                    }
                    else
                    {
                        IgnoreError<EmptyInviteSerialError>();
                    }
                }
                if (inviteUser != null)
                {
                    try
                    {
                        serialValue = new Guid(serial);
                    }
                    catch
                    {

                    }
                    addRoleForInvite = invitationSettings.AddToUserRoleWhenHasInvite;
                }
                else// if(AllSettings.Current.InvitationSettings.InviteMode== InviteMode.Limit)//开启未受邀请功能限制模式
                {
                    //addToUninviteRole = true; //是否加到未邀请用户组
                    addRoleForInvite = invitationSettings.AddToUserRoleWhenNoInvite;
                }
            }

            //只要上面的这些检查有一个无法通过，都不再继续执行
            if (Context.Current.Errors.HasUnCatchedError)
                return UserRegisterState.Failure;

            if(isRightValidateCode == false)
                return UserRegisterState.Failure;

            UserRegisterErrorCode result = UserRegisterErrorCode.Unknown;

            UserPointCollection points = AllSettings.Current.PointSettings.UserPoints;

            for (int i = 0; i < 8; i++)
            {
                defaultPoints[i] = points[i].Enable ? points[i].InitialValue : 0;
            }

            #region  初始用户组

            UserRoleCollection roles;

            roles = new UserRoleCollection();
            UserRole userRole;
            if (settings.NewUserPracticeTime > 0)//见习用户
            {
                userRole = new UserRole();
                userRole.RoleID = Role.NewUsers.RoleID;
                userRole.BeginDate = DateTime.MinValue;
                userRole.EndDate = DateTimeUtil.Now.AddMinutes(settings.NewUserPracticeTime);
                roles.Add(userRole);
            }

            if (addRoleForInvite != Guid.Empty) //未受邀请用户
            {
                Role role = AllSettings.Current.RoleSettings.GetRole(addRoleForInvite);

                //只能加入普通用户组和非虚拟的用户组
                if (role != null && role.IsNormal && role.IsVirtualRole == false)
                {
                    userRole = new UserRole();
                    userRole.RoleID = addRoleForInvite;
                    roles.Add(userRole);
                }
            }

            if (AllSettings.Current.RegisterSettings.EmailVerifyMode != EmailVerifyMode.Disabled)//Email未验证用户
            {
                userRole = new UserRole();
                userRole.RoleID = Role.EmailNotProvedUsers.RoleID;
                roles.Add(userRole);
            }

            // AddUsersToRoles(roles);

            #endregion

            //邀请人不为空， 尝试更新邀请人积分
            if (inviteUser != null)
            {
                int newUserId = userID;
                if (InvitePointAction.Instance.UpdateUserPoint(inviteUser.UserID, InvitePointType.InviteNewUser
                    , delegate(PointActionManager.TryUpdateUserPointState state)
                    {
                        if (state == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                        {
                            result = (UserRegisterErrorCode)UserDao.Instance.Register(ref newUserId, username, email, password
                                , passwordFormat, roles, ip, serialValue, inviteUser.UserID
                                , settings.EmailVerifyMode != EmailVerifyMode.Required, defaultPoints, SameIPInterval);
                        }
                        else
                        {
                            result = UserRegisterErrorCode.InviteUserPointShort;
                        }
                        return result == UserRegisterErrorCode.Success;
                    }))
                {
                    result = UserRegisterErrorCode.Success;
                }

                userID = newUserId;
            }
            else
            {
                result = (UserRegisterErrorCode)UserDao.Instance.Register(ref userID, username, email, password
                    , passwordFormat, roles, ip, serialValue, 0
                    , settings.EmailVerifyMode != EmailVerifyMode.Required, defaultPoints, SameIPInterval);
            }
            switch (result)
            {
                case UserRegisterErrorCode.Success:
                    //发送欢迎Email ,如果邮箱不是必须验证的情况下

                    //如果需要验证email才能注册，则发送验证email
                    if (settings.EmailVerifyMode == EmailVerifyMode.Required)
                    {
                        SendActiveEmail(userID, username, email);
                        return UserRegisterState.NeedActive;
                    }
                    else if (settings.EnableWelcomeMail)
                    {
                        try
                        {
                            EmailBase emailToSend = new WelcomeEmail(email, username, truePassword);
                            emailToSend.Send();
                        }
                        catch { }
                    }

#if !Passport
                    FeedBO.Instance.CreateOpenSpaceFeed(userID);//动态
#endif
                    AuthUser user = GetAuthUser(userID);
                    SetUserLogin(user, tempPassword, password, false);//用户注册成功就设置成已经登陆的

                    if (AllSettings.Current.RegisterSettings.EmailVerifyMode != EmailVerifyMode.Required)
                    {
                        if (OnUserCreated != null)
                        {
                            OnUserCreated(user);
                        }
                    }

                    return UserRegisterState.Success;

                case UserRegisterErrorCode.UsernameExsits:
                    ThrowError(new DuplicateUsernameError("username", username));
                    break;
                case UserRegisterErrorCode.EmailExists:
                    ThrowError(new DuplicateEmailError("email", email));
                    break;
                case UserRegisterErrorCode.InvitationCodeError:
                    ThrowError(new InviteSerialError("serial", serial));
                    break;
                case UserRegisterErrorCode.IdExists:
                    ThrowError(new DuplicateUserIDError("userID", userID));
                    break;
                case UserRegisterErrorCode.RegisterFrequent:
                    ThrowError(new RegisterFrequentError("ip"));
                    break;
                case UserRegisterErrorCode.InviteUserPointShort:
                    ThrowError(new InviterPointShortError(string.IsNullOrEmpty(inviteUser.Realname) ? inviteUser.Username : inviteUser.Realname));
                    break;
                default:
                    ThrowError(new UnknownError());
                    break;
            }
            return UserRegisterState.Failure;
        }

        /// <summary>
        /// 管理员创建用户
        /// </summary>
        public bool AdminCreateUser(string username, string password, string email, string ip, ref int userID)
        {
            if (username != null)
                username = username.Trim();

            bool dataError = false;
            if (string.IsNullOrEmpty(username))
            { ThrowError(new EmptyUsernameError("username")); dataError = true; }
            if (string.IsNullOrEmpty(password))
            { ThrowError(new EmptyPasswordError("password")); dataError = true; }
            if (string.IsNullOrEmpty(email))
            { ThrowError(new EmptyEmailError("email")); dataError = true; }

            if (dataError) return false;

            ValidateUsername(username, "username");

            if (HasUnCatchedError)
                return false;

            EncryptFormat passwordFormat = EncryptFormat.bbsMax;

            string tempPassword = password;
            password = SecurityUtil.Encrypt(passwordFormat, password);
            bool nameChecked = (AllSettings.Current.NameCheckSettings.EnableRealnameCheck == false);

            int[] defaultPoints = new int[8];
            UserPointCollection points = AllSettings.Current.PointSettings.UserPoints;
            for (int i = 0; i < 8; i++)
            {
                defaultPoints[i] = points[i].Enable ? points[i].InitialValue : 0;
            }
            UserRegisterErrorCode result = UserRegisterErrorCode.Unknown;
            result = (UserRegisterErrorCode)UserDao.Instance.Register(ref userID, username, email, password, passwordFormat, new UserRoleCollection(), ip, null, 0, true, defaultPoints, 0);

            if (result == UserRegisterErrorCode.Success)
            {
                //    try
                //    {
                //        UpdateUserGeneralPoint(userID);
                //    }
                //    catch { }
                if (OnUserCreated != null)
                {
                    AuthUser user = GetAuthUser(userID);
                    OnUserCreated(user);
                }

                return true;
            }
            switch (result)
            {
                case UserRegisterErrorCode.UsernameExsits:
                    ThrowError(new DuplicateUsernameError("username", username));
                    break;
                case UserRegisterErrorCode.EmailExists:
                    ThrowError(new DuplicateEmailError("email", email));
                    break;
                case UserRegisterErrorCode.IdExists:
                    ThrowError(new DuplicateUserIDError("userID", userID));
                    break;
            }

            return false;

            // return UserDao.Instance.Register (username, email, password, passwordFormat, ip, null, 0, nameChecked, ref userID);
        }


        #endregion

        #region 修改用户资料和数据

        /// <summary>
        /// 更改用户名
        /// </summary>
        /// <param name="UserID">用户名</param>
        /// <param name="username">用户新用户名</param>
        public bool AdminTryUpdateUsername(AuthUser operatorUser, int targetUserID, string username)
        {
            return TryUpdateUsername(operatorUser, targetUserID, username, false);
        }

        public bool PropUpdateUserSignature(AuthUser user, DateTime expiresDate)
        {
            if (user == null|| user==User.Guest)
                return false;

            user .SignaturePropFlag .ExpiresDate = expiresDate;
            return UserDao.Instance.UpdateUserSignature(user.UserID,user.SignaturePropFlag.GetStringForSave());
        }

        /// <summary>
        /// 更改用户名
        /// </summary>
        /// <param name="UserID">用户名</param>
        /// <param name="username">用户新用户名</param>
        public bool TryUpdateUsername(AuthUser operatorUser, int targetUserID, string username, bool ignorePermission)
        {
            username = username.Trim();

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            User user = GetUser(targetUserID);

            if (user == null)
            {
                ThrowError(new UserNotExistsError("targetUserID", targetUserID));
                return false;
            }

            if (!ignorePermission)
            {
                if (!CanEditUserProfile(operatorUser, targetUserID))
                {
                    ThrowError(new NoPermissionEditUserProfileError());
                    return false;
                }
            }

            if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                return true;

            if (ValidateUsername(username, "username"))
            {
                UserDao.Instance.AdminUpdateUsername(targetUserID, username);
                RemoveUserCache(targetUserID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 激活账号
        /// </summary>
        /// <param name="targetUserIds"></param>
        /// <param name="isActive">是否</param>
        public void AdminActivingUsers(AuthUser operatorUser, int[] targetUserIds, bool isActive)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }

            if (CanEditUserProfile(operatorUser, targetUserIds[0]))
            {
                if (targetUserIds.Length > 0)
                {
                    //UserDao.Instance.ActivingUsers(targetUserIds, isActive);
                }
            }
            else
            {

            }
        }


        /// <summary>
        /// 管理员修改用户信息（用户基本信息）
        /// </summary>
        /// <param name="userinfo">用户</param>
        /// <returns></returns>
        public bool AdminUpdateUserProfile(AuthUser operatorUser
            , int targetUserId
            , string realname
            , string email
            ,bbsMax.Enums.Gender gender
            , DateTime birthday
            , string signature
            , bool realnameChecked
            , bool isActive
            , bool emailValidated
            //, bool clearAvatarProp
            //, bool clearSignatureProp
            , UserExtendedValueCollection extendedFields
            )
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            AuthUser targetUser = GetAuthUser(targetUserId, true);
            if (targetUser == null || targetUser == User.Guest)
            {
                ThrowError(new UserNotExistsError("targetUserID", targetUserId));
                return false;
            }


            if (!CanEditUserProfile(operatorUser, targetUserId))
            {
                ThrowError(new NoPermissionEditUserProfileError());
                return false;
            }
            if (email == null)
                email = string.Empty;

            email = email.Trim();

            if (!targetUser.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                ValidateEmail(email, "email");
            }

            string signatureOrigal = signature;

            SignatureFormat signatureFormat = GetSignatureFormat(operatorUser);


            #region 处理签名
            switch (signatureFormat)
            {
                case SignatureFormat.Ubb:
                    signature = MaxLabs.bbsMax.Ubb.SignatureParser.ParseForSave(operatorUser.UserID, signature);
                    break;
                case SignatureFormat.Text:
                    UserBO.UserSignatureTagSettings UbbSetting = new UserSignatureTagSettings(operatorUser.UserID);
                    signature = HttpUtility.HtmlEncode(signature);
                    if (UbbSetting.AllowDefaultEmoticon || UbbSetting.AllowUserEmoticon)
                        signature = EmoticonParser.ParseToHtml(targetUserId, signature
                            , UbbSetting.AllowUserEmoticon, UbbSetting.AllowDefaultEmoticon);
                    break;
            }
            targetUser.SignaturePropFlag.OriginalData = signature;
            signature = targetUser.SignaturePropFlag.GetStringForSave();
            #endregion

            if (CanEditRole(operatorUser, targetUserId))
            {
                if (emailValidated)
                {
                    RemoveUserFromRole(targetUserId, Role.EmailNotProvedUsers);
                }
                else
                {
                    if (AllSettings.Current.RegisterSettings.EmailVerifyMode == EmailVerifyMode.Optional
                        || AllSettings.Current.RegisterSettings.EmailVerifyMode == EmailVerifyMode.Required)
                        AddUserToRole(targetUserId, Role.EmailNotProvedUsers);
                }
            }

            if (!HasUnCatchedError)
            {
                bool passportSuccess = true;
#if !Passport
                if (Globals.PassportClient.EnablePassport)
                {

                    //int i = 0;
                    List<MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy> temp = new List<MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy>();
                    foreach (UserExtendedValue value in extendedFields)
                    {
                        foreach (ExtendedField field in AllSettings.Current.ExtendedFieldSettings.PassportFields)
                        {
                            if (value.ExtendedFieldID == field.Key)
                            {
                                temp.Add(GetUserExtendedValueProxy(value));
                                break;
                            }
                        }
                    }

                    if (temp.Count > 0)
                    {
                        MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy[] fields = new MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy[temp.Count];
                        temp.CopyTo(fields);

                        APIResult apiResult = null;

                        try
                        {
                            apiResult = Globals.PassportClient.PassportService.User_AdminUpdateUserProFile(operatorUser.UserID
                                , targetUserId
                                , realname
                                , email
                                , (bbsMax.PassportServerInterface.Gender)gender
                                , birthday
                                , signatureOrigal
                                , realnameChecked
                                , isActive
                                , emailValidated
                                , fields);
                        }
                        catch (Exception ex)
                        {
                            ThrowError(new APIError(ex.Message));

                            return false;
                        }


                        if (apiResult != null)
                        {
                            if (apiResult.IsSuccess == false)
                            {
                                for (int j = 0; j < apiResult.Messages.Length; j++)
                                {
                                    ThrowError(new CustomError(apiResult.ErrorTargets[j], apiResult.Messages[j]));
                                }
                            }
                            passportSuccess = apiResult.IsSuccess;
                        }
                        else
                        {
                            passportSuccess = false;
                        }
                    }
                }
#endif
                if (passportSuccess)
                {

                    bool result = UserDao.Instance.AdminUpdateUserProfile(targetUserId, realname, email, gender, birthday, isActive, emailValidated, signature, signatureFormat, extendedFields);
                    if (result)
                    {
                        //User user = GetUserFromCache<SimpleUser>(targetUserId);
                        if (ContainsUserCache(targetUserId, true))
                        {
                            RemoveUserCache(targetUserId);    //更新缓存
                            //user = GetUser(targetUserId);     //更新缓存
                        }

                        targetUser = GetAuthUser(targetUserId);

                        if (OnUserProfileChanged != null) OnUserProfileChanged(targetUser);//调用事件处理函数
                    }
                    return result;

                }
                return passportSuccess;
            }
            else
                return false;

        }

        public void AdminUpdateUserinfo(AuthUser operatorUser, int targetUserID, DateTime regDate, int totalOnlineTime, int totalMonthOnlineTime)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }

            if (CanEditUserProfile(operatorUser, targetUserID))
            {
                UserDao.Instance.AdminUpdateUserinfo(targetUserID, regDate, totalOnlineTime, totalMonthOnlineTime);
                RemoveUserCache(targetUserID);
            }
            else
            {
                ThrowError(new NoPermissionEditUserProfileError());
            }
        }

        /// <summary>
        /// 填写邀请码， 用户后期
        /// </summary>
        /// <param name="inviteCode"></param>
        /// <returns></returns>
        public bool SetUserInviteCode(int operatorUserID, string inviteCode)
        {
            if (operatorUserID <= 0)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            User inviter;
            InviteBO.Instance.ValidateInvideCode(inviteCode, out inviter);
            if (inviter != null)
            {
                if (InvitePointAction.Instance.UpdateUserPoint(inviter.UserID, InvitePointType.InviteNewUser, delegate(PointActionManager.TryUpdateUserPointState status)
                {
                    if (status == PointActionManager.TryUpdateUserPointState.CheckSucceed)
                    {
                        Guid serial = Guid.Empty;

                        try
                        {
                            serial = new Guid(inviteCode);
                        }
                        catch
                        { }

                        InviteBO.Instance.SetUserInviteSerial(operatorUserID, inviter.UserID, serial);

                        return true;
                    }

                    return false;
                }))
                {
                    InvitationSettings invitationSettings = AllSettings.Current.InvitationSettings;

                    //已经填写了邀请码，自动加入“提供邀请码自动加入的用户组”
                    if (invitationSettings.AddToUserRoleWhenHasInvite != Guid.Empty)
                        AddUserToRole(operatorUserID, invitationSettings.AddToUserRoleWhenHasInvite);

                    //已经填写了邀请码，自动从“未提供邀请码自动加入的用户组”移除
                    if (invitationSettings.AddToUserRoleWhenNoInvite != Guid.Empty)
                        RemoveUserFromRole(operatorUserID, invitationSettings.AddToUserRoleWhenNoInvite);

                    //RemoveUserFromRole(operatorUserID, Role.InviteLessUsers,true);//移除未受邀请用户组 

                    //缓存更新
                    User user = GetUserFromCache(operatorUserID);
                    if (user != null)
                    {
                        user.InviterID = inviter.UserID;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool UpdateUserProfile(AuthUser operatorUser, MaxLabs.bbsMax.Enums.Gender gender, Int16 birthYear, short birthMonth, Int16 birthday, string signature, float timeZone, UserExtendedValueCollection extendedFields, SignatureFormat signatureFormat)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }
            bool result = false;

            ContentKeywordSettings keywords = AllSettings.Current.ContentKeywordSettings;

            string keyword = null;

            if (keywords.BannedKeywords.IsMatch(signature, out keyword))
            {
                ThrowError(new SignatureBannedKeywordsError("signature", keyword));
                return false;
            }

            #region 处理签名
            switch (signatureFormat)
            {
                case SignatureFormat.Ubb:
                    signature = MaxLabs.bbsMax.Ubb.SignatureParser.ParseForSave(operatorUser.UserID, signature);
                    break;
                case SignatureFormat.Text:
                    UserBO.UserSignatureTagSettings UbbSetting = new UserSignatureTagSettings(operatorUser.UserID);
                    signature = HttpUtility.HtmlEncode(signature);
                    if (UbbSetting.AllowDefaultEmoticon || UbbSetting.AllowUserEmoticon)
                        signature = EmoticonParser.ParseToHtml(operatorUser.UserID, signature
                            , UbbSetting.AllowUserEmoticon, UbbSetting.AllowDefaultEmoticon);
                    break;
            }


            operatorUser.SignaturePropFlag.OriginalData = signature;
            signature = operatorUser.SignaturePropFlag.GetStringForSave();

            #endregion

            signature = signature.Trim();

            if (signature.Length > MaxSignatureLength(operatorUser))
            {
                ThrowError(new SignatureLengthOverflowError("signature", MaxSignatureLength(operatorUser)));
                return false;
            }

            bool passportSuccess = true;
            if (!HasUnCatchedError)
            {
#if !Passport
                if (Globals.PassportClient.EnablePassport)
                {
                    List<MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy> temp = new List<MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy>();
                    foreach (UserExtendedValue value in extendedFields)
                    {
                        foreach (ExtendedField field in AllSettings.Current.ExtendedFieldSettings.PassportFields)
                        {
                            if (value.ExtendedFieldID == field.Key)
                            {
                                temp.Add(GetUserExtendedValueProxy(value));
                                break;
                            }
                        }
                    }

                    if (temp.Count > 0)
                    {
                        MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy[] fields = new MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy[temp.Count];
                        temp.CopyTo(fields);

                        APIResult apiResult = null;

                        try
                        {
                            apiResult = Globals.PassportClient.PassportService.User_UpdateUserProFile(operatorUser.UserID,
                                                                                    (bbsMax.PassportServerInterface.Gender)gender,
                                                                                                                        birthYear,
                                                                                                                        birthMonth,
                                                                                                                        birthday,
                                                                                                                        operatorUser.Signature,
                                                                                                                        timeZone,
                                                                                                                        fields);
                        }
                        catch (Exception ex)
                        {
                            ThrowError(new APIError(ex.Message));
                            return false;
                        }

                        if (apiResult != null)
                        {
                            if (apiResult.IsSuccess == false)
                            {
                                for (int j = 0; j < apiResult.Messages.Length; j++)
                                {
                                    ThrowError(new CustomError(apiResult.ErrorTargets[j], apiResult.Messages[j]));
                                }
                            }
                            passportSuccess = apiResult.IsSuccess;
                        }
                        else
                        {
                            passportSuccess = false;
                        }
                    }
                }
#endif
                if (passportSuccess)
                {
                    if (UserDao.Instance.UpdateUserProfile(operatorUser.UserID, gender, birthYear, birthMonth, birthday, signature, signatureFormat, timeZone, extendedFields))
                    {
                        RemoveUserCache(operatorUser.UserID);//移除缓存
#if !Passport
                        FeedBO.Instance.CreateUpdateUserProfileFeed(operatorUser.UserID);//添加动态
#endif
                        if (OnUserProfileChanged != null) OnUserProfileChanged(GetAuthUser(operatorUser.UserID));//调用事件处理函数

                        result = true;
                    }
                }

                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新用户资料
        /// </summary>
        public bool UpdateUserProfile(AuthUser operatorUser, MaxLabs.bbsMax.Enums.Gender gender, Int16 birthYear, short birthMonth, Int16 birthday, string signature, float timeZone, UserExtendedValueCollection extendedFields)
        {
            SignatureFormat signatureFormat = GetSignatureFormat(operatorUser);
            return UpdateUserProfile(operatorUser, gender, birthYear, birthMonth, birthday, signature, timeZone, extendedFields, signatureFormat);
        }


        public bool Server_UpdateUserProfile(AuthUser operatorUser, MaxLabs.bbsMax.Enums.Gender gender, Int16 birthYear, short birthMonth, Int16 birthday, string signature, float timeZone, UserExtendedValueCollection extendedFields)
        {
            return UpdateUserProfile(operatorUser, gender, birthYear, birthMonth, birthday, signature, timeZone, extendedFields, SignatureFormat.Html);
        }

        private MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy GetUserExtendedValueProxy(UserExtendedValue extendValue)
        {
            if (extendValue == null)
                return null;
            MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy proxy = new MaxLabs.bbsMax.PassportServerInterface.UserExtendedValueProxy();

            proxy.ExtendedFieldID = extendValue.ExtendedFieldID;
            proxy.PrivacyType = (int)extendValue.PrivacyType;
            proxy.UserID = extendValue.UserID;
            proxy.Value = extendValue.Value;

            return proxy;
        }

        public bool UpdateLastVisitIP(int targetUserID, string newIP)
        {
            User user = GetUser(targetUserID);

            if (user == null)
            {
                ThrowError(new UserNotExistsError("targetUserID", targetUserID));
                return false;
            }

            if (UserDao.Instance.UpdateLastVisitIP(targetUserID, newIP))
            {
                user.LastVisitIP = newIP;

                return true;
            }

            return false;
        }

        public bool UpdateSkinID(AuthUser operatorUser, string skinID)
        {
            if (operatorUser.UserID == 0)
            {
                if (HttpContext.Current != null)
                {
                    HttpCookie cookie = new HttpCookie("maxskin");
                    cookie.Value = skinID;
                    HttpContext.Current.Response.SetCookie(cookie);
                }
            }
            else
            {
                if (UserDao.Instance.UpdateSkinID(operatorUser.UserID, skinID))
                {
                    operatorUser.SkinID = skinID;
                    return true;
                }
            }

            return false;
        }

        public string GetCookieSkinID()
        {
            if (HttpContext.Current != null)
            {
                HttpCookie skinCookie = CookieUtil.Get("maxskin");
                return skinCookie == null ? string.Empty : skinCookie.Value;
            }

            return string.Empty;
        }

        public void UpdateUserSelectFriendGroupID(AuthUser user, int groupID)
        {
            if (user.SelectFriendGroupID != groupID)
            {
                UserDao.Instance.UpdateUserSelectFriendGroupID(user.UserID, groupID);
                user.SelectFriendGroupID = groupID;
            }
        }

        public void UpdateUserReplyReturnThreadLastPage(AuthUser user, bool returnLastPage)
        {
            if (user.ReplyReturnThreadLastPage == null || user.ReplyReturnThreadLastPage.Value != returnLastPage)
            {
                UserDao.Instance.UpdateUserReplyReturnThreadLastPage(user.UserID, returnLastPage);
                user.ReplyReturnThreadLastPage = returnLastPage;
            }
        }

        /// <summary>
        /// 更新Email
        /// </summary>
        public bool UpdateEmail(AuthUser operatorUser, int targetUserId, string email)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            User user = GetUser(targetUserId);

            if (user == null)
            {
                ThrowError(new UserNotExistsError("targetUserID", targetUserId));
                return false;
            }

            if (operatorUser.UserID != targetUserId)
            {
                if (!CanEditUserProfile(operatorUser, targetUserId))
                {
                    ThrowError(new NoPermissionEditUserProfileError());
                    return false;
                }
            }

            ValidateEmail(email, "email");

            if (HasUnCatchedError) return false;


            if (string.Compare(user.Email, email, true) != 0)
            {
#if !Passport
                if (Globals.PassportClient.EnablePassport)
                {
                    APIResult result = null;

                    try
                    {
                        result = Globals.PassportClient.PassportService.User_UpdateEmail(operatorUser.UserID, targetUserId, email);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                        return false;
                    }

                    if (result != null)
                    {
                        if (result.IsSuccess == false)
                        {
                            for (int i = 0; i < result.Messages.Length; i++)
                            {
                                ThrowError(new CustomError(result.ErrorTargets[i], result.Messages[i]));
                            }
                        }
                        else
                        {
                            user.Email = email;
                        }

                        return result.IsSuccess;
                    }
                    else
                    {
                        return false;
                    }
                }
                else 
#endif
                {
                    if (UserDao.Instance.UpdateEmail(targetUserId, email))
                    {
                        user.Email = email;
                        if (OnUserEmailChanged != null) OnUserEmailChanged(targetUserId, email);
                    }
                    else
                    {
                        ThrowError(new DuplicateEmailError("email", email));
                        return false;
                    }
                }
            }

                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        public void Client_UpdateUserEmail(int userID , string email)
        {
            UserDao.Instance.ValidateUserEmail(userID, email);//既然是同步过来的那肯定是已经通过验证的最终邮箱地址，可以直接验证通过

            AuthUser user = GetUserFromCache<AuthUser>(userID);
            if (user != null)
            {
                user.EmailValidated = true;
            }
        }

        public void Server_UpdateUserDoing(int userID, string doing)
        {
            KeywordReplaceRegulation keywordReg = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;
            doing = keywordReg.Replace(doing);

            UserDao.Instance.UpdateUserDoing(userID, doing);

            User user = UserBO.Instance.GetUserFromCache(userID);

            if (user != null)
            {
                user.Doing = doing;
                user.DoingDate = DateTimeUtil.Now;
            }
        }

        public void UpdateUserDoing(int userID,string content)
        {
            Server_UpdateUserDoing(userID, content);

            if (OnUserDoingUpdated != null) OnUserDoingUpdated(userID, content);
        }

        /// <summary>
        /// 更新我的Email
        /// </summary>
        /// <param name="email"></param>
        public bool UpdateEmail(AuthUser operatorUser, string email)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (string.IsNullOrEmpty(email))
            {
                ThrowError(new EmptyEmailError("email"));
                return false;
            }

            if (string.Compare(operatorUser.Email.Trim(), email.Trim(), true) == 0)
                return false;

            if (HasUnCatchedError)
                return false;

            return UpdateEmail(operatorUser, operatorUser.UserID, email);
        }

        /// <summary>
        /// 绑定邮箱时使用的邮箱更新.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool LoginEmailBind(AuthUser operatorUser, string password, string email)
        {

            if (!operatorUser.IsActive)//账号是否激活
            {
                string activeUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ActivingUser, operatorUser.UserID, operatorUser.Password, false);
                //BbsRouter.GetUrl("register", string.Format("resend=1&userid={0}&code={1}", user.UserID, SecurityUtil.SHA1(user.Password)));

                ThrowError(new UserNotActivedError("username", operatorUser.UserID, operatorUser.Email, activeUrl));
                return false;
            }

            if (!operatorUser.EmailValidated)
            {
                string reactiveUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ValidateEmail, operatorUser.UserID, operatorUser.Password, false);

                ThrowError(new EmailNotValidatedError("username", operatorUser.Email, reactiveUrl));
                return false;
            }

            //如果已经验证,则设置状态为已登录.
            SetUserLogin(operatorUser, password, operatorUser.Password, true);

            return true;

        }

        /// <summary>
        /// 更新用户侧边栏状态
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="status"></param>
        public void SetSidebarStatus(AuthUser operatorUser, EnableStatus status)
        {
            UserDao.Instance.SetSidebarStatus(operatorUser.UserID, status);

            ///更新缓存
            operatorUser.EnableDisplaySidebar = status;
        }

        public int UpdateUsersDatas(int startUserID, int updateCount, bool updatePostCount, bool updateBlogCount, bool updateInviteCount, bool updateCommentCount, bool updatePictureCount, bool updateShareCount, bool updateDoingCount, bool updateDiskFileCount)
        {
            return UserDao.Instance.UpdateUsersDatas(startUserID, updateCount, updatePostCount, updateBlogCount, updateInviteCount, updateCommentCount, updatePictureCount, updateShareCount, updateDoingCount, updateDiskFileCount);
        }


        #endregion

        #region 用户登陆

        /// <summary>
        /// 用户登陆
        /// </summary>
        public bool Login(string username, string password, string ip, bool autoLogin, bool isUsernameLogin)
        {

            if (string.IsNullOrEmpty(username))
            {
                if (isUsernameLogin)
                {
                    ThrowError(new EmptyUsernameError("username"));
                }
                else
                {
                    ThrowError(new EmptyEmailError("username"));
                }

            }

            if (isUsernameLogin == false)
            {
                if (ValidateUtil.IsEmail(username)==false)
                {
                    ThrowError(new EmailFormatError("username", username));
                    return false;
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
            }

            if (HasUnCatchedError) return false;

            //是否限制访问 //TODO: 不应该在这判断
            //if ((AllSettings.Current.AccessLimitSettings.AccessIPLimitMode == LimitMode.Allow && !AllSettings.Current.AccessLimitSettings.AdminIPLimitList.IsMatch(ip))
            //    || (AllSettings.Current.AccessLimitSettings.AccessIPLimitMode == LimitMode.Reject && AllSettings.Current.AccessLimitSettings.AdminIPLimitList.IsMatch(ip))
            //    )

            if (true == AllSettings.Current.AccessLimitSettings.AccessIPLimitList.IsMatch(ip))
            {
                ThrowError(new IPIsLimitAccessError("ip", ip));
                return false;
            }

            AuthUser user = null;
            string kaputCause = string.Empty;

            if (isUsernameLogin)
            {
                user = GetAuthUser(username, true);
                if (user == null || user == User.Guest)
                {
                    ThrowError(new UserNotExistsError("username", username));
                    return false;
                }
            }
            else
            {
                bool duplicateEmail;
                user = GetAuthUserByEmail(username, true, out duplicateEmail);
                if (user == null || user == User.Guest)
                {
                    ThrowError(new EmailNotExistError("username"));
                    return false;
                }

                if (duplicateEmail)
                {
                    kaputCause = "Email与其他账号冲突";
                    string bindemailUrl = BbsRouter.GetUrl("bindemail", string.Format("email={0}", username));
                    ThrowError(new UserEmailLoginRepeatError("bindemail", username, bindemailUrl));
                    return false;
                }

            }

            if (!SecurityUtil.ComparePassword(user.PasswordFormat, password, user.Password))
            {
                kaputCause = "密码错误！";
                ThrowError(new PasswordError("password"));
                return false;
            }

            if (!CanLogin(user))
            {
                kaputCause = "没有权限登录！";
                ThrowError(new NoPermissionLoginError());
                return false;
            }

            if (!user.IsActive)//账号是否激活
            {
                kaputCause = "账号没有激活";
                string activeUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ActivingUser, user.UserID, user.Password, false);
                    //BbsRouter.GetUrl("register", string.Format("resend=1&userid={0}&code={1}", user.UserID, SecurityUtil.SHA1(user.Password)));

                ThrowError(new UserNotActivedError("username", user.UserID, user.Email, activeUrl));
                return false;
            }

            //如果是使用邮箱登录的用户,需要检查邮箱是否验证
            if (false == isUsernameLogin)
            {
                if (!user.EmailValidated)
                {
                    kaputCause = "邮箱没有验证";
                    string reactiveUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ValidateEmail, user.UserID, user.Password, false);

                    ThrowError(new EmailNotValidatedError("username", user.Email, reactiveUrl));
                    return false;
                }
            }

            if (user.Roles.IsInRole(Role.FullSiteBannedUsers))
            {
                kaputCause = "被整站屏蔽";
                ThrowError(new UserIsBanned(user.Username));
                return false;
            }

            SetUserLogin(user, password, user.Password, autoLogin);
            UserDao.Instance.UpdateUserLoginCount(user.UserID, ip);

            return true;
        }

        public void Logout()
        {
            int userID = User.CurrentID;
            CookieUtil.Remove(cookieKey_User);
            HttpContext.Current.Items[cacheKey_CurrentUserID] = 0;
            RemoveUserCache(userID);

            if (OnUserLogout != null)
                if (userID > 0)
                    OnUserLogout(userID);
        }

        /// <summary>
        /// 直接设置用户为登陆状态
        /// </summary>
        /// <param name="userId">用户编号</param>
        /// <param name="encryptPassword">这个密码是UserDatas表里那个已经加密过得密码</param>
        /// <param name="autoLogin">是否不存cookie不过期</param>
        public void SetUserLogin(User user, string password, string encryptPassword, bool autoLogin)
        {
            HttpContext.Current.Items[cacheKey_CurrentUserID] = user.UserID;
            DateTime? expires = null;
            if (autoLogin)
                expires = DateTimeUtil.Now.AddMonths(6);
            CookieUtil.Set(cookieKey_User, EncodeCookie(user.UserID, encryptPassword), expires);
        }

        #endregion

        #region  绑定邮箱

        public bool BindEmail(string username, string password, string email)
        {
            //用户名为空
            if (string.IsNullOrEmpty(username))
            {
                ThrowError(new EmptyUsernameError("username"));
            }

            //密码为空
            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
            }

            //email为空
            if (string.IsNullOrEmpty(email))
            {
                ThrowError(new EmptyEmailError("email"));
            }
            //email格式错误
            else if (ValidateUtil.IsEmail(email))
            {
                ThrowError(new EmailFormatError("email", email));
            }

            //发生以上任何一种错误，立即返回
            if (HasUnCatchedError) return false;

            AuthUser user = GetAuthUser(username);

            if (user == null || user == User.Guest)
            {
                ThrowError(new UserNotExistsError("username", username));
                return false;
            }

            string kaputCause = string.Empty;

            if (!SecurityUtil.ComparePassword(user.PasswordFormat, password, user.Password))
            {
                kaputCause = "密码错误!";
                ThrowError(new PasswordError("password"));
                return false;
            }

            if (string.Compare(user.Email, email, true) != 0)
            {
                kaputCause = "您填写的用户名并没有使用该邮箱，可能您尚未在本站注册";
                ThrowError(new BindEmailError("username"));
                return false;
            }

            string emailValidateUrl;
            if (user.IsActive == false)
            {
                emailValidateUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ActivingUser, user.UserID, user.Password, false);
                ThrowError(new UserNotActivedError("username", user.UserID, email, emailValidateUrl));
                return false;
            }

            if (user.EmailValidated == false)
            {
                emailValidateUrl = UrlHelper.GetSendEmailUrl(ValidateEmailAction.ValidateEmail, user.UserID, user.Password, false);
                ThrowError(new EmailNotValidatedError("email", email, emailValidateUrl));
                return false;
            }

            UserDao.Instance.RemoveRepeatedEmail(user.UserID, user.Email);

            return true;
        }

        public AuthUser CheckUserNameAndPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                ThrowError(new EmptyUsernameError("username"));
            }

            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
            }

            if (HasUnCatchedError) return null;

            AuthUser user = GetAuthUser(username);

            if (user == null || user == User.Guest)
            {
                ThrowError(new UserNotExistsError("username", username));
                return null;
            }

            if (!SecurityUtil.ComparePassword(user.PasswordFormat, password, user.Password))
            {
                ThrowError(new PasswordError("password"));
                return null;
            }

            return user;
        }
        #endregion

        #region 找回密码

        int _RecoverPasswordSerialValidDays = 3;//暂时的， 那个找回密码的序列号有效期，暂定为3天

        /// <summary>
        /// 发送取回密码的序列号
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public bool TryRecoverPassword(string username, string email)
        {
            User user;

            if (!Pool<EmailRegex>.Instance.IsMatch(email))
            {
                ThrowError(new EmailFormatError("email", email));
            }

            if (string.IsNullOrEmpty(username))
            {
                ThrowError(new EmptyUsernameError("username"));
                return false;
            }

            user = UserBO.Instance.GetUser(username.Trim());
            if (user == null)
            {
                ThrowError(new UserNotExistsError("username", username));
                return false;
            }
            else if (!user.IsActive)
            {
                //ThrowError(new UserNotActivedError("username", user.UserID, email, ????));
                ThrowError(new CustomError("username", "您的帐号：" + user.Username + " 还未激活，不能使用此功能。"));
            }

            if (HasUnCatchedError) return false;

            if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                //创建一个找回密码序列号
                Guid serial = SerialBO.Instance.CreateSerial(user.UserID, DateTimeUtil.Now.AddDays(_RecoverPasswordSerialValidDays), SerialType.RecoverPassword).Serial;

                try
                {
                    RecoverPasswordEmail sendEmail = new RecoverPasswordEmail(user.Email, user.Username, serial.ToString());
                    sendEmail.Send();
                }
                catch
                {
                    ThrowError(new EmailSendError());
                    return false;
                }
                UserDao.Instance.CreateRecoverPasswordLog(user.UserID, email, serial.ToString(),IPUtil.GetCurrentIP());
                return true;
            }
            else
            {
                ThrowError(new EmailNotMatchError("email"));
            }

            return false;
        }

        public bool ResetPasswordBySerial(string recoverPasswordSerial, string newPassword)
        {
            if (!AllSettings.Current.RecoverPasswordSettings.Enable)
            {
                ThrowError(new RecoverPasswordDisableError());
                return false;
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                ThrowError(new EmptyPasswordError("newPassword"));
                return false;
            }

            MaxSerial serial = SerialBO.Instance.GetSerial(recoverPasswordSerial, SerialType.RecoverPassword);

            if (serial == null)
            {
                ThrowError(new SerialError("recoverPasswordSerial", recoverPasswordSerial));
                return false;
            }

            string encodedPassword = SecurityUtil.Encrypt(EncryptFormat.bbsMax, newPassword);

            int userID = serial.OwnerUserId;

            UserDao.Instance.ResetUserPassword(userID, encodedPassword, EncryptFormat.bbsMax);

            SerialBO.Instance.DeleteSerial(userID, SerialType.RecoverPassword);

            RemoveUserCache(userID);

            User user = GetUser(userID);

            SetUserLogin(user, newPassword, encodedPassword, false);

            UserDao.Instance.SetRecoverPasswordLogSuccess(serial.Serial.ToString(),IPUtil.GetCurrentIP());

            return true;
        }

        /// <summary>
        /// 检测找回密码序列号
        /// </summary>
        /// <returns>用户名</returns>
        public string GetRecoverPasswordUsername(Guid serial)
        {

            MaxSerial recoverSerial = SerialBO.Instance.GetSerial(serial, SerialType.RecoverPassword);
            if (recoverSerial != null)
            {
                User user = GetUser(recoverSerial.OwnerUserId);
                if (user != null)
                    return user.Username;
            }

            return string.Empty;
            // return UserDao.Instance.GetRecoverPasswordUsername(serial);
        }

        /// <summary>
        /// 获取找回密码的记录
        /// </summary>
        /// <returns></returns>
        public RecoverPasswordLogCollection GetRecoverPasswordLogs( AuthUser operateUser, RecoverPasswordLogFilter filter,int pageNumber)
        {
            if (CanManageUser(operateUser))
            {
                return UserDao.Instance.GetRecoverPasswordLogs(filter, pageNumber);
            }
            return new RecoverPasswordLogCollection();
        }

        #endregion

        #region 用户查找/搜索

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID">当前用户ID</param>
        /// <param name="getType"></param>
        /// <param name="pageNmuber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="userSortNumber">当前用户排名</param>
        /// <returns></returns>
        public UserCollection GetUsers(int userID, UserOrderBy orderField, int pageNmuber, int pageSize, out int userSortNumber)
        {
            UserCollection users = UserDao.Instance.GetUsers(userID, orderField, pageNmuber, pageSize, out userSortNumber);

            ProcessKeyword(users, ProcessKeywordMode.TryUpdateKeyword);

            return users;
        }

        public UserCollection GetUsers(UserFilter filter, int pageNumber)
        {
            int? total = null;
            if (filter.IsNull == true)
                total = VarsManager.Stat.TotalUsers;

            UserCollection users = UserDao.Instance.GetUsers(filter, pageNumber, total);

            ProcessKeyword(users, ProcessKeywordMode.TryUpdateKeyword);

            return users;
        }

        ///// <summary>
        ///// 更新的用户
        ///// </summary>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //public UserCollection GetLastUpdateUsers(int count)
        //{
        //    return UserDao.Instance.GetLastUpdateUsers(count);
        //}

        /// <summary>
        /// 获取没有使用的用户编号
        /// </summary>
        /// <param name="beginID"></param>
        /// <param name="endID"></param>
        /// <returns></returns>
        public List<Int32Scope> GetNotUseUserIDs(int operatorUserid, int beginID, int endID)
        {

            if (operatorUserid <= 0)
            {
                ThrowError(new NotLoginError());
                return null;
            }
            if (beginID > endID)
            {
                int temp = beginID;
                beginID = endID;
                endID = temp;
            }
            return UserDao.Instance.GetNotUseUserIDs(beginID, endID);
        }

        public List<int> GetNotUserIDs(int beginID, int endID, int pageNumber, int pageSize, out int totalCount)
        {
            List<int> userIDs = UserDao.Instance.GetUserIDs(beginID, endID);

            if (userIDs.Count == beginID - endID + 1)
            {
                totalCount = 0;
                return new List<int>();
            }
            if (userIDs.Count == 0)
            {
                totalCount = beginID - endID + 1;
                return null;
            }

            int start = beginID + (pageNumber - 1) * pageSize;

            List<int> result = new List<int>();
            for (int i = start; i < endID; i++)
            {
                if (userIDs.Contains(i) == false)
                    result.Add(i);

                if (result.Count >= pageSize)
                    break;
            }

            totalCount = endID - beginID + 1 - userIDs.Count;

            return result;
        }

        /// <summary>
        /// 查找用户
        /// </summary>
        public UserCollection SearchUsers(AdminUserFilter filter, int userID, int pageNumber, out int totalCount)
        {
            if (pageNumber < 1) pageNumber = 1;
            UserCollection users = UserDao.Instance.SearchUsers(filter, userID, pageNumber, out totalCount);

            ProcessKeyword(users, ProcessKeywordMode.TryUpdateKeyword);
            return users;
        }

        public UserCollection SearchUsers(AdminUserFilter filter, int userID)
        {
            int pageNumber = 1;
            int totalCount;//没用
            UserCollection users = UserDao.Instance.SearchUsers(filter, userID, pageNumber, out totalCount);

            ProcessKeyword(users, ProcessKeywordMode.FillOriginalText);

            return users;
        }

        public UserCollection AdminSearchUsers(int operatorUserID, AdminUserFilter filter, int pageNumber, out int totalCount)
        {
            if (pageNumber <= 0) pageNumber = 1;

            if (false == ManagePermissionSet.HasPermissionForSomeone(operatorUserID, ManageUserPermissionSet.ActionWithTarget.EditUserProfile))
            {
                ThrowError(new NoPermissionEditUserProfileError());
                totalCount = 0;
                return new UserCollection();
            }

            Guid[] exculdeRoles = ManagePermissionSet.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.User);

            UserCollection users = UserDao.Instance.AdminSearchUsers(filter, exculdeRoles, pageNumber, out totalCount);

            ProcessKeyword(users, ProcessKeywordMode.FillOriginalText);

            return users;
        }

        //public UserCollection GetUserList(AdminUserFilter.OrderBy order, int pageNumber, int pageSize, bool desc)
        //{
        //    if (pageNumber < 1)
        //        pageNumber = 1;
        //    if (pageSize < 0)
        //        pageSize = Consts.DefaultPageSize;

        //    return UserDao.Instance.GetUserlist(order, pageNumber, pageSize, desc);
        //}
        /*
        public void DeleteUsers(int operatorUserID, IEnumerable<int> targetUserIds,bool deletePost)
        {
            List<string> userFiles;
            if (operatorUserID > 0)
            {
                if (!ValidateUtil.HasItems<int>(targetUserIds)) return;

                if (false == ManagePermissionSet.HasPermissionForSomeone(operatorUserID, ManageUserPermissionSet.ActionWithTarget.EditUserProfile))
                {
                    ThrowError(new NoPermissionEditUserProfileError());
                    return;
                }

                Guid[] exculdeRoles = ManagePermissionSet.GetNoPermissionTargetRoleIds(operatorUserID, PermissionTargetType.User);
                UserDao.Instance.DeleteUsers(targetUserIds, deletePost);
                RemoveUsersCache(targetUserIds);
                //StatManager.UpdateNewUserStat();
            }
            else
            {
                ThrowError(new NotLoginError());
            }
        }
        */
        /// <summary>
        /// 删除单个用户
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="deletePost"></param>
        public int DeleteUser(AuthUser operatorUser, int targetUserID, int currentStep, int stepCount)
        {
            if (!CanDeleteUser(operatorUser, targetUserID))// AllSettings.Current.ManageUserPermissionSet.Can(operatorUser.UserID, ManageUserPermissionSet.ActionWithTarget.DeleteUser, targetUserID))
            {
                //TODO Throw No Permission Error;
                return 0;
            }

            if (operatorUser.UserID == targetUserID)
            {
                ThrowError(new CanNotDeleteSelfError());
                return 0;
            }

            User targetUser = null;
            if (targetUserID > 0)
            {
                targetUser = GetUser(targetUserID);
            }

            if (targetUser == null)
            {
                ThrowError<ErrorInfo>(new UserNotExistsError("targetUserID", targetUserID));
                return 0;
            }

            if (currentStep == 0)
            {
                DeleteUserAvatarFile(targetUser);
#if !Passport
                BanUser(operatorUser, targetUserID, DateTime.MaxValue, "删除用户，临时屏蔽", true);//屏蔽用户先
#endif
                return 0;
            }
            else if (currentStep + 1 == stepCount)
            {
                /*--------------版主和屏蔽用户检查----------------*/
                bool isModerator = false;//是否版主
                bool isBannedUser = false;//是否被屏蔽用户

                isModerator = targetUser.Roles.IsInRole(Role.Moderators)
                    || targetUser.Roles.IsInRole(Role.JackarooModerators)
                    || targetUser.Roles.IsInRole(Role.CategoryModerators);

                isBannedUser = targetUser.Roles.IsInRole(Role.ForumBannedUsers)
                    || targetUser.Roles.IsInRole(Role.FullSiteBannedUsers);
                /*===========================================*/

                if (isBannedUser)
                {
#if !Passport
                    BannedUserProvider.ClearInnerTable();
#endif
                }


                //ModeratorProvider.ClearInnerTable();
#if !Passport
                ForumBO.Instance.ClearAllCache();
                ThreadCachePool.ClearAllCache();
#endif
                //ForumManager.RemoveAllForumsCache();//如果被删除用户是版主， 更新整个版块列表缓存， 
                
                Logs.LogManager.LogOperation(new Logs.DeleteUser(operatorUser.UserID, operatorUser.Username, targetUser.Username, IPUtil.GetCurrentIP()));
                RemoveUserCache(targetUserID);//移除缓存
                
                //CacheUtil.Clear();
            }
            else
            {
                int result = UserDao.Instance.DeleteUser(targetUserID, currentStep);
                return result;
            }
            return 0;
        }

        /// <summary>
        /// 返回用户组成员
        /// </summary>
        /// <returns></returns>
        public UserCollection GetRoleMembers(int operatorUserID, Guid roleID, int pageSize, int pageNumber, out int totalCount)
        {
            RoleSettings roleSetting = AllSettings.Current.RoleSettings;
            //if( ManagePermissionSet.Can(operatorUserID, ManageUserPermissionSet.ActionWithTarget.  )

            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = Consts.DefaultPageSize;

            Role role = roleSetting.GetRole(roleID);
            totalCount = 0;
            if (role == null)
            {
                return new UserCollection();
            }

            if (role.IsLevel)
            {
                int levelValue1, levelValue2 = int.MaxValue;
                levelValue1 = role.RequiredPoint;
                foreach (Role r in roleSetting.GetLevelRoles())
                {
                    if (r.RequiredPoint > role.RequiredPoint)
                    {
                        levelValue2 = r.RequiredPoint;
                        break;
                    }
                }

                return UserDao.Instance.GetRoleMembers(roleSetting.LevelLieOn
                    , new Int32Scope(levelValue1, levelValue2)
                    , pageSize, pageNumber, out totalCount);
            }

            return UserDao.Instance.GetRoleMembers(roleID, pageSize, pageNumber, out totalCount);
        }

        /// <summary>
        /// 在多少分钟内是否存在某IP
        /// </summary>
        public bool IPIsExistInMinutes(string ip, int timeSpan)
        {
            return UserDao.Instance.IPIsExistInMinutes(ip, timeSpan);
        }

        /// <summary>
        /// 受targetUserId邀请注册的用户
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public UserCollection GetInvitees(int targetUserId, int pageSize, int pageNumber, out int totalCount)
        {
            return UserDao.Instance.GetInvitees(targetUserId, pageSize, pageNumber, out totalCount);
        }

        #endregion

        #region 密码修改


        /// <summary>
        /// 修改用户密码
        /// </summary>
        public void ResetUserPassword(AuthUser operatorUser, int targetUserID, string password)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
                return;
            }


#if !Passport

            if (Globals.PassportClient.EnablePassport)
            {
                APIResult result = Globals.PassportClient.PassportService.User_ResetUserPassword(operatorUser.UserID, targetUserID, password);

                if (!result.IsSuccess)
                {
                    string msg = string.Empty;

                    foreach (string s in result.Messages)
                    {
                        msg += s;
                    }

                    ThrowError(new CustomError(msg));
                }
                return;
            }
#endif

            User targetUser = GetUser(targetUserID);

            if (targetUser == null)
            {
                ThrowError(new InvalidParamError("targetUserID"));
                return;
            }
            else
            {
                if (operatorUser.UserID != targetUserID && !CanEditUserProfile(operatorUser, targetUserID))
                {
                    ThrowError(new NoPermissionEditUserProfileError());
                    return;
                }
            }

            if (!HasUnCatchedError)
            {

                EncryptFormat format = EncryptFormat.bbsMax;

                string tempPassword = password;

                password = SecurityUtil.Encrypt(format, password);

                UserDao.Instance.ResetUserPassword(targetUserID, password, format);
                //清除被重设密码用户的缓存
                RemoveUserCache(targetUserID);

                //如果操作者和操作目标是同一个用户（也就是改自己的密码），则要把自己重新设置登陆状态（更新cookie）
                if (operatorUser.UserID == targetUserID)
                    SetUserLogin(operatorUser, tempPassword, password, false);

                Logs.LogManager.LogOperation(new Logs.ChangePassword(operatorUser.UserID, targetUser.UserID, operatorUser.Username, targetUser.Username, IPUtil.GetCurrentIP()));

                if (OnUserPasswordChanged != null) OnUserPasswordChanged(targetUserID, password);
            }
        }

        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ResetPassword(AuthUser operatorUser, string oldPassword, string newPassword)
        {

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                ThrowError(new EmptyPasswordError("oldpassword"));
                //return false;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                ThrowError(new NewPasswordEmptyError("newPassword", newPassword));
                //return false;
            }

            if (HasUnCatchedError)
                return false;

#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                APIResult results = null;
                try
                {
                    results = Globals.PassportClient.PassportService.User_ChangePassword(operatorUser.UserID, oldPassword, newPassword);
                }
                catch(Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }
                if (!results.IsSuccess)
                {
                    for (int i = 0; i < results.Messages.Length; i++)
                    {
                        ThrowError<CustomError>(new CustomError(results.ErrorTargets[i], results.Messages[i]));
                    }
                }
                else
                {
                    ResetUserPassword(operatorUser, operatorUser.UserID, newPassword);
                }
            }
            else
#endif
            {
                if (!SecurityUtil.ComparePassword(operatorUser.PasswordFormat, oldPassword, operatorUser.Password))
                {
                    ThrowError(new PasswordError("oldpassword"));
                    return false;
                }
                //实际执行重置自己的密码（改方法将同时清除operatUser的缓存）
                ResetUserPassword(operatorUser, operatorUser.UserID, newPassword);
                if (!HasUnCatchedError)
                {
                    //RemoveUserCache(operatorUserID); //上面的方法中已经清过，此处不需要
                    //SetUserLogin(user, newPassword, SecurityUtil.Encrypt(EncryptFormat.bbsMax, newPassword), false);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region  处理用户时区

        /// <summary>
        /// 获取用户的时差
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public float GetUserTimeDiffrence(AuthUser user)
        {
            if (user.UserID <= 0)
                //return 0.0d;
                return AllSettings.Current.DateTimeSettings.ServerTimeZone - DateTimeUtil.DatabaseTimeDifference;

            return user.TimeZone - DateTimeUtil.DatabaseTimeDifference;
            //return (double)(user.TimeZone-AllSettings.Current.DateTimeSettings.ServerTimeZone);

        }

        //public float GetUserTimeDiffrence()
        //{
        //    return GetUserTimeDiffrence(GetCurrentUser());
        //}

        //public DateTime UserDateTimeToUtcDateTime(string dateTime)
        //{
        //    DateTime dt = StringUtil.TryParse<DateTime>(dateTime);
        //    return UserDateTimeToUtcDateTime(dt);
        //}

        //public DateTime UserDateTimeToUtcDateTime(DateTime dateTime)
        //{
        //    return UserDateTimeToUtcDateTime(GetCurrentUser(), dateTime);
        //}

        //public DateTime UserDateTimeToUtcDateTime(AuthUser operatorUser, DateTime dateTime)
        //{
        //    try
        //    {
        //        if (operatorUser == User.Guest)
        //            return dateTime.AddHours(-AllSettings.Current.DateTimeSettings.ServerTimeZone);

        //        return dateTime.AddHours(-operatorUser.TimeZone);
        //    }
        //    catch
        //    {
        //        return dateTime;
        //    }

        //}

        #endregion

        #region 根据用户名获取UserID  GetUserID / GetUserIDs

        /// <summary>
        /// 获取一组用户ID,并缓存用户信息
        /// </summary>
        public List<int> GetUserIDs(IEnumerable<string> usernames)
        {
            if (!ValidateUtil.HasItems<string>(usernames))
                return new List<int>();

            return UserDao.Instance.GetUserIDs(usernames);
        }


        public int GetUserID(string username)
        {
            int userID = GetUserIDFromPageCache(username, UserLoginType.Username);

            if (userID == -1)
            {
                userID = UserDao.Instance.GetUserID(username);
                SetUserIDPageCache(username, userID, UserLoginType.Username);
            }

            return userID;
        }

        /// <summary>
        /// 根据关键字搜索用户
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<int> GetUserIDs(string keyword)
        {
            return UserDao.Instance.GetUserIDs(keyword);
        }

        #endregion

        #region 获得用户的密码信息

        /// <summary>
        /// 获得用户的密码信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserPassword GetUserPassword(int userID)
        {
            return UserDao.Instance.GetUserPassword(userID);
        }

        #endregion

        #region 获取用户 GetUser / GetUsers

        #region GetUser

        /// <summary>
        /// 获取用户
        /// </summary>
        public User GetUser(int userID)
        {
            return GetUser(userID, GetUserOption.Default);
        }

        public User GetUser(int userID, GetUserOption option)
        {
            return GetUser(userID, option, false,false);
        }

        public User GetUser(int userID , bool syncFromPassport)
        {
            return GetUser(userID, GetUserOption.Default,false, syncFromPassport);
        }

        public User GetUser(int userID, GetUserOption option, bool getFriends,bool syncFromPassport)
        {
            if (userID <= 0)
            {
                if (option == GetUserOption.WithAll || option == GetUserOption.WithGuest)
                    return User.Guest;
                else
                    return null;
            }

            bool needProcessKeyword;
            User user = GetUserFromCache<User>(userID, out needProcessKeyword);

            if (user == null)
            {
                user = UserDao.Instance.GetUser(userID, getFriends);

                if (user != null)
                    needProcessKeyword = true;

                if (user == null && (option == GetUserOption.WithAll || option == GetUserOption.WithDeletedUser))
                {
                    user = User.BuildDeletedUser(userID);
                }

                if (user != null)
                {
                    AddUserToCache(user);
                }
            }

            else if (user.IsDeleted && (option != GetUserOption.WithAll && option != GetUserOption.WithDeletedUser))
            {
                user = null;
            }

#if !Passport

            if (syncFromPassport && user == null && Globals.PassportClient.EnablePassport)
            {
                Client_MappingUserRecord(userID);
                user = UserDao.Instance.GetUser(userID, getFriends);
            }

#endif

            if (needProcessKeyword)
                ProcessKeyword(user, ProcessKeywordMode.TryUpdateKeyword);

            return user;
        }

        public User GetUser(string username)
        {
            return GetUser(username, false);
        }

        public User GetUser(string username, bool getFriends)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            int userID = GetUserIDFromPageCache(username, UserLoginType.Username);

            if (userID != -1)
                return GetUser(userID);

            User user = UserDao.Instance.GetUser(username, getFriends);

            if (user != null && user.UserID > 0)
            {
                AddUserToCache(user);
                SetUserIDPageCache(username, user.UserID, UserLoginType.Username);
                ProcessKeyword(user, ProcessKeywordMode.TryUpdateKeyword);
            }

            return user;
        }

        #endregion

        #region GetUsers

        public UserCollection GetUsers(IEnumerable<string> usernames)
        {
            return GetUsers(usernames, false);
        }

        /// <summary>
        /// 取得一组用户并缓存
        /// </summary>
        public UserCollection GetUsers(IEnumerable<string> usernames, bool getFriends)
        {
            if (!ValidateUtil.HasItems<string>(usernames))
                return new UserCollection();

            UserCollection users = UserDao.Instance.GetUsers(usernames, getFriends);

            ProcessKeyword(users, ProcessKeywordMode.TryUpdateKeyword);

            return users;
        }

        /// <summary>
        /// 返回一组用户实例（顺序没有规律）
        /// </summary>
        public UserCollection GetUsers(IEnumerable<int> userIds)
        {
            return GetUsers(userIds, GetUserOption.Default);
        }

        public UserCollection GetUsers(IEnumerable<int> userIds, GetUserOption option)
        {
            return GetUsers(userIds, option, false);
        }

        /// <summary>
        /// 返回一组用户实例（顺序没有规律），且可以指定是否包含已被删除的用户和游客
        /// </summary>
        /// <param name="targetUserIds"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public UserCollection GetUsers(IEnumerable<int> userIds, GetUserOption option, bool getFriends)
        {
            UserCollection users = new UserCollection();

            if (!ValidateUtil.HasItems<int>(userIds))
                return users;

            List<int> needReadUserIds = null;

            foreach (int userID in userIds)
            {
                //UserID不合法
                if (userID < 0)
                    continue;

                //UserID表示游客
                else if (userID == 0)
                {
                    if (option == GetUserOption.WithAll || option == GetUserOption.WithGuest)
                        users.Add(User.Guest);
                }

                //正常的UserID
                else
                {

                    User user = GetUserFromCache(userID);

                    //无法从缓存中取得的User对象，暂时统一填充DeletedUser实例，稍后到数据库中重新取这一部分的数据，并填充回来
                    if (user == null)
                    {
                        if (needReadUserIds == null)
                            needReadUserIds = new List<int>();

                        needReadUserIds.Add(userID);

                    }
                    else if (
                        user.IsDeleted == false
                        ||
                        (user.IsDeleted && option == GetUserOption.WithAll || option == GetUserOption.WithDeletedUser)
                        )

                        users.Add(user);
                }

            }

            if (needReadUserIds != null && needReadUserIds.Count > 0)
            {
                UserCollection usersFromDatabase = UserDao.Instance.GetUsers(needReadUserIds, getFriends);

                //如果返回的集合要包含已经被删除的用户，则要给数据库中无法取得的用户创建一个DeletedUser实例
                if (option == GetUserOption.WithAll || option == GetUserOption.WithDeletedUser)
                {
                    foreach (int userID in needReadUserIds)
                    {
                        User userFromDatabase;

                        //数据库中没有这个用户，创建一个DeletedUser实例
                        if (usersFromDatabase.TryGetValue(userID, out userFromDatabase) == false)
                            userFromDatabase = User.BuildDeletedUser(userID);

                        users.Add(userFromDatabase);

                        AddUserToCache(userFromDatabase);
                    }
                }

                //如果返回的集合不需要包含已经被删除的用户，则直接把数据库查询返回的结果添加到集合中
                else
                {
                    foreach (User user in usersFromDatabase)
                    {
                        users.Add(user);

                        AddUserToCache(user);
                    }
                }

            }

            ProcessKeyword(users, ProcessKeywordMode.TryUpdateKeyword);
            return users;
        }

        #endregion

        #region GetSimpleUser

        public SimpleUser GetSimpleUser(int userID)
        {
            return GetSimpleUser(userID, GetUserOption.Default,false);
        }

        public SimpleUser GetSimpleUser(int userID , GetUserOption option )
        {
            return GetSimpleUser(userID, option,false);
        }

        public SimpleUser GetSimpleUser(int userID, bool syncFromPassport)
        {
            return GetSimpleUser(userID, GetUserOption.Default, syncFromPassport);
        }

        public SimpleUser GetSimpleUser(int userID, GetUserOption option, bool syncFromPassport)
        {
            if (userID <= 0)
            {
                if (option == GetUserOption.WithAll || option == GetUserOption.WithGuest)
                    return SimpleUser.Guest;

                else
                    return null;
            }

            SimpleUser user = GetSimpleUserFromCache(userID);

            if (user == null)
            {
                user = UserDao.Instance.GetSimpleUser(userID);

                if (user == null && (option == GetUserOption.WithAll || option == GetUserOption.WithDeletedUser))
                {
                    user = SimpleUser.BuildDeletedUser(userID);
                }
                
                if (user != null)
                {
                    AddSimpleUserToCache(user);
                }
            }
            else if (user.IsDeleted && (option != GetUserOption.WithAll && option != GetUserOption.WithDeletedUser))
            {
                user = null;
            }

#if !Passport

            if (user == null && syncFromPassport && Globals.PassportClient.EnablePassport)
            {
                Client_MappingUserRecord(userID);
                user = UserDao.Instance.GetSimpleUser(userID);
            }
#endif
            return user;
        }

        #endregion

        #region GetSimpleUsers

        public SimpleUserCollection GetSimpleUsers(IEnumerable<int> userIds)
        {
            return GetSimpleUsers(userIds, GetUserOption.Default);
        }

        public SimpleUserCollection GetSimpleUsers(IEnumerable<int> userIds, GetUserOption option)
        {
            SimpleUserCollection users = new SimpleUserCollection();

            if (!ValidateUtil.HasItems<int>(userIds))
                return users;

            List<int> needReadUserIds = null;

            foreach (int userID in userIds)
            {
                //UserID不合法
                if (userID < 0)
                    continue;

                //UserID表示游客
                else if (userID == 0)
                {
                    if (option == GetUserOption.WithAll || option == GetUserOption.WithGuest)
                        users.Add(SimpleUser.Guest);
                }

                //正常的UserID
                else
                {

                    SimpleUser user = GetSimpleUserFromCache(userID);

                    //无法从缓存中取得的User对象，暂时统一填充DeletedUser实例，稍后到数据库中重新取这一部分的数据，并填充回来
                    if (user == null)
                    {
                        if (needReadUserIds == null)
                            needReadUserIds = new List<int>();

                        needReadUserIds.Add(userID);

                    }
                    else if (
                        user.IsDeleted == false
                        ||
                        (user.IsDeleted && option == GetUserOption.WithAll || option == GetUserOption.WithDeletedUser)
                        )

                        users.Add(user);
                }

            }

            if (needReadUserIds != null && needReadUserIds.Count > 0)
            {
                SimpleUserCollection usersFromDatabase = UserDao.Instance.GetSimpleUsers(needReadUserIds);

                //如果返回的集合要包含已经被删除的用户，则要给数据库中无法取得的用户创建一个DeletedUser实例
                if (option == GetUserOption.WithAll || option == GetUserOption.WithDeletedUser)
                {
                    foreach (int userID in needReadUserIds)
                    {
                        SimpleUser userFromDatabase;

                        //数据库中没有这个用户，创建一个DeletedUser实例
                        if (usersFromDatabase.TryGetValue(userID, out userFromDatabase) == false)
                            userFromDatabase = SimpleUser.BuildDeletedUser(userID);

                        users.Add(userFromDatabase);

                        AddSimpleUserToCache(userFromDatabase);
                    }
                }

                //如果返回的集合不需要包含已经被删除的用户，则直接把数据库查询返回的结果添加到集合中
                else
                {
                    foreach (SimpleUser user in usersFromDatabase)
                    {
                        users.Add(user);

                        AddSimpleUserToCache(user);
                    }
                }

            }

            return users;
        }

        #endregion

        #endregion

        #region 填充用户数据到列表中涉及的方法

        public SimpleUser GetFilledSimpleUser(int userID)
        {
            if (userID == 0) return User.Guest;

            SimpleUser user;
            if (PageCacheUtil.TryGetValue<SimpleUser>(cacheKey_User_StartAs + userID, out user))
            {
                return user;
            }

            throw new Exception("User is not filled which userid is " + userID);
            //return null;
        }

        #region WaitForFillSimpleUser

        /// <summary>
        /// 等待填充SimpleUser。等到aspx执行之时将被正式填充
        /// </summary>
        /// <param name="list"></param>
        public void WaitForFillSimpleUser<T>(T item, int actionType) where T : IFillSimpleUsers
        {
            if (item == null)
                return;

            List<int> waitForFillUserIds;

            if (PageCacheUtil.TryGetValue<List<int>>(cacheKey_FillSimpleUserIds, out waitForFillUserIds) == false)
            {
                waitForFillUserIds = new List<int>();
                PageCacheUtil.Set(cacheKey_FillSimpleUserIds, waitForFillUserIds);
            }

            int[] userIds = item.GetUserIdsForFill(actionType);

            foreach (int userID in userIds)
            {
                if (waitForFillUserIds.Contains(userID) == false)
                    waitForFillUserIds.Add(userID);
            }
        }


        /// <summary>
        /// 等待填充SimpleUser。等到aspx执行之时将被正式填充
        /// </summary>
        /// <param name="list"></param>
        public void WaitForFillSimpleUser<T>(T item) where T : IFillSimpleUser
        {
            if (item == null)
                return;

            List<int> waitForFillUserIds;

            if (PageCacheUtil.TryGetValue<List<int>>(cacheKey_FillSimpleUserIds, out waitForFillUserIds) == false)
            {
                waitForFillUserIds = new List<int>();
                PageCacheUtil.Set(cacheKey_FillSimpleUserIds, waitForFillUserIds);
            }

            int userID = item.GetUserIDForFill();

            if (waitForFillUserIds.Contains(userID) == false)
                waitForFillUserIds.Add(userID);

        }

        #endregion

        #region WaitForFillSimpleUsers

        /// <summary>
        /// 等待填充SimpleUser。等到aspx执行之时将被正式填充
        /// </summary>
        /// <param name="list"></param>
        public void WaitForFillSimpleUsers<T>(IEnumerable<T> list, int actionType) where T : IFillSimpleUsers
        {
            if (list == null)
                return;

            List<int> waitForFillUserIds;

            if (PageCacheUtil.TryGetValue<List<int>>(cacheKey_FillSimpleUserIds, out waitForFillUserIds) == false)
            {
                waitForFillUserIds = new List<int>();
                PageCacheUtil.Set(cacheKey_FillSimpleUserIds, waitForFillUserIds);
            }

            int[] userIds;
            foreach (T item in list)
            {
                if (item == null)
                    continue;

                userIds = item.GetUserIdsForFill(actionType);

                foreach (int userID in userIds)
                {
                    if (waitForFillUserIds.Contains(userID) == false)
                        waitForFillUserIds.Add(userID);
                }
            }
        }

        /// <summary>
        /// 等待填充SimpleUser。等到aspx执行之时将被正式填充
        /// </summary>
        /// <param name="list"></param>
        public void WaitForFillSimpleUsers<T>(IEnumerable<T> list) where T : IFillSimpleUser
        {
            if (list == null)
                return;

            List<int> waitForFillUserIds;

            if (PageCacheUtil.TryGetValue<List<int>>(cacheKey_FillSimpleUserIds, out waitForFillUserIds) == false)
            {
                waitForFillUserIds = new List<int>();
                PageCacheUtil.Set(cacheKey_FillSimpleUserIds, waitForFillUserIds);
            }

            int userID;
            foreach (T item in list)
            {
                if (item == null)
                    continue;

                userID = item.GetUserIDForFill();

                if (waitForFillUserIds.Contains(userID) == false)
                    waitForFillUserIds.Add(userID);
            }
        }

        #endregion

        /// <summary>
        /// 提交等待填充的SimpleUser
        /// </summary>
        public void SubmitFillSimpleUsers()
        {

            List<int> waitForFillUserIds;

            if (PageCacheUtil.TryGetValue<List<int>>(cacheKey_FillSimpleUserIds, out waitForFillUserIds))
            {

                //Stopwatch s = new Stopwatch();
                //s.Start();

                GetSimpleUsers(waitForFillUserIds, GetUserOption.WithAll);

                //s.Stop();
                //LogHelper.CreateErrorLog2("InitLogs", "SubmitFillSimpleUsers:" + s.ElapsedMilliseconds + "s");

                PageCacheUtil.Remove(cacheKey_FillSimpleUserIds);
            }

        }


        #region FillSimpleUser

        /// <summary>
        /// 立即填充SimpleUser
        /// </summary>
        /// <param name="list"></param>
        public void FillSimpleUser<T>(T item, int actionType) where T : IFillSimpleUsers
        {
            if (item == null)
                return;

            List<int> waitForFillUserIds = new List<int>();

            int[] userIds = item.GetUserIdsForFill(actionType);

            foreach (int userID in userIds)
            {
                if (waitForFillUserIds.Contains(userID) == false)
                    waitForFillUserIds.Add(userID);
            }

            GetSimpleUsers(waitForFillUserIds, GetUserOption.WithAll);
        }


        /// <summary>
        /// 立即填充SimpleUser
        /// </summary>
        /// <param name="list"></param>
        public void FillSimpleUser<T>(T item) where T : IFillSimpleUser
        {
            if (item == null)
                return;

            int userID = item.GetUserIDForFill();

            GetSimpleUser(userID, GetUserOption.WithAll);
        }

        #endregion

        #region FillSimpleUsers

        /// <summary>
        /// 立即填充SimpleUser
        /// </summary>
        /// <param name="list"></param>
        public void FillSimpleUsers<T>(IEnumerable<T> list, int actionType) where T : IFillSimpleUsers
        {
            if (list == null)
                return;

            List<int> waitForFillUserIds = new List<int>();

            int[] userIds;
            foreach (T item in list)
            {
                if (item == null)
                    continue;

                userIds = item.GetUserIdsForFill(actionType);

                foreach (int userID in userIds)
                {
                    if (waitForFillUserIds.Contains(userID) == false)
                        waitForFillUserIds.Add(userID);
                }
            }

            GetSimpleUsers(waitForFillUserIds, GetUserOption.WithAll);
        }

        /// <summary>
        /// 立即填充SimpleUser。等到aspx执行之时将被正式填充
        /// </summary>
        /// <param name="list"></param>
        public void FillSimpleUsers<T>(IEnumerable<T> list) where T : IFillSimpleUser
        {
            if (list == null)
                return;

            List<int> waitForFillUserIds = new List<int>();

            int userID;
            foreach (T item in list)
            {
                if (item == null)
                    continue;

                userID = item.GetUserIDForFill();

                if (waitForFillUserIds.Contains(userID) == false)
                    waitForFillUserIds.Add(userID);
            }

            GetSimpleUsers(waitForFillUserIds, GetUserOption.WithAll);
        }

        #endregion

        #region CacheSimpleUsers

        [Obsolete("请使用WaitForFillSimpleUsers方法")]
        public SimpleUserCollection CacheSimpleUsers(IEnumerable<int> userIds)
        {
            return GetSimpleUsers(userIds);
        }

        [Obsolete("请使用WaitForFillSimpleUsers方法")]
        public SimpleUserCollection CacheSimpleUsers(params IEnumerable<int>[] userIdsCollection)
        {
            List<int> userIds = new List<int>();

            for (int i = 0; i < userIdsCollection.Length; i++)
            {
                foreach (int id in userIdsCollection[i])
                {
                    if (!userIds.Contains(id))
                    {
                        userIds.Add(id);
                    }
                }
            }
            return GetSimpleUsers(userIds);
        }

        #endregion

        #endregion

        #region 检测

        [Obsolete]
        public bool ValidateLoginStatus()
        {
            if (SafeMode)
            {
                if (this.IsExecutorLogin)
                    return true;

                else
                {
                    ThrowError(new NotLoginError());
                    return false;
                }
            }
            else
                return true;
        }

        #region  检查用户名

        private static Regex regChar = new Regex(@"[~!#\$%\^&\*\(\)\+=;:""'\|\\/\?<>,\[\]\{\}]+", RegexOptions.Compiled);
        /// <summary>
        /// 根据系统设置或权限检查用户名是否正确
        /// 
        /// 错误：
        /// EmptyUsernameError
        /// UsernameLengthError
        /// UsernameCharError
        /// UsernameHasForbiddenWordsError
        /// </summary>
        public bool ValidateUsername(string username, string paramName)
        {
            //检查用户名是否为空
            if (username == null || username.Trim() == string.Empty)
            {
                ThrowError(new EmptyUsernameError(paramName));
                return false;
            }

            username = username.Trim();


            MatchCollection matchs = regChar.Matches(username);

            if (matchs.Count > 0)
            {
                string otherChars = string.Empty;
                foreach (Match m in matchs)
                {
                    otherChars += m.Value + " ";
                }
                ThrowError(new UsernameCharError(paramName, username, otherChars));
                return false;
            }

            //检查用户名长度
            int usernameLength = StringUtil.GetByteCount(username);
            int maxLength = AllSettings.Current.RegisterLimitSettings.UserNameLengthScope.MaxValue;
            int minLength = AllSettings.Current.RegisterLimitSettings.UserNameLengthScope.MinValue;
            if (usernameLength < minLength
                || usernameLength > maxLength)
            {
                ThrowError(new UsernameLengthError(paramName, username, maxLength, minLength));
                return false;
            }

            //检查是否允许的字符类型
            if (AllSettings.Current.RegisterLimitSettings.AllowUsernames.IsMach(username) == false)
            {
                ThrowError(new UsernameCharError(paramName, username));
                return false;
            }


            //检测非法字符,关键字
            if (AllSettings.Current.RegisterLimitSettings.UserNameForbiddenWords.IsMach(username))
            {
                ThrowError(new UsernameHasForbiddenWordsError(paramName, username));
                return false;
            }
            if (GetUser(username) != null) //检测用户名是否被占用   
            {
                ThrowError(new UsernameIsExistsError("username", username));
                return false;
            }

            return true;
        }

        #endregion

        #region 检查Email是否可用
        /// <summary>
        /// 检查Email是否可用
        /// 
        /// 错误：
        /// EmptyEmailError
        /// EmailFormatError
        /// EmailForbiddenError
        /// </summary>
        /// <returns></returns>
        public bool ValidateEmail(string email, string paramName)
        {
            if (string.IsNullOrEmpty(email))
            {
                ThrowError(new EmptyEmailError(paramName));
                return false;
            }

            //检查格式是否正确
            if (!ValidateUtil.IsEmail(email))
            {
                ThrowError(new EmailFormatError(paramName, email));
                return false;
            }

            //检查email是否被管理员禁止
            if (
                (AllSettings.Current.RegisterLimitSettings.RegisterEmailLimitMode == LimitMode.Allow
                && !AllSettings.Current.RegisterLimitSettings.RegisterEmailLimitList.IsMach(email))
                ||
                (AllSettings.Current.RegisterLimitSettings.RegisterEmailLimitMode == LimitMode.Reject
                && AllSettings.Current.RegisterLimitSettings.RegisterEmailLimitList.IsMach(email))
                )
            {
                ThrowError(new EmailForbiddenError(paramName, email));
                return false;
            }

            if (UserDao.Instance.EmailIsExsits(email))
            {
                ThrowError(new DuplicateEmailError(paramName, email));
            }


            return true;
        }
        #endregion

        //#region 检测IP
        //public bool CheckIP(string input)
        //{
        //    RegisterLimitSettings limitSettings = AllSettings.Current.RegisterLimitSettings;

        //    ////在多少分钟内是否存在某IP //TODO:存储过程
        //    //if (AllSettings.Current.RegisterLimitSettings.TimeSpanForContinuousRegister != 0 && IPIsExistInMinutes(input, AllSettings.Current.RegisterLimitSettings.TimeSpanForContinuousRegister))
        //    //    return new RegisterFrequent();

        //    if (limitSettings.RegisterIPLimitMode == LimitModes.Free)
        //        return null;
        //    else if (
        //            (limitSettings.RegisterIPLimitMode == LimitModes.Allow && !limitSettings.RegisterIPLimitList.IsMatch(input))
        //            ||
        //            (limitSettings.RegisterIPLimitMode == LimitModes.Reject && limitSettings.RegisterIPLimitList.IsMatch(input))
        //            )
        //    {
        //        return new IPIsLimitToReg();
        //    }
        //    else
        //        return null;
        //}
        //#endregion

        //#region 检测邀请码
        ///// <summary>
        ///// 检查邀请码
        ///// </summary>
        //public bool CheckInviteCode(string input)
        //{
        //    if (string.IsNullOrEmpty(input))
        //        return false;
        //    else
        //    {
        //        try
        //        {
        //            Guid g = new Guid(input);
        //            if (InviteBO.Instance.GetInviteSerial(g) != null)
        //                return true;
        //            else
        //                return false;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //#endregion


        public bool CanRegister(string ip)
        {

            switch (AllSettings.Current.RegisterLimitSettings.RegisterIPLimitMode)
            {
                case LimitMode.Allow:
                    if (!AllSettings.Current.RegisterLimitSettings.RegisterIPLimitList.IsMatch(ip))
                    {
                        ThrowError(new RegisterIPLimitError());
                        return false;
                    }
                    break;
                case LimitMode.Reject:
                    {
                        if (AllSettings.Current.RegisterLimitSettings.RegisterIPLimitList.IsMatch(ip))
                        {
                            ThrowError(new RegisterIPLimitError());
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        #endregion

        #region 积分

        public void TryResetUserPointsCache(int userID)
        {
            if (userID > 0)
            {
                User user = GetUserFromCache<User>(userID);
                if (user != null)
                {
                    UserDao.Instance.FillUserPoints(user);
                }
            }
        }

        /// <summary>
        /// 返回积分管理器
        /// </summary>
        private UserPointAction PointManager
        {
            get
            {
                return UserPointAction.Instance;
            }
        }

        /// <summary>
        /// 取得用户的积分信息
        /// </summary>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        public List<PointInfo> GetUserPointInfos(int targetUserID)
        {
            User user = UserBO.Instance.GetUser(targetUserID);
            UserPointCollection pointCollection = AllSettings.Current.PointSettings.EnabledUserPoints;
            List<PointInfo> Points = new List<PointInfo>();
            PointInfo pointInfo;
            if (user != null)
            {
                foreach (UserPoint p in pointCollection)
                {
                    pointInfo = new PointInfo();
                    pointInfo.Index = (int)p.Type;
                    pointInfo.Name = p.Name;
                    pointInfo.Value = user.ExtendedPoints[pointInfo.Index];
                    pointInfo.MaxValue = p.MaxValue;
                    pointInfo.MinValue = p.MinValue;
                    pointInfo.Icon = AllSettings.Current.PointSettings.PointIcons.GetPointIcon(p.Type, pointInfo.Value);
                    Points.Add(pointInfo);
                }
            }

            return Points;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <param name="points">用户的最终积分</param>
        public void UpdateUserPoints(AuthUser operatorUser, int targetUserID, int[] points ,string operateName,string remarks)
        {
            if (CanEditUserPoints(operatorUser, targetUserID))
            {
                foreach (UserPoint userPoint in AllSettings.Current.PointSettings.UserPoints)
                {
                    int pointID = (int)userPoint.Type;

                    if (userPoint.Enable)
                    {
                        if (points[pointID] < userPoint.MinValue)
                            points[pointID] = userPoint.MinValue;

                        else if (points[pointID] > userPoint.MaxValue)
                            points[pointID] = userPoint.MaxValue;
                    }
                }

                int generalPoint;
                UserDao.Instance.SetUserPoint(targetUserID, points, out generalPoint,operateName,remarks);

                User user = GetUserFromCache(targetUserID);
                if (user != null)
                {
                    user.ExtendedPoints = points;
                    user.Points = generalPoint;
                }
            }
            else
            {
                ThrowError(new NoPermissionEditUserPointError());
            }
        }

        public void UpdateAllUserPoints()
        {
            int[] maxPoints = new int[8];
            int[] minPoints = new int[8];

            UserPointCollection userPoints = AllSettings.Current.PointSettings.UserPoints;
            for (int i = 0; i < 8; i++)
            {
                UserPoint userPoint = userPoints[i];
                if (userPoint.Enable)
                {
                    int maxValue = userPoint.MaxValue;
                    if (AllSettings.Current.PaySettings.EnablePointRecharge)
                    {
                        foreach (PointRechargeRule rule in AllSettings.Current.PointSettings.PointRechargeRules)
                        {
                            if (rule.UserPointType == userPoint.Type && rule.Enable)
                            {
                                maxValue = int.MaxValue;
                                break;
                            }
                        }
                    }
                    maxPoints[i] = maxValue;
                    minPoints[i] = userPoint.MinValue;
                }
                else
                {
                    maxPoints[i] = int.MaxValue;
                    minPoints[i] = int.MinValue;
                }
            }
            UserDao.Instance.UpdateAllUserPoints(maxPoints, minPoints);

            RemoveAllUserCache();
        }

        /// <summary>
        /// 更新用户积分（正数加分、负数减分）
        /// 可能抛出的错误：UserPointOverMaxValueError UserPointOverMinValueError
        /// </summary>
        /// <param name="throwOverMaxValueError">高于上限 是否抛出错误,如果为false不抛错 则取系统设置的最大值</param>
        /// <param name="throwOverMinValueError">低于下限 是否抛出错误,如果为false不抛错 则取系统设置的最小值</param>
        public bool UpdateUserPoints(int userID, bool throwOverMaxValueError, bool throwOverMinValueError, int[] points,string operateName,string remarks)
        {
            return UpdateUserPoints(userID, throwOverMaxValueError, throwOverMinValueError, points[0], points[1], points[2], points[3], points[4], points[5], points[6], points[7],operateName,remarks);
        }

        /// <summary>
        /// 更新用户积分（正数加分、负数减分）
        /// </summary>
        /// <param name="throwOverMaxValueError">高于上限 是否抛出错误,如果为false不抛错 则取系统设置的最大值</param>
        /// <param name="throwOverMinValueError">低于下限 是否抛出错误,如果为false不抛错 则取系统设置的最小值</param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="point5"></param>
        /// <param name="point6"></param>
        /// <param name="point7"></param>
        /// <param name="point8"></param>
        public bool UpdateUserPoints(int userID, bool throwOverMaxValueError, bool throwOverMinValueError, int point1, int point2, int point3, int point4, int point5, int point6, int point7, int point8,string operateName,string remarks)
        {
            int[] points = new int[8] { point1, point2, point3, point4, point5, point6, point7, point8 };
            bool mustUpdate = false;
            foreach (int point in points)
            {
                if (point != 0)
                {
                    mustUpdate = true;
                    break;
                }
            }
            if (mustUpdate == false)//全都是等于0  不需要更新
                return true;

            return UpdateUserPoint(userID, throwOverMinValueError, throwOverMaxValueError, points, operateName, remarks);
        }

        /// <summary>
        /// 积分兑换
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="pointType"></param>
        /// <param name="targetPointType"></param>
        /// <param name="pointValue">兑换的值</param>
        /// <returns></returns>
        public bool ExechangePoint(AuthUser operatorUser, string password, UserPointType pointType, UserPointType targetPointType, int pointValue)
        {
            if (AllSettings.Current.PointSettings.EnablePointExchange == false)
            {
                ThrowError(new CustomError("", "系统未开启积分兑换功能"));
                return false;
            }

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
            }

            if (pointValue < 1)
            {
                ThrowError<UserPointExchangePointValueError>(new UserPointExchangePointValueError("pointValue"));
            }

            if (HasUnCatchedError)
                return false;

            //加密后结果和数据库结果对比 //TODO:密码错误日志
            if (!SecurityUtil.ComparePassword(operatorUser.PasswordFormat, password, operatorUser.Password))
            {
                ThrowError<PasswordError>(new PasswordError("password"));
                return false;
            }

            UserPoint userPoint = AllSettings.Current.PointSettings.GetUserPoint(pointType);
            UserPoint targetUserPoint = AllSettings.Current.PointSettings.GetUserPoint(targetPointType);

            PointExchangeRule rule = AllSettings.Current.PointSettings.PointExchangeRules.GetRule(pointType, targetPointType);
            if (rule == null)
            {
                ThrowError<UserPointExchangeRuleNotExistsError>(new UserPointExchangeRuleNotExistsError("UserPointExchangeRuleNotExistsError", userPoint.Name, targetUserPoint.Name));
                return false;
            }

            int proportion = AllSettings.Current.PointSettings.ExchangeProportions.GetProportion(pointType);
            int targetProportion = AllSettings.Current.PointSettings.ExchangeProportions.GetProportion(targetPointType);
            int targetPointValue = pointValue * targetProportion * (100 - rule.TaxRate) / (proportion * 100);

            int[] points = new int[Consts.PointCount];

            points[(int)pointType] = -pointValue;
            points[(int)targetPointType] = targetPointValue;

            int[] minValues = new int[8];
            minValues[(int)pointType] = rule.MinRemainingValue;

            int point;
            int result = UpdateUserPoint3(operatorUser.UserID, true, false, points, minValues, null, out point,"积分兑换",
                string.Concat("从", userPoint.Name, "转到", targetUserPoint.Name, ",兑换税率", rule.TaxRate, "%,", "兑换比例1:", targetProportion)
                );

            int remaining = point - pointValue;
            if (result != 0)
            {
                ThrowError<UserPointExchangeMinRemainingError>(new UserPointExchangeMinRemainingError("UserPointExchangeMinRemainingError", userPoint.Name, rule.MinRemainingValue, remaining));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 转帐
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="targetUserName">要转给的用户</param>
        /// <param name="pointType">积分类型</param>
        /// <param name="pointValue">数量</param>
        /// <returns></returns>
        public bool TransferPoint(AuthUser operatorUser, string password, string targetUserName, UserPointType pointType, int pointValue)
        {
            if (AllSettings.Current.PointSettings.EnablePointTransfer == false)
            {
                ThrowError(new CustomError("", "系统未开启转帐功能"));
                return false;
            }

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                ThrowError(new EmptyPasswordError("password"));
            }

            if (string.IsNullOrEmpty(targetUserName))
            {
                ThrowError<EmptyUsernameError>(new EmptyUsernameError("targetUserName"));
            }

            if (pointValue < 1)
            {
                ThrowError<User_UserPointTransferPointValueError>(new User_UserPointTransferPointValueError("pointValue"));
            }

            if (HasUnCatchedError)
                return false;

            User targetUser = GetUser(targetUserName);

            if (targetUser == null)
            {
                ThrowError<UserNotExistsError>(new UserNotExistsError("targetUserName", targetUserName));
                return false;
            }

            if (operatorUser.Username == targetUser.Username)
            {
                ThrowError<UserPointCannotTransferToSelfError>(new UserPointCannotTransferToSelfError("UserPointCannotTransferToSelfError"));
                return false;
            }

            //User user = GetUser(operatorUserID);

            //加密后结果和数据库结果对比 //TODO:密码错误日志
            if (!SecurityUtil.ComparePassword(operatorUser.PasswordFormat, password, operatorUser.Password))
            {
                if (!SecurityUtil.ComparePassword(operatorUser.PasswordFormat, HttpUtility.HtmlEncode(password), operatorUser.Password))
                {
                    ThrowError<PasswordError>(new PasswordError("password"));
                    return false;
                }
            }

            PointTransferRule rule = AllSettings.Current.PointSettings.PointTransferRules.GetRule(pointType);

            if (rule == null || !rule.CanTransfer)
            {
                ThrowError<UserPointCannotTransferError>(new UserPointCannotTransferError("UserPointCannotTransferError", AllSettings.Current.PointSettings.GetUserPoint(pointType).Name));
                return false;
            }


            int tempValue = pointValue * (100 + rule.TaxRate) / 100;

            if (pointValue * (100 + rule.TaxRate) % 100 != 0)//有小数  进1
                tempValue = tempValue + 1;

            int[] minValues = new int[8];
            minValues[(int)rule.PointType] = rule.MinRemainingValue;

            int[] points = new int[Consts.PointCount];
            points[(int)pointType] = -tempValue;

            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);

                try
                {
                    bool success = false;
                    int userPoint;
                    int result = UpdateUserPoint3(operatorUser.UserID, true, false, points, minValues, null, out userPoint, "积分转账", string.Concat("转账到用户:", targetUser.Username, ",实际转账", pointValue, ",付税：", (tempValue - pointValue)));
                    if (result != 0)
                    {
                        int remaining = userPoint - tempValue;
                        ThrowError<UserPointTransferMinRemainingError>(new UserPointTransferMinRemainingError("UserPointExchangeMinRemainingError", AllSettings.Current.PointSettings.GetUserPoint(pointType).Name, rule.MinRemainingValue, remaining));
                    }
                    else
                        success = true;

                    if (!success)
                    {
                        context.RollbackTransaction();
                        return false;
                    }
                    points = new int[Consts.PointCount];
                    points[(int)pointType] = pointValue;
                    result = UpdateUserPoint3(targetUser.UserID, false, false, points, null, null, out userPoint, "积分转账", string.Concat("从用户：“", operatorUser.Username, "”转入积分"));
                    if (result != 0)
                    {
                        context.RollbackTransaction();
                        return false;
                    }
                    context.CommitTransaction();
                    return true;
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }
            }

        }

        /// <summary>
        /// 积分交易
        /// </summary>
        /// <param name="fromUserID">付出积分的人</param>
        /// <param name="toUserID">获得积分的人</param>
        /// <param name="pointValue">积分值 必须为大于0</param>
        /// <param name="pointType">积分类型</param>
        /// <param name="ignoreTax">是否忽略税率</param>
        /// <param name="getTaxFromTargetUser">true指税由获得积分人的交，false指税由付出积分的人交</param>
        /// <returns></returns>
        public bool TradePoint(int fromUserID, int toUserID, int pointValue, UserPointType pointType, bool ignoreTax, bool getTaxFromTargetUser, PointActionManager.TryUpdateUserPointCallback beforeUpdatePoint)
        {
            if (pointValue < 1)
            {
                ThrowError<User_UserPointTradePointValueError>(new User_UserPointTradePointValueError("pointValue"));
            }

            if (HasUnCatchedError)
                return false;

            User user = GetUser(fromUserID);
            if (user == null)
            {
                ThrowError<UserNotExistsError>(new UserNotExistsError("fromUserID", fromUserID.ToString()));
                return false;
            }

            User targetUser = GetUser(toUserID);

            if (targetUser == null)
            {
                ThrowError<UserNotExistsError>(new UserNotExistsError("toUserID", toUserID.ToString()));
                return false;
            }

            int taxPoint = 0;
            if (ignoreTax == false)
            {
                taxPoint = pointValue * AllSettings.Current.PointSettings.TradeRate / 100;

                if (pointValue * AllSettings.Current.PointSettings.TradeRate % 100 != 0)//有小数  进1
                    taxPoint = taxPoint + 1;
            }


            int pay = 0, get = 0;
            if (getTaxFromTargetUser)
            {
                pay = pointValue;
                get = pointValue - taxPoint;
            }
            else
            {
                pay = pointValue + taxPoint;
                get = pointValue;
            }


            using (BbsContext context = new BbsContext())
            {
                context.BeginTransaction(IsolationLevel.ReadUncommitted);

                int[] points = new int[8];
                points[(int)pointType] = 0 - pay;

                try
                {
                    bool success;
                    if (beforeUpdatePoint != null)
                    {
                        success = beforeUpdatePoint(PointActionManager.TryUpdateUserPointState.CheckSucceed);
                        if (success == false)
                        {
                            context.RollbackTransaction();
                            return false;
                        }
                    }

                    success = UpdateUserPoint(fromUserID, true, false, points, "积分转账",
                        string.Concat(
                        "转出到", targetUser.Username,pointValue, (!getTaxFromTargetUser ? ",付出交易税" + taxPoint : "")));
                    if (success)
                    {
                        points = new int[8];
                        points[(int)pointType] = get;

                        success = UpdateUserPoint(toUserID, false, false, points, "积分转账",
                            string.Concat( "从" , user.Username , "转入" , pointValue, (getTaxFromTargetUser ? ",扣除税率" + taxPoint : "")));
                        if (success == false)
                        {
                            context.RollbackTransaction();
                            return false;
                        }
                        else
                        {
                            context.CommitTransaction();
                        }
                    }
                    else
                    {
                        context.RollbackTransaction();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    context.RollbackTransaction();
                    throw ex;
                }

                return true;
            }
        }

        public bool UpdateUserPoints(int userID, bool throwOverMaxValue, bool throwOverMinValue, int[] points, int count, PointActionManager.TryUpdateUserPointCallback beforeUpdate, string operateName, string remarks)
        {
            if (beforeUpdate == null)
            {
                return UserBO.Instance.UpdateUserPoints(userID, throwOverMaxValue, throwOverMinValue, points[0] * count, points[1] * count, points[2] * count, points[3] * count, points[4] * count, points[5] * count, points[6] * count, points[7] * count,operateName,remarks);
            }

            bool mustUpdate = false;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != 0)
                {
                    points[i] = points[i] * count;
                    mustUpdate = true;
                }
            }
            bool updatePoint;
            if (mustUpdate == false)//全都是等于0  不需要更新
            {
                updatePoint = beforeUpdate(PointActionManager.TryUpdateUserPointState.CheckSucceed);
                //sek 2010-4-26
                return updatePoint;
                //2010-4-26以前
                //return true;
            }

            User user = GetUser(userID);
            if (user == null)
            {
                updatePoint = beforeUpdate(PointActionManager.TryUpdateUserPointState.CheckFailed);
                return false;
            }


            int[] resultPoints;
            int generalPoint;
            lock (user.UpdateUserPointLocker)
            {
                if (CheckUserPoint(userID, throwOverMinValue, throwOverMaxValue, points))
                {
                    //using (BbsContext context = new BbsContext())
                    //{
                    //    context.BeginTransaction();
                    try
                    {
                        updatePoint = beforeUpdate(PointActionManager.TryUpdateUserPointState.CheckSucceed);
                        if (updatePoint)
                        {
                            UpdateUserPoint(userID, false, false, false, points, out resultPoints, out generalPoint,operateName,remarks);
                            //context.CommitTransaction();
                            //return true;
                        }
                        else
                        {
                            //context.RollbackTransaction();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        updatePoint = beforeUpdate(PointActionManager.TryUpdateUserPointState.CheckFailed);
                        //context.RollbackTransaction();
                        throw ex;
                    }
                    //}
                }
                else
                {
                    updatePoint = beforeUpdate(PointActionManager.TryUpdateUserPointState.CheckFailed);
                    return false;
                }
            }

            if (resultPoints != null)
            {
                user.Points = generalPoint;
                user.ExtendedPoints = resultPoints;
            }

            return true;
        }

        /// <summary>
        /// 调用此方法时 外面的要更新积分的用户不能加锁
        /// </summary>
        public bool UpdateUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, string operateName, string remarks)
        {
            int point;
            int result = UpdateUserPoint3(userID, throwOverMinValueError, throwOverMaxValueError, points, null, null, out point, operateName, remarks);

            return CheckPointResult(result, point, points);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="updateUserCache">为true时 外面的要更新积分的用户不能加锁</param>
        /// <param name="throwOverMinValueError"></param>
        /// <param name="throwOverMaxValueError"></param>
        /// <param name="points"></param>
        /// <param name="resultPoints"></param>
        /// <param name="generalPoint"></param>
        /// <returns></returns>
        public bool UpdateUserPoint(int userID, bool updateUserCache, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, out int[] resultPoints, out int generalPoint, string operateName, string remarks)
        {
            int point;
            int result = UpdateUserPoint3(userID, updateUserCache, throwOverMinValueError, throwOverMaxValueError, points, null, null, out point, out resultPoints, out generalPoint,operateName,remarks);

            return CheckPointResult(result, point, points);
        }

        /// <summary>
        /// 调用此方法时 外面的要更新积分的用户不能加锁
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="throwOverMinValueError"></param>
        /// <param name="throwOverMaxValueError"></param>
        /// <param name="points"></param>
        /// <param name="minValues"></param>
        /// <param name="maxValues"></param>
        /// <param name="userPoint"></param>
        /// <returns></returns>
        public int UpdateUserPoint3(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int userPoint, string operateName, string remarks)
        {
            int[] resultPoints;
            int generalPoint;
            return UpdateUserPoint3(userID, true, throwOverMinValueError, throwOverMaxValueError, points, minValues, maxValues, out userPoint, out resultPoints, out generalPoint, operateName, remarks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="updateUserCache">为true时 外面的要更新积分的用户不能加锁</param>
        /// <param name="throwOverMinValueError"></param>
        /// <param name="throwOverMaxValueError"></param>
        /// <param name="points"></param>
        /// <param name="minValues"></param>
        /// <param name="maxValues"></param>
        /// <param name="userPoint"></param>
        /// <param name="resultPoints"></param>
        /// <param name="generalPoint"></param>
        /// <returns></returns>
        public int UpdateUserPoint3(int userID, bool updateUserCache, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int userPoint, out int[] resultPoints, out int generalPoint, string operateName, string remarks)
        {
            bool isNullMinvalues = false;
            bool isNullMaxvalues = false;
            if (minValues == null)
            {
                minValues = new int[8];
                isNullMinvalues = true;
            }
            if (maxValues == null)
            {
                maxValues = new int[8];
                isNullMaxvalues = true;
            }

            foreach (UserPoint point in AllSettings.Current.PointSettings.UserPoints)
            {
                int tempPointID = (int)point.Type;
                if (isNullMinvalues || minValues[tempPointID] < point.MinValue)
                    minValues[tempPointID] = point.MinValue;

                if (isNullMaxvalues || maxValues[tempPointID] > point.MaxValue)
                    maxValues[tempPointID] = point.MaxValue;
            }
            int result = UpdateUserPoint2(userID, updateUserCache, throwOverMinValueError, throwOverMinValueError, points, minValues, maxValues, out userPoint, out resultPoints, out generalPoint,operateName,remarks);
            return result;
        }

        private int UpdateUserPoint2(int userID, bool updateUserCache, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int userPoint, out int[] resultPoints, out int generalPoint, string operateName, string remarks)
        {
            resultPoints = null;
            generalPoint = 0;
            userPoint = 0;
            //积分都为0时 不要检查
            bool mustCheck = false;
            foreach (int p in points)
            {
                if (p != 0)
                {
                    mustCheck = true;
                    break;
                }
            }
            if (mustCheck == false)
            {
                return 0;
            }

            int result = UserDao.Instance.UpdateUserPoint(userID, throwOverMinValueError, throwOverMaxValueError, points, minValues, maxValues, out resultPoints, out generalPoint, out userPoint, operateName,remarks);

            if (result == 0 && updateUserCache)
            {
                User user = GetUserFromCache(userID);
                if (user != null)
                {
                    user.ExtendedPoints = resultPoints;
                    user.Points = generalPoint;
                }
            }

            return result;

        }



        private bool CheckPointResult(int result, int point, int[] points)
        {
            bool success = false;
            if (result == 0)
                success = true;
            else if (result < 0 && result > -9)
            {
                int pointID = 0 - result - 1;
                UserPoint userPoint = AllSettings.Current.PointSettings.UserPoints.GetUserPoint((UserPointType)pointID);
                ThrowError<UserPointOverMinValueError>(new UserPointOverMinValueError("UserPointOverMinValueError", userPoint.Type, point - userPoint.MinValue, point + points[pointID], userPoint.MinValue));
            }
            else if (result > 0 && result < 9)
            {
                int pointID = result - 1;
                UserPoint userPoint = AllSettings.Current.PointSettings.UserPoints.GetUserPoint((UserPointType)pointID);
                ThrowError<UserPointOverMaxValueError>(new UserPointOverMaxValueError("UserPointOverMaxValueError", userPoint.Type, userPoint.MaxValue - point, point + points[pointID], userPoint.MaxValue));
            }

            return success;
        }

        public bool CheckUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points)
        {
            if (throwOverMinValueError == false && throwOverMaxValueError == false)
                return true;

            int[] minValues = new int[8];
            int[] maxValues = new int[8];
            foreach (UserPoint tempPoint in AllSettings.Current.PointSettings.UserPoints)
            {
                int tempPointID = (int)tempPoint.Type;
                minValues[tempPointID] = tempPoint.MinValue;
                maxValues[tempPointID] = tempPoint.MaxValue;
            }
            int point;
            int result = CheckUserPoint(userID, throwOverMinValueError, throwOverMaxValueError, points, minValues, maxValues, out point);

            return CheckPointResult(result, point, points);
        }

        /// <summary>
        /// 检查积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="throwOverMinValueError"></param>
        /// <param name="throwOverMaxValueError"></param>
        /// <param name="points"></param>
        /// <param name="minValues"></param>
        /// <param name="maxValues"></param>
        /// <param name="point"></param>
        /// <returns>返回值-8到8 0为检查通过 -1到-8表示积分超出下限  如-1表示积分1超出下限 1到8表示积分超出上限  如1表示积分1超出上限</returns>
        public int CheckUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int point)
        {
            point = 0;
            if (throwOverMinValueError == false && throwOverMaxValueError == false)
                return 0;

            //积分都为0时 不要检查
            bool mustCheck = false;
            foreach (int p in points)
            {
                if (p != 0)
                {
                    mustCheck = true;
                    break;
                }
            }
            if (mustCheck == false)
                return 0;

            if (minValues == null)
            {
                minValues = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    minValues[i] = int.MinValue;
                }
            }
            if (maxValues == null)
            {
                maxValues = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    maxValues[i] = int.MaxValue;
                }
            }

            return UserDao.Instance.CheckUserPoint(userID, throwOverMinValueError, throwOverMaxValueError, points, minValues, maxValues, out point);
        }


        /// <summary>
        /// 积分公式可用的字段
        /// </summary>
        /// <returns></returns>
        public PointExpressionColumCollection GetGeneralPointExpressionColums()
        {
            return UserDao.Instance.GetGeneralPointExpressionColums();
        }

        /// <summary>
        /// 更新总积分公式
        /// </summary>
        /// <param name="generalPointExpression"></param>
        /// <returns></returns>
        public bool UpdatePointsExpression(string generalPointExpression)
        {

            bool success = GetPointsExpression(ref generalPointExpression);

            if (success == false)
                return false;

            UserDao.Instance.UpdatePointsExpression(generalPointExpression);

            //RemoveAllUserCache();

            return true;
        }

        public bool ReCountUsersPoints(int startUserID, int updateCount, out int endUserID)
        {
            endUserID = 0;
            string key = AllSettings.Current.PointSettings.GeneralPointExpression;

            string result;

            bool hasCache = false;
            string expression = key;
            if (CacheUtil.TryGetValue<string>(key, out result))
            {
                hasCache = true;
            }
            else
            {
                bool success = GetPointsExpression(ref expression);
                if (success == false)
                    return false;

            }

            UserDao.Instance.ReCountUsersPoints(expression, startUserID, updateCount, out endUserID, ref result);

            if (hasCache == false)
            {
                CacheUtil.Set<string>(key, result, CacheTime.Normal, CacheExpiresType.Sliding);
            }

            RemoveAllUserCache();

            return true;
        }


        private bool GetPointsExpression(ref string generalPointExpression)
        {
            if (string.IsNullOrEmpty(generalPointExpression))
            {
                ThrowError<UserPointEmptyPointsExpressionError>(new UserPointEmptyPointsExpressionError("generalPointExpression"));
                return false;
            }

            string temp = generalPointExpression.Replace("(", "").Replace(")", "").Replace("+", "|").Replace("-", "|").Replace("*", "|").Replace("/", "|").Replace(" ", "");
            string[] colums = temp.Split('|');


            List<string> tempColums = new List<string>();

            PointExpressionColumCollection expressionColums = UserBO.Instance.GetGeneralPointExpressionColums();
            StringBuilder errorColums = new StringBuilder();
            foreach (string colum in colums)
            {
                if (ValidateUtil.IsNumber(colum))
                    continue;
                bool isErrorColum = true;
                foreach (PointExpressionColum expressionColum in expressionColums)
                {
                    if (string.Compare(expressionColum.FriendlyShow, colum, true) == 0)
                    {
                        isErrorColum = false;
                        break;
                    }
                }
                if (isErrorColum && colum != string.Empty)
                    errorColums.Append("\"").Append(colum).Append("\"").Append(",");
                if (colum == string.Empty)
                {
                    ThrowError<UserPointPointsExpressionFormatError>(new UserPointPointsExpressionFormatError("generalPointExpression", generalPointExpression));
                    return false;
                }

                tempColums.Add(colum.ToLower());
            }
            if (errorColums.Length > 0)
            {
                ThrowError<UserPointExpressionColumsError>(new UserPointExpressionColumsError("generalPointExpression", errorColums.ToString(0, errorColums.Length - 1)));
                return false;
            }

            //检查是否有字段 做除数
            colums = generalPointExpression.Split('/');
            int i = 0;
            foreach (string colum in colums)
            {
                i++;
                if (i == 1)
                    continue;
                else
                {
                    string str = null;
                    if (colum.Trim().StartsWith("("))
                    {
                        int j = colum.IndexOf(')');
                        if (j == -1)
                        {
                            str = colum.Substring(1);
                        }
                        else
                        {
                            str = colum.Substring(1, j - 1);
                        }

                        str = str.Replace("+", "|").Replace("-", "|").Replace("*", "|");//.Replace("/", "|");

                        foreach (string field in str.Split('|'))
                        {
                            if (tempColums.Contains(field.Trim().ToLower()))
                            {
                                ThrowError<UserPointExpressionDivisorError>(new UserPointExpressionDivisorError("generalPointExpression", field));
                                return false;
                            }
                        }
                    }
                    else
                    {
                        str = colum;

                        str = str.Replace("+", "|").Replace("-", "|").Replace("*", "|");

                        string firstField = str.Split('|')[0];

                        if (tempColums.Contains(firstField.Trim().ToLower()))
                        {
                            ThrowError<UserPointExpressionDivisorError>(new UserPointExpressionDivisorError("generalPointExpression", firstField));
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region 扩展字段

        private static List<ExtendedFieldType> s_ExtendedFieldTypes = new List<ExtendedFieldType>();

        public void RegisterExtendedFieldType(ExtendedFieldType type)
        {
            lock (s_ExtendedFieldTypes)
            {
                s_ExtendedFieldTypes.Add(type);
            }
        }

        public ExtendedFieldType[] GetRegistedExtendedFieldTypes()
        {
            lock (s_ExtendedFieldTypes)
            {
                return s_ExtendedFieldTypes.ToArray();
            }
        }

        public ExtendedFieldType GetExtendedFieldType(string typeName)
        {
            lock (s_ExtendedFieldTypes)
            {
                foreach (ExtendedFieldType type in s_ExtendedFieldTypes)
                {
                    if (type.GetType().Name == typeName)
                        return type;
                }
            }

            return null;
        }

        /// <summary>
        /// 读取请求中用户扩展字段的值
        /// </summary>
        /// <returns></returns>
        public UserExtendedValueCollection LoadExtendedFieldValues()
        {
            UserExtendedValueCollection result = new UserExtendedValueCollection();

            foreach (ExtendedField field in AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport)
            {
                if (field.IsHidden)
                    continue;

                ExtendedFieldType type = GetExtendedFieldType(field.FieldTypeName);

                if (type != null)
                {
                    UserExtendedValue value = new UserExtendedValue();
                    value.ExtendedFieldID = field.Key;
                    value.Value = type.LoadValueFromRequest(field);
                    if (field.DisplayType == ExtendedFieldDisplayType.UserCustom)
                    {
                        string key = field.Key + "_displaytype";
                        string obj = HttpContext.Current.Request.Form[key];
                        if (obj != null)
                        {
                            int privacyTypeValue;
                            if (int.TryParse(obj, out privacyTypeValue))
                                value.PrivacyType = (ExtendedFieldDisplayType)privacyTypeValue;
                            else
                                value.PrivacyType = ExtendedFieldDisplayType.AllVisible;
                        }
                        else
                            value.PrivacyType = ExtendedFieldDisplayType.AllVisible;
                    }
                    else
                        value.PrivacyType = field.DisplayType;

                    result.Add(value);
                }
            }

            return result;
        }

        public void UpdateExtendedFieldVersion(int userID, string version)
        {
            UserDao.Instance.UpdateExtendedFieldVersion(userID, version);
        }

        #endregion

        #region 权限判断部分

        /// <summary>
        /// 编辑用户资料
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        public bool CanEditUserProfile(AuthUser operatorUser, int targetUserID)
        {
            return ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserProfile, targetUserID);
        }


        /// <summary>
        /// 是否有管理用户资料的权限
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <returns></returns>
        public bool CanManageUser(AuthUser operatorUser)
        {
            return ManagePermissionSet.HasPermissionForSomeone(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserProfile);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        public bool CanDeleteUser(AuthUser operatorUser, int targetUserID)
        {
            return ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.DeleteUser, targetUserID);
        }

        /// <summary>
        /// 实名认证权限
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <returns></returns>
        public bool CanRealnameCheck(AuthUser operatorUser)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_NameCheck);
        }

        public bool CanAvatarCheck(AuthUser operatorUser)
        {
            return AllSettings.Current.BackendPermissions.Can(operatorUser, BackendPermissions.Action.Manage_AvatarCheck);
        }

        /// <summary>
        /// 修改用户组
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        public bool CanEditRole(AuthUser operatorUser, int targetUserID)
        {
            return ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, targetUserID);
        }

        public bool CanLogin(AuthUser operatorUser)
        {
            return PermissionSet.Can(operatorUser, UserPermissionSet.Action.Login);
        }

        public bool CanEditUserAvatar(AuthUser operatorUser, int tagetUserID)
        {
            return ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserProfile, tagetUserID);
        }

        public bool CanEditUserPoints(AuthUser operatorUser, int targetUserID)
        {
            return ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserPoints, targetUserID);
        }


        #region 签名设置部分

        public SignatureFormat GetSignatureFormat(User user)
        {
            return AllSettings.Current.UserSettings.SignatureFormat.GetValue(user);
        }

        public int MaxSignatureLength(User user)
        {
            return AllSettings.Current.UserSettings.SignatureLength.GetValue(user);
        }
        #endregion

        ///// <summary>
        ///// 编辑用户密码
        ///// </summary>
        ///// <param name="operatorUserID"></param>
        ///// <param name="targetUserID"></param>
        ///// <returns></returns>
        //public bool CanEditPassword(int operatorUserID, int targetUserID)
        //{
        //    return ManagePermissionSet.Can(operatorUserID, ManageUserPermissionSet.ActionWithTarget.EditUserPassword, targetUserID);
        //}

        /// <summary>
        /// 修改头像
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserID"></param>
        /// <returns></returns>
        //public bool CanEditAvatar(int operatorUserID, int targetUserID)
        //{
        //    return ManagePermissionSet.Can(operatorUserID, ManageUserPermissionSet.ActionWithTarget.UploadAvatar, targetUserID);
        //}

        /// <summary>
        /// 取得权限对象
        /// </summary>
        private UserPermissionSet PermissionSet
        {
            get
            {
                return AllSettings.Current.UserPermissionSet;
            }
        }

        private ManageUserPermissionSet ManagePermissionSet
        {
            get
            {
                return AllSettings.Current.ManageUserPermissionSet;
            }
        }

        #endregion

        #region 对用户进行用户组操作

        #region AddUsersToRoles


        ///// <summary>
        ///// 将一组用户加入一组用户组，并检查操作者的权限
        ///// </summary>
        ///// <param name="operatorUserID"></param>
        ///// <param name="usersInRoles"></param>
        ///// <returns></returns>
        //public bool AddUsersToRoles(AuthUser operatorUser, UserRoleCollection userRoles)
        //{
        //    if (operatorUser.UserID <= 0)
        //    {
        //        ThrowError(new NotLoginError());
        //        return false;
        //    }

        //    if (userRoles == null || userRoles.Count == 0)
        //    {
        //        ThrowError(new NoUsersAddToRolesError("userRoles", userRoles));
        //        return false;
        //    }

        //    List<int> userIds = new List<int>();

        //    for (int i = 0; i < userRoles.Count; i ++ )
        //    {
        //        UserRole userRole = userRoles[i];

        //        if (userRole.RoleID == Guid.Empty
        //            ||
        //            userRole.Role == null
        //            ||
        //            ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, userRole.UserID) == false
        //            )
        //            userRoles.RemoveAt(i);

        //        else if (userIds.Contains(userRole.UserID) == false)
        //            userIds.Add(userRole.UserID);
        //    }

        //    if (userRoles.Count == 0)
        //    {
        //        return true;
        //    }

        //    UserDao.Instance.AddUsersToRoles(userRoles);

        //    RemoveUsersCache(userIds);

        //    return true;
        //}

        /// <summary>
        /// 将一组用户加入一组用户组
        /// </summary>
        /// <param name="usersInRoles"></param>
        /// <returns></returns>
        internal bool AddUsersToRoles(UserRoleCollection userRoles)
        {
            if (userRoles == null || userRoles.Count == 0)
            {
                ThrowError(new NoUsersAddToRolesError("userRoles", userRoles));
                return false;
            }

            List<int> userIds = new List<int>();

            for (int i = 0; i < userRoles.Count; i++)
            {
                UserRole userRole = userRoles[i];

                if (userRole.RoleID == Guid.Empty || userRole.Role == null || userRole.Role.IsVirtualRole == true)
                    userRoles.RemoveAt(i);

                else if (userIds.Contains(userRole.UserID) == false)
                    userIds.Add(userRole.UserID);
            }

            UserDao.Instance.AddUsersToRoles(userRoles);

            RemoveUsersCache(userIds);

            return true;
        }

        #endregion

        #region AddUsersToRole

        /// <summary>
        /// 将一组用户加入指定的用户组
        /// </summary>
        /// <param name="usersInRoles"></param>
        /// <returns></returns>
        public int AddUsersToRole(AuthUser operatorUser, IEnumerable<int> userIds, Role role, DateTime beginDate, DateTime enddate)// UserRoleCollection userRoles)
        {
            if (role == null || role.IsVirtualRole)
                return 0;

            List<int> hasPermissionUserIDs = new List<int>();

            if (!ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, role))
            {
                ThrowError(new NoPermissionEditThisRoleError(role.Name));
                return 0;
            }

            foreach (int i in userIds)
            {
                if (ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, i))
                {
                    hasPermissionUserIDs.Add(i);
                }
            }

            if (hasPermissionUserIDs.Count == 0)
            {
                return 0;
            }

            UserRoleCollection userRoles = new UserRoleCollection(true);

            UserRole userRole;

            SimpleUserCollection users = GetSimpleUsers(userIds);

            SimpleUser user;
            foreach (int i in hasPermissionUserIDs)
            {
                if (users.TryGetValue(i, out user))
                {
                    userRole = new UserRole();
                    userRole.UserID = i;
                    userRole.Username = user.Username;
                    userRole.RoleID = role.RoleID;
                    userRole.BeginDate = beginDate;
                    userRole.EndDate = enddate;
                    userRoles.Add(userRole);
                }
            }

            UserDao.Instance.AddUsersToRoles(userRoles);

            RemoveUsersCache(hasPermissionUserIDs);

            string ip = IPUtil.GetCurrentIP();

            foreach (UserRole ur in userRoles)
            {
                Logs.LogManager.LogOperation(new Logs.RoleChange(operatorUser.UserID, operatorUser.Username, ur.UserID, ur.Username, role, ip, true));
            }

            return hasPermissionUserIDs.Count;
        }

        /// <summary>
        /// 将一组用户加入一个用户组
        /// </summary>
        internal bool AddUsersToRole(IEnumerable<int> userIds, Role role)
        {
            return AddUsersToRole(userIds, role, DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// 将一组用户加入一个用户组
        /// </summary>
        internal bool AddUsersToRole(IEnumerable<int> userIds, Role role, DateTime beginDate, DateTime endDate)
        {
            if (role == null || role.IsVirtualRole)
                return false;

            UserRoleCollection userRoles = new UserRoleCollection(true);
            UserRole userRole;
            foreach (int i in userIds)
            {
                userRole = new UserRole();
                userRole.UserID = i;
                userRole.RoleID = role.RoleID;
                userRole.BeginDate = beginDate;
                userRole.EndDate = endDate;
                userRoles.Add(userRole);
            }

            UserDao.Instance.AddUsersToRoles(userRoles);

            RemoveUsersCache(userIds);

            return true;
        }

        #endregion

        #region AddUserToRole

        /// <summary>
        /// 添加单个用户到用户组
        /// </summary>
        internal bool AddUserToRole(int userID, Role role)
        {
            return AddUserToRole(userID, role.RoleID, DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// 添加单个用户到用户组
        /// </summary>
        internal bool AddUserToRole(int userID, Guid roleID)
        {
            return AddUserToRole(userID, roleID, DateTime.MinValue, DateTime.MaxValue);
        }

        /// <summary>
        /// 添加单个用户到用户组
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        internal bool AddUserToRole(int userID, Role role, DateTime beginDate, DateTime endDate)
        {
            return AddUserToRole(userID, role.RoleID, beginDate, endDate);
        }

        /// <summary>
        /// 添加单个用户到用户组
        /// </summary>
        internal bool AddUserToRole(int userID, Guid roleID, DateTime beginDate, DateTime endDate)
        {
            UserRole userRole = new UserRole();
            userRole.UserID = userID;
            userRole.RoleID = roleID;
            userRole.BeginDate = beginDate;
            userRole.EndDate = endDate;

            UserRoleCollection collection = new UserRoleCollection(true);
            collection.Add(userRole);

            return AddUsersToRoles(collection);
        }

        #endregion


        //================================================================================================

        #region RemoveUsersFromRoles

        /// <summary>
        /// 将一组用户从一组用户组中移除，这将检查操作者的权限
        /// </summary>
        /// <param name="operatorUser">操作者</param>
        /// <param name="userIds">要操作的一组userID</param>
        /// <param name="roleIds">要从这一组用户组中移除</param>
        /// <returns></returns>
        public bool RemoveUsersFromRoles(AuthUser operatorUser, IEnumerable<int> userIds, IEnumerable<Guid> roleIds)
        {

            if (!ValidateUtil.HasItems<int>(userIds) || !ValidateUtil.HasItems<Guid>(roleIds))
                return false;

            //权限检查



            List<int> hasPermissionUserIDs = new List<int>();

            foreach (int i in userIds)
            {
                if (ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, i))
                {
                    hasPermissionUserIDs.Add(i);
                }
            }

            if (hasPermissionUserIDs.Count == 0)
            {
                return false;
            }

            UserDao.Instance.RemoveUsersFromRoles(hasPermissionUserIDs, roleIds);

            RemoveUsersCache(hasPermissionUserIDs);

            UserCollection users = GetUsers(hasPermissionUserIDs);

            foreach (User user in users)
            {
                Logs.LogManager.LogOperation(new Logs.RoleChange(operatorUser.UserID, operatorUser.Username, user.UserID, user.Username, roleIds, IPUtil.GetCurrentIP(), false));
            }

            return true;
        }


        /// <summary>
        /// 将一组用户从一组用户组移除
        /// </summary>
        /// <returns></returns>
        internal bool RemoveUsersFromRoles(IEnumerable<int> userIds, IEnumerable<Guid> roleIds)
        {
            if (userIds == null || ValidateUtil.HasItems(userIds) == false)
            {
                ThrowError(new NoUsersRemoveFromRolesError("userIds"));
                return false;
            }

            if (roleIds == null || ValidateUtil.HasItems(roleIds) == false)
            {
                ThrowError(new NoUsersRemoveFromRolesError("roleIds"));
                return false;
            }

            UserDao.Instance.RemoveUsersFromRoles(userIds, roleIds);

            RemoveUsersCache(userIds);

            return true;
        }

        #endregion

        #region RemoveUsersFromRole

        /// <summary>
        /// 将一组用户从指定的用户组移除。这将检查操作者的权限，并记录下操作日志
        /// </summary>
        public bool RemoveUsersFromRole(AuthUser operatorUser, IEnumerable<int> userIds, Role role)
        {
            Guid[] roleIds = new Guid[] { role.RoleID };
            if (!ManagePermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserRole, role))
            {
                ThrowError(new NoPermissionEditThisRoleError(role.Name));
                return false;
            }
            return RemoveUsersFromRoles(operatorUser, userIds, roleIds);
        }

        /// <summary>
        /// 将一组用户从指定的用户组移除。这将检查操作者的权限，并记录下操作日志
        /// </summary>
        public bool RemoveUsersFromRole(AuthUser operatorUser, IEnumerable<int> userIds, Guid roleID)
        {
            Role role = AllSettings.Current.RoleSettings.GetRole(roleID);
            return RemoveUsersFromRole(operatorUser, userIds, role);
        }

        //----------------------------------------

        /// <summary>
        /// 将一组用户从指定的用户组移除
        /// </summary>
        internal bool RemoveUsersFromRole(IEnumerable<int> userIds, Role role)
        {
            Guid[] roleIds = new Guid[] { role.RoleID };

            return RemoveUsersFromRoles(userIds, roleIds);
        }

        /// <summary>
        /// 将一组用户从指定的用户组移除
        /// </summary>
        internal bool RemoveUsersFromRole(IEnumerable<int> userIds, Guid roleID)
        {
            Guid[] roleIds = new Guid[] { roleID };

            return RemoveUsersFromRoles(userIds, roleIds);
        }

        #endregion

        #region RemoveUserFromRole

        /// <summary>
        /// 将指定的用户从指定ID的用户组移除，这将检查操作者的权限并记录用户组变更日志
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="targetUserId"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool RemoveUserFromRole(AuthUser operatorUser, int userID, Guid roleID)
        {
            int[] userIds = new int[] { userID };
            Guid[] roleIds = new Guid[] { roleID };

            return RemoveUsersFromRoles(operatorUser, userIds, roleIds);
        }

        public bool RemoveUserFromRole(AuthUser operatorUser, int userID, Role role)
        {
            int[] userIds = new int[] { userID };
            Guid[] roleIds = new Guid[] { role.RoleID };

            return RemoveUsersFromRoles(operatorUser, userIds, roleIds);
        }

        //-----------------------------------

        /// <summary>
        /// 将指定的用户从指定ID的用户组移除
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        internal bool RemoveUserFromRole(int userID, Guid roleID)
        {
            int[] userIds = new int[] { userID };
            Guid[] roleIds = new Guid[] { roleID };

            return RemoveUsersFromRoles(userIds, roleIds);
        }

        /// <summary>
        /// 将指定的用户从指定ID的用户组移除
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        internal bool RemoveUserFromRole(int userID, Role role)
        {
            int[] userIds = new int[] { userID };
            Guid[] roleIds = new Guid[] { role.RoleID };

            return RemoveUsersFromRoles(userIds, roleIds);
        }

        #endregion

        #endregion

        #region  获取各个字段的排名

        public Dictionary<string, int> GetUserRankInfo(int userID, params string[] fields)
        {
            return UserDao.Instance.GetUserRankInfo(userID, fields);
        }

        #endregion

        #region 在线

        ///// <summary>
        ///// 修改隐身状态
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="invisible"></param>
        ///// <returns></returns>
        //public bool UpdateUserOnlineStatus(int userID, OnlineStatus onlineStatus)
        //{
        //    AuthUser user = GetUserFromCache<AuthUser>(userID);
        //    if (user != null)
        //    {
        //        if (user.OnlineStatus == onlineStatus)
        //            return true;
        //    }

        //    UserDao.Instance.UpdateUserOnlineStatus(userID, onlineStatus);

        //    if (user != null)
        //    {
        //        user.OnlineStatus = onlineStatus;
        //    }

        //    return true;
        //}

        /// <summary>
        /// 更新一组用户的在线时长。请注意不要让列表超过300项，否则将无法正常更新
        /// </summary>
        /// <param name="userOnlineInfos"></param>
        /// <returns></returns>
        public bool UpdateOnlineTime300(List<UserOnlineInfo> userOnlineInfos)
        {
            if (userOnlineInfos == null || userOnlineInfos.Count == 0)
                return true;

            if (userOnlineInfos.Count > 400)
                throw new ArgumentException("请不要让列表超过300项，否则将无法正常更新");

            //return UserDao.Instance.UpdateOnlineTime(onlineTimes, userIDs, lastUpdateOnlineTimes, out totalOnlineTimes, out monthOnlineTimes, out weekOnlineTimes, out dayOnlineTimes);

            int[] totalOnlineTimes = null, monthOnlineTimes = null, weekOnlineTimes = null, dayOnlineTimes = null;
            bool success;

            try
            {
                success = UserDao.Instance.UpdateOnlineTime(userOnlineInfos, out totalOnlineTimes, out monthOnlineTimes, out weekOnlineTimes, out dayOnlineTimes);
            }
            catch (Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
                success = false;
            }

            if (success)
            {
                for (int i = 0; i < userOnlineInfos.Count; i++)
                {
                    int userID = userOnlineInfos[i].UserID;
                    User user = GetUserFromCache(userID);

                    if (user != null)
                    {
                        user.TotalOnlineTime = totalOnlineTimes[i];
                        user.MonthOnlineTime = monthOnlineTimes[i];
                        user.WeekOnlineTime = weekOnlineTimes[i];
                        user.DayOnlineTime = dayOnlineTimes[i];
                        user.LastVisitDate = DateTimeUtil.Now;
                    }
                }
            }

            return success;

        }

        public bool UpdateOnlineTime(OnlineMember member)
        {
            int minutes = (int)(DateTimeUtil.Now - member.CreateDate).TotalMinutes;

            List<int> onlineTimes = new List<int>();
            List<int> userIDs = new List<int>();
            List<DateTime> lastUpdateOnlineTimes = new List<DateTime>();
            onlineTimes.Add(minutes);
            userIDs.Add(member.UserID);
            lastUpdateOnlineTimes.Add(member.UpdateDate);

            List<UserOnlineInfo> infos = new List<UserOnlineInfo>();

            UserOnlineInfo info = new UserOnlineInfo();
            info.UserID = member.UserID;
            info.OnlineMinutes = minutes;
            info.UpdateDate = member.UpdateDate;

            infos.Add(info);

            return UpdateOnlineTime300(infos);
        }

        #endregion

        #region Notiry通知

        /// <summary>
        /// 获取用户的通知设置
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserNotifySetting GetNotifySetting(int userID)
        {
            string cacheKey = string.Format(cacheKey_NotifySetting, userID);
#if Passport 
       

            User user = GetUserFromCache(userID);
            if (user != null)
            {
                CacheUtil.Remove(cacheKey);
                return user.NotifySetting;
            }
#endif
            UserNotifySetting Setting = null;

            if (!CacheUtil.TryGetValue<UserNotifySetting>(cacheKey, out Setting))
            {
#if !Passport
                if (Globals.PassportClient.EnablePassport)
                {
                    MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy[] values = Globals.PassportClient.PassportService.User_GetNotifySettings(userID);

                    if (values != null)
                    {
                        foreach (MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy proxy in values)
                        {
                            foreach (NotifySettingItem item in Setting.AllNotify)
                            {
                                if (item.NotifyType == proxy.Key)
                                    item.OpenState = (NotifyState)proxy.Value;
                            }
                        }
                    }
                }
                else
#endif
                {
                    Setting = UserDao.Instance.GetUserNotifySetting(userID);
                }

                CacheUtil.Set<UserNotifySetting>(cacheKey, Setting);
            }
            return Setting;
        }

        /// <summary>
        /// 保存用户的通知设置
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="setting"></param>
        public void SetNotifySetting(int userID, UserNotifySetting setting)
        {
#if !Passport

            if (Globals.PassportClient.EnablePassport)
            {
                MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy[] results;
                List<MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy> list = new List<MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy>();

                foreach (NotifySettingItem item in setting.AllNotify)
                {
                    MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy proxy = new MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy();
                    proxy.Key = item.NotifyType;
                    proxy.Value = (int)item.OpenState;
                    list.Add(proxy);
                }

                results = new MaxLabs.bbsMax.PassportServerInterface.KeyValueProxy[list.Count];
                list.CopyTo(results, 0);

                try
                {
                    Globals.PassportClient.PassportService.User_SetNotifySettings(userID, results);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                }
            }
            else
#endif
            {
                UserDao.Instance.SetUserNotifySetting(userID, setting);
            }

            User user = GetUserFromCache(userID);

            if (user != null)
            {
                user.NotifySetting = setting;
            }
            CacheUtil.Set<UserNotifySetting>(string.Format(cacheKey_NotifySetting, userID), setting);

        }

        public void UpdateMaxSystemNotifyID(AuthUser operatorUser, int sysNotifyID)
        {
            operatorUser.LastReadSystemNotifyID = sysNotifyID;
            UserDao.Instance.UpdateMaxSystemNotifyID(operatorUser.UserID, sysNotifyID);
        }

        #endregion

        #region  勋章

        public UserCollection GetMedalUsers(int medalID, int? medalLevelID, string userName, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            if (string.IsNullOrEmpty(userName))
            {
                return UserDao.Instance.GetMedalUsers(medalID, medalLevelID, pageNumber, pageSize, out totalCount);
            }
            else
            {
                User user = UserBO.Instance.GetUser(userName);
                if (user == null)
                    return new UserCollection();

                MedalLevel level = AllSettings.Current.MedalSettings.Medals.GetValue(medalID).GetMedalLevel(user, false);
                if (level != null)
                {
                    if (medalLevelID != null && level.ID != medalLevelID.Value)
                    {
                        return new UserCollection();
                    }

                    UserCollection users = new UserCollection();
                    users.Add(user);

                    totalCount = 1;
                    return users;
                }

                return new UserCollection();

            }
        }

        public bool AddMedalUsers(AuthUser operatorUser, int medalID, int medalLevelID, IEnumerable<int> userIDs, DateTime endDate, string url)
        {
            if (false == AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserMedal))
            {
                ThrowError<NoPermissionManageUserMedalError>(new NoPermissionManageUserMedalError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(userIDs) == false)
            {
                ThrowError<EmptyAddMedalUserError>(new EmptyAddMedalUserError("userIDs"));
                return false;
            }

            Medal medal = AllSettings.Current.MedalSettings.Medals.GetValue(medalID);

            if (medal == null)
            {
                ThrowError<MedalNotExistsError>(new MedalNotExistsError("usermedal", medalID, medalLevelID));
                return false;
            }

            bool has = false;
            MedalLevel medalLevel = null;
            foreach (MedalLevel level in medal.Levels)
            {
                if (level.ID == medalLevelID)
                {
                    medalLevel = level;
                    has = true;
                    break;
                }
            }

            if (has == false)
            {
                ThrowError<MedalNotExistsError>(new MedalNotExistsError("usermedal", medalID, medalLevelID));
                return false;
            }

            if (endDate < DateTimeUtil.Now)
            {
                ThrowError<InvalidUserMedalEndDateError>(new InvalidUserMedalEndDateError("endDate", endDate));
                return false;
            }

            if (UserDao.Instance.AddMedalUsers(medalID, medalLevelID, userIDs, endDate, url))
            {
                RemoveUsersCache(userIDs);

                Logs.LogManager.LogOperation(
                    new Medal_AddUsersMedal(operatorUser.UserID, operatorUser.Name, medalID, medal.Name, medalLevelID, medalLevel.Name, IPUtil.GetCurrentIP(), userIDs)
                );
                return true;
            }
            else
                return false;
        }

        public bool AddMedalsToUser(AuthUser operatorUser, int targetUserID, Dictionary<int, int> medalIDs, List<DateTime> endDates, bool ignorePermission)
        {
            if (ignorePermission && operatorUser != null)
            {
                if (false == AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserMedal))
                {
                    ThrowError<NoPermissionManageUserMedalError>(new NoPermissionManageUserMedalError());
                    return false;
                }

            }

            if (UserDao.Instance.AddMedalsToUser(targetUserID, medalIDs, endDates))
            {
                RemoveUserCache(targetUserID);

                //List<int> tempUserIDs = new List<int>();
                //tempUserIDs.Add(userID);
                //Logs.LogManager.LogOperation(
                //    new Medal_AddUsersMedal(operatorUser.UserID, operatorUser.Name, medalID, medal.Name, medalLevelID, medalLevel.Name, IPUtil.GetCurrentIP(), tempUserIDs)
                //);

                return true;
            }
            else
                return false;
        }

        public bool UpdateUserMedalEndDate(AuthUser operatorUser, int medalID, Dictionary<int, DateTime> endDates, Dictionary<int, string> urls)
        {
            if (false == AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserMedal))
            {
                ThrowError<NoPermissionManageUserMedalError>(new NoPermissionManageUserMedalError());
                return false;
            }

            if (endDates.Count == 0)
                return true;

            Guid[] excludeRoleIDs = AllSettings.Current.ManageUserPermissionSet.GetNoPermissionTargetRoleIds(operatorUser, PermissionTargetType.User);

            if (UserDao.Instance.UpdateUserMedalEndDate(medalID, endDates, excludeRoleIDs, urls))
            {
                RemoveUsersCache(endDates.Keys);
                return true;
            }
            else
                return false;
        }

        public bool UpdateUserMedals(AuthUser operatorUser, int targetUserID, Dictionary<int, DateTime> endDates, Dictionary<int, string> urls)
        {
            if (false == AllSettings.Current.ManageUserPermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, targetUserID))
            {
                ThrowError<NoPermissionManageUserMedalError>(new NoPermissionManageUserMedalError());
                return false;
            }
            if (urls.Count == 0)
                return true;

            bool success = UserDao.Instance.UpdateUserMedals(targetUserID, endDates, urls);

            if (success)
            {
                RemoveUserCache(targetUserID);
            }

            return success;
        }

        public bool DeleteMedalUsers(AuthUser operatorUser, int medalID, IEnumerable<int> userIDs)
        {
            if (ValidateUtil.HasItems<int>(userIDs) == false)
            {
                ThrowError<EmptyDeleteMedalUserError>(new EmptyDeleteMedalUserError("userIDs"));
                return false;
            }

            List<int> resultUserIDs = new List<int>();
            foreach (int userID in userIDs)
            {
                if (AllSettings.Current.ManageUserPermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, userID))
                    resultUserIDs.Add(userID);
            }

            if (resultUserIDs.Count == 0)
            {
                ThrowError<NoPermissionManageUserMedalError>(new NoPermissionManageUserMedalError());
                return false;
            }

            if (UserDao.Instance.DeleteMedalUsers(medalID, userIDs))
            {
                RemoveUsersCache(userIDs);

                Medal medal = AllSettings.Current.MedalSettings.Medals.GetValue(medalID);
                if (medal != null)
                {
                    List<int> tempUserIDs = new List<int>();
                    foreach (int userID in userIDs)
                        tempUserIDs.Add(userID);
                    Logs.LogManager.LogOperation(
                        new Medal_DeleteUsersMedal(operatorUser.UserID, operatorUser.Name, medalID, medal.Name, IPUtil.GetCurrentIP(), tempUserIDs)
                    );
                }
                return true;
            }
            else
                return false;

        }


        public bool DeleteUserMedals(AuthUser operatorUser, int targetUserID, IEnumerable<int> medalIDs)
        {
            if (AllSettings.Current.ManageUserPermissionSet.Can(operatorUser, ManageUserPermissionSet.ActionWithTarget.EditUserMedal, targetUserID) == false)
            {
                ThrowError<NoPermissionManageUserMedalError>(new NoPermissionManageUserMedalError());
                return false;
            }

            if (ValidateUtil.HasItems<int>(medalIDs) == false)
            {
                ThrowError<EmptyDeleteUserMedalError>(new EmptyDeleteUserMedalError("medalIDs"));
                return false;
            }

            bool success = UserDao.Instance.DeleteUserMedals(targetUserID, medalIDs);

            if (success)
            {
                RemoveUserCache(targetUserID);

                MedalCollection medals = AllSettings.Current.MedalSettings.Medals;

                foreach (Medal medal in medals)
                {
                    List<int> tempUserIDs = new List<int>();
                    tempUserIDs.Add(targetUserID);
                    Logs.LogManager.LogOperation(
                         new Medal_DeleteUsersMedal(operatorUser.UserID, operatorUser.Name, medal.ID, medal.Name, operatorUser.LastVisitIP, tempUserIDs)
                     );
                }
            }

            return success;
        }
        #endregion

        #region 过滤非法关键字

        private void ProcessKeyword(User user, ProcessKeywordMode mode)
        {
            if (user == null || user.UserID <= 0)
                return;

#if !Passport

            //作为passport客户端，不需要处理关键字，全部交由通行证处理即可
            if (Globals.PassportClient.EnablePassport)
                return;

#endif

            //当前是蜘蛛发起的请求则不处理关键字
            if (RequestVariable.Current != null && RequestVariable.Current.IsSpider == false)
                return;

            //更新关键字模式，如果这个用户并不需要处理，直接退出
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                if (AllSettings.Current.ContentKeywordSettings.ReplaceKeywords.NeedUpdate<User>(user) == false)
                    return;
            }

            UserCollection users = new UserCollection();

            users.Add(user);

            ProcessKeyword(users, mode);
        }

        private void ProcessKeyword(UserCollection users, ProcessKeywordMode mode)
        {
            if (users.Count == 0)
                return;

#if !Passport

            //作为passport客户端，不需要处理关键字，全部交由通行证处理即可
            if (Globals.PassportClient.EnablePassport)
                return;

#endif

            //当前是蜘蛛发起的请求则不处理关键字
            if (RequestVariable.Current != null && RequestVariable.Current.IsSpider == false)
                return;

            KeywordReplaceRegulation keyword = AllSettings.Current.ContentKeywordSettings.ReplaceKeywords;

            bool needProcess = false;

            //更新关键字模式，只在必要的情况下才取恢复信息并处理
            if (mode == ProcessKeywordMode.TryUpdateKeyword)
            {
                needProcess = keyword.NeedUpdate<User>(users);
            }
            //填充原始内容模式，始终都要取恢复信息，但不处理
            else
            {
                needProcess = true;
            }

            if (needProcess)
            {
                RevertableCollection<User> usersWithReverter = UserDao.Instance.GetUserWithReverters(users.GetKeys());

                if (usersWithReverter != null)
                {
                    if (keyword.Update(usersWithReverter))
                    {
                        UserDao.Instance.UpdateUserKeywords(usersWithReverter);
                    }

                    //将新数据填充到旧的列表
                    usersWithReverter.FillTo(users);

                    foreach (User user in users)
                        user.SignatureFormatted = null;
                }
            }
        }

        #endregion

        #region 屏蔽用户部分

#if !Passport

        /// <summary>
        /// 根据版块屏蔽
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="userID"></param>
        /// <param name="forumIDs"></param>
        /// <param name="endDate"></param>
        public void BanUser(AuthUser operatorUser, int userID, Dictionary<int, DateTime> forumIdsWithEndDate, string cause)
        {
            //if (forumInfos == null || forumInfos.Count == 0)
            //{
            //    ThrowError(new BanUserNoForumIDError("forumInfos"));
            //}
            if (userID <= 0)
            {
                ThrowError(new UserIDInvalidError("userID", userID));
            }


            List<string> forumNames = new List<string>();

            if (AllSettings.Current.BackendPermissions.Can(operatorUser
               , BackendPermissions.ActionWithTarget.Manage_BanUsers, userID) == false)
            {
                #region 无权限的版块过滤
                BannedUserCollection banned = BannedUserProvider.GetUserBannedForumInfo(userID);
                Forum forum = null;
                List<int> noPermissionForumIds = new List<int>();

                //寻找无权限版块， 但存在的版块
                foreach (KeyValuePair<int, DateTime> forumInfo in forumIdsWithEndDate)
                {
                    bool isExist = false;
                    forum = ForumBO.Instance.GetForum(forumInfo.Key);
                    if (forum != null)
                    {
                        if (false == forum.ManagePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.BanUser, userID))
                        {
                            foreach (BannedUser b in banned)
                            {
                                if (b.ForumID == forumInfo.Key)
                                {
                                    forumIdsWithEndDate[forumInfo.Key] = b.EndDate;
                                    isExist = true;
                                    break;
                                }
                            }

                            if (isExist) continue;

                            noPermissionForumIds.Add(forumInfo.Key);
                        }
                    }
                    else
                    {
                        noPermissionForumIds.Add(forumInfo.Key);
                    }
                }

                //清除无权限版块
                foreach (int i in noPermissionForumIds)
                {
                    forumIdsWithEndDate.Remove(i);
                }


                /*记录日志用的*/

                foreach (KeyValuePair<int, DateTime> info in forumIdsWithEndDate)
                {
                    forumNames.Add(ForumBO.Instance.GetForum(info.Key).ForumName);
                }
                /**/


                //寻找无权限， 但被清除掉的版块
                foreach (BannedUser bi in banned)
                {
                    if (bi.ForumID == 0)
                        continue;
                    forum = ForumBO.Instance.GetForum(bi.ForumID);
                    if (false == forum.ManagePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.BanUser, userID))
                    {
                        if (!forumIdsWithEndDate.ContainsKey(bi.ForumID))
                        {
                            forumIdsWithEndDate.Add(bi.ForumID, bi.EndDate);
                        }
                    }
                }

                #endregion
            }
            else
            {
                foreach (KeyValuePair<int, DateTime> info in forumIdsWithEndDate)
                {
                    forumNames.Add(ForumBO.Instance.GetForum(info.Key).ForumNameText);
                }
            }
            if (HasUnCatchedError)
                return;

            SimpleUser targetUser = UserBO.Instance.GetSimpleUser(userID);
            if (targetUser == null || targetUser == SimpleUser.Guest)
            {
                ThrowError(new UserNotExistsError("userID", userID));
                return;
            }

            BannedUserDao.Instance.BanUser(operatorUser.Name, BanType.Ban, DateTimeUtil.Now, cause, forumIdsWithEndDate, userID, targetUser.Name, IPUtil.GetCurrentIP()); ;

            BannedUserProvider.ClearInnerTable();
        }

        public bool BanUsersWholeForum(AuthUser operatorUser, IEnumerable<int> userIds, string userIP)
        {
            if (operatorUser == User.Guest || operatorUser == null)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            foreach (int userId in userIds)
            {
                if (!AllSettings.Current.BackendPermissions.Can(operatorUser
                 , BackendPermissions.ActionWithTarget.Manage_BanUsers,userId))
                {
                    ThrowError(new CustomError("","您没有权限屏蔽ID为"+ userId +"的用户"));
                    return false;
                }
            }

            BannedUserDao.Instance.BanUsersWholeForum(operatorUser.Name, userIds, userIP);
            BannedUserProvider.ClearInnerTable();

            return true;
        }

        public void BanUser(AuthUser operatorUser, int userID, int forumID, DateTime endDate, string cause)
        {

            //ForumV5 forum = ForumBOV5.Instance.GetForum(forumID);
            //if (forum.ManagePermission.Can(operatorUserID, ManageForumPermissionSetNode.ActionWithTarget.BanUser, userID))
            //{
            if (UserBO.Instance.GetUser(userID) == null)
            {
                ThrowError(new UserIDInvalidError("userID", userID));
            }

            if (HasUnCatchedError)
                return;

            Dictionary<int, DateTime> foruminfo = new Dictionary<int, DateTime>();
            foruminfo.Add(forumID, endDate);

            BanUser(operatorUser, userID, foruminfo, cause);
            //}
            //else
            //{
            //    ThrowError(new NoPermissionBanUserError());
            //}
        }


        /// <summary>
        /// 取消屏蔽
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="userID"></param>
        /// <param name="ForumID">版块ID</param>
        public int UnBanUsers(AuthUser operatorUser, IEnumerable<int> userIDs, int forumID)
        {

            if (!ValidateUtil.HasItems<int>(userIDs))
            {
                return 0;
            }

            //Forum forum = ForumManager.GetForum(forumID);
            List<int> cancelShieldUserIds = new List<int>(userIDs);
            ManageForumPermissionSetNode managePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(forumID);

            foreach (int userID in userIDs)
            {
                if (!managePermission.Can(operatorUser, ManageForumPermissionSetNode.ActionWithTarget.BanUser, userID))
                {
                    cancelShieldUserIds.Remove(userID);
                }
            }

            if (cancelShieldUserIds.Count > 0)
            {
                BannedUserDao.Instance.CancelBan(cancelShieldUserIds, forumID, operatorUser.Name, IPUtil.GetCurrentIP());
                BannedUserProvider.ClearInnerTable();
            }

            return cancelShieldUserIds.Count;
        }

        /// <summary>
        /// 返回指定用户在某个版块是否被屏蔽
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="forumID">版块</param>
        /// <returns></returns>
        public bool IsBanned(int userID, int forumID)
        {
            return BannedUserProvider.IsBanned(userID, forumID);
        }

        /// <summary>
        /// 整站屏蔽
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="userID"></param>
        /// <param name="endDate"></param>
        public void BanUser(AuthUser operatorUser, int userID, DateTime endDate, string cause, bool igronPermission)
        {

            if (!igronPermission && !AllSettings.Current.BackendPermissions.Can(operatorUser
             , BackendPermissions.ActionWithTarget.Manage_BanUsers, userID))
            {
                ThrowError(new NoPermissionBanUserError());
            }

            if (userID <= 0)
            {
                ThrowError(new UserIDInvalidError("userID", userID));
            }
            if (HasUnCatchedError)
                return;

            Dictionary<int, DateTime> temp = new Dictionary<int, DateTime>();
            temp.Add(0, endDate);

            SimpleUser targetUser = UserBO.Instance.GetSimpleUser(userID);
            if (targetUser == null || targetUser == SimpleUser.Guest)
            {
                ThrowError(new UserNotExistsError("userID", userID));
                return;
            }

            BannedUserDao.Instance.BanUser(operatorUser.Name, BanType.BanAll, DateTimeUtil.Now, cause, temp, userID, targetUser.Name, IPUtil.GetCurrentIP());
            BannedUserProvider.ClearInnerTable();


            //          if (writeLog)
            //              Logs.LogManager.LogOperation(new Logs.BanUser(operatorUser.UserID, operatorUser.Username, userID, targetUser.Username, cause, IPUtil.GetCurrentIP()));
        }

        public void BanUser(AuthUser operatorUser, int userID, DateTime endDate, string cause)
        {
            BanUser(operatorUser, userID, endDate, cause, false);
        }

        /// <summary>
        /// 取消屏蔽
        /// </summary>
        /// <param name="operatorUserID"></param>
        /// <param name="userID"></param>
        public int UnBanUsers(AuthUser operatorUser, IEnumerable<int> userIDs)
        {
            if (!ValidateUtil.HasItems<int>(userIDs))
            {
                return 0;
            }

            List<int> unBanUserIds = new List<int>(userIDs);
            foreach (int userID in userIDs)
            {
                if (AllSettings.Current.BackendPermissions.Can(operatorUser
             , BackendPermissions.ActionWithTarget.Manage_BanUsers, userID) == false)
                {
                    unBanUserIds.Remove(userID);
                }
            }

            if (unBanUserIds.Count > 0)
            {
                BannedUserDao.Instance.CancelBan(unBanUserIds, operatorUser.Name, IPUtil.GetCurrentIP());
                BannedUserProvider.ClearInnerTable();
            }
            return unBanUserIds.Count;
        }

#endif


        #endregion

        #region 手机绑定

      
        /// <summary>
        /// 判断用户是否已经绑定了手机
        /// </summary>
        /// <param name="operationUser"></param>
        /// <returns></returns>
        private bool IsBoundMobilePhone(AuthUser operationUser)
        {
            return (operationUser.MobilePhone != 0);
        }

        /// <summary>
        /// 判断手机号码是否符合格式要求
        /// </summary>
        /// <param name="mobilePhone"></param>
        /// <returns></returns>
        private bool ValidateMobilePhone(string mobilePhone, string paramName)
        {
            if (string.IsNullOrEmpty(mobilePhone))
            {
                ThrowError(new EmptyMobilePhoneError(paramName));
                return false;
            }

            if (mobilePhone.Length != 11)
            {
                ThrowError(new InvalidMobilePhoneError(paramName));
                return false;
            }

            if (!StringUtil.IsIntegerFormat(mobilePhone))
            {
                ThrowError(new InvalidMobilePhoneError(paramName));
                return false;
            }

            if (StringUtil.StartsWith(mobilePhone, "13") ||
                StringUtil.StartsWith(mobilePhone, "15") ||
                StringUtil.StartsWith(mobilePhone, "18"))
            {
                return true;
            }
            else
            {
                ThrowError(new InvalidMobilePhoneError(paramName));
                return false;
            }

        }

        private bool SendBindOrUnbindSms(AuthUser operatorUser, string mobilePhone, MobilePhoneAction action)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (AllSettings.Current.PhoneValidateSettings.Open == false)
            {
                ThrowError(new PhoneBindClosedError());
                return false;
            }

            if (action == MobilePhoneAction.Bind)
            {
                if (true == IsBoundMobilePhone(operatorUser))
                {
                    ThrowError(new MobileBoundAlreadyError());
                    return false;
                }
            }
            else if (action == MobilePhoneAction.Unbind)
            {
                if (false == IsBoundMobilePhone(operatorUser))
                {
                    ThrowError(new MobileUnBoundAlreadyError());
                    return false;
                }
            }

            if (!ValidateMobilePhone(mobilePhone, "mobilePhone"))
            {
                return false;
            }

#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                
                APIResult result;
                if (action == MobilePhoneAction.Bind)
                {
                    try
                    {
                        result = Globals.PassportClient.PassportService.User_SendBindPhoneSms(operatorUser.UserID, mobilePhone);
                    }
                    catch(Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                        return false;
                    }
                }
                else if (action == MobilePhoneAction.Unbind)
                {
                    try
                    {
                        result = Globals.PassportClient.PassportService.User_SendUnbindPhoneSms(operatorUser.UserID);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                        return false;
                    }
                }
                else
                {
                    result = null;
                }

                if (result != null)
                {
                    if (result.IsSuccess == false)
                    {
                        for (int i = 0; i < result.Messages.Length; i++)
                        {
                            ThrowError<CustomError>(new CustomError(result.ErrorTargets[i], result.Messages[i]));
                        }
                    }

                    return result.IsSuccess;
                }
                else
                {
                    return false;
                }
            }
            else
#endif
            {
                long mobilePhoneNumber = long.Parse(mobilePhone);

                Random random = new Random();
                int code = random.Next(1, int.MaxValue);
                code = code % 1000000;

                string smsCode = code.ToString("000000");

                int result=UserTempDataDao.Instance.SaveBindOrUnbindSmsCode(operatorUser.UserID, action, mobilePhoneNumber, smsCode, 0, "");
                switch(result)
                {
                    case 2:
                        ThrowError(new PhoneRepeatError());
                        return false;
                    case 1:
                        ThrowError(new PhoneValidateLimitError());
                        return false;
                    default:
                        break;
                }

                string smsContent;
                if (action == MobilePhoneAction.Bind)
                {
                    smsContent = string.Format(Lang.User_PhoneBindSmsContent, smsCode);
                }
                else if (action == MobilePhoneAction.Unbind)
                {
                    smsContent = string.Format(Lang.User_PhoneUnbindSmsContent, smsCode);
                }
                else
                    throw new NotSupportedException("此处不能传入MobilePhoneAction.Change");

                ISMSSender sender = Globals.CurrentAppConfig.MaxPlugins[PhoneValidateSettings.PluginName] as ISMSSender;
                if (sender == null)
                {
                    ThrowError(new CustomError("无法打开短信发送接口，请联系管理员"));
                    return false;
                }

                //发送带有验证码的短信
                if (!sender.SendSMS(mobilePhone.ToString(), smsContent))
                {
                    ThrowError(new CustomError("发送验证短信失败，请联系管理员"));
                    return false;
                }

                return true;
            }
        }

        private bool SetPhoneBySmsCode(AuthUser operatorUser, string mobilePhone, string smsCode, MobilePhoneAction action)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (AllSettings.Current.PhoneValidateSettings.Open == false)
            {
                ThrowError(new PhoneBindClosedError());
                return false;
            }

            if (!ValidateMobilePhone(mobilePhone, "mobilePhone"))
            {
                return false;
            }

            if (string.IsNullOrEmpty(smsCode))
            {
                ThrowError(new EmptySmsCodeError("smsCode"));
                return false;
            }

            bool isSuccess = false;
            long mobilePhone_long = long.Parse(mobilePhone);
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                APIResult result;
                if (action == MobilePhoneAction.Bind)
                {
                    try
                    {
                        result = Globals.PassportClient.PassportService.User_BindMobilePhone(operatorUser.UserID, mobilePhone, smsCode);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                        return false;
                    }
                }
                else if (action == MobilePhoneAction.Unbind)
                {
                    try
                    {
                        result = Globals.PassportClient.PassportService.User_UnbindPhoneBySmsCode(operatorUser.UserID, smsCode);
                    }
                    catch (Exception ex)
                    {
                        ThrowError(new APIError(ex.Message));
                        return false;
                    }
                }
                else
                {
                    result = null;
                }

                if (result != null)
                {
                    if (result.IsSuccess == false)
                    {
                        for (int i = 0; i < result.Messages.Length; i++)
                        {
                            ThrowError(new CustomError(result.ErrorTargets[i],result.Messages[i]));
                        }
                    }
                    return result.IsSuccess;
                }
                else
                {
                    return false;
                }
            }
            else
#endif
            {
                UserTempDataDao.Instance.SetPhoneBySmsCode(operatorUser.UserID, mobilePhone_long, smsCode, action, out isSuccess);

                if (isSuccess == false)
                {
                    ThrowError(new SmsCodeError("smsCode"));
                    return isSuccess;
                }
                else
                {
                    if (action == MobilePhoneAction.Bind)
                    {
                        if (OnUserBindMobilePhone != null)
                        {
                            OnUserBindMobilePhone(operatorUser.UserID, mobilePhone_long);
                        }
                    }
                    else if (action == MobilePhoneAction.Unbind)
                    {
                        if (OnUserUnbindMobilePhone != null)
                        {
                            OnUserUnbindMobilePhone(operatorUser.UserID);
                        }
                    }
                }

                return isSuccess;
            }
        }

        #region 绑定手机操作

        /// <summary>
        /// 发送手机绑定的短信验证码
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="mobilePhone"></param>
        /// <returns></returns>
        public bool SendBindPhoneSms(AuthUser operatorUser, string mobilePhone)
        {
            return SendBindOrUnbindSms(operatorUser, mobilePhone, MobilePhoneAction.Bind);
        }

        /// <summary>
        /// 根据输入的短信验证码绑定手机
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="mobilePhone"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public bool BindPhoneBySmsCode(AuthUser operatorUser, string mobilePhone, string smsCode)
        {
            bool isSuccess = SetPhoneBySmsCode(operatorUser, mobilePhone, smsCode, MobilePhoneAction.Bind);
            if (isSuccess)
            {
                operatorUser.MobilePhone = long.Parse(mobilePhone);
            }

            return isSuccess;
        }

        #endregion

        #region 解除绑定操作

        /// <summary>
        /// 发送解除手机绑定的短信验证码
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <returns></returns>
        public bool SendUnbindPhoneSms(AuthUser operatorUser)
        {
            return SendBindOrUnbindSms(operatorUser, operatorUser.MobilePhone.ToString(), MobilePhoneAction.Unbind);
        }

        /// <summary>
        /// 根据输入的短信验证码解除手机绑定
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        public bool UnbindPhoneBySmsCode(AuthUser operatorUser, string smsCode)
        {
            bool isSuccess = SetPhoneBySmsCode(operatorUser, operatorUser.MobilePhone.ToString(), smsCode, MobilePhoneAction.Unbind);
            if (isSuccess)
            {
                operatorUser.MobilePhone = 0;

                if (OnUserUnbindMobilePhone != null)
                {
                    OnUserUnbindMobilePhone(operatorUser.UserID);
                }
            }

            return isSuccess;
        }

        #endregion

        #region 更改绑定操作

        /// <summary>
        /// 发送更改手机绑定的短信验证码
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="newMobilePhone"></param>
        /// <returns></returns>
        public bool SendChangePhoneSms(AuthUser operatorUser, string newMobilePhone)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (AllSettings.Current.PhoneValidateSettings.Open == false)
            {
                ThrowError(new PhoneBindClosedError());
                return false;
            }

            if (false == IsBoundMobilePhone(operatorUser))
            {
                ThrowError(new MobileUnBoundAlreadyError());
                return false;
            }

            if (!ValidateMobilePhone(newMobilePhone, "newMobilePhone"))
            {
                return false;
            }

            long newMobilePhone_long = long.Parse(newMobilePhone);

            if (operatorUser.MobilePhone == newMobilePhone_long)
            {
                ThrowError(new MobilePhoneSameError("newMobilePhone"));
                return false;
            }

#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                APIResult result =null;
                try
                {
                    result = Globals.PassportClient.PassportService.User_SendChangePhoneSms(operatorUser.UserID, newMobilePhone);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }
                if (result != null)
                {
                    if (result.IsSuccess == false)
                    {
                        for (int i = 0; i < result.Messages.Length; i++)
                        {
                            ThrowError(new CustomError(result.ErrorTargets[i], result.Messages[i]));
                        }
                    }

                    return result.IsSuccess;
                }
                else
                {
                    return false;
                }
            }
            else
#endif
            {
                Random random = new Random();
                string oldSmsCode = (random.Next(0, int.MaxValue / 2) % 1000000).ToString("000000");
                string newSmsCode = (random.Next(int.MaxValue / 2, int.MaxValue) % 1000000).ToString("000000");

                int result=UserTempDataDao.Instance.SaveBindOrUnbindSmsCode(operatorUser.UserID, MobilePhoneAction.Change, newMobilePhone_long, newSmsCode, operatorUser.MobilePhone, oldSmsCode);
                switch(result)
                {
                    case 2:
                        ThrowError(new PhoneRepeatError());
                        return false;
                    case 1:
                        ThrowError(new PhoneValidateLimitError());
                        return false;
                    default:
                        break;
                }

                //发送带有验证码的短信

                ISMSSender smsSender = Globals.CurrentAppConfig.MaxPlugins[PhoneValidateSettings.PluginName] as ISMSSender;
                if (smsSender == null)
                {
                    ThrowError(new CustomError("无法打开短信发送接口，请联系管理员"));
                    return false;
                }

                if (smsSender.SendSMS(operatorUser.MobilePhone.ToString(), string.Format(Lang.User_PhoneChangeSmsForOldPhoneContent, oldSmsCode)))
                {
                    if (!smsSender.SendSMS(newMobilePhone, string.Format(Lang.User_PhoneChangeSmsForNewPhoneContent, newSmsCode)))
                    {
                        ThrowError(new CustomError("发送验证短信失败，请联系管理员"));
                        return false;
                    }
                }
                else
                {
                    ThrowError(new CustomError("发送验证短信失败，请联系管理员"));
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// 根据输入的短信验证码更改手机绑定
        /// </summary>
        /// <param name="operatorUser"></param>
        /// <param name="newMobilePhone"></param>
        /// <param name="oldSmsCode"></param>
        /// <param name="newSmsCode"></param>
        /// <returns></returns>
        public bool ChangePhoneBySmsCode(AuthUser operatorUser, string newMobilePhone, string oldSmsCode, string newSmsCode)
        {

            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return false;
            }

            if (AllSettings.Current.PhoneValidateSettings.Open == false)
            {
                ThrowError(new PhoneBindClosedError());
                return false;
            }

            ValidateMobilePhone(newMobilePhone, "newMobilePhone");

            if (string.IsNullOrEmpty(oldSmsCode))
            {
                ThrowError(new EmptySmsCodeError("oldSmsCode"));
            }

            if (string.IsNullOrEmpty(newSmsCode))
            {
                ThrowError(new EmptySmsCodeError("newSmsCode"));
            }

            if (HasUnCatchedError)
            {
                return false;
            }

                bool oldSuccess = false;
                bool newSuccess = false;
                long newMobilePhone_long = long.Parse(newMobilePhone);

#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                APIResult result =null;
                try
                {
                    result = Globals.PassportClient.PassportService.User_ChangePhoneBySmsCode(operatorUser.UserID, newMobilePhone, oldSmsCode, newSmsCode);
                }
                catch (Exception ex)
                {
                    ThrowError(new APIError(ex.Message));
                    return false;
                }

                if (result != null)
                {
                    if (result.IsSuccess == false)
                    {
                        for (int i = 0; i < result.Messages.Length; i++)
                        { 
                            ThrowError(new CustomError(result.ErrorTargets[i],result.Messages[i]));
                        }
                    }
                    operatorUser.MobilePhone = newMobilePhone_long;

                    return result.IsSuccess;
                }
                else
                {
                    return false;
                }
            }
            else
#endif
            {
                UserTempDataDao.Instance.ChangePhoneBySmsCode(operatorUser.UserID, newMobilePhone_long, newSmsCode, operatorUser.MobilePhone, oldSmsCode, out oldSuccess, out newSuccess);

                if (oldSuccess == false)
                {
                    ThrowError(new SmsCodeError("oldSmsCode"));
                }

                if (newSuccess == false)
                {
                    ThrowError(new SmsCodeError("newSmsCode"));
                }

                if (HasUnCatchedError)
                {
                    return false;
                }

                if (OnUserBindMobilePhone != null)
                {
                    OnUserBindMobilePhone(operatorUser.UserID, newMobilePhone_long);
                }

                operatorUser.MobilePhone = newMobilePhone_long;

                return true;
            }
        }

        #endregion

        public void UpdateUserPhone( AuthUser operatorUser, User user, string mobilePhone)
        {

            if (!UserBO.Instance.CanEditUserProfile(operatorUser, user.UserID))
            {
                ThrowError(new NoPermissionEditUserProfileError());
                return;
            }

            if (ValidateMobilePhone(mobilePhone, "mobilePhone"))
            {
                ThrowError(new InvalidMobilePhoneError("mobilePhone"));
                return;
            }

            long mobilePhoneNumber = long.Parse(mobilePhone);

            UserDao.Instance.UpdateUserPhone(user.UserID, mobilePhoneNumber);
            user.MobilePhone = mobilePhoneNumber;

            if (OnUserBindMobilePhone != null)
                OnUserBindMobilePhone(user.UserID, mobilePhoneNumber);
        }

        public void RemoveUserPhone( AuthUser operatorUser, User user)
        {
            if (!UserBO.Instance.CanEditUserProfile(operatorUser, user.UserID))
            {
                ThrowError(new NoPermissionEditUserProfileError());
                return;
            }

            UserDao.Instance.UpdateUserPhone(user.UserID, 0);
            user.MobilePhone = 0;
            if (OnUserBindMobilePhone != null)
                OnUserBindMobilePhone(user.UserID, 0);
        }

        #endregion

        #region Class UserSignatureTagSettings

        /// <summary>
        /// 签名中的特殊UBB标签设置
        /// </summary>
        public class UserSignatureTagSettings
        {
            private User MyUser;
            public UserSignatureTagSettings(int userID)
            {
                MyUser = UserBO.Instance.GetUser(userID, GetUserOption.WithAll);
            }

            public bool AllowImage
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowImageTag.GetValue(MyUser);
                }
            }

            public bool AllowFlash
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowFlashTag.GetValue(MyUser);
                }
            }

            public bool AllowUrl
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowUrlTag.GetValue(MyUser);
                }
            }

            public bool AllowVideo
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowVideoTag.GetValue(MyUser);
                }
            }

            public bool AllowAudio
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowAudioTag.GetValue(MyUser);
                }
            }

            public bool AllowTable
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowTableTag.GetValue(MyUser);
                }
            }

            public bool AllowDefaultEmoticon
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowDefaultEmoticon.GetValue(MyUser);
                }
            }

            public bool AllowUserEmoticon
            {
                get
                {
                    return AllSettings.Current.UserSettings.AllowUserEmoticon.GetValue(MyUser);
                }
            }
        }

        #endregion

#if !Passport
        string mostActiveUsersCacheKey = "User/MostActiveUsers/{0}";
        int mostActiveUsersCacheCount = 30;

        public UserCollection GetMostActiveUsers(ActiveUserType type, int count)
        {
            string key = string.Format(mostActiveUsersCacheKey, type.ToString());
            List<int> userIDs;
            if (CacheUtil.TryGetValue<List<int>>(key, out userIDs))
            {
                List<int> results = new List<int>();
                int i = 0;
                foreach (int id in userIDs)
                {
                    results.Add(id);
                    i++;
                    if (i >= count)
                        break;
                }
                UserCollection tempUsers = GetUsers(results);
                UserCollection users = new UserCollection();
                foreach (int userID in userIDs)
                {
                    User temp = tempUsers.GetValue(userID);
                    if (temp != null)
                        users.Add(temp);
                }
                return users;
            }
            else
            {
                UserCollection users = UserDao.Instance.GetMostActiveUsers(type, mostActiveUsersCacheCount);

                if (type == ActiveUserType.WeekOnlineTime || type == ActiveUserType.WeekPosts)
                {

                }

                UserCollection results = new UserCollection();
                userIDs = new List<int>();
                int i = 0;
                foreach (User user in users)
                {
                    userIDs.Add(user.UserID);
                    if (i < count)
                    {
                        results.Add(user);
                        i++;
                    }
                }

                SetMostActiveUsersCache(type, key, userIDs);
                return results;
            }
        }

        public void ClearMostActiveUsersCache(IEnumerable<ActiveUserType> types)
        {
            foreach (ActiveUserType type in types)
            {
                //这类始终不移除 让其自动过期
                if (type == ActiveUserType.WeekOnlineTime || type == ActiveUserType.DayOnlineTime)
                    continue;

                string key = string.Format(mostActiveUsersCacheKey, type.ToString());
                CacheUtil.Remove(key);

            }
        }

        private void SetMostActiveUsersCache(ActiveUserType type, string key, List<int> userIDs)
        {
            if (type == ActiveUserType.WeekOnlineTime)
            {
                CacheUtil.Set<List<int>>(key, userIDs, AllSettings.Current.CacheSettings.WeekUserCacheTime, CacheExpiresType.Absolute);
            }
            else if (type == ActiveUserType.DayOnlineTime)
            {
                CacheUtil.Set<List<int>>(key, userIDs, AllSettings.Current.CacheSettings.DayUserCacheTime, CacheExpiresType.Absolute);
            }
            else
            {
                //else if (type == ActiveUserType.WeekPosts)
                //{
                //    CacheUtil.Set<List<int>>(key, userIDs, AllSettings.Current.CacheSettings.WeekPostCacheTime, CacheExpiresType.Absolute);
                //}
                //else
                //{
                //    CacheUtil.Set<List<int>>(key, userIDs, AllSettings.Current.CacheSettings.DayPostCacheTime, CacheExpiresType.Absolute);
                //}

                CacheUtil.Set<List<int>>(key, userIDs);
            }
        }

        /// <summary>
        /// 发帖的时候
        /// </summary>
        /// <param name="user"></param>
        public void UpdateMostActiveUsersCacheWhenPost(User user)
        {
            UpdateMostActiveUsersCacheWhenPost(ActiveUserType.MonthPosts, user);
            UpdateMostActiveUsersCacheWhenPost(ActiveUserType.WeekPosts, user);
            UpdateMostActiveUsersCacheWhenPost(ActiveUserType.DayPosts, user);
        }

        private static object dayPostLocker = new object();
        private static object weekPostLocker = new object();
        private static object monthPostLocker = new object();
        private void UpdateMostActiveUsersCacheWhenPost(ActiveUserType type, User user)
        {
            string key = string.Format(mostActiveUsersCacheKey, type.ToString());
            List<int> userIDs;
            if (CacheUtil.TryGetValue<List<int>>(key, out userIDs))
            {
                object locker;
                if (type == ActiveUserType.MonthPosts)
                    locker = monthPostLocker;
                else if (type == ActiveUserType.WeekPosts)
                    locker = weekPostLocker;
                else
                    locker = dayPostLocker;

                lock (locker)
                {
                    UserCollection tempUsers = GetUsers(userIDs);
                    UserCollection users = new UserCollection();
                    foreach(int userID in userIDs)
                    {
                        User temp = tempUsers.GetValue(userID);
                        if (temp != null)
                            users.Add(temp);
                    }
                    if (users.Count > 0)
                    {
                        bool isLarge = false;
                        if (type == ActiveUserType.WeekPosts)
                            isLarge = (user.WeekPosts <= users[users.Count - 1].WeekPosts);
                        else if (type == ActiveUserType.MonthPosts)
                            isLarge = (user.MonthPosts <= users[users.Count - 1].MonthPosts);
                        else
                            isLarge = (user.DayPosts <= users[users.Count - 1].DayPosts);

                        if (isLarge)
                        {
                            if (users.Count < mostActiveUsersCacheCount)
                            {
                                if (userIDs.Contains(user.UserID) == false)
                                {
                                    userIDs.Add(user.UserID);
                                    //CacheUtil.Set<List<int>>(key, userIDs);
                                    SetMostActiveUsersCache(type, key, userIDs);
                                }
                            }
                        }
                        else
                        {
                            List<int> result = new List<int>();
                            int count = 0;
                            bool hasAdd = false;
                            foreach (User temp in users)
                            {
                                if (temp.UserID == user.UserID)
                                    continue;

                                if (hasAdd == false)
                                {
                                    bool isLarge2 = false;
                                    if (type == ActiveUserType.WeekPosts)
                                        isLarge2 = (user.WeekPosts > temp.WeekPosts);
                                    else if (type == ActiveUserType.MonthPosts)
                                        isLarge2 = (user.MonthPosts > temp.MonthPosts);
                                    else
                                        isLarge2 = (user.DayPosts > temp.DayPosts);

                                    if (isLarge2)
                                    {
                                        result.Add(user.UserID);
                                        hasAdd = true;
                                        count++;

                                        if (count == mostActiveUsersCacheCount)
                                            break;
                                    }
                                }
                                result.Add(temp.UserID);
                                count++;
                                if (count == mostActiveUsersCacheCount)
                                    break;
                            }
                            //CacheUtil.Set<List<int>>(key, result);
                            SetMostActiveUsersCache(type, key, result);
                        }
                    }
                    else
                    {
                        userIDs.Add(user.UserID);
                        //CacheUtil.Set<List<int>>(key, userIDs);
                        SetMostActiveUsersCache(type, key, userIDs);
                    }
                }
            }
        }
#endif

        public void Server_UpdatePassportUserExtendFieldCache(ExtendedFieldCollection extendFields)
        {
            if (OnUserExtendFieldChanged != null) 
                OnUserExtendFieldChanged(extendFields);
        }

        public void Client_UpdatePassportUserExtendFieldCache(MaxLabs.Passport.Proxy.ExtendedFieldProxy[] extendedFields)
        {
            MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy[] proxys = new MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy[extendedFields.Length];

            int i = 0;
            foreach (MaxLabs.Passport.Proxy.ExtendedFieldProxy proxy in extendedFields)
            {
                proxys[i] = GetExtendedFieldProxy(proxy);
                i++;

                ExtendedField field = AllSettings.Current.ExtendedFieldSettings.PassportFields.GetValue(proxy.Key);
                if (field != null)
                {
                    if ((int)field.DisplayType != proxy.DisplayType)//隐私类型改变了
                    {

                        if (proxy.DisplayType != (int)ExtendedFieldDisplayType.UserCustom)
                        {
                            //整个循环中只会有一个 发生这种情况  所以只用单个更新
                            UserDao.Instance.UpdateUserExtendProfilePrivacy(field.Key, (ExtendedFieldDisplayType)proxy.DisplayType);
                        }
                    }
                }
            }

            Globals.PassportClient.PassportConfig.ExtendedFields = proxys;

            foreach (ExtendedField field in AllSettings.Current.ExtendedFieldSettings.PassportFields)
            {
                bool has = false;
                foreach (MaxLabs.Passport.Proxy.ExtendedFieldProxy proxy in extendedFields)
                {
                    if (proxy.Key == field.Key)
                    {
                        has = true;
                        break;
                    }
                }

                //整个循环中只会有一个 发生这种情况  所以只用单个删除
                if (has == false)
                    UserDao.Instance.DeleteUserExtendProfile(field.Key);
            }

            AllSettings.Current.ExtendedFieldSettings.ClearPassportFields();

            RemoveAllUserCache();
        }

        private MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy GetExtendedFieldProxy(MaxLabs.Passport.Proxy.ExtendedFieldProxy proxy)
        {
            MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy result = new MaxLabs.bbsMax.PassportServerInterface.ExtendedFieldProxy();
            result.Description = proxy.Description;
            result.DisplayType = proxy.DisplayType;
            result.FieldTypeName = proxy.FieldTypeName;
            result.IsRequired = proxy.IsRequired;
            result.Key = proxy.Key;
            result.Name = proxy.Name;
            result.Searchable = proxy.Searchable;
            result.Settings = new MaxLabs.bbsMax.PassportServerInterface.StringKeyValueProxy[proxy.Settings.Count];

            int i = 0;
            foreach(MaxLabs.Passport.Proxy.StringKeyValueProxy keyValue in proxy.Settings)
            {
                MaxLabs.bbsMax.PassportServerInterface.StringKeyValueProxy temp = new MaxLabs.bbsMax.PassportServerInterface.StringKeyValueProxy();
                temp.Key = keyValue.Key;
                temp.Value = keyValue.Value;
                result.Settings[i] = temp;
                i++;
            }
            result.SortOrder = proxy.SortOrder;

            return result;
        }

        public void DeleteUserExtendFields(string key)
        {
            UserDao.Instance.DeleteUserExtendProfile(key);

            RemoveAllUserCache();
        }

        public void UpdateUserExtendFieldPrivacy(string key, ExtendedFieldDisplayType privacy)
        {
            if (privacy == ExtendedFieldDisplayType.UserCustom) //保留原来
                return;

            UserDao.Instance.UpdateUserExtendProfilePrivacy(key, privacy);

            RemoveAllUserCache();
        }

        /// <summary>
        /// 清理过期数据
        /// </summary>
        public void ClearExperiesExtendField()
        {
            ExtendedFieldCollection fiels = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport;

            string[] keys = fiels.GetKeys();
            UserDao.Instance.ClearExperiesExtendField(keys);
        }

        /// <summary>
        /// 清理过期数据
        /// </summary>
        public void ClearExpiresUserData()
        {
            UserDao.Instance.ClearExpiresUserData();
        }
    }
}