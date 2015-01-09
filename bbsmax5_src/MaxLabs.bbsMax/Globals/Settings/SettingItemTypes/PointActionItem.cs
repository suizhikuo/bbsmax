//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public class PointActionItem : SettingBase
    {
        public PointActionItem()
		{
            RoleID = Guid.Empty;
            Action = string.Empty;
            MinRemaining = int.MinValue;
            MaxValue = 100;
            MinValue = 1;
            PointValues = new StringList();
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
        public string Action { get; set; }

        /// <summary>
        /// 积分类型
        /// </summary>
        [SettingItem]
        public UserPointType PointType { get; set; }

        /// <summary>
        /// 交易后允许剩余的最低余额 不能低于积分下限（null时为积分下限）
        /// </summary>
        [SettingItem]
        public int MinRemaining { get; set; }

        /// <summary>
        /// 交易的最小金额
        /// </summary>
        [SettingItem]
        public int MinValue { get; set; }

        /// <summary>
        /// 交易的最大金额
        /// </summary>
        [SettingItem]
        public int MaxValue { get; set; }



        /// <summary>
        /// 积分值 8个
        /// </summary>
        [SettingItem]
        public StringList PointValues { get; set; }


        private int[] points;
        /// <summary>
        /// 始终返回8个
        /// </summary>
        public int[] Points
        {
            get
            {
                if (points == null)
                {
                    points = new int[8];
                    for (int i = 0;i<PointValues.Count;i++)
                    {
                        if (i == 8)
                            break;
                        try
                        {
                            points[i] = int.Parse(PointValues[i]);
                        }
                        catch { }
                    }
                }

                return points;
            }
        }

        public int? DisplayMaxValue
        {
            get
            {
                if (MaxValue == int.MaxValue)
                    return null;
                else
                    return MaxValue;
            }
        }

        public int? DisplayMinRemaining
        {
            get
            {
                if (MinRemaining == int.MinValue)
                    return null;
                else
                    return MinRemaining;
            }
        }

    }


    public class PointActionItemCollection : Collection<PointActionItem>, ISettingItem
    {

		#region ISettingItem 成员

		public string GetValue()
		{
			StringList list = new StringList();

            foreach (PointActionItem item in this)
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
                    PointActionItem pointActionItem = new PointActionItem();

                    this.Add(pointActionItem);

                    pointActionItem.Parse(item);
				}
			}
		}

		#endregion
	}

	
}