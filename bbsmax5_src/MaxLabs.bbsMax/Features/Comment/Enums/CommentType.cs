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
    public enum CommentType : byte
    {
        /// <summary>
        /// 所有
        /// </summary>
        All = 0,

        /// <summary>
        /// 留言板
        /// </summary>
        Board = 1,

        /// <summary>
        /// 日志评论
        /// </summary>
        Blog = 2,

        /// <summary>
        /// 记录评论
        /// </summary>
        Doing = 3,

        /// <summary>
        /// 相片评论
        /// </summary>
        Photo = 4,
        
        /// <summary>
        /// 分享评论
        /// </summary>
        Share = 5
    }
}