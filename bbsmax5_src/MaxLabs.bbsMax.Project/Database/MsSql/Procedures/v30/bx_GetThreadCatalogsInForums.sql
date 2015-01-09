-- =============================================
-- Author:		<sek>
-- Create date: <2007/1/29>
-- Description:	<>
-- =============================================
CREATE PROCEDURE [bx_GetThreadCatalogsInForums]
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_ThreadCatalogsInForums] ORDER BY SortOrder ASC
	RETURN


