-- =============================================
-- Author:		zzbird
-- Create date: 2006/12/30
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_CreatePoll]
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

	@PollItems nvarchar(4000),
	@Multiple int,
	@AlwaysEyeable bit,
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
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--DECLARE @NewThreadID     int;
	DECLARE @ReturnValue     int;


	EXECUTE @ReturnValue = [bx_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		1,
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


	--插入投票信息
    INSERT INTO [bx_Polls]
       ([ThreadID]
       ,[Multiple]
       ,[AlwaysEyeable]
       ,[ExpiresDate])
     VALUES
       (@ThreadID
       ,@Multiple
       ,@AlwaysEyeable
       ,@ExpiresDate);

	----------增加投票数---------------
	EXEC [bx_DoCreateStat] @ForumID,5, 1
		--------------------------
	--字表字段变量
	DECLARE @ItemName nvarchar(512)
	SET @ItemName = ''
	
	--数量计数
	DECLARE @Index int
	SET @Index = 0
	
	WHILE(@PollItems <> '')
	BEGIN
		IF (CharIndex(char(13), @PollItems) = 0) BEGIN
			SET @ItemName = @PollItems
			SET @PollItems  = ''	
		END
		ELSE BEGIN
			SET @ItemName = substring(rtrim(ltrim(@PollItems)), 1, charIndex(char(13), rtrim(ltrim(@PollItems))) - 1)
			SET @PollItems = substring(rtrim(ltrim(@PollItems)), charIndex(char(13), rtrim(ltrim(@PollItems))) + 1, len(rtrim(ltrim(@PollItems)))-charIndex(char(13), rtrim(ltrim(@PollItems))))
		END

		INSERT INTO bx_PollItems(
			ThreadID,
			ItemName
		) VALUES (
			@ThreadID,
			REPLACE(@ItemName, char(10), N'')
		)
		
	END

	--SELECT @UserTotalThreads=TotalThreads,@UserTotalPosts=TotalPosts FROM [bx_UserProfiles] WHERE UserID = @UserID;

	RETURN (0);

END


