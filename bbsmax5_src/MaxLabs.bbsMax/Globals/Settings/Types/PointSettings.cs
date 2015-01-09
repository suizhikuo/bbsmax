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
using System.Reflection;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class PointSettings : SettingBase//, ICloneable
    {
        public PointSettings()
        {
            UserPoints = new UserPointCollection();
            ExchangeProportions = new PointExchangeProportionCollection();

            ExchangeProportions.Add(UserPointType.Point1, 10);
            ExchangeProportions.Add(UserPointType.Point2, 1);
            ExchangeProportions.Add(UserPointType.Point3, 1);
            ExchangeProportions.Add(UserPointType.Point4, 1);
            ExchangeProportions.Add(UserPointType.Point5, 1);
            ExchangeProportions.Add(UserPointType.Point6, 1);
            ExchangeProportions.Add(UserPointType.Point7, 1);
            ExchangeProportions.Add(UserPointType.Point8, 1);

            EnablePointExchange = false;
            
            EnablePointTransfer = false;

            PointRechargeRules = new PointRechargeRuleCollection();

            PointExchangeRules = new PointExchangeRuleCollection();
            PointExchangeRule rule = new PointExchangeRule();
            rule.PointType = UserPointType.Point2;
            rule.TargetPointType = UserPointType.Point1;
            rule.TaxRate = 2;
            PointExchangeRules.Add(rule);

            GeneralPointName = "总积分";
            GeneralPointExpression = "p1+p2*10";
            DisplayGeneralPoint = true;
            TradeRate = 2;

            PointTransferRules = new PointTransferRuleCollection();
            PointTransferRule tRule = new PointTransferRule();
            tRule.CanTransfer = true;
            tRule.PointType = UserPointType.Point1;
            tRule.TaxRate = 2;
            PointTransferRules.Add(tRule);

            PointIcons = new PointIconCollection();
            PointIcon icon = new PointIcon();
            icon.IconCount = 4;
            icon.IconsString = "fortune_3.gif|fortune_2.gif|fortune_1.gif";
            icon.PointType = UserPointType.Point1;
            icon.PointValue = 1000;
            PointIcons.Add(icon);
        }

        /// <summary>
        /// 积分等级图标
        /// </summary>
        [SettingItem]
        public PointIconCollection PointIcons { get; set; }


        private UserPointCollection userPoints;
        /// <summary>
        /// 8个积分的设置 永远返回8个 并且是按积分1积分2...积分8 顺序排列
        /// </summary>
        [SettingItem]
        public UserPointCollection UserPoints 
        {
            get 
            {
                return userPoints;
            }
            set 
            {
                if (value.Count == 0)
                {
                    userPoints = GetDefaultUserPoints();
                    return;
                }
                userPoints = value;

                //补足8项
                if (userPoints.Count < 8)
                {
                    foreach (UserPoint userPoint in GetDefaultUserPoints())
                    {
                        bool has = false;
                        foreach (UserPoint tempUserPoint in userPoints)
                        {
                            if (tempUserPoint.Type == userPoint.Type)
                            {
                                has = true;
                                break;
                            }
                        }
                        if (!has)
                            userPoints.Add(userPoint);
                    }
                }

                //排序
                UserPointCollection tempPoints = new UserPointCollection();
                foreach (UserPoint userPoint in userPoints)
                {
                    int index = -1;
                    for (int i = 0; i < tempPoints.Count; i++)
                    {
                        if ((int)userPoint.Type < (int)tempPoints[i].Type)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index == -1)
                        tempPoints.Add(userPoint);
                    else
                        tempPoints.Insert(index, userPoint);
                }

                userPoints = tempPoints;

            }
        }

        /// <summary>
        /// 兑换比例 按8个积分
        /// </summary>
        [SettingItem]
        public PointExchangeProportionCollection ExchangeProportions { get; set; }

        [SettingItem]
        public bool EnablePointExchange { get; set; }

        /// <summary>
        /// 积分兑换规则
        /// </summary>
        [SettingItem]
        public PointExchangeRuleCollection PointExchangeRules { get; set; }



        [SettingItem]
        public bool EnablePointTransfer { get; set; }

        /// <summary>
        /// 积分转帐
        /// </summary>
        [SettingItem]
        public PointTransferRuleCollection PointTransferRules { get; set; }

        /// <summary>
        /// 积分充值规则
        /// </summary>
        [SettingItem]
        public PointRechargeRuleCollection PointRechargeRules { get; set; }


        /// <summary>
        /// 交易税率 （百分号前的数字）
        /// </summary>
        [SettingItem]
        public int TradeRate { get; set; }

        [SettingItem]
        public bool DisplayGeneralPoint { get; set; }

        /// <summary>
        /// 总积分名称
        /// </summary>
        [SettingItem]
        public string GeneralPointName { get; set; }

        /// <summary>
        /// 总积分公式
        /// </summary>
        [SettingItem]
        public string GeneralPointExpression { get; set; }

        #region ICloneable 成员

        //public object Clone()
        //{
        //    PointSettings setting = new PointSettings();
        //    setting.UserPoints = UserPoints;
        //    setting.TradeRate = TradeRate;
        //    setting.PointExchangeRules = PointExchangeRules;
        //    setting.GeneralPointName = GeneralPointName;
        //    setting.ExchangeProportions = ExchangeProportions;
        //    setting.GeneralPointExpression = GeneralPointExpression;
        //    setting.PointTransferRules = PointTransferRules;
        //    setting.PointIcons = PointIcons;
        //    setting.EnablePointExchange = EnablePointExchange;
        //    setting.e
        //    return setting;

        //}

        #endregion

        /// <summary>
        /// 获取当前系统的有效用户积分
        /// </summary>
        /// <returns></returns>
        public UserPointCollection EnabledUserPoints
        {
            get
            {
                UserPointCollection points = new UserPointCollection();
                foreach (UserPoint userPoint in UserPoints)
                {
                    if (userPoint.Enable)
                    {
                        points.Add(userPoint);
                    }
                }
                if (points.Count == 0)
                {
                    UserPoint userPoint = UserPoints[0];
                    userPoint.Enable = true;
                    points.Add(userPoint);
                }
                return points;
            }
        }

        public UserPoint GetUserPoint(UserPointType userPointType)
        {
            if (userPointType == UserPointType.GeneralPoint)
            {
                UserPoint point = new UserPoint();
                point.Name = GeneralPointName;
                point.UnitName = string.Empty;
                point.Type = userPointType;
                point.Enable = true;
                point.Display = DisplayGeneralPoint;
                return point;
            }
            foreach (UserPoint userPoint in UserPoints)
            {
                if (userPointType == userPoint.Type)
                    return userPoint;
            }
            return null;
        }

        public UserPointCollection GetDefaultUserPoints()
        {
            UserPointCollection tempPoints = new UserPointCollection();
            UserPoint point = new UserPoint();
            point.Type = UserPointType.Point1;
            point.Enable = true;
            point.Display = true;
            point.Name = "金钱";
            point.InitialValue = 1000;
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point2;
            point.Enable = true;
            point.Display = true;
            point.Name = "威望";
            point.InitialValue = 100;
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point3;
            point.Enable = false;
            point.Name = "积分3";
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point4;
            point.Enable = false;
            point.Name = "积分4";
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point5;
            point.Enable = false;
            point.Name = "积分5";
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point6;
            point.Enable = false;
            point.Name = "积分6";
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point7;
            point.Enable = false;
            point.Name = "积分7";
            tempPoints.Add(point);

            point = new UserPoint();
            point.Type = UserPointType.Point8;
            point.Enable = false;
            point.Name = "积分8";
            tempPoints.Add(point);
            return tempPoints;
        }

       
    }
}