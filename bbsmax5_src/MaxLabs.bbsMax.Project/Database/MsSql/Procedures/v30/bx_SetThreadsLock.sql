-- =============================================
-- Author:		SEK
-- Create date: <2007/1/4>
-- Description:	<锁定主题>
-- =============================================
CREATE PROCEDURE [bx_SetThreadsLock]
	@ForumID int,
	@ThreadIdentities varchar(8000),
	@LockThread bit
AS
	SET NOCOUNT ON 
	EXEC ('UPDATE bx_Threads SET IsLocked='+@LockThread+' WHERE ThreadID IN (' + @ThreadIdentities + ') AND ForumID = ' + @ForumID) 
	RETURN


