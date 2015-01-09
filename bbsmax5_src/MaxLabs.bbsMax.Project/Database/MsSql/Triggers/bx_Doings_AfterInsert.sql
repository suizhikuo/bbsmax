--添加记录触发器
CREATE TRIGGER [bx_Doings_AfterInsert]
	ON [bx_Doings]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int,TotalDoings int);

	INSERT INTO @tempTable 
		SELECT DISTINCT T.UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Doings] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [INSERTED] T;

	UPDATE [bx_Users]
		SET
			bx_Users.TotalDoings = T.TotalDoings
		FROM @tempTable T
		WHERE
			T.UserID = [bx_Users].UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalDoings FROM @tempTable;

END