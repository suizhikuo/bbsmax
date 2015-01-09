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

namespace MaxLabs.bbsMax.Web.max_api.tenpay
{
    public partial class tenpay_return : System.Web.UI.Page
    {
        /// <summary>
        /// 支付结果处理页面,根据回跳时的URL所带的参数取得支付结果。
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string resultHtml = "<meta content=\"China TENCENT\" name=\"TENCENT_ONLINE_PAYMENT\">\n"
                + "<script language=\"javascript\">\n"
                + "window.location.href='{0}';\n"
                + "</script>";

                string errormsg = "";
                string bargainorID = AllSettings.Current.PaySettings.Tenpay_BargainorID;
                string key = AllSettings.Current.PaySettings.Tenpay_Key;
                //判断签名
                if (Pay.Md5Pay.GetPayValueFromUrl(Request.QueryString, bargainorID, key, out errormsg))
                {
                    //认证签名成功
                    //支付判断
                    if (Pay.Md5Pay.PayResult == Pay.Md5Pay.PAYOK)
                    {
                        //支付成功，同定单号md5pay.Transaction_id可能会多次通知，请务必注意判断订单是否重复的逻辑
                        //处理业务逻辑，处理db之类的
                        //errmsg = "支付成功";
                        //Response.Write(errmsg+"<br/>");
                        //Response.Write("财付通定单号:"+ md5pay.Transaction_id +"(请牢记定单号)"+"<br/>");	

                        //跳转到成功页面，财付通收到<meta content=\"China TENCENT\" name=\"TENCENT_ONLINE_PAYMENT\">，认为通知成功

                        string orderNo = Request.QueryString["sp_billno"].ToString();
                        string transactionNo = Request.QueryString["transaction_id"].ToString();
                        string payIp = Request.UserHostAddress;
                        DateTime payDate = DateTimeUtil.Now;
                        bool success = PayBO.Instance.TenpayReturn(orderNo, transactionNo, payIp, payDate);
                        string resultUrl = string.Format(resultHtml, AllSettings.Current.PaySettings.PayResultUrl + "?no=" + orderNo);
                        Response.Write(resultUrl);
                        return;
                    }
                }
                //认证签名失败
                Response.Write("没有获取到相应参数");
                return;
            }
            catch
            {
                Response.Write("没有获取到相应参数");
            }
        }
    }
}