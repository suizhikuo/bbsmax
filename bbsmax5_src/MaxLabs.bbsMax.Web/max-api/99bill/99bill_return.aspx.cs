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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_api._99bill
{
    public partial class _9bill_return : System.Web.UI.Page
    {
        public int rtnOk = 0;
        public string rtnUrl = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string merchantAcctId = Request["merchantAcctId"].ToString().Trim();
                string version = Request["version"].ToString().Trim();
                string language = Request["language"].ToString().Trim();
                string signType = Request["signType"].ToString().Trim();
                string payType = Request["payType"].ToString().Trim();
                string bankId = Request["bankId"].ToString().Trim();
                string orderNo = Request["orderId"].ToString().Trim();
                string orderTime = Request["orderTime"].ToString().Trim();
                string orderAmount = Request["orderAmount"].ToString().Trim();
                string dealId = Request["dealId"].ToString().Trim();
                string bankDealId = Request["bankDealId"].ToString().Trim();
                string dealTime = Request["dealTime"].ToString().Trim();
                string payAmount = Request["payAmount"].ToString().Trim();
                string fee = Request["fee"].ToString().Trim();
                string ext1 = Request["ext1"].ToString().Trim();
                string ext2 = Request["ext2"].ToString().Trim();
                string payResult = Request["payResult"].ToString().Trim();
                string errCode = Request["errCode"].ToString().Trim();
                string signMsg = Request["signMsg"].ToString().Trim();
                string payIp = Request.UserHostAddress;
                DateTime payDate = DateTimeUtil.Now;
                bool success = PayBO.Instance._99BillReturn(merchantAcctId, version, language, signType, payType, bankId, orderNo, orderTime,
                   orderAmount, dealId, bankDealId, dealTime, payAmount, fee, ext1, ext2, payResult, errCode, signMsg, payIp, payDate);
                rtnOk = 1;
                rtnUrl = AllSettings.Current.PaySettings.PayResultUrl + "?no=" + orderNo;
                return;
            }
            catch (Exception ex)
            {
                Response.Write("没有获取到相应参数");
            }
        }
    }
}