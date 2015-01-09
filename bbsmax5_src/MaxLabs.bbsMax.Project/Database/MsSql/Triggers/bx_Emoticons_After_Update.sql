--计算分组总大小和分组表情数
CREATE TRIGGER [bx_Emoticons_After_Update] ON  [bx_Emoticons] 
FOR  UPDATE 
AS
BEGIN

	SET NOCOUNT ON;

	IF UPDATE(GroupID) BEGIN

		DECLARE @GroupTable table(GroupID  int);

		INSERT @GroupTable SELECT DISTINCT GroupID FROM INSERTED;
		INSERT @GroupTable SELECT DISTINCT GroupID FROM DELETED;

		DECLARE @temp Table(GroupID int, TotalSize int, TotalEmoticons int);
		
		INSERT @temp 
		SELECT DISTINCT GroupID
			,ISNULL((SELECT SUM(FileSize) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
			,ISNULL((SELECT COUNT(*) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
		FROM @GroupTable  t;

		UPDATE bx_EmoticonGroups SET TotalSizes = t.TotalSize, TotalEmoticons = t.TotalEmoticons FROM @temp t WHERE bx_EmoticonGroups.GroupID = t.GroupID;

	END

END