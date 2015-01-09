-- =============================================
-- Author:		<sek>
-- Create date: <2006/12/21>
-- Description:	<获取某个用户管理的版块>
-- =============================================
CREATE PROCEDURE bx_GetModeratorForums 
	@UserID int
AS
	SET NOCOUNT ON 
	SELECT * FROM [bx_Moderators] WITH(NOLOCK) WHERE UserID=@UserID AND ExpiresDate>GETDATE();
	RETURN


