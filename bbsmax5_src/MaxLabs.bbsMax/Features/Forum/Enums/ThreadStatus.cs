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
    //public enum ThreadStatus
    //{
    //    Normal = 0,

    //    /// <summary>
    //    /// 如果这是一个特殊帖（如提问帖），表示已经结束（已解决），但并不会锁定主题，用户仍然可以回复
    //    /// </summary>
    //    Final = 1,

    //    /// <summary>
    //    /// 被合并到其它主题
    //    /// </summary>
    //    Join = 5,

    //    /// <summary>
    //    /// 移动帖，已经移动到其它版块
    //    /// </summary>
    //    Move = 6,

    //}
    public enum ThreadStatus : byte
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 合并的Sticky
        /// </summary>
        Sticky = 2,

        /// <summary>
        /// 合并的GlobalSticky
        /// </summary>
        GlobalSticky = 3,

        /// <summary>
        /// 在回收站
        /// </summary>
        Recycled = 4,

        /// <summary>
        /// 未审核
        /// </summary>
        UnApproved = 5
    }



}