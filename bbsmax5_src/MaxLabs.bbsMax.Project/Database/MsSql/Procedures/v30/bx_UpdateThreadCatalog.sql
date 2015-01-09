-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/22>
-- Description:	<修改>
-- =============================================
CREATE PROCEDURE [bx_UpdateThreadCatalog] 
	@ThreadCatalogID int,
	@ThreadCatalogName nvarchar(64),
	@LogoUrl nvarchar(512)
AS
	SET NOCOUNT ON
	IF EXISTS (SELECT * FROM [bx_ThreadCatalogs] WITH (NOLOCK) WHERE ThreadCatalogName=@ThreadCatalogName AND ThreadCatalogID<>@ThreadCatalogID)
			RETURN (3) 
--	ELSE IF EXISTS (SELECT * FROM [bx_ThreadCatalogs] WITH (NOLOCK) WHERE LogoUrl=@LogoUrl AND ThreadCatalogID<>@ThreadCatalogID)
--			RETURN (6) 
	ELSE BEGIN
		UPDATE [bx_ThreadCatalogs] SET
		[ThreadCatalogName]=@ThreadCatalogName,
		[LogoUrl]=@LogoUrl 
		WHERE ThreadCatalogID=@ThreadCatalogID
		RETURN (0)
	END


