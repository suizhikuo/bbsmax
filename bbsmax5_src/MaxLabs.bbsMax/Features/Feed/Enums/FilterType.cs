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

namespace MaxLabs.bbsMax.Enums
{
    /// <summary>
    /// 过滤类型
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// 过滤某个好友的某个应用的某个动作
        /// </summary>
        FilterUserAppAction = 0,

        /// <summary>
        /// 过滤某个好友的所有通知
        /// </summary>
        FilterUser = 1,

        /// <summary>
        /// 过滤某个应用的所有通知
        /// </summary>
        FilterApp = 2,

        /// <summary>
        /// 过滤所有好友指定应用指定动作的动态
        /// </summary>
        FilterAppAction = 3,

    }
}