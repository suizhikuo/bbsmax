CREATE TRIGGER bx_DiskFiles_AfterInsert  ON [bx_DiskFiles] 
FOR INSERT
AS
BEGIN
	SET NOCOUNT ON;

-------------------

	DECLARE @tempTable table(DirectoryID int ,TotalSize bigint, TotalFiles int);

	INSERT INTO @tempTable 
		SELECT DISTINCT DirectoryID,
			(ISNULL((SELECT SUM(FileSize) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
			,(ISNULL((SELECT COUNT(*) FROM [bx_DiskFiles] f WITH (NOLOCK) WHERE f.DirectoryID = T.DirectoryID), 0))
		FROM [INSERTED] T;

	UPDATE [bx_DiskDirectories] SET bx_DiskDirectories.TotalSize = t.TotalSize , TotalFiles = t.TotalFiles FROM @tempTable t WHERE [bx_DiskDirectories].DirectoryID = t.DirectoryID;

-------------------

	DECLARE @tempTable2 table(UserID int, TotalSize bigint, TotalDiskFiles int);

	INSERT INTO @tempTable2
		SELECT DISTINCT UserID,
			(ISNULL((SELECT SUM(TotalSize) FROM [bx_DiskDirectories] d WITH (NOLOCK) WHERE d.UserID = T.UserID), 0)),
			(ISNULL((SELECT SUM(TotalFiles) FROM [bx_DiskDirectories] d WITH (NOLOCK) WHERE d.UserID = T.UserID), 0))
		FROM [INSERTED] T;

	UPDATE [bx_UserVars]
		SET
			bx_UserVars.UsedDiskSpaceSize = t.TotalSize,
			bx_UserVars.TotalDiskFiles = t.TotalDiskFiles
		FROM @tempTable2 t
		WHERE
			t.UserID = bx_UserVars.UserID;

-------------------

	SELECT 'ResetAuthUser' AS XCMD, UserID,  TotalSize AS UsedDiskSpaceSize, TotalDiskFiles FROM @tempTable2;

END