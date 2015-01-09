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
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Errors
{
    public class UserPointTradeRemainingError : ErrorInfo
    {
        public UserPointTradeRemainingError(string target, string pointName,int remainingValue,int minRemainingValue)
            : base(target) 
        {
            m_PointName = pointName;
            RemainingValue = remainingValue;
            MinRemainingValue = minRemainingValue;
        }

        private string m_PointName;
        public string PointName
        {
            get
            {
                return m_PointName;
            }
            set
            {
                m_PointName = value;
            }
        }
        public int RemainingValue { get; private set; }
        public int MinRemainingValue { get; private set; }
        public override string Message
        {
            get { return string.Format(Lang_Error.User_UserPointTradeRemainingError, PointName, RemainingValue, MinRemainingValue); }
        }
    }
}