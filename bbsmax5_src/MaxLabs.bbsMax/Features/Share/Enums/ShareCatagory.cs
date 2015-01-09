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
    /// 分享类型(如网址,视频等等)
    /// </summary>
    public enum ShareType
    {
        /// <summary>
        /// 所有分类
        /// </summary>
        All = 0,
        /// <summary>
        /// 网址
        /// </summary>
        URL = 1,

        /// <summary>
        /// 视频
        /// </summary>
        Video = 2,

        /// <summary>
        /// 音乐
        /// </summary>
        Music = 3,

        /// <summary>
        /// Flash
        /// </summary>
        Flash = 4,

        /// <summary>
        /// 日志
        /// </summary>
        Blog = 5,

        /// <summary>
        /// 相册
        /// </summary>
        Album = 6,

        /// <summary>
        /// 图片
        /// </summary>
        Picture = 7,

        /// <summary>
        /// 群组
        /// </summary>
        Forum = 8,

        /// <summary>
        /// 话题
        /// </summary>
        Topic = 9,

        /// <summary>
        /// 用户
        /// </summary>
        User = 10,

        /// <summary>
        /// TAG 
        /// </summary>
        Tag = 11,

        /// <summary>
        /// 新闻
        /// </summary>
        News = 12,
    }
}