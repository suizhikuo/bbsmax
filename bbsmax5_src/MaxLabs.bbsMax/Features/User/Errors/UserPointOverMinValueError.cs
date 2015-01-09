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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Errors
{
    public class UserPointOverMinValueError : ErrorInfo
    {
        public UserPointOverMinValueError(string target, UserPointType pointType, int canReduceValue, int pointValue, int minPointValue)
            : base(target) 
        {
            CanReduceValue = canReduceValue < 0 ? 0 : canReduceValue;
            m_UserPointType = pointType;
            PointValue = pointValue;
            MinPointValue = minPointValue;
        }

        private UserPointType m_UserPointType;
        public UserPoint UserPoint
        {
            get
            {
                return AllSettings.Current.PointSettings.UserPoints.GetUserPoint(m_UserPointType);
            }
        }

        /// <summary>
        /// 再减少多少后 刚好达到最小值
        /// </summary>
        public int CanReduceValue { get; private set; }

        /// <summary>
        /// 积分操作后  用户的积分
        /// </summary>
        public int PointValue { get; private set; }

        /// <summary>
        /// 系统允许的最小值
        /// </summary>
        public int MinPointValue { get; private set; }

        public override string Message
        {
            get { return string.Format("{0}超出系统允许的最小值，您不能进行此操作", UserPoint.Name); }
        }
    }
}