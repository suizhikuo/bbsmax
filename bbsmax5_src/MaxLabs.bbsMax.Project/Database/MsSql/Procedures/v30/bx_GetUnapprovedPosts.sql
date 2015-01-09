-- =============================================
-- Author:		<sek>
-- Create date: <2007/2/8>
-- Description:	<>
-- =============================================
CREATE PROCEDURE [bx_GetUnapprovedPosts]
	@ForumID int
AS
	SET NOCOUNT ON 
	
	IF(@ForumID>0)
	begin
		SELECT * FROM [bx_Threads] WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadID IN(SELECT DISTINCT(ThreadID) FROM [bx_Posts] WHERE ForumID=@ForumID AND SortOrder >= 5000000000000000)
		SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID IN (SELECT ThreadID FROM [bx_Threads]  WITH(NOLOCK) WHERE ForumID=@ForumID) AND ForumID=@ForumID AND SortOrder >= 5000000000000000 ORDER BY ThreadID,SortOrder DESC
	end
	ELSE
	begin
		SELECT * FROM [bx_Threads] WITH(NOLOCK) WHERE ThreadID IN(SELECT DISTINCT(ThreadID) FROM [bx_Posts] WHERE SortOrder >= 5000000000000000)
		SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE SortOrder >= 5000000000000000 ORDER BY ThreadID,SortOrder DESC
	end


