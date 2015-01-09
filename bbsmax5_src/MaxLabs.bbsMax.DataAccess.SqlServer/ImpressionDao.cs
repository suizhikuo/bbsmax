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
using System.Data;
using MaxLabs.bbsMax.Filters;
using System.Collections;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class ImpressionDao : DataAccess.ImpressionDao
    {
        public override ImpressionTypeCollection GetImpressionTypesForUse(int targetUserID, int usrCount, int sysCount)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = @"
DECLARE @TempTable table(TypeID int);

INSERT INTO @TempTable
SELECT TOP (@UsrCount1) TypeID
FROM bx_Impressions WHERE UserID = @TargetUserID ORDER BY UpdateDate DESC;

SELECT * FROM bx_ImpressionTypes C RIGHT JOIN @TempTable A ON C.TypeID = A.TypeID; 

DECLARE @Count2 AS int;
DECLARE @Count3 AS int;

SET @Count2 = @UsrCount - (SELECT COUNT(*) FROM @TempTable) + @SysCount;

IF @Count2 > 0 BEGIN

    EXEC(
    'SELECT TOP ' + @Count2 + ' *, (SELECT TOP 1 C.UpdateDate FROM bx_Impressions C WHERE C.TypeID = A.TypeID ORDER BY C.UpdateDate DESC) AS UpdateDate FROM bx_ImpressionTypes A WHERE A.TypeID IN(SELECT B.TypeID FROM bx_Impressions B WHERE B.UserID != ' + @TargetUserID + ') ORDER BY UpdateDate'
    );

END";

                db.CreateTopParameter("@UsrCount1", usrCount);
                db.CreateParameter<int>("@UsrCount", usrCount, SqlDbType.Int);
                db.CreateParameter<int>("@SysCount", sysCount, SqlDbType.Int);

                db.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    ImpressionTypeCollection result = new ImpressionTypeCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ImpressionType(reader));
                        }
                    }

                    return result;
                }
            }
        }

        [StoredProcedure(Name = "bx_Impression_GetLastRecord", Script = @"
CREATE PROCEDURE {name}
    @UserID        int,
    @TargetUserID  int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 *, B.Text, B.KeywordVersion FROM bx_ImpressionRecords A LEFT JOIN bx_ImpressionTypes B ON B.TypeID = A.TypeID WHERE UserID = @UserID AND TargetUserID = @TargetUserID ORDER BY RecordID DESC;
END")]
        public override ImpressionRecord GetLastImpressionRecord(int userID, int targetUserID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Impression_GetLastRecord";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    if (reader.Read())
                        return new ImpressionRecord(reader);

                    return null;
                }
            }
        }

        public override ImpressionRecordCollection GetTargetUserImpressionRecords(int targetUserID, int pageNumber, int pageSize, ref int? totalCount)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_ImpressionRecordsWithTypeInfo";
                db.Pager.Condition = "TargetUserID = @TargetUserID";
                db.Pager.SortField = "RecordID";
                db.Pager.PageNumber = pageNumber;
                db.Pager.PageSize = pageSize;
                db.Pager.TotalRecords = totalCount;
                db.Pager.SelectCount = true;

                db.CreateParameter<int>("@TargetUserID", targetUserID, System.Data.SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    ImpressionRecordCollection result = new ImpressionRecordCollection(reader);

                    if (reader.NextResult() && reader.Read())
                    {
                        totalCount = reader.Get<int>(0);
                        result.TotalRecords = totalCount.Value;
                    }

                    return result;
                }
            }
        }

        private string BuildCondition(SqlQuery query, AdminImpressionTypeFilter filter)
        {
            StringBuffer condition = new StringBuffer();

            if (string.IsNullOrEmpty(filter.SearchKey) == false)
            {
                condition += "[Text] LIKE '%' + @SearchKey + '%'";
                query.CreateParameter<string>("@SearchKey", filter.SearchKey, SqlDbType.NVarChar, 100);
            }

            return condition.ToString();
        }

        public override ImpressionTypeCollection GetImpressionTypesForAdmin(AdminImpressionTypeFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_ImpressionTypes]";

                if (filter.Order == AdminImpressionTypeFilter.OrderBy.TypeID)
                    query.Pager.SortField = "TypeID";
                else
                {
                    query.Pager.SortField = "RecordCount";

                    query.Pager.PrimaryKey = "TypeID";
                }
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = BuildCondition(query, filter);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ImpressionTypeCollection types = new ImpressionTypeCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            types.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return types;
                }
            }
        }

        private string BuildCondition(SqlQuery query, AdminImpressionRecordFilter filter)
        {
            StringBuffer condition = new StringBuffer();

            if (filter.TypeID != null)
            {
                condition += " AND [TypeID] = @TypeID";
                query.CreateParameter<int>("@TypeID", filter.TypeID.Value, SqlDbType.Int);
            }

            if (string.IsNullOrEmpty(filter.SearchKey) == false)
            {
                condition += " AND [TypeID] IN (SELECT [TypeID] FROM [bx_ImpressionTypes] WHERE [Text] LIKE '%' + @SearchKey + '%')";
                query.CreateParameter<string>("@SearchKey", filter.SearchKey, SqlDbType.NVarChar, 50);
            }


            if (filter.UserID != null)
            {
                condition += " AND [UserID] = @UserID";
                query.CreateParameter<int>("@UserID", filter.UserID.Value, SqlDbType.Int);
            }

            if (filter.TargetUserID != null)
            {
                condition += " AND [TargetUserID] = @TargetUserID";
                query.CreateParameter<int>("@TargetUserID", filter.TargetUserID.Value, SqlDbType.Int);
            }

            if (string.IsNullOrEmpty(filter.User) == false)
            {
                condition += " AND [UserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @Username)";
                query.CreateParameter<string>("@Username", filter.User, SqlDbType.NVarChar, 50);
            }

            if (string.IsNullOrEmpty(filter.TargetUser) == false)
            {
                condition += " AND [TargetUserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @TargetUserName)";
                query.CreateParameter<string>("@TargetUserName", filter.TargetUser, SqlDbType.NVarChar, 50);
            }

            if (filter.BeginDate != null)
            {
                condition += " AND  [CreateDate] >= @BeginDate";
                query.CreateParameter<DateTime>("@BeginDate", filter.EndDate.Value, SqlDbType.DateTime);
            }

            if (filter.EndDate != null)
            {
                condition += " AND [CreateDate] <= @EndDate";
                query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
            }


            if (condition.Length > 0)
            {
                condition.InnerBuilder.Remove(0, 5);
            }

            return condition.ToString();
        }

        public override ImpressionRecordCollection GetImpressionRecordsForAdmin(AdminImpressionRecordFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_ImpressionRecordsWithTypeInfo]";
                query.Pager.ResultFields = "*";
                query.Pager.SortField = "RecordID";
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = BuildCondition(query, filter);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ImpressionRecordCollection types = new ImpressionRecordCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            types.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return types;
                }
            }
        }

        [StoredProcedure(Name = "bx_Impression_Create", Script = @"
CREATE PROCEDURE {name}
    @UserID        int,
    @TargetUserID  int,
    @Text          nvarchar(100),
    @TimeLimit     int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now AS datetime;
    DECLARE @Ago AS datetime;

    SET @Now = GETDATE();
    SET @Ago = DATEADD(hh,-@TimeLimit,GETDATE());

    --判断是否在合法时间间隔内
    IF NOT EXISTS(SELECT * FROM bx_ImpressionRecords WHERE UserID = @UserID AND TargetUserID = @TargetUserID AND CreateDate >= @Ago AND CreateDate <= @Now)
    BEGIN
        DECLARE @TypeID AS int;

        SELECT @TypeID = TypeID FROM bx_ImpressionTypes WHERE Text = @Text;

        --判断是否是已有印象类型
        IF @TypeID IS NULL
        BEGIN
            INSERT INTO bx_ImpressionTypes (Text) VALUES (@Text);

            SELECT @TypeID = @@IDENTITY;
        END

        --记录用户印象评价的记录
        INSERT INTO bx_ImpressionRecords (UserID, TargetUserID, TypeID) VALUES (@UserID, @TargetUserID, @TypeID);

        --更新用户被印象评价的计数器
        IF NOT EXISTS(SELECT * FROM bx_Impressions WHERE UserID = @TargetUserID AND TypeID = @TypeID)
        BEGIN
            INSERT INTO bx_Impressions (UserID, TypeID) VALUES (@TargetUserID, @TypeID);
        END

        SELECT 1;
    END
    ELSE
    BEGIN
        SELECT 0;
    END
END")]
        public override bool CreateImpression(int userID, int targetUserID, string text, int timeLimit)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Impression_Create";
                db.CommandType = System.Data.CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, System.Data.SqlDbType.Int);
                db.CreateParameter<int>("@TargetUserID", targetUserID, System.Data.SqlDbType.Int);
                db.CreateParameter<string>("@Text", text, System.Data.SqlDbType.NVarChar, 100);
                db.CreateParameter<int>("@TimeLimit", timeLimit, System.Data.SqlDbType.Int);

                return db.ExecuteScalar<int>() == 1;
            }
        }

        public override bool DeleteImpressionTypesForAdmin(AdminImpressionTypeFilter filter, int topCount, out int deletedCount)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildCondition(query, filter);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (TypeID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [TypeID] FROM [bx_ImpressionTypes] WHERE " + conditions + @";

DELETE [bx_ImpressionTypes] WHERE TypeID IN (SELECT [TypeID] FROM @DeleteData);

SELECT @@ROWCOUNT;";

                query.CreateTopParameter("@TopCount", topCount);

                query.CommandText = sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    if (reader.Read())
                        deletedCount = reader.Get<int>(0);

                    return true;
                }
            }
        }

        [StoredProcedure(Name = "bx_Impression_DeleteByType", Script = @"
CREATE PROCEDURE {name}
    @UserID  int,
    @TypeID  int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM bx_Impressions WHERE UserID = @UserID AND TypeID = @TypeID;

    DELETE FROM bx_ImpressionRecords WHERE TargetUserID = @UserID AND TypeID = @TypeID;
END")]
        public override void DeleteImpressionTypeForUser(int userID, int typeID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Impression_DeleteByType";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TypeID", typeID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        public override bool DeleteImpressionTypes(int[] typeIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_ImpressionTypes WHERE TypeID IN (@TypeIDs)";

                query.CreateInParameter<int>("@TypeIDs", typeIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override bool DeleteImpressionRecordsForAdmin(AdminImpressionRecordFilter filter, int topCount, out int deletedCount)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildCondition(query, filter);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (RecordID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [RecordID] FROM [bx_ImpressionRecords] WHERE " + conditions + @";

DELETE [bx_ImpressionRecords] WHERE TypeID IN (SELECT [RecordID] FROM @DeleteData);

SELECT @@ROWCOUNT;";

                query.CreateTopParameter("@TopCount", topCount);

                query.CommandText = sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    if (reader.Read())
                        deletedCount = reader.Get<int>(0);

                    return true;
                }
            }
        }

        public override bool DeleteImpressionRecords(int[] recordIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_ImpressionRecords WHERE TypeID IN (@TypeIDs)";

                query.CreateInParameter<int>("@TypeIDs", recordIDs);

                query.ExecuteNonQuery();

                return true;
            }
        }

        public override FriendCollection GetFriendsHasImpressions(int userID, int pageNumber, int pageSize)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.Pager.TableName = "bx_FriendsHasImpressions";
                db.Pager.SortField = "FriendUserID";
                db.Pager.Condition = "UserID = @UserID";
                db.Pager.PageNumber = pageNumber;
                db.Pager.PageSize = pageSize;
                db.Pager.SelectCount = true;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    FriendCollection result = new FriendCollection(reader);

                    if (reader.NextResult() && reader.Read())
                    {
                        result.TotalRecords = reader.Get<int>(0);
                    }

                    return result;
                }
            }
        }

        public override Hashtable GetImressionsTypesForUsers(int[] userIDs, int top)
        {
            using (SqlQuery db = new SqlQuery())
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < userIDs.Length; i++)
                {
                    sb.Append("SELECT TOP (@Top) A.*, B.UserID FROM bx_ImpressionTypes A RIGHT JOIN bx_Impressions B ON A.TypeID = B.TypeID WHERE B.UserID = ").Append(userIDs[i]).AppendLine("");

                    if (i < userIDs.Length - 1)
                        sb.AppendLine("UNION");
                }

                sb.Append(" ORDER BY B.UserID");

                db.CommandText = sb.ToString();

                db.CreateTopParameter("@Top", top);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    Hashtable result = new Hashtable();

                    ImpressionTypeCollection types = null;

                    int lastUserId = -1;

                    while (reader.Read())
                    {
                        int userID = reader.Get<int>("UserID");

                        ImpressionType type = new ImpressionType(reader);

                        if (userID != lastUserId)
                        {
                            lastUserId = userID;

                            types = new ImpressionTypeCollection();

                            result.Add(userID, types);
                        }

                        types.Add(type);
                    }

                    return result;
                }
            }
        }

        public override RevertableCollection<ImpressionRecord> GetImpressionRecordsWithReverters(int[] impressionTypeIDs)
        {
            if (ValidateUtil.HasItems(impressionTypeIDs) == false)
                return null;

            RevertableCollection<ImpressionRecord> records = new RevertableCollection<ImpressionRecord>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
    B.*,
	A.Text,
    A.KeywordVersion,
	TextReverter = ISNULL(R.TextReverter, '')
FROM 
    bx_ImpressionRecords AS B
LEFT JOIN
	bx_ImpressionTypes AS A WITH(NOLOCK) ON A.TypeID = B.TypeID
LEFT JOIN 
	bx_ImpressionTypeReverters AS R WITH(NOLOCK) ON R.TypeID = A.TypeID
WHERE 
	B.TypeID IN (@TypeIDs)";

                query.CreateInParameter<int>("@TypeIDs", impressionTypeIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string textReverter = reader.Get<string>("TextReverter");

                        ImpressionRecord record = new ImpressionRecord(reader);

                        records.Add(record, textReverter);
                    }
                }
            }

            return records;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Impression_UpdateImpressionRecordKeywords", Script = @"
CREATE PROCEDURE {name}
    @TypeID                int,
    @KeywordVersion        varchar(32),
    @Text                  nvarchar(100),
    @TextReverter          nvarchar(1000)
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = TypeID
    {PrimaryKeyParam} = @TypeID

    {Table} = bx_ImpressionTypes
    {Text} = Text
    {TextParam} = @Text

    {RevertersTable} = bx_ImpressionTypeReverters
    {TextReverter} = TextReverter
    {TextReverterParam} = @TextReverter
    
*/

END")]
        #endregion
        public override void UpdateImpressionRecordKeywords(RevertableCollection<ImpressionRecord> processlist)
        {
            string procedure = "bx_Impression_UpdateImpressionRecordKeywords";
            string table = "bx_ImpressionTypes";
            string primaryKey = "TypeID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 100;
            SqlDbType reve_Type = SqlDbType.NVarChar; int reve_Size = 1000;

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
                foreach (Revertable<ImpressionRecord> item in processlist)
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
                            query.CreateParameter<string>("@Text_" + i, item.Value.Text, text_Type, text_Size);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);
                            query.CreateParameter<string>("@Text_" + i, null, text_Type, text_Size);
                        }

                        //如果恢复信息发生了变化，更新
                        if (item.ReverterChanged)
                            query.CreateParameter<string>("@TextReverter_" + i, item.Reverter, reve_Type, reve_Size);
                        else
                            query.CreateParameter<string>("@TextReverter_" + i, null, reve_Type, reve_Size);

                        i++;

                    }

                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
        }
    }
}