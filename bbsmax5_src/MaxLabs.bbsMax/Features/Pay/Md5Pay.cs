//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;

namespace MaxLabs.bbsMax.Pay
{
       /// <summary>
    /// 完成功能如下
    /// 1:支付请求
    /// 2:支付结果处理。
    /// 3:查询订单请求.
    /// 4:查询订单结果处理.
    /// </summary>
    public class Md5Pay
    {

        /// <summary>
        /// 财付通支付接口
        /// </summary>
        private const string Getway = "https://www.tenpay.com/cgi-bin/v1.0/pay_gate.cgi";

        /// <summary>
        /// 财付通查询请求URL
        /// </summary>
        public const string QueryGateurl = "http://portal.tenpay.com/cfbiportal/cgi-bin/cfbiqueryorder.cgi";

        /// <summary>
        /// 业务代码, 财付通支付支付接口填1 
        /// </summary>
        private const int Cmdno = 1;

        /// <summary>
        /// 查询命令.2
        /// </summary>
        private const int QueryCmdno = 2;

        /// <summary>
        /// 费用类型,现在暂只支持 1:人民币
        /// </summary>
        private const int Fee_type = 1;

        /// <summary>
        /// 取时间戳生成随即数,替换交易单号中的后10位流水号
        /// 财付通的交易单号中不允许出现非数字的字符
        /// </summary>
        /// <returns></returns>
        public static UInt32 UnixStamp()
        {
            TimeSpan ts = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToUInt32(ts.TotalSeconds);
        }
        private static int payResult;
        /// <summary>
        ///  支付結果
        /// </summary>
        public const int PAYOK = 0;
        public const int PAYSPERROR = 1;
        public const int PAYMD5ERROR = 2;
        public const int PAYERROR = 3;
        /// <summary>
        /// 支付结果 
        /// 0:支付成功.
        /// 1:商户号错.
        /// 2:签名错误.
        /// 3:支付失败.
        /// </summary>
        public static int PayResult
        {
            get { return payResult; }
            set { payResult = value; }
        }

        /// <summary>
        /// 支付结果说明字段
        /// </summary>
        public string PayResultStr
        {
            get
            {
                switch (payResult)
                {
                    case PAYOK:
                        return "支付成功";
                    case PAYSPERROR:
                        return "商户号错";
                    case PAYMD5ERROR:
                        return "签名错误";
                    case PAYERROR:
                        return "支付失败";
                    default:
                        return "未知类型(" + payResult + ")";
                }
            }
        }

        /// <summary>
        /// 对字符串进行URL编码
        /// </summary>
        /// <param name="instr">待编码的字符串</param>
        /// <returns>编码结果</returns>
        private static string UrlEncode(string instr)
        {
            if (instr == null || instr.Trim() == "")
                return "";
            else
            {
                return instr.Replace("%", "%25").Replace("=", "%3d").Replace("&", "%26").
                    Replace("\"", "%22").Replace("?", "%3f").Replace("'", "%27").Replace(" ", "%20");
            }
        }

        /// <summary>
        /// 对字符串进行URL解码
        /// </summary>
        /// <param name="instr">待解码的字符串</param>
        /// <returns>解码结果</returns>
        private static string UrlDecode(string instr)
        {
            if (instr == null || instr.Trim() == "")
                return "";
            else
            {
                return instr.Replace("%3d", "=").Replace("%26", "&").Replace("%22", "\"").Replace("%3f", "?")
                    .Replace("%27", "'").Replace("%20", " ").Replace("%25", "%");
            }
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        private static string GetMD5(string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);

            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

        /// <summary>
        /// 获取支付签名
        /// </summary>
        /// <returns>根据参数得到签名</returns>
        private static string GetPaySign(string date, string bargainorID, string key, string transactionNo, string orderNo, long orderAmount, string returnUrl, string productDesc, string spbillCreateIp)
        {
            string sign_text = "cmdno=" + Cmdno + "&date=" + date + "&bargainor_id=" + bargainorID + "&transaction_id=" + transactionNo + "&sp_billno=" + orderNo + "&total_fee="
                + orderAmount + "&fee_type=" + Fee_type + "&return_url=" + returnUrl + "&attach=" + productDesc + "&spbill_create_ip=" + spbillCreateIp + "&key=" + key;
            return GetMD5(sign_text);
        }

        /// <summary>
        /// 获取支付页面URL
        /// </summary>
        /// <param name="url">如果函数返回真,是支付URL,如果函数返回假,是错误信息</param>
        /// <returns>函数执行是否成功</returns>
        public static string GetPayUrl(string date, string bargainorID, string key, string orderNo, long totalFee, string transactionNo, string productName, string productDesc, string returnUrl, string spbillCreateIp)
        {
            string sign = GetPaySign(date, bargainorID, key, transactionNo, orderNo, totalFee, returnUrl, productDesc, spbillCreateIp);

            string url = Getway + "?cmdno=" + Cmdno + "&date=" + date + "&bank_type=0&desc=" + HttpUtility.UrlEncode(productName, Encoding.GetEncoding("gb2312")) + "&purchaser_id=&bargainor_id="
                  + bargainorID + "&transaction_id=" + transactionNo + "&sp_billno=" + orderNo + "&total_fee=" + totalFee
                  + "&fee_type=" + Fee_type + "&return_url=" + returnUrl + "&attach=" + productDesc + "&spbill_create_ip=" + spbillCreateIp + "&sign=" + sign;
            return url;
        }

        /// <summary>
        /// 获取支付结果签名
        /// </summary>
        /// <returns>根据参数得到签名</returns>
        private static string GetPayResultSign(string date, string transactionNo, string orderNo, long totalFee, int feeType, string productDesc, string key)
        {
            string sign_text = "cmdno=" + Cmdno + "&pay_result=" + payResult + "&date=" + date + "&transaction_id=" + transactionNo
                + "&sp_billno=" + orderNo + "&total_fee=" + totalFee + "&fee_type=" + feeType + "&attach=" + productDesc + "&key=" + key;
            return GetMD5(sign_text);
        }

        /// <summary>
        /// 从支付结果页面的URL请求参数中获取结果信息
        /// </summary>
        /// <param name="querystring">支付结果页面的URL请求参数</param>
        /// <param name="errmsg">函数执行不成功的话,返回错误信息</param>
        /// <returns>函数执行是否成功</returns>
        public static bool GetPayValueFromUrl(NameValueCollection queryString, string bargainorID, string key,out string errmsg)
        {
            //结果URL参数样例如下
            /*
            ?cmdno=1&pay_result=0&pay_info=OK&date=20070423&bargainor_id=1201143001&transaction_id=1201143001200704230000000013
            &sp_billno=13&total_fee=1&fee_type=1&attach=%D5%E2%CA%C7%D2%BB%B8%F6%B2%E2%CA%D4%BD%BB%D2%D7%B5%A5				
            &sign=ADD7475F2CAFA793A3FB35051869E301
            */

            #region 进行参数校验

            if (queryString == null || queryString.Count == 0)
            {
                errmsg = "参数为空";
                return false;
            }

            if (queryString["cmdno"] == null || queryString["cmdno"].ToString().Trim() != Cmdno.ToString())
            {
                errmsg = "没有cmdno参数或cmdno参数不正确";
                return false;
            }

            if (queryString["pay_result"] == null)
            {
                errmsg = "没有pay_result参数";
                return false;
            }

            if (queryString["date"] == null)
            {
                errmsg = "没有date参数";
                return false;
            }

            if (queryString["pay_info"] == null)
            {
                errmsg = "没有pay_info参数";
                return false;
            }

            if (queryString["bargainor_id"] == null)
            {
                errmsg = "没有bargainor_id参数";
                return false;
            }

            if (queryString["transaction_id"] == null)
            {
                errmsg = "没有transaction_id参数";
                return false;
            }

            if (queryString["sp_billno"] == null)
            {
                errmsg = "没有sp_billno参数";
                return false;
            }

            if (queryString["total_fee"] == null)
            {
                errmsg = "没有total_fee参数";
                return false;
            }

            if (queryString["fee_type"] == null)
            {
                errmsg = "没有fee_type参数";
                return false;
            }

            if (queryString["attach"] == null)
            {
                errmsg = "没有attach参数";
                return false;
            }

            if (queryString["sign"] == null)
            {
                errmsg = "没有sign参数";
                return false;
            }

            #endregion

            errmsg = "";

            try
            {
                payResult = Int32.Parse(queryString["pay_result"].Trim());

                string payerrmsg = UrlDecode(queryString["pay_info"].Trim());
                string date = queryString["date"];
                string transactionNo = queryString["transaction_id"];
                string orderNo = queryString["sp_billno"];
                long totalFee = long.Parse(queryString["total_fee"]);
                int feetype = Int32.Parse(queryString["fee_type"]);
                string attach = queryString["attach"];
                if (queryString["bargainor_id"] != bargainorID)
                {
                    payResult = PAYSPERROR;
                    return true;
                }

                string strsign = queryString["sign"];
                string sign = GetPayResultSign(date, transactionNo, orderNo, totalFee, feetype, attach, key);

                if (sign != strsign)
                {
                    payResult = PAYMD5ERROR;
                }

                return true;
            }
            catch (Exception err)
            {
                errmsg = "解析参数出错:" + err.Message;
                return false;
            }
        }
    }
}