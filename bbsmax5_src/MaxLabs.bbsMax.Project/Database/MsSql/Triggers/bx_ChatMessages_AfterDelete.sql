EXEC bx_Drop 'bx_ChatMessages_AfterDelete';

GO

CREATE TRIGGER [bx_ChatMessages_AfterDelete]
	ON [bx_ChatMessages]
	AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @tempTable table(UserID int, UnreadMessages int);
	INSERT INTO @tempTable 
		SELECT DISTINCT UserID,
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.IsRead = 0), 0)
		FROM [DELETED] T;
	
	UPDATE [bx_UserVars]
		SET
			bx_UserVars.UnreadMessages = T.UnreadMessages
		FROM @tempTable T
		WHERE
			T.UserID = bx_UserVars.UserID;

	--发出重新填充UserInfo的XCMD命令
	SELECT 'ResetAuthUser' AS XCMD, UserID, UnreadMessages FROM @tempTable;	

	DECLARE @tempTable2 table(UserID int, TargetUserID int, TotalMessages int, UnreadMessages int);
	INSERT INTO @tempTable2
		SELECT DISTINCT UserID, TargetUserID,
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.TargetUserID = T.TargetUserID), 0),
			ISNULL((SELECT COUNT(*) FROM [bx_ChatMessages] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.IsRead = 0), 0)
		FROM [DELETED] T;
	
	UPDATE [bx_ChatSessions]
		SET
			bx_ChatSessions.TotalMessages = T.TotalMessages,
			bx_ChatSessions.UnreadMessages = T.UnreadMessages
		FROM @tempTable2 T
		WHERE
			T.UserID = bx_ChatSessions.UserID
			AND
			T.TargetUserID = bx_ChatSessions.TargetUserID;

END