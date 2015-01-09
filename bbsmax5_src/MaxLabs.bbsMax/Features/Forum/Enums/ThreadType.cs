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
    public enum ThreadType : byte
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 投票帖
        /// </summary>
        Poll = 1,

        /// <summary>
        /// 提问帖
        /// </summary>
        Question = 2,

        /// <summary>
        /// 交易帖
        /// </summary>
        Trade = 3,

        /// <summary>
        /// PK帖，辩论帖
        /// </summary>
        Polemize = 4,


        /// <summary>
        /// 这是一个重定向
        /// </summary>
        Redirect = 10,

        /// <summary>
        /// 被合并到其它主题
        /// </summary>
        Join = 11,

        /// <summary>
        /// 移动帖，已经移动到其它版块
        /// </summary>
        Move = 12,

    }

}