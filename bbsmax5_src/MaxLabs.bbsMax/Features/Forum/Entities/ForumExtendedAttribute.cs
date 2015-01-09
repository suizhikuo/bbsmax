//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class ForumExtendedAttribute : SettingBase
    {
        public ForumExtendedAttribute()
        {

            SigninForumWithoutPassword = new Exceptable<bool>(false);

            LinkOpenByNewWidow = false;

            TitleAttach = string.Empty;
            MetaKeywords = string.Empty;
            MetaDescription = string.Empty;
        }



        /// <summary>
        /// 进入版块不需要密码
        /// </summary>
        [SettingItem]
        public Exceptable<bool> SigninForumWithoutPassword { get; set; }

        /// <summary>
        /// 是否新窗口打开
        /// </summary>
        [SettingItem]
        public bool LinkOpenByNewWidow { get; set; }

        [SettingItem]
        public string TitleAttach { get; set; }

        [SettingItem]
        public string MetaKeywords { get; set; }

        [SettingItem]
        public string MetaDescription { get; set; }
     
    }
}