--添加好友触发器
CREATE TRIGGER [bx_Friends_AfterInsert]
	ON [bx_Friends]
	AFTER INSERT
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @tempTable table(UserID int, GroupID int,TotalFriends int);

	INSERT INTO @tempTable 
	SELECT DISTINCT UserID,GroupID,
		(ISNULL((SELECT COUNT(*) FROM [bx_Friends] as m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID = T.GroupID), 0))
		FROM [INSERTED] T
		WHERE T.GroupID > 0;


	UPDATE [bx_FriendGroups]
		SET
			TotalFriends = T.TotalFriends
		FROM @tempTable T
		WHERE
			T.UserID = bx_FriendGroups.UserID
			AND
			T.GroupID = bx_FriendGroups.GroupID;



	DECLARE @tempTable2 table(UserID int,TotalFriends int);

	INSERT INTO @tempTable2 
	SELECT DISTINCT UserID,
					(ISNULL((SELECT COUNT(*) FROM bx_Friends m WITH (NOLOCK) WHERE m.UserID = T.UserID AND m.GroupID >= 0), 0))
		FROM [INSERTED] T
		WHERE T.GroupID >= 0;


	UPDATE [bx_UserInfos]
		SET
			bx_UserInfos.TotalFriends = T.TotalFriends
		FROM @tempTable2 T
		WHERE
			T.UserID = bx_UserInfos.UserID;

			
	--发出重新填充UserInfo的XCMD命令(已经在bx_UserInfos触发器中统一做)
	--SELECT 'ResetUser' AS XCMD, UserID, TotalFriends FROM @tempTable2;

END