-- =============================================
-- Author:		zzbird
-- Create date: 2006/12/30
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_CreateThread]
	@ForumID int,
	@ThreadCatalogID int,
	@ThreadStatus tinyint,
	@ThreadType tinyint,
	@IconID int,
	@Subject nvarchar(256),
	@SubjectStyle nvarchar(300),
	@Price int,
	@UserID int,
	@NickName nvarchar(64),
	@IsLocked bit,
	@IsValued bit,

	@Content ntext,
	@ContentFormat tinyint,
	@EnableSignature bit,
--	@EnableEmoticons bit,
--	@EnableHTML bit,
--	@EnableSafeHTML bit,
--	@EnableMaxCode bit,
	@EnableReplyNotice bit,
	@IPAddress nvarchar(64),
	
	@AttachmentIds varchar(8000),
	@AttachmentFileNames ntext,
	@AttachmentFileIds text,
	@AttachmentFileSizes varchar(8000),
	@AttachmentPrices varchar(8000),
	@AttachmentFileExtNames ntext,
	@HistoryAttachmentIDs varchar(500),


	@ThreadRandNumber int,
	@ThreadID int output,
	@PostID int output,
	@UserTotalThreads int output,
	@UserTotalPosts int output

AS
BEGIN

	SET NOCOUNT ON;

	--DECLARE @NewThreadID     int;
	DECLARE @ReturnValue     int;
	--DECLARE @PostForumID int;
	

	DECLARE @TempSortOrder BIGINT,@PostDate datetime

	SET @PostDate = getdate();
	EXEC [bx_GetSortOrder] @ThreadStatus, @ThreadRandNumber, @PostDate, @TempSortOrder OUTPUT;

	--DECLARE @PostType tinyint;
	--BEGIN TRANSACTION
	
	--IF @IsApproved = 1 --BEGIN
		--SET @PostType=1
	--ELSE
		--SET @PostType=4
		--SET @PostForumID=@ForumID
		INSERT INTO [bx_Threads]
				([ForumID]
				,[ThreadCatalogID]
				,[ThreadType]
				,[IconID]
				,[Subject]
				,[SubjectStyle]
				,[Price]
				,[PostUserID]
				,[PostNickName]
				,[LastPostUserID]
				,[LastPostNickName]
				,[IsLocked]
				,[IsValued]
				,[SortOrder])
		 VALUES
				(@ForumID
				,@ThreadCatalogID
				,@ThreadType
				,@IconID
				,@Subject
				,@SubjectStyle
				,@Price
				,@UserID
				,@NickName
				,@UserID
				,@NickName
				,@IsLocked
				,@IsValued
				,@TempSortOrder )
	
		
	SELECT @ThreadID = @@IDENTITY;
	IF(@ThreadID>0)
	----------统计：增加主题数---------------
	EXEC bx_DoCreateStat @ForumID,3, 1
	--------------------------
	DECLARE @IsApproved bit
	IF @ThreadStatus=5
		SET @IsApproved=0
	ELSE
		SET @IsApproved=1
		
	EXECUTE @ReturnValue = [bx_CreatePost] 
		0
		,@ThreadID
		,1
		,@IconID
		,@Subject
		,@Content
		,@ContentFormat
		,@EnableSignature
--		,@EnableEmoticons
--		,@EnableHTML
--		,@EnableSafeHTML
--		,@EnableMaxCode
		,@EnableReplyNotice
		,@ForumID
		,@UserID
		,@NickName
		,@IPAddress
		--,@Attachments

		,@AttachmentIds
		,@AttachmentFileNames
		,@AttachmentFileIds
		,@AttachmentFileSizes
		,@AttachmentPrices
		,@AttachmentFileExtNames
		,@HistoryAttachmentIDs

		,0

		,@IsApproved
		,@ThreadRandNumber
		,@PostID output
		,0
		--,@Points
		,1

	--SELECT @PostID = @@IDENTITY;

	IF @PostID > 0 BEGIN
	----------统计：增加回复数---------------
		EXEC bx_DoCreateStat @ForumID,4, 1
		--------------------------
	END
	
	DECLARE @TempForumID int
	IF @ThreadStatus < 4 AND @UserID<>0 BEGIN
		SET @TempForumID=@ForumID;

		UPDATE [bx_Users]
		   SET [TotalTopics] = [TotalTopics] + 1
			  ,[TotalPosts] = [TotalPosts] + 1
			  ,[LastPostDate] = getdate()

		 WHERE UserID = @UserID;

    END
	ELSE
		SET @TempForumID=-2;

	UPDATE [bx_Forums]
		   SET [TotalThreads] = [TotalThreads] + 1
			  ,[TotalPosts] = [TotalPosts] + 1
			  ,[TodayThreads] = [TodayThreads] + 1
			  ,[TodayPosts] = [TodayPosts] + 1
			  ,[LastThreadID] = @ThreadID
		 WHERE [ForumID] = @TempForumID;


	UPDATE [bx_ThreadCatalogsInForums] SET TotalThreads=TotalThreads+1 WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID
	
	IF @UserID=0 BEGIN
		SET @UserTotalThreads = 0;
		SET @UserTotalPosts = 0;

	END
	ELSE
		SELECT @UserTotalThreads=[TotalTopics],
			@UserTotalPosts=[TotalPosts]

			FROM [bx_Users] WITH (NOLOCK) WHERE UserID = @UserID;


		RETURN (0);
		--IF @IsApproved=1
			--RETURN (0);
		--ELSE
			--RETURN 100

END


