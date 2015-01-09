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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class user_pointexchange : CenterPageBase
    {

        protected override string PageTitle
        {
            get { return "兑换 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "point-exchange"; }
        }

        protected override string NavigationKey
        {
            get { return "point-exchange"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("兑换");

            if (AllSettings.Current.PointSettings.EnablePointExchange == false)
                ShowError("系统未开启积分兑换功能");

            if (_Request.IsClick("exchangepoint"))
            {
                ExchangePoint();
                return;
            }
        }

        private void ExchangePoint()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("password", "pointvalue", "pointtype", "targetpointtype");

            string password = _Request.Get("password", Method.Post, string.Empty, false);

            string valueString = _Request.Get("pointvalue", Method.Post, string.Empty);

            int pointValue;
            if (!int.TryParse(valueString, out pointValue))
            {
                msgDisplay.AddError("pointvalue", Lang_Error.User_UserPointExechangePointValueError);
            }

            valueString = _Request.Get("pointtype",Method.Post,string.Empty);
            if (string.Empty == valueString)
            {
                msgDisplay.AddError("pointtype", Lang_Error.User_UserPointEmptyExchangePointTypeError);
            }

            UserPointType pointType = StringUtil.TryParse<UserPointType>(valueString,UserPointType.Point1);

            valueString = _Request.Get("targetpointtype", Method.Post, string.Empty);
            if (string.Empty == valueString)
            {
                msgDisplay.AddError("targetpointtype", Lang_Error.User_UserPointEmptyExchangeTargetPointTypeError);
            }


            UserPointType targetPointType = StringUtil.TryParse<UserPointType>(valueString, UserPointType.Point1);

            if (msgDisplay.HasAnyError())
                return;

            try
            {
                if (!UserBO.Instance.ExechangePoint(My, password, pointType, targetPointType, pointValue))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                    _Request.Clear(Method.Post);
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }

        }


        private UserPointCollection canExchangePoints;
        /// <summary>
        /// 可以兑换出去的积分
        /// </summary>
        protected UserPointCollection CanExchangePoints
        {
            get
            {
                if (canExchangePoints == null)
                {
                    canExchangePoints = new UserPointCollection();
                    foreach (PointExchangeRule rule in AllSettings.Current.PointSettings.PointExchangeRules)
                    {
                        if (canExchangePoints.GetUserPoint(rule.PointType) == null)
                        {
                            canExchangePoints.Add(AllSettings.Current.PointSettings.GetUserPoint(rule.PointType));
                        }
                    }
                }
                return canExchangePoints;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        protected PointExchangeRuleCollection GetPointExchangeRules(UserPointType pointType)
        {
            PointExchangeRuleCollection rules = new PointExchangeRuleCollection();
            foreach (PointExchangeRule rule in AllSettings.Current.PointSettings.PointExchangeRules)
            {
                if (rule.PointType == pointType)
                    rules.Add(rule);
            }
            return rules;
        }

        protected bool isEnd(UserPointType pointType,int index)
        {
            if (index == GetPointExchangeRules(pointType).Count - 1)
                return true;
            return false;
        }

        protected string GetExchangeProportion(UserPointType pointType,UserPointType targetPointType)
        {
            PointExchangeProportion proportion = null;
            PointExchangeProportion targetProportion = null;
            foreach (PointExchangeProportion tempProportion in AllSettings.Current.PointSettings.ExchangeProportions)
            {
                if (tempProportion.UserPointType == pointType)
                {
                    proportion = tempProportion;
                    if (targetProportion != null)
                        break;
                }
                if (tempProportion.UserPointType == targetPointType)
                {
                    targetProportion = tempProportion;
                    if (proportion != null)
                        break;
                }
            }

            int greatestCommonDivisor = MathUtil.GetGreatestCommonDivisor(proportion.Proportion, targetProportion.Proportion);

            return proportion.Proportion / greatestCommonDivisor + ":" + (targetProportion.Proportion / greatestCommonDivisor);
        }
    }
}