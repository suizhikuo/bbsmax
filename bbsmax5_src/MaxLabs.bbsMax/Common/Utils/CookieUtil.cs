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
using System.Text.RegularExpressions;
using System.Web;
using MaxLabs.bbsMax.Settings;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax
{
    public class CookieUtil
    {
        #region 读取当前域名的主域

        /// <summary>
        /// 当前的cookie域
        /// </summary>
        public static string CookieDomain
        {
            get
            {
                string domain = HttpContext.Current.Request.Url.DnsSafeHost;
                
                string mainDoamin;
                string cookieDomain;

                UrlUtil.BuildMainDomain(domain, true, out mainDoamin, out cookieDomain);

                return cookieDomain;
            }
        }

        #endregion

        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="cookieKey"></param>
        public static void Remove(string cookieKey)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieKey];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);


                //if (AllSettings.Current.SiteSettings.EnableCookieDomain && !string.IsNullOrEmpty(CookieDomain))
                    //cookie.Domain = CookieDomain;

                if (!string.IsNullOrEmpty(CookieDomain))
                    cookie.Domain = CookieDomain;
                cookie.Path = "/";

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 修改Cookie的值 如果不存在键则创建
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expires">过期时间</param>
        public static void Set(string cookieKey, string cookieValue, DateTime? expires)
        {
            HttpCookie cookie = new HttpCookie(cookieKey, cookieValue);

            //if (AllSettings.Current.SiteSettings.EnableCookieDomain && !string.IsNullOrEmpty(CookieDomain))
                //cookie.Domain = CookieDomain;

            if (!string.IsNullOrEmpty(CookieDomain))
                cookie.Domain = CookieDomain;
            cookie.Path = "/";

            if (expires != null && expires.Value != DateTime.MinValue)
                cookie.Expires = expires.Value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 修改Cookie的值 如果不存在键则创建 (不设置过期时间)
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="cookieValue"></param>
        public static void Set(string cookieKey, string cookieValue)
        {
            Set(cookieKey, cookieValue, DateTime.MinValue);
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <returns></returns>
        public static HttpCookie Get(string cookieKey)
        {
            return HttpContext.Current.Request.Cookies[cookieKey];
        }

        /// <summary>
        /// 设置Cookie (不设置过期时间)
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="itemKey">子项</param>
        /// <param name="cookieValue">子项的值</param>
        public static void Set(string cookieKey, string itemKey, string cookieValue)
        {
            Set(cookieKey, itemKey, cookieValue, null);
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="cookieKey">键值</param>
        /// <param name="itemKey">子项</param>
        /// <param name="cookieValue">子项的值</param>
        /// <param name="expires">过期时间</param>
        public static void Set(string cookieKey, string itemKey, string cookieValue, DateTime? expires)
        {
            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieKey];

            cookie[itemKey] = cookieValue;

            //if (AllSettings.Current.SiteSettings.EnableCookieDomain && !string.IsNullOrEmpty(CookieDomain))
            //    cookie.Domain = CookieDomain;

            if (!string.IsNullOrEmpty(CookieDomain))
                cookie.Domain = CookieDomain;
            cookie.Path = "/";

            if (expires != null)
                cookie.Expires = expires.Value;

        }
    }
}