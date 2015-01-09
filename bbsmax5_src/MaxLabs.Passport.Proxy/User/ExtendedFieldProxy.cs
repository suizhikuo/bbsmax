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


namespace MaxLabs.Passport.Proxy
{
    public class ExtendedFieldProxy : ProxyBase
    {
        public ExtendedFieldProxy() { }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        ///// <summary>
        ///// 能否被搜索
        ///// </summary>
        public bool Searchable { get; set; }

        ///// <summary>
        ///// 是否在用户资料页等用户信息显示的地方显示
        ///// </summary>
        //[SettingItem]
        //public bool DisplayInUserInfo { get; set; }
        public int DisplayType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }

        public string Key { get; set; }

        /// <summary>
        /// 扩展字段的类型名
        /// </summary>
        public string FieldTypeName { get; set; }

        /// <summary>
        /// 附加的控件设置，如最大长度，控件默认内容等于控件特性相关的东西
        /// </summary>
        public List<StringKeyValueProxy> Settings { get; set; }
    }
}