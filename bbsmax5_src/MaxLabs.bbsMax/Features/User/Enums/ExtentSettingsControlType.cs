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
    #region 控件类型
        public enum FieldControlType
        {
            /// <summary>
            /// 自定义输入框
            /// </summary>
            Text = 0,

            /// <summary>
            /// Email地址
            /// </summary>
            Email = 1,

            /// <summary>
            /// 网址
            /// </summary>
            Url = 2,

            /// <summary>
            /// 数字
            /// </summary>
            Number = 3,

            /// <summary>
            /// 颜色选择器
            /// </summary>
            Color = 4,

            /// <summary>
            /// 日期选择器
            /// </summary>
            Datetime = 5,

            /// <summary>
            /// 地址选择器
            /// </summary>
            Address = 6,

            /// <summary>
            /// 自定义单选框
            /// </summary>
            Radio = 7,

            /// <summary>
            /// 自定义复选框
            /// </summary>
            Checkbox = 8,

            /// <summary>
            /// 自定义下拉框
            /// </summary>
            Select = 9,

            /// <summary>
            /// 自定义下拉框(支持3级)
            /// </summary>
            HasChildren = 10
        }
        #endregion
}