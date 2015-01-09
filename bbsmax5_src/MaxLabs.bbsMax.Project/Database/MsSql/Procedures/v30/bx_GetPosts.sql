-- =============================================
-- Author:		sek
-- Create date: 2007/3/6
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bx_GetPosts]
	@ThreadID int
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_Posts] WITH(NOLOCK) WHERE ThreadID=@ThreadID AND [SortOrder]<4000000000000000 ORDER BY [SortOrder]
	RETURN


