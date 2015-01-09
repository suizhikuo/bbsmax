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

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class ForumDaoV5 : DataAccess.ForumDaoV5
    {

        #region 存储过程 bx_GetAllForums
        [StoredProcedure(Name = "bx_GetAllForums", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_Forums WITH (NOLOCK) WHERE [ClubID]=0 ORDER BY [ParentID],[SortOrder] ASC;
END
"
            )]
        #endregion
        public override ForumCollection GetAllForums()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllForums";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ForumCollection(reader);
                }
            }
        }

        #region 存储过程 bx_v5_CreateForum
        [StoredProcedure(Name = "bx_v5_CreateForum", Script = @"
CREATE PROCEDURE {name}
	@ParentID int,
	@ForumType tinyint,
	@CodeName nvarchar(128),
	@ForumName nvarchar(1024),
	@Description ntext,
	@Readme ntext,
	@LogoUrl nvarchar(256),
	@ThemeID nvarchar(64),
	@Password nvarchar(64),
	@ExtendedAttributes [ntext],
	@ForumID int output,
	@ThreadCatalogStatus tinyint,
	@ColumnSpan tinyint,
	@SortOrder int
AS
	SET NOCOUNT ON 
	
	IF(EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE CodeName=@CodeName))
	begin
	set @ForumID = 0
	RETURN (13)	
	end
	
	IF ((@ParentID=0) OR (EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@ParentID)) )
	BEGIN
	
	DECLARE @Condition varchar(50)
	SET @Condition='ParentID='+str(@ParentID)
	--EXECUTE bbsMax_Common_GetSortOrder 'bx_Forums',@Condition,@SortOrder output
	
	INSERT INTO [bx_Forums] (
	[ParentID],
	[ForumType],
	[ThreadCatalogStatus],
	[CodeName],
	[ForumName],
	[Description],
	[Readme],
	[LogoSrc],
	[ThemeID],
	[Password],
	[SortOrder],
	[ExtendedAttributes],
	[ColumnSpan]
) VALUES (
	@ParentID,
	@ForumType,
	@ThreadCatalogStatus,
	@CodeName,
	@ForumName,
	@Description,
	@Readme,
	@LogoUrl,
	@ThemeID,
	@Password,
	@SortOrder,
	@ExtendedAttributes,
	@ColumnSpan
)
		set @ForumID = @@IDENTITY;
		RETURN (0)
	END
	
	ELSE
	begin
	set @ForumID = 0
		RETURN (-1)
		end
"
            )]
        #endregion
        public override int CreateForum(string codeName, string forumName, int parentID, ForumType forumType, string password, string logoUrl
            , string themeID, string readme, string description, ThreadCatalogStatus threadCatalogStaus, int columnSpan, int sortOrder
            , ForumExtendedAttribute forumExtendedDatas, out int forumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_CreateForum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@CodeName", codeName, SqlDbType.NVarChar, 128);
                query.CreateParameter<string>("@ForumName", forumName, SqlDbType.NVarChar, 1024);
                query.CreateParameter<int>("@ParentID", parentID, SqlDbType.Int);
                query.CreateParameter<int>("@ForumType", (int)forumType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 64);
                query.CreateParameter<string>("@LogoUrl", logoUrl, SqlDbType.NVarChar, 256);
                query.CreateParameter<string>("@ThemeID", themeID, SqlDbType.NVarChar, 64);
                query.CreateParameter<string>("@Readme", readme, SqlDbType.NText);
                query.CreateParameter<string>("@Description", description, SqlDbType.NText);
                query.CreateParameter<int>("@ThreadCatalogStatus", (int)threadCatalogStaus, SqlDbType.TinyInt);
                query.CreateParameter<int>("@ColumnSpan", columnSpan, SqlDbType.TinyInt);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);
                query.CreateParameter<string>("@ExtendedAttributes", forumExtendedDatas.ToString(), SqlDbType.NText);

                query.CreateParameter<int>("@ForumID", SqlDbType.Int, ParameterDirection.Output);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                forumID = (int)query.Parameters["@ForumID"].Value;
                return (int)query.Parameters["@ErrorCode"].Value;
            }
        }

        #region 存储过程 bx_v5_UpdateForum
        [StoredProcedure(Name = "bx_v5_UpdateForum", Script = @"
CREATE PROCEDURE {name}
	@ForumID int,
	--@ParentID int,
	@ForumType tinyint,
	@CodeName nvarchar(128),
	@ForumName nvarchar(1024),
	@Description ntext,
	@Readme ntext,
	@LogoUrl nvarchar(256),
	@ThemeID nvarchar(64),
	@Password nvarchar(64),
	@ExtendedAttributes [ntext],
	@ColumnSpan tinyint,
	@SortOrder int
AS
	SET NOCOUNT ON 
	IF(EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE CodeName=@CodeName AND ForumID <> @ForumID))
	RETURN (13)	

	--IF ((@ParentID=0) OR (EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@ParentID)) )
	--BEGIN
	--IF(@ParentID=@ForumID)
	--RETURN (14)
	
	UPDATE [bx_Forums] SET
	--[ParentID] = @ParentID,
	[ForumType] = @ForumType,
	[CodeName] = @CodeName,
	[ForumName] = @ForumName,
	[Description] = @Description,
	[Readme] = @Readme,
	[LogoSrc] = @LogoUrl,
	[ThemeID] = @ThemeID,
	[Password] = @Password,
	[ExtendedAttributes] = @ExtendedAttributes,
	[ColumnSpan] = @ColumnSpan,
	[SortOrder] = @SortOrder
WHERE
	[ForumID] = @ForumID
	
	RETURN (0)
"
            )]
        #endregion
        public override int UpdateForum(int forumID, string codeName, string forumName, ForumType forumType, string password, string logoUrl
            , string readme, string description, string themeID, int columnSpan, int sortOrder, ForumExtendedAttribute forumExtendedDatas)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_UpdateForum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@CodeName", codeName, SqlDbType.NVarChar, 128);
                query.CreateParameter<string>("@ForumName", forumName, SqlDbType.NVarChar, 1024);
                query.CreateParameter<int>("@ForumType", (int)forumType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Password", password, SqlDbType.NVarChar, 64);
                query.CreateParameter<string>("@LogoUrl", logoUrl, SqlDbType.NVarChar, 256);
                query.CreateParameter<string>("@ThemeID", themeID, SqlDbType.NVarChar, 64);
                query.CreateParameter<string>("@Readme", readme, SqlDbType.NText);
                query.CreateParameter<string>("@Description", description, SqlDbType.NText);
                query.CreateParameter<int>("@ColumnSpan", columnSpan, SqlDbType.TinyInt);
                query.CreateParameter<int>("@SortOrder", sortOrder, SqlDbType.Int);
                query.CreateParameter<string>("@ExtendedAttributes", forumExtendedDatas.ToString(), SqlDbType.NText);

                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();
                return (int)query.Parameters["@ErrorCode"].Value;
            }
        }

        #region 存储过程 bx_v5_DeleteForum
        [StoredProcedure(Name = "bx_v5_DeleteForum", Script = @"
CREATE PROCEDURE {name}
	@ForumID int
AS
	SET NOCOUNT ON
	
	BEGIN TRANSACTION
	DECLARE @SortOrder int,@ParentID int,@ErrorNum int
	SET @ErrorNum=0
	
	DELETE [bx_Threads] WHERE ForumID=@ForumID 
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
		
	--TODO DELETE [bx_Threads] WHERE ForumID=@ForumID  删除屏蔽用户
	--IF @@error<>0
	--	SET @ErrorNum=@ErrorNum+1
	
	SELECT @ParentID=ParentID FROM [bx_Forums] WITH(NOLOCK) WHERE ForumID=@ForumID 
	SELECT @SortOrder=MAX(SortOrder)+1 FROM [bx_Forums] WITH(NOLOCK) WHERE ParentID=@ParentID  
	IF @SortOrder IS NULL
			SET @SortOrder=0
			
	UPDATE [bx_Forums] SET ParentID=@ParentID,SortOrder=SortOrder+@SortOrder WHERE ParentID=@ForumID
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1

    DELETE [bx_StickThreads] WHERE ForumID=@ForumID;
    IF @@error<>0
		SET @ErrorNum=@ErrorNum+1 	

	DELETE [bx_Forums] WHERE ForumID=@ForumID;
	IF @@error<>0
		SET @ErrorNum=@ErrorNum+1
		
	IF @ErrorNum=0
		BEGIN
			COMMIT TRANSACTION
			--EXECUTE bx_UpdateForumData @ParentID
			RETURN (0)
		END
		ELSE
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
"
            )]
        #endregion
        public override bool DeleteForum(int forumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_DeleteForum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();
                return (int)query.Parameters["@ErrorCode"].Value == 0;
            }
        }

        #region 存储过程 bx_UpdateForumReadme
        [StoredProcedure(Name = "bx_UpdateForumReadme", Script = @"
CREATE PROCEDURE {name}
    @ForumID int,
    @Readme  ntext
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_Forums SET Readme=@Readme WHERE ForumID=@ForumID;
END
"
            )]
        #endregion
        public override bool UpdateForumReadme(int forumID, string readme)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateForumReadme";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@Readme", readme, SqlDbType.NText);

                query.ExecuteNonQuery();

                return true;
            }
        }

        #region 存储过程 bx_UpdateForumData
        [StoredProcedure(Name = "bx_UpdateForumData", Script = @"
CREATE PROCEDURE {name}
	@ForumID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TotalThreads int;
	DECLARE @TotalPosts int,@LastThreadID INT--@TodayThreads int,@TodayPosts int;

    -- Insert statements for procedure here
	SELECT @TotalThreads = COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < 4;
	SELECT @TotalPosts = COUNT(*) FROM [bx_Posts] WITH (NOLOCK)  WHERE ForumID = @ForumID AND [SortOrder] < 4000000000000000;
	--SELECT @TodayThreads=COUNT(*) FROM [bx_Threads] WITH(NOLOCK) WHERE [ForumID] = @ForumID AND ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums WITH(NOLOCK) WHERE ForumID=@ForumID)
	--SELECT @TodayPosts=COUNT(*) FROM [bx_Posts] P WITH (NOLOCK) WHERE P.ForumID = @ForumID AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums WITH(NOLOCK) WHERE ForumID=@ForumID)
	SELECT @LastThreadID=ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) WHERE T1.ForumID=@ForumID AND T1.SortOrder < 4000000000000000)
	
	UPDATE [bx_Forums] SET [TotalThreads] = @TotalThreads, [TotalPosts] = @TotalPosts, [LastThreadID]=ISNULL(@LastThreadID,0) WHERE [ForumID] = @ForumID;
	
	SELECT * FROM bx_Forums WITH (NOLOCK) WHERE ForumID = @ForumID;
END
"
            )]
        #endregion
        public override Forum UpdateForumData(int forumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateForumData";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = int.MaxValue;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                        return new Forum(reader);
                }

                return null;
            }
        }

        public override void UpdateFormsLastThreadID(IEnumerable<int> forumIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();
                foreach (int forumID in forumIDs)
                {
                    sql.AppendFormat(@"UPDATE [bx_Forums] SET [LastThreadID]=(
SELECT ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) WHERE T1.ForumID=@ForumID_{0} AND T1.SortOrder < 4000000000000000)
);", forumID);
                    query.CreateParameter<int>(string.Format("@ForumID_{0}", forumID), forumID, SqlDbType.Int);
                }

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;
                query.CommandTimeout = int.MaxValue;

                query.ExecuteNonQuery();
            }
        }

        #region 存储过程 bx_ResetTodayPosts
        [StoredProcedure(Name = "bx_ResetTodayPosts", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
    
    DECLARE @Today datetime;
    SELECT @Today = CONVERT(DATETIME, CONVERT(varchar(100), GETDATE(), 23));

    UPDATE bx_Forums SET  
         TodayPosts = (SELECT COUNT(*) FROM bx_Posts P WITH(NOLOCK) WHERE ForumID = F.ForumID AND P.SortOrder<4000000000000000 AND P.CreateDate > @Today)
        ,TodayThreads = (SELECT COUNT(*) FROM bx_Threads T WITH(NOLOCK) WHERE ForumID = F.ForumID AND ThreadStatus<4 AND T.CreateDate > @Today)
        FROM bx_Forums F
        WHERE ForumID = F.ForumID

	--UPDATE bx_Forums SET  TodayPosts = 0,  TodayThreads = 0;
	UPDATE bx_Forums SET 
		YestodayLastThreadID=(SELECT ISNULL(MAX(ThreadID),0) FROM [bx_Threads] T WITH (NOLOCK) WHERE ForumID=F.ForumID AND ThreadStatus<4 AND T.CreateDate<@Today),
		YestodayLastPostID=(SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T2 WITH(NOLOCK) ON P.ThreadID=T2.ThreadID WHERE T2.ForumID=F.ForumID AND P.SortOrder<4000000000000000 AND T2.ThreadStatus<4 AND P.CreateDate<@Today)
		FROM [bx_Forums] F WHERE ForumID=F.ForumID;
/*    
    DECLARE @Today int;
    DECLARE @Yestoday datetime;
    SET @Yestoday = DATEADD(day, -1, GETDATE());
    SELECT @Today = DATEPART(year,@Yestoday)*10000+DATEPART(month,@Yestoday)*100+DATEPART(day,@Yestoday);

    DELETE [bx_DayLastThreads] WHERE [Day] = @Today;
    INSERT INTO [bx_DayLastThreads]([Day],[LastThreadID]) 
	SELECT @Today,(SELECT ISNULL(MAX(ThreadID),0) FROM bx_Threads T WITH(NOLOCK) WHERE CreateDate< CONVERT(DATETIME, CONVERT(varchar(100), GETDATE(), 23)));
*/
END
"
            )]
        #endregion
        public override void ResetTodayPosts()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ResetTodayPosts";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = int.MaxValue;

                query.ExecuteNonQuery();
            }
        }

        public override bool UpdateForums(IEnumerable<int> forumIDs, IEnumerable<int> sortOrders)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();

                int i = 0;
                foreach (int forumID in forumIDs)
                {
                    sql.AppendFormat(@"UPDATE [bx_Forums] SET [SortOrder] = @SortOrder_{0} WHERE [ForumID] = @ForumID_{0};", i);

                    int j = 0;
                    foreach (int sortOrder in sortOrders)
                    {
                        if (i == j)
                        {
                            query.CreateParameter<int>("@SortOrder_" + i, sortOrder, SqlDbType.Int);
                            query.CreateParameter<int>("@ForumID_" + i, forumID, SqlDbType.Int);
                            break;
                        }
                        j++;
                    }

                    i++;
                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
            return true;
        }

        #region 存储过程 bx_MoveForum
        [StoredProcedure(Name = "bx_MoveForum", Script = @"
CREATE PROCEDURE {name}
    @forumID int,
    @parentID int
AS
SET NOCOUNT ON;
IF (EXISTS(SELECT * FROM [bx_Forums] WITH(NOLOCK) WHERE ForumID=@parentID ) or @parentID=0)
	BEGIN
		DECLARE @MaxSortOrder int
		SELECT @MaxSortOrder=MAX(SortOrder) FROM bx_Forums WITH(NOLOCK) WHERE ParentID=@parentID
		IF @MaxSortOrder IS NULL
			SET @MaxSortOrder=0
		UPDATE bx_Forums 
		SET ParentID=@parentID, SortOrder=@MaxSortOrder+1
		WHERE ForumID = @forumID
		RETURN (0)
	END
ELSE
	RETURN (1)
"
            )]
        #endregion
        public override bool MoveFourm(int forumID, int parentID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_MoveForum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@forumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@parentID", parentID, SqlDbType.Int);
                query.CreateParameter<int>("@errorCode", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)query.Parameters["@errorCode"].Value == 0;
            }
        }

        #region 存储过程 bx_UpdateForumStatus
        [StoredProcedure(Name = "bx_UpdateForumStatus", Script = @"
CREATE PROCEDURE {name}
	@ForumIdentities varchar(8000),
	@ForumStatus tinyint
AS
	SET NOCOUNT ON

	BEGIN TRANSACTION
	
	IF @ForumStatus<2 BEGIN
		EXEC ('Alter PROCEDURE bx_GetDisabledTriggerForumIDs
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='''';
			END')
	END
	ELSE BEGIN
--		DECLARE @SQLString nvarchar(4000),@Count INT
--		SET @SQLString=N'SELECT @Count=count(*) FROM [bx_Forums] WHERE ForumStatus>1 AND ForumID Not in('+@ForumIdentities+')'
--		EXECUTE sp_executesql @SQLString,N'@Count int output',@Count output
--		IF @Count>0 BEGIN--存在不正常的版面
--			COMMIT TRANSACTION
--			RETURN 20
--		END

		EXEC ('Alter PROCEDURE bx_GetDisabledTriggerForumIDs
				@ForumIDs nvarchar(64) output
			AS
			BEGIN
				SET NOCOUNT ON;
				set @ForumIDs='''+@ForumIdentities+''';
			END')
	END

	IF @@ERROR<>0 BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

	EXEC ('Update [bx_Forums] SET ForumStatus='+@ForumStatus+' WHERE [ForumID] IN (' + @ForumIdentities + ') ') 
	
	IF @@ERROR<>0 BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END

COMMIT TRANSACTION
RETURN 0;
"
            )]
        #endregion
        public override bool UpdateForumStatus(IEnumerable<int> forumIDs, ForumStatus forumStatus)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateForumStatus";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@ForumIdentities", StringUtil.Join(forumIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ForumStatus", (int)forumStatus, SqlDbType.TinyInt);
                query.CreateParameter<int>("@errorCode", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)query.Parameters["@errorCode"].Value == 0;
            }
        }

        #region 存储过程 bx_DeleteForumThreads
        [StoredProcedure(Name = "bx_DeleteForumThreads", Script = @"
CREATE PROCEDURE {name}
	@ForumID int,
	@DeleteCount int
AS
BEGIN
	SET NOCOUNT ON;

	EXEC (N'DELETE bx_Threads WHERE ThreadID IN (SELECT TOP ' + @DeleteCount + N' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ForumID = ' + @ForumID + N')');
	RETURN @@ROWCOUNT;

END
"
            )]
        #endregion
        public override int DeleteForumThreads(int forumID, int deleteCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteForumThreads";
                query.CommandType = CommandType.StoredProcedure;
                query.CommandTimeout = int.MaxValue;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@DeleteCount", deleteCount, SqlDbType.Int);
                query.CreateParameter<int>("@DeletedCount", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)query.Parameters["@DeletedCount"].Value;
            }
        }


        #region 存储过程 bx_MoveForumThreads
        [StoredProcedure(Name = "bx_MoveForumThreads", Script = @"
CREATE PROCEDURE {name}
	@OldForumID int,
	@NewForumID int,
	@MoveCount int
AS
BEGIN
	SET NOCOUNT ON 
	EXEC (N'Update bx_Threads SET ForumID='+@NewForumID+' WHERE ThreadID IN (SELECT TOP ' + @MoveCount + N' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ForumID = ' + @OldForumID + N')');
	RETURN @@ROWCOUNT;
END
"
            )]
        #endregion
        public override int MoveForumThreads(int oldForumID, int newForumID, int moveCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_MoveForumThreads";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@OldForumID", oldForumID, SqlDbType.Int);
                query.CreateParameter<int>("@NewForumID", newForumID, SqlDbType.Int);
                query.CreateParameter<int>("@MoveCount", moveCount, SqlDbType.Int);
                query.CreateParameter<int>("@MovedCount", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return (int)query.Parameters["@MovedCount"].Value;
            }
        }

        public override void UpdateForumsExtendedAttributes(Dictionary<int, ForumExtendedAttribute> extendedAttributes)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();
                int i = 0;
                foreach (KeyValuePair<int, ForumExtendedAttribute> pair in extendedAttributes)
                {
                    sql.AppendFormat("UPDATE bx_Forums SET ExtendedAttributes = @ExtendedAttributes_{0} WHERE ForumID = @ForumID_{0};", i);
                    query.CreateParameter<string>(string.Format("@ExtendedAttributes_{0}", i), pair.Value.ToString(), SqlDbType.NText);
                    query.CreateParameter<int>(string.Format("@ForumID_{0}", i), pair.Key, SqlDbType.Int);
                    i++;
                }

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
        }



        #region threadCatalog

        #region 存储过程 bx_MoveForumThreads
        [StoredProcedure(Name = "bx_GetAllThreadCatalogs", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON
	SELECT * FROM bx_ThreadCatalogs WHERE ThreadCatalogID<>0
END
"
            )]
        #endregion
        public override ThreadCatalogCollection GetAllThreadCatalogs()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllThreadCatalogs";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCatalogCollection(reader);
                }
            }
        }


        public override ThreadCatalogCollection CreateThreadCatelogs(IEnumerable<string> catelogNames)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();

                int i = 0;
                foreach (string name in catelogNames)
                {
                    sql.AppendFormat(@"
IF NOT EXISTS(SELECT * FROM [bx_ThreadCatalogs] WHERE [ThreadCatalogName] = @Name_{0})
    INSERT INTO [bx_ThreadCatalogs]([ThreadCatalogName],[LogoUrl])VALUES(@Name_{0},'');
", i);

                    query.CreateParameter<string>("@Name_" + i, name, SqlDbType.NVarChar, 400);

                    i++;
                }

                query.CommandText = sql.ToString() + @"
SELECT * FROM [bx_ThreadCatalogs] WHERE [ThreadCatalogName] IN(@Names);
";
                query.CreateInParameter<string>("@Names", catelogNames);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCatalogCollection(reader);
                }
            }

        }

        public override bool UpdateThreadCatalogs(ThreadCatalogCollection threadCatalogs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();

                int i = 0;
                foreach (ThreadCatalog threadCatalog in threadCatalogs)
                {
                    sql.AppendFormat(@"UPDATE [bx_ThreadCatalogs] SET [ThreadCatalogName] = @ThreadCatalogName_{0},[LogoUrl]=@LogoUrl_{0} WHERE [ThreadCatalogID] = @ThreadCatalogID_{0};", i);


                    query.CreateParameter<string>("@ThreadCatalogName_" + i, threadCatalog.ThreadCatalogName, SqlDbType.NVarChar, 64);
                    query.CreateParameter<string>("@LogoUrl_" + i, threadCatalog.LogoUrl, SqlDbType.NVarChar, 512);
                    query.CreateParameter<int>("@ThreadCatalogID_" + i, threadCatalog.ThreadCatalogID, SqlDbType.Int);


                    i++;
                }

                query.CommandText = sql.ToString();
                query.ExecuteNonQuery();
            }
            return true;
        }

        #region 存储过程 bx_MoveForumThreads
        [StoredProcedure(Name = "bx_GetThreadCatalogsInForums", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON
	SELECT * FROM [bx_ThreadCatalogsInForums] ORDER BY SortOrder ASC
END
"
            )]
        #endregion
        public override ForumThreadCatalogCollection GetThreadCatalogsInForums()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetThreadCatalogsInForums";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ForumThreadCatalogCollection(reader);
                }
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_DeleteForumThreadCatalog", Script = @"
CREATE PROCEDURE {name}
    @ForumID            int,
    @ThreadCatalogID    int
AS BEGIN
    SET NOCOUNT ON;
    
    DELETE [bx_ThreadCatalogsInForums] WHERE [ForumID]=@ForumID AND [ThreadCatalogID]=@ThreadCatalogID;

    IF NOT EXISTS(SELECT * FROM [bx_ThreadCatalogsInForums] WHERE [ThreadCatalogID]=@ThreadCatalogID)
        DELETE [bx_ThreadCatalogs] WHERE [ThreadCatalogID] = @ThreadCatalogID;

END
")]
        #endregion
        public override void DeleteForumThreadCatalog(int forumID, int threadCatalogID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteForumThreadCatalog";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadCatalogID", threadCatalogID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_UpdateForumThreadCatalogStatus", Script = @"
CREATE PROCEDURE {name}
    @ForumID                int,
    @ThreadCatalogStatus    tinyint
AS BEGIN
    SET NOCOUNT ON;
    
    UPDATE [bx_Forums] SET ThreadCatalogStatus = @ThreadCatalogStatus WHERE ForumID = @ForumID;

END
")]
        #endregion
        public override bool UpdateForumThreadCatalogStatus(int forumID, ThreadCatalogStatus status)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdateForumThreadCatalogStatus";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadCatalogStatus", (int)status, SqlDbType.TinyInt);

                query.ExecuteNonQuery();
            }

            return true;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_AddThreadCatalogToForum", Script = @"
CREATE PROCEDURE {name}
	@ForumID int,
	@ThreadCatalogIDs varchar(8000),
	@SortOrders varchar(8000)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRANSACTION

    DECLARE @T table(tid int,total int);
	IF EXISTS (SELECT * FROM bx_ThreadCatalogsInForums WITH (NOLOCK) WHERE ForumID = @ForumID) BEGIN
        INSERT INTO @T SELECT ThreadCatalogID,TotalThreads FROM bx_ThreadCatalogsInForums;
		---删除forumID下的主题分类
		delete bx_ThreadCatalogsInForums where ForumID=@ForumID;
    END
		IF(@@error<>0)
		BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
		END
		DECLARE @ThreadCatalogID int,@i int,@SortOrder int,@Condition varchar(100),@j int
	
		SET @SortOrders=@SortOrders+N','
		SELECT @j=CHARINDEX(',',@SortOrders)
		
		SET @ThreadCatalogIDs=@ThreadCatalogIDs+N','
		SET @Condition='ForumID='+str(@ForumID)
		SELECT @i=CHARINDEX(',',@ThreadCatalogIDs)
		
		WHILE(@i>1)
		BEGIN
			SELECT @ThreadCatalogID=SUBSTRING(@ThreadCatalogIDs,0, @i)
			SELECT @SortOrder=SUBSTRING(@SortOrders,0, @j)
			--EXECUTE bx_Common_GetSortOrder 'bx_ThreadCatalogsInForums', @Condition,@SortOrder output
            DECLARE @TotalCount int;
            SELECT @TotalCount = ISNULL(total,0) FROM @T WHERE tid = @ThreadCatalogID;
			INSERT INTO bx_ThreadCatalogsInForums (ForumID, ThreadCatalogID,SortOrder,TotalThreads)
				VALUES (@ForumID, @ThreadCatalogID,@SortOrder,@TotalCount);
			
		
			SELECT @ThreadCatalogIDs=SUBSTRING(@ThreadCatalogIDs,@i+1,LEN(@ThreadCatalogIDs)-@i)
			SELECT @i=CHARINDEX(',',@ThreadCatalogIDs)
			
			SELECT @SortOrders=SUBSTRING(@SortOrders,@j+1,LEN(@SortOrders)-@j)
			SELECT @j=CHARINDEX(',',@SortOrders)
		END
		IF(@@error<>0)
				BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
		
		COMMIT TRANSACTION
		RETURN (0);
END
")]
        #endregion
        public override bool AddThreadCatalogToForum(int forumID, ForumThreadCatalogCollection forumThreadCatalogs)
        {
            List<int> threadCatalogIDs = new List<int>();
            List<int> sortOrders = new List<int>();

            foreach (ForumThreadCatalog catalog in forumThreadCatalogs)
            {
                threadCatalogIDs.Add(catalog.ThreadCatalogID);
                sortOrders.Add(catalog.SortOrder);
            }

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_AddThreadCatalogToForum";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadCatalogIDs", StringUtil.Join(threadCatalogIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@SortOrders", StringUtil.Join(sortOrders), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)query.Parameters["@ErrorCode"].Value == 0;
            }
        }


        #region 存储过程 bx_v5_UpdateForumThreadCatalogData
        [StoredProcedure(Name = "bx_v5_UpdateForumThreadCatalogData", Script = @"
CREATE PROCEDURE {name}
	 @ForumID            int
    ,@ThreadCatalogID    int
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=(
		SELECT COUNT(1) FROM bx_Threads WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID AND ThreadStatus<4
		) WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID;
END
"
            )]
        #endregion
        public override bool UpdateForumThreadCatalogData(int forumID, int threadCatalogID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_UpdateForumThreadCatalogData";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadCatalogID", threadCatalogID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }

            return true;
        }

        #endregion


        #region Moderators


        public override bool AddModerators(ModeratorCollection moderators)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder builder = new StringBuilder();

                int i = 0;
                foreach (Moderator m in moderators)
                {
                    builder.AppendFormat("DELETE FROM bx_Moderators WHERE UserID=@UserID{0} AND ForumID=@ForumID{0};", i);
                    builder.AppendFormat(@"INSERT INTO bx_Moderators( UserID, ForumID, BeginDate, EndDate, ModeratorType, SortOrder, AppointorID)
                                            VALUES(@UserID{0}, @ForumID{0}, @BeginDate{0}, @EndDate{0},@ModeratorType{0}, @SortOrder{0}, @AppointorID{0})", i);

                    query.CreateParameter<int>(string.Format("@UserID{0}", i), m.UserID, SqlDbType.Int);
                    query.CreateParameter<int>(string.Format("@ForumID{0}", i), m.ForumID, SqlDbType.Int);
                    query.CreateParameter<DateTime>(string.Format("@BeginDate{0}", i), m.BeginDate, SqlDbType.DateTime);
                    query.CreateParameter<DateTime>(string.Format("@EndDate{0}", i), m.EndDate, SqlDbType.DateTime);
                    query.CreateParameter<byte>(string.Format("@ModeratorType{0}", i), (byte)m.ModeratorType, SqlDbType.TinyInt);
                    query.CreateParameter<int>(string.Format("@SortOrder{0}", i), m.SortOrder, SqlDbType.Int);
                    query.CreateParameter<int>(string.Format("@AppointorID{0}", i), m.AppointorID, SqlDbType.Int);
                    i++;
                }
                query.CommandText = builder.ToString();
                query.ExecuteNonQuery();
                return true;
            }
        }


        public override bool RemoveModerator(int forumid, int userid)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE FROM bx_Moderators WHERE ForumID=@ForumID AND UserID=@UserID";
                query.CreateParameter<int>("@ForumID", forumid, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userid, SqlDbType.Int);
                query.ExecuteNonQuery();
            }
            return true;
        }

        #region 存储过程 bx_GetAllModerators
        [StoredProcedure(Name = "bx_GetAllModerators", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON
	SELECT * FROM [bx_Moderators] WHERE EndDate>GETDATE() ORDER BY ForumID, SortOrder ASC;
END
"
            )]
        #endregion
        public override ModeratorCollection GetAllModerators()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllModerators";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ModeratorCollection(reader);
                }
            }
        }

        #endregion
    }
}