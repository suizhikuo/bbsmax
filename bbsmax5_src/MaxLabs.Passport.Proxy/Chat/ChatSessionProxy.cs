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

namespace MaxLabs.Passport.Proxy
{
    
    public class ChatSessionProxy : ProxyBase
    {
        public ChatSessionProxy() { }

        public int ChatSessionID { get; set; }

        public int OwnerID { get; set; }

        public int UserID { get; set; }

        public int TotalMessages { get; set; }

        public int UnreadMessages { get; set; }

        public string LastMessage { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}