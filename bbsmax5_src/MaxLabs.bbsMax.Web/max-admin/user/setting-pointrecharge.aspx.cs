//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_pointrecharge : AdminPageBase //: SettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_PointRecharge; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("save"))
            {
                Save();
            }
        }

        protected PointRechargeRuleCollection Rules
        {
            get
            {
                return AllSettings.Current.PointSettings.PointRechargeRules;
            }
        }

        protected PointRechargeRule GetRule(UserPointType type)
        {
            foreach (PointRechargeRule rule in Rules)
            {
                if (rule.UserPointType == type)
                    return rule;
            }

            return null;
        }


        private void Save()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("money", "point", "minvalue");
            PointRechargeRuleCollection rules = new PointRechargeRuleCollection();
            int i = 0;
            foreach (UserPoint point in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                int pointID = (int)point.Type;
                PointRechargeRule rule = new PointRechargeRule();
                rule.UserPointType = point.Type;
                rule.Enable = _Request.IsChecked("canRecharge." + pointID, Method.Post, false);
                rule.Money = _Request.Get<int>("money." + pointID, Method.Post, 0);
                rule.Point = _Request.Get<int>("point." + pointID, Method.Post, 0);
                rule.MinValue = _Request.Get<int>("minvalue." + pointID, Method.Post, 0);

                if (rule.Enable)
                {
                    if (rule.Money < 1)
                    {
                        msgDisplay.AddError("money", i, "人民币必须为大于0的整数");
                    }

                    if (rule.Point < 1)
                    {
                        msgDisplay.AddError("point", i, "积分数量必须为大于0的整数");
                    }

                    if (rule.Point < 1)
                    {
                        msgDisplay.AddError("minvalue", i, "一次至少需要充值的积分数量必须大于0");
                    }
                }
                rules.Add(rule);
                i++;
            }

            if (msgDisplay.HasAnyError())
                return;


            PointSettings pointSetting = SettingManager.CloneSetttings<PointSettings>(PointSettings);
            PaySettings paySettings = SettingManager.CloneSetttings<PaySettings>(PaySettings);


            pointSetting.PointRechargeRules = rules;
            paySettings.EnablePointRecharge = _Request.Get<bool>("enable", Method.Post, false);
            if (paySettings.EnablePointRecharge)
            {
                paySettings.ProductName = _Request.Get("ProductName", Method.Post, string.Empty);
                paySettings.EnableAlipay = _Request.Get<bool>("enableAlipay", Method.Post, false);
                paySettings.EnableTenpay = _Request.Get<bool>("EnableTenpay", Method.Post, false);
                paySettings.Enable99Bill = _Request.Get<bool>("Enable99Bill", Method.Post, false);

                if (paySettings.EnableAlipay)
                {
                    paySettings.Alipay_SellerEmail = _Request.Get("Alipay_SellerEmail", Method.Post, string.Empty);
                    paySettings.Alipay_PartnerID = _Request.Get("Alipay_PartnerID", Method.Post, string.Empty);
                    paySettings.Alipay_Key = _Request.Get("Alipay_Key", Method.Post, string.Empty);
                }

                if (paySettings.EnableTenpay)
                {
                    paySettings.Tenpay_BargainorID = _Request.Get("Tenpay_BargainorID", Method.Post, string.Empty);
                    paySettings.Tenpay_Key = _Request.Get("Tenpay_Key", Method.Post, string.Empty);
                }

                if (paySettings.Enable99Bill)
                {
                    paySettings._99Bill_MerchantAcctID = _Request.Get("_99Bill_MerchantAcctID", Method.Post, string.Empty);
                    paySettings._99Bill_Key = _Request.Get("_99Bill_Key", Method.Post, string.Empty);
                }
            }
            else
            {
                paySettings.EnableAlipay = false;
                paySettings.EnableTenpay = false;
                paySettings.Enable99Bill = false;
            }

            if (paySettings.EnablePointRecharge)
            {
                foreach (PointRechargeRule rule in rules)
                {
                    if (rule.Enable)
                    {
                        pointSetting.UserPoints.GetUserPoint(rule.UserPointType).MaxValue = int.MaxValue;
                    }
                }
            }

            try
            {
                SettingManager.SaveSettings(pointSetting);
                SettingManager.SaveSettings(paySettings);

                PostBOV5.Instance.ClearShowChargePointLinks();
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

    }
}