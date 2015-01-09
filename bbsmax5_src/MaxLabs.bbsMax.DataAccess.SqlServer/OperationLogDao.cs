//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;

using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class OperationLogDao : Logs.OperationLogDao
    {
        [StoredProcedure(Name = "bx_OperationLog_Create", Script = @"
CREATE PROCEDURE {name}
	@OperatorID		int,
	@TargetID_1		int,
	@TargetID_2		int,
	@TargetID_3		int,
	@OperatorIP		varchar(50),
	@OperationType	varchar(100),
	@Message		nvarchar(1000),
	@CreateTime		datetime
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [bx_OperationLogs] (
		OperatorID, 
		TargetID_1, 
		TargetID_2, 
		TargetID_3, 
		OperatorIP,
		OperationType, 
		Message, 
		CreateTime
	) VALUES (
		@OperatorID, 
		@TargetID_1, 
		@TargetID_2, 
		@TargetID_3, 
		@OperatorIP, 
		@OperationType, 
		@Message, 
		@CreateTime
	);
END")]
        public override void CreateOperationLog(Operation op)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_OperationLog_Create";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@OperatorID", op.OperatorID, SqlDbType.Int);
                db.CreateParameter<int>("@TargetID_1", op.TargetID_1.GetValueOrDefault(0), SqlDbType.Int);
                db.CreateParameter<int>("@TargetID_2", op.TargetID_2.GetValueOrDefault(0), SqlDbType.Int);
                db.CreateParameter<int>("@TargetID_3", op.TargetID_3.GetValueOrDefault(0), SqlDbType.Int);
                db.CreateParameter<string>("@OperatorIP", op.OperatorIP, SqlDbType.VarChar, 50);
                db.CreateParameter<string>("@OperationType", op.TypeName, SqlDbType.VarChar, 100);
                db.CreateParameter<string>("@Message", op.Message, SqlDbType.NVarChar, 1000);
                db.CreateParameter<DateTime>("@CreateTime", op.CreateTime, SqlDbType.DateTime);

                db.ExecuteNonQuery();
            }
        }

        /*     [StoredProcedure(Name="bx_InsertBanUserLogs",Script= @"
     CREATE PROCEDURE {name}
         @OperationType varchar(50),
         @OperatorName  nvarchar(50),
         @Cause         nvarchar(1000),
         @ForumInfos    varchar(1000),
         @TargetID      int,
         @TargetName    nvarchar(200),
         @TargetIP      varchar(50)
     AS
     BEGIN
         SET NOCOUNT ON;
         INSERT INTO [bx_BanUserLogs](
             OperationType,
             OperatorName,
             Cause,
             ForumID,
             BeginDate,
             EndDate,
             TargetID,
             TargetName,
             TargetIP
         ) Values(
             @OperationType,
             @OperatorName,
             @Cause,
             @ForumID,
             @BeginDate,
             @EndDate,
             @TargetID,
             @TargetName,
             @TargetIP
         );
     END
     ")]

             public override void InsertBanUserOperationLog(BanUserOperation banuser)
             {
                 using (SqlQuery query = new SqlQuery())
                 {
                     query.CommandText = "bx_InsertBanUserLogs";
                     query.CommandType = CommandType.StoredProcedure;

                     query.CreateParameter<string>("@OperationType", banuser.OperationType, SqlDbType.VarChar, 50);
                     query.CreateParameter<string>("@OperationName", banuser.OperatorName, SqlDbType.NVarChar, 50);
                     query.CreateParameter<string>("@Cause", banuser.Cause, SqlDbType.NVarChar, 200);
                   //  query.CreateParameter<int>("@ForumID", banuser.ForumID, SqlDbType.Int);
                     query.CreateParameter<DateTime>("@BeginDate",banuser.BeginDate,SqlDbType.DateTime);
                     query.CreateParameter<DateTime>("@EndDate", banuser.EndDate, SqlDbType.DateTime);
                     query.CreateParameter<int>("@TargetID",banuser.UserID,SqlDbType.Int);
                     query.CreateParameter<string>("@TargetName",banuser.UserName,SqlDbType.NVarChar,50);
                     query.CreateParameter<string>("@TargetIP",banuser.UserIP,SqlDbType.VarChar,50);

                     query.ExecuteNonQuery();
                 }
             }
     */

        public override OperationLogCollection GetOperationLogsBySearch(int pageNumber, OperationLogFilter filter)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_OperationLogs";
                db.Pager.SortField = "LogID";
                db.Pager.PageSize = filter.PageSize;

                SqlConditionBuilder sb = new SqlConditionBuilder(SqlConditionStart.None);

                if (filter.OperatorID != null)
                {
                    sb.Append("OperatorID = @OperatorID");

                    db.CreateParameter<int>("@OperatorID", filter.OperatorID.Value, SqlDbType.Int);
                }
                else if (filter.OperatorName != null && filter.OperatorName != string.Empty)
                {
                    sb.Append("OperatorID IN (SELECT UserID FROM bx_Users WHERE Username = @OperatorName)");

                    db.CreateParameter<string>("@OperatorName", filter.OperatorName, SqlDbType.NVarChar, 50);
                }

                if (filter.OperatorIP != null && filter.OperatorIP != string.Empty)
                {
                    sb.Append("OperatorIP = @OperatorIP");

                    db.CreateParameter<string>("@OperatorIP", filter.OperatorIP, SqlDbType.VarChar, 50);
                }

                if (filter.OperationType != null && filter.OperationType != string.Empty)
                {
                    sb.Append("OperationType = @OperationType");

                    db.CreateParameter<string>("@OperationType", filter.OperationType, SqlDbType.VarChar, 100);
                }

                if (filter.TargetID_1 != null)
                {
                    sb.Append("TargetID_1 = @TargetID_1");

                    db.CreateParameter<int>("@TargetID_1", filter.TargetID_1.Value, SqlDbType.Int);
                }

                if (filter.TargetID_2 != null)
                {
                    sb.Append("TargetID_2 = @TargetID_2");

                    db.CreateParameter<int>("@TargetID_2", filter.TargetID_2.Value, SqlDbType.Int);
                }

                if (filter.TargetID_3 != null)
                {
                    sb.Append("TargetID_3 = @TargetID_3");

                    db.CreateParameter<int>("@TargetID_3", filter.TargetID_3.Value, SqlDbType.Int);
                }

                if (filter.BeginDate != null)
                {
                    sb.Append("CreateTime >= @BeginDate");

                    db.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    sb.Append("CreateTime <= @EndDate");

                    db.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                db.Pager.Condition = sb.ToString();
                db.Pager.PageNumber = pageNumber;
                db.Pager.SelectCount = true;

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    OperationLogCollection result = new OperationLogCollection(reader);

                    reader.NextResult();

                    reader.Read();

                    result.TotalRecords = reader.Get<int>(0);

                    return result;
                }
            }
        }

        public override void DeleteOperationLogs(JobDataClearMode mode, DateTime dateTime, int saveRows)
        {
            using (SqlQuery query = new SqlQuery())
            {
                switch (mode)
                {
                    case JobDataClearMode.ClearByDay:
                        query.CommandText = "DELETE FROM bx_OperationLogs WHERE CreateTime <= @Time;";

                        query.CreateParameter<DateTime>("@Time", dateTime, SqlDbType.DateTime);
                        break;

                    case JobDataClearMode.ClearByRows:
                        query.CommandText = "DELETE FROM bx_OperationLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_OperationLogs ORDER BY LogID DESC) AS O)";
                        query.CreateTopParameter("@TopCount", saveRows);
                        break;

                    case JobDataClearMode.CombinMode:
                        query.CommandText = "DELETE FROM bx_OperationLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_OperationLogs ORDER BY LogID DESC) AS O) AND CreateTime > @Time";
                        query.CreateTopParameter("@TopCount", saveRows);
                        query.CreateParameter<DateTime>("@Time", dateTime, SqlDbType.DateTime);
                        break;
                }

                query.ExecuteNonQuery();
            }
        }




        [StoredProcedure(Name = "bx_IPLogs_Create", Script = @"
CREATE PROCEDURE {name}
	@UserID		    int,
	@Username		nvarchar(50),
	@NewIP		    varchar(50),
	@CreateDate		datetime,
    @OldIP          varchar(50),
    @VisitUrl       varchar(200)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [bx_IPLogs] (
		UserID,
        Username,
        NewIP,
		CreateDate,
        OldIP,
        VisitUrl
	) VALUES (
		@UserID,
        @Username,
        @NewIP,
		@CreateDate,
        @OldIP,
        @VisitUrl
	);
END")]
        public override void LogUserIPChanged(UserIPLog log)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_IPLogs_Create";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", log.UserID, SqlDbType.Int);
                db.CreateParameter<string>("@Username", log.UserName, SqlDbType.NVarChar, 50);
                db.CreateParameter<string>("@NewIP", log.NewIP, SqlDbType.VarChar, 50);
                db.CreateParameter<DateTime>("@CreateDate", log.CreateDate, SqlDbType.DateTime);
                db.CreateParameter<string>("@OldIP", log.OldIP, SqlDbType.VarChar, 50);
                db.CreateParameter<string>("@VisitUrl", log.VisitUrl, SqlDbType.VarChar, 200);

                db.ExecuteNonQuery();
            }
        }

        public override UserIPLogCollection GetUserIPLogsBySearch(UserIPSearchFilter filter, int pagenumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder condition = new StringBuilder();
                string SortField = "CreateDate";

                if (filter != null)
                {

                    switch (filter.OrderBy)
                    {
                        case UserIPLogSortOrder.Username:
                            SortField = "Username";
                            break;
                        case UserIPLogSortOrder.NewIP:
                            SortField = "NewIP";
                            break;
                    }

                    if (!string.IsNullOrEmpty(filter.Username))
                    {
                        condition.Append(" AND Username = @Username");
                        query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                    }

                    if (filter.UserID != null)
                    {
                        condition.Append(" AND UserID=@UserID");
                        query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
                    }

                    if (!string.IsNullOrEmpty(filter.NewIP))
                    {
                        condition.Append(" AND NewIP=@NewIP");
                        query.CreateParameter<string>("@NewIP", filter.NewIP, SqlDbType.VarChar, 50);
                    }

                    if (filter.BeginDate != null)
                    {
                        condition.Append(" AND CreateDate>=@BeginDate");
                        query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                    }

                    if (filter.EndDate != null)
                    {
                        condition.Append(" AND CreateDate<=@EndDate");
                        query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                    }

                }

                query.Pager.SortField = SortField;
                query.Pager.TableName = "bx_IPLogs";
                query.Pager.PageSize = filter.PageSize;
                query.Pager.PageNumber = pagenumber;
                query.Pager.PrimaryKey = "LogID";
                query.Pager.IsDesc = filter != null ? filter.IsDesc : true;
                if (condition.Length > 0)
                    query.Pager.Condition = condition.ToString().Substring(4);
                query.Pager.SelectCount = true;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserIPLogCollection iploglist = new UserIPLogCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            iploglist.TotalRecords = reader.Get<int>(0);
                    }

                    return iploglist;

                }

            }
        }


        public override UserIPLogCollection GetUserIPLogsByIP(string IP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_IPLogs] LEFT JOIN bx_BannedUsers ON bx_BannedUsers.UserID=bx_IPLogs.UserID WHERE NewIP=@IP";
                query.CreateParameter<string>("@IP", IP, SqlDbType.VarChar, 50);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserIPLogCollection collection = new UserIPLogCollection(reader);
                    return collection;
                }
            }
        }

        public override UserIPLogCollection GetUserIPLogsByIP(string IP, int pageNumber, int pageSize, out int total)
        {
            total = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_IPLogs]";
                query.Pager.SortField = "[LogID]";
                query.Pager.Condition = " LogID in(SELECT Max(LogID) FROM [bx_IPLogs] WHERE NewIP = @IP GROUP BY UserID) ";

                query.Pager.AfterExecute = "SELECT * FROM bx_BannedUsers WHERE UserID in(SELECT UserID FROM [bx_IPLogs] WHERE NewIP = @IP GROUP BY UserID)";

                //query.CommandText = "SELECT * FROM [bx_IPLogs] LEFT JOIN bx_BannedUsers ON bx_BannedUsers.UserID=bx_IPLogs.UserID WHERE NewIP=@IP";
                query.CreateParameter<string>("@IP", IP, SqlDbType.VarChar, 50);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserIPLogCollection collection = new UserIPLogCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            total = reader.GetInt32(0);
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            int userID = reader.Get<int>("UserID");
                            int forumID = reader.Get<int>("ForumID");
                            foreach (UserIPLog log in collection)
                            {
                                if (log.UserID == userID)
                                {
                                    log.BannedForumID = forumID;
                                }
                            }
                        }
                    }
                    return collection;
                }
            }
        }

        public override void DeleteUserIPLog(JobDataClearMode mode, DateTime dateTime, int saveRows)
        {
            using (SqlQuery query = new SqlQuery())
            {
                switch (mode)
                {
                    case JobDataClearMode.ClearByDay:
                        query.CommandText = "DELETE FROM bx_IPLogs WHERE [CreateDate]<=@Time;";
                        query.CreateParameter<DateTime>("@Time", dateTime, SqlDbType.DateTime);
                        break;
                    case JobDataClearMode.ClearByRows:
                        query.CommandText = "DELETE FROM bx_IPLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_IPLogs ORDER BY LogID DESC) AS O)";
                        query.CreateTopParameter("@TopCount", saveRows);
                        break;
                    case JobDataClearMode.CombinMode:
                        query.CommandText = "DELETE FROM bx_IPLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_IPLogs ORDER BY LogID DESC) AS O) AND CreateDate > @Time";
                        query.CreateTopParameter("@TopCount", saveRows);
                        query.CreateParameter<DateTime>("@Time", dateTime, SqlDbType.DateTime);
                        break;
                }
                query.ExecuteNonQuery();
            }
        }

#if !Passport

        public override List<BanForumInfo> GetBanForumInfos(int banLogID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_BanUserLogForumInfos WHERE LogID = @LogID";
                query.CreateParameter<int>("@LogID", banLogID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    List<BanForumInfo> results = new List<BanForumInfo>();


                    while (reader.Next)
                    {
                        BanForumInfo foruminfo = new BanForumInfo(reader);
                        results.Add(foruminfo);
                    }

                    return results;
                }
            }
        }

        public override BanUserOperationCollection GetBanUserLogsByUserID(int userid)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT * FROM bx_BanUserLogs  WHERE UserID = @userid ORDER BY OperationTime DESC;
SELECT * FROM bx_BanUserLogForumInfos WHERE LogID IN(SELECT LogID FROM bx_BanUserLogs  WHERE UserID = @userid) ORDER BY LogID DESC;";
                query.CreateParameter<int>("@userid", userid, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    BanUserOperationCollection collection = new BanUserOperationCollection(reader);

                    BanUserOperation operation = null;
                    if(reader.NextResult())
                    {
                        while (reader.Next)
                        {
                            BanForumInfo foruminfo = new BanForumInfo(reader);
                            if (operation != null && foruminfo.ID == operation.ID)
                                operation.ForumInfoList.Add(foruminfo);
                            else
                            {
                                operation = collection.GetValue(foruminfo.ID);
                                operation.ForumInfoList.Add(foruminfo);
                            }
                        }
                    }
                    return collection;
                }
            }

        }

        public override BanUserOperationCollection GetBanUserLogsBySearch(BanUserLogFilter filter, int pagenumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder condition = new StringBuilder();

                string SortField = "OperationTime";

                if (filter.OrderBy != null)
                {
                    switch (filter.OrderBy)
                    {
                        case BanUserLogSortOrder.LogID:
                            SortField = "LogID";
                            break;
                        case BanUserLogSortOrder.OperationType:
                            SortField = "OperationType";
                            break;
                        case BanUserLogSortOrder.UserID:
                            SortField = "UserID";
                            break;
                        case BanUserLogSortOrder.UserName:
                            SortField = "UserName";
                            break;
                        default:
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(filter.Username))
                {
                    condition.Append(" AND Username=@Username");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (filter.UserID != null)
                {
                    condition.Append(" AND UserID=@UserID");
                    query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
                }

                if (filter.ForumID != null)
                {
                    condition.Append(" AND LogID IN( SELECT LogID FROM bx_BanUserLogForumInfos WHERE  ForumID=@ForumID)");
                    query.CreateParameter<int>("@ForumID", filter.ForumID.Value, SqlDbType.Int);
                }

                if (filter.OperationType != null)
                {
                    condition.Append(" AND OperationType=@OperationType");
                    query.CreateParameter<int>("@OperationType", (int)filter.OperationType.Value, SqlDbType.Int);
                }

                if (filter.BeginDate != null)
                {
                    condition.Append(" AND OperationTime>=@BeginDate");
                    query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    condition.Append(" AND OperationTime<=@EndDate");
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                query.Pager.SortField = SortField;
                query.Pager.PageNumber = pagenumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TableName = "bx_BanUserLogs";
                query.Pager.PrimaryKey = "LogID";
                query.Pager.IsDesc = filter != null ? filter.IsDesc : true;
                query.Pager.SelectCount = true;
                if (condition.Length > 0)
                {
                    query.Pager.Condition = condition.ToString().Substring(4);
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    BanUserOperationCollection collection = new BanUserOperationCollection(reader);
                    if (reader.NextResult())
                    {
                        while(reader.Next)
                        collection.TotalRecords = reader.Get<int>(0);
                    }
                    return collection;
                }
            }
        }

        public override void DeleteBanUserOperationLogs(JobDataClearMode mode, DateTime datetime, int saveRows)
        {
            using (SqlQuery query = new SqlQuery())
            {
                switch (mode)
                { 
                    case JobDataClearMode.ClearByDay:
                        query.CommandText = "DELETE FROM bx_BanUserLogs WHERE [OperationTime]<=@Time";
                        query.CreateParameter<DateTime>("@Time", datetime, SqlDbType.DateTime);
                        break;
                    case JobDataClearMode.ClearByRows:
                        query.CommandText = "DELETE FROM bx_BanUserLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_BanUserLogs ORDER BY LogID DESC) AS O)";
                        query.CreateTopParameter("@TopCount", saveRows);
                        break;
                    case JobDataClearMode.CombinMode:
                        query.CommandText = "DELETE FROM bx_BanUserLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_BanUserLogs ORDER BY LogID DESC) AS O) AND OperationTime >= @Time";
                        query.CreateTopParameter("@TopCount", saveRows);
                        query.CreateParameter<DateTime>("@Time", datetime, SqlDbType.DateTime);
                        break;
                }
                query.ExecuteNonQuery();
            }
        }
#endif
        public override UserMobileLogCollection GetUserMobileLogsBySearch(UserMobileSearchFilter filter, int pagenumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder condition = new StringBuilder();

                string SortField = "OperationDate";

                if (filter.OrderBy != null)
                {
                    switch (filter.OrderBy.Value)
                    { 
                        case UserMobileLogSortOrder.LogID:
                            SortField = "LogID";
                            break;
                        case UserMobileLogSortOrder.UserID:
                            SortField = "UserID";
                            break;
                        //case UserMobileLogSortOrder.Username:
                        //    SortField = "Username";
                        //    break;
                        default: break;
                    }
                }

                if (filter.UserID != null)
                {
                    condition.Append(" AND UserID=@UserID");
                    query.CreateParameter<int>("@UserID",filter.UserID.Value,SqlDbType.Int);
                }

                if (!string.IsNullOrEmpty(filter.Username))
                {
                    condition.Append(" AND Username=@Username");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (false==(filter.OperationType == null||(int)filter.OperationType==0))
                {
                    condition.Append(" AND OperationType=@OperationType");
                    query.CreateParameter<int>("@OperationType", (int)filter.OperationType, SqlDbType.SmallInt);
                }

                if (filter.BeginDate != null)
                {
                    condition.Append(" AND OperationDate>=@BeginDate");
                    query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    condition.Append(" AND OperationDate<=@EndDate");
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                if (filter.MobilePhone != null)
                {
                    condition.Append(" AND MobilePhone = @MobilePhone");
                    query.CreateParameter<long>("@MobilePhone", filter.MobilePhone.Value, SqlDbType.BigInt);
                }

                query.Pager.SortField = SortField;
                query.Pager.PageNumber = pagenumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TableName = "bx_UserMobileOperationLogs";
                query.Pager.PrimaryKey = "LogID";
                query.Pager.IsDesc = filter != null ? filter.IsDesc : true;
                query.Pager.SelectCount = true;
                if (condition.Length > 0)
                {
                    query.Pager.Condition = condition.ToString().Substring(4);
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserMobileLogCollection collection = new UserMobileLogCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Next)
                            collection.TotalRecords = reader.Get<int>(0);
                    }
                    return collection;
                }
            }
        }

        public override void DeleteUserMobileOperationLogs(JobDataClearMode mode, DateTime datetime, int saveRows)
        {
            using (SqlQuery query = new SqlQuery())
            {
                switch (mode)
                {
                    case JobDataClearMode.ClearByDay:
                        query.CommandText = "DELETE FROM bx_UserMobileOperationLogs WHERE [OperationDate]<=@Time";
                        query.CreateParameter<DateTime>("@Time", datetime, SqlDbType.DateTime);
                        break;
                    case JobDataClearMode.ClearByRows:
                        query.CommandText = "DELETE FROM bx_UserMobileOperationLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_UserMobileOperationLogs ORDER BY LogID DESC) AS O)";
                        query.CreateTopParameter("@TopCount", saveRows);
                        break;
                    case JobDataClearMode.CombinMode:
                        query.CommandText = "DELETE FROM bx_UserMobileOperationLogs WHERE LogID < (SELECT MIN(O.LogID) FROM (SELECT TOP(@TopCount) LogID FROM bx_UserMobileOperationLogs ORDER BY LogID DESC) AS O) AND [OperationDate] >= @Time";
                        query.CreateTopParameter("@TopCount", saveRows);
                        query.CreateParameter<DateTime>("@Time", datetime, SqlDbType.DateTime);
                        break;
                }
                query.ExecuteNonQuery();
            }
        }


        public override UserGetPropCollection GetUserGetPropCollection(UserGetPropFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder condition = new StringBuilder();

                string SordField;

                switch (filter.Order)
                { 
                    case UserGetPropFilter.OrderBy.CreateDate:
                        SordField = "CreateDate";
                        break;
                    case UserGetPropFilter.OrderBy.PropID:
                        SordField = "PropID";
                        break;
                    case UserGetPropFilter.OrderBy.UserID:
                        SordField = "UserID";
                        break;
                    default: 
                        SordField = ""; 
                        break;
                }

                if (filter.UserID != null)
                {
                    condition.Append(" AND UserID=@UserID");
                    query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
                }

                if (false == string.IsNullOrEmpty(filter.Username))
                {
                    condition.Append(" AND Username=@Username");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (filter.PropID != null)
                {
                    condition.Append(" AND PropID=@PropID");
                    query.CreateParameter<int>("@PropID", filter.PropID.Value, SqlDbType.Int);
                }

                if (filter.GetPropType != null)
                {
                    condition.Append(" AND GetPropType=@GetPropType");
                    query.CreateParameter<byte>("@GetPropType", (byte)filter.GetPropType.Value, SqlDbType.TinyInt);
                }

                if (filter.BeginDate != null)
                {
                    condition.Append(" AND OperationDate>=@BeginDate");
                    query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    condition.Append(" AND OperationDate<=@EndDate");
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                query.Pager.TableName = "bx_UserGetPropLogs";
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "LogID";
                query.Pager.SelectCount = true;
                query.Pager.SortField = SordField;

                if (condition.Length > 0)
                {
                    query.Pager.Condition = condition.ToString().Substring(4);
                }

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserGetPropCollection collection = new UserGetPropCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Next)
                        collection.TotalRecords = (int)reader[0];
                    }
                    return collection;
                }
            }
        }



    }
}