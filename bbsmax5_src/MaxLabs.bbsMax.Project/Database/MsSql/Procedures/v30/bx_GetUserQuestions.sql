-- =============================================
-- Author:		
-- Create date: 2008/5/13
-- Description:	<获取用户的问题帖子>
-- =============================================
CREATE PROCEDURE [bx_GetUserQuestions]
	@UserID int,
	@Count int,
	@ExceptThreadID int 
AS
	SET NOCOUNT ON;
	IF @ExceptThreadID>0
		EXEC('SELECT TOP '+ @Count +' *,IsClosed=( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID ) FROM bx_Threads WHERE ThreadType = 2 AND PostUserID='+@UserID+' AND ThreadID<>'+@ExceptThreadID+' AND [SortOrder] < 4000000000000000 ORDER BY ThreadID DESC')
	ELSE
		EXEC('SELECT TOP '+ @Count +' *,IsClosed=( SELECT CASE WHEN IsClosed=1 THEN 1 ELSE (CASE WHEN ExpiresDate < getdate() THEN 1 ELSE 0 END) END FROM bx_Questions q1 WITH (NOLOCK) WHERE q1.ThreadID = bx_Threads.ThreadID ) FROM bx_Threads WHERE ThreadType = 2 AND PostUserID='+@UserID+' AND [SortOrder] < 4000000000000000 ORDER BY ThreadID DESC')
	