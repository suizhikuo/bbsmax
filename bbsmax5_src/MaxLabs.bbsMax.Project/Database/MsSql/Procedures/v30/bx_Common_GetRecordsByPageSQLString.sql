CREATE PROCEDURE [bx_Common_GetRecordsByPageSQLString]
	@PageIndex int,
	@PageSize int,
	@TableName varchar(256),
	@SelectFields varchar(8000) = '*', --查询字段，默认为 *
	@Condition nvarchar(4000) = N'',     --条件例如"DirectoryID=4"
	@SortField varchar(256) = '[SortOrder]',
	@IsDesc bit = 1, --是否倒序
	@TotalRecords int = -1,
	@ResetOrder bit output,----------- 1表示读取数据的时候 排序要反过来
	@SQLString  nvarchar(4000) output
AS
BEGIN
	SET NOCOUNT ON;

	--DECLARE @SQLString nvarchar(4000);
	DECLARE @WhereString1 nvarchar(4000);
	DECLARE @WhereString2 nvarchar(4000);
	--DECLARE @ResetOrder bit----------- 1表示读取数据的时候 排序要反过来
	IF @Condition IS NULL OR @Condition = N'' BEGIN
		SELECT @WhereString1 = N'';
		SELECT @WhereString2 = N'WHERE ';
	END
	ELSE BEGIN
		SELECT @WhereString1 = N'WHERE ' + @Condition;
		SELECT @WhereString2 = N'WHERE ' + @Condition + N' AND ';
	END

	IF @PageIndex = 0 BEGIN
		SELECT @SQLString = N'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName + ' WITH (NOLOCK)
			' + @WhereString1 + '
			ORDER BY ' + @SortField;
		IF @IsDesc = 1
			SELECT @SQLString = @SQLString + ' DESC';
		SET @ResetOrder=0	
	END
	ELSE BEGIN
	-----------------------------------------------
		SET @SQLString='';
		DECLARE @GetFromLast BIT
		IF @TotalRecords=-1
			SET @GetFromLast=0
		ELSE BEGIN
			DECLARE @TotalPage INT,@ResidualCount INT
			SET @ResidualCount=@TotalRecords%@PageSize
			IF @ResidualCount=0
				SET @TotalPage=@TotalRecords/@PageSize
			ELSE
				SET @TotalPage=@TotalRecords/@PageSize+1
			IF @PageIndex>@TotalPage/2 --从最后页算上来
				SET @GetFromLast=1
			ELSE
				SET @GetFromLast=0
	-------------------------------------------------
	-------------------------------------------------
			IF @GetFromLast=1 BEGIN
				IF @PageIndex=@TotalPage-1 BEGIN
					IF @ResidualCount=0
						SET @ResidualCount=@PageSize;
					SELECT @SQLString = N'SELECT top ' + STR(@ResidualCount)
						+ N' ' + @SelectFields
						+ N' FROM ' + @TableName + ' WITH (NOLOCK)
						' + @WhereString1 + '
						ORDER BY ' + @SortField;
					IF @IsDesc = 0--反过来
						SELECT @SQLString = @SQLString + ' DESC';
					--EXEC (@SQLString);
					SET @ResetOrder=1
				END 
				ELSE IF  @PageIndex>@TotalPage-1 BEGIN --已经超过最大页数
					SELECT @SQLString = N'SELECT ' + @SelectFields
						+ N' FROM ' + @TableName + ' WITH (NOLOCK) WHERE 1=2'
					--EXEC (@SQLString);
					SET @ResetOrder=0
				END
				ELSE BEGIN
					SET @PageIndex=@TotalPage-(@PageIndex+1)---
					IF @IsDesc=1
						SET @IsDesc=0
					ELSE
						SET @IsDesc=1
					SET @ResetOrder=1
				END  
			END
			ELSE 
				SET @ResetOrder=0
		END
		
		IF @SQLString='' BEGIN
			DECLARE @TopCount INT
			IF @GetFromLast=1 BEGIN
				IF @ResidualCount > 0
					SET @TopCount=@PageSize * (@PageIndex-1)+@ResidualCount;
				ELSE
					SET @TopCount=@PageSize * (@PageIndex)+@ResidualCount;
				IF @TopCount = 0
					SET @TopCount = @PageSize;
			END
			ELSE
				SET @TopCount=@PageSize * @PageIndex
		-------------------------------------------------
			IF @IsDesc = 1
				SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
				+ N' ' + @SelectFields
				+ N' FROM ' + @TableName+' WITH (NOLOCK) 
				' + @WhereString2 + @SortField + ' <
					(SELECT Min(' + @SortField + ') FROM 
						(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
							' + @WhereString1 + '
								ORDER BY ' + @SortField + ' DESC) AS IDX)
				ORDER BY ' + @SortField + ' DESC';
			ELSE
				SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
				+ N' ' + @SelectFields
				+ N' FROM ' + @TableName+' WITH (NOLOCK) 
				' + @WhereString2 + @SortField + ' >
					(SELECT Max(' + @SortField + ') FROM 
						(SELECT TOP ' + STR(@TopCount) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
							' + @WhereString1 + '
								ORDER BY ' + @SortField + ' ASC) AS IDX)
				ORDER BY ' + @SortField + ' ASC';
		END
	END
	--EXEC (@SQLString);
	--RETURN @SQLString

END

