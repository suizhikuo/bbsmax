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
    public class UserProxy : ProxyBase
    {
        public UserProxy() { }

        public int UserID;

        public byte Gender { get; set; }

        public DateTime Birthdat { get; set; }

        public long MobilePhone { get; set; }

        public string Realname { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string AvatarSrc { get; set; }

        public string AvatarUrl_120px { get; set; }

        public string AvatarUrl_48px { get; set; }

        public string AvatarUrl_24px { get; set; }

        public int[] Points { get; set; }
    }
}