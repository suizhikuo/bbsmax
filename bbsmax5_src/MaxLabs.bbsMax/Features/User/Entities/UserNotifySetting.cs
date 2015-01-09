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
using System.Reflection;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 用户的通知设置
    /// </summary>
    public class UserNotifySetting
    {
        public UserNotifySetting( string values)
        {
            this.AllNotify = new NotifySettingItemCollection();
            foreach (NotifyType nt in NotifyBO.AllNotifyTypes)
            {
                NotifySettingItem item = new NotifySettingItem();
                item.NotifyType = nt.TypeID;
                item.OpenState = AllSettings.Current.NotifySettings.GetNotifySystemState(item.NotifyType);
                this.AllNotify.Add(item);
            }

            
            while (!string.IsNullOrEmpty(values) && values.Length % 4==0 && values.Length>=4 )
            {
                int value = 0;
                try
                {
                    value = int.Parse(values.Substring(0, 4));
                }
                catch
                {
                    return;
                }
                int type = (int)(value / 10);
                NotifyState state = (NotifyState)(value % 10);

                foreach (NotifySettingItem item in this.AllNotify)
                {
                    if (item.OpenState == NotifyState.AlwaysOpen || item.OpenState == NotifyState.AlwaysClose)
                        continue;
                    if (item.NotifyType == type)
                        item.OpenState = state;
                }
                values = values.Substring(4);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public NotifyState GetNotifyState( int type )
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

        /// <summary>
        /// 
        /// </summary>
        public NotifySettingItemCollection AllNotify
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuffer buffer=new StringBuffer("");

            foreach (NotifySettingItem item in this.AllNotify)
            {
                if (item.OpenState != NotifyState.AlwaysClose && item.OpenState != NotifyState.AlwaysOpen)
                    buffer += string.Format("{0:0000}", ((int)item.NotifyType) * 10 + (int)item.OpenState);
            }
            return buffer.ToString();
        }
    }
}