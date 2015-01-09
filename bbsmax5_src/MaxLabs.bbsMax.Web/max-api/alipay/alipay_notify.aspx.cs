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
using System.Collections.Specialized;
using System.Text;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_api.alipay
{
    public partial class alipay_notify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request.Form.AllKeys.Length > 0)
                {
                    string notifyID = Request.Form["notify_id"].ToString();
                    string sign = Request.Form["sign"];
                    string tradeStatus = Request.Form["trade_status"];
                    string buyerEmail = Request.Form["buyer_email"];
                    string orderNo = Request.Form["out_trade_no"];
                    string transactionNo = Request.Form["trade_no"];
                    string payIp = Request.UserHostAddress;
                    string key = AllSettings.Current.PaySettings.Alipay_Key;
                    DateTime payDate = DateTimeUtil.Now;
                    int i;
                    NameValueCollection coll;
                    coll = Request.Form;
                    String[] requestarr = coll.AllKeys;

                    //进行排序；
                    string[] Sortedstr = Pay.AliPay.BubbleSort(requestarr);

                    //构造待md5摘要字符串 ；

                    StringBuilder prestr = new StringBuilder();
                    for (i = 0; i < Sortedstr.Length; i++)
                    {
                        if (Request.Form[Sortedstr[i]] != "" && Sortedstr[i] != "sign" && Sortedstr[i] != "sign_type")
                        {
                            prestr.Append(Sortedstr[i]).Append("=").Append(Request.Form[Sortedstr[i]]);

                            if (i != Sortedstr.Length - 1)
                            {
                                prestr.Append("&");
                            }
                        }

                    }
                    prestr.Append(key);

                    bool success = PayBO.Instance.AlipayReturn(notifyID, prestr.ToString(), buyerEmail, orderNo, transactionNo, sign, tradeStatus, payIp, payDate);
                    string resultUrl = string.Format("<script type=\"text/javascript\">window.location.href='{0}';</script>", AllSettings.Current.PaySettings.PayResultUrl + "?no=" + orderNo);
                    Response.Write(resultUrl);
                    return;
                }
            }
            catch
            {
                Response.Write("没有获取到相应的参数");
            }
        }
    }
}