//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;


using System.Data.SqlClient;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class NotifyDao : DataAccess.NotifyDao
    {
        /// <summary>
        /// 根据通知ID获取用户ID
        /// </summary>
        /// <param name="notifyID">通知ID</param>
        #region Stored Procedure
        [StoredProcedure(Name = "bx_GetUserIDByNotifyID", Script = @"
CREATE PROCEDURE {name}
    @NotifyID        int
AS BEGIN
    SET NOCOUNT ON;

    SELECT [UserID] FROM [bx_Notify] WHERE [NotifyID] = @NotifyID;
END")]
        #endregion
        public override int GetUserIDByNotifyID(int notifyID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserIDByNotifyID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@NotifyID", notifyID, SqlDbType.Int);

                return query.ExecuteScalar<int>();
            }
        }


        [StoredProcedure(Name = "bx_RegisterNotifyType", Script = @"
CREATE PROCEDURE {name}
@TypeName      nvarchar(50)
,@Keep         bit
,@Description  nvarchar(200)
AS
BEGIN
SET NOCOUNT ON;

IF NOT  EXISTS( SELECT * FROM bx_NotifyTypes WHERE TypeName = @TypeName) BEGIN
INSERT INTO bx_NotifyTypes( TypeName, [Keep], Description ) VALUES( @TypeName ,@Keep ,@Description );
END

SELECT * FROM bx_NotifyTypes WHERE TypeName = @TypeName;

END
")]
        public override bool RegisterNotifyType(string notifyType, bool keep, string description, out NotifyType type)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_RegisterNotifyType";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<string>("@TypeName", notifyType, SqlDbType.NVarChar, 50);
                query.CreateParameter<bool>("@Keep", keep, SqlDbType.Bit);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 200);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    type = null;
                    while (reader.Next)
                        type = new NotifyType(reader);

                    return true;
                }
            }
        }

        //        #region 系统通知


        public override void SetSystemNotifyReadUserIDs(int notifyID, string userIDsString)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_SystemNotifies SET ReadUserIDs = @IDs WHERE NotifyID = @NotifyID";
                query.CreateParameter<string>("@IDs", userIDsString, SqlDbType.Text);
                query.CreateParameter<int>("@NotifyID", notifyID, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
        }

        public override SystemNotifyCollection GetSystemNotifies()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = " SELECT * FROM bx_SystemNotifies WHERE EndDate> GETDATE() ORDER BY NotifyID DESC";
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SystemNotifyCollection notifys = new SystemNotifyCollection(reader);
                    return notifys;
                }
            }
        }

        public override SystemNotify GetSystemNotify(int notifyID)
        {
            SystemNotify notify = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_SystemNotifies WHERE EndDate< GETDATE(); SELECT * FROM bx_SystemNotifies WHERE NotifyID = @NotifyID;";
                query.CreateParameter<int>("@NotifyID", notifyID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                        notify = new SystemNotify(reader);
                    return notify;
                }
            }
        }

        public override void DeleteSystemNotifys(IEnumerable<int> notifyIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_SystemNotifies WHERE NotifyID IN ( @NotifyIDs )";
                query.CreateInParameter<int>("@NotifyIDs", notifyIDs);
                query.ExecuteNonQuery();
            }
        }

        public override SystemNotify UpdateSystemNotify(int notifyID, string subject, string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate, int dispatcherID, string dispatcherIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"Update bx_SystemNotifies SET Subject = @Subject,  BeginDate = @BeginDate, EndDate = @EndDate, ReceiveRoles = @ReceiveRoles , ReceiveUserIDs = @ReceiveUserIDs, Content = @Content, DispatcherID = @DispatcherID, DispatcherIP = @DispatcherIP WHERE NotifyID = @NotifyID ;
SELECT * FROM bx_SystemNotifies WHERE NotifyID = @NotifyID;
";
                query.CreateParameter<string>("@Subject", subject, SqlDbType.NVarChar, 200);
                query.CreateParameter<int>("@NotifyID", notifyID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@ReceiveRoles", StringUtil.Join(receiveRoles, ","), SqlDbType.Text);
                query.CreateParameter<string>("@ReceiveUserIDs", StringUtil.Join(receiveUserIDs, ","), SqlDbType.VarChar, 2000);
                query.CreateParameter<string>("@Content", Content, SqlDbType.NVarChar, 2000);
                query.CreateParameter<int>("@DispatcherID", dispatcherID, SqlDbType.Int);
                query.CreateParameter<string>("@DispatcherIP", dispatcherIP, SqlDbType.VarChar, 200);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SystemNotify result = null;

                    while (reader.Next)
                        result = new SystemNotify(reader);

                    return result;
                }
            }
        }

        public override SystemNotify CreateSystemNotify(string subject, string Content, IEnumerable<Guid> receiveRoles, IEnumerable<int> receiveUserIDs, DateTime beginDate, DateTime endDate, int dispatcherID, string dispatcherIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"INSERT INTO bx_SystemNotifies(Subject, BeginDate, EndDate, ReceiveRoles, ReceiveUserIDs, Content, DispatcherID, DispatcherIP) VALUES( @Subject, @BeginDate, @EndDate,@ReceiveRoles, @ReceiveUserIDs, @Content, @DispatcherID, @DispatcherIP);
        SELECT * FROM bx_SystemNotifies WHERE NotifyID = @@IDENTITY;
";
                query.CreateParameter<string>("@Subject", subject, SqlDbType.NVarChar, 200);
                query.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@ReceiveRoles", StringUtil.Join(receiveRoles, ","), SqlDbType.Text);
                query.CreateParameter<string>("@ReceiveUserIDs", StringUtil.Join(receiveUserIDs, ","), SqlDbType.VarChar, 2000);
                query.CreateParameter<string>("@Content", Content, SqlDbType.NVarChar, 2000);
                query.CreateParameter<int>("@DispatcherID", dispatcherID, SqlDbType.Int);
                query.CreateParameter<string>("@DispatcherIP", dispatcherIP, SqlDbType.VarChar, 200);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SystemNotify result = null;

                    while (reader.Next)
                        result = new SystemNotify(reader);

                    return result;
                }
            }
        }


        /// <summary>
        /// 高级搜索
        /// </summary>
        public override NotifyCollection AdminGetNotifiesBySearch(AdminNotifyFilter notifyFilter, int pageNumber, IEnumerable<Guid> excludeRoleIds)
        {

            using (SqlQuery query = new SqlQuery())
            {

                StringBuilder condition = FilterToCondition(query, notifyFilter);

                string exlcludeUserIDs = DaoUtil.GetExcludeRoleSQL("UserID", excludeRoleIds, query);
                if (!string.IsNullOrEmpty(exlcludeUserIDs))
                {
                    condition.Append(" AND " + exlcludeUserIDs);
                }

                query.Pager.IsDesc = true;
                query.Pager.TableName = "[bx_Notify]";
                query.Pager.SortField = "[NotifyID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = notifyFilter.PageSize;
                query.Pager.SelectCount = true;
                query.Pager.Condition = condition.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    NotifyCollection notifies = new NotifyCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            notifies.TotalRecords = reader.Get<int>(0);
                        }
                    }
                    return notifies;
                }

            }
        }

        /// <summary>
        /// 自定清除N天以前的通知
        /// </summary>
        /// <param name="days"></param>
        public override void ClearNotify(int days, int rows, JobDataClearMode clearMode)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (clearMode == JobDataClearMode.ClearByDay)
                {
                    string Sql = "DELETE FROM bx_Notify WHERE DateDiff(day,UpdateDate,Getdate())>= @Days ";
                    query.CommandText = Sql;
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }
                else if (clearMode == JobDataClearMode.ClearByRows)
                {
                    query.CommandText = "DELETE FROM bx_Notify WHERE NotifyID NOT IN( SELECT TOP " + rows + " NotifyID FROM bx_Notify ORDER BY NotifyID DESC)";
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }
                else if (clearMode == JobDataClearMode.CombinMode)
                {
                    string Sql = "DELETE FROM bx_Notify WHERE  NotifyID NOT IN( SELECT TOP " + rows + " NotifyID FROM bx_Notify ORDER BY NotifyID DESC) AND DateDiff(day,UpdateDate,Getdate())>= @Days ";
                    query.CommandText = Sql;
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }

                query.ExecuteNonQuery();
            }
        }

        public override NotifyCollection GetTopNotifys(int userID, int typeID, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {

                query.CommandText = "SELECT TOP (@TopCount) * FROM [bx_Notify] WHERE UserID = @UserID AND IsRead = 0 ORDER BY NotifyID DESC";
                if (typeID != 0)
                {
                    query.Pager.Condition += " AND [TypeID] = @TypeID";
                    query.CreateParameter<int>("@TypeID", typeID, SqlDbType.TinyInt);
                }
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.CreateTopParameter("@TopCount", count);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new NotifyCollection(reader);
                }
            }
        }

        /// <summary>
        /// 获取用户的某条通知
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="notifyID">消息ID</param>
        #region Stored Procedure
        [StoredProcedure(Name = "bx_GetNotify", Script = @"
CREATE PROCEDURE {name}
    @UserID          int
   ,@NotifyID        int
   ,@SetRead         bit
AS BEGIN
    SET NOCOUNT ON;

    IF @SetRead = 1 BEGIN

        IF @UserID IS NULL
            UPDATE [bx_Notify] SET [IsRead] = 1 WHERE [NotifyID] = @NotifyID;
        ELSE
            UPDATE [bx_Notify] SET [IsRead] = 1 WHERE [UserID] = @UserID AND [NotifyID] = @NotifyID;

    END;

    IF @UserID IS NULL
        SELECT * FROM [bx_Notify] WHERE [NotifyID] = @NotifyID;
    ELSE
        SELECT * FROM [bx_Notify] WHERE [UserID] = @UserID AND [NotifyID] = @NotifyID;

END")]
        #endregion
        public override T GetNotify<T>(int? userID, int notifyID, bool isSetRead)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetNotify";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int?>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@NotifyID", notifyID, SqlDbType.Int);
                query.CreateParameter<bool>("@SetRead", isSetRead, SqlDbType.Bit);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        T result = new T();
                        result.ParseFromWrap(reader);

                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取指定用户指定类型的所有通知
        /// </summary>
        /// <param name="notifyType">指定类型</param>
        /// <param name="userID">指定的用户的ID</param>
        /// <returns>指定用户指定类型的所有通知集合</returns>
        public override NotifyCollection GetNotifiesByType(int userID, int type, int pageSize, int pageNumber, ref int? count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.TableName = "[bx_Notify]";
                query.Pager.SortField = "[NotifyID]";
                query.Pager.PrimaryKey = "[NotifyID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = count;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "1 = 1 AND [UserID] = @UserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


                if (type != 0)
                {
                    query.Pager.Condition += " AND [TypeID] = @TypeID";
                    query.CreateParameter<int>("@TypeID", type, SqlDbType.Int);
                }


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    NotifyCollection notifies = new NotifyCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            notifies.TotalRecords = reader.Get<int>(0);
                            count = notifies.TotalRecords;
                        }
                    }

                    return notifies;
                }
            }
        }

        /// <summary>
        /// 获取指定用户/所有用户的所有通知
        /// </summary>
        /// <param name="userID">指定用户的ID,可以为空,为空则为要获取所有用户</param>
        /// <returns>返回指定用户的所有通知集合</returns>
        public override NotifyCollection GetNotifies(int? userID, int pageSize, int pageNumber, ref int? count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.TableName = "[bx_Notify]";
                query.Pager.SortField = "[NotifyID]";
                query.Pager.PrimaryKey = "[NotifyID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = count;
                query.Pager.SelectCount = true;
                if (userID != null)
                {
                    query.Pager.Condition = @"[UserID] = @UserID";
                }

                query.CreateParameter<int?>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    NotifyCollection notifies = new NotifyCollection(reader);

                    if (count == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            count = reader.Get<int>(0);
                        }
                    }
                    return notifies;
                }
            }
        }

        /// <summary>
        /// 获取几个通知
        /// </summary>
        public override NotifyCollection GetNotifies(IEnumerable<int> notifyIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_Notify] WHERE [NotifyID] IN (@NotifyIDs);";

                query.CreateInParameter<int>("@NotifyIDS", notifyIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new NotifyCollection(reader);
                }
            }
        }

        /// <summary>
        /// 高级搜索
        /// </summary>
        public override NotifyCollection GetNotifiesBySearch(AdminNotifyFilter notifyFilter, int pageNumber)
        {

            using (SqlQuery query = new SqlQuery())
            {

                StringBuilder condition = FilterToCondition(query, notifyFilter);

                query.Pager.IsDesc = true;
                query.Pager.TableName = "[bx_Notify]";
                query.Pager.SortField = "[NotifyID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = notifyFilter.PageSize;
                query.Pager.SelectCount = true;
                query.Pager.Condition = condition.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    NotifyCollection notifies = new NotifyCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            notifies.TotalRecords = reader.Get<int>(0);
                        }
                    }
                    return notifies;
                }

            }
        }

        /// <summary>
        /// 新通知
        /// </summary>
        /// <param name="notifyType">通知类型</param>
        /// <param name="userID">始终表示这个通知的拥有者的ID</param>
        /// <param name="relatedUserID">比如加好友,这里指的就是加我为好友的用户ID; 比如留言或回复,指的就是给我留言或回复的用户ID...</param>
        /// <param name="senderIP">发送者的IP</param>
        /// <param name="content">内容</param>
        /// <param name="parameters">额外参数,如果加好友,要带好友分组信息</param>
        #region Stored Procedure
        [StoredProcedure(Name = "bx_AddNotify", Script = @"
CREATE PROCEDURE {name}
    @TypeID          int
   ,@UserID          int
   ,@Content         nvarchar(1000)
   ,@Keyword         varchar(200)
   ,@NotifyDatas     ntext
   ,@ClientID        int
   ,@Actions         nvarchar(2000)
AS BEGIN
    SET NOCOUNT ON;

DECLARE @ExistNotifyID int;
DECLARE @UniteNotify     bit
SET @ExistNotifyID = ( SELECT TOP 1 NotifyID FROM bx_Notify WHERE [TypeID] = @TypeID AND [UserID] = @UserID AND Keyword = @Keyword);-- AND [Parameters] = @Parameters );

IF @ExistNotifyID IS NOT NULL BEGIN
UPDATE bx_Notify SET Content = @Content,NotifyDatas = @NotifyDatas, UpdateDate = GETDATE(), IsRead = 0 WHERE [TypeID] = @TypeID AND [UserID] = @UserID AND Keyword = @Keyword;
END
ELSE BEGIN
INSERT INTO bx_Notify(
     UserID
    ,TypeID
    ,Content
    ,Keyword
    ,NotifyDatas
    ,ClientID
    ,Actions
) VALUES(
     @UserID
    ,@TypeID
    ,@Content
    ,@Keyword
    ,@NotifyDatas
    ,@ClientID
    ,@Actions
);
END

SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;

END")]
        #endregion
        public override bool AddNotify(int userID, int typeID, string Content, string keyword, string datas,int clientID,string actions, out UnreadNotifies unreadNotifies)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AddNotify";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@TypeID", typeID, SqlDbType.TinyInt);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Keyword", keyword, SqlDbType.VarChar, 1000);
                query.CreateParameter<string>("@Content", Content, SqlDbType.NVarChar, 1000);
                query.CreateParameter<string>("@NotifyDatas", datas, SqlDbType.NText);
                query.CreateParameter<int>("@ClientID", clientID, SqlDbType.Int);
                query.CreateParameter<string>("@Actions",actions,SqlDbType.NVarChar,2000);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    unreadNotifies = new UnreadNotifies(reader);
                }
            }

            return true;
        }


        /// <summary>
        /// 删除通知
        /// </summary>
        /// <param name="userID">通知的用户ID</param>
        /// <param name="messageID">要删除的通知的ID</param>
        public override bool DeleteNotify(int? userID, int notifyID, out UnreadNotifies unreadNotifies)
        {
            unreadNotifies = null;

            IEnumerable<int> notifies = new int[] { notifyID };
            UnreadNotifyCollection unreads;
            bool success = DeleteNotifies(userID, notifies, out  unreads);

            foreach (UnreadNotifies un in unreads)
                unreadNotifies = un;

            return success;

        }

        /// <summary>
        /// 删除多个通知
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="notifyIDs">要删除的通知的ID集,不允许为空</param>

        [StoredProcedure(Name = "bx_DeleteNotifies",Script= @"
CREATE PROCEDURE {name}
@UserID    int
,@NotifyIDs  varchar(8000)
AS 
BEGIN
SET NOCOUNT ON;
DECLARE @NotifyIDTable table(NotifyID int);

INSERT @NotifyIDTable SELECT item FROM bx_GetIntTable(@NotifyIDs,',');

IF @UserID IS NULL BEGIN
    DECLARE @TempUserIDs table( userid int);
    INSERT @TempUserIDs SELECT DISTINCT UserID FROM bx_Notify WHERE [NotifyID] IN ( SELECT NotifyID FROM  @NotifyIDTable);
    DELETE FROM [bx_Notify] WHERE [NotifyID] IN ( SELECT NotifyID FROM  @NotifyIDTable);
    SELECT * FROM bx_UnreadNotifies WHERE UserID IN (SELECT userid FROM @TempUserIDs);
END
ELSE BEGIN
    DELETE FROM [bx_Notify] WHERE UserID = @UserID AND [NotifyID] IN ( SELECT NotifyID FROM  @NotifyIDTable); 
    SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;
END

END
")]
        public override bool DeleteNotifies(int? userID, IEnumerable<int> notifyIDs, out UnreadNotifyCollection unreadNotifies)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"bx_DeleteNotifies";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int?>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@NotifyIDs", StringUtil.Join( notifyIDs), SqlDbType.VarChar,8000);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    unreadNotifies = new UnreadNotifyCollection(reader);
                }
            }

            return true;
        }

        private StringBuilder FilterToCondition(SqlQuery query, AdminNotifyFilter notifyFilter)
        {
            #region 组合SQL语句

            StringBuilder condition = new StringBuilder("1=1");

            if (notifyFilter.NotifyType != null)
            {
                condition.Append(" AND [TypeID] = @TypeID ");
                query.CreateParameter<int?>("@TypeID", notifyFilter.NotifyType, SqlDbType.Int);
            }

            if (notifyFilter.IsRead != null)
            {
                condition.Append(" AND [IsRead] = @IsRead ");
                query.CreateParameter<bool?>("@IsRead", notifyFilter.IsRead, SqlDbType.Bit);
            }

            if (notifyFilter.BeginDate != null)
            {
                condition.Append(" AND [UpdateDate] >= @BeginDate ");
                query.CreateParameter<DateTime?>("@BeginDate", notifyFilter.BeginDate, SqlDbType.DateTime);
            }

            if (notifyFilter.EndDate != null)
            {
                condition.Append(" AND [UpdateDate] <= @EndDate ");
                query.CreateParameter<DateTime?>("@EndDate", notifyFilter.EndDate, SqlDbType.DateTime);
            }

            if (!string.IsNullOrEmpty(notifyFilter.Owner))
            {
                condition.Append(" AND [UserID] IN (SELECT [UserID] FROM [bx_Users] WHERE [Username] LIKE '%'+ @Owner + '%') ");
                query.CreateParameter<string>("@Owner", notifyFilter.Owner, SqlDbType.NVarChar, 50);
            }

            #endregion
            return condition;
        }


        [StoredProcedure(Name = "bx_LoadAllNotifyType", Script = @"
CREATE PROCEDURE {name} 
AS
BEGIN
SET NOCOUNT ON;
SELECT * FROM bx_NotifyTypes;
END
")]
        public override NotifyTypeCollection LoadAllNotifyType()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_LoadAllNotifyType";
                query.CommandType = CommandType.StoredProcedure;

                NotifyTypeCollection results;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    results = new NotifyTypeCollection(reader);
                }
                return results;
            }
        }

        /// <summary>
        /// 删除指定用户指定类型的多个通知
        /// </summary>
        /// <param name="userID">指定用户ID</param>
        /// <param name="notifyType">指定类型</param>
        #region Stored Procedure
        [StoredProcedure(Name = "bx_DeleteNotifysByType", Script = @"
CREATE PROCEDURE {name}
    @UserID             int
   ,@TypeID             tinyint
AS BEGIN
    SET NOCOUNT ON;

    DELETE FROM 
        [bx_Notify] 
    WHERE 
        (@UserID IS NULL OR [UserID] = @UserID) 
    AND 
        [TypeID] = @TypeID;
END")]
        #endregion
        public override bool DeleteNotifysByType(int? userID, int notifyType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteNotifysByType";

                query.CreateParameter<int?>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TypeID", notifyType, SqlDbType.Int);

                query.ExecuteNonQuery();
            }

            return true;
        }



        /// <summary>
        /// 删除符合指定条件的通知
        /// </summary>
        public override bool DeleteNotifiesBySearch(AdminNotifyFilter notifyFilter, Guid[] excludeRoleIds)
        {
            using (SqlQuery query = new SqlQuery())
            {

                #region 组合SQL语句

                StringBuilder builder = FilterToCondition(query, notifyFilter);


                string excludeRoleSql = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIds, query);

                if (string.IsNullOrEmpty(excludeRoleSql) == false)
                    builder.Append(" AND " + excludeRoleSql);

                #endregion

                query.CommandText = "DELETE FROM bx_Notify WHERE " + builder.ToString();

                query.ExecuteNonQuery();
            }

            return true;
        }


        [StoredProcedure(Name = "bx_IgnoreNotifies",Script= @"
CREATE PROCEDURE {name}
@UserID    int
,@NotifyIDs  varchar(8000)
AS 
BEGIN
SET NOCOUNT ON;

DECLARE @NotifyIDTable table(NotifyID int);
INSERT @NotifyIDTable SELECT item FROM bx_GetIntTable(@NotifyIDs,',');


DELETE FROM bx_Notify WHERE UserID = @UserID AND NotifyID IN( SELECT NotifyID FROM  @NotifyIDTable) AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] <> 1 );

UPDATE bx_Notify SET IsRead = 1 WHERE UserID = @UserID AND NotifyID IN( SELECT NotifyID FROM  @NotifyIDTable) AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] = 1 );

SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;
END
")]

        public override bool IgnoreNotify( int userID ,IEnumerable<int> notifyIDs ,out UnreadNotifies unreadNotifies )
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_IgnoreNotifies";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@NotifyIDs", StringUtil.Join(notifyIDs), SqlDbType.VarChar, 8000);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    unreadNotifies = new UnreadNotifies(reader);
                }
            }
            return true;
        }

        [StoredProcedure(Name = "bx_IgnoreNotifyByType", Script = @"
CREATE PROCEDURE {name}
@UserID    int
,@TypeID   int
AS 
BEGIN
SET NOCOUNT ON;

IF @TypeID <>0 BEGIN
DECLARE @Keep bit

SET @Keep = ( SELECT [Keep] FROM bx_NotifyTypes WHERE TypeID = @TypeID );

    IF @Keep <> 1 BEGIN
        DELETE FROM bx_Notify WHERE UserID = @UserID AND TypeID = @TypeID;
        RETURN;
    END
    ELSE BEGIN
        UPDATE bx_Notify SET IsRead = 1 WHERE UserID = @UserID AND TypeID = @TypeID;
        RETURN;
    END
END

UPDATE bx_Notify SET IsRead = 1 WHERE UserID = @UserID AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] = 1);
DELETE FROM bx_Notify WHERE UserID = @UserID AND TypeID IN( SELECT TypeID FROM bx_NotifyTypes WHERE [Keep] <> 1);

SELECT * FROM bx_UnreadNotifies WHERE UserID = @UserID;
END

")]

        public override bool IgnoreNotifyByType(int userID, int typeID, out UnreadNotifies unreads)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_IgnoreNotifyByType";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TypeID", typeID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    unreads = new UnreadNotifies(reader);
                }
            }

            return true;
        }
    }
}