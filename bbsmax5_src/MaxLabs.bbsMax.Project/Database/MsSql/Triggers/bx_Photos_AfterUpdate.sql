--创建新相片的触发器
EXEC bx_Drop 'bx_Photos_AfterUpdate';

GO

CREATE TRIGGER [bx_Photos_AfterUpdate]
	ON [bx_Photos]
	AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	--如果UserID字段被更改，需要重新统计受影响的用户的TotalPhotos值
	IF UPDATE([UserID]) BEGIN
		DECLARE @tempTable2 table(UserID int, UsedAlbumSize bigint, TotalPhotos int);
		INSERT INTO @tempTable2 (UserID, UsedAlbumSize, TotalPhotos)
			SELECT DISTINCT UserID,
				ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.UserID = T.[UserID]), 0),
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] P WITH (NOLOCK) WHERE P.UserID = T.UserID), 0)
			FROM [INSERTED] T;
		INSERT INTO @tempTable2 (UserID, UsedAlbumSize, TotalPhotos)
			SELECT DISTINCT UserID,
				ISNULL((SELECT SUM(FileSize) FROM [bx_Photos] p WITH (NOLOCK) WHERE p.[UserID] = T.[UserID]), 0),
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] p WITH (NOLOCK) WHERE p.UserID = T.UserID), 0)
			FROM [DELETED] T
			WHERE UserID NOT IN (SELECT UserID FROM @tempTable2);
		
		UPDATE [bx_Users]
			SET
				bx_Users.TotalPhotos = T.TotalPhotos
			FROM @tempTable2 T
			WHERE
				T.UserID = bx_Users.UserID;
		UPDATE [bx_UserVars]
			SET
				bx_UserVars.UsedAlbumSize = T.UsedAlbumSize
			FROM @tempTable2 T
			WHERE
				T.UserID = bx_UserVars.UserID;
		--发出重新填充UserInfo的XCMD命令
		SELECT 'ResetUser' AS XCMD, UserID, TotalPhotos, UsedAlbumSize FROM @tempTable2;

	END


	--如果AlbumID字段被更改，需要重新统计受影响的相册的TotalPhotos值
	IF UPDATE([AlbumID]) BEGIN
		DECLARE @tempTable table(AlbumID int, TotalPhotos int, UpdateDate datetime DEFAULT(GETDATE()));

		INSERT INTO @tempTable (AlbumID, TotalPhotos)
			SELECT DISTINCT AlbumID,
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] as m WITH (NOLOCK) WHERE m.AlbumID = T.AlbumID), 0)
			FROM [INSERTED] T;

		INSERT INTO @tempTable (AlbumID, TotalPhotos)
			SELECT DISTINCT AlbumID,
				ISNULL((SELECT COUNT(*) FROM [bx_Photos] as m WITH (NOLOCK) WHERE m.AlbumID = T.AlbumID), 0)
			FROM [DELETED] T
			WHERE AlbumID NOT IN (SELECT AlbumID FROM @tempTable);
		
		UPDATE [bx_Albums]
			SET
				[bx_Albums].TotalPhotos = T.TotalPhotos,
				[bx_Albums].[UpdateDate] = T.UpdateDate
			FROM @tempTable T
			WHERE
				T.AlbumID = [bx_Albums].AlbumID;
		--SELECT 'ResetAlbum' AS XCMD, AlbumID, TotalPhotos, UpdateDate FROM @TempTable2;
	END
END

GO