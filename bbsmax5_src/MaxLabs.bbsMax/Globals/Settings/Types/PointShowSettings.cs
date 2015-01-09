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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public class PointShowSettings : SettingBase
    {
        public PointShowSettings()
        {
            this.UsePointType = UserPointType.Point1;
            this.MinPrice = 1;
            this.MaxPrice = 99999999;
            this.PointBalance = 0;
            this.ClickInterval = 5 * 60; //点击时间间隔 5 分钟
            this.IpClickCountInDay = 1; 
        }

        /// <summary>
        /// 同一用户访问的有效时间间隔
        /// </summary>
        [SettingItem]
        public int ClickInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 同一IP匿名用户每小时允许产生几个点击
        /// </summary>
        [SettingItem]
        public int IpClickCountInDay
        {
            get;
            set;
        }

        [SettingItem]
        public UserPointType UsePointType
        {
            get;
            set;
        }

        /// <summary>
        /// 单价最低
        /// </summary>
        [SettingItem]
        public int MinPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 最高单价
        /// </summary>
        [SettingItem]
        public int MaxPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 最低积分余额
        /// </summary>
        [SettingItem]
        public int PointBalance
        {
            get;
            set;
        }
    }
}