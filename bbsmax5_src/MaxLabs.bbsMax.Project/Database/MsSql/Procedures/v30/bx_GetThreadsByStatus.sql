-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetThreadsByStatus]--[bx_GetThreadsInSystemForum]
	@ForumID int,
	@Status tinyint,--1正常,2置顶,3总置顶,4待审核,5回收站
	@PageIndex int,
	@PageSize int,
	@GetTotalThreadsCount bit
AS
BEGIN
	SET NOCOUNT ON;
	
	--DECLARE @MinSortOrder bigint,@MaxSortOrder bigint
	--SET @MinSortOrder=@ThreadLocation*1000000000000000
	--SET @MaxSortOrder=(@ThreadLocation+1)*1000000000000000

	DECLARE @Condition nvarchar(1000);
	
	IF(@ForumID=0) 
		SET @Condition = N'';
	ELSE
		SET @Condition = N'[ForumID]=' + STR(@ForumID)+' AND ';
		
	IF(@Status=5)
		SET @Condition=@Condition+' SortOrder>'+cast(@Status*1000000000000000 as char(16))
	ELSE
		SET @Condition=@Condition+' SortOrder>'+cast(@Status*1000000000000000 as char(16))+' AND SortOrder<'+cast((@Status+1)*1000000000000000 as char(16))

	DECLARE @ReturnFields varchar(8000);
	SET @ReturnFields = N'*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END'

	DECLARE @ReturnValue INT,@TotalCount INT,@SQLString NVARCHAR(4000)
	SET @SQLString=N'SELECT @TotalCount=COUNT(ThreadID) FROM [bx_Threads] WITH (NOLOCK) WHERE '+@Condition
	EXECUTE sp_executesql @SQLString,N'@TotalCount int output',@TotalCount output
	
	EXECUTE @ReturnValue = bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[SortOrder]', 1,@TotalCount ;

	IF(@GetTotalThreadsCount=1)
		SELECT @TotalCount
	IF @ReturnValue=1
		SELECT @ReturnValue
END


