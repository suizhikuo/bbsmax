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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class ImpressionDao : DaoBase<ImpressionDao>
    {
        public abstract ImpressionRecordCollection GetTargetUserImpressionRecords(int targetUserID, int pageNumber, int pageSize, ref int? totalCount);

        public abstract bool CreateImpression(int userID, int targetUserID, string text, int timeLimit);

        public abstract RevertableCollection<ImpressionRecord> GetImpressionRecordsWithReverters(int[] impressionTypeIDs);

        public abstract void UpdateImpressionRecordKeywords(RevertableCollection<ImpressionRecord> processlist);

        public abstract ImpressionTypeCollection GetImpressionTypesForAdmin(AdminImpressionTypeFilter filter, int pageNumber);

        public abstract ImpressionRecordCollection GetImpressionRecordsForAdmin(AdminImpressionRecordFilter filter, int pageNumber);

        public abstract ImpressionTypeCollection GetImpressionTypesForUse(int targetUserID, int usrCount, int sysCount);

        public abstract bool DeleteImpressionTypesForAdmin(AdminImpressionTypeFilter filter, int stepDeleteCount, out int stepCount);

        public abstract bool DeleteImpressionTypes(int[] typeIDs);

        public abstract bool DeleteImpressionRecordsForAdmin(AdminImpressionRecordFilter filter, int stepDeleteCount, out int stepCount);

        public abstract bool DeleteImpressionRecords(int[] recordIDs);

        public abstract ImpressionRecord GetLastImpressionRecord(int userID, int targetUserID);

        public abstract System.Collections.Hashtable GetImressionsTypesForUsers(int[] userIDs, int top);

        public abstract FriendCollection GetFriendsHasImpressions(int userID, int pageNumber, int pageSize);

        public abstract void DeleteImpressionTypeForUser(int userID, int typeID);
    }
}