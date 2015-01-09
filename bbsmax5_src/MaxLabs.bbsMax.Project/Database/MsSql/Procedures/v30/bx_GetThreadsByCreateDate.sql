-- =============================================
-- Author:		<zzbird>
-- Create date: <2006/11/8>
-- Description:	<获取版块的正常主题按发表时间排序 @ForumID<1时 获取全部版块正常主题>
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByCreateDate]
	@ForumIDs varchar(8000), --为''时获取所有版块
    @PageIndex int,
    @PageSize int,
    @TotalThreads int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Condition Nvarchar(1000)
	IF @ForumIDs <> ''
		SET @Condition = ' ForumID IN('+@ForumIDs+') AND SortOrder<4000000000000000 '
	ELSE
		SET @Condition = ' SortOrder<4000000000000000 '
		
	DECLARE @ReturnFields varchar(8000);
	SET @ReturnFields = N'*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END'
		
	DECLARE @ReturnValue INT
	EXECUTE @ReturnValue = bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[ThreadID]', 1, @TotalThreads

	IF @ReturnValue=1
		SELECT @ReturnValue
		
END
		
