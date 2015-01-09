-- =============================================
-- Author:		<SEK>
-- Create date: <Create Date,,>
-- Description:	<获取主题的所有回复用户包括主题用户>
-- =============================================
CREATE PROCEDURE [bx_GetPostUserIDsFormThreads]
	@ThreadIDs varchar(8000)
AS
BEGIN
	SET NOCOUNT ON;

	EXEC('SELECT DISTINCT UserID FROM bx_Posts WHERE SortOrder<4000000000000000 AND ThreadID in('+@ThreadIDs+')');
END


