//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Errors;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax
{
    public class PayBO : BOBase<PayBO>
    {

        public string CreateUserPay(User operatorUser, string orderNo, byte payment, UserPointType payType, int payValue, string submitIp)
        {
            if (operatorUser == User.Guest)
            {
                ThrowError(new NotLoginError());
                return null;
            }

            UserPoint userPoint = AllSettings.Current.PointSettings.GetUserPoint(payType);

            if (userPoint.Enable == false)
            {
                ThrowError(new CustomError("", "您要充值的积分“" + userPoint.Name + "”未启用"));
                return null;
            }

            decimal orderAmount = 0;
            bool hasRule = false;
            foreach (PointRechargeRule rule in AllSettings.Current.PointSettings.PointRechargeRules)
            {
                if (rule.UserPointType == payType)
                {
                    if (rule.Enable == false)
                    {
                        ThrowError(new CustomError("", "您不能对该积分进行充值"));
                        return null;
                    }
                    if (payValue < rule.MinValue)
                    {
                        ThrowError(new CustomError("", "一次最少需要充值" + rule.MinValue + userPoint.UnitName + userPoint.Name));
                        return null;
                    }

                    decimal temp = (decimal)rule.Money * payValue / rule.Point;

                    //保留两位小数  进一法
                    temp = temp - (decimal)0.005;
                    if (temp == (decimal)0.005)
                        temp = (decimal)0.006;

                    orderAmount = decimal.Round(temp, 2);
                    if(rule.Money * payValue * 100 % rule.Point > 0)
                    {
                        orderAmount = orderAmount + (decimal)0.01;
                    }


                    hasRule = true;
                    break;
                }
            }

            if (hasRule == false)
            {
                ThrowError(new CustomError("", "管理员未设置可充值的积分"));
                return null;
            }

            string payTypeName = userPoint.Name;


            string payUrl = string.Empty;
            int userID = operatorUser.UserID;
            //充值方式
            switch (payment)
            {
                case 1:
                    if (AllSettings.Current.PaySettings.EnableAlipay == false)
                    {
                        ThrowError(new CustomError("", "管理员已关闭支付宝充值方式"));
                        return null;
                    }
                    payUrl = Alipay(userID, orderNo, orderAmount, payment, (byte)payType, payTypeName, payValue, submitIp);
                    break;
                case 2:
                    if (AllSettings.Current.PaySettings.EnableTenpay == false)
                    { 
                        ThrowError(new CustomError("","管理员已关闭财付通充值方式"));
                        return null;
                    }
                    payUrl = Tenpay(userID, orderNo, orderAmount, payment, (byte)payType, payTypeName, payValue, submitIp);
                    break;
                case 3:
                    if (AllSettings.Current.PaySettings.Enable99Bill == false)
                    {
                        ThrowError(new CustomError("", "管理员已关闭快钱充值方式"));
                        return null;
                    }
                    payUrl = _99Bill(userID, orderNo, orderAmount, payment, (byte)payType, payTypeName, payValue, submitIp);
                    break;
                default:
                    ThrowError(new CustomError("payment", "没有选择支付方式"));
                    break;
            }
            return payUrl;
        }

        public UserPay GetUserPay(string orderNo)
        {
            return PayDao.Instance.GetUserPay(orderNo);
        }

        public UserPayCollection GetUserPays(int userID, PaylogFilter filter, int pageSize, int pageNumber)
        {
            return PayDao.Instance.GetUserPays(userID, filter, pageSize, pageNumber);
        }

        public UserPayCollection AdminSearchUserPays(AuthUser operatorUser, PaylogFilter filter, int pageSize, int pageIndex)
        {
            if (!operatorUser.IsManager)
                return new UserPayCollection();
            return PayDao.Instance.AdminSearchUserPays(filter, pageSize, pageIndex);
        }

        //财付通充值
        private string Tenpay(int userID, string orderNo, decimal orderAmount, byte payment, byte payType, string payTypeName, int payValue, string submitIp)
        {
            string bargainorID = AllSettings.Current.PaySettings.Tenpay_BargainorID;
            string key = AllSettings.Current.PaySettings.Tenpay_Key;
            //商品名称
            string productName = string.Format(AllSettings.Current.PaySettings.ProductName,payValue.ToString(),payTypeName);
            string returnUrl = AllSettings.Current.PaySettings.Tenpay_ReturnUrl;
            string date = System.DateTime.Now.ToString("yyyyMMdd");
            //商品总金额,以分为单位.
            long totalFee = Convert.ToInt64(orderAmount * 100);

            //財付通交易号,需保证此订单号每天唯一,切不能重复！
            string transactionNo = AllSettings.Current.PaySettings.Tenpay_BargainorID + date + Pay.Md5Pay.UnixStamp();

            //交易标识
            string productDesc = AllSettings.Current.PaySettings.Tenpay_Attach;

            string spbillCreateip = submitIp;
            string tenpayUrl = Pay.Md5Pay.GetPayUrl(date, bargainorID, key, orderNo, totalFee, transactionNo, productName, productDesc, returnUrl, spbillCreateip);
            //充值记录信息记入数据库.
            bool success = PayDao.Instance.CreateUserPayItem(userID, orderNo, orderAmount, payment, payType, payValue, submitIp, productName);
            if (success)
            {
                return tenpayUrl;
            }
            else
            {
                return null;
            }
        }

        //快钱充值
        private string _99Bill(int userID, string orderNo, decimal orderAmount, byte payment, byte payType, string payTypeName, int payValue, string submitIp)
        {
            string InputCharset = Pay._99Bill.InputCharset;
            string returnUrl = AllSettings.Current.PaySettings._99Bill_ReturnUrl;
            string version = Pay._99Bill.version;
            string language = Pay._99Bill.language;
            string signType = Pay._99Bill.signType;
            string merchantAcctID = AllSettings.Current.PaySettings._99Bill_MerchantAcctID;
            string productName = string.Format(AllSettings.Current.PaySettings.ProductName, payValue.ToString(),payTypeName);
            string productDesc = productName;
            string totalFee = ((int)(orderAmount * 100)).ToString();
            string PayerName = "";
            string payerContactType = Pay._99Bill.payerContactType;
            string productNum = Pay._99Bill.productNum;
            string paymentType = Pay._99Bill.payType;
            string redoFlag = Pay._99Bill.redoFlag;
            string key = AllSettings.Current.PaySettings._99Bill_Key;
            //订单提交时间
            string orderTime = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            StringBuilder signMsgValBuilder = new StringBuilder();
            Pay._99Bill.appendParam(signMsgValBuilder, "inputCharset", InputCharset);
            Pay._99Bill.appendParam(signMsgValBuilder, "bgUrl", returnUrl);
            Pay._99Bill.appendParam(signMsgValBuilder, "version", version);
            Pay._99Bill.appendParam(signMsgValBuilder, "language", language);
            Pay._99Bill.appendParam(signMsgValBuilder, "signType", signType);
            Pay._99Bill.appendParam(signMsgValBuilder, "merchantAcctId", merchantAcctID);
            Pay._99Bill.appendParam(signMsgValBuilder, "payerName", PayerName);
            Pay._99Bill.appendParam(signMsgValBuilder, "payerContactType", payerContactType);
            Pay._99Bill.appendParam(signMsgValBuilder, "payerContact", "");
            Pay._99Bill.appendParam(signMsgValBuilder, "orderId", orderNo);
            Pay._99Bill.appendParam(signMsgValBuilder, "orderAmount", totalFee);
            Pay._99Bill.appendParam(signMsgValBuilder, "orderTime", orderTime);
            Pay._99Bill.appendParam(signMsgValBuilder, "productName", productName);
            Pay._99Bill.appendParam(signMsgValBuilder, "productId", "");
            Pay._99Bill.appendParam(signMsgValBuilder, "productNum", productNum);
            Pay._99Bill.appendParam(signMsgValBuilder, "productDesc", productDesc);
            Pay._99Bill.appendParam(signMsgValBuilder, "ext1", "");
            Pay._99Bill.appendParam(signMsgValBuilder, "ext2", "");
            Pay._99Bill.appendParam(signMsgValBuilder, "payType", paymentType);
            Pay._99Bill.appendParam(signMsgValBuilder, "redoFlag", redoFlag);
            Pay._99Bill.appendParam(signMsgValBuilder, "pid", "");

            string signMsgVal = signMsgValBuilder.ToString();

            string signResultValue = string.Concat(signMsgVal, "&key=", key);
            string signMsg = Pay._99Bill.GetMD5(signResultValue, "utf-8").ToUpper();

            string kqBillUrl = string.Concat(Pay._99Bill._99BillUrl, "?", signMsgVal, "&signMsg=", signMsg);
            //充值记录信息记入数据库.
            bool success = PayDao.Instance.CreateUserPayItem(userID, orderNo, orderAmount, payment, payType, payValue, submitIp, productName);
            if (success)
            {
                return kqBillUrl;
            }
            else
            {
                return null;
            }
        }

        //支付宝充值
        private string Alipay(int userID, string orderNo, decimal orderAmount, byte payment, byte payType, string payTypeName, int payValue, string submitIp)
        {
            //业务参数赋值；
            string gateway = Pay.AliPay.Getway;
            string service = Pay.AliPay.Service;
            string partner = AllSettings.Current.PaySettings.Alipay_PartnerID;
            string signtype = Pay.AliPay.SignType;
            string subject = string.Format(AllSettings.Current.PaySettings.ProductName, payValue.ToString(),payTypeName);
            string body = subject;
            string paymenttype = Pay.AliPay.PaymentType;
            string orderamount = orderAmount.ToString();
            string showurl = AllSettings.Current.PaySettings.Alipay_ShowUrl;
            string sellerEmail = AllSettings.Current.PaySettings.Alipay_SellerEmail;
            string key = AllSettings.Current.PaySettings.Alipay_Key;
            string returnurl = AllSettings.Current.PaySettings.Alipay_ReturnUrl;
            string notifyurl = AllSettings.Current.PaySettings.Alipay_NotifyUrl;
            string inputcharset = Pay.AliPay.InputCharset;


            string alipayUrl = Pay.AliPay.CreatUrl(
                gateway,
                service,
                partner,
                signtype,
                orderNo,
                subject,
                body,
                paymenttype,
                orderamount,
                showurl,
                sellerEmail,
                key,
                returnurl,
                inputcharset,
                notifyurl
                );
            //充值记录信息记入数据库.
            bool success = PayDao.Instance.CreateUserPayItem(userID, orderNo, orderAmount, payment, payType, payValue, submitIp, subject);
            if (success)
            {
                return alipayUrl;
            }
            else
            {
                return null;
            }

        }

        //支付宝充值返回
        public bool AlipayReturn(string notifyID, string prestr, string buyerEmail, string orderNo, string transactionNo, string sign, string tradeStatus, string payIp, DateTime payDate)
        {
            string charset = Pay.AliPay.InputCharset;
            string partnerID = AllSettings.Current.PaySettings.Alipay_PartnerID;
            string getway = Pay.AliPay.Getway;
            string alipayUrl = getway + "service=notify_verify" + "&partner=" + partnerID + "&notify_id=" + notifyID;

            //获取支付宝ATN返回结果，true是正确的订单信息，false 是无效的
            string responseTxt = Pay.AliPay.Get_Http(alipayUrl, 120000);
            string mySign = Pay.AliPay.GetMD5(prestr, charset);
            //验证支付发过来的消息，签名是否正确
            if (mySign == sign && responseTxt == "true" && tradeStatus == "TRADE_FINISHED")
            {
                int userID;
                bool success = PayDao.Instance.UpdateUserPayItem(buyerEmail, orderNo, transactionNo, payIp,payDate, out userID);
                if (success)
                    UserBO.Instance.TryResetUserPointsCache(userID);
                return true;
            }
            else
            {
                ThrowError(new CustomError("result", "签名字符串验证失败"));
                return false;
            }
        }

        //财付通和快钱的充值接口
        public bool TenpayReturn(string orderNo, string transactionNo, string payIp,DateTime payDate)
        {
            int userID;
            bool success = PayDao.Instance.UpdateUserPayItem("", orderNo, transactionNo, payIp, payDate,out userID);
            if (success)
                UserBO.Instance.TryResetUserPointsCache(userID);
            return success;
        }

        //财付通充值返回
        public bool _99BillReturn(string merchantAcctId,string version,string language, string signType,string  payType,string bankId,string  orderNo,string  orderTime,
               string orderAmount, string dealId, string bankDealId, string dealTime, string payAmount, string fee, string ext1, string ext2, string payResult, string errCode, string signMsg, string payIp, DateTime payDate)
        {
            string key = AllSettings.Current.PaySettings._99Bill_Key;
            //生成加密串。必须保持如下顺序。
            //string merchantSignMsgVal = "";
            StringBuilder merchantSignMsgValBuilder = new StringBuilder();
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "merchantAcctId", merchantAcctId);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "version", version);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "language", language);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "signType", signType);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "payType", payType);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "bankId", bankId);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "orderId", orderNo);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "orderTime", orderTime);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "orderAmount", orderAmount);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "dealId", dealId);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "bankDealId", bankDealId);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "dealTime", dealTime);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "payAmount", payAmount);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "fee", fee);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "ext1", ext1);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "ext2", ext2);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "payResult", payResult);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "errCode", errCode);
            Pay._99Bill.appendParam(merchantSignMsgValBuilder, "key", key);

            string merchantSignMsgVal = merchantSignMsgValBuilder.ToString();

            string merchantSignMsg = Pay._99Bill.GetMD5(merchantSignMsgVal, "utf-8");

            //商家进行数据处理，并跳转会商家显示支付结果的页面
            ///首先进行签名字符串验证
            if (StringUtil.EqualsIgnoreCase(signMsg, merchantSignMsg))
            {
                switch (payResult)
                {
                    case "10":
                        /*  
                         ' 商户网站逻辑处理，比方更新订单支付状态为成功
                        ' 特别注意：只有signMsg.ToUpper() == merchantSignMsg.ToUpper()，且payResult=10，才表示支付成功！同时将订单金额与提交订单前的订单金额进行对比校验。
                        */
                        //报告给快钱处理结果，并提供将要重定向的地址。
                        int userID;
                        PayDao.Instance.UpdateUserPayItem("", orderNo, dealId, payIp, payDate, out userID);
                        if (userID > 0)
                            UserBO.Instance.TryResetUserPointsCache(userID);
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }
    }
}