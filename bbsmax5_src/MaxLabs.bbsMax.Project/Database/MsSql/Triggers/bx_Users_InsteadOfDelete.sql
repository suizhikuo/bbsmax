
EXEC bx_Drop 'bx_Users_InsteadOfDelete';

GO


CREATE TRIGGER [bx_Users_InsteadOfDelete]
	ON [bx_Users]
	INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;
	DELETE [bx_Users] WHERE [UserID] IN (SELECT [UserID] FROM [DELETED]);
	
	
	DELETE bx_Denouncings WHERE Type=4 AND TargetID IN (SELECT [UserID] FROM [DELETED]);
	
END