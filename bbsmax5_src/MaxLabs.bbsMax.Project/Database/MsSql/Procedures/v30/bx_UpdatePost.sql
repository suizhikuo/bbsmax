-- =============================================
-- Author:		SEK
-- Create date: <2006/12/27>
-- Description:	<更新回复> 
-- =============================================
CREATE PROCEDURE [bx_UpdatePost] 
	@PostID int,
	--@PostType tinyint,
	@IconID int,
	@Subject nvarchar(256),
	@Content ntext,
	--@Attachments ntext,---Modify by 帅帅
	@ContentFormat tinyint,
	@EnableSignature bit,
--	@EnableEmoticons bit,
--	@EnableHTML bit,
--	@EnableSafeHTML bit,
--	@EnableMaxCode bit,
	@EnableReplyNotice bit,
	@IsApproved bit,
	@LastEditorID int,
	@LastEditor nvarchar(65),
--	@GetDeletedDiskFileIDs bit,
	
	@AttachmentIds varchar(8000),
	@AttachmentFileNames ntext,
	@AttachmentFileIds text,
	@AttachmentFileSizes varchar(8000),
	@AttachmentPrices varchar(8000),
	@AttachmentFileExtNames ntext,
	@HistoryAttachmentIDs varchar(500)
AS

SET NOCOUNT ON

DECLARE @UserID int,@OldSortOrder bigint;
SELECT @UserID=UserID,@OldSortOrder=SortOrder FROM  bx_Posts WHERE PostID=@PostID;---Modify by 帅帅


DECLARE @SortOrder bigint;

IF @IsApproved=1
	EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @SortOrder OUTPUT;
ELSE
	EXEC [bx_UpdateSortOrder] 5, @OldSortOrder, @SortOrder OUTPUT;

--IF @IsApproved=1
	--UPDATE [bx_Posts] SET
		--[IconID] = @IconID,
		--[Subject] = @Subject,
		--[Content] = @Content,
		--[ContentFormat] = @ContentFormat,
		--[EnableSignature] = @EnableSignature,
		--[EnableReplyNotice] = @EnableReplyNotice,
		--[LastEditorID]=@LastEditorID,
		--[LastEditor]=@LastEditor,
		--[UpdateDate] = getdate(),
		--[HistoryAttachmentIDs] = @HistoryAttachmentIDs,
		--[SortOrder] = @SortOrder
	--WHERE
		--[PostID] = @PostID
--ELSE
	UPDATE [bx_Posts] SET
		[IconID] = @IconID,
		[Subject] = @Subject,
		[Content] = @Content,
		[ContentFormat] = @ContentFormat,
		[EnableSignature] = @EnableSignature,
		[EnableReplyNotice] = @EnableReplyNotice,
		[LastEditorID]=@LastEditorID,
		[LastEditor]=@LastEditor,
		[UpdateDate] = getdate(),
		[HistoryAttachmentIDs] = @HistoryAttachmentIDs,
		[SortOrder] = @SortOrder,
		[KeywordVersion] = ''
	WHERE
		[PostID] = @PostID
		
	
		IF DATALENGTH(@AttachmentIds) > 0 BEGIN
			DECLARE @AttachmentTable table(TempID int identity(1,1), AttachmentID int, FileName nvarchar(256), FileExtName varchar(10), FileID varchar(50), FileSize bigint, Price int);
		
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


			DECLARE @NewAttchmentCount int;

			DECLARE @AttachmentIDsTable table(AttachmentID int);
			INSERT INTO @AttachmentIDsTable SELECT [AttachmentID] FROM [bx_Attachments] WHERE PostID=@PostID;

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
					
			SELECT @NewAttchmentCount = @@ROWCOUNT;
			
			EXEC('SELECT TOP ' + @NewAttchmentCount + ' [AttachmentID] FROM [bx_Attachments] WHERE PostID = '+@PostID + ' ORDER BY [AttachmentID] DESC');
					
			--IF @GetDeletedDiskFileIDs = 1
				--SELECT DISTINCT FileID FROM bx_Attachments WHERE PostID=@PostID AND FileID NOT IN(SELECT FileID FROM @AttachmentTable) AND FileID NOT IN(SELECT DISTINCT FileID FROM bx_Attachments WHERE PostID<>@PostID OR (PostID = @PostID AND AttachmentID NOT IN(SELECT AttachmentID FROM @AttachmentTable))) 
			
			DELETE bx_Attachments WHERE PostID=@PostID AND 
				AttachmentID IN(SELECT [AttachmentID] FROM @AttachmentIDsTable) 
				AND AttachmentID NOT IN(SELECT AttachmentID FROM @AttachmentTable);


			UPDATE bx_Attachments SET
				FileName = T.FileName,
				Price = T.Price
				FROM @AttachmentTable T
				WHERE T.AttachmentID > 0 AND T.AttachmentID = bx_Attachments.AttachmentID;
				
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

		END 
		ELSE BEGIN
			--IF @GetDeletedDiskFileIDs = 1
				--SELECT DISTINCT FileID FROM bx_Attachments WHERE PostID=@PostID AND FileID NOT IN(SELECT DISTINCT FileID FROM bx_Attachments WHERE PostID<>@PostID) 
			DELETE bx_Attachments WHERE PostID = @PostID;

		END


	RETURN (0);

--Cleanup:
    --BEGIN
    	--ROLLBACK TRANSACTION
    	--RETURN (-1)
    --END
