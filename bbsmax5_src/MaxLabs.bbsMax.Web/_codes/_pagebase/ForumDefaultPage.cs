//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using MaxLabs.WebEngine;
using MaxLabs.WebEngine.Template;

namespace MaxLabs.bbsMax.Web
{
    public partial class ForumDefaultPage : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string absoluteUrl = Request.Url.AbsolutePath;

            int startIndex = Globals.AppRoot.Length + 1;
            int codenameLength;

            if (StringUtil.EndsWithIgnoreCase(absoluteUrl, "/default.aspx"))
                codenameLength = absoluteUrl.Length - startIndex - 13;

            else if (StringUtil.EndsWithIgnoreCase(absoluteUrl, "/index.aspx"))
                codenameLength = absoluteUrl.Length - startIndex - 11;

            else
                return;

            string codename = absoluteUrl.Substring(startIndex, codenameLength);

            string path = TemplateManager.ParseTemplate("~/max-templates/default/forums/list.aspx");

            string rawurl = Request.RawUrl;

            int queryIndex = rawurl.IndexOf('?');

            if (queryIndex != -1)
            {
                string query = rawurl.Substring(queryIndex + 1);

                if (string.IsNullOrEmpty(query) == false)
                {
                    Server.Transfer(string.Concat(path, "?codename=", codename, "&", query));
                    return;
                }
            }

            Server.Transfer(string.Concat(path, "?codename=", codename));
        }
    }
}