//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//Author:		周杨思/Andrew Chow
//Date:			2006/1/29
//Description:
//				和主题有关的枚举
//------------------------------------------------------------------------------

//Amendment history:
//------------------------------------------------------------------------------
//Datetime		Modified by		Checked by		Summary
//------------------------------------------------------------------------------
//yyyy/MM/DD	周杨思							
//------------------------------------------------------------------------------
using System;

namespace MaxLabs.bbsMax.Enums
{
    
    public enum PostType
    {
        /// <summary>
        /// 正常帖子
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 是主题的内容
        /// </summary>
        ThreadContent = 1,
        /// <summary>
        /// PK帖中的支持方
        /// </summary>
        Polemize_Agree = 2,

        /// <summary>
        /// PK帖中的反对方
        /// </summary>
        Polemize_Against = 3,

        /// <summary>
        /// PK帖中的中立方
        /// </summary>
        Polemize_Neutral = 4,
    }

    public enum ViewPointType : byte
    {
        Agree = 0,

        Against = 1,

        Neutral = 2,
    }

    public enum MyThreadType
    {
        /// <summary>
        /// 我的主题
        /// </summary>
        MyThread = 0,

        /// <summary>
        /// 我参与的主题
        /// </summary>
        MyParticipantThread = 1,

        /// <summary>
        /// 我的未审核主题
        /// </summary>
        MyUnapprovedThread = 2,

        /// <summary>
        /// 我的未审核回复 的主题
        /// </summary>
        MyUnapprovedPostThread = 3
    }

    public enum ContentFormat
    {
        EnableEmoticons = 2,
        EnableHTML = 4,
        /// <summary>
        /// 以前旧格式
        /// </summary>
        EnableMaxCode = 8,
        /// <summary>
        /// 新的UBB格式
        /// </summary>
        EnableMaxCode3 = 16,

        /// <summary>
        /// 是否是5.0版本发的帖子
        /// </summary>
        IsV5_0 = 32,
    }
}