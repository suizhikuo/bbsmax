-- =============================================
-- Author:		<Author,SEK>
-- Create date: <Create 07/05/18,>
-- Description:	<Description,,>
-- =============================================

Create PROCEDURE [bx_GetThreadCatalogForumIDs]
	@ThreadCatalogID int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT * FROM [bx_ThreadCatalogsInForums] WHERE ThreadCatalogID=@ThreadCatalogID
END

