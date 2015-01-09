//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Settings;


namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class FeedDao : DataAccess.FeedDao
    {
        #region 存储过程 bx_GetFeedTemplates
        [StoredProcedure(Name = "bx_GetFeedTemplates", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_FeedTemplates] ORDER BY [AppID],[ActionType];
END
"
            )]
        #endregion
        public override FeedTemplateCollection GetFeedTemplates()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetFeedTemplates";
                query.CommandType = CommandType.StoredProcedure;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new FeedTemplateCollection(reader);
                }
            }
        }

        public override void SetTemplates(FeedTemplateCollection feedTemplates)
        {
            using (SqlQuery query = new SqlQuery(QueryMode.Prepare))
            {
                query.CommandText = @"
IF EXISTS(SELECT * FROM [bx_FeedTemplates] WHERE [AppID]=@AppID AND [ActionType]=@ActionType)
	UPDATE [bx_FeedTemplates] SET
                 [Title]=@Title 
                ,[IconUrl]=@IconUrl
                ,[Description]=@Description 
                WHERE [AppID]=@AppID AND [ActionType]=@ActionType;
ELSE
	INSERT [bx_FeedTemplates] (
                 [AppID]

                ,[ActionType]

                ,[Title]
                ,[IconUrl]
                ,[Description]
                ) VALUES(
                  @AppID

                , @ActionType

                , @Title
                , @IconUrl
                , @Description
                );
";

                foreach (FeedTemplate feedTemplate in feedTemplates)
                {
                    query.CreateParameter<Guid>("@AppID", feedTemplate.AppID, SqlDbType.UniqueIdentifier);

                    query.CreateParameter<int>("@ActionType", feedTemplate.ActionType, SqlDbType.Int);

                    query.CreateParameter<string>("@Title", feedTemplate.Title, SqlDbType.NVarChar, 900);
                    query.CreateParameter<string>("@IconUrl", feedTemplate.IconSrc, SqlDbType.NVarChar, 200);
                    query.CreateParameter<string>("@Description", feedTemplate.Description, SqlDbType.NVarChar, 2900);

                    query.ExecuteNonQuery();
                }

                //query.Submit();
            }
        }

        public override FeedCollection GetAllUserFeeds(int feedID, int getCount, Guid appID, int? actionType)
        {
            using (SqlQuery query = new SqlQuery())
            {

                //--INSERT INTO @FeedIDs SELECT TOP (@TopCount) [ID] FROM [bx_Feeds] WITH(NOLOCK) WHERE [ID]<@FeedID 
                //--    AND (@ActionType IS NULL OR ([AppID] = @AppID AND [ActionType] = @ActionType))
                //--    OR ([AppID] = @BasicAppID AND [ActionType] = @SiteFeedActionID AND [CreateDate] < @MaxDateTime)
                //--ORDER BY [CreateDate] DESC;

                query.CommandText = @"
DECLARE @MaxDateTime datetime;
SELECT @MaxDateTime = ISNULL([CreateDate],'9999-12-31') FROM [bx_Feeds] WITH(NOLOCK) WHERE [ID] = @FeedID;
            
DECLARE @FeedIDs table(FeedID int);

IF @ActionType IS NULL
    INSERT INTO @FeedIDs SELECT TOP (@TopCount) [ID] FROM [bx_Feeds] WITH(NOLOCK) WHERE [ID]<@FeedID 
        OR ([AppID] = @BasicAppID AND [ActionType] = @SiteFeedActionID AND [CreateDate] < @MaxDateTime)
    ORDER BY [CreateDate] DESC;
ELSE
    INSERT INTO @FeedIDs SELECT TOP (@TopCount) [ID] FROM [bx_Feeds] WITH(NOLOCK) WHERE [ID]<@FeedID 
        AND ([AppID] = @AppID AND [ActionType] = @ActionType)
        OR ([AppID] = @BasicAppID AND [ActionType] = @SiteFeedActionID AND [CreateDate] < @MaxDateTime)
    ORDER BY [CreateDate] DESC;
    


SELECT * FROM [bx_Feeds] WITH(NOLOCK) WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [CreateDate] DESC;
SELECT * FROM [bx_UserFeeds] WITH(NOLOCK) WHERE [FeedID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
";
                query.CreateTopParameter("@TopCount", getCount);

                query.CreateParameter<int>("@FeedID", feedID, SqlDbType.Int);
                query.CreateParameter<Guid>("@AppID", appID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<Guid>("@BasicAppID", Consts.App_BasicAppID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<int>("@SiteFeedActionID", (int)AppActionType.SiteFeed, SqlDbType.Int);
                query.CreateParameter<int?>("@ActionType", actionType, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    FeedCollection feeds = new FeedCollection(reader);
                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        while (reader.Read())
                        {
                            UserFeed userFeed = new UserFeed(reader);

                            currentFeed = ProcessFeed(feeds, currentFeed, userFeed, false);
                        }
                    }
                    return feeds;
                }
            }
            #region
            /*
            string condition = string.Empty;
            if (appID != Guid.Empty && actionType != null)
                condition = "AND [AppID]=@AppID AND [ActionType]=@ActionType ";
            using (DbSession db = new DbSession())
            {
                using (IDataReader reader = db.ExecuteReader(@"
DECLARE @FeedIDs table(FeedID int);
INSERT INTO @FeedIDs SELECT TOP " + getCount + @" [ID] FROM [bx_Feeds]  WHERE [ID]<@FeedID 
"+ condition +@" ORDER BY [ID] DESC;
SELECT * FROM [bx_Feeds] WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [ID] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
"
                    , db.Param<int>("@FeedID",feedID)
                    , db.Param<Guid>("@AppID",appID)
                    , db.Param<int?>("@ActionType",actionType)
                    ))
                {
                    FeedCollection feeds = new FeedCollection(reader);
                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        DataReaderWrap readerWrap = new DataReaderWrap(reader);
                        while (readerWrap.Next)
                        {
                            UserFeed userFeed = new UserFeed(readerWrap);

                            if (currentFeed == null || userFeed.FeedID != currentFeed.ID)
                            {
                                currentFeed = feeds.GetValue(userFeed.FeedID);
                                if(currentFeed!=null)
                                    currentFeed.Users = new UserFeedCollection();
                            }

                            if (currentFeed != null)
                            {
                                if (userFeed.UserID != currentFeed.TargetUserID)
                                    currentFeed.Users.Add(userFeed);
                                else
                                    currentFeed.IsSpecial = true;
                            }
                        }
                    }
                    return feeds;
                }
            }
            */
            #endregion
        }

        public override FeedCollection GetFriendFeeds(int userID, IEnumerable<int> friendUserIDs, int feedID, int getCount, Guid appID, int? actionType)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"

DECLARE @MaxDateTime datetime;
SELECT @MaxDateTime = [CreateDate] FROM [bx_Feeds] WHERE [ID] = @FeedID;
SELECT @MaxDateTime = ISNULL(@MaxDateTime,'9999-12-31')

IF '@FriendUserIDs' = N'' BEGIN
    SELECT * FROM [bx_Feeds] WHERE [AppID] = @BasicAppID AND [ActionType] = @SiteFeedActionID AND [CreateDate] < @MaxDateTime ORDER BY [CreateDate] DESC;
    RETURN;
END";
                if (ValidateUtil.HasItems<int>(friendUserIDs))
                {
                    query.CommandText = query.CommandText + @"
DECLARE @TempFeedIDs table(FeedID int,CDate DateTime);
INSERT INTO @TempFeedIDs(FeedID) SELECT TOP (@TopCount) [FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID] = F.[ID] WHERE 
    [UserID] IN(@FriendUserIDs) AND F.[CreateDate] < @MaxDateTime --[FeedID]<@FeedID
" + (actionType == null ? "" : (" AND [AppID] = @AppID AND [ActionType] = @ActionType")) + @"
    AND F.[TargetUserID] <> @UserID 
    --OR ([AppID] = @BasicAppID AND [ActionType] = @SiteFeedActionID AND F.[CreateDate] < @MaxDateTime)
    GROUP BY [FeedID] ORDER BY MAX(F.[CreateDate]) DESC;

UPDATE @TempFeedIDs SET CDate=[CreateDate] FROM [bx_Feeds] WHERE FeedID=[ID];
INSERT INTO @TempFeedIDs
SELECT TOP (@TopCount) [ID],[CreateDate] FROM [bx_Feeds] WHERE 
 ([AppID] = @BasicAppID AND [ActionType] = @SiteFeedActionID AND [CreateDate] < @MaxDateTime) ORDER BY [CreateDate] DESC;

DECLARE @FeedIDs table(FeedID int);
INSERT INTO @FeedIDs
SELECT TOP (@TopCount) [FeedID] FROM @TempFeedIDs ORDER BY [CDate];


SELECT * FROM [bx_Feeds] WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [ID] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM @FeedIDs) AND [UserID] IN(@FriendUserIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID]=F.[ID] WHERE F.[ID] IN(SELECT [FeedID] FROM @FeedIDs) AND UF.[UserID]=F.[TargetUserID] AND F.[TargetUserID] IN(@FriendUserIDs)) ORDER BY [CreateDate] DESC;
";
                    query.CreateTopParameter("@TopCount", getCount);

                }
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@FeedID", feedID, SqlDbType.Int);
                query.CreateParameter<Guid>("@AppID", appID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<Guid>("@BasicAppID", Consts.App_BasicAppID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<int>("@SiteFeedActionID", (int)AppActionType.SiteFeed, SqlDbType.Int);
                query.CreateParameter<int?>("@ActionType", actionType, SqlDbType.Int);
                //, query.CreateParameter<string>("@FriendUserIDs", StringUtil.JoinForSql(friendUserIDs, false), SqlDbType.VarChar, 8000)


                query.CreateInParameter("@FriendUserIDs", friendUserIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    FeedCollection feeds = new FeedCollection(reader);
                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        while (reader.Read())
                        {
                            UserFeed userFeed = new UserFeed(reader);

                            currentFeed = ProcessFeed(feeds, currentFeed, userFeed, false);
                        }
                    }
                    if (reader.NextResult())// 这一组是特殊的 处理类似加好友的 如果TargetUserID 是好友 需要把UserFeeds 表里与该动态有关的用户都取出来
                    {
                        Feed currentFeed = null;
                        while (reader.Read())
                        {
                            UserFeed userFeed = new UserFeed(reader);

                            currentFeed = ProcessFeed(feeds, currentFeed, userFeed, true);
                        }
                    }
                    return feeds;
                }
            }

            #region
            /*
            string condition = string.Empty;
            if (appID != Guid.Empty && actionType != null)
                condition = " AND [AppID]=@AppID AND [ActionType]=@ActionType ";
            using (DbSession db = new DbSession())
            {
                using (IDataReader reader = db.ExecuteReader(@"
DECLARE @FeedIDs table(FeedID int);
INSERT INTO @FeedIDs SELECT TOP " + getCount + @" UF.[FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID]=F.[ID]  WHERE UF.[UserID] IN(@FriendUserIDs) AND UF.[FeedID]<@FeedID 
    "+condition+@"
    AND F.[TargetUserID]<>@UserID GROUP BY UF.[FeedID] ORDER BY UF.[FeedID] DESC;
SELECT * FROM [bx_Feeds] WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [ID] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM @FeedIDs) AND [UserID] IN(@FriendUserIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID]=F.[ID] WHERE F.[ID] IN(SELECT [FeedID] FROM @FeedIDs) AND UF.[UserID]=F.[TargetUserID] AND F.[TargetUserID] IN(@FriendUserIDs)) ORDER BY [CreateDate] DESC;
"
                    , db.ParamReplace("@FriendUserIDs", StringUtil.Join(friendUserIDs))
                    , db.Param<int>("@UserID", userID)
                    , db.Param<int>("@FeedID", feedID)
                    , db.Param<Guid>("@AppID", appID)
                    , db.Param<int?>("@ActionType", actionType)
                    )
                   )
                {

                    FeedCollection feeds = new FeedCollection(reader);
                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        DataReaderWrap readerWrap = new DataReaderWrap(reader);
                        while (readerWrap.Next)
                        {
                            UserFeed userFeed = new UserFeed(readerWrap);

                            if (currentFeed == null || userFeed.FeedID != currentFeed.ID)
                            {
                                currentFeed = feeds.GetValue(userFeed.FeedID);
                                if(currentFeed!=null)
                                    currentFeed.Users = new UserFeedCollection();
                            }


                            if (currentFeed != null)
                            {
                                if (userFeed.UserID != currentFeed.TargetUserID)
                                    currentFeed.Users.Add(userFeed);
                                else
                                    currentFeed.IsSpecial = true;
                            }
                        }
                    }
                    if (reader.NextResult())// 这一组是特殊的 处理类似加好友的 如果TargetUserID 是好友 需要把UserFeeds 表里与该动态有关的用户都取出来
                    {
                        Feed currentFeed = null;

                        DataReaderWrap readerWrap = new DataReaderWrap(reader);
                        while (readerWrap.Next)
                        {
                            UserFeed userFeed = new UserFeed(readerWrap);

                            if (currentFeed == null || userFeed.FeedID != currentFeed.ID)
                            {
                                currentFeed = feeds.GetValue(userFeed.FeedID);
                                if (currentFeed != null)
                                    currentFeed.Users = new UserFeedCollection();
                            }


                            if (currentFeed != null)
                            {
                                if (userFeed.UserID!=userID && userFeed.UserID != currentFeed.TargetUserID)
                                    currentFeed.Users.Add(userFeed);
                            }
                        }
                    }
                    return feeds;
                }
            }
             */
            #endregion
        }

        public override FeedCollection GetUserFeeds(int userID, int feedID, int getCount, Guid appID, int? actionType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DECLARE @FeedIDs table(FeedID int);
INSERT INTO @FeedIDs SELECT DISTINCT TOP (@TopCount) [FeedID] FROM [bx_UserFeeds] WHERE [UserID]=@UserID AND [FeedID]<@FeedID
" + (actionType == null ? "" : (" AND [FeedID] IN(SELECT [ID] FROM [bx_Feeds] WHERE [AppID] = @AppID AND [ActionType] = @ActionType)")) + @"
 ORDER BY [FeedID] DESC;
SELECT * FROM [bx_Feeds] WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [ID] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [UserID] = @UserID AND [FeedID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID]=F.[ID] WHERE F.[ID] IN(SELECT [FeedID] FROM @FeedIDs) AND UF.[UserID]=F.[TargetUserID] AND F.[TargetUserID]=@UserID) ORDER BY [CreateDate] DESC;
";
                query.CreateTopParameter("@TopCount", getCount);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@FeedID", feedID, SqlDbType.Int);
                query.CreateParameter<Guid>("@AppID", appID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<int?>("@ActionType", actionType, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    FeedCollection feeds = new FeedCollection(reader);

                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        while (reader.Read())
                        {
                            UserFeed userFeed = new UserFeed(reader);

                            currentFeed = ProcessFeed(feeds, currentFeed, userFeed, false);
                        }
                    }
                    if (reader.NextResult())// 这一组是特殊的 处理类似加好友的 如果TargetUserID 是好友 需要把UserFeeds 表里与该动态有关的用户都取出来
                    {
                        Feed currentFeed = null;

                        while (reader.Read())
                        {
                            UserFeed userFeed = new UserFeed(reader);

                            currentFeed = ProcessFeed(feeds, currentFeed, userFeed, true);
                        }
                    }
                    return feeds;
                }
            }
            #region
            /*
            string condition = string.Empty;
            if (appID != Guid.Empty && actionType != null)
                condition = " AND [AppID]=@AppID AND [ActionType]=@ActionType ";
            using (DbSession db = new DbSession())
            {
                using (IDataReader reader = db.ExecuteReader(@"
DECLARE @FeedIDs table(FeedID int);
INSERT INTO @FeedIDs SELECT TOP "+ getCount + @" DISTINCT [FeedID] FROM [bx_UserFeeds] WHERE [UserID]=@UserID AND [FeedID]<@FeedID
"+ condition +@"
;
SELECT * FROM [bx_Feeds] WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs) ORDER BY [FeedID] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM @FeedIDs) GROUP BY [FeedID] ORDER BY [FeedID] DESC,[CreateDate] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID]=F.[ID] WHERE F.[ID] IN(SELECT [FeedID] FROM @FeedIDs) AND UF.[UserID]=F.[TargetUserID] AND F.[TargetUserID]=@UserID) ORDER BY [CreateDate] DESC;
"
                    , db.Param<int>("@UserID", userID)
                    , db.Param<int>("@FeedID", feedID)
                    , db.Param<Guid>("@AppID", appID)
                    , db.Param<int?>("@ActionType", actionType)
                    )
                   )
                {
                    FeedCollection feeds = new FeedCollection(reader);
                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        DataReaderWrap readerWrap = new DataReaderWrap(reader);
                        while (readerWrap.Next)
                        {
                            UserFeed userFeed = new UserFeed(readerWrap);

                            if (currentFeed == null || userFeed.FeedID != currentFeed.ID)
                            {
                                currentFeed = feeds.GetValue(userFeed.FeedID);
                                if (currentFeed != null)
                                    currentFeed.Users = new UserFeedCollection();
                            }


                            if (currentFeed != null)
                            {
                                if (userFeed.UserID != currentFeed.TargetUserID)
                                    currentFeed.Users.Add(userFeed);
                                else
                                    currentFeed.IsSpecial = true;
                            }
                        }
                    }
                    if (reader.NextResult())// 这一组是特殊的 处理类似加好友的 如果TargetUserID 是好友 需要把UserFeeds 表里与该动态有关的用户都取出来
                    {
                        Feed currentFeed = null;

                        DataReaderWrap readerWrap = new DataReaderWrap(reader);
                        while (readerWrap.Next)
                        {
                            UserFeed userFeed = new UserFeed(readerWrap);

                            if (currentFeed == null || userFeed.FeedID != currentFeed.ID)
                            {
                                currentFeed = feeds.GetValue(userFeed.FeedID);
                                if(currentFeed!=null)
                                    currentFeed.Users = new UserFeedCollection();
                            }


                            if (currentFeed != null)
                            {
                                if (userFeed.UserID != userID && userFeed.UserID != currentFeed.TargetUserID)
                                    currentFeed.Users.Add(userFeed);
                            }
                        }
                    }
                    return feeds;
                }
            }
             */
            #endregion
        }

        private Feed ProcessFeed(FeedCollection feeds, Feed currentFeed, UserFeed userFeed, bool checkUser)
        {
            if (currentFeed == null || userFeed.FeedID != currentFeed.ID)
            {
                currentFeed = feeds.GetValue(userFeed.FeedID);
                if (currentFeed != null && currentFeed.Users == null)
                    currentFeed.Users = new UserFeedCollection();
            }


            if (currentFeed != null)
            {
                if (userFeed.UserID == currentFeed.TargetUserID)
                    currentFeed.IsSpecial = true;
                if (!checkUser)
                    currentFeed.Users.Add(userFeed);
                else
                {
                    foreach (UserFeed tempUserFeed in currentFeed.Users)
                    {
                        if (tempUserFeed.UserID == userFeed.UserID)
                            return currentFeed;
                    }
                    currentFeed.Users.Add(userFeed);
                }
                //if (userFeed.UserID != userID && userFeed.UserID != currentFeed.TargetUserID)
                //currentFeed.Users.Add(userFeed);
            }
            return currentFeed;
        }


        public override FeedCollection GetFeeds(IEnumerable<int> feedIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT * FROM [bx_Feeds] WHERE [ID] IN(@FeedIDs) ORDER BY [CreateDate] DESC;
SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(@FeedIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
";
                query.CreateInParameter("@FeedIDs", feedIDs);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    FeedCollection feeds = new FeedCollection(reader);

                    if (reader.NextResult())
                    {
                        Feed currentFeed = null;

                        while (reader.Read())
                        {
                            UserFeed userFeed = new UserFeed(reader);

                            currentFeed = ProcessFeed(feeds, currentFeed, userFeed, false);
                        }
                    }
                    return feeds;
                }
            }
        }

        /// <summary>
        /// 如果UserID为空，则完全删除这一组Feed；
        /// 如果UserID不为空，则把用户从这些Feed移除。如果某个Feed只关联了这一个用户，则整条Feed被删除
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="feedIDs"></param>
        public override void DeleteFeeds(int? userID, IEnumerable<int> feedIDs)
        {
            bool emptyFeeds = true;

            foreach (int id in feedIDs)
            {
                emptyFeeds = false;
                break;
            }

            if (emptyFeeds)
                return;

            if (userID == null)
            {
                using (SqlQuery query = new SqlQuery())
                {
                    query.CommandText = @"
DELETE [bx_Feeds] WHERE [ID] IN(@FeedIDs);
";
                    query.CreateInParameter("@FeedIDs", feedIDs);

                    query.ExecuteNonQuery();
                    return;
                }
            }
            using (SqlSession db = new SqlSession())
            {
                db.BeginTransaction();

                using (SqlQuery query = db.CreateQuery())
                {
                    query.CommandText = "DELETE [bx_UserFeeds] WHERE [UserID]=@UserID AND [FeedID] IN(@FeedIDs);";

                    query.CreateParameter<int>("@UserID", userID.Value, SqlDbType.Int);

                    query.CreateInParameter("@FeedIDs", feedIDs);

                    query.ExecuteNonQuery();
                }

                using (SqlQuery query = db.CreateQuery(QueryMode.Prepare))
                {


                    query.CommandText = @"
IF NOT EXISTS(SELECT * FROM [bx_UserFeeds] WHERE [FeedID]=@FeedID)
    DELETE [bx_Feeds] WHERE [ID] = @FeedID;
--以下是删除 类似加好友的动态
DECLARE @FeedCount int;
SELECT @FeedCount = COUNT(*) FROM bx_UserFeeds WHERE [FeedID]=@FeedID
IF @FeedCount = 1 AND EXISTS(SELECT * FROM bx_Feeds F INNER JOIN bx_UserFeeds UF ON F.[ID]=UF.[FeedID] WHERE F.[ID]=@FeedID AND UF.[UserID]=F.[TargetUserID]) BEGIN
    DELETE [bx_Feeds] WHERE [ID] = @FeedID;
END
";

                    foreach (int feedID in feedIDs)
                    {
                        query.CreateParameter<int>("@FeedID", feedID, SqlDbType.Int);
                        query.ExecuteNonQuery();
                    }
                }

                db.CommitTransaction();
            }
            #region
            /*
            using (DbSession db = new DbSession())
            {
                if (userID == null)
                {
                    db.ExecuteNonQuery(@"
DELETE [bx_Feeds] WHERE [ID] IN(@FeedIDs);
"
                    , db.ParamReplace("@FeedIDs", StringUtil.Join(feedIDs))
                    );
                    return;
                }

                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE [bx_UserFeeds] WHERE [UserID]=@UserID AND [FeedID] IN(@FeedIDs);");

                int i = 0;
                foreach (int feedID in feedIDs)
                {
                    string s = db.FormatSql(@"
IF NOT EXISTS(SELECT * FROM [bx_UserFeeds] WHERE [FeedID]=@FeedID)
    DELETE [bx_Feeds] WHERE [ID] = @FeedID;
--以下是删除 类似加好友的动态
DECLARE @Feed" + i +@"Count int;
SELECT @Feed" + i + @"Count = COUNT(*) FROM bx_UserFeeds WHERE [FeedID]=@FeedID
IF @Feed" + i + @"Count = 1 AND EXISTS(SELECT * FROM bx_Feeds F INNER JOIN bx_UserFeeds UF ON F.[ID]=UF.[FeedID] WHERE F.[ID]=@FeedID AND UF.[UserID]=F.[TargetUserID]) BEGIN
    DELETE [bx_Feeds] WHERE [ID] = @FeedID;
END
"
                        , db.Param<int>("@FeedID",feedID)
                        );
                    sql.AppendLine(s);
                    i++;
                }
                db.ExecuteNonQuery(
                      sql.ToString()
                    , db.Param<int>("@UserID",userID.Value)
                    , db.ParamReplace("@FeedIDs", StringUtil.Join(feedIDs))
                    );
            }
            */
            #endregion
        }


        public override void DeleteFeeds(DateTime? dateTime, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (count == 0)
                {
                    query.CommandText = @"
DELETE bx_Feeds WHERE CreateDate<@DateTime;
";
                }
                else
                {
                    query.CommandText = @"
IF @DateTime IS NULL
    DELETE bx_Feeds WHERE [ID]<(SELECT MIN(ID) FROM (SELECT TOP (@TopCount) ID FROM bx_Feeds ORDER BY [ID] DESC) AS T);
ELSE
    DELETE bx_Feeds WHERE [ID]<(SELECT MIN(ID) FROM (SELECT TOP (@TopCount) ID FROM bx_Feeds ORDER BY [ID] DESC) AS T) AND CreateDate<@DateTime;
";
                    query.CreateTopParameter("@TopCount", count);
                }
                query.CreateParameter<DateTime?>("@DateTime", dateTime, SqlDbType.DateTime);
                query.CommandType = CommandType.Text;
                query.CommandTimeout = int.MaxValue;

                query.ExecuteNonQuery();
            }
        }

        [StoredProcedure(Name = "bx_CreateFeed", FileName = "bx_CreateFeed.sql")]
        public override int CreateFeed(Feed feed, UserFeed userFeed, FeedSendItem.SendType sendType, bool canJoin)
        {
            int feedID = 0;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateFeed";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<bool>("@IsSpecial", feed.IsSpecial, SqlDbType.Bit);
                query.CreateParameter<int>("@UserID", userFeed.UserID, SqlDbType.Int);
                query.CreateParameter<int?>("@TargetID", feed.TargetID, SqlDbType.Int);
                query.CreateParameter<int>("@TargetUserID", feed.TargetUserID, SqlDbType.Int);
                query.CreateParameter<int>("@ActionType", feed.ActionType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@PrivacyType", (int)feed.PrivacyType, SqlDbType.TinyInt);
                query.CreateParameter<Guid>("@AppID", feed.AppID, SqlDbType.UniqueIdentifier);

                query.CreateParameter<string>("@Title", feed.Title, SqlDbType.NVarChar, 1000);
                query.CreateParameter<string>("@Realname", userFeed.Realname, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@Description", feed.Description, SqlDbType.NVarChar, 2500);
                query.CreateParameter<string>("@TargetNickname", feed.TargetNickname, SqlDbType.NVarChar, 50);
                query.CreateParameter<string>("@VisibleUserIDs", StringUtil.Join(feed.VisibleUserIDs), SqlDbType.VarChar, 800);

                query.CreateParameter<DateTime>("@CreateDate", feed.CreateDate, SqlDbType.DateTime);
                query.CreateParameter<bool>("@CanJoin", canJoin, SqlDbType.Bit);
                query.CreateParameter<int>("@DefaultSendType", (int)sendType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@CommentTargetID", feed.CommentTargetID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        feedID = reader.Get<int>("FeedID");
                    }
                }
            }

            return feedID;
        }


        public override void SetUserNoAddFeedApps(int userID, UserNoAddFeedAppCollection userNoAddFeedApps)
        {
            using (SqlSession db = new SqlSession())
            {
                db.BeginTransaction();

                //using (SqlQuery query = db.CreateQuery())
                //{
                //    query.CommandText = "DELETE [bx_UserNoAddFeedApps] WHERE [UserID]=@UserID;";

                //    query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                //    query.ExecuteNonQuery();
                //}

                using (SqlQuery query = db.CreateQuery(QueryMode.Prepare))
                {
                    query.CommandText = @"
IF EXISTS(SELECT * FROM [bx_UserNoAddFeedApps] WHERE AppID=@AppID AND UserID=@UserID AND ActionType=@ActionType)
    UPDATE [bx_UserNoAddFeedApps] SET [Send]=@Send WHERE AppID=@AppID AND UserID=@UserID AND ActionType=@ActionType;
ELSE
    INSERT INTO [bx_UserNoAddFeedApps](
                        [AppID]
                       ,[UserID]
                       ,[ActionType]
                       ,[Send]
                       ) VALUES (
                        @AppID
                       ,@UserID
                       ,@ActionType
                       ,@Send
                       );
";
                    foreach (UserNoAddFeedApp userNoAddFeedApp in userNoAddFeedApps)
                    {
                        query.CreateParameter<Guid>("@AppID", userNoAddFeedApp.AppID, SqlDbType.UniqueIdentifier);
                        query.CreateParameter<int>("@ActionType", userNoAddFeedApp.ActionType, SqlDbType.Int);
                        query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                        query.CreateParameter<bool>("@Send", userNoAddFeedApp.Send, SqlDbType.Bit);

                        query.ExecuteNonQuery();
                    }
                }

                db.CommitTransaction();
            }
        }

        #region 存储过程 bx_GetUserNoAddFeedApps
        [StoredProcedure(Name = "bx_GetUserNoAddFeedApps", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_UserNoAddFeedApps] WHERE [UserID]=@UserID;
END
"
            )]
        #endregion
        public override UserNoAddFeedAppCollection GetUserNoAddFeedApps(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserNoAddFeedApps";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    UserNoAddFeedAppCollection userNoAddFeedApps = new UserNoAddFeedAppCollection(reader);
                    return userNoAddFeedApps;
                }
            }
        }

        [StoredProcedure(Name = "bx_CreateFeedFilter", FileName = "bx_CreateFeedFilter.sql")]
        public override void CreateFeedFilter(FeedFilter feedFilter)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateFeedFilter";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<Guid>("@AppID", feedFilter.AppID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<int>("@UserID", feedFilter.UserID, SqlDbType.Int);
                query.CreateParameter<int?>("@FriendUserID", feedFilter.FriendUserID, SqlDbType.Int);
                query.CreateParameter<int?>("@ActionType", feedFilter.ActionType, SqlDbType.Int);


                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 bx_GetFeedFilters
        [StoredProcedure(Name = "bx_GetFeedFilters", Script = @"
CREATE PROCEDURE {name}
    @UserID int
AS
BEGIN
	SET NOCOUNT ON;
    SELECT * FROM [bx_FeedFilters] WHERE [UserID]=@UserID;
END
"
            )]
        #endregion
        public override FeedFilterCollection GetFeedFilters(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetFeedFilters";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new FeedFilterCollection(reader);
                }
            }
        }

        public override void DeleteFeedFilters(int userID, IEnumerable<int> feedFilterIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE [bx_FeedFilters] WHERE [UserID]=@UserID AND [ID] IN(@FeedFilterIDs);";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);


                query.CreateInParameter<int>("@FeedFilterIDs", feedFilterIDs);


                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 bx_UpdateFeedPrivacyType
        [StoredProcedure(Name = "bx_UpdateFeedPrivacyType", Script = @"
CREATE PROCEDURE {name}
     @PrivacyType     tinyint
    ,@ActionType      tinyint
    ,@TargetID        int
    ,@VisibleUserIDs  varchar(800)
    ,@AppID           uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE [bx_Feeds] SET 
        [PrivacyType] = @PrivacyType
      , [VisibleUserIDs] = @VisibleUserIDs 
    WHERE [AppID] = @AppID AND [ActionType] = @ActionType AND [TargetID] = @TargetID;
END
"
            )]
        #endregion
        public override void UpdateFeedPrivacyType(Guid appID, int actionType, int targetID, PrivacyType privacyType, List<int> visibleUserIDs)
        {
            string userIDs = StringUtil.Join(visibleUserIDs);
            //如果超出800 会被数据库截断  所以要处理
            if (userIDs.Length > 800)
            {
                userIDs = userIDs.Substring(0, 800);
                userIDs = userIDs.Substring(0, userIDs.LastIndexOf(','));
            }
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateFeedPrivacyType";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@PrivacyType", (int)privacyType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@ActionType", actionType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@TargetID", targetID, SqlDbType.Int);
                query.CreateParameter<Guid>("@AppID", appID, SqlDbType.UniqueIdentifier);
                query.CreateParameter<string>("@VisibleUserIDs", userIDs, SqlDbType.VarChar, 800);


                query.ExecuteNonQuery();
            }
        }


        public override void DeleteFeeds(Guid appID, int actionType, IEnumerable<int> targetIDs, IEnumerable<List<int>> userIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (userIDs == null)
                {
                    query.CommandText = "DELETE [bx_Feeds] WHERE [AppID] = @AppID AND [ActionType] = @ActionType AND [TargetID] in(@TargetIDs);";
                    query.CommandType = CommandType.Text;
                    query.CreateInParameter<int>("@TargetIDs", targetIDs);
                }
                else
                {
                    StringBuilder sql = new StringBuilder();

                    int i = 0;
                    foreach (int targetID in targetIDs)
                    {
                        sql.AppendFormat(@"
        DECLARE @FeedID int;
        SELECT @FeedID = [ID] FROM [bx_Feeds] WHERE [AppID] = @AppID AND [ActionType] = @ActionType AND [TargetID] = @TargetID_{0};
        DELETE [bx_UserFeeds] WHERE [UserID] in (@UserIDs_{0}) AND FeedID = @FeedID;
        IF NOT EXISTS(SELECT * FROM [bx_UserFeeds] WHERE FeedID = @FeedID)
            DELETE [bx_Feeds] WHERE [ID] = @FeedID;
"
                            , i);

                        List<int> uids = new List<int>();
                        int j = 0;
                        foreach (List<int> tempUserIDs in userIDs)
                        {
                            if (j == i)
                            {
                                uids = tempUserIDs;
                                break;
                            }
                            j++;
                        }



                        query.CreateParameter<int>("@TargetID_" + i, targetID, SqlDbType.Int);
                        query.CreateInParameter<int>("@UserIDs_" + i, uids);

                        i++;
                    }

                    query.CommandText = sql.ToString();
                }

                query.CreateParameter<int>("@ActionType", actionType, SqlDbType.TinyInt);
                query.CreateParameter<Guid>("@AppID", appID, SqlDbType.UniqueIdentifier);
                query.ExecuteNonQuery();
            }
        }


        public override void DeleteSearchFeeds(FeedSearchFilter filter, int deleteTopCount, out int deletedCount)
        {
            string order;

            if (filter.Order == FeedSearchFilter.OrderBy.ID)
            {
                if (filter.IsDesc)
                    order = " ORDER BY [ID] DESC ";
                else
                    order = " ORDER BY [ID] ASC ";
            }
            else
            {
                if (filter.IsDesc)
                    order = " ORDER BY [CreateDate] DESC ";
                else
                    order = " ORDER BY [CreateDate] ASC ";
            }

            if (filter.UserID != null)
            {
                string condition = @" UserID = @UserID "
                            + (filter.AppID == null ? "" : (@" AND [FeedID] IN(SELECT [ID] FROM [bx_Feeds] WHERE [AppID]=@AppID "
                            + (filter.AppActionType == null ? "" : (" AND [ActionType] = @ActionType)"))))
                            + (filter.BeginDate == null ? "" : (" AND [CreateDate] > @BeginDate"))
                            + (filter.EndDate == null ? "" : (" AND [CreateDate] < @EndDate"));




                using (SqlQuery query = new SqlQuery())
                {
                    query.CommandText = @"
DELETE [bx_UserFeeds] WHERE [ID] IN (SELECT TOP (@TopCount) [ID] FROM [bx_UserFeeds] WHERE " + condition + order + @" );
SELECT @@ROWCOUNT;
DELETE [bx_Feeds] WHERE [ID] NOT IN(SELECT DISTINCT [FeedID] FROM [bx_UserFeeds]);
--以下是删除 类似加好友的动态 
DECLARE @T TABLE(UserCount int,FeedID int);
DECLARE @FeedIDs TABLE(FeedID int);
INSERT INTO @T SELECT COUNT(*),[FeedID] FROM bx_UserFeeds GROUP BY [FeedID];
INSERT INTO @FeedIDs SELECT [FeedID] FROM bx_UserFeeds UF INNER JOIN bx_Feeds F ON UF.[FeedID]=F.[ID] WHERE F.[ID] IN(SELECT [FeedID] FROM @T WHERE [UserCount]=1) AND UF.[UserID]=F.[TargetUserID];
DELETE [bx_Feeds] WHERE [ID] IN(SELECT [FeedID] FROM @FeedIDs);
";
                    query.CreateTopParameter("@TopCount", deleteTopCount);
                    query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
                    query.CreateParameter<Guid?>("@AppID", filter.AppID, SqlDbType.UniqueIdentifier);
                    query.CreateParameter<int?>("@ActionType", filter.AppActionType, SqlDbType.Int);
                    query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
                    query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);


                    deletedCount = 0;
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            deletedCount = reader.Get<int>(0);
                        }
                    }
                }
            }
            else
            {
                string condition = @" 1 = 1 "
                            + (filter.AppID == null ? "" : (@" AND [AppID]=@AppID ")
                            + (filter.AppActionType == null ? "" : (" AND [ActionType] = @ActionType")))
                            + (filter.BeginDate == null ? "" : (" AND [CreateDate] > @BeginDate"))
                            + (filter.EndDate == null ? "" : (" AND [CreateDate] < @EndDate"));
                using (SqlQuery query = new SqlQuery())
                {
                    query.CommandText = "DELETE [bx_Feeds] WHERE [ID] IN (SELECT TOP (@TopCount) [ID] FROM [bx_Feeds] WHERE " + condition + order + ")";

                    query.CreateTopParameter("@TopCount", deleteTopCount);
                    query.CreateParameter<Guid?>("@AppID", filter.AppID, SqlDbType.UniqueIdentifier);
                    query.CreateParameter<int?>("@ActionType", filter.AppActionType, SqlDbType.Int);
                    query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
                    query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);

                    deletedCount = 0;
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            deletedCount = reader.Get<int>(0);
                        }
                    }
                }

            }
        }
        public override FeedCollection SearchFeeds(int pageNumber, FeedSearchFilter filter, ref int totalCount)
        {

            using (SqlSession db = new SqlSession())
            {
                List<int> feedIDs = new List<int>();
                using (SqlQuery query = db.CreateQuery())
                {
                    query.Pager.IsDesc = filter.IsDesc;
                    if (filter.UserID != null)
                    {
                        query.Pager.ResultFields = "[FeedID]";
                        if (filter.Order == FeedSearchFilter.OrderBy.ID)
                            query.Pager.SortField = "[ID]";
                        else
                        {
                            query.Pager.SortField = "[CreateDate]";
                            query.Pager.PrimaryKey = "[ID]";
                        }
                        query.Pager.TableName = "[bx_UserFeeds]";
                        query.Pager.Condition = @" UserID = @UserID "
                            + (filter.AppID == null ? "" : (@" AND [FeedID] IN(SELECT [ID] FROM [bx_Feeds] WHERE [AppID]=@AppID "
                            + (filter.AppActionType == null ? "" : (" AND [ActionType] = @ActionType)"))))
                            + (filter.BeginDate == null ? "" : (" AND [CreateDate] > @BeginDate"))
                            + (filter.EndDate == null ? "" : (" AND [CreateDate] < @EndDate"));
                    }
                    else
                    {
                        query.Pager.ResultFields = "[ID]";
                        if (filter.Order == FeedSearchFilter.OrderBy.ID)
                            query.Pager.SortField = "[ID]";
                        else
                        {
                            query.Pager.SortField = "[CreateDate]";
                            query.Pager.PrimaryKey = "[ID]";
                        }

                        query.Pager.TableName = "[bx_Feeds]";
                        query.Pager.Condition = @" 1 = 1 "
                            + (filter.AppID == null ? "" : (@" AND [AppID]=@AppID ")
                            + (filter.AppActionType == null ? "" : (" AND [ActionType] = @ActionType")))
                            + (filter.BeginDate == null ? "" : (" AND [CreateDate] > @BeginDate"))
                            + (filter.EndDate == null ? "" : (" AND [CreateDate] < @EndDate"));
                    }
                    query.Pager.PageNumber = pageNumber;
                    query.Pager.PageSize = filter.PageSize;

                    query.Pager.TotalRecords = totalCount;
                    query.Pager.SelectCount = true;

                    query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
                    query.CreateParameter<Guid?>("@AppID", filter.AppID, SqlDbType.UniqueIdentifier);
                    query.CreateParameter<int?>("@ActionType", filter.AppActionType, SqlDbType.Int);
                    query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
                    query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);


                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            feedIDs.Add(reader.GetInt32(0));
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                totalCount = reader.GetInt32(0);
                            }
                        }
                    }
                }

                if (feedIDs.Count == 0)
                    return new FeedCollection();


                string sql;
                string sortField = (filter.Order == FeedSearchFilter.OrderBy.ID ? "[ID]" : "[CreateDate]");
                string order = (filter.IsDesc ? " DESC " : " ASC ");
                if (filter.UserID == null)
                {
                    sql = @"
                    SELECT * FROM [bx_Feeds] WHERE [ID] IN(@FeedIDs) ORDER BY " + sortField + order + @";
                    SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(@FeedIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
";
                }
                else
                {
                    sql = @"
                    SELECT * FROM [bx_Feeds] WHERE [ID] IN(@FeedIDs) ORDER BY " + sortField + order + @";
                    SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(@FeedIDs) ORDER BY [FeedID] DESC,[CreateDate] DESC;
                    SELECT * FROM [bx_UserFeeds] WHERE [FeedID] IN(SELECT [FeedID] FROM [bx_UserFeeds] UF INNER JOIN [bx_Feeds] F ON UF.[FeedID]=F.[ID] WHERE F.[ID] IN(@FeedIDs) AND UF.[UserID]=F.[TargetUserID] AND F.[TargetUserID]=@UserID) ORDER BY [CreateDate] DESC;
";
                }


                using (SqlQuery query = db.CreateQuery())
                {
                    query.CommandText = sql;

                    query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);

                    query.CreateInParameter<int>("@FeedIDs", feedIDs);
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        FeedCollection feeds = new FeedCollection(reader);

                        if (reader.NextResult())
                        {
                            Feed currentFeed = null;

                            while (reader.Read())
                            {
                                UserFeed userFeed = new UserFeed(reader);

                                currentFeed = ProcessFeed(feeds, currentFeed, userFeed, false);
                            }
                        }

                        if (reader.NextResult())// 这一组是特殊的 处理类似加好友的 如果TargetUserID 是好友 需要把UserFeeds 表里与该动态有关的用户都取出来
                        {
                            Feed currentFeed = null;

                            while (reader.Read())
                            {
                                UserFeed userFeed = new UserFeed(reader);

                                currentFeed = ProcessFeed(feeds, currentFeed, userFeed, true);
                            }
                        }
                        return feeds;
                    }
                }

            }
        }
    }
}