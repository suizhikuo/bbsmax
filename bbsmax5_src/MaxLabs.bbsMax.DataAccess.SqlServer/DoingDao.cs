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
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class DoingDao : DataAccess.DoingDao
    {
        #region StoredProcedure
        [StoredProcedure(Name = "bx_Doing_GetTodayPostDoingCount", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@Today  	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Doings WHERE [UserID] = @UserID AND [CreateDate] >= @Today;
END")]
        #endregion
        public override int GetTodayPostDoingCount(int userID, DateTime today)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Doing_GetTodayPostDoingCount";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<DateTime>("@Today", today, SqlDbType.DateTime);

                return db.ExecuteScalar<int>();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Doing_GetPostDoingCount", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@BeginDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Doings WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END")]
        #endregion
        public override int GetPostDoingCount(int userID, DateTime beginDate, DateTime endDate)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Doing_GetPostDoingCount";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                db.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

                return db.ExecuteScalar<int>();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Doing_GetDoing", Script = @"
CREATE PROCEDURE {name}
	@DoingID	int
AS
BEGIN
    SET NOCOUNT ON;
	SELECT * FROM bx_Doings WHERE DoingID = @DoingID
END")]
        #endregion
        public override Doing GetDoing(int doingID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"bx_Doing_GetDoing";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@DoingID", doingID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Doing(reader);
                    }
                }

                return null;
            }
        }

        public override DoingCollection GetDoings(IEnumerable<int> doingIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_Doings] WHERE [Doing] IN (@DoingIDs);";

                query.CreateInParameter<int>("@DoingIDs", doingIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new DoingCollection(reader);
                }
            }
        }

        public override DoingCollection GetUserDoingsWithComments(int doingOwnerID, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount)
        {
            DoingCollection doings = null;

            using (SqlSession db = new SqlSession())
            {
                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.TableName = "[bx_Doings]";
                    query.Pager.SortField = "[DoingID]";
                    query.Pager.SelectCount = true;
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = pageSize;
                    query.Pager.TotalRecords = totalCount;

                    query.Pager.Condition = "[UserID]=@UserID";
                    query.CreateParameter<int>("@UserID", doingOwnerID, SqlDbType.Int);

                    //if (dataAccessLevel == DataAccessLevel.Normal)
                    //{
                    //    query.Pager.Condition += " AND [PrivacyType] IN (0, 3)";
                    //}
                    //else if (dataAccessLevel == DataAccessLevel.Friend)
                    //{
                    //    query.Pager.Condition += " AND [PrivacyType] IN (0, 1, 3)";
                    //}

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        doings = new DoingCollection(reader);

                        if (totalCount == null && reader.NextResult() && reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }

                        doings.TotalRecords = totalCount.GetValueOrDefault();
                    }
                }

                FillDoingComments(doings, db);
            }

            return doings;
        }

        public override DoingCollection GetFriendDoingsWithComments(int friendOwnerID, int pageNumber, int pageSize, ref int? totalCount)
        {
            DoingCollection doings = null;

            using (SqlSession db = new SqlSession())
            {
                using (SqlQuery query = new SqlQuery())
                {
                    query.Pager.TableName = "[bx_Doings]";
                    query.Pager.SortField = "DoingID";
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = pageSize;
                    query.Pager.IsDesc = true;
                    query.Pager.SelectCount = true;
                    query.Pager.Condition = "[UserID] IN (SELECT [FriendUserID] FROM [bx_Friends] WHERE [UserID] = @UserID)";

                    query.CreateParameter<int>("@UserID", friendOwnerID, SqlDbType.Int);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        doings = new DoingCollection(reader);

                        if (totalCount == null && reader.NextResult() && reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }

                        doings.TotalRecords = totalCount.GetValueOrDefault();
                    }
                }

                FillDoingComments(doings, db);
            }

            return doings;
        }

        public override DoingCollection GetEveryoneDoingsWithComments(int pageNumber, int pageSize, ref int? totalCount)
        {
            DoingCollection doings = null;

            using (SqlSession db = new SqlSession())
            {
                using (SqlQuery query = new SqlQuery())
                {
                    query.Pager.TableName = "[bx_Doings]";
                    query.Pager.SortField = "[DoingID]";
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = pageSize;
                    query.Pager.TotalRecords = totalCount;
                    query.Pager.IsDesc = true;
                    query.Pager.SelectCount = true;

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        doings = new DoingCollection(reader);

                        if (totalCount == null && reader.NextResult() && reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }

                        doings.TotalRecords = totalCount.GetValueOrDefault();
                    }
                }

                FillDoingComments(doings, db);
            }

            return doings;
        }

        public override DoingCollection GetUserCommentedDoingsWithComments(int userID, int pageNumber, int pageSize, ref int? totalCount)
        {
            DoingCollection doings = null;

            using (SqlSession db = new SqlSession())
            {
                using (SqlQuery query = new SqlQuery())
                {
                    query.Pager.TableName = "[bx_Doings]";
                    query.Pager.SortField = "DoingID";
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = pageSize;
                    query.Pager.TotalRecords = totalCount;
                    query.Pager.IsDesc = true;
                    query.Pager.SelectCount = true;
                    query.Pager.Condition = "[DoingID] IN (SELECT [TargetID] FROM [bx_Comments] WHERE [Type]=3 AND [UserID] = @UserID)";

                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        doings = new DoingCollection(reader);

                        if (totalCount == null && reader.NextResult() && reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }

                        doings.TotalRecords = totalCount.GetValueOrDefault();
                    }
                }

                FillDoingComments(doings, db);
            }

            return doings;
        }

        private void FillDoingComments(DoingCollection doings, SqlSession db)
        {
            if (doings.Count == 0)
                return;

            List<int> minIds = new List<int>();
            List<int> maxIds = new List<int>();

            for (int i = 0; i < doings.Count; i++)
            {
                if (doings[i].TotalComments == 0)
                    continue;
                else if (doings[i].TotalComments == 1)
                    minIds.Add(doings[i].DoingID);
                else
                {
                    minIds.Add(doings[i].DoingID);
                    maxIds.Add(doings[i].DoingID);
                }
            }

            if (minIds.Count == 0)
                return;

            using (SqlQuery query = db.CreateQuery())
            {

                query.CommandText = @"
SELECT * FROM bx_Comments WHERE CommentID IN(
    SELECT Min(CommentID) FROM [bx_Comments] WHERE [Type]=3 AND IsApproved = 1 AND [TargetID] IN(@MinTargetIDs) GROUP BY TargetID
);
";
                if (maxIds.Count > 0)
                {
                    query.CommandText += @"
SELECT * FROM bx_Comments WHERE CommentID IN(
    SELECT Max(CommentID) FROM [bx_Comments] WHERE [Type]=3 AND IsApproved = 1 AND [TargetID] IN(@MaxTargetIDs) GROUP BY TargetID
);
";
                }

                query.CreateInParameter<int>("@MinTargetIDs", minIds);
                query.CreateInParameter<int>("@MaxTargetIDs", maxIds);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Comment comment = new Comment(reader);
                        Doing doing = doings.GetValue(comment.TargetID);
                        if (doing != null)
                        {
                            if (doing.CommentList.ContainsKey(comment.CommentID) == false)
                                doing.CommentList.Add(comment);
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            Comment comment = new Comment(reader);
                            Doing doing = doings.GetValue(comment.TargetID);
                            if (doing != null)
                            {
                                if (doing.CommentList.ContainsKey(comment.CommentID) == false)
                                    doing.CommentList.Add(comment);
                            }
                        }
                    }
                }
            }

            db.Connection.Close();

            //Doing doing = null;
            //int lastDoingID = -1;

            //for (int i = 0; i < comments.Count; i++)
            //{
            //    int doingID = comments[i].TargetID;

            //    if (doingID != lastDoingID)
            //    {
            //        doing = doings.GetValue(doingID);

            //        lastDoingID = doingID;
            //    }

            //    doing.CommentList.Add(comments[i]);
            //}
        }

        public override DoingCollection GetDoingsBySearch(Guid[] excludeRoleIDs, AdminDoingFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, excludeRoleIDs, false);

                query.Pager.TableName = "[bx_Doings]";

                query.Pager.SortField = filter.Order.ToString();

                if (filter.Order != AdminDoingFilter.OrderBy.DoingID)
                {
                    query.Pager.PrimaryKey = "DoingID";
                }

                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = conditions.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DoingCollection doings = new DoingCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            doings.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return doings;
                }
            }
        }

        #region
        [StoredProcedure(Name = "bx_Doing_Add", Script = @"
CREATE PROCEDURE {name} 
    @UserID       int,
    @Content      nvarchar(200),
    @CreateIP     varchar(50)
AS BEGIN
    SET NOCOUNT ON;

    IF LEN(@Content)>0 BEGIN
        INSERT INTO bx_Doings([UserID],[TotalComments],[Content],[CreateIP]) VALUES(@UserID,0,@Content,@CreateIP);
        SELECT @@IDENTITY;
    END 
    ELSE BEGIN
        SELECT 0;
    END
    UPDATE bx_Users SET Doing = @Content, DoingDate = getdate() WHERE UserID = @UserID;
END
")]
        #endregion
        public override void AddDoing(int creatorID, string creatorIP, string content, out int doingID)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_Doing_Add";

                query.CreateParameter<int>("@UserID", creatorID, SqlDbType.Int);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@CreateIP", creatorIP, SqlDbType.VarChar, 50);

                doingID = query.ExecuteScalar<int>();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Doing_DeleteDoing", Script = @"
CREATE PROCEDURE {name} 
    @DoingID    int
AS BEGIN
    SET NOCOUNT ON;

    DELETE FROM bx_Doings WHERE DoingID = @DoingID;

END
")]
        #endregion
        public override void DeleteDoing(int doingID)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = @"bx_Doing_DeleteDoing";

                query.CreateParameter<int>("@DoingID", doingID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        public override DeleteResult DeleteDoings(int operatorID, IEnumerable<int> doingIDs, IEnumerable<Guid> excludeRoleIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);

                if (string.IsNullOrEmpty(excludeRolesSql) == false)
                    excludeRolesSql = " AND ([UserID] = @UserID OR " + excludeRolesSql + ")";

                string sql = @"
DECLARE @DeleteData table (UserID int, DoingID int);

INSERT INTO @DeleteData SELECT [UserID],[DoingID] FROM [bx_Doings] WHERE [DoingID] IN (@DoingIDs)" + excludeRolesSql + @";

DELETE [bx_Doings] WHERE DoingID IN (SELECT [DoingID] FROM @DeleteData);

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];";

                query.CommandText = sql;

                query.CreateInParameter<int>("@DoingIDs", doingIDs);
                query.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    while (reader.Read())
                    {
                        deleteResult.Add(reader.Get<int>("UserID"), reader.Get<int>("Count"));
                    }

                    return deleteResult;
                }
            }
        }

        public override DeleteResult DeleteDoingsBySearch(AdminDoingFilter filter, IEnumerable<Guid> excludeRoleIDs, int deleteTopCount, out int deletedCount)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, excludeRoleIDs, true);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, DoingID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserID],[DoingID] FROM [bx_Doings] " + conditions + @";

DELETE [bx_Doings] WHERE DoingID IN (SELECT [DoingID] FROM @DeleteData);

SELECT @@ROWCOUNT;

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];";

                query.CreateTopParameter("@TopCount", deleteTopCount);

                query.CommandText = sql.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    if (reader.Read())
                        deletedCount = reader.Get<int>(0);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            deleteResult.Add(reader.Get<int>("UserID"), reader.Get<int>("Count"));
                        }
                    }

                    return deleteResult;
                }
            }

        }

        private string BuildConditionsByFilter(SqlQuery query, AdminDoingFilter filter, IEnumerable<Guid> excludeRoleIDs, bool startWithWhere)
        {
            StringBuffer sqlConditions = new StringBuffer();


            if (filter.UserID != null && filter.UserID > 0)
            {
                sqlConditions += " AND UserID = @UserID";

                query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
            }

            if (!string.IsNullOrEmpty(filter.Username))
            {
                sqlConditions += " AND UserID IN (SELECT UserID FROM bx_Users WHERE Username = @Username)";
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
            }

            if (!string.IsNullOrEmpty(filter.Content))
            {
                sqlConditions += " AND Content LIKE '%'+@Content+'%'";
                query.CreateParameter<string>("@Content", filter.Content, SqlDbType.NVarChar, 200);
            }

            if (!string.IsNullOrEmpty(filter.IP))
            {
                sqlConditions += " AND CreateIP LIKE  '%'+@IP+'%'";
                query.CreateParameter<string>("@IP", filter.IP, SqlDbType.VarChar, 50);
            }

            if (filter.BeginDate != null)
            {
                sqlConditions += " AND CreateDate >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            }

            if (filter.EndDate != null)
            {
                sqlConditions += " AND CreateDate <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }

            string excludeRoleSQL = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);

            if (string.IsNullOrEmpty(excludeRoleSQL) == false)
            {
                sqlConditions += " AND " + excludeRoleSQL;
            }

            if (sqlConditions.Length > 0)
            {
                sqlConditions.Remove(0, 5);
                if (startWithWhere)
                    sqlConditions.InnerBuilder.Insert(0, " WHERE ");
            }

            return sqlConditions.ToString();
        }

        public override RevertableCollection<Doing> GetDoingsWithReverters(IEnumerable<int> doingIDs)
        {
            if (ValidateUtil.HasItems(doingIDs) == false)
                return null;

            RevertableCollection<Doing> doings = new RevertableCollection<Doing>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	ContentReverter = ISNULL(R.ContentReverter, '')
FROM 
	bx_Doings A WITH(NOLOCK)
LEFT JOIN 
	bx_DoingReverters R WITH(NOLOCK) ON R.DoingID = A.DoingID
WHERE 
	A.DoingID IN (@DoingIDs)";

                query.CreateInParameter<int>("@DoingIDs", doingIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string contentReverter = reader.Get<string>("ContentReverter");

                        Doing doing = new Doing(reader);

                        doings.Add(doing, contentReverter);
                    }
                }
            }

            return doings;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Doing_UpdateDoingKeywords", Script = @"
CREATE PROCEDURE {name}
    @DoingID                  int,
    @KeywordVersion           varchar(32),
    @Content                  nvarchar(200),
    @ContentReverter          nvarchar(4000)
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = DoingID
    {PrimaryKeyParam} = @DoingID

    {Table} = bx_Doings
    {Text} = Content
    {TextParam} = @Content

    {RevertersTable} = bx_DoingReverters
    {TextReverter} = ContentReverter
    {TextReverterParam} = @ContentReverter
    
*/

END")]
        #endregion
        public override void UpdateDoingKeywords(RevertableCollection<Doing> processlist)
        {
            string procedure = "bx_Doing_UpdateDoingKeywords";
            string table = "bx_Doings";
            string primaryKey = "DoingID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 200;
            SqlDbType reve_Type = SqlDbType.NVarChar; int reve_Size = 4000;

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
                foreach (Revertable<Doing> item in processlist)
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