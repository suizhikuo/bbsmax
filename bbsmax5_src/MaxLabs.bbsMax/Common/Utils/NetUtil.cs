//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.IO;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using System.Net;

namespace MaxLabs.bbsMax
{
    public class NetUtil
    {
        /// <summary>
        /// 获取指定 指定URL的HTML源代码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding">如果为NULL 则自动去获取编码</param>
        /// <returns></returns>
        public static string GetHtml(string url, Encoding encoding)
        {
            try
            {
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);
                HttpWebResponse res;

                try
                {
                    res = (HttpWebResponse)hwr.GetResponse();
                }
                catch
                {
                    return string.Empty;
                }

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream mystream = res.GetResponseStream())
                    {
                        //没有指定编码，猜
                        if (encoding == null)
                        {
                            return DecodeData(mystream, res);
                            //using (MemoryStream msTemp = new MemoryStream())
                            //{
                            //    int len = 0;
                            //    byte[] buff = new byte[512];

                            //    while ((len = mystream.Read(buff, 0, 512)) > 0)
                            //    {
                            //        msTemp.Write(buff, 0, len);

                            //    }
                            //    res.Close();

                            //    if (msTemp.Length > 0)
                            //    {
                            //        msTemp.Seek(0, SeekOrigin.Begin);

                            //        byte[] PageBytes = new byte[msTemp.Length];

                            //        msTemp.Read(PageBytes, 0, PageBytes.Length);
                            //        msTemp.Seek(0, SeekOrigin.Begin);

                            //        int DetLen = 0;
                            //        byte[] DetectBuff = new byte[4096];

                            //        CharsetListener listener = new CharsetListener();
                            //        UniversalDetector Det = new UniversalDetector(null);

                            //        while ((DetLen = msTemp.Read(DetectBuff, 0, DetectBuff.Length)) > 0 && !Det.IsDone())
                            //        {
                            //            Det.HandleData(DetectBuff, 0, DetectBuff.Length);
                            //        }

                            //        Det.DataEnd();

                            //        if (Det.GetDetectedCharset() != null)
                            //        {
                            //            return System.Text.Encoding.GetEncoding(Det.GetDetectedCharset()).GetString(PageBytes);
                            //        }
                            //    }
                            //}
                        }
                        //指定了编码
                        else
                        {
                            using (StreamReader reader = new StreamReader(mystream, encoding))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 用当前用户的身份转发当前页面的内容到远程页面， 并返回远程返回的HTML。
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="url"></param>
        /// <param name="headFields"></param>
        /// <returns></returns>
        public static HttpWebResponse PostToRemoteUrl(HttpRequest Request, string url, params KeyValuePair<string, string>[] headFields)
        {
            string newUrl = url;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(newUrl);

            request.Method = "POST";
            request.ContentType = "application/octet-stream";

            if (headFields != null)
            {
                foreach (KeyValuePair<string, string> s in headFields)
                {
                    request.Headers.Set(s.Key, s.Value);
                }
            }

            byte[] avatarData = new byte[Request.InputStream.Length];

            Request.InputStream.Read(avatarData, 0, avatarData.Length);

            HttpCookie userCookie = Request.Cookies[UserBO.cookieKey_User];

            if (userCookie != null)
            {
                request.Headers.Set(HttpRequestHeader.Cookie, UserBO.cookieKey_User + "=" + userCookie.Value);
            }

            using (Stream str = request.GetRequestStream())
            {
                str.Write(avatarData, 0, avatarData.Length);
                str.Close();
            }

            return (HttpWebResponse)request.GetResponse();
        }

        private static string DecodeData(Stream responseStream, HttpWebResponse response)
        {
            string name = null;
            string text2 = response.Headers["content-type"];
            if (text2 != null)
            {
                int index = text2.IndexOf("charset=");
                if (index != -1)
                {
                    name = text2.Substring(index + 8);
                }
            }
            MemoryStream stream = new MemoryStream();
            byte[] buffer = new byte[0x400];
            for (int i = responseStream.Read(buffer, 0, buffer.Length); i > 0; i = responseStream.Read(buffer, 0, buffer.Length))
            {
                stream.Write(buffer, 0, i);
            }
            responseStream.Close();
            if (name == null)
            {
                MemoryStream stream3 = stream;
                stream3.Seek((long)0, SeekOrigin.Begin);
                string text3 = new StreamReader(stream3, Encoding.ASCII).ReadToEnd();
                if (text3 != null)
                {
                    int startIndex = text3.IndexOf("charset=");
                    int num4 = -1;
                    if (startIndex != -1)
                    {
                        num4 = text3.IndexOf("\"", startIndex);
                        if (num4 != -1)
                        {
                            int num5 = startIndex + 8;
                            name = text3.Substring(num5, (num4 - num5) + 1).TrimEnd(new char[] { '>', '"' });
                        }
                    }
                }
            }
            Encoding aSCII = null;
            if (name == null)
            {
                aSCII = Encoding.GetEncoding("gb2312");
            }
            else
            {
                try
                {
                    if (name == "GBK")
                    {
                        name = "GB2312";
                    }
                    aSCII = Encoding.GetEncoding(name);
                }
                catch
                {
                    aSCII = Encoding.GetEncoding("gb2312");
                }
            }
            stream.Seek((long)0, SeekOrigin.Begin);
            StreamReader reader2 = new StreamReader(stream, aSCII);
            return reader2.ReadToEnd();
        }
    }
}