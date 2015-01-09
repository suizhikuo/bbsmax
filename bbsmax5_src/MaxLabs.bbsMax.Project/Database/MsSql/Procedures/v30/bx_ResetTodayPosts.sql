CREATE PROCEDURE [bx_ResetTodayPosts]
AS
	SET NOCOUNT ON;
	UPDATE bx_Forums SET  TodayPosts = 0,  TodayThreads = 0;
	UPDATE bx_Forums SET 
		YestodayLastThreadID=(SELECT ISNULL(MAX(ThreadID),0) FROM [bx_Threads] WITH (NOLOCK) WHERE ForumID=T.ForumID AND SortOrder<4000000000000000),
		YestodayLastPostID=(SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] P WITH (NOLOCK) INNER JOIN [bx_Threads] T2 WITH(NOLOCK) ON P.ThreadID=T2.ThreadID WHERE T2.ForumID=T.ForumID AND P.SortOrder<4000000000000000 AND T2.SortOrder<4000000000000000)
		FROM [bx_Forums] T WHERE ForumID=T.ForumID
	


