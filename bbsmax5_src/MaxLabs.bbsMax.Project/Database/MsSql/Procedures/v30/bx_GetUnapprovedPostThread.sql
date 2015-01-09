-- =============================================
-- Author:		<SEK>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetUnapprovedPostThread]
	@ThreadID int,
	@UserID int,
	@PageIndex int,
	@PageSize int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *,IsClosed=CASE
WHEN ThreadType=1 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polls p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=2 THEN ( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID )
WHEN ThreadType=4 THEN ( SELECT CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END FROM bx_Polemizes p1 WITH (NOLOCK) WHERE p1.ThreadID = bx_Threads.ThreadID )
ELSE 0
END
	FROM [bx_Threads] WHERE ThreadID=@ThreadID


    DECLARE @Condition varchar(8000),@User nvarchar(100)
	IF @UserID>=0
		SET @User='UserID='+str(@UserID)+' AND '
	ELSE
		SET @User=''

	SET @Condition=' ('+@User+' ThreadID='+str(@ThreadID)+' AND SortOrder >= 5000000000000000)'
	EXECUTE bx_Common_GetRecordsByPage @PageIndex,@PageSize,N'bx_Posts',N'*',@Condition,N'[SortOrder]',0
	
	IF @UserID>0
		SELECT COUNT(*) FROM bx_Posts WHERE UserID = @UserID AND ThreadID= @ThreadID AND SortOrder >= 5000000000000000;
	ELSE
		SELECT COUNT(*) FROM bx_Posts WHERE ThreadID= @ThreadID AND SortOrder >= 5000000000000000;
END


