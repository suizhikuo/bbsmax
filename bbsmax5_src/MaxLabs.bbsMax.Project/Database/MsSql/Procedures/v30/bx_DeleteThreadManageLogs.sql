-- =============================================
-- Author:		SEK
-- Create date: <2006/12/28>
-- Description:	<批量删除日志>
-- =============================================
CREATE PROCEDURE [bx_DeleteThreadManageLogs]
	@LogIdentities varchar(8000)
AS
	SET NOCOUNT ON 
	EXEC ('DELETE [bx_ThreadManageLogs] WHERE [LogID] IN (' + @LogIdentities + ') ') 
	IF @@ROWCOUNT > 0
		RETURN (0)
	ELSE
		RETURN (1)


