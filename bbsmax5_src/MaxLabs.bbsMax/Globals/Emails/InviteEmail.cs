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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Email
{
    public class InviteEmail : EmailBase
    {
        public InviteEmail(string toEmail, string message, string serial, string username, int userID)
            : base(
            toEmail,
            AllSettings.Current.InvitationSettings.InviteEmailTitle,
            AllSettings.Current.InvitationSettings.InviteEmailContent
            )
        {
            this.Render.RegisterVariable("serial", serial);
            this.Render.RegisterVariable("username", username);
            this.Render.RegisterVariable("message", message);
            this.Render.RegisterVariable("space", string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", UrlUtil.JoinUrl(Globals.SiteRoot, BbsRouter.GetUrl("space/" + userID))));
            this.Render.RegisterVariable("url", string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", UrlUtil.JoinUrl(Globals.SiteRoot, BbsRouter.GetUrl("register/"+serial)))); 
        }

        public InviteEmail(string toEmail, Guid serial, string username, int userID)
            : base(
            toEmail,
            AllSettings.Current.InvitationSettings.InviteSerialEmailTitle,
            AllSettings.Current.InvitationSettings.InviteSerialEmailContent
            )
        {
            this.Render.RegisterVariable("serial", serial.ToString());
            this.Render.RegisterVariable("username", username);
            this.Render.RegisterVariable("space", string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", UrlUtil.JoinUrl(Globals.SiteRoot, BbsRouter.GetUrl("space/" + userID))));
            this.Render.RegisterVariable("url", string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", UrlUtil.JoinUrl(Globals.SiteRoot, BbsRouter.GetUrl("register/" + serial)))); 
        }
    }
}