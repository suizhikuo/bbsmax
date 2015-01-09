//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Errors
{
    public class NotEnoughPointBuyAttachment : ErrorInfo
    {
        public NotEnoughPointBuyAttachment(string target, UserPoint point, int price, int myPoint)
            : base(target)
        {
            Point = point;
            Price = price;
            MyPoint = myPoint;
        }

        public UserPoint Point
        {
            get;
            private set;
        }

        public int Price
        {
            get;
            private set;
        }

        public int MyPoint
        {
            get;
            private set;
        }

        public override bool HtmlEncodeMessage
        {
            get
            {
                return false;
            }
        }

        public override string Message
        {
            get
            {
                string url = "";
                if(PostBOV5.Instance.IsShowChargePointLink(Point))
                    url = string.Format("[ <a href=\"{0}\" target=\"_blank\">立即充值</a> ]",BbsRouter.GetUrl("my/pay"));
                return string.Format("您的{0}不足,不能购买该附件(您当前还有<span style=\"color:red\">{0}{1}{2}</span>,购买该附件需要<span style=\"color:red\">{0}{3}{2}</span> {4})", Point.Name, MyPoint, Point.UnitName, Price, url);
            }
        }
    }
}