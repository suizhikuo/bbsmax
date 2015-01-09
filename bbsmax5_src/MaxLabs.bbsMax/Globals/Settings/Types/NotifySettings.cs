//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System ;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Enums;
using System.Reflection;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public class NotifySettings : SettingBase
    {
        public NotifySettings()
        {
            this.AllNotify = new NotifySettingItemCollection();
            this.NotifySaveDays = 60;
            this.ClearJobExecuteTime = 3;
            this.NotifySaveRows = 100000;
            this.DataClearMode = JobDataClearMode.Disabled;
        }

        //private NotifySettingItemCollection m_AllNotify;

        [SettingItem]
        public NotifySettingItemCollection AllNotify
        {
            get;
            set;
        }

        /// <summary>
        /// 通知保留时间
        /// </summary>
        [SettingItem]
        public int NotifySaveDays
        {
            get;
            set;
        }

        /// <summary>
        /// 每天的几点执行这个清除通知的任务
        /// </summary>
        [SettingItem]
        public int ClearJobExecuteTime
        {
            get;
            set;
        }

        [SettingItem]
        public int NotifySaveRows
        {
            get;
            set;
        }

        [SettingItem]
        public JobDataClearMode DataClearMode
        {
            get;
            set;
        }

        /// <summary>
        /// 返回系统是否开启了某类通知
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NotifyState GetNotifySystemState(int type)
        {
            foreach (NotifySettingItem item in this.AllNotify)
            {
                if (item.NotifyType == type)
                {
                    return item.OpenState;
                }
            }
            return NotifyState.DefaultOpen;
        }
    }

    public enum NotifyState
    {
        /// <summary>
        /// 始终打开
        /// </summary>
        AlwaysOpen =0,

        /// <summary>
        /// 始终关闭
        /// </summary>
        AlwaysClose=1,

        /// <summary>
        /// 默认打开
        /// </summary>
        DefaultOpen=2,

        /// <summary>
        /// 默认关闭
        /// </summary>
        DefaultClose=3
    }
}