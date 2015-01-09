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
using System.Data;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class UserDao : DaoBase<UserDao>
    {
        public abstract Guid CreateAdminSession(int userID, string IpAddress);

        public abstract ConsoleLoginLogCollection GetConsoleLoginLogs(int count);

        public abstract bool HasAdminSession(int userID, int expiresMinute, string ip, out Guid sessionID);

        public abstract void AdminLogout(int userID);

        /// <summary>
        /// 直接更新用户的DOING
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="doing"></param>
        public abstract void UpdateUserDoing(int userID, string doing);


        /// <summary>
        /// 用户注册
        /// </summary>
        public abstract int Register(ref int userID, string username, string email, string password, EncryptFormat passwordFormat, UserRoleCollection initRoles, string ip, Guid? serial, int inviterID, bool IsActive, int[] userPoints, int ipInterval);

        public abstract void UpdateMaxSystemNotifyID(int userID, int sysNotifyID);

        /// <summary>
        /// 修改签名
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        public abstract bool UpdateUserSignature(int userID, string signature);

        /// <summary>
        /// 修改用户资料
        /// </summary>
        public abstract bool UpdateUserProfile(int usrID, Gender gender, Int16 birthYear, Int16 birthMonth, Int16 birthday, string signature, SignatureFormat signatureFormat, float timeZone, UserExtendedValueCollection extendedFields);

        public abstract bool UpdateLastVisitIP(int userID, string newIP);

        public abstract bool UpdateSkinID(int userID, string skinID);

        public abstract bool UpdateEmail(int userID, string email);

        public abstract void UseTemporaryRealname(IEnumerable<int> userIds);

        public abstract UserNotifySetting GetUserNotifySetting(int userID);

        public abstract void SetUserNotifySetting(int userID, UserNotifySetting setting);

        public abstract void AdminUpdateUserinfo(int targetUserID, DateTime regDate, int totalOnlineTime, int totalMonthOnlineTime);

        /// <summary>
        ///设置用户侧边栏状态
        /// </summary>
        /// <param name="status"></param>
        public abstract void SetSidebarStatus(int userID, EnableStatus status);

        /// <summary>
        /// 获取用户组成员（非等级用户组）
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract UserCollection GetRoleMembers(Guid roleID, int pageSize, int pageNumber, out int totalCount);

        /// <summary>
        /// 获取等级用户组成员
        /// </summary>
        /// <returns></returns>
        public abstract UserCollection GetRoleMembers(LevelLieOn levelLieOn, Int32Scope levelScope, int pageSize, int pageNumber, out int totalCount);

        /// <summary>
        /// 取得从未通过头像认证的用户（为了再次提头像认证数据的时候不重复加积分）
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public abstract IList<int> GetNeverAvatarCheckedUsers(IEnumerable<int> userIds);

        public abstract PointExpressionColumCollection GetGeneralPointExpressionColums();

        /// <summary>
        /// 升级程序使用的 
        /// </summary>
        /// <param name="pointSetting"></param>
        /// <returns></returns>
        public abstract PointExpressionColumCollection GetGeneralPointExpressionColums(PointSettings pointSetting);

        public abstract SimpleUserCollection GetSimpleUsers(IEnumerable<int> userIds);

        public abstract SimpleUser GetSimpleUser(int userID);

        public abstract void ReCountUsersPoints(string expression, int startUserID, int updateCount, out int endUserID, ref string resultExpression);

        /// <summary>
        /// 更新总积分公式
        /// </summary>
        /// <param name="expression"></param>
        public abstract void UpdatePointsExpression(string expression);

        public abstract void SetUserPoint(int userID, int[] points, out int generalPoint, string operateName, string remarks);

        public abstract int UpdateUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int[] resultPoints, out int generalPoint, out int userPoint, string operateName, string remarks);

        public abstract int CheckUserPoint(int userID, bool throwOverMinValueError, bool throwOverMaxValueError, int[] points, int[] minValues, int[] maxValues, out int point);

        public abstract int ActivingUsers(Guid serialGuid,out int userID);

        /// <summary>
        /// 修改用户资料， （管理员接口）
        /// </summary>
        /// <returns></returns>
        public abstract bool AdminUpdateUserProfile(int userID, string realname, string email, Gender gender, DateTime birthday, bool isActive, bool emailValidated, string signature, SignatureFormat signatureFormat, UserExtendedValueCollection extendedFields);

        public abstract void AdminUpdateUsername(int userID, string username);

        public abstract void UpdateUserLoginCount(int userID, string loginIP);

        /// <summary>
        /// 修改用户邮箱并且把邮箱设置为已经验证
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        public abstract void ValidateUserEmail(int userID, string email);

        public abstract UserPassword GetUserPassword(int userID);

        public abstract AuthUser GetAuthUser(string username);

        public abstract AuthUser GetAuthUser(int userID);

        public abstract bool SetRealnameChecked(int operatorUserID, int userID, bool nameChecked, string remark);

        public abstract bool UpdateUserRealname(int userID, string realname);

        /// <summary>
        /// 修改实名认证用户照片（从接口获取）
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="photos"></param>
        /// <returns></returns>
        public abstract bool UpdateAuthenticUserPhoto(int userID, string photos, int state);

        public abstract AuthUser GetAuthUserByEmail(string email, out bool duplicateEmail);

        /// <summary>
        /// 获取用户的真实身份信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public abstract AuthenticUser GetAuthenticUser(int userid);

        /// <summary>
        /// 获取真实的用户身份信息
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public abstract AuthenticUserCollection GetAuthenticUsers(AuthenticUserFilter filter, int pageNumber);

        /// <summary>
        /// 保存用户真实身份验证信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="realname"></param>
        /// <param name="idNumber"></param>
        /// <param name="idCardFile"></param>
        /// <param name="birthday"></param>
        /// <param name="gender"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public abstract bool SaveAuthenticUserInfo(int userID, string realname, string idNumber, string idCardFileFace, string idCardFileBack, DateTime birthday, Gender gender, string area);

        public abstract bool CheckIdNumberExist(string idNumber);

        /// <summary>
        /// 获取单个用户
        /// </summary>
        public abstract User GetUser(string userName, bool getFriends);

        public abstract User GetUser(int userID, bool getFriends);

        /// <summary>
        /// 获取一组用户
        /// </summary>
        /// <param name="usernames"></param>
        /// <returns></returns>
        public abstract UserCollection GetUsers(IEnumerable<string> usernames, bool getFriends);

        /// <summary>
        /// 获取一组用户
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        public abstract UserCollection GetUsers(IEnumerable<int> userIDs, bool getFriends);

        /// <summary>
        /// 获取一组用户ID
        /// </summary>
        public abstract List<int> GetUserIDs(IEnumerable<string> usernames);

        public abstract List<int> GetUserIDs(int beginID, int endID);

        /// <summary>
        /// 根据用户名获取一个用户的UserID
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public abstract int GetUserID(string username);

        public abstract UserCollection GetUsers(int userID, UserOrderBy orderByField, int pageNmuber, int pageSize, out int userSortNumber);

        public abstract UserCollection GetUsers(UserFilter filter, int pageNumber, int? total);

        /// <summary>
        /// 前台基本查找用户
        /// </summary>
        public abstract UserCollection SearchUsers(AdminUserFilter condition, int userID, int pageNumber, out int totalCount);

        /// <summary>
        /// 后台详细
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderField"></param>
        /// <param name="orderType"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public abstract UserCollection AdminSearchUsers(AdminUserFilter filter, Guid[] excludeRoles, int pageNumber, out int totalCount);

        public abstract UserCollection GetMostActiveUsers(ActiveUserType type, int count);

        ///// <summary>
        ///// 用户列表
        ///// </summary>
        //[Obsolete]
        //public abstract UserCollection GetUserlist(UserOrderBy orderField, int pageNumber, int pageSize, bool desc);

        /// <summary>
        /// 删除用户
        /// </summary>
        public abstract int DeleteUser(int userID, int step);

        /// <summary>
        /// 在多少分钟内是否存在某IP
        /// </summary>
        public abstract bool IPIsExistInMinutes(string ip, int timeSpan);

        /// <summary>
        /// 获取未被占用的UserID
        /// </summary>
        public abstract List<Int32Scope> GetNotUseUserIDs(int beginID, int endID);

        /// <summary>
        /// 修改用户密码
        /// </summary>
        public abstract void ResetUserPassword(int userID, string encodedPassword, EncryptFormat format);

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="avatar">头像地址</param>
        public abstract bool UpdateAvatar(int userID, string avatarSrc, bool avatarChecked);


        /// <summary>
        /// 如果用户的积分超出积分的上限或者下限则将其更新到上限或者下限
        /// </summary>
        /// <param name="maxPoints">按顺序8个积分的上限，未启用的积分使用整型最大值</param>
        /// <param name="minPoints">按顺序8个积分的下限，未启用的积分使用整型最小值</param>
        public abstract void UpdateAllUserPoints(int[] maxPoints, int[] minPoints);

        /*** 用户组操作 ***/

        /// <summary>
        /// 将一组用户加入一组用户组
        /// </summary>
        /// <param name="userRoles"></param>
        public abstract void AddUsersToRoles(UserRoleCollection userRoles);

        /// <summary>
        /// 将一组用户从一组用户组移除
        /// </summary>
        /// <param name="userRoles"></param>
        public abstract int RemoveUsersFromRoles(IEnumerable<int> userIds, IEnumerable<Guid> roleIds);

        /// <summary>
        /// 更新某个用户的用户组（该用户的原用户组信息被清空）
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <param name="userRoles"></param>
        public abstract void UpdateUserRoles(int targetUserId, UserRoleCollection userRoles);

        /// <summary>
        /// 重设扩展字段版本
        /// </summary>
        /// <param name="userID">目标用户ID</param>
        /// <param name="version">扩展字段版本</param>
        public abstract void UpdateExtendedFieldVersion(int userID, string version);

        public abstract bool EmailIsExsits(string email);

        public abstract UserCollection GetInvitees(int targetUserId, int pageSize, int pageNumber, out int totalCount);

        public abstract bool UpdateOnlineTime(List<UserOnlineInfo> userOnlineInfos, out int[] totalOnlineTimes, out int[] monthOnlineTimes, out int[] weekOnlineTimes, out int[] dayOnlineTimes);

        public abstract List<int> GetUserIDs(string keyword);

        //public abstract void UpdateUserOnlineStatus(int userID, OnlineStatus onlineStatus);

        public abstract UserCollection GetMedalUsers(int medalID, int? medalLevelID, int pageNumber, int pageSize, out int totalCount);

        public abstract bool AddMedalUsers(int medalID, int medalLevelID, IEnumerable<int> userIDs, DateTime endDate, string url);

        public abstract bool AddMedalsToUser(int userID, Dictionary<int, int> medalIDs, List<DateTime> endDates);

        public abstract bool UpdateUserMedalEndDate(int medalID, Dictionary<int, DateTime> endDates, Guid[] excludeRoleIDs, Dictionary<int, string> urls);

        public abstract bool UpdateUserMedals(int userID, Dictionary<int, DateTime> endDates, Dictionary<int, string> urls);

        public abstract bool DeleteMedalUsers(int medalID, IEnumerable<int> userIDs);

        public abstract bool DeleteUserMedals(int userID, IEnumerable<int> medalIDs);

        public abstract void ClearExpiresUserData();

        public abstract int UpdateUsersDatas(int startUserID, int updateCount, bool updatePostCount, bool updateBlogCount, bool updateInviteCount, bool updateCommentCount, bool updatePictureCount, bool updateShareCount, bool updateDoingCount, bool updateDiskFileCount);

        public abstract RevertableCollection<User> GetUserWithReverters(IEnumerable<int> userIDs);

        public abstract void UpdateUserKeywords(RevertableCollection<User> processlist);

        /*
        public abstract int UpdateAndLockAvatar(int userID, string avatarSrc, DateTime lockTo, string reason);

        public abstract string GetAvatarLockReason(int userID);

        public abstract void UnlockUserAvatar(int userID);

        */

        //public abstract BannedUserCollection GetAllBannedUserInfos();

        /// <summary>
        /// 获取用户的各个字段排名
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public abstract Dictionary<string, int> GetUserRankInfo(int userID, params string[] fields);

        public abstract string GetUnValidatedEmail(int userID);

        public abstract void UpdateUserSelectFriendGroupID(int userID, int groupID);

        public abstract void UpdateUserReplyReturnThreadLastPage(int userID, bool returnLastPage);

        /// <summary>
        /// 将重复的邮箱置空
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        public abstract void RemoveRepeatedEmail(int userid, string email);

        /// <summary>
        /// 加入用户手机号码
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="phoneNum"></param>
        public abstract void UpdateUserPhone(int userID, long mobilePhone);

        public abstract void FillUserPoints(User user);

        public abstract void UpdateUserProfile(int userID, UserExtendedValueCollection insertFields, UserExtendedValueCollection updateFields);

        public abstract void UpdateUserExtendProfilePrivacy(string key, ExtendedFieldDisplayType privacy);

        public abstract void DeleteUserExtendProfile(string key);

        public abstract void ClearExperiesExtendField(IEnumerable<string> keys);

        #region 找回密码记录
        /// <summary>
        /// 记录用户找回密码日志
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        /// <param name="serial"></param>
        public abstract void CreateRecoverPasswordLog(int userID, string email, string serial,string ip);

        /// <summary>
        /// 获取找回密码记录
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public abstract RecoverPasswordLogCollection GetRecoverPasswordLogs(RecoverPasswordLogFilter filter, int pageNumber);

        /// <summary>
        /// 要是找回密码成功，把找回密码日志设置为成功
        /// </summary>
        /// <param name="serial"></param>
        public abstract void SetRecoverPasswordLogSuccess( string serial,string ip);

        #endregion
    }
}