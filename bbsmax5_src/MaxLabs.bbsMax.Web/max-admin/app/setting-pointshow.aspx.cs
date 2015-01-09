//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_admin.global
{
    public partial class setting_pointshow : AdminPageBase
    {

        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Setting_PointShow;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<PointShowSettings>("savesetting");
        }

        protected int TimeUnit
        {
            get;
            set;
        }

        private int[] TimeUnits = {86400,3600,60,1 };

        protected int Interval
        {
            get
            {
                int t = PointShowSettings.ClickInterval;
                TimeUnit = 1;
                foreach(int u in TimeUnits)
                {
                    if (u < t)
                    {
                        TimeUnit = u;
                        break;
                    }
                }

                return t / TimeUnit;
            }
        }

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "MinPrice")
            {
                int minPrice = _Request.Get<int>("minprice", Method.Post, 0);
                if (minPrice < 1)
                {
                    ThrowError(new CustomError("minprice", "最低上榜价格不能低于1"));
                    return false;
                }
            }
            else if (property.Name == "MaxPrice")
            {
                int maxrice = _Request.Get<int>("maxprice", Method.Post, 0);
                if (maxrice < 1)
                {
                    ThrowError(new CustomError("maxprice", "最高上榜价格不能低于1"));
                    return false;
                }
            }
            else if (property.Name == "IpClickCountInDay")
            {
                int val = _Request.Get<int>("IpClickCountInDay", Method.Post, 0);
                if (val < 0)
                {
                    ThrowError(new CustomError("IpClickCountInDay", "不能小于0"));
                    return false;
                }
            }
            else if (property.Name == "ClickInterval")
            {
                int interval = _Request.Get<int>("interval", Method.Post, 0);
                int timeunit = _Request.Get<int>("timeunit", Method.Post, 1);
                int trueValue = interval * timeunit;

                if (trueValue < 0)
                {
                    ThrowError(new CustomError("ClickInterval", "有效时间间隔不能小于0"));

                    return false;
                }

                property.SetValue(setting, trueValue, null);

                return true;
            }

            return base.SetSettingItemValue(setting, property);
        }



        protected UserPoint PointType
        {
            get
            {
                return PointSettings.GetUserPoint(PointShowSettings.UsePointType);
            }
        }
    }
}