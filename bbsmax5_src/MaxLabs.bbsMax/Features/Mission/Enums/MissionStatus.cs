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
	/// 任务状态
	/// </summary>
    public enum MissionStatus
    {

        /// <summary>
        /// 正在进行中(如果任务已经完成 但未领取奖励 也属于进行中的任务)
        /// </summary>
        Underway = 0,

        /// <summary>
        /// 任务完成 并且领取过奖励
        /// </summary>
        Finish = 1,

        /// <summary>
        /// 超时未完成
        /// </summary>
        OverTime = 2,

        /// <summary>
        /// 已被放弃
        /// </summary>
        Abandon = 3,


        /// <summary>
        /// 失败的任务,包括超时未完成和放弃的任务
        /// </summary>
        Fail = 100,
    }
}