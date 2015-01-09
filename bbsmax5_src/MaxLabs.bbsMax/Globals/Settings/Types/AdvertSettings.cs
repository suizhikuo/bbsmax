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

namespace MaxLabs.bbsMax.Settings
{
    public  class AdvertSettings:SettingBase
    {
        public AdvertSettings()
        {
#if DEBUG
            this.EnableDefer = true;
#else
            this.EnableDefer = false;
#endif
#if DEBUG
            this.EnableAdverts = false;
#else
            this.EnableAdverts = true;
#endif
        }


        /// <summary>
        /// 是否启用后期输出（页面加载完后输出）
        /// </summary>
        [SettingItem]
        public bool EnableDefer
        {
            get;
            set;
        }


        /// <summary>
        /// 是否开启广告系统
        /// </summary>
        [SettingItem]
        public bool EnableAdverts
        {
            get;
            set;
        }
    }
}