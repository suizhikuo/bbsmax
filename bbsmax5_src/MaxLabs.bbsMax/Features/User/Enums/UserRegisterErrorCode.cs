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

namespace MaxLabs.bbsMax.Enums
{
    /// <summary>
    /// 这里的枚举值是Dao层注册用户时返回的状态码
    /// 
    /// wenquan 09-02-16
    /// </summary>
    public enum  UserRegisterErrorCode:int
    {
        /// <summary>
        ///  未知错误
        /// </summary>
        Unknown= -1,

        /// <summary>
        /// 成功注册
        /// </summary>
        Success=0,

        /// <summary>
        /// 用户名被占用
        /// </summary>
        UsernameExsits=1,

        /// <summary>
        /// Email被占用
        /// </summary>
        EmailExists=  2  ,

        /// <summary>
        /// 邀请码错误
        /// </summary>
        InvitationCodeError=3,

        /// <summary>
        /// ID已经存在
        /// </summary>
        IdExists=4,

        /// <summary>
        /// 注册过于频繁， 相同IP注册间隔时间小于系统设置
        /// </summary>
        RegisterFrequent=5,

        /// <summary>
        /// 邀请人积分不足
        /// </summary>
        InviteUserPointShort= 6
    }
}