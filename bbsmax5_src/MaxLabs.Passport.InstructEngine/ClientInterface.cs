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
using MaxLabs.Passport.Proxy;
using System.Net;
using System.IO;

namespace MaxLabs.Passport.InstructEngine
{
    public class ClientInterface
    {
        public string Url
        {
            get;
            set;
        }

        public void ReceiveInstruct(string key,string clientUrl, InstructProxy[] instructs)
        {
            string instructXML = Serializer.GetXML(instructs);

            WebRequest request = HttpWebRequest.Create(clientUrl);
            request.Method = "POST";
            request.ContentType = "application/octet-stream";
            request.Headers.Add("key", key);

            using (Stream stream = request.GetRequestStream())
            {
                byte[] contents = Encoding.UTF8.GetBytes(instructXML);
                stream.Write(contents, 0, contents.Length);
                stream.Close();
            }
            WebResponse response = request.GetResponse();
            using(StreamReader reader  = new StreamReader( response.GetResponseStream(), Encoding.UTF8))
            {
                string responseText = reader.ReadToEnd();
            }
            response.Close();
        }
    }
}