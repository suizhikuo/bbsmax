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

namespace MaxLabs.bbsMax.Pay
{
    public class _99Bill
    {
        /// <summary>
        /// 快钱接口
        /// </summary>
        public const string _99BillUrl="https://www.99bill.com/gateway/recvMerchantInfoAction.htm";

        //字符集.固定选择值。可为空。
        //只能选择1、2、3.
        //1代表UTF-8; 2代表GBK; 3代表gb2312
        //默认值为1
        //如果在web.config文件中设置了编码方式，例如<globalization requestEncoding="utf-8" responseEncoding="utf-8"/>（如未设则默认为utf-8），
        //那么，inputCharset的取值应与已设置的编码方式相一致
        public const string InputCharset = "1";

        //网关版本.固定值
        //快钱会根据版本号来调用对应的接口处理程序。
        //本代码版本号固定为v2.0
        public const string version = "v2.0";

        //语言种类.固定选择值。
        //只能选择1、2、3
        //1代表中文；2代表英文
        //默认值为1
        public const string language = "1";

        //签名类型.固定值
        //1代表MD5签名
        //当前版本固定为1
        public const string signType = "1";

        //支付人联系方式类型.固定选择值
        //只能选择1
        //1代表Email
        public const string payerContactType = "1";

        //商品数量
        //可为空，非空时必须为数字
        public const string productNum = "1";

        //支付方式.固定选择值
        //只能选择00、10、11、12、13、14
        //00：组合支付（网关支付页面显示快钱支持的各种支付方式，推荐使用）10：银行卡支付（网关支付页面只显示银行卡支付）.11：电话银行支付（网关支付页面只显示电话支付）.12：快钱账户支付（网关支付页面只显示快钱账户支付）.13：线下支付（网关支付页面只显示线下支付方式）
        public const string payType = "00";

        //同一订单禁止重复提交标志
        //固定选择值： 1、0
        //1代表同一订单号只允许提交1次；0表示同一订单号在没有支付成功的前提下可重复提交多次。默认为0建议实物购物车结算类商户采用0；虚拟产品类商户采用1
        public const string redoFlag = "0";

        //功能函数。将变量值不为空的参数组成字符串
        public static void appendParam(StringBuilder builder, String paramId, String paramValue)
        {
            if (builder.Length > 0)
                builder.Append("&");

            if (paramValue != "")
            {


                builder.Append(paramId).Append("=").Append(paramValue);
            }
        }

        //功能函数。将变量值不为空的参数组成字符串。结束

        //功能函数。将字符串进行编码格式转换，并进行MD5加密，然后返回。开始
        public static  string GetMD5(string dataStr, string codeType)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(codeType).GetBytes(dataStr));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        //功能函数。将字符串进行编码格式转换，并进行MD5加密，然后返回。结束
    }
}