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
    public enum KeywordType
    {
        /// <summary>
        /// 通过
        /// </summary>
        Pass,

        /// <summary>
        /// 直接禁止
        /// </summary>
        Banned,

        /// <summary>
        /// 直接替换
        /// </summary>
        Replace,

        /// <summary>
        /// 检查到包含如下关键字，发表的文章需要审核
        /// </summary>
        Approved
    }
}