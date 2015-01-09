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
    /// 用户积分变更
    /// </summary>
    /// <param name="userID">用户ID</param>
    /// <param name="points">新的8个积分</param>
    public delegate void UserPointChanged(int userID, int[] points);

    /// <summary>
    /// 用户资料变更
    /// </summary>
    /// <param name="userID"></param>
    public delegate void UserProfileChanged(AuthUser user);


    public delegate void UserExtendFieldChanged(ExtendedFieldCollection extendedFields);

    /// <summary>
    /// 用户修改密码
    /// </summary>
    /// <param name="userID">用户ID</param>
    /// <param name="password">新密码</param>
    public delegate void UserPasswordChanged(int userID, string password);

    public delegate void UserAvatarChanged(int userID , string avatarSrc, string smallAvatarPath, string defaultAvatarPath, string bigAvatarPath);

    public delegate void UserCreated( AuthUser newUser);

    public delegate void UserRealnameChecked(int userID , string realname,string idCardNumber );

    public delegate void UserCancelRealnameCheck(int userID);

    public delegate void UserBindMobilePhine( int userID , long phineNumber);

    public delegate void UserUnbindMobilePhone(int userID);

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <param name="userID"></param>
    public delegate void UserLogout( int userID );

    /// <summary>
    /// 用户邮箱改变
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="newEmail"></param>
    public delegate void UserEmailChanged(int userID, string newEmail);

    /// <summary>
    /// 用户修改DOING
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="doing"></param>
    public delegate void UserDoingUpdated(int userID , string doing);
}