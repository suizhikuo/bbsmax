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
using MaxLabs.Passport.ClientKit.PassportServerInterface;

namespace MaxLabs.Passport.ClientKit
{
    /// <summary>
    /// 通过这个类的API对象访问Passport接口
    /// </summary>
    public static class AsmxAccess
    {
        private static PassportServerInterface.Service m_API;

        /// <summary>
        /// WebService访问对象
        /// </summary>
        public static PassportServerInterface.Service API
        {
            get
            {
                PassportServerInterface.Service api = m_API;
                if (api == null)
                {
                    api = new PassportServerInterface.Service();
                    api.Url = LocalConfig.APIUrl;             //passport服务端 Api文件所在URL
                    api.ClientInfoHeadValue = SoapHeader;     //附带客户端信息
                    api.Timeout = 10 * 1000;                  // web service通讯超时时间
                    m_API = api;
                }

                return m_API;
            }
        }

        private static ClientInfoHead m_SoapHeader;
        private static ClientInfoHead SoapHeader
        {
            get
            {
                ClientInfoHead soapHeader = m_SoapHeader;
                if (soapHeader == null)
                {
                    soapHeader = new ClientInfoHead();
                    soapHeader.ClientID = LocalConfig.ClientID;    //客户端ID
                    soapHeader.AccessKey = LocalConfig.AccessKey;  //通讯密钥
                    m_SoapHeader = soapHeader;
                }
                return soapHeader;
            }
        }
    }
}