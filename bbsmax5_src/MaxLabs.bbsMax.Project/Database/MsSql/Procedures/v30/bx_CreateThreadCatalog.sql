-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/22>
-- Description:	<创建>
-- =============================================
CREATE PROCEDURE [bx_CreateThreadCatalog] 
	@ThreadCatalogName nvarchar(64),
	@LogoUrl nvarchar(512)
AS
	SET NOCOUNT ON 
	IF EXISTS (SELECT * FROM [bx_ThreadCatalogs] WITH (NOLOCK) WHERE ThreadCatalogName=@ThreadCatalogName)
			RETURN (3) 
--	ELSE IF EXISTS (SELECT * FROM [bx_ThreadCatalogs] WITH (NOLOCK) WHERE LogoUrl=@LogoUrl)
--			RETURN (6) 
	ELSE
		BEGIN
--		declare @SortOrder int,@Condition varchar(100)
--		SET @Condition='ForumID='+str(@ForumID)
--		EXECUTE MaxCommon_GetSortOrder 'bx_ThreadCatalogs', @Condition,@SortOrder output
		INSERT INTO [bx_ThreadCatalogs] (
		[ThreadCatalogName],
		[LogoUrl]
		) VALUES (
		@ThreadCatalogName,
		@LogoUrl
		)
		RETURN (0)
		END


