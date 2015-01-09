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
    public class DataForNewUser : UserProxy
    {
        public DataForNewUser() { }

        public bool IsActive{get;set;}

        public string Password { get; set; }
        
        public int PasswordFormat { get; set; }

        public float TimeZone { get; set; }

        public string GenderName{get;set;}

        public string Signature { get; set; }

        public int SignatureFormat { get; set; }

        public string IPAddress { get; set; }

        public int InviterID { get; set; }

        public List<UserExtendedValueProxy> ExtendedFields { get; set; }

        public List<FriendGroupProxy> FriendGroups { get; set; }
    }
}