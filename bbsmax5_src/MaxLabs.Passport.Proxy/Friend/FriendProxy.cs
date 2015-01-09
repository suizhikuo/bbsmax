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
    
    public class FriendProxy:ProxyBase
    {
                /// <summary>
        /// 好友热度
        /// </summary>
        public int Hot { get; set; }

        /// <summary>
        /// 好友的所有者ID
        /// </summary>
        public int OwnerID { get; set; }

        /// <summary>
        /// 好友ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 好友组
        /// </summary>
        public int GroupID { get; set; }

        /// <summary>
        /// 成为好友时间
        /// </summary>
        public DateTime CreateDate { get; set; }



        /// <summary>
        /// 当前用户是否在线（不包括隐身）
        /// </summary>
        public bool IsOnline
        {
            get;
            set;
        }



        public bool IsShield
        {
            get;
            set;
        }
    }
}