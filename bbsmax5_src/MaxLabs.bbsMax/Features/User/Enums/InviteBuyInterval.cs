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
    /// 邀请码购买时间间隔设置
    /// </summary>
    public enum InviteBuyInterval
    {
        /// <summary>
        /// 不限
        /// </summary>
        Disable,

        /// <summary>
        /// 限制每年能买多少个
        /// </summary>
        ByYear,

        /// <summary>
        /// 限制每个月能买多少个
        /// </summary>
        ByMonth,

        /// <summary>
        /// 限制每天能购买多少个
        /// </summary>
        ByDay,

        /// <summary>
        /// 限制每小时能买多少个
        /// </summary>
        ByHour
    }
}