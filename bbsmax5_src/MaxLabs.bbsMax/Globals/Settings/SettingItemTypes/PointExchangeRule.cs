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

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 积分兑换规则
    /// </summary>
    public sealed class PointExchangeRule : SettingBase, ICloneable
	{

        public PointExchangeRule()
        {
            TaxRate = 2;
        }
        /// <summary>
        /// 要兑换的积分类型
        /// </summary>
        [SettingItem]
		public UserPointType PointType { get; set; }

        /// <summary>
        /// 要兑换到的目标积分类型
        /// </summary>
        [SettingItem]
        public UserPointType TargetPointType { get; set; }

        /// <summary>
        /// 兑换税率 （百分号前的数字）
        /// </summary>
        [SettingItem]
		public int TaxRate { get; set; }

        /// <summary>
        /// 兑换后剩余最低余额
        /// </summary>
        [SettingItem]
        public int MinRemainingValue { get; set; }

        /// <summary>
        /// 唯一标志
        /// </summary>
        public string Key
        {
            get { return PointType.ToString().ToLower() + "-" + TargetPointType.ToString().ToLower(); }
        }

        private UserPoint m_UserPoint;
        /// <summary>
        /// 要兑换的积分
        /// </summary>
        public UserPoint UserPoint
        {
            get
            {
                if (m_UserPoint == null)
                {
                    m_UserPoint = AllSettings.Current.PointSettings.GetUserPoint(PointType);
                }
                return m_UserPoint;
            }
        }

        private UserPoint m_TargetUserPoint;
        /// <summary>
        /// 要兑换成的目标积分
        /// </summary>
        public UserPoint TargetUserPoint
        {
            get
            {
                if (m_TargetUserPoint == null)
                {
                    m_TargetUserPoint = AllSettings.Current.PointSettings.GetUserPoint(TargetPointType);
                }
                return m_TargetUserPoint;
            }
        }


        #region ICloneable 成员

        public object Clone()
        {
            PointExchangeRule rule = new PointExchangeRule();
            rule.PointType = PointType;
            rule.TargetPointType = TargetPointType;
            rule.TaxRate = TaxRate;
            rule.MinRemainingValue = MinRemainingValue;
            return rule;
        }

        #endregion
    }

    public class PointExchangeRuleCollection : Collection<PointExchangeRule>, ISettingItem
	{
        /// <summary>
        /// 检查是否已经存在该规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public bool IsExists(PointExchangeRule rule)
        {
            foreach (PointExchangeRule tempRule in this)
            {
                if (rule.Key == tempRule.Key)
                    return true;
            }
            return false;
        }


        public PointExchangeRule GetRule(UserPointType pointType,UserPointType targetPointType)
        {
            PointExchangeRule rule = new PointExchangeRule();
            rule.PointType = pointType;
            rule.TargetPointType = targetPointType;
            return GetRule(rule.Key);
        }

        public PointExchangeRule GetRule(string key)
        {
            foreach (PointExchangeRule tempRule in this)
            {
                if (string.Compare(tempRule.Key,key,true) == 0)
                    return tempRule;
            }
            return null;
        }

        new public void Add(PointExchangeRule rule)
        {
            foreach (PointExchangeRule tempRule in this)
            {
                if (rule.Key == tempRule.Key)
                {
                    return;
                }
            }
            base.Add(rule);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PointExchangeRule item in this)
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
                    PointExchangeRule pointExchangeRule = new PointExchangeRule();

                    this.Add(pointExchangeRule);

                    pointExchangeRule.Parse(item);
                }
            }
        }

        #endregion
    }
}