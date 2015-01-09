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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax
{
    public class BannedUserBO : BOBase<BannedUserBO>
    {
        private static BannedUserCollection s_AllBannedUsers = null;

        /// <summary>
        /// 获得所有版块的所有屏蔽用户，包含尚未生效的和已经过期的
        /// </summary>
        /// <returns></returns>
        public BannedUserCollection GetAllBannedUsers()
        {
            BannedUserCollection result = s_AllBannedUsers;

            if (result == null)
            {
                result = BannedUserDao.Instance.GetAllBannedUsers();
                s_AllBannedUsers = result;
            }

            return result;
        }

        /// <summary>
        /// 清除屏蔽用户的缓存
        /// </summary>
        internal void ClearBannedUserCache()
        {
            s_AllBannedUsers = null;
            ForumCollection forums = ForumBO.Instance.GetAllForums();
            foreach (Forum forum in forums)
            {
                forum.ClearBannedUserCache();
            }
        }

        /// <summary>
        /// 获得所有板块的有效期内的屏蔽用户
        /// </summary>
        /// <returns></returns>
        public BannedUserCollection GetBannedUsers()
        {
            return GetAllBannedUsers().Limited;
        }
    }
}