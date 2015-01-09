//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 积分兑换规则
    /// </summary>
    public class PointExchangeProportion : SettingBase
	{

        /// <summary>
        /// 积分类型  
        /// </summary>
        [SettingItem]
        public UserPointType UserPointType { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        [SettingItem]
        public int Proportion { get; set; }

	}

    public class PointExchangeProportionCollection : Collection<PointExchangeProportion>, ISettingItem
	{
        public int this[UserPointType pointType]
        {
            get
            {
                foreach (PointExchangeProportion proportion in this)
                {
                    if (proportion.UserPointType == pointType)
                        return proportion.Proportion;
                }
                return 1;
            }
        }
        public int GetProportion(UserPointType pointType)
        {
            return this[pointType];
        }
        public void Add(UserPointType userPointType, int proportion)
        {
            bool have = false;
            foreach (PointExchangeProportion tempProportion in this)
            {
                if (tempProportion.UserPointType == userPointType)
                {
                    have = true;
                    tempProportion.Proportion = proportion;
                }
            }
            if (!have)
            {
                PointExchangeProportion tempProportion = new PointExchangeProportion();
                tempProportion.UserPointType = userPointType;
                tempProportion.Proportion = proportion;
                base.Add(tempProportion);
            }
        }

        new public void Add(PointExchangeProportion proportion)
        {
            Add(proportion.UserPointType, proportion.Proportion);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PointExchangeProportion item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    PointExchangeProportion pointExchangeProportion = new PointExchangeProportion();

                    pointExchangeProportion.Parse(item);

                    this.Add(pointExchangeProportion);

                }
            }
        }

        #endregion
    }
}