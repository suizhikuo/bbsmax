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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class EmoticonDao:DaoBase<EmoticonDao>
    {
        public abstract UserEmoticonInfoCollection AdminGetUserEmoticonInfos(EmoticonFilter filter, int pageIndex, IEnumerable<Guid> excludeRoleIDs);
        public abstract List<string> AdminDeleteUserAllEmoticon(int userID);
        public abstract EmoticonCollection AdminGetUserEmoticons(int userID, int pageSize, int pageIndex);
        public abstract List<string> AdminiDeleteEmoticons(int userID, IEnumerable<int> emoticonIds);

        public abstract EmoticonCollection GetEmoticons(int userID, IEnumerable<int> emoticonID);
        public abstract EmoticonCollection GetEmoticons(int userID, int GroupID, int pageSize, int pageNumber, bool isDesc, out int totalCount);
        public abstract EmoticonCollection GetEmoticons(int userID, int groupID);
        public abstract EmoticonCollection GetEmoticons(int userID);

        public abstract EmoticonGroupCollection GetEmoticonGroups(int userID);

        public abstract EmoticonGroup CreateGroup(int userID, string groupName);

        public abstract EmoticonGroup CreateGroup(int userID, string groupName, EmoticonCollection emotions);

        public abstract EmoticonGroup GetEmoticonGroup( int userID, int groupID);
        
        public abstract List<string> DeleteGroup(int userID, int groupID);

        public abstract void RenameGroup( int userID, int groupID, string groupName);

        public abstract void CreateEmoticons(EmoticonCollection emoticons);

        public abstract List<string> DeleteEmoticons(int userID, int groupID, IEnumerable<int> emoticonIDs);

        /*--------------------3.0-----------------------------*/
        public abstract bool RenameEmoticonShortcut(int userID, int groupID, List<int> emoticonIDs, List<string> newShortcuts);
        public abstract bool CreateEmoticonsAndGroups(int userID, Dictionary<string, EmoticonCollection> emoticons);
        public abstract bool MoveEmoticons(int userID, int groupID, int newGroupID, IEnumerable<int> emoticonIdentities);

    }
}