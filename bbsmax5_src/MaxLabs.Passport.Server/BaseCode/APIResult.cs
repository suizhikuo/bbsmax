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
using System.Runtime.Serialization;
using MaxLabs.Passport.Proxy;

namespace MaxLabs.Passport.Server
{
    [Serializable]
    public class APIResult
    {
       
        public bool IsSuccess { get; set; }

        private List<string> m_Message { get; set; }

        public void AddError(string message)
        {
            AddError(string.Empty, message);
        }

        public void AddError(string target, string message)
        {
            this.ErrorTargets.Add(target);
            this.Messages.Add(message);
        }

        public List<string> Messages
        {
            get
            {
                if (m_Message == null)
                    m_Message = new List<string>();
                return m_Message;
            }
        }

        private List<string> m_ErrorTargets { get; set; }
        public List<string> ErrorTargets
        {
            get
            {
                if (m_ErrorTargets == null)
                    m_ErrorTargets = new List<string>();
                return m_ErrorTargets;
            }
        }

        public ProxyBase Data { get; set; }

        public int ErrorCode { get; set; }

    }
}