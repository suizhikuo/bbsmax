 
EXEC bx_Drop 'bx_Users_Exp_AfterUpdate';

GO


CREATE TRIGGER [bx_Users_Exp_AfterUpdate]
	ON [bx_Users]
	AFTER UPDATE
AS
BEGIN
	-- 注意  修改以下  内容要相应修改 UserDao.cs 里的 UpdatePointsExpression(string expression)

	SET NOCOUNT ON;

	/* 如果是更新积分 不在这里更新总积分
	IF (UPDATE ([Point_1]) OR UPDATE ([Point_2])) BEGIN
		DECLARE @MaxValue int;
		SET @MaxValue = 2147483647;
		SET ARITHABORT OFF;
		SET ANSI_WARNINGS OFF;
		
		UPDATE bx_Users SET Points = ISNULL([Point_1]+[Point_2]*10,@MaxValue) WHERE [UserID] IN(SELECT DISTINCT [UserID] FROM [INSERTED]);
	END
	*/
	
	IF (UPDATE([IsActive])) BEGIN
			DECLARE @NewUserID int,@NewUsername nvarchar(50),@DeletedCount int,@InsertCount int;
			
			SELECT @InsertCount=COUNT(*) FROM [INSERTED] WHERE [IsActive]=1;
			SELECT @DeletedCount=COUNT(*) FROM [DELETED] WHERE [IsActive]=1;
			
			SELECT TOP 1 @NewUserID = UserID,@NewUsername = Username FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 ORDER BY [UserID] DESC;
			
			UPDATE [bx_Vars] SET  NewUserID = @NewUserID, NewUsername = @NewUsername, TotalUsers = TotalUsers + @InsertCount - @DeletedCount WHERE [ID]=(SELECT TOP 1 ID FROM [bx_Vars]);

			IF @@ROWCOUNT = 0 BEGIN
				DECLARE @TotalUsers int;
				SELECT @TotalUsers = COUNT(*) FROM [bx_Users] WITH (NOLOCK) WHERE [IsActive] = 1 AND [UserID]<>0;
				INSERT [bx_Vars] (NewUserID,NewUsername,TotalUsers)VALUES(@NewUserID,@NewUsername,@TotalUsers);
			END
			
			SELECT 'ResetVars' AS XCMD;
	END
	
END


