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
    /// 收索模式。
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// 搜索指定用户主题
        /// </summary>
        UserThread = 0,
        /// <summary>
        /// 按主题。
        /// </summary>
        Subject = 1,
        /// <summary>
        /// 按内容。
        /// </summary>
        Content = 2,

        /// <summary>
        /// 按用户ID 搜索用户主题
        /// </summary>
        ThreadUserID = 3,

        /// <summary>
        /// 搜索用户帖子
        /// </summary>
        UserPost = 4,

        /// <summary>
        /// 按用户ID 搜索用户帖子
        /// </summary>
        PostUserID = 5,

        TopicContent = 6,
    }
    /// <summary>
    /// 排序。
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// 升序。
        /// </summary>
        ASC = 0,
        /// <summary>
        /// 降序。
        /// </summary>
        DESC = 1
    }
    /// <summary>
    /// 时间段。
    /// </summary>
    public enum DateMode
    {
        /// <summary>
        /// 所有时间。
        /// </summary>
        All = 0,
        /// <summary>
        /// 一天内。
        /// </summary>
        OneDay = 1,
        /// <summary>
        /// 两天内。
        /// </summary>
        TwoDays = 2,
        /// <summary>
        /// 一个星期内。
        /// </summary>
        Week = 7,
        /// <summary>
        /// 一个月内。
        /// </summary>
        OneMonth = 30,
        /// <summary>
        /// 三个月内。
        /// </summary>
        ThreeMonths = 90,
        /// <summary>
        /// 半年内。
        /// </summary>
        SixMonths = 180,
        /// <summary>
        /// 一年内。
        /// </summary>
        Year = 365
    }
    /// <summary>
    /// 收索模式。
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// 全文索引收索。
        /// </summary>
        FullTextIndex = 0,
        /// <summary>
        /// 用Like语句收索。
        /// </summary>
        LikeStatement = 1
    }
}