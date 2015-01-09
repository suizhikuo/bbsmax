--新建相册的触发器
EXEC bx_Drop 'bx_Albums_AfterInsert';

GO


CREATE TRIGGER [bx_Albums_AfterInsert]
	ON [bx_Albums]
	AFTER INSERT
AS
BEGIN
	
	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int, TotalAlbums int);

	INSERT INTO @tempTable 
		SELECT DISTINCT UserID,
			ISNULL((SELECT COUNT(*) FROM [bx_Albums] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0)
		FROM [INSERTED] T;
	
	UPDATE [bx_Users]
		SET
			bx_Users.TotalAlbums = T.TotalAlbums
		FROM @tempTable T
		WHERE
			T.UserID = bx_Users.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalAlbums FROM @tempTable;	
END