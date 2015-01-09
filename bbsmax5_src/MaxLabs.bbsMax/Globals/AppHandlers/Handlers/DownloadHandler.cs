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
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Settings;
using System.IO;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class DownloadHandler : IAppHandler
    {
        #region IAppHandler 成员

        public IAppHandler CreateInstance()
        {
            return new DownloadHandler();
        }

        public string Name
        {
            get { return "down"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            if (AllSettings.Current.DownloadSettings.UseDownloadFilter && UserBO.Instance.GetCurrentUserID() <= 0)
            {
                if (context.Request.UrlReferrer == null)
                {
                    BlockDownload(context);
                    return;
                }

                string host = context.Request.UrlReferrer.Host;

                if (string.IsNullOrEmpty(host))
                {
                    BlockDownload(context);
                    return;
                }

                if (context.Request.Url.Host == host)
                {
                    NormalDownload(context);
                    return;
                }

                if (AllSettings.Current.DownloadSettings.AllowReferrerHost.IsMach(host) == false)
                {
                    BlockDownload(context);
                    return;
                }
                else
                {
                    NormalDownload(context);
                    return;
                }
            }
            else
            {
                NormalDownload(context);
                return;
            }
        }

        private void NormalDownload(System.Web.HttpContext context)
        {
            string action = context.Request.QueryString["action"];

            if (string.IsNullOrEmpty(action) == false)
            {
                FileManager.DownloadFile(action, context);
            }
        }

        private void BlockDownload(System.Web.HttpContext context)
        {
            string path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Root), "max-assets/images/protector_image.gif");

            if (File.Exists(path))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = "image/gif";
                context.Response.AddHeader("Content-Disposition", "inline;filename=protector_image.gif");
                context.Response.WriteFile(path);
                context.Response.End();
            }
        }

        #endregion
    }
}