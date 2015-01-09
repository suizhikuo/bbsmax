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
using System.Data.SqlClient;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class BlogDao : DataAccess.BlogDao
    {
        #region =========↓日志↓====================================================================================================

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_ExistsBlogArticle", Script = @"
CREATE PROCEDURE {name}
	@ArticleID	int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_BlogArticles WHERE ArticleID = @ArticleID)
		SELECT 1;
	ELSE
		SELECT 0;
END")]
        #endregion
        public override bool ExistsBlogArticle(int articleID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Blog_ExistsBlogArticle";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ArticleID", articleID, SqlDbType.Int);

                return db.ExecuteScalar<bool>();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_GetPostBlogArticleCount", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@BeginDate	datetime,
	@EndDate    datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_BlogArticles WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END")]
        #endregion
        public override int GetPostBlogArticleCount(int userID, DateTime beginDate, DateTime endDate)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Blog_GetPostBlogArticleCount";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                db.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

                return db.ExecuteScalar<int>();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_GetCommentCountForArticle", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@ArticleID	int,
	@BeginDate	datetime,
	@EndDate    datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Comments WHERE [Type] = 2 AND [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate AND [TargetID] = @ArticleID;
END")]
        #endregion
        public override int GetCommentCountForArticle(int userID, int articleID, DateTime beginDate, DateTime endDate)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Blog_GetCommentCountForArticle";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@ArticleID", articleID, SqlDbType.Int);
                db.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                db.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

                return db.ExecuteScalar<int>();
            }
        }

        [StoredProcedure(Name = "bx_Blog_GetBlogArticleCount", Script = @"
CREATE PROCEDURE {name}
	@UserID			int,
	@TargetUserID	int,
	@BeginDate		datetime,
	@EndDate    datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM bx_Comments WHERE [Type] = 2 AND [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate AND [TargetID] IN (
		SELECT ArticleID FROM bx_BlogArticles WHERE bx_BlogArticles.UserID = @TargetUserID
	);
END")]
        public override int GetCommentCountForUser(int userID, int targetUserID, DateTime beginDate, DateTime endDate)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Blog_GetBlogArticleCount";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                db.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                db.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

                return db.ExecuteScalar<int>();
            }
        }

        [StoredProcedure(Name = "bx_Blog_PostBlogArticle", FileName = "bx_Blog_PostBlogArticle.sql")]
        public override int CreateBlogArticle(int userID, string createIP, string subject, string content, string thumbnail, int? categoryID, bool enableComment, PrivacyType privacyType, string password, bool isApproved)
        {
            using (SqlQuery query = new SqlQuery())
            {
                SqlParameter outputIDParam = query.CreateParameter<int>("@ArticleID", SqlDbType.Int, ParameterDirection.Output);

                query.CommandText = "bx_Blog_PostBlogArticle";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@CategoryID", categoryID == null ? 0 : categoryID.Value, SqlDbType.Int);
                query.CreateParameter<bool>("@IsApproved", isApproved, SqlDbType.Bit);
                query.CreateParameter<bool>("@EnableComment", enableComment, SqlDbType.Bit);
                query.CreateParameter<PrivacyType>("@PrivacyType", privacyType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@CreateIP", createIP, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Thumb", thumbnail, SqlDbType.VarChar, 200);
                query.CreateParameter<string>("@Subject", subject, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Content", content, SqlDbType.NText);

                query.ExecuteNonQuery();

                return Convert.ToInt32(outputIDParam.Value);
            }
        }

        [StoredProcedure(Name = "bx_Blog_PostBlogArticle", FileName = "bx_Blog_PostBlogArticle.sql")]
        public override bool UpdateBlogArticle(int userID, string createIP, int articleID, string subject, string content, string thumbnail, int? categoryID, bool enableComment, PrivacyType privacyType, string password, bool isApproved)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_PostBlogArticle";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ArticleID", articleID, SqlDbType.Int);
                query.CreateParameter<int>("@CategoryID", categoryID == null ? 0 : categoryID.Value, SqlDbType.Int);
                query.CreateParameter<bool>("@IsApproved", isApproved, SqlDbType.Bit);
                query.CreateParameter<bool>("@EnableComment", enableComment, SqlDbType.Bit);
                query.CreateParameter<PrivacyType>("@PrivacyType", privacyType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@CreateIP", createIP, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Thumb", thumbnail, SqlDbType.VarChar, 200);
                query.CreateParameter<string>("@Subject", subject, SqlDbType.NVarChar, 200);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Content", content, SqlDbType.NText);

                query.ExecuteNonQuery();

                return true;
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_VisitBlogArticle", Script = @"
CREATE PROCEDURE {name}
      @ArticleID            int
     ,@UserID               int
AS
BEGIN

	SET NOCOUNT ON; 

	BEGIN TRANSACTION
	
	--是否不是日志作者本人访问日志,如果不是,则要更新访问者记录
	UPDATE [bx_BlogArticles] SET [TotalViews] = [TotalViews] + 1 WHERE [ArticleID] = @ArticleID AND [UserID] <> @UserID;
	IF @@ROWCOUNT > 0 BEGIN
	
	    IF EXISTS (SELECT [UserID] FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID AND [UserID] = @UserID)
			UPDATE [bx_BlogArticleVisitors] SET [ViewDate] = GETDATE() WHERE [BlogArticleID] = @ArticleID AND [UserID] = @UserID;
		ELSE BEGIN
			INSERT INTO [bx_BlogArticleVisitors] ([BlogArticleID], [UserID]) VALUES (@ArticleID, @UserID); --写入访问者本次的访问记录
			DELETE FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID AND [UserID] NOT IN (SELECT TOP 10 [UserID] FROM [bx_BlogArticleVisitors] WHERE [BlogArticleID] = @ArticleID ORDER BY [ViewDate] DESC); --清除该日志不在前10条的访问记录
		END

	END
	
	IF(@@error<>0)
		GOTO Cleanup;

	GOTO CommitTrans;
	
	
CommitTrans:
	BEGIN
		COMMIT TRANSACTION
		RETURN (0);
	END
                    
Cleanup:
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

END")]
        #endregion
        public override void VisitArticle(int userID, int articleID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_VisitBlogArticle";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ArticleID", articleID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        /**************************************
         *       Get开头的函数获取数据        *
         **************************************/

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_GetBlogArticle", Script = @"
CREATE PROCEDURE {name}
    @ArticleID      int
AS BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM
        [bx_Tags]
    WHERE
        [ID] IN (SELECT [TagID] FROM [bx_TagRelation] WHERE [Type] = 1 AND [TargetID] = @ArticleID)
    AND 
        [IsLock] = 0;

    SELECT * FROM [bx_BlogArticleVisitors] WHERE  [BlogArticleID] = @ArticleID ORDER BY [ViewDate] DESC;

    SELECT * FROM [bx_BlogArticles] WHERE [ArticleID] = @ArticleID;
END
")]
        #endregion
        public override BlogArticle GetBlogArticle(int articleID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_GetBlogArticle";

                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ArticleID", articleID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    TagCollection tags = new TagCollection(reader);

                    BlogArticleVisitorCollection visitors = null;

                    if (reader.NextResult())
                        visitors = new BlogArticleVisitorCollection(reader);

                    BlogArticle articles = null;

                    if (reader.NextResult() && reader.Read())
                    {
                        articles = new BlogArticle(reader);

                        articles.Tags = tags;
                        articles.LastVisitors = visitors;
                    }

                    return articles;
                }
            }
        }

        public override BlogArticleCollection GetUserBlogArticles(int userID, int? categoryID, int? tagID, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_BlogArticles]";
                query.Pager.SortField = "[ArticleID]";
                query.Pager.IsDesc = true;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;

                SqlConditionBuilder cb = new SqlConditionBuilder(SqlConditionStart.None);

                if (categoryID.HasValue)
                {
                    cb.Append("[CategoryID] = @CategoryID");

                    query.CreateParameter<int>("@CategoryID", categoryID.Value, SqlDbType.Int);

                    if (categoryID.Value == 0)
                    {
                        cb.Append("[UserID] = @UserID");

                        query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                    }
                }
                else
                {
                    cb.Append("[UserID] = @UserID");

                    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                }

                if (tagID.HasValue)
                {
                    cb.Append("[ArticleID] IN (SELECT [TargetID] FROM [bx_TagRelation] WHERE [Type] = 1 AND [TagID] = @TagID)");

                    query.CreateParameter<int>("@TagID", tagID.Value, SqlDbType.Int);
                }

                if (dataAccessLevel == DataAccessLevel.Normal)
                {
                    cb.Append("[PrivacyType] IN (0, 3)");
                }
                else if (dataAccessLevel == DataAccessLevel.Friend)
                {
                    cb.Append("[PrivacyType] IN (0, 1, 3)");
                }

                query.Pager.Condition = cb.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    BlogArticleCollection articles = new BlogArticleCollection(reader);

                    if (reader.NextResult())
                    {
                        if (totalCount == null && reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }

                        articles.TotalRecords = totalCount.GetValueOrDefault();
                    }

                    return articles;
                }
            }
        }

        public override BlogArticleCollection GetFriendBlogArticles(int friendOwnerID, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_BlogArticles]";
                query.Pager.SortField = "ArticleID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[UserID] IN (SELECT [FriendUserID] FROM [bx_Friends] WHERE [UserID] = @UserID) AND ([PrivacyType] IN (0,1,3))";

                query.CreateParameter<int>("@UserID", friendOwnerID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    BlogArticleCollection articles = new BlogArticleCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            articles.TotalRecords = reader.Get<int>(0);
                        }
                    }
                    return articles;
                }
            }
        }

        public override BlogArticleCollection GetEveryoneBlogArticles(int pageNumber, int pageSize, ref int? totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_BlogArticles]";
                query.Pager.SortField = "[UpdateDate]";
                query.Pager.PrimaryKey = "[ArticleID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = " ([PrivacyType] IN (0,3))";

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    BlogArticleCollection articles = new BlogArticleCollection(reader);

                    if (totalCount == null && reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }

                    articles.TotalRecords = totalCount.GetValueOrDefault();

                    return articles;
                }
            }
        }

        public override BlogArticleCollection GetVisitedBlogArticles(int visitorID, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.TableName = "[bx_BlogArticles]";
                query.Pager.SortField = "[ArticleID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.Condition = @"[ArticleID] IN (SELECT [BlogArticleID] FROM [bx_BlogArticleVisitors] WHERE [UserID] = @UserID)";

                query.CreateParameter<int>("@UserID", visitorID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    BlogArticleCollection articles = new BlogArticleCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            articles.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return articles;
                }

            }
        }

        public override BlogArticleCollection GetSimilarArticles(int articleID, int number, DataAccessLevel dataAccessLevel)
        {
            string c = string.Empty;

            if (dataAccessLevel == DataAccessLevel.Normal)
            {
                c = "[PrivacyType] IN (0, 3) AND ";
            }
            else if (dataAccessLevel == DataAccessLevel.Friend)
            {
                c = "[PrivacyType] IN (0, 1, 3) AND ";
            }

            string sql =
@"SELECT TOP (@Number) * FROM [bx_BlogArticles] WHERE " + c + @"[ArticleID] <> @ArticleID AND [ArticleID] IN (
	SELECT DISTINCT [TargetID] FROM [bx_TagRelation] WHERE [TagID] IN (
		SELECT [TagID] FROM [bx_TagRelation] WHERE [TargetID] = @ArticleID AND [Type] = 1
	) AND [Type] = 1
);";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                query.CreateTopParameter("@Number", number);
                query.CreateParameter<int>("@ArticleID", articleID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new BlogArticleCollection(reader);
                }
            }
        }

        public override BlogArticleCollection GetBlogArticlesBySearch(int operatorID, Guid[] excludeRoleIDs, AdminBlogArticleFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, operatorID, excludeRoleIDs, false);

                query.Pager.TableName = "[bx_BlogArticles]";

                query.Pager.SortField = filter.Order.ToString();

                if (filter.Order != AdminBlogArticleFilter.OrderBy.ArticleID)
                {
                    query.Pager.PrimaryKey = "ArticleID";
                }

                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = conditions.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    BlogArticleCollection articles = new BlogArticleCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            articles.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return articles;
                }
            }
        }

        /**************************************
         *      Delete开头的函数删除数据      *
         **************************************/

        public override DeleteResult DeleteBlogArticles(IEnumerable<int> articleIDs, int operatorID, IEnumerable<Guid> excludeRoleIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorID, excludeRoleIDs, query);

                if (string.IsNullOrEmpty(excludeRolesSql) == false)
                    excludeRolesSql = " AND ([UserID] = @UserID OR " + excludeRolesSql + ")";

                string sql = @"
DECLARE @DeleteData table (UserID int, ArticleID int);

INSERT INTO @DeleteData SELECT [UserID],[ArticleID] FROM [bx_BlogArticles] WHERE [ArticleID] IN (@ArticleIDs)" + excludeRolesSql + @";

DELETE [bx_BlogArticles] WHERE ArticleID IN (SELECT [ArticleID] FROM @DeleteData);

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];";

                query.CommandText = sql;

                query.CreateInParameter<int>("@ArticleIDs", articleIDs);
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

        public override DeleteResult DeleteBlogArticlesBySearch(AdminBlogArticleFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount)
        {

            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, operatorUserID, excludeRoleIDs, true);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, ArticleID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserID],[ArticleID] FROM [bx_BlogArticles] " + conditions + @";

DELETE [bx_BlogArticles] WHERE ArticleID IN (SELECT [ArticleID] FROM @DeleteData);

SELECT @@ROWCOUNT;

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];";

                query.CreateTopParameter("@TopCount", topCount);

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

        private string BuildConditionsByFilter(SqlQuery query, AdminBlogArticleFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, bool startWithWhere)
        {
            StringBuffer sqlConditions = new StringBuffer();

            if (string.IsNullOrEmpty(filter.SearchKey) == false)
            {
                switch (filter.SearchMode)
                {
                    case SearchArticleMethod.Subject: //标题筛选
                        sqlConditions += " AND [Subject] LIKE '%' + @Keywords + '%'";
                        break;

                    case SearchArticleMethod.FullText: //正文筛选
                        sqlConditions += " AND [Content] LIKE '%' + @Keywords + '%'";
                        break;

                    default:
                        sqlConditions += " AND ([Subject] LIKE '%' + @Keywords + '%' OR [Content] LIKE '%' + @Keywords + '%')";
                        break;
                }
                query.CreateParameter<string>("@Keywords", filter.SearchKey, SqlDbType.NVarChar, 1000);
            }

            if (string.IsNullOrEmpty(filter.CreateIP) == false)
            {
                sqlConditions += " AND [CreateIP] LIKE '%' + @CreateIP + '%'";
                query.CreateParameter<string>("@CreateIP", filter.CreateIP, SqlDbType.VarChar, 50);
            }

            if (filter.ArticleID != null)
            {
                sqlConditions += " AND [ArticleID] = @ArticleID";
                query.CreateParameter<int?>("@ArticleID", filter.ArticleID, SqlDbType.Int);
            }

            if (filter.AuthorID != null)
            {
                sqlConditions += " AND [UserID] = @UserID";
                query.CreateParameter<int?>("@UserID", filter.AuthorID, SqlDbType.Int);
            }

            if (filter.BeginDate != null)
            {
                sqlConditions += " AND [CreateDate] >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            }

            if (filter.EndDate != null)
            {
                sqlConditions += " AND [CreateDate] <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }

            if (filter.TotaLViewsScopeBegin != null && filter.TotalViewsScopeEnd != null)
            {
                sqlConditions += " AND ([TotalViews] >= @TotalViewsScopeBegin AND [TotalViews] <= @TotalViewsScopeEnd)";
                query.CreateParameter<int?>("@TotalViewsScopeBegin", filter.TotaLViewsScopeBegin, SqlDbType.Int);
                query.CreateParameter<int?>("@TotalViewsScopeEnd", filter.TotalViewsScopeEnd, SqlDbType.Int);
            }

            if (filter.TotalCommentsScopeBegin != null && filter.TotalCommentsScopeEnd != null)
            {
                sqlConditions += " AND ([TotalComments] >= @TotalCommentsScopeBegin AND [TotalComments] <= @TotalCommentsScopeEnd)";
                query.CreateParameter<int?>("@TotalCommentsScopeBegin", filter.TotalCommentsScopeBegin, SqlDbType.Int);
                query.CreateParameter<int?>("@TotalCommentsScopeEnd", filter.TotalCommentsScopeEnd, SqlDbType.Int);

            }

            if (string.IsNullOrEmpty(filter.Username) == false)
            {
                sqlConditions += " AND [UserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @Username)";
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
            }

            string excludeRoleSQL = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorUserID, excludeRoleIDs, query);

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

        #endregion


        public override BlogArticleCollection GetCommentedArticles(int userID, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_BlogArticles]";
                query.Pager.SortField = "ArticleID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[ArticleID] IN (SELECT [TargetID] FROM [bx_Comments] WHERE [Type]=2 AND [UserID] = @UserID AND IsApproved = 1)";


                //query.Pager.Condition += " AND [PrivacyType] < 2";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                BlogArticleCollection articles;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    articles = new BlogArticleCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            articles.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                return articles;
            }
        }







        #region =========↓分类↓====================================================================================================

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_CreateBlogCategory", Script = @"
CREATE PROCEDURE {name}
    @UserID          int
   ,@Name            nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;

	IF NOT EXISTS (SELECT * FROM bx_BlogCategories WHERE UserID = @UserID AND Name = @Name) BEGIN

		INSERT INTO [bx_BlogCategories]([UserID],[Name]) VALUES (@UserID, @Name);
	END
	ELSE BEGIN
		
		SELECT -1;
	END

    SELECT CAST(@@IDENTITY AS int)
END")]
        #endregion
        public override bool CreateBlogCategory(int userID, string categoryName, out int newCategoryId)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_CreateBlogCategory";

                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Name", categoryName, SqlDbType.NVarChar, 50);

                newCategoryId = query.ExecuteScalar<int>();

                if (newCategoryId == -1)
                    return false;
            }
            return true;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_EditBlogCategory", Script = @"
CREATE PROCEDURE {name}
    @CategoryID      int
   ,@Name            nvarchar(50)
AS BEGIN
    SET NOCOUNT ON;

    UPDATE 
        [bx_BlogCategories]
    SET 
        [Name] = @Name,
		[KeywordVersion] = ''
    WHERE 
        [CategoryID] = @CategoryID;
END")]
        #endregion
        public override bool UpdateBlogCategory(int categoryID, string categoryName)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_EditBlogCategory";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);
                query.CreateParameter<string>("@Name", categoryName, SqlDbType.NVarChar, 50);


                query.ExecuteNonQuery();
            }

            return true;
        }

        /**************************************
         *       Get开头的函数获取数据        *
         **************************************/

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_GetBlogCategory", Script = @"
CREATE PROCEDURE {name}
    @CategoryID     int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_BlogCategories] WHERE [CategoryID] = @CategoryID;
END")]
        #endregion
        public override BlogCategory GetBlogCategory(int categoryID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_GetBlogCategory";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@CategoryID", categoryID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BlogCategory(reader);
                    }
                    return new BlogCategory();
                }
            }

        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_GetUserBlogCategories", Script = @"
CREATE PROCEDURE {name}
    @UserID          int
AS BEGIN
    SET NOCOUNT ON;

    SELECT * FROM [bx_BlogCategories] WHERE  [UserID] = @UserID;
END")]
        #endregion
        public override BlogCategoryCollection GetUserBlogCategories(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Blog_GetUserBlogCategories";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new BlogCategoryCollection(reader);
                }
            }

        }

        public override BlogCategoryCollection GetBlogCategoriesBySearch(Guid[] excludeRoleIDs, AdminBlogCategoryFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, excludeRoleIDs, false);

                query.Pager.TableName = "[bx_BlogCategories]";
                query.Pager.SortField = filter.Order.ToString();
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = conditions.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    BlogCategoryCollection categories = new BlogCategoryCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            categories.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return categories;
                }
            }
        }

        /**************************************
         *      Delete开头的函数删除数据      *
         **************************************/

        public override DeleteResult DeleteBlogCategories(IEnumerable<int> categoryIDs, bool deleteArticle, int operatorID, IEnumerable<Guid> excludeRoleIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);

                if (string.IsNullOrEmpty(excludeRolesSql) == false)
                    excludeRolesSql = " AND  ([UserID] = @UserID OR " + excludeRolesSql + ")";

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, [CategoryID] int, [ArticleCount] int);

INSERT INTO @DeleteData SELECT [A].[UserID],[A].[CategoryID],(SELECT COUNT(*) FROM [bx_BlogArticles] AS [B] WHERE [B].[CategoryID]=[A].[CategoryID]) FROM [bx_BlogCategories] AS [A] WHERE [A].[CategoryID] IN (@CategoryIDs)" + excludeRolesSql + ";";

                if (deleteArticle)
                {

                    sql += @"
DELETE [bx_BlogCategories] WHERE [CategoryID] IN (SELECT [CategoryID] FROM @DeleteData);";

                }
                else
                {

                    sql += @"
UPDATE [bx_BlogArticles] SET [CategoryID] = 0 WHERE [CategoryID] IN (SELECT [CategoryID] FROM @DeleteData);";

                }

                sql += @"
DELETE [bx_BlogCategories] WHERE [CategoryID] IN (SELECT [CategoryID] FROM @DeleteData);

SELECT [UserID],SUM([ArticleCount]) AS [Count] FROM @DeleteData GROUP BY [UserID];";

                query.CreateInParameter("@CategoryIDs", categoryIDs);
                query.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);

                query.CommandText = sql.ToString();

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

        public override DeleteResult DeleteBlogCategoriesBySearch(AdminBlogCategoryFilter filter, bool deleteArticle, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount)
        {

            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, excludeRoleIDs, true);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, [CategoryID] int, [ArticleCount] int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [A].[UserID],[A].[CategoryID],(SELECT COUNT(*) FROM [bx_BlogArticles] AS [B] WHERE [B].[CategoryID]=[A].[CategoryID]) FROM [bx_BlogCategories] AS [A] " + conditions + ";";

                if (deleteArticle)
                {

                    sql += @"
DELETE [bx_BlogArticles] WHERE [CategoryID] IN (SELECT [CategoryID] FROM @DeleteData);";

                }
                else
                {

                    sql += @"
UPDATE [bx_BlogArticles] SET [CategoryID] = 0 WHERE [CategoryID] IN (SELECT [CategoryID] FROM @DeleteData);";

                }

                sql += @"
DELETE [bx_BlogCategories] WHERE [CategoryID] IN (SELECT [CategoryID] FROM @DeleteData);

SELECT @@ROWCOUNT;

SELECT [UserID],SUM([ArticleCount]) AS [Count] FROM @DeleteData GROUP BY [UserID];";

                query.CreateTopParameter("@TopCount", topCount);

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

        private string BuildConditionsByFilter(SqlQuery query, AdminBlogCategoryFilter filter, IEnumerable<Guid> excludeRoleIDs, bool startWithWhere)
        {
            StringBuffer sqlConditions = new StringBuffer();

            if (string.IsNullOrEmpty(filter.SearchKey) == false)
            {
                sqlConditions += " AND [Name] LIKE '%' + @SearchKey + '%'";
                query.CreateParameter<string>("@SearchKey", filter.SearchKey, SqlDbType.NVarChar, 50);
            }
            if (filter.BeginDate != null)
            {
                sqlConditions += " AND  [CreateDate] >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.EndDate, SqlDbType.DateTime);
            }
            if (filter.EndDate != null)
            {
                sqlConditions += " AND [CreateDate] <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }
            if (string.IsNullOrEmpty(filter.Username) == false)
            {
                sqlConditions += " AND [UserID] = (SELECT [UserID] FROM [bx_Users] WHERE [Username] = @Username)";
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);

            }

            string excludeRoleSQL = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);

            if (string.IsNullOrEmpty(excludeRoleSQL) == false)
                sqlConditions += " AND " + excludeRoleSQL;

            sqlConditions += " AND [CategoryID] != 0";

            if (sqlConditions.Length > 0)
            {
                sqlConditions.Remove(0, 5);

                if (startWithWhere)
                    sqlConditions.InnerBuilder.Insert(0, " WHERE ");
            }

            return sqlConditions.ToString();
        }

        #endregion

        #region=========↓关键字↓==================================================================================================

        public override Revertable2Collection<BlogArticle> GetBlogArticlesWithReverters(IEnumerable<int> articleIDs)
        {
            if (ValidateUtil.HasItems(articleIDs) == false)
                return null;

            Revertable2Collection<BlogArticle> articles = new Revertable2Collection<BlogArticle>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	SubjectReverter = ISNULL(R.SubjectReverter, ''),
	ContentReverter = ISNULL(R.ContentReverter, '')
FROM 
	bx_BlogArticles A WITH(NOLOCK)
LEFT JOIN 
	bx_BlogArticleReverters R WITH(NOLOCK) ON R.ArticleID = A.ArticleID
WHERE 
	A.ArticleID IN (@ArticleIDs)";

                query.CreateInParameter<int>("@ArticleIDs", articleIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string subjectReverter = reader.Get<string>("SubjectReverter");
                        string contentReverter = reader.Get<string>("ContentReverter");

                        BlogArticle article = new BlogArticle(reader);

                        articles.Add(article, subjectReverter, contentReverter);
                    }
                }
            }

            return articles;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_UpdateBlogArticleKeywords", Script = @"
CREATE PROCEDURE {name}
    @ArticleID         int,
    @KeywordVersion    varchar(32),
    @Subject           nvarchar(200),
    @SubjectReverter   nvarchar(4000),
    @Content           ntext,
    @ContentReverter   ntext
AS BEGIN

/* include : Procedure_UpdateKeyword2.sql

    {PrimaryKey} = ArticleID
    {PrimaryKeyParam} = @ArticleID


    {Table} = bx_BlogArticles
    {Text1} = Subject
    {Text1Param} = @Subject

    {Text2} = Content
    {Text2Param} = @Content


    {RevertersTable} = bx_BlogArticleReverters
    {Text1Reverter} = SubjectReverter
    {Text1ReverterParam} = @SubjectReverter

    {Text2Reverter} = ContentReverter
    {Text2ReverterParam} = @ContentReverter

*/

END")]
        #endregion
        public override void UpdateBlogArticleKeywords(Revertable2Collection<BlogArticle> processlist)
        {
            string procedure = "bx_Blog_UpdateBlogArticleKeywords";
            string table = "bx_BlogArticles";
            string primaryKey = "ArticleID";

            SqlDbType text1_Type = SqlDbType.NVarChar; int text1_Size = 50;
            SqlDbType reve1_Type = SqlDbType.NVarChar; int reve1_Size = 4000;
            SqlDbType text2_Type = SqlDbType.NText; //int text2_Size = 1500;
            SqlDbType reve2_Type = SqlDbType.NText; //int reve2_Size = 4000;


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
                foreach (Revertable2<BlogArticle> item in processlist)
                {
                    //此项确实需要更新，且不只是版本发生了变化
                    if (item.NeedUpdate && item.OnlyVersionChanged == false)
                    {
                        sql.InnerBuilder.AppendFormat(@"EXEC {1} @ID_{0}, @KeywordVersion_{0}, @Text1_{0}, @Reverter1_{0}, @Text2_{0}, @Reverter2_{0};", i, procedure);

                        query.CreateParameter<int>("@ID_" + i, item.Value.GetKey(), SqlDbType.Int);

                        //如果文本1或文本2发生了变化，更新
                        if (item.Text1Changed || item.Text2Changed)
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, item.Value.KeywordVersion, SqlDbType.VarChar, 32);

                            //文本1发生了变化
                            if (item.Text1Changed)
                                query.CreateParameter<string>("@Text1_" + i, item.Value.Text1, text1_Type, text1_Size);
                            else
                                query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);

                            //文本2发生了变化
                            if (item.Text2Changed)
                                query.CreateParameter<string>("@Text2_" + i, item.Value.Text2, text2_Type/*, text2_Size*/);
                            else
                                query.CreateParameter<string>("@Text2_" + i, null, text2_Type/*, text2_Size*/);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);

                            query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);
                            query.CreateParameter<string>("@Text2_" + i, null, text2_Type/*, text2_Size*/);

                        }

                        //如果恢复信息1发生了变化，更新
                        if (item.Reverter1Changed)
                            query.CreateParameter<string>("@Reverter1_" + i, item.Reverter1, reve1_Type, reve1_Size);
                        else
                            query.CreateParameter<string>("@Reverter1_" + i, null, reve1_Type, reve1_Size);

                        //如果恢复信息2发生了变化，更新
                        if (item.Reverter2Changed)
                            query.CreateParameter<string>("@Reverter2_" + i, item.Reverter2, reve2_Type);
                        else
                            query.CreateParameter<string>("@Reverter2_" + i, null, reve2_Type);

                        i++;

                    }
                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
        }

        public override RevertableCollection<BlogCategory> GetBlogCategoriesWithReverters(IEnumerable<int> categoryIDs)
        {
            if (ValidateUtil.HasItems(categoryIDs) == false)
                return null;

            RevertableCollection<BlogCategory> categories = new RevertableCollection<BlogCategory>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	NameReverter = ISNULL(R.NameReverter, '')
FROM 
	bx_BlogCategories A WITH(NOLOCK)
LEFT JOIN 
	bx_BlogCategoryReverters R WITH(NOLOCK) ON R.CategoryID = A.CategoryID
WHERE 
	A.CategoryID IN (@CategoryIDs)";

                query.CreateInParameter<int>("@CategoryIDs", categoryIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string nameReverter = reader.Get<string>("NameReverter");

                        BlogCategory category = new BlogCategory(reader);

                        categories.Add(category, nameReverter);
                    }
                }
            }

            return categories;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Blog_UpdateBlogCategoryKeywords", Script = @"
CREATE PROCEDURE {name}
    @CategoryID            int,
    @KeywordVersion        varchar(32),
    @Name                  nvarchar(50),
    @NameReverter          nvarchar(4000)
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = CategoryID
    {PrimaryKeyParam} = @CategoryID

    {Table} = bx_BlogCategories
    {Text} = Name
    {TextParam} = @Name

    {RevertersTable} = bx_BlogCategoryReverters
    {TextReverter} = NameReverter
    {TextReverterParam} = @NameReverter
    
*/

END")]
        #endregion
        public override void UpdateBlogCategoryKeywords(RevertableCollection<BlogCategory> processlist)
        {
            string procedure = "bx_Blog_UpdateBlogCategoryKeywords";
            string table = "bx_BlogCategories";
            string primaryKey = "CategoryID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 50;
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
                foreach (Revertable<BlogCategory> item in processlist)
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

        #endregion
    }
}