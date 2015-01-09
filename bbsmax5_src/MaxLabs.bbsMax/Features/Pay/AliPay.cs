//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Pay
{
    public class AliPay
    {       
        /// <summary>
        /// 支付宝接口
        /// </summary>
        public const string Getway = "https://www.alipay.com/cooperate/gateway.do?";

        /// <summary>
        /// 接口名称
        /// </summary>
        public const string Service = "create_direct_pay_by_user";

        //签名类型.固定值
        //1代表MD5签名
        //当前版本固定为1
        public const string SignType = "MD5";

        //支付类型 1：商品购买 2：服务购买3：网络拍卖4：捐赠	
        public const string PaymentType = "1";

        /// <summary>
        /// 字符编码
        /// </summary>
        public const string InputCharset = "utf-8";

        public static string GetMD5(string s, string _input_charset)
        {
            /// <summary>
            /// 与ASP兼容的MD5加密算法
            /// </summary>

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        public static string[] BubbleSort(string[] array)
        {
            /// <summary>
            /// 冒泡排序法
            /// </summary>

            int i, j; //交换标志 
            string temp;

            bool exchange;

            for (i = 0; i < array.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假

                for (j = array.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(array[j + 1], array[j]) < 0) //交换条件
                    {
                        temp = array[j + 1];
                        array[j + 1] = array[j];
                        array[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return array;
        }

        public static string CreatUrl(
            string gateway,
            string service,
            string partner,
            string signtype,
            string orderno,
            string subject,
            string body,
            string paymenttype,
            string totalfee,
            string showurl,
            string selleremail,
            string key,
            string returnurl,
            string inputcharset,
            string notifyurl
            )
        {

            /// <summary>
            /// created by sunzhizhi 2006.5.21,sunzhizhi@msn.com。
            /// </summary>
            int i;

            //构造数组；
            string[] Oristr ={ 
                "service="+service, 
                "partner=" + partner, 
                "subject=" + subject, 
                "body=" + body, 
                "out_trade_no=" + orderno, 
                "total_fee=" + totalfee, 
                "show_url=" + showurl,  
                "payment_type=" + paymenttype, 
                "seller_email=" + selleremail, 
                "notify_url=" + notifyurl,
                "_input_charset="+inputcharset,          
                "return_url=" + returnurl
                };

            //进行排序；
            string[] Sortedstr = BubbleSort(Oristr);


            //构造待md5摘要字符串 ；

            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);

                }
                else
                {

                    prestr.Append(Sortedstr[i] + "&");
                }
            }

            prestr.Append(key);

            //生成Md5摘要；
            string sign = GetMD5(prestr.ToString(), inputcharset);

            //构造支付Url；
            char[] delimiterChars = { '=' };
            StringBuilder parameter = new StringBuilder();
            parameter.Append(gateway);
            for (i = 0; i < Sortedstr.Length; i++)
            {

                parameter.Append(Sortedstr[i].Split(delimiterChars)[0] + "=" + HttpUtility.UrlEncode(Sortedstr[i].Split(delimiterChars)[1]) + "&");
            }

            parameter.Append("sign=" + sign + "&sign_type=" + signtype);


            //返回支付Url；
            return parameter.ToString();
        }

        //获取远程服务器ATN结果
        public static String Get_Http(String a_strUrl, int timeout)
        {
            string strResult;
            try
            {

                HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(a_strUrl);
                myReq.Timeout = timeout;
                HttpWebResponse HttpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {

                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }
    }
}