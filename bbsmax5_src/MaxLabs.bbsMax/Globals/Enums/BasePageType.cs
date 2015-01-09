//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

namespace MaxLabs.bbsMax.Enums
{
    /// <summary>
    /// 基本页面类型
    /// </summary>
    public enum BasePageType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 个人主页
        /// </summary>
        Space = 1,
        /// <summary>
        /// 记录页
        /// </summary>
        Doing = 3,
        /// <summary>
        /// 日志页
        /// </summary>
        Blog = 4,
        /// <summary>
        /// 相册页
        /// </summary>
        Album = 5,
        /// <summary>
        /// 分享页
        /// </summary>
        Share = 6,
        /// <summary>
        /// 话题页
        /// </summary>
        Topic = 7,
        /// <summary>
        /// 留言页
        /// </summary>
        Board = 8,
        /// <summary>
        /// 好友页
        /// </summary>
        Friend = 9
    }
}