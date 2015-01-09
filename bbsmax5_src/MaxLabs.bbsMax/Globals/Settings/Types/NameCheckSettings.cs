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
    public class NameCheckSettings : SettingBase
    {
        public NameCheckSettings()
        {
            EnableRealnameCheck = false;
            CanChinese = true;
            CanEnglish = true;
            this.NeedIDCardFile = true;
            this.MaxIDCardFileSize = 2048 * 1024;
        }

        /// <summary>
        /// 开启实名认证
        /// </summary>
        [SettingItem]
        public bool EnableRealnameCheck { get; set; }

        /// <summary>
        /// 用户名允许中文
        /// </summary>
        [SettingItem]
        public bool CanChinese { get; set; }

        /// <summary>
        /// 用户名允许英文
        /// </summary>
        [SettingItem]
        public bool CanEnglish { get; set; }

        /// <summary>
        /// 是否要上传身份证扫描件
        /// </summary>
        [SettingItem]
        public bool NeedIDCardFile { get; set; }

        /// <summary>
        /// 身份证文件最大值限制
        /// </summary>
        [SettingItem]
        public int MaxIDCardFileSize { get; set; }
    }
}