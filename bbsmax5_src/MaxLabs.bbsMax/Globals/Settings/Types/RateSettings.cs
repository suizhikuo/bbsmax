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
    public sealed class RateSettings : SettingBase//, ICloneable
    {
        public RateSettings()
        {
            RateSets = new RateSetCollection();
            RateSet set = new RateSet();
            set.RateItems = new RateSetItemCollection();
            RateSetItem item = new RateSetItem();
            item.MaxValue = 50;
            item.MinValue = -50;
            item.MaxValueInTime = 100;
            item.PointType = UserPointType.Point1;
            item.RoleID = Guid.Empty;
            set.RateItems.Add(item);

            item = new RateSetItem();
            item.MaxValue = 1000;
            item.MinValue = -1000;
            item.MaxValueInTime = 100000;
            item.PointType = UserPointType.Point1;
            item.RoleID = Role.Owners.RoleID;
            item.RoleSortOrder = 0;
            set.RateItems.Add(item);

            item = new RateSetItem();
            item.MaxValue = 1000;
            item.MinValue = -1000;
            item.MaxValueInTime = 100000;
            item.PointType = UserPointType.Point2;
            item.RoleID = Role.Owners.RoleID;
            item.RoleSortOrder = 0;
            set.RateItems.Add(item);

            RateSets.Add(set);
        }


        [SettingItem]
        public RateSetCollection RateSets { get; set; }


        //#region ICloneable 成员

        //public object Clone()
        //{
        //    RateSettings setting = new RateSettings();

        //    setting.RateItems = RateItems;

        //    return setting;
        //}

        //#endregion


        public void ClearExperiesData()
        {
            /*
            RateSetItemCollection rateItems = new RateSetItemCollection();

            foreach (RateSetItem item in RateItems)
            {
                if (item.RoleID == Guid.Empty || AllSettings.Current.RoleSettings.Roles.GetValue(item.RoleID) != null)
                {
                    rateItems.Add(item);
                }
            }

            RateSettings setting = new RateSettings();

            setting.RateItems = rateItems;

            try
            {
                SettingManager.SaveSettings(setting);
            }
            catch { }
            */
        }
    }
}