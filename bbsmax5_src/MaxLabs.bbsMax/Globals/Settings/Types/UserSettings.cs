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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public class UserSettings:SettingBase
    {
        public UserSettings()
        {
            
            this.AllowTableTag          = new Exceptable<bool>(false);
            this.AllowVideoTag          = new Exceptable<bool>(false);
            this.AllowAudioTag          = new Exceptable<bool>(false);
            this.AllowFlashTag          = new Exceptable<bool>(false);

            this.AllowUrlTag            = new Exceptable<bool>(true);
            this.AllowImageTag          = new Exceptable<bool>(true);
            this.AllowUserEmoticon      = new Exceptable<bool>(true);
            this.AllowDefaultEmoticon   = new Exceptable<bool>(true);

            this.SignatureHeight        = new Exceptable<int>(100);
            this.SignatureLength        = new Exceptable<int>(200);
            this.SignatureFormat        = new Exceptable<SignatureFormat>(MaxLabs.bbsMax.Enums.SignatureFormat.Ubb);

        }


        /// <summary>
        /// 使用系统表情
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowDefaultEmoticon { get; set; }

        /// <summary>
        /// 签名区的高度
        /// </summary>
        [SettingItem]
        public Exceptable<int> SignatureHeight { get; set; }

        /// <summary>
        /// 签名的长度
        /// </summary>
        [SettingItem]
        public Exceptable<int> SignatureLength { get; set; }

        /// <summary>
        /// 签名格式
        /// </summary>
        [SettingItem]
        public Exceptable<SignatureFormat> SignatureFormat { get; set; }

        /// <summary>
        /// 使用表情
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowUserEmoticon { get; set; }

        /// <summary>
        /// 允许使用img标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowImageTag { get; set; }


        /// <summary>
        /// 允许使用Flash标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowFlashTag { get; set; }



        /// <summary>
        /// 允许使用Audio标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowAudioTag { get; set; }

        /// <summary>
        /// 允许使用Video标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowVideoTag { get; set; }


        /// <summary>
        /// 允许使用Table标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowTableTag { get; set; }


        /// <summary>
        /// 允许使用Url标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowUrlTag { get; set; }
    }
}