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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Errors
{
    /// <summary>
    /// 购买邀请码时的积分错误
    /// </summary>
    public class BuyInviteSerialPointError : ErrorInfo
    {
        string _pointName, _reason = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointName">积分名称</param>
        /// <param name="reason">原因(超出 or 低于)</param>
        public BuyInviteSerialPointError(string pointName, Type errorType)
        {
            _pointName = pointName;
            if (errorType == typeof(UserPointOverMaxValueError))
            {
                _reason = Lang_Error.Global_BeyondMax;
            }
            else if (errorType == typeof(UserPointOverMinValueError))
            {
                _reason = Lang_Error.Global_BeyondMin;
            }
        }

        public override string Message
        {
            get { return string.Format(Lang_Error.User_BuyInviteSerialPointError, _pointName, _reason); }
        }
    }
}