CREATE PROCEDURE [bx_GetPostMarksBySeach]
	@PageIndex int,
	@PageSize int,
	@Keyword nvarchar(4000),
	@ActionType int
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @Condition varchar(8000)
	-------Modify By 帅帅 2007/3/20----------start
	IF(@Keyword='') BEGIN
			SET @Condition=''
	END
	ELSE BEGIN
		IF(@ActionType=1)--操作者
			SET @Condition='UserID in('+@Keyword+')'
		ELSE IF(@ActionType=2)--日期
			SET @Condition='CreateDate LIKE ''%'+@Keyword+'%'''
		ELSE IF(@ActionType=3)--被评分的帖子
		SET @Condition='PostID in (SELECT PostID FROM bx_Posts WITH (NOLOCK) WHERE [SortOrder]<4000000000000000 AND [Subject] like ''%' + @Keyword + '%'')'
		ELSE IF(@ActionType=4)--原因
		SET @Condition='Reason LIKE ''%'+@Keyword+'%'''
	END
	EXECUTE bx_Common_GetRecordsByPage_LongCondition @PageIndex,@PageSize,'bx_PostMarks','*','','PostMarkID',1
	IF(@Condition<>'')
		EXEC('SELECT COUNT(1) TotalRecord FROM bx_PostMarks WHERE ' +@Condition) 
	ELSE
		EXEC('SELECT COUNT(1) TotalRecord FROM bx_PostMarks')

END

