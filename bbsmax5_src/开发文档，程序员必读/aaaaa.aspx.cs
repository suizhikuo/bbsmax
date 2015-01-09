//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class aaaaa :AdminPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected int RndMax
        {
            get
            {
                return 10000;
            }
        }

        protected int Width
        {
            get
            {
                return 500;
            }
        }

        protected int GetRndNumber(int rndMinValue, int rndMaxValue )
        {
            return new Random().Next(rndMaxValue, rndMaxValue);
        }

        protected string Weekday
        {
            get
            {
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Monday :
                        return "星期一";
                    case DayOfWeek.Tuesday:
                        return "星期二";
                    case  DayOfWeek.Wednesday :
                        return "星期三";
                    case DayOfWeek.Thursday:
                        return "星期四";
                    case DayOfWeek.Friday:
                        return "星期五";
                    case DayOfWeek.Saturday:
                        return "星期六";
                    default :
                        return "星期天";
                }

            }
        }
    }
}