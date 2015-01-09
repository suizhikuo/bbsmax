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
using System.Web.Services;
using System.Web;
using MaxLabs.bbsMax.Settings;
using System.Web.Services.Protocols;
using System.Net;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;

namespace MaxLabs.Passport.Server
{
    public class ServiceBase : System.Web.Services.WebService
    {
        public ClientInfoHead clientinfo=new ClientInfoHead();
        public ServiceBase()
        {
            if (!AllSettings.Current.PassportServerSettings.EnablePassportService)
            {
                this.Context.Response.Clear();
                Context.Response.Charset = "utf-8";
                Context.Response.Write("passport服务已关闭");
                this.Context.Response.End();
                return ;
            }
       }

        protected bool CheckClient()
        {
            if (CurrentClient == null)
            {
                Context.Response.Clear();
                Context.Response.End();
                return false;
            }
            return true;
        }

        private PassportClient m_CurrentClient;
        protected PassportClient CurrentClient
        {
            get
            {
                if (m_CurrentClient == null)
                {
                    if (clientinfo.ClientID > 0 && !string.IsNullOrEmpty(clientinfo.AccessKey))
                    {
                        PassportClient client = PassportBO.Instance.GetPassportClient(clientinfo.ClientID);
                        if (client == null) return null;
                        if (client.AccessKey == clientinfo.AccessKey)
                        {
                            m_CurrentClient = client;
                        }
                        else
                        {
                            m_CurrentClient = null;
                        }
                    }
                }

                return m_CurrentClient;
            }
        }
    }

    public class ClientInfoHead :SoapHeader
    {
        public ClientInfoHead() { }

        public ClientInfoHead(int clientid, string accesskey)
        {
            this.ClientID = clientid;
            this.AccessKey = accesskey;
        }
        

        public int ClientID { get; set; }

        public string AccessKey { get; set; }
    }
}