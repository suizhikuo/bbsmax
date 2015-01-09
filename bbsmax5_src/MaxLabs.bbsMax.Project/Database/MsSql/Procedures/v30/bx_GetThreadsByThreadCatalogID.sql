-- =============================================
-- Author:		<zzbird>
-- Create date: <2006/11/8>
-- Description:	
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByThreadCatalogID]
	@ForumID int,
    @ThreadCatalogID int, 
    @PageIndex int,
    @PageSize int,
    @TotalThreads int
AS
BEGIN

    SET NOCOUNT ON;

	DECLARE @Condition nvarchar(100);
	SET @Condition = N'[ForumID]=' + CAST(@ForumID as varchar(10)) + N' AND [ThreadCatalogID]=' + CAST(@ThreadCatalogID as varchar(10)) + N' AND [SortOrder] < 4000000000000000';

	DECLARE @ReturnFields varchar(8000);
	SET @ReturnFields = N'*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END'

	DECLARE @ReturnValue INT--,@TotalCount INT
	--SELECT @TotalCount=COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [ThreadCatalogID] = @ThreadCatalogID AND [SortOrder] < 800000000000000
	
	EXECUTE @ReturnValue = bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[SortOrder]', 1,@TotalThreads

    --IF @ReturnTotalThreads = 1
        --SELECT @TotalCount
	IF @ReturnValue=1
		SELECT @ReturnValue


END


