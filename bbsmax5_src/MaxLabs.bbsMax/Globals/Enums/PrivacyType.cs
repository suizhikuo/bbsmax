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
    /// 文章隐私类型
    /// </summary>
    public enum PrivacyType : byte
    {
        /// <summary>
        /// 所有人可见
        /// </summary>
        AllVisible = 0,

        /// <summary>
        /// 好友可见
        /// </summary>
        FriendVisible = 1,

        /// <summary>
        /// 自己可见 
        /// </summary>
        SelfVisible = 2,

        /// <summary>
        /// 需要密码
        /// </summary>
        NeedPassword = 3,

        /// <summary>
        /// 指定用户可见
        /// </summary>
        AppointUser = 4,
    }
}