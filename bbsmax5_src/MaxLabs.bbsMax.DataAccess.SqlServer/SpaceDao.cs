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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class SpaceDao : DataAccess.SpaceDao
    {
        public override SpaceData GetSpaceDataForVisit(int spaceOwnerID, DataAccessLevel dataAccessLevel)
        {
            string privacyCondition = null;

            if (dataAccessLevel == DataAccessLevel.Normal)
            {
                privacyCondition = " AND [PrivacyType] IN (0, 3)";
            }
            else if (dataAccessLevel == DataAccessLevel.Friend)
            {
                privacyCondition = " AND [PrivacyType] IN (0, 1, 3)";
            }

            StringBuffer sql = new StringBuffer();

			sql += "SELECT TOP 5 * FROM bx_BlogArticles WHERE UserID = @UserID" + privacyCondition + " ORDER BY ArticleID DESC; \r\n";
			sql += "SELECT TOP 4 * FROM bx_Albums WHERE UserID = @UserID" + privacyCondition + " ORDER BY AlbumID DESC; \r\n";
			sql += "SELECT TOP 5 * FROM bx_Comments WHERE Type = 1 AND TargetUserID = @UserID AND IsApproved = 1 ORDER BY CommentID DESC; \r\n";
			sql += "SELECT TOP 5 * FROM bx_Doings WHERE UserID = @UserID ORDER BY DoingID DESC; \r\n";
			sql += "SELECT TOP 6 * FROM bx_Visitors WHERE UserID = @UserID ORDER BY CreateDate DESC; \r\n";
			sql += "SELECT TOP 12 * FROM bx_Friends WHERE UserID = @UserID AND GroupID >= 0 ORDER BY Hot, CreateDate DESC; \r\n";

            sql += "SELECT TOP 5 * FROM bx_SharesView WHERE UserID = @UserID" + privacyCondition + " AND [PrivacyType] != 2 ORDER BY ShareID DESC;";

            sql += "SELECT TOP 12 *, B.Text, B.KeywordVersion FROM bx_Impressions A LEFT JOIN bx_ImpressionTypes B ON B.TypeID = A.TypeID WHERE UserID = @UserID ORDER BY UpdateDate DESC;";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql.ToString();

                query.CreateParameter<int>("@UserID", spaceOwnerID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    SpaceData result = new SpaceData();

                    result.ArticleList = new BlogArticleCollection(reader);

                    reader.NextResult();

                    result.AlbumList = new AlbumCollection(reader);

                    reader.NextResult();

                    result.CommentList = new CommentCollection(reader);

                    reader.NextResult();

                    result.DoingList = new DoingCollection(reader);

                    reader.NextResult();

                    result.VisitorList = new VisitorCollection(reader);

                    reader.NextResult();

                    result.FriendList = new FriendCollection(reader);

                    reader.NextResult();

                    result.ShareList = new ShareCollection(reader);

                    reader.NextResult();

                    result.ImpressionList = new ImpressionCollection(reader);

                    return result;
                }
            }
        }

        public override VisitorCollection GetSpaceVisitors(int spaceOwnerID, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.SortField = "CreateDate";
                query.Pager.PrimaryKey = "[ID]";
                query.Pager.TableName = "bx_Visitors";
                query.Pager.Condition = "UserID = @UserID";
                query.Pager.IsDesc = true;

                query.CreateParameter<int>("@UserID", spaceOwnerID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    VisitorCollection visitors = new VisitorCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            visitors.TotalRecords = reader.Get<int>(0);
                    }

                    return visitors;
                }
            }
        }

        public override VisitorCollection GetSpaceVisitTrace(int visitorID, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.SortField = "CreateDate";
                query.Pager.PrimaryKey = "[ID]";
                query.Pager.TableName = "bx_Visitors";
                query.Pager.Condition = "VisitorUserID = @UserID";
                query.Pager.IsDesc = true;

                query.CreateParameter<int>("@UserID", visitorID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    VisitorCollection visitors = new VisitorCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            visitors.TotalRecords = reader.Get<int>(0);
                    }

                    return visitors;
                }
            }
        }

        public override void UpdateSpacePrivacy(int userID, SpacePrivacyType blogPrivacy, SpacePrivacyType feedPrivacy, SpacePrivacyType boardPrivacy, SpacePrivacyType doingPrivacy, SpacePrivacyType albumPrivacy, SpacePrivacyType spacePrivacy, SpacePrivacyType sharePrivacy, SpacePrivacyType friendListPrivacy, SpacePrivacyType informationPrivacy)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"UPDATE bx_UserInfos SET 
                                      BlogPrivacy = @BlogPrivacy
                                     ,FeedPrivacy = @FeedPrivacy
                                     ,BoardPrivacy = @BoardPrivacy
                                     ,DoingPrivacy = @DoingPrivacy
                                     ,AlbumPrivacy = @AlbumPrivacy
                                     ,SpacePrivacy = @SpacePrivacy
                                     ,SharePrivacy = @SharePrivacy
                                     ,FriendListPrivacy = @FriendListPrivacy
                                     ,InformationPrivacy = @InformationPrivacy
                                      WHERE UserID = @UserID";

                query.CreateParameter<SpacePrivacyType>("@BlogPrivacy", blogPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@FeedPrivacy", feedPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@BoardPrivacy", boardPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@DoingPrivacy", doingPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@AlbumPrivacy", albumPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@SpacePrivacy", spacePrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@SharePrivacy", sharePrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@FriendListPrivacy", friendListPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<SpacePrivacyType>("@InformationPrivacy", informationPrivacy, SqlDbType.TinyInt);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_UpdateLastVisitor", Script = @"
CREATE PROCEDURE {name}
     @UserID int
    ,@VisitorUserID int
    ,@CreateIP varchar(50)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @OldCreateIP varchar(50)
	DECLARE @OldCreateDate datetime

	IF(@UserID <> @VisitorUserID AND EXISTS (SELECT * FROM bx_Users WHERE UserID = @VisitorUserID)) BEGIN

		SELECT @OldCreateIP = CreateIP,@OldCreateDate = CreateDate FROM bx_Visitors WHERE UserID = @UserID AND VisitorUserID = @VisitorUserID

		IF (@OldCreateIP IS NOT NULL) AND (@OldCreateDate IS NOT NULL) BEGIN

			UPDATE bx_Visitors SET CreateDate = GETDATE(),CreateIP = @CreateIP WHERE UserID = @UserID AND VisitorUserID = @VisitorUserID;
		END
		ELSE BEGIN
			INSERT INTO bx_Visitors(UserID,VisitorUserID,CreateIP) VALUES (@UserID,@VisitorUserID,@CreateIP);
		END

		UPDATE bx_Users SET SpaceViews = SpaceViews + 1 WHERE UserID = @UserID;

	    SELECT 'ResetUser' AS XCMD, UserID, SpaceViews FROM bx_Users WHERE UserID = @UserID;
	END
END
")]
        public override void UpdateVisitor(int userID, int visitorUserID, string createIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateLastVisitor";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@VisitorUserID", visitorUserID, SqlDbType.Int);
                query.CreateParameter<string>("@CreateIP", createIP, SqlDbType.VarChar, 50);

                query.ExecuteNonQuery();
            }
        }

        public override VisitorCollection SelectVisitors(int userID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            VisitorCollection visitors = new VisitorCollection();

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.SortField = "CreateDate";
                query.Pager.TableName = "bx_UserVisitors";
                query.Pager.Condition = "UserID = @UserID";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    visitors = new VisitorCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                }
            }

            return visitors;
        }

        public override VisitorCollection SelectVisitors(int userID, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"SELECT TOP (@Count) * FROM bx_UserVisitors WHERE UserID = @UserID ORDER BY CreateDate DESC";

                query.CreateTopParameter("@Count", count);

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new VisitorCollection(reader);
                }
            }
        }

        [StoredProcedure(Name = "bx_UpdateSpaceTheme", Script = @"
CREATE PROCEDURE {name}
     @UserID int
    ,@Theme nvarchar(50)
AS
BEGIN

    SET NOCOUNT ON;

    UPDATE bx_Users SET SpaceTheme = @Theme WHERE UserID = @UserID;

END
")]
        public override void UpdateSpaceTheme(int userID, string theme)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_UpdateSpaceTheme";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Theme", theme, SqlDbType.NVarChar, 50);

                query.ExecuteNonQuery();
            }
        }

    }
}