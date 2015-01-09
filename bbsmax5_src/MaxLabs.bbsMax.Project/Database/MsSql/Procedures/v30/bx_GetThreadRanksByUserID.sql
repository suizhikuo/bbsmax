-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/26>
-- Description:	<根据threadID获取信息>
-- =============================================
CREATE PROCEDURE [bx_GetThreadRanksByUserID]
	@UserID int
AS
	SET NOCOUNT ON
	SELECT * FROM [bx_ThreadRanks] WITH(NOLOCK) WHERE UserID=@UserID 
	RETURN


