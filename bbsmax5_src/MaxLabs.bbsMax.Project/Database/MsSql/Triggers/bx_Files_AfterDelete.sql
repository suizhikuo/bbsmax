CREATE TRIGGER bx_Files_AfterDelete  ON [bx_Files]
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO bx_DeletingFiles (ServerFilePath) SELECT ServerFilePath FROM DELETED;
END