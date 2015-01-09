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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Errors
{
    public class NoPermissionDeleteThreadError:ErrorInfo
    {

        Forum forum;
        public NoPermissionDeleteThreadError( Forum forum )
        {
            this.forum = forum;
        }

        public override string Message
        {
            get { return string.Format("没有您没有权限删除板块“{0}”的帖子！", forum.ForumNameText); }
        }
    }
}