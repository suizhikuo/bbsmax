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
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Email
{
    public class InviteSerialEmail : EmailBase
    {
        public InviteSerialEmail(string toEmail, string serial, string username)
            : base(
            toEmail,
            AllSettings.Current.InvitationSettings.InviteSerialEmailTitle,
            AllSettings.Current.InvitationSettings.InviteSerialEmailContent
            )
        {
            this.Render.RegisterVariable("serial", serial);
            this.Render.RegisterVariable("username", username);
			this.Render.RegisterVariable("url", Globals.SiteRoot + BbsRouter.GetUrl("register", "invite=" + serial)); // Globals.AbsoluteWebRoot + "register.aspx?invite=" + serial);
        }
    }
}