-- =============================================
-- Author:		<sek>
-- Create date: <Create Date,,>
-- Description:	<我参与的主题>
-- =============================================
CREATE PROCEDURE [bx_GetMyParticipantThreads]
	@UserID int,
	@PageIndex int,
	@PageSize int
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Condition varchar(8000)
	SET @Condition='ThreadID in (SELECT DISTINCT ThreadID FROM bx_Posts WHERE UserID=' + str(@UserID) + ' AND [SortOrder]<4000000000000000)'

	

	DECLARE @ReturnFields varchar(8000);
	SET @ReturnFields = N'*,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END'

	EXECUTE bx_Common_GetRecordsByPage @PageIndex, @PageSize, N'bx_Threads', @ReturnFields, @Condition, N'[SortOrder]', 1
	
	SELECT COUNT(distinct ThreadID) FROM bx_Posts WHERE UserID = @UserID AND [SortOrder]<4000000000000000
END


