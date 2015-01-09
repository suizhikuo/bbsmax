-- =============================================
-- Author:		zzbird
-- Create date: 2006/12/30
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_CreateQuestion]
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
--	@EnableEmoticons bit,
--	@EnableHTML bit,
--	@EnableSafeHTML bit,
--	@EnableMaxCode bit,
	@EnableReplyNotice bit,
	@IPAddress nvarchar(64),

	@Reward int,  --本主题的奖励
	@RewardCount int,  --本主题最多可以奖励给多少帖子
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
	--@Points int output,
	--@TradePoints int,
	--@TradePointID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
--	DECLARE @TempPoints int;
--	exec('SELECT @TempPoints=ExtendedPoints_'+@TradePointID+' FROM [bx_UserProfiles] WHERE UserID='+@UserID);
--	if @TempPoints-@TradePoints
	--exec('UPDATE [bx_UserProfiles] SET ExtendedPoints_'+@TradePointID+'=ExtendedPoints_'+@TradePointID+'-'+@TradePoints+' WHERE UserID='+@UserID);
	--IF(@@error<>0)
		--GOTO Cleanup;

	DECLARE @ReturnValue     int;


	EXECUTE @ReturnValue = [bx_CreateThread]
		@ForumID,
		@ThreadCatalogID,
		@ThreadStatus,
		2,
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

	INSERT INTO [bx_Questions]
           ([ThreadID]
           ,[Reward]
           ,[RewardCount]
           ,[AlwaysEyeable]
           ,[ExpiresDate])
     VALUES
           (@ThreadID
           ,@Reward
           ,@RewardCount
           ,@AlwaysEyeable
           ,@ExpiresDate)

	----------增加问题数---------------
		EXEC [bx_DoCreateStat] @ForumID,6, 1
		--------------------------
	IF @ThreadStatus = 5 --当问题属于未审核时，由于提问的分数已扣，所以返回积分更新缓存（审核过的帖子由bx_CreateThread里返回）
		SELECT @UserTotalThreads=TotalTopics,
				@UserTotalPosts=TotalPosts
			FROM [bx_Users] WHERE UserID = @UserID;	

	RETURN (0);

END


