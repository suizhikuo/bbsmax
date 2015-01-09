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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web
{
    public class pay : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "充值 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "pay"; }
        }

        protected override string NavigationKey
        {
            get { return "pay"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddNavigationItem("充值");

            if (AllSettings.Current.PaySettings.EnablePointRecharge == false)
                ShowError("系统未开启充值功能");

            if (Points.Count == 0)
            {
                ShowError("管理员还未设置可以充值的积分");
            }

            if (_Request.IsClick("confirmResult"))
            {
                UserPay();
            }
        }

        protected PointRechargeRuleCollection Rules
        {
            get
            {
                return AllSettings.Current.PointSettings.PointRechargeRules;
            }
        }

        private UserPointCollection m_Points;
        protected UserPointCollection Points
        {
            get
            {
                if (m_Points == null)
                {
                    m_Points = new UserPointCollection();
                    foreach (UserPoint point in AllSettings.Current.PointSettings.EnabledUserPoints)
                    {
                        if (point.Enable == false)
                            continue;
                        foreach (PointRechargeRule rule in Rules)
                        {
                            if (point.Type == rule.UserPointType && rule.Enable)
                            {
                                m_Points.Add(point);
                            }
                        }
                    }
                }

                return m_Points;
            }
        }

        protected int GetPoint(UserPointType type)
        {
            return My.ExtendedPoints[(int)type];
        }


        protected void UserPay()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            string orderNo = System.DateTime.Now.ToString("yyyyMMddHHmmssfffff");

            UserPointType? type = _Request.Get<UserPointType>("currentPointType", Method.Post);

            if (type == null)
            {
                msgDisplay.AddError("请选择要充值的积分类型");
                return;
            }

            //byte payType = _Request.Get<byte>("paytypename", Method.Post,1);
            byte payment = _Request.Get<byte>("payment", Method.Post, 1);
            int payValue = _Request.Get<int>("payvalue", Method.Post, 0);
            //decimal orderAmount = 0.01M;
            string submitIp = _Request.IpAddress;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    PayUrl = PayBO.Instance.CreateUserPay(My, orderNo, payment, type.Value, payValue, submitIp);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                    return;
                }

                if (!string.IsNullOrEmpty(PayUrl))
                {
                    //Response.Redirect(PayUrl);
                    Response.Write("<script type=\"text/javascript\">opener.showMask();location.href='" + PayUrl + "';</script>");
                    Response.End();
                }
                else
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        ShowError(error);
                    });

                    return;
                }
            }

        }


        private Dictionary<UserPointType, string> m_ErrorMessages;
        protected Dictionary<UserPointType, string> ErrorMessages
        {
            get
            {
                if (m_ErrorMessages == null)
                {
                    GetData();
                }
                return m_ErrorMessages;
            }
        }

        private void GetData()
        {
            m_ErrorMessages = new Dictionary<UserPointType, string>();
            //m_MinValues = new Dictionary<UserPointType, int>();
            foreach (PointRechargeRule rule in AllSettings.Current.PointSettings.PointRechargeRules)
            {
                // m_MinValues.Add(rule.UserPointType, rule.MinValue);
                UserPoint userPoint = AllSettings.Current.PointSettings.GetUserPoint(rule.UserPointType);
                m_ErrorMessages.Add(rule.UserPointType, "一次最少需要充值" + rule.MinValue + userPoint.UnitName + userPoint.Name);
            }
        }

        //private Dictionary<UserPointType, int> m_MinValues;
        //protected Dictionary<UserPointType, int> MinValues
        //{
        //    get
        //    {
        //        if (m_MinValues == null)
        //        {
        //            GetData();
        //        }
        //        return m_MinValues;
        //    }
        //}


        public bool EnableAlipay
        {
            get { return AllSettings.Current.PaySettings.EnableAlipay; }
        }

        public bool EnableTenpay
        {
            get { return AllSettings.Current.PaySettings.EnableTenpay; }
        }

        public bool Enable99Bill
        {
            get { return AllSettings.Current.PaySettings.Enable99Bill; }
        }

        protected bool EnableMany
        {
            get
            {
                int t = 0;
                if (Enable99Bill) t++;
                if (EnableAlipay) t++;
                if (EnableTenpay) t++;
                return t > 1;
            }
        }

        protected bool IsPostSuccess;

        protected string PayUrl;
    }
}