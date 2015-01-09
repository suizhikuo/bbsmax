CREATE TRIGGER [bx_Denouncings_AfterDelete]
	ON [bx_Denouncings]
	AFTER DELETE
AS
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT 'ResetDenouncingCount' AS XCMD;
END
