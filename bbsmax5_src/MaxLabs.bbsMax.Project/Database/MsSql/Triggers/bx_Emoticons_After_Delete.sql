CREATE TRIGGER [bx_Emoticons_After_Delete] ON  [bx_Emoticons] 
FOR DELETE
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @temp Table(GroupID int, TotalSize int, TotalEmoticons int);

	INSERT @temp 
		SELECT DISTINCT GroupID
		,ISNULL((SELECT SUM(FileSize) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
		,ISNULL((SELECT COUNT(*) FROM bx_Emoticons WITH (NOLOCK) WHERE bx_Emoticons.GroupID = t.GroupID), 0)
	FROM DELETED t;

	UPDATE bx_EmoticonGroups SET TotalSizes = t.TotalSize, TotalEmoticons = t.TotalEmoticons FROM @temp t WHERE bx_EmoticonGroups.GroupID = t.GroupID;

END