-- =============================================
-- Author:		<sek>
-- Create date: <2006/1/29>
-- Description:	<>
-- =============================================
CREATE PROCEDURE [bx_RemoveThreadCatalogFromForum]
	@ForumID Int,
    @ThreadCatalogID Int
AS
	SET NOCOUNT ON
	DELETE [bx_ThreadCatalogsInForums] WHERE ForumID=@ForumID AND ThreadCatalogID=@ThreadCatalogID
	IF @@ROWCOUNT > 0
			RETURN (0);
		ELSE
			RETURN (1);


