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
    /// 通知小类
    /// </summary>
    public enum FixNotifies : int
    {
        #region Passport内置通知类型 固化在安装数据里
        /// <summary>
        /// 管理员操作提醒
        /// </summary>
        AdminManage = 1,

        /// <summary>
        /// 好友验证提醒
        /// </summary>
        FriendNotify = 2,

        /// <summary>
        /// 打招呼提醒
        /// </summary>
        HailNotify = 3,
      
        #endregion 

        #region 论坛固化通知类型

        /// <summary>
        /// 评论类的通知
        /// </summary>
        CommentNotify = 101,

        /// <summary>
        /// 道具
        /// </summary>
        PropNotify = 102,

        #endregion
    }
}