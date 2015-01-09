-- =============================================
-- Author:		<zzbird>
-- Create date: <2006/11/8>
-- Description:	<获取版块的正常主题不包括全局置顶主题按SortOrder排序 @ForumID=0时 获取全部正常主题此时包括全局置顶主题并且按时间排序>
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByForumID]
    @ForumID int, 
    @PageIndex int,
    @PageSize int,
    @TotalThreads int
AS
BEGIN

    SET NOCOUNT ON;

	DECLARE @Condition varchar(1000);
	IF @ForumID = 0
		SET @Condition = N'[SortOrder] < 4000000000000000';
	ELSE  
		SET @Condition = N'[ForumID]=' + STR(@ForumID) + N' AND [SortOrder] < 3000000000000000';
		
	DECLARE @ReturnFields varchar(8000);
	SET @ReturnFields = N'*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END'
	
	DECLARE @ReturnValue INT
	IF @ForumID = 0
		EXECUTE @ReturnValue = bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[CreateDate]', 1, @TotalThreads
	ELSE
		EXECUTE @ReturnValue = bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[SortOrder]', 1, @TotalThreads

	IF @ReturnValue=1
		SELECT @ReturnValue
END


