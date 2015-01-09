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

namespace MaxLabs.bbsMax
{
    public class AdminSessionStruct
    {
        private DateTime m_LastUpdate;

        public DateTime LastUpdate
        {
            get { return m_LastUpdate; }
            set { m_LastUpdate = value; }
        }

        private Guid m_AdminSessionID;

        public Guid AdminSessionID
        {
            get { return m_AdminSessionID; }
            set { m_AdminSessionID = value; }
        }

        private string m_Password;

        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }

        public AdminSessionStruct(Guid sessionID, DateTime lastUpdate, string password)
        {
            LastUpdate = lastUpdate;
            AdminSessionID = sessionID;
            Password = password;
        }
    }
}