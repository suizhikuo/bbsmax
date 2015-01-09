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

namespace MaxLabs.bbsMax.Settings
{
    public class PaySettings : SettingBase
    {
        public PaySettings()
        {
            this.Alipay_SellerEmail = "";
            this.Alipay_PartnerID = "";
            this.Alipay_Key = "";
            this.Alipay_ShowUrl = "";

            this.Tenpay_BargainorID="";
            this.Tenpay_Key = ""; 
            this.Tenpay_Attach = "";

            this._99Bill_MerchantAcctID = "";
            this._99Bill_Key = "";


            this.ProductName = "充值{0}个{1}";

            EnablePointRecharge = false;
            EnableAlipay = true;
            EnableTenpay = true;
            Enable99Bill = true;
        }       


        /// <summary>
        /// 支付宝卖家账号
        /// </summary>
        [SettingItem]
        public string Alipay_SellerEmail { get; set; }

        /// <summary>
        /// 合作伙伴在支付宝的用户ID
        /// </summary>
        [SettingItem]
        public string Alipay_PartnerID { get; set; }

        /// <summary>
        ///支付宝合作验证码（key）
        /// </summary>
        [SettingItem]
        public string Alipay_Key { get; set; }

        /// <summary>
        ///展示地址
        /// </summary>
        [SettingItem]
        public string Alipay_ShowUrl { get; set; }
        /// <summary>
        /// 结果返回URL，仅适用亍立即返回处理结果的接口。支付宝处理完请求后，立即将处理结果返回给这个URL。
        /// </summary>
        /// 

        public string Alipay_ReturnUrl 
        { 
            get 
            { 
                return string.Format("{0}/max-api/alipay/alipay_return.aspx", Globals.FullAppRoot); 
            } 
        }

        /// <summary>
        /// 针对该交易的交易状态同步通知接收URL。
        /// </summary>
        public string Alipay_NotifyUrl 
        { 
            get 
            { 
                return string.Format("{0}/max-api/alipay/alipay_notify.aspx", Globals.FullAppRoot); 
            } 
        }

        /// <summary>
        /// 财付通商户号（替换为自已的商户号）
        /// </summary>
        [SettingItem]
        public string Tenpay_BargainorID { get; set; }

        /// <summary>
        /// 财付通商户KEY（替换为自已的KEY）
        /// </summary>
        [SettingItem]
        public string Tenpay_Key { get; set; }

        /// <summary>
        /// 财付通商品标识
        /// </summary>
        [SettingItem]
        public string Tenpay_Attach { get; set; }

        /// <summary>
        /// 财付通支付结果回跳页面
        /// </summary>
        public string Tenpay_ReturnUrl 
        {
            get 
            {
                return string.Format("{0}/max-api/tenpay/tenpay_return.aspx", Globals.FullAppRoot); 
            }
        }

        /// <summary>
        /// 快钱人民币网关账户号
        /// </summary>
        [SettingItem]
        public string _99Bill_MerchantAcctID { get; set; }

        /// <summary>
        /// 快钱人民币网关密钥
        /// </summary>
        [SettingItem]
        public string _99Bill_Key { get; set; }

        /// <summary>
        /// 快钱服务器接受支付结果的后台地址.与[pageUrl]不能同时为空。必须是绝对地址。
        /// </summary>
        public string _99Bill_ReturnUrl
        {
            get
            {
                return string.Format("{0}/max-api/99bill/99bill_return.aspx", Globals.FullAppRoot);
            }
        }
        /// <summary>
        /// 商品名称
        /// </summary>
        [SettingItem]
        public string ProductName { get; set; }

        /// <summary>
        /// 结果地址
        /// </summary>
        public string PayResultUrl 
        {
            get
            {
                return UrlUtil.JoinUrl(Globals.SiteRoot,  BbsRouter.GetUrl("my/payresult"));
            }
        }

        /// <summary>
        /// 是否开启支付功能
        /// </summary>
        [SettingItem]
        public bool EnablePointRecharge { get; set; }

        //是否开启支付宝
        [SettingItem]
        public bool EnableAlipay { get; set; }

        //是否开启财付通
        [SettingItem]
        public bool EnableTenpay { get; set; }

        //是否开启快钱
        [SettingItem]
        public bool Enable99Bill { get; set; }

    }
}