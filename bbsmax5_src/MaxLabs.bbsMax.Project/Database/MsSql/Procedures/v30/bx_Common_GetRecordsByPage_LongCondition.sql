CREATE PROCEDURE bx_Common_GetRecordsByPage_LongCondition
	@PageIndex int,
	@PageSize int,
	@TableName nvarchar(256),
	@SelectFields varchar(8000) = N'*', --查询字段，默认为 *
	@Condition nvarchar(4000) = N'',     --条件例如"DirectoryID=4"
	@SortField nvarchar(256) = N'[SortOrder]',
	@IsDesc bit = 1 --是否倒序
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @SQLString nvarchar(4000);
	DECLARE @WhereString1 nvarchar(4000);
	DECLARE @WhereString2 nvarchar(4000);

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
	END
	ELSE BEGIN
		IF @IsDesc = 1
			SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName+' WITH (NOLOCK) 
			' + @WhereString2 + @SortField + ' <
				(SELECT Min(' + @SortField + ') FROM 
					(SELECT TOP ' + STR(@PageSize * @PageIndex) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
						' + @WhereString1 + '
							ORDER BY ' + @SortField + ' DESC) AS IDX)
			ORDER BY ' + @SortField + ' DESC';
		ELSE
			SELECT @SQLString = 'SELECT TOP ' + STR(@PageSize)
			+ N' ' + @SelectFields
			+ N' FROM ' + @TableName+' WITH (NOLOCK) 
			' + @WhereString2 + @SortField + ' >
				(SELECT Max(' + @SortField + ') FROM 
					(SELECT TOP ' + STR(@PageSize * @PageIndex) + ' ' + @SortField + ' FROM ' + @TableName+' WITH (NOLOCK)
						' + @WhereString1 + '
							ORDER BY ' + @SortField + ' ASC) AS IDX)
			ORDER BY ' + @SortField + ' ASC';
	END

	EXEC (@SQLString);

END


