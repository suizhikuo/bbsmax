-- =============================================
-- Author:		SEK
-- Create date: <2007/1/30>
-- Description:	<>
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByThreadType]
	@ForumID int,
	@ThreadType tinyint, 
	@PageIndex int,
	@PageSize int,
	@ReturnTotalThreads bit
AS
	SET NOCOUNT ON 
	DECLARE @Condition varchar(1000)
	SET @Condition='ForumID=' + str(@ForumID) + ' AND ThreadType=' + str(@ThreadType) + ' AND [SortOrder] < 4000000000000000';

	DECLARE @ReturnFields varchar(500);
	IF @ThreadType = 1 --投票
		SET @ReturnFields = N'*,IsClosed=( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )'
	ELSE IF @ThreadType = 2 --提问
		SET @ReturnFields = N'*,IsClosed=( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )'
	ELSE IF @ThreadType = 4 --
		SET @ReturnFields = N'*,IsClosed=( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )'
	ELSE
		SET @ReturnFields = N'*,IsClosed=0'

	DECLARE @ReturnValue INT,@TotalCount INT
	SELECT @TotalCount=COUNT(*) FROM bx_Threads WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadType=@ThreadType AND  [SortOrder] < 4000000000000000

	EXECUTE @ReturnValue = bx_Common_GetRecordsByPage @PageIndex, @PageSize, 'bx_Threads', @ReturnFields ,@Condition, N'[SortOrder]', 1,@TotalCount
	
	IF(@ReturnTotalThreads=1)
		SELECT @TotalCount
		
	IF @ReturnValue=1
		SELECT @ReturnValue
		
	RETURN


