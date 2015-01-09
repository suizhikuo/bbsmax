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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Errors
{
    /// <summary>
    /// 没有权限对指定用户进行管理的错误
    /// </summary>
    public abstract class NoPermissionWithTargetError : NoPermissionError
    {

        private User m_TargetUser = null;

        public NoPermissionWithTargetError(string permissionItemName)
            : base(permissionItemName)
        { }

        public NoPermissionWithTargetError(User targetUser)
        {
            m_TargetUser = targetUser;
        }

        public NoPermissionWithTargetError()
        { }

        public override string Message
        {
            get { return string.Format("您没有权限{0}", this.PermissionItemName); }
        }

        public User TargetUser
        {
            get { return m_TargetUser; }
        }
    }
}