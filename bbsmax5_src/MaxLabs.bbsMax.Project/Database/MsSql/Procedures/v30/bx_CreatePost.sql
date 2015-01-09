-- =============================================
-- Author:		zzbird
-- Create date: 2006/12/30
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_CreatePost]
	@ParentID int,
	@ThreadID int,
	@PostType tinyint,
	@IconID int,
	@Subject nvarchar(256),
	@Content ntext,
	@ContentFormat tinyint,
	@EnableSignature bit,
--	@EnableEmoticons bit,
--	@EnableHTML bit,
--	@EnableSafeHTML bit,
--	@EnableMaxCode bit,
	@EnableReplyNotice bit,
	@ForumID int,
	@UserID int,
	@NickName nvarchar(64),
	@IPAddress nvarchar(32),

--	@Attachments ntext,
	@AttachmentIds varchar(8000),
	@AttachmentFileNames ntext,
	@AttachmentFileIds text,
	@AttachmentFileSizes varchar(8000),
	@AttachmentPrices varchar(8000),
	@AttachmentFileExtNames ntext,
	@HistoryAttachmentIDs varchar(500),

	@UpdateTotalPosts bit = 1,

	@IsApproved bit,
	@PostRandNumber tinyint,
	@PostID int output,
	@UserTotalPosts int output,
	--@Points int output,
	@UpdateSortOrder bit
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @Type tinyint;
	IF @IsApproved = 1 --BEGIN
		SET @Type=1
	ELSE
		SET @Type=5

	DECLARE @TempSortOrder BIGINT,@PostDate datetime

	SET @PostDate = getdate();
	EXEC [bx_GetSortOrder] @Type, @PostRandNumber, @PostDate, @TempSortOrder OUTPUT 

	--SET @TempSortOrder = [dbo].bx_GetSortOrder(@Type,@PostRandNumber, getdate())

	INSERT INTO [bx_Posts]
           ([ParentID]
           ,[ForumID]
           ,[ThreadID]
           ,[PostType]
           ,[IconID]
           ,[Subject]
           ,[Content]
           ,[ContentFormat]
           ,[EnableSignature]
--           ,[EnableEmoticons]
--           ,[EnableHTML]
--           ,[EnableSafeHTML]
--           ,[EnableMaxCode]
           ,[EnableReplyNotice]
--           ,[IsApproved]
           ,[UserID]
           ,[NickName]
           ,[IPAddress]
           ,[HistoryAttachmentIDs]
           ,[SortOrder])
     VALUES
           (@ParentID
           ,@ForumID
           ,@ThreadID
           ,@PostType
           ,@IconID
           ,@Subject
           ,@Content
           ,@ContentFormat
           ,@EnableSignature
--           ,@EnableEmoticons
--           ,@EnableHTML
--           ,@EnableSafeHTML
--           ,@EnableMaxCode
           ,@EnableReplyNotice
--           ,@IsApproved
           ,@UserID
           ,@NickName
           ,@IPAddress
		   ,@HistoryAttachmentIDs
           ,@TempSortOrder )

	SELECT @PostID = @@IDENTITY;

	IF @UserID<>0 BEGIN
		-- 添加辩论用户 --
		IF @PostType = 2
			EXEC [bx_AddPolemizeUser] @ThreadID,@UserID,0
		ELSE IF @PostType = 3
			EXEC [bx_AddPolemizeUser] @ThreadID,@UserID,1
		ELSE IF @PostType = 4
			EXEC [bx_AddPolemizeUser] @ThreadID,@UserID,2
		---------------------

	END

	--//添加附件列表
	IF DATALENGTH(@AttachmentIds) > 0 BEGIN

		DECLARE @AttachmentTable table(TempID int identity(1,1), AttachmentID int, FileName nvarchar(256), FileExtName varchar(10), FileID varchar(50), FileSize bigint, Price int);

		--DECLARE @NewAttachCount int
		
		INSERT INTO @AttachmentTable (AttachmentID) SELECT item FROM bx_GetIntTable(@AttachmentIds, '|');

		UPDATE @AttachmentTable SET
			[FileName] = T.item
			FROM bx_GetStringTable_ntext(@AttachmentFileNames, N'|') T
			WHERE TempID = T.id;

		UPDATE @AttachmentTable SET
			FileID = T.item
			FROM bx_GetStringTable_text(@AttachmentFileIds, '|') T
			WHERE TempID = T.id;

		UPDATE @AttachmentTable SET
			FileSize = T.item
			FROM bx_GetBigIntTable(@AttachmentFileSizes, '|') T
			WHERE TempID = T.id;

		UPDATE @AttachmentTable SET
			Price = T.item
			FROM bx_GetIntTable(@AttachmentPrices, '|') T
			WHERE TempID = T.id;
			
		UPDATE @AttachmentTable SET
			FileExtName = T.item
			FROM bx_GetStringTable_ntext(@AttachmentFileExtNames, N'|') T
			WHERE TempID = T.id;


		INSERT INTO bx_Attachments(
			PostID,
			FileID,
			FileName,
			FileSize,
			FileType,
			Price,
			UserID
			) SELECT 
			@PostID,
			T.FileID,
			T.FileName,
			T.FileSize,
			T.FileExtName,
			T.Price,
			@UserID
			FROM @AttachmentTable T
			WHERE T.AttachmentID < 0;
		
		SELECT [AttachmentID] FROM [bx_Attachments] WHERE PostID=@PostID ORDER BY [AttachmentID] DESC;
		
		INSERT INTO bx_Attachments(
			PostID,
			FileID,
			FileName,
			FileSize,
			FileType,
			Price,
			UserID
			) SELECT 
			@PostID,
			T.FileID,
			T.FileName,
			T.FileSize,
			T.FileExtName,
			T.Price,
			@UserID
			FROM @AttachmentTable T
			WHERE T.AttachmentID = 0; 
		
		SELECT [AttachmentID],[FileID] FROM [bx_Attachments] WHERE PostID=@PostID;
		
		--SELECT @NewAttachCount = @ROWCOUNT;

		--DECLARE @AttachmentIDTable table(TempID int identity(1,1),AttachID int)
		--INSERT INTO @AttachmentIDTable(AttachID) SELECT [AttachmentID] FROM [bx_Attachments] WHERE PosID=@PostID;

		--DECLARE @i int,@PostContent;
		--SET @i = 1;
		
		
		--WHILE(@i<@NewAttachCount+1) BEGIN
			
		--END

		--UPDATE bx_Attachments SET
			--FileName = T.FileName,
			--Price = T.Price
			--FROM @AttachmentTable T
			--WHERE T.AttachmentID > 0 AND T.AttachmentID = bx_Attachments.AttachmentID;
			
	END

	--添加附件结束
	--@ForumID = -2 表示该帖子未审核
    IF @IsApproved=1 AND @UpdateTotalPosts = 1 BEGIN
    
		--DECLARE @TempForumID int
	
		----------增加回复数---------------
		EXEC [bx_DoCreateStat] @ForumID,4, 1
		
		--------------------------
	    
		DECLARE @SortOrder bigint
		
		IF @UpdateSortOrder = 1 BEGIN
			SELECT @SortOrder = [SortOrder] FROM [bx_Threads] WITH (NOLOCK) WHERE [ThreadID] = @ThreadID
			
			IF @SortOrder < 2000000000000000--500000000000000
				EXEC [bx_GetSortOrder] 1, @PostID, @PostDate, @SortOrder OUTPUT 
				--SELECT @SortOrder = [dbo].bx_GetSortOrder(1,@PostID, getdate());
			ELSE IF @SortOrder < 3000000000000000--800000000000000
				EXEC [bx_GetSortOrder] 2, @PostID, @PostDate, @SortOrder OUTPUT 
				--SELECT @SortOrder = [dbo].bx_GetSortOrder(2,@PostID, getdate());-- + 400000000000000;
			ELSE
				EXEC [bx_GetSortOrder] 3, @PostID, @PostDate, @SortOrder OUTPUT 
				--SELECT @SortOrder = [dbo].bx_GetSortOrder(3,@PostID, getdate());-- + 700000000000000;
		
		END
		
        UPDATE [bx_Forums]
           SET [TotalPosts] = [TotalPosts] + 1,
				[TodayPosts] = [TodayPosts] + 1,
				[LastThreadID] = @ThreadID
         WHERE [ForumID] = @ForumID;

        -------
	    IF @UpdateSortOrder = 1
			UPDATE [bx_Threads]
			   SET [TotalReplies] = [TotalReplies] + 1,
					[LastPostUserID] = @UserID,
					[LastPostNickName] = @NickName,
					[UpdateDate] = getdate(),
					[SortOrder] = @SortOrder
			WHERE ThreadID=@ThreadID
		ELSE
			UPDATE [bx_Threads]
			   SET [TotalReplies] = [TotalReplies] + 1,
					[LastPostUserID] = @UserID,
					[LastPostNickName] = @NickName,
					[UpdateDate] = getdate()
			WHERE ThreadID=@ThreadID

        -------
		IF @UserID<>0 BEGIN
			UPDATE [bx_Users]
			   SET	[LastPostDate] = getdate(),
					[TotalPosts] = [TotalPosts] + 1
				  --,[ExtendedPoints_1] = [ExtendedPoints_1] + @ExtendedPoints_1
				  --,[ExtendedPoints_2] = [ExtendedPoints_2] + @ExtendedPoints_2
				  --,[ExtendedPoints_3] = [ExtendedPoints_3] + @ExtendedPoints_3
				  --,[ExtendedPoints_4] = [ExtendedPoints_4] + @ExtendedPoints_4
				  --,[ExtendedPoints_5] = [ExtendedPoints_5] + @ExtendedPoints_5
				  --,[ExtendedPoints_6] = [ExtendedPoints_6] + @ExtendedPoints_6
				  --,[ExtendedPoints_7] = [ExtendedPoints_7] + @ExtendedPoints_7
				  --,[ExtendedPoints_8] = [ExtendedPoints_8] + @ExtendedPoints_8
			 WHERE UserID = @UserID;
			 
			--IF @@ROWCOUNT < 1
			--BEGIN
				--execute bx_CreateUserProfile @UserID
				--IF(@@error<>0)
					--GOTO Cleanup;

				--UPDATE [bx_UserProfiles]
				--SET	[LastCreatePostDate] = getdate(),
					--[TotalPosts] = [TotalPosts] + 1
				  --,[ExtendedPoints_1] = [ExtendedPoints_1] + @ExtendedPoints_1
				  --,[ExtendedPoints_2] = [ExtendedPoints_2] + @ExtendedPoints_2
				  --,[ExtendedPoints_3] = [ExtendedPoints_3] + @ExtendedPoints_3
				  --,[ExtendedPoints_4] = [ExtendedPoints_4] + @ExtendedPoints_4
				  --,[ExtendedPoints_5] = [ExtendedPoints_5] + @ExtendedPoints_5
				  --,[ExtendedPoints_6] = [ExtendedPoints_6] + @ExtendedPoints_6
				  --,[ExtendedPoints_7] = [ExtendedPoints_7] + @ExtendedPoints_7
				  --,[ExtendedPoints_8] = [ExtendedPoints_8] + @ExtendedPoints_8
				--WHERE UserID = @UserID;
				
				--IF(@@error<>0)
					--GOTO Cleanup;
			--END
		END
--
--	SELECT --@UserTotalThreads=TotalThreads,
--		@UserTotalPosts=TotalPosts,
--		@Points=Points,
--		@ExtendedPoints_1=ExtendedPoints_1,
--		@ExtendedPoints_2=ExtendedPoints_2,
--		@ExtendedPoints_3=ExtendedPoints_3,
--		@ExtendedPoints_4=ExtendedPoints_4,
--		@ExtendedPoints_5=ExtendedPoints_5,
--		@ExtendedPoints_6=ExtendedPoints_6,
--		@ExtendedPoints_7=ExtendedPoints_7,
--		@ExtendedPoints_8=ExtendedPoints_8
--		FROM [bx_UserProfiles] WHERE UserID = @UserID;

    END
    IF @UserID<>0 BEGIN
		--IF NOT EXISTS (SELECT * FROM [bx_UserProfiles] WHERE UserID = @UserID) BEGIN
			--execute bx_CreateUserProfile @UserID
			--IF(@@error<>0)
					--GOTO Cleanup;
		--END
		SELECT --@UserTotalThreads=TotalThreads,
			@UserTotalPosts=ISNULL(TotalPosts,0)
			--@Points=ISNULL(Points,0),
			--@ExtendedPoints_1=ExtendedPoints_1,
			--@ExtendedPoints_2=ExtendedPoints_2,
			--@ExtendedPoints_3=ExtendedPoints_3,
			--@ExtendedPoints_4=ExtendedPoints_4,
			--@ExtendedPoints_5=ExtendedPoints_5,
			--@ExtendedPoints_6=ExtendedPoints_6,
			--@ExtendedPoints_7=ExtendedPoints_7,
			--@ExtendedPoints_8=ExtendedPoints_8
			FROM [bx_Users] WHERE UserID = @UserID;
			
	END 
	ELSE BEGIN
			SET  @UserTotalPosts = 0;
			--SET  @Points = 0;
			--SET  @ExtendedPoints_1 = 0;
			--SET  @ExtendedPoints_2 = 0;
			--SET  @ExtendedPoints_3 = 0;
			--SET  @ExtendedPoints_4 = 0;
			--SET  @ExtendedPoints_5 = 0;
			--SET  @ExtendedPoints_6 = 0;
			--SET  @ExtendedPoints_7 = 0;
			--SET  @ExtendedPoints_8 = 0;
	END

	
END


