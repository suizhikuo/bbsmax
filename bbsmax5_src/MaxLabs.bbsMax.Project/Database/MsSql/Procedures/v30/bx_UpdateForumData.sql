-- =============================================
-- Author:		zzbird
-- Create date: 2006/12/31
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_UpdateForumData]
	@ForumID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TotalThreads int;
	DECLARE @TotalPosts int,@LastThreadID INT--@TodayThreads int,@TodayPosts int;

    -- Insert statements for procedure here
	SELECT @TotalThreads = COUNT(*) FROM [bx_Threads] WITH (NOLOCK) WHERE [ForumID] = @ForumID AND [SortOrder] < 4000000000000000;
	SELECT @TotalPosts = COUNT(*) FROM [bx_Posts] WITH (NOLOCK)  WHERE ForumID = @ForumID AND [SortOrder] < 4000000000000000;
	--SELECT @TodayThreads=COUNT(*) FROM [bx_Threads] WITH(NOLOCK) WHERE [ForumID] = @ForumID AND ThreadID>(SELECT YestodayLastThreadID FROM bx_Forums WITH(NOLOCK) WHERE ForumID=@ForumID)
	--SELECT @TodayPosts=COUNT(*) FROM [bx_Posts] P WITH (NOLOCK) WHERE P.ForumID = @ForumID AND P.PostID>(SELECT YestodayLastPostID FROM bx_Forums WITH(NOLOCK) WHERE ForumID=@ForumID)
	SELECT @LastThreadID=ISNULL(ThreadID,0) FROM [bx_Posts] WITH(NOLOCK) WHERE PostID = (SELECT ISNULL(MAX(PostID),0) FROM [bx_Posts] T1 WITH (NOLOCK) WHERE T1.ForumID=@ForumID AND T1.SortOrder < 4000000000000000)
	
	UPDATE [bx_Forums] SET [TotalThreads] = @TotalThreads, [TotalPosts] = @TotalPosts, [LastThreadID]=ISNULL(@LastThreadID,0) WHERE [ForumID] = @ForumID;
	
	SELECT * FROM bx_Forums WITH (NOLOCK) WHERE ForumID = @ForumID;
END


