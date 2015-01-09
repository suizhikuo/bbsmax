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
    /// 积分充值规则
    /// </summary>
    public class PointRechargeRule : SettingBase
	{

        [SettingItem]
        public bool Enable { get; set; }

        /// <summary>
        /// 积分类型  
        /// </summary>
        [SettingItem]
        public UserPointType UserPointType { get; set; }


        /// <summary>
        /// 需要人民币 
        /// </summary>
        [SettingItem]
        public int Money { get; set; }

        /// <summary>
        /// 可以充值的积分数
        /// </summary>
        [SettingItem]
        public int Point { get; set; }

        /// <summary>
        /// 一次最少需要充值积分数
        /// </summary>
        [SettingItem]
        public int MinValue { get; set; }
	}

    public class PointRechargeRuleCollection : Collection<PointRechargeRule>, ISettingItem
	{
        public new void Add(PointRechargeRule rule)
        {
            bool has = false;
            foreach (PointRechargeRule temp in this)
            {
                if (temp.UserPointType == rule.UserPointType)
                {
                    temp.Enable = rule.Enable;
                    temp.MinValue = rule.MinValue;
                    temp.Money = rule.Money;
                    temp.Point = rule.Point;
                    has = true;
                    break;
                }
            }
            if (has == false)
                base.Add(rule);
        }
        

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PointRechargeRule item in this)
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
                    PointRechargeRule rule = new PointRechargeRule();

                    rule.Parse(item);

                    this.Add(rule);

                }
            }
        }

        #endregion
    }
}