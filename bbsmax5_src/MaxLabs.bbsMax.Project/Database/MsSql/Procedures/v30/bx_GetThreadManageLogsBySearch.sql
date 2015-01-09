-- =============================================
-- Author:		SEK
-- Create date: <2006/12/28>
-- Description:	<获取日志>
-- =============================================
CREATE PROCEDURE [bx_GetThreadManageLogsBySearch]
	@PageIndex int,
	@PageSize int,
	@Keyword nvarchar(4000),
	@ActionType tinyint,
	@ForumID int
AS
	SET NOCOUNT ON 
	DECLARE @Condition nvarchar(4000)
	-------Modify By 帅帅 2007/3/20----------start
		IF(@Keyword='') BEGIN
				SET @Condition=''
		   IF (@ForumID IS NOT NULL) SET @Condition = 'ForumID = '+ CAST(@ForumID AS NVARCHAR);
		END
		ELSE BEGIN
			IF(@ActionType=1)--操作者
			SET @Condition='UserID in('+@Keyword+')'
			ELSE IF(@ActionType=2)--IP地址
				SET @Condition='IPAddress LIKE ''%'+@Keyword+'%'''
			ELSE IF(@ActionType=3)--时间
			SET @Condition='CONVERT(NVARCHAR(20),CreateDate,120) LIKE ''%'+@Keyword+'%'''  --Modify by gyb 
			ELSE IF(@ActionType=4)--版面名称
			SET @Condition='ForumID in(SELECT ForumID FROM bx_Forums WITH(NOLOCK) WHERE ForumName Like ''%'+@Keyword+'%'')'
			ELSE IF(@ActionType=5)--主题标题
			SET @Condition='ThreadID in(SELECT ThreadID FROM bx_Threads WITH(NOLOCK) WHERE Subject Like ''%'+@Keyword+'%'')'
			ELSE IF(@ActionType=6)--动作
			SET @Condition='ActionType='+@Keyword
			ELSE IF(@ActionType=7)--原因
			SET @Condition='Reason LIKE ''%'+@Keyword+'%'''
			IF (@ForumID IS NOT NULL) SET @Condition = @Condition + ' AND ForumID = '+ CAST(@ForumID AS NVARCHAR);
		END
		
	-------Modify By 帅帅 2007/3/20----------end
	EXECUTE bx_Common_GetRecordsByPage_LongCondition @PageIndex,@PageSize,'bx_ThreadManageLogs','*',@Condition,'LogID',1
	DECLARE @S INT
	IF(@Condition<>'')
	EXEC('SELECT COUNT(1) TotalRecord FROM bx_ThreadManageLogs WHERE ' +@Condition) 
	ELSE
	EXEC('SELECT COUNT(1) TotalRecord FROM bx_ThreadManageLogs')

