--创建新相片的触发器
EXEC bx_Drop 'bx_Albums_AfterUpdate';

GO

CREATE TRIGGER [bx_Albums_AfterUpdate]
	ON [bx_Albums]
	AFTER UPDATE
AS
BEGIN

	SET NOCOUNT ON;
	SELECT 'ResetAlbum' AS XCMD, [INSERTED].* FROM [INSERTED];

END

GO