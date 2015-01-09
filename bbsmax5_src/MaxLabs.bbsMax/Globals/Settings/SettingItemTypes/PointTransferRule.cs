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
    /// 积分转换规则
    /// </summary>
    public sealed class PointTransferRule : SettingBase, ICloneable
	{

        public PointTransferRule()
        {
            TaxRate = 20;
        }
        /// <summary>
        /// 积分类型
        /// </summary>
        [SettingItem]
		public UserPointType PointType { get; set; }

        private bool canTransfer;
        /// <summary>
        /// 是否允许转换
        /// </summary>
        [SettingItem]
        public bool CanTransfer 
        {
            get 
            {
                if (canTransfer)
                {
                    //此处加try 是升级程序获取不到 UserPoint
                    try
                    {
                        if (UserPoint.Enable)
                            return canTransfer;
                        else
                            return false;
                    }
                    catch
                    {
                        return canTransfer;
                    }
                }
                return canTransfer;
            }
            set { canTransfer = value; } 
        }

        /// <summary>
        /// 转换税率 （百分号前的数字）
        /// </summary>
        [SettingItem]
		public int TaxRate { get; set; }

        /// <summary>
        /// 转换后剩余最低余额
        /// </summary>
        [SettingItem]
        public int MinRemainingValue { get; set; }

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


        #region ICloneable 成员

        public object Clone()
        {
            PointTransferRule rule = new PointTransferRule();
            rule.PointType = PointType;
            rule.TaxRate = TaxRate;
            rule.MinRemainingValue = MinRemainingValue;
            return rule;
        }

        #endregion
    }

    public class PointTransferRuleCollection : Collection<PointTransferRule>, ISettingItem
	{
        /// <summary>
        /// 检查是否已经存在该规则
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        //public bool IsExists(PointExchangeRule rule)
        //{
        //    foreach (PointExchangeRule tempRule in this)
        //    {
        //        if (rule.Key == tempRule.Key)
        //            return true;
        //    }
        //    return false;
        //}

        public PointTransferRule GetRule(UserPointType pointType)
        {
            foreach (PointTransferRule tempRule in this)
            {
                if (tempRule.PointType == pointType)
                    return tempRule;
            }
            return null;
        }

        //new public void Add(PointTransferRule rule)
        //{
        //    foreach (PointExchangeRule tempRule in this)
        //    {
        //        if (rule.Key == tempRule.Key)
        //        {
        //            return;
        //        }
        //    }
        //    base.Add(rule);
        //}

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PointTransferRule item in this)
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
                    PointTransferRule pointTransferRule = new PointTransferRule();

                    pointTransferRule.Parse(item);

                    this.Add(pointTransferRule);

                }
            }
        }

        #endregion
    }
}