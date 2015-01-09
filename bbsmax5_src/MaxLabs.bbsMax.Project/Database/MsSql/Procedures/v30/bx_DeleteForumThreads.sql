-- =============================================
-- Author:		zzbird
-- Create date: 2007/1/4
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_DeleteForumThreads]
	@ForumID int,
	@DeleteCount int
AS
BEGIN
	SET NOCOUNT ON;

	EXEC (N'DELETE bx_Threads WHERE ThreadID IN (SELECT TOP ' + @DeleteCount + N' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ForumID = ' + @ForumID + N')');
	RETURN @@ROWCOUNT;

END


