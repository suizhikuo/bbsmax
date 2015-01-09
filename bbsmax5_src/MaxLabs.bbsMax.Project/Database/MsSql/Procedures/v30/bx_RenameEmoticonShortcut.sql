CREATE PROCEDURE [bx_RenameEmoticonShortcut]
	@UserID INT,
	@GroupID INT,
	@EmoticonIDs  VARCHAR(8000),
	@NewShortcuts NVARCHAR(4000)--注意传进来的快捷方式不能相同
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @EmotionIDAndShortcutsTable TABLE
	(
		T_Index INT IDENTITY(1,1),
		T_EmoticonID INT DEFAULT 0,
		T_Shortcut NVARCHAR(64)  COLLATE Chinese_PRC_CI_AS_WS DEFAULT ''
	)	
	INSERT INTO @EmotionIDAndShortcutsTable(T_EmoticonID)
	SELECT item FROM bx_GetIntTable(@EmoticonIDs,N'|') 
	
	UPDATE @EmotionIDAndShortcutsTable
	SET T_Shortcut=item
	FROM bx_GetStringTable_nvarchar(@NewShortcuts,N'|')
	WHERE T_Index=id
	

	IF EXISTS(
	SELECT T_Shortcut FROM [bx_Emoticons] 
	INNER JOIN @EmotionIDAndShortcutsTable 
	ON Shortcut=T_Shortcut AND Shortcut<>''
	WHERE  EmoticonID NOT IN(SELECT T_EmoticonID FROM @EmotionIDAndShortcutsTable))
		RETURN 3

	BEGIN TRANSACTION

	UPDATE [bx_Emoticons]
	SET Shortcut= RAND()+T_EmoticonID
	FROM @EmotionIDAndShortcutsTable AS T
	WHERE EmoticonID=T_EmoticonID
	IF @@ERROR<>0
		ROLLBACK TRANSACTION
	
	UPDATE [bx_Emoticons]
	SET Shortcut=T_Shortcut
	FROM @EmotionIDAndShortcutsTable AS T
	WHERE EmoticonID=T_EmoticonID
	IF @@ERROR<>0
		ROLLBACK TRANSACTION

	COMMIT TRANSACTION
		RETURN 0
END