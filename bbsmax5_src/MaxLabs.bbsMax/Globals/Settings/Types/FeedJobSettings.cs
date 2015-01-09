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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class FeedJobSettings : SettingBase, ICloneable
    {
        public FeedJobSettings()
        {
            Enable = true;
            ExecuteTime = 2;
            Day = 60;
            Count = 100000;
            ClearMode = JobDataClearMode.CombinMode;
        }

        /// <summary>
        /// 是否开启定时清除
        /// </summary>
        [SettingItem]
        public bool Enable { get; set; }

        /// <summary>
        /// 执行时间  每天的几点执行
        /// </summary>
        [SettingItem]
        public int ExecuteTime { get; set; }

        /// <summary>
        /// 删除几天以前的动态
        /// </summary>
        [SettingItem]
        public int Day { get; set; }


        /// <summary>
        /// 保留条数
        /// </summary>
        [SettingItem]
        public int Count { get; set; }

        [SettingItem]
        public JobDataClearMode ClearMode
        {
            get;
            set;
        }

        #region ICloneable 成员

        public object Clone()
        {
            FeedJobSettings setting = new FeedJobSettings();
            setting.Enable = Enable;
            setting.ExecuteTime = ExecuteTime;
            setting.Day = Day;
            setting.Count = Count;
            return setting;
        }

        #endregion
    }
}