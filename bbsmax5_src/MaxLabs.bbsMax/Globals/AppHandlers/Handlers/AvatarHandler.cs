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
using MaxLabs.bbsMax.Entities;
using System.Web;
using MaxLabs.bbsMax.Settings;
using System.Net;
using System.Threading;


namespace MaxLabs.bbsMax.AppHandlers
{
    public class AvatarHandler : IAppHandler
    {

        #region IAppHandler Members

        public IAppHandler CreateInstance()
        {
            return new AvatarHandler();
        }

        public string Name
        {
            get { return "avatar"; }
        }



        public void ProcessRequest(System.Web.HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            string cookie = Request.QueryString["authcookie"];

            if (string.IsNullOrEmpty(cookie))
            {
                context.Response.End();
                return;
            }

            int userID = UserBO.Instance.GetUserID(cookie, true);
            AuthUser My = UserBO.Instance.GetAuthUser(userID);

            if (My ==null || My == User.Guest)
            {
                Response.Write("error");
                Response.End();
            }

            if (string.IsNullOrEmpty(Request.QueryString["file"]))
            {
                string phyFile;
                Response.Write(UserBO.Instance.SaveTempAvatar(My, Request, out phyFile));
                Response.End();
            }
            else
            {
                if (Globals.PassportClient.EnablePassport)
                {
PassportClientConfig settings = Globals.PassportClient;
                    string passportAvatarUrl = string.Concat(settings.AvatarGeneratorUrl, "?kachashow=true&file=aaa.jpg");
                    KeyValuePair<string, string>[] heads = new KeyValuePair<string, string>[1];
                    heads[0] = new KeyValuePair<string, string>("size", Request.Headers["size"]);
                    HttpWebResponse response = NetUtil.PostToRemoteUrl(Request, passportAvatarUrl, heads);
                    Thread.Sleep(300);
                }
                else
                {
                    UserBO.Instance.SaveAvatar(My, My.UserID, Request);
                }
            }
        }

        #endregion
    }
}