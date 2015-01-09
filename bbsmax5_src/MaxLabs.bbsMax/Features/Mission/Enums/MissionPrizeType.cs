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
	/// 任务奖励类型
	/// </summary>
    public enum MissionPrizeType
    {

        /// <summary>
        /// 积分
        /// </summary>
        Point = 0,

        /// <summary>
        /// 用户组
        /// </summary>
        UserGroup = 1,

        /// <summary>
        /// 勋章
        /// </summary>
        Medal = 2,

        /// <summary>
        /// 邀请码
        /// </summary>
        InviteSerial = 3,

        Prop = 4
    }
}