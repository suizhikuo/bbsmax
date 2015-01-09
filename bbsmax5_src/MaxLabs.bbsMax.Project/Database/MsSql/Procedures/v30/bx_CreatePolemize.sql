CREATE PROCEDURE [bx_CreatePolemize]
	@ForumID int,
	@ThreadCatalogID int,
	@ThreadStatus tinyint,

	@IconID int,
	@Subject nvarchar(256),
	@SubjectStyle nvarchar(300),
	@UserID int,
	@NickName nvarchar(64),
	@IsLocked bit,
	@IsValued bit,

	@Content ntext,
	@ContentFormat tinyint,
	@EnableSignature bit,
	@EnableReplyNotice bit,
	@IPAddress nvarchar(64),

	@AgreeViewPoint ntext,
	@AgainstViewPoint ntext,
	@ExpiresDate datetime,

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
	--@Points int output
AS
BEGIN
	SET NOCOUNT ON;

	--DECLARE @NewThreadID     int;
	DECLARE @ReturnValue     int;


	EXECUTE @ReturnValue = [bx_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		4,
		@IconID,
		@Subject,
		@SubjectStyle,
		0,
		@UserID,
		@NickName,
		@IsLocked,
		@IsValued,
		@Content,
		@ContentFormat,
		@EnableSignature,
--		@EnableEmoticons,
--		@EnableHTML,
--		@EnableSafeHTML,
--		@EnableMaxCode,
		@EnableReplyNotice,
		@IPAddress,

		@AttachmentIds,
		@AttachmentFileNames,
		@AttachmentFileIds,
		@AttachmentFileSizes,
		@AttachmentPrices,
		@AttachmentFileExtNames,
		@HistoryAttachmentIDs,
		
		@ThreadRandNumber,
		@ThreadID output,
		@PostID output,
		@UserTotalThreads output,
		@UserTotalPosts output
		--@Points output


	--插入辩论信息
    INSERT INTO [bx_Polemizes]
       ([ThreadID]
       ,[AgreeViewPoint]
       ,[AgainstViewPoint]
       ,[ExpiresDate])
     VALUES
       (@ThreadID
       ,@AgreeViewPoint
       ,@AgainstViewPoint
       ,@ExpiresDate);

	----------增加辩论帖统计数---------------
	EXEC [bx_DoCreateStat] @ForumID,30, 1
		--------------------------

	--SELECT @UserTotalThreads=TotalThreads,@UserTotalPosts=TotalPosts FROM [bx_UserProfiles] WHERE UserID = @UserID;

	RETURN (0);
END