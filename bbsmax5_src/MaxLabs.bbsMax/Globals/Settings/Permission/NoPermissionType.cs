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

namespace MaxLabs.bbsMax.Settings
{
    public enum NoPermissionType
    {
        /// <summary>
        /// 没有这个权限
        /// </summary>
        NoPermission,

        /// <summary>
        /// 没有管理目标用户的权限
        /// </summary>
        NoPermissionForTargetUser,

        /// <summary>
        /// 目标对象被其他管理员修改过，而您没有权限对该管理员进行管理
        /// </summary>
        NoPermissionForLastEditor
    }
}