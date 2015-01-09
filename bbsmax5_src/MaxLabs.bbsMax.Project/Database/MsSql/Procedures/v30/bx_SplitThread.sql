-- =============================================
-- Author:		SEK
-- Create date: <2006/12/31>
-- Description:	<分割主题>
-- =============================================
CREATE PROCEDURE [bx_SplitThread] 
	@ThreadID int,
	@PostIdentities varchar(8000),
	@NewSubject nvarchar(256)
AS
	SET NOCOUNT ON 
	DECLARE @SortOrder bigint,@NewThreadID int,@TotalReplies int,@PostUserID int,@PostNickName nvarchar(64),@LastPostUserID int,@LastPostNickName nvarchar(64)
	
	Declare @ForumID int,@ThreadCatalogID int
	SELECT @ForumID = ForumID,@ThreadCatalogID=ThreadCatalogID FROM bx_Threads WITH(NOLOCK) WHERE ThreadID=@ThreadID
	
	DECLARE @E1 int,@E2 int,@E3 int,@E4 int
	BEGIN TRANSACTION
	SELECT @SortOrder = ISNULL(MAX(SortOrder)+1,0) FROM bx_Threads WITH (NOLOCK) WHERE SortOrder<2000000000000000
	INSERT bx_Threads(ForumID,ThreadCatalogID,ThreadType,IconID,Subject,PostUserID,PostNickName,SortOrder) select ForumID,ThreadCatalogID,ThreadType,IconID,@NewSubject,PostUserID,PostNickName,@SortOrder from bx_Threads with(nolock) where ThreadID=@ThreadID
	SELECT @E1 = @@error
	SET @NewThreadID = @@IDENTITY
	
	EXEC ('UPDATE [bx_Posts] SET ThreadID=' + @NewThreadID + ' WHERE [PostID] IN (' + @PostIdentities + ') AND [ThreadID]=' + @ThreadID) 
	SELECT @E2 = @@error
		--SET @TotalReplies=@@ROWCOUNT-1--第一个Post是主题内容，不属于回复，所以减1
	
	--更新新主题--
	SELECT TOP 1 @PostUserID=UserID,@PostNickName=NickName FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@NewThreadID AND SortOrder<4000000000000000 ORDER BY PostID
	SELECT TOP 1 @LastPostUserID=UserID,@LastPostNickName=NickName FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@NewThreadID AND SortOrder<4000000000000000 ORDER BY PostID DESC
	
	UPDATE [bx_Posts] SET PostType=1 WHERE PostID=(SELECT MIN(PostID) FROM [bx_Posts] WHERE ForumID=@ForumID AND ThreadID=@NewThreadID)
	SELECT @TotalReplies = COUNT(*)-1 FROM [bx_Posts] WITH(NOLOCK) WHERE ForumID=@ForumID AND ThreadID=@NewThreadID AND SortOrder<4000000000000000--第一个Post是主题内容，不属于回复，所以减1
	UPDATE bx_Threads SET TotalReplies = @TotalReplies, TotalViews = @TotalReplies, PostUserID = @PostUserID, PostNickName = @PostNickName, LastPostUserID = @LastPostUserID, LastPostNickName = @LastPostNickName where ThreadID = @NewThreadID
	SELECT @E3 = @@error
	
	--更新原主题---
	SELECT TOP 1 @LastPostUserID = UserID, @LastPostNickName = NickName FROM [bx_Posts] WITH(NOLOCK)  WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000 ORDER BY PostID DESC
	DECLARE @OldTotalReplies int
	SELECT @OldTotalReplies = COUNT(*) - 1 FROM [bx_Posts] WITH (NOLOCK) WHERE ThreadID = @ThreadID AND SortOrder<4000000000000000 --第一个Post是主题内容，不属于回复，所以减1
	UPDATE bx_Threads SET TotalReplies = @OldTotalReplies, TotalViews = TotalViews - (@TotalReplies + 1), LastPostUserID = @LastPostUserID, LastPostNickName = @LastPostNickName WHERE ThreadID = @ThreadID
	SELECT @E4 = @@error
	
	IF(@E1 = 0 AND @E2 = 0 AND @E3 = 0 AND @E4 = 0)
	BEGIN
		COMMIT TRANSACTION
		UPDATE [bx_Forums] SET [TotalThreads] = TotalThreads+1, [TodayThreads]=TodayThreads+1,[LastThreadID]=@NewThreadID WHERE [ForumID] = @ForumID;
		EXECUTE bx_UpdateForumThreadCatalogsData @ForumID,@ThreadCatalogID
		--EXECUTE bx_UpdateForumData @ForumID
		RETURN (0)
	END
	ELSE
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (-1)
	END


