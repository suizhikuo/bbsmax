CREATE TRIGGER bx_DeletingFiles_AfterInsert  ON [bx_DeletingFiles]
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 300 'DeleteFile' AS XCMD, DeletingFileID, ServerFilePath FROM INSERTED;
END