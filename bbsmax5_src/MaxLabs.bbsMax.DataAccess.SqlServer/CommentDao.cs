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
using System.Data.SqlClient;
using System.Collections.Generic;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class CommentDao : DataAccess.CommentDao
    {

        public override CommentCollection GetLastestCommentsForSomeone(int targetUserID, CommentType type, int top)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string getTargetNameSql = null;

                switch (type)
                {
                    case CommentType.Blog:
                        getTargetNameSql = "(SELECT [Subject] FROM [bx_BlogArticles] WHERE [ArticleID]=TargetID) AS [TargetName] ";
                        break;

                    case CommentType.Photo:
                        getTargetNameSql = "(SELECT [Name] FROM [bx_Photos] WHERE [PhotoID]=TargetID) AS [TargetName] ";
                        break;

                    default:
                        getTargetNameSql = string.Empty;
                        break;
                }

                query.CommandText = "SELECT TOP (@TopCount) *, " + getTargetNameSql + " FROM bx_Comments WHERE [TargetUserID]=@TargetUserID AND [Type]=@Type ORDER BY [CommentID] DESC";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                query.CreateParameter<CommentType>("@Type", type, SqlDbType.TinyInt);

                query.CreateTopParameter("@TopCount", top);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new CommentCollection(reader);
                }
            }
        }

        public override CommentCollection GetCommentsBySearch(int operatorID, Guid[] excludeRoleIDs, AdminCommentFilter filter, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, false, operatorID, excludeRoleIDs);

                query.Pager.TableName = "[bx_Comments]";
                query.Pager.SortField = filter.Order.ToString();
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;

                query.Pager.Condition = conditions.ToString();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    CommentCollection comments = new CommentCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            comments.TotalRecords = reader.Get<int>(0);
                        }
                    }

                    return comments;
                }
            }
        }

        public override DeleteResult DeleteCommentsBySearch(AdminCommentFilter filter, int operatorID, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount)
        {
            deletedCount = 0;

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, true, operatorID, excludeRoleIDs);

                StringBuffer sql = new StringBuffer();

                sql += @"
DECLARE @DeleteData table (UserID int, CommentID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserID],[CommentID] FROM [bx_Comments] " + conditions + @";

DELETE [bx_Comments] WHERE CommentID IN (SELECT [CommentID] FROM @DeleteData);

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

        private string BuildConditionsByFilter(SqlQuery query, AdminCommentFilter filter, bool startWithWhere, int operatorUserID, IEnumerable<Guid> excludeRoleIds)
        {
            StringBuffer sqlCondition = new StringBuffer();

            if (filter.Type != CommentType.All)
            {
                sqlCondition += " AND Type = @Type";
                query.CreateParameter<int>("@Type", (int)filter.Type, SqlDbType.Int);
            }

            if (filter.IsApproved != null)
            {
                sqlCondition += " AND IsApproved = @IsApproved";
                query.CreateParameter<bool?>("@IsApproved", filter.IsApproved, SqlDbType.Bit);
            }

            if (string.IsNullOrEmpty(filter.Username) == false)
            {
                sqlCondition += " AND UserID = (SELECT UserID FROM bx_Users WHERE Username = @Username)";
                query.CreateParameter<string>("@Username", filter.Username, SqlDbType.NVarChar, 50);
            }

            if (string.IsNullOrEmpty(filter.TargetUsername) == false)
            {
                sqlCondition += " AND TargetUserID IN (SELECT UserID FROM bx_Users WHERE Username = @TargetUsername)";
                query.CreateParameter<string>("@TargetUsername", filter.TargetUsername, SqlDbType.NVarChar, 50);
            }

            if (filter.BeginDate != null)
            {
                sqlCondition += " AND CreateDate >= @BeginDate";
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            }

            if (filter.EndDate != null)
            {
                sqlCondition += " AND CreateDate <= @EndDate";
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
            }

            if (!string.IsNullOrEmpty(filter.Content))
            {
                sqlCondition += " AND Content LIKE '%'+@Content+'%'";
                query.CreateParameter<string>("@Content", filter.Content, SqlDbType.NVarChar, 50);
            }

            if (!string.IsNullOrEmpty(filter.IP))
            {
                sqlCondition += " AND CreateIP = @CreateIP";
                query.CreateParameter<string>("@CreateIP", filter.IP, SqlDbType.VarChar, 50);
            }

            string excludeRoleSQL = DaoUtil.GetExcludeRoleSQL("[UserID]", "[LastEditUserID]", operatorUserID, excludeRoleIds, query);

            if (string.IsNullOrEmpty(excludeRoleSQL) == false)
            {
                sqlCondition += " AND " + excludeRoleSQL;
            }


            if (sqlCondition.Length != 0)
            {
                sqlCondition.Remove(0, 5);
            }

            if (startWithWhere && sqlCondition.Length > 0)
            {
                sqlCondition.InnerBuilder.Insert(0, " WHERE ");
            }

            return sqlCondition.ToString();
        }

        #region=========↓关键字↓==================================================================================================

        public override RevertableCollection<Comment> GetCommentsWithReverters(IEnumerable<int> commentIDs)
        {
            if (ValidateUtil.HasItems(commentIDs) == false)
                return null;

            RevertableCollection<Comment> comments = new RevertableCollection<Comment>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	ContentReverter = ISNULL(R.ContentReverter, '')
FROM 
	bx_Comments A WITH(NOLOCK)
LEFT JOIN 
	bx_CommentReverters R WITH(NOLOCK) ON R.CommentID = A.CommentID
WHERE 
	A.CommentID IN (@CommentIDs)";

                query.CreateInParameter<int>("@CommentIDs", commentIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string contentReverter = reader.Get<string>("ContentReverter");

                        Comment comment = new Comment(reader);

                        comments.Add(comment, contentReverter);
                    }
                }
            }

            return comments;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Comment_UpdateCommentKeywords", Script = @"
CREATE PROCEDURE {name}
    @CommentID                int,
    @KeywordVersion           varchar(32),
    @Content                  nvarchar(3000),
    @ContentReverter          ntext
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = CommentID
    {PrimaryKeyParam} = @CommentID

    {Table} = bx_Comments
    {Text} = Content
    {TextParam} = @Content

    {RevertersTable} = bx_CommentReverters
    {TextReverter} = ContentReverter
    {TextReverterParam} = @ContentReverter
    
*/

END")]
        #endregion
        public override void UpdateCommentKeywords(RevertableCollection<Comment> processlist)
        {
            string procedure = "bx_Comment_UpdateCommentKeywords";
            string table = "bx_Comments";
            string primaryKey = "CommentID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 3000;
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
                foreach (Revertable<Comment> item in processlist)
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

        #endregion

        public override void AddComment(int userID, int targetID, CommentType type, bool isApproved, string content, /* string contentReverter, */ string createIP, out int targetUserID, out int newCommentId)
        {
            //string tableName = string.Empty;
            targetUserID = 0;

            string getTargetUserSql;

            switch (type)
            {
                case CommentType.Blog:
                    getTargetUserSql = "(SELECT UserID FROM bx_BlogArticles WHERE [ArticleID] = @TargetID)";
                    break;

                case CommentType.Photo:
                    getTargetUserSql = "(SELECT UserID FROM bx_Photos WHERE [PhotoID] = @TargetID)";
                    break;

                case CommentType.Doing:
                    getTargetUserSql = "(SELECT UserID FROM bx_Doings WHERE [DoingID] = @TargetID)";
                    break;

                case CommentType.Share:
                    getTargetUserSql = "(SELECT UserID FROM bx_UserShares WHERE [UserShareID] = @TargetID)";
                    break;

                case CommentType.Board:
                    getTargetUserSql = "@TargetID";
                    break;

                default:
                    getTargetUserSql = string.Empty;
                    break;
            }

            string sql = string.Format(@"
DECLARE @TargetUserID int;
SET @TargetUserID = {0};
INSERT INTO bx_Comments([UserID],[TargetID],[TargetUserID],[LastEditUserID],[IsApproved],[Type],[Content],[CreateIP]) VALUES(@UserID,@TargetID,@TargetUserID,@UserID,@IsApproved,@Type,@Content,@CreateIP);
SELECT CAST(@@IDENTITY as int) AS NewID, @TargetUserID AS TargetID
", getTargetUserSql);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsApproved", isApproved, SqlDbType.Bit);
                query.CreateParameter<int>("@Type", (int)type, SqlDbType.Int);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 3000);
                query.CreateParameter<string>("@CreateIP", createIP, SqlDbType.VarChar, 50);
                targetUserID = 0; newCommentId = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        targetUserID = reader.GetInt32(1);
                        newCommentId = reader.GetInt32(0);
                    }
                }
            }
        }

        public override void DeleteComment(int userID, int commentID, out int commentUserID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT TargetUserID FROM bx_Comments WHERE (UserID = @UserID OR TargetUserID = @UserID) AND CommentID = @CommentID;
                                      DELETE FROM bx_Comments WHERE (UserID = @UserID OR TargetUserID = @UserID) AND CommentID = @CommentID";

                query.CreateParameter<int>("@CommentID", commentID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                commentUserID = query.ExecuteScalar<int>();
            }
        }

        public override void DeleteComments(IEnumerable<int> commentIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Comments WHERE CommentID IN (@CommentIds)";

                query.CreateInParameter<int>("@CommentIds", commentIds);

                query.ExecuteNonQuery();
            }
        }

        public override DeleteResult DeleteCommentsByFilter(AdminCommentFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult)
        {

            using (SqlQuery query = new SqlQuery())
            {
                string conditions = BuildConditionsByFilter(query, filter, true, operatorUserID, excludeRoleIDs);

                string sql = string.Empty;

                if (getDeleteResult)
                {
                    sql = @"SELECT [UserID],COUNT(*) AS [Count] FROM [bx_Comments] {0} GROUP BY [UserID];";
                }
                sql += "DELETE FROM bx_Comments {0};";


                query.CommandText = string.Format(sql, conditions);

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

        public override void ApproveComments(IEnumerable<int> commentIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Comments SET IsApproved = @IsApproved WHERE CommentID IN (@CommentIds)";

                query.CreateParameter<bool>("@IsApproved", true, SqlDbType.Bit);

                query.CreateInParameter<int>("@CommentIds", commentIds);

                query.ExecuteNonQuery();
            }
        }

        public override void UpdateComment(int commentID, int operatorUserID, bool isApproved, string content, string contentReverter, out int targetID)
        {
            targetID = 0;
            string sql = @"SELECT TargetID FROM bx_Comments WHERE CommentID = @CommentID;
                           UPDATE bx_Comments SET Content = @Content, IsApproved = @IsApproved, KeywordVersion = '', LastEditUserID = @OperatorUserID WHERE CommentID = @CommentID;";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql;

                query.CreateParameter<int>("@CommentID", commentID, SqlDbType.Int);
                query.CreateParameter<int>("@OperatorUserID", operatorUserID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsApproved", isApproved, SqlDbType.Bit);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 3000);
                query.CreateParameter<string>("@ContentReverter", contentReverter, SqlDbType.NVarChar, 1000);

                targetID = query.ExecuteScalar<int>();
            }
        }

        public override CommentCollection GetCommentsByTargetID(int targetID, CommentType type, int pageNumber, int pageSize, bool isDesc, out int totalCount)
        {
            totalCount = 0;
            string sqlCondition = "Type=@Type AND TargetID = @TargetID AND IsApproved = 1";

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Comments";
                query.Pager.Condition = sqlCondition;
                query.Pager.SortField = "CommentID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.ResultFields = "*";
                query.Pager.IsDesc = isDesc;


                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)type, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    CommentCollection comments = new CommentCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return comments;
                }
            }
        }

        public override CommentCollection GetCommentsByUserID(int userID, CommentType type, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            string sqlCondition = "Type = @Type AND UserID = @UserID AND IsApproved = 1";

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "bx_Comments";
                query.Pager.Condition = sqlCondition;
                query.Pager.SortField = "CommentID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.ResultFields = "*";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)type, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    CommentCollection comments = new CommentCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return comments;
                }
            }
        }

        public override CommentCollection GetComments(int targetID, CommentType type, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT TOP (@Count) * FROM bx_Comments WHERE TargetID = @TargetID AND Type = @Type ORDER BY CommentID DESC";



                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)type, SqlDbType.Int);

                query.CreateTopParameter("@Count", count);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new CommentCollection(reader);
                }
            }
        }

        public override CommentCollection GetCommentsByFilter(AdminCommentFilter filter, int operatorUserID, IEnumerable<Guid> excludeRoleIDs, int pageNumber, out int totalCount)
        {
            totalCount = 0;

            using (SqlQuery query = new SqlQuery())
            {

                query.Pager.TableName = "bx_Comments";
                query.Pager.SortField = filter.Order.ToString();
                if (filter.Order != AdminCommentFilter.OrderBy.CommentID)
                    query.Pager.PrimaryKey = "CommentID";
                query.Pager.IsDesc = filter.IsDesc;
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.SelectCount = true;
                query.Pager.Condition = BuildConditionsByFilter(query, filter, false, operatorUserID, excludeRoleIDs);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    CommentCollection comments = new CommentCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return comments;
                }
            }
        }

        public override Comment GetComment(int commentID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * From bx_Comments WHERE CommentID = @CommentID";

                query.CreateParameter<int>("@CommentID", commentID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Comment(reader);
                    }
                    return null;
                }
            }
        }

        public override CommentCollection GetComments(IEnumerable<int> commentIds)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_Comments] WHERE CommentID IN (@CommentIds);";

                query.CreateInParameter<int>("@CommentIds", commentIds);

                CommentCollection comments = new CommentCollection();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        comments.Add(new Comment(reader));
                    }
                    return comments;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="type"></param>
        /// <param name="getCount">取最新的前N条 和 最旧的一条</param>
        /// <param name="isGetAll"></param>
        /// <returns></returns>
        public override CommentCollection GetComments(int targetID, CommentType type, int getCount, bool isGetAll)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (isGetAll)
                {
                    query.CommandText = "SELECT * FROM [bx_Comments] WHERE Type = @Type AND TargetID = @TargetID AND IsApproved = 1 ORDER BY CommentID ASC;";
                }
                else
                {
                    query.CommandText = @"
SELECT TOP(@TopCount) * FROM [bx_Comments] WHERE Type = @Type AND TargetID = @TargetID AND IsApproved = 1 ORDER BY CommentID DESC;
SELECT TOP 1 * FROM [bx_Comments] WHERE Type = @Type AND TargetID = @TargetID ORDER BY CommentID ASC;
                ";
                    query.CreateTopParameter("@TopCount", getCount);
                }

                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)type, SqlDbType.TinyInt);

                CommentCollection comments = new CommentCollection();

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (isGetAll)
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment(reader));
                        }
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            comments.Insert(0, new Comment(reader));
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                comments.Insert(0, new Comment(reader));
                            }
                        }
                    }
                    return comments;
                }
            }
        }
    }
}