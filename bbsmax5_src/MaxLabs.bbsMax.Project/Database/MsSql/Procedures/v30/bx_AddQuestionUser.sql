CREATE PROCEDURE [bx_AddQuestionUser]
	@ThreadID int,
	@UserID int,
	@BestPostIsUseful bit
AS
	SET NOCOUNT ON;
	
	DECLARE @IsClosed bit
	
	SELECT @IsClosed = IsClosed FROM [bx_Questions] WHERE ThreadID=@ThreadID
	
	IF @IsClosed IS NULL
		RETURN (1) -- 不存在
	ELSE IF @IsClosed = 0
		RETURN (2) -- 还未揭贴
	
	IF(EXISTS (SELECT * FROM [bx_QuestionUsers] WHERE ThreadID=@ThreadID AND UserID=@UserID))
		RETURN (3) -- 已经投过
		
	BEGIN TRANSACTION
		
	INSERT INTO [bx_QuestionUsers](ThreadID,UserID,BestPostIsUseful) VALUES(@ThreadID,@UserID,@BestPostIsUseful)
	IF(@@error<>0)
		GOTO Cleanup;
	
	IF @BestPostIsUseful = 1 BEGIN
		UPDATE [bx_Questions] SET UsefulCount=UsefulCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	ELSE BEGIN
		UPDATE [bx_Questions] SET UnusefulCount=UnusefulCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	
		COMMIT TRANSACTION;
		RETURN (0);
Cleanup:
    BEGIN
    	ROLLBACK TRANSACTION
    	RETURN (-1)
    END
	