-- =============================================
-- Author:		<SEK>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetUnapprovedPostThreads]
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
		SET @Condition='ThreadID in (SELECT DISTINCT P.ThreadID FROM [bx_Posts] P INNER JOIN [bx_Threads] T ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.SortOrder<4000000000000000)'
	ELSE
		SET @Condition='ThreadID in (SELECT DISTINCT P.ThreadID FROM [bx_Posts] P INNER JOIN [bx_Threads] T ON P.ThreadID=T.ThreadID WHERE '+@User+' T.ForumID='+str(@ForumID)+' AND P.SortOrder >= 5000000000000000 AND T.SortOrder<4000000000000000)'

	DECLARE @TotalCount INT,@SQLString NVARCHAR(4000)
	
	IF @ForumID=0
		SET @SQLString='SELECT @TotalCount=COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P INNER JOIN [bx_Threads] T ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.SortOrder<4000000000000000'
	ELSE
		SET @SQLString='SELECT @TotalCount=COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P INNER JOIN [bx_Threads] T ON P.ThreadID=T.ThreadID WHERE '+@User+' P.SortOrder >= 5000000000000000 AND T.ForumID='+str(@ForumID)+' AND T.SortOrder<4000000000000000'
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

		SELECT T.*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = T.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = T.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = T.ThreadID )
ELSE 0
END,
		(SELECT COUNT(1) FROM bx_Posts WHERE ThreadID=T.ThreadID  AND SortOrder >= 5000000000000000) as UnApprovedPostsCount FROM bx_Threads T  WHERE T.ThreadID in (SELECT ThreadID FROM @ThreadIDTable) ORDER BY SortOrder DESC')
		
	
	
	SELECT @TotalCount
	
		--IF @PageIndex = 0 BEGIN
			--SELECT @SQLString = 'SELECT TOP ' + CAST(@PageSize as varchar(16)) + ' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ' + @Condition + ' ORDER BY SortOrder DESC';
		--END
		--ELSE BEGIN
			--DECLARE @GetFromLast BIT
			--DECLARE @TotalPage INT,@ResidualCount INT
			--SET @ResidualCount=@TotalCount%@PageSize
			--IF @ResidualCount=0
				--SET @TotalPage=@TotalCount/@PageSize
			--ELSE
				--SET @TotalPage=@TotalCount/@PageSize+1
			--IF @PageIndex>@TotalPage/2 --从最后页算上来
				--SET @GetFromLast=1
			--ELSE
				--SET @GetFromLast=0
				
			--IF @GetFromLast=1 BEGIN
				--IF @PageIndex=@TotalPage-1 BEGIN
					--IF @ResidualCount=0
						--SET @ResidualCount=@PageSize;
					--SELECT @SQLString = N'SELECT TOP ' + STR(@ResidualCount)
						--+ ' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ' + @Condition + ' ORDER BY PostID DESC';
				--END 
				--ELSE IF @PageIndex>@TotalPage-1 ----已经超过最大页数
					--SELECT @SQLString = N'SELECT PostID FROM bx_Posts WITH (NOLOCK) WHERE 1=2';
				--ELSE BEGIN
					--SET @PageIndex=@TotalPage-(@PageIndex+1)---
				--END  
			--END
			--IF @SQLString = '' BEGIN
				--DECLARE @TopCount INT,@Order NVARCHAR(100)
				--IF @GetFromLast=1 BEGIN
					--SET @TopCount=@PageSize * (@PageIndex-1)+@ResidualCount
					--SELECT @SQLString = 'SELECT TOP ' + CAST(@PageSize as varchar(16)) + ' PostID FROM bx_Posts WITH (NOLOCK) WHERE ' + @Condition + ' AND PostID < (SELECT MIN(PostID) FROM (SELECT TOP ' + CAST(@TopCount AS varchar(16)) + ' PostID FROM bx_Posts WITH (NOLOCK) WHERE ' + @Condition + ' ORDER BY PostID DESC) AS IDX)  ORDER BY PostID DESC';
				--END
				--ELSE BEGIN
					--SET @TopCount=@PageSize * @PageIndex
					--SELECT @SQLString = 'SELECT TOP ' + CAST(@PageSize as varchar(16)) + ' PostID FROM bx_Posts WITH (NOLOCK) WHERE ' + @Condition + ' AND PostID > (SELECT Max(PostID) FROM (SELECT TOP ' + CAST(@TopCount AS varchar(16)) + ' PostID FROM bx_Posts WITH (NOLOCK) WHERE ' + @Condition + ' ORDER BY PostID) AS IDX)  ORDER BY PostID';
				--END
				
			--END
		--END	
/*
	CREATE TABLE #ThreadIDTable(ThreadID int NOT NULL)
	
	INSERT INTO #ThreadIDTable EXECUTE bx_Common_GetRecordsByPage @PageIndex,@PageSize,N'bx_Threads',N'ThreadID',@Condition,N'[SortOrder]',1,@TotalCount
	
	SELECT T.*,(SELECT COUNT(1) FROM bx_Posts WHERE ThreadID=T.ThreadID  AND ForumID=-2) as UnApprovedPostsCount FROM bx_Threads T  WHERE T.ThreadID in (SELECT ThreadID FROM #ThreadIDTable) ORDER BY SortOrder DESC
	
	DROP TABLE #ThreadIDTable
*/	
	--SELECT @TotalCount
/*	
	IF @ForumID=0
		EXEC('SELECT COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P INNER JOIN [bx_Threads] T ON P.ThreadID=T.ThreadID WHERE '+@User+' P.ForumID=-2 AND T.ForumID>0')
	ELSE
		EXEC('SELECT COUNT(DISTINCT P.ThreadID) FROM [bx_Posts] P INNER JOIN [bx_Threads] T ON P.ThreadID=T.ThreadID WHERE P.ForumID=-2 '+@User+' AND T.ForumID='+@ForumID)
*/
END


