--删除相片的触发器
EXEC bx_Drop 'bx_Photos_AfterDelete';

GO

CREATE TRIGGER [bx_Photos_AfterDelete] ON [bx_Photos] AFTER DELETE
AS
BEGIN

	SET NOCOUNT ON;
	
	DELETE bx_Denouncings WHERE Type=1 AND TargetID IN (SELECT PhotoID FROM [DELETED]);
	
	DELETE bx_Files
		WHERE FileID IN (SELECT FileID FROM [DELETED])
			AND FileID NOT IN (SELECT FileID FROM [bx_UsedFileIds] WHERE FileID IN (SELECT FileID FROM [DELETED]))
	
	--更新用户已用相册空间
	
	DECLARE @TempTable1 table (UserID int, UsedAlbumSize bigint, TotalPhotos int);
	
	INSERT INTO @TempTable1 (UserID, UsedAlbumSize, TotalPhotos) 
	SELECT 
		DISTINCT T.UserID, 
		ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[UserID] = T.[UserID]), 0),
		ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[UserID] = T.[UserID]), 0)
	FROM [DELETED] T;
	
	UPDATE [bx_UserVars] SET [bx_UserVars].[UsedAlbumSize] = T.[UsedAlbumSize]
	FROM @TempTable1 AS T 
	WHERE [bx_UserVars].[UserID] = T.[UserID];
	
	UPDATE [bx_Users] SET [bx_Users].[TotalPhotos] = T.[TotalPhotos]
	FROM @TempTable1 T
	WHERE T.[UserID] = [bx_Users].[UserID];

	SELECT 'ResetUser' AS XCMD, UserID, TotalPhotos, UsedAlbumSize FROM @TempTable1;

	--更新相册总图片数
	
	DECLARE @TempTable2 table(AlbumID int, TotalPhotos int, UpdateDate datetime DEFAULT(GETDATE()));
	
	INSERT INTO @TempTable2 (AlbumID, TotalPhotos)
	SELECT 
		DISTINCT [AlbumID], 
		ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.[AlbumID] = T.[AlbumID]), 0)
	FROM [DELETED] T;
	
	UPDATE [bx_Albums] SET [bx_Albums].[TotalPhotos] = T.[TotalPhotos], [bx_Albums].[UpdateDate] = T.UpdateDate
	FROM @TempTable2 T
	WHERE [bx_Albums].[AlbumID] = T.[AlbumID];

	--触发XCMD

	--SELECT 'ResetAlbum' AS XCMD, AlbumID, TotalPhotos, UpdateDate FROM @TempTable2;

END


