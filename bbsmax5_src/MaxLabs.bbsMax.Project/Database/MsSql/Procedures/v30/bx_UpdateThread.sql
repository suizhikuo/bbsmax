-- =============================================
-- Author:		SEK
-- Create date: <2009/10/29>
-- Description:	<更新主题>
-- =============================================
CREATE PROCEDURE [bx_UpdateThread]
	@ThreadID int,
	@ThreadCatalogID int,
	@IconID int,
	@Subject nvarchar(256),
	@PostID int,
	@Content ntext,
	@ContentFormat tinyint,
	@EnableSignature bit,
	@EnableReplyNotice bit,
	@IsApproved bit,
	@LastEditorID int,
	@LastEditor nvarchar(64),
	@Price int,
	@AttachmentIds varchar(8000),
	@AttachmentFileNames ntext,
	@AttachmentFileIds text,
	@AttachmentFileSizes varchar(8000),
	@AttachmentPrices varchar(8000),
	@AttachmentFileExtNames ntext,
	@HistoryAttachmentIDs varchar(500)
AS

SET NOCOUNT ON

	DECLARE @OldSortOrder bigint, @NewSortOrder bigint
	SELECT @OldSortOrder = SortOrder FROM [bx_Threads] WHERE [ThreadID] = @ThreadID;

	IF @IsApproved=1 AND @OldSortOrder> 5000000000000000
		EXEC [bx_UpdateSortOrder] 1, @OldSortOrder, @NewSortOrder OUTPUT;
		-- SET @NewSortOrder=[dbo].bx_UpdateSortOrder(1,@OldSortOrder)
	ELSE IF @IsApproved=0 AND @OldSortOrder < 4000000000000000
		EXEC [bx_UpdateSortOrder] 5, @OldSortOrder, @NewSortOrder OUTPUT;
		--SET @NewSortOrder=[dbo].bx_UpdateSortOrder(5,@OldSortOrder)
	ELSE 
		SET @NewSortOrder = @OldSortOrder;

	UPDATE [bx_Threads] SET
		[ThreadCatalogID] = @ThreadCatalogID,
		[IconID] = @IconID,
		[Subject] = @Subject,
		[Price] = @Price,
		[UpdateDate] = getdate(),
		[SortOrder] = @NewSortOrder,
		[KeywordVersion] = ''
	WHERE
		[ThreadID] = @ThreadID;
--ELSE
	--UPDATE [dbo].[bx_Threads] SET
		--[ThreadCatalogID]=@ThreadCatalogID,
		--[IconID] = @IconID,
		--[Subject] = @Subject,
		--[UpdateDate] = getdate(),
		--[SortOrder] = [dbo].bx_UpdateSortOrder(5,SortOrder)
	--WHERE
		--[ThreadID] = @ThreadID

---更新bsMax_Posts--
	--DECLARE @IsApproved bit
	--IF @ThreadStatus=5
		--SET @IsApproved=0
	--ELSE
		--SET @IsApproved=1
		
Execute bx_UpdatePost
		@PostID,
		@IconID,
		@Subject,
		@Content,
		@ContentFormat,
		@EnableSignature,
		@EnableReplyNotice,
		@IsApproved,
		@LastEditorID,
		@LastEditor,
		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs


