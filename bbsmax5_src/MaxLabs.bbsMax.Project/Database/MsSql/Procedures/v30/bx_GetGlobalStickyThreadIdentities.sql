-- =============================================
-- Author:		zzbird
-- Create date: <2007/3/18>
-- Description:	<获取总置顶主题ID>
-- =============================================
CREATE PROCEDURE [bx_GetGlobalStickyThreadIdentities]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [ThreadID] FROM [bx_Threads] WITH (NOLOCK) WHERE SortOrder>=3000000000000000 AND SortOrder<4000000000000000 ORDER BY SortOrder DESC;
END


