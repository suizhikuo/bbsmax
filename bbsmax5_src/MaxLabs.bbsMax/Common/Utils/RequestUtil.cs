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
using System.Web;
using System.IO;
using System.Web.Caching;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax
{
    public class RequestUtil
    {

        public static string GetPlatformName(HttpRequest request)
        {
            SpiderType spiderType;
            return GetPlatformName(request, out spiderType);
        }

        /// <summary>
        /// 获得操作系统名称（及版本号）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isSpider"></param>
        /// <returns></returns>
        public static string GetPlatformName(HttpRequest request, out SpiderType spiderType)
        {
            spiderType = SpiderType.Other;
            string userAgent = request.UserAgent;

            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            else if (userAgent.IndexOf("Windows NT 6.1") != -1)
                return "Windows 7";

            else if (userAgent.IndexOf("Windows NT 6") != -1)
                return "Windows Vista";

            else if (userAgent.IndexOf("Windows NT 5.1") != -1)
                return "Windows XP";

            else if (userAgent.IndexOf("Windows NT 5.2") != -1)
                return "Windows Server 2003";

            else if (userAgent.IndexOf("Windows NT 5") != -1)
                return "Windows 2000";

            else if (userAgent.IndexOf("iPhone") != -1)
                return "iPhone";

            else if (userAgent.IndexOf("(iPad;") != -1)
                return "iPad";

            else if (userAgent.IndexOf("Android") != -1)
                return "Android";

            else if (userAgent.IndexOf("9x") != -1)
                return "Windows ME";

            else if (userAgent.IndexOf("98") != -1)
                return "Windows 98";

            else if (userAgent.IndexOf("95") != -1)
                return "Windows 95";

            else if (userAgent.IndexOf("NT 4") != -1)
                return "Windows NT 4";

            spiderType = GetSpiderName(userAgent);
            if (spiderType != SpiderType.Other)
            {
                return spiderType.ToString();
            }

            if (request.Browser != null && !string.IsNullOrEmpty(request.Browser.Platform))
                return request.Browser.Platform.Replace("WinCE", "Windows CE");
            else
                return "Unknown";

        }

        /// <summary>
        /// 根据UserAgent获取蜘蛛名称，如果不是可识别的蜘蛛将返回null
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static SpiderType GetSpiderName(string userAgent)
        {
            //Baidu
            if (userAgent.IndexOf("baiduspider", StringComparison.OrdinalIgnoreCase) != -1 && userAgent.IndexOf("www.baidu.com/search/spider.htm") != -1)
                return SpiderType.Baidu;

            //Yahoo
            else if (userAgent.IndexOf("yahooseeker/", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Yahoo;

            else if (userAgent.IndexOf("yahoo!", StringComparison.OrdinalIgnoreCase) != -1)
            {
                if (userAgent.IndexOf("help.yahoo.com/help/us/ysearch/slurp", StringComparison.OrdinalIgnoreCase) != -1)
                    return SpiderType.Yahoo;
                else if (userAgent.IndexOf("misc.yahoo.com.cn/help.html", StringComparison.OrdinalIgnoreCase) != -1)
                    return SpiderType.Yahoo;
                else if (userAgent.IndexOf("misc.yahoo.com.cn/help.html", StringComparison.OrdinalIgnoreCase) != -1)
                    return SpiderType.Yahoo;
            }

            //Google
            else if (userAgent.IndexOf("googlebot", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Google;

            else if (string.Compare(userAgent, "mediapartners-google", true) == 0)
                return SpiderType.Google;

            else if (string.Compare(userAgent, "google image crawler", true) == 0)
                return SpiderType.Google;
            //Sougou
            else if (userAgent.IndexOf("sogou", StringComparison.OrdinalIgnoreCase) != -1 && userAgent.IndexOf("spider", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Sogou;

            //Sohu
            else if (string.Compare(userAgent, "sohu-search", true) == 0)
                return SpiderType.Sohu;

            //Youdao
            else if (userAgent.IndexOf("youdaobot/", StringComparison.OrdinalIgnoreCase) != -1 && userAgent.IndexOf("www.youdao.com/help/webmaster/spider/", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Youdao;

            else if (userAgent.IndexOf("yodaobot/", StringComparison.OrdinalIgnoreCase) != -1 && userAgent.IndexOf("www.yodao.com/help/webmaster/spider/", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Youdao;

            //Windows Live Search
            else if (userAgent.IndexOf("msnbot", StringComparison.OrdinalIgnoreCase) != -1 && userAgent.IndexOf("search.msn.com/msnbot.htm", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Bing;

            //Qihoo
            else if (userAgent.IndexOf("qihoobot", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Qihoo;

            //Soso
            else if (userAgent.IndexOf("sosospider", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Soso;

            //Alexa
            else if (userAgent.IndexOf("ia_archiver", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Alexa;

            else if (userAgent.IndexOf("iaarchiver", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Alexa;

            //ASPSeek
            else if (userAgent.IndexOf("aspseek/", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.ASPSeek;

            //Oracle
            else if (string.Compare(userAgent, "oracle ultra search", true) == 0)
                return SpiderType.Oracle;

            //Lexxe
            else if (userAgent.IndexOf("lexxebot/", StringComparison.OrdinalIgnoreCase) != -1)
                return SpiderType.Lexxe;

            return SpiderType.Other;
        }

        /// <summary>
        /// 获得浏览器名称（包括版本号）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetBrowserName(HttpRequest request)
        {
            HttpBrowserCapabilities browser = request.Browser;

            if (browser == null)
                return "Unknown";

            string text;
            if (browser.Browser == "IE")
            {
                if (browser.Beta)
                    text = string.Concat("Internet Explorer ", browser.Version, "(beta)");
                else
                    text = "Internet Explorer " + browser.Version;
            }
            else
            {
                string userAgent = request.UserAgent;

                if (userAgent != null && userAgent.IndexOf("Chrome") != -1)
                    text = "Chrome";
                else if (userAgent != null && userAgent.IndexOf("Safari") != -1)
                    text = "Safari";
                else if (browser.Beta)
                    text = string.Concat(browser.Browser, " ", browser.Version, "(beta)");
                else
                    text = string.Concat(browser.Browser, " ", browser.Version);
            }

            return text;

        }

        #region GZIP压缩相关

        /// <summary>
        /// 获取压缩类型
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="prefs"></param>
        /// <returns></returns>
        public static CompressingType GetCompressingType(HttpContext context)
        {
            string acceptedTypes = context.Request.Headers["Accept-Encoding"];
            // if we couldn't find the header, bail out
            if (acceptedTypes == null)
            {
                return CompressingType.None;
            }

            // the actual types could be , delimited.  split 'em out.
            string[] schemes = acceptedTypes.Split(',');


            bool foundDeflate = false;
            bool foundGZip = false;
            bool foundStar = false;

            float deflateQuality = 0f;
            float gZipQuality = 0f;
            float starQuality = 0f;

            bool isAcceptableDeflate;
            bool isAcceptableGZip;
            bool isAcceptableStar;

            for (int i = 0; i < schemes.Length; i++)
            {
                string acceptEncodingValue = schemes[i].Trim();

                if (StringUtil.StartsWithIgnoreCase(acceptEncodingValue, "deflate"))
                {
                    foundDeflate = true;

                    float newDeflateQuality = GetQuality(acceptEncodingValue);
                    if (deflateQuality < newDeflateQuality)
                        deflateQuality = newDeflateQuality;
                }

                else if (StringUtil.StartsWithIgnoreCase(acceptEncodingValue, "gzip") || StringUtil.StartsWithIgnoreCase(acceptEncodingValue, "x-gzip"))
                {
                    foundGZip = true;

                    float newGZipQuality = GetQuality(acceptEncodingValue);
                    if (gZipQuality < newGZipQuality)
                        gZipQuality = newGZipQuality;
                }

                else if (StringUtil.StartsWith(acceptEncodingValue, '*'))
                {
                    foundStar = true;

                    float newStarQuality = GetQuality(acceptEncodingValue);
                    if (starQuality < newStarQuality)
                        starQuality = newStarQuality;
                }
            }

            isAcceptableStar = foundStar && (starQuality > 0);
            isAcceptableDeflate = (foundDeflate && (deflateQuality > 0)) || (!foundDeflate && isAcceptableStar);
            isAcceptableGZip = (foundGZip && (gZipQuality > 0)) || (!foundGZip && isAcceptableStar);

            if (isAcceptableDeflate && !foundDeflate)
                deflateQuality = starQuality;

            if (isAcceptableGZip && !foundGZip)
                gZipQuality = starQuality;


            // do they support any of our compression methods?
            if (!(isAcceptableDeflate || isAcceptableGZip || isAcceptableStar))
            {
                return CompressingType.None;
            }


            // if deflate is better according to client
            if (isAcceptableDeflate && (!isAcceptableGZip || (deflateQuality > gZipQuality)))
                return CompressingType.Deflate;

            // if gzip is better according to client
            if (isAcceptableGZip && (!isAcceptableDeflate || (deflateQuality < gZipQuality)))
                return CompressingType.GZip;

            if (isAcceptableGZip)
                return CompressingType.GZip;

            // if we're here, the client either didn't have a preference or they don't support compression
            if (isAcceptableDeflate)
                return CompressingType.Deflate;

            if (isAcceptableDeflate || isAcceptableStar)
                return CompressingType.Deflate;
            if (isAcceptableGZip)
                return CompressingType.GZip;

            // return null.  we couldn't find a filter.
            return CompressingType.None;
        }

        static float GetQuality(string acceptEncodingValue)
        {
            int qParam = acceptEncodingValue.IndexOf("q=", StringComparison.OrdinalIgnoreCase);

            if (qParam >= 0)
            {
                float val = 0.0f;
                try
                {
                    val = float.Parse(acceptEncodingValue.Substring(qParam + 2, acceptEncodingValue.Length - (qParam + 2)));
                }
                catch (FormatException)
                {

                }
                return val;
            }
            else
                return 1;
        }


        public class FastAspxCacheData
        {
            public byte[] Data;
            public DateTime LastModified;
        }

        public static bool CompressStaticContent(HttpContext context)
        {
            if (Globals.UseStaticCompress == false || StringUtil.EndsWithIgnoreCase(context.Request.PhysicalPath, ".fast.aspx") == false)
                return false;

            SetInstalledKey(context);

            string physicalPath = context.Request.PhysicalPath.Remove(context.Request.PhysicalPath.Length - 10);
            string contentType = null;

            if (StringUtil.EndsWithIgnoreCase(physicalPath, ".css"))
                contentType = "text/css";

            else if (StringUtil.EndsWithIgnoreCase(physicalPath, ".js"))
                contentType = "application/x-javascript";

            else
                return false;

            CompressingType compressingType = RequestUtil.GetCompressingType(context);

            context.Response.ClearHeaders();

            if (compressingType == CompressingType.None)
            {
                if (File.Exists(physicalPath))
                {
                    DateTime lastModified = File.GetLastWriteTime(physicalPath);

                    if (context.Request.Headers["If-Modified-Since"] != null)
                    {
                        DateTime ifModifiedSince;

                        if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"].Split(';')[0], out ifModifiedSince))
                        {
                            if (ifModifiedSince > lastModified.AddSeconds(-1))
                            {
                                context.Response.StatusCode = 304;
                                context.Response.StatusDescription = "Not Modified";
                                context.ApplicationInstance.CompleteRequest();
                                return true;
                            }
                        }
                    }

                    context.Response.ContentType = contentType;

                    if (lastModified < DateTime.Now)
                        context.Response.Cache.SetLastModified(lastModified);

                    context.Response.Cache.SetETag(lastModified.Ticks.ToString());
                    context.Response.BinaryWrite(File.ReadAllBytes(physicalPath));

                    context.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                string cacheKey = string.Concat("fast.aspx/", compressingType.ToString(), "/", physicalPath);

                FastAspxCacheData cacheData = null;

                if (CacheUtil.TryGetValue<FastAspxCacheData>(cacheKey, out cacheData) == false)
                {
                    if (File.Exists(physicalPath))
                    {
                        cacheData = new FastAspxCacheData();

                        using (MemoryStream stream = new MemoryStream())
                        {

                            Stream compressStream = null;

                            if (compressingType == CompressingType.GZip)
                                compressStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress);
                            else if (compressingType == CompressingType.Deflate)
                                compressStream = new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Compress);

                            byte[] buffer = File.ReadAllBytes(physicalPath);

                            compressStream.Write(buffer, 0, buffer.Length);
                            compressStream.Close();
                            compressStream.Dispose();
                            compressStream = null;

                            cacheData.Data = stream.ToArray();

                        }

                        cacheData.LastModified = File.GetLastWriteTime(physicalPath);

                        CacheUtil.Set<FastAspxCacheData>(cacheKey, cacheData, CacheTime.Long, CacheExpiresType.Sliding, new CacheDependency(physicalPath));
                    }
                }

                if (cacheData != null && cacheData.Data.Length > 0)
                {
                    if (context.Request.Headers["If-Modified-Since"] != null)
                    {
                        DateTime ifModifiedSince;

                        if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"].Split(';')[0], out ifModifiedSince))
                        {
                            if (ifModifiedSince > cacheData.LastModified.AddSeconds(-1))
                            {
                                context.Response.StatusCode = 304;
                                context.Response.StatusDescription = "Not Modified";
                                context.ApplicationInstance.CompleteRequest();
                                return true;
                            }
                        }
                    }

                    context.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;

                    context.Response.ContentType = contentType;

                    if (compressingType == CompressingType.GZip)
                        context.Response.AppendHeader("Content-Encoding", "gzip");
                    else
                        context.Response.AppendHeader("Content-Encoding", "deflate");

                    if (cacheData.LastModified < DateTime.Now)
                        context.Response.Cache.SetLastModified(cacheData.LastModified);

                    context.Response.Cache.SetETag(cacheData.LastModified.Ticks.ToString());
                    context.Response.BinaryWrite(cacheData.Data);

                    context.ApplicationInstance.CompleteRequest();
                }
            }

            return true;
        }


        const string INSTALLED_KEY = "httpcompress.attemptedinstall";
        static readonly object INSTALLED_TAG = new object();
        public static void SetInstalledKey(HttpContext content)
        {
            if (content.Items.Contains(INSTALLED_KEY))
                content.Items[INSTALLED_KEY] = INSTALLED_TAG;
            else
                content.Items.Add(INSTALLED_KEY, INSTALLED_TAG);
        }
        public static bool ContainsInstalledKey(HttpContext content)
        {
            if (content.Items.Contains(INSTALLED_KEY))
                return true;
            else
                return false;
        }

        #endregion
    }

    public enum CompressingType
    {
        GZip, Deflate, None
    }
}