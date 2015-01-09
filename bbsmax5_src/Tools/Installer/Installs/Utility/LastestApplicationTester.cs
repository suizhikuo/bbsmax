//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using Max.Installs;
using Max.Installs.VersionService;

namespace Max.Installs
{
    public class LastestApplicationTester
    {
        private LastestApplicationTester()
        {
        }

        public static string CheckVersion()
        {
            Service service = new Service();
            service.Timeout = 3000;
            string msg = string.Empty;
            try
            {
                string url;

                if (HttpContext.Current.Request.Url.Port == 80)
                    url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.FilePath;
                else
                    url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port + HttpContext.Current.Request.FilePath;

                url = url.Substring(0, url.Length - VirtualPathUtility.GetFileName(HttpContext.Current.Request.FilePath).Length);

#if SQLSERVER
                msg = service.CheckVersion(Settings.Version, "SQLServer", url);
#endif
#if SQLITE
                msg = service.CheckVersion(Settings.Version, "SQLite", url);
#endif
            }
            catch { }
            return msg;
        }
    }
}