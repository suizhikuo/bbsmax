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
using MaxLabs.Passport.ClientKit.PassportServerInterface;
using System.Web;
using System.Runtime.Remoting.Contexts;

namespace MaxLabs.Passport.ClientKit
{
    public static class UserUtil
    {
        /// <summary>
        /// 获取所有的好友
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static FriendProxy[] GetFriends(int userID)
        {
            FriendProxy[] friends = AsmxAccess.API.Friend_GetFriends(userID);
            return friends;
        }

        /// <summary>
        /// 获取用户信息COOKIE值
        /// </summary>
        /// <returns></returns>
        private static string GetTicketCookie()
        {
            if (HttpContext.Current == null)
                return string.Empty;

            HttpCookie cookie = HttpContext.Current.Request.Cookies[PassportServerConfig.CookieName];

            if (cookie == null)
                return string.Empty;

            return cookie.Value;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        public static UserProxy CurrentUser
        {
            get
            {
                string cookie = GetTicketCookie();

                if(string.IsNullOrEmpty( cookie))
                    return null;

                UserProxy user = GetUserByCookieValue(cookie);

                return user;
            }
        }

        /// <summary>
        /// 根据COOKIE值获取用户
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static UserProxy GetUserByCookieValue(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
                return null;

            UserProxy user = null;
            if (CacheUtil.TryGetValue<UserProxy>(cookie, out user) == false)
            {
                user = AsmxAccess.API.User_ValidateTicket(cookie);
                if (user != null)
                {
                    DataConvertUtil.ConvertUserUrl(user);
                    CacheUtil.Set<UserProxy>(cookie, user);
                }
            }

            return user;
        }

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static UserProxy GetUser( int userID )
        {
            UserProxy user =  AsmxAccess.API.User_GetUser(userID);
            if (user != null)
            {
                DataConvertUtil.ConvertUserUrl(user);
            }
            return user;
        }

        /// <summary>
        /// 用户重设密码
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        public static APIResult ChangePassword(int userID, string oldPwd, string newPwd)
        {
            return AsmxAccess.API.User_ChangePassword(userID, oldPwd, newPwd);
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static UserProxy GetUser(string username)
        {
            UserProxy user = AsmxAccess.API.User_GetByUsername(username);
            if (user != null)
            {
                DataConvertUtil.ConvertUserUrl(user);
            }
            return user;
        }

        /// <summary>
        /// 拷贝从Passport返回的用户信息到本地用户对象
        /// </summary>
        /// <param name="localUser">本地用户对象， 需继承UserProxy</param>
        /// <param name="passportUser">从Passport获取回来的用户信息</param>
        public static void CopyUserInfos( UserProxy localUser,UserProxy passportUser )
        {
            localUser.AvatarSrc = passportUser.AvatarSrc;
            localUser.AvatarUrl_120px = passportUser.AvatarUrl_120px;
            localUser.AvatarUrl_24px = passportUser.AvatarUrl_24px;
            localUser.AvatarUrl_48px = passportUser.AvatarUrl_48px;
            localUser.MobilePhone = passportUser.MobilePhone;
            localUser.Realname = passportUser.Realname;
            localUser.Email = passportUser.Email;
            localUser.Gender = passportUser.Gender;
            localUser.Points = new int[passportUser.Points.Length];
            passportUser.Points.CopyTo(localUser.Points, 0);
            localUser.Birthdat = passportUser.Birthdat;
        }

        /*
/// <summary>
/// 获取当前用户，可以只是继承自UserProxy的任意类型
/// </summary>
/// <typeparam name="T">返回的用户实体类型，可以只是继承自UserProxy的任意类型</typeparam>
/// <returns></returns>
public static T GetCurrentUser<T>() where T:UserProxy,new()
{
    UserProxy current = CurrentUser;
    if (current == null)
        return null;

    T result = new T();
    CopyUserInfos(result, current);
    return result;
}
*/
    }
}