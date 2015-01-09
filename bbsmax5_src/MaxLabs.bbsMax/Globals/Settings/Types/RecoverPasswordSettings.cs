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
    public class RecoverPasswordSettings : SettingBase
    {
        public RecoverPasswordSettings()
        {
            Enable = true;
            EmailTitle = "找回 {username} 在 {sitename} 的密码";
            EmailContent = @"
{username}&nbsp;你好，<br/><br/>
请点击下面的地址或将下面的地址输入到浏览器地址栏完成取回密码操作。
<font color=""#002500"">(注意：如果您没有进行过取回密码操作，请不要点击此链接)</font><br/><br/><br/>
{url}<br/>
(本地址在24小时内有效)<br/><br/>
==========================================================<br/>
{site}<br/>
{sitename}<br/>
";
        }

        /// <summary>
        /// 开启找回密码功能
        /// </summary>
        [SettingItem]
        public bool Enable { get; set; }

        [SettingItem]
        public string EmailTitle { get; set; }

        [SettingItem]
        public string EmailContent { get; set; }
    }
}