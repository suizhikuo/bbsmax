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
    class PostDaoV5 : DataAccess.PostDaoV5
    {
        #region sortorder 16 位 只能用于Post了 Thread 15位
        /// <summary>
        /// 一般置顶 2开头
        /// </summary>
        private const long SortOrder_Stick = 2000000000000000;

        /// <summary>
        /// 总置顶 3开头
        /// </summary>
        private const long SortOrder_GlobalStick = 3000000000000000;

        /// <summary>
        /// 回收 4开头
        /// </summary>
        private const long SortOrder_Recycle = 4000000000000000;

        /// <summary>
        /// 未审核 5开头
        /// </summary>
        private const long SortOrder_Unapproved = 5000000000000000;

        #endregion



        private const string ThreadFields = @" * ";
//*,IsClosed=CASE
//WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
//WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
//WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
//ELSE 0
//END
//";
        private const string PostColumns = @"
[PostID]
,[MarkCount]
,[HistoryAttachmentIDs]
";
//
        private const string PostFields_Start = @"

        SET @PostFieldsString = '

DECLARE @S2 VARCHAR(8000),@D2 DATETIME;

            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
SELECT @D2 = GETDATE();
		    INSERT INTO @PostIDTable (
" + PostColumns + @"
) ' + @SQLString + ';

    SET @S2 =''|A1=''+ " + sql2 + @";
";

        private const string PostFields = PostFields_Start + PostFields_End + s1;
        private const string PostFields2 = PostFields_Start + PostFields_End + s2;


        private const string PostFields3 = PostFields_Start + @"

IF ''' + @ExcuteSqlString2 + ''' = ''1''  BEGIN 
SELECT @D2 = GETDATE();
		        INSERT INTO @PostIDTable (
    " + PostColumns + @"
    ) ' + @SQLString2 + ';
    SET @S2 =@S2 + ''|A2=''+ " + sql2 + @";
END
" + PostFields_End + s1;

        private const string PostFields_End = "'+" + PostFields_ExtendInfo + ";";

//        private const string PostFields_End = @"
//
//		    DECLARE @HistoryAttach table(PostID int,AttachID int);
//		    DECLARE @Count int,@I int;
//            SELECT @D2 = GETDATE();
//		    SELECT @Count = COUNT(*) FROM @PostIDTable;
//            
//    SET @S2 =@S2 + ''|A3=''+ " + sql2 + @";
//		    SET @I = 1;
//    		
//            SELECT @D2 = GETDATE();
//		    WHILE(@I < @Count+1) BEGIN
//			    DECLARE @PID int,@HistoryAttachmentString varchar(500);
//			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs FROM @PostIDTable WHERE ID = @I;
//			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
//				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
//			    SET @I = @I + 1;
//		    END
//    SET @S2 =@S2 + ''|A4=''+ " + sql2 + @";
//            SELECT @D2 = GETDATE();
//            --SELECT * FROM @PostIDTable ORDER BY SortOrder;
//		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
// 
//    SET @S2 =@S2 + ''|A5=''+ " + sql2 + @";
//            SELECT @D2 = GETDATE();
//
//		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;
//
//            
//    SET @S2 =@S2 + ''|A6=''+ " + sql2 + @";
//            SELECT @D2 = GETDATE();
//
//		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
//    		
//    SET @S2 =@S2 + ''|A7=''+ " + sql2 + @";
//            SELECT @D2 = GETDATE();
//		    SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.* FROM [bx_PostMarks] m WITH (NOLOCK) INNER JOIN @PostIDTable i ON m.PostID = i.PostID ORDER BY m.PostMarkID DESC;
//
//    SET @S2 =@S2 + ''|A8=''+ " + sql2 + @";
//            
//            SELECT @S2;
//';
//
//";

        private const string s1 = " EXEC (@PostFieldsString); ";
        private const string s2 = @"

    IF @BestPostID > 0 BEGIN
        EXECUTE sp_executesql 
          @PostFieldsString,
          N'@TID INT,@BID INT,@CID INT',
          @TID = @ThreadID
         ,@BID = @BestPostID
         ,@CID = @ContentID
    END
    ELSE BEGIN
        EXECUTE sp_executesql 
          @PostFieldsString,
          N'@TID INT,@CID INT',
          @TID = @ThreadID
         ,@CID = @ContentID
    END

";


        private const string PostFields_DeclarePostIDTable = @"

        SET @PostFieldsString = '
            DECLARE @S2 VARCHAR(8000),@D2 DATETIME;
            DECLARE @PostIDTable table(ID int identity(1,1),
[PostID] [int] NOT NULL
,[MarkCount] [int] NULL
,[HistoryAttachmentIDs]   varchar(500) NULL
);
'
";

        private const string PostFields_ExtendInfo = @"
'
		    DECLARE @HistoryAttach table(PostID int,AttachID int);
		    DECLARE @Count int,@I int;
            SELECT @D2 = GETDATE();
		    SELECT @Count = COUNT(*) FROM @PostIDTable;
            
            DECLARE @PostMarkIDTable table(MarkID int);
    SET @S2 =@S2 + ''|A3=''+ " + sql2 + @";
		    SET @I = 1;
    		
            SELECT @D2 = GETDATE();
		    WHILE(@I < @Count+1) BEGIN
			    DECLARE @PID int,@HistoryAttachmentString varchar(500),@PostMarkCount int;
			    SELECT @PID = PostID,@HistoryAttachmentString = HistoryAttachmentIDs,@PostMarkCount = MarkCount FROM @PostIDTable WHERE ID = @I;
			    IF @HistoryAttachmentString IS NOT NULL AND @HistoryAttachmentString <> ''''
				    INSERT INTO @HistoryAttach(PostID,AttachID) SELECT @PID,item FROM bx_GetIntTable(@HistoryAttachmentString,'','');
			    SET @I = @I + 1;
                IF @PostMarkCount > 0
                    INSERT INTO @PostMarkIDTable(MarkID) SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.PostMarkID FROM [bx_PostMarks] m WHERE m.PostID = @PID ORDER BY m.PostMarkID DESC;
		    END
    SET @S2 =@S2 + ''|A4=''+ " + sql2 + @";
            SELECT @D2 = GETDATE();
            --SELECT * FROM @PostIDTable ORDER BY SortOrder;
		    SELECT p.* FROM @PostIDTable i INNER JOIN [bx_Posts] p WITH (NOLOCK) ON i.PostID = p.PostID ORDER BY p.SortOrder;
 
    SET @S2 =@S2 + ''|A5=''+ " + sql2 + @";
            SELECT @D2 = GETDATE();

		    SELECT a.* FROM [bx_Attachments] a WITH (NOLOCK) INNER JOIN @PostIDTable i ON a.PostID = i.PostID ORDER BY a.AttachmentID;

            
    SET @S2 =@S2 + ''|A6=''+ " + sql2 + @";
            SELECT @D2 = GETDATE();

		    SELECT h.PostID as HPostID, a.* FROM [bx_Attachments] a WITH (NOLOCK), @HistoryAttach h WHERE a.AttachmentID = h.AttachID;
    		
    SET @S2 =@S2 + ''|A7=''+ " + sql2 + @";
            SELECT @D2 = GETDATE();
            SELECT * FROM [bx_PostMarks] WHERE PostMarkID in(SELECT MarkID FROM @PostMarkIDTable);
		    --SELECT TOP ' + CAST(@TopMarkCount as varchar(16)) + ' m.* FROM [bx_PostMarks] m WITH (NOLOCK) INNER JOIN @PostIDTable i ON m.PostID = i.PostID ORDER BY m.PostMarkID DESC;

    SET @S2 =@S2 + ''|A8=''+ " + sql2 + @";
            
            SELECT @S2;
'
";



        private static Dictionary<string, int> sortNumbers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private int GetSortNumber(string key)
        {
            int sortNumber;
            if (sortNumbers.TryGetValue(key, out sortNumber))
            {
                if (sortNumber >= 99)
                    sortNumber = 0;
                else
                    sortNumber++;

                sortNumbers[key] = sortNumber;

                return sortNumber;
            }
            else
            {
                sortNumbers.Add(key, 0);
                return 0;
            }
        }


        private long GetSortOrder(string key, bool isThreadSortOrder)
        {
            int postRandNumber = GetSortNumber(key);
            DateTime tempDateTime = DateTime.Parse("1970-01-01 00:00:00");
            long resultSortOrder = (long)((DateTimeUtil.Now - tempDateTime).TotalMilliseconds * 100);
            if (postRandNumber > 0)
                resultSortOrder += postRandNumber % 100;

            if (isThreadSortOrder)
                return resultSortOrder;
            else
                return resultSortOrder + 1 * 1000000000000000;
        }

        private static int tempPostID = 0;
        private int GetTempPostID()
        {
            tempPostID = tempPostID - 1;
            return tempPostID;
        }

        private static void SetTopMarkCountParam(SqlQuery query)
        {
            query.CreateParameter<int>("@TopMarkCount", AllSettings.Current.BbsSettings.ShowMarksCount, SqlDbType.Int);
        }


        [StoredProcedure(Name = "bx_DeleteThreads", Script = @"
CREATE PROCEDURE {name}
	@ForumID int,
	@ThreadStatus tinyint,
	@ThreadIdentities varchar(8000)
AS
	SET NOCOUNT ON 

	IF @ThreadStatus<4
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ')')
	ELSE IF @ThreadStatus=4
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ') AND ThreadStatus = 4')
	ELSE
		EXEC ('DELETE [bx_Threads] WHERE  [ForumID]=' + @ForumID+' AND [ThreadID] IN (' + @ThreadIdentities + ') AND ThreadStatus = 5')

	DECLARE @RowCount int
	SET @RowCount=@@ROWCOUNT
	IF @RowCount> 0
	BEGIN
		----------记录统计数据:增加删除主题数---------------
		EXEC [bx_DoCreateStat] @ForumID,7, @RowCount
		--------------------------
		RETURN (0);
	END
	ELSE
	begin
		RETURN (1)
	end
")]
        public override bool DeleteThreads(int forumID, ThreadStatus threadStatus, IEnumerable<int> threadIdentities)
        {
            int postCount = 0;
            ThreadCollectionV5 threads = GetThreads(threadIdentities);

            foreach (BasicThread thread in threads)
            {
                postCount += thread.TotalReplies + 1;
                if (postCount > 1000)
                    break;
            }

            if (postCount == 0)
                return true;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteThreads";
                query.CommandType = CommandType.StoredProcedure;

                if (postCount > 1000)
                    query.CommandTimeout = 600;

                query.CreateParameter<string>("@ThreadIdentities", StringUtil.Join(threadIdentities), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadStatus", (int)threadStatus, SqlDbType.TinyInt);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return Convert.ToInt32(query.Parameters["@ErrorCode"].Value) == 0;
            }
        }

        public override PostAuthorExtendInfo GetPostAuthorExtendInfo(int userID, DataAccessLevel dataAccessLevel)
        {
            string privacyCondition = null;

            if (dataAccessLevel == DataAccessLevel.Normal)
            {
                privacyCondition = " AND [PrivacyType] = 0 ";
            }
            else if (dataAccessLevel == DataAccessLevel.Friend)
            {
                privacyCondition = " AND [PrivacyType] IN (0, 1)";
            }



            string privacyConditionOfPhoto = string.Empty;

            if (dataAccessLevel == DataAccessLevel.Normal)
            {
                privacyConditionOfPhoto = " AND AlbumID IN(SELECT AlbumID FROM bx_Albums WHERE UserID = @UserID AND [PrivacyType] = 0)";
            }
            else if (dataAccessLevel == DataAccessLevel.Friend)
            {
                privacyConditionOfPhoto = " AND AlbumID IN(SELECT AlbumID FROM bx_Albums WHERE UserID = @UserID AND [PrivacyType] IN (0, 1))";
            }


            StringBuffer sql = new StringBuffer();

            sql += "SELECT TOP 1 * FROM bx_BlogArticles WHERE UserID = @UserID" + privacyCondition + " ORDER BY ArticleID DESC; \r\n";
            sql += "SELECT TOP 4 * FROM bx_Photos WHERE UserID = @UserID" + privacyConditionOfPhoto + " ORDER BY PhotoID DESC; \r\n";
            sql += "SELECT TOP 6 *, B.Text, B.KeywordVersion FROM bx_Impressions A LEFT JOIN bx_ImpressionTypes B ON B.TypeID = A.TypeID WHERE UserID = @UserID ORDER BY UpdateDate DESC;";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = sql.ToString();

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    PostAuthorExtendInfo result = new  PostAuthorExtendInfo();

                    result.NewArticles = new BlogArticleCollection(reader);

                    reader.NextResult();

                    result.NewPhotos = new PhotoCollection(reader);

                    reader.NextResult();

                    result.Impressions = new ImpressionCollection(reader);

                    return result;
                }
            }
        }

        public override AttachmentCollection GetAttachments(int operatorUserID, DateTime? beginDate, DateTime? endDate, string keyword, int pageNumber, int pageSize, ExtensionList fileTypes)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Attachments]";
                query.Pager.SortField = "[AttachmentID]";


                StringBuilder condition = new StringBuilder();
                condition.Append(" [UserID] = @UserID");

                query.CreateParameter<int>("@UserID", operatorUserID, SqlDbType.Int);

                if (beginDate != null)
                {
                    condition.Append(" AND [CreateDate] >= @BeginDate");
                    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                }
                if (endDate != null)
                {
                    condition.Append(" AND [CreateDate] <= @EndDate");
                    query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                }

                if (false == string.IsNullOrEmpty(keyword))
                {
                    condition.Append(" AND [FileName] LIKE '%'+@Keyword+'%'");
                    query.CreateParameter<string>("@Keyword", keyword, SqlDbType.NVarChar, 100);
                }
                if (fileTypes != null && fileTypes.Count > 0 && !fileTypes.Contains("*"))
                {
                    condition.Append(" And (");
                    int i = 0;
                    string ft;
                    foreach (string s in fileTypes)
                    {
                        ft = "@f_" + i++;
                        condition.Append(" FileType=" + ft);
                        query.CreateParameter<string>(ft, s, SqlDbType.VarChar, 10);

                        if (fileTypes.Count > i)
                            condition.Append(" OR ");
                    }

                    condition.Append(")");
                }

                query.Pager.Condition = condition.ToString();


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    AttachmentCollection attachments = new AttachmentCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            attachments.TotalRecords = reader.GetInt32(0);
                        }
                    }
                    return attachments;
                }
            }
        }



        #region 存储过程 bx_GetThread
        [StoredProcedure(Name = "bx_GetThreadByID", Script = @"
CREATE PROCEDURE {name}
    @ThreadID int
AS
BEGIN
    SET NOCOUNT ON;

	SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID;
     
END
"
            )]
        #endregion
        public override BasicThread GetThread(int threadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetThreadByID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return GetThread(reader, null);
                    }
                }
            }

            return null;
        }

        [StoredProcedure(Name = "bx_GetPosts", Script = @"
CREATE PROCEDURE {name}
	@ThreadID int
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@ThreadID AND [SortOrder]<4000000000000000 ORDER BY [SortOrder]
	RETURN
")]
        public override PostCollectionV5 GetPosts(int threadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetPosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PostCollectionV5(reader);
                }
            }
        }

        /// <summary>
        /// 解锁/锁定主题
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <param name="lockThread"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SetThreadsLock", Script = @"
CREATE PROCEDURE {name}
	@ForumID                    int,
	@ThreadIdentities           varchar(8000),
	@LockThread                 bit
AS
BEGIN
	SET NOCOUNT ON 
	EXEC ('UPDATE bx_Threads SET IsLocked='+@LockThread+' WHERE ThreadID IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID) 
	RETURN
END
")]
        public override bool SetThreadsLock(int forumID, IEnumerable<int> threadIdentities, bool lockThread)
        {
            string identitieString = StringUtil.Join(threadIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetThreadsLock";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadIdentities", StringUtil.Join(threadIdentities), SqlDbType.VarChar, 8000);
                query.CreateParameter<bool>("@LockThread", lockThread, SqlDbType.Bit);

                query.ExecuteNonQuery();
            }
            return true;
        }


        /// <summary>
        /// 自动沉帖（仍然可以回复，但不会被顶到前面）
        /// </summary>
        /// <param name="forumID"></param>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        //        [StoredProcedure(Name="SetThreadNotUpdateSortOrder",Script= @"
        //CREATE PROCEDURE {name}
        //@ThreadIDs   varchar(8000)
        //,@UpdateSortOrder bit
        //,@ForumID  int
        //AS
        //
        //BEGIN
        //
        //DECLARE @ThreadIDTable Table(id int);
        //
        //INSERT @ThreadIDTable SELECT (bx_GetIntTable(@ThreadIDs,','))
        //
        //    IF UpdateSortOrder=0 BEGIN
        //
        //        DECLARE @TempSortOrder BIGINT,@PostDate datetime
        //	    SET @PostDate = getdate();
        //	    EXEC [bx_GetSortOrder] @ThreadStatus, @ThreadRandNumber, @PostDate, @TempSortOrder OUTPUT;
        //
        //    END
        //    ELSE
        //    BEGIN
        //
        //    END
        //
        //END
        //")]
        public override bool SetThreadNotUpdateSortOrder(int forumID, IEnumerable<int> threadIds, bool updateSortOrder)
        {
            string cmdText = "UPDATE bx_Threads SET UpdateSortOrder = @UpdateSortOrder WHERE ThreadID IN(@ThreadIds) AND ForumID = @ForumID";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = cmdText;
                query.CommandType = CommandType.Text;
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateSortOrder", updateSortOrder, SqlDbType.Bit);

                query.CreateInParameter("@ThreadIds", threadIds);

                query.ExecuteNonQuery();
            }

            return true;
        }


        /// <summary>
        /// 移动帖子
        /// </summary>
        /// <param name="OldForumID"></param>
        /// <param name="NewForumID"></param>
        /// <param name="IsKeepLink">是否在原版块保持链接</param>
        /// <returns></returns>
        #region 存储过程
        [StoredProcedure(Name = "bx_MoveThreads", Script = @"
CREATE PROCEDURE {name} 
	@OldForumID                 int,
	@NewForumID                 int,
	@ThreadIdentities           varchar(8000),
	@IsKeepLink                 bit
AS
BEGIN
	SET NOCOUNT ON;
	IF EXISTS (SELECT * FROM [bx_Forums] WITH (NOLOCK) WHERE ForumID=@NewForumID AND ParentID<>0)
		BEGIN
			DECLARE @ThreadID int,@SortOrder bigint,@i int
			BEGIN TRANSACTION

			SET @ThreadIdentities=@ThreadIdentities+N','
			SELECT @i=CHARINDEX(',',@ThreadIdentities)
			
			DECLARE @tempTable1 TABLE(TempThreadID INT,TempSortOrder BIGINT)
			DECLARE @tempTable2 TABLE(TempThreadID INT,TempSortOrder BIGINT,TempThreadCatalogID INT,TempThreadType TINYINT,TempThreadStatus TINYINT,TempIconID INT,TempSubject NVARCHAR(256),TempPostUserID INT,TempPostNickName NVARCHAR(64),TempLastPostUserID INT,TempLastPostNickName NVARCHAR(64),TempCreateDate datetime,TempUpdateDate datetime,TempLastPostID int)
			DECLARE @tempTable3 TABLE(TempCatalogID INT,TempTotalThreads INT)

			SELECT @SortOrder=ISNULL(MAX(SortOrder),0) FROM bx_Threads WITH (NOLOCK)

			DECLARE @ThreadCount INT
			SET @ThreadCount=0;
			WHILE(@i>1) BEGIN
				SELECT @ThreadID=SUBSTRING(@ThreadIdentities,0, @i)
				--SET @OldSortOrder=-1
				IF(@IsKeepLink=1) BEGIN
                    DECLARE @LSortOrder bigint;
                    DECLARE @LPostID int;
                    SELECT @LSortOrder = ISNULL(MAX(SortOrder),0)+1 FROM bx_Posts WITH (NOLOCK) WHERE SortOrder<2000000000000000;
                    INSERT INTO bx_Posts(ForumID,ThreadID,Subject,Content,NickName,SortOrder) VALUES(@OldForumID,@ThreadID,'','','',@LSortOrder);
                    IF @@error<>0 BEGIN
				        ROLLBACK TRANSACTION
				        RETURN (-1)
			        END

                    SELECT @LPostID = @@IDENTITY;
                    DELETE bx_Posts WHERE PostID = @LPostID;                    
                    IF @@error<>0 BEGIN
				        ROLLBACK TRANSACTION
				        RETURN (-1)
			        END

					INSERT INTO @tempTable2 SELECT @ThreadID,SortOrder,ThreadCatalogID,ThreadType,6,IconID,(CAST(ThreadID as nvarchar(16))+N','+Subject),PostUserID,PostNickName,LastPostUserID,LastPostNickName,CreateDate,UpdateDate,@LPostID FROM  bx_Threads WITH (NOLOCK) WHERE ForumID = @OldForumID AND ThreadID=@ThreadID
					--IF EXISTS(SELECT * FROM @tempTable3 WHERE TempCatalogID=(SELECT TempCatalogID WHERE)) 
					DECLARE @CatalogID int
					SELECT @CatalogID=TempThreadCatalogID FROM @tempTable2 WHERE  TempThreadID=@ThreadID
					IF EXISTS(SELECT * FROM @tempTable3 WHERE TempCatalogID=@CatalogID) 
						UPDATE @tempTable3 SET TempTotalThreads=TempTotalThreads+1 WHERE TempCatalogID=@CatalogID
					ELSE
						INSERT INTO @tempTable3 VALUES(@CatalogID,1)
				END
				SET @SortOrder=@SortOrder+1;
				INSERT INTO @tempTable1 VALUES(@ThreadID,@SortOrder)

				SELECT @ThreadIdentities=SUBSTRING(@ThreadIdentities,@i+1,LEN(@ThreadIdentities)-@i)
				SELECT @i=CHARINDEX(',',@ThreadIdentities)
				
				SET @ThreadCount=@ThreadCount+1;
			END

			UPDATE bx_Threads SET ForumID=@NewForumID,SortOrder=TempSortOrder FROM @tempTable1 WHERE ThreadID = TempThreadID
			IF @@error<>0 BEGIN
				ROLLBACK TRANSACTION
				RETURN (-1)
			END
			IF(@IsKeepLink=1) BEGIN
				INSERT INTO bx_Threads(ForumID,ThreadCatalogID,ThreadType,IconID,Subject,PostUserID,PostNickName,LastPostUserID,LastPostNickName,CreateDate,UpdateDate,SortOrder,LastPostID) select @OldForumID,TempThreadCatalogID,12,TempIconID,TempSubject,TempPostUserID,TempPostNickName,TempLastPostUserID,TempLastPostNickName,TempCreateDate,TempUpdateDate,TempSortOrder,TempLastPostID FROM @tempTable2
				IF @@error<>0 BEGIN
					ROLLBACK TRANSACTION
					RETURN (-1)
				END
				UPDATE bx_Forums SET TotalThreads=TotalThreads+@ThreadCount WHERE  ForumID=@OldForumID
				UPDATE bx_ThreadCatalogsInForums SET TotalThreads=TotalThreads+T3.TempTotalThreads FROM  @tempTable3 T3 WHERE ForumID=@OldForumID AND ThreadCatalogID=T3.TempCatalogID
			END
			COMMIT TRANSACTION
			RETURN 0
		END
	ELSE
		RETURN (-1)

END

")]
        #endregion
        public override bool MoveThreads(int oldForumID, int newForumID, IEnumerable<int> threadIdentities, bool isKeepLink)
        {
            string identities = StringUtil.Join(threadIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_MoveThreads";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@OldForumID", oldForumID, SqlDbType.Int);
                query.CreateParameter<int>("@NewForumID", newForumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadIdentities", identities, SqlDbType.VarChar, 8000);
                query.CreateParameter<bool>("@IsKeepLink", isKeepLink, SqlDbType.Bit);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)query.Parameters["@ErrorCode"].Value == 0;
            }
        }


        public override ThreadCollectionV5 GetThreadsByStatus(ThreadStatus status, int? forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int pageNumber, int pageSize, ref int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = isDesc;
                query.Pager.ResultFields = ThreadFields;
                ProcessSortField(query, sortType);

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Threads]";

                StringBuilder condition = new StringBuilder();

                if (forumID != null)
                {
                    condition.Append(" [ForumID] = @ForumID ");
                    query.CreateParameter<int>("@ForumID", forumID.Value, SqlDbType.Int);
                }

                if (condition.Length != 0)
                    condition.Append(" AND ");


                condition.Append(" [ThreadStatus] = @ThreadStatus ");
                query.CreateParameter<int>("@ThreadStatus", (int)status, SqlDbType.TinyInt);
                //switch (status)
                //{
                //    case ThreadStatus.Normal:
                //        condition.Append(" [ThreadStatus] = 1 ");
                //        break;
                //    case ThreadStatus.Sticky:
                //        condition.Append(" [ThreadStatus] = 2 ");
                //        break;
                //    case ThreadStatus.GlobalSticky:
                //        condition.Append(" [ThreadStatus] = 3 ");
                //        break;
                //    case ThreadStatus.Recycled:
                //        condition.Append(" [ThreadStatus] = 4 ");
                //        break;
                //    default:
                //        condition.Append(" [ThreadStatus] = 5 ");
                //        break;
                //}

                if (beginDate != null)
                {
                    condition.Append(" AND [CreateDate] >= @BeginDate ");
                    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                }

                if (endDate != null)
                {
                    condition.Append(" AND [CreateDate] <= @EndDate ");
                    query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                }

                query.Pager.Condition = condition.ToString();

                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }



        public override void GetThreadWithReplies(int threadID, int totalCount, bool getThread, int pageNumber, int pageSize, out BasicThread thread, out PostCollectionV5 replies)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = "*";
                query.Pager.SortField = "[SortOrder]";
                query.Pager.PrimaryKey = "[PostID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Posts]";

                query.Pager.Condition = " ThreadID=@ThreadID AND [ThreadStatus]<4 ";

                if (getThread)
                {
                    query.Pager.AfterExecute = string.Concat(@"
SELECT ", ThreadFields, @"
          FROM [bx_Threads] WITH (NOLOCK)
         WHERE [ThreadID] = @ThreadID AND [ThreadStatus]<4;
");
                }

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                thread = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    replies = new PostCollectionV5(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            thread = new BasicThread(reader);
                        }
                    }
                }
            }
        }


        public override bool SetThreadsStatus(int forumID, IEnumerable<int> stickForumIDs, IEnumerable<int> threadIdentities, ThreadStatus threadLocation, StickSortType? sortType)
        {
            string identitieString = StringUtil.Join(threadIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sqlString = new StringBuilder();
                if ((threadLocation == ThreadStatus.Sticky || threadLocation == ThreadStatus.GlobalSticky) && sortType != null && sortType == StickSortType.StickDate)
                {
                    foreach (int threadID in threadIdentities)
                    {
                        if (forumID == 0)
                            sqlString.AppendFormat("UPDATE bx_Threads SET ThreadStatus = @ThreadStatus,SortOrder = @SortOrder_{0} WHERE ThreadID = @ThreadID_{0};", threadID);
                        else
                            sqlString.AppendFormat("UPDATE bx_Threads SET ThreadStatus = @ThreadStatus,SortOrder = @SortOrder_{0} WHERE ForumID = @ForumID AND ThreadID = @ThreadID_{0};", threadID);
                        query.CreateParameter<long>(string.Format("@SortOrder_{0}", threadID), GetSortOrder("thread",true), SqlDbType.BigInt);
                        query.CreateParameter<int>(string.Format("@ThreadID_{0}", threadID), threadID, SqlDbType.Int);
                    }
                }
                else
                {
                    sqlString.Append(@"
	IF @ForumID = 0 BEGIN
		EXEC ('UPDATE bx_Threads SET ThreadStatus = ' + @ThreadStatus + ' WHERE ThreadID IN (' + @ThreadIdentities + ')')
		 
	END
	ELSE BEGIN
		EXEC ('UPDATE bx_Threads SET ThreadStatus = ' + @ThreadStatus + ' WHERE ForumID = ' + @ForumID + ' AND ThreadID IN (' + @ThreadIdentities + ')')
	END
");

                }

                sqlString.Append("EXEC('DELETE bx_StickThreads WHERE ThreadID in(' + @ThreadIdentities + ');');");

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadStatus", (int)threadLocation, SqlDbType.TinyInt);
                query.CreateParameter<string>("@ThreadIdentities", identitieString, SqlDbType.VarChar, 8000);


                //query.ExecuteNonQuery();

                if (threadLocation == ThreadStatus.Sticky && stickForumIDs != null && ValidateUtil.HasItems<int>(stickForumIDs))
                {
                    //StringBuilder sql = new StringBuilder();
                    //query.Parameters.Clear();
                    int i = 0;
                    foreach (int threadID in threadIdentities)
                    {
                        foreach (int tempForumID in stickForumIDs)
                        {
                            sqlString.AppendFormat("INSERT INTO bx_StickThreads(ThreadID,ForumID) VALUES(@ThreadID2_{0},@ForumID2_{0});", i);
                            query.CreateParameter<int>("@ThreadID2_" + i, threadID, SqlDbType.Int);
                            query.CreateParameter<int>("@ForumID2_" + i, tempForumID, SqlDbType.Int);
                            i++;
                        }
                    }

                }

                query.CommandText = sqlString.ToString();
                query.CommandType = CommandType.Text;
                query.ExecuteNonQuery();
            }
            return true;
        }



        public override bool GetThread(int threadID, bool normalOnly, int totalCount, bool getThread, bool getReplies, int postUserID, int pageIndex, int pageSize, out BasicThread thread, out PostCollectionV5 replies, out int repliesCountForUser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置标题样式
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <param name="Style"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SetThreadsSubjectStyle", Script = @"
CREATE PROCEDURE {name}
	@ForumID            int,
	@ThreadIdentities   varchar(8000),
	@Style              nvarchar(300)
AS
BEGIN
	SET NOCOUNT ON
	EXEC ('UPDATE [bx_Threads] SET SubjectStyle='''+@Style+''' WHERE [ThreadID] IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID) 
	IF @@ROWCOUNT > 0
		RETURN (0)
	ELSE
		RETURN (1)
END
")]
        public override bool SetThreadsSubjectStyle(int forumID, IEnumerable<int> threadIdentities, string style)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetThreadsSubjectStyle";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadIdentities", StringUtil.Join(threadIdentities), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@Style", style, SqlDbType.NVarChar, 300);

                query.ExecuteNonQuery();
            }
            return true;
        }

        public override ThreadCollectionV5 GetThreads(int pageNumber, TopicFilter filter, Guid[] excludeRoleIDs, ref int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = filter.IsDesc;

                switch (filter.Order)
                {
                    case TopicFilter.OrderBy.TopicID:
                        query.Pager.SortField = "ThreadID";
                        break;
                    case TopicFilter.OrderBy.LastReplyDate:
                        query.Pager.SortField = "UpdateDate";
                        break;
                    case TopicFilter.OrderBy.ReplyCount:
                        query.Pager.SortField = "TotalReplies";
                        break;
                    default:
                        query.Pager.SortField = "TotalViews";
                        break;
                }

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_TopicsWithContents]";

                query.Pager.Condition = BuilderSearchThreadCondition(filter, excludeRoleIDs, query, false);

                ThreadCollectionV5 threads;
                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    threads = new ThreadCollectionV5(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return threads;
                }
            }
        }


        public override DeleteResult DeleteSearchTopics(TopicFilter filter, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult, int topCount, out int deletedCount, out List<int> threadIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {

                string conditon = BuilderSearchThreadCondition(filter, excludeRoleIDs, query, false);

                query.CommandText = @"
DECLARE @Table table(TempTopicID int,TempUserID int,TempForumID int,TempThreadStatus tinyint);
INSERT INTO @Table SELECT TOP (@TopCount) [ThreadID],[PostUserID],[ForumID],[ThreadStatus] FROM [bx_TopicsWithContents] WITH (NOLOCK) WHERE " + conditon + @";
SELECT TempTopicID FROM @Table;
--SELECT FileID FROM [bx_Attachments] WHERE PostID IN(SELECT PostID FROM [bx_Posts] WHERE ThreadID IN(SELECT TempTopicID FROM @Table))
--    AND FileID NOT IN(SELECT FileID FROM [bx_Attachments] WHERE PostID NOT IN(SELECT PostID FROM [bx_Posts] WHERE ThreadID IN(SELECT TempTopicID FROM @Table)));
";

                if (getDeleteResult)
                {
                    query.CommandText += @"
SELECT [TempUserID],[TempForumID],COUNT(*) AS [Count] FROM @Table WHERE TempThreadStatus<4 GROUP BY [TempUserID],[TempForumID];";

                }


                query.CommandText = query.CommandText + @"
DELETE [bx_Threads] WHERE ThreadID IN(SELECT TempTopicID FROM @Table);
SELECT @@ROWCOUNT;
";
                query.CreateTopParameter("@TopCount", topCount);

                threadIDs = new List<int>();
                //                fileIDs = new List<string>();
                deletedCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    while (reader.Read())
                    {
                        threadIDs.Add(reader.Get<int>("TempTopicID"));
                    }
                    //if (reader.NextResult())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        fileIDs.Add(reader.Get<string>("FileID"));
                    //    }
                    //}

                    if (getDeleteResult)
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                deleteResult.Add(reader.Get<int>("TempUserID"), reader.Get<int>("Count"), reader.Get<int>("TempForumID"));
                            }
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            deletedCount = reader.Get<int>(0);
                        }
                    }
                    return deleteResult;
                }
            }
        }

        private string BuilderSearchThreadCondition(TopicFilter filter, IEnumerable<Guid> excludeRoleIDs, SqlQuery query, bool startWithWhere)
        {
            SqlConditionBuilder condition;
            if (startWithWhere)
                condition = new SqlConditionBuilder(SqlConditionStart.Where);
            else
                condition = new SqlConditionBuilder(SqlConditionStart.None);

            condition += (filter.ForumID == null ? "" : ("AND [ForumID] = @ForumID "));
            condition += (filter.UserID == null ? "" : ("AND [PostUserID] = @UserID "));
            condition += (filter.TopicID == null ? "" : ("AND [ThreadID] = @TopicID "));
            condition += (string.IsNullOrEmpty(filter.CreateIP) ? "" : ("AND [IPAddress] = @CreateIP "));

            if (filter.Status == null)
                condition += (filter.IncludeStick == false ? "AND [ThreadStatus] = 1 " : ("AND [ThreadStatus] < 4 "));
            else if (filter.Status.Value == ThreadStatus.Recycled)
                condition += ("AND [ThreadStatus] = 4 ");
            else if (filter.Status.Value == ThreadStatus.UnApproved)
                condition += ("AND [ThreadStatus] = 5 ");

            condition += (filter.IncludeValued == true ? "" : ("AND [IsValued] = 0 "));
            condition += (filter.MinReplyCount == null ? "" : ("AND [TotalReplies] >= @MinReplyCount "));
            condition += (filter.MaxReplyCount == null ? "" : ("AND [TotalReplies] <= @MaxReplyCount "));
            condition += (filter.MinViewCount == null ? "" : ("AND [TotalViews] >= @MinViewCount "));
            condition += (filter.MaxViewCount == null ? "" : ("AND [TotalViews] <= @MaxViewCount "));
            condition += (filter.BeginDate == null ? "" : ("AND [CreateDate] > @BeginDate "));
            condition += (filter.EndDate == null ? "" : ("AND [CreateDate] < @EndDate "));

            if (string.IsNullOrEmpty(filter.KeyWord) == false)
            {
                if (filter.SearchMode == SearchArticleMethod.Subject)
                {
                    condition += " AND [Subject] LIKE '%'+@keyword+'%' ";
                }
                else if (filter.SearchMode == SearchArticleMethod.FullText)
                {
                    condition += " AND [Content] LIKE '%'+@keyword+'%' ";
                }
                else
                {
                    condition += " AND ([Subject] LIKE '%'+@keyword+'%' OR [Content] LIKE '%'+@keyword+'%') ";
                }

                query.CreateParameter<string>("@keyword", filter.KeyWord, SqlDbType.NVarChar, 256);
            }

            condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[PostUserID]", excludeRoleIDs, query));

            if (filter.ForumID != null)
                query.CreateParameter<int?>("@ForumID", filter.ForumID, SqlDbType.Int);
            if (string.IsNullOrEmpty(filter.CreateIP) == false)
                query.CreateParameter<string>("@CreateIP", filter.CreateIP, SqlDbType.NVarChar, 64);
            if (filter.TopicID != null)
                query.CreateParameter<int?>("@TopicID", filter.TopicID, SqlDbType.Int);
            if (filter.MaxReplyCount != null)
                query.CreateParameter<int?>("@MaxReplyCount", filter.MaxReplyCount, SqlDbType.Int);
            if (filter.MaxViewCount != null)
                query.CreateParameter<int?>("@MaxViewCount", filter.MaxViewCount, SqlDbType.Int);
            if (filter.MinReplyCount != null)
                query.CreateParameter<int?>("@MinReplyCount", filter.MinReplyCount, SqlDbType.Int);
            if (filter.MinViewCount != null)
                query.CreateParameter<int?>("@MinViewCount", filter.MinViewCount, SqlDbType.Int);
            if (filter.UserID != null)
                query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
            if (filter.BeginDate != null)
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            if (filter.EndDate != null)
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);

            return condition.ToString();
        }


        public override PostCollectionV5 GetPosts(int pageNumber, PostFilter filter, Guid[] excludeRoleIDs, ref int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = filter.IsDesc;

                query.Pager.SortField = "PostID";

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Posts]";

                query.Pager.Condition = BuilderSearchPostCondition(filter, excludeRoleIDs, query, false);

                PostCollectionV5 posts;
                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    posts = new PostCollectionV5(reader);

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    return posts;
                }
            }
        }

        public override DeleteResult DeleteSearchPosts(PostFilter filter, IEnumerable<Guid> excludeRoleIDs, bool getDeleteResult, int topCount, out int deletedCount, out List<int> threadIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {

                string conditon = BuilderSearchPostCondition(filter, excludeRoleIDs, query, false);

                query.CommandText = @"
DECLARE @Table table(TempPostID int,TempTopicID int,TempUserID int,TempForumID int,TempThreadStatus tinyint);
INSERT INTO @Table SELECT TOP (@TopCount) [PostID],[ThreadID],[UserID],[ForumID],[ThreadStatus] FROM [bx_Posts] WHERE " + conditon + @";
SELECT DISTINCT TempTopicID FROM @Table;
";

                if (getDeleteResult)
                {
                    query.CommandText += @"
SELECT [TempUserID],[TempForumID],COUNT(*) AS [Count] FROM @Table WHERE TempThreadStatus<4 GROUP BY [TempUserID],[TempForumID];";

                }


                query.CommandText = query.CommandText + @"
DELETE [bx_Posts] WHERE PostID IN(SELECT TempPostID FROM @Table);
SELECT @@ROWCOUNT;
";
                query.CreateTopParameter("@TopCount", topCount);

                threadIDs = new List<int>();
                deletedCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    DeleteResult deleteResult = new DeleteResult();

                    while (reader.Read())
                    {
                        int threadID = reader.Get<int>("TempTopicID");
                        if (threadIDs.Contains(threadID) == false)
                            threadIDs.Add(threadID);
                    }

                    if (getDeleteResult)
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                deleteResult.Add(reader.Get<int>("TempUserID"), reader.Get<int>("Count"), reader.Get<int>("TempForumID"));
                            }
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            deletedCount = reader.Get<int>(0);
                        }
                    }
                    return deleteResult;
                }
            }
        }


        private string BuilderSearchPostCondition(PostFilter filter, IEnumerable<Guid> excludeRoleIDs, SqlQuery query, bool startWithWhere)
        {
            SqlConditionBuilder condition;
            if (startWithWhere)
                condition = new SqlConditionBuilder(SqlConditionStart.Where);
            else
                condition = new SqlConditionBuilder(SqlConditionStart.None);


            condition += (filter.ForumID == null ? "" : ("AND [ForumID] = @ForumID "));
            condition += "AND PostType <> 1 ";
            condition += (filter.UserID == null ? "" : ("AND [UserID] = @UserID "));
            condition += (filter.PostID == null ? "" : ("AND [PostID] = @PostID "));
            condition += (string.IsNullOrEmpty(filter.CreateIP) ? "" : ("AND [IPAddress] = @CreateIP "));

            if (filter.IsUnapproved != null && filter.IsUnapproved.Value)
                condition += (" AND [SortOrder] >  " + SortOrder_Unapproved);
            else if (filter.IsUnapproved != null && filter.IsUnapproved.Value == false)
                condition += (" AND [ThreadStatus] < " + SortOrder_Unapproved);

            condition += (filter.BeginDate == null ? "" : ("AND [CreateDate] > @BeginDate "));
            condition += (filter.EndDate == null ? "" : ("AND [CreateDate] < @EndDate "));

            if (string.IsNullOrEmpty(filter.KeyWord) == false)
            {
                if (filter.SearchMode == SearchArticleMethod.Subject)
                {
                    condition += " AND [Subject] LIKE '%'+@keyword+'%' ";
                }
                else if (filter.SearchMode == SearchArticleMethod.FullText)
                {
                    condition += " AND [Content] LIKE '%'+@keyword+'%' ";
                }
                else
                {
                    condition += " AND ([Subject] LIKE '%'+@keyword+'%' OR [Content] LIKE '%'+@keyword+'%') ";
                }

                query.CreateParameter<string>("@keyword", filter.KeyWord, SqlDbType.NVarChar, 256);
            }

            condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query));

            if (filter.ForumID != null)
                query.CreateParameter<int?>("@ForumID", filter.ForumID, SqlDbType.Int);
            if (string.IsNullOrEmpty(filter.CreateIP) == false)
                query.CreateParameter<string>("@CreateIP", filter.CreateIP, SqlDbType.NVarChar, 64);
            if (filter.PostID != null)
                query.CreateParameter<int?>("@PostID", filter.PostID, SqlDbType.Int);
            if (filter.UserID != null)
                query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);
            if (filter.BeginDate != null)
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            if (filter.EndDate != null)
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);

            return condition.ToString();
        }


        public override ThreadCollectionV5 GetThreads(int? forumID, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int offSet, bool includeStick, int pageNumber, int pageSize, ref int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = isDesc;
                query.Pager.ResultFields = ThreadFields;

                ProcessSortField(query, sortType);

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.Offset = offSet;
                query.Pager.TableName = "[bx_Threads]";

                StringBuilder condition = new StringBuilder();

                if (forumID != null)
                {
                    condition.Append(" [ForumID]=@ForumID ");
                    query.CreateParameter<int>("@ForumID", forumID.Value, SqlDbType.Int);
                }

                if (condition.Length != 0)
                    condition.Append(" AND ");
                //置顶在缓存中取
                //if (includeStick)
                    //condition.Append(" [ThreadStatus] < 4 ");
                //else
                    condition.Append(" [ThreadStatus] = 1 ");

                if (forumID != null)
                {
                    ProcessThreadDateScope(beginDate, endDate, query, condition);
                }
                else
                {
                    if (beginDate != null)
                    {
                        condition.Append(" AND [CreateDate] >= @BeginDate ");
                        query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                    }

                    if (endDate != null)
                    {
                        condition.Append(" AND [CreateDate] <= @EndDate ");
                        query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                    }
                }
                query.Pager.Condition = condition.ToString();

                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader,null));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        private BasicThread GetThread(XSqlDataReader reader, DateTime? expiresDate)
        {
            ThreadType threadType = reader.Get<ThreadType>("ThreadType");
            switch (threadType)
            {
                case ThreadType.Poll:
                    PollThreadV5 poll = new PollThreadV5(reader);
                    if (expiresDate != null)
                        poll.IsClosed = expiresDate.Value <= DateTimeUtil.Now;
                    return (BasicThread)poll;
                case ThreadType.Polemize:
                    PolemizeThreadV5 polemize = new PolemizeThreadV5(reader);
                    if (expiresDate != null)
                        polemize.IsClosed = expiresDate.Value <= DateTimeUtil.Now;
                    return (BasicThread)polemize;
                case ThreadType.Question:
                    QuestionThread question = new QuestionThread(reader);
                    if (question.IsClosed == false)
                    {
                        if (expiresDate != null)
                            question.IsClosed = expiresDate.Value <= DateTimeUtil.Now;
                    }
                    return (BasicThread)question;
                default:
                    return new BasicThread(reader);
            }
        }
        #region 分割主题存储过程
        /// <summary>
        /// 分割主题
        /// </summary>
        /// <param name="threadID">原主题ID</param>
        /// <param name="postIdentities">要做为分割出来的主题的postID</param>
        /// <param name="newSubject">分割出来的主题标题</param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SplitThread", Script = @"
CREATE PROCEDURE {name} 
	@ThreadID int,
	@PostIdentities varchar(8000),
	@NewSubject nvarchar(256)
AS
	SET NOCOUNT ON 
	DECLARE @SortOrder bigint,@NewThreadID int,@TotalReplies int,@PostUserID int,@PostNickName nvarchar(64),@LastPostUserID int,@LastPostNickName nvarchar(64)
	
	Declare @ForumID int,@ThreadCatalogID int
	SELECT @ForumID = ForumID,@ThreadCatalogID=ThreadCatalogID FROM bx_Threads WITH(NOLOCK) WHERE ThreadID=@ThreadID
	
	DECLARE @E1 int,@E2 int,@E3 int,@E4 int
	BEGIN TRANSACTION
	SELECT @SortOrder = ISNULL(MAX(SortOrder)+1,0) FROM bx_Threads WITH (NOLOCK) WHERE ThreadStatus = 1;
	INSERT bx_Threads(ForumID,ThreadCatalogID,ThreadType,IconID,Subject,PostUserID,PostNickName,SortOrder,LastPostID,ThreadStatus) select ForumID,ThreadCatalogID,ThreadType,IconID,@NewSubject,PostUserID,PostNickName,@SortOrder,0,ThreadStatus from bx_Threads with(nolock) where ThreadID=@ThreadID
	SET @NewThreadID = @@IDENTITY
    
    
    
	
	EXEC ('UPDATE [bx_Posts] SET ThreadID=' + @NewThreadID + ' WHERE [PostID] IN (' + @PostIdentities + ') AND [ThreadID]=' + @ThreadID) 
	SELECT @E1 = @@error

    DECLARE @LastPostIDTable table(Tid int,TLastPostID int);
    DECLARE @OldLastPostID int,@NewLastPostID int;
    SELECT @OldLastPostID = MAX(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000;
    SELECT @NewLastPostID = MAX(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @NewThreadID AND SortOrder<4000000000000000;

    INSERT INTO @LastPostIDTable(Tid,TLastPostID) VALUES(@ThreadID,@OldLastPostID);
    INSERT INTO @LastPostIDTable(Tid,TLastPostID) VALUES(@NewThreadID,@NewLastPostID);

    UPDATE bx_Threads SET LastPostID = TLastPostID FROM @LastPostIDTable WHERE ThreadID = Tid;
    SELECT @E2 = @@error
		--SET @TotalReplies=@@ROWCOUNT-1--第一个Post是主题内容，不属于回复，所以减1
	
	--更新新主题--
	SELECT TOP 1 @PostUserID=UserID,@PostNickName=NickName FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@NewThreadID AND SortOrder<4000000000000000 ORDER BY PostID
	SELECT TOP 1 @LastPostUserID=UserID,@LastPostNickName=NickName FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@NewThreadID AND SortOrder<4000000000000000 ORDER BY PostID DESC
	
	UPDATE [bx_Posts] SET PostType=1 WHERE PostID=(SELECT MIN(PostID) FROM [bx_Posts] WITH (NOLOCK) WHERE ForumID=@ForumID AND ThreadID=@NewThreadID)
	SELECT @TotalReplies = COUNT(*)-1 FROM [bx_Posts] WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadID=@NewThreadID AND SortOrder<4000000000000000--第一个Post是主题内容，不属于回复，所以减1
	UPDATE bx_Threads SET TotalReplies = @TotalReplies, TotalViews = @TotalReplies, PostUserID = @PostUserID, PostNickName = @PostNickName, LastPostUserID = @LastPostUserID, LastPostNickName = @LastPostNickName where ThreadID = @NewThreadID
	SELECT @E3 = @@error
	
	--更新原主题---
	SELECT TOP 1 @LastPostUserID = UserID, @LastPostNickName = NickName FROM [bx_Posts] WITH(NOLOCK)  WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000 ORDER BY PostID DESC
	DECLARE @OldTotalReplies int
	SELECT @OldTotalReplies = COUNT(*) - 1 FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000 --第一个Post是主题内容，不属于回复，所以减1
	UPDATE bx_Threads SET TotalReplies = @OldTotalReplies, TotalViews = TotalViews - (@TotalReplies + 1), LastPostUserID = @LastPostUserID, LastPostNickName = @LastPostNickName WHERE ThreadID = @ThreadID
	SELECT @E4 = @@error
	
	IF(@E1 = 0 AND @E2 = 0 AND @E3 = 0 AND @E4 = 0)
	BEGIN
        EXECUTE bx_SetPostFloor @NewThreadID;
        EXECUTE bx_SetPostFloor @ThreadID;
		COMMIT TRANSACTION
		UPDATE [bx_Forums] SET [TotalThreads] = TotalThreads+1, [TodayThreads]=TodayThreads+1,[LastThreadID]=@NewThreadID WHERE [ForumID] = @ForumID;
		EXECUTE bx_UpdateForumThreadCatalogsData @ForumID,@ThreadCatalogID
		--EXECUTE bx_UpdateForumData @ForumID
		RETURN (0)
	END
	ELSE
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END
")]

        #endregion
        public override bool SplitThread(int threadID, IEnumerable<int> postIdentities, string newSubject)
        {
            string identitiesString = StringUtil.Join(postIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SplitThread";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<string>("@PostIdentities", identitiesString, SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@NewSubject", newSubject, SqlDbType.NVarChar, 256);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)query.Parameters["@ErrorCode"].Value == 0;
            }
        }

        [StoredProcedure(Name = "bx_DeletePosts", Script = @"
CREATE PROCEDURE {name} 
	@ForumID int,
	@ThreadID int,
	@UserID int,
	@IsDeleteAnyPost bit,
	@PostIdentities varchar(8000)
AS
BEGIN

	SET NOCOUNT ON 

	IF(@IsDeleteAnyPost=1)
	BEGIN
		EXEC ('DELETE [bx_Posts] WHERE [ForumID]=' + @ForumID + ' AND [ThreadID]=' + @ThreadID + ' AND [PostID] IN (' + @PostIdentities + ')')
	END
	ELSE
		BEGIN
			DECLARE @Count int,@SQLString nvarchar(4000)
			SET @SQLString = N'SELECT @Count=count(*) FROM [bx_Posts] WITH(NOLOCK) WHERE UserID <> '+str(@UserID)+' AND [ForumID]=' + str(@ForumID) + ' AND [ThreadID]=' + str(@ThreadID) + ' AND [PostID] IN (' + @PostIdentities + ')'
			EXECUTE sp_executesql @SQLString,N'@Count int output',@Count output
			IF(@Count>0)
				RETURN 101
			ELSE
				EXEC ('DELETE [bx_Posts] WHERE [UserID]=' + @UserID + ' AND [ForumID]=' + @ForumID + ' AND [ThreadID]=' + @ThreadID + ' AND [PostID] IN (' + @PostIdentities + ')') 
		END
	DECLARE @RowCount int
	SET @RowCount = @@ROWCOUNT
	IF @RowCount > 0 BEGIN
		----------增加删除回复数---------------
		EXEC [bx_DoCreateStat] @ForumID,8, @RowCount
		--------------------------
		RETURN (0)
	END
	ELSE
		RETURN (1)

END

")]
        public override bool DeletePosts(int forumID, int threadID, IEnumerable<int> postIdentities, int userID, bool isDeleteAnyPost)
        {
            string IdentitiesString = StringUtil.Join(postIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeletePosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsDeleteAnyPost", isDeleteAnyPost, SqlDbType.Bit);
                query.CreateParameter<string>("@PostIdentities", IdentitiesString, SqlDbType.VarChar, 8000);
                query.CreateParameter<bool>("@ErrorCode", SqlDbType.Bit, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return Convert.ToInt32(query.Parameters["@ErrorCode"].Value) == 0;
            }
        }

        #region 合并主题存储过程
        [StoredProcedure(Name = "bx_v5_JoinThreads", Script = @"
CREATE PROCEDURE {name}
	@OldThreadID int,
	@NewThreadID int,
	@IsKeepLink bit
AS
	SET NOCOUNT ON 
	IF EXISTS (SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID AND ThreadType<10 AND ThreadStatus<4)
	BEGIN
		DECLARE @NewForumID int,@OldForumID int,@OldThreadCatalogID int
		DECLARE @TotalReplies int,@TotalViews int,@LastPostUserID int,@LastPostNickName nvarchar(64)
		SELECT @OldForumID=ForumID,@OldThreadCatalogID=ThreadCatalogID,@TotalReplies=TotalReplies+1,@TotalViews=TotalViews FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@OldThreadID
		SELECT @NewForumID=ForumID FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID
		
		DECLARE @E1 int,@E2 int,@E3 int,@E4 int
		BEGIN TRANSACTION

        DECLARE @OldThreadContentSortOrder bigint,@NewThreadContentSortOrder bigint;
        DECLARE @OldThreadContentID int,@NewThreadContentID int;
        SELECT @OldThreadContentSortOrder = SortOrder,@OldThreadContentID = PostID FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID=@OldThreadID AND PostType = 1;
        SELECT @NewThreadContentSortOrder = SortOrder,@NewThreadContentID = PostID FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID=@NewThreadID AND PostType = 1;

        IF @OldThreadContentSortOrder < @NewThreadContentSortOrder BEGIN
            DECLARE @Temp table(pid int,psortorder bigint);
            INSERT INTO @Temp(pid,psortorder) VALUES(@OldThreadContentID,@NewThreadContentSortOrder);
            INSERT INTO @Temp(pid,psortorder) VALUES(@NewThreadContentID,@OldThreadContentSortOrder);
            UPDATE [bx_Posts] SET SortOrder = psortorder FROM @Temp WHERE PostID = pid;
        END
            

		UPDATE [bx_Posts] SET PostType = 0 WHERE ThreadID=@OldThreadID AND PostType = 1;
		SELECT @E4=@@error
		
		UPDATE [bx_Posts] SET ForumID=@NewForumID, ThreadID=@NewThreadID WHERE ThreadID=@OldThreadID;
		SELECT @E1=@@error
		

		SELECT top 1 @LastPostUserID=UserID,@LastPostNickName=NickName FROM [bx_Posts] WITH (NOLOCK) WHERE ForumID=@NewForumID AND ThreadID=@NewThreadID AND SortOrder<4000000000000000 order by PostID DESC
		
		UPDATE [bx_Threads] SET TotalReplies=TotalReplies+@TotalReplies,TotalViews=TotalViews+@TotalViews,LastPostUserID=@LastPostUserID,LastPostNickName=@LastPostNickName WHERE ThreadID=@NewThreadID
		SELECT @E2=@@error
		
		IF (@IsKeepLink=1)
			BEGIN
			UPDATE [bx_Threads] SET Subject=(CAST(@NewThreadID as nvarchar(16))+N','+Subject),ThreadType=11 WHERE ThreadID=@OldThreadID
			SELECT @E3=@@error
			END
		ELSE
			BEGIN
			DELETE [bx_Threads] WHERE ThreadID=@OldThreadID
			SELECT @E3=@@error
			END
			
		IF(@E1=0 AND @E2=0 AND @E3=0 AND @E4=0)
			BEGIN
            
            EXECUTE bx_SetPostFloor @NewThreadID;
            
			COMMIT TRANSACTION
			IF(@OldForumID<>@NewForumID) BEGIN
				IF (@IsKeepLink=1) BEGIN
					UPDATE [bx_Forums] SET [TotalPosts] = TotalPosts-@TotalReplies WHERE [ForumID] = @OldForumID;
				END
				UPDATE [bx_Forums] SET [TotalPosts] = TotalPosts+@TotalReplies WHERE [ForumID] = @NewForumID;
			END
			
			RETURN (0)
			END
		ELSE
			BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
			END
	END
	ELSE
		RETURN (-1)
")]
        #endregion
        public override bool JoinThreads(int oldThreadID, int newThreadID, bool isKeepLink)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_JoinThreads";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@OldThreadID", oldThreadID, SqlDbType.Int);
                query.CreateParameter<int>("@NewThreadID", newThreadID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsKeepLink", isKeepLink, SqlDbType.Bit);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return (int)query.Parameters["@ErrorCode"].Value == 0;
            }
        }



        public override ThreadCollectionV5 GetMyThreads(int userID, bool isApproved, int pageNumber, int pageSize, int offset, out int totalThreads)
        {
            totalThreads = 0;

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = ThreadFields;
                query.Pager.SortField = "[ThreadID]";

                query.Pager.PrimaryKey = "[ThreadID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Threads]";
                query.Pager.Offset = offset;

                StringBuilder condition = new StringBuilder();

                condition.Append(" [PostUserID]=@UserID ");
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                if (isApproved)
                {
                    //置顶的 在缓存里取
                    //condition.Append(" AND [ThreadStatus] < 4 ");
                    condition.Append(" AND [ThreadStatus] = 1 ");
                }
                else
                    condition.Append(" AND [ThreadStatus] = 5 ");

                query.Pager.Condition = condition.ToString();


                ThreadCollectionV5 threads;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    threads = new ThreadCollectionV5(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalThreads = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        public override ThreadCollectionV5 GetMyParticipantThreads(int userID, int pageNumber, int pageSize, out int totalThreads)
        {
            totalThreads = 0;

            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = ThreadFields;
                query.Pager.SortField = "[ThreadID]";

                query.Pager.PrimaryKey = "[ThreadID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Threads]";

                query.Pager.Condition = " ThreadID in (SELECT DISTINCT ThreadID FROM bx_Posts WITH (NOLOCK) WHERE UserID=@UserID AND [SortOrder]<4000000000000000) ";

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                ThreadCollectionV5 threads;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    threads = new ThreadCollectionV5(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalThreads = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        #region 存储过程 bx_GetThread
        [StoredProcedure(Name = "bx_GetThreadCount", Script = @"
CREATE PROCEDURE {name}
    @ForumID        int
   ,@IncludeStick   bit
   ,@BeginDate      DateTime
   ,@EndDate        DateTime
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ThreadStatus tinyint;
    IF @IncludeStick = 1
        SET @ThreadStatus = 4;
    ELSE
        SET @ThreadStatus = 2;

    IF @BeginDate IS NULL AND @EndDate IS NULL
        SELECT COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < @ThreadStatus;
    ELSE IF @BeginDate IS NULL
        SELECT COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < @ThreadStatus AND [CreateDate] <= @EndDate;
    ELSE
        SELECT COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadStatus] < @ThreadStatus AND [CreateDate] >= @BeginDate; 
     
END
"
            )]
        #endregion
        public override int GetThreadCount(int forumID, DateTime? beginDate, DateTime? endDate, bool includeStick)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetThreadCount";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<bool>("@IncludeStick", includeStick, SqlDbType.Bit);
                query.CreateParameter<DateTime?>("@BeginDate", beginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime?>("@EndDate", endDate, SqlDbType.DateTime);

                return query.ExecuteScalar<int>();
            }
        }

        #region 存储过程 bx_GetStickThreads
        [StoredProcedure(Name = "bx_v5_GetStickThreads", Script = @"
CREATE PROCEDURE {name}
    @ForumID int
AS
BEGIN
    SET NOCOUNT ON;

	SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadStatus] = 2 AND (ForumID = @ForumID OR ThreadID IN(SELECT ThreadID FROM bx_StickThreads WHERE ForumID = @ForumID)) ORDER BY SortOrder DESC;
     
END
"
            )]
        #endregion
        public override ThreadCollectionV5 GetStickThreads(int forumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetStickThreads";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ThreadCollectionV5 threads = new ThreadCollectionV5();
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    return threads;
                }
            }
        }

  
        public override ThreadCollectionV5 GetStickThreads(IEnumerable<int> forumIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadStatus] = 2 AND (ForumID in(@ForumIDs) OR ThreadID IN(SELECT ThreadID FROM bx_StickThreads WHERE ForumID in(@ForumIDs))) ORDER BY SortOrder DESC;
                ";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@ForumIDs", forumIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ThreadCollectionV5 threads = new ThreadCollectionV5();
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    return threads;
                }
            }
        }


        #region 存储过程 bx_GetGlobalThreads
        [StoredProcedure(Name = "bx_GetGlobalThreads", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
    SET NOCOUNT ON;

	SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadStatus] = 3 ORDER BY [SortOrder] DESC;
     
END
"
            )]
        #endregion
        public override ThreadCollectionV5 GetGlobalThreads()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetGlobalThreads";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ThreadCollectionV5 threads = new ThreadCollectionV5();
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    return threads;
                }
            }
        }

        private int GetDayInt(DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }

        private void ProcessThreadDateScope(DateTime? beginDate, DateTime? endDate, SqlQuery query, StringBuilder condition)
        {
            DeclareVariableCollection beforeSqlDeclare;
            string beforeSql;
            string c;

            ProcessThreadDateScope(beginDate, endDate, query, out beforeSqlDeclare, out beforeSql, out c);

            condition.Append(c);

            if (beforeSql != string.Empty)
            {
                if (query.Pager.BeforeExecute == null)
                {
                    query.Pager.BeforeExecuteDealcre = beforeSqlDeclare;
                    query.Pager.BeforeExecute = beforeSql;
                }
                else
                {
                    if (query.Pager.BeforeExecuteDealcre.Count == 0)
                        query.Pager.BeforeExecuteDealcre = beforeSqlDeclare;
                    else
                        query.Pager.BeforeExecuteDealcre.AddRange(beforeSqlDeclare);

                    query.Pager.BeforeExecute += beforeSql;
                }
            }
        }

        private void ProcessThreadDateScope(DateTime? beginDate, DateTime? endDate, SqlQuery query, out DeclareVariableCollection beforeSqlDeclare ,out string beforeSql, out string condition)
        {
            beforeSqlDeclare = new DeclareVariableCollection();
            beforeSql = string.Empty;
            condition = string.Empty;

            if (beginDate != null)
            {
                beforeSqlDeclare.Add("@StartLastThreadID", SqlDbType.Int);

                beforeSql += @"
                SELECT @StartLastThreadID = LastThreadID FROM bx_DayLastThreads WITH(NOLOCK) WHERE Day = @StartDay;
                IF @StartLastThreadID IS NULL BEGIN
                    SELECT @StartLastThreadID = ISNULL(MAX(ThreadID),0) FROM bx_Threads WITH(NOLOCK) WHERE CreateDate < @BDate;
                    IF NOT EXISTS(SELECT * FROM bx_DayLastThreads WITH(NOLOCK) WHERE Day = @StartDay) BEGIN
                        INSERT INTO bx_DayLastThreads([Day],[LastThreadID]) VALUES(@StartDay,@StartLastThreadID);
                    END
                END
";
                condition = " AND ThreadID> @StartLastThreadID ";

                //如果是获取今天的  应该传昨天的时间进去 取昨天的最后一个主题后面的主题
                query.CreateParameter<int>("@StartDay", GetDayInt(beginDate.Value/*.AddDays(1 - 1)*/), SqlDbType.Int);

                //如果是获取今天的  应该传今天的时间进去 
                query.CreateParameter<DateTime>("@BDate", new DateTime(beginDate.Value.Year, beginDate.Value.Month, beginDate.Value.Day).AddDays(1), SqlDbType.DateTime);
            }

            if (endDate != null)
            {
                beforeSqlDeclare.Add("@EndLastThreadID", SqlDbType.Int);

                beforeSql += @"
                SELECT @EndLastThreadID = LastThreadID FROM bx_DayLastThreads WITH(NOLOCK) WHERE Day = @EndDay;
                IF @EndLastThreadID IS NULL BEGIN
                    SELECT @EndLastThreadID = ISNULL(MAX(ThreadID),0) FROM bx_Threads WITH(NOLOCK) WHERE CreateDate < @EDate;
                    IF NOT EXISTS(SELECT * FROM bx_DayLastThreads WITH(NOLOCK) WHERE Day = @EndDay) BEGIN
                        INSERT INTO bx_DayLastThreads([Day],[LastThreadID]) VALUES(@EndDay,@EndLastThreadID);
                    END
                END
";
                condition += " AND ThreadID<=@EndLastThreadID ";
                query.CreateParameter<int>("@EndDay", GetDayInt(endDate.Value), SqlDbType.Int);

                //如果是获取今天的  应该传明天的时间进去 
                query.CreateParameter<DateTime>("@EDate", new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day).AddDays(1), SqlDbType.DateTime);

            }
        }

        public override ThreadCollectionV5 GetThreadsByThreadCatalogID(int forumID, int threadCatalogID, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, int offset, ref int totalThreads)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = isDesc;
                query.Pager.ResultFields = ThreadFields;
                ProcessSortField(query, sortType);

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_Threads]";
                query.Pager.Offset = offset;

                StringBuilder condition = new StringBuilder();

                condition.Append(" [ThreadCatalogID]=@ThreadCatalogID ");
                query.CreateParameter<int>("@ThreadCatalogID", threadCatalogID, SqlDbType.Int);

                condition.Append(" AND [ForumID]=@ForumID ");
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);

                //置顶 的在缓存里取
                //condition.Append(" AND [ThreadStatus] < 4 ");
                condition.Append(" AND [ThreadStatus] = 1 ");

                ProcessThreadDateScope(beginDate, endDate, query, condition);

                //if (beginDate != null)
                //{
                //    //condition.Append(" AND [CreateDate] >= @BeginDate ");
                //    //query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                //}

                //if (endDate != null)
                //{
                //    //condition.Append(" AND [CreateDate] <= @EndDate ");
                //    //query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                //}

                query.Pager.Condition = condition.ToString();

                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalThreads = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        private void ProcessSortField(SqlQuery query, ThreadSortField? sortType)
        {
            query.Pager.SortField = GetSortField(sortType);

            if (sortType!=null && (sortType.Value == ThreadSortField.Replies
                || sortType.Value == ThreadSortField.Views))
            {
                query.Pager.PrimaryKey = "[ThreadID]";
            }
        }

        private string GetSortField(ThreadSortField? sortType)
        {
            if (sortType == null)
            {
                return "[SortOrder]";
            }
            else if (sortType.Value == ThreadSortField.CreateDate)
            {
                return "[ThreadID]";
            }
            else if (sortType.Value == ThreadSortField.Replies)
            {
                return "[TotalReplies]";
            }
            else if (sortType.Value == ThreadSortField.Views)
            {
                return "[TotalViews]";
            }
            else if (sortType.Value == ThreadSortField.LastReplyDate)
            {
                return "[LastPostID]";
            }
            else
                return "[SortOrder]";
        }

        public override ThreadCollectionV5 GetValuedThreads(IEnumerable<int> forumIDs, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, bool includeStick, int offset, out int totalThreads)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = isDesc;
                query.Pager.ResultFields = ThreadFields;
                ProcessSortField(query, sortType);

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = returnTotalThreads;
                query.Pager.TableName = "[bx_Threads]";
                query.Pager.Offset = offset;

                StringBuilder condition = new StringBuilder();

                condition.Append(" [IsValued]=1 ");

                bool isOneForumID = false;
                if (forumIDs != null)
                {
                    int forumID = 0;

                    int count = 0;
                    foreach (int tempForumID in forumIDs)
                    {
                        forumID = tempForumID;
                        count++;
                        if (count == 2)
                            break;
                    }

                    if (count == 1)
                    {
                        isOneForumID = true;
                        condition.Append(" AND [ForumID]=@ForumID ");
                        query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                    }
                    else if (count > 1)
                    {
                        condition.Append(" AND [ForumID] IN(@ForumIDs) ");
                        query.CreateInParameter<int>("@ForumIDs", forumIDs);
                    }

                }

                if (includeStick)
                {
                    condition.Append(" AND [ThreadStatus] < 4 ");
                }
                else
                {
                    //置顶的在缓存里取
                    //condition.Append(" AND [ThreadStatus] < 4 ");
                    condition.Append(" AND [ThreadStatus] = 1 ");
                }

                if (isOneForumID)
                {
                    ProcessThreadDateScope(beginDate, endDate, query, condition);
                }
                else
                {
                    if (beginDate != null)
                    {
                        condition.Append(" AND [CreateDate] >= @BeginDate ");
                        query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                    }

                    if (endDate != null)
                    {
                        condition.Append(" AND [CreateDate] <= @EndDate ");
                        query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                    }
                }

                query.Pager.Condition = condition.ToString();

                totalThreads = 0;
                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalThreads = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        public override ThreadCollectionV5 GetThreads(IEnumerable<int> threadids)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (!ValidateUtil.HasItems<int>(threadids))
                    return new ThreadCollectionV5();

                string sql = string.Format("SELECT {0} FROM bx_Threads WITH (NOLOCK) WHERE ThreadID IN( @ThreadIDs );", ThreadFields);
                query.CreateInParameter<int>("@ThreadIDs", threadids);
                query.CommandText = sql;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ThreadCollectionV5 threads = new ThreadCollectionV5();
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    return threads;
                }
            }
        }


        [StoredProcedure(Name = "bx_GetUnapprovedPosts", FileName = "v30\\bx_GetUnapprovedPosts.sql")]
        public override Dictionary<BasicThread, PostCollectionV5> GetUnapprovedPosts(int forumID)
        {
            Dictionary<BasicThread, PostCollectionV5> unapprovedPosts = new Dictionary<BasicThread, PostCollectionV5>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUnapprovedPosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicThread thread = GetThread(reader, null);
                        unapprovedPosts.Add(thread, new PostCollectionV5());
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            PostV5 post = new PostV5(reader);
                            foreach (BasicThread thread in unapprovedPosts.Keys)
                            {
                                if (thread.ThreadID == post.ThreadID)
                                {
                                    unapprovedPosts[thread].Add(post);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return unapprovedPosts;
        }


        #region 存储过程 bx_GetUnapprovedPostThreads
        [StoredProcedure(Name = "bx_GetUnapprovedPostThreads", Script = @"
CREATE PROCEDURE {name}
	@ForumID int,--大于0时按版块获取
	@UserID int,--大于0时按用户获取
	@PageIndex int,
	@PageSize int
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @Condition varchar(8000),@User nvarchar(100)
	
	IF @UserID>0
		SET @User=' P.UserID='+str(@UserID)+' AND '
	ELSE
		SET @User=''

	IF @ForumID=0
		SET @Condition='ThreadID in (SELECT DISTINCT P.ThreadID FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ThreadStatus<4)'
	ELSE
		SET @Condition='ThreadID in (SELECT DISTINCT P.ThreadID FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' T.ForumID='+str(@ForumID)+' AND P.SortOrder >= 5000000000000000 AND T.ThreadStatus<4)'

	DECLARE @TotalCount INT,@SQLString NVARCHAR(4000)
	
	IF @ForumID=0
		SET @SQLString='SELECT @TotalCount=COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ThreadStatus<4'
	ELSE
		SET @SQLString='SELECT @TotalCount=COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T WITH (NOLOCK) ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ForumID='+str(@ForumID)+' AND T.ThreadStatus<4'
	EXECUTE sp_executesql @SQLString,N'@TotalCount int output',@TotalCount output


	DECLARE @ResetOrder bit----------- 1表示读取数据的时候 排序要反过来
	EXECUTE bx_Common_GetRecordsByPageSQLString
						@PageIndex,
						@PageSize,
						N'bx_Threads',
						N'ThreadID',
						@Condition,
						N'[SortOrder]',
						1,
						@TotalCount,
						@ResetOrder OUTPUT,
						@SQLString OUTPUT
	
	EXEC ('DECLARE @ThreadIDTable table(ThreadID int NOT NULL);

		INSERT INTO @ThreadIDTable ' + @SQLString + ';

		SELECT T.*
--,IsClosed=CASE
--WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = T.ThreadID )
--WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = T.ThreadID )
--WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = T.ThreadID )
--ELSE 0
--END,
		,(SELECT COUNT(1) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID=T.ThreadID  AND SortOrder >= 5000000000000000) as UnApprovedPostsCount FROM bx_Threads T WITH (NOLOCK)  WHERE T.ThreadID in (SELECT ThreadID FROM @ThreadIDTable) ORDER BY SortOrder DESC')
		
	
	
	SELECT @TotalCount; 
     
END
"
            )]
        #endregion
        public override ThreadCollectionV5 GetUnapprovedPostThreads(int? forumID, int? userID, int pageNumber, int pageSize)
        {
            ThreadCollectionV5 threads = new ThreadCollectionV5();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUnapprovedPostThreads";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID == null ? 0 : forumID.Value, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID == null ? 0 : userID.Value, SqlDbType.Int);
                query.CreateParameter<int>("@PageIndex", pageNumber - 1, SqlDbType.Int);
                query.CreateParameter<int>("@PageSize", pageSize, SqlDbType.Int);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicThread thread = GetThread(reader, null);
                        thread.UnApprovedPostsCount = (int)reader["UnApprovedPostsCount"];
                        threads.Add(thread);
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                            threads.TotalRecords = reader.GetInt32(0);
                    }
                }
            }
            return threads;
        }

        #region 存储过程 bx_v5_GetUnapprovedPostThread
        [StoredProcedure(Name = "bx_v5_GetUnapprovedPostThread", Script = @"
CREATE PROCEDURE {name}
	@ThreadID       int,
	@UserID         int,
	@PageIndex      int,
	@PageSize       int,
    @TopMarkCount   int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Condition varchar(8000),@User nvarchar(100)
	IF @UserID IS NOT NULL
		SET @User='UserID='+str(@UserID)+' AND '
	ELSE
		SET @User=''

	SET @Condition=' ('+@User+' ThreadID='+str(@ThreadID)+' AND SortOrder >= 5000000000000000)'

    SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@ThreadID;

    DECLARE @TotalCount int;
	IF @UserID>0
		SELECT @TotalCount = COUNT(*) FROM bx_Posts WITH (NOLOCK) WHERE UserID = @UserID AND ThreadID= @ThreadID AND SortOrder >= 5000000000000000;
	ELSE
		SELECT @TotalCount = COUNT(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID= @ThreadID AND SortOrder >= 5000000000000000;

    SELECT @TotalCount;

    DECLARE @SQLString nvarchar(4000);
    DECLARE @PostFieldsString nvarchar(4000);
    DECLARE @ResetOrder bit;

    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    '" + PostColumns+@"',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        " + PostFields + @"
	
END
"
            )]
        #endregion
        public override void GetUnapprovedPostThread(int threadID, int? userID, int pageNumber, int pageSize, out BasicThread thread, out PostCollectionV5 posts, out int totalCount)
        {
            totalCount = 0;
            thread = null;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetUnapprovedPostThread";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int?>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@PageIndex", pageNumber - 1, SqlDbType.Int);
                query.CreateParameter<int>("@PageSize", pageSize, SqlDbType.Int);
                SetTopMarkCountParam(query);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        thread = GetThread(reader, null);
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }

                    posts = GetPosts(reader, false);
                }
            }
        }

        public override ThreadCollectionV5 GetThreads(int forumID, ThreadType threadType, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, int offset, out int totalThreads)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = isDesc;
                query.Pager.ResultFields = ThreadFields;
                ProcessSortField(query, sortType);

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = returnTotalThreads;
                query.Pager.TableName = "[bx_Threads]";
                query.Pager.Offset = offset;

                StringBuilder condition = new StringBuilder();

                condition.Append(" [ThreadType]=@ThreadType ");
                query.CreateParameter<int>("@ThreadType", (int)threadType, SqlDbType.TinyInt);

                condition.Append(" AND [ForumID]=@ForumID ");
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);

                //置顶在缓存中取
                //condition.Append(" AND [ThreadStatus] < 4 ");
                condition.Append(" AND [ThreadStatus] = 1 ");


                ProcessThreadDateScope(beginDate, endDate, query, condition);

                //if (beginDate != null)
                //{
                //    condition.Append(" AND [CreateDate] >= @BeginDate ");
                //    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                //}

                //if (endDate != null)
                //{
                //    condition.Append(" AND [CreateDate] <= @EndDate ");
                //    query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                //}

                query.Pager.Condition = condition.ToString();

                totalThreads = 0;
                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader, null));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalThreads = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        public override ThreadCollectionV5 GetHotThreads(IEnumerable<int> forumIDs, int reqireReplies, int pageNumber, int pageSize, ThreadSortField? sortType, DateTime? beginDate, DateTime? endDate, bool isDesc, bool returnTotalThreads, out int totalThreads)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = isDesc;
                query.Pager.ResultFields = ThreadFields;

                ProcessSortField(query, sortType);

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = returnTotalThreads;
                query.Pager.TableName = "[bx_Threads]";

                StringBuilder condition = new StringBuilder();

                if (forumIDs != null)
                {
                    int forumID = 0;

                    int count = 0;
                    foreach (int tempForumID in forumIDs)
                    {
                        forumID = tempForumID;
                        count++;
                        if (count == 2)
                            break;
                    }

                    if (count == 1)
                    {
                        condition.Append(" [ForumID]=@ForumID ");
                        query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                    }
                    else if (count > 1)
                    {
                        condition.Append(" [ForumID] IN(@ForumIDs) ");
                        query.CreateInParameter<int>("@ForumIDs", forumIDs);
                    }

                }

                if (condition.Length > 0)
                    condition.Append(" AND ");

                condition.Append(" [ThreadStatus] = 1 ");

                if (reqireReplies > 0)
                {
                    condition.Append(" AND [TotalReplies] >= @Replies ");
                    query.CreateParameter<int>("@Replies", reqireReplies, SqlDbType.Int);
                }
                else if (reqireReplies == 0)
                {
                    condition.Append(" AND [TotalReplies] > 0 ");
                }

                ProcessThreadDateScope(beginDate, endDate, query, condition);

                //if (beginDate != null)
                //{
                //    condition.Append(" AND [CreateDate] >= @BeginDate ");
                //    query.CreateParameter<DateTime>("@BeginDate", beginDate.Value, SqlDbType.DateTime);
                //}

                //if (endDate != null)
                //{
                //    condition.Append(" AND [CreateDate] <= @EndDate ");
                //    query.CreateParameter<DateTime>("@EndDate", endDate.Value, SqlDbType.DateTime);
                //}

                query.Pager.Condition = condition.ToString();

                totalThreads = 0;
                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader,null));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalThreads = reader.Get<int>(0);
                        }
                    }
                }

                return threads;
            }
        }

        public override ThreadCollectionV5 GetNewThreads(IEnumerable<int> forumIDs, int totalThreads, int pageNumber, int pageSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = ThreadFields;
                query.Pager.SortField = "[ThreadID]";

                query.Pager.PrimaryKey = "[ThreadID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = false;
                query.Pager.TableName = "[bx_Threads]";

                StringBuilder condition = new StringBuilder();

                if (forumIDs != null)
                {
                    int forumID = 0;

                    int count = 0;
                    foreach (int tempForumID in forumIDs)
                    {
                        forumID = tempForumID;
                        count++;
                        if (count == 2)
                            break;
                    }

                    if (count == 1)
                    {
                        condition.Append(" [ForumID]=@ForumID ");
                        query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                    }
                    else if (count > 1)
                    {
                        condition.Append(" [ForumID] IN(@ForumIDs) ");
                        query.CreateInParameter<int>("@ForumIDs", forumIDs);
                    }

                }

                if (condition.Length > 0)
                    condition.Append(" AND ");

                condition.Append(" [ThreadStatus] < 4 ");

                query.Pager.Condition = condition.ToString();

                ThreadCollectionV5 threads = new ThreadCollectionV5();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threads.Add(GetThread(reader,null));
                    }
                }

                return threads;
            }
        }


        public override ThreadCollectionV5 GetNewThreads(IEnumerable<int> forumIDs, int count)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (forumIDs != null)
                {
                    query.CommandText = "SELECT TOP (@TopCount) " + ThreadFields + @" FROM bx_Threads WHERE ForumID in(@ForumIDs) AND ThreadStatus = 1 ORDER BY ThreadID DESC ;";
                    query.CreateInParameter<int>("@ForumIDs", forumIDs);
                }
                else
                    query.CommandText = "SELECT TOP (@TopCount) " + ThreadFields + @" FROM bx_Threads WHERE ThreadStatus = 1 ORDER BY ThreadID DESC ;";
                query.CreateTopParameter("@TopCount", count);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCollectionV5(reader);
                }
            }
        }

        public override ThreadCollectionV5 GetTopViewThreads(IEnumerable<int> forumIDs, int count, DateTime? beginDate, DateTime? endDate)
        {
            using (SqlQuery query = new SqlQuery())
            {
                DeclareVariableCollection beforeSqlDeclare;
                string beforeSql, condition;
                ProcessThreadDateScope(beginDate, endDate, query, out beforeSqlDeclare, out beforeSql, out condition);

                if (forumIDs != null && ValidateUtil.HasItems<int>(forumIDs))
                {
                    query.CommandText = string.Concat(@"
 SELECT TOP (@TopCount) ", ThreadFields, @" FROM bx_Threads WHERE ForumID in(@ForumIDs) AND ThreadStatus = 1 ", condition, @" ORDER BY TotalViews DESC;
");
                    query.CreateInParameter<int>("@ForumIDs", forumIDs);
                }
                else
                {
                    query.CommandText = string.Concat(@"
 SELECT TOP (@TopCount) ", ThreadFields, @" FROM bx_Threads WHERE ThreadStatus = 1 ", condition, @" ORDER BY TotalViews DESC;
");
                }

                query.CreateTopParameter("@TopCount", count);


                query.CommandText = beforeSqlDeclare.GetDeclareVariableSql() + beforeSql + query.CommandText;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCollectionV5(reader);
                }
            }
        }

        public override ThreadCollectionV5 GetUserQuestionThreads(int userID, int count, int exceptThreadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = string.Concat(@"
    IF @ExceptThreadID>0
		SELECT TOP(@TopCount) ", ThreadFields, @" FROM bx_Threads WITH (NOLOCK) WHERE ThreadType = 2 AND PostUserID=@UserID AND ThreadID<>@ExceptThreadID AND [ThreadStatus] < 4 ORDER BY ThreadID DESC;
	ELSE
		SELECT TOP(@TopCount) ", ThreadFields, @" FROM bx_Threads WITH (NOLOCK) WHERE ThreadType = 2 AND PostUserID=@UserID AND [ThreadStatus] < 4 ORDER BY ThreadID DESC;
");
                query.CommandType = CommandType.Text;

                query.CreateTopParameter("@TopCount", count);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ExceptThreadID", exceptThreadID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCollectionV5(reader);
                }
            }
        }

        #region 存储过程 bx_GetPolemizeWithReplies
        [StoredProcedure(Name = "bx_GetPolemizeWithReplies", Script = @"
CREATE PROCEDURE {name}
     @ThreadID          int
    ,@PageIndex         int
    ,@PageSize          int
    ,@TotalCount        int
    ,@GetExtendedInfo   bit    
    ,@GetThread         bit 
    ,@GetThreadContent  bit
    ,@CheckThreadType   bit
    ,@PostType          tinyint
    ,@TopMarkCount      int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RealThreadType tinyint;
    SELECT @RealThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID; 

    IF @CheckThreadType = 1 BEGIN

        IF @RealThreadType >= 10
            SET @RealThreadType = 0;
        SELECT @RealThreadType;
        IF @RealThreadType != 4 
            RETURN;

    END 

    DECLARE @GetPostCondition nvarchar(4000);
    SET @GetPostCondition = '';
    
    IF @GetThreadContent = 1
        SET @GetPostCondition = ' ThreadID = ' + CAST(@ThreadID as varchar(16));

    DECLARE @SQLString nvarchar(4000);
    DECLARE @PostFieldsString nvarchar(4000);

    IF @GetPostCondition <> '' BEGIN
        SELECT 1;
        IF @GetExtendedInfo = 0
            EXEC('SELECT TOP 1 * FROM bx_Posts WITH (NOLOCK) WHERE '+ @GetPostCondition + ' ORDER BY PostID');
        ELSE BEGIN
            SET @SQLString = 'SELECT TOP 1 " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE ' + @GetPostCondition + ' ORDER BY PostID';
            " + PostFields + @"
        END
    END
    ELSE
        SELECT 0;
    
    IF @GetThread = 1 BEGIN
        SELECT " + ThreadFields + @" FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID AND ThreadStatus < 4;
    END    


    IF @TotalCount IS NULL BEGIN
        IF @PostType IS NULL
            SELECT @TotalCount = TotalReplies + 1 FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;
        ELSE
            SELECT @TotalCount = Count(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND PostType = @PostType AND [SortOrder]<4000000000000000;
    END

    SELECT @TotalCount;

    IF (@PageIndex * @PageSize) > @TotalCount BEGIN --如果大于最大页 就取最后页
        SET @PageIndex = @TotalCount / @PageSize - 1;
        IF @TotalCount = 0 OR @TotalCount % @PageSize > 0
            SET @PageIndex = @PageIndex + 1;
    END

	SET @SQLString='';

    DECLARE @Condition nvarchar(4000);
    
    IF @PostType IS NULL
        SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16)) + ' AND [SortOrder]<4000000000000000 ';
    ELSE
        SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16)) + ' AND [SortOrder]<4000000000000000 AND [PostType]=' + CAST(@PostType as varchar(16));

	DECLARE @ResetOrder bit;

    IF @GetExtendedInfo = 0 BEGIN
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'*',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        EXECUTE ('' + @SQLString + '');
        
        SELECT @ResetOrder;
    END
    ELSE BEGIN
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'" + PostColumns + @"',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        " + PostFields + @"
    END
     
END
"
            )]
        #endregion
        public override void GetPolemizeWithReplies(int threadID, PostType? postType, int pageNumber, int pageSize, bool getExtendedInfo, bool getThread, bool getThreadContent, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetPolemizeWithReplies";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<bool>("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);
                query.CreateParameter<bool>("@GetThread", getThread, SqlDbType.Bit);
                query.CreateParameter<int>("@PageIndex", pageNumber - 1, SqlDbType.Int);
                query.CreateParameter<int>("@PageSize", pageSize, SqlDbType.Int);
                SetTopMarkCountParam(query);

                int? postTypeValue = null;
                if (postType != null)
                    postTypeValue = (int)postType.Value;

                query.CreateParameter<int?>("@PostType", postTypeValue, SqlDbType.TinyInt);

                if (thread != null && postType == null)
                    totalCount = thread.TotalReplies + 1;

                query.CreateParameter<int?>("@TotalCount", totalCount, SqlDbType.Int);
                query.CreateParameter<bool>("@CheckThreadType", checkThreadType, SqlDbType.Bit);

                if (thread != null && thread.ThreadContent != null)
                    getThreadContent = false;

                query.CreateParameter<bool>("@GetThreadContent", getThreadContent, SqlDbType.Bit);
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (checkThreadType)
                    {
                        while (reader.Read())
                        {
                            threadType = (ThreadType)reader.Get<byte>(0);
                        }
                    }

                    bool hasReadThreadContent = false;

                    #region Read hasReadThreadContent value
                    if (checkThreadType == false)
                    {
                        while (reader.Read())
                        {
                            hasReadThreadContent = reader.Get<int>(0) == 1;
                        }
                    }
                    else
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                hasReadThreadContent = reader.Get<int>(0) == 1;
                            }
                        }
                    }
                    #endregion

                    PostCollectionV5 tempPosts = new PostCollectionV5();

                    #region 读取 主题内容 设置tempPosts
                    if (hasReadThreadContent)
                    {
                        if (getExtendedInfo)
                        {
                            tempPosts = GetPosts(reader, false);
                        }
                        else
                        {
                            if (reader.NextResult())
                            {
                                tempPosts = new PostCollectionV5(reader);
                            }
                        }
                    }
                    #endregion

                    if (getThread)
                    {
                        #region Read thread
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                thread = GetThread(reader, null);
                            }
                        }
                        #endregion
                    }

                    if (thread != null && tempPosts.Count > 0)
                    {
                        thread.ThreadContent = tempPosts[0];
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }

                    if (getExtendedInfo)
                    {
                        posts = GetPosts(reader, false);
                    }
                    else
                    {
                        if (reader.NextResult())
                        {
                            posts = new PostCollectionV5(reader);
                        }
                        else
                            posts = new PostCollectionV5();
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            bool resetOrder = reader.Get<bool>(0);
                            if (resetOrder)
                            {
                                PostCollectionV5 results = new PostCollectionV5();
                                for (int i = posts.Count; i > 0; i--)
                                {
                                    results.Add(posts[i - 1]);
                                }

                                posts = results;
                            }
                        }
                    }
                }
            }
        }



        private const string sql1 = " CONVERT(varchar(100), getdate()-@D1, 14) ";
        private const string sql2 = " CONVERT(varchar(100), getdate()-@D2, 14) ";

        #region 存储过程 bx_GetPagePosts
        [StoredProcedure(Name = "bx_GetPagePosts", Script = @"
CREATE PROCEDURE {name}
     @ThreadID          int
    ,@PageIndex         int
    ,@PageSize          int
    ,@TotalCount        int
    ,@ThreadType        int
    ,@GetExtendedInfo   bit    
    ,@GetThread         bit 
    ,@CheckThreadType   bit
    ,@GetBestPost       bit 
    ,@GetThreadContent  bit
    ,@TopMarkCount      int
    ,@OnlyNormal        bit
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @S VARCHAR(8000);

    DECLARE @BestPostID int;--,@ProcessBestPost bit;
    DECLARE @RealThreadType tinyint;
    DECLARE @TempCount int,@ContentID int;
    
    DECLARE @D1 datetime;
    SELECT @D1 = GETDATE();    

    SELECT @RealThreadType = ThreadType,@TempCount = TotalReplies + 1,@ContentID = ContentID  FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
    
    IF @TempCount < 1 BEGIN
        SELECT @TempCount = COUNT(*) FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @ThreadID;
        UPDATE bx_Threads SET TotalReplies = @TempCount - 1 WHERE ThreadID = @ThreadID;
    END

    SET @S = '1='+ " + sql1 + @";

    SET @BestPostID = 0;

    IF @CheckThreadType = 1 BEGIN

        IF @RealThreadType >= 10
            SET @RealThreadType = 0;
        SELECT @RealThreadType;
        IF @RealThreadType != @ThreadType 
            RETURN;

    END 


    DECLARE @SQLString nvarchar(4000),@SQLString2 nvarchar(4000);
    SET @SQLString = '';
    SET @SQLString2 = '';
    
    DECLARE @PostFieldsString nvarchar(4000);
    " + PostFields_DeclarePostIDTable + @"
    IF @RealThreadType = 2 BEGIN --问题帖
            SELECT @BestPostID = BestPostID FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            IF NOT EXISTS(SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BestPostID AND SortOrder < 4000000000000000) BEGIN -- 最佳答案被删除了
                SET @BestPostID = 0;
                UPDATE bx_Questions SET BestPostID = 0 WHERE ThreadID = @ThreadID; -- 顺便把BestPostID更新为0 免得每次都来查询
            END
        
            IF @BestPostID <> 0 AND @GetBestPost = 1 BEGIN
                IF @GetExtendedInfo = 0
                    SET @SQLString = 'SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BID;';-- + CAST(@BestPostID AS varchar(16)) + '';
                ELSE BEGIN
                    SET @PostFieldsString = @PostFieldsString + ' INSERT INTO @PostIDTable(" + PostColumns + @") SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BID; ';
                    
                END
            END
    END

    IF @GetThreadContent = 1  BEGIN ---  都取出来吧 AND (@RealThreadType = 2 OR @RealThreadType = 4) --问题帖 或者 辩论帖 才取出主题内容
        
        IF @GetExtendedInfo = 0
            SET @SQLString2 = ' SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @CID;';-- + CAST(@ContentID as varchar(16));
        ELSE
            SET @PostFieldsString = @PostFieldsString + ' INSERT INTO @PostIDTable(" + PostColumns + @") SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = @CID';

    END

    IF @GetExtendedInfo = 0 BEGIN
        IF @SQLString <> '' OR @SQLString2 <>'' BEGIN
            SELECT 1;
            IF @SQLString <> ''  BEGIN
                SELECT @D1 = GETDATE();
                EXECUTE sp_executesql 
                  @SQLString,
                  N'@BID INT'
                 ,@BID = @BestPostID 
                SET @S =@S + '|2='+ " + sql1 + @";
            END
            IF @SQLString2 <> ''  BEGIN
                SELECT @D1 = GETDATE();
                EXECUTE sp_executesql 
                  @SQLString2,
                  N'@CID INT'
                 ,@CID = @ContentID
                SET @S =@S + '|3='+ " + sql1 + @";
            END
            IF @SQLString = '' OR @SQLString2 = ''
                SELECT 0 AS PostID;
        END
        ELSE
            SELECT 0;
    END 
    ELSE
        SELECT 0;
    
    IF @GetThread = 1 BEGIN
        IF @OnlyNormal = 1 BEGIN
            SELECT @D1 = GETDATE();
            SELECT " + ThreadFields + @" FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID AND ThreadStatus < 4;
            
    SET @S =@S + '|4='+ " + sql1 + @";
        END
        ELSE BEGIN
            SELECT @D1 = GETDATE();
            SELECT " + ThreadFields + @" FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            
    SET @S =@S + '|5='+ " + sql1 + @";
        END
    END    


    IF @TotalCount IS NULL OR @OnlyNormal = 0 BEGIN
        IF @OnlyNormal = 0 BEGIN
            SELECT @D1 = GETDATE();
            SELECT @TotalCount = Count(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            
    SET @S =@S + '|6='+ " + sql1 + @";
        END
        ELSE BEGIN
            SELECT @D1 = GETDATE();
            SELECT @TotalCount = @TempCount;
            --SELECT @TotalCount = TotalReplies + 1 FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;
            
    SET @S =@S + '|7='+ " + sql1 + @";
        END
    END
    
    IF @OnlyNormal = 0
        SELECT @TotalCount;

    IF @BestPostID <> 0 -- 减掉最佳答案 
        SET @TotalCount = @TotalCount - 1;

    IF (@PageIndex * @PageSize) > @TotalCount BEGIN --如果大于最大页 就取最后页
        SET @PageIndex = @TotalCount / @PageSize - 1;
        IF  @TotalCount = 0 OR @TotalCount % @PageSize > 0
            SET @PageIndex = @PageIndex + 1;
    END

	SET @SQLString='';

    DECLARE @Condition nvarchar(4000);
    
    IF @BestPostID > 0 
        SET @Condition = '[PostID]<>@BID AND [ThreadID]=@TID ';
    ELSE
        SET @Condition = '[ThreadID]=@TID ';

    IF @OnlyNormal = 1
        SET @Condition = @Condition + ' AND [SortOrder]<4000000000000000 ';

	DECLARE @ResetOrder bit;

    IF @GetExtendedInfo = 0 BEGIN
        
        SELECT @D1 = GETDATE();
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'*',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT
        
    SET @S =@S + '|8='+ " + sql1 + @";
        SELECT @D1 = GETDATE();

    IF @BestPostID > 0 BEGIN
        EXECUTE sp_executesql 
          @SQLString,
          N'@TID INT,@BID INT',
          @TID = @ThreadID
         ,@BID = @BestPostID
    END
    ELSE BEGIN
        EXECUTE sp_executesql 
          @SQLString,
          N'@TID INT',
          @TID = @ThreadID
    END

    SET @S =@S + '|9='+ " + sql1 + @";
        SELECT @ResetOrder;
    END
    ELSE BEGIN
        SELECT @D1 = GETDATE();
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'" + PostColumns+ @"',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

    SET @PostFieldsString = @PostFieldsString + ' 
    SELECT @D2 = GETDATE();
INSERT INTO @PostIDTable(" + PostColumns + @")'+@SQLString+'
    SET @S2 =@S2 + ''|A1=''+ " + sql2 + @";
';

    SET @S =@S + '|10='+ " + sql1 + @";
        SELECT @D1 = GETDATE();
    SET @PostFieldsString = @PostFieldsString + " + PostFields_ExtendInfo + s2 + @"
    SET @S =@S + '|11='+ " + sql1 + @";
        SET @ResetOrder = 0;
        SELECT @ResetOrder;
    END
    SELECT @S;
    
END
"
            )]
        #endregion
        public override void GetPosts(int threadID, bool onlyNormal, int pageNumber, int pageSize, int? totalCount, bool getExtendedInfo, bool getThread, bool getThreadContent, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetPagePosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<bool>("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);
                query.CreateParameter<bool>("@GetThread", getThread, SqlDbType.Bit);
                query.CreateParameter<int>("@PageIndex", pageNumber - 1, SqlDbType.Int);
                query.CreateParameter<int>("@PageSize", pageSize, SqlDbType.Int);
                query.CreateParameter<int?>("@TotalCount", totalCount, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadType", (int)threadType, SqlDbType.TinyInt);
                query.CreateParameter<bool>("@CheckThreadType", checkThreadType, SqlDbType.Bit);
                query.CreateParameter<bool>("@OnlyNormal", onlyNormal, SqlDbType.Bit);

                SetTopMarkCountParam(query);


                if (getThreadContent)
                {
                    if (thread != null && thread.ThreadContent != null)// && thread is BasicThread)
                    {
                        getThreadContent = false;
                    }
                }

                bool getBestPost = false;

                QuestionThread question = null;
                if (getThreadContent)
                {
                    if (thread != null && thread is QuestionThread)
                    {
                        question = (QuestionThread)thread;
                        getBestPost = question.BestPost == null;
                    }
                    else if (thread == null && getThread)
                    {
                        getBestPost = true;
                    }
                }

                if (pageNumber == 1)
                    getThreadContent = false;

                query.CreateParameter<bool>("@GetBestPost", getBestPost, SqlDbType.Bit);
                query.CreateParameter<bool>("@GetThreadContent", getThreadContent, SqlDbType.Bit);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (checkThreadType)
                    {
                        while (reader.Read())
                        {
                            threadType = (ThreadType)reader.Get<byte>(0);
                        }
                    }

                    bool hasReadThreadContentOrBestPost = false;

                    #region Read hasReadThreadContentOrBestPost value
                    if (checkThreadType == false)
                    {
                        while (reader.Read())
                        {
                            hasReadThreadContentOrBestPost = reader.Get<int>(0) == 1;
                        }
                    }
                    else
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                hasReadThreadContentOrBestPost = reader.Get<int>(0) == 1;
                            }
                        }
                    }
                    #endregion

                    PostCollectionV5 tempPosts = new PostCollectionV5();

                    #region 读取 主题内容 最佳答案 设置tempPosts  仅当 getExtendedInfo = false 时有
                    if (hasReadThreadContentOrBestPost)
                    {
                        //if (getExtendedInfo)
                        //{
                        //    tempPosts = GetPosts(reader, false);
                        //}
                        //else
                        //{
                            if (reader.NextResult())
                            {
                                tempPosts = new PostCollectionV5(reader);
                            }

                            if (reader.NextResult())
                            {
                                if (reader.Read())
                                {
                                    int id = reader.Get<int>("PostID");
                                    if (id != 0)
                                    {
                                        PostV5 post = new PostV5(reader);
                                        tempPosts.Add(post);
                                    }
                                }
                            }
                        //}
                    }
                    #endregion

                    if (getThread)
                    {
                        #region Read thread
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                thread = GetThread(reader,null);
                            }
                        }
                        #endregion
                    }

                    int total = 0;
                    if (onlyNormal == false)
                    {
                        if (reader.NextResult() && reader.Read())
                        {
                            total = reader.Get<int>(0);
                        }
                    }

                    if (thread != null && tempPosts.Count > 0)
                    {
                        SetThreadContent(thread, question, tempPosts, getBestPost, pageNumber, false);
                    }

                    if (getExtendedInfo)
                    {
                        posts = GetPosts(reader, false);

                        if (thread != null && (getBestPost || getThreadContent))
                        {
                            SetThreadContent(thread, question, posts, getBestPost, pageNumber, true);
                        }
                    }
                    else
                    {
                        if (reader.NextResult())
                        {
                            posts = new PostCollectionV5(reader);
                        }
                        else
                            posts = new PostCollectionV5();
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            bool resetOrder = reader.Get<bool>(0);
                            if (resetOrder)
                            {
                                PostCollectionV5 results = new PostCollectionV5();
                                for (int i = posts.Count; i > 0; i--)
                                {
                                    results.Add(posts[i - 1]);
                                }

                                posts = results;
                            }
                        }
                    }

                    if (reader.NextResult())
                    {
                        string s = null;
                        while (reader.Read())
                        {
                            s = reader.Get<string>(0);
                        }

#if !Publish
                        if (reader.SqlQuery.TempInfo != null)
                            reader.SqlQuery.TempInfo += "-----" + s;
                        else
                            reader.SqlQuery.TempInfo = s;
#endif
                    }

                    if (pageNumber == 1 && thread != null && posts.Count > 0)
                        thread.ThreadContent = posts[0];
                        

                    posts.TotalRecords = total;
                }
            }
        }

        private void SetThreadContent(BasicThread thread, QuestionThread question, PostCollectionV5 posts, bool getBestPost, int pageNumber, bool getExtendedInfo)
        {
            bool removeContent = false;
            bool removeBestPost = false;

            if (thread is QuestionThread)
                question = (QuestionThread)thread;

            foreach (PostV5 post in posts)
            {
                //if (post.PostType == PostType.ThreadContent)
                //    thread.ThreadContent = post;
                //bool isBestPost = false;
                if (question != null)
                {
                    if (question.BestPostID == post.PostID)
                    {
                        question.BestPost = post;
                        //isBestPost = true;
                        removeBestPost = true;
                    }
                }

                if (post.PostID == thread.ContentID)
                {
                    thread.ThreadContent = post;
                    removeContent = true;
                }
            }

            if (getExtendedInfo)
            {
                if (removeBestPost)
                    posts.Remove(question.BestPost);
                if (removeContent && pageNumber > 1)
                    posts.Remove(thread.ThreadContent);
            }

            if (question != null && question.BestPostID != 0 && question.BestPost == null && getBestPost)
            {
                //这种情况 可能发生在  最佳答案被删除的情况下   所以把BestPostID更新为0
                question.BestPostID = 0;
            }
        }



        #region 存储过程 bx_GetUserPosts
        [StoredProcedure(Name = "bx_GetThreadUserPosts", Script = @"
CREATE PROCEDURE {name}
     @ThreadID          int
    ,@UserID            int
    ,@PageIndex         int
    ,@PageSize          int
    ,@ThreadType        int
    ,@GetExtendedInfo   bit    
    ,@GetThread         bit 
    ,@CheckThreadType   bit
    ,@TopMarkCount      int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RealThreadType tinyint;
    SELECT @RealThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID; 

    IF @CheckThreadType = 1 BEGIN

        IF @RealThreadType >= 10
            SET @RealThreadType = 0;
        SELECT @RealThreadType;
        IF @RealThreadType != @ThreadType 
            RETURN;

    END 

    
    IF @GetThread = 1 BEGIN
        SELECT " + ThreadFields + @" FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID AND ThreadStatus < 4;
    END    


    DECLARE @TotalCount int;
    SELECT @TotalCount = Count(*) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND UserID = @UserID;

    SELECT @TotalCount;

    IF (@PageIndex * @PageSize) > @TotalCount BEGIN --如果大于最大页 就取最后页
        SET @PageIndex = @TotalCount / @PageSize - 1;
        IF @TotalCount = 0 or @TotalCount % @PageSize > 0
            SET @PageIndex = @PageIndex + 1;
    END

    DECLARE @Condition nvarchar(4000);

    SET @Condition = '[UserID] = ' + CAST(@UserID as varchar(16)) + ' AND [ThreadID]=' + CAST(@ThreadID as varchar(16)) + ' AND [SortOrder]<4000000000000000 ';

	DECLARE @ResetOrder bit,@SQLString nvarchar(4000);

    IF @GetExtendedInfo = 0 BEGIN
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'*',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        EXECUTE ('' + @SQLString + '');
        
        SELECT @ResetOrder;
    END
    ELSE BEGIN
        DECLARE @PostFieldsString nvarchar(4000);
	    EXECUTE bx_Common_GetRecordsByPageSQLString 
				    @PageIndex,
				    @PageSize,
				    N'bx_Posts',
				    N'" + PostColumns + @"',
				    @Condition,
				    N'[SortOrder]',
				    0,
				    @TotalCount,
				    @ResetOrder OUTPUT,
				    @SQLString OUTPUT

        " + PostFields + @"
    END
     
END
"
            )]
        #endregion
        public override void GetUserPosts(int threadID, int userID, int pageNumber, int pageSize, bool getExtendedInfo, bool getThread, bool checkThreadType, ref BasicThread thread, out PostCollectionV5 posts, ref ThreadType threadType, out int totalCount)
        {
            totalCount = 0;
            posts = new PostCollectionV5();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetThreadUserPosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<bool>("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);
                query.CreateParameter<bool>("@GetThread", getThread, SqlDbType.Bit);
                query.CreateParameter<int>("@PageIndex", pageNumber - 1, SqlDbType.Int);
                query.CreateParameter<int>("@PageSize", pageSize, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadType", (int)threadType, SqlDbType.TinyInt);
                query.CreateParameter<bool>("@CheckThreadType", checkThreadType, SqlDbType.Bit);

                SetTopMarkCountParam(query);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    bool isFirstRead = true;
                    if (checkThreadType)
                    {
                        isFirstRead = false;
                        while (reader.Read())
                        {
                            threadType = (ThreadType)reader.Get<byte>(0);
                        }
                    }

                    if (getThread)
                    {
                        if (isFirstRead)
                        {
                            while (reader.Read())
                            {
                                thread = GetThread(reader,null);
                            }
                        }
                        else
                        {
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    thread = GetThread(reader,null);
                                }
                            }
                        }

                        isFirstRead = false;
                    }

                    if (isFirstRead)
                    {
                        while (reader.Read())
                            totalCount = reader.Get<int>(0);
                    }
                    else
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                                totalCount = reader.Get<int>(0);
                        }
                    }

                    if (getExtendedInfo)
                    {
                        posts = GetPosts(reader, false);
                    }
                    else
                    {
                        if (reader.NextResult())
                        {
                            posts = new PostCollectionV5(reader);
                        }
                        else
                            posts = new PostCollectionV5();
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            bool resetOrder = reader.Get<bool>(0);
                            if (resetOrder)
                            {
                                PostCollectionV5 results = new PostCollectionV5();
                                for (int i = posts.Count; i > 0; i--)
                                {
                                    results.Add(posts[i - 1]);
                                }

                                posts = results;
                            }
                        }
                    }
                }
            }
        }

        /*
        #region 存储过程 bx_GetCachedPosts
        [StoredProcedure(Name = "bx_GetCachedPosts", Script = @"
CREATE PROCEDURE {name}
     @ThreadID            int
    ,@GetOldCount         int
    ,@GetNewCount         int 
    ,@TotalCount          int
    ,@BestPostID          int
    ,@TopMarkCount        int    
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SQLString nvarchar(4000),@SQLString2 nvarchar(4000);
	SET @SQLString='';
    SET @SQLString2 = '';

    DECLARE @Condition nvarchar(4000);
    SET @Condition = '[ThreadID]=' + CAST(@ThreadID as varchar(16))+' AND [SortOrder]<4000000000000000 ';

    IF @BestPostID <> 0 BEGIN
        SET @TotalCount = @TotalCount - 1;
        SET @Condition = '[PostID]<>' + CAST(@BestPostID as varchar(16)) + ' AND ' + @Condition;
    END 

    DECLARE @ExcuteSqlString2 char(1);
    IF @GetOldCount + @GetNewCount >= @TotalCount BEGIN
        SET @SQLString = 'SELECT TOP '+ CAST((@GetOldCount + @GetNewCount) as varchar(16)) +' " + PostColumns + @" FROM [bx_Posts] WITH(NOLOCK) WHERE ' + @Condition + ' ORDER BY PostID DESC ';
        SET @SQLString2 = @SQLString;
        SET @ExcuteSqlString2 = '0';
    END
    ELSE BEGIN
        SET @ExcuteSqlString2 = '1';
        SET @SQLString = '
                            SELECT * FROM 
                            (SELECT TOP '+ CAST(@GetOldCount as varchar(16)) +' " + PostColumns + @" FROM [bx_Posts] WITH(NOLOCK) WHERE ' + @Condition + ' ORDER BY PostID ASC) AS T1
                         ';

        SET @SQLString2 = '
                            SELECT * FROM 
                            (SELECT TOP '+ CAST(@GetNewCount as varchar(16)) +' " + PostColumns + @" FROM [bx_Posts] WITH(NOLOCK) WHERE ' + @Condition + ' ORDER BY PostID DESC) AS T2
                          ';
    END
        
        DECLARE @PostFieldsString nvarchar(4000);
        " + PostFields2 + @"
     
END
"
            )]
        #endregion

        */
        public override void GetPosts(int threadID, int totalReplies, int getOldCount, int getNewCount, ref BasicThread thread, out PostV5 threadContent, out PostCollectionV5 topPosts)
        {
            threadContent = null;
            topPosts = null;

            using (SqlQuery query = new SqlQuery())
            {
                int bestPostID = 0;
                if (SetBestPost(query, thread))
                {
                    QuestionThread question = (QuestionThread)thread;
                    bestPostID = question.BestPostID;
                }

                query.CommandText = "bx_GetCachedPosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@TotalCount", totalReplies + 1, SqlDbType.Int);//要获取主题内容  所以回复总数加1
                query.CreateParameter<int>("@GetOldCount", getOldCount + 1, SqlDbType.Int);//要获取主题内容  所以加1
                query.CreateParameter<int>("@GetNewCount", getNewCount, SqlDbType.Int);

                SetTopMarkCountParam(query);
                query.CreateParameter<int>("@BestPostID", bestPostID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    topPosts = GetPosts(reader, true);
                    if (topPosts.Count > 0)
                    {
                        threadContent = topPosts[0];
                        topPosts.RemoveAt(0);
                    }
                }
            }
        }

        #region 存储过程 bx_v5_GetBestPost
        [StoredProcedure(Name = "bx_v5_GetBestPost", Script = @"
CREATE PROCEDURE {name}
     @ThreadID       int
    ,@BestPostID     int
    ,@TopMarkCount   int   
AS
BEGIN
    SET NOCOUNT ON;
    IF @BestPostID<>0 AND NOT EXISTS(SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @BestPostID AND SortOrder<4000000000000000) BEGIN -- 最佳答案被删除了
        SET @BestPostID = 0;
        UPDATE bx_Questions SET BestPostID = 0 WHERE ThreadID = @ThreadID; -- 顺便把BestPostID更新为0 免得每次都来查询
    END

    IF @BestPostID <> 0 BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = ' + CAST(@BestPostID AS varchar(16)) + '';
        " + PostFields + @"
    END 
END
"
            )]
        #endregion
        /// <summary>
        /// 如果是问题帖子 并且有最佳答案则返回true
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="thread">必须是 QuestionThreadV5 类型</param>
        /// <returns></returns>
        private bool SetBestPost(SqlQuery query, BasicThread thread)
        {
            if (thread is QuestionThread)
            {
                QuestionThread question = (QuestionThread)thread;
                if (question.BestPost != null)
                    return true;
                else if (question.BestPostID == 0)
                    return false;
                else
                {
                    query.CommandType = CommandType.StoredProcedure;
                    query.CommandText = "bx_v5_GetBestPost";
                    query.CreateParameter<int>("@ThreadID", thread.ThreadID, SqlDbType.Int);
                    query.CreateParameter<int>("@BestPostID", question.BestPostID, SqlDbType.Int);
                    SetTopMarkCountParam(query);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        question.BestPost = GetPost(reader, true);
                        if (question.BestPost == null)
                            question.BestPostID = 0;
                    }

                    query.Parameters.Clear();

                    return question.BestPost != null;
                }
            }

            return false;
        }

        #region 存储过程 bx_v5_GetPost
        [StoredProcedure(Name = "bx_v5_GetPost", Script = @"
CREATE PROCEDURE {name}
     @PostID              int
    ,@GetExtendedInfo     bit
    ,@TopMarkCount        int   
AS
BEGIN
    SET NOCOUNT ON;
    IF @GetExtendedInfo = 0
        SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
    ELSE BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = ' + CAST(@PostID AS varchar(16)) + '';
        " + PostFields + @"
    END
END
"
            )]
        #endregion
        public override PostV5 GetPost(int postID, bool getExtendedInfo)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetPost";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);
                query.CreateParameter<bool>("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);
                SetTopMarkCountParam(query);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return GetPost(reader, true);
                }
            }
        }



        #region 存储过程 bx_GetUserPosts
        [StoredProcedure(Name = "bx_GetUserPosts", Script = @"
CREATE PROCEDURE {name}
     @UserID               int
    ,@BeginDate            datetime
    ,@EndDate              datetime
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @BeginDate AND [CreateDate] <= @EndDate;
END
"
            )]
        #endregion
        public override PostCollectionV5 GetUserPosts(int userID, DateTime beginDate, DateTime endDate)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetUserPosts";
                query.CommandType = CommandType.StoredProcedure;


                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@BeginDate", beginDate, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@EndDate", endDate, SqlDbType.DateTime);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PostCollectionV5(reader);
                }
            }
        }



        #region 存储过程 bx_v5_GetThreadFirstPost
        [StoredProcedure(Name = "bx_v5_GetThreadFirstPost", Script = @"
CREATE PROCEDURE {name}
     @ThreadID            int
    ,@GetExtendedInfo     bit
    ,@TopMarkCount        int   
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @PostID int;
    --SET @PostID = (SELECT TOP 1 PostID FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID ORDER BY PostID);
    SELECT @PostID = ContentID FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;

    IF @GetExtendedInfo = 0
        SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
    ELSE BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = ' + CAST(@PostID AS varchar(16)) + '';
        " + PostFields + @"
    END
END
"
            )]
        #endregion
        public override PostV5 GetThreadFirstPost(int threadID, bool getExtendedInfo)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetThreadFirstPost";
                query.CommandType = CommandType.StoredProcedure; 

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<bool>("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);
                SetTopMarkCountParam(query);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return GetPost(reader, true);
                }
            }
        }

        private PostV5 GetPost(XSqlDataReader reader, bool isFirstRead)
        {
            PostCollectionV5 posts = GetPosts(reader, isFirstRead);
            if (posts.Count > 0)
                return posts[0];
            else
                return null;
        }

        private PostCollectionV5 GetPosts(XSqlDataReader reader, bool isFirstRead)
        {
            PostCollectionV5 posts = new PostCollectionV5();

            List<int> replyIDs = new List<int>();

            if (isFirstRead)
            {
                while (reader.Read())
                {
                    PostV5 post = new PostV5(reader);
                    post.Attachments = new AttachmentCollection();
                    post.PostMarks = new PostMarkCollection();
                    posts.Add(post);
                    replyIDs.Add(post.PostID);
                }
            }
            else
            {
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        PostV5 post = new PostV5(reader);
                        post.Attachments = new AttachmentCollection();
                        post.PostMarks = new PostMarkCollection();
                        posts.Add(post);
                        replyIDs.Add(post.PostID);
                    }
                }
            }
            //读取下一个结果集
            if (reader.NextResult())
            {
                while (reader.Read()) //附件列表
                {
                    Attachment attachment = new Attachment(reader);
                    int replyIndex = replyIDs.IndexOf(attachment.PostID);
                    if (replyIndex != -1)
                    {
                        posts[replyIndex].Attachments.Add(attachment);
                    }
                }
            }
            //历史附件
            if (reader.NextResult())
            {
                while (reader.Read()) //附件列表
                {
                    Attachment attachment = new Attachment(reader);
                    attachment.AttachType = AttachType.History;
                    int hpostID = reader.Get<int>("HPostID");
                    int replyIndex = replyIDs.IndexOf(hpostID);
                    if (replyIndex != -1)
                    {
                        posts[replyIndex].Attachments.Add(attachment);
                    }
                }
            }
            if (reader.NextResult())//评分列表
            {
                while (reader.Read())
                {
                    PostMark postMark = new PostMark(reader);
                    int replyIndex = replyIDs.IndexOf(postMark.PostID);
                    if (replyIndex != -1)
                    {
                        posts[replyIndex].PostMarks.Add(postMark);
                    }
                }
            }
            if (reader.NextResult())
            {
                string s = null;
                while (reader.Read())
                {
                    s = reader.Get<string>(0);
                }

#if !Publish
                if (reader.SqlQuery.TempInfo != null)
                    reader.SqlQuery.TempInfo += "-----" + s;
                else
                    reader.SqlQuery.TempInfo = s;
#endif
            }

            return posts;
        }

        [StoredProcedure(Name = "bx_GetPostsByIdentities", FileName = "v30\\bx_GetPostsByIdentities.sql")]
        public override PostCollectionV5 GetPosts(IEnumerable<int> postIds)
        {
            if (ValidateUtil.HasItems<int>(postIds) == false)
                return new PostCollectionV5();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetPostsByIdentities";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@PostIdentities", StringUtil.Join(postIds), SqlDbType.VarChar, 8000);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return GetPosts(reader, true);

                }
            }
        }


        public override Dictionary<int, PostV5> GetThreadContents(IEnumerable<int> threadIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostType=1 AND ThreadID in(@ThreadIDs);
"
                    ;
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@ThreadIDs", threadIDs);

                Dictionary<int, PostV5> posts = new Dictionary<int, PostV5>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PostV5 post = new PostV5(reader);
                        if (posts.ContainsKey(post.ThreadID) == false)
                            posts.Add(post.ThreadID, post);
                    }
                }

                return posts;
            }
        }

        #region 添加 更新

        #region 存储过程 bx_CreateThread
        [StoredProcedure(Name = "bx_v5_CreateThread", Script = @"
CREATE PROCEDURE {name}
	@ForumID                int,
	@ThreadCatalogID        int,
	@ThreadStatus           tinyint,
	@ThreadType             tinyint,
	@IconID                 int,
	@Subject                nvarchar(256),
	@SubjectStyle           nvarchar(300),
	@Price                  int,
	@UserID                 int,
	@NickName               nvarchar(64),
	@IsLocked               bit,
	@IsValued               bit,

	@Content                ntext,
	@ContentFormat          tinyint,
	@EnableSignature        bit,
	@EnableReplyNotice      bit,
	@IPAddress              nvarchar(64),
	
	@AttachmentIds          varchar(8000),
	@AttachmentFileNames    ntext,
	@AttachmentFileIds      text,
	@AttachmentFileSizes    varchar(8000),
	@AttachmentPrices       varchar(8000),
	@AttachmentFileExtNames ntext,
	@HistoryAttachmentIDs   varchar(500),


	@ThreadRandNumber       int,
	@UserTotalThreads       int output,
	@UserTotalPosts         int output,
    @ThreadID               int output,
    @PostID                 int output,
    @ExtendData             ntext
   ,@TopMarkCount                   int
   ,@TempPostID             int
   ,@AttachmentType         tinyint
   ,@Words                  nvarchar(400)
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;


	DECLARE @ReturnValue     int;

	DECLARE @TempSortOrder BIGINT,@PostDate datetime
	
	SET @PostDate = getdate();
	EXEC [bx_GetSortOrder] 1, @ThreadRandNumber, @PostDate, 1, @TempSortOrder OUTPUT;
	
    DECLARE @TempThreadID int
    SELECT @TempThreadID = ThreadID FROM bx_Threads WITH(NOLOCK) WHERE LastPostID = @TempPostID;
    IF @TempThreadID IS NOT NULL BEGIN
        DECLARE @LastPostID int;
        SELECT @LastPostID = MAX(PostID) FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @TempThreadID AND SortOrder<4000000000000000;
        IF @LastPostID IS NULL BEGIN    
            SELECT @LastPostID = MAX(PostID) FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @TempThreadID;
            IF @LastPostID IS NULL --说明是不正常的主题  没有一个post
                DELETE bx_Threads WHERE ThreadID = @TempThreadID;
        END
        IF @LastPostID IS NOT NULL BEGIN
            UPDATE bx_Threads SET LastPostID = @LastPostID WHERE ThreadID = @TempThreadID;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END
    
    BEGIN TRAN

	INSERT INTO [bx_Threads]
			([ForumID]
			,[ThreadCatalogID]
			,[ThreadType]
			,[IconID]
			,[Subject]
			,[SubjectStyle]
			,[Price]
			,[PostUserID]
			,[PostNickName]
			,[LastPostUserID]
			,[LastPostNickName]
			,[IsLocked]
			,[IsValued]
			,[SortOrder]
            ,[LastPostID]
            ,[ThreadStatus]
            ,[AttachmentType]
            ,[ExtendData]
            ,[Words]
            ,[PostedCount])
	 VALUES
			(@ForumID
			,@ThreadCatalogID
			,@ThreadType
			,@IconID
			,@Subject
			,@SubjectStyle
			,@Price
			,@UserID
			,@NickName
			,@UserID
			,@NickName
			,@IsLocked
			,@IsValued
			,@TempSortOrder
            ,@TempPostID
            ,@ThreadStatus
            ,@AttachmentType
            ,@ExtendData
            ,@Words
            ,1)
	
    SET @ErrorCount = @ErrorCount + @@error;
		
	SELECT @ThreadID = @@IDENTITY;
	IF(@ThreadID>0) BEGIN
	    ----------统计：增加主题数---------------
	    EXEC bx_DoCreateStat @ForumID,3, 1
        SET @ErrorCount = @ErrorCount + @@error;
    END
	    --------------------------
    IF @Words <> '' BEGIN
        INSERT INTO bx_ThreadWords(ThreadID,Word) SELECT @ThreadID,T.item FROM bx_GetStringTable_nvarchar(@Words, N',') T;
        SET @ErrorCount = @ErrorCount + @@error;
    END

	DECLARE @IsApproved bit
	IF @ThreadStatus=5
		SET @IsApproved=0
	ELSE
		SET @IsApproved=1
		
	EXECUTE @ReturnValue = [bx_v5_CreatePost] 
		 0
		,@ThreadID
		,1
		,@IconID
		,@Subject
		,@Content
		,@ContentFormat
		,@EnableSignature

		,@EnableReplyNotice
		,@ForumID
		,@UserID
		,@NickName
		,@IPAddress

		,@AttachmentIds
		,@AttachmentFileNames
		,@AttachmentFileIds
		,@AttachmentFileSizes
		,@AttachmentPrices
		,@AttachmentFileExtNames
		,@HistoryAttachmentIDs

		,0

		,@IsApproved
		,@ThreadRandNumber
		,0
		,1
        ,0
        ,0
        ,@PostID output
        ,@TopMarkCount
        ,1
        ,1
    
    SET @ErrorCount = @ErrorCount + @@error;

      
    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;

    
	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
    END  

	IF @PostID > 0 BEGIN
	    ----------统计：增加回复数---------------
		EXEC bx_DoCreateStat @ForumID,4, 1
		--------------------------
        SET @ErrorCount = @ErrorCount + @@error;
	END


    DECLARE @Today DateTime,@Monday DateTime;
	SET @Today = CONVERT(varchar(12) , getdate(), 102);
			
	DECLARE @m int;
	SELECT @m = DATEPART(weekday, @Today);
	IF @m = 1
		SELECT @m = 8;
	SELECT @Monday = DATEADD(day, 2-@m, @Today);


    DECLARE @WeekPosts int,@DayPosts int,@LastPostDate DateTime;
    SELECT @WeekPosts = WeekPosts,@DayPosts = DayPosts,@LastPostDate = LastPostDate FROM bx_Users WHERE UserID = @UserID;

	DECLARE @TempForumID int
	IF @ThreadStatus < 4 AND @UserID<>0 BEGIN
		SET @TempForumID=@ForumID;

        IF @LastPostDate >= @Monday
            SET @WeekPosts = @WeekPosts + 1;
        ELSE
            SET @WeekPosts = 1;

        IF @LastPostDate >= @Today
            SET @DayPosts = @DayPosts + 1;
        ELSE
            SET @DayPosts = 1;


		UPDATE [bx_Users]
		   SET [TotalTopics] = [TotalTopics] + 1
			  ,[TotalPosts] = [TotalPosts] + 1
              ,[WeekPosts] = @WeekPosts
              ,[DayPosts] = @DayPosts
			  ,[LastPostDate] = getdate()

		 WHERE UserID = @UserID;
        SET @ErrorCount = @ErrorCount + @@error;

    END
	ELSE BEGIN
		SET @TempForumID=-2;
        
        DECLARE @MustUpdate bit;
        IF @LastPostDate < @Monday BEGIN
            SET @WeekPosts = 0;
            SET @MustUpdate = 1;
        END
        IF @LastPostDate < @Today BEGIN
            SET @DayPosts = 0;
            SET @MustUpdate = 1;
        END
        IF @MustUpdate = 1 BEGIN
            UPDATE [bx_Users]
		        SET [WeekPosts] = @WeekPosts
                    ,[DayPosts] = @DayPosts
            WHERE UserID = @UserID;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END

	UPDATE [bx_Forums]
		   SET [TotalThreads] = [TotalThreads] + 1
			  ,[TotalPosts] = [TotalPosts] + 1
			  ,[TodayThreads] = [TodayThreads] + 1
			  ,[TodayPosts] = [TodayPosts] + 1
			  ,[LastThreadID] = @ThreadID
		 WHERE [ForumID] = @TempForumID;
    SET @ErrorCount = @ErrorCount + @@error;


	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=TotalThreads+1 WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID;
    SET @ErrorCount = @ErrorCount + @@error;
	
	IF @UserID=0 BEGIN
		SET @UserTotalThreads = 0;
		SET @UserTotalPosts = 0;

	END
	ELSE
		SELECT @UserTotalThreads=[TotalTopics],
			@UserTotalPosts=[TotalPosts]

			FROM [bx_Users] WITH (NOLOCK) WHERE UserID = @UserID;


    RETURN 0;

END
"
            )]
        #endregion
        public override bool CreateThread(int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
            , string subject, string subjectStyle, int price, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_CreateThread";

                SetCreateThreadParams(query, forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, postUserID, postNickName, isLocked
                    , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs, null, attachType, words);

                query.CreateParameter<int>("@ThreadID", SqlDbType.Int, ParameterDirection.Output);
                query.CreateParameter<int>("@PostID", SqlDbType.Int, ParameterDirection.Output);
                query.CreateParameter("@ThreadType", (int)ThreadType.Normal, SqlDbType.TinyInt);
                query.CreateParameter("@Price", price, SqlDbType.Int);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    GetThread(reader, attachments, null, out thread, out post, out attachmentIDs, out fileIDs);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue != -1)
                {
                    //threadID = Convert.ToInt32(query.Parameters["@ThreadID"].Value);
                    //postID = Convert.ToInt32(query.Parameters["@PostID"].Value);
                    totalThreads = (query.Parameters["@UserTotalThreads"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalThreads"].Value));
                    totalPosts = (query.Parameters["@UserTotalPosts"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalPosts"].Value));

                    return true;
                }
                else
                {
                    //threadID = 0;
                    //postID = 0;
                    totalPosts = 0;
                    totalThreads = 0;

                    return false;
                }
            }

        }


        private void GetThread(XSqlDataReader reader, AttachmentCollection attachments, DateTime? expiresDate, out BasicThread thread, out PostV5 post, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {
            attachmentIDs = new List<int>();
            fileIDs = new Dictionary<string, int>();
            thread = null;
            post = null;
            while (reader.Read())
            {
                thread = GetThread(reader, expiresDate);
            }
            if (attachments.Count > 0)
            {
                //本地刚上传的附件 ID
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        attachmentIDs.Add(reader.Get<int>(0));
                    }
                }
                //所有附件
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        string fileID = reader.Get<string>("fileID");
                        if (fileIDs.ContainsKey(fileID) == false)
                            fileIDs.Add(fileID, reader.Get<int>("attachmentID"));
                    }
                }
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    post = new PostV5(reader);
                }
            }
        }

        #region 存储过程 bx_CreatePoll
        [StoredProcedure(Name = "bx_v5_CreatePoll", Script = @"
CREATE PROCEDURE {name}
    @ForumID                    int,
	@ThreadCatalogID            int,
	@ThreadStatus               tinyint,

	@IconID                     int,
	@Subject                    nvarchar(256),
	@SubjectStyle               nvarchar(300),
	@UserID                     int,
	@NickName                   nvarchar(64),
	@IsLocked                   bit,
	@IsValued                   bit,

	@Content                    ntext,
	@ContentFormat              tinyint,
	@EnableSignature            bit,
	@EnableReplyNotice          bit,
	@IPAddress                  nvarchar(64),

	@PollItems                  nvarchar(4000),
	@Multiple                   int,
	@AlwaysEyeable              bit,
	@ExpiresDate                datetime,

	@AttachmentIds              varchar(8000),
	@AttachmentFileNames        ntext,
	@AttachmentFileIds          text,
	@AttachmentFileSizes        varchar(8000),
	@AttachmentPrices           varchar(8000),
	@AttachmentFileExtNames     ntext,
	@HistoryAttachmentIDs       varchar(500),


	@ThreadRandNumber           int,
	@UserTotalThreads           int output,
	@UserTotalPosts             int output,
    @ExtendData                 ntext
   ,@TopMarkCount                   int
   ,@TempPostID                 int
   ,@AttachmentType             tinyint
   ,@Words                  nvarchar(400)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	DECLARE @ReturnValue     int;
    DECLARE @ThreadID int, @PostID int

	EXECUTE @ReturnValue = [bx_v5_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		1,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IPAddress,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		@ThreadRandNumber,
		@UserTotalThreads output,
		@UserTotalPosts output,
        @ThreadID  output,
        @PostID    output,
        @ExtendData,
        @TopMarkCount,
        @TempPostID,
        @AttachmentType,
        @Words

    SET @ErrorCount = @ErrorCount + @@error;

    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;
	--插入投票信息
    INSERT INTO [bx_Polls]
       ([ThreadID]
       ,[Multiple]
       ,[AlwaysEyeable]
       ,[ExpiresDate])
     VALUES
       (@ThreadID
       ,@Multiple
       ,@AlwaysEyeable
       ,@ExpiresDate);

    SET @ErrorCount = @ErrorCount + @@error;

	----------增加投票数---------------
	EXEC [bx_DoCreateStat] @ForumID,5, 1
    SET @ErrorCount = @ErrorCount + @@error;
		--------------------------
	--字表字段变量
	DECLARE @ItemName nvarchar(512)
	SET @ItemName = ''
	
	--数量计数
	DECLARE @Index int
	SET @Index = 0
	
	WHILE(@PollItems <> '')
	BEGIN
		IF (CharIndex(char(13), @PollItems) = 0) BEGIN
			SET @ItemName = @PollItems
			SET @PollItems  = ''	
		END
		ELSE BEGIN
			SET @ItemName = substring(rtrim(ltrim(@PollItems)), 1, charIndex(char(13), rtrim(ltrim(@PollItems))) - 1)
			SET @PollItems = substring(rtrim(ltrim(@PollItems)), charIndex(char(13), rtrim(ltrim(@PollItems))) + 1, len(rtrim(ltrim(@PollItems)))-charIndex(char(13), rtrim(ltrim(@PollItems))))
		END

		INSERT INTO bx_PollItems(
			ThreadID,
			ItemName
		) VALUES (
			@ThreadID,
			REPLACE(@ItemName, char(10), N'')
		)
        SET @ErrorCount = @ErrorCount + @@error;
		
	END
    
    SELECT * FROM bx_PollItems WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END

END
"
        )]
        #endregion
        public override bool CreatePoll(string pollItems, int pollMultiple, bool pollIsAlwaysEyeable, DateTime pollExpiresDate
            , int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
            , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_CreatePoll";

                SetCreateThreadParams(query, forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, postUserID, postNickName, isLocked
                    , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs, null, attachType, words);

                query.CreateParameter("@PollItems", pollItems, SqlDbType.NText);
                query.CreateParameter("@Multiple", pollMultiple, SqlDbType.Int);
                query.CreateParameter("@AlwaysEyeable", pollIsAlwaysEyeable, SqlDbType.Bit);
                query.CreateParameter("@ExpiresDate", pollExpiresDate, SqlDbType.DateTime);

                PollItemCollectionV5 pollitems = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    GetThread(reader, attachments, pollExpiresDate, out thread, out post, out attachmentIDs, out fileIDs);

                    if (reader.NextResult())
                    {
                        pollitems = new PollItemCollectionV5(reader);
                    }
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue != -1)
                {
                    totalThreads = (query.Parameters["@UserTotalThreads"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalThreads"].Value));
                    totalPosts = (query.Parameters["@UserTotalPosts"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalPosts"].Value));
                }
                else
                {
                    totalThreads = 0;
                    totalPosts = 0;
                    return false;
                }


                string extendData = PollThreadV5.GetExtendData(pollIsAlwaysEyeable, pollExpiresDate, pollMultiple, pollitems, new List<int>());

                thread.SetExtendData(extendData);

                UpdateThreadExtendData(extendData, thread.ThreadID, query);

                return true;


            }

        }

        #region 存储过程 bx_v5_UpdateThreadExtendData
        [StoredProcedure(Name = "bx_v5_UpdateThreadExtendData", Script = @"
CREATE PROCEDURE {name}
    @ExtendData     ntext,
    @ThreadID       int
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE bx_Threads SET ExtendData = @ExtendData WHERE ThreadID = @ThreadID;
END
"
        )]
        #endregion
        private void UpdateThreadExtendData(string extendData, int threadID, SqlQuery query)
        {
            query.CommandText = "bx_v5_UpdateThreadExtendData";
            query.Parameters.Clear();
            query.CreateParameter("@ThreadID", threadID, SqlDbType.Int);
            query.CreateParameter("@ExtendData", extendData, SqlDbType.NText);

            query.ExecuteNonQuery();
        }

        #region 存储过程 bx_v5_GetThreadExtendData
        [StoredProcedure(Name = "bx_v5_GetThreadExtendData", Script = @"
CREATE PROCEDURE {name}
    @ThreadID       int,
    @ThreadType     tinyint
AS
BEGIN
    SET NOCOUNT ON;
    IF @ThreadType = 1 BEGIN -- 投票
        SELECT * FROM bx_Polls WITH (NOLOCK) WHERE ThreadID = @ThreadID;
        SELECT * FROM bx_PollItems WITH (NOLOCK) WHERE ThreadID = @ThreadID;
        SELECT DISTINCT UserID FROM bx_PollItemDetails WITH (NOLOCK) WHERE ItemID IN(SELECT ItemID FROM bx_PollItems WITH (NOLOCK) WHERE ThreadID = @ThreadID);
    END
    ELSE IF @ThreadType = 2 BEGIN
        SELECT * FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
        SELECT * FROM bx_QuestionRewards WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
    END
    ELSE IF @ThreadType = 4 BEGIN
        SELECT * FROM bx_Polemizes WITH (NOLOCK) WHERE ThreadID = @ThreadID; 
        SELECT * FROM bx_PolemizeUsers WITH (NOLOCK) WHERE ThreadID = @ThreadID;
    END
END
"
        )]
        #endregion
        public override void SetThreadExtendData(BasicThread thread)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_GetThreadExtendData";

                query.CreateParameter("@ThreadID", thread.ThreadID, SqlDbType.Int);
                query.CreateParameter("@ThreadType", (int)thread.ThreadType, SqlDbType.TinyInt);

                string extendData = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    extendData = GetExtendData(thread, thread.ThreadType, reader, true);
                }

                if (extendData != null)
                {
                    query.Parameters.Clear();
                    UpdateThreadExtendData(extendData, thread.ThreadID, query);
                }
            }
        }

        private string GetExtendData(BasicThread thread, ThreadType threadType, XSqlDataReader reader, bool isFirstRead)
        {
            string extendData = null;
            switch (threadType)
            {
                case ThreadType.Poll:
                    PollThreadV5 poll = thread == null ? new PollThreadV5() : (PollThreadV5)thread;
                    if (isFirstRead || reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            poll.FillPoll(reader);
                        }
                    }

                    if (reader.NextResult())
                    {
                        poll.PollItems = new PollItemCollectionV5(reader);
                    }
                    if (thread == null || poll.VotedUserIDs == null)
                        poll.VotedUserIDs = new List<int>();
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            poll.VotedUserIDs.Add(reader.Get<int>(0));
                        }
                    }
                    extendData = poll.GetExtendData();
                    break;
                case ThreadType.Question:
                    QuestionThread question = thread == null ? new QuestionThread() : (QuestionThread)thread;
                    if (isFirstRead || reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            question.FillQuestion(reader);
                        }
                    }

                    if (thread == null || question.Rewards == null)
                        question.Rewards = new Dictionary<int, int>();
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            question.Rewards.Add(reader.Get<int>("PostID"), reader.Get<int>("Reward"));
                        }
                    }
                    extendData = question.GetExtendData();
                    break;
                case ThreadType.Polemize:
                    PolemizeThreadV5 polemize = thread == null ? new PolemizeThreadV5() : (PolemizeThreadV5)thread;
                    if (isFirstRead || reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            polemize.FillPolemize(reader);
                        }
                    }
                    if (reader.NextResult())
                    {
                        polemize.FillPolemizeUsers(reader);
                    }
                    extendData = polemize.GetExtendData();
                    break;
                default: break;
            }

            if (thread != null)
                thread.ExtendDataIsNull = false;

            return extendData;
        }


        #region 存储过程 bx_CreateQuestion
        [StoredProcedure(Name = "bx_v5_CreateQuestion", Script = @"
CREATE PROCEDURE {name}
    @ForumID                    int,
	@ThreadCatalogID            int,
	@ThreadStatus               tinyint,

	@IconID                     int,
	@Subject                    nvarchar(256),
	@SubjectStyle               nvarchar(300),
	@UserID                     int,
	@NickName                   nvarchar(64),
	@IsLocked                   bit,
	@IsValued                   bit,

	@Content                    ntext,
	@ContentFormat              tinyint,
	@EnableSignature            bit,
	@EnableReplyNotice          bit,
	@IPAddress                  nvarchar(64),

	@Reward                     int,  --本主题的奖励
	@RewardCount                int,  --本主题最多可以奖励给多少帖子
	@AlwaysEyeable              bit,
	@ExpiresDate                datetime,

	@AttachmentIds              varchar(8000),
	@AttachmentFileNames        ntext,
	@AttachmentFileIds          text,
	@AttachmentFileSizes        varchar(8000),
	@AttachmentPrices           varchar(8000),
	@AttachmentFileExtNames     ntext,
	@HistoryAttachmentIDs       varchar(500),

	@ThreadRandNumber           int,
	@UserTotalThreads           int output,
	@UserTotalPosts             int output,
    @ExtendData                 ntext
   ,@TopMarkCount                   int
   ,@TempPostID                 int
   ,@AttachmentType             tinyint
   ,@Words                      nvarchar(400)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	DECLARE @ReturnValue     int;
    DECLARE @ThreadID int, @PostID int

	EXECUTE @ReturnValue = [bx_v5_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		2,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IPAddress,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		@ThreadRandNumber,
		@UserTotalThreads output,
		@UserTotalPosts output,
        @ThreadID output, 
        @PostID output,
        @ExtendData,
        @TopMarkCount,
        @TempPostID,
        @AttachmentType,
        @Words

    SET @ErrorCount = @ErrorCount + @@error;
    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;

	INSERT INTO [bx_Questions]
           ([ThreadID]
           ,[Reward]
           ,[RewardCount]
           ,[AlwaysEyeable]
           ,[ExpiresDate])
     VALUES
           (@ThreadID
           ,@Reward
           ,@RewardCount
           ,@AlwaysEyeable
           ,@ExpiresDate)
    SET @ErrorCount = @ErrorCount + @@error;
	----------增加问题数---------------
		EXEC [bx_DoCreateStat] @ForumID,6, 1
        SET @ErrorCount = @ErrorCount + @@error;
		--------------------------
	--IF @ThreadStatus = 5 --当问题属于未审核时，由于提问的分数已扣，所以返回积分更新缓存（审核过的帖子由bx_CreateThread里返回）
		--SELECT @UserTotalThreads=TotalTopics,
				--@UserTotalPosts=TotalPosts
			--FROM [bx_Users] WHERE UserID = @UserID;	

    SELECT * FROM [bx_Questions] WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END

END
"
        )]
        #endregion
        public override bool CreateQuestion(int questionReward, int questionRewardCount, bool questionIsAlwaysEyeable, DateTime questionExpiresDate
           , int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
           , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
           , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_CreateQuestion";

                string extendData = QuestionThread.GetExtendData(false, questionReward, questionRewardCount, questionIsAlwaysEyeable, questionExpiresDate
                    , 0, 0, 0, new Dictionary<int, int>());

                SetCreateThreadParams(query, forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, postUserID, postNickName, isLocked
                    , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs, extendData, attachType, words);

                query.CreateParameter<int>("@Reward", questionReward, SqlDbType.Int);
                query.CreateParameter<int>("@RewardCount", questionRewardCount, SqlDbType.Int);
                query.CreateParameter<bool>("@AlwaysEyeable", questionIsAlwaysEyeable, SqlDbType.Bit);
                query.CreateParameter<DateTime>("@ExpiresDate", questionExpiresDate, SqlDbType.DateTime);


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    GetThread(reader, attachments, questionExpiresDate, out thread, out post, out attachmentIDs, out fileIDs);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue != -1)
                {
                    totalThreads = (query.Parameters["@UserTotalThreads"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalThreads"].Value));
                    totalPosts = (query.Parameters["@UserTotalPosts"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalPosts"].Value));
                }
                else
                {
                    totalThreads = 0;
                    totalPosts = 0;
                    return false;
                }

                return true;


            }

        }

        #region 存储过程 bx_CreatePolemize
        [StoredProcedure(Name = "bx_v5_CreatePolemize", Script = @"
CREATE PROCEDURE {name}
    @ForumID                    int,
	@ThreadCatalogID            int,
	@ThreadStatus               tinyint,

	@IconID                     int,
	@Subject                    nvarchar(256),
	@SubjectStyle               nvarchar(300),
	@UserID                     int,
	@NickName                   nvarchar(64),
	@IsLocked                   bit,
	@IsValued                   bit,

	@Content                    ntext,
	@ContentFormat              tinyint,
	@EnableSignature            bit,
	@EnableReplyNotice          bit,
	@IPAddress                  nvarchar(64),

	@AgreeViewPoint             ntext,
	@AgainstViewPoint           ntext,
	@ExpiresDate                datetime,

	@AttachmentIds              varchar(8000),
	@AttachmentFileNames        ntext,
	@AttachmentFileIds          text,
	@AttachmentFileSizes        varchar(8000),
	@AttachmentPrices           varchar(8000),
	@AttachmentFileExtNames     ntext,
	@HistoryAttachmentIDs       varchar(500),

	@ThreadRandNumber           int,
	@UserTotalThreads           int output,
	@UserTotalPosts             int output,
    @ExtendData                 ntext
   ,@TopMarkCount               int
   ,@TempPostID                 int
   ,@AttachmentType             tinyint
   ,@Words                      nvarchar(400)
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	DECLARE @ReturnValue     int;
    DECLARE @ThreadID int, @PostID int

	EXECUTE @ReturnValue = [bx_v5_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		4,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IPAddress,

		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		
		@ThreadRandNumber,
		@UserTotalThreads output,
		@UserTotalPosts output,
        @ThreadID output,
        @PostID output,
        @ExtendData,
        @TopMarkCount,
        @TempPostID,
        @AttachmentType,
        @Words
    SET @ErrorCount = @ErrorCount + @@error;

    IF @ReturnValue = -1
        SET @ErrorCount = @ErrorCount + 1;
	--插入辩论信息
    INSERT INTO [bx_Polemizes]
       ([ThreadID]
       ,[AgreeViewPoint]
       ,[AgainstViewPoint]
       ,[ExpiresDate])
     VALUES
       (@ThreadID
       ,@AgreeViewPoint
       ,@AgainstViewPoint
       ,@ExpiresDate);
    SET @ErrorCount = @ErrorCount + @@error;

	----------增加辩论帖统计数---------------
	EXEC [bx_DoCreateStat] @ForumID,30, 1
    SET @ErrorCount = @ErrorCount + @@error;
		--------------------------

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END
END
"
        )]
        #endregion
        public override bool CreatePolemize(string agreeViewPoint, string againstViewPoint, DateTime polemizeExpiresDate
           , int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
           , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
           ,ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out int totalThreads, out int totalPosts, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_CreatePolemize";

                string extendData = PolemizeThreadV5.GetExtendData(againstViewPoint, againstViewPoint, 0, 0, 0, polemizeExpiresDate, null);

                SetCreateThreadParams(query, forumID, threadCatalogID, threadStatus, iconID, subject, subjectStyle, postUserID, postNickName, isLocked
                    , isValued, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs, extendData, attachType, words);

                query.CreateParameter<string>("@AgreeViewPoint", agreeViewPoint, SqlDbType.NText);
                query.CreateParameter<string>("@AgainstViewPoint", againstViewPoint, SqlDbType.NText);
                query.CreateParameter<DateTime>("@ExpiresDate", polemizeExpiresDate, SqlDbType.DateTime);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    GetThread(reader, attachments, polemizeExpiresDate, out thread, out post, out attachmentIDs, out fileIDs);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue != -1)
                {
                    totalThreads = (query.Parameters["@UserTotalThreads"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalThreads"].Value));
                    totalPosts = (query.Parameters["@UserTotalPosts"].Value == DBNull.Value ? 0 : Convert.ToInt32(query.Parameters["@UserTotalPosts"].Value));
                }
                else
                {
                    totalThreads = 0;
                    totalPosts = 0;
                    return false;
                }

                return true;


            }

        }

        #region 存储过程 bx_UpdateThread
        [StoredProcedure(Name = "bx_v5_UpdateThread", Script = @"
CREATE PROCEDURE {name}
	@ThreadID                       int,
	@ThreadCatalogID                int,
	@IconID                         int,
	@Subject                        nvarchar(256),
	@PostID                         int,
	@Content                        ntext,
	@ContentFormat                  tinyint,
	@EnableSignature                bit,
	@EnableReplyNotice              bit,
	@IsApproved                     bit,
	@LastEditorID                   int,
	@LastEditor                     nvarchar(64),
	@Price                          int,
	@AttachmentIds                  varchar(8000),
	@AttachmentFileNames            ntext,
	@AttachmentFileIds              text,
	@AttachmentFileSizes            varchar(8000),
	@AttachmentPrices               varchar(8000),
	@AttachmentFileExtNames         ntext,
	@HistoryAttachmentIDs           varchar(500)
   ,@TopMarkCount                   int
   ,@AttachmentType                 tinyint
   ,@Words                          nvarchar(400)
AS

SET NOCOUNT ON


	DECLARE @OldThreadStatus tinyint, @NewThreadStatus tinyint,@OldWords nvarchar(400);
	SELECT @OldThreadStatus = ThreadStatus,@OldWords = Words FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID;
    IF @IsApproved = 1 AND @OldThreadStatus = 5
        SET @NewThreadStatus = 1;
    ELSE IF @IsApproved = 0 AND @OldThreadStatus < 4
        SET @NewThreadStatus = 5;
    ELSE
        SET @NewThreadStatus = @OldThreadStatus;
        

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

	UPDATE [bx_Threads] SET
		[ThreadCatalogID] = @ThreadCatalogID,
		[IconID] = @IconID,
		[Subject] = @Subject,
		[Price] = @Price,
		[UpdateDate] = CASE WHEN TotalReplies = 0 THEN getdate() ELSE [UpdateDate] END,
		[ThreadStatus] = @NewThreadStatus,
		[KeywordVersion] = '',
        [AttachmentType] = @AttachmentType,
        [Words] = @Words
	WHERE
		[ThreadID] = @ThreadID;
    SET @ErrorCount = @ErrorCount + @@error;

    IF @Words <> @OldWords BEGIN
        DELETE bx_ThreadWords WHERE ThreadID = @ThreadID;
        SET @ErrorCount = @ErrorCount + @@error;
        IF @Words <> '' BEGIN
            INSERT INTO bx_ThreadWords(ThreadID,Word) SELECT @ThreadID,T.item FROM bx_GetStringTable_nvarchar(@Words, N',') T;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END

    SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID;

Execute bx_v5_UpdatePost
		@PostID,
		@IconID,
		@Subject,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IsApproved,
		@LastEditorID,
		@LastEditor,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
        0,
        @TopMarkCount

    SET @ErrorCount = @ErrorCount + @@error;

    IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END
"
            )]
        #endregion
        public override bool UpdateThread(int threadID, int postID, bool isApproved, int threadCatalogID, int iconID
            , string subject, int price, int lastEditorID, string lastEditorName, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , ThreadAttachType attachType, string words, out BasicThread thread, out PostV5 post, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_UpdateThread";

                SetUpdatePostParams(query, postID, iconID, subject, lastEditorID, lastEditorName
                    , content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, isApproved, attachments, historyAttachmentIDs);

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadCatalogID", threadCatalogID, SqlDbType.Int);
                query.CreateParameter<int>("@Price", price, SqlDbType.Int);
                query.CreateParameter<int>("@AttachmentType", (int)attachType, SqlDbType.TinyInt);
                query.CreateParameter<string>("@Words", words, SqlDbType.NVarChar, 400);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    GetThread(reader, attachments, null, out thread, out post, out attachmentIDs, out fileIDs);
                }
            }

            return true;
        }


        private void SetCreateThreadParams(SqlQuery query, int forumID, int threadCatalogID, ThreadStatus threadStatus, int iconID
            , string subject, string subjectStyle, int postUserID, string postNickName, bool isLocked, bool isValued, string content, bool enableHtml
            , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, string ipAddress, AttachmentCollection attachments
            , IEnumerable<int> historyAttachmentIDs, string extendData, ThreadAttachType attachType, string words)
        {
            query.CreateParameter("@ThreadCatalogID", threadCatalogID, SqlDbType.Int);
            query.CreateParameter("@ThreadStatus", (int)threadStatus, SqlDbType.TinyInt);
            //query.CreateParameter("@ThreadType", (int)threadType, SqlDbType.TinyInt);

            query.CreateParameter("@SubjectStyle", subjectStyle, SqlDbType.NVarChar, 300);

            //query.CreateParameter("@Price", price, SqlDbType.Int);

            query.CreateParameter("@IsLocked", isLocked, SqlDbType.Bit);
            query.CreateParameter("@IsValued", isValued, SqlDbType.Bit);

            query.CreateParameter("@ExtendData", extendData, SqlDbType.NText);

            query.CreateParameter<int>("@ThreadRandNumber", GetSortNumber("thread"), SqlDbType.Int);


            query.CreateParameter<int>("@UserTotalThreads", SqlDbType.Int, ParameterDirection.Output);

            query.CreateParameter<int>("@TempPostID", GetTempPostID(), SqlDbType.Int);

            query.CreateParameter<int>("@AttachmentType", (int)attachType, SqlDbType.TinyInt);

            query.CreateParameter<string>("@Words", words, SqlDbType.NVarChar, 400);

            SetCreatePostParams(query, forumID, iconID, subject, postUserID, postNickName, content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice
                , ipAddress, attachments, historyAttachmentIDs);

        }

        private void SetCreatePostParams(SqlQuery query, int forumID, int iconID, string subject, int postUserID
            , string postNickName, string content, bool enableHtml, bool enableMaxCode3, bool enableSignature, bool enableReplyNotice
            , string ipAddress, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs)
        {

            query.CreateParameter("@IconID", iconID, SqlDbType.Int);
            query.CreateParameter("@Subject", subject, SqlDbType.NVarChar, 256);
            query.CreateParameter("@Content", content, SqlDbType.NText);


            int contentFormat = 0;
            if (enableHtml)
                contentFormat = contentFormat | (int)ContentFormat.EnableHTML;
            if (enableMaxCode3)
                contentFormat = contentFormat | (int)ContentFormat.EnableMaxCode3;

            contentFormat = contentFormat | (int)ContentFormat.IsV5_0;

            query.CreateParameter("@ContentFormat", contentFormat, SqlDbType.TinyInt);


            query.CreateParameter("@EnableSignature", enableSignature, SqlDbType.Bit);
            query.CreateParameter("@EnableReplyNotice", enableReplyNotice, SqlDbType.Bit);
            query.CreateParameter("@ForumID", forumID, SqlDbType.Int);

            query.CreateParameter("@UserID", postUserID, SqlDbType.Int);
            query.CreateParameter("@NickName", postNickName, SqlDbType.NVarChar, 64);

            query.CreateParameter("@IPAddress", ipAddress, SqlDbType.NVarChar, 64);

            query.CreateParameter<int>("@UserTotalPosts", SqlDbType.Int, ParameterDirection.Output);

            query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

            SetAttachmentParams(query, attachments, historyAttachmentIDs);
            SetTopMarkCountParam(query);



        }

        private void SetUpdatePostParams(SqlQuery query, int postID, int iconID, string subject, int lastEditorID
            , string lastEditor, string content, bool enableHtml, bool enableMaxCode3, bool enableSignature, bool enableReplyNotice
            , bool isApproved, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs)
        {

            query.CreateParameter("@PostID", postID, SqlDbType.Int);
            query.CreateParameter("@IconID", iconID, SqlDbType.Int);
            query.CreateParameter("@Subject", subject, SqlDbType.NVarChar, 256);
            query.CreateParameter("@Content", content, SqlDbType.NText);


            int contentFormat = 0;
            if (enableHtml)
                contentFormat = contentFormat | (int)ContentFormat.EnableHTML;
            if (enableMaxCode3)
                contentFormat = contentFormat | (int)ContentFormat.EnableMaxCode3;

            contentFormat = contentFormat | (int)ContentFormat.IsV5_0;

            query.CreateParameter("@ContentFormat", contentFormat, SqlDbType.TinyInt);


            query.CreateParameter("@EnableSignature", enableSignature, SqlDbType.Bit);
            query.CreateParameter("@EnableReplyNotice", enableReplyNotice, SqlDbType.Bit);
            query.CreateParameter("@IsApproved", isApproved, SqlDbType.Bit);

            query.CreateParameter("@LastEditorID", lastEditorID, SqlDbType.Int);
            query.CreateParameter("@LastEditor", lastEditor, SqlDbType.NVarChar, 64);

            //query.CreateParameter("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);

            SetAttachmentParams(query, attachments, historyAttachmentIDs);
            SetTopMarkCountParam(query);

        }

        private void SetAttachmentParams(SqlQuery query, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs)
        {
            query.CreateParameter("@AttachmentIds", attachments.BuildIds(), SqlDbType.VarChar, 8000);
            query.CreateParameter("@AttachmentFileNames", attachments.BuildFileNames(), SqlDbType.NText);
            query.CreateParameter("@AttachmentFileIds", attachments.BuildFileIds(), SqlDbType.Text);
            query.CreateParameter("@AttachmentFileSizes", attachments.BuildFileSizes(), SqlDbType.VarChar, 8000);
            query.CreateParameter("@AttachmentPrices", attachments.BuildPrices(), SqlDbType.VarChar, 8000);
            query.CreateParameter("@AttachmentFileExtNames", attachments.BuildFileExtNames(), SqlDbType.NText);
            query.CreateParameter("@HistoryAttachmentIDs", StringUtil.Join(historyAttachmentIDs), SqlDbType.VarChar, 500);
        }

        #region 存储过程 bx_CreatePost
        [StoredProcedure(Name = "bx_v5_CreatePost", Script = @"
CREATE PROCEDURE {name}
	@ParentID                       int,
	@ThreadID                       int,
	@PostType                       tinyint,
	@IconID                         int,
	@Subject                        nvarchar(256),
	@Content                        ntext,
	@ContentFormat                  tinyint,
	@EnableSignature                bit,
	@EnableReplyNotice              bit,
	@ForumID                        int,
	@UserID                         int,
	@NickName                       nvarchar(64),
	@IPAddress                      nvarchar(32),

	@AttachmentIds                  varchar(8000),
	@AttachmentFileNames            ntext,
	@AttachmentFileIds              text,
	@AttachmentFileSizes            varchar(8000),
	@AttachmentPrices               varchar(8000),
	@AttachmentFileExtNames         ntext,
	@HistoryAttachmentIDs           varchar(500),

	@IsCreatePost                   bit = 1,

	@IsApproved                     bit,
	@PostRandNumber                 tinyint,
	@UserTotalPosts                 int output,
	@UpdateSortOrder                bit,
    @GetExtendedInfo                bit,
    @GetThreadEnableReplyNotice     bit,
    @PostID                         int output
   ,@TopMarkCount                   int
   ,@GetPost                        bit
   ,@GetThread                      bit
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Type tinyint;
	IF @IsApproved = 1 --BEGIN
		SET @Type=1
	ELSE
		SET @Type=5

    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN CreatePost


    DECLARE @RealForumID int,@PostedCount int;
    
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
    BEGIN TRAN UpdatePostedCount

    SELECT @RealForumID = ForumID,@PostedCount = PostedCount FROM bx_Threads with(xlock,rowlock) WHERE ThreadID = @ThreadID;
    IF @PostedCount IS NOT NULL
        SET @PostedCount = @PostedCount + 1;
    

	DECLARE @TempSortOrder BIGINT,@PostDate datetime;

	SET @PostDate = getdate();
	EXEC [bx_GetSortOrder] @Type, @PostRandNumber, @PostDate, 0, @TempSortOrder OUTPUT 
    

    IF @IsCreatePost = 0
        SET @PostedCount = 1;

	INSERT INTO [bx_Posts]
           ([ParentID]
           ,[ForumID]
           ,[ThreadID]
           ,[PostType]
           ,[IconID]
           ,[Subject]
           ,[Content]
           ,[ContentFormat]
           ,[EnableSignature]
           ,[EnableReplyNotice]
           ,[UserID]
           ,[NickName]
           ,[IPAddress]
           ,[HistoryAttachmentIDs]
           ,[SortOrder]
           ,[FloorNumber])
     VALUES
           (@ParentID
           ,@RealForumID
           ,@ThreadID
           ,@PostType
           ,@IconID
           ,@Subject
           ,@Content
           ,@ContentFormat
           ,@EnableSignature
           ,@EnableReplyNotice
           ,@UserID
           ,@NickName
           ,@IPAddress
		   ,@HistoryAttachmentIDs
           ,@TempSortOrder
           ,@PostedCount)

    SET @ErrorCount = @ErrorCount + @@error;

	SELECT @PostID = @@IDENTITY;

    IF @PostType = 1 --主题内容
        UPDATE bx_Threads SET LastPostID = @PostID,ContentID = @PostID, PostedCount = @PostedCount WHERE ThreadID = @ThreadID;
    ELSE IF @IsApproved = 1
        UPDATE bx_Threads SET LastPostID = @PostID, PostedCount = @PostedCount WHERE ThreadID = @ThreadID;
    ELSE 
        UPDATE bx_Threads SET PostedCount = @PostedCount WHERE ThreadID = @ThreadID;
    
    SET @ErrorCount = @ErrorCount + @@error;

    COMMIT TRAN UpdatePostedCount
    
    IF @GetThread = 1
        SELECT " + ThreadFields + @" FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = @ThreadID; 

    IF @GetThreadEnableReplyNotice = 1
        SELECT EnableReplyNotice FROM bx_Posts WITH (NOLOCK) WHERE ThreadID = @ThreadID AND PostType = 1;--主题内容

	--//添加附件列表
	IF DATALENGTH(@AttachmentIds) > 0 BEGIN

		DECLARE @AttachmentTable table(TempID int identity(1,1), AttachmentID int, FileName nvarchar(256), FileExtName nvarchar(10), FileID varchar(50), FileSize bigint, Price int);

		--DECLARE @NewAttachCount int
		
		INSERT INTO @AttachmentTable (AttachmentID) SELECT item FROM bx_GetIntTable(@AttachmentIds, '|');
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			[FileName] = T.item
			FROM bx_GetStringTable_ntext(@AttachmentFileNames, N'|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			FileID = T.item
			FROM bx_GetStringTable_text(@AttachmentFileIds, '|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			FileSize = T.item
			FROM bx_GetBigIntTable(@AttachmentFileSizes, '|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;

		UPDATE @AttachmentTable SET
			Price = T.item
			FROM bx_GetIntTable(@AttachmentPrices, '|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;
			
		UPDATE @AttachmentTable SET
			FileExtName = T.item
			FROM bx_GetStringTable_ntext(@AttachmentFileExtNames, N'|') T
			WHERE TempID = T.id;
        SET @ErrorCount = @ErrorCount + @@error;


		INSERT INTO bx_Attachments(
			PostID,
			FileID,
			FileName,
			FileSize,
			FileType,
			Price,
			UserID
			) SELECT 
			@PostID,
			T.FileID,
			T.FileName,
			T.FileSize,
			T.FileExtName,
			T.Price,
			@UserID
			FROM @AttachmentTable T
			WHERE T.AttachmentID < 0 AND T.FileID IS NOT NULL;
        SET @ErrorCount = @ErrorCount + @@error; 
		
		SELECT [AttachmentID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID ORDER BY [AttachmentID] DESC;
		
		INSERT INTO bx_Attachments(
			PostID,
			FileID,
			FileName,
			FileSize,
			FileType,
			Price,
			UserID
			) SELECT 
			@PostID,
			T.FileID,
			T.FileName,
			T.FileSize,
			T.FileExtName,
			T.Price,
			@UserID
			FROM @AttachmentTable T
			WHERE T.AttachmentID = 0 AND T.FileID IS NOT NULL;
        SET @ErrorCount = @ErrorCount + @@error; 
		
		SELECT [AttachmentID],[FileID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID;
			
	END




    DECLARE @Today DateTime,@Monday DateTime;
    DECLARE @WeekPosts int,@DayPosts int,@MonthPosts int,@LastPostDate DateTime;

    IF @UserID<>0 BEGIN
	    SET @Today = CONVERT(varchar(12) , getdate(), 102);
			
	    DECLARE @m int;
	    SELECT @m = DATEPART(weekday, @Today);
	    IF @m = 1
		    SELECT @m = 8;
	    SELECT @Monday = DATEADD(day, 2-@m, @Today);


        SELECT @WeekPosts = WeekPosts,@DayPosts = DayPosts,@MonthPosts = MonthPosts,@LastPostDate = LastPostDate FROM bx_Users WHERE UserID = @UserID;
    END

	--添加附件结束
	--@ForumID = -2 表示该帖子未审核   IsCreatePost 1 表示发回复 0表示发主题 
    IF @IsApproved=1 AND @IsCreatePost = 1 BEGIN
    
		--DECLARE @TempForumID int
	
		----------增加回复数---------------
		EXEC [bx_DoCreateStat] @ForumID,4, 1
        SET @ErrorCount = @ErrorCount + @@error;
		
		--------------------------
	    
		DECLARE @SortOrder bigint
		
		IF @UpdateSortOrder = 1 BEGIN
			-- DECLARE @ThreadStatus tinyint;
            -- SELECT @ThreadStatus = [ThreadStatus] FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID
			
            EXEC [bx_GetSortOrder] 1, @PostID, @PostDate, 1, @SortOrder OUTPUT 
            /*
			IF @SortOrder < 2000000000000000--500000000000000
				EXEC [bx_GetSortOrder] 1, @PostID, @PostDate, @SortOrder OUTPUT 
			ELSE IF @SortOrder < 3000000000000000--800000000000000
				EXEC [bx_GetSortOrder] 2, @PostID, @PostDate, @SortOrder OUTPUT 
			ELSE
				EXEC [bx_GetSortOrder] 3, @PostID, @PostDate, @SortOrder OUTPUT 
            */		

		END

        UPDATE [bx_Forums]
            SET [TotalPosts] = [TotalPosts] + 1,
				[TodayPosts] = [TodayPosts] + 1,
				[LastThreadID] = @ThreadID
            WHERE [ForumID] = @ForumID;
        SET @ErrorCount = @ErrorCount + @@error;
		

        BEGIN TRAN UpdateThread
        -------
	    IF @UpdateSortOrder = 1
			UPDATE [bx_Threads] with(updlock,rowlock) 
			   SET [TotalReplies] = [TotalReplies] + 1,
					[LastPostUserID] = @UserID,
					[LastPostNickName] = @NickName,
					[UpdateDate] = getdate(),
					[SortOrder] = @SortOrder
			WHERE ThreadID=@ThreadID;
		ELSE
			UPDATE [bx_Threads] with(updlock,rowlock) 
			   SET [TotalReplies] = [TotalReplies] + 1,
					[LastPostUserID] = @UserID,
					[LastPostNickName] = @NickName,
					[UpdateDate] = getdate()
			WHERE ThreadID=@ThreadID;
        SET @ErrorCount = @ErrorCount + @@error;

        COMMIT TRAN UpdateThread
        -------
		IF @UserID<>0 BEGIN

            IF @LastPostDate >= @Monday
                SET @WeekPosts = @WeekPosts + 1;
            ELSE
                SET @WeekPosts = 1;

            IF @LastPostDate >= @Today
                SET @DayPosts = @DayPosts + 1;
            ELSE
                SET @DayPosts = 1;

            IF DATEPART(year, GETDATE()) = DATEPART(year, GETDATE()) AND DATEPART(month, GETDATE()) = DATEPART(month,@LastPostDate)
                SET @MonthPosts = @MonthPosts + 1;
            ELSE
                SET @MonthPosts = 1;

			UPDATE [bx_Users]
			   SET	[LastPostDate] = getdate(),
					[TotalPosts] = [TotalPosts] + 1
                    ,[WeekPosts] = @WeekPosts
                    ,[MonthPosts] = @MonthPosts
                    ,[DayPosts] = @DayPosts
			 WHERE UserID = @UserID;
            SET @ErrorCount = @ErrorCount + @@error;
			 
		END

    END
    ELSE IF @IsCreatePost = 1 AND @UserID<>0 BEGIN
        DECLARE @MustUpdate bit;
        IF @LastPostDate < @Monday BEGIN
            SET @WeekPosts = 0;
            SET @MustUpdate = 1;
        END
        IF @LastPostDate < @Today BEGIN
            SET @DayPosts = 0;
            SET @MustUpdate = 1;
        END
        IF DATEPART(year, GETDATE()) <> DATEPART(year,@LastPostDate) OR DATEPART(month, GETDATE()) <> DATEPART(month,@LastPostDate) BEGIN
            SET @MonthPosts = 1;
        END
        IF @MustUpdate = 1 BEGIN
            UPDATE [bx_Users]
		        SET [WeekPosts] = @WeekPosts
                    ,[DayPosts] = @DayPosts
                    ,[MonthPosts] = @MonthPosts
            WHERE UserID = @UserID;
            SET @ErrorCount = @ErrorCount + @@error;
        END
    END

	IF @UserID<>0 BEGIN
		-- 添加辩论用户 --
		IF @PostType = 2
			EXEC [bx_v5_AddPolemizeUser] @ThreadID,@UserID,0
		ELSE IF @PostType = 3
			EXEC [bx_v5_AddPolemizeUser] @ThreadID,@UserID,1
		ELSE IF @PostType = 4
			EXEC [bx_v5_AddPolemizeUser] @ThreadID,@UserID,2
		---------------------
        SET @ErrorCount = @ErrorCount + @@error;

	END	

    IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN CreatePost
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN CreatePost
    END

    IF @UserID<>0 BEGIN
		
		SELECT 
			@UserTotalPosts=ISNULL(TotalPosts,0)
			FROM [bx_Users] WHERE UserID = @UserID;
			
	END 
	ELSE BEGIN
			SET  @UserTotalPosts = 0;
	END
    
    IF @GetPost = 1 BEGIN
        IF @GetExtendedInfo = 0
            SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
        ELSE BEGIN
            DECLARE @SQLString nvarchar(4000);
            DECLARE @PostFieldsString nvarchar(4000);
            SET @SQLString = 'SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = '+ CAST(@PostID AS varchar(16));  --'SELECT '+ CAST(@PostID AS varchar(16)) + ',''' + @HistoryAttachmentIDs + '''';
            " + PostFields + @"
        END
    END

    
    RETURN 0;
    
END
"
        )]
        #endregion
        public override bool CreatePost(int threadID, bool getPost, bool isApproved, PostType postType, int iconID, string subject, string content
            , bool enableHtml, bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, int forumID, int postUserID, string userName
            , string ipAddress, int parentID, bool updateSortOrder, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
            , BasicThread thread
            , out PostV5 post, out int totalPosts, out List<int> AttachmentIDs, out Dictionary<string, int> fileIDs, out bool threadEnableReplyNotice)
        {
            AttachmentIDs = new List<int>();
            fileIDs = new Dictionary<string, int>();
            threadEnableReplyNotice = true;

            post = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_CreatePost";

                SetCreatePostParams(query, forumID, iconID, subject, postUserID, userName, content, enableHtml, enableMaxCode3
                    , enableSignature, enableReplyNotice, ipAddress, attachments, historyAttachmentIDs);

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@PostType", (int)postType, SqlDbType.TinyInt);

                query.CreateParameter<bool>("@IsApproved", isApproved, SqlDbType.Bit);
                query.CreateParameter<int>("@PostRandNumber", GetSortNumber("post"), SqlDbType.TinyInt);

                query.CreateParameter<int>("@ParentID", parentID, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateSortOrder", updateSortOrder, SqlDbType.Bit);
                query.CreateParameter("@GetExtendedInfo", true, SqlDbType.Bit);
                query.CreateParameter("@GetThreadEnableReplyNotice", true, SqlDbType.Bit);
                query.CreateParameter<int>("@PostID", SqlDbType.Int, ParameterDirection.Output);
                query.CreateParameter<bool>("@GetPost", getPost, SqlDbType.Bit);
                query.CreateParameter<bool>("@GetThread", false, SqlDbType.Bit);

                string extendData = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threadEnableReplyNotice = reader.Get<bool>("EnableReplyNotice");
                    }

                    if (attachments.Count > 0)
                    {
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                AttachmentIDs.Add(reader.Get<int>(0));
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    string fileID = reader.Get<string>("fileID");
                                    if (fileIDs.ContainsKey(fileID) == false)
                                        fileIDs.Add(fileID, reader.Get<int>("attachmentID"));
                                }
                            }
                        }
                    }

                    if (getPost)
                    {
                        PostCollectionV5 posts = GetPosts(reader, false);
                        if (posts.Count > 0)
                            post = posts[0];
                        else
                            post = null;
                    }

                    extendData = GetExtendData(thread, thread.ThreadType, reader, false);

                }

                if (getPost == false)
                {
                    post = new PostV5();
                    post.PostID = (int)query.Parameters["@PostID"].Value;
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;
                if (returnValue != -1)
                {
                    totalPosts = Convert.ToInt32(query.Parameters["@UserTotalPosts"].Value);
                    if (extendData != null)
                    {
                        UpdateThreadExtendData(extendData, threadID, query);
                    }
                }
                else
                {
                    totalPosts = 0;
                    return false;
                }

                return true;
            }
        }

        #region 存储过程 bx_UpdatePost
        [StoredProcedure(Name = "bx_v5_UpdatePost", Script = @"
CREATE PROCEDURE {name}
	@PostID                         int,
	@IconID                         int,
	@Subject                        nvarchar(256),
	@Content                        ntext,
	@ContentFormat                  tinyint,
	@EnableSignature                bit,
	@EnableReplyNotice              bit,
	@IsApproved                     bit,
	@LastEditorID                   int,
	@LastEditor                     nvarchar(65),
	
	@AttachmentIds                  varchar(8000),
	@AttachmentFileNames            ntext,
	@AttachmentFileIds              text,
	@AttachmentFileSizes            varchar(8000),
	@AttachmentPrices               varchar(8000),
	@AttachmentFileExtNames         ntext,
	@HistoryAttachmentIDs           varchar(500),
    @GetExtendedInfo                bit
    ,@TopMarkCount                  int
AS
BEGIN
    SET NOCOUNT ON


    DECLARE @UserID int,@OldSortOrder bigint;
    SELECT @UserID=UserID,@OldSortOrder=SortOrder FROM  bx_Posts WITH (NOLOCK) WHERE PostID=@PostID;---Modify by 帅帅


    DECLARE @ErrorCount int;
    SET @ErrorCount = 0;

    BEGIN TRAN

    DECLARE @SortOrder bigint;

    IF @IsApproved=1
	    EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @SortOrder OUTPUT;
    ELSE
	    EXEC [bx_UpdateSortOrder] 5, @OldSortOrder, @SortOrder OUTPUT;

	UPDATE [bx_Posts] SET
		[IconID] = @IconID,
		[Subject] = @Subject,
		[Content] = @Content,
		[ContentFormat] = @ContentFormat,
		[EnableSignature] = @EnableSignature,
		[EnableReplyNotice] = @EnableReplyNotice,
		[LastEditorID]=@LastEditorID,
		[LastEditor]=@LastEditor,
		[UpdateDate] = getdate(),
		[HistoryAttachmentIDs] = @HistoryAttachmentIDs,
		[SortOrder] = @SortOrder,
		[KeywordVersion] = ''
	WHERE
		[PostID] = @PostID
		
    SET @ErrorCount = @ErrorCount + @@error;
	
		IF DATALENGTH(@AttachmentIds) > 0 BEGIN
			DECLARE @AttachmentTable table(TempID int identity(1,1), AttachmentID int, FileName nvarchar(256), FileExtName varchar(10), FileID varchar(50), FileSize bigint, Price int);
		
			INSERT INTO @AttachmentTable (AttachmentID) SELECT item FROM bx_GetIntTable(@AttachmentIds, '|');

			UPDATE @AttachmentTable SET
				[FileName] = T.item
				FROM bx_GetStringTable_ntext(@AttachmentFileNames, N'|') T
				WHERE TempID = T.id;

			UPDATE @AttachmentTable SET
				FileID = T.item
				FROM bx_GetStringTable_text(@AttachmentFileIds, '|') T
				WHERE TempID = T.id;

			UPDATE @AttachmentTable SET
				FileSize = T.item
				FROM bx_GetBigIntTable(@AttachmentFileSizes, '|') T
				WHERE TempID = T.id;

			UPDATE @AttachmentTable SET
				Price = T.item
				FROM bx_GetIntTable(@AttachmentPrices, '|') T
				WHERE TempID = T.id;
				
			UPDATE @AttachmentTable SET
				FileExtName = T.item
				FROM bx_GetStringTable_ntext(@AttachmentFileExtNames, N'|') T
				WHERE TempID = T.id;


			DECLARE @NewAttchmentCount int;

			DECLARE @AttachmentIDsTable table(AttachmentID int);
			INSERT INTO @AttachmentIDsTable SELECT [AttachmentID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID;

			INSERT INTO bx_Attachments(
				PostID,
				FileID,
				FileName,
				FileSize,
				FileType,
				Price,
				UserID
				) SELECT 
				@PostID,
				T.FileID,
				T.FileName,
				T.FileSize,
				T.FileExtName,
				T.Price,
				@UserID
				FROM @AttachmentTable T
				WHERE T.AttachmentID < 0;
            SET @ErrorCount = @ErrorCount + @@error;
					
			SELECT @NewAttchmentCount = @@ROWCOUNT;
			
			EXEC('SELECT TOP ' + @NewAttchmentCount + ' [AttachmentID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID = '+@PostID + ' ORDER BY [AttachmentID] DESC');
				
			DELETE bx_Attachments WHERE PostID=@PostID AND 
				AttachmentID IN(SELECT [AttachmentID] FROM @AttachmentIDsTable) 
				AND AttachmentID NOT IN(SELECT AttachmentID FROM @AttachmentTable);
            SET @ErrorCount = @ErrorCount + @@error;

			UPDATE bx_Attachments SET
				FileName = T.FileName,
				Price = T.Price
				FROM @AttachmentTable T
				WHERE T.AttachmentID > 0 AND T.AttachmentID = bx_Attachments.AttachmentID AND T.FileName<>'' AND T.FileName is not null;
            SET @ErrorCount = @ErrorCount + @@error;

			UPDATE bx_Attachments SET
				Price = T.Price
				FROM @AttachmentTable T
				WHERE T.AttachmentID > 0 AND T.AttachmentID = bx_Attachments.AttachmentID AND (T.FileName = '' OR T.FileName is not null);
		    SET @ErrorCount = @ErrorCount + @@error;

			INSERT INTO bx_Attachments(
				PostID,
				FileID,
				FileName,
				FileSize,
				FileType,
				Price,
				UserID
				) SELECT 
				@PostID,
				T.FileID,
				T.FileName,
				T.FileSize,
				T.FileExtName,
				T.Price,
				@UserID
				FROM @AttachmentTable T
				WHERE T.AttachmentID = 0;
			SET @ErrorCount = @ErrorCount + @@error;

			SELECT [AttachmentID],[FileID] FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID=@PostID;

		END 
		ELSE BEGIN
			DELETE bx_Attachments WHERE PostID = @PostID;
            SET @ErrorCount = @ErrorCount + @@error;
		END

    IF @GetExtendedInfo = 0
        SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;
    ELSE BEGIN
        DECLARE @SQLString nvarchar(4000);
        DECLARE @PostFieldsString nvarchar(4000);
        SET @SQLString = 'SELECT " + PostColumns + @" FROM bx_Posts WITH (NOLOCK) WHERE PostID = '+ CAST(@PostID AS varchar(16));  --'SELECT '+ CAST(@PostID AS varchar(16)) + ',''' + @HistoryAttachmentIDs + '''';
        " + PostFields + @"
    END

	IF @ErrorCount > 0 BEGIN
        ROLLBACK TRAN
        RETURN -1;
    END
    ELSE BEGIN
        COMMIT TRAN
        RETURN 0;
    END
END
"
            )]
        #endregion
        public override bool UpdatePost(int postID, bool getExtendedInfo, bool isApproved, int iconID
           , string subject, int lastEditorID, string lastEditorName, string content, bool enableHtml
           , bool enableMaxCode3, bool enableSignature, bool enableReplyNotice, AttachmentCollection attachments, IEnumerable<int> historyAttachmentIDs
           , out PostV5 post, out List<int> attachmentIDs, out Dictionary<string, int> fileIDs)
        {
            post = null;
            attachmentIDs = new List<int>();
            fileIDs = new Dictionary<string, int>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandType = CommandType.StoredProcedure;
                query.CommandText = "bx_v5_UpdatePost";

                SetUpdatePostParams(query, postID, iconID, subject, lastEditorID, lastEditorName
                    , content, enableHtml, enableMaxCode3, enableSignature, enableReplyNotice, isApproved, attachments, historyAttachmentIDs);

                query.CreateParameter("@GetExtendedInfo", getExtendedInfo, SqlDbType.Bit);
                //SetTopMarkCountParam(query);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (attachments.Count > 0)
                    {
                        while (reader.Read())
                        {
                            attachmentIDs.Add(reader.Get<int>(0));
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                string fileID = reader.Get<string>("fileID");
                                if (fileIDs.ContainsKey(fileID) == false)
                                    fileIDs.Add(fileID, reader.Get<int>("attachmentID"));
                            }
                        }
                    }

                    PostCollectionV5 posts = GetPosts(reader, attachments.Count == 0);

                    if (posts.Count > 0)
                        post = posts[0];
                    else
                        post = null;
                }
            }

            return true;
        }


        #region 存储过程 bx_UpdatePostContent
        [StoredProcedure(Name = "bx_UpdatePostContent", Script = @"
CREATE PROCEDURE {name}
     @PostID   int,
     @Content  ntext
AS
BEGIN
	SET NOCOUNT ON;
    UPDATE [bx_Posts] SET [Content] = @Content WHERE [PostID] = @PostID;
END
"
            )]
        #endregion
        public override bool UpdatePostContent(int postID, string content)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);
                query.CreateParameter<string>("@Content", content, SqlDbType.NText);

                query.CommandText = "bx_UpdatePostContent";
                query.CommandType = CommandType.StoredProcedure;

                query.ExecuteNonQuery();
            }
            return true;
        }

        #region 存储过程 bx_RepairTotalReplyCount
        [StoredProcedure(Name = "bx_RepairTotalReplyCount", Script = @"
CREATE PROCEDURE {name}
     @ThreadID   int
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @Total int;
    SELECT @Total = COUNT(*)-1 FROM bx_Posts WITH(NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000;
    UPDATE [bx_Threads] SET [TotalReplies] = @Total WHERE [ThreadID] = @ThreadID; 
    SELECT @Total;
END
"
            )]
        #endregion
        public override int RepairTotalReplyCount(int threadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                query.CommandText = "bx_RepairTotalReplyCount";
                query.CommandType = CommandType.StoredProcedure;

                return query.ExecuteScalar<int>();
            }
        }
        #endregion


        #region 存储过程 bx_v5_GetPostUserIDsFormThreads
        [StoredProcedure(Name = "bx_v5_GetPostUserIDsFormThreads", Script = @"
CREATE PROCEDURE {name}
	@ThreadIDs varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;

	EXEC('SELECT DISTINCT UserID FROM bx_Posts WITH (NOLOCK) WHERE SortOrder<4000000000000000 AND ThreadID in('+@ThreadIDs+')');
END
"
            )]
        #endregion
        public override List<int> GetPostUserIDsFormThreads(IEnumerable<int> threadIDs)
        {
            List<int> userIDs = new List<int>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetPostUserIDsFormThreads";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@ThreadIDs", StringUtil.Join(threadIDs), SqlDbType.VarChar, 8000);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userIDs.Add(reader.Get<int>(0));
                    }
                }
            }

            return userIDs;
        }

        #region 附件


        public override AttachmentCollection GetAttachments(int userID, IEnumerable<int> attachmentIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM bx_Attachments WITH (NOLOCK) WHERE [UserID] = @UserID AND [AttachmentID] IN(@AttachmentIDs);";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateInParameter<int>("@AttachmentIDs", attachmentIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AttachmentCollection(reader);
                }
            }
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_v5_GetUserTodayAttachments", Script = @"
CREATE PROCEDURE {name}
    @UserID      int,
    @DateTime    datetime
AS BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @DateTime;
END
")]
        #endregion
        public override AttachmentCollection GetUserTodayAttachments(int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetUserTodayAttachments";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@DateTime", new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day, 0, 0, 0), SqlDbType.DateTime);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AttachmentCollection(reader);
                }
            }
        }

        #region 存储过程 bx_v5_GetUserTodayAttachmentInfo
        [StoredProcedure(Name = "bx_v5_GetUserTodayAttachmentInfo", Script = @"
CREATE PROCEDURE {name}
    @UserID         int,
    @DateTime       datetime,
    @ExcludePostID  int
AS BEGIN
    SET NOCOUNT ON;
    
    IF @ExcludePostID IS NULL
       SELECT Count(*) AS TotalCount,ISNULL(SUM(FileSize),0) AS TotalFileSize FROM [bx_Attachments] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @DateTime;
    ELSE
       SELECT Count(*) AS TotalCount,ISNULL(SUM(FileSize),0) AS TotalFileSize FROM [bx_Attachments] WITH (NOLOCK) WHERE [UserID] = @UserID AND PostID!=@ExcludePostID AND [CreateDate] >= @DateTime; 
END
"
            )]
        #endregion
        public override void GetUserTodayAttachmentInfo(int userID, int? excludePostID, out int totalCount, out long totalFileSize)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetUserTodayAttachmentInfo";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int?>("@ExcludePostID", excludePostID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@DateTime", new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day, 0, 0, 0), SqlDbType.DateTime);

                totalCount = 0;
                totalFileSize = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        totalCount = reader.Get<int>(0);
                        totalFileSize = reader.Get<long>(1);
                    }
                }
            }
        }

        #region 存储过程 bx_v5_GetAttachmentsByPostID
        [StoredProcedure(Name = "bx_v5_GetAttachmentsByPostID", Script = @"
CREATE PROCEDURE {name}
    @PostID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID = @PostID;

END
"
            )]
        #endregion
        /// <summary>
        /// 通过postID获取附件列表
        /// </summary>
        /// <param name="postID"></param>
        /// <returns></returns>
        public override AttachmentCollection GetAttachments(int postID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetAttachmentsByPostID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new AttachmentCollection(reader);
                }
            }
        }
        #region 存储过程 bx_v5_GetAttachments
        [StoredProcedure(Name = "bx_v5_GetAttachment", Script = @"
CREATE PROCEDURE {name}
	@AttachmentID               int,
	@UpdateTotalDownloads       bit
AS
BEGIN
	SET NOCOUNT ON;
	IF(@UpdateTotalDownloads = 1)
		UPDATE bx_Attachments SET TotalDownloads=TotalDownloads+1 Where AttachmentID=@AttachmentID
	
	SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE AttachmentID =@AttachmentID;

    IF @UpdateTotalDownloads = 1
        SELECT ThreadID FROM [bx_Posts] P INNER JOIN [bx_Attachments] A ON P.PostID = A.PostID WHERE A.AttachmentID =  @AttachmentID;

END"
            )]
        #endregion
        public override Attachment GetAttachment(int attachmentID, bool updateTotalDownloads, out int threadID)
        {
            Attachment attachment = null;
            threadID = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetAttachment";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AttachmentID", attachmentID, SqlDbType.Int);
                query.CreateParameter<bool>("@UpdateTotalDownloads", updateTotalDownloads, SqlDbType.Bit);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        attachment = new Attachment(reader);
                    }
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            threadID = reader.Get<int>(0);
                        }
                    }
                }
            }

            return attachment;
        }


        #region StoredProcedure
        [StoredProcedure(Name = "bx_GetAttachmentByDiskFileID", Script = @"
CREATE PROCEDURE {name}
     @DiskFileID  int
    ,@PostID      int
AS BEGIN
    SET NOCOUNT ON;
    SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE [PostID] = @PostID AND [FileID] = (SELECT [FileID] FROM [bx_DiskFiles] WITH (NOLOCK) WHERE [DiskFileID] = @DiskFileID);
    SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE [PostID] = @PostID; 
    SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID = (SELECT ThreadID FROM [bx_Posts] WITH (NOLOCK) WHERE [PostID] = @PostID);
END
")]
        #endregion
        public override void GetAttachment(int diskFileID, int postID, out Attachment attachment, out PostV5 post, out BasicThread thread)
        {
            attachment = null;
            post = null;
            thread = null;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAttachmentByDiskFileID";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter("@DiskFileID", diskFileID, SqlDbType.Int);
                query.CreateParameter("@PostID", postID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attachment = new Attachment(reader);
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            post = new PostV5(reader);
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            thread = GetThread(reader, null);
                        }
                    }
                }
            }
        }




        #region 存储过程 bx_v5_GetAttachments
        [StoredProcedure(Name = "bx_v5_CreateAttachmentExchange", Script = @"
CREATE PROCEDURE {name}
	@AttachmentID       int, 
	@UserID             int,
	@Price              int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_AttachmentExchanges WITH (NOLOCK) WHERE AttachmentID=@AttachmentID AND UserID=@UserID)
		RETURN 0;
	Insert into bx_AttachmentExchanges(AttachmentID,UserID,Price,CreateDate) values(@AttachmentID,@UserID,@Price,getdate())
	IF(@@ROWCOUNT>0)
		RETURN 0
	ELSE
		RETURN 1

END
"
            )]
        #endregion
        public override bool CreateAttachmentExchange(int attachmentID, int userID, int price)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_CreateAttachmentExchange";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AttachmentID", attachmentID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Price", price, SqlDbType.Int);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return Convert.ToInt32(query.Parameters["@ErrorCode"].Value) == 0;
            }
        }


        #region 存储过程 bx_v5_GetAttachmentIsBuy
        [StoredProcedure(Name = "bx_v5_GetAttachmentIsBuy", Script = @"
CREATE PROCEDURE {name}
	@UserID             INT,
	@AttachmentID       INT
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM bx_AttachmentExchanges WITH (NOLOCK) WHERE AttachmentID=@AttachmentID AND UserID=@UserID)
		RETURN 0
	ELSE
	BEGIN
		IF EXISTS(SELECT * FROM bx_Attachments WITH (NOLOCK) WHERE AttachmentID=@AttachmentID AND Price=0)
			RETURN 0
		ELSE
			RETURN -1
	END
END
"
            )]
        #endregion
        public override bool IsBuyedAttachment(int userID, int attachmentID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetAttachmentIsBuy";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@AttachmentID", attachmentID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                return Convert.ToInt32(query.Parameters["@ErrorCode"].Value) == 0;
            }
        }

        public override AttachmentExchangeCollection GetAttachmentExchanges(int attachmentID, int pageNumber, int pageSize, out int totalCount, out int totalSellMoney)
        {
            totalCount = 0;
            totalSellMoney = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = "*";
                query.Pager.SortField = "[CreateDate]";

                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_AttachmentExchanges]";

                query.Pager.Condition = " [AttachmentID] = @AttachmentID ";

                query.CreateParameter<int>("@AttachmentID", attachmentID, SqlDbType.Int);

                query.Pager.AfterExecute = "SELECT SUM(Price) FROM bx_AttachmentExchanges WITH (NOLOCK) WHERE [AttachmentID] = @AttachmentID;";

                AttachmentExchangeCollection exchanges;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    exchanges = new AttachmentExchangeCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalSellMoney = reader.Get<int>(0);
                        }
                    }
                }

                return exchanges;
            }
        }

        public override ThreadExchangeCollection GetThreadExchanges(int threadID, int pageNumber, int pageSize, out int totalCount, out int totalSellMoney)
        {
            totalCount = 0;
            totalSellMoney = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = "*";
                query.Pager.SortField = "[CreateDate]";

                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_ThreadExchanges]";

                query.Pager.Condition = " [ThreadID] = @ThreadID ";

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                query.Pager.AfterExecute = "SELECT SUM(Price) FROM bx_ThreadExchanges WITH (NOLOCK) WHERE [ThreadID] = @ThreadID;";

                ThreadExchangeCollection exchanges;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    exchanges = new ThreadExchangeCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalSellMoney = reader.Get<int>(0);
                        }
                    }
                }

                return exchanges;
            }
        }


        public override AttachmentCollection GetAttachments(int pageNumber, AttachmentFilter filter, Guid[] excludeRoleIDs, ref int totalCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = filter.IsDesc;

                switch (filter.Order)
                {
                    case AttachmentFilter.OrderBy.AttachmentID:
                        query.Pager.SortField = "AttachmentID";
                        break;
                    case AttachmentFilter.OrderBy.FileSize:
                        query.Pager.SortField = "FileSize";
                        break;
                    case AttachmentFilter.OrderBy.Price:
                        query.Pager.SortField = "Price";
                        break;
                    default:
                        query.Pager.SortField = "TotalDownloads";
                        break;
                }

                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = filter.PageSize;
                query.Pager.TotalRecords = totalCount;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_AttacmentsWithForumID]";

                query.Pager.Condition = BuilderSearchAttachmentCondition(filter, excludeRoleIDs, query, false);



                AttachmentCollection attachments;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    attachments = new AttachmentCollection(reader);

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.GetInt32(0);
                        }
                    }
                    return attachments;
                }
            }
        }

        public override void DeleteAttachments(int forumID, IEnumerable<int> attachmentIDs, IEnumerable<Guid> excludeRoleIDs, out List<int> threadIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                SqlConditionBuilder condition = new SqlConditionBuilder(SqlConditionStart.None);

                if (forumID != 0)
                {
                    condition += " AND ForumID=@ForumID ";
                    query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                }

                condition += " AND [AttachmentID] in(@AttachmentIDs) ";
                query.CreateInParameter<int>("@AttachmentIDs", attachmentIDs);

                condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query));

                query.CommandText = @"
DECLARE @Table table(AttachmentID int,FileID varchar(50),ThreadID int);
INSERT INTO @Table SELECT [AttachmentID],[FileID],[ThreadID] FROM [bx_AttacmentsWithForumID] WHERE " + condition.ToString() + @";
DELETE [bx_Attachments] WHERE AttachmentID IN(SELECT AttachmentID FROM @Table);
SELECT DISTINCT ThreadID FROM @Table;
";
                threadIDs = new List<int>();
                //fileIDs = new List<string>();
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threadIDs.Add(reader.Get<int>(0));
                    }
                }
            }
        }

        public override void DeleteSearchAttachments(AttachmentFilter filter, IEnumerable<Guid> excludeRoleIDs, int topCount, out int deletedCount, out List<int> threadIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {

                string conditon = BuilderSearchAttachmentCondition(filter, excludeRoleIDs, query, false);

                query.CommandText = @"
DECLARE @Table table(AttachmentID int,FileID varchar(50),ThreadID int);
INSERT INTO @Table SELECT TOP (@TopCount) [AttachmentID],[FileID],[ThreadID] FROM [bx_AttacmentsWithForumID] WHERE " + conditon + @";
DELETE [bx_Attachments] WHERE AttachmentID IN(SELECT AttachmentID FROM @Table);
SELECT @@ROWCOUNT;
SELECT DISTINCT ThreadID FROM @Table;
";


                query.CreateTopParameter("@TopCount", topCount);

                threadIDs = new List<int>();
                //fileIDs = new List<string>();
                deletedCount = 0;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deletedCount = reader.Get<int>(0);
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            threadIDs.Add(reader.Get<int>(0));
                        }
                    }
                }
            }
        }


        private string BuilderSearchAttachmentCondition(AttachmentFilter filter, IEnumerable<Guid> excludeRoleIDs, SqlQuery query, bool startWithWhere)
        {
            SqlConditionBuilder condition;
            if (startWithWhere)
                condition = new SqlConditionBuilder(SqlConditionStart.Where);
            else
                condition = new SqlConditionBuilder(SqlConditionStart.None);

            condition += (filter.ForumID == null ? "" : ("AND [ForumID] = @ForumID "));
            condition += (filter.UserID == null ? "" : ("AND [UserID] = @UserID "));
            condition += (string.IsNullOrEmpty(filter.FileType) ? "" : ("AND [FileType] = @FileType "));
            condition += (string.IsNullOrEmpty(filter.KeyWord) ? "" : ("AND [FileName] LIKE '%'+@keyword+'%' "));
            condition += (filter.MinFileSize == null ? "" : ("AND [FileSize] >= @MinFileSize "));
            condition += (filter.MaxFileSize == null ? "" : ("AND [FileSize] <= @MaxFileSize "));
            condition += (filter.MinTotalDownload == null ? "" : ("AND [TotalDownloads] >= @MinTotalDownload "));
            condition += (filter.MaxTotalDownload == null ? "" : ("AND [TotalDownloads] <= @MaxTotalDownload "));
            condition += (filter.MinPrice == null ? "" : ("AND [Price] >= @MinPrice "));
            condition += (filter.MaxPrice == null ? "" : ("AND [Price] <= @MaxPrice "));
            condition += (filter.BeginDate == null ? "" : ("AND [CreateDate] > @BeginDate "));
            condition += (filter.EndDate == null ? "" : ("AND [CreateDate] < @EndDate "));

            if (string.IsNullOrEmpty(filter.KeyWord) == false)
            {
                query.CreateParameter<string>("@keyword", filter.KeyWord, SqlDbType.NVarChar, 256);
            }

            if (string.IsNullOrEmpty(filter.FileType) == false)
            {
                query.CreateParameter<string>("@FileType", filter.FileType, SqlDbType.VarChar, 10);
            }

            condition.AppendAnd(DaoUtil.GetExcludeRoleSQL("[UserID]", excludeRoleIDs, query));

            if (filter.ForumID != null)
                query.CreateParameter<int?>("@ForumID", filter.ForumID, SqlDbType.Int);
            if (filter.UserID != null)
                query.CreateParameter<int?>("@UserID", filter.UserID, SqlDbType.Int);

            if (filter.MaxFileSize != null)
            {
                query.CreateParameter<long>("@MaxFileSize", ConvertUtil.GetFileSize(filter.MaxFileSize.Value, filter.MaxFileSizeUnit), SqlDbType.BigInt);
            }
            if (filter.MinFileSize != null)
                query.CreateParameter<long>("@MinFileSize", ConvertUtil.GetFileSize(filter.MinFileSize.Value, filter.MinFileSizeUnit), SqlDbType.BigInt);

            if (filter.MaxTotalDownload != null)
                query.CreateParameter<int?>("@MaxTotalDownload", filter.MaxTotalDownload, SqlDbType.Int);
            if (filter.MinTotalDownload != null)
                query.CreateParameter<int?>("@MinTotalDownload", filter.MinTotalDownload, SqlDbType.Int);

            if (filter.MaxPrice != null)
                query.CreateParameter<int?>("@MaxPrice", filter.MaxPrice, SqlDbType.Int);
            if (filter.MinPrice != null)
                query.CreateParameter<int?>("@MinPrice", filter.MinPrice, SqlDbType.Int);

            if (filter.BeginDate != null)
                query.CreateParameter<DateTime?>("@BeginDate", filter.BeginDate, SqlDbType.DateTime);
            if (filter.EndDate != null)
                query.CreateParameter<DateTime?>("@EndDate", filter.EndDate, SqlDbType.DateTime);

            return condition.ToString();
        }



        #endregion

        public override void DeleteTopicStatus(IEnumerable<int> ids)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE [bx_TopicStatus] WHERE ID IN(@IDS);";
                query.CommandType = CommandType.Text;
                query.CommandTimeout = int.MaxValue;
                query.CreateInParameter<int>("@IDS", ids);

                query.ExecuteNonQuery();
            }
        }

        public override void DeleteTopicStatus(IEnumerable<int> threadIDs, TopicStatuType type)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "DELETE [bx_TopicStatus] WHERE ThreadID IN(@ThreadIDs) AND Type=@Type;";
                query.CommandType = CommandType.Text;
                query.CommandTimeout = int.MaxValue;

                query.CreateInParameter<int>("@ThreadIDs", threadIDs);
                query.CreateParameter<int>("@Type", (int)type, SqlDbType.TinyInt);

                query.ExecuteNonQuery();
            }
        }

        public override void CreateTopicStatus(IEnumerable<int> threadIDs, TopicStatuType type, DateTime endDate)
        {
            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sql = new StringBuilder();

                int i = 0;
                foreach (int threadID in threadIDs)
                {
                    sql.AppendFormat(@"
IF EXISTS(SELECT * FROM [bx_TopicStatus] WHERE ThreadID = @ThreadID_{0} AND Type = @Type)
    UPDATE [bx_TopicStatus] SET [EndDate] = @EndDate WHERE ThreadID = @ThreadID_{0} AND Type = @Type;
ELSE
    INSERT INTO [bx_TopicStatus](ThreadID,Type,EndDate) VALUES(@ThreadID_{0},@Type,@EndDate);
", i);
                    query.CreateParameter<int>("@ThreadID_" + i, threadID, SqlDbType.Int);
                    i++;
                }

                query.CreateParameter<int>("Type", (int)type, SqlDbType.TinyInt);
                query.CreateParameter<DateTime>("EndDate", endDate, SqlDbType.DateTime);

                query.CommandText = sql.ToString();
                query.CommandType = CommandType.Text;

                query.ExecuteNonQuery();
            }
        }

        public override bool UpdateThreadCatalog(int forumID, IEnumerable<int> threadIDs, int threadCatalogID)
        {
            string cmdText = "UPDATE bx_Threads SET ThreadCatalogID=@ThreadCatalogID WHERE ThreadID IN (" + StringUtil.Join(threadIDs) + ") AND ForumID = @ForumID;";

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = cmdText;
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadCatalogID", threadCatalogID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
            return true;
        }


        /// <summary>
        /// 提升主题
        /// </summary>
        /// <param name="threadID"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SetThreadsUp", Script = @"
CREATE PROCEDURE {name}
	@ThreadIdentities varchar(8000),
	@ForumID int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ThreadID int;
	--DECLARE @SortOrder bigint
	DECLARE @i int
	DECLARE @j int
	
	DECLARE @SortOrder BIGINT,@PostDate datetime
	
	SET @PostDate = getdate();
	
	
	SET @ThreadIdentities = @ThreadIdentities + N','
	SELECT @i = CHARINDEX(',', @ThreadIdentities)
	
	SET @j = 0
	
	WHILE ( @i > 1 ) BEGIN
			SELECT @ThreadID = SUBSTRING(@ThreadIdentities,0, @i)	

			EXEC [bx_GetSortOrder] 1, @j, @PostDate, 1, @SortOrder OUTPUT;

			UPDATE bx_Threads SET SortOrder = @SortOrder WHERE ForumID=@ForumID AND ThreadID=@ThreadID
			
			SELECT @ThreadIdentities = SUBSTRING(@ThreadIdentities, @i + 1, LEN(@ThreadIdentities) - @i)
			SELECT @i = CHARINDEX(',',@ThreadIdentities)
			SELECT @j = @j + 1
	END
	RETURN
END


")]
        public override bool SetThreadsUp(int forumID, IEnumerable<int> threadIdentities)
        {
            string identitieString = StringUtil.Join(threadIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetThreadsUp";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadIdentities", identitieString, SqlDbType.VarChar, 8000);

                query.ExecuteNonQuery();
            }
            return true;
        }


        /// <summary>
        /// 加入/解除精华
        /// </summary>
        /// <param name="threadIdentities"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SetThreadsValued", Script = @"
CREATE PROCEDURE {name} 
	@ForumID INT,
	@ThreadIdentities varchar(8000),
	@IsValued bit
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @RowCount int

	EXEC ('UPDATE bx_Threads SET IsValued=' + @IsValued + ' WHERE ThreadID IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID + ' AND IsValued<>' + @IsValued) 
	
	SET @RowCount=@@ROWCOUNT
	IF @RowCount> 0
		
		----------统计：增加删除精华数---------------
		IF @IsValued=0
			SELECT @RowCount= 0 - @RowCount;

		EXEC [bx_DoCreateStat] @ForumID,2, @RowCount
		--------------------------
	RETURN
END
")]
        public override bool SetThreadsValued(int forumID, IEnumerable<int> threadIdentities, bool isValued)
        {
            string identitieString = StringUtil.Join(threadIdentities);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetThreadsValued";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadIdentities", identitieString, SqlDbType.VarChar, 8000);
                query.CreateParameter<bool>("@IsValued", isValued, SqlDbType.Bit);

                query.ExecuteNonQuery();
            }
            return true;
        }



        #region bx_v5_ApprovePosts

        [StoredProcedure(Name = "bx_v5_ApprovePosts", Script = @"
CREATE PROCEDURE {name} 
	@PostIdentities varchar(8000)
AS
	SET NOCOUNT ON
	
	EXEC('SELECT DISTINCT(ThreadID) FROM [bx_Posts] WITH (NOLOCK) WHERE PostID IN('+@PostIdentities+') AND SortOrder >= 5000000000000000')
	
	
	DECLARE @TempTable table(tempId int IDENTITY(1, 1),TempPostID int,TempSortOrder bigint);
	
	INSERT INTO @TempTable(TempPostID) SELECT item FROM bx_GetIntTable(@PostIdentities, ',');
	
	UPDATE @TempTable SET TempSortOrder = SortOrder FROM [bx_Posts] WHERE TempPostID = PostID;
	
	DECLARE @i int,@Total int;
	SET @i = 0;
	SELECT @Total = COUNT(*) FROM @TempTable;
	
	WHILE(@i<@Total) BEGIN
		SET @i = @i + 1;
		DECLARE @SortOrder bigint,@OldSortOrder bigint;
	
		SELECT @OldSortOrder = TempSortOrder FROM @TempTable WHERE tempId = @i;
		EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @SortOrder OUTPUT;
		
		UPDATE @TempTable SET TempSortOrder = @SortOrder WHERE tempId = @i; 
	END
	
	UPDATE [bx_Posts] SET SortOrder=TempSortOrder FROM @TempTable WHERE PostID = TempPostID AND SortOrder >= 5000000000000000;
")]
        #endregion
        public override List<int> ApprovePosts(IEnumerable<int> postIDs)
        {
            string identitieString = StringUtil.Join(postIDs);
            List<int> threadIdentities = new List<int>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_ApprovePosts";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@PostIdentities", identitieString, SqlDbType.Text);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threadIdentities.Add(reader.GetInt32(0));
                    }
                }

                SetThreadLastPostID(threadIdentities, query);
            }
            return threadIdentities;
        }


        public override void UpdatePostsForumID(IEnumerable<int> postIDs,int forumID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Posts SET ForumID = @ForumID WHERE PostID in(@PostIDs);";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateInParameter<int>("@PostIDs", postIDs);

                query.ExecuteNonQuery();
            }
        }

        private void SetThreadLastPostID(IEnumerable<int> threadIDs, SqlQuery query)
        {
            if (ValidateUtil.HasItems<int>(threadIDs) == false)
                return;
            query.CommandType = CommandType.Text;
            query.Parameters.Clear();

            StringBuilder sql = new StringBuilder();
            sql.Append("DECLARE @LastPostID int;");

            int i = 0;
            foreach (int id in threadIDs)
            {
                sql.AppendFormat(@"SELECT @LastPostID = MAX(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID=@ThreadID_{0} AND SortOrder < 4000000000000000;
IF @LastPostID IS NULL
    SELECT @LastPostID = MIN(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID=@ThreadID_{0} AND PostType = 1;
IF @LastPostID IS NULL
    SELECT @LastPostID = MIN(PostID) FROM bx_Posts WITH (NOLOCK) WHERE ThreadID=@ThreadID_{0};
"
                    , i);
                query.CreateParameter<int>("@ThreadID_" + i, id, SqlDbType.Int);
            }
            query.CommandText = sql.ToString();
            query.ExecuteNonQuery();
        }


        #region 杂项
        #region bx_SetPostLoveHate
        [StoredProcedure(Name = "bx_SetPostLoveHate", Script = @"
CREATE PROCEDURE {name} 
	@UserID         int,
	@PostID         int,
	@IsLove         bit,
    @CanSetMore     bit
AS
BEGIN
	SET NOCOUNT ON
	
    IF NOT EXISTS(SELECT * FROM bx_Posts WITH (NOLOCK) WHERE PostID=@PostID)
        RETURN 1; --不存在这个帖子

    DECLARE @LoveCount int,@HateCount int;
    IF @IsLove = 1 BEGIN
        SET @LoveCount = 1;
        SET @HateCount = 0;
    END
    ELSE BEGIN
        SET @LoveCount = 0;
        SET @HateCount = 1;
    END

    IF EXISTS(SELECT * FROM bx_PostLoveHates WITH (NOLOCK) WHERE PostID=@PostID AND UserID=@UserID) BEGIN
        IF @CanSetMore = 0
            RETURN 2; --已经支持反对过了
        ELSE
            UPDATE bx_PostLoveHates SET LoveCount = LoveCount + @LoveCount, HateCount = HateCount + @HateCount WHERE PostID=@PostID AND UserID=@UserID; 
    END 
    ELSE
        INSERT INTO bx_PostLoveHates(PostID,UserID,LoveCount,HateCount) VALUES(@PostID,@UserID,@LoveCount,@HateCount);

    UPDATE bx_Posts SET LoveCount = @LoveCount, HateCount = @HateCount WHERE PostID = @PostID;
    
    SELECT ThreadID FROM bx_Posts WITH (NOLOCK) WHERE PostID = @PostID;

    RETURN 0;
END
"
            )]
        #endregion
        public override int SetPostLoveHate(int userID, int postID, bool isLove, bool canSetMore, out int threadID)
        {
            threadID = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetPostLoveHate";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);
                query.CreateParameter<bool>("@CanSetMore", canSetMore, SqlDbType.Bit);
                query.CreateParameter<bool>("@IsLove", isLove, SqlDbType.Bit);

                SqlParameter param = query.CreateParameter<int>("ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threadID = reader.Get<int>(0);
                    }
                }

                return (int)param.Value;
            }
        }


        #region bx_SetThreadRank
        [StoredProcedure(Name = "bx_SetThreadRank", Script = @"
CREATE PROCEDURE {name} 
	@ThreadID       int,
	@UserID         int,
	@Rank           tinyint
AS
BEGIN
	SET NOCOUNT ON
	IF EXISTS (SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND PostUserID=@UserID)
		RETURN (1)--不能给自己评级
	IF EXISTS (SELECT * FROM [bx_ThreadRanks] WHERE ThreadID=@ThreadID AND UserID=@UserID)
		 UPDATE [bx_ThreadRanks] SET Rank=@Rank,UpdateDate=getdate() WHERE ThreadID=@ThreadID AND UserID=@UserID 
	ELSE
		INSERT INTO [bx_ThreadRanks] (
		                                [ThreadID],
		                                [UserID],
		                                [Rank]
		                                ) VALUES (
		                                @ThreadID,
		                                @UserID,
		                                @Rank
		                                );
	--更新平均值---
	DECLARE @Count int,@TotalRank int
	SELECT @Count=COUNT(*) FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID;
	SELECT @TotalRank=SUM(Rank) FROM [bx_ThreadRanks] WITH (NOLOCK) WHERE ThreadID=@ThreadID;
	UPDATE [bx_Threads] SET Rank=@TotalRank/@Count WHERE ThreadID=@ThreadID;
	
    SELECT @TotalRank/@Count AS NewRank;

	RETURN (0)
END
"
            )]
        #endregion
        public override int SetThreadRank(int threadID, int userID, int addRank, out int threadRank)
        {
            threadRank = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetThreadRank";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@Rank", addRank, SqlDbType.TinyInt);

                SqlParameter param = query.CreateParameter<int>("ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threadRank = reader.Get<int>(0);
                    }
                }

                return (int)param.Value;
            }
        }


        public override ThreadRankCollection GetThreadRanks(int threadID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = "*";
                query.Pager.SortField = "[CreateDate]";

                query.Pager.PrimaryKey = "[UserID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_ThreadRanks]";

                query.Pager.Condition = " [ThreadID] = @ThreadID ";

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                ThreadRankCollection ranks;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    ranks = new ThreadRankCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }

                return ranks;
            }
        }



        /// <summary>
        /// 添加购买记录
        /// </summary>
        #region bx_SetThreadRank
        [StoredProcedure(Name = "bx_CreateThreadExchange", Script = @"
CREATE PROCEDURE {name} 
	@ThreadID       int,
	@UserID         int,
	@Price          int
AS
BEGIN
	SET NOCOUNT ON
    IF NOT EXISTS(SELECT * FROM bx_ThreadExchanges WITH (NOLOCK) WHERE ThreadID = @ThreadID AND UserID = @UserID)
        INSERT INTO bx_ThreadExchanges (ThreadID,UserID,Price) VALUES (@ThreadID,@UserID,@Price);
END
"
            )]
        #endregion
        public override bool CreateThreadExchange(int threadID, int userID, int price)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_CreateThreadExchange";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@Price", price, SqlDbType.Int);


                query.ExecuteNonQuery();
            }

            return true;
        }


        /// <summary>
        /// 是否购买过帖子
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="threadID"></param>
        /// <returns></returns>
        #region bx_IsBuyThread
        [StoredProcedure(Name = "bx_IsBuyThread", Script = @"
CREATE PROCEDURE {name} 
	@ThreadID       int,
	@UserID         int
AS
BEGIN
	SET NOCOUNT ON
    IF EXISTS(SELECT * FROM bx_ThreadExchanges WITH (NOLOCK) WHERE ThreadID = @ThreadID AND UserID = @UserID)
        SELECT 1;
END
"
            )]
        #endregion
        public override bool IsBuyedThread(int userID, int threadID)
        {
            bool isBuy = false;

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_IsBuyThread";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isBuy = true;
                    }
                    else
                        isBuy = false;
                }
            }

            return isBuy;
        }


        #region bx_v5_Vote
        [StoredProcedure(Name = "bx_v5_Vote", Script = @"
CREATE PROCEDURE {name} 
	@ItemIDs            varchar(8000),
	@ThreadID           int,
	@UserID             int,
	@NickName           nvarchar(64)
AS
	SET NOCOUNT ON
	IF EXISTS (SELECT * FROM bx_Polls WITH(NOLOCK) WHERE ThreadID=@ThreadID AND ExpiresDate<getdate()) 
		RETURN (1)--已经过期
	IF EXISTS (SELECT * FROM [bx_PollItemDetails] WITH(NOLOCK) WHERE ItemID IN(SELECT ItemID FROM [bx_PollItems] WITH (NOLOCK) WHERE ThreadID=@ThreadID) AND UserID=@UserID)
		RETURN (2)--	当前用户已经投过票
	IF EXISTS (SELECT * FROM bx_Threads WITH(NOLOCK) WHERE ThreadID=@ThreadID AND IsLocked=1) 
		RETURN (3)--已经锁定
	DECLARE @ItemID int,@i int
	SET @ItemIDs=@ItemIDs+N','
	SELECT @i=CHARINDEX(',',@ItemIDs)
			
	WHILE(@i>1)
		BEGIN
			SELECT @ItemID=SUBSTRING(@ItemIDs,0, @i)
			
			UPDATE bx_PollItems SET PollItemCount=PollItemCount+1 WHERE ItemID=@ItemID
			
			IF(@@ROWCOUNT>0)
				INSERT INTO bx_PollItemDetails(ItemID,UserID,NickName) VALUES(@ItemID,@UserID,@NickName)
			
			SELECT @ItemIDs=SUBSTRING(@ItemIDs,@i+1,LEN(@ItemIDs)-@i)
			SELECT @i=CHARINDEX(',',@ItemIDs)
		END

    EXECUTE bx_v5_GetThreadExtendData @ThreadID,1; 

	RETURN (0)
"
            )]
        #endregion
        public override int Vote(IEnumerable<int> itemIDs, int threadID, int userID, string nickName, PollThreadV5 poll)
        {
            string itemIDsString = StringUtil.Join(itemIDs);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_Vote";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@ItemIDs", itemIDsString, SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@NickName", nickName, SqlDbType.NVarChar, 64);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                string extendData = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    extendData = GetExtendData(poll, ThreadType.Poll, reader, true);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue == 0 && extendData != null)
                {
                    UpdateThreadExtendData(extendData, threadID, query);
                }

                return returnValue;
            }
        }



        #region bx_GetPollItemDetails
        [StoredProcedure(Name = "bx_GetPollItemDetails", Script = @"
CREATE PROCEDURE {name} 
	@ThreadID           int
AS
	SET NOCOUNT ON

    SELECT * FROM [bx_PollItemDetails] WITH(NOLOCK) WHERE ItemID IN(SELECT ItemID FROM [bx_PollItems] WHERE ThreadID=@ThreadID);
"
            )]
        #endregion
        public override PollItemDetailsCollectionV5 GetPollItemDetails(int threadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetPollItemDetails";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PollItemDetailsCollectionV5(reader);
                }
            }
        }

        #region bx_AddPolemizeUser
        [StoredProcedure(Name = "bx_v5_AddPolemizeUser", Script = @"
CREATE PROCEDURE {name} 
    @ThreadID int, 
	@UserID int,
	@ViewPointType tinyint
AS
	SET NOCOUNT ON;
	
	IF(NOT EXISTS (SELECT * FROM [bx_Polemizes] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND ExpiresDate>GETDATE()))
		RETURN (1) --过期了
	
	DECLARE @ViewPoint TINYINT
	SELECT @ViewPoint=ViewPointType FROM [bx_PolemizeUsers] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID
	IF @ViewPoint = 0 --已支持过正方观点
		RETURN (2)
	ELSE IF @ViewPoint = 1 --已支持过反方观点
		RETURN (3)
	ELSE IF @ViewPoint = 2 --已支持过中方观点
		RETURN (4)
		
	INSERT INTO [bx_PolemizeUsers](ThreadID,UserID,ViewPointType) VALUES(@ThreadID,@UserID,@ViewPointType)
		
	IF @ViewPointType = 0 BEGIN
		UPDATE [bx_Polemizes] SET AgreeCount=AgreeCount+1 WHERE ThreadID=@ThreadID
	END
	ELSE IF @ViewPointType = 1 BEGIN
		UPDATE [bx_Polemizes] SET AgainstCount=AgainstCount+1 WHERE ThreadID=@ThreadID
	END
	ELSE IF @ViewPointType = 2 BEGIN
		UPDATE [bx_Polemizes] SET NeutralCount=NeutralCount+1 WHERE ThreadID=@ThreadID
	END
	
    EXECUTE bx_v5_GetThreadExtendData @ThreadID,4; 
	RETURN (0);
"
            )]
        #endregion
        public override int Polemize(int threadID, int userID, ViewPointType viewPointType, PolemizeThreadV5 polemize)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_AddPolemizeUser";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@ViewPointType", (int)viewPointType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                string extendData = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    extendData = GetExtendData(polemize, ThreadType.Polemize, reader, true);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue == 0 && extendData != null)
                {
                    UpdateThreadExtendData(extendData, threadID, query);
                }

                return returnValue;
            }
        }

        #region bx_FinalQuestion
        [StoredProcedure(Name = "bx_v5_FinalQuestion", Script = @"
CREATE PROCEDURE {name} 
    @ThreadID               int,
	@BestPostID             int,
	@PostRewards            varchar(8000)
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @ThreadType tinyint;
	SELECT @ThreadType = ThreadType FROM bx_Threads WITH (NOLOCK) WHERE ThreadID = @ThreadID;

	IF @ThreadType <> 2
		RETURN (1); -- FinalQuestionStatus.NotQuestion
	ELSE BEGIN
		DECLARE @IsClosed bit,@ExpiresDate datetime;
		SELECT @IsClosed = IsClosed,@ExpiresDate=ExpiresDate FROM bx_Questions WITH (NOLOCK) WHERE ThreadID = @ThreadID;
		IF @IsClosed = 1 OR @ExpiresDate<GETDATE()
			RETURN (2);  --FinalQuestionStatus.Finaled
        IF NOT EXISTS(SELECT * FROM bx_Posts WITH(NOLOCK) WHERE PostID=@BestPostID AND ThreadID=@ThreadID)
            RETURN 3; --invilad bestpostid
		ELSE BEGIN
			--Postid:Reward, 12:60,13:70,1:2,
---------------------------------------------
			DECLARE @i int,@j int,@PostID int,@Reward int,@TotalReward int,@RewardCount int,@CanGetRewardCount int;--@ErrorCode int
			SET @PostRewards=@PostRewards+N','
			SET @TotalReward=0
			SET @RewardCount=0
			
			SELECT @j=CHARINDEX(':',@PostRewards)
			SELECT @i=CHARINDEX(',',@PostRewards)
			
			WHILE(@j>0 AND @i>2)
				BEGIN	
					SELECT @PostID=SUBSTRING(@PostRewards,0, @j)
					SELECT @PostRewards=SUBSTRING(@PostRewards,@j+1,len(@PostRewards)-@j)
					
					SELECT @i=CHARINDEX(',',@PostRewards)
					SELECT @Reward=SUBSTRING(@PostRewards,0, @i)
					SELECT @PostRewards=SUBSTRING(@PostRewards,@i+1,len(@PostRewards)-@i)
					
					SELECT @j=CHARINDEX(':',@PostRewards)
					SELECT @i=CHARINDEX(',',@PostRewards)
					
					SET @TotalReward=@TotalReward+@Reward
					SET @RewardCount=@RewardCount+1
					
					INSERT INTO [bx_QuestionRewards](ThreadID,PostID,Reward) VALUES(@ThreadID,@PostID,@Reward)
					
				END
				
			SELECT @Reward=Reward,@CanGetRewardCount=RewardCount FROM [bx_Questions] WITH(NOLOCK) WHERE ThreadID=@ThreadID
			IF(@Reward<>@TotalReward)
				BEGIN
					RETURN 4;
				END
			
			IF(@RewardCount>@CanGetRewardCount)
				BEGIN
					RETURN 5;
				END
				
			UPDATE [bx_Questions] SET IsClosed=1,BestPostID=@BestPostID WHERE ThreadID=@ThreadID

            EXECUTE bx_v5_GetThreadExtendData @ThreadID,2; 
			RETURN (0)
		END
	END

END
"
            )]
        #endregion
        public override int FinalQuestion(int threadID, int bestPostID, Dictionary<int, int> rewards, QuestionThread question)
        {
            StringBuilder rewardsString = new StringBuilder();

            int i = 0;
            foreach (KeyValuePair<int, int> reward in rewards)
            {
                if (reward.Key != 0)
                {
                    if (i != 0)
                        rewardsString.Append(",");

                    rewardsString.Append(reward.Key);
                    rewardsString.Append(":");
                    rewardsString.Append(reward.Value);
                    i++;
                }
            }

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_FinalQuestion";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@BestPostID", bestPostID, SqlDbType.Int);
                query.CreateParameter<string>("@PostRewards", rewardsString.ToString(), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                string extendData = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    extendData = GetExtendData(question, ThreadType.Question, reader, true);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue == 0 && extendData != null)
                {
                    UpdateThreadExtendData(extendData, threadID, query);
                }

                return returnValue;
            }
        }

        #region bx_VoteQuestionBestPost
        [StoredProcedure(Name = "bx_VoteQuestionBestPost", Script = @"
CREATE PROCEDURE {name} 
	@ThreadID           int,
	@UserID             int,
	@BestPostIsUseful   bit
AS
	SET NOCOUNT ON;
	
	DECLARE @IsClosed bit
	
	SELECT @IsClosed = IsClosed FROM [bx_Questions] WITH (NOLOCK) WHERE ThreadID=@ThreadID
	
	IF @IsClosed IS NULL
		RETURN (1) -- 不存在
	ELSE IF @IsClosed = 0
		RETURN (2) -- 还未揭贴
	
	IF(EXISTS (SELECT * FROM [bx_QuestionUsers] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID))
		RETURN (3) -- 已经投过
		
	BEGIN TRANSACTION
		
	INSERT INTO [bx_QuestionUsers](ThreadID,UserID,BestPostIsUseful) VALUES(@ThreadID,@UserID,@BestPostIsUseful)
	IF(@@error<>0)
		GOTO Cleanup;
	
	IF @BestPostIsUseful = 1 BEGIN
		UPDATE [bx_Questions] SET UsefulCount=UsefulCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	ELSE BEGIN
		UPDATE [bx_Questions] SET UnusefulCount=UnusefulCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	
		COMMIT TRANSACTION;

        EXECUTE bx_v5_GetThreadExtendData @ThreadID,2; 
		RETURN (0);
Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END
"
            )]
        #endregion
        public override int VoteQuestionBestPost(int threadID, int userID, bool isUseful, QuestionThread question)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_VoteQuestionBestPost";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<bool>("@BestPostIsUseful", isUseful, SqlDbType.Bit);
                query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                string extendData = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    extendData = GetExtendData(question, ThreadType.Question, reader, true);
                }

                int returnValue = (int)query.Parameters["@ErrorCode"].Value;

                if (returnValue == 0 && extendData != null)
                {
                    UpdateThreadExtendData(extendData, threadID, query);
                }

                return returnValue;
            }
        }

        #region bx_IsRepliedThread
        [StoredProcedure(Name = "bx_v5_IsRepliedThread", Script = @"
CREATE PROCEDURE {name} 
    @ThreadID   int,
	@UserID     int
AS
BEGIN

	SET NOCOUNT ON;
	IF EXISTS (SELECT 1 FROM bx_Posts WITH (NOLOCK) WHERE UserID = @UserID AND ThreadID = @ThreadID AND SortOrder < 5000000000000000)
		RETURN (0);
	ELSE
		RETURN (-1);

END
"
            )]
        #endregion
        public override bool IsRepliedThread(int threadID, int userID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_IsRepliedThread";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<int>("@IsReplied", SqlDbType.Int, ParameterDirection.ReturnValue);

                query.ExecuteNonQuery();

                bool isReplied = (Convert.ToInt32(query.Parameters["@IsReplied"].Value) == 0);

                return isReplied;
            }
        }

        #region bx_v5_CreatePostMark
        [StoredProcedure(Name = "bx_v5_CreatePostMark", Script = @"
CREATE PROCEDURE {name} 
    @PostID             int, 
	@UserID             int,
    @Username           nvarchar(50),
	@CreateDate         datetime,
	@ExtendedPoints_1   int,
	@ExtendedPoints_2   int,
	@ExtendedPoints_3   int,
	@ExtendedPoints_4   int,
	@ExtendedPoints_5   int,
	@ExtendedPoints_6   int,
	@ExtendedPoints_7   int,
	@ExtendedPoints_8   int,
	@Reason             ntext
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [UserID] = @UserID AND [PostID] = @PostID)
		RETURN 1;
	ELSE BEGIN
		insert into [bx_PostMarks](
			PostID,
			UserID,
            Username,
			CreateDate,
			ExtendedPoints_1,
			ExtendedPoints_2,
			ExtendedPoints_3,
			ExtendedPoints_4,
			ExtendedPoints_5,
			ExtendedPoints_6,
			ExtendedPoints_7,
			ExtendedPoints_8,
			Reason
			)
		Values
		(
			@PostID, 
			@UserID,
            @Username,
			@CreateDate,
			@ExtendedPoints_1,
			@ExtendedPoints_2,
			@ExtendedPoints_3,
			@ExtendedPoints_4,
			@ExtendedPoints_5,
			@ExtendedPoints_6,
			@ExtendedPoints_7,
			@ExtendedPoints_8,
			@Reason
		);

        
        UPDATE [bx_Posts] SET MarkCount = (SELECT Count(*) FROM [bx_PostMarks] WITH (NOLOCK) WHERE [PostID] = @PostID) WHERE [PostID] = @PostID;
        SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [UserID] = @UserID AND [PostID] = @PostID;

		RETURN 0;
		
	END

END
"
            )]
        #endregion
        public override int CreatePostMark(int postID, int userID, string username, DateTime createDate, int[] points, string reason, out PostMark postMark)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_CreatePostMark";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);
                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@Username", username, SqlDbType.NVarChar, 50);
                query.CreateParameter<DateTime>("@CreateDate", createDate, SqlDbType.DateTime);
                query.CreateParameter<int>("@ExtendedPoints_1", points[0], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_2", points[1], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_3", points[2], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_4", points[3], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_5", points[4], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_6", points[5], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_7", points[6], SqlDbType.Int);
                query.CreateParameter<int>("@ExtendedPoints_8", points[7], SqlDbType.Int);
                query.CreateParameter<string>("@Reason", reason, SqlDbType.NText);
                SqlParameter errorCode = query.CreateParameter<int>("@ErrorCode", SqlDbType.Int, ParameterDirection.ReturnValue);

                postMark = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        postMark = new PostMark(reader);
                    }
                }

                return Convert.ToInt32(errorCode.Value);
            }
        }


        #region bx_v5_GetUserTodayPostMarks
        [StoredProcedure(Name = "bx_v5_GetUserTodayPostMarks", Script = @"
CREATE PROCEDURE {name}
    @UserID      int,
    @DateTime    datetime
AS BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [UserID] = @UserID AND [CreateDate] >= @DateTime;
END
")]
        #endregion
        public override PostMarkCollection GetUserTodayPostMarks(int userID)
        {
            DateTime time = new DateTime(DateTimeUtil.Now.Year, DateTimeUtil.Now.Month, DateTimeUtil.Now.Day, 0, 0, 0);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetUserTodayPostMarks";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<DateTime>("@DateTime", time, SqlDbType.DateTime);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PostMarkCollection(reader);
                }
            }
        }

        public override void CancelRates(int postID, IEnumerable<int> postMarkIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
DELETE [bx_PostMarks] WHERE [PostMarkID] IN(@PostMarkIDs);
UPDATE [bx_Posts] SET MarkCount = (SELECT Count(*) FROM [bx_PostMarks] WITH (NOLOCK) WHERE [PostID] = @PostID) WHERE [PostID] = @PostID;
";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@PostMarkIDs", postMarkIDs);
                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }

        }

        public override PostMarkCollection GetPostMarks(IEnumerable<int> postMarkIDs)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE [PostMarkID] IN(@PostMarkIDs);";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@PostMarkIDs", postMarkIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new PostMarkCollection(reader);
                }
            }

        }


        public override PostMarkCollection GetPostMarks(int postID, int pageNumber, int pageSize, out int totalCount)
        {
            totalCount = 0;
            using (SqlQuery query = new SqlQuery())
            {
                query.Pager.IsDesc = true;
                query.Pager.ResultFields = "*";
                query.Pager.SortField = "[PostMarkID]";

                query.Pager.PrimaryKey = "[PostMarkID]";
                query.Pager.PageNumber = pageNumber;
                query.Pager.PageSize = pageSize;
                //query.Pager.TotalRecords = totalThreads;
                query.Pager.SelectCount = true;
                query.Pager.TableName = "[bx_PostMarks]";

                query.Pager.Condition = " [PostID] = @PostID ";

                query.CreateParameter<int>("@PostID", postID, SqlDbType.Int);

                PostMarkCollection postMarks;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    postMarks = new PostMarkCollection(reader);
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            totalCount = reader.Get<int>(0);
                        }
                    }
                }

                return postMarks;
            }
        }

        #region bx_v5_CreateThreadManageLog
        [StoredProcedure(Name = "bx_v5_CreateThreadManageLog", Script = @"
CREATE PROCEDURE {name}
	@UserID         int,
	@UserName       varchar(64),
	@NickName       varchar(64),
	@IPAddress      varchar(15),
	@PostUserIDs    varchar(8000),
	@ActionType     tinyint,
	@ForumID        int,
	@ThreadIDs      varchar(8000),
	@ThreadSubjects ntext,
	@Reason         nvarchar(256),
	@IsPublic       bit
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @TempTable TABLE(LogID INT IDENTITY(1,1),ThreadID INT,PostUserID INT,ThreadSubject nvarchar(256) COLLATE Chinese_PRC_CI_AS_WS)

	INSERT @TempTable (ThreadID) 
		SELECT item FROM bx_GetIntTable(@ThreadIDs,',')

	UPDATE @TempTable
			SET [ThreadSubject]=T.item
			FROM (SELECT * FROM bx_GetStringTable_ntext(@ThreadSubjects,N',')) T
			WHERE T.id=LogID;

	UPDATE @TempTable
			SET [PostUserID]=T.item
			FROM (SELECT * FROM bx_GetIntTable(@PostUserIDs, ',')) T
			WHERE T.id=LogID;

	INSERT INTO [bx_ThreadManageLogs] (
		[UserID],
		[UserName],
		[NickName],
		[IPAddress],
		[PostUserID],
		[ActionType],
		[ForumID],
		[ThreadID],
		[ThreadSubject],
		[Reason],
		[CreateDate],
		[IsPublic]
	) SELECT @UserID,@UserName,@NickName,@IPAddress,PostUserID,@ActionType,@ForumID,ThreadID,ThreadSubject,@Reason,getdate(),@IsPublic FROM @TempTable;

	----更新主题日志记录------
	IF @ActionType <> 1 AND @ActionType <> 17 AND @ActionType <> 18 AND @IsPublic = 1 BEGIN
		UPDATE bx_Threads Set ThreadLog = @UserName + '|' + CAST(@ActionType as NVARCHAR) + '|' + CAST(getdate() AS NVARCHAR) WHERE ThreadID IN (SELECT ThreadID FROM @TempTable);
        SELECT @UserName + '|' + CAST(@ActionType as NVARCHAR) + '|' + CAST(getdate() AS NVARCHAR) AS ThreadLog;
    END

END
")]
        #endregion
        public override void CreateThreadManageLog(int userID, string username, string realname, string ipAddress, ModeratorCenterAction actionType
            , IEnumerable<int> postUserIDs
            , int forumID, IEnumerable<int> threadIDs, IEnumerable<string> subjects, string reason, bool isPublic, out string threadLog)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_CreateThreadManageLog";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@UserID", userID, SqlDbType.Int);
                query.CreateParameter<string>("@NickName", realname, SqlDbType.NVarChar, 64);
                query.CreateParameter<string>("@UserName", username, SqlDbType.NVarChar, 64);
                query.CreateParameter<string>("@IPAddress", ipAddress, SqlDbType.VarChar, 15);
                query.CreateParameter<string>("@PostUserIDs", StringUtil.Join(postUserIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<int>("@ActionType", (int)actionType, SqlDbType.TinyInt);
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);
                query.CreateParameter<string>("@ThreadIDs", StringUtil.Join(threadIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<string>("@ThreadSubjects", StringUtil.Join(subjects), SqlDbType.NText);
                query.CreateParameter<string>("@Reason", reason, SqlDbType.NVarChar, 256);
                query.CreateParameter<bool>("@IsPublic", isPublic, SqlDbType.Bit);

                threadLog = null;
                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        threadLog = reader.Get<string>(0);
                    }
                }
            }
        }


        #region bx_v5_GetThreadManageLogs
        [StoredProcedure(Name = "bx_v5_GetThreadManageLogs", Script = @"
CREATE PROCEDURE {name}
     @ThreadID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadManageLogs WITH (NOLOCK) WHERE ThreadID=@ThreadID ORDER BY LogID DESC;
END
")]
        #endregion
        public override ThreadManageLogCollectionV5 GetThreadManageLogs(int threadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_v5_GetThreadManageLogs";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadManageLogCollectionV5(reader);
                }
            }

        }


        #region bx_GetAllStickThreadForums
        [StoredProcedure(Name = "bx_GetAllStickThreadForums", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_StickThreads  WITH (NOLOCK);
END
")]
        #endregion
        public override StickThreadCollection GetAllStickThreadForums()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetAllStickThreadForums";
                query.CommandType = CommandType.StoredProcedure;

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new StickThreadCollection(reader);
                }
            }
        }



        #region bx_ClearSearchResult
        [StoredProcedure(Name = "bx_ClearSearchResult", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN

    SET NOCOUNT ON;
	
	--清除搜索结果数据
	DELETE bx_SearchPostResults WHERE UpdateDate < DATEADD(minute, -30, getdate());

END")]
        #endregion
        public override void ClearSearchResult()
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_ClearSearchResult";
                query.CommandType = CommandType.StoredProcedure;

                query.ExecuteNonQuery();
            }
        }

        #region bx_GetThreadsByPostCreateDate
        [StoredProcedure(Name = "bx_GetThreadsByPostCreateDate", Script = @"
CREATE PROCEDURE {name}
    @CreateDate     datetime
AS
BEGIN

    SET NOCOUNT ON;
	--DECLARE @SortOrder BIGINT;
	
	--EXEC [bx_GetSortOrder] 1, 0, @CreateDate, 1, @SortOrder OUTPUT;
	
	SELECT T1.* 
	 FROM bx_Threads T1 WITH (NOLOCK) WHERE ThreadStatus = 1 AND [UpdateDate]>@CreateDate ORDER BY [UpdateDate] DESC

END")]
        #endregion
        public override ThreadCollectionV5 GetThreadsByPostCreateDate(DateTime postCreateDate)
        {
            List<int> firstPostIDs = new List<int>();

            ThreadCollectionV5 threads = new ThreadCollectionV5();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_GetThreadsByPostCreateDate";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<DateTime>("@CreateDate", postCreateDate, SqlDbType.DateTime);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BasicThread thread = GetThread(reader, null);
                        if (thread.UpdateDate > postCreateDate)
                        {
                            firstPostIDs.Add(thread.ContentID);
                            threads.Add(thread);
                        }
                    }
                }

                if (firstPostIDs.Count == 0)
                    return threads;
            }


            PostCollectionV5 firstPosts = GetPosts(firstPostIDs);

            for (int i = 0; i < firstPosts.Count; i++)
            {
                for (int j = 0; j < threads.Count; j++)
                {
                    if (firstPosts[i].ThreadID == threads[j].ThreadID)
                    {
                        threads[j].ThreadContent = firstPosts[i];
                        break;
                    }
                }
            }

            return threads;
        }



        public override ThreadCollectionV5 GetThreads(ThreadSortField sortType, int count, IEnumerable<int> forumIDs)
        {
            using (SqlQuery query = new SqlQuery())
            {
                if (forumIDs != null && ValidateUtil.HasItems<int>(forumIDs))
                {
                    query.CommandText = @"
SELECT TOP(@TopCount) " + ThreadFields + @" FROM bx_Threads WITH(NOLOCK) WHERE ForumID in(@ForumIDs) AND ThreadStatus = 1 ORDER BY " + GetSortField(sortType) + @" DESC;
";
                    query.CreateInParameter<int>("@ForumIDs", forumIDs);
                }
                else
                {
                    query.CommandText = @"
SELECT TOP(@TopCount) " + ThreadFields + @" FROM bx_Threads WITH(NOLOCK) WHERE ThreadStatus = 1 ORDER BY " + GetSortField(sortType) + @" DESC;
";
                }

                query.CreateTopParameter("@TopCount", count);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    return new ThreadCollectionV5(reader);
                }
            }
        }


        /// <summary>
        /// 批量更新帖子点击数
        /// </summary>
        /// <param name="updateList"></param>
        public override void UpdateThreadViews(Dictionary<int, int> updateList)
        {
            if (updateList.Count < 1)
                return;

            Dictionary<int, List<int>> updateDetail = new Dictionary<int, List<int>>();
            foreach (KeyValuePair<int, int> pair in updateList)
            {
                int threadID = pair.Key;
                int views = pair.Value;

                List<int> threadIds;
                if (updateDetail.TryGetValue(views, out threadIds))
                {
                    threadIds.Add(threadID);
                }
                else
                {
                    threadIds = new List<int>();
                    threadIds.Add(threadID);
                    updateDetail.Add(views, threadIds);
                }
            }

            int i = 0;

            using (SqlQuery query = new SqlQuery())
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<int, List<int>> pair in updateDetail)
                {
                    int viewCount = pair.Key;
                    List<int> threadIds = pair.Value;

                    if (threadIds == null || threadIds.Count == 0)
                        continue;

                    else if (threadIds.Count == 1)
                    {
                        sb.Append("UPDATE bx_Threads SET TotalViews = TotalViews + @V").Append(i).Append(" WHERE ThreadID = @T").Append(i).Append(";");
                        query.CreateParameter<int>("V" + i, viewCount, SqlDbType.Int);
                        query.CreateParameter<int>("T" + i, threadIds[0], SqlDbType.Int);
                    }

                    else
                    {
                        sb.Append("UPDATE bx_Threads SET TotalViews = TotalViews + @V").Append(i).Append(" WHERE ThreadID IN (");

                        int j = 0;
                        foreach (int threadID in threadIds)
                        {
                            if (j > 0)
                                sb.Append(",");

                            string paramName = string.Concat("@T", i.ToString(), "_", j.ToString());

                            sb.Append(paramName);

                            query.CreateParameter<int>(paramName, threadID, SqlDbType.Int);

                            j++;
                        }
                        sb.Append(");");

                        query.CreateParameter<int>("V" + i, viewCount, SqlDbType.Int);
                        
                    }
                    
                    i++;
                }

                if (sb.Length == 0)
                    return;

                query.CommandText = sb.ToString();
                query.CommandType = CommandType.Text;
                //query.CommandTimeout = int.MaxValue;

                query.ExecuteNonQuery();
            }
        }



        public override bool JudgementThreads(IEnumerable<int> threadIds, int forumID, int judgementID)
        {
            string idsString = StringUtil.Join(threadIds);

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "UPDATE bx_Threads SET JudgementID=@JudgementID WHERE ThreadID IN (" + idsString + ") AND ForumID = @ForumID";
                query.CommandType = CommandType.Text;

                query.CreateParameter<int>("@JudgementID", judgementID, SqlDbType.Int);
                query.CreateParameter<int>("@ForumID", forumID, SqlDbType.Int);

                return query.ExecuteNonQuery() > 0;
            }
        }


        #region StoredProcedure bx_UpdatePostsShielded
        [StoredProcedure(Name = "bx_UpdatePostsShielded", Script = @"
CREATE PROCEDURE {name}
	@PostIDs        varchar(8000),
	@IsShielded     bit
AS
	SET NOCOUNT ON
	EXEC('UPDATE [bx_Posts] SET IsShielded='+@IsShielded+' WHERE PostID IN('+@PostIDs+') AND IsShielded<>'+@IsShielded)
")]
        #endregion
        public override bool UpdatePostsShielded(IEnumerable<int> postIDs, bool IsShielded)
        {

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_UpdatePostsShielded";
                query.CommandType = CommandType.StoredProcedure;

                query.CreateParameter<string>("@PostIDs", StringUtil.Join(postIDs), SqlDbType.VarChar, 8000);
                query.CreateParameter<bool>("@IsShielded", IsShielded, SqlDbType.Bit);

                query.ExecuteNonQuery();
            }

            return true;
        }




        #region StoredProcedure
        [StoredProcedure(Name = "bx_ReCountTopicsAndPosts", Script = @"
CREATE PROCEDURE {name}
    @RecountToday       bit,
    @RecountYestoday    bit,
    @Today              datetime,
    @Yestoday           datetime
AS BEGIN
    SET NOCOUNT ON;

    --更新昨日发帖数
    IF @RecountYestoday = 1 BEGIN
        DECLARE @YestodayTopics int;
        DECLARE @YestodayPosts int;
        SELECT @YestodayTopics=COUNT(*) FROM bx_Threads WITH(NOLOCK)  WHERE ThreadStatus<4 AND CreateDate>@Yestoday AND CreateDate<@Today;
        SELECT @YestodayPosts=COUNT(*) FROM bx_Posts WITH(NOLOCK)  WHERE SortOrder<4000000000000000 AND CreateDate>@Yestoday AND CreateDate<@Today;

        DECLARE @MaxPosts int
        SELECT top 1 @MaxPosts=MaxPosts FROM bx_Vars;

        IF @YestodayPosts > @MaxPosts
           UPDATE bx_Vars SET  @MaxPosts=@YestodayPosts,MaxPostDate='',LastResetDate=GETDATE(),YestodayPosts=@YestodayPosts,YestodayTopics=@YestodayTopics;
        ELSE
           UPDATE bx_Vars SET  LastResetDate=GETDATE(),YestodayPosts=@YestodayPosts,YestodayTopics=@YestodayTopics;
    END

    -- 更新今日发帖数
    
    IF @RecountToday = 1 BEGIN
        BEGIN TRANSACTION
        UPDATE bx_Forums SET TodayThreads=0,TodayPosts=0;
        IF(@@error<>0)
		        GOTO Cleanup;

        UPDATE bx_Forums SET TodayThreads = T.TotalCount FROM(
        SELECT COUNT(*) as TotalCount,ForumID FROM bx_Threads WITH(NOLOCK)  WHERE ThreadStatus<4 AND CreateDate>@Today GROUP BY ForumID
        ) AS T WHERE T.ForumID = bx_Forums.ForumID;
        IF(@@error<>0)
		        GOTO Cleanup;

        UPDATE bx_Forums SET TodayPosts = T.TotalCount FROM(
        SELECT COUNT(*) as TotalCount,ForumID FROM bx_Posts WITH(NOLOCK)  WHERE SortOrder<4000000000000000 AND CreateDate>@Today GROUP BY ForumID
        ) AS T WHERE T.ForumID = bx_Forums.ForumID;
        IF(@@error<>0)
		        GOTO Cleanup;
        

        COMMIT TRANSACTION
            RETURN 0;
        Cleanup:
            BEGIN
    	        ROLLBACK TRANSACTION
    	        RETURN (-1);
            END

    END
    
    RETURN 0;
END
")]
        #endregion
        public override bool ReCountTopicsAndPosts(bool recountToday, bool recountYestoday)
        {
            using (SqlQuery query = new SqlQuery())
            {
                DateTime date = DateTimeUtil.Now.Date;

                query.CommandText = "bx_ReCountTopicsAndPosts";
                query.CommandTimeout = int.MaxValue;
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<DateTime>("@Today", date, SqlDbType.DateTime);
                query.CreateParameter<DateTime>("@Yestoday", date.AddDays(-1), SqlDbType.DateTime);
                query.CreateParameter<bool>("@RecountToday", recountToday, SqlDbType.Bit);
                query.CreateParameter<bool>("@RecountYestoday", recountYestoday, SqlDbType.Bit);
                query.CreateParameter<int>("@ReturnValue", SqlDbType.Int, ParameterDirection.ReturnValue);
                query.ExecuteNonQuery();

                return query.Parameters["@ReturnValue"].Value.ToString() == "0";
            }
        }


        public override bool StartPostFullTextIndex()
        {
            string sql = @"
IF (SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))=1
BEGIN
	IF (SELECT DATABASEPROPERTY (db_name(),'IsFulltextEnabled'))<>1
		EXEC sp_fulltext_database 'enable';

	IF NOT EXISTS(SELECT * FROM sysfulltextcatalogs WHERE [name]='FTCatalog_bx_Posts')
		EXEC sp_fulltext_catalog 'FTCatalog_bx_Posts','create';
    IF NOT EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bx_Posts')
	BEGIN
		EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'create', @keyname=N'PK_bx_Posts', @ftcat=N'FTCatalog_bx_Posts';
		EXEC sp_fulltext_column @tabname=N'[bx_Posts]', @colname=N'Content', @action=N'add';
		EXEC sp_fulltext_column @tabname=N'[bx_Posts]', @colname=N'Subject', @action=N'add';

        EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'start_change_tracking';
	    EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'start_background_updateindex';
    END
	IF NOT EXISTS(SELECT * FROM sysfulltextcatalogs WHERE [name]='FTCatalog_bx_Threads')
		EXEC sp_fulltext_catalog 'FTCatalog_bx_Threads','create';
    IF NOT EXISTS(SELECT t.name FROM sysobjects t INNER JOIN sysfulltextcatalogs ftc ON ftc.ftcatid=objectproperty(t.id,'TableFulltextCatalogID') WHERE t.name='bx_Threads')
	BEGIN
		EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'create', @keyname=N'PK_bx_Threads', @ftcat=N'FTCatalog_bx_Threads';
		EXEC sp_fulltext_column @tabname=N'[bx_Threads]', @colname=N'Subject', @action=N'add';
	
        EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'start_change_tracking';
	    EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'start_background_updateindex';
    END	
	
    SELECT 1;
END
ELSE
    SELECT 0;";
            try
            {
                using (SqlQuery query = new SqlQuery())
                {
                    query.CommandType = CommandType.Text;
                    query.CommandText = sql;

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.Get<int>(0) == 1)
                                return true;
                            else
                                return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public override bool StopPostFullTextIndex()
        {
            string sql = @"
IF (SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))=1
BEGIN
    IF (SELECT DATABASEPROPERTY (db_name(),'IsFulltextEnabled'))=1 BEGIN
        EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'stop_change_tracking';
        EXEC sp_fulltext_table @tabname=N'[bx_Threads]', @action=N'stop_background_updateindex';
        EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'stop_change_tracking';
        EXEC sp_fulltext_table @tabname=N'[bx_Posts]', @action=N'stop_background_updateindex';
	    EXEC sp_fulltext_database 'disable';
    END
END";

            try
            {
                using (SqlQuery query = new SqlQuery())
                {
                    query.CommandText = sql;
                    query.CommandType = CommandType.Text;

                    query.ExecuteNonQuery();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion


        #region 搜索
        /// <summary>
        /// 返回null时表示不存在 searchID
        /// </summary>
        /// <param name="searchID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="totalCount"></param>
        /// <param name="keyword"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override void SearchTopics(Guid searchID, int pageSize, int pageNumber, SearchType searchType, int maxResult, List<int> canVisitForumIDs, List<int> allForumIDs, out int totalCount, out string keyword, out SearchMode mode, out ThreadCollectionV5 threads, out PostCollectionV5 posts)
        {
            threads = null;
            posts = null;
            totalCount = 0;
            keyword = null;
            mode = SearchMode.Subject;
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT * FROM bx_SearchPostResults WITH (NOLOCK) WHERE [ID]=@ID;
UPDATE bx_SearchPostResults SET UpdateDate = GETDATE() WHERE [ID]=@ID; 
";
                query.CommandType = CommandType.Text;
                query.CreateParameter<Guid>("@ID", searchID, SqlDbType.UniqueIdentifier);

                bool isDesc = true;
                List<int> threadIDs = new List<int>();
                List<int> postIDs = new List<int>();

                List<int> forumIDs = new List<int>();
                int targetUserID = 0;
                bool isBefore = false;
                DateTime? PostDate = null;


                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mode = (SearchMode)reader.Get<byte>("SearchMode");
                        keyword = reader.Get<string>("Keyword");
                        threadIDs = StringUtil.Split2<int>(reader.Get<string>("ThreadIDs"));
                        postIDs = StringUtil.Split2<int>(reader.Get<string>("PostIDs"));
                        isDesc = reader.Get<bool>("IsDesc");

                        forumIDs = StringUtil.Split2<int>(reader.Get<string>("ForumIDs"));
                        targetUserID = reader.Get<int>("TargetUserID");
                        isBefore = reader.Get<bool>("IsBefore");
                        PostDate = reader.GetNullable<DateTime>("PostDate");
                    }
                }
                if (keyword == null)
                    return;
                string[] keywords = keyword.Split(' ');

                List<string> resultKeywords = new List<string>();
                foreach (string word in keywords)
                {
                    string tempWord = DaoUtil.GetSafeString(word).Trim();
                    if (tempWord == string.Empty)
                        continue;

                    resultKeywords.Add(tempWord);
                }

                threads = new ThreadCollectionV5();
                posts = new PostCollectionV5();


                if (mode == SearchMode.Subject || mode == SearchMode.UserThread)
                {
                    if (threadIDs.Count == 0)
                    {
                        return;
                    }


                    totalCount = threadIDs.Count;


                    int start = pageSize * (pageNumber - 1);
                    int end = start + pageSize;

                    if (end > totalCount)
                        end = totalCount;

                    if (start >= end)
                        return;

                    threadIDs.Sort();

                    if (isDesc)
                        threadIDs.Reverse();

                    List<int> inThreadIDs = new List<int>();
                    for (int i = start; i < end; i++)
                    {
                        inThreadIDs.Add(threadIDs[i]);
                    }


                    query.CommandText = @"
DECLARE @ThreadIDTable table(ID int identity(1,1), TID int);
DECLARE @Rows int;

INSERT INTO @ThreadIDTable(TID)
    SELECT ThreadID FROM bx_Threads WITH(NOLOCK) WHERE ThreadID IN(@ThreadIDs) AND ThreadStatus<4 ORDER BY ThreadID;

SELECT @Rows = @@RowCount;

IF @Rows = @Count BEGIN --说明上次搜索结果中没有被删除的情况
    SELECT 1;
    SELECT * FROM bx_Threads WITH(NOLOCK) WHERE ThreadStatus<4 AND ThreadID in(@InThreadIDs) ORDER BY ThreadID " + (isDesc ? "DESC" : "ASC") + @";
END 
ELSE BEGIN
    SELECT 0;
    SELECT TID FROM @ThreadIDTable;
END
";
                    query.CreateParameter<int>("@Count", totalCount, SqlDbType.Int);
                    query.CreateInParameter<int>("@InThreadIDs", inThreadIDs);
                    query.CreateInParameter<int>("@ThreadIDs", threadIDs);

                    List<int> resultThreadIDs = new List<int>();
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        bool isResult = false;
                        while (reader.Read())
                        {
                            isResult = reader.Get<int>(0) == 1;
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                if (isResult)
                                    threads.Add(new BasicThread(reader));
                                else
                                    resultThreadIDs.Add(reader.GetInt32(0));
                            }
                        }

                        if (isResult)
                            return;
                    }
                    #region
                    if (threadIDs.Count > resultThreadIDs.Count)
                    {
                        int minID, maxID;
                        if (isDesc == false)
                        {
                            minID = threadIDs[0];
                            maxID = threadIDs[threadIDs.Count - 1];
                        }
                        else
                        {
                            maxID = threadIDs[0];
                            minID = threadIDs[threadIDs.Count - 1];
                        }

                        query.CommandType = CommandType.Text;
                        query.Parameters.Clear();


                        string tempKeyword;

                        string sql = GetSearchTopicsSql(query, false, forumIDs, canVisitForumIDs, allForumIDs, resultKeywords, targetUserID, mode, searchType, PostDate, isBefore, isDesc, maxResult - resultThreadIDs.Count, minID, maxID, out tempKeyword);

                        if (sql == null)
                        {
                            threads = new ThreadCollectionV5();
                            posts = new PostCollectionV5();
                            return;
                        }

                        query.CommandText = sql;

                        using (XSqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (isDesc)
                                    resultThreadIDs.Add(reader.GetInt32(0));
                            }
                        }

                        query.Parameters.Clear();
                        query.CommandText = @"
UPDATE bx_SearchPostResults SET ThreadIDs = @NewThreadIDs WHERE [ID]=@ID;
";
                        query.CommandType = CommandType.Text;

                        query.CreateParameter<string>("@NewThreadIDs", StringUtil.Join(resultThreadIDs), SqlDbType.Text);
                        query.CreateParameter<Guid>("@ID", searchID, SqlDbType.UniqueIdentifier);

                        query.ExecuteNonQuery();
                    }
                    #endregion

                    totalCount = resultThreadIDs.Count;


                    start = pageSize * (pageNumber - 1);
                    end = start + pageSize;

                    if (end > totalCount)
                        end = totalCount;

                    if (start >= end)
                        return;

                    if (isDesc)
                        resultThreadIDs.Reverse();

                    inThreadIDs = new List<int>();
                    for (int i = start; i < end; i++)
                    {
                        inThreadIDs.Add(resultThreadIDs[i]);
                    }

                    query.Parameters.Clear();
                    query.CommandType = CommandType.Text;
                    query.CommandText = @"
SELECT * FROM bx_Threads WITH(NOLOCK) WHERE ThreadStatus<4 AND ThreadID in(@ThreadIDs) ORDER BY ThreadID " + (isDesc ? "DESC" : "ASC") + @";
";
                    query.CreateInParameter<int>("@ThreadIDs", inThreadIDs);
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            threads.Add(new BasicThread(reader));
                        }
                    }
                }
                else
                {
                    if (postIDs.Count == 0)
                    {
                        return;
                    }

                    totalCount = postIDs.Count;


                    int start = pageSize * (pageNumber - 1);
                    int end = start + pageSize;

                    if (end > totalCount)
                        end = totalCount;

                    if (start >= end)
                        return;

                    postIDs.Sort();

                    if (isDesc)
                        postIDs.Reverse();

                    List<int> inPostIDs = new List<int>();
                    for (int i = start; i < end; i++)
                    {
                        inPostIDs.Add(postIDs[i]);
                    }

                    query.CommandText = @"
DECLARE @PostIDTable table(ID int identity(1,1), TID int);
DECLARE @Rows int;

INSERT INTO @PostIDTable(TID)
    SELECT PostID FROM bx_Posts WITH(NOLOCK) WHERE PostID IN(@PostIDs) AND SortOrder<4000000000000000 ORDER BY PostID;

SELECT @Rows = @@RowCount;

IF @Rows = @Count BEGIN --说明上次搜索结果中没有被删除的情况
    SELECT 1;
    SELECT * FROM bx_Posts WITH(NOLOCK) WHERE SortOrder<4000000000000000 AND PostID in(@InPostIDs) ORDER BY PostID " + (isDesc ? "DESC" : "ASC") + @";
END 
ELSE BEGIN
    SELECT 0;
    SELECT TID FROM @PostIDTable;
END
";
                    query.CreateParameter<int>("@Count", totalCount, SqlDbType.Int);
                    query.CreateInParameter<int>("@InPostIDs", inPostIDs);
                    query.CreateInParameter<int>("@PostIDs", postIDs);

                    List<int> resultPostIDs = new List<int>();
                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        bool isResult = false;

                        while (reader.Read())
                        {
                            isResult = reader.Get<int>(0) == 1;
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                if (isResult)
                                    posts.Add(new PostV5(reader));
                                else
                                    resultPostIDs.Add(reader.GetInt32(0));
                            }
                        }

                        if (isResult)
                            return;
                    }

                    if (postIDs.Count > resultPostIDs.Count)
                    {
                        int minID, maxID;
                        if (isDesc == false)
                        {
                            minID = postIDs[0];
                            maxID = postIDs[postIDs.Count - 1];
                        }
                        else
                        {
                            maxID = postIDs[0];
                            minID = postIDs[postIDs.Count - 1];
                        }

                        query.CommandType = CommandType.Text;
                        query.Parameters.Clear();

                        string tempKeyword;
                        string sql = GetSearchTopicsSql(query, false, forumIDs, canVisitForumIDs, allForumIDs, resultKeywords, targetUserID, mode, searchType, PostDate, isBefore, isDesc, maxResult - resultPostIDs.Count, minID, maxID, out tempKeyword);
                        if (sql == null)
                        {
                            threads = new ThreadCollectionV5();
                            posts = new PostCollectionV5();
                            return;
                        }

                        query.CommandText = sql;

                        using (XSqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (isDesc)
                                    resultPostIDs.Add(reader.GetInt32(0));
                            }
                        }

                        query.Parameters.Clear();
                        query.CommandText = @"
UPDATE bx_SearchPostResults SET PostIDs = @NewPostIDs WHERE [ID]=@ID;
";
                        query.CommandType = CommandType.Text;

                        query.CreateParameter<string>("@NewPostIDs", StringUtil.Join(resultPostIDs), SqlDbType.Text);
                        query.CreateParameter<Guid>("@ID", searchID, SqlDbType.UniqueIdentifier);

                        query.ExecuteNonQuery();
                    }

                    totalCount = resultPostIDs.Count;


                    start = pageSize * (pageNumber - 1);
                    end = start + pageSize;

                    if (end > totalCount)
                        end = totalCount;

                    if (start >= end)
                        return;

                    if (isDesc)
                        resultPostIDs.Reverse();

                    inPostIDs = new List<int>();
                    for (int i = start; i < end; i++)
                    {
                        inPostIDs.Add(resultPostIDs[i]);
                    }

                    query.Parameters.Clear();
                    query.CommandType = CommandType.Text;
                    query.CommandText = @"
SELECT * FROM bx_Posts WITH(NOLOCK) WHERE SortOrder<4000000000000000 AND PostID in(@PostIDs) ORDER BY PostID " + (isDesc ? "DESC" : "ASC") + @";
";
                    query.CreateInParameter<int>("@PostIDs", inPostIDs);

                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            posts.Add(new PostV5(reader));
                        }
                    }
                }
            }
        }


        public override Guid SearchTopics(int operatorUserID, string ip, List<int> forumIDs, List<int> canVisitForumIDs, List<int> allForumIDs, IEnumerable<string> keywords, int targetUserID, SearchMode mode, SearchType searchType, DateTime? postDate, bool isBefore, bool isDesc, int maxResultCount, int intervalTime)
        {
            using (SqlQuery query = new SqlQuery())
            {
                string sql = @"
IF @UserID > 0 BEGIN
    IF EXISTS(SELECT * FROM bx_SearchPostResults WITH (NOLOCK) WHERE UserID=@UserID AND CreateDate>@CreateDate) BEGIN
        SELECT 1;
        RETURN;
    END
    ELSE BEGIN
        SELECT 0;
    END
END
ELSE BEGIN
    IF EXISTS(SELECT * FROM bx_SearchPostResults WITH (NOLOCK) WHERE IP=@IP AND UserID=0 AND CreateDate>@CreateDate) BEGIN
        SELECT 1;
        RETURN;
    END
    ELSE BEGIN
        SELECT 0;
    END
END
";
                query.CreateParameter<DateTime>("@CreateDate", DateTimeUtil.Now.AddSeconds(0 - intervalTime), SqlDbType.DateTime);
                query.CreateParameter<int>("@UserID", operatorUserID, SqlDbType.Int);
                query.CreateParameter<string>("@IP", ip, SqlDbType.VarChar, 50);

                string keyword;

                sql = sql + GetSearchTopicsSql(query, true, forumIDs, canVisitForumIDs, allForumIDs, keywords, targetUserID, mode, searchType, postDate, isBefore, isDesc, maxResultCount, null, null, out keyword);

                List<int> threadIDs = new List<int>();
                List<int> postIDs = new List<int>();
                if (sql == null)
                {
                }
                else
                {
                    query.CommandText = sql;
                    query.CommandType = CommandType.Text;
                    query.CommandTimeout = 10;


                    using (XSqlDataReader reader = query.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.Get<int>(0) == 1)//超出搜索时间间隔
                                return Guid.Empty;
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                if (mode == SearchMode.Content || mode == SearchMode.UserPost || mode == SearchMode.TopicContent)
                                {
                                    int postID = reader.Get<int>("PostID");
                                    if (postIDs.Contains(postID) == false)
                                        postIDs.Add(postID);
                                }
                                else
                                {
                                    int threadID = reader.Get<int>("ThreadID");
                                    if (threadIDs.Contains(threadID) == false)
                                        threadIDs.Add(threadID);
                                }
                            }
                        }
                    }
                }
                query.CommandText = @"
INSERT INTO bx_SearchPostResults([ID],[UserID],[IP],[Keyword],[SearchMode],[IsDesc],[ThreadIDs],[PostIDs],[ForumIDs],[TargetUserID],[IsBefore],[PostDate])
                    VALUES(@ID,@UserID,@IP,@Keyword,@SearchMode,@IsDesc,@ThreadIDs,@PostIDs,@TempForumIDs,@TargetUserID,@IsBefore,@PostDate);
";
                Guid id = Guid.NewGuid();
                query.CommandType = CommandType.Text;
                query.Parameters.Clear();
                query.CreateParameter<Guid>("@ID", id, SqlDbType.UniqueIdentifier);
                query.CreateParameter<int>("@UserID", operatorUserID, SqlDbType.Int);
                query.CreateParameter<string>("@IP", ip, SqlDbType.VarChar, 50);
                query.CreateParameter<string>("@Keyword", keyword, SqlDbType.NVarChar, 200);
                query.CreateParameter<int>("@SearchMode", (int)mode, SqlDbType.TinyInt);
                query.CreateParameter<bool>("@IsDesc", isDesc, SqlDbType.Bit);
                query.CreateParameter<string>("@ThreadIDs", StringUtil.Join(threadIDs), SqlDbType.Text);
                query.CreateParameter<string>("@PostIDs", StringUtil.Join(postIDs), SqlDbType.Text);
                query.CreateParameter<string>("@TempForumIDs", StringUtil.Join(forumIDs), SqlDbType.Text);
                query.CreateParameter<int>("@TargetUserID", targetUserID, SqlDbType.Int);
                query.CreateParameter<bool>("@IsBefore", isBefore, SqlDbType.Bit);
                query.CreateParameter<DateTime?>("@PostDate", postDate, SqlDbType.DateTime);

                query.ExecuteNonQuery();

                return id;
            }
        }

        private string GetSearchTopicsSql(SqlQuery query, bool isFirstSearch, List<int> forumIDs, List<int> canVisitForumIDs, List<int> allForumIDs, IEnumerable<string> keywords, int targetUserID, SearchMode mode, SearchType searchType, DateTime? postDate, bool isBefore, bool isDesc, int maxResultCount, int? minId, int? maxId, out string keyword)
        {
            keyword = targetUserID.ToString();


            string condition = "";

            if (minId != null && maxId != null)
            {
                if (mode == SearchMode.Content || mode == SearchMode.UserPost || mode == SearchMode.TopicContent)
                {
                    if (isDesc)
                        condition += " (PostID<@MinID OR PostID>@MaxID) AND ";
                    else
                        condition += " PostID>@MaxID AND ";
                }
                else
                {
                    if (isDesc)
                        condition += " (ThreadID<@MinID OR ThreadID>@MaxID) AND ";
                    else
                        condition += " ThreadID>@MaxID AND ";
                }
                query.CreateParameter<int>("@MinID", minId.Value, SqlDbType.Int);
                query.CreateParameter<int>("@MaxID", maxId.Value, SqlDbType.Int);
            }

            List<int> tempForumIDs;
            if (isFirstSearch)
            {
                tempForumIDs = forumIDs;
            }
            else
            {
                if (forumIDs != null && forumIDs.Count > 0)
                {
                    tempForumIDs = new List<int>();
                    foreach (int temforumID in forumIDs)
                    {
                        if (canVisitForumIDs.Contains(temforumID))
                        {
                            tempForumIDs.Add(temforumID);
                        }
                    }

                    if (tempForumIDs.Count == 0)
                        return null;
                }
                else
                    tempForumIDs = null;
            }

            if (tempForumIDs != null && tempForumIDs.Count > 0)
            {
                if (tempForumIDs.Count * 2 > allForumIDs.Count)
                {
                    List<int> notInForumIDs = new List<int>();
                    foreach (int tempForumID in allForumIDs)
                    {
                        if (tempForumIDs.Contains(tempForumID) == false)
                            notInForumIDs.Add(tempForumID);
                    }

                    if (notInForumIDs.Count > 0)
                    {
                        condition = " ForumID not in(@ForumIDs) AND ";
                        query.CreateInParameter<int>("@ForumIDs", notInForumIDs);
                    }
                }
                else
                {
                    condition = " ForumID in(@ForumIDs) AND ";
                    query.CreateInParameter<int>("@ForumIDs", tempForumIDs);
                }
            }
            if (postDate != null)
            {
                if (isBefore)
                    condition += " CreateDate < @PostDate AND ";
                else
                    condition += " CreateDate > @PostDate AND ";

                query.CreateParameter<DateTime>("@PostDate", postDate.Value, SqlDbType.DateTime);
            }

            string orderby;
            if (isDesc)
            {
                if (mode == SearchMode.Content || mode == SearchMode.UserPost || mode == SearchMode.TopicContent)
                {
                    orderby = " ORDER BY PostID DESC ";
                }
                else
                {
                    orderby = " ORDER BY ThreadID DESC ";
                }
            }
            else
            {
                if (mode == SearchMode.Content || mode == SearchMode.UserPost || mode == SearchMode.TopicContent)
                    orderby = " ORDER BY PostID ASC ";
                else
                    orderby = " ORDER BY ThreadID ASC ";
            }



            string sql = string.Empty;

            if (mode == SearchMode.UserThread)
            {
                sql = sql + @"
    SELECT TOP(@TopCount) ThreadID FROM bx_Threads WITH(NOLOCK) WHERE PostUserID = @UserID AND " + condition + @" ThreadStatus<4 " + orderby + @";
";
                query.CreateParameter<int>("@UserID", targetUserID, SqlDbType.Int);
            }
            else if (mode == SearchMode.UserPost)
            {
                sql = sql + @"
    SELECT TOP(@TopCount) PostID FROM bx_Posts WITH(NOLOCK) WHERE UserID = @UserID AND " + condition + @" SortOrder<4000000000000000 " + orderby + @";
";
                query.CreateParameter<int>("@UserID", targetUserID, SqlDbType.Int);
            }
            else
            {
                #region keyword

                keyword = string.Empty;
                string keywordCondition = null;

                int i = 0;
                foreach (string word in keywords)
                {
                    string tempWord = DaoUtil.GetSafeString(word).Trim();
                    if (tempWord == string.Empty)
                        continue;

                    keyword += " " + word;

                    if (mode == SearchMode.Subject)
                    {
                        if (searchType == SearchType.LikeStatement)
                        {
                            condition += " Subject LIKE'%'+ @Word_" + i + " +'%' AND ";
                            //condition += " Subject LIKE'%" + tempWord + "%' AND ";
                            query.CreateParameter<string>("@Word_" + i, tempWord, SqlDbType.NVarChar, 20);
                        }
                        else
                            keywordCondition += "\"*" + tempWord + "*\" AND ";
                    }
                    else if (mode == SearchMode.Content || mode == SearchMode.TopicContent)
                    {
                        if (searchType == SearchType.LikeStatement)
                        {
                            condition += " [Content] LIKE'%'+ @Word_" + i + " +'%' AND ";
                            //condition += " [Content] LIKE'%" + tempWord + "%' AND ";
                            query.CreateParameter<string>("@Word_" + i, tempWord, SqlDbType.NVarChar, 20);
                        }
                        else
                            keywordCondition += "\"*" + tempWord + "*\" AND ";
                    }

                    i++;
                }

                keyword = keyword.Trim();

                if (keywordCondition != null)
                {
                    keywordCondition = keywordCondition.Substring(0, keywordCondition.Length - 4);

                    if (mode == SearchMode.Subject)
                        condition += " Contains([Subject], @keywordCondition) AND ";
                    //condition += " Contains([Subject],'" + keywordCondition + "') AND ";
                    else
                        condition += " Contains([Content], @keywordCondition) AND ";
                    //condition += " Contains([Content],'" + keywordCondition + "') AND ";

                    query.CreateParameter<string>("@keywordCondition", keywordCondition, SqlDbType.NVarChar, 100);
                }
                #endregion

                if (mode == SearchMode.Subject)
                {
                    sql = sql + @"
SELECT TOP(@TopCount) ThreadID FROM [bx_Threads] WITH(NOLOCK) WHERE " + condition + @" ThreadStatus<4 " + orderby + @";
";
                }
                else if (mode == SearchMode.TopicContent)
                {
                    sql = sql + @"
SELECT TOP(@TopCount) PostID FROM [bx_Posts] WITH(NOLOCK) WHERE PostType=1 AND " + condition + @" SortOrder<4000000000000000 " + orderby + @";
";
                }
                else if (mode == SearchMode.Content)
                {
                    sql = sql + @"
SELECT TOP(@TopCount) PostID FROM [bx_Posts] WITH(NOLOCK) WHERE " + condition + @" SortOrder<4000000000000000 " + orderby + @";
";
                }
            }


            query.CreateTopParameter("@TopCount", maxResultCount);


            return sql;

        }



        public override void CheckThreads(int operatorUserID, IEnumerable<int> threadIDs, out List<int> buyedIds, out List<int> repliedIDs)
        {
            buyedIds = new List<int>();
            repliedIDs = new List<int>();
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT DISTINCT ThreadID FROM bx_ThreadExchanges WITH (NOLOCK) WHERE ThreadID in(@ThreadIDs) AND UserID=@UserID;
SELECT DISTINCT ThreadID FROM bx_Posts WITH (NOLOCK) WHERE ThreadID in(@ThreadIDs) AND UserID=@UserID AND PostType<>1;
";
                query.CommandType = CommandType.Text;

                query.CreateInParameter<int>("@ThreadIDs", threadIDs);
                query.CreateParameter<int>("@UserID", operatorUserID, SqlDbType.Int);

                using (XSqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        buyedIds.Add(reader.Get<int>(0));
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            repliedIDs.Add(reader.Get<int>(0));
                        }
                    }
                }
            }
        }



        #endregion




        #region StoredProcedure
        [StoredProcedure(Name = "bx_SetThreadImage", Script = @"
CREATE PROCEDURE {name}
    @ThreadID       int,
    @AttachmentID   int,
    @ImageUrl       varchar(200),
    @ImageCount     int
AS BEGIN
    SET NOCOUNT ON;
    IF EXISTS(SELECT * FROM bx_ThreadImages WHERE ThreadID = @ThreadID)
        UPDATE bx_ThreadImages SET AttachmentID = @AttachmentID,ImageUrl = @ImageUrl, ImageCount = @ImageCount WHERE ThreadID = @ThreadID;
    ELSE
        INSERT INTO bx_ThreadImages(ThreadID,AttachmentID,ImageUrl,ImageCount)VALUES(@ThreadID,@AttachmentID,@ImageUrl,@ImageCount);
END
")]
        #endregion
        public override void SetThreadImage(int threadID, int attchmentID, string imageUrl, int imageCount)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_SetThreadImage";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);
                query.CreateParameter<int>("@AttachmentID", attchmentID, SqlDbType.Int);
                query.CreateParameter<int>("@ImageCount", imageCount, SqlDbType.Int);
                query.CreateParameter<string>("@ImageUrl", imageUrl, SqlDbType.VarChar, 200);

                query.ExecuteNonQuery();
            }
        }





        #region StoredProcedure
        [StoredProcedure(Name = "bx_DeleteThreadImage", Script = @"
CREATE PROCEDURE {name}
    @ThreadID       int
AS BEGIN
    SET NOCOUNT ON;
    DELETE bx_ThreadImages WHERE ThreadID = @ThreadID;
END
")]
        #endregion
        public override void DeleteThreadImage(int threadID)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = "bx_DeleteThreadImage";
                query.CommandType = CommandType.StoredProcedure;
                query.CreateParameter<int>("@ThreadID", threadID, SqlDbType.Int);

                query.ExecuteNonQuery();
            }
        }













        #region 关键字 =================================

        public override Revertable2Collection<PostV5> GetPostsWithReverters(IEnumerable<int> postIDs)
        {
            if (ValidateUtil.HasItems(postIDs) == false)
                return null;

            Revertable2Collection<PostV5> posts = new Revertable2Collection<PostV5>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	SubjectReverter = ISNULL(R.SubjectReverter, ''),
	ContentReverter = ISNULL(R.ContentReverter, '')
FROM 
	bx_Posts A WITH(NOLOCK)
LEFT JOIN 
	bx_PostReverters R WITH(NOLOCK) ON R.PostID = A.PostID
WHERE 
	A.PostID IN (@PostIDs)";

                query.CreateInParameter<int>("@PostIDs", postIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string subjectReverter = reader.Get<string>("SubjectReverter");
                        string contentReverter = reader.Get<string>("ContentReverter");

                        PostV5 post = new PostV5(reader);

                        posts.Add(post, subjectReverter, contentReverter);
                    }
                }
            }

            return posts;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Post_UpdatePostKeywords", Script = @"
CREATE PROCEDURE {name}
    @PostID            int,
    @KeywordVersion    varchar(32),
    @Subject           nvarchar(256),
    @SubjectReverter   nvarchar(4000),
    @Content           ntext,
    @ContentReverter   ntext
AS BEGIN

/* include : Procedure_UpdateKeyword2.sql

    {PrimaryKey} = PostID
    {PrimaryKeyParam} = @PostID


    {Table} = bx_Posts
    {Text1} = Subject
    {Text1Param} = @Subject

    {Text2} = Content
    {Text2Param} = @Content


    {RevertersTable} = bx_PostReverters
    {Text1Reverter} = SubjectReverter
    {Text1ReverterParam} = @SubjectReverter

    {Text2Reverter} = ContentReverter
    {Text2ReverterParam} = @ContentReverter

*/

END")]
        #endregion
        public override void UpdatePostKeywords(Revertable2Collection<PostV5> processlist)
        {
            string procedure = "bx_Post_UpdatePostKeywords";
            string table = "bx_Posts";
            string primaryKey = "PostID";

            SqlDbType text1_Type = SqlDbType.NVarChar; int text1_Size = 256;
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
                foreach (Revertable2<PostV5> item in processlist)
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


        public override RevertableCollection<BasicThread> GetThreadWithReverters(IEnumerable<int> threadIDs)
        {
            if (ValidateUtil.HasItems(threadIDs) == false)
                return null;

            RevertableCollection<BasicThread> threads = new RevertableCollection<BasicThread>();

            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
SELECT 
	A.*,
	SubjectReverter = ISNULL(R.SubjectReverter, '')
FROM 
	bx_Threads A WITH(NOLOCK)
LEFT JOIN 
	bx_ThreadReverters R WITH(NOLOCK) ON R.ThreadID = A.ThreadID
WHERE 
	A.ThreadID IN (@ThreadIDs)";

                query.CreateInParameter<int>("@ThreadIDs", threadIDs);

                using (XSqlDataReader reader = query.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string nameReverter = reader.Get<string>("SubjectReverter");

                        BasicThread thread = new BasicThread(reader);

                        threads.Add(thread, nameReverter);
                    }
                }
            }

            return threads;
        }

        #region StoredProcedure
        [StoredProcedure(Name = "bx_Thread_UpdateThreadKeywords", Script = @"
CREATE PROCEDURE {name}
    @ThreadID              int,
    @KeywordVersion        varchar(32),
    @Subject               nvarchar(256),
    @SubjectReverter       nvarchar(4000)
AS BEGIN

/* include : Procedure_UpdateKeyword.sql
    {PrimaryKey} = ThreadID
    {PrimaryKeyParam} = @ThreadID

    {Table} = bx_Threads
    {Text} = Subject
    {TextParam} = @Subject

    {RevertersTable} = bx_ThreadReverters
    {TextReverter} = SubjectReverter
    {TextReverterParam} = @SubjectReverter
    
*/

END")]
        #endregion
        public override void UpdateThreadKeywords(RevertableCollection<BasicThread> processlist)
        {
            string procedure = "bx_Thread_UpdateThreadKeywords";
            string table = "bx_Threads";
            string primaryKey = "ThreadID";

            SqlDbType text_Type = SqlDbType.NVarChar; int text_Size = 256;
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
                foreach (Revertable<BasicThread> item in processlist)
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





#if DEBUG

        [StoredProcedure(Name = "bx_Common_GetRecordsByPage", FileName = "v30\\bx_Common_GetRecordsByPage.sql")]
        public void t1()
        { }

        [StoredProcedure(Name = "bx_Common_GetRecordsByPageSQLString", FileName = "v30\\bx_Common_GetRecordsByPageSQLString.sql")]
        public void t2()
        { }

        [StoredProcedure(Name = "bx_DoCreateStat", FileName = "v30\\bx_DoCreateStat.sql")]
        public void t3()
        { }

        [StoredProcedure(Name = "bx_GetSortOrder", FileName = "v30\\bx_GetSortOrder.sql")]
        public void t4()
        { }

        [StoredProcedure(Name = "bx_Common_GetRecordsByPage_LongCondition", FileName = "v30\\bx_Common_GetRecordsByPage_LongCondition.sql")]
        public void t5()
        { }

        [StoredProcedure(Name = "bx_UpdateSortOrder", FileName = "v30\\bx_UpdateSortOrder.sql")]
        public void t6()
        { }

        [StoredProcedure(Name = "bx_GetDisabledTriggerForumIDs", FileName = "v30\\bx_GetDisabledTriggerForumIDs.sql")]
        public void t7()
        { }

        //[StoredProcedure(Name = "bx_AutoFinalQuestion", FileName = "v30\\bx_AutoFinalQuestion.sql")]
        //public void t8()
        //{ }

        [StoredProcedure(Name = "bx_GetPolemizePosts", FileName = "v30\\bx_GetPolemizePosts.sql")]
        public void t9()
        { }


         /// <summary>
        /// 提升主题
        /// </summary>
        /// <param name="threadID"></param>
        /// <returns></returns>
        [StoredProcedure(Name = "bx_SetPostFloor", Script = @"
CREATE PROCEDURE {name}
	@ThreadID int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Count int;

    DECLARE @T TABLE(id int identity(1,1),pid int);
    INSERT INTO @T(pid) SELECT PostID FROM bx_Posts WHERE ThreadID = @ThreadID ORDER BY SortOrder;

    SELECT @Count = @@rowcount;

    UPDATE bx_Posts SET FloorNumber = id FROM @T  WHERE bx_Posts.PostID = pid;
    UPDATE bx_Threads SET PostedCount = @Count WHERE ThreadID = @ThreadID;
END
")]
        public void t10()
        {
        }



        [StoredProcedure(Name = "bx_UpdateForumThreadCatalogsData", FileName = "v30\\bx_UpdateForumThreadCatalogsData.sql")]
        public void t11()
        { }
#endif
    }
}