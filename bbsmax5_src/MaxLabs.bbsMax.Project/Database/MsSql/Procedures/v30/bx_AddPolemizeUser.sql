CREATE PROCEDURE [bx_AddPolemizeUser]
	@ThreadID int, 
	@UserID int,
	@ViewPointType tinyint
AS
	SET NOCOUNT ON;
	
	IF(NOT EXISTS (SELECT * FROM [bx_Polemizes] WHERE ThreadID=@ThreadID AND ExpiresDate>GETDATE()))
		RETURN (2) --过期了
	
	DECLARE @ViewPoint TINYINT
	SELECT @ViewPoint=ViewPointType FROM [bx_PolemizeUsers] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID
	IF @ViewPoint = 0 --已支持过正方观点
		RETURN (5)
	ELSE IF @ViewPoint = 1 --已支持过反方观点
		RETURN (6)
	ELSE IF @ViewPoint = 2 --已支持过中方观点
		RETURN (7)
	--IF(EXISTS (SELECT * FROM [bx_PolemizeUsers] WITH (NOLOCK) WHERE ThreadID=@ThreadID AND UserID=@UserID))
		--RETURN (1); --辩论过了
	
	BEGIN TRANSACTION
		
	INSERT INTO [bx_PolemizeUsers](ThreadID,UserID,ViewPointType) VALUES(@ThreadID,@UserID,@ViewPointType)
	IF(@@error<>0)
		GOTO Cleanup;
		
	IF @ViewPointType = 0 BEGIN
		UPDATE [bx_Polemizes] SET AgreeCount=AgreeCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	ELSE IF @ViewPointType = 1 BEGIN
		UPDATE [bx_Polemizes] SET AgainstCount=AgainstCount+1 WHERE ThreadID=@ThreadID
		IF(@@error<>0)
			GOTO Cleanup;
	END
	ELSE IF @ViewPointType = 2 BEGIN
		UPDATE [bx_Polemizes] SET NeutralCount=NeutralCount+1 WHERE ThreadID=@ThreadID
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