CREATE PROCEDURE [bx_SearchStatement]
(
  @SearchText	nvarchar(1000),
  @SearchType   tinyint,
  @SearchMode	tinyint,
  @ForumIDs		varchar(8000),
  @PostUserID	int,
  @PageSize		int,
  @PageIndex	int,
  @Sort	tinyint
)
AS
BEGIN
		SET NOCOUNT ON;
	
	DECLARE @Condition nvarchar(4000);
	SET @Condition = N'';
	
	--data
	DECLARE @Datas nvarchar(50);
	SET @Datas	=N'bx_Threads t';
	DECLARE @SelectString nvarchar(256);
	SET @SelectString = ' t.* '--N't.ThreadID,t.Subject,t.PostNickName,t.LastPostNickName,t.ForumID,t.TotalViews,t.TotalReplies,t.PostUserID,t.CreateDate,t.UpdateDate';	


	
	IF @ForumIDs<>'' BEGIN
		--SET @Datas = @Datas + N',bbsMax_Forums f';
		IF @SearchMode=2 OR @SearchMode=4
			SET @Condition=' ForumID IN('+@ForumIDs+') AND '
		ELSE
			SET @Condition=' t.ForumID IN('+@ForumIDs+') AND '
	END
	 
	DECLARE @Order nvarchar(100)
	DECLARE @TempOrder nvarchar(100)
	IF @SearchMode=1 OR @SearchMode=0 BEGIN
		SET @Condition=@Condition+' t.SortOrder<4000000000000000 AND '
		SET @Order='t.ThreadID'
		SET @TempOrder = 'ThreadID'
	END
	ELSE IF @SearchMode=2 OR @SearchMode=4 BEGIN
		SET @Condition=@Condition+' p.SortOrder<4000000000000000 AND '
		--SET @Order='P.ThreadID'
		--SET @TempOrder = 'ThreadID'
		SET @Datas=N'bx_Posts p'
		SET @Order='PostID'
		SET @SelectString = ' p.* '
		SET @TempOrder = 'PostID'
	END

	

	IF @SearchType=1 BEGIN  --使用LIKE搜索--
		IF @SearchMode=1 --标题--
			SET @Condition=@Condition+' t.Subject LIKE''%'+@SearchText+'%'' '
		ELSE IF @SearchMode=0 BEGIN--按作者--

				SET @Condition=@Condition+' PostUserID='+STR(@PostUserID)
		END
		ELSE IF @SearchMode=4  BEGIN
			SET @Condition=@Condition+' UserID='+STR(@PostUserID)
		END
		ELSE IF @SearchMode=2 BEGIN
			
			SET @Condition = @Condition+' [Content] LIKE''%'+@SearchText+'%'' '

		END
	END	
	ELSE BEGIN
		IF @SearchMode=1 --标题--
			SET @Condition=@Condition + ' Contains(t.[Subject],''"*' + @SearchText + '*"'') ';
		ELSE IF @SearchMode=2 BEGIN
			--SET @Datas=@Datas + N',bbsMax_Posts p'

			SET @Condition = @Condition+' Contains([Content],''"*' + @SearchText + '*"'')'

		END
	END



	DECLARE @SQLString NVARCHAR(4000)--,@Count int

	DECLARE @TableName VARCHAR(256),@SelectFields varchar(512),@IsDesc bit,@SortField varchar(256)
	SET @TableName=@Datas;
	SET @SelectFields=@SelectString;
	SET @IsDesc=@Sort;
	SET @SortField=@Order

	DECLARE @WhereString1 nvarchar(800);
	DECLARE @WhereString2 nvarchar(800);
	IF @Condition IS NULL OR @Condition = N'' BEGIN
		SELECT @WhereString1 = N'';
		SELECT @WhereString2 = N'WHERE ';
	END
	ELSE BEGIN
		SELECT @WhereString1 = N'WHERE ' + @Condition;
		SELECT @WhereString2 = N'WHERE ' + @Condition + N' AND ';
	END

	IF @PageIndex = 0 BEGIN
		SELECT @SQLString = N'SELECT TOP ' + STR(@PageSize+1)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName + ' WITH (NOLOCK)
			' + @WhereString1 + '
			ORDER BY ' + @SortField;
		IF @IsDesc = 1
			SELECT @SQLString = @SQLString + ' DESC';	
	END
	ELSE BEGIN
	-----------------------------------------------
	
			DECLARE @TopCount INT,@SelectCount INT
			SET @TopCount=@PageSize * @PageIndex
			SET @SelectCount=@PageSize+1
		-------------------------------------------------
			IF @IsDesc = 1
				SELECT @SQLString = 'SELECT TOP ' + STR(@SelectCount)
				+ N' ' + @SelectFields
				+ N' FROM ' + @TableName+' WITH (NOLOCK) 
				' + @WhereString2 + @SortField + ' <
					(SELECT Min('+@TempOrder+') FROM 
						(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
							' + @WhereString1 + '
								ORDER BY ' + @SortField + ' DESC) AS IDX)
				ORDER BY ' + @SortField + ' DESC';
			ELSE
				SELECT @SQLString = 'SELECT TOP ' + STR(@SelectCount)
				+ N' ' + @SelectFields
				+ N' FROM ' + @TableName+' WITH (NOLOCK) 
				' + @WhereString2 + @SortField + ' >
					(SELECT Max('+@TempOrder+') FROM 
						(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
							' + @WhereString1 + '
								ORDER BY ' + @SortField + ' ASC) AS IDX)
				ORDER BY ' + @SortField + ' ASC';
		END
--select @SQLString
	EXEC (@SQLString);


END










