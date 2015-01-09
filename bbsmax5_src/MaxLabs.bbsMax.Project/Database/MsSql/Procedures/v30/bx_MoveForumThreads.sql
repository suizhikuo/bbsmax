-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/9>
-- Description:	<合并版块时移动帖子>
-- =============================================
CREATE PROCEDURE [bx_MoveForumThreads]
	@OldForumID int,
	@NewForumID int,
	@MoveCount int
AS
BEGIN
	SET NOCOUNT ON 
	EXEC (N'Update bx_Threads SET ForumID='+@NewForumID+' WHERE ThreadID IN (SELECT TOP ' + @MoveCount + N' ThreadID FROM bx_Threads WITH (NOLOCK) WHERE ForumID = ' + @OldForumID + N')');
	RETURN @@ROWCOUNT;
END


