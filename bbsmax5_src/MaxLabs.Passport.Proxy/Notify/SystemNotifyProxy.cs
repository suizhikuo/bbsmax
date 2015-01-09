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
    
    public class SystemNotifyProxy : NotifyProxy
    {
        public SystemNotifyProxy() 
        {
        }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public string  Subject { get; set; }

        public List<Guid> ReceiveRoles { get; set; }

        public int DispatcherID { get; set; }

        public string DispatcherIP { get; set; }

        public List<int> ReceiveUserIDs { get; set; }

        public DateTime CreateDate { get; set; }

        public string ReadUserIDs { get; set; }


    }
}