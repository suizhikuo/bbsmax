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
//				和论坛有关的枚举
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
    public enum DeleteStatus
    {
        /// <summary>
        /// 未知错误，可能发生于业务层/数据层之间
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// 创建成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 记录不存在
        /// </summary>
        NotExists = 1,

        /// <summary>
        /// 不能为空
        /// </summary>
        NotNull = 2,

        /// <summary>
        /// 正在删除
        /// </summary>
        Deleting = 10,

        /// <summary>
        /// 没有权限
        /// </summary>
        NoPermission = 101

    }
}