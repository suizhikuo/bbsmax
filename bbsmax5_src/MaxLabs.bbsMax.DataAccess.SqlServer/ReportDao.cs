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
using MaxLabs.bbsMax.Enums;
using System.Data;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Filters;
using System.Data.SqlClient;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
	public class DenouncingDao : DataAccess.DenouncingDao
    {
		[StoredProcedure(Name="bx_Denouncing_Create", Script= @"
CREATE PROCEDURE {name}
	@UserID			int,
	@TargetID		int,
	@TargetUserID	int,
	@Type			tinyint,
	@Content		nvarchar(50),
	@CreateIP		varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DenouncingID int;

	IF EXISTS(SELECT * FROM bx_Denouncings WHERE TargetID = @TargetID AND Type = @Type)
	BEGIN
		SELECT @DenouncingID = DenouncingID FROM bx_Denouncings WHERE TargetID = @TargetID AND Type = @Type;
	END
	ELSE
	BEGIN
		INSERT INTO bx_Denouncings(TargetID,TargetUserID,Type) VALUES (@TargetID,@TargetUserID,@Type);

		SELECT @DenouncingID = @@IDENTITY;
	END

	IF NOT EXISTS(SELECT * FROM bx_DenouncingContents WHERE DenouncingID = @DenouncingID AND UserID = @UserID)
	BEGIN
		INSERT INTO bx_DenouncingContents (DenouncingID, UserID,Content) VALUES (@DenouncingID, @UserID,@Content);
	END
END")]
		public override void CreateDenouncing(int userID, int targetID, int targetUserID, DenouncingType type, string content, string createIP)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Denouncing_Create";
				query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
				query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
				query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
				query.CreateParameter<DenouncingType>("@Type", type, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@CreateIP", createIP, SqlDbType.VarChar, 50);

                query.ExecuteNonQuery();
            }
        }

		[StoredProcedure(Name="bx_Denouncing_Delete", Script=@"
CREATE PROCEDURE {name}
	@DenouncingID	int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM bx_DenouncingContents WHERE DenouncingID = @DenouncingID;

	DELETE FROM bx_Denouncings WHERE DenouncingID = @DenouncingID;
END")]
		public override void DeleteDenouncing(int denouncingID)
		{
			using (SqlQuery query = new SqlQuery())
			{
				query.CommandText = "bx_Denouncing_Delete";
				query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@DenouncingID", denouncingID, SqlDbType.Int);

				query.ExecuteNonQuery();
			}
		}

		public override void DeleteDenouncings(int[] denouncingIDs)
		{
			using (SqlQuery query = new SqlQuery())
			{
				query.CommandText = @"
				DELETE FROM bx_DenouncingContents WHERE DenouncingID IN (@DenouncingIDs);
				DELETE FROM bx_Denouncings WHERE DenouncingID IN (@DenouncingIDs);";

				query.CreateInParameter<int>("@DenouncingIDs", denouncingIDs);

				query.ExecuteNonQuery();
			}
		}

		[StoredProcedure(Name="bx_Denouncing_Ignore", Script=@"
CREATE PROCEDURE {name}
	@DenouncingID	int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE bx_Denouncings SET IsIgnore = 1 WHERE DenouncingID = @DenouncingID;
END")]
		public override void IgnoreDenouncing(int denouncingID)
		{
			using (SqlQuery query = new SqlQuery())
			{
				query.CommandText = "bx_Denouncing_Ignore";
				query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@DenouncingID", denouncingID, SqlDbType.Int);

				query.ExecuteNonQuery();
			}
		}

		public override void IgnoreDenouncings(int[] denouncingIDs)
		{
			using (SqlQuery query = new SqlQuery())
			{
				query.CommandText = "UPDATE bx_Denouncings SET IsIgnore = 1 WHERE DenouncingID IN (@DenouncingIDs)";
				
				query.CreateInParameter<int>("@DenouncingIDs", denouncingIDs);

				query.ExecuteNonQuery();
			}
		}

		[StoredProcedure(Name="bx_Denouncing_GetByID", Script= @"
CREATE PROCEDURE {name}
	@DenouncingID	int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM bx_Denouncings WHERE DenouncingID = @DenouncingID;
END")]
		public override Denouncing GetDenouncing(int denouncingID)
		{
			using (SqlQuery query = new SqlQuery())
			{
				query.CommandText = "bx_Denouncing_GetByID";
				query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@DenouncingID", denouncingID, SqlDbType.Int);

				using (XSqlDataReader reader = query.ExecuteReader())
				{
					if (reader.Read())
						return new Denouncing(reader);
					else
						return null;
				}
			}
		}

		[StoredProcedure(Name="bx_Denouncing_CheckState", Script=@"
CREATE PROCEDURE {name}
	@UserID		int,
	@TargetID	int,
	@TargetType	tinyint
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT * FROM bx_Denouncings WHERE IsIgnore = 1 AND TargetID = @TargetID AND Type = @TargetType) 
	BEGIN
		RETURN 2;
	END
	
	IF EXISTS (SELECT * FROM bx_DenouncingContents WHERE UserID = UserID AND DenouncingID IN (
		SELECT A.DenouncingID FROM bx_Denouncings A WHERE  TargetID = @TargetID AND Type = @TargetType))
	BEGIN
		RETURN 3;
	END

	RETURN 1;
END")]
		public override CheckDenouncingStateResult CheckDenouncingState(int operatorID, int targetID, DenouncingType targetType)
		{
			using (SqlQuery query = new SqlQuery())
			{
				query.CommandText = "bx_Denouncing_CheckState";
				query.CommandType = CommandType.StoredProcedure;

				query.CreateParameter<int>("@UserID", operatorID, SqlDbType.Int);
				query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
				query.CreateParameter<DenouncingType>("@TargetType", targetType, SqlDbType.TinyInt);

				SqlParameter result = query.CreateParameter<int>("@Result", SqlDbType.Int, ParameterDirection.ReturnValue);

				query.ExecuteNonQuery();

				switch ((int)result.Value)
				{
					case 2:
						return CheckDenouncingStateResult.WasIgnoreByAdmin;

					case 3:
						return CheckDenouncingStateResult.WasDenounced;

					default:
						return CheckDenouncingStateResult.CanDenouncing;
				}
			}
		}

		public override DenouncingCollection GetDenouncingBySearch(DenouncingFilter filter, int pageNumber)
        {
			using (SqlSession db = new SqlSession())
			{
				DenouncingCollection denouncings = null;

				using (SqlQuery query = db.CreateQuery())
				{
					query.Pager.PageNumber = pageNumber;
					query.Pager.PageSize = filter.PageSize;
					query.Pager.TableName = "bx_Denouncings";
					query.Pager.SortField = "DenouncingID";
					query.Pager.IsDesc = filter.IsDesc;
					query.Pager.SelectCount = true;

					GetSearchDenouncingsCondition(query, filter);

					using (XSqlDataReader reader = query.ExecuteReader())
					{
						denouncings = new DenouncingCollection(reader);

						if (reader.NextResult())
						{
							while (reader.Read())
								denouncings.TotalRecords = reader.Get<int>(0);
						}
					}
				}

				FillDenouncingContents(denouncings, db);

				return denouncings;
			}
        }

		private void FillDenouncingContents(DenouncingCollection denouncings, SqlSession db)
		{
			if (denouncings.Count == 0)
				return;

			DenouncingContentCollection contents = null;

			using (SqlQuery query = db.CreateQuery())
			{
				query.CommandText = "SELECT * FROM bx_DenouncingContents WHERE DenouncingID IN (@DenouncingIDs) ORDER BY [DenouncingID]";

				query.CreateInParameter<int>("@DenouncingIDs", denouncings.GetDenouncingIDs());

				using (XSqlDataReader reader = query.ExecuteReader())
				{
					contents = new DenouncingContentCollection(reader);
				}
			}

			db.Connection.Close();

			Denouncing denouncing = null;
			int lastDenouncingID = -1;

			for (int i = 0; i < contents.Count; i++)
			{
				int denouncingID = contents[i].DenouncingID;

				if (denouncingID != lastDenouncingID)
				{
					denouncing = denouncings.GetValue(denouncingID);

					lastDenouncingID = denouncingID;
				}

				denouncing.ContentList.Add(contents[i]);
			}
		}

		private void GetSearchDenouncingsCondition(SqlQuery query, DenouncingFilter filter)
        {
            StringBuilder condition = new StringBuilder();

			if (filter.Type != DenouncingType.All)
			{
				condition.Append(" AND Type = @Type");
				query.CreateParameter<int>("@Type", (int)filter.Type, SqlDbType.TinyInt);
			}
			
			if (filter.ReportState != null && filter.ReportState.Value != 2)
			{
				condition.Append(" AND IsIgnore = @IsIgnore");

				bool isIgnore = false;

				if (filter.ReportState.Value == 1)
					isIgnore = true;
				else
					isIgnore = false;

				query.CreateParameter<bool>("@IsIgnore", isIgnore, SqlDbType.Bit);
			}

			if (filter.BeginDate != null)
			{
				condition.Append(" AND CreateDate >= @BeginDate");
				query.CreateParameter<DateTime>("@BeginDate", filter.BeginDate.Value, SqlDbType.DateTime);
			}

			if (filter.EndDate != null)
			{
				condition.Append(" AND CreateDate <= @EndDate");
				query.CreateParameter<DateTime>("@EndDate", filter.EndDate.Value, SqlDbType.DateTime);
			}

            if (condition.Length != 0)
                condition = condition.Remove(0, 5);

            query.Pager.Condition = condition.ToString();
        }

        [StoredProcedure(Name="bx_Denouncing_GetCount", Script= @"
CREATE PROCEDURE {name}
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 1;  --photo
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 3;  --article
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 5;  --share
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 4;  --user
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 6;  --topic
    SELECT COUNT(*) FROM bx_Denouncings WHERE IsIgnore = 0 AND Type = 7;  --reply
END")]
        public override void GetDenouncingCount(
            out int? denouncingPhotoCount,
            out int? denouncingArticleCount,
            out int? denouncingShareCount,
            out int? denouncingUserCount,
            out int? denouncingTopicCount,
            out int? denouncingReplyCount)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Denouncing_GetCount";
                db.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        denouncingPhotoCount = reader.GetInt32(0);
                    }
                    else
                    {
                        denouncingPhotoCount = 0;
                    }

                    reader.NextResult();

                    if (reader.Read())
                    {
                        denouncingArticleCount = reader.GetInt32(0);
                    }
                    else
                    {
                        denouncingArticleCount = 0;
                    }

                    reader.NextResult();

                    if (reader.Read())
                    {
                        denouncingShareCount = reader.GetInt32(0);
                    }
                    else
                    {
                        denouncingShareCount = 0;
                    }

                    reader.NextResult();

                    if (reader.Read())
                    {
                        denouncingUserCount = reader.GetInt32(0);
                    }
                    else
                    {
                        denouncingUserCount = 0;
                    }

                    reader.NextResult();


                    if (reader.Read())
                    {
                        denouncingTopicCount = reader.GetInt32(0);
                    }
                    else
                    {
                        denouncingTopicCount = 0;
                    }

                    reader.NextResult();

                    if (reader.Read())
                    {
                        denouncingReplyCount = reader.GetInt32(0);
                    }
                    else
                    {
                        denouncingReplyCount = 0;
                    }
                }
            }
        }

        private int[] GetTargetIDs(DenouncingCollection denouncings)
        {
            int[] ids = new int[denouncings.Count];

            for (int i = 0; i < ids.Length; i++ )
            {
                ids[i] = denouncings[i].TargetID;
            }

            return ids;
        }

        public override DenouncingCollection GetDenouncingWithArticle(DenouncingFilter filter, int pageNumber)
        {
            using (SqlSession db = new SqlSession())
            {
                DenouncingCollection denouncings = null;

                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;
                    query.Pager.TableName = "bx_Denouncings";
                    query.Pager.SortField = "DenouncingID";
                    query.Pager.IsDesc = filter.IsDesc;
                    query.Pager.SelectCount = true;

                    filter.Type = DenouncingType.Blog;

                    GetSearchDenouncingsCondition(query, filter);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        denouncings = new DenouncingCollection(reader);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                denouncings.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillDenouncingContents(denouncings, db);

                if (denouncings.Count > 0)
                {
                    int[] targetIDs = GetTargetIDs(denouncings);

                    BlogArticleCollection articles = null;

                    using (SqlQuery query = db.CreateQuery())
                    {
                        query.CommandText = "SELECT * FROM bx_BlogArticles WHERE ArticleID IN (@IDs)";

                        query.CreateInParameter<int>("@IDs", targetIDs);

                        using (XSqlDataReader reader = query.ExecuteReader())
                        {
                            articles = new BlogArticleCollection(reader);
                        }
                    }

                    BlogBO.Instance.ProcessKeyword(articles, ProcessKeywordMode.FillOriginalText);

                    for (int i = 0; i < denouncings.Count; i++)
                    {
                        for (int j = 0; j < articles.Count; j++)
                        {
                            if (denouncings[i].TargetID == articles[j].ArticleID)
                            {
                                denouncings[i].TargetArticle = articles[j];
                                break;
                            }
                        }
                    }
                }

                return denouncings;
            }
        }

        public override DenouncingCollection GetDenouncingWithPhoto(DenouncingFilter filter, int pageNumber)
        {
            using (SqlSession db = new SqlSession())
            {
                DenouncingCollection denouncings = null;

                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;
                    query.Pager.TableName = "bx_Denouncings";
                    query.Pager.SortField = "DenouncingID";
                    query.Pager.IsDesc = filter.IsDesc;
                    query.Pager.SelectCount = true;

                    filter.Type = DenouncingType.Photo;

                    GetSearchDenouncingsCondition(query, filter);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        denouncings = new DenouncingCollection(reader);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                denouncings.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillDenouncingContents(denouncings, db);

                if (denouncings.Count > 0)
                {
                    int[] targetIDs = GetTargetIDs(denouncings);

                    PhotoCollection photos = null;

                    using (SqlQuery query = db.CreateQuery())
                    {
                        query.CommandText = "SELECT * FROM bx_Photos WHERE PhotoID IN (@IDs)";

                        query.CreateInParameter<int>("@IDs", targetIDs);

                        using (XSqlDataReader reader = query.ExecuteReader())
                        {
                            photos = new PhotoCollection(reader);
                        }
                    }

                    AlbumBO.Instance.ProcessKeyword(photos, ProcessKeywordMode.FillOriginalText);

                    for (int i = 0; i < denouncings.Count; i++)
                    {
                        for (int j = 0; j < photos.Count; j++)
                        {
                            if (denouncings[i].TargetID == photos[j].PhotoID)
                            {
                                denouncings[i].TargetPhoto = photos[j];
                                break;
                            }
                        }
                    }
                }
                return denouncings;
            }
        }

        public override DenouncingCollection GetDenouncingWithShare(DenouncingFilter filter, int pageNumber)
        {
            using (SqlSession db = new SqlSession())
            {
                DenouncingCollection denouncings = null;

                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;
                    query.Pager.TableName = "bx_Denouncings";
                    query.Pager.SortField = "DenouncingID";
                    query.Pager.IsDesc = filter.IsDesc;
                    query.Pager.SelectCount = true;

                    filter.Type = DenouncingType.Share;

                    GetSearchDenouncingsCondition(query, filter);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        denouncings = new DenouncingCollection(reader);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                denouncings.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillDenouncingContents(denouncings, db);

                if (denouncings.Count > 0)
                {
                    int[] targetIDs = GetTargetIDs(denouncings);

                    ShareCollection shares = null;

                    using (SqlQuery query = db.CreateQuery())
                    {
                        query.CommandText = "SELECT * FROM bx_SharesView WHERE ShareID IN (@IDs)";

                        query.CreateInParameter<int>("@IDs", targetIDs);

                        using (XSqlDataReader reader = query.ExecuteReader())
                        {
                            shares = new ShareCollection(reader);
                        }
                    }

                    ShareBO.Instance.ProcessKeyword(shares, ProcessKeywordMode.FillOriginalText);

                    for (int i = 0; i < denouncings.Count; i++)
                    {
                        for (int j = 0; j < shares.Count; j++)
                        {
                            if (denouncings[i].TargetID == shares[j].ShareID)
                            {
                                denouncings[i].TargetShare = shares[j];
                                break;
                            }
                        }
                    }
                }
                return denouncings;
            }
        }

        public override DenouncingCollection GetDenouncingWithUser(DenouncingFilter filter, int pageNumber)
        {
            using (SqlSession db = new SqlSession())
            {
                DenouncingCollection denouncings = null;

                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;
                    query.Pager.TableName = "bx_Denouncings";
                    query.Pager.SortField = "DenouncingID";
                    query.Pager.IsDesc = filter.IsDesc;
                    query.Pager.SelectCount = true;

                    filter.Type = DenouncingType.Space;

                    GetSearchDenouncingsCondition(query, filter);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        denouncings = new DenouncingCollection(reader);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                denouncings.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillDenouncingContents(denouncings, db);

                if (denouncings.Count > 0)
                {
                    UserBO.Instance.WaitForFillSimpleUsers<Denouncing>(denouncings);
                }

                return denouncings;
            }
        }

        public override DenouncingCollection GetDenouncingWithTopic(DenouncingFilter filter, int pageNumber)
        {
            using (SqlSession db = new SqlSession())
            {
                DenouncingCollection denouncings = null;

                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;
                    query.Pager.TableName = "bx_Denouncings";
                    query.Pager.SortField = "DenouncingID";
                    query.Pager.IsDesc = filter.IsDesc;
                    query.Pager.SelectCount = true;

                    filter.Type = DenouncingType.Topic;

                    GetSearchDenouncingsCondition(query, filter);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        denouncings = new DenouncingCollection(reader);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                denouncings.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillDenouncingContents(denouncings, db);

                if (denouncings.Count > 0)
                {
                    int[] targetIDs = GetTargetIDs(denouncings);

                    ThreadCollectionV5 threads = PostBOV5.Instance.GetThreads(targetIDs);

                    for (int i = 0; i < denouncings.Count; i++)
                    {
                        for (int j = 0; j < threads.Count; j++)
                        {
                            if (denouncings[i].TargetID == threads[j].ThreadID)
                            {
                                denouncings[i].TargetTopic = threads[j];
                                break;
                            }
                        }
                    }

                }
                return denouncings;
            }
        }

        public override DenouncingCollection GetDenouncingWithReply(DenouncingFilter filter, int pageNumber)
        {
            using (SqlSession db = new SqlSession())
            {
                DenouncingCollection denouncings = null;

                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;
                    query.Pager.TableName = "bx_Denouncings";
                    query.Pager.SortField = "DenouncingID";
                    query.Pager.IsDesc = filter.IsDesc;
                    query.Pager.SelectCount = true;

                    filter.Type = DenouncingType.Reply;

                    GetSearchDenouncingsCondition(query, filter);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        denouncings = new DenouncingCollection(reader);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                denouncings.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillDenouncingContents(denouncings, db);

                if (denouncings.Count > 0)
                {
                    int[] targetIDs = GetTargetIDs(denouncings);

                    PostCollectionV5 posts = PostBOV5.Instance.GetPosts(targetIDs);

                    for (int i = 0; i < denouncings.Count; i++)
                    {
                        for (int j = 0; j < posts.Count; j++)
                        {
                            if (denouncings[i].TargetID == posts[j].ThreadID)
                            {
                                denouncings[i].TargetReply = posts[j];
                                break;
                            }
                        }
                    }

                }
                return denouncings;
            }
        }
    }
}