-- =============================================
-- Author:		<sek>
-- Create date: <2006/1/29>
-- Description:	<>
-- =============================================
CREATE PROCEDURE [bx_GetAllThreadCatalogs]

AS
	SET NOCOUNT ON
	SELECT * FROM bx_ThreadCatalogs WHERE ThreadCatalogID<>0
	RETURN


