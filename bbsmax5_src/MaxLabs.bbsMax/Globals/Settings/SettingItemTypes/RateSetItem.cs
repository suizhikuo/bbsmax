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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class RateSetItem : SettingBase, ICloneable
	{
        public RateSetItem()
        {
            RoleID = Guid.Empty;
        }

        /// <summary>
        /// 用户组ID（如果不为Guid.Empty 表示这是一个例外）
        /// </summary>
        [SettingItem]
        public Guid RoleID { get; set; }

        /// <summary>
        /// 排序 优先级（即当用户同时在两个例外用户组里，则使用RoleSortOrder较小的例外设置）
        /// 越小 优先级越高
        /// </summary>
        [SettingItem]
        public int RoleSortOrder { get; set; }


        [SettingItem]
        public UserPointType PointType { get; set; }

        /// <summary>
        /// 平分最小值
        /// </summary>
        [SettingItem]
        public int MinValue { get; set; }

        /// <summary>
        /// 平分最大值
        /// </summary>
        [SettingItem]
        public int MaxValue { get; set; }

        /// <summary>
        /// 24小时内允许评分最大值 （即所评分数的绝对值之和）
        /// </summary>
        [SettingItem]
        public int MaxValueInTime { get; set; }


        public UserPoint UserPoint
        {
            get
            {
                return AllSettings.Current.PointSettings.GetUserPoint(PointType);
            }
        }


        #region ICloneable 成员

        public object Clone()
        {
            RateSetItem item = new RateSetItem();
            item.MaxValue = MaxValue;
            item.MaxValueInTime = MaxValueInTime;
            item.MinValue = MinValue;
            item.PointType = PointType;
            item.RoleID = RoleID;
            item.RoleSortOrder = RoleSortOrder;

            return item;
        }

        #endregion

    }

    public class RateSetItemCollection : Collection<RateSetItem>, ISettingItem
	{
        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (RateSetItem item in this)
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
                    RateSetItem rankItem = new RateSetItem();

                    rankItem.Parse(item);
                    this.Add(rankItem);

                }
            }
        }

        #endregion
    }
}