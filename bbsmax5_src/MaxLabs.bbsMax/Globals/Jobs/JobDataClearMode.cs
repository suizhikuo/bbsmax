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
    public enum JobDataClearMode
    {

        /// <summary>
        /// 不执行
        /// </summary>
        Disabled = 0,


        /// <summary>
        /// 按天数清除数据
        /// </summary>
        ClearByDay = 1,

        /// <summary>
        /// 按条数清除数据
        /// </summary>
        ClearByRows = 2,
        
        /// <summary>
        /// 组合模式
        /// </summary>
        CombinMode = 3,
    }
}