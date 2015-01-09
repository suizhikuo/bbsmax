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
    public class UserPointOverMaxValueError : ErrorInfo
    {
        public UserPointOverMaxValueError(string target, UserPointType pointType, int canAddValue, int pointValue, int maxPointValue)
            : base(target) 
        {
            m_UserPointType = pointType;
            CanAddValue = canAddValue < 0 ? 0 : canAddValue;
            PointValue = pointValue;
            MaxPointValue = maxPointValue;
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
        /// 再加几分后刚好达到最大值
        /// </summary>
        public int CanAddValue{ get; private set; }

        /// <summary>
        /// 积分操作后  用户的积分
        /// </summary>
        public int PointValue { get; private set; }

        /// <summary>
        /// 系统允许的最大值
        /// </summary>
        public int MaxPointValue { get; private set; }

        public override string Message
        {
            get { return string.Format("{0}超出系统允许的最大值，您不能进行此操作", UserPoint.Type); }
        }
    }
}