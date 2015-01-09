CREATE PROCEDURE [bx_GetQuestion]
	@ThreadID int,
	@OnlyQuestion bit
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Questions] WHERE ThreadID=@ThreadID
	SELECT PostID,Reward FROM [bx_QuestionRewards] WHERE ThreadID=@ThreadID
	
	DECLARE @BestPostID INT
	SELECT @BestPostID = BestPostID FROM [bx_Questions] WHERE ThreadID=@ThreadID
	IF @BestPostID > 0 BEGIN
		SELECT * FROM [bx_Posts] WHERE PostID=@BestPostID AND [SortOrder]<4000000000000000;
		IF @OnlyQuestion = 0 AND EXISTS(SELECT * FROM [bx_Posts] WHERE PostID=@BestPostID AND [SortOrder]<4000000000000000) BEGIN
			SELECT * FROM [bx_Attachments] WITH (NOLOCK) WHERE PostID = @BestPostID ORDER BY AttachmentID;
			SELECT * FROM [bx_PostMarks] WITH (NOLOCK) WHERE PostID = @BestPostID ORDER BY PostMarkID DESC;
		END
	END

