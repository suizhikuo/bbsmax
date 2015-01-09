-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/22>
-- Description:	<删除>
-- =============================================
CREATE PROCEDURE [bx_DeleteThreadCatalog]
	@ThreadCatalogID int
AS
BEGIN
	SET NOCOUNT ON 
	---Modify by 帅帅 2007-10-25---start--
	BEGIN TRANSACTION
	DELETE bx_ThreadCatalogs where ThreadCatalogID = @ThreadCatalogID;
	IF @@ERROR<>0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (1);
	END

	DELETE bx_ThreadCatalogsInForums WHERE ThreadCatalogID = @ThreadCatalogID
	IF @@ERROR<>0
	BEGIN
		ROLLBACK TRANSACTION
		RETURN (1);
	END
	
	COMMIT TRANSACTION
	RETURN 0
	---Modify by 帅帅 2007-10-25---end--
END


