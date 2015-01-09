-- =============================================
-- Author:		sek
-- Create date: 2007/3/2
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetPost]
	@PostID int
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Posts] WITH (NOLOCK) WHERE PostID=@PostID
	RETURN


