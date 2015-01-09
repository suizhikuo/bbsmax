
EXEC bx_Drop 'bx_Users_AfterInsert';

GO


CREATE TRIGGER [bx_Users_AfterInsert]
	ON [bx_Users]
	AFTER INSERT
AS
BEGIN


	SET NOCOUNT ON;
	
	DECLARE @NewUserID int, @NewUsername nvarchar(50), @Count int;
	
	SELECT @Count = Count(*) FROM [INSERTED] WHERE [IsActive] = 1 AND [UserID] <> 0;
	
	IF @Count = 0
		RETURN;
		
	SELECT TOP 1 @NewUserID = UserID, @NewUsername = Username FROM [INSERTED] WHERE [IsActive] = 1 ORDER BY [UserID] DESC; 

	UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = TotalUsers + @Count;
	
	IF @@ROWCOUNT = 0 BEGIN
		DECLARE @TotalUsers int;
		SELECT @TotalUsers = COUNT(*) FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 AND [UserID]<>0;
		INSERT [bx_Vars] (NewUserID, NewUsername, TotalUsers) VALUES (@NewUserID, @NewUsername, @TotalUsers);
	END
	
	SELECT 'ResetVars' AS XCMD;
	
END


