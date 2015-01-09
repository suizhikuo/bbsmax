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
using System.Data;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class PointLogDao : DataAccess.PointLogDao
    {
        public override Entities.PointLogTypeCollection GetPointLogTypes()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_PointLogTypes";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PointLogTypeCollection(reader);
                }
            }
        }

        public override void GetPointStatInfo(PointLogFilter filter, int pointIndex, out int count, out int produceCount, out int consumeCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sb = new StringBuilder();
                string sql1 = string.Format("SELECT SUM(Point{0}) FROM bx_PointLogs WHERE  Point{0} > 0 ", pointIndex);
                string sql2 = string.Format("SELECT SUM(Point{0}) FROM bx_PointLogs WHERE  Point{0} < 0 ", pointIndex);
                string sql0 = string.Format("SELECT SUM(Point_{0}) FROM bx_Users WHERE Point_{0}>0", pointIndex + 1);

                if (!string.IsNullOrEmpty(filter.Username))
                {
                    sb.Append(" AND UserID IN( SELECT UserID FROM bx_Users WHERE Username = @Username )");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                    sql2 += " AND Username = @Username;";
                }

                if (filter.OperateID != null)
                {
                    sb.Append(" AND OperateID = @OperateID");
                    query.CreateParameter<int>("@OperateID", filter.OperateID.Value, SqlDbType.Int);
                }

                if (filter.BeginDate != null)
                {
                    sb.Append(" AND CreateTime >= @BeginDate");
                    query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    sb.Append(" AND CreateTime <= @EndDate");
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                sql1 += sb;
                sql2 += sb;
                string sql = string.Format("{0};{1};{2};", sql0, sql1, sql2);

                query.CommandText = sql;
                count = 0;
                produceCount = 0;
                consumeCount = 0;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Next)
                        count = reader.Get<int>(0);
                    if (reader.NextResult())
                    {
                        while (reader.Next)
                            produceCount = reader.Get<int>(0);

                        if (reader.NextResult())
                            while (reader.Next)
                                consumeCount = reader.Get<int>(0);
                    }
                }
            }
        }

        public override PointLogCollection GetPointLogs(PointLogFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(filter.Username))
                {
                    sb.Append(" AND UserID IN( SELECT UserID FROM bx_Users WHERE Username = @Username )");
                    query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
                }

                if (filter.OperateID != null)
                {
                    sb.Append(" AND OperateID = @OperateID");
                    query.CreateParameter<int>("@OperateID", filter.OperateID.Value, SqlDbType.Int);
                }

                if (filter.BeginDate != null)
                {
                    sb.Append(" AND CreateTime >= @BeginDate");
                    query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
                }

                if (filter.EndDate != null)
                {
                    sb.Append(" AND CreateTime <= @EndDate");
                    query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
                }

                if (sb.Length > 0)
                    sb.Remove(0, 4);

                query.Pager.TableName = "bx_PointLogs";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PrimaryKey = "LogID";
                query.Pager.SortField = "LogID";
                query.Pager.IsDesc = true;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;
                query.Pager.Condition = sb.ToString();
                

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PointLogCollection logs = new PointLogCollection(reader);

                    if (reader.NextResult())
                        while (reader.Next)
                            logs.TotalRecords = reader.Get<int>(0);
                    return logs;
                }
            }
        }


        public override void ClearPointLogs(int days, int rows, JobDataClearMode mode)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (mode == JobDataClearMode.ClearByDay)
                {
                    query.CommandText = "DELETE FROM bx_PointLogs WHERE CreateTime < DATEADD(day, 0 - @Days, GETDATE())";
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }
                else if (mode == JobDataClearMode.ClearByRows)
                {
                    query.CommandText = "DELETE FROM bx_PointLogs WHERE  LogID < ( SELECT MIN(LogID) FROM(SELECT TOP (@Rows) LogID FROM bx_PointLogs ORDER BY LogID DESC) t)";
                    query.CreateTopParameter("@Rows", rows);
                }
                else if (mode == JobDataClearMode.CombinMode)
                {
                    query.CommandText = "DELETE FROM bx_PointLogs WHERE  LogID < ( SELECT MIN(LogID) FROM(SELECT TOP (@Rows) LogID FROM bx_PointLogs ORDER BY LogID DESC) t) AND CreateTime < DATEADD(day, @Days, GETDATE())";
                    query.CreateTopParameter("@Rows", rows);
                    query.CreateParameter<int>("@Days", days, SqlDbType.Int);
                }
                query.ExecuteNonQuery();
            }
        }


#if DEBUG

        [StoredProcedure(Name = "bx_CreatePointLogs", Script = @"
CREATE PROCEDURE {name}
@UserID             int
,@Point0            int
,@Point1            int
,@Point2            int
,@Point3            int
,@Point4            int
,@Point5            int
,@Point6            int
,@Point7            int
,@Current0          int 
,@Current1          int 	
,@Current2          int  
,@Current3          int  
,@Current4          int  
,@Current5          int  
,@Current6          int  
,@Current7          int  
,@Operate           nvarchar(50)
,@Remarks           nvarchar(200)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @LogTypeID int;

IF EXISTS( SELECT * FROM bx_PointLogTypes WHERE  OperateName = @Operate)
 SET @LogTypeID = (SELECT OperateID FROM bx_PointLogTypes WHERE OperateName = @Operate);
ELSE BEGIN
 INSERT INTO bx_PointLogTypes(OperateName) VALUES(@Operate);   
 SET @LogTypeID = @@IDENTITY;
END

IF @Point0 IS NULL
    SET @Point0 = 0;
IF @Point1 IS NULL
    SET @Point1 = 0;
IF @Point2 IS NULL
    SET @Point2 = 0;
IF @Point3 IS NULL
    SET @Point3 = 0;
IF @Point4 IS NULL
    SET @Point4 = 0;
IF @Point5 IS NULL
    SET @Point5 = 0;
IF @Point6 IS NULL
    SET @Point6 = 0;
IF @Point7 IS NULL
    SET @Point7 = 0;

IF @Current0 IS NULL
    SET @Current0 = 0;
IF @Current1 IS NULL
    SET @Current1 = 0;
IF @Current2 IS NULL
    SET @Current2 = 0;
IF @Current3 IS NULL
    SET @Current3 = 0;
IF @Current4 IS NULL
    SET @Current4 = 0;
IF @Current5 IS NULL
    SET @Current5 = 0;
IF @Current6 IS NULL
    SET @Current6 = 0;
IF @Current7 IS NULL
    SET @Current7 = 0;

INSERT INTO bx_PointLogs(UserID, Point0, Point1, Point2, Point3, Point4, Point5, Point6, Point7, 
Current0 ,Current1,Current2 ,Current3 ,Current4 ,Current5 ,Current6 ,Current7 ,  
OperateID, Remarks)
VALUES( @UserID, @Point0, @Point1, @Point2, @Point3, @Point4, @Point5, @Point6, @Point7
,@Current0 ,@Current1,@Current2 ,@Current3 ,@Current4 ,@Current5 ,@Current6 ,@Current7 
, @LogTypeID, @Remarks );
END
")]
        public void CreatePointLog(int userID, int[] pointValues, string operateType, string remarks)
        {

        }
#endif
    }
}