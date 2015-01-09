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
using System.Data;
using System.Data.SqlClient;

using MaxLabs.bbsMax.DataAccess;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class InviteDao : DataAccess.InviteDao
    {

        #region 存储过程 检查用户制定时间段内的邀请码购买数量是否超出

        [StoredProcedure(Name = "bx_CheckOverFlowBuyCount", Script = @"
CREATE PROCEDURE {name}
    @UserID            int,
    @BeginTime         datetime,
    @EndTime           datetime,
    @BuyCount          int
AS BEGIN

    SET NOCOUNT ON;

    IF (SELECT COUNT(*) FROM bx_InviteSerials WHERE UserID = @UserID AND CreateDate BETWEEN @BeginTime AND @EndTime)>=@BuyCount
        SELECT 1;
    ELSE 
        SELECT 0;
END
")]
        #endregion
        public override bool CheckOverFlowBuyCount(int userID, InviteBuyInterval interval, int canBuyCount)
        {

            if (interval == InviteBuyInterval.Disable) return false;

            using (SqlQuery query = new SqlQuery())
            {
                DateTime beginTime = DateTimeUtil.Now, endTime = DateTimeUtil.Now;

                switch (interval)
                {
                    case InviteBuyInterval.ByDay:
                        beginTime = DateTimeUtil.Now.AddDays(-1);
                        break;
                    case InviteBuyInterval.ByHour:
                        beginTime = DateTimeUtil.Now.AddHours(-1);
                        break;
                    case InviteBuyInterval.ByMonth:
                        beginTime = DateTimeUtil.Now.AddMonths(-1);
                        break;
                    case InviteBuyInterval.ByYear:
                        beginTime = DateTimeUtil.Now.AddYears(-1);
                        break;
                }

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@BeginTime", beginTime, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndTime", endTime, SqlDbType.DateTime);
                query.CreateParameter<int>("@BuyCount", canBuyCount, SqlDbType.Int);

                query.CommandText = "bx_CheckOverFlowBuyCount";
                query.CommandType = CommandType.StoredProcedure;

                object result = null;
                result = query.ExecuteScalar();

                if (result == null) return false;

                if (Convert.ToInt32(result) == 0)
                    return false;
                else
                    return true;
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_BuyInviteSerial", Script = @"
CREATE PROCEDURE {name} 
    @UserID        int,
    @BuyCount      int,
    @ExpiresDate   datetime,
    @Remark        varchar(200)
AS BEGIN
    SET NOCOUNT ON;
    DECLARE @i int;
    SET @i = 0;
    WHILE @i < @BuyCount
    BEGIN
        INSERT INTO bx_InviteSerials (UserID, ExpiresDate, Status,Remark) VALUES (@UserID, @ExpiresDate, 0,@Remark);
        SET @i = @i + 1;
    END
END

")]
        #endregion
        public override void BuyInviteSerial(int userID, int buyCount, DateTime ExpiresDate,string remark)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "[bx_BuyInviteSerial]";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@BuyCount", buyCount, SqlDbType.Int);
                query.CreateParameter<DateTime>("@ExpiresDate", ExpiresDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@Remark", remark, SqlDbType.NVarChar, 200);
                query.ExecuteNonQuery();
            }
        }

        public override int GetBuyCountOfTimeSpan(int userID, InviteBuyInterval interval)
        {
            if (interval == InviteBuyInterval.Disable) return -1;

            using (SqlQuery query = new SqlQuery())
            {
                DateTime beginTime = DateTimeUtil.Now, endTime = DateTimeUtil.Now;

                switch (interval)
                {
                    case InviteBuyInterval.ByDay:
                        beginTime = DateTimeUtil.Now.AddDays(-1);
                        break;
                    case InviteBuyInterval.ByHour:
                        beginTime = DateTimeUtil.Now.AddHours(-1);
                        break;
                    case InviteBuyInterval.ByMonth:
                        beginTime = DateTimeUtil.Now.AddMonths(-1);
                        break;
                    case InviteBuyInterval.ByYear:
                        beginTime = DateTimeUtil.Now.AddYears(-1);
                        break;
                }



                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@BeginTime", beginTime, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndTime", endTime, SqlDbType.DateTime);

                query.CommandText = "SELECT COUNT(*) FROM bx_InviteSerials WHERE UserID = @UserID AND CreateDate BETWEEN @BeginTime AND @EndTime";


                object result = null;
                result = query.ExecuteScalar();

                if (result == null) return 0;

                return Convert.ToInt32(result);

            }
        }

        /// <summary>
        /// 过滤器到条件字符
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private string BuildCondition(InviteSerialFilter filter, SqlQuery query)
        {
            StringBuilder sb = new StringBuilder();
            if (filter.SearchField != null)
            {
                switch (filter.SearchField.Value)
                {
                    case InviteSerialSearchField.Serial:
                        sb.Append(" AND Serial LIKE '%'+@Serial+'%'");
                        query.CreateParameter<string>("@Serial", filter.SearchKey, SqlDbType.VarChar, 50);
                        break;

                    case InviteSerialSearchField.ToEmail:
                        if (!string.IsNullOrEmpty(filter.SearchKey))
                        {
                            sb.Append(" AND ToEmail LIKE '%'+@ToEmail+'%'");
                            query.CreateParameter<string>("@ToEmail", filter.SearchKey, SqlDbType.VarChar, 200);
                        }
                        break;

                    case InviteSerialSearchField.UserID:
                        int userId = 0;
                        int.TryParse(filter.SearchKey, out userId);
                        sb.Append(" AND UserID = @UserID");
                        query.CreateParameter<int>("@UserID", userId, SqlDbType.Int);
                        break;
                    case InviteSerialSearchField.Username:
                        if (!string.IsNullOrEmpty(filter.SearchKey))
                        {
                            sb.Append(" AND UserID = (SELECT [UserID] FROM bx_Users WHERE Username = @Username)");
                            query.CreateParameter<string>("@Username", filter.SearchKey, SqlDbType.NVarChar, 50);
                        }
                        break;

                    case InviteSerialSearchField.ToUsername:
                        if (!string.IsNullOrEmpty(filter.SearchKey))
                        {
                            sb.Append(" AND ToUserID = (SELECT [UserID] FROM bx_Users WHERE Username = @Username)");
                            query.CreateParameter<string>("@Username", filter.SearchKey, SqlDbType.NVarChar, 50);
                        }
                        break;
                }
            }

            if (false == string.IsNullOrEmpty(filter.Username))
            {
                sb.Append(" AND UserID IN (SELECT UserID From bx_Users WHERE Username LIKE '%'+@Username+'%')");
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
            }

            if (filter.CreateDateBegin != null)
            {
                sb.Append(" AND CreateDate >= @CreateDate1");
                query.CreateParameter<DateTime>("@CreateDate1", filter.CreateDateBegin.Value, SqlDbType.DateTime);
            }

            if (filter.CreateDateEnd != null)
            {
                sb.Append(" AND CreateDate <= @CreateDate2");
                query.CreateParameter<DateTime>("@CreateDate2", filter.CreateDateEnd.Value, SqlDbType.DateTime);
            }

            if (filter.ExpiresDateBegin != null)
            {
                sb.Append(" AND ExpiresDate >= @ExpiresDate1");
                query.CreateParameter<DateTime>("@ExpiresDate1", filter.ExpiresDateBegin.Value, SqlDbType.DateTime);
            }

            if (filter.ExpiresDateEnd != null)
            {
                sb.Append(" AND ExpiresDate <= @ExpiresDate2");
                query.CreateParameter<DateTime>("@ExpiresDate2", filter.ExpiresDateEnd.Value, SqlDbType.DateTime);
            }

            if (filter.Status != null && filter.Status != InviteSerialStatus.All && filter.Status != InviteSerialStatus.Expires)
            {
                sb.Append(" AND [Status] = @Status");
                query.CreateParameter<byte>("@Status", (byte)filter.Status.Value, SqlDbType.TinyInt);
            }
            else if (filter.Status == InviteSerialStatus.Expires)
            {
                sb.Append(" AND Status <> 1 AND ExpiresDate <= GETDATE()");
            }

            if (sb.Length > 0)
                sb.Remove(0, 5);

            return sb.ToString();
        }

        /// <summary>
        /// 根据过滤器删除
        /// </summary>
        /// <param name="filter"></param>
        public override void DeleteByFilter(InviteSerialFilter filter)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string condition = BuildCondition(filter, query);
                query.CommandText = "DELETE FROM [bx_InviteSerials] " + (string.IsNullOrEmpty(condition) ? ";" : "WHERE " + condition);
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_UserInviteSerialStatInfo", Script = @"
CREATE PROCEDURE {name} 
@UserID      int 
AS
BEGIN
SET NOCOUNT ON;
SELECT * FROM bx_SerialCounter WHERE UserID = @UserID;
END
")]
        #endregion
        public override InviteSerialStat GetStat(int userID)
        {
            UpdateExpiresSerialStatus();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UserInviteSerialStatInfo";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new InviteSerialStat(reader);
                }
            }
            InviteSerialStat stat = new InviteSerialStat();
            stat.UserID = userID;
            return stat;
        }

        public override InviteSerialStatCollection GetStatList(InviteSerialStatus state, int pageSize, int pageNumber, out int rowCount)
        {
            UpdateExpiresSerialStatus();
            using (SqlQuery query = new SqlQuery())
            {
                string orderField = "[TotalSerial]";

                switch (state)
                {
                    case InviteSerialStatus.Used:
                        orderField = "[Used]";
                        break;
                    case InviteSerialStatus.Expires:
                        orderField = "Expiress";
                        break;
                    case InviteSerialStatus.Unused:
                        orderField = "Unused";
                        break;
                }

                query.Pager.TableName = "bx_SerialCounter";
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1;
                query.Pager.PageSize = pageSize > 0 ? pageSize : 20;
                query.Pager.SelectCount = true;
                query.Pager.SortField = orderField;
                query.Pager.PrimaryKey = "[UserID]";

                rowCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    InviteSerialStatCollection stats = new InviteSerialStatCollection(reader);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            rowCount = reader.GetInt32(0);
                    }
                    return stats;
                }
            }
        }

        public override void UpdateInviteSerialEmailAndStatus(int userID, InviteSerialCollection serials)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sbSql = new StringBuilder();

                int i = 0;

                if (serials != null)
                {
                    foreach (InviteSerial serial in serials)
                    {
                        sbSql.Append(@"UPDATE [bx_InviteSerials] SET ToEmail = @Email").Append(i);

                        query.CreateParameter<string>("@Email" + i, serial.ToEmail, SqlDbType.VarChar, 200);

                        sbSql.Append(" ,Status = @Status").Append(i);

                        query.CreateParameter<byte>("@Status" + i, (byte)serial.Status, SqlDbType.TinyInt);

                        sbSql.Append(" ,ExpiresDate = @ExpiresDate").Append(i);

                        query.CreateParameter<DateTime>("@ExpiresDate" + i, serial.ExpiresDate, SqlDbType.DateTime);

                        sbSql.Append(@" WHERE Serial = @Serial").Append(i).Append(" ; ");

                        query.CreateParameter<Guid>("@Serial" + i, serial.Serial, SqlDbType.UniqueIdentifier);

                        i++;
                    }

                    query.CommandText = sbSql.ToString();
                    query.ExecuteNonQuery();
                }

            }
        }

        public override InviteSerialCollection GetInviteSerials(int operatorUserID, InviteSerialStatus status, string filter, int pageNumber, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer Condition = new StringBuffer();

                Condition += " AND UserID = @UserID";
                query.CreateParameter<int>("@UserID", operatorUserID, SqlDbType.Int);

                if (status != InviteSerialStatus.All)
                {

                    if (status != InviteSerialStatus.Expires)
                    {
                        Condition += " AND [Status] = @Status";
                        query.CreateParameter<byte>("@Status", (byte)status, SqlDbType.TinyInt);
                    }

                    else if (status == InviteSerialStatus.Expires)
                    {
                        Condition += " AND Status <> 1 AND ExpiresDate <= GETDATE()";
                    }
                }

                if (string.IsNullOrEmpty(filter) == false)
                {
                    Condition += " AND (Serial LIKE '%'+ @word +'%' OR ToUserID IN( SELECT UserID FROM bx_Users WHERE Username LIKE '%'+ @word +'%' OR Realname  LIKE '%'+ @word +'%' ))";
                    query.CreateParameter<string>("@word", filter, SqlDbType.NVarChar, 50);
                }

                if (Condition.Length > 0)
                    Condition.Remove(0, 5);

                query.Pager.SortField = "CreateDate";
                query.Pager.IsDesc = true;
                query.Pager.TableName = "[bx_InviteSerials]";
                query.Pager.SelectCount = true;
                query.Pager.PageSize = 20;
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1;
                query.Pager.Condition = Condition.ToString();
                query.Pager.PrimaryKey = "[ID]";

                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    InviteSerialCollection Serials = new InviteSerialCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalCount = reader.GetInt32(0);
                            Serials.TotalRecords = totalCount;
                        }
                    }
                    return Serials;
                }
            }

        }

        #region 存储过程
        [StoredProcedure(Name = "bx_CreateInviteSerials", Script = @"
CREATE PROCEDURE {name}
     @AddNum       int
    ,@UserIDs      varchar(8000)
    ,@ExpiresDate  datetime
    ,@Remark       varchar(200)
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @i int;
    SET @i = 0;
    WHILE @i < @AddNum BEGIN

        INSERT INTO [bx_InviteSerials](UserID,ExpiresDate,Remark)
            SELECT item, @ExpiresDate,@Remark FROM bx_GetIntTable(@UserIDs, ',');
        SET @i=@i+1;
    END
END")]
        #endregion
        public override void CreateInviteSerials(IEnumerable<int> userIDs, int addNum, DateTime expiresDate,string remark)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateInviteSerials";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@AddNum", addNum, SqlDbType.Int);
                query.CreateParameter<string>("@UserIDs", StringUtil.Join(userIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<DateTime>("@ExpiresDate", expiresDate, SqlDbType.DateTime);
                query.CreateParameter<string>("@Remark", remark, SqlDbType.NVarChar, 200);


                query.ExecuteNonQuery();
            }
        }

        public override void DeleteInviteSerials(IEnumerable<int> serialIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM [bx_InviteSerials] WHERE [ID] IN (@SerialIDs)";

                query.CreateInParameter<int>("@SerialIDs", serialIDs);

                query.ExecuteNonQuery();
            }
        }

        #region 获取单个邀请码
        #region 存储过程
        [StoredProcedure(Name = "bx_GetInviteSerialByToEmail", Script = @"
CREATE PROCEDURE {name}
    @ToEmail     nvarchar(200)
AS BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM [bx_InviteSerials] WHERE ToEmail = @ToEmail;
END")]
        #endregion
        public override InviteSerial GetInviteSerial(string toEmail)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetInviteSerialByToEmail";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@ToEmail", toEmail, SqlDbType.NVarChar, 200);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new InviteSerial(reader);
                }
            }
            return null;
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_GetInviteSerialByToUserID", Script = @"
CREATE PROCEDURE {name}
    @ToUserID     int
AS BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 * FROM [bx_InviteSerials] WHERE ToUserID=@ToUserID;
END")]
        #endregion
        public override InviteSerial GetInviteSerial(int toUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetInviteSerialByToUserID";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@ToUserID", toUserID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new InviteSerial(reader);
                }
            }
            return null;
        }

        #region 存储过程
        [StoredProcedure(Name = "bx_GetInviteSerialBySerial", Script = @"
CREATE PROCEDURE {name}
    @Serial     uniqueidentifier
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_InviteSerials] WHERE Serial = @Serial;
END")]
        #endregion
        public override InviteSerial GetInviteSerial(Guid serial)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetInviteSerialBySerial";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<Guid>("@Serial", serial, SqlDbType.UniqueIdentifier);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new InviteSerial(reader);
                }
            }
            return null;
        }
        #endregion
        public override InviteSerialCollection GetInviteSerials(int? ownerUserId, InviteSerialFilter filter, int pageNumber, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string Condition = BuildCondition(filter, query);
                if (ownerUserId != null)
                {
                    Condition += " AND UserID = @OwnerID";
                    query.CreateParameter<int>("@OwnerID", ownerUserId.Value, SqlDbType.Int);
                }

               


                if (filter.Pagesize < 1) filter.Pagesize = 20;


                query.Pager.SortField = filter.Order.Value.ToString();
                query.Pager.IsDesc = filter.IsDesc.Value;
                query.Pager.TableName = "[bx_InviteSerials]";
                query.Pager.SelectCount = true;
                query.Pager.PageSize = filter.Pagesize;
                query.Pager.PageNumber = pageNumber > 0 ? pageNumber : 1;
                query.Pager.Condition = Condition;
                query.Pager.PrimaryKey = "[ID]";

                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    InviteSerialCollection Serials = new InviteSerialCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalCount = reader.GetInt32(0);
                            Serials.TotalRecords = totalCount;
                        }
                    }
                    return Serials;
                }
            }
        }

        /// <summary>
        /// 通过用户ID,查看邀请关系
        /// </summary>
        #region 存储过程
        [StoredProcedure(Name = "bx_GetInviteRelation", Script = @"
CREATE PROCEDURE {name}
    @UserID     int
AS BEGIN
    SET NOCOUNT ON;

    DECLARE @tempID int,@Serial uniqueidentifier;
    DECLARE @T table(Serial uniqueidentifier)
    WHILE @UserID>0
    BEGIN
	    SET @tempID=0
	    SELECT TOP 1 @tempID=UserID,@Serial=Serial FROM bx_InviteSerials WHERE ToUserID=@UserID
	    SET @UserID=@tempID
	    IF(@UserID>0)
		    INSERT INTO @T SELECT @Serial
    END
    SELECT * FROM bx_InviteSerials WHERE [Serial] IN(SELECT Serial FROM @T)
END")]
        #endregion
        public override InviteSerialCollection GetInviteRelation(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetInviteRelation";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new InviteSerialCollection(reader);
                }
            }
        }

        private void UpdateExpiresSerialStatus()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_InviteSerials SET Status = 2 WHERE Status <> 1 AND ExpiresDate <= GETDATE()";
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_SetUserInviteSerial", Script = @"
CREATE PROCEDURE {name}
 @Serial           uniqueidentifier
,@UserID           int
,@InviteID         int
AS
BEGIN

    SET NOCOUNT ON;

    IF @Serial IS NOT NULL AND @Serial <> '00000000-0000-0000-0000-000000000000'  BEGIN
        UPDATE bx_InviteSerials SET ToUserID = @UserID, [Status] = 1 WHERE Serial = @Serial
        SELECT @InviteID = UserID FROM bx_InviteSerials WHERE Serial = @Serial
    END

    --互加好友
    IF @InviteID <> @UserID  BEGIN
        UPDATE bx_UserInfos SET InviterID = @InviteID WHERE [UserID] = @UserID
        UPDATE bx_Users SET [TotalInvite] = [TotalInvite] + 1 WHERE [UserID] = @InviteID
        IF NOT EXISTS(SELECT * FROM bx_Friends WHERE UserID = @UserID AND FriendUserID = @InviteID)
	        INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
	        VALUES(@UserID,@InviteID,0,0,GETDATE());

        IF NOT EXISTS(SELECT * FROM bx_Friends WHERE UserID = @InviteID AND FriendUserID = @UserID)
	        INSERT INTO bx_Friends([UserID],[FriendUserID],[GroupID],[Hot],[CreateDate]) 
	        VALUES(@InviteID,@UserID,0,0,GETDATE());

    END

END
")]
        public override void SetUserInviteSerial(int userId, int inviterID, Guid serial)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetUserInviteSerial";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userId, SqlDbType.Int);
                query.CreateParameter<Guid>("@Serial", serial, SqlDbType.UniqueIdentifier);
                query.CreateParameter<int>("@InviteID", inviterID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }
    }
}