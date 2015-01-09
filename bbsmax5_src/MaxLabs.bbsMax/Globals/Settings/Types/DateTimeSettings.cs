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
    /// <summary>
    /// 日期时间设置
    /// </summary>
    public class DateTimeSettings:SettingBase
    {
        /// <summary>
        /// 日期格式
        /// </summary>
        [SettingItem]
        public string DateFormat
        {
            get;
            set;
        }

        /// <summary>
        /// 时间格式
        /// </summary>
        [SettingItem]
        public string TimeFormat
        {
            get;set;
        }

        /// <summary>
        /// 口语显示
        /// </summary>
        [SettingItem]
        public bool EnableSpoken
        {
            get;
            set;
        }

        public DateTimeSettings()
        {
            this.DateFormat = "yyyy-MM-dd";
            this.TimeFormat = "HH:mm:ss";
            this.EnableSpoken = true;
            this.ServerTimeZone = (float)(TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours);
        }

        /// <summary>
        /// 时区
        /// </summary>
        [SettingItem]
        public float ServerTimeZone
        {
            get;
            set;
        }
 
    }
}