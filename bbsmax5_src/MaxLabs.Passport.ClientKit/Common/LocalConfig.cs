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
using System.Collections.Specialized;

namespace MaxLabs.Passport.ClientKit
{
    /// <summary>
    /// Passport客户端本地配置,Web.Config文件里的appSettings 下的那行passportclient
    /// </summary>
    public static class LocalConfig
    {
        static LocalConfig()
        {
            string clientInfos = System.Configuration.ConfigurationManager.AppSettings["passportclient"];
            if (!string.IsNullOrEmpty(clientInfos))
            {
                string[] fieds = clientInfos.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in fieds)
                {
                    int f = s.IndexOf("=");
                    if (f <= 0)continue;

                    string name = s.Substring(0,f);
                    string value = "";

                    if(f<s.Length-1)value = s.Substring(f + 1);

                    if ("clientid".Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        int.TryParse(value, out m_ClientID);
                    }
                    else if( "accesskey".Equals(name, StringComparison.OrdinalIgnoreCase) )
                    {
                        m_AccessKey = value;
                    }
                    else if ("server".Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        value=value.Replace("\\","/");
                        if (!value.EndsWith("/")) value += "/";
                        m_ServerUrl = value;
                        m_APIUrl = value + "api.asmx";
                    }
                }
            }
        }

        private static string m_APIUrl;
        /// <summary>
        /// API文件地址，文件名是固定的都是api.asmx
        /// </summary>
        public static string APIUrl
        {
            get
            {
                return m_APIUrl;
            }
        }

        private static int m_ClientID;

        /// <summary>
        /// 客户端ID
        /// </summary>
        public static int ClientID
        {
            get
            {
                return m_ClientID;
            }
        }

        private static string m_AccessKey;
        /// <summary>
        /// 通信密钥
        /// </summary>
        public static string AccessKey
        {
            get
            {
                return m_AccessKey;
            }
        }

        private static string m_ServerUrl;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string ServerUrl
        {
            get
            {
                return m_ServerUrl;
            }
        }
    }
}