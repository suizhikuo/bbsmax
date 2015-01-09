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
    public partial class user_pointtransfer : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "转帐 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "point-transfer"; }
        }

        protected override string NavigationKey
        {
            get { return "point-transfer"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("转帐");

            if (AllSettings.Current.PointSettings.EnablePointTransfer == false)
                ShowError("系统未开启转帐功能");

            if (_Request.IsClick("transferpoint"))
            {
                TransferPoint();
                return;
            }
        }


        private void TransferPoint()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("password", "pointvalue", "pointtype", "targetUsername");

            string password = _Request.Get("password", Method.Post, string.Empty, false);

            string valueString = _Request.Get("pointvalue", Method.Post, string.Empty);

            int pointValue;
            if (!int.TryParse(valueString, out pointValue))
            {
                msgDisplay.AddError("pointvalue", Lang_Error.User_UserPointTransferPointValueError);
            }

            valueString = _Request.Get("pointtype",Method.Post,string.Empty);
            if (string.Empty == valueString)
            {
                msgDisplay.AddError("pointtype", Lang_Error.User_UserPointEmptyTransferPointTypeError);
            }

            UserPointType pointType = StringUtil.TryParse<UserPointType>(valueString,UserPointType.Point1);

            string targetUsername = _Request.Get("targetusername", Method.Post, string.Empty);

            if (msgDisplay.HasAnyError())
                return;

            try
            {
                if (!UserBO.Instance.TransferPoint(My, password, targetUsername, pointType, pointValue))
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


        private UserPointCollection canTransferPoints;
        /// <summary>
        /// 可以转出去的积分
        /// </summary>
        protected UserPointCollection CanTransferPoints
        {
            get
            {
                if (canTransferPoints == null)
                {
                    canTransferPoints = new UserPointCollection();
                    foreach (PointTransferRule rule in AllSettings.Current.PointSettings.PointTransferRules)
                    {
                        if (rule.CanTransfer)
                        {
                            if (canTransferPoints.GetUserPoint(rule.PointType) == null)
                            {
                                canTransferPoints.Add(AllSettings.Current.PointSettings.GetUserPoint(rule.PointType));
                            }
                        }
                    }
                }
                return canTransferPoints;
            }
        }

        private PointTransferRuleCollection transferRules;
        protected PointTransferRuleCollection TransferRules
        {
            get
            {
                if (transferRules == null)
                {
                    transferRules = AllSettings.Current.PointSettings.PointTransferRules;
                }
                return transferRules;
            }
        }
    }
}