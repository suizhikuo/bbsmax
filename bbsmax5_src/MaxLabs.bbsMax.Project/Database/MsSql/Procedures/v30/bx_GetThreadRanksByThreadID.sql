-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/26>
-- Description:	<根据threadID获取信息>
-- =============================================
CREATE PROCEDURE [bx_GetThreadRanksByThreadID]
	@ThreadID int
AS
	SET NOCOUNT ON  
	SELECT * FROM [bx_ThreadRanks] WITH(NOLOCK) WHERE ThreadID=@ThreadID
	RETURN


