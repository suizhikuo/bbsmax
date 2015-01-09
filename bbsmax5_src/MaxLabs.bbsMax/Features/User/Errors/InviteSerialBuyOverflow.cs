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

using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Errors
{
    public class InviteSerialBuyOverflow : ErrorInfo
    {
        private InviteBuyInterval _interval;
        private int _canBuyCount;
        public InviteSerialBuyOverflow(int canBuyCount, InviteBuyInterval interval)
        {
            _canBuyCount = canBuyCount;
            _interval = interval;
        }

        public override string Message
        {
            get
            {
                string intervalString = "";
                switch (_interval)
                {
                    case InviteBuyInterval.ByDay:
                        intervalString = Lang.Common_Day;
                        break;
                    case InviteBuyInterval.ByHour:
                        intervalString = Lang.Common_Hour;
                        break;
                    case InviteBuyInterval.ByMonth:
                        intervalString = Lang.Common_Month;
                        break;
                    case InviteBuyInterval.ByYear:
                        intervalString = Lang.Common_Year;
                        break;
                }

                return string.Format(Lang_Error.Invite_SerialBuyOverflow, intervalString, _canBuyCount);

            }
        }
    }
}