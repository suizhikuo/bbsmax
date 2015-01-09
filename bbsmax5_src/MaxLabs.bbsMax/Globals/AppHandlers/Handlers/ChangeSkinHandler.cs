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
using System.IO;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine.Template;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.AppHandlers
{
	public class ChangeSkinHandler : IAppHandler
	{
		public IAppHandler CreateInstance()
		{
			return new ChangeSkinHandler();
		}

		public string Name
		{
			get { return "ChangeSkin"; }
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
            HttpRequest Request = HttpContext.Current.Request;

            string skinID = Request.QueryString["skin"];

            if (skinID != null)
            {
                Skin skin = TemplateManager.GetSkin(skinID);

                if (skin != null && skin.Enabled)
                {
                    UserBO.Instance.UpdateSkinID(User.Current, skinID);
                }
            }

            string returnUrl = Request.QueryString["u"];
            
            if (string.IsNullOrEmpty(returnUrl))
            {
                if (Request.UrlReferrer != null)
                {
                    returnUrl = Request.UrlReferrer.OriginalString;
                }
            }

            if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith(Globals.FullAppRoot + "/"))
            {
                returnUrl = GetFilteredUrl(returnUrl);
            }
            else
            {
                returnUrl = BbsRouter.GetIndexUrl();
            }

            HttpContext.Current.Response.Redirect(returnUrl);
        }

        private string GetFilteredUrl(string url)
        {
            string urlInfo = BbsRouter.GetUrlInfo(url);

            bool gotoIndexPage;

            switch (urlInfo)
            {
                case "login":
                case "register":
                case "recoverpassword":
                case "info":
                    gotoIndexPage = true;
                    break;

                default:
                    if (urlInfo.StartsWith("logout/", StringComparison.OrdinalIgnoreCase)
                        ||
                        urlInfo.StartsWith("max-dialogs/", StringComparison.OrdinalIgnoreCase)
                        ||
                        urlInfo.StartsWith("register/", StringComparison.OrdinalIgnoreCase)
                        )
                        gotoIndexPage = true;
                    else
                        gotoIndexPage = false;
                    break;
            }

            if (gotoIndexPage)
                return BbsRouter.GetIndexUrl();
            else
                return url;
        }
	}
}