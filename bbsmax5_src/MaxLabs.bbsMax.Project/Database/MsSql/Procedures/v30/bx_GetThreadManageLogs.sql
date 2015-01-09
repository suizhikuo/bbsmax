---- By 帅帅 2007/9/14----
CREATE PROCEDURE [bx_GetThreadManageLogs]
	@ThreadID INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM bx_ThreadManageLogs WHERE ThreadID=@ThreadID
END