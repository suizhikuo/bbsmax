-- =============================================
-- Author:		<sek>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetMyThreads]
	@UserID int,
	@IsApproved bit,
	@PageIndex int,
	@PageSize int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Condition varchar(8000)
	IF @IsApproved=1
		SET @Condition='PostUserID='+str(@UserID)+' AND [SortOrder]<4000000000000000'
	ELSE
		SET @Condition='PostUserID='+str(@UserID)+' AND [SortOrder] >= 5000000000000000'

	DECLARE @ReturnFields varchar(8000);
	SET @ReturnFields = N'*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END'

	EXECUTE bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[SortOrder]', 1

	IF @IsApproved=1
		SELECT COUNT(1) FROM bx_Threads WHERE PostUserID=@UserID AND [SortOrder]<4000000000000000
	ELSE
		SELECT COUNT(1) FROM bx_Threads WHERE PostUserID=@UserID  AND [SortOrder] >= 5000000000000000
END


