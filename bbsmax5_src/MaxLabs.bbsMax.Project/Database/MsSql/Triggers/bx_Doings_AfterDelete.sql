--删除记录触发器
EXEC bx_Drop 'bx_Doings_AfterDelete';

GO


CREATE TRIGGER [bx_Doings_AfterDelete]
	ON [bx_Doings]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int,TotalDoings int);

	DELETE [bx_Comments] WHERE TargetID IN (SELECT DoingID FROM [DELETED]) AND Type = 3;

	INSERT INTO @tempTable 
		SELECT DISTINCT T.UserID,
			(ISNULL((SELECT COUNT(*) FROM [bx_Doings] as m WITH (NOLOCK) WHERE m.UserID = T.UserID), 0))
		FROM [DELETED] T;

	UPDATE [bx_Users]
		SET
			bx_Users.TotalDoings = T.TotalDoings
		FROM @tempTable T
		WHERE
			T.UserID = bx_Users.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetUser' AS XCMD, UserID, TotalDoings FROM @tempTable;

END