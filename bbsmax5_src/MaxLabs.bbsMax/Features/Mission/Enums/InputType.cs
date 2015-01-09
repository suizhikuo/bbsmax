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
	/// 输入框类型
	/// </summary>
    public enum InputType
    {

        Text = 0,

        /// <summary>
        /// 单选框
        /// </summary>
        Radio = 1,

        /// <summary>
        /// 复选框
        /// </summary>
        Checkbox = 2,

        RadioList = 3,

        CheckboxList = 4,

        ForumList = 5,

        UserGroupList = 6,

        /// <summary>
        /// 不需要填值的 只需要描述
        /// </summary>
        Description = 7

    }
}