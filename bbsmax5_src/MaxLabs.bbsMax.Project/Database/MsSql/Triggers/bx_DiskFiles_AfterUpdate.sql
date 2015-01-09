CREATE TRIGGER bx_DiskFiles_AfterUpdate ON [bx_DiskFiles] 
FOR  UPDATE
AS
BEGIN

	SET NOCOUNT ON;

	IF (UPDATE(DirectoryID)) BEGIN
	
		DECLARE @DirectoryIDTable table(DirectoryID int);
		
		INSERT @DirectoryIDTable SELECT DISTINCT DirectoryID FROM INSERTED;
		INSERT @DirectoryIDTable SELECT DISTINCT DirectoryID FROM DELETED;

		DECLARE @tempTable table(DirectoryID int, TotalSize bigint, TotalFiles int);
	
		INSERT INTO @tempTable 
			SELECT DISTINCT DirectoryID,
				(ISNULL((SELECT SUM(FileSize) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
				,(ISNULL((SELECT COUNT(*) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
			FROM @DirectoryIDTable T;
	
		UPDATE [bx_DiskDirectories] SET bx_DiskDirectories.TotalSize = t.TotalSize , TotalFiles = t.TotalFiles FROM @tempTable t WHERE [bx_DiskDirectories].DirectoryID = t.DirectoryID;
	END
END