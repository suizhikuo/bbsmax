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
    public enum OnlineShowType
    {
        /// <summary>
        /// 显示全部
        /// </summary>
        ShowAll = 0,
        /// <summary>
        /// 显示会员
        /// </summary>
        ShowMember = 1,
        /// <summary>
        /// 始终不显示列表
        /// </summary>
        NeverShow = 2,

        /// <summary>
        /// 只显示统计数
        /// </summary>
        OnlyShowCount = 3,

        ///// <summary>
        ///// 显示注册用户和游客
        ///// </summary>
        //EveryoneAndGuest=0,
        ///// <summary>
        ///// 仅显示注册用户
        ///// </summary>
        //Everyone=1,
        ///// <summary>
        ///// 全部不显示
        ///// </summary>
        //None=3,
    }

    public enum DisplayStatus
    {
        /// <summary>
        /// 是
        /// </summary>
        Yes = 0,

        /// <summary>
        /// 否
        /// </summary>
        No = 1,

        /// <summary>
        /// 默认
        /// </summary>
        Default = 2,
    }
}