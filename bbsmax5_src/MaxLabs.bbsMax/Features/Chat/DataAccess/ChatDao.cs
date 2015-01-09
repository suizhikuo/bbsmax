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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess
{
    public abstract class ChatDao : DaoBase<ChatDao>
    {

        public abstract void AdminDeleteSessions(IEnumerable<int> sessionIds, IEnumerable<Guid> exludeRoles);

        public abstract void ClearMessage(int days, int rows, bool ClearNoRead, JobDataClearMode mode);

        public abstract void AdminDeleteMessage(IEnumerable<int> messageIDs , IEnumerable<Guid> excludeRoleIds);

        public abstract bool IgnoreAllMessage(int userID);

        public abstract void IgnoreSession(int userID, int targetUserID);

        public abstract ChatSessionCollection AdminGetSessions( ChatSessionFilter filter,int pageNumber,IEnumerable<Guid> excludeRoleIds);

        public abstract ChatMessageCollection GetLastChatMessages(int userID, int targetUserID,int maxMessageID,int MessageCount);

        public abstract ChatSession GetChatSession(int sessionID);

        public abstract ChatSessionCollection GetChatSessions(int userID, int pageNumber, int pageSize);

        public abstract ChatSession GetChatSession(int userID, int targetUserID);

        public abstract ChatSessionCollection GetChatSessionsWithUnreadMessages(int userID, int topCount);

        //public abstract ChatMessageCollection GetLastChatMessages(int userID, int targetUserID, int lastCount, bool returnTotalRecords, bool setIsRead);

        public abstract ChatMessageCollection GetChatMessages(int userID, int targetUserID, int pageNumber, int pageSize, bool updateIsReaded);

        public abstract ChatMessageCollection SendMessage(int userID, int targetUserID, string content, string ip, bool getNewMessages, int lastMessageID);

        public abstract bool DeleteChatSessions(int userID, IEnumerable<int> targetUserIds);

        public abstract bool DeleteChatMessages(int userID, IEnumerable<int> messageIds);

        public abstract RevertableCollection<ChatMessage> GetChatMessageWithReverters(IEnumerable<int> messageIDs);

        public abstract void UpdateChatMessageKeywords(RevertableCollection<ChatMessage> processlist);

    }
}