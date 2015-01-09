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
using System.Collections.Specialized;
using System.Web;
using MaxLabs.bbsMax.RegExp;

namespace MaxLabs.bbsMax
{
    public class UrlUtil
    {
        private readonly static string[] s_Domains_CN = new string[] { "com", "net", "org", "gov", "edu", "co", "中国", "公司", "网络", "ac", "bj", "sh", "tj", "cq", "he", "sx", "nm", "ln", "jl", "hl", "js", "zj", "ah", "fj", "jx", "sd", "ha", "hb", "hn", "gd", "gx", "hi", "sc", "gz", "yn", "xz", "sn", "gs", "qh", "nx", "xj", "tw", "hk", "mo" };
        private readonly static string[] s_Domains_RU = new string[] { "com", "net", "org", "gov", "edu", "co", "pp" };

        private static Regex s_LocalRegex = null;
        private static RootRegex s_RootRegex = null;

        private static Dictionary<string, KeyValuePair<string, string>> s_CachedMainDomains = new Dictionary<string, KeyValuePair<string, string>>(StringComparer.OrdinalIgnoreCase);

        public static void BuildMainDomain(string domain, bool cacheable, out string mainDomain, out string cookieDomain)
        {
            KeyValuePair<string, string> domainInfo;

            if (s_CachedMainDomains.TryGetValue(domain, out domainInfo))
            {
                mainDomain = domainInfo.Key;
                cookieDomain = domainInfo.Key;

                return;
            }

            mainDomain = null;

            string[] splitDomain = domain.Split('.');

            if (splitDomain.Length == 4 && ValidateUtil.IsIPAddress(domain))    //如果是ip地址
                mainDomain = string.Empty;

            else if (splitDomain.Length == 2)
                mainDomain = string.Empty;

            else if (splitDomain.Length > 2)
            {
                switch (splitDomain[splitDomain.Length - 1])
                {
                    case "cn":
                        foreach (string part in s_Domains_CN)
                        {
                            if (splitDomain[splitDomain.Length - 2] == part)
                            {
                                mainDomain = string.Concat(splitDomain[splitDomain.Length - 3], ".", splitDomain[splitDomain.Length - 2], ".", splitDomain[splitDomain.Length - 1]);
                                break;
                            }
                        }
                        break;

                    case "ru":
                        foreach (string part in s_Domains_RU)
                        {
                            if (splitDomain[splitDomain.Length - 2] == part)
                            {
                                mainDomain = string.Concat(splitDomain[splitDomain.Length - 3], ".", splitDomain[splitDomain.Length - 2], ".", splitDomain[splitDomain.Length - 1]);
                                break;
                            }
                        }
                        break;

                    default:
                        break;
                }

                if (string.IsNullOrEmpty(mainDomain))
                    mainDomain = string.Concat(splitDomain[splitDomain.Length - 2], ".", splitDomain[splitDomain.Length - 1]);
            }
            else
                mainDomain = string.Empty;


            if (mainDomain.Length == 0)
            {
                cookieDomain = string.Empty;
                mainDomain = domain;
            }
            else
            {
                cookieDomain = "." + mainDomain;
            }
        }

        /// <summary>
        /// 获取主域
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string GetMainDomain(string domain)
        {
            string maindomain, cookiedomain;

            BuildMainDomain(domain, false, out maindomain, out cookiedomain);

            return maindomain;
        }

        /// <summary>
        /// 获取cookie域
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static string GetCookieDomain(string domain)
        {
            string maindomain, cookiedomain;

            BuildMainDomain(domain, false, out maindomain, out cookiedomain);

            return cookiedomain;
        }

        public static string ReplaceRootVar(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            if (s_RootRegex == null)
                s_RootRegex = new RootRegex();

            return s_RootRegex.Replace(content, Globals.AppRoot);
        }

        /// <summary>
        /// 检查地址是否为本地地址（包含相对路径和绝对路径，例如：xxx://开头的都不是本地地址）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLocalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            if (StringUtil.StartsWith(url, '/'))
                return true;

            //最常见的前缀先判断，避免每次调用正则性能下降
            if (
                StringUtil.StartsWithIgnoreCase(url, "http://")
                || StringUtil.StartsWithIgnoreCase(url, "https://")
                || StringUtil.StartsWithIgnoreCase(url, "ftp://")
                )
            {
                return false;
            }

            if (s_LocalRegex == null)
                s_LocalRegex = new Regex(@"^\w+://.*$", RegexOptions.IgnoreCase);

            return !s_LocalRegex.IsMatch(url);

        }


        /// <summary>
        /// 检查指定的Url是否是应用程序内的地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrlInApp(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            //如果有提交ReturnUrl且是本程序内的安全地址，则返回true
            if (
                StringUtil.StartsWith(url, Globals.AppRoot + "/")
                || StringUtil.StartsWith(url, Globals.FullAppRoot + "/")
                || url == Globals.AppRoot
                || url == Globals.FullAppRoot
                )

                return true;

            return false;
        }

        public static bool IsUrlInMainDomain(string url)
        {
            //如果有提交ReturnUrl且是本程序内的安全地址，则登陆成功后自动跳转到returnurl
            if (string.IsNullOrEmpty(url))
                return false;

            //绝对地址，那么肯定是安全地址
            if (url[0] == '/')
                return true;

            Uri uri;

            try
            {
                uri = new Uri(url);
            }
            catch
            {
                return false;
            }

            //得到传入url的主域（例如 www.abc.com 将得到 abc.com）
            string mainDomain = GetMainDomain(uri.DnsSafeHost);

            string domain = HttpContext.Current.Request.Url.DnsSafeHost;

            //如果当前请求的域名确实属于传入url的主域，那么return true
            if (string.Compare(domain, mainDomain, true) == 0 || StringUtil.EndsWithIgnoreCase(domain, "." + mainDomain))
                return true;

            return false;
        }

        ////private static Regex ipRegex = new Regex(@"^(\d+\.*)+$");
        //public static string GetMainDomain(string domain, bool forCookieDomain)
        //{
        //    string mainDomain = null;

        //    string[] splitDomain = domain.Split('.');

        //    if (splitDomain.Length == 4 && ValidateUtil.IsIPAddress(domain))    //如果是ip地址
        //        mainDomain = string.Empty;

        //    else if (splitDomain.Length == 2)
        //        mainDomain = string.Empty;

        //    else if (splitDomain.Length > 2)
        //    {
        //        switch (splitDomain[splitDomain.Length - 1])
        //        {
        //            case "cn":
        //                foreach (string part in Domains_CN)
        //                {
        //                    if (splitDomain[splitDomain.Length - 2] == part)
        //                    {
        //                        mainDomain = string.Concat(splitDomain[splitDomain.Length - 3], ".", splitDomain[splitDomain.Length - 2], ".", splitDomain[splitDomain.Length - 1]);
        //                        break;
        //                    }
        //                }
        //                break;

        //            case "ru":
        //                foreach (string part in Domains_RU)
        //                {
        //                    if (splitDomain[splitDomain.Length - 2] == part)
        //                    {
        //                        mainDomain = string.Concat(splitDomain[splitDomain.Length - 3], ".", splitDomain[splitDomain.Length - 2], ".", splitDomain[splitDomain.Length - 1]);
        //                        break;
        //                    }
        //                }
        //                break;

        //            default:
        //                break;
        //        }

        //        if (string.IsNullOrEmpty(mainDomain))
        //            mainDomain = string.Concat(splitDomain[splitDomain.Length - 2], ".", splitDomain[splitDomain.Length - 1]);
        //    }
        //    else
        //        mainDomain = string.Empty;

        //    if (forCookieDomain)
        //    {
        //        if (mainDomain.Length != 0)
        //            mainDomain = "." + mainDomain;
        //    }
        //    else
        //    {
        //        if (mainDomain.Length == 0)
        //            mainDomain = domain;
        //    }

        //    return mainDomain;
        //}

        public static string ResolveUrl(string relativeUrl)
        {
            if (relativeUrl != null && StringUtil.StartsWith(relativeUrl, "~/"))
            {
                return JoinUrl(Globals.AppRoot, relativeUrl.Substring(1));
            }
            return relativeUrl;
        }

        /// <summary>
        /// 移除路径中的.和..，并转换成其对应的真实路径。
        /// 例如 aa/bb/../cc 将被处理为 aa/cc
        /// </summary>
        /// <returns></returns>
        public static string ConvertDots(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (url.Contains("./") == false || url.Contains("/") == false)
                return url;

            string[] parts = url.Split('/');

            List<string> builder = new List<string>();

            foreach (string part in parts)
            {
                if (part == ".")
                    continue;

                else if (part == "..")
                {
                    if (builder.Count <= 0 || (builder.Count == 1 && builder[0] == ""))
                        throw new Exception("当前提供的路径中的..已经超出了路径的范围，无法处理");


                    builder.RemoveAt(builder.Count - 1);
                }

                else
                    builder.Add(part);
            }

            StringBuilder urlBuilder = new StringBuilder();

            bool isFirst = true;
            foreach (string newItem in builder)
            {
                if (isFirst == true)
                    isFirst = false;
                else
                    urlBuilder.Append("/");

                urlBuilder.Append(newItem);
            }

            return urlBuilder.ToString();
        }

        private static readonly char[] urlTrimChars = new char[] { '/', '\\' };

        /// <summary>
        /// 拼接两个Url，不管url1的结尾是否包含/或\，也不管url2的开头是否包含/或\，都能正确地拼接
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static string JoinUrl(string url1, string url2)
        {
            string url = string.Concat(url1.TrimEnd(urlTrimChars), "/", url2.TrimStart(urlTrimChars));
            return url.Replace('\\', '/');
        }

        public static string JoinUrl(string url1, string url2, string url3)
        {
            string url = string.Concat(url1.TrimEnd(urlTrimChars), "/", url2.Trim(urlTrimChars), "/", url3.TrimStart(urlTrimChars));
            //while (url.Contains("//"))
            //{
            //    url = url.Replace("//", "/");
            //}
            return url.Replace('\\', '/');
        }

        /// <summary>
        /// 拼接多个Url，不管前一个url的结尾是否包含/或\，也不管后一个url的开头是否包含/或\，都能正确地拼接
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        public static string JoinUrl(params string[] urls)
        {

            if (urls == null || urls.Length == 0)
                return string.Empty;

            else if (urls.Length == 1)
                return urls[0];

            StringBuilder builder = new StringBuilder();

            int i = 0;
            foreach (string url in urls)
            {
                if (string.IsNullOrEmpty(url) == false)
                {
                    if (i == 0)
                    {
                        builder.Append(url.TrimEnd(urlTrimChars));
                    }
                    else if (i == urls.Length - 1)
                    {
                        builder.Append("/");
                        builder.Append(url.TrimStart(urlTrimChars));
                    }
                    else
                    {
                        builder.Append("/");
                        builder.Append(url.Trim(urlTrimChars));
                    }
                }
                i++;
            }
            return builder.Replace('\\', '/').ToString();
            //return builder.ToString();
        }

        public static HttpValueCollection ParseQueryString(string query)
        {
            return ParseQueryString(query, Encoding.UTF8);
        }

        public static HttpValueCollection ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            if (query.Length > 0 && query[0] == '?')
            {
                query = query.Substring(1);
            }
            return new HttpValueCollection(query, false, true, encoding);
        }



        public static string FormatLink(string link)
        {
            if (link.IndexOf("://") > 0)
                return link;
            else
                return "http://" + link;
        }

        public static string SafeUrl(string url)
        {
            StringBuilder builder = new StringBuilder(url);
            builder = builder.Replace("<", "%3C");
            builder = builder.Replace(">", "%3E");
            builder = builder.Replace("\"", "%22");
            builder = builder.Replace("'", "%27");
            return builder.ToString();
        }


        public static bool IsUrlCanReturn(string url)
        {
            string urlInfo = BbsRouter.GetUrlInfo(url);

            switch (urlInfo)
            {
                case "login":
                case "logout":
                case "register":
                case "recoverpassword":
                case "info":
                    return false;

                default:
                    if (urlInfo.StartsWith("logout/", StringComparison.OrdinalIgnoreCase)
                        ||
                        urlInfo.StartsWith("max-dialogs/", StringComparison.OrdinalIgnoreCase)
                        ||
                        urlInfo.StartsWith("register/", StringComparison.OrdinalIgnoreCase)
                        )
                        return false;
                    else
                        return true;
            }
        }

        public static void Redirect(string url)
        {
            System.Web.HttpContext.Current.Response.Status = "301 Moved Permanently";
            System.Web.HttpContext.Current.Response.AddHeader("Location", url); 

        }

    }
}