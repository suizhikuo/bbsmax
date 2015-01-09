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
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web
{
    public class payresult : BbsPageBase
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Userpay == null)
            {
                ShowError(new InvalidParamError("no"));
            }
        }

        protected override string PageName
        {
            get { return "payresult"; }
        }

        private string m_OrderNo;
        protected string OrderNo
        {
            get
            {
                if (m_OrderNo == null)
                {
                    m_OrderNo = _Request.Get("no", Method.Get);
                    if (m_OrderNo == null)
                        ShowError("缺少参数“no”");
                }
                return m_OrderNo;
            }
        }

        private UserPay m_Userpay;
        protected UserPay Userpay
        {
            get
            {
                if (m_Userpay == null)
                {
                    m_Userpay = PayBO.Instance.GetUserPay(OrderNo);
                }
                return m_Userpay;
            }
        }
    }
}