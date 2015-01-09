//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;


using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
   
	/// <summary>
	/// 论坛设置
	/// </summary>
	public class CacheSettings : SettingBase
	{
        public CacheSettings()
		{

            WeekPostCacheTime = 5;
            DayPostCacheTime = 5;
            WeekUserCacheTime = 5;
            DayUserCacheTime = 5;

		}



        /// <summary>
        /// 
        /// </summary>
        /// 
        [SettingItem]
        public int WeekPostCacheTime { get; set; }

        [SettingItem]
        public int DayPostCacheTime { get; set; }

        [SettingItem]
        public int WeekUserCacheTime { get; set; }

        [SettingItem]
        public int DayUserCacheTime { get; set; }
	}
}