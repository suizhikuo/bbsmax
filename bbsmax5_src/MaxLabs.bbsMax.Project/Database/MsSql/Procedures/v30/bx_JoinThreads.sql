-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/31>
-- Description:	<合并主题>
-- =============================================
CREATE PROCEDURE [bx_JoinThreads]
	@OldThreadID int,
	@NewThreadID int,
	@IsKeepLink bit
AS
	SET NOCOUNT ON 
	IF EXISTS (SELECT * FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID AND ThreadType<10 AND SortOrder<4000000000000000)
	BEGIN
		DECLARE @NewForumID int,@OldForumID int,@OldThreadCatalogID int
		DECLARE @TotalReplies int,@TotalViews int,@LastPostUserID int,@LastPostNickName nvarchar(64)
		SELECT @OldForumID=ForumID,@OldThreadCatalogID=ThreadCatalogID,@TotalReplies=TotalReplies+1,@TotalViews=TotalViews FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@OldThreadID
		SELECT @NewForumID=ForumID FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID
		
		DECLARE @E1 int,@E2 int,@E3 int,@E4 int
		BEGIN TRANSACTION
		UPDATE [bx_Posts] SET PostType = 0 WHERE ThreadID=@OldThreadID AND PostType = 1
		SELECT @E4=@@error
		
		UPDATE [bx_Posts] SET ForumID=@NewForumID, ThreadID=@NewThreadID WHERE ThreadID=@OldThreadID
		SELECT @E1=@@error
		

		SELECT top 1 @LastPostUserID=UserID,@LastPostNickName=NickName FROM [bx_Posts] WITH (NOLOCK) WHERE ForumID=@NewForumID AND ThreadID=@NewThreadID AND SortOrder<4000000000000000 order by PostID DESC
		
		UPDATE [bx_Threads] SET TotalReplies=TotalReplies+@TotalReplies,TotalViews=TotalViews+@TotalViews,LastPostUserID=@LastPostUserID,LastPostNickName=@LastPostNickName WHERE ThreadID=@NewThreadID
		SELECT @E2=@@error
		
		IF (@IsKeepLink=1)
			BEGIN
			UPDATE [bx_Threads] SET Subject=(CAST(@NewThreadID as nvarchar(16))+N','+Subject),ThreadType=11 WHERE ThreadID=@OldThreadID
			SELECT @E3=@@error
			END
		ELSE
			BEGIN
			DELETE [bx_Threads] WHERE ThreadID=@OldThreadID
			SELECT @E3=@@error
			END
			
		IF(@E1=0 AND @E2=0 AND @E3=0 AND @E4=0)
			BEGIN
			COMMIT TRANSACTION
			--更新bx_Forums--
			--SELECT @NewForumID=ForumID FROM [bx_Threads] WITH (NOLOCK) WHERE ThreadID=@NewThreadID
			--EXECUTE bx_UpdateForumData @OldForumID
			IF(@OldForumID<>@NewForumID) BEGIN
				IF (@IsKeepLink=1) BEGIN
					UPDATE [bx_Forums] SET [TotalPosts] = TotalPosts-@TotalReplies WHERE [ForumID] = @OldForumID;
				--END ELSE BEGIN
					--UPDATE [bx_Forums] SET [TotalThreads] = TotalThreads-1,[TotalPosts] = TotalPosts-@TotalReplies,LastThreadID = (SELECT ISNULL(MAX(ThreadID),0) FROM [bx_Threads] WITH(NOLOCK) WHERE ForumID=@OldForumID  AND SortOrder<4000000000000000) WHERE [ForumID] = @OldForumID;
					--EXECUTE bx_UpdateForumThreadCatalogsData @OldForumID,@OldThreadCatalogID
				END
				UPDATE [bx_Forums] SET [TotalPosts] = TotalPosts+@TotalReplies WHERE [ForumID] = @NewForumID;
			END
			--ELSE BEGIN
				--IF (@IsKeepLink=0) BEGIN
					--UPDATE [bx_Forums] SET [TotalThreads] = TotalThreads-1,LastThreadID = (SELECT ISNULL(MAX(ThreadID),0) FROM [bx_Threads] WITH(NOLOCK) WHERE ForumID=@OldForumID AND SortOrder<4000000000000000) WHERE [ForumID] = @OldForumID;
					--EXECUTE bx_UpdateForumThreadCatalogsData @OldForumID,@OldThreadCatalogID
				--END
			--END
			
			RETURN (0)
			END
		ELSE
			BEGIN
			ROLLBACK TRANSACTION
			RETURN (-1)
			END
	END
	ELSE
		RETURN (-1)


