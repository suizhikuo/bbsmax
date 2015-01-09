//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Data;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;
using System.Collections.Specialized;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class BannedUserDao : DaoBase<BannedUserDao>
    {

        public abstract BannedUserCollection GetAllBannedUsers();

        public abstract void CancelBan(IEnumerable<int> userIds, string operatorname, string userip);

        public abstract void CancelBan(IEnumerable<int> userIds, int ForumID, string operatorname, string userip);

        public abstract void BanUser(string operatorname, BanType bantype, DateTime operationtime, string cause, Dictionary<int, DateTime> foruminfos, int userid, string targetname, string userip);

  //      public abstract void BanUser(int userID, Dictionary<int, DateTime> forumBanInfo, string cause);

  //      public abstract void BanUser(int userID, int forumID, DateTime endDate, string cause);

        public abstract SimpleUserCollection GetBannedUsers(int ForumID, int pageSize, int pageNumber, out int totalCount);

        public abstract void BanUsersWholeForum(string operatorName,IEnumerable<int> userIds,string userIP);
    }
}