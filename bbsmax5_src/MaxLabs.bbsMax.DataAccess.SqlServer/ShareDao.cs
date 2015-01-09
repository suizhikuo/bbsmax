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
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class ShareDao : DataAccess.ShareDao
	{
		#region StoredProcedure
		[StoredProcedure(Name = "bx_Share_GetPostShareCount", Script = @"
CREATE PROCEDURE {name}
	@UserID		int,
	@BeginDate	datetime,
	@EndDate	datetime
AS
BEGIN
	SET NOCOUNT ON;

	SELECT COUNT(*) FROM [bx_SharesView] WHERE [UserID] = @UserID AND [CreateDate] BETWEEN @BeginDate AND @EndDate;
END")]
		#endregion
		public override int GetPostShareCount(int userID, DateTime beginDate, DateTime endDate)
		{
			using (SqlQuery db = new SqlQuery())
			{
				db.CommandText = "bx_Share_GetPostShareCount";
				db.CommandType = CommandType.StoredProcedure;

				db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
				db.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
				db.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

				return db.ExecuteScalar<int>();
			}
		}

		public override ShareCollection GetUserFavorites(int favOwnerID, ShareType? favType, int pageNumber, int pageSize, ref int? totalCount)
		{
			using (SqlQuery query = new SqlQuery())
			{
                query.Pager.TableName = "[bx_SharesView]";
				query.Pager.SortField = "[ShareID]";
				query.Pager.IsDesc = true;
				query.Pager.PageNumber = pageNumber;
				query.Pager.PageSize = pageSize;
				query.Pager.TotalRecords = totalCount;
				query.Pager.SelectCount = true;

				SqlConditionBuilder cb = new SqlConditionBuilder(SqlConditionStart.None);

				cb.Append("[UserID] = @UserID");

				query.CreateParameter<int>("@UserID", favOwnerID, SqlDbType.Int);

				if (favType.HasValue && favType.Value != ShareType.All)
				{
					cb.Append("[Type] = @Type");

					query.CreateParameter<ShareType>("@Type", favType.Value, SqlDbType.TinyInt);
				}

				cb.Append("[PrivacyType] = 2");

				query.Pager.Condition = cb.ToString();

				using (XSqlDataReader reader = query.ExecuteReader())
				{
                    ShareCollection shares = new ShareCollection(reader);

                    if (reader.NextResult())
					{
                        if (totalCount == null && reader.Read())
						{
                            totalCount = reader.Get<int>(0);
						}

						shares.TotalRecords = totalCount.GetValueOrDefault();
					}

					return shares;
				}
			}
		}

        [StoredProcedure(Name="bx_Share_GetUserShare", Script= @"
CREATE PROCEDURE {name}
    @UserShareID int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM bx_SharesView WHERE UserShareID=@UserShareID;
END")]
        public override Share GetUserShares(int userShareID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_Share_GetUserShare";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserShareID", userShareID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new Share(reader);

                    return null;
                }
            }
        } 

		public override ShareCollection GetUserShares(int shareOwnerID, ShareType? shareType, DataAccessLevel dataAccessLevel, int pageNumber, int pageSize, ref int? totalCount)
		{
			using (SqlQuery query = new SqlQuery())
			{
                query.Pager.TableName = "[bx_SharesView]";
				query.Pager.SortField = "[UserShareID]";
				query.Pager.IsDesc = true;
				query.Pager.PageNumber = pageNumber;
				query.Pager.PageSize = pageSize;
				query.Pager.TotalRecords = totalCount;
				query.Pager.SelectCount = true;

				SqlConditionBuilder cb = new SqlConditionBuilder(SqlConditionStart.None);

				cb.Append("[UserID2] = @UserID");

				query.CreateParameter<int>("@UserID", shareOwnerID, SqlDbType.Int);

				if (shareType.HasValue && shareType.Value != ShareType.All)
				{
					cb.Append("[Type] = @Type");

					query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
				}

				if (dataAccessLevel == DataAccessLevel.Normal)
				{
					cb.Append("[PrivacyType] IN (0, 3)");
				}
				else if (dataAccessLevel == DataAccessLevel.Friend)
				{
					cb.Append("[PrivacyType] IN (0, 1, 3)");
				}
				else if(dataAccessLevel == DataAccessLevel.DataOwner)
				{
					cb.Append("[PrivacyType] != 2");
				}

				query.Pager.Condition = cb.ToString();

                ShareCollection shares;
				using (XSqlDataReader reader = query.ExecuteReader())
				{
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
					{
                        if (totalCount == null && reader.Read())
						{
                            totalCount = reader.Get<int>(0);
						}

						shares.TotalRecords = totalCount.GetValueOrDefault();
					}

				}
                FillComments(shares, query);

				return shares;
			}
		}

		public override ShareCollection GetFriendShares(int userID, ShareType? shareType, int pageNumber, int pageSize)
		{
			using (SqlQuery query = new SqlQuery())
			{
                query.Pager.TableName = "[bx_SharesView]";
				query.Pager.SortField = "ShareID";
				query.Pager.PageNumber = pageNumber;
				query.Pager.PageSize = pageSize;
				query.Pager.IsDesc = true;
				query.Pager.SelectCount = true;

                StringBuilder sql = new StringBuilder();
				if (shareType.HasValue&& shareType.Value != ShareType.All)
				{
                    sql.Append(" [Type] = @Type AND ");

					query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
				}

                sql.Append(" [UserID2] IN (SELECT [FriendUserID] FROM [bx_Friends] WHERE [UserID] = @UserID) AND ([PrivacyType] IN (0,1,3)) ");
				query.Pager.Condition = sql.ToString();

				query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                ShareCollection shares;
				using (XSqlDataReader reader = query.ExecuteReader())
				{
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
					{
                        if (reader.Read())
						{
                            shares.TotalRecords = reader.Get<int>(0);
						}
                    }
                }
                FillComments(shares, query);
                return shares;
			}
		}


        public override ShareCollection GetHotShares(ShareType? shareType, DateTime? beginDate, HotShareSortType sortType, int pageNumber, int pageSize, out int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.PrimaryKey = "[UserShareID]";
                if (sortType == HotShareSortType.ShareCount)
                    query.Pager.SortField = "ShareCount";
                else
                    query.Pager.SortField = " [AgreeAndOpposeCount] ";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;

                StringBuilder sql = new StringBuilder(" UserShareID IN(SELECT MIN(UserShareID) FROM bx_SharesView WHERE ");
                if (shareType.HasValue && shareType.Value != ShareType.All)
                {
                    sql.Append(" [Type] = @Type AND ");

                    query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
                }

                if (beginDate != null)
                {
                    sql.Append(" [CreateDate] >= @BeginDate AND ");
                    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                }

                sql.Append(" [PrivacyType] = 0 GROUP BY ShareID) ");
                query.Pager.Condition = sql.ToString();

                ShareCollection shares;
                totalCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }
                FillComments(shares, query);
                return shares;
            }
        }


		public override ShareCollection GetEveryoneShares(ShareType? shareType, int pageNumber, int pageSize, ref int? totalCount)
		{
			using (SqlQuery query = new SqlQuery())
			{
                query.Pager.TableName = "[bx_SharesView]";
				query.Pager.SortField = "[ShareID]";
				query.Pager.PageNumber = pageNumber;
				query.Pager.PageSize = pageSize;
				query.Pager.TotalRecords = totalCount;
				query.Pager.IsDesc = true;
				query.Pager.SelectCount = true;

                StringBuilder sql = new StringBuilder(" UserShareID IN(SELECT MIN(UserShareID) FROM bx_SharesView WHERE");
				if (shareType.HasValue && shareType.Value != ShareType.All)
				{
                    sql.Append(" [Type] = @Type AND ");

					query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
				}
                sql.Append(" [PrivacyType] = 0  GROUP BY ShareID) ");

                query.Pager.Condition = sql.ToString();

                ShareCollection shares;
				using (XSqlDataReader reader = query.ExecuteReader())
				{
                    shares = new ShareCollection(reader);

                    if (totalCount == null && reader.NextResult())
					{
                        if (reader.Read())
						{
                            totalCount = reader.Get<int>(0);
						}
					}

					shares.TotalRecords = totalCount.GetValueOrDefault();

				}
                FillComments(shares, query);
				return shares;
			}
		}

		public override ShareCollection GetSharesBySearch(Guid[] excludeRoleIDs, ShareFilter filter, int pageNumber)
		{
			using (SqlQuery query = new SqlQuery())
			{
				string sqlCondition = BuildConditionsByFilter(query, filter, excludeRoleIDs, false);

                query.Pager.TableName = "[bx_SharesView]";
				query.Pager.SortField = filter.Order.ToString();

				if (filter.Order != ShareFilter.OrderBy.ShareID)
				{
					query.Pager.PrimaryKey = "ShareID";
				}

				query.Pager.PageNumber = pageNumber;
				query.Pager.PageSize = filter.PageSize;
				query.Pager.SelectCount = true;
				query.Pager.IsDesc = filter.IsDesc;

				query.Pager.Condition = sqlCondition;


				using (XSqlDataReader reader = query.ExecuteReader())
				{
					ShareCollection shares = new ShareCollection(reader);

					if (reader.NextResult())
					{
						if (reader.Read())
						{
							shares.TotalRecords = reader.Get<int>(0);
						}
					}

					return shares;
				}
			}
		}

		/**************************************
		 *      Delete开头的函数删除数据      *
		 **************************************/

		public override DeleteResult DeleteShares(int operatorID, int[] shareIDs, IEnumerable<Guid> excludeRoleIDs)
		{
			using (SqlQuery query = new SqlQuery())
			{
				string excludeRolesSql = DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);

				if (string.IsNullOrEmpty(excludeRolesSql) == false)
					excludeRolesSql = " AND ([UserID] = @UserID OR " + excludeRolesSql + ")";

				string sql = @"
DECLARE @DeleteData table (UserID int, ShareID int);

INSERT INTO @DeleteData SELECT [UserID],[ShareID] FROM [bx_SharesView] WHERE [ShareID] IN (@ShareIDs)" + excludeRolesSql + @";

DELETE [bx_Shares] WHERE ShareID IN (SELECT [ShareID] FROM @DeleteData);

SELECT [UserID],COUNT(*) AS [Count] FROM @DeleteData GROUP BY [UserID];";

				query.CommandText = sql;

				query.CreateInParameter<int>("@ShareIDs", shareIDs);
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

		public override DeleteResult DeleteSharesBySearch(ShareFilter filter, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount)
		{
			deletedCount = 0;

			using (SqlQuery query = new SqlQuery())
			{
				string conditions = BuildConditionsByFilter(query, filter, excludeRoleIDs, true);

				StringBuffer sql = new StringBuffer();

				sql += @"
DECLARE @DeleteData table (UserID int, ShareID int);

INSERT INTO @DeleteData SELECT TOP (@TopCount) [UserID],[ShareID] FROM [bx_SharesView] " + conditions + @";

DELETE [bx_Shares] WHERE [ShareID] IN (SELECT [ShareID] FROM @DeleteData);

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

		private string BuildConditionsByFilter(SqlQuery query, ShareFilter filter, IEnumerable<Guid> excludeRoleIDs, bool startWithWhere)
		{
			SqlConditionBuilder condition = new SqlConditionBuilder(startWithWhere ? SqlConditionStart.Where : SqlConditionStart.None);

			if (filter.UserID != null)
			{
				condition.AppendAnd("[UserID] = @UserID");

				query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
			}

			if (filter.ShareID != null)
			{
				condition.AppendAnd("[ShareID] = @ShareID");

				query.CreateParameter<int?>("@ShareID", filter.ShareID, SqlDbType.Int);
			}

			if (filter.ShareType != null)
			{
				condition.AppendAnd("[Type] = @Type");

				query.CreateParameter<ShareType?>("@Type", filter.ShareType, SqlDbType.Int);
			}

			if (filter.PrivacyType != null)
			{
				condition.AppendAnd("[PrivacyType] = @PrivacyType");

				query.CreateParameter<PrivacyType?>("@PrivacyType", filter.PrivacyType, SqlDbType.Int);
			}

			if (filter.BeginDate != null)
			{
				condition.AppendAnd("[CreateDate] > @BeginDate");

				query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
			}

			if (filter.EndDate != null)
			{
				condition.AppendAnd("[CreateDate] < @EndDate");

				query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);
			}

			condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query));

			return condition.ToString();
		}

		#region=========↓关键字↓==================================================================================================

//        public override void FillShareReverters(TextRevertable2Collection processlist)
//        {
//            if (processlist == null || processlist.Count == 0)
//                return;

//            using (SqlQuery query = new SqlQuery())
//            {
//                query.CommandText = "SELECT * FROM bx_ShareReverters WHERE ShareID IN (@ShareIDs)";

//                query.CreateInParameter<int>("@ShareIDs", processlist.GetIds());

//                using (XSqlDataReader reader = query.ExecuteReader())
//                {
//                    SqlDataReaderWrap readerWrap = new SqlDataReaderWrap(reader);

//                    while (readerWrap.Next)
//                    {
//                        int shareID = readerWrap.Get<int>("ShareID");

//                        string contentReverter = readerWrap.Get<string>("ContentReverter");
//                        string descriptionReverter = readerWrap.Get<string>("DescriptionReverter");

//                        processlist.FillReverter(shareID, descriptionReverter, contentReverter);
//                    }
//                }
//            }
//        }

//        #region StoredProcedure
//        [StoredProcedure(Name = "bx_Share_UpdateShareKeywords", Script = @"
//CREATE PROCEDURE {name}
//    @ShareID				int,
//    @Description			nvarchar(1000),
//    @DescriptionVersion		varchar(32),
//    @DescriptionReverter	nvarchar(4000),
//    @Content				nvarchar(3000),
//    @ContentVersion			varchar(32),
//    @ContentReverter		ntext
//AS BEGIN
//    SET NOCOUNT ON;
//
//    IF (@Description IS NOT NULL AND @Content IS NOT NULL)
//        UPDATE [bx_Shares] SET Description = @Description, DescriptionVersion = @DescriptionVersion, Content = @Content, ContentVersion = @ContentVersion WHERE ShareID = @ShareID;
//    ELSE IF (@Description IS NOT NULL)
//        UPDATE [bx_Shares] SET Description = @Description, DescriptionVersion = @DescriptionVersion WHERE ShareID = @ShareID;
//    ELSE IF (@Content IS NOT NULL)
//        UPDATE [bx_Shares] SET Content = @Content, ContentVersion = @ContentVersion WHERE ShareID = @ShareID;
//
//    IF (@DescriptionReverter IS NOT NULL AND @ContentReverter IS NOT NULL) BEGIN
//        UPDATE [bx_ShareReverters] SET DescriptionReverter = @DescriptionReverter, ContentReverter = @ContentReverter WHERE ShareID = @ShareID;
//        IF @@ROWCOUNT = 0
//            INSERT INTO bx_ShareReverters (ShareID, DescriptionReverter, ContentReverter) VALUES (@ShareID, @DescriptionReverter, @ContentReverter);
//    END
//    ELSE IF (@DescriptionReverter IS NOT NULL) BEGIN
//        UPDATE [bx_ShareReverters] SET DescriptionReverter = @DescriptionReverter WHERE ShareID = @ShareID;
//        IF @@ROWCOUNT = 0
//            INSERT INTO bx_ShareReverters (ShareID, DescriptionReverter, ContentReverter) VALUES (@ShareID, @DescriptionReverter, N'');
//    END
//    ELSE IF (@ContentReverter IS NOT NULL) BEGIN
//        UPDATE [bx_ShareReverters] SET ContentReverter = @ContentReverter WHERE ShareID = @ShareID;
//        IF @@ROWCOUNT = 0
//            INSERT INTO bx_ShareReverters (ShareID, DescriptionReverter, ContentReverter) VALUES (@ShareID, N'', @ContentReverter);
//    END
//
//END")]
//        #endregion
//        public override void UpdateShareKeywords(TextRevertable2Collection processlist)
//        {
//            if (processlist == null || processlist.Count == 0)
//                return;

//            using (SqlQuery query = new SqlQuery())
//            {
//                StringBuilder sql = new StringBuilder();

//                string reverter;

//                int i = 0;

//                foreach (ITextRevertable2 content in processlist)
//                {
//                    sql.AppendFormat("EXEC bx_Share_UpdateShareKeywords @ShareID_{0}, @Description_{0}, @DescriptionVersion_{0}, @DescriptionReverter_{0}, @Content_{0}, @ContentVersion_{0}, @ContentReverter_{0};", i);

//                    query.CreateParameter<int>("@ShareID_" + i, content.GetKey(), SqlDbType.Int);

//                    reverter = processlist.GetReverter1(content.GetKey());

//                    if (reverter == null)
//                    {
//                        query.CreateParameter<string>("@Description_" + i, null, SqlDbType.NVarChar, 1000);
//                        query.CreateParameter<string>("@DescriptionVersion_" + i, null, SqlDbType.VarChar, 32);
//                        query.CreateParameter<string>("@DescriptionReverter_" + i, null, SqlDbType.NVarChar, 4000);
//                    }
//                    else
//                    {
//                        query.CreateParameter<string>("@Description_" + i, content.Text1, SqlDbType.NVarChar, 1000);
//                        query.CreateParameter<string>("@DescriptionVersion_" + i, content.KeywordVersion, SqlDbType.VarChar, 32);
//                        query.CreateParameter<string>("@DescriptionReverter_" + i, reverter, SqlDbType.NVarChar, 4000);
//                    }



//                    reverter = processlist.GetReverter2(content.GetKey());

//                    if (reverter == null)
//                    {
//                        query.CreateParameter<string>("@Content_" + i, null, SqlDbType.NVarChar, 3000);
//                        query.CreateParameter<string>("@ContentVersion_" + i, null, SqlDbType.VarChar, 32);
//                        query.CreateParameter<string>("@ContentReverter_" + i, null, SqlDbType.NText);
//                    }
//                    else
//                    {
//                        query.CreateParameter<string>("@Content_" + i, content.Text2, SqlDbType.NVarChar, 3000);
//                        query.CreateParameter<string>("@ContentVersion_" + i, content.TextVersion2, SqlDbType.VarChar, 32);
//                        query.CreateParameter<string>("@ContentReverter_" + i, reverter, SqlDbType.NText);
//                    }

//                    i++;
//                }

//                if (sql.Length > 0)
//                {
//                    query.CommandText = sql.ToString();

//                    query.ExecuteNonQuery();
//                }
//            }
//        }

		#endregion

		#region 存储过程 bx_CreateShare
		[StoredProcedure(Name = "bx_CreateShare", Script = @"
CREATE PROCEDURE {name}
     @UserID               int
    ,@Type                 tinyint
    ,@PrivacyType          tinyint
    ,@Title                nvarchar(100)
    ,@Url                  nvarchar(512)
    ,@Content              nvarchar(2800)
    ,@Description          nvarchar(1000)
    ,@SortOrder            bigint
    ,@TargetID             int
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [bx_Shares] ([UserID], [Type], [Content], [SortOrder],[TargetID],[Url])
    VALUES(@UserID, @Type, @Content, @SortOrder, @TargetID, @Url);

    DECLARE @ShareID int;

    SELECT @ShareID = @@IDENTITY;

    IF @Type = 9 BEGIN --- 主题
        IF @PrivacyType = 2
            UPDATE bx_Threads SET CollectionCount = CollectionCount+1 WHERE ThreadID = @TargetID;
        ELSE
            UPDATE bx_Threads SET ShareCount = ShareCount+1 WHERE ThreadID = @TargetID;
    END 

    INSERT INTO [bx_UserShares] ([UserID], [ShareID], [PrivacyType], [Subject], [Description]) 
    VALUES (@UserID, @ShareID, @PrivacyType, @Title, @Description);

    SELECT @ShareID;
END
"
            )]
        #endregion
        public override int CreateShare(int userID, ShareType shareType, PrivacyType privacyType, string title, string url, string content, string description, string descriptionReverter, int targetID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateShare";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)shareType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@PrivacyType", (int)privacyType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Title", title, SqlDbType.NVarChar, 100);
                query.CreateParameter<string>("@Url", url, SqlDbType.NVarChar, 512);
                query.CreateParameter<string>("@Content", content, SqlDbType.NVarChar, 2800);
                query.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 1000);
                query.CreateParameter<long>("@SortOrder", DateTime.Now.Date.Ticks, SqlDbType.BigInt);
                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);

                return query.ExecuteScalar<int>();
            }

        }


        #region 存储过程 bx_GetShare
        [StoredProcedure(Name = "bx_GetShare", Script = @"
CREATE PROCEDURE {name}
     @ShareID   int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_SharesView] WHERE [ShareID] = @ShareID;
END
"
            )]
        #endregion
        public override Share GetShare(int shareID)
        {
            Share share = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetShare";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@ShareID", shareID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        share = new Share(reader);
                    }
                }
            }
            return share;
        }


        public override ShareCollection GetShares(IEnumerable<int> shareIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {

                query.CommandText = "SELECT * FROM [bx_SharesView] WHERE [ShareID] IN (@ShareIDs);";

                query.CreateInParameter<int>("@ShareIDs", shareIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ShareCollection(reader);
                }
            }
        }


        /// <summary>
        /// 获取用户分享
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="shareType">为null时 表示获取所有类型分享</param>
        /// <param name="privacyType"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public override ShareCollection GetUserShares(int userID, ShareType shareType, bool? includeFriendVisiable, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {

                StringBuffer conditions = new StringBuffer();

                conditions += "UserID = @UserID";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                if (shareType != ShareType.All)
                {
                    conditions += " AND [Type] = @ShareType";
                    query.CreateParameter<int>("@ShareType", (int)shareType, SqlDbType.TinyInt);
                }

                if (includeFriendVisiable != null)
                {
                    if (includeFriendVisiable.Value)
                        conditions += " AND ([PrivacyType] < 2)";
                    else
                        conditions += " AND ([PrivacyType] = 0)";

                }

                query.Pager.IsDesc = true;
                query.Pager.SortField = "UserShareID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.Condition = conditions.ToString();

                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }
                FillComments(shares, query);
                return shares;
            }

        }


        public override ShareCollection GetFriendShares(int userID, ShareType shareType, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                StringBuffer conditions = new StringBuffer();

                conditions += "UserID IN (@UserIDs)";
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                if (shareType != ShareType.All)
                {
                    conditions += " AND [Type] = @ShareType";
                    query.CreateParameter<int>("@ShareType", (int)shareType, SqlDbType.TinyInt);
                }

                conditions += " AND [PrivacyType] < 2";

                query.Pager.IsDesc = true;
                query.Pager.SortField = "UserShareID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.Condition = conditions.ToString();

                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }
                FillComments(shares, query);
                return shares;
            }
        }

        public override ShareCollection GetUserCommentedShares(int userID, ShareType? shareType, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.SortField = "UserShareID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;
                query.Pager.Condition = "[UserShareID] IN (SELECT [TargetID] FROM [bx_Comments] WHERE [Type]=5 AND [UserID] = @UserID AND IsApproved = 1)";

                if (shareType != null && shareType != ShareType.All)
                {
                    query.Pager.Condition += " AND [Type] = @ShareType";
                    query.CreateParameter<int>("@ShareType", (int)shareType, SqlDbType.TinyInt);
                }

                query.Pager.Condition += " AND [PrivacyType] < 2";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            shares.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }
                FillComments(shares, query);
                return shares;
            }
        }


        public override ShareCollection GetAllUserShares(ShareType shareType, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.SortField = "UserShareID";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.Condition = @"
    (@ShareType = 0 OR [Type] = @ShareType)
AND 
    [PrivacyType] = 0
";

                query.CreateParameter<int>("@ShareType", (int)shareType, SqlDbType.TinyInt);

                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }
                FillComments(shares, query);
                return shares;
            }

        }


        public override ShareCollection SearchShares(int pageNumber, ShareFilter filter, IEnumerable<Guid>  excludeRoleIDs, ref int totalCount)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = filter.IsDesc;

                if (filter.Order == ShareFilter.OrderBy.ShareID)
                    query.Pager.SortField = filter.Order.ToString();

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_SharesView]";

                SqlConditionBuilder condition = new SqlConditionBuilder(SqlConditionStart.None);
                condition += (filter.UserID == null ? "" : ("AND [UserID] = @UserID "));
                condition += (filter.ShareID == null ? "" : ("AND [ShareID] = @ShareID "));
                condition += (filter.ShareType == ShareType.All ? "" : ("AND [Type] = @Type "));
                condition += (filter.PrivacyType == null ? "" : ("AND [PrivacyType] = @PrivacyType "));
                condition += (filter.BeginDate == null ? "" : ("AND [CreateDate] > @BeginDate "));
                condition += (filter.EndDate == null ? "" : ("AND [CreateDate] < @EndDate "));
                condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query));

                query.Pager.Condition = condition.ToString();



                query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
                query.CreateParameter<int?>("@ShareID", filter.ShareID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)filter.ShareType, SqlDbType.Int);
                query.CreateParameter<int?>("@PrivacyType", filter.PrivacyType == null ? 0 : (int)filter.PrivacyType, SqlDbType.Int);
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);



                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ShareCollection shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                    return shares;
                }
            }
        }

        public override DeleteResult DeleteSearchShares(ShareFilter filter, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult)
        {

            using (SqlQuery query = new SqlQuery())
            {

                //string condition = @" 1 = 1 "
                //        + (filter.UserID == null ? "" : ("AND [UserID] = @UserID "))
                //        + (filter.ShareID == null ? "" : ("AND [ShareID] = @ShareID "))
                //        + (filter.ShareCatagory == ShareCatagory.All ? "" : ("AND [Type] = @Type "))
                //        + (filter.PrivacyType == null ? "" : ("AND [PrivacyType] = @PrivacyType "))
                //        + (filter.BeginDate == null ? "" : ("AND [CreateDate] > @BeginDate "))
                //        + (filter.EndDate == null ? "" : ("AND [CreateDate] < @EndDate "))
                //        + "AND " + DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query);

                SqlConditionBuilder condition = new SqlConditionBuilder(SqlConditionStart.Where);
                condition += (filter.UserID == null ? "" : ("AND [UserID] = @UserID "));
                condition += (filter.ShareID == null ? "" : ("AND [ShareID] = @ShareID "));
                condition += (filter.ShareType == ShareType.All ? "" : ("AND [Type] = @Type "));
                condition += (filter.PrivacyType == null ? "" : ("AND [PrivacyType] = @PrivacyType "));
                condition += (filter.BeginDate == null ? "" : ("AND [CreateDate] > @BeginDate "));
                condition += (filter.EndDate == null ? "" : ("AND [CreateDate] < @EndDate "));
                condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query));

                if (getDeleteResult)
                {
                    query.CommandText = @"
SELECT [UserID],COUNT(*) AS [Count] FROM [bx_SharesView] " + condition.ToString() + " GROUP BY [UserID];";

                }
                else
                    query.CommandText = string.Empty;


                query.CommandText = query.CommandText + @"
DELETE [bx_SharesView] " + condition.ToString();


                query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
                query.CreateParameter<int?>("@ShareID", filter.ShareID, SqlDbType.Int);
                query.CreateParameter<int>("@Type", (int)filter.ShareType, SqlDbType.Int);
                query.CreateParameter<int?>("@PrivacyType", filter.PrivacyType == null ? 0 : (int)filter.PrivacyType, SqlDbType.Int);
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);



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

        //public override void TryUpdateKeyword(TextReverterCollection_Temp replaceKeywordContents)
        //{

        //    using (SqlQuery query = new SqlQuery())
        //    {
        //        StringBuilder sql = new StringBuilder();

        //        int i = 0;
        //        foreach (TextReverter_Temp content in replaceKeywordContents)
        //        {
        //            sql.AppendFormat(@"UPDATE [bx_Shares] SET [Description] = @Description_{0}, [DescriptionReverter] = @DescriptionReverter_{0} WHERE [ShareID] = @ShareID_{0};", i);

        //            query.CreateParameter<string>("@Description_" + i, content.Text1, SqlDbType.NVarChar, 500);
        //            query.CreateParameter<string>("@DescriptionReverter_" + i, content.TextReverter1, SqlDbType.NVarChar, 500);
        //            query.CreateParameter<int>("@ShareID_" + i, content.ID, SqlDbType.Int);

        //            i++;
        //        }

        //        query.CommandText = sql.ToString();
        //        query.ExecuteNonQuery();
        //    }

        //}

        public override Revertable2Collection<Share> GetSharesWithReverters(IEnumerable<int> shareIDs)
        {
            if (ValidateUtil.HasItems(shareIDs) == false)
                return null;

            Revertable2Collection<Share> shares = new Revertable2Collection<Share>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	SubjectReverter = ISNULL(R.SubjectReverter, ''),
	DescriptionReverter = ISNULL(R.DescriptionReverter, '')
FROM 
	[bx_SharesView] A WITH(NOLOCK)
LEFT JOIN 
	bx_UserShareReverters R WITH(NOLOCK) ON R.UserShareID = A.UserShareID
WHERE 
	A.ShareID IN (@ShareIDs)";

                query.CreateInParameter<int>("@ShareIDs", shareIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string subjectReverter = reader.Get<string>("SubjectReverter");
                        string descriptionReverter = reader.Get<string>("DescriptionReverter");

                        Share share = new Share(reader);

                        shares.Add(share, subjectReverter, descriptionReverter);
                    }
                }
            }

            return shares;
        }

        public override RevertableCollection<Share> GetSharesWithReverters1(IEnumerable<int> shareIDs)
        {
            if (ValidateUtil.HasItems(shareIDs) == false)
                return null;

            RevertableCollection<Share> shares = new RevertableCollection<Share>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	ContentReverter = ISNULL(R.ContentReverter, '')
FROM 
	[bx_SharesView] A WITH(NOLOCK)
LEFT JOIN 
	bx_ShareReverters R WITH(NOLOCK) ON R.ShareID = A.ShareID
WHERE 
	A.ShareID IN (@ShareIDs)";

                query.CreateInParameter<int>("@ShareIDs", shareIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string contentReverter = reader.Get<string>("ContentReverter");

                        Share share = new Share(reader);

                        shares.Add(share, contentReverter);
                    }
                }
            }

            return shares;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Share_UpdateShareKeywords", Script = @"
CREATE PROCEDURE {name}
    @ShareID               int,
    @KeywordVersion        varchar(32),
    @Subject               nvarchar(1000),
    @SubjectReverter       nvarchar(4000),
    @Description           ntext,
    @DescriptionReverter   ntext
AS BEGIN

/* include : Procedure_UpdateKeyword2.sql

    {PrimaryKey} = UserShareID
    {PrimaryKeyParam} = @ShareID


    {Table} = [bx_UserShares]
    {Text1} = Subject
    {Text1Param} = @Subject

    {Text2} = Description
    {Text2Param} = @Description


    {RevertersTable} = bx_UserShareReverters
    {Text1Reverter} = SubjectReverter
    {Text1ReverterParam} = @SubjectReverter

    {Text2Reverter} = DescriptionReverter
    {Text2ReverterParam} = @DescriptionReverter

*/

END")]
        #endregion
        public override void UpdateShareKeywords(Revertable2Collection<Share> processlist)
        {
            string procedure = "bx_Share_UpdateShareKeywords";
            string table = "[bx_UserShares]";
            string primaryKey = "UserShareID";

            SqlDbType text1_Type = SqlDbType.NVarChar; int text1_Size = 1000;
            SqlDbType reve1_Type = SqlDbType.NVarChar; int reve1_Size = 4000;
            SqlDbType text2_Type = SqlDbType.NVarChar; int text2_Size = 2800;
            SqlDbType reve2_Type = SqlDbType.NText; //int reve2_Size = 4000;


            if (processlist == null || processlist.Count == 0)
                return;

            //有一部分项是不需要更新文本的（例如：只有版本或恢复信息发生了变化），把这部分的ID取出来，一次性更新以提高性能
            List<int> shareIDs = processlist.GetNeedUpdateButTextNotChangedKeys();


            List<int> needUpdateButTextNotChangedIds = new List<int>();
            foreach (int id in shareIDs)
            {
                Share share = processlist.GetValue(id).Value;
                if (needUpdateButTextNotChangedIds.Contains(share.UserShareID) == false)
                    needUpdateButTextNotChangedIds.Add(share.UserShareID);
            }

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
                foreach (Revertable2<Share> item in processlist)
                {
                    //此项确实需要更新，且不只是版本发生了变化
                    if (item.NeedUpdate && item.OnlyVersionChanged == false)
                    {
                        sql.InnerBuilder.AppendFormat(@"EXEC {1} @ID_{0}, @KeywordVersion_{0}, @Text1_{0}, @Reverter1_{0}, @Text2_{0}, @Reverter2_{0};", i, procedure);

                        query.CreateParameter<int>("@ID_" + i, item.Value.UserShareID, SqlDbType.Int);

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
                                query.CreateParameter<string>("@Text2_" + i, item.Value.Text2, text2_Type, text2_Size);
                            else
                                query.CreateParameter<string>("@Text2_" + i, null, text2_Type, text2_Size);
                        }
                        else
                        {
                            query.CreateParameter<string>("@KeywordVersion_" + i, null, SqlDbType.VarChar, 32);

                            query.CreateParameter<string>("@Text1_" + i, null, text1_Type, text1_Size);
                            query.CreateParameter<string>("@Text2_" + i, null, text2_Type, text2_Size);

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

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Share_UpdateShareKeywords1", Script = @"
CREATE PROCEDURE {name}
    @ShareID               int,
    @KeywordVersion        varchar(32),
    @Content               nvarchar(1000),
    @ContentReverter       nvarchar(4000)
AS BEGIN

/* include : Procedure_UpdateKeyword.sql

    {PrimaryKey} = ShareID
    {PrimaryKeyParam} = @ShareID


    {Table} = [bx_Shares]
    {Text} = Content
    {TextParam} = @Content

    {RevertersTable} = bx_ShareReverters
    {TextReverter} = ContentReverter
    {TextReverterParam} = @ContentReverter

*/

END")]
        #endregion
        public override void UpdateShareKeywords1(RevertableCollection<Share> processlist)
        {
            string procedure = "bx_Share_UpdateShareKeywords1";
            string table = "[bx_Shares]";
            string primaryKey = "ShareID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 1000;
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
                foreach (Revertable<Share> item in processlist)
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
        [StoredProcedure(Name="bx_Share_Agree", Script= @"
create procedure {name}
    @ShareID    int,
    @UserID     int
as
begin
    SET NOCOUNT ON

    if not exists(select * from bx_ShareAgreeOrOpposeLogs where ShareID = @ShareID and UserID = @UserID) begin

        insert into bx_ShareAgreeOrOpposeLogs (ShareID, UserID, IsAgree) values (@ShareID, @UserID, 1);

        update 
            bx_Shares
        set
            AgreeAndOpposeCount = AgreeCount + OpposeCount + 1,
            AgreeCount = AgreeCount + 1,
            SortOrder = SortOrder + 1
        where
            ShareID = @ShareID;

    end
    
end")]
        public override void AgreeShare(int userID, int shareID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Share_Agree";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ShareID", shareID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_Share_Oppose", Script = @"
create procedure {name}
    @ShareID    int,
    @UserID     int
as
begin
    SET NOCOUNT ON

    if not exists(select * from bx_ShareAgreeOrOpposeLogs where ShareID = @ShareID and UserID = @UserID) begin

        insert into bx_ShareAgreeOrOpposeLogs (ShareID, UserID, IsAgree) values (@ShareID, @UserID, 0);

        update 
            bx_Shares
        set
            AgreeAndOpposeCount = AgreeCount + OpposeCount + 1,
            OpposeCount = OpposeCount + 1,
            SortOrder = SortOrder + 1
        where
            ShareID = @ShareID;

    end

end")]
        public override void OpposeShare(int userID, int shareID)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Share_Oppose";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ShareID", shareID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name="bx_Share_ReShare", Script= @"
create procedure {name}
    @ShareID        int,
    @UserID         int,
    @PrivacyType    tinyint,
    @Subject        nvarchar(100),
    @Description    nvarchar(1000)
as
begin
    SET NOCOUNT ON
    
    insert into [bx_UserShares] (ShareID, UserID, PrivacyType, Subject, Description) values (@ShareID, @UserID, @PrivacyType, @Subject, @Description);

    update bx_Shares set ShareCount = ShareCount + 1 where ShareID = @ShareID;
    
    DECLARE @Type tinyint,@TargetID int;
    SELECT @Type = Type, @TargetID = TargetID FROM [bx_Shares] WHERE ShareID = @ShareID;
    IF @Type = 9 BEGIN --- 主题
        IF @PrivacyType = 2
            UPDATE bx_Threads SET CollectionCount = CollectionCount+1 WHERE ThreadID = @TargetID;
        ELSE
            UPDATE bx_Threads SET ShareCount = ShareCount+1 WHERE ThreadID = @TargetID;
    END 

end")]
        public override void ReShare(int userID, int shareID, PrivacyType privacyType, string subject, string description)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Share_ReShare";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@ShareID", shareID, SqlDbType.Int);
                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<PrivacyType>("@PrivacyType", privacyType, SqlDbType.TinyInt);
                db.CreateParameter<string>("@Subject", subject, SqlDbType.NVarChar, 100);
                db.CreateParameter<string>("@Description", description, SqlDbType.NVarChar, 1000);

                db.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name="bx_Share_GetAgreeStates", Script= @"
create procedure {name}
    @UserID     int,
    @ShareIDs   varchar(100)
as
begin
    SET NOCOUNT ON

    declare @sql nvarchar(500);

    set @sql = N'select ShareID, IsAgree from bx_ShareAgreeOrOpposeLogs where UserID = @uid and ShareID IN (' + @ShareIDs + ')';

    exec sp_executesql @sql, N'@uid int', @uid = @UserID;
end")]
        public override Hashtable GetAgreeStates(int userID, int[] shareIDs)
        {
            using (SqlQuery db = new SqlQuery())
            {
                db.CommandText = "bx_Share_GetAgreeStates";
                db.CommandType = CommandType.StoredProcedure;

                db.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                db.CreateParameter<string>("@ShareIDs", StringUtil.Join(shareIDs), SqlDbType.VarChar, 100);

                using (XSqlDataReader reader = db.ExecuteReader())
                {
                    Hashtable result = new Hashtable();

                    while (reader.Read())
                    {
                        result.Add(reader.Get<int>(0), reader.Get<bool>(1));
                    }

                    return result;
                }
            }
        }

//        [StoredProcedure(Name="bx_Share_GetComments", Script= @"
//create procedure {name}
//    @ShareID int
//as
//begin
//    SET NOCOUNT ON
//
//    SELECT * FROM [bx_Comments] WHERE [Type]= 5 AND [TargetID] = @ShareID ORDER BY CommentID ASC;
//end")]
//        public override CommentCollection GetShareComments(int shareID)
//        {
//            using (SqlQuery query = new SqlQuery())
//            {
//                query.CommandText = "bx_Share_GetComments";
//                query.CommandType = CommandType.StoredProcedure;

//                query.CreateParameter<int>("@ShareID", shareID, SqlDbType.Int);

//                using (XSqlDataReader reader = query.ExecuteReader())
//                {
//                    return new CommentCollection(reader);
//                }
//            }
//        }

        public override ShareCollection GetFriendSharesOrderByRank(int userID, ShareType? shareType, DateTime? beginDate, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.PrimaryKey = "[UserShareID]";
                query.Pager.SortField = "[UserShareID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;

                if (shareType.HasValue && shareType.Value != ShareType.All)
                {
                    query.Pager.Condition = "[Type] = @Type AND ";

                    query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
                }

                if (beginDate.HasValue)
                {
                    query.Pager.Condition = "[CreateDate] >= @BeginDate AND ";

                    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                }

                query.Pager.Condition = "[UserID] IN (SELECT [FriendUserID] FROM [bx_Friends] WHERE [UserID] = @UserID) AND ([PrivacyType] IN (0,1,3))";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            shares.TotalRecords = reader.Get<int>(0);
                        }
                    }
                }

                FillComments(shares, query);
                return shares;
            }
        }

        public override ShareCollection GetEveryoneSharesOrderByRank(ShareType? shareType, DateTime? beginDate, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.PrimaryKey = "[UserShareID]";
                query.Pager.SortField = "[UserShareID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;

                if (shareType.HasValue && shareType.Value != ShareType.All)
                {
                    query.Pager.Condition = "[Type] = @Type AND ";

                    query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
                }

                if (beginDate.HasValue)
                {
                    query.Pager.Condition = "[CreateDate] >= @BeginDate AND ";

                    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                }

                query.Pager.Condition = " ([PrivacyType] IN (0,3))";

                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            shares.TotalRecords = reader.Get<int>(0);
                        }
                    }

                }
                FillComments(shares, query);
                return shares;
            }
        }

        public override ShareCollection GetCommentedSharesOrderByRank(int userID, ShareType? shareType, int pageSize, int pageNumber)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.TableName = "[bx_SharesView]";
                query.Pager.PrimaryKey = "[UserShareID]";
                query.Pager.SortField = "[UserShareID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.IsDesc = true;
                query.Pager.SelectCount = true;

                query.Pager.Condition = "";

                if (shareType.HasValue && shareType.Value != ShareType.All)
                {
                    query.Pager.Condition = "[Type] = @Type AND ";

                    query.CreateParameter<ShareType>("@Type", shareType.Value, SqlDbType.TinyInt);
                }

                query.Pager.Condition =  query.Pager.Condition + " [UserShareID] IN (SELECT [TargetID] FROM [bx_Comments] WHERE [Type]=5 AND [UserID] = @UserID AND IsApproved = 1)";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                ShareCollection shares;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    shares = new ShareCollection(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            shares.TotalRecords = reader.Get<int>(0);
                        }
                    }

                }

                FillComments(shares, query);
                return shares;
            }
        }











        private void FillComments(ShareCollection shares, SqlQuery query)
        {
            if (shares.Count == 0)
                return;

            List<int> minIds = new List<int>();
            List<int> maxIds = new List<int>();

            for (int i = 0; i < shares.Count; i++)
            {
                if (shares[i].TotalComments == 0)
                    continue;
                else if (shares[i].TotalComments == 1)
                    minIds.Add(shares[i].UserShareID);
                else
                {
                    minIds.Add(shares[i].UserShareID);
                    maxIds.Add(shares[i].UserShareID);
                }
            }

            if (minIds.Count == 0)
                return;

            query.Parameters.Clear();
            query.CommandType = CommandType.Text;

            query.CommandText = @"
SELECT * FROM bx_Comments WHERE CommentID IN(
    SELECT Min(CommentID) FROM [bx_Comments] WHERE [Type]=5 AND IsApproved = 1 AND [TargetID] IN(@MinTargetIDs) GROUP BY TargetID
)
";
            if (maxIds.Count > 0)
            {
                query.CommandText += @"
 UNION 
SELECT * FROM bx_Comments WHERE CommentID IN(
    SELECT Max(CommentID) FROM [bx_Comments] WHERE [Type]=5 AND IsApproved = 1 AND [TargetID] IN(@MaxTargetIDs) GROUP BY TargetID
)
";
            }

            query.CreateInParameter<int>("@MinTargetIDs", minIds);
            query.CreateInParameter<int>("@MaxTargetIDs", maxIds);

            Share share = null;

            using (XSqlDataReader reader = query.ExecuteReader())
            {
                while (reader.Read())
                {
                    Comment comment = new Comment(reader);

                    if (share == null || share.UserShareID!=comment.TargetID)
                    {
                        foreach (Share tempShare in shares)
                        {
                            if (tempShare.UserShareID == comment.TargetID)
                            {
                                share = tempShare;
                                break;
                            }
                        }
                    }
                    if (share != null)
                    {
                        if (share.CommentList.ContainsKey(comment.CommentID) == false)
                            share.CommentList.Add(comment);
                    }
                }
            }
        }

    }
}