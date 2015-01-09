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
using System.Data.SqlClient;

using MaxLabs.bbsMax.Entities;
using System.Data;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class ChatDao : DataAccess.ChatDao
    {

        public override void AdminDeleteSessions(IEnumerable<int> sessionIds, IEnumerable<Guid> exludeRoles)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRoleCondition = DaoUtil.GetExcludeRoleSQL("UserID", exludeRoles, query);

                query.CommandText = "DELETE FROM bx_ChatSessions WHERE ChatSessionID IN( @IDs) ";

                if (!string.IsNullOrEmpty(excludeRoleCondition))
                {
                    query.CommandText += " AND " + excludeRoleCondition;
                }

                query.CreateInParameter<int>("@IDs", sessionIds);
                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Chat_GetLastMessages", Script = @"
CREATE PROCEDURE {name}
 @UserID          int
,@TargetUserID    int
,@LastMessageID    int
AS
BEGIN
    SET NOCOUNT ON;

    IF @LastMessageID = 0
        SELECT TOP 20 * FROM bx_ChatMessages WHERE UserID = @UserID AND TargetUserID = @TargetUserID ORDER BY MessageID DESC;
    ELSE
        SELECT TOP 20 * FROM bx_ChatMessages WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND MessageID > @LastMessageID ORDER BY MessageID DESC;

     UPDATE bx_ChatMessages SET IsRead = 1 WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND IsRead = 0;
END
")]
        public override ChatMessageCollection GetLastChatMessages(int userID, int targetUserID, int lastMessageID, int MessageCount)
        {
            ChatMessageCollection result;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Chat_GetLastMessages";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                query.CreateParameter<int>("@LastMessageID", lastMessageID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    result = new ChatMessageCollection();

                    while (reader.Read())
                    {
                        result.Insert(0, new ChatMessage(reader));
                    }
                }

                return result;
            }
        }

        public override ChatSessionCollection GetChatSessions(int userID, int pageNumber, int pageSize)
        {
            ChatSessionCollection results = new ChatSessionCollection();
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_ChatSessions";
                query.Pager.PrimaryKey = "ChatSessionID";
                query.Pager.SortField = "UpdateDate";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "UserID = @UserID";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    results = new ChatSessionCollection(reader);
                    while (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            results.TotalRecords = reader.Get<int>(00);
                        }
                    }
                }
            }
            return results;
        }

        public override void ClearMessage(int days, int rows, bool ClearNoRead, JobDataClearMode mode)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (mode == JobDataClearMode.ClearByDay)
                {
                    query.CommandText = "DELETE FROM bx_ChatMessages WHERE CreateDate < DATEADD(day, 0 - @Days, GETDATE())" + (ClearNoRead ? "" : " AND IsRead = 1;");
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }
                else if (mode == JobDataClearMode.ClearByRows)
                {
                    query.CommandText = "DELETE FROM bx_ChatMessages WHERE  MessageID < ( SELECT MIN(MessageID) FROM(SELECT TOP (@Rows) MessageID FROM bx_ChatMessages ORDER BY MessageID DESC) t)" + (ClearNoRead ? "" : " AND IsRead = 1;");
                    query.CreateTopParameter("@Rows", rows);
                }
                else if (mode == JobDataClearMode.CombinMode)
                {
                    query.CommandText = "DELETE FROM bx_ChatMessages WHERE  MessageID < ( SELECT MIN(MessageID) FROM(SELECT TOP (@Rows) MessageID FROM bx_ChatMessages ORDER BY MessageID DESC) t) AND CreateDate < DATEADD(day, @Days, GETDATE())" + (ClearNoRead ? "" : " AND IsRead = 1;");
                    query.CreateTopParameter("@Rows", rows);
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }

                query.CreateParameter<bool>("@ClearNoRead", ClearNoRead, SqlDbType.Bit);
                query.ExecuteNonQuery();

            }
        }

        public override ChatSessionCollection AdminGetSessions(ChatSessionFilter filter, int pageNumber, IEnumerable<Guid> excludeRoleIds)
        {
            ChatSessionCollection sessions;
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRoleCondition = DaoUtil.GetExcludeRoleSQL("UserID", excludeRoleIds, query);
                StringBuffer buffer = new StringBuffer();

                if (filter.UserID != null)
                {
                    buffer += " AND UserID = @UserID";
                    query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
                }

                if (!string.IsNullOrEmpty(filter.Username))
                {
                    buffer += " AND UserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%'+@Username+'%' ) OR  TargetUserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%'+@Username+'%' )";
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }
                //if (!string.IsNullOrEmpty(filter.TargetUsername))
                //{
                //    buffer += " AND TargetUserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%'+@TargetUser+'%' )";
                //    query.CreateParameter<string>("@TargetUser", filter.TargetUsername, SqlDbType.NVarChar, 50);
                //}

                //if (!string.IsNullOrEmpty(filter.Contains))
                //{
                //    buffer +=" AND ChatSessionID IN( SELECT  ) "
                //}

                if (filter.BeginDate != null)
                {
                    buffer += " AND CreateDate >= @BeginDate";
                    query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }
                if (filter.EndDate != null)
                {
                    buffer += " AND UpdateDate <= @EndDate";
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                if (!string.IsNullOrEmpty(excludeRoleCondition))
                {
                    buffer += " AND " + excludeRoleCondition;
                    excludeRoleCondition = DaoUtil.GetExcludeRoleSQL("TargetUserID", excludeRoleIds, query);
                    buffer += " AND " + excludeRoleCondition;
                }

                if (buffer.Length > 0)
                    buffer.Remove(0, 5);

                //query.CommandText = "SELECT * FROM bx_ChatSessions";
                query.Pager.TableName = "bx_ChatSessions";
                query.Pager.PrimaryKey = "ChatSessionID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SortField = "ChatSessionID";
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = buffer.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    sessions = new ChatSessionCollection(reader);
                    while (reader.NextResult())
                        if (reader.Read())
                            sessions.TotalRecords = reader.GetInt32(0);
                }
            }
            return sessions;
        }

        [StoredProcedure(Name = "bx_Chat_GetChatSession", Script = @"
CREATE PROCEDURE {name}
      @UserID         int
     ,@TargetUserID   int
AS BEGIN

    SET NOCOUNT ON;

    SELECT * FROM bx_ChatSessions WHERE UserID = @UserID AND TargetUserID = @TargetUserID;

END")]
        public override ChatSession GetChatSession(int userID, int targetUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Chat_GetChatSession";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new ChatSession(reader);
                }
            }

            return null;
        }


        public override bool IgnoreAllMessage(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_ChatMessages SET IsRead = 1 WHERE UserID = @UserID AND IsRead = 0;";
                query.CreateParameter<int>("@UserID", userID,SqlDbType.Int);
                query.ExecuteNonQuery();
                return true;
            }
        }

        public override void IgnoreSession(int userID, int targetUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_ChatMessages SET IsRead = 1 WHERE UserID = @UserID AND TargetUserID = @TargetUserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        public override void AdminDeleteMessage(IEnumerable<int> messageIDs, IEnumerable<Guid> excludeRoles)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRoleUserIds = DaoUtil.GetExcludeRoleSQL("UserID", excludeRoles, query);
                query.CommandText = "DELETE FROM bx_ChatMessages WHERE MessageID IN( @IDs)";

                if (!string.IsNullOrEmpty(excludeRoleUserIds))
                    query.CommandText += " AND " + excludeRoleUserIds;

                query.CreateInParameter<int>("@IDs", messageIDs);
                query.ExecuteNonQuery();
            }
        }

        public override ChatSession GetChatSession(int sessionID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = " SELECT * FROM bx_ChatSessions WHERE ChatSessionID = @SessionID";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@SessionID", sessionID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new ChatSession(reader);
                }
            }

            return null;
        }

        public override ChatSessionCollection GetChatSessionsWithUnreadMessages(int userID, int topCount)
        {
            ChatSessionCollection result;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT TOP (@TopCount) * FROM bx_ChatSessions WHERE UserID = @UserID AND UnreadMessages > 0 ORDER BY UpdateDate DESC";


                query.CreateTopParameter("@TopCount", topCount);

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    result = new ChatSessionCollection(reader);
                }
            }

            return result;
        }


        public override ChatMessageCollection GetChatMessages(int userID, int targetUserID, int pageNumber, int pageSize, bool updateIsReaded)
        {
            ChatMessageCollection result;

            using (SqlQuery query = new SqlQuery())
            {
                if (pageNumber != 0)
                {
                    query.Pager.BeforeExecuteDealcre.Add("@TotalMessages", SqlDbType.Int);
                    query.Pager.BeforeExecute = @"SELECT @TotalMessages = TotalMessages FROM bx_ChatSessions WHERE UserID = @UserID AND TargetUserID = @TargetUserID;";
                    query.Pager.TableName = "bx_ChatMessages";
                    query.Pager.SortField = "MessageID";
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = pageSize;
                    query.Pager.IsDesc = true;
                    query.Pager.SelectCount = true;
                    query.Pager.TotalRecordsVariable = "@TotalMessages";
                    query.Pager.Condition = "UserID = @UserID AND TargetUserID = @TargetUserID";

                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                    query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);

                    if (updateIsReaded)
                    {
                        query.Pager.AfterExecute = @"
     UPDATE bx_ChatMessages SET IsRead = 1 WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND IsRead = 0;
                    ";
                    }
                }
                else
                {
                    query.CommandText = "SELECT Top " + pageSize + " * FROM bx_ChatMessages WHERE UserID = @UserID AND TargetUserID = @TargetUserID ORDER BY MessageID Desc;";
                    query.CommandText += "SELECT COUNT(*) FROM bx_ChatMessages WHERE UserID = @UserID AND TargetUserID = @TargetUserID;";
                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                    query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    result = new ChatMessageCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            result.TotalRecords = reader.Get<int>(0);
                    }
                }

                return result;
            }
        }

        [StoredProcedure(Name = "bx_Chat_SendMessage", Script = @"
CREATE PROCEDURE {name}
      @UserID         int
     ,@TargetUserID   int
     ,@Content        nvarchar(3000)
     ,@CreateIP       varchar(50)
     ,@GetNewMessages bit
     ,@LastMessageID  int
AS BEGIN

    SET NOCOUNT ON;

    DECLARE @NewMessageID int;

    INSERT INTO
        [bx_ChatMessages](
             [UserID]
            ,[TargetUserID]
            ,[IsReceive]
            ,[IsRead]
            ,[Content]
            ,[CreateIP]
        ) VALUES (
             @UserID
            ,@TargetUserID
            ,0
            ,1
            ,@Content
            ,@CreateIP
        );

    SELECT @NewMessageID = @@IDENTITY;

    INSERT INTO
        [bx_ChatMessages](
             [UserID]
            ,[TargetUserID]
            ,[IsReceive]
            ,[IsRead]
            ,[FromMessageID]
            ,[Content]
            ,[CreateIP]
        ) VALUES (
             @TargetUserID
            ,@UserID
            ,1
            ,0
            ,@NewMessageID
            ,@Content
            ,@CreateIP
        );

    DECLARE @Now datetime;
    SET @Now = GETDATE();

    UPDATE [bx_ChatSessions] SET TotalMessages = TotalMessages + 1, LastMessage = @Content, UpdateDate = @Now WHERE UserID = @UserID AND TargetUserID = @TargetUserID;
    IF @@ROWCOUNT = 0
        INSERT INTO [bx_ChatSessions] (UserID, TargetUserID, TotalMessages, LastMessage, CreateDate) VALUES (@UserID, @TargetUserID, 1, @Content, @Now);

    UPDATE [bx_ChatSessions] SET TotalMessages = TotalMessages + 1, UnreadMessages = UnreadMessages + 1, LastMessage = @Content, UpdateDate = @Now WHERE UserID = @TargetUserID AND TargetUserID = @UserID;
    IF @@ROWCOUNT = 0
        INSERT INTO [bx_ChatSessions] (UserID, TargetUserID, TotalMessages, UnreadMessages, LastMessage, CreateDate) VALUES (@TargetUserID, @UserID, 1, 1, @Content, @Now);

    UPDATE [bx_UserVars] SET UnreadMessages = UnreadMessages + 1 WHERE UserID = @TargetUserID;

    IF @GetNewMessages = 1
        EXEC bx_Chat_GetLastMessages @UserID, @TargetUserID, @LastMessageID;

END")]
        public override ChatMessageCollection SendMessage(int userID, int targetUserID, string content, string ip, bool getNewMessages, int lastMessageID)
        {
            ChatMessageCollection messages = new ChatMessageCollection();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Chat_SendMessage";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 3000);
                query.CreateParameter<string>("@CreateIP", ip, SqlDbType.VarChar, 50);

                query.CreateParameter<bool>("@GetNewMessages", getNewMessages, SqlDbType.Bit);
                query.CreateParameter<int>("@LastMessageID", lastMessageID, SqlDbType.Int);

                if (getNewMessages)
                {
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                            messages.Insert(0, new ChatMessage(reader));
                    }
                }
                else
                    query.ExecuteNonQuery();
            }

            return messages;
        }

        public override bool DeleteChatSessions(int userID, IEnumerable<int> targetUserIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DELETE bx_ChatSessions WHERE UserID = @UserID AND TargetUserID IN (@TargetUserIds);
DELETE bx_ChatMessages WHERE UserID = @UserID AND TargetUserID IN (@TargetUserIds);";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@TargetUserIds", targetUserIds);

                query.ExecuteNonQuery();
            }

            return true;
        }

        public override bool DeleteChatMessages(int userID, IEnumerable<int> messageIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE bx_ChatMessages WHERE UserID = @UserID AND MessageID IN (@MessageIds)";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@MessageIds", messageIds);

                query.ExecuteNonQuery();
            }

            return true;
        }

        public override RevertableCollection<ChatMessage> GetChatMessageWithReverters(IEnumerable<int> messageIDs)
        {
            if (ValidateUtil.HasItems(messageIDs) == false)
                return null;

            RevertableCollection<ChatMessage> messages = new RevertableCollection<ChatMessage>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	[ContentReverter] = ISNULL(R.[ContentReverter], '')
FROM 
	[bx_ChatMessages] A WITH(NOLOCK)
LEFT JOIN 
	bx_ChatMessageReverters R WITH(NOLOCK) ON R.[MessageID] = A.[MessageID]
WHERE 
	A.MessageID IN (@MessageIDs)";

                query.CreateInParameter<int>("@MessageIDs", messageIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string contentReverter = reader.Get<string>("ContentReverter");

                        ChatMessage message = new ChatMessage(reader);

                        messages.Add(message, contentReverter);
                    }
                }
            }

            return messages;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Chat_UpdateMessageContentKeywords", Script = @"
CREATE PROCEDURE {name}
    @MessageID                int,
    @KeywordVersion           varchar(32),
    @Content                  ntext,
    @ContentReverter          ntext
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = MessageID
    {PrimaryKeyParam} = @MessageID

    {Table} = bx_ChatMessages
    {Text} = Content
    {TextParam} = @Content

    {RevertersTable} = bx_ChatMessageReverters
    {TextReverter} = ContentReverter
    {TextReverterParam} = @ContentReverter
    
*/

END")]
        #endregion
        public override void UpdateChatMessageKeywords(RevertableCollection<ChatMessage> processlist)
        {
            string procedure = "bx_Chat_UpdateMessageContentKeywords";
            string table = "bx_ChatMessages";
            string primaryKey = "MessageID";

            SqlDbType text_Type = SqlDbType.NText;
            SqlDbType reve_Type = SqlDbType.NText;

            if (processlist == null || processlist.Count == 0)
                return;

            //有一部分项是不需要更新文本的（例如：只有版本或恢复信息发生了变化），把这部分的ID取出来，一次性更新以提高性能
            List<int> needUpdateButTextNotChangedIds = processlist.GetNeedUpdateButTextNotChangedKeys();

            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer sql = new StringBuffer();

                //前面取出的可以一次性更新版本而无需更新文本的部分项，在此批量更新
                if (needUpdateButTextNotChangedIds.Count > 0)
                {
                    sql += @"UPDATE " + table + " SET KeywordVersion = @NewVersion WHERE " + primaryKey + " IN (@NeedUpdateButTextNotChangedIds);";

                    query.CreateParameter<string>("@NewVersion", processlist.Version, SqlDbType.VarChar, 32);
                    query.CreateInParameter("@NeedUpdateButTextNotChangedIds", needUpdateButTextNotChangedIds);
                }

                int i = 0;
                foreach (Revertable<ChatMessage> item in processlist)
                {
                    //此项确实需要更新，且不只是版本发生了变化
                    if (item.NeedUpdate && item.OnlyVersionChanged == false)
                    {

                        sql.InnerBuilder.AppendFormat(@"EXEC {1} @ID_{0}, @KeywordVersion_{0}, @Text_{0}, @TextReverter_{0};", i, procedure);

                        query.CreateParameter<int>("@ID_" + i, item.Value.GetKey(), SqlDbType.Int);

                        //如果文字发生了变化，更新
                        if (item.TextChanged)
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, item.Value.KeywordVersion, SqlDbType.VarChar, 32);
                            query.CreateParameter<string>("@Text_" + i, item.Value.Text, text_Type);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);
                            query.CreateParameter<string>("@Text_" + i, null, text_Type);
                        }

                        //如果恢复信息发生了变化，更新
                        if (item.ReverterChanged)
                            query.CreateParameter<string>("@TextReverter_" + i, item.Reverter, reve_Type);
                        else
                            query.CreateParameter<string>("@TextReverter_" + i, null, reve_Type);

                        i++;

                    }

                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
        }
    }
}