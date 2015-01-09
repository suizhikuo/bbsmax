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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Settings
{
    public class NotifySettingItem:SettingBase
    {
        public NotifySettingItem()
        {

        }

        [SettingItem]
        public NotifyState OpenState { get; set; }

        [SettingItem]
        public int NotifyType { get; set; }
    }

    public class NotifySettingItemCollection : Collection<NotifySettingItem>, ISettingItem
    {
        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (NotifySettingItem  item in this)
            {
                if (item.OpenState == NotifyState.DefaultOpen) continue;
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);
            foreach (string s in list)
            {
                NotifySettingItem item = new NotifySettingItem();
                item.Parse(s);
                //foreach (NotifySettingItem settingItem in this)
                //{
                //    if (item.NotifyType == settingItem.NotifyType)
                //    {
                //        settingItem.OpenState = item.OpenState;
                //    }
                //}
                Add(item);
            }
        }

        #endregion
    }

}